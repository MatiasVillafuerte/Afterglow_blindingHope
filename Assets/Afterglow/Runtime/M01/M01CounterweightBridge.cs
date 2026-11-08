using UnityEngine;
using UnityEngine.InputSystem;
namespace Afterglow.M01
{
    public sealed class M01CounterweightBridge:MonoBehaviour
    {
        public const string OpenEvent="N01.S04.BRIDGE_OPEN";
        public string openEvent=OpenEvent,hintEvent="N1E.BRIDGE_HINT";
        public Transform deck,leverArm,counterweight;
        public BoxCollider crossing;
        /// <summary>Barandas laterales: solo bloquean con el puente bajado (la garganta sigue siendo una caída intencional).</summary>
        public Collider[] rails;
        public M01Dialogue dialogue;
        public GameObject prompt;
        public float duration=1;
        InputAction interact;bool requested,moving;float elapsed;
        M01PlayerRules owner;Vector3 weightStart;Quaternion leverStart;
        void OnEnable(){interact=new InputAction("Lower bridge",InputActionType.Button,"<Keyboard>/e");interact.performed+=_=>requested=true;interact.Enable();}
        void Start(){weightStart=counterweight.localPosition;leverStart=leverArm.localRotation;Apply(M01Session.Instance.data.events.Contains(openEvent)?1:0);}
        void OnDisable(){interact?.Dispose();if(owner)owner.restrictedActivity=false;}
        void Apply(float t)
        {
            deck.localRotation=Quaternion.Euler(Mathf.Lerp(-85,0,t),0,0);
            leverArm.localRotation=leverStart*Quaternion.Euler(Mathf.Lerp(0,50,t),0,0);
            counterweight.localPosition=weightStart+Vector3.down*(1.1f*t);
            crossing.enabled=t>=1;
            if(rails!=null)foreach(var r in rails)if(r)r.enabled=t>=1;
            if(prompt)prompt.SetActive(t<=0);
        }
        void Update()
        {
            bool pressed=requested;requested=false;
            if(moving){elapsed+=Time.deltaTime;float t=Mathf.Clamp01(elapsed/duration);Apply(Mathf.SmoothStep(0,1,t));if(t>=1){moving=false;if(owner)owner.restrictedActivity=false;}return;}
            if(!pressed||Time.timeScale<=0||M01Expedition.Transitioning||dialogue.IsOpen)return;
            var data=M01Session.Instance.data;
            if(data.events.Contains(openEvent)||(!string.IsNullOrEmpty(hintEvent)&&!data.events.Contains(hintEvent)))return;
            var p=FindAnyObjectByType<AfterglowPlayer>();
            if(!p||p.Dead||!p.Grounded||Vector3.Distance(p.transform.position,transform.position)>1.8f)return;
            owner=p.GetComponent<M01PlayerRules>();if(owner.BlocksControl||owner.restrictedActivity)return;
            data.events.Add(openEvent);moving=true;elapsed=0;owner.restrictedActivity=true;
        }
    }
}
