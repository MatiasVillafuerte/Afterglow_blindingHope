using UnityEngine;

namespace Afterglow.M01
{
    /// <summary>Ofrenda de arena lanzada por Platicio (GDD 9.4): cae en arco, caduca a los 5 s y parpadea los últimos 3.</summary>
    public sealed class M01ArenaOffering : MonoBehaviour
    {
        public Vector3 from, to; public M01BossArena arena; float born; Renderer body;
        void Start() { born = Time.time; body = GetComponentInChildren<Renderer>(); }
        void Update()
        {
            float t = Time.time - born;
            if (t < .7f) { var p = Vector3.Lerp(from, to, t / .7f); p.y += Mathf.Sin(t / .7f * Mathf.PI) * 3; transform.position = p; return; }
            transform.position = to;
            float life = t - .7f;
            if (body) body.enabled = life < 2 || (int)(life * 10) % 2 == 0;
            if (life >= 5) { Destroy(gameObject); return; }
            var player = FindAnyObjectByType<AfterglowPlayer>();
            if (player && Vector3.Distance(player.transform.position + Vector3.up * .35f, transform.position) < .95f) { arena.CollectOffering(); Destroy(gameObject); }
        }
    }
}
