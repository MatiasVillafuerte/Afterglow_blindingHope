using UnityEngine;
using UnityEngine.InputSystem;

namespace Afterglow.M01
{
    /// <summary>Palanca de topes: devuelve a su inicio los carros no enganchados (N2.03). No reinicia la galería.</summary>
    public sealed class M01CartResetLever : MonoBehaviour
    {
        public M01Dialogue dialogue;
        public M01RailCart[] carts;
        public Transform arm;
        public string lineId = "N2.03", hintEvent = "N2.LEVER_HINT";
        public float radius = 1.8f;
        InputAction interact; bool requested; float animate;
        void OnEnable() { interact = new InputAction("Reset carts", InputActionType.Button, "<Keyboard>/e"); interact.performed += _ => { if (dialogue && !dialogue.IsOpen) requested = true; }; interact.Enable(); }
        void OnDisable() { interact?.Dispose(); }
        void Update()
        {
            if (arm && animate > 0) { animate -= Time.deltaTime; arm.localRotation = Quaternion.Euler(0, 0, Mathf.Sin((1 - animate / .5f) * Mathf.PI) * 45); }
            if (!requested) return; requested = false;
            var player = FindAnyObjectByType<AfterglowPlayer>();
            if (!player || player.Dead || Time.timeScale == 0 || Vector3.Distance(player.transform.position, transform.position) > radius) return;
            foreach (var cart in carts) if (cart) cart.ResetToStart();
            animate = .5f;
            var data = M01Session.Instance.data;
            if (!data.events.Contains(hintEvent)) { data.events.Add(hintEvent); dialogue.Begin(new[] { lineId }); }
            else M01Toast.Message("Los carros vuelven a sus topes", 2f);
        }
    }
}
