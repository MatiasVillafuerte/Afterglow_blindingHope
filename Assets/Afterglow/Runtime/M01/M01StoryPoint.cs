using UnityEngine;
using UnityEngine.InputSystem;

namespace Afterglow.M01
{
    public sealed class M01StoryPoint : MonoBehaviour
    {
        public M01Dialogue dialogue;
        public string eventId;
        public string[] lineIds;
        public bool automatic, givePickaxe, givePhone;
        public float radius = 2;
        public string destination;
        public Vector3 arrival = new Vector3(4,.05f,0);
        public AfterglowEnemy[] wakeEnemies;
        public string prerequisiteEvent;
        public AfterglowEnemy defeatedEnemy;
        /// <summary>Si no está vacío, al terminar la conversación se evalúa el regalo único de Platicio de ese nivel.</summary>
        public string giftLevel;
        InputAction interact;
        bool requested, running;
        void OnEnable()
        {
            interact = new InputAction("Story interaction",InputActionType.Button,"<Keyboard>/e");
            interact.performed += _ => { if(dialogue && !dialogue.IsOpen) requested=true; }; interact.Enable();
        }
        void OnDisable() { interact?.Dispose(); }
        void Start()
        {
            if(wakeEnemies!=null)foreach(var enemy in wakeEnemies)if(enemy)enemy.enabled=M01Session.Instance.data.events.Contains(eventId);
        }
        void Update()
        {
            bool pressed=requested;requested=false;
            if(running || (!automatic&&!pressed) || dialogue.IsOpen || M01Expedition.Transitioning)return;
            var player=FindAnyObjectByType<AfterglowPlayer>();
            if(!player||player.Dead||Vector3.Distance(player.transform.position,transform.position)>radius)return;
            var data=M01Session.Instance.data;
            if((!string.IsNullOrEmpty(prerequisiteEvent)&&!data.events.Contains(prerequisiteEvent))||(defeatedEnemy&&!defeatedEnemy.Defeated))return;
            if(!string.IsNullOrEmpty(eventId)&&data.events.Contains(eventId))return;
            running=true;
            dialogue.Begin(lineIds,()=>{
                if(!string.IsNullOrEmpty(eventId))data.events.Add(eventId);
                if(eventId=="P02.CAPTAIN"&&!data.events.Contains("M01.WORLD_STARTED"))data.events.Add("M01.WORLD_STARTED");
                if(givePickaxe){data.GrantKeyItem("PICO");player.GetComponent<M01PlayerRules>().hasPickaxe=true;}
                if(givePhone)data.GrantKeyItem("TELEFONO");
                if(!string.IsNullOrEmpty(giftLevel))M01PlaticioGift.Roll(giftLevel);
                if(wakeEnemies!=null)foreach(var enemy in wakeEnemies)if(enemy)enemy.enabled=true;
                running=false;
                if(!string.IsNullOrEmpty(destination))M01Expedition.Travel(destination,arrival,fade:eventId=="P02.CAPTAIN");
            });
        }
    }
}
