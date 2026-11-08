using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Afterglow.M01
{
    /// <summary>
    /// Corredor de N6 (N6A): voces de los atrapados como subtítulos al avanzar (no detienen al jugador); E en la puerta
    /// inicia el rescate y la irrupción del taladro, y lleva a la arena. Tras perder contra el jefe, mantener F 1 s junto a
    /// la puerta omite rescate y presentación (N6C.03–04).
    /// </summary>
    public sealed class M01RescueDoor : MonoBehaviour
    {
        public M01Dialogue dialogue; public string arenaScene = "N06S02"; public Vector3 arenaArrival = new Vector3(0, .05f, -7.5f);
        public float[] voiceX = { -6, -2, 3 }; public string[] voiceLines = { "N6A.02", "N6A.03", "N6A.04" };
        InputAction interact, skip; bool requested, running; int voice; float skipHeld; AfterglowPlayer player; GameObject skipHint;
        M01Progress Data => M01Session.Instance.data;
        void OnEnable() { interact = new InputAction("Rescue", InputActionType.Button, "<Keyboard>/e"); interact.performed += _ => { if (dialogue && !dialogue.IsOpen) requested = true; }; interact.Enable(); skip = new InputAction("Skip", InputActionType.Button, "<Keyboard>/f"); skip.Enable(); }
        void OnDisable() { interact?.Dispose(); skip?.Dispose(); }
        void Start() { player = FindAnyObjectByType<AfterglowPlayer>(); if (Data.events.Contains("N6.DERROTA")) M01Toast.Message(dialogue.LookupText("N6C.03"), 4f); }
        void Update()
        {
            if (!player || running || Time.timeScale == 0) { requested = false; return; }
            while (voice < voiceX.Length && player.transform.position.x > voiceX[voice]) { M01Toast.Message(dialogue.LookupText(voiceLines[voice]), 2.6f); voice++; }
            bool near = Vector3.Distance(player.transform.position, transform.position) < 2.4f;
            if (near && Data.events.Contains("N6.DERROTA") && skip.IsPressed()) { skipHeld += Time.deltaTime; if (skipHeld > 1f) { running = true; M01BossArena.SkipIntro = true; M01Expedition.Travel(arenaScene, arenaArrival, fade: true); } }
            else skipHeld = 0;
            if (!requested) return; requested = false;
            if (near && !dialogue.IsOpen) StartCoroutine(Rescue());
        }
        IEnumerator Talk(params string[] lines) { bool done = false; dialogue.Begin(lines, () => done = true); while (!done) yield return null; }
        IEnumerator Rescue()
        {
            running = true;
            yield return Talk("N6A.06", "N6A.07", "N6A.08");
            var cam = FindAnyObjectByType<AfterglowCamera>();
            for (float t = 0; t < 1.2f; t += Time.deltaTime) { if (cam) cam.transform.position += Random.insideUnitSphere * .06f; yield return null; }
            yield return Talk("N6A.10", "N6A.11");
            if (!Data.events.Contains("N6A.RESCATE")) Data.events.Add("N6A.RESCATE");
            M01Expedition.Travel(arenaScene, arenaArrival, fade: true);
        }
    }
}
