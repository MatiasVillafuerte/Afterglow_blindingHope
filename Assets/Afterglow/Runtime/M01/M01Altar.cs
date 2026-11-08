using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Afterglow.M01
{
    public sealed class M01Altar : MonoBehaviour
    {
        public M01Dialogue dialogue;
        public int quotaIndex;
        /// <summary>Altar compartido del refugio: acredita la cuota del nivel de la expedición actual.</summary>
        public bool shared = true;
        public float interactionRadius = 2;
        InputAction interact;
        bool requested, opening;
        void OnEnable() { interact = new InputAction("Offer", InputActionType.Button, "<Keyboard>/e"); interact.performed += _ => { if (!dialogue.IsOpen) requested = true; }; interact.Enable(); }
        void OnDisable() { interact?.Dispose(); }
        void Update()
        {
            var data = M01Session.Instance.data;
            if (shared && !opening) quotaIndex = Mathf.Clamp(data.currentLevel - 1, 0, 4);
            if (!data.worldClosed && !opening && data.investigated[quotaIndex] && data.contributions[quotaIndex] >= M01Progress.Quotas[quotaIndex] && !data.opened[quotaIndex]) StartCoroutine(Open());
            if (!requested) return; requested = false;
            var player = FindAnyObjectByType<AfterglowPlayer>();
            if (!player || player.Dead || dialogue.IsOpen || Vector3.Distance(player.transform.position, transform.position) > interactionRadius) return;
            int missing = Mathf.Max(0, M01Progress.Quotas[quotaIndex] - data.contributions[quotaIndex]);
            if (data.worldClosed || missing == 0 || data.offerings.Count == 0) { Present(data.Offer(quotaIndex)); return; }
            var preview = data.offerings.Where(item => !item.stolen).Take(missing).Select(item => item.kind).ToArray();
            dialogue.BeginChoice("Unidades que se tomarán para el aporte: " + (preview.Length > 0 ? string.Join(", ", preview) : "ninguna disponible") + ".", new[] { "Entregar", "Cancelar" }, selection =>
            {
                if (selection != 0) return;
                // Commit only after confirmation, before presentation; held input cannot duplicate it.
                Present(data.Offer(quotaIndex));
            }, "Altar del Tío");
        }
        void Present(M01AltarResult result)
        {
            dialogue.BeginResolved(result.lineId, result.missing);
            if (result.rejected > 0) StartCoroutine(Reprimand());
        }
        IEnumerator Reprimand()
        {
            var player = FindAnyObjectByType<AfterglowPlayer>();
            var stone = GameObject.CreatePrimitive(PrimitiveType.Sphere); stone.name = "Piedra cómica sin daño";
            Destroy(stone.GetComponent<Collider>()); stone.transform.localScale = Vector3.one * .35f;
            float time = 0;
            while (time < .6f)
            {
                time += Time.unscaledDeltaTime;
                stone.transform.position = player.transform.position + Vector3.up * Mathf.Lerp(3.4f, 1.8f, Mathf.Clamp01(time / .6f));
                yield return null;
            }
            Destroy(stone);
        }
        IEnumerator Open()
        {
            opening = true;
            yield return new WaitForSeconds(2);
            while (dialogue.IsOpen) yield return null;
            if (M01Session.Instance.data.TryOpen(quotaIndex)) dialogue.Begin(new[] { "ALT.11" });
            opening = false;
        }
    }
}
