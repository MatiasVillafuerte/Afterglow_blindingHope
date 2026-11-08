using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Afterglow.M01
{
    /// <summary>
    /// Efectos del árbol (M01SkillTree) sobre el personaje activo: marcha, aéreo, salto, velocidad, Barrido (mantener X
    /// 0,5 s y soltar), Giro minero (Z), Esquiva (Shift), Ojo entrenado (vida visible), Recuperación. El daño (Filo,
    /// Preciso) y Enlazados se aplican en M01PlayerRules; Bolsillos y Afinidad en el botín.
    /// </summary>
    public sealed class M01SkillEffects : MonoBehaviour
    {
        AfterglowPlayer player; M01PlayerRules rules; CharacterController controller;
        float baseJump, xDown = -1, nextSpin, nextDash, dashUntil, lastHurt, lastRegen, eyeRefresh; Vector3 dashDir; int lastHealth;
        public static int Rank(string id) => M01SkillTree.Rank(id);

        void Start() { player = GetComponent<AfterglowPlayer>(); rules = GetComponent<M01PlayerRules>(); controller = GetComponent<CharacterController>(); baseJump = player.jumpHeight; lastHealth = player.Health; lastHurt = Time.time; }

        void Update()
        {
            if (M01Session.Instance == null) return;
            rules.walkingAttackUnlocked = Rank("MARCHA") > 0; rules.airAttackUnlocked = Rank("AEREO") > 0;
            player.jumpHeight = baseJump * (1 + .05f * Rank("SALTO"));
            if (player.Health < lastHealth) lastHurt = Time.time; lastHealth = player.Health;
            var k = Keyboard.current; if (k == null || player.Dead || rules.BlocksControl) { xDown = -1; return; }
            // Barrido frontal: mantener X al menos 0,5 s y soltar.
            if (k.xKey.wasPressedThisFrame) xDown = Time.time;
            if (k.xKey.wasReleasedThisFrame && xDown > 0 && Time.time - xDown >= .5f && Rank("BARRIDO") > 0 && player.Grounded) Sweep(1.9f, 1.35f, "Barrido");
            if (k.xKey.wasReleasedThisFrame) xDown = -1;
            // Giro minero: Z, área alrededor, enfriamiento 3 s.
            if (k.zKey.wasPressedThisFrame && Rank("GIRO") > 0 && Time.time >= nextSpin) { nextSpin = Time.time + 3; Spin(); }
            // Esquiva breve: Shift, impulso corto sin invulnerabilidad, enfriamiento 2 s.
            if ((k.leftShiftKey.wasPressedThisFrame || k.rightShiftKey.wasPressedThisFrame) && Rank("ESQUIVA") > 0 && Time.time >= nextDash)
            { nextDash = Time.time + 2; dashUntil = Time.time + .18f; dashDir = player.Facing; }
            if (Time.time < dashUntil) controller.Move(dashDir * 19f * Time.deltaTime);
            // Recuperación: 1 de vida cada 20 s tras 5 s sin recibir daño.
            if (Rank("RECUPERACION") > 0 && Time.time - lastHurt > 5 && Time.time - lastRegen > 20)
            {
                lastRegen = Time.time; var m = rules.Progress.Active;
                if (m.health < m.maxHealth) { m.health++; player.SynchronizeWorldOneState(); lastHealth = player.Health; }
            }
            if (Time.time > eyeRefresh) { eyeRefresh = Time.time + .25f; Eye(); }
        }

        void Sweep(float width, float reach, string label)
        {
            int hit = 0;
            foreach (var col in Physics.OverlapBox(transform.position + Vector3.up * .85f + player.Facing * reach * .5f, new Vector3(width * .5f, .8f, reach * .5f + .1f), Quaternion.LookRotation(player.Facing), ~0, QueryTriggerInteraction.Ignore))
            { var e = col.GetComponentInParent<AfterglowEnemy>(); if (e && !e.Defeated && e.TakeDamage(rules.StrikeDamage())) hit++; }
            M01Toast.Message(label + (hit > 0 ? " · " + hit : ""), 1.2f);
        }

        void Spin()
        {
            foreach (var col in Physics.OverlapSphere(transform.position + Vector3.up * .8f, 1.8f, ~0, QueryTriggerInteraction.Ignore))
            { var e = col.GetComponentInParent<AfterglowEnemy>(); if (e && !e.Defeated) e.TakeDamage(rules.StrikeDamage()); }
            if (player.visual) StartCoroutine(SpinVisual());
        }

        System.Collections.IEnumerator SpinVisual() { var t0 = Time.time; var v = player.visual; while (Time.time - t0 < .35f) { v.localRotation = Quaternion.Euler(0, (Time.time - t0) / .35f * 360, 0); yield return null; } v.localRotation = Quaternion.identity; }

        void Eye()
        {
            bool on = Rank("OJO") > 0;
            foreach (var e in FindObjectsByType<AfterglowEnemy>(FindObjectsSortMode.None))
            {
                var label = e.transform.Find("Vida visible");
                if (!on || e.Defeated || e.GetComponent<M01Hangzzer>() == null && e.maxHealth > 4) { if (label) label.gameObject.SetActive(false); continue; }
                if (!label)
                {
                    var t = new GameObject("Vida visible").AddComponent<TextMeshPro>(); t.transform.SetParent(e.transform, false); t.transform.localPosition = Vector3.up * 2.1f;
                    t.fontSize = 2.4f; t.alignment = TextAlignmentOptions.Center; t.color = new Color(1, .55f, .5f); t.rectTransform.sizeDelta = new Vector2(3, 1); label = t.transform;
                }
                label.gameObject.SetActive(true); label.GetComponent<TextMeshPro>().text = "Vida " + e.Health + " / " + e.maxHealth;
                if (Camera.main) label.rotation = Camera.main.transform.rotation;
            }
        }
    }
}
