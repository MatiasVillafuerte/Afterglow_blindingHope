using UnityEngine;
using UnityEngine.InputSystem;

namespace Afterglow.M01
{
    public sealed class M01DialoguePoint : MonoBehaviour
    {
        public M01Dialogue dialogue;
        public string[] lineIds;
        public float radius = 2;
        InputAction interact;
        bool requested;
        void OnEnable()
        {
            interact = new InputAction("Talk", InputActionType.Button, "<Keyboard>/e");
            interact.performed += _ => { if (!dialogue.IsOpen) requested = true; }; interact.Enable();
        }
        void Update()
        {
            if (!requested) return; requested = false;
            if (dialogue.IsOpen) return;
            var player = FindAnyObjectByType<AfterglowPlayer>();
            if (player && !player.Dead && Vector3.Distance(player.transform.position, transform.position) <= radius)
                dialogue.Begin(lineIds);
        }
        void OnDisable() { interact?.Dispose(); }
    }
}
