using UnityEngine;
using UnityEngine.InputSystem;
namespace Afterglow.M01
{
    public sealed class M01Investigation : MonoBehaviour
    {
        public M01Dialogue dialogue;
        public int index;
        InputAction interact;
        bool requested;
        void OnEnable(){interact=new InputAction("Investigate",InputActionType.Button,"<Keyboard>/e");interact.performed+=_=>{if(!dialogue.IsOpen)requested=true;};interact.Enable();}
        void OnDisable(){interact?.Dispose();}
        void Update()
        {
            if(!requested)return;requested=false;var player=FindAnyObjectByType<AfterglowPlayer>();
            if(!player||player.Dead||dialogue.IsOpen||Vector3.Distance(player.transform.position,transform.position)>2)return;
            dialogue.Begin(new[]{"N1D.01","N1D.02","N1D.03"},()=>M01Session.Instance.data.investigated[index]=true);
        }
    }
}
