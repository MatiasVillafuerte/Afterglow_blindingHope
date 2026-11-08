using UnityEngine;

namespace Afterglow.M01
{
    /// <summary>
    /// Carro sobre riel (N2). El jugador lo empuja caminando contra él a lo largo del riel; los topes limitan su recorrido.
    /// Sobre su placa registra un evento persistente: abre una reja o cuenta para Q03. Los carros marcados quedan
    /// enganchados en su apartadero. La palanca de topes devuelve los no enganchados a su inicio.
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public sealed class M01RailCart : MonoBehaviour
    {
        public string cartId, eventId;
        public float min, max, start, plate, speed = 2.6f;
        public bool marked;
        public float halfLength = .68f;
        public bool OnPlate => Mathf.Abs(transform.position.x - plate) < .32f;
        public bool Latched { get; private set; }
        AfterglowPlayer player; float lastAudio;

        void Start()
        {
            player = FindAnyObjectByType<AfterglowPlayer>();
            var data = M01Session.Instance.data;
            if (!string.IsNullOrEmpty(eventId) && data.events.Contains(eventId)) { SetX(plate); Latched = marked; }
            else SetX(start);
        }

        public void ResetToStart() { if (!Latched) SetX(start); }

        void SetX(float x) { var p = transform.position; p.x = Mathf.Clamp(x, min, max); transform.position = p; }

        void Update()
        {
            if (Latched || Time.timeScale == 0 || !player || player.Dead) return;
            var delta = player.transform.position - transform.position;
            if (Mathf.Abs(delta.z) > .75f || Mathf.Abs(delta.y) > .7f) return;
            float input = player.MoveInput.x;
            // El jugador está pegado a una cara del carro y avanza hacia él.
            bool fromLeft = delta.x < 0 && delta.x > -(halfLength + .55f) && input > .4f;
            bool fromRight = delta.x > 0 && delta.x < halfLength + .55f && input < -.4f;
            if (!fromLeft && !fromRight) return;
            float before = transform.position.x;
            SetX(before + Mathf.Sign(input) * speed * Time.deltaTime);
            if (Mathf.Abs(transform.position.x - before) > .0001f && Time.time - lastAudio > .25f) lastAudio = Time.time;
            if (OnPlate && !string.IsNullOrEmpty(eventId))
            {
                var data = M01Session.Instance.data;
                if (!data.events.Contains(eventId)) { data.events.Add(eventId); if (marked) M01Toast.Message("Carro en su apartadero", 2.2f); }
                if (marked) { SetX(plate); Latched = true; }
            }
        }
    }
}
