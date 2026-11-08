using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace Afterglow.M01
{
    [Serializable] public sealed class M01Line { public string id, speaker, text; }
    [Serializable] public sealed class M01Script { public M01Line[] lines; }

    public sealed class M01Dialogue : MonoBehaviour
    {
        public TextAsset script;
        public TMP_Text body, hint;
        public TMP_Text speakerLabel;
        public TMP_Text[] choiceLabels;
        public UnityEngine.UI.Image[] choiceBackgrounds;
        int page = 1;
        int PageEnd => body.textInfo.pageCount > 0 ? body.textInfo.pageInfo[Mathf.Min(page-1,body.textInfo.pageCount-1)].lastCharacterIndex + 1 : body.textInfo.characterCount;
        public CanvasGroup panel;
        public float charactersPerSecond = 40;
        bool conversationOpen,externalModal;
        int releasedFrame=-1;
        float modalTimeScale;
        public bool IsOpen => conversationOpen||externalModal||Time.frameCount<=releasedFrame;
        public bool AcquireModal()
        {
            if(IsOpen)return false;externalModal=true;modalTimeScale=Time.timeScale;Time.timeScale=0;return true;
        }
        public void ReleaseModal(){if(!externalModal)return;externalModal=false;Time.timeScale=modalTimeScale;requested=false;releasedFrame=Time.frameCount+1;}
        public string CurrentId { get; private set; }
        public int VisibleCharacters => body.maxVisibleCharacters;
        public int LineIndex { get; private set; }
        InputAction advance;
        InputAction previousChoice, nextChoice;
        string[] choices;
        int choiceIndex;
        bool requested;
        M01Script catalog;
        string[] ids;
        float visible, previousTimeScale;
        Action finished;
        public string LookupText(string id) => Array.Find(catalog.lines, line => line.id == id).text;
        public void BeginChoice(string text, string[] options, Action<int> onChosen, string title = "Teléfono de minas")
        {
            if (IsOpen) return;
            BeginText(title, text, () => onChosen(choiceIndex));
            choices = options; choiceIndex = 0; UpdateChoiceHint();
        }
        void UpdateChoiceHint()
        {
            hint.text = "[ E ]  Confirmar     ↑ / ↓  Elegir";
            UpdateChoicePresentation();
        }
        void UpdateChoicePresentation()
        {
            bool hasChoices=choices!=null;
            // Con más opciones que botones, la lista se desplaza alrededor de la elegida (▲/▼ indican que hay más).
            int slots=choiceLabels!=null?choiceLabels.Length:0;
            int first=hasChoices&&choices.Length>slots?Mathf.Clamp(choiceIndex-slots/2,0,choices.Length-slots):0;
            if(choiceLabels!=null)for(int i=0;i<choiceLabels.Length;i++){
                int option=first+i;
                bool active=hasChoices&&option<choices.Length;choiceLabels[i].transform.parent.gameObject.SetActive(active);
                if(active){
                    string mark=i==0&&first>0?"▲ ":i==slots-1&&option<choices.Length-1?"▼ ":"";
                    choiceLabels[i].text=mark+choices[option];choiceBackgrounds[i].color=option==choiceIndex?new Color(1,.79f,.32f):new Color(.89f,.88f,.81f);
                }
            }
            body.rectTransform.offsetMax=new Vector2(hasChoices?-350:-34,-58);
            body.ForceMeshUpdate();
        }
        public void BeginResolved(string id, int missing)
        {
            BeginText("Altar del Tío", LookupText(id).Replace("{cantidad}", missing.ToString()));
        }
        public void BeginText(string speaker, string text, Action onFinished = null)
        {
            if (IsOpen) return;
            var lines = new System.Collections.Generic.List<M01Line>(catalog.lines);
            string id = "UI." + Guid.NewGuid().ToString("N");
            lines.Add(new M01Line { id = id, speaker = speaker, text = text });
            catalog.lines = lines.ToArray(); Begin(new[] { id }, onFinished);
        }

        void Awake()
        {
            catalog = JsonUtility.FromJson<M01Script>(script.text);
            panel.alpha = 0;
            advance = new InputAction("Advance dialogue", InputActionType.Button, "<Keyboard>/e");
            advance.performed += _ => requested = true;
            advance.Enable();
            previousChoice = new InputAction("Previous option", InputActionType.Button, "<Keyboard>/upArrow");
            nextChoice = new InputAction("Next option", InputActionType.Button, "<Keyboard>/downArrow");
            previousChoice.performed += _ => { if (IsOpen && choices != null) { choiceIndex = (choiceIndex + choices.Length - 1) % choices.Length; UpdateChoiceHint(); } };
            nextChoice.performed += _ => { if (IsOpen && choices != null) { choiceIndex = (choiceIndex + 1) % choices.Length; UpdateChoiceHint(); } };
            previousChoice.Enable(); nextChoice.Enable();
        }
        public void Begin(string[] lineIds, Action onFinished = null)
        {
            if (IsOpen || lineIds == null || lineIds.Length == 0) return;
            foreach (var id in lineIds)
                if (Array.Find(catalog.lines, line => line.id == id) == null)
                    throw new ArgumentException("Unknown dialogue ID: " + id);
            ids = lineIds; finished = onFinished; LineIndex = 0; choices = null;
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0; conversationOpen = true; requested = false;
            panel.transform.SetAsLastSibling();
            panel.alpha = 1;
            ShowLine();
        }
        void ShowLine()
        {
            var line = Array.Find(catalog.lines, candidate => candidate.id == ids[LineIndex]);
            CurrentId = line.id;
            if(speakerLabel)speakerLabel.text=line.speaker;
            body.text = line.text;
            body.overflowMode=TextOverflowModes.Page;page=1;body.pageToDisplay=page;
            UpdateChoicePresentation();
            body.ForceMeshUpdate();
            body.maxVisibleCharacters = 0; visible = 0;
            hint.text = "[ E ]  Completar";
        }
        void Update()
        {
            if (!conversationOpen) { requested = false; return; }
            visible += charactersPerSecond * Time.unscaledDeltaTime;
            body.maxVisibleCharacters = Mathf.Min((int)visible, PageEnd);
            if(choices==null)hint.text=body.maxVisibleCharacters<PageEnd?"[ E ]  Completar":"[ E ]  Siguiente";
            if (!requested) return;
            requested = false;
            if (body.maxVisibleCharacters < PageEnd)
            {
                visible = PageEnd;
                body.maxVisibleCharacters = PageEnd;
            }
            else if(page<body.textInfo.pageCount){page++;body.pageToDisplay=page;visible=body.textInfo.pageInfo[page-1].firstCharacterIndex;body.maxVisibleCharacters=(int)visible;}
            else if (++LineIndex < ids.Length) ShowLine();
            else
            {
                conversationOpen = false; panel.alpha = 0; Time.timeScale = previousTimeScale;
                var callback = finished; finished = null; callback?.Invoke();
            }
        }
        void OnDestroy()
        {
            advance?.Dispose();
            previousChoice?.Dispose(); nextChoice?.Dispose();
            if (conversationOpen) Time.timeScale = previousTimeScale;
            ReleaseModal();
        }
    }
}
