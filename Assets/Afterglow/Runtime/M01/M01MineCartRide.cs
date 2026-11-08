using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Afterglow.M01
{
    /// <summary>
    /// Minijuego de vagoneta (N5). La vagoneta avanza sola a 7 m/s por tramos de riel; [Espacio] salta y [Abajo] frena al
    /// 70 % durante 1,5 s como máximo (2 s de recuperación). Los minerales suspendidos se recogen por contacto. Caer o chocar
    /// con un obstáculo reinicia el recorrido desde el inicio con el contador del intento a cero. Al llegar, el intento pasa al
    /// minero del andén (N05S03), que cuenta los minerales. Durante la conducción no hay teléfono, guardado ni ataque.
    /// </summary>
    public sealed class M01MineCartRide : MonoBehaviour
    {
        public float speed = 7f, brakeFactor = .7f, brakeMax = 1.5f, brakeRecovery = 2f, jumpHeight = 1.6f, gravity = 25f, endX = 168f, killDepth = 4f;
        public Vector3 start = new Vector3(2, 0, 0);
        public Transform cart; public List<Transform> minerals = new List<Transform>(); public List<Transform> obstacles = new List<Transform>();
        public Transform package; public string packageItem = "Q08.PAQUETE";
        public string arrivalScene = "N05S03"; public Vector3 arrival = new Vector3(-6, .05f, -4);
        public int Collected { get; private set; }
        public int Attempts { get; private set; }
        public static int LastAttemptMinerals = -1;
        AfterglowPlayer player; M01PlayerRules rules; float vy, brakeUsed, brakeCooldown; bool grounded, finished, braking; TMP_Text counter; Vector3 seat = new Vector3(0, .55f, 0);
        InputAction jump, brake; bool jumpRequested;
        readonly HashSet<Transform> taken = new HashSet<Transform>();

        void OnEnable() { jump = new InputAction("Cart jump", InputActionType.Button, "<Keyboard>/space"); jump.performed += _ => jumpRequested = true; jump.Enable(); brake = new InputAction("Cart brake", InputActionType.Button, "<Keyboard>/downArrow"); brake.Enable(); }
        void OnDisable() { jump?.Dispose(); brake?.Dispose(); if (rules) rules.scriptedControl = false; }

        void Start()
        {
            player = FindAnyObjectByType<AfterglowPlayer>(); rules = player.GetComponent<M01PlayerRules>();
            var canvas = new GameObject("Contador de minerales", typeof(Canvas), typeof(UnityEngine.UI.CanvasScaler)); canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay; canvas.GetComponent<Canvas>().sortingOrder = 320;
            var scaler = canvas.GetComponent<UnityEngine.UI.CanvasScaler>(); scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920, 1080);
            counter = new GameObject("Texto", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>(); counter.transform.SetParent(canvas.transform, false);
            var r = (RectTransform)counter.transform; r.anchorMin = r.anchorMax = new Vector2(1, 1); r.pivot = new Vector2(1, 1); r.anchoredPosition = new Vector2(-40, -200); r.sizeDelta = new Vector2(520, 60);
            counter.fontSize = 38; counter.alignment = TextAlignmentOptions.MidlineRight; counter.color = new Color(1f, .93f, .75f); counter.fontStyle = FontStyles.Bold;
            var d = M01Session.Instance.data; if (package && d.questItems.Contains(packageItem)) package.gameObject.SetActive(false);
            Restart(false);
        }

        void Restart(bool fell)
        {
            if (fell) { Attempts++; M01Toast.Message("Caíste. La vagoneta vuelve al inicio", 2.4f); }
            Collected = 0; taken.Clear(); foreach (var m in minerals) if (m) m.gameObject.SetActive(true);
            cart.position = start; vy = 0; grounded = true; brakeUsed = 0; brakeCooldown = 0; finished = false;
            Seat();
            var cam = FindAnyObjectByType<AfterglowCamera>(); if (cam) cam.Snap();
        }

        void Seat()
        {
            rules.scriptedControl = true;
            var cc = player.GetComponent<CharacterController>(); cc.enabled = false;
            player.transform.position = cart.position + seat; player.RestoreWorldOneFacing(Vector3.right);
        }

        void Update()
        {
            counter.text = "Minerales: " + Collected;
            if (finished || Time.timeScale == 0) { jumpRequested = false; return; }
            float dt = Time.deltaTime;
            braking = brake.IsPressed() && brakeUsed < brakeMax && brakeCooldown <= 0;
            if (braking) brakeUsed += dt; else if (brakeUsed > 0) { brakeCooldown = brakeRecovery; brakeUsed = 0; }
            if (brakeCooldown > 0) brakeCooldown -= dt;
            var p = cart.position; p.x += speed * (braking ? brakeFactor : 1) * dt;
            if (jumpRequested && grounded) { vy = Mathf.Sqrt(2 * gravity * jumpHeight); grounded = false; }
            jumpRequested = false;
            vy -= gravity * dt; p.y += vy * dt;
            // Riel bajo las ruedas: colliders de la capa 0 (tramos del montaje).
            if (vy <= 0 && Physics.Raycast(new Vector3(p.x, p.y + .6f, p.z), Vector3.down, out var hit, .7f, 1 << 0, QueryTriggerInteraction.Ignore))
            { p.y = hit.point.y; vy = 0; grounded = true; }
            else grounded = false;
            cart.position = p; player.transform.position = p + seat;
            foreach (var m in minerals) if (m && m.gameObject.activeSelf && !taken.Contains(m) && Vector3.Distance(m.position, p + Vector3.up * 1.0f) < 1.1f) { taken.Add(m); m.gameObject.SetActive(false); Collected++; }
            if (package && package.gameObject.activeSelf && Vector3.Distance(package.position, p + Vector3.up) < 1.2f)
            {
                package.gameObject.SetActive(false); var data = M01Session.Instance.data;
                if (!data.questItems.Contains(packageItem)) { data.questItems.Add(packageItem); M01Progress.Announce("Paquete de la vía"); }
            }
            foreach (var o in obstacles) if (o && Mathf.Abs(o.position.x - p.x) < .7f && p.y < o.position.y + .55f) { Restart(true); return; }
            if (p.y < start.y - killDepth) { Restart(true); return; }
            if (p.x >= endX) Finish();
        }

        void Finish()
        {
            finished = true; LastAttemptMinerals = Collected; rules.scriptedControl = false;
            player.GetComponent<CharacterController>().enabled = true;
            M01Expedition.Travel(arrivalScene, arrival, fade: true);
        }
    }
}
