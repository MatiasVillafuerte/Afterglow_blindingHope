using UnityEngine;
using UnityEngine.InputSystem;
namespace Afterglow.M01
{
    public sealed class M01RouteDoor:MonoBehaviour
    {
        public string destination;
        public Vector3 arrival=new Vector3(4,.05f,0);
        public bool leaveExpedition, enterExpedition;
        public int requiredOpening=-1;
        public string requiredEvent;
        bool warned;
        public bool requiresInteract;
        public float interactionRadius=1.8f;
        public float verticalTolerance;
        public bool fade, blockedWhenWorldClosed;
        InputAction interact;
        bool requested;
        void OnEnable()
        {
            if(!requiresInteract)return;
            interact=new InputAction("Door",InputActionType.Button,"<Keyboard>/e");
            interact.performed+=_=>requested=Time.timeScale>0&&!M01Expedition.Transitioning;
            interact.Enable();
        }
        void OnDisable(){interact?.Dispose();}
        void Update()
        {
            bool pressed=requested;requested=false;
            if(Time.timeScale==0||M01Expedition.Transitioning)return;
            if(requiresInteract&&!pressed)return;
            var player=FindAnyObjectByType<AfterglowPlayer>();
            if(!player||player.Dead)return;
            if(verticalTolerance>0&&Mathf.Abs(player.transform.position.y-transform.position.y)>verticalTolerance)return;
            var flat=player.transform.position-transform.position;flat.y=0;
            if(flat.magnitude>(requiresInteract?interactionRadius:1.05f)){warned=false;return;}
            var data=M01Session.Instance.data;
            if(blockedWhenWorldClosed&&data.worldClosed)return;
            if((requiredOpening>=0&&!data.opened[requiredOpening])||(!string.IsNullOrEmpty(requiredEvent)&&!data.events.Contains(requiredEvent)))
            {
                if(!warned){warned=true;FindAnyObjectByType<M01Dialogue>().BeginText("Paso cerrado",requiredOpening>=0?"Primero termina la investigación y completa el aporte en el altar.":"Habla con quien te espera junto a la entrada.");}return;
            }
            M01Expedition.Travel(destination,arrival,leaveExpedition,enterExpedition,fade);
        }
    }
}
