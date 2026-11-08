using UnityEngine;
using UnityEngine.InputSystem;

namespace Afterglow.M01
{
    /// <summary>Placa de señal torcida (Q06): E la endereza de forma permanente. No altera la ruta obligatoria.</summary>
    public sealed class M01SignPlate : MonoBehaviour
    {
        public M01Dialogue dialogue; public string eventId; public Transform board; public float radius = 1.8f;
        InputAction interact; bool requested;
        void OnEnable() { interact = new InputAction("Fix plate", InputActionType.Button, "<Keyboard>/e"); interact.performed += _ => { if (dialogue && !dialogue.IsOpen) requested = true; }; interact.Enable(); }
        void OnDisable() { interact?.Dispose(); }
        void Start() { Apply(M01Session.Instance.data.events.Contains(eventId)); }
        void Apply(bool fixedPlate) { if (board) board.localRotation = Quaternion.Euler(0, 0, fixedPlate ? 0 : 28); }
        void Update()
        {
            if (!requested) return; requested = false;
            var p = FindAnyObjectByType<AfterglowPlayer>(); var data = M01Session.Instance.data;
            if (!p || p.Dead || Time.timeScale == 0 || Vector3.Distance(p.transform.position, transform.position) > radius || data.events.Contains(eventId)) return;
            data.events.Add(eventId); Apply(true);
            int count = 0; for (int i = 0; i < 3; i++) if (data.events.Contains("Q06.PLACA." + i)) count++;
            M01Toast.Message("Placa enderezada " + count + " / 3", 2.2f);
        }
    }
}
