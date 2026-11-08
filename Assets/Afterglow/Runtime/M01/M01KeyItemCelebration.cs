using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Afterglow.M01
{
    /// <summary>
    /// Presentación de la primera entrega de un objeto clave (GDD 0.5): pausa la acción, el personaje levanta
    /// ambas manos, la cámara se acerca, el objeto brilla sobre él con «Conseguiste [nombre]» y después vuelven
    /// el encuadre y el control. Solo la dispara M01Progress.GrantKeyItem, que no repite entregas ya registradas,
    /// así que cargar o volver a la sala no la reproduce.
    /// Icono reemplazable: Resources/KeyItems/&lt;nombre&gt;.png; si falta, se genera uno provisional.
    /// </summary>
    public sealed class M01KeyItemCelebration : MonoBehaviour
    {
        const float ZoomIn = .45f, Raise = .25f, MinHold = 1.5f, Hold = 2.4f, ZoomOut = .4f;
        static M01KeyItemCelebration instance;
        readonly Queue<string> pending = new Queue<string>();
        public static bool Playing { get; private set; }
        public static string Current { get; private set; }
        public static int Played { get; private set; }
        /// <summary>Fase actual para pruebas: Idle, ZoomIn, Hold, ZoomOut.</summary>
        public static string Phase { get; private set; } = "Idle";
        Transform leftArm, rightArm; Quaternion leftRest, rightRest; float armBlend;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Install()
        {
            if (instance) return;
            var go = new GameObject("Key item celebration");
            DontDestroyOnLoad(go);
            instance = go.AddComponent<M01KeyItemCelebration>();
        }

        void OnEnable() { M01Progress.Acquired += OnAcquired; }
        void OnDisable() { M01Progress.Acquired -= OnAcquired; Playing = false; Phase = "Idle"; }
        void OnAcquired(string name, int count, bool keyItem) { if (keyItem) { pending.Enqueue(name); if (!Playing) StartCoroutine(Run()); } }

        IEnumerator Run()
        {
            Playing = true;
            while (pending.Count > 0)
            {
                string name = pending.Dequeue();
                yield return Present(name);
            }
            Playing = false; Current = null; Phase = "Idle";
        }

        IEnumerator Present(string name)
        {
            Current = name;
            var player = FindAnyObjectByType<AfterglowPlayer>();
            var dialogue = FindAnyObjectByType<M01Dialogue>();
            var cameraRig = FindAnyObjectByType<AfterglowCamera>();
            var cam = cameraRig ? cameraRig.GetComponent<Camera>() : Camera.main;
            if (!player || !cam) yield break;

            // Pausa: reutiliza el modal del diálogo (congela tiempo, IA, botín e interacción).
            bool modal = false; float previousScale = Time.timeScale;
            for (float waited = 0; waited < 3; waited += Time.unscaledDeltaTime)
            {
                if (!dialogue) { Time.timeScale = 0; break; }
                if (dialogue.AcquireModal()) { modal = true; break; }
                yield return null;
            }
            if (dialogue && !modal) { Time.timeScale = 0; }

            leftArm = Find(player.visual, "Player_LeftArmA"); rightArm = Find(player.visual, "Player_RightArmA");
            if (leftArm) leftRest = leftArm.localRotation; if (rightArm) rightRest = rightArm.localRotation;

            var startPosition = cam.transform.position; var startRotation = cam.transform.rotation;
            float headTop = HeadTop(player);
            var focus = player.transform.position + Vector3.up * (headTop + .1f);
            var closePosition = focus - startRotation * Vector3.forward * 6.2f;

            var item = BuildItem(player, name, headTop);
            var banner = BuildBanner(name);
            var interact = new InputAction("Skip celebration", InputActionType.Button, "<Keyboard>/e"); bool skip = false;
            interact.performed += _ => skip = true; interact.Enable();

            Phase = "ZoomIn";
            for (float t = 0; t < ZoomIn; t += Time.unscaledDeltaTime)
            {
                float k = Mathf.SmoothStep(0, 1, t / ZoomIn);
                cam.transform.position = Vector3.Lerp(startPosition, closePosition, k);
                armBlend = Mathf.Clamp01(t / Raise);
                Animate(item, banner, t, Mathf.Clamp01((t - .15f) / .3f));
                yield return null;
            }
            cam.transform.position = closePosition; armBlend = 1;
            Phase = "Hold"; skip = false;
            for (float t = 0; t < Hold && !(skip && t > MinHold); t += Time.unscaledDeltaTime) { Animate(item, banner, ZoomIn + t, 1); yield return null; }
            Phase = "ZoomOut";
            for (float t = 0; t < ZoomOut; t += Time.unscaledDeltaTime)
            {
                float k = Mathf.SmoothStep(0, 1, t / ZoomOut);
                cam.transform.SetPositionAndRotation(Vector3.Lerp(closePosition, startPosition, k), startRotation);
                armBlend = 1 - k;
                Animate(item, banner, 3, 1 - k);
                yield return null;
            }
            interact.Dispose();
            cam.transform.SetPositionAndRotation(startPosition, startRotation);
            armBlend = 0; ApplyArms();
            Destroy(item.root); Destroy(banner.root);
            if (modal) dialogue.ReleaseModal(); else Time.timeScale = previousScale <= 0 ? 1 : previousScale;
            Played++;
        }

        void LateUpdate() { if (Playing) ApplyArms(); }

        void ApplyArms()
        {
            // El rig se espeja con VisualFacing, por lo que las rotaciones locales sirven para ambos sentidos.
            if (rightArm) rightArm.localRotation = Quaternion.Slerp(rightRest, Quaternion.Euler(0, 0, -128), armBlend);
            if (leftArm) leftArm.localRotation = Quaternion.Slerp(leftRest, Quaternion.Euler(0, 0, 128), armBlend);
        }

        static Transform Find(Transform root, string name)
        {
            if (!root) return null;
            foreach (var t in root.GetComponentsInChildren<Transform>(true)) if (t.name == name) return t;
            return null;
        }

        static float HeadTop(AfterglowPlayer player)
        {
            float top = 1.9f;
            foreach (var r in player.visual.GetComponentsInChildren<SpriteRenderer>()) if (r.enabled && r.name == "Player_Head") top = r.bounds.max.y - player.transform.position.y;
            return Mathf.Clamp(top, 1.2f, 3f);
        }

        sealed class Visual { public GameObject root; public Transform icon, glow; public CanvasGroup group; public RectTransform panel; }

        Visual BuildItem(AfterglowPlayer player, string name, float headTop)
        {
            var root = new GameObject("Objeto clave · " + name);
            root.transform.position = player.transform.position + Vector3.up * (headTop + .5f);
            var glow = new GameObject("Brillo", typeof(SpriteRenderer));
            glow.transform.SetParent(root.transform, false);
            var glowRenderer = glow.GetComponent<SpriteRenderer>(); glowRenderer.sprite = GlowSprite(); glowRenderer.sortingOrder = 40; glowRenderer.color = new Color(1f, .9f, .45f, .95f);
            glow.transform.localScale = Vector3.one * 1.2f;
            var icon = new GameObject("Icono", typeof(SpriteRenderer));
            icon.transform.SetParent(root.transform, false);
            var iconRenderer = icon.GetComponent<SpriteRenderer>(); iconRenderer.sprite = IconFor(player, name); iconRenderer.sortingOrder = 41;
            float size = Mathf.Max(iconRenderer.sprite.bounds.size.x, iconRenderer.sprite.bounds.size.y);
            icon.transform.localScale = Vector3.one * (.6f / Mathf.Max(.01f, size));
            return new Visual { root = root, icon = icon.transform, glow = glow.transform };
        }

        Visual BuildBanner(string name)
        {
            var root = new GameObject("Banner objeto clave", typeof(Canvas), typeof(CanvasScaler), typeof(CanvasGroup));
            var canvas = root.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 420;
            var scaler = root.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920, 1080); scaler.matchWidthOrHeight = .5f;
            var panel = new GameObject("Panel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(root.transform, false);
            var rect = (RectTransform)panel.transform; rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f); rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = new Vector2(0, -300);
            panel.GetComponent<Image>().color = new Color(.08f, .09f, .13f, .9f);
            var underline = new GameObject("Subrayado", typeof(RectTransform), typeof(Image));
            underline.transform.SetParent(panel.transform, false);
            var u = (RectTransform)underline.transform; u.anchorMin = new Vector2(0, 0); u.anchorMax = new Vector2(1, 0); u.pivot = new Vector2(.5f, 0); u.sizeDelta = new Vector2(0, 6);
            underline.GetComponent<Image>().color = new Color(1f, .79f, .32f);
            var label = new GameObject("Texto", typeof(RectTransform), typeof(TextMeshProUGUI));
            label.transform.SetParent(panel.transform, false);
            var text = label.GetComponent<TextMeshProUGUI>();
            text.text = "Conseguiste <color=#FFC94F><b>" + name + "</b></color>"; text.fontSize = 46; text.alignment = TextAlignmentOptions.Center; text.color = new Color(.97f, .95f, .9f);
            text.textWrappingMode = TextWrappingModes.NoWrap; text.raycastTarget = false;
            var l = (RectTransform)label.transform; l.anchorMin = Vector2.zero; l.anchorMax = Vector2.one; l.offsetMin = l.offsetMax = Vector2.zero;
            text.ForceMeshUpdate();
            rect.sizeDelta = new Vector2(text.preferredWidth + 90, 92);
            var group = root.GetComponent<CanvasGroup>(); group.alpha = 0;
            return new Visual { root = root, group = group, panel = rect };
        }

        static void Animate(Visual item, Visual banner, float t, float visibility)
        {
            var cam = Camera.main;
            if (cam) item.root.transform.rotation = cam.transform.rotation;
            float pop = Mathf.Clamp01(t / .3f);
            float scale = (pop < 1 ? Mathf.Lerp(.2f, 1.15f, pop) : 1 + Mathf.Sin(t * 4) * .04f) * visibility;
            item.root.transform.localScale = Vector3.one * Mathf.Max(.001f, scale);
            item.glow.localRotation = Quaternion.Euler(0, 0, t * 40);
            item.glow.localScale = Vector3.one * (1.2f + Mathf.Sin(t * 5) * .1f);
            banner.group.alpha = visibility;
            banner.panel.localScale = Vector3.one * Mathf.Lerp(.85f, 1, visibility);
        }

        static Sprite IconFor(AfterglowPlayer player, string name)
        {
            var custom = Resources.Load<Sprite>("KeyItems/" + name);
            if (custom) return custom;
            if (name == M01Progress.KeyItemName("PICO"))
            {
                var visual = player.GetComponentInChildren<M01PickaxeVisual>(true);
                if (visual && visual.sprite && visual.sprite.sprite) return visual.sprite.sprite;
            }
            return name == M01Progress.KeyItemName("TELEFONO") ? PhoneSprite() : GlowSprite();
        }

        static Sprite glowSprite, phoneSprite;

        /// <summary>Halo radial con rayos (procedural).</summary>
        static Sprite GlowSprite()
        {
            if (glowSprite) return glowSprite;
            const int size = 128; var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            var px = new Color[size * size];
            for (int y = 0; y < size; y++) for (int x = 0; x < size; x++)
                {
                    float u = (x + .5f) / size * 2 - 1, v = (y + .5f) / size * 2 - 1, r = Mathf.Sqrt(u * u + v * v), a = Mathf.Atan2(v, u);
                    float core = Mathf.Clamp01(1 - r / .55f); core *= core;
                    float rays = Mathf.Pow(Mathf.Abs(Mathf.Cos(a * 6)), 18) * Mathf.Clamp01(1 - r) * .8f;
                    float alpha = Mathf.Clamp01(core + rays);
                    px[y * size + x] = new Color(1, 1, 1, alpha);
                }
            tex.SetPixels(px); tex.Apply();
            glowSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(.5f, .5f), 100);
            return glowSprite;
        }

        /// <summary>Icono PROVISIONAL del teléfono de minas; reemplazable en Resources/KeyItems/Teléfono de minas.png.</summary>
        static Sprite PhoneSprite()
        {
            if (phoneSprite) return phoneSprite;
            const int w = 80, h = 128; var tex = new Texture2D(w, h, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
            var px = new Color[w * h]; var body = new Color(.24f, .32f, .27f); var edge = new Color(.1f, .13f, .12f); var screen = new Color(.72f, .88f, .62f); var key = new Color(.85f, .8f, .62f);
            for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
                {
                    Color c = Color.clear;
                    bool inBody = RoundRect(x, y, 6, 4, w - 6, h - 22, 10);
                    bool antenna = x >= w - 20 && x <= w - 13 && y >= h - 26 && y < h - 2;
                    if (inBody || antenna) c = RoundRect(x, y, 9, 7, w - 9, h - 25, 8) || (antenna && x > w - 19 && x < w - 14 && y < h - 4) ? body : edge;
                    if (RoundRect(x, y, 16, h - 60, w - 16, h - 32, 4)) c = screen;
                    for (int r = 0; r < 3; r++) for (int k = 0; k < 3; k++)
                        {
                            float cx = 22 + k * 18, cy = 20 + r * 14; if ((x - cx) * (x - cx) + (y - cy) * (y - cy) < 26) c = key;
                        }
                    px[y * w + x] = c;
                }
            tex.SetPixels(px); tex.Apply();
            phoneSprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(.5f, .5f), 100);
            phoneSprite.name = "Telefono_provisional";
            return phoneSprite;
        }

        static bool RoundRect(int x, int y, int x0, int y0, int x1, int y1, int r)
        {
            if (x < x0 || x >= x1 || y < y0 || y >= y1) return false;
            int dx = x < x0 + r ? x0 + r - x : x >= x1 - r ? x - (x1 - r - 1) : 0;
            int dy = y < y0 + r ? y0 + r - y : y >= y1 - r ? y - (y1 - r - 1) : 0;
            return dx * dx + dy * dy <= r * r;
        }
    }
}
