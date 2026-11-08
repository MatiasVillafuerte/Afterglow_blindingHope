using UnityEngine;

namespace Afterglow.M01
{
    /// <summary>Reja que sube cuando existe su evento (placa del carro). Queda abierta: nunca encierra al jugador.</summary>
    public sealed class M01Gate : MonoBehaviour
    {
        public string eventId;
        public Transform bars;
        public Collider blocker;
        public float lift = 2.9f, duration = .8f;
        float t; Vector3 closed;

        void Start()
        {
            closed = bars ? bars.localPosition : Vector3.zero;
            if (M01Session.Instance.data.events.Contains(eventId)) { t = 1; Apply(); }
        }

        void Update()
        {
            if (t >= 1 || !M01Session.Instance.data.events.Contains(eventId)) return;
            t = Mathf.Min(1, t + Time.deltaTime / duration);
            Apply();
            if (t >= 1) M01Toast.Message("La reja se abrió", 2.2f);
        }

        void Apply()
        {
            if (bars) bars.localPosition = closed + Vector3.up * lift * Mathf.SmoothStep(0, 1, t);
            if (blocker) blocker.enabled = t < .5f;
        }
    }
}
