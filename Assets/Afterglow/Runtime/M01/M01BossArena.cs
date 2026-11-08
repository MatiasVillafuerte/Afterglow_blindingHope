using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Afterglow.M01
{
    /// <summary>
    /// Batalla contra PerforAudora (GDD 9). Tres grupos de resistencias montadas (2, 3 y 4, con 3 de vida): bajan solo cuando
    /// el taladro se atasca y conservan su daño entre atascos. Eliminar el tercer grupo vence al taladro. Piedras con aviso
    /// durante toda la pelea (más en los atascos). Platicio lanza ofrendas de arena; el altar de la arena procesa 5:
    /// neutral → mejora del Tío 15 s; con perjuicio → lo elimina; con mejora activa → rechaza sin consumir.
    /// Fases 2 y 3: 40 s jugables sin entrega válida → un único perjuicio. Morir reinicia desde el corredor (N6C).
    /// El guardado queda bloqueado. Los textos de reacción no pausan; las explicaciones N6A.13–15 y N6B.05–06 sí.
    /// </summary>
    public sealed class M01BossArena : MonoBehaviour
    {
        public static bool SkipIntro;
        public M01Dialogue dialogue; public M01BossDrill drill; public Transform altar, sonLedge; public M01Ending ending;
        public GameObject resistancePrefab; public Material warnMaterial, rockMaterial, offeringMaterial, lootMaterial;
        public Vector3 center; public float radius = 9.4f;
        public int Phase { get; private set; }
        public int ArenaOfferings { get; private set; }
        public string Effect { get; private set; } = "Neutral";
        public bool Won { get; private set; }
        static readonly int[] GroupSize = { 2, 3, 4 };
        readonly List<int> group = new List<int>(); readonly List<AfterglowEnemy> exposed = new List<AfterglowEnemy>();
        readonly HashSet<string> said = new HashSet<string>();
        float nextRock, nextThrow, buffUntil, deadline; bool fighting, clockActive, deathMarked; string buffKind;
        InputAction interact; bool requested; AfterglowPlayer player;
        M01Progress Data => M01Session.Instance.data;

        void OnEnable() { interact = new InputAction("Arena altar", InputActionType.Button, "<Keyboard>/e"); interact.performed += _ => { if (dialogue && !dialogue.IsOpen) requested = true; }; interact.Enable(); }
        void OnDisable() { interact?.Dispose(); M01PlayerRules.ResetTemporary(); }

        IEnumerator Start()
        {
            player = FindAnyObjectByType<AfterglowPlayer>();
            M01PlayerRules.ResetTemporary();
            if (Data.events.Contains("N6.VICTORIA")) { drill.gameObject.SetActive(false); ending.Resume(); yield break; }
            M01PlayerRules.SaveBlocked = true;
            drill.OnStuck = Dismount; drill.OnFreed = Remount;
            yield return null; while (M01Expedition.Transitioning) yield return null;
            if (!SkipIntro)
            {
                M01Toast.Message("Taladro aurífero", 3f);
                yield return Talk("N6A.13", "N6A.14", "N6A.15");
            }
            SkipIntro = false;
            StartPhase(1);
        }

        IEnumerator Talk(params string[] lines) { bool done = false; dialogue.Begin(lines, () => done = true); while (!done) yield return null; }
        void Say(string id) { if (said.Add(id)) M01Toast.Message(dialogue.LookupText(id), 3.2f); }

        void StartPhase(int phase)
        {
            Phase = phase; group.Clear(); for (int i = 0; i < GroupSize[phase - 1]; i++) group.Add(3);
            fighting = true; drill.Begin(phase); RefreshMounted();
            clockActive = phase >= 2; deadline = Time.time + 40;
            nextRock = Time.time + 2; nextThrow = Time.time + 1.5f;
        }

        void Dismount()
        {
            Say("N6B.01");
            // Fase 2 alterna lado de bajada; fase 3 baja por los dos lados.
            var right = drill.transform.right; int side = Phase == 2 ? (Random.value < .5f ? 1 : -1) : 1;
            for (int i = 0; i < group.Count; i++)
            {
                if (group[i] <= 0) continue;
                float s = Phase == 3 ? (i % 2 == 0 ? 1 : -1) : side;
                var pos = drill.transform.position + right * s * 1.7f + drill.transform.forward * (-.8f + i * .7f); pos.y = center.y + .05f;
                var go = Instantiate(resistancePrefab, pos, Quaternion.identity); go.name = "N6.RESISTENCIA." + Phase + "." + i;
                var e = go.GetComponent<AfterglowEnemy>(); e.maxHealth = 3; e.stationary = true; e.noReward = true;
                StartCoroutine(SetHealth(e, group[i])); exposed.Add(e);
            }
            RefreshMounted();
        }

        IEnumerator SetHealth(AfterglowEnemy e, int hp) { yield return null; int lost = e.maxHealth - hp; if (lost > 0) { var col = e.GetComponent<CharacterController>(); e.RestoreWorldOneState(hp, e.transform.position); } }

        void Remount()
        {
            bool survivors = false;
            for (int i = 0; i < exposed.Count; i++) { var e = exposed[i]; if (!e) continue; int idx = int.Parse(e.name.Split('.').Last()); group[idx] = e.Defeated ? 0 : e.Health; survivors |= !e.Defeated; Destroy(e.gameObject); }
            exposed.Clear();
            if (survivors) Say("N6B.03");
            RefreshMounted();
        }

        void RefreshMounted()
        {
            var mounted = drill.transform.Find("Resistencias montadas");
            if (!mounted) return;
            int alive = group.Count(h => h > 0); bool shown = drill.State != "Atascado";
            for (int i = 0; i < mounted.childCount; i++) mounted.GetChild(i).gameObject.SetActive(shown && i < alive);
        }

        void Update()
        {
            if (!player) return;
            if (player.Dead && !deathMarked) { deathMarked = true; if (!Data.events.Contains("N6.DERROTA")) Data.events.Add("N6.DERROTA"); M01Toast.Message(dialogue.LookupText("N6C.01"), 3f); fighting = false; drill.Halt(); }
            if (!fighting || Time.timeScale == 0) { requested = false; return; }
            // Grupo eliminado durante un atasco.
            if (drill.State == "Atascado" && exposed.Count > 0 && exposed.All(e => !e || e.Defeated))
            {
                for (int i = 0; i < exposed.Count; i++) if (exposed[i]) group[int.Parse(exposed[i].name.Split('.').Last())] = 0;
                if (group.All(h => h <= 0)) { foreach (var e in exposed) if (e) Destroy(e.gameObject, 1f); exposed.Clear(); StartCoroutine(NextGroup()); return; }
            }
            // Piedras: más frecuentes durante el atasco; lejos del altar.
            if (Time.time >= nextRock)
            {
                bool stuck = drill.State == "Atascado"; nextRock = Time.time + (stuck ? Random.Range(.9f, 1.4f) : Random.Range(2.4f, 3.8f));
                for (int tries = 0; tries < 8; tries++)
                {
                    var p = center + Quaternion.Euler(0, Random.Range(0, 360f), 0) * Vector3.forward * Random.Range(0, radius - 1.5f);
                    if (Vector3.Distance(Flat(p), Flat(altar.position)) < 2.6f) continue;
                    M01FallingRock.Spawn(new Vector3(p.x, center.y, p.z), Random.value < .3f, warnMaterial, rockMaterial); break;
                }
            }
            // Ofrendas de arena: cada 2–5 s, máximo 7 en suelo, salvo con mejora activa.
            if (Time.time >= nextThrow)
            {
                nextThrow = Time.time + Random.Range(2f, 5f);
                if (Effect != "Mejora" && FindObjectsByType<M01ArenaOffering>(FindObjectsSortMode.None).Length < 7)
                {
                    var go = GameObject.CreatePrimitive(PrimitiveType.Sphere); Destroy(go.GetComponent<Collider>()); go.name = "Ofrenda de arena"; go.transform.localScale = Vector3.one * .34f;
                    go.GetComponent<MeshRenderer>().sharedMaterial = offeringMaterial;
                    var o = go.AddComponent<M01ArenaOffering>(); o.arena = this; o.from = sonLedge.position;
                    var target = center + Quaternion.Euler(0, Random.Range(0, 360f), 0) * Vector3.forward * Random.Range(1.5f, radius - 2f); target.y = center.y + .35f; o.to = target;
                }
            }
            // Mejora con duración; plazo del Tío en fases 2 y 3 (no visible en el HUD).
            if (Effect == "Mejora" && Time.time >= buffUntil) { ClearEffect(); }
            if (clockActive && Effect != "Perjuicio" && Time.time >= deadline) ApplyDebuff();
            if (requested) { requested = false; if (Vector3.Distance(Flat(player.transform.position), Flat(altar.position)) < 2.2f) Deliver(); }
        }

        static Vector3 Flat(Vector3 v) { v.y = 0; return v; }

        public void CollectOffering() { ArenaOfferings++; M01Toast.Message("Ofrenda de arena · " + ArenaOfferings, 1.6f); }

        void Deliver()
        {
            if (Effect == "Mejora") { Say("N6B.12"); said.Remove("N6B.12"); return; }
            if (ArenaOfferings < 5) { M01Toast.Message(dialogue.LookupText("N6B.13").Replace("{cantidad}", (5 - ArenaOfferings).ToString()), 2.6f); return; }
            ArenaOfferings -= 5; deadline = Time.time + 40;
            if (Effect == "Perjuicio") { ClearEffect(); M01Toast.Message(dialogue.LookupText("N6B.11"), 3f); return; }
            string[] kinds = { "Daño", "Velocidad", "Intervalo" }; buffKind = kinds[Random.Range(0, 3)];
            Effect = "Mejora"; buffUntil = Time.time + 15;
            if (buffKind == "Daño") M01PlayerRules.DamageBonus = 1; else if (buffKind == "Velocidad") M01PlayerRules.SpeedMultiplier = 1.15f; else M01PlayerRules.IntervalMultiplier = .8f;
            M01Toast.Message(dialogue.LookupText("N6B.09"), 3f);
        }

        void ApplyDebuff()
        {
            ClearEffect(); Effect = "Perjuicio";
            // El daño actual es 1: el sorteo excluye su reducción (GDD 9.5).
            if (Random.value < .5f) M01PlayerRules.SpeedMultiplier = .85f; else M01PlayerRules.IntervalMultiplier = 1.2f;
            M01Toast.Message(dialogue.LookupText("N6B.10"), 3.2f);
        }

        void ClearEffect() { Effect = "Neutral"; M01PlayerRules.DamageBonus = 0; M01PlayerRules.SpeedMultiplier = M01PlayerRules.IntervalMultiplier = 1; }

        IEnumerator NextGroup()
        {
            fighting = false; drill.Halt();
            foreach (var r in FindObjectsByType<M01FallingRock>(FindObjectsSortMode.None)) Destroy(r.gameObject);
            if (Phase == 3) { yield return Victory(); yield break; }
            M01Toast.Message("Nuevas resistencias suben al taladro", 2.6f);
            yield return new WaitForSeconds(2f);
            if (Phase == 1) yield return Talk("N6B.05", "N6B.06");
            else yield return Talk("N6B.08");
            StartPhase(Phase + 1);
        }

        IEnumerator Victory()
        {
            Won = true; clockActive = false; ClearEffect();
            foreach (var o in FindObjectsByType<M01ArenaOffering>(FindObjectsSortMode.None)) Destroy(o.gameObject);
            ArenaOfferings = 0;
            if (!Data.events.Contains("N6.VICTORIA")) Data.events.Add("N6.VICTORIA");
            var flash = new GameObject("Explosión").AddComponent<Light>(); flash.transform.position = drill.transform.position + Vector3.up * 2; flash.type = LightType.Point; flash.range = 14; flash.color = new Color(1, .75f, .3f);
            for (float t = 0; t < 1.2f; t += Time.deltaTime) { flash.intensity = Mathf.Sin(t / 1.2f * Mathf.PI) * 8; drill.transform.localScale = Vector3.one * (1 + t * .15f); yield return null; }
            Destroy(flash.gameObject); var piecePos = drill.transform.position; drill.gameObject.SetActive(false);
            M01PlayerRules.SaveBlocked = false;
            ending.Begin(piecePos);
        }
    }
}
