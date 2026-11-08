using UnityEngine;
using UnityEngine.InputSystem;
namespace Afterglow.M01
{
    public sealed class M01PauseMenu:MonoBehaviour
    {
        public M01Dialogue dialogue;
        InputAction pause;
        bool requested;
        void OnEnable(){pause=new InputAction("Pause",InputActionType.Button,"<Keyboard>/escape");pause.performed+=_=>requested=true;pause.Enable();}
        void OnDisable(){pause?.Dispose();}
        void Update()
        {
            if(!requested)return;requested=false;
            if(dialogue.IsOpen||M01Expedition.Transitioning)return;
            dialogue.BeginChoice("Selecciona una opción.",new[]{"Continuar",M01Save.Allowed?"Guardar":"Guardar · bloqueado","Salir"},choice=>{
                if(choice==1)Save(false);else if(choice==2)ExitMenu();
            },"Pausa");
        }
        void Save(bool exit)
        {
            if(!M01Save.Allowed){dialogue.BeginText("Guardar","Espera a estar sobre suelo seguro y fuera de una secuencia para guardar.");return;}
            if(M01Save.TryWrite(out string error)){if(exit)Quit();else dialogue.Begin(new[]{"SYS.01"});}
            else{Debug.LogWarning("M01 save: "+error);dialogue.Begin(new[]{"SYS.02"});}
        }
        void ExitMenu()
        {
            if(!M01Save.Allowed)
            {
                dialogue.BeginChoice(dialogue.LookupText("SYS2.04"),new[]{"Salir sin guardar","Cancelar"},choice=>{if(choice==0)ConfirmExit();},"Salir");return;
            }
            dialogue.BeginChoice(dialogue.LookupText("SYS.03"),new[]{"Guardar y salir","Salir sin guardar","Cancelar"},choice=>{
                if(choice==0)Save(true);else if(choice==1)ConfirmExit();
            },"Salir");
        }
        void ConfirmExit()=>dialogue.BeginChoice(dialogue.LookupText("SYS.04"),new[]{"Salir","Cancelar"},choice=>{if(choice==0)Quit();},"Salir sin guardar");
        public static void Quit()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying=false;
            #else
            Application.Quit();
            #endif
        }
    }
}
