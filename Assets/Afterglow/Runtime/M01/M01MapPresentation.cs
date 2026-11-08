using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Afterglow.M01
{
    /// <summary>
    /// Presentación del selector de Mundo 1 sobre la ilustración Mapa_Mundo1_Concepto (Resources/UI/Mapa_Mundo1).
    /// Conexiones según Mapa_Mundo1_Funcional: Acceso → N1 → N2 → N3 → N4 → N5 → N6.
    /// Distingue dónde está el jugador, el nodo enfocado, los disponibles, los investigados y los cerrados,
    /// y detalla qué falta para abrir la siguiente mina. La lógica de foco y entrada sigue en M01RouteMap.
    /// </summary>
    public sealed class M01MapPresentation : MonoBehaviour
    {
        // Posiciones de las losas pintadas en la ilustración (píxeles de 1672 × 941, origen arriba a la izquierda).
        static readonly Vector2[] Plates = { new Vector2(727, 790), new Vector2(1392, 677), new Vector2(873, 494), new Vector2(261, 326), new Vector2(1027, 281), new Vector2(1460, 229) };
        static readonly Vector2 Access = new Vector2(378, 742);
        const float ImageW = 1440, ImageH = 810, SourceW = 1672, SourceH = 941;

        Canvas canvas; RectTransform image; GameObject root;
        readonly Image[] rings = new Image[6], fills = new Image[6], locks = new Image[6], checks = new Image[6];
        readonly TMP_Text[] numbers = new TMP_Text[6], labels = new TMP_Text[6];
        readonly Image[] routes = new Image[6];
        RectTransform cursor, here; TMP_Text title, state, requirement, instructions;
        static Sprite circle, padlock, check, white;
        int focused;

        public Vector2 NodePosition(int i) => ToCanvas(Plates[i]);
        static Vector2 ToCanvas(Vector2 p) => new Vector2(p.x / SourceW * ImageW - ImageW / 2, ImageH / 2 - p.y / SourceH * ImageH);
        public bool Visible => root && root.activeSelf;

        public void Build()
        {
            if (root) return;
            root = new GameObject("Selector Mundo 1", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.transform.SetParent(transform, false);
            canvas = root.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 300;
            var scaler = root.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920, 1080); scaler.matchWidthOrHeight = .5f;
            var backdrop = Panel("Fondo", root.transform, Vector2.zero, new Vector2(1920, 1080), new Color(.13f, .09f, .07f, 1));
            backdrop.anchorMin = Vector2.zero; backdrop.anchorMax = Vector2.one; backdrop.sizeDelta = Vector2.zero;

            var art = Resources.Load<Sprite>("UI/Mapa_Mundo1");
            image = Panel("Ilustración", root.transform, new Vector2(0, 120), new Vector2(ImageW, ImageH), Color.white);
            image.GetComponent<Image>().sprite = art; image.GetComponent<Image>().preserveAspect = true;
            var frame = image.gameObject.AddComponent<Outline>(); frame.effectColor = new Color(.35f, .22f, .1f); frame.effectDistance = new Vector2(5, -5);

            var titleBar = Panel("Título", image, new Vector2(0, ImageH / 2 - 34), new Vector2(560, 56), new Color(.16f, .1f, .06f, .86f));
            Text("Texto", titleBar, Vector2.zero, new Vector2(540, 56), 32, new Color(1f, .86f, .52f), "MUNDO 1 · VALLE MINERO", FontStyles.Bold);

            // Tramos rectos entre nodos: oro si el tramo está abierto, gris si no (complementan el camino pintado).
            for (int i = 0; i < 6; i++)
            {
                Vector2 a = i == 0 ? ToCanvas(Access) : ToCanvas(Plates[i - 1]), b = ToCanvas(Plates[i]);
                var seg = Panel("Tramo " + i, image, (a + b) / 2, new Vector2(Vector2.Distance(a, b) - 80, 4), Color.white);
                seg.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(b.y - a.y, b.x - a.x) * Mathf.Rad2Deg);
                routes[i] = seg.GetComponent<Image>();
            }

            here = Panel("Estás aquí", image, ToCanvas(Access) + new Vector2(0, 66), new Vector2(190, 46), new Color(.98f, .95f, .86f, .95f));
            Text("Texto", here, Vector2.zero, new Vector2(186, 46), 21, new Color(.25f, .15f, .08f), "Estás aquí · Acceso", FontStyles.Bold);

            for (int i = 0; i < 6; i++)
            {
                var p = ToCanvas(Plates[i]);
                var ring = Panel("Nodo N" + (i + 1), image, p, new Vector2(78, 78), Color.white); ring.GetComponent<Image>().sprite = Circle(); rings[i] = ring.GetComponent<Image>();
                var fill = Panel("Relleno", ring, Vector2.zero, new Vector2(62, 62), Color.white); fill.GetComponent<Image>().sprite = Circle(); fills[i] = fill.GetComponent<Image>();
                numbers[i] = Text("Número", ring, Vector2.zero, new Vector2(70, 70), 36, Color.white, (i + 1).ToString(), FontStyles.Bold);
                var lk = Panel("Candado", ring, new Vector2(30, 30), new Vector2(30, 34), Color.white); lk.GetComponent<Image>().sprite = Padlock(); locks[i] = lk.GetComponent<Image>();
                var ck = Panel("Investigado", ring, new Vector2(30, 30), new Vector2(34, 34), Color.white); ck.GetComponent<Image>().sprite = Check(); checks[i] = ck.GetComponent<Image>();
                var plate = Panel("Rótulo", image, p + new Vector2(0, -62), new Vector2(210, 34), new Color(.14f, .09f, .05f, .82f));
                labels[i] = Text("Texto", plate, Vector2.zero, new Vector2(206, 34), 19, new Color(.98f, .93f, .8f), M01RouteMap.Names[i], FontStyles.Bold);
            }
            cursor = Panel("Cursor", image, Vector2.zero, new Vector2(36, 30), new Color(1f, .78f, .25f));
            Text("Flecha", cursor, Vector2.zero, new Vector2(40, 40), 34, new Color(1f, .78f, .25f), "▼", FontStyles.Bold);
            cursor.GetComponent<Image>().color = Color.clear;

            var info = Panel("Información", root.transform, new Vector2(0, -445), new Vector2(ImageW, 170), new Color(.98f, .95f, .88f, 1));
            var infoOutline = info.gameObject.AddComponent<Outline>(); infoOutline.effectColor = new Color(.45f, .3f, .15f); infoOutline.effectDistance = new Vector2(3, -3);
            title = Text("Nivel", info, new Vector2(-180, 50), new Vector2(1040, 46), 34, new Color(.22f, .13f, .07f), "", FontStyles.Bold, TextAlignmentOptions.MidlineLeft);
            state = Text("Estado", info, new Vector2(-180, 8), new Vector2(1040, 36), 25, new Color(.3f, .22f, .15f), "", FontStyles.Normal, TextAlignmentOptions.MidlineLeft);
            requirement = Text("Requisito", info, new Vector2(-180, -32), new Vector2(1040, 36), 25, new Color(.3f, .22f, .15f), "", FontStyles.Normal, TextAlignmentOptions.MidlineLeft);
            instructions = Text("Controles", info, new Vector2(520, -50), new Vector2(380, 60), 22, new Color(.42f, .32f, .22f), "", FontStyles.Normal, TextAlignmentOptions.MidlineRight);
            var legend = Text("Leyenda", info, new Vector2(520, 30), new Vector2(380, 70), 19, new Color(.42f, .32f, .22f),
                "<color=#E9B43C>●</color> Elegido   <color=#3E9E6E>●</color> Disponible\n<color=#2F7D57>✓</color> Investigado   <color=#7A7470>●</color> Cerrado", FontStyles.Normal, TextAlignmentOptions.MidlineRight);
            legend.richText = true;
            root.SetActive(false);
        }

        public void Show(M01Progress data, int focus, bool canReturn) { Build(); root.SetActive(true); Refresh(data, focus, canReturn, null); }
        public void Hide() { if (root) root.SetActive(false); }

        public void Refresh(M01Progress data, int focus, bool canReturn, string message)
        {
            if (!root) return;
            focused = focus;
            for (int i = 0; i < 6; i++)
            {
                bool available = M01RouteMap.Available(data, i), investigated = i < 5 && data.investigated[i];
                Color fillColor = investigated ? new Color(.18f, .49f, .34f) : available ? new Color(.24f, .62f, .43f) : new Color(.48f, .46f, .44f);
                fills[i].color = fillColor;
                rings[i].color = i == focus ? new Color(1f, .78f, .25f) : available ? new Color(.98f, .95f, .86f) : new Color(.3f, .28f, .27f);
                rings[i].rectTransform.sizeDelta = Vector2.one * (i == focus ? 92 : 78);
                numbers[i].text = (i + 1).ToString();
                checks[i].enabled = investigated;
                locks[i].enabled = !available;
                labels[i].color = available ? new Color(.98f, .93f, .8f) : new Color(.72f, .68f, .62f);
                routes[i].color = available ? new Color(1f, .82f, .3f, .55f) : new Color(.3f, .28f, .27f, .35f);
            }
            title.text = "N" + (focus + 1) + " · " + M01RouteMap.Names[focus];
            bool open = M01RouteMap.Available(data, focus);
            if (focus < 5)
                state.text = (open ? "<color=#2F7D57><b>Disponible</b></color>" : "<color=#8A3A2A><b>Cerrado</b></color>") +
                    "   ·   Investigación de N" + (focus + 1) + ": " + (data.investigated[focus] ? "<color=#2F7D57>completa</color>" : "pendiente") +
                    "   ·   Aporte para abrir N" + (focus + 2) + ": " + data.contributions[focus] + " / " + M01Progress.Quotas[focus];
            else state.text = (open ? "<color=#2F7D57><b>Disponible</b></color>" : "<color=#8A3A2A><b>Cerrado</b></color>") + "   ·   Rescate y batalla final. No tiene cuota posterior.";
            if (focus == 0) requirement.text = open ? "Entrada al refugio con el altar compartido." : "Completa el encargo del capitán.";
            else
            {
                int p = focus - 1;
                string Mark(bool ok) => ok ? "<color=#2F7D57><b>[listo]</b></color>" : "<color=#B0402C><b>[falta]</b></color>";
                requirement.text = open ? (M01RouteMap.HasScene(focus) ? "Entrada lista desde el acceso." : "<color=#8A5A1A>Entrada en producción.</color>")
                    : "Para abrir: " + Mark(data.investigated[p]) + " investigar el final de N" + focus + "   " + Mark(data.contributions[p] >= M01Progress.Quotas[p]) + " aporte de N" + focus + " en el altar (" + data.contributions[p] + " / " + M01Progress.Quotas[p] + ")";
            }
            if (!string.IsNullOrEmpty(message)) requirement.text = message;
            instructions.text = "Flechas · elegir     [E] entrar" + (canReturn ? "\n[Esc] volver al acceso" : "");
        }

        void Update()
        {
            if (!Visible) return;
            var p = ToCanvas(Plates[focused]);
            cursor.anchoredPosition = p + new Vector2(0, 66 + Mathf.Sin(Time.unscaledTime * 5) * 6);
            float pulse = 1 + Mathf.Sin(Time.unscaledTime * 4) * .05f;
            rings[focused].rectTransform.localScale = Vector3.one * pulse;
            here.localScale = Vector3.one * (1 + Mathf.Sin(Time.unscaledTime * 2.5f) * .03f);
        }

        static RectTransform Panel(string name, Transform parent, Vector2 pos, Vector2 size, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var r = (RectTransform)go.transform; r.anchoredPosition = pos; r.sizeDelta = size;
            var img = go.GetComponent<Image>(); img.color = color; img.raycastTarget = false; if (!white) white = null;
            return r;
        }

        static TMP_Text Text(string name, Transform parent, Vector2 pos, Vector2 size, float fontSize, Color color, string text, FontStyles style = FontStyles.Normal, TextAlignmentOptions align = TextAlignmentOptions.Center)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            var r = (RectTransform)go.transform; r.anchoredPosition = pos; r.sizeDelta = size;
            var t = go.GetComponent<TextMeshProUGUI>(); t.text = text; t.fontSize = fontSize; t.color = color; t.fontStyle = style; t.alignment = align; t.raycastTarget = false;
            t.textWrappingMode = TextWrappingModes.NoWrap; t.richText = true;
            return t;
        }

        static Sprite Circle()
        {
            if (circle) return circle;
            const int s = 128; var tex = new Texture2D(s, s, TextureFormat.RGBA32, false); var px = new Color[s * s];
            for (int y = 0; y < s; y++) for (int x = 0; x < s; x++) { float d = Vector2.Distance(new Vector2(x + .5f, y + .5f), new Vector2(s / 2f, s / 2f)); px[y * s + x] = new Color(1, 1, 1, Mathf.Clamp01(s / 2f - 1 - d)); }
            tex.SetPixels(px); tex.Apply(); circle = Sprite.Create(tex, new Rect(0, 0, s, s), new Vector2(.5f, .5f)); return circle;
        }

        static Sprite Check()
        {
            if (check) return check;
            const int s = 48; var tex = new Texture2D(s, s, TextureFormat.RGBA32, false); var px = new Color[s * s];
            var green = new Color(.18f, .55f, .36f);
            for (int y = 0; y < s; y++) for (int x = 0; x < s; x++)
                {
                    float d = Vector2.Distance(new Vector2(x + .5f, y + .5f), new Vector2(s / 2f, s / 2f));
                    Color c = d < s / 2f - 1 ? (d > s / 2f - 5 ? Color.white : green) : Color.clear;
                    // Trazo de verificación: dos segmentos gruesos.
                    float a = DistSeg(new Vector2(x, y), new Vector2(13, 25), new Vector2(21, 16)), b = DistSeg(new Vector2(x, y), new Vector2(21, 16), new Vector2(35, 33));
                    if (Mathf.Min(a, b) < 3.2f && d < s / 2f - 4) c = Color.white;
                    px[y * s + x] = c;
                }
            tex.SetPixels(px); tex.Apply(); check = Sprite.Create(tex, new Rect(0, 0, s, s), new Vector2(.5f, .5f)); return check;
        }

        static float DistSeg(Vector2 p, Vector2 a, Vector2 b) { var ab = b - a; float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / ab.sqrMagnitude); return Vector2.Distance(p, a + ab * t); }

        static Sprite Padlock()
        {
            if (padlock) return padlock;
            const int w = 48, h = 56; var tex = new Texture2D(w, h, TextureFormat.RGBA32, false); var px = new Color[w * h];
            var dark = new Color(.25f, .22f, .2f); var light = new Color(.86f, .82f, .76f);
            for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
                {
                    Color c = Color.clear;
                    if (x >= 4 && x < w - 4 && y >= 2 && y < 30) c = (x < 7 || x >= w - 7 || y < 5 || y >= 27) ? dark : light;
                    float dx = x - w / 2f, dy = y - 30; float r = Mathf.Sqrt(dx * dx + dy * dy);
                    if (y >= 28 && r >= 11 && r <= 17) c = dark;
                    if (x >= w / 2 - 2 && x <= w / 2 + 1 && y >= 10 && y <= 20) c = dark;
                    px[y * w + x] = c;
                }
            tex.SetPixels(px); tex.Apply(); padlock = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(.5f, .5f)); return padlock;
        }
    }
}
