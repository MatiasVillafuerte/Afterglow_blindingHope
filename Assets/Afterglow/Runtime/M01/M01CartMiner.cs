using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Afterglow.M01
{
    /// <summary>
    /// Mineros de la vagoneta (N5). Rol "salida": explica el objetivo (N5.02) y pide aceptación (Subir / Todavía no).
    /// Rol "llegada": cuenta los minerales del intento; si faltan pide repetir; si alcanza el umbral entrega una sola vez
    /// las ofrendas necesarias para la cuota de N5 (N5.04). Recarga y repetición no duplican la recompensa.
    /// </summary>
    public sealed class M01CartMiner : MonoBehaviour
    {
        public M01Dialogue dialogue; public bool arrivalRole; public int threshold = 12;
        public string rideScene = "N05S02", rewardEvent = "N5.PREMIO"; public Vector3 rideStart = new Vector3(2, .6f, 0);
        InputAction interact; bool requested;
        M01Progress Data => M01Session.Instance.data;
        void OnEnable() { interact = new InputAction("Cart miner", InputActionType.Button, "<Keyboard>/e"); interact.performed += _ => { if (dialogue && !dialogue.IsOpen) requested = true; }; interact.Enable(); }
        void OnDisable() { interact?.Dispose(); }

        System.Collections.IEnumerator Start()
        {
            if (!arrivalRole || M01MineCartRide.LastAttemptMinerals < 0) yield break;
            while (M01Expedition.Transitioning || dialogue.IsOpen) yield return null;
            yield return new WaitForSeconds(.4f);
            Evaluate(M01MineCartRide.LastAttemptMinerals); M01MineCartRide.LastAttemptMinerals = -1;
        }

        void Update()
        {
            if (!requested) return; requested = false;
            var p = FindAnyObjectByType<AfterglowPlayer>();
            if (!p || p.Dead || dialogue.IsOpen || Vector3.Distance(p.transform.position, transform.position) > 2.4f) return;
            if (arrivalRole) { dialogue.BeginText("Minero del andén", Data.events.Contains(rewardEvent) ? "Ya te di las ofrendas del viaje. La puerta del bloqueo está al fondo." : "Aquí cuento los minerales de cada viaje. Necesitamos " + threshold + "."); return; }
            dialogue.Begin(new[] { "N5.02" }, () => dialogue.BeginChoice("¿Subir a la vagoneta? Hacen falta " + threshold + " minerales.", new[] { "Subir", "Todavía no" }, choice =>
            {
                if (choice == 0) M01Expedition.Travel(rideScene, rideStart, fade: true);
            }, "Minero de la estación"));
        }

        void Evaluate(int minerals)
        {
            if (minerals < threshold)
            {
                dialogue.BeginChoice("Este viaje trajo " + minerals + " de " + threshold + " minerales. Faltan " + (threshold - minerals) + ". ¿Lo intentamos otra vez?", new[] { "Repetir", "Ahora no" }, choice =>
                { if (choice == 0) M01Expedition.Travel(rideScene, rideStart, fade: true); }, "Minero del andén");
                return;
            }
            if (Data.events.Contains(rewardEvent)) { dialogue.BeginText("Minero del andén", minerals + " minerales. Buen viaje; la recompensa ya te la había dado."); return; }
            Data.events.Add(rewardEvent);
            int missing = Mathf.Max(0, M01Progress.Quotas[4] - Data.contributions[4]);
            int held = Data.offerings.Count(o => !o.stolen);
            int give = Mathf.Max(0, missing - held);
            for (int i = 0; i < give; i++) Data.CollectOffering("N5.PREMIO." + i, "Coca", false);
            dialogue.Begin(new[] { "N5.04" });
        }
    }
}
