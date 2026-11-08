using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Afterglow.M01
{
    /// <summary>
    /// Avisos breves que no pausan (GDD 0.5 · presentación de adquisiciones comunes y timbre P01.04).
    /// Se apilan en la esquina superior derecha bajo el HUD; un aviso igual que sigue visible se agrupa como «×N».
    /// Escucha las adquisiciones confirmadas por M01Progress; los objetos clave los presenta M01KeyItemCelebration.
    /// </summary>
    public sealed class M01Toast : MonoBehaviour
    {
        const float Life = 2.6f, FadeIn = .15f, FadeOut = .35f, Spacing = 58;
        const int MaxVisible = 5;
        sealed class Entry { public string key, label; public int count; public float born; public RectTransform rect; public CanvasGroup group; public TMP_Text text; public float y; public float life; public bool centered; }
        static M01Toast instance;
        readonly List<Entry> entries = new List<Entry>();
        RectTransform root;

        public static int Visible => instance ? instance.entries.Count : 0;
        public static string Describe() { if (!instance) return ""; var parts = new List<string>(); foreach (var e in instance.entries) parts.Add(e.text.text); return string.Join(" | ", parts); }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Install()
        {
            if (instance) return;
            var go = new GameObject("Acquisition toasts", typeof(Canvas), typeof(CanvasScaler));
            DontDestroyOnLoad(go);
            var canvas = go.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 350;
            var scaler = go.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920, 1080); scaler.matchWidthOrHeight = .5f;
            instance = go.AddComponent<M01Toast>();
            instance.root = (RectTransform)go.transform;
        }

        void OnEnable() { M01Progress.Acquired += OnAcquired; }
        void OnDisable() { M01Progress.Acquired -= OnAcquired; }

        void OnAcquired(string name, int count, bool keyItem) { if (!keyItem) Push("Conseguiste " + name, name, count, Life); }

        /// <summary>Aviso libre (por ejemplo, el timbre). No agrupa con adquisiciones.</summary>
        public static void Message(string text, float seconds = 3.2f)
        {
            if (!instance) Install();
            instance.Push(text, "MSG:" + text, 0, seconds, true);
        }

        void Push(string label, string key, int count, float life, bool centered = false)
        {
            var existing = entries.Find(e => e.key == key && Time.unscaledTime - e.born < e.life - FadeOut);
            if (existing != null && count > 0)
            {
                existing.count += count; existing.born = Time.unscaledTime - FadeIn; existing.rect.localScale = Vector3.one * 1.12f;
                existing.text.text = Format(existing.label, existing.count); return;
            }
            var go = new GameObject("Aviso", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            go.transform.SetParent(root, false);
            var rect = (RectTransform)go.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(centered ? .5f : 1, 1); rect.pivot = new Vector2(centered ? .5f : 1, 1);
            var background = go.GetComponent<Image>(); background.color = new Color(.08f, .09f, .13f, .86f); background.raycastTarget = false;
            var textGo = new GameObject("Texto", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGo.transform.SetParent(go.transform, false);
            var text = textGo.GetComponent<TextMeshProUGUI>();
            text.fontSize = centered ? 40 : 30; text.color = new Color(.97f, .95f, .9f); text.alignment = centered ? TextAlignmentOptions.Center : TextAlignmentOptions.MidlineRight; text.raycastTarget = false;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            var textRect = (RectTransform)textGo.transform; textRect.anchorMin = Vector2.zero; textRect.anchorMax = Vector2.one; textRect.offsetMin = new Vector2(22, 0); textRect.offsetMax = new Vector2(-22, 0);
            var entry = new Entry { centered = centered, key = key, label = label, count = count, born = Time.unscaledTime, rect = rect, group = go.GetComponent<CanvasGroup>(), text = text, life = life };
            text.text = Format(label, count);
            text.ForceMeshUpdate();
            rect.sizeDelta = new Vector2(text.preferredWidth + 44, centered ? 68 : 50);
            entries.Insert(0, entry);
            while (entries.Count > MaxVisible) { Destroy(entries[entries.Count - 1].rect.gameObject); entries.RemoveAt(entries.Count - 1); }
            Layout(true);
        }

        static string Format(string label, int count) => count > 1 ? label + "  <color=#FFC94F>×" + count + "</color>" : label;

        void Layout(bool snapNew)
        {
            int side = 0, middle = 0;
            for (int i = 0; i < entries.Count; i++)
            {
                float target = entries[i].centered ? -330 - (middle++) * 80 : -292 - (side++) * Spacing;
                if (snapNew && i == 0) entries[i].y = target + 18;
                entries[i].y = Mathf.Lerp(entries[i].y, target, snapNew && i == 0 ? 0 : 1 - Mathf.Exp(-Time.unscaledDeltaTime * 14));
            }
        }

        void Update()
        {
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                var e = entries[i]; float t = Time.unscaledTime - e.born;
                if (t >= e.life) { Destroy(e.rect.gameObject); entries.RemoveAt(i); continue; }
                e.group.alpha = t < FadeIn ? t / FadeIn : t > e.life - FadeOut ? (e.life - t) / FadeOut : 1;
                e.rect.localScale = Vector3.Lerp(e.rect.localScale, Vector3.one, 1 - Mathf.Exp(-Time.unscaledDeltaTime * 12));
                if (e.text.text != Format(e.label, e.count)) e.text.text = Format(e.label, e.count);
                e.text.ForceMeshUpdate();
                e.rect.sizeDelta = new Vector2(e.text.preferredWidth + (e.centered ? 70 : 44), e.centered ? 68 : 50);
            }
            Layout(false);
            for (int i = 0; i < entries.Count; i++)
                entries[i].rect.anchoredPosition = new Vector2((entries[i].centered ? 0 : -28) + (Time.unscaledTime - entries[i].born < FadeIn ? (1 - (Time.unscaledTime - entries[i].born) / FadeIn) * 40 : 0), entries[i].y);
        }
    }
}
