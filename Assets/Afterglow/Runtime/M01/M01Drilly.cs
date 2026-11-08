using UnityEngine;

namespace Afterglow.M01
{
    /// <summary>
    /// Drilly (GDD 7.2, N3): 3 de vida. Taladro que detecta y sigue al jugador bajo tierra (montículo de tierra que se mueve),
    /// tras un intervalo emerge con aviso (el montículo tiembla) e inflige daño si el jugador sigue encima.
    /// Queda fuera un momento, vulnerable al pico, y vuelve a enterrarse. Intervalos expuestos como parámetros de prueba.
    /// </summary>
    [RequireComponent(typeof(AfterglowEnemy))]
    public sealed class M01Drilly : MonoBehaviour
    {
        public float detect = 7, chaseSpeed = 2.2f, followTime = 2.4f, warn = .6f, exposed = 1.6f, hitRadius = 1.25f; public int damage = 1;
        public SpriteRenderer body; public Transform mound;
        public string State { get; private set; } = "Enterrado";
        AfterglowEnemy enemy; Transform player; CharacterController controller; float stateAt; Vector3 home;
        void Awake() { enemy = GetComponent<AfterglowEnemy>(); enemy.stationary = true; controller = GetComponent<CharacterController>(); }
        void Start() { player = FindAnyObjectByType<AfterglowPlayer>()?.transform; home = transform.position; Bury(); }
        void Bury() { State = "Enterrado"; stateAt = Time.time; if (body) body.enabled = false; if (mound) mound.gameObject.SetActive(true); if (controller) controller.enabled = false; }
        void Update()
        {
            if (Time.timeScale == 0 || !player || enemy.Defeated) { if (enemy.Defeated && mound) mound.gameObject.SetActive(false); return; }
            var d = player.position - transform.position; d.y = 0;
            switch (State)
            {
                case "Enterrado":
                    if (d.magnitude < detect) { State = "Siguiendo"; stateAt = Time.time; }
                    break;
                case "Siguiendo":
                    if (d.magnitude > .2f) transform.position += d.normalized * Mathf.Min(chaseSpeed * Time.deltaTime, d.magnitude);
                    if (Time.time - stateAt > followTime) { State = "Aviso"; stateAt = Time.time; }
                    break;
                case "Aviso":
                    if (mound) mound.localScale = new Vector3(1, 1 + Mathf.Abs(Mathf.Sin((Time.time - stateAt) * 40)) * .5f, 1);
                    if (Time.time - stateAt > warn) Emerge(d.magnitude);
                    break;
                case "Fuera":
                    if (Time.time - stateAt > exposed) Bury();
                    break;
            }
        }
        void Emerge(float distance)
        {
            State = "Fuera"; stateAt = Time.time;
            if (mound) { mound.localScale = Vector3.one; mound.gameObject.SetActive(false); }
            if (body) body.enabled = true; if (controller) controller.enabled = true;
            Physics.SyncTransforms();
            if (distance <= hitRadius) player.GetComponent<AfterglowPlayer>()?.TakeDamage(damage);
        }
    }
}
