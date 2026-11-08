using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Afterglow.M01
{
    /// <summary>
    /// Hangzzer (GDD 7.2, N2): 4 de vida, cuelga de un farol. Al ver al jugador baja, simula silbar y llama
    /// Resisworm desde la boca del túnel; luego vuelve a su posición y repite tras un intervalo configurable.
    /// Solo puede recibir golpes mientras está abajo. Los refuerzos no dan botín (no se puede cultivar EXP).
    /// Parámetros abiertos del GDD (intervalo y límite de refuerzos) quedan expuestos para pruebas.
    /// </summary>
    [RequireComponent(typeof(AfterglowEnemy))]
    public sealed class M01Hangzzer : MonoBehaviour
    {
        public Vector3 anchor, ground, tunnel;
        public float detect = 6.5f, interval = 6f, descend = .9f, whistle = 1.3f, exposed = 2.6f;
        public int maxAlive = 2, maxPerCycle = 2, maxTotal = 4;
        public GameObject reinforcementPrefab;
        public Material lootMaterial;
        public SpriteRenderer body;
        public Sprite hanging, whistling;
        public Transform wire;
        public string State { get; private set; } = "Colgado";
        /// <summary>Altura de la figura colgada (pivote en el cable): baja hasta que los pies tocan el suelo.</summary>
        float HangHeight => hanging ? hanging.bounds.size.y : 1.4f;
        public int Summoned { get; private set; }
        AfterglowEnemy enemy; Transform player; readonly List<AfterglowEnemy> alive = new List<AfterglowEnemy>();
        float nextCycle; AudioSource audioSource; AudioClip whistleClip;

        void Awake()
        {
            enemy = GetComponent<AfterglowEnemy>(); enemy.stationary = true;
            audioSource = gameObject.AddComponent<AudioSource>(); audioSource.spatialBlend = .6f;
        }

        IEnumerator Start()
        {
            player = FindAnyObjectByType<AfterglowPlayer>()?.transform;
            yield return null;
            if (enemy.Defeated) yield break;
            Place(anchor); Show(hanging);
            while (!enemy.Defeated)
            {
                if (Time.timeScale == 0 || !player || Time.time < nextCycle || Vector3.Distance(Flat(player.position), Flat(ground)) > detect) { Sway(); yield return null; continue; }
                State = "Bajando";
                var low = ground + Vector3.up * HangHeight;
                for (float t = 0; t < descend; t += Time.deltaTime) { Place(Vector3.Lerp(anchor, low, Mathf.SmoothStep(0, 1, t / descend))); yield return null; }
                Place(ground); State = "Silbando"; Show(whistling); Whistle();
                yield return new WaitForSeconds(whistle * .45f);
                Call();
                yield return new WaitForSeconds(whistle * .55f);
                State = "Expuesto";
                yield return new WaitForSeconds(exposed);
                if (enemy.Defeated) break;
                State = "Subiendo"; Show(hanging); var from = ground + Vector3.up * HangHeight;
                for (float t = 0; t < descend; t += Time.deltaTime) { Place(Vector3.Lerp(from, anchor, Mathf.SmoothStep(0, 1, t / descend))); yield return null; }
                Place(anchor); State = "Colgado"; nextCycle = Time.time + interval;
            }
            State = "Derrotado";
            if (wire) wire.gameObject.SetActive(false);
        }

        static Vector3 Flat(Vector3 v) { v.y = 0; return v; }

        void Place(Vector3 p)
        {
            transform.position = p;
            Physics.SyncTransforms();
            if (wire) { var top = anchor + Vector3.up * 1.2f; wire.position = (top + p + Vector3.up * 1.3f) / 2; wire.localScale = new Vector3(.05f, Mathf.Max(.05f, top.y - p.y - 1.3f), .05f); }
        }

        void Sway() { if (body && State == "Colgado") body.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * 1.6f) * 6); }

        void Show(Sprite s) { if (body && s) { body.sprite = s; body.transform.localRotation = Quaternion.identity; } }

        void Call()
        {
            alive.RemoveAll(e => !e || e.Defeated);
            int calls = Mathf.Min(Mathf.Min(maxPerCycle, maxAlive - alive.Count), maxTotal - Summoned);
            for (int i = 0; i < calls && reinforcementPrefab; i++)
            {
                var go = Instantiate(reinforcementPrefab, tunnel + new Vector3((i - .5f) * 1.2f, 0, 0), Quaternion.identity);
                go.name = "N02.REFUERZO." + (++Summoned);
                var worm = go.GetComponent<AfterglowEnemy>(); worm.noReward = true; worm.detection = 14;
                alive.Add(worm);
            }
            if (calls > 0) M01Toast.Message("¡Hangzzer llama refuerzos!", 2f);
        }

        void Whistle()
        {
            if (!whistleClip)
            {
                const int rate = 22050; var samples = new float[(int)(rate * .8f)];
                for (int i = 0; i < samples.Length; i++) { float t = (float)i / rate; float f = 1500 + Mathf.Sin(t * 9) * 380 + t * 600; samples[i] = Mathf.Sin(t * f * Mathf.PI * 2) * .12f * Mathf.Clamp01(t * 12) * Mathf.Clamp01((.8f - t) * 8); }
                whistleClip = AudioClip.Create("Silbido Hangzzer", samples.Length, 1, rate, false); whistleClip.SetData(samples, 0);
            }
            audioSource.PlayOneShot(whistleClip);
        }
    }
}
