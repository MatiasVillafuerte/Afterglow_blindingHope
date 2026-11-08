using System.Collections;
using System.IO;
using System.Linq;
using Afterglow;
using Afterglow.M01;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

/// <summary>
/// Base de los recorridos de nivel en Play Mode (2026-11-08). Teclado simulado mediante el Input System (misma ruta que
/// las teclas reales). Cada nivel declara su fixture (niveles anteriores completados) y su ruta.
/// </summary>
public abstract class M01RouteRunner : MonoBehaviour
{
    protected abstract string Evidence { get; }
    protected abstract string Title { get; }
    protected abstract IEnumerator Route();
    protected string report; protected int failures; protected float started;
    InputSettings.BackgroundBehavior previousBackground; InputSettings.EditorInputBehaviorInPlayMode previousFocus; string previousSave;
    protected AfterglowPlayer Player => FindAnyObjectByType<AfterglowPlayer>();
    protected M01Dialogue Dialogue => FindAnyObjectByType<M01Dialogue>();
    protected M01Progress Data => M01Session.Instance.data;
    protected string SceneId => SceneManager.GetActiveScene().name;

    static void SafeWrite(string path, string text) { for (int i = 0; i < 8; i++) { try { File.WriteAllText(path, text); return; } catch (IOException) { System.Threading.Thread.Sleep(40); } } }
    protected bool Check(string label, bool ok) { if (!ok) failures++; report += $"- {(ok ? "PASS" : "FAIL")} {label}\n"; SafeWrite(Evidence + "/Pruebas.md", report); return ok; }
    protected void Note(string text) { report += text + "\n"; SafeWrite(Evidence + "/Pruebas.md", report); }
    protected void Keys(params Key[] keys) => InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState(keys.Where(k => k != Key.None).ToArray()));
    protected IEnumerator Tap(Key key) { Keys(key); yield return new WaitForSecondsRealtime(.1f); Keys(); yield return new WaitForSecondsRealtime(.14f); }
    protected IEnumerator Arrival(string id) { float end = Now + 12; while ((SceneId != id || M01Expedition.Transitioning) && Now < end) yield return null; yield return new WaitForSecondsRealtime(.45f); Check("Llegada a " + id, SceneId == id && !M01Expedition.Transitioning); }
    protected IEnumerator Speak() { int presses = 0; while (Dialogue && Dialogue.IsOpen && presses++ < 80) yield return Tap(Key.E); for (float t = 0; t < .25f || M01KeyItemCelebration.Playing; t += Time.unscaledDeltaTime) { if (t > 8) break; yield return null; } }
    protected IEnumerator Capture(string name) { yield return new WaitForSecondsRealtime(.6f); yield return new WaitForEndOfFrame(); ScreenCapture.CaptureScreenshot(Evidence + "/" + name + ".png"); yield return null; }
    protected IEnumerator Door(string destination) { yield return Tap(Key.E); yield return Arrival(destination); }

    protected IEnumerator Walk(float x, float z, bool jump = false, float timeout = 12)
    {
        float end = Now + timeout; bool reached = false, jumpSent = !jump;
        while (Now < end && !Player.Dead)
        {
            if (Dialogue && Dialogue.IsOpen) { Keys(); yield return Speak(); end = Now + timeout; }
            var p = Player.transform.position; var d = new Vector2(x - p.x, z - p.z);
            if (d.magnitude < .25f) { reached = true; break; }
            Key h = Mathf.Abs(d.x) < .15f ? Key.None : d.x > 0 ? Key.RightArrow : Key.LeftArrow;
            Key v = Mathf.Abs(d.y) < .15f ? Key.None : d.y > 0 ? Key.UpArrow : Key.DownArrow;
            if (!jumpSent && Player.Grounded) { Keys(h, v, Key.Space); jumpSent = true; } else Keys(h, v);
            yield return null;
        }
        Keys(); float settle = Now + 2.5f; while (!Player.Grounded && Now < settle) yield return null;
        yield return new WaitForSecondsRealtime(.08f);
        Check($"Paso {SceneId} ({x}, {z}) a altura {Player.transform.position.y:F2}" + (reached ? "" : $" · quedó en {Player.transform.position} · escala {Time.timeScale} · bloqueo {(Player.GetComponent<M01PlayerRules>() ? Player.GetComponent<M01PlayerRules>().BlocksControl : false)} · diálogo {(Dialogue && Dialogue.IsOpen)}"), reached);
    }

    protected IEnumerator Face(Key direction) { Keys(direction); yield return new WaitForSecondsRealtime(.06f); Keys(); yield return new WaitForSecondsRealtime(.05f); }

    protected IEnumerator Fight(AfterglowEnemy enemy, float seconds = 20, bool record = true)
    {
        float end = Now + seconds;
        while (enemy && !enemy.Defeated && !Player.Dead && Now < end)
        {
            if (Dialogue && Dialogue.IsOpen) { Keys(); yield return Speak(); continue; }
            var d = enemy.transform.position - Player.transform.position; var flat = new Vector2(d.x, d.z);
            if (flat.magnitude > 1.1f || Mathf.Abs(d.y) > .8f)
            {
                Key h = Mathf.Abs(flat.x) < .2f ? Key.None : flat.x > 0 ? Key.RightArrow : Key.LeftArrow;
                Key v = Mathf.Abs(flat.y) < .2f ? Key.None : flat.y > 0 ? Key.UpArrow : Key.DownArrow;
                Keys(h, v); yield return null; continue;
            }
            // Apunta con una o dos flechas según el ángulo: el volumen mide 0,8 m de ancho.
            var aim = flat.normalized; Key ah = Mathf.Abs(aim.x) < .38f ? Key.None : aim.x > 0 ? Key.RightArrow : Key.LeftArrow, av = Mathf.Abs(aim.y) < .38f ? Key.None : aim.y > 0 ? Key.UpArrow : Key.DownArrow;
            Keys(ah, av, Key.X);
            yield return new WaitForSecondsRealtime(.06f); Keys(); yield return new WaitForSecondsRealtime(.6f);
        }
        Keys();
        if (record) Check("Enemigo derrotado con el pico: " + (enemy ? enemy.name : "?") + $" (vida del jugador {Player.Health}/{Player.maxHealth})", enemy && enemy.Defeated);
    }

    protected AfterglowEnemy Enemy(string id) => FindObjectsByType<AfterglowEnemy>(FindObjectsSortMode.None).FirstOrDefault(e => e.name == id);

    /// <summary>Teléfono → refugio → altar compartido → selector: comprueba la apertura del nivel siguiente.</summary>
    protected IEnumerator PayAndOpen(int index, int quota)
    {
        int legit = Data.offerings.Count(o => !o.stolen);
        Check($"Ofrendas legítimas para la cuota {quota}: {legit}", legit >= quota - Data.contributions[index]);
        yield return Tap(Key.T); yield return Speak();
        yield return Arrival("N01S01"); if (SceneId != "N01S01") yield break;
        Check("El teléfono conserva el nivel de la expedición", Data.currentLevel == index + 1);
        yield return Walk(4, -4.4f); yield return Tap(Key.E); yield return Speak();
        float deadline = Now + 10; while (!Data.opened[index] && Now < deadline) { if (Dialogue && Dialogue.IsOpen) yield return Speak(); yield return null; }
        yield return Speak();
        Check($"Altar compartido: cuota {quota} cubierta y nivel siguiente abierto", Data.contributions[index] >= quota && Data.opened[index]);
        yield return Walk(-1, -6.6f); yield return Tap(Key.E);
        yield return Arrival("M01_MAPA");
        var map = FindAnyObjectByType<M01RouteMap>(); yield return new WaitForSecondsRealtime(.5f);
        for (int guard = 0; map && map.Focused != index + 1 && guard < 4; guard++)
            foreach (var k in new[] { Key.RightArrow, Key.DownArrow, Key.UpArrow, Key.LeftArrow }) { int before = map.Focused; yield return Tap(k); if (map.Focused == index + 1) break; if (map.Focused < before || map.Focused > index + 1) yield return Tap(k == Key.RightArrow ? Key.LeftArrow : k == Key.LeftArrow ? Key.RightArrow : k == Key.UpArrow ? Key.DownArrow : Key.UpArrow); }
        Check($"Selector: N{index + 2} disponible y enfocado", map && map.IsOpen && map.Focused == index + 1 && M01RouteMap.Available(Data, index + 1));
        yield return Capture("99_Selector_siguiente");
    }

    /// <summary>Fixture declarado: niveles anteriores completados por sus propias pruebas.</summary>
    protected void CompleteBefore(int level)
    {
        var d = Data;
        foreach (var e in new[] { "P01.LETTER", "P01.BELL", "P02.CAPTAIN", "M01.WORLD_STARTED", "M01.N01_ENTERED", "N1A.INTRO", "N1C.PHONE", "N1B.PICKAXE" }) if (!d.events.Contains(e)) d.events.Add(e);
        d.pickaxeOwned = d.phoneOwned = true;
        for (int i = 0; i < level - 1; i++) { d.investigated[i] = true; d.contributions[i] = M01Progress.Quotas[i]; d.opened[i] = true; }
        d.lastMapNode = level;
    }

    protected IEnumerator EnterFromMap(int level)
    {
        yield return new WaitForSecondsRealtime(.6f);
        var map = FindAnyObjectByType<M01RouteMap>(); if (map && !map.IsOpen) map.Open();
        yield return new WaitForSecondsRealtime(.3f);
        Check($"Selector abierto con N{level} enfocado y escena disponible", map && map.IsOpen && map.Focused == level - 1 && M01RouteMap.HasScene(level - 1));
        yield return Capture("00_Selector");
        yield return Tap(Key.E);
        yield return Arrival(M01RouteMap.EntryScene(level - 1));
        started = Now;
    }

    // Reloj de juego para los plazos: un fotograma lento del editor cuenta como un paso de simulación, no como segundos reales.
    float clock; protected float Now => clock;
    void LateUpdate() { clock += Time.timeScale > 0 ? Time.deltaTime / Time.timeScale : Mathf.Min(Time.unscaledDeltaTime, Time.maximumDeltaTime); }
    float previousMaxDelta = .3333f;
    IEnumerator Start()
    {
        Directory.CreateDirectory(Evidence); report = "# " + Title + "\n\nTeclado simulado (Input System). Fixture: niveles anteriores completados por sus pruebas.\n\n";
        previousBackground = InputSystem.settings.backgroundBehavior; previousFocus = InputSystem.settings.editorInputBehaviorInPlayMode;
        InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
        // Ayuda de prueba: pasos de simulación cortos para que un fotograma lento del editor no arrastre al jugador más allá del punto de parada.
        previousMaxDelta = Time.maximumDeltaTime; Time.maximumDeltaTime = 1f / 30f;
        InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        previousSave = File.Exists(M01Save.Path) ? File.ReadAllText(M01Save.Path) : null;
        yield return Route();
        Check("Guardado personal intacto", previousSave == (File.Exists(M01Save.Path) ? File.ReadAllText(M01Save.Path) : null));
        report += $"\nFinalizado. Fallos: {failures}\n"; SafeWrite(Evidence + "/Pruebas.md", report);
        Destroy(gameObject);
    }

    void OnDestroy() { if (Keyboard.current != null) Keys(); InputSystem.settings.backgroundBehavior = previousBackground; InputSystem.settings.editorInputBehaviorInPlayMode = previousFocus; Time.maximumDeltaTime = previousMaxDelta; }
}
