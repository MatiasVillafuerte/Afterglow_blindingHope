using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Afterglow.M01
{
    /// <summary>
    /// Cierre de Mundo 1 (GDD 10): pieza dorada a examinar (objeto clave, una vez), llegada del capitán, Platicio se une al
    /// equipo, vagoneta escondida, viaje y pantalla «Continuará». En esa pantalla, E pide volver al menú principal con
    /// Guardar y volver / Volver sin guardar / Cancelar; no guarda solo. Cargar un cierre ya viajado muestra Continuará.
    /// </summary>
    public sealed class M01Ending : MonoBehaviour
    {
        public M01Dialogue dialogue; public GameObject piece, captain, cart, cartMiner; public Transform cartPath;
        public Sprite sonSprite;
        InputAction interact; bool requested; string step = "Inactivo"; AfterglowPlayer player; GameObject continua;
        M01Progress Data => M01Session.Instance.data;
        public string Step => step;

        void OnEnable() { interact = new InputAction("Ending", InputActionType.Button, "<Keyboard>/e"); interact.performed += _ => { if (dialogue && !dialogue.IsOpen) requested = true; }; interact.Enable(); }
        void OnDisable() { interact?.Dispose(); }
        void Awake() { if (piece) piece.SetActive(false); if (captain) captain.SetActive(false); if (cart) cart.SetActive(false); if (cartMiner) cartMiner.SetActive(false); }

        public void Begin(Vector3 piecePosition)
        {
            player = FindAnyObjectByType<AfterglowPlayer>();
            piece.transform.position = new Vector3(piecePosition.x, piece.transform.position.y, piecePosition.z); piece.SetActive(true);
            step = "Pieza"; dialogue.Begin(new[] { "FIN.02" });
        }

        /// <summary>Al cargar una partida posterior a la victoria.</summary>
        public void Resume()
        {
            player = FindAnyObjectByType<AfterglowPlayer>();
            if (Data.events.Contains("FIN.VIAJE")) { ShowContinua(); step = "Continúa"; return; }
            if (Data.events.Contains("FIN.CONVERSACION")) { captain.SetActive(true); cart.SetActive(true); cartMiner.SetActive(true); step = "Vagoneta"; return; }
            piece.SetActive(true); step = "Pieza";
        }

        void Update()
        {
            if (piece && piece.activeSelf) piece.transform.Rotate(0, 60 * Time.deltaTime, 0);
            if (!requested) return; requested = false;
            if (!player) player = FindAnyObjectByType<AfterglowPlayer>();
            if (step == "Continúa") { AskMenu(); return; }
            if (!player || player.Dead || dialogue.IsOpen) return;
            if (step == "Pieza" && Near(piece, 2.2f))
            {
                Data.GrantKeyItem("PIEZA");
                StartCoroutine(Conversation());
            }
            else if (step == "Vagoneta" && Near(cart, 2.6f))
                dialogue.BeginChoice(dialogue.LookupText("FIN.18"), new[] { "Viajar", "Prepararme primero" }, choice => { if (choice == 0) StartCoroutine(Travel()); }, "Vagoneta");
        }

        bool Near(GameObject target, float r) => target && target.activeSelf && Vector3.Distance(player.transform.position, target.transform.position) < r;

        IEnumerator Talk(params string[] lines) { bool done = false; dialogue.Begin(lines, () => done = true); while (!done) yield return null; }

        IEnumerator Conversation()
        {
            step = "Conversación";
            while (M01KeyItemCelebration.Playing || dialogue.IsOpen) yield return null;
            yield return Talk("FIN.04");
            piece.SetActive(false); captain.SetActive(true);
            yield return Talk("FIN.06", "FIN.08", "FIN.09", "FIN.10", "FIN.11", "FIN.13", "FIN.14");
            Data.UnlockSon(); if (!Data.events.Contains("FIN.CONVERSACION")) Data.events.Add("FIN.CONVERSACION");
            yield return Talk("FIN.15");
            cart.SetActive(true); cartMiner.SetActive(true); step = "Vagoneta";
        }

        IEnumerator Travel()
        {
            step = "Viaje"; var rules = player.GetComponent<M01PlayerRules>(); rules.scriptedControl = true;
            Data.worldClosed = true; if (!Data.events.Contains("FIN.VIAJE")) Data.events.Add("FIN.VIAJE");
            var cc = player.GetComponent<CharacterController>(); cc.enabled = false;
            var son = new GameObject("Platicio en la vagoneta").AddComponent<SpriteRenderer>(); son.sprite = sonSprite;
            Vector3 a = cart.transform.position, b = cartPath ? cartPath.position : a + Vector3.right * 14;
            for (float t = 0; t < 4.5f; t += Time.deltaTime)
            {
                var p = Vector3.Lerp(a, b, Mathf.SmoothStep(0, 1, t / 4.5f));
                cart.transform.position = p; player.transform.position = p + new Vector3(-.35f, .55f, 0); son.transform.position = p + new Vector3(.45f, .55f, 0);
                yield return null;
            }
            ShowContinua(); step = "Continúa";
        }

        void ShowContinua()
        {
            if (continua) return;
            var rules = player ? player.GetComponent<M01PlayerRules>() : null; if (rules) rules.scriptedControl = true;
            continua = new GameObject("Continuará", typeof(Canvas), typeof(CanvasScaler));
            var canvas = continua.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 500;
            var scaler = continua.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920, 1080);
            var bg = new GameObject("Fondo", typeof(RectTransform), typeof(Image)); bg.transform.SetParent(continua.transform, false);
            var r = (RectTransform)bg.transform; r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.sizeDelta = Vector2.zero; bg.GetComponent<Image>().color = new Color(.06f, .04f, .03f, .96f);
            Text("Continuará", 120, new Vector2(0, 40), new Color(1f, .82f, .45f));
            Text("[E] Volver al menú principal", 38, new Vector2(0, -120), new Color(.95f, .9f, .82f));
        }

        void Text(string s, float size, Vector2 pos, Color color)
        {
            var t = new GameObject(s, typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>(); t.transform.SetParent(continua.transform, false);
            ((RectTransform)t.transform).anchoredPosition = pos; ((RectTransform)t.transform).sizeDelta = new Vector2(1600, 200);
            t.text = s; t.fontSize = size; t.color = color; t.alignment = TextAlignmentOptions.Center; t.fontStyle = FontStyles.Bold;
        }

        void AskMenu()
        {
            if (dialogue.IsOpen) return;
            continua.GetComponent<Canvas>().sortingOrder = 100; // el diálogo de confirmación queda encima
            dialogue.BeginChoice("¿Volver al menú principal?", new[] { "Guardar y volver", "Volver sin guardar", "Cancelar" }, choice =>
            {
                continua.GetComponent<Canvas>().sortingOrder = 500;
                if (choice == 2) return;
                if (choice == 0)
                {
                    var save = new M01SaveFile { scene = SceneManager.GetActiveScene().path, position = player.transform.position, facing = Vector3.right, progress = Data };
                    if (!M01Save.WriteFile(save, M01Save.Path, out var error)) { Debug.LogWarning("M01 save: " + error); dialogue.Begin(new[] { "SYS.02" }); return; }
                }
                M01PlayerRules.ResetTemporary();
                SceneManager.LoadScene(M01Expedition.ScenePath("M01_MENU"));
            }, "Continuará");
        }
    }
}
