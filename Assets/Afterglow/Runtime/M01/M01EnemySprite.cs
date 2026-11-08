using UnityEngine;

namespace Afterglow.M01
{
    /// <summary>
    /// Figura 2D de un enemigo a partir de las poses separadas de su lámina original
    /// (Assets/Afterglow/Character/Enemigos). Resisworm: avanza a brincos (pose 0 agachado ↔ pose 2 en salto),
    /// se yergue (pose 1) al preparar la mordida. Mira hacia el jugador.
    /// </summary>
    public sealed class M01EnemySprite : MonoBehaviour
    {
        public AfterglowEnemy enemy;
        public SpriteRenderer body;
        public Sprite crouch, stand, hop;
        public float hopHeight = .35f, hopPeriod = .42f;
        Transform player; Vector3 rest;

        void Start() { player = FindAnyObjectByType<AfterglowPlayer>()?.transform; rest = body.transform.localPosition; }

        void LateUpdate()
        {
            if (!enemy || !body) return;
            if (player) body.flipX = player.position.x < enemy.transform.position.x;
            float phase = enemy.AttackPhase;
            if (phase >= 0 && phase < .7f) { body.sprite = phase < .3f ? stand : crouch; body.transform.localPosition = rest; return; }
            if (enemy.Moving)
            {
                float t = (Time.time % hopPeriod) / hopPeriod;
                body.sprite = t < .5f ? hop : crouch;
                body.transform.localPosition = rest + Vector3.up * Mathf.Sin(t * Mathf.PI) * hopHeight;
            }
            else { body.sprite = crouch; body.transform.localPosition = rest + Vector3.up * Mathf.Abs(Mathf.Sin(Time.time * 2.2f)) * .04f; }
        }
    }
}
