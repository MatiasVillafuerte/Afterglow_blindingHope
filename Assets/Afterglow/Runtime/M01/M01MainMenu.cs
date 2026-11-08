using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Afterglow.M01
{
    public sealed class M01MainMenu:MonoBehaviour
    {
        public M01Dialogue dialogue;
        IEnumerator Start()
        {
            yield return null;
            var rect=(RectTransform)dialogue.panel.transform;rect.anchoredPosition=new Vector2(0,60);rect.sizeDelta=new Vector2(1180,600);
            foreach(Transform child in rect.parent)if(child!=rect)child.gameObject.SetActive(false);
            Show();
        }
        void Show()
        {
            bool valid=M01Save.ReadFile(M01Save.Path,out _,out string error);
            dialogue.BeginChoice(valid?"Continúa tu partida o empieza una nueva.":"Empieza una nueva partida. "+error,valid?new[]{"Continuar","Nueva partida","Salir"}:new[]{"Nueva partida","Salir"},choice=>{
                if(valid&&choice==0){if(!M01Save.Continue(out string reason))dialogue.BeginText("Continuar",reason,Show);return;}
                if(choice==(valid?1:0)){M01Session.Instance.data=new M01Progress();SceneManager.LoadScene(M01Expedition.ScenePath("PRO_CASA"));}
                else M01PauseMenu.Quit();
            },"Afterglow: Blinding Hope");
        }
    }
}
