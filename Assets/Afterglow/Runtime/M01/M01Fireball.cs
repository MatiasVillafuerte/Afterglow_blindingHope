using UnityEngine;

namespace Afterglow.M01
{
    /// <summary>Proyectil del Lighmet: viaja en línea recta, ilumina su entorno y daña al tocar al jugador.</summary>
    public sealed class M01Fireball : MonoBehaviour
    {
        public Vector3 velocity; public int damage = 1; public Sprite sprite; float born;
        void Start()
        {
            born = Time.time;
            var light = gameObject.AddComponent<Light>(); light.type = LightType.Point; light.color = new Color(.8f, 1f, .3f); light.range = 4.5f; light.intensity = 2.5f; light.shadows = LightShadows.None;
            if (sprite) { var sr = gameObject.AddComponent<SpriteRenderer>(); sr.sprite = sprite; transform.localScale = Vector3.one * .45f; }
        }
        void Update()
        {
            if (Time.timeScale == 0) return;
            transform.position += velocity * Time.deltaTime;
            var player = FindAnyObjectByType<AfterglowPlayer>();
            if (player && Vector3.Distance(transform.position, player.transform.position + Vector3.up * .9f) < .55f) { player.TakeDamage(damage); Destroy(gameObject); return; }
            if (Time.time - born > 3.5f || Physics.CheckSphere(transform.position, .15f, 1 << 0, QueryTriggerInteraction.Ignore)) Destroy(gameObject);
        }
    }
}
