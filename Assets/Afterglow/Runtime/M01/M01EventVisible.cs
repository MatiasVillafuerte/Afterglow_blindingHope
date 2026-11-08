using UnityEngine;

namespace Afterglow.M01
{
    /// <summary>Muestra el objeto solo si existe el evento (p. ej. el minero rescatado de Q05 en la bocamina).</summary>
    public sealed class M01EventVisible : MonoBehaviour
    {
        public string eventId;
        public GameObject[] extra;
        void Start() { Refresh(); }
        void Update() { Refresh(); }
        void Refresh()
        {
            bool on = M01Session.Instance.data.events.Contains(eventId);
            foreach (var r in GetComponentsInChildren<Renderer>(true)) r.enabled = on;
            foreach (var b in GetComponents<Behaviour>()) if (b != this) b.enabled = on;
            if (extra != null) foreach (var g in extra) if (g) g.SetActive(on);
        }
    }
}
