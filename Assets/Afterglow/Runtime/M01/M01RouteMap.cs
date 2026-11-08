using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace Afterglow.M01
{
    public sealed class M01RouteMap:MonoBehaviour
    {
        public static readonly string[] Names={"Entrada de la mina","Rieles cruzados","Galerías de las señales","Galería sin luz","La vía suspendida","Cámara del derrumbe"};
        public M01Dialogue dialogue;
        public GameObject view,miniature;
        public TMP_Text title,details,instructions;
        public RectTransform marker;
        public RectTransform[] nodes;
        public bool standalone;
        public UnityEngine.UI.Image[] badges;
        public TMP_Text[] nodeLabels;
        public bool IsOpen{get;private set;}
        public int Focused{get;private set;}
        InputAction interact,cancel,up,down,left,right;
        bool confirm,back;Vector2 direction;int openingFrame;
        M01Progress Data=>M01Session.Instance.data;
        public static bool Available(M01Progress data,int index)=>!data.worldClosed&&(index==0?data.events.Contains("P02.CAPTAIN")&&data.events.Contains("M01.WORLD_STARTED"):data.opened[index-1]);
        public static string Id(int index)=>"M01N"+(index+1).ToString("00");
        /// <summary>Escena de entrada de cada nivel: N1 entra al refugio con altar; los demás a su primera sección.</summary>
        public static string EntryScene(int index)=>"N"+(index+1).ToString("00")+"S01";
        public static bool HasScene(int index)=>UnityEngine.SceneManagement.SceneUtility.GetBuildIndexByScenePath(M01Expedition.ScenePath(EntryScene(index)))>=0;
        M01MapPresentation presentation;
        M01MapPresentation Presentation{get{if(!presentation){presentation=GetComponent<M01MapPresentation>()??gameObject.AddComponent<M01MapPresentation>();presentation.Build();}return presentation;}}
        bool CanReturn=>!standalone||Data.events.Contains("M01.N01_ENTERED");
        public static string Reason(M01Progress data,int index)
        {
            if(data.worldClosed)return "Mundo concluido. Continúa el viaje de cierre.";
            if(index==0)return Available(data,index)?"Disponible":"Completa el encargo del capitán.";
            int p=index-1;
            if(data.opened[p])return "Disponible";
            string a=data.investigated[p]?"Investigación completa":"Falta investigar N"+index;
            string b="Aporte N"+index+": "+data.contributions[p]+" / "+M01Progress.Quotas[p];
            return a+" · "+b+(data.investigated[p]&&data.contributions[p]>=M01Progress.Quotas[p]?" · Espera el anuncio del minero.":"");
        }
        InputAction Action(string name,string key,System.Action callback){var a=new InputAction(name,InputActionType.Button,"<Keyboard>/"+key);a.performed+=_=>callback();a.Enable();return a;}
        void OnEnable()
        {
            interact=Action("Map confirm","e",()=>confirm=true);cancel=Action("Map close","escape",()=>back=true);
            up=Action("Map north","upArrow",()=>direction=Vector2.up);down=Action("Map south","downArrow",()=>direction=Vector2.down);left=Action("Map west","leftArrow",()=>direction=Vector2.left);right=Action("Map east","rightArrow",()=>direction=Vector2.right);
        }
        IEnumerator Start()
        {
            view.SetActive(false);
            miniature.SetActive(false);
            if(!standalone)yield break;
            yield return null;
            while(M01Expedition.Transitioning)yield return null;
            Open();
        }
        public bool Open()
        {
            if(IsOpen||M01Expedition.Transitioning||Data.worldClosed||Data.escortActive||!Available(Data,0)||!dialogue.AcquireModal())return false;
            IsOpen=true;Focused=Mathf.Clamp(Data.lastMapNode-1,0,5);openingFrame=Time.frameCount;
            view.SetActive(false);miniature.SetActive(false);Presentation.Show(Data,Focused,CanReturn);Refresh();return true;
        }
        public void Close()
        {
            if(!IsOpen)return;
            IsOpen=false;
            // Scene unloading can destroy the UI before this component is disabled.
            if(view)view.SetActive(false);
            if(miniature)miniature.SetActive(false);
            if(presentation)presentation.Hide();
            if(dialogue)dialogue.ReleaseModal();
            confirm=back=false;direction=Vector2.zero;
        }
        void Refresh(string message=null)
        {
            Presentation.Refresh(Data,Focused,CanReturn,message);
            for(int i=0;i<6;i++)
            {
                bool available=Available(Data,i);bool investigated=i<5&&Data.investigated[i];
                badges[i].color=i==Focused?new Color(1,.76f,.27f):available?new Color(.28f,.65f,.48f):new Color(.33f,.35f,.39f);
                nodeLabels[i].text="N"+(i+1)+"\n"+(investigated?"Investigado":available?"Disponible":"Cerrado");
            }
            marker.anchoredPosition=nodes[Focused].anchoredPosition+new Vector2(0,42);
            title.text=Id(Focused)+" · "+Names[Focused];
            details.text=Reason(Data,Focused)+"\n"+(Focused<5?"Investigación propia: "+(Data.investigated[Focused]?"completa":"pendiente")+"\nCrédito propio: "+Data.contributions[Focused]+" / "+M01Progress.Quotas[Focused]:"Este nivel no tiene una cuota posterior.");
            if(Focused>0)details.text+="\nEscena de este nivel pendiente de producción.";
            instructions.text="Flechas · Elegir ruta     [ E ] Entrar";
            if(!standalone||Data.events.Contains("M01.N01_ENTERED"))instructions.text+="     [ Esc ] Volver";
        }
        void Move(Vector2 dir)
        {
            int best=-1;float score=float.PositiveInfinity;
            // Navigation follows the actual connected path, including locked neighbours.
            foreach(int i in new[]{Focused-1,Focused+1})if(i>=0&&i<6){var delta=Presentation.NodePosition(i)-Presentation.NodePosition(Focused);float forward=Vector2.Dot(delta.normalized,dir);if(forward>.2f){float cost=delta.magnitude/(forward*forward);if(cost<score){best=i;score=cost;}}}
            if(best>=0){Focused=best;Data.lastMapNode=best+1;Refresh();}
        }
        void Update()
        {
            bool pressed=confirm,escape=back;var dir=direction;confirm=back=false;direction=Vector2.zero;
            if(IsOpen)
            {
                if(Data.worldClosed){Close();return;}
                if(Time.frameCount<=openingFrame)return;
                if(escape)
                {
                    if(!standalone)Close();
                    else if(Data.events.Contains("M01.N01_ENTERED"))
                    {
                        Close();
                        M01Expedition.Travel("N01S01",M01Expedition.MineEntrance,fade:true);
                    }
                    return;
                }
                if(dir!=Vector2.zero)Move(dir);
                if(pressed)
                {
                    if(!Available(Data,Focused)){Refresh();return;}
                    if(!HasScene(Focused)){Refresh("<color=#8A5A1A>La entrada de este nivel todavía está en producción.</color>");return;}
                    if(!Data.events.Contains("M01.N01_ENTERED"))Data.events.Add("M01.N01_ENTERED");
                    string entry=EntryScene(Focused);
                    Close();M01Expedition.Travel(entry,M01Expedition.SectionEntrance(entry),leave:standalone,enter:true,fade:true);
                }
                return;
            }
            if(!pressed||dialogue.IsOpen||M01Expedition.Transitioning||Time.timeScale==0)return;
            var p=FindAnyObjectByType<AfterglowPlayer>();var rules=p?p.GetComponent<M01PlayerRules>():null;
            if(p&&!p.Dead&&rules&&!rules.BlocksControl&&!rules.restrictedActivity&&Vector3.Distance(p.transform.position,transform.position)<2.4f)Open();
        }
        void OnDisable(){Close();interact?.Dispose();cancel?.Dispose();up?.Dispose();down?.Dispose();left?.Dispose();right?.Dispose();}
    }
}
