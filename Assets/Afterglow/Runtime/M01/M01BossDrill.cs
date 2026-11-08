using System.Collections.Generic;
using UnityEngine;

namespace Afterglow.M01
{
    /// <summary>
    /// PerforAudora (GDD 9.2). Apunta, anuncia la embestida y avanza. Fase 1: recta a 6 m/s, aviso 1,5 s, atasco 7 s.
    /// Fase 2: 7 m/s, giro hasta 15 °/s, atasco 7 s. Fase 3: 8 m/s, giro hasta 25 °/s, atasco 8 s. En fases 2 y 3 fija la
    /// dirección en los últimos 0,5 s. Cualquier punto de la pared circular lo atasca; entonces sus resistencias bajan y solo
    /// así reciben daño. Si alcanza al jugador: 2 de daño, se detiene, retrocede y gira ruedas 1 s antes de volver a apuntar.
    /// No recibe daño directo del pico.
    /// </summary>
    public sealed class M01BossDrill : MonoBehaviour
    {
        public Vector3 arenaCenter; public float arenaRadius = 9.4f, bodyRadius = 1.2f;
        public Transform model; public Transform warning; public int phase = 1;
        public string State { get; private set; } = "Quieto";
        public float StateTime => Time.time - stateAt;
        public System.Action OnStuck, OnFreed;
        static readonly float[] Speeds = { 6, 7, 8 }, Turn = { 0, 15, 25 }, Stuck = { 7, 7, 8 };
        public float aimTime = .8f, telegraph = 1.5f;
        Transform player; float stateAt, chargeStart; Vector3 heading = Vector3.forward; bool active;
        public bool Exposed => State == "Atascado";

        public void Begin(int newPhase) { phase = Mathf.Clamp(newPhase, 1, 3); active = true; Set("Apuntando"); }
        public void Halt() { active = false; Set("Quieto"); if (warning) warning.gameObject.SetActive(false); }
        void Start() { player = FindAnyObjectByType<AfterglowPlayer>()?.transform; if (warning) warning.gameObject.SetActive(false); }
        void Set(string s) { State = s; stateAt = Time.time; }

        Vector3 ToPlayer() { var d = player.position - transform.position; d.y = 0; return d.sqrMagnitude > .01f ? d.normalized : heading; }

        void Update()
        {
            if (!active || !player || Time.timeScale == 0) return;
            int i = phase - 1;
            switch (State)
            {
                case "Apuntando":
                    heading = Vector3.RotateTowards(heading, ToPlayer(), 4f * Time.deltaTime, 0);
                    Face();
                    if (StateTime > aimTime) { Set("Aviso"); if (warning) warning.gameObject.SetActive(true); }
                    break;
                case "Aviso":
                    if (phase > 1) heading = Vector3.RotateTowards(heading, ToPlayer(), 2f * Time.deltaTime, 0);
                    Face();
                    if (model) model.localPosition = new Vector3(Mathf.Sin(Time.time * 60) * .05f, 0, 0);
                    if (StateTime > telegraph) { Set("Embistiendo"); chargeStart = Time.time; if (warning) warning.gameObject.SetActive(false); if (model) model.localPosition = Vector3.zero; }
                    break;
                case "Embistiendo":
                    // Corrección limitada en fases 2 y 3, salvo en los últimos 0,5 s (dirección fija para poder esquivar).
                    float elapsed = Time.time - chargeStart; float remaining = (arenaRadius * 2) / Speeds[i] - elapsed;
                    if (Turn[i] > 0 && remaining > .5f) heading = Vector3.RotateTowards(heading, ToPlayer(), Turn[i] * Mathf.Deg2Rad * Time.deltaTime, 0);
                    Face();
                    var next = transform.position + heading * Speeds[i] * Time.deltaTime;
                    var flat = next - arenaCenter; flat.y = 0;
                    if (flat.magnitude > arenaRadius - bodyRadius) { Set("Atascado"); OnStuck?.Invoke(); M01Toast.Message("¡El taladro se atascó!", 2f); break; }
                    transform.position = next;
                    var p = player.GetComponent<AfterglowPlayer>();
                    var toPlayer = player.position - transform.position; toPlayer.y = 0;
                    if (toPlayer.magnitude < bodyRadius + .45f && Vector3.Dot(toPlayer.normalized, heading) > .2f) { if (p.TakeDamage(2)) Set("Retrocediendo"); }
                    break;
                case "Retrocediendo":
                    if (StateTime < .5f) transform.position -= heading * 2.5f * Time.deltaTime;
                    if (model) model.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * 40) * 4);
                    if (StateTime > 1f) { if (model) model.localRotation = Quaternion.identity; Set("Apuntando"); }
                    break;
                case "Atascado":
                    if (model) model.localPosition = new Vector3(Mathf.Sin(Time.time * 35) * .03f, 0, 0);
                    if (StateTime > Stuck[i]) { if (model) model.localPosition = Vector3.zero; OnFreed?.Invoke(); transform.position -= heading * 1.2f; Set("Apuntando"); }
                    break;
            }
        }

        void Face() { transform.rotation = Quaternion.LookRotation(heading, Vector3.up); }
    }
}
