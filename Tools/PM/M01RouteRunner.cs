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
    /// <summary>Recorrido encadenado (M01FullRun): sin fixture; el estado viene del nivel anterior jugado.</summary>
    public static bool Chained;
    protected abstract string Evidence { get; }
    protected abstract string Title { get; }
    protected virtual string ReportFile => "Pruebas.md";
    protected abstract IEnumerator Route();
    protected string report; protected int failures; protected float started;
    InputSettings.BackgroundBehavior previousBackground; InputSettings.EditorInputBehaviorInPlayMode previousFocus; string previousSave;
    protected AfterglowPlayer Player => FindAnyObjectByType<AfterglowPlayer>();
    protected M01Dialogue Dialogue => FindAnyObjectByType<M01Dialogue>();
    protected M01Progress Data => M01Session.Instance.data;
    protected string SceneId => SceneManager.GetActiveScene().name;

    static void SafeWrite(string path, string text) { for (int i = 0; i < 8; i++) { try { File.WriteAllText(path, text); return; } catch (IOException) { System.Threading.Thread.Sleep(40); } } }
    protected bool Check(string label, bool ok) { if (!ok) failures++; report += $"- {(ok ? "PASS" : "FAIL")} {label}\n"; SafeWrite(Evidence + "/" + ReportFile, report); return ok; }
    protected void Note(string text) { report += text + "\n"; SafeWrite(Evidence + "/" + ReportFile, report); }
    protected void Keys(params Key[] keys) => InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState(keys.Where(k => k != Key.None).ToArray()));
    protected IEnumerator Tap(Key key) { Keys(key); yield return new WaitForSecondsRealtime(.1f); Keys(); yield return new WaitForSecondsRealtime(.14f); }
    protected IEnumerator Arrival(string id) { float end = Now + 12; while ((SceneId != id || M01Expedition.Transitioning) && Now < end) yield return null; yield return new WaitForSecondsRealtime(.45f); Check("Llegada a " + id, SceneId == id && !M01Expedition.Transitioning); }
    protected IEnumerator Speak() { int presses = 0; while (Dialogue && Dialogue.IsOpen && presses++ < 80) yield return Tap(Key.E); for (float t = 0; t < .25f || M01KeyItemCelebration.Playing; t += Time.unscaledDeltaTime) { if (t > 8) break; yield return null; } }
    protected IEnumerator Capture(string name) { yield return new WaitForSecondsRealtime(.6f); yield return new WaitForEndOfFrame(); ScreenCapture.CaptureScreenshot(Evidence + "/" + name + ".png"); yield return null; }
    /// <summary>Avanza diálogos encadenados (con pausas de escena entre ellos) hasta llegar a la escena indicada.</summary>
    protected IEnumerator SpeakUntil(string scene, float timeout = 30)
    {
        float end = Now + timeout;
        while (SceneId != scene && Now < end) { if (Dialogue && Dialogue.IsOpen) yield return Speak(); else yield return null; }
    }

    protected IEnumerator Door(string destination) { yield return Tap(Key.E); yield return Arrival(destination); }

    protected IEnumerator Walk(float x, float z, bool jump = false, float timeout = 12)
    {
        float end = Now + timeout; bool reached = false, jumpSent = !jump;
        while (Now < end && !Player.Dead)
        {
            if (Dialogue && Dialogue.IsOpen) { Keys(); yield return Speak(); end = Now + timeout; }
            var p = Player.transform.position; var d = new Vector2(x - p.x, z - p.z);
            if (d.magnitude < .25f) { reached = true; break; }
            // Como un jugador: si un enemigo al mismo nivel corta el paso, primero se le hace frente.
            var near = FindObjectsByType<AfterglowEnemy>(FindObjectsSortMode.None).Where(e => !e.Defeated && !e.stationary && Mathf.Abs(e.transform.position.y - p.y) < .8f && Vector3.Distance(e.transform.position, p) < 1.5f).OrderBy(e => Vector3.Distance(e.transform.position, p)).FirstOrDefault();
            if (near) { Keys(); yield return Fight(near, 8, false); end = Now + timeout; continue; }
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
        if (enemy && enemy.Defeated) yield return CollectLoot(enemy.transform.position);
        yield return HealIfLow();
        if (record) Check("Enemigo derrotado con el pico: " + (enemy ? enemy.name : "?") + $" (vida del jugador {Player.Health}/{Player.maxHealth})", enemy && enemy.Defeated);
    }

    /// <summary>
    /// Con 4 de vida o menos usa un consumible desde el inventario (I → Consumibles → Núcleo o Fragmento), como haría un
    /// jugador que compró en la tienda del pueblo antes de la expedición.
    /// </summary>
    protected IEnumerator HealIfLow()
    {
        var d = Data;
        if (Player.Dead || Player.Health > 4 || (d.restorers <= 0 && d.fragments <= 0) || (Dialogue && Dialogue.IsOpen)) yield break;
        yield return UseConsumable(d.restorers > 0);
    }

    /// <summary>
    /// Espera a que el texto del diálogo termine de escribirse: la primera E sobre un texto a medias solo lo completa, y el
    /// texto avanza en tiempo real, así que con el editor lento una espera fija deja la E sin elegir la opción.
    /// </summary>
    protected IEnumerator WaitTyped(float timeout = 5)
    {
        yield return null;
        for (float end = Now + timeout; Now < end && Dialogue && Dialogue.IsOpen && Dialogue.body.maxVisibleCharacters < Dialogue.body.textInfo.characterCount;) yield return null;
        yield return new WaitForSecondsRealtime(.05f);
    }

    protected IEnumerator UseConsumable(bool core)
    {
        int before = Player.Health;
        // Abre el inventario cuando el control está libre (no durante el retroceso de un golpe ni una transición).
        for (float end = Now + 3; Now < end && (Dialogue && Dialogue.IsOpen || Player.GetComponent<M01PlayerRules>().BlocksControl);) yield return null;
        yield return Tap(Key.I); yield return WaitTyped();
        if (!(Dialogue && Dialogue.IsOpen)) { Note($"- Inventario no se abrió (vida {before})"); yield break; }
        yield return Tap(Key.DownArrow); yield return Tap(Key.E); yield return WaitTyped();
        if (core) yield return Tap(Key.DownArrow);
        yield return Tap(Key.E); yield return Speak();
        Note($"- Consumible usado ({(core ? "Núcleo +5" : "Fragmento +2")}): vida {before} → {Player.Health}");
    }

    /// <summary>Recoge el botín soltado cerca (curativos incluidos), como haría un jugador tras el combate.</summary>
    protected IEnumerator CollectLoot(Vector3 around)
    {
        yield return new WaitForSecondsRealtime(.5f);
        float end = Now + 5;
        while (Now < end && !Player.Dead)
        {
            var loot = FindObjectsByType<M01Loot>(FindObjectsSortMode.None).Where(l => l && Vector3.Distance(l.transform.position, around) < 3.5f && Mathf.Abs(l.transform.position.y - Player.transform.position.y) < 1.2f).OrderBy(l => Vector3.Distance(l.transform.position, Player.transform.position)).FirstOrDefault();
            if (!loot) break;
            var d = loot.transform.position - Player.transform.position;
            Keys(Mathf.Abs(d.x) < .15f ? Key.None : d.x > 0 ? Key.RightArrow : Key.LeftArrow, Mathf.Abs(d.z) < .15f ? Key.None : d.z > 0 ? Key.UpArrow : Key.DownArrow);
            yield return null;
        }
        Keys();
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
        if (Chained) yield return Shop();
        yield return Walk(-1, -6.6f); yield return Tap(Key.E);
        yield return Arrival("M01_MAPA");
        var map = FindAnyObjectByType<M01RouteMap>(); yield return new WaitForSecondsRealtime(.5f);
        for (int guard = 0; map && map.Focused != index + 1 && guard < 4; guard++)
            foreach (var k in new[] { Key.RightArrow, Key.DownArrow, Key.UpArrow, Key.LeftArrow }) { int before = map.Focused; yield return Tap(k); if (map.Focused == index + 1) break; if (map.Focused < before || map.Focused > index + 1) yield return Tap(k == Key.RightArrow ? Key.LeftArrow : k == Key.LeftArrow ? Key.RightArrow : k == Key.UpArrow ? Key.DownArrow : Key.UpArrow); }
        Check($"Selector: N{index + 2} disponible y enfocado", map && map.IsOpen && map.Focused == index + 1 && M01RouteMap.Available(Data, index + 1));
        yield return Capture("99_Selector_siguiente");
    }

    /// <summary>
    /// Tienda del pueblo (M01_PUEBLO): con dinero ganado en la partida compra Núcleos (12, +5) hasta tener dos, como
    /// preparación de un jugador antes de la siguiente expedición. Vuelve a N01S01, junto al altar.
    /// </summary>
    /// <summary>Núcleos que el recorrido encadenado lleva como máximo: los que compraría un jugador prudente antes del jefe.</summary>
    const int ShopCores = 4;
    protected IEnumerator Shop()
    {
        var d = Data;
        if (d.money < 12 || d.restorers >= ShopCores) { Note($"- Tienda: sin compra (dinero {d.money}, núcleos {d.restorers})"); yield break; }
        int money = d.money, cores = d.restorers;
        yield return Walk(-10, -3.4f); yield return Tap(Key.E); yield return Arrival("M01_PUEBLO"); if (SceneId != "M01_PUEBLO") yield break;
        // Los tres servicios están en fila (x 9, 19, 29; z 1): se rodean por z −2,5 para no chocar ni hablar con el vendedor de ofrendas robadas.
        yield return Walk(36, -2.5f); yield return Walk(9, -2.5f); yield return Walk(9, -0.3f);
        var shop = FindObjectsByType<M01ServiceNPC>().OrderBy(n => Vector3.Distance(n.transform.position, Player.transform.position)).FirstOrDefault();
        if (!Check("Frente a la tienda (servicio más cercano)", shop && shop.service == M01Service.Shop)) yield break;
        while (d.money >= 12 && d.restorers < ShopCores)
        {
            int before = d.restorers;
            yield return Tap(Key.E); if (!(Dialogue && Dialogue.IsOpen && Dialogue.CurrentId == "PUE.12")) { yield return Speak(); break; }
            while (Dialogue && Dialogue.IsOpen && Dialogue.CurrentId == "PUE.12") yield return Tap(Key.E);
            yield return WaitTyped(); yield return Tap(Key.DownArrow); yield return Tap(Key.E); yield return Speak();
            if (d.restorers == before) break;
        }
        Check($"Tienda del pueblo: núcleos {cores} → {d.restorers}, dinero {money} → {d.money}", d.restorers > cores);
        yield return Walk(9, -2.5f); yield return Walk(38.6f, -2.5f); 
        // Puerta de paso (radio plano 1,05 m, sin E) en (40, 0): camina hacia su centro solo hasta que empieza la transición.
        for (float end = Now + 6; SceneId == "M01_PUEBLO" && !M01Expedition.Transitioning && Now < end;)
        { var p = Player.transform.position; Keys(Mathf.Abs(40 - p.x) < .15f ? Key.None : p.x < 40 ? Key.RightArrow : Key.LeftArrow, Mathf.Abs(p.z) < .15f ? Key.None : p.z < 0 ? Key.UpArrow : Key.DownArrow); yield return null; }
        Keys(); yield return Arrival("N01S01");
    }

    /// <summary>Fixture declarado: niveles anteriores completados por sus propias pruebas.</summary>
    protected void CompleteBefore(int level)
    {
        if (Chained) { Note("Encadenado: sin fixture; estado heredado del nivel anterior."); return; }
        var d = Data;
        foreach (var e in new[] { "P01.LETTER", "P01.BELL", "P02.CAPTAIN", "M01.WORLD_STARTED", "M01.N01_ENTERED", "N1A.INTRO", "N1C.PHONE", "N1B.PICKAXE" }) if (!d.events.Contains(e)) d.events.Add(e);
        d.pickaxeOwned = d.phoneOwned = true;
        for (int i = 0; i < level - 1; i++) { d.investigated[i] = true; d.contributions[i] = M01Progress.Quotas[i]; d.opened[i] = true; }
        d.lastMapNode = level;
        // Compras previstas en la tienda del pueblo con el dinero de los niveles anteriores: un Núcleo (12) por nivel completado.
        if (d.restorers < level - 1) d.restorers = level - 1;
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
        // Encadenado: la vida no se recupera entre niveles; al entrar, un jugador prudente usa Núcleos si le faltan 5 o más.
        if (Chained) for (int guard = 0; guard < 3 && Player.Health <= Player.maxHealth - 5 && Data.restorers > 0; guard++) yield return UseConsumable(true);
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
        report += $"\nFinalizado. Fallos: {failures}\n"; SafeWrite(Evidence + "/" + ReportFile, report);
        Destroy(gameObject);
    }

    void OnDestroy() { if (Keyboard.current != null) Keys(); InputSystem.settings.backgroundBehavior = previousBackground; InputSystem.settings.editorInputBehaviorInPlayMode = previousFocus; Time.maximumDeltaTime = previousMaxDelta; }
}
