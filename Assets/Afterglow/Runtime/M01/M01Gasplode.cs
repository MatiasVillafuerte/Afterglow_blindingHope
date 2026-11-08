using UnityEngine;

namespace Afterglow.M01
{
    /// <summary>
    /// Gasplode (GDD 7.2, N3): sensor de gas inmóvil. Al detectar al jugador dentro de su zona se alarma (pose roja)
    /// y explota tras un aviso breve. Se puede rodear. Vida, radio y daño están por definir en el GDD: aquí son
    /// parámetros de prueba (1 de vida, zona de aviso 2,6 m, radio de explosión 1,9 m, daño 2). Un pico lo desactiva.
    /// </summary>
    [RequireComponent(typeof(AfterglowEnemy))]
    public sealed class M01Gasplode : MonoBehaviour
    {
        public float warnRadius = 2.6f, blastRadius = 1.9f, fuse = .9f; public int blastDamage = 2;
        public SpriteRenderer body; public Sprite idle, alert, blast;
        public string State { get; private set; } = "Quieto";
        AfterglowEnemy enemy; Transform player; float armedAt = -1, blastEnd;
        void Awake() { enemy = GetComponent<AfterglowEnemy>(); enemy.stationary = true; }
        void Start() { player = FindAnyObjectByType<AfterglowPlayer>()?.transform; if (body && idle) body.sprite = idle; }
        void Update()
        {
            if (Time.timeScale == 0 || !player) return;
            if (State == "Explotó") { if (Time.time > blastEnd && body) body.enabled = false; return; }
            if (enemy.Defeated) { State = "Desactivado"; return; }
            var d = player.position - transform.position; d.y = 0;
            if (State == "Quieto" && d.magnitude < warnRadius && Mathf.Abs(player.position.y - transform.position.y) < 1.2f) { State = "Alarma"; armedAt = Time.time; if (body && alert) body.sprite = alert; }
            if (State == "Alarma")
            {
                if (body) body.transform.localScale = Vector3.one * (1 + Mathf.Sin((Time.time - armedAt) * 30) * .05f);
                if (Time.time - armedAt >= fuse) Explode(d.magnitude);
            }
        }
        void Explode(float distance)
        {
            State = "Explotó"; blastEnd = Time.time + .5f;
            if (body && blast) { body.sprite = blast; body.transform.localScale = Vector3.one * 1.3f; }
            var p = player.GetComponent<AfterglowPlayer>();
            if (distance <= blastRadius && p) p.TakeDamage(blastDamage);
            var col = GetComponent<CharacterController>(); if (col) col.enabled = false;
            enemy.enabled = false;
        }
    }
}
