using UnityEngine;

namespace Afterglow.M01
{
    /// <summary>
    /// Lighmet (GDD 7.2, N4): 4 de vida. Espíritu de fuego que levita sin rumbo fijo dentro de su zona. Al detectar al
    /// jugador lanza bolas de fuego. El espíritu y cada proyectil emiten luz que ayuda a orientarse en la oscuridad.
    /// </summary>
    [RequireComponent(typeof(AfterglowEnemy))]
    public sealed class M01Lighmet : MonoBehaviour
    {
        public float wander = 2.5f, hover = 1.1f, detect = 7f, interval = 2.4f, shotSpeed = 5f, windup = .45f; public int shotDamage = 1;
        public SpriteRenderer body; public Sprite idle, cast, fireball;
        AfterglowEnemy enemy; Transform player; Vector3 home, target; float nextShot, castUntil, releaseAt = -1; int hitsAtCast; Light glow;
        void Awake() { enemy = GetComponent<AfterglowEnemy>(); enemy.stationary = true; }
        void Start()
        {
            player = FindAnyObjectByType<AfterglowPlayer>()?.transform; home = transform.position; target = home;
            glow = new GameObject("Luz del Lighmet").AddComponent<Light>(); glow.transform.SetParent(transform, false); glow.transform.localPosition = Vector3.up * (hover + .5f);
            glow.type = LightType.Point; glow.color = new Color(.75f, 1f, .35f); glow.range = 6; glow.intensity = 2.2f; glow.shadows = LightShadows.None;
            if (body) { body.transform.localPosition = Vector3.up * hover; body.sprite = idle; }
        }
        void Update()
        {
            if (enemy.Defeated) { if (glow) glow.enabled = false; return; }
            if (Time.timeScale == 0 || !player) return;
            if (Vector3.Distance(transform.position, target) < .2f) target = home + new Vector3(Random.Range(-wander, wander), 0, Random.Range(-wander * .5f, wander * .5f));
            transform.position = Vector3.MoveTowards(transform.position, target, .8f * Time.deltaTime); Physics.SyncTransforms();
            if (body) { body.transform.localPosition = Vector3.up * (hover + Mathf.Sin(Time.time * 2.4f) * .12f); body.flipX = player.position.x < transform.position.x; body.sprite = Time.time < castUntil ? cast : idle; }
            glow.intensity = 2f + Mathf.Sin(Time.time * 9) * .25f;
            var d = player.position - transform.position;
            // Aviso legible: el conjuro dura «windup» con el sprite de lanzamiento; un golpe del pico durante el aviso lo cancela.
            if (releaseAt >= 0)
            {
                if (enemy.HitsTaken > hitsAtCast) { releaseAt = -1; castUntil = Time.time; }
                else if (Time.time >= releaseAt) { releaseAt = -1; Shoot(); }
            }
            else if (d.magnitude < detect && Time.time >= nextShot)
            {
                nextShot = Time.time + interval; castUntil = Time.time + windup + .2f; releaseAt = Time.time + windup; hitsAtCast = enemy.HitsTaken;
            }
        }
        void Shoot()
        {
            {
                var shot = new GameObject("Bola de fuego"); shot.transform.position = transform.position + Vector3.up * (hover + .3f);
                var p = shot.AddComponent<M01Fireball>(); p.velocity = (player.position + Vector3.up * .9f - shot.transform.position).normalized * shotSpeed; p.damage = shotDamage; p.sprite = fireball;
            }
        }
    }
}
