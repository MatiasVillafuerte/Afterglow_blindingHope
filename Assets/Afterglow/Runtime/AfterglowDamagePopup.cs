using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Afterglow
{
    /// <summary>
    /// Cifra de daño sobre quien recibe un impacto efectivo (GDD 0.5, referencia DañoCausado.png):
    /// estallido rojo irregular, número claro y grueso con borde oscuro; entra con rebote, se lee y se retira.
    /// </summary>
    public sealed class AfterglowDamagePopup : MonoBehaviour
    {
        const float Pop = .16f, Settle = .26f, Hold = .75f, End = 1.0f;
        static Canvas canvas;
        static Sprite burst;
        Transform target; Vector3 offset; RectTransform rect; CanvasGroup group; float born; float lift;

        public static int Active { get; private set; }

        /// <summary>Muestra la vida efectivamente perdida. Ignora cantidades nulas.</summary>
        public static void Spawn(Transform target, int amount)
        {
            if (amount <= 0 || !target) return;
            EnsureCanvas();
            var go = new GameObject("Daño " + amount, typeof(RectTransform), typeof(CanvasGroup));
            go.transform.SetParent(canvas.transform, false);
            var popup = go.AddComponent<AfterglowDamagePopup>();
            popup.target = target;
            popup.offset = Vector3.up * (TopOf(target) + .25f);
            popup.rect = (RectTransform)go.transform;
            popup.rect.sizeDelta = new Vector2(120, 120);
            popup.group = go.GetComponent<CanvasGroup>();
            popup.born = Time.time;
            // Golpes seguidos sobre el mismo objetivo se apilan hacia arriba para seguir legibles.
            foreach (var other in canvas.GetComponentsInChildren<AfterglowDamagePopup>())
                if (other != popup && other.target == target) popup.lift += 46;

            var image = new GameObject("Estallido", typeof(RectTransform), typeof(Image));
            image.transform.SetParent(go.transform, false);
            var img = image.GetComponent<Image>(); img.sprite = BurstSprite(); img.raycastTarget = false;
            ((RectTransform)image.transform).sizeDelta = new Vector2(120, 120);

            var label = new GameObject("Cifra", typeof(RectTransform), typeof(TextMeshProUGUI));
            label.transform.SetParent(go.transform, false);
            var text = label.GetComponent<TextMeshProUGUI>();
            ((RectTransform)label.transform).sizeDelta = new Vector2(120, 120);
            text.text = amount.ToString();
            text.fontSize = 60; text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            text.color = new Color(.86f, .89f, .96f);
            text.raycastTarget = false;
            text.outlineWidth = .32f;
            text.outlineColor = new Color32(24, 27, 42, 255);
            Active++;
            popup.Place(1);
        }

        static float TopOf(Transform target)
        {
            float top = 1.7f; bool found = false;
            foreach (var r in target.GetComponentsInChildren<Renderer>())
            {
                if (r is ParticleSystemRenderer || !r.enabled) continue;
                float h = r.bounds.max.y - target.position.y;
                if (!found || h > top) { top = h; found = true; }
            }
            return Mathf.Clamp(top, .8f, 3f);
        }

        static void EnsureCanvas()
        {
            if (canvas) return;
            var go = new GameObject("Damage popups", typeof(Canvas), typeof(CanvasScaler));
            DontDestroyOnLoad(go);
            canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 400;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = .5f;
        }

        /// <summary>Estallido de 11 puntas irregulares generado una sola vez (relleno rojo y borde oscuro).</summary>
        static Sprite BurstSprite()
        {
            if (burst) return burst;
            const int size = 160; const int points = 11;
            var radii = new float[points * 2]; var angles = new float[points * 2];
            var random = new System.Random(5);
            for (int i = 0; i < points * 2; i++)
            {
                angles[i] = (i + (float)(random.NextDouble() - .5) * .35f) / (points * 2) * Mathf.PI * 2;
                radii[i] = i % 2 == 0 ? .86f + (float)random.NextDouble() * .12f : .58f + (float)random.NextDouble() * .08f;
            }
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
            var fill = new Color(.89f, .17f, .21f); var edge = new Color(.42f, .06f, .09f);
            var pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float u = (x + .5f) / size * 2 - 1, v = (y + .5f) / size * 2 - 1;
                    float r = Mathf.Sqrt(u * u + v * v), a = Mathf.Atan2(v, u); if (a < 0) a += Mathf.PI * 2;
                    float boundary = Boundary(a, angles, radii);
                    float d = (boundary - r) * size * .5f; // píxeles hasta el borde
                    float alpha = Mathf.Clamp01(d + .5f);
                    var c = Color.Lerp(edge, fill, Mathf.Clamp01((d - 3f) / 1.5f));
                    c.a = alpha; pixels[y * size + x] = c;
                }
            texture.SetPixels(pixels); texture.Apply();
            burst = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(.5f, .5f), 100);
            return burst;
        }

        static float Boundary(float angle, float[] angles, float[] radii)
        {
            int n = angles.Length;
            for (int i = 0; i < n; i++)
            {
                float a0 = angles[i], a1 = i + 1 < n ? angles[i + 1] : angles[0] + Mathf.PI * 2;
                float test = angle < angles[0] ? angle + Mathf.PI * 2 : angle;
                if (test >= a0 && test <= a1)
                {
                    // Intersección del rayo con el segmento entre dos vértices.
                    var p0 = new Vector2(Mathf.Cos(a0), Mathf.Sin(a0)) * radii[i];
                    var p1 = new Vector2(Mathf.Cos(a1), Mathf.Sin(a1)) * radii[(i + 1) % n];
                    var dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                    var e = p1 - p0; float den = dir.x * e.y - dir.y * e.x;
                    if (Mathf.Abs(den) < 1e-5f) return radii[i];
                    return (p0.x * e.y - p0.y * e.x) / den;
                }
            }
            return radii[0];
        }

        void Place(float scale)
        {
            var cam = Camera.main;
            if (!cam || !target) { group.alpha = 0; return; }
            var screen = cam.WorldToScreenPoint(target.position + offset);
            if (screen.z <= 0) { group.alpha = 0; return; }
            var canvasRect = (RectTransform)canvas.transform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screen, null, out var local);
            rect.anchoredPosition = local + Vector2.up * lift;
            rect.localScale = Vector3.one * scale;
        }

        void Update()
        {
            float t = Time.time - born;
            if (t >= End) { Destroy(gameObject); return; }
            float scale = t < Pop ? Mathf.Lerp(.35f, 1.22f, t / Pop) : t < Settle ? Mathf.Lerp(1.22f, 1f, (t - Pop) / (Settle - Pop)) : 1f;
            if (t > Hold) lift += 70 * Time.deltaTime;
            Place(scale);
            group.alpha = t > Hold ? 1 - (t - Hold) / (End - Hold) : 1;
        }

        void OnDestroy() { Active = Mathf.Max(0, Active - 1); }
    }
}
