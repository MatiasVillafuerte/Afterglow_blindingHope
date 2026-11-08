using System;
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
/// Recorrido de N2 en Play Mode (2026-11-08). Teclado simulado por el Input System (misma ruta que las teclas reales,
/// como N1PlayChecks). Fixture declarado: N1 ya completado (investigación, aporte 3 y N2 abierto), pico y teléfono.
/// Recorre: selector → N2 → Platicio y regalo → Q03 y Q04 → desvío alto (hueco, rocas, caja) → cruce (carros, reja,
/// palanca) → patio (Hangzzer y refuerzos, segundo carro) → galería (puente, ascenso, investigación) → teléfono →
/// altar (aporte 5) → apertura de N3 en el selector.
/// </summary>
public sealed class N2PlayChecks : MonoBehaviour
{
    const string Evidence = "Docs/Implementacion/Niveles/N2_Evidencias";
    string report = "# Recorrido N2 en Play Mode\n\nTeclado simulado (Input System). Fixture: N1 completado, pico y teléfono.\n\n"; int failures;
    InputSettings.BackgroundBehavior previousBackground; InputSettings.EditorInputBehaviorInPlayMode previousFocus;
    AfterglowPlayer Player => FindAnyObjectByType<AfterglowPlayer>();
    M01Dialogue Dialogue => FindAnyObjectByType<M01Dialogue>();
    M01Progress Data => M01Session.Instance.data;
    string SceneId => SceneManager.GetActiveScene().name;
    float started;

    static void SafeWrite(string path, string text) { for (int i = 0; i < 8; i++) { try { File.WriteAllText(path, text); return; } catch (IOException) { System.Threading.Thread.Sleep(40); } } }
    public static string Main() { var g = new GameObject("N2 route checks"); DontDestroyOnLoad(g); g.AddComponent<N2PlayChecks>(); return "Recorrido N2 iniciado"; }
    bool Check(string label, bool ok) { if (!ok) failures++; report += $"- {(ok ? "PASS" : "FAIL")} {label}\n"; SafeWrite(Evidence + "/Pruebas.md", report); return ok; }
    void Keys(params Key[] keys) => InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState(keys.Where(k => k != Key.None).ToArray()));
    IEnumerator Tap(Key key) { Keys(key); yield return new WaitForSecondsRealtime(.1f); Keys(); yield return new WaitForSecondsRealtime(.14f); }
    IEnumerator Arrival(string id) { float end = Time.realtimeSinceStartup + 12; while ((SceneId != id || M01Expedition.Transitioning) && Time.realtimeSinceStartup < end) yield return null; yield return new WaitForSecondsRealtime(.45f); Check("Llegada a " + id, SceneId == id && !M01Expedition.Transitioning); }
    IEnumerator Speak() { int presses = 0; while (Dialogue && Dialogue.IsOpen && presses++ < 80) yield return Tap(Key.E); for (float t = 0; t < .25f || M01KeyItemCelebration.Playing; t += Time.unscaledDeltaTime) { if (t > 8) break; yield return null; } }
    IEnumerator Capture(string name) { yield return new WaitForSecondsRealtime(.6f); yield return new WaitForEndOfFrame(); ScreenCapture.CaptureScreenshot(Evidence + "/" + name + ".png"); yield return null; }

    IEnumerator Walk(float x, float z, bool jump = false, float timeout = 12)
    {
        float end = Time.realtimeSinceStartup + timeout; bool reached = false, jumpSent = !jump;
        while (Time.realtimeSinceStartup < end && !Player.Dead)
        {
            if (Dialogue && Dialogue.IsOpen) { Keys(); yield return Speak(); end = Time.realtimeSinceStartup + timeout; }
            var p = Player.transform.position; var d = new Vector2(x - p.x, z - p.z);
            if (d.magnitude < .25f) { reached = true; break; }
            Key h = Mathf.Abs(d.x) < .15f ? Key.None : d.x > 0 ? Key.RightArrow : Key.LeftArrow;
            Key v = Mathf.Abs(d.y) < .15f ? Key.None : d.y > 0 ? Key.UpArrow : Key.DownArrow;
            if (!jumpSent && Player.Grounded) { Keys(h, v, Key.Space); jumpSent = true; } else Keys(h, v);
            yield return null;
        }
        Keys(); float settle = Time.realtimeSinceStartup + 2.5f; while (!Player.Grounded && Time.realtimeSinceStartup < settle) yield return null;
        yield return new WaitForSecondsRealtime(.08f);
        Check($"Paso {SceneId} ({x}, {z}) a altura {Player.transform.position.y:F2}", reached);
    }

    IEnumerator Face(Key direction) { Keys(direction); yield return new WaitForSecondsRealtime(.06f); Keys(); yield return new WaitForSecondsRealtime(.05f); }

    IEnumerator Fight(AfterglowEnemy enemy, float seconds = 20, bool record = true)
    {
        float end = Time.realtimeSinceStartup + seconds;
        while (enemy && !enemy.Defeated && !Player.Dead && Time.realtimeSinceStartup < end)
        {
            if (Dialogue && Dialogue.IsOpen) { Keys(); yield return Speak(); continue; }
            var d = enemy.transform.position - Player.transform.position; var flat = new Vector2(d.x, d.z);
            if (flat.magnitude > 1.1f || Mathf.Abs(d.y) > .8f)
            {
                Key h = Mathf.Abs(flat.x) < .2f ? Key.None : flat.x > 0 ? Key.RightArrow : Key.LeftArrow;
                Key v = Mathf.Abs(flat.y) < .2f ? Key.None : flat.y > 0 ? Key.UpArrow : Key.DownArrow;
                Keys(h, v); yield return null; continue;
            }
            Keys(Mathf.Abs(flat.x) >= Mathf.Abs(flat.y) ? (flat.x > 0 ? Key.RightArrow : Key.LeftArrow) : (flat.y > 0 ? Key.UpArrow : Key.DownArrow), Key.X);
            yield return new WaitForSecondsRealtime(.06f); Keys(); yield return new WaitForSecondsRealtime(.6f);
        }
        Keys();
        if (record) Check("Enemigo derrotado con el pico: " + (enemy ? enemy.name : "?"), enemy && enemy.Defeated);
    }

    IEnumerator PushCart(M01RailCart cart, bool right, float seconds = 16)
    {
        float end = Time.realtimeSinceStartup + seconds;
        while (cart && !cart.OnPlate && Time.realtimeSinceStartup < end) { if (Dialogue && Dialogue.IsOpen) { Keys(); yield return Speak(); end = Time.realtimeSinceStartup + seconds; } Keys(right ? Key.RightArrow : Key.LeftArrow); yield return null; }
        Keys(); yield return new WaitForSecondsRealtime(.2f);
    }

    M01RailCart Cart(string id) => FindObjectsByType<M01RailCart>().FirstOrDefault(c => c.cartId == id);
    AfterglowEnemy Enemy(string id) => FindObjectsByType<AfterglowEnemy>().FirstOrDefault(e => e.name == id);

    IEnumerator Start()
    {
        Directory.CreateDirectory(Evidence);
        previousBackground = InputSystem.settings.backgroundBehavior; previousFocus = InputSystem.settings.editorInputBehaviorInPlayMode;
        InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
        InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        string previousSave = File.Exists(M01Save.Path) ? File.ReadAllText(M01Save.Path) : null;

        // Fixture declarado: N1 terminado según su propia prueba (N1PlayChecks).
        var d = Data;
        foreach (var e in new[] { "P01.LETTER", "P01.BELL", "P02.CAPTAIN", "M01.WORLD_STARTED", "M01.N01_ENTERED", "N1A.INTRO", "N1C.PHONE", "N1B.PICKAXE" }) if (!d.events.Contains(e)) d.events.Add(e);
        d.pickaxeOwned = d.phoneOwned = true; d.investigated[0] = true; d.contributions[0] = 3; d.opened[0] = true; d.lastMapNode = 1;
        yield return new WaitForSecondsRealtime(.6f);
        var map = FindAnyObjectByType<M01RouteMap>();
        if (map && !map.IsOpen) map.Open();
        yield return new WaitForSecondsRealtime(.3f);
        Check("Selector abierto con N1 enfocado y N2 disponible", map && map.IsOpen && map.Focused == 0 && M01RouteMap.Available(d, 1) && M01RouteMap.HasScene(1));
        yield return Tap(Key.RightArrow);
        Check("Foco pasa a N2", map.Focused == 1);
        yield return Capture("00_Selector_N2");
        yield return Tap(Key.E);
        yield return Arrival("N02S01"); if (SceneId != "N02S01") yield break;
        started = Time.realtimeSinceStartup;
        Check("Nivel actual 2 y expedición activa", d.currentLevel == 2 && d.expeditionActive);
        yield return Capture("01_Anden_entrada");

        // Andén: Platicio (orientación y regalo único) y encargos.
        yield return Walk(-6.5f, -3.4f);
        yield return Speak();
        Check("Platicio orienta y evalúa un regalo único", d.events.Contains("N2.INTRO") && d.events.Any(e => e.StartsWith("PLATICIO.REGALO.N2.")));
        yield return Walk(5, -3.5f); yield return Tap(Key.E); yield return Speak();
        Check("Q03 aceptado", d.Quest("Q03").active);
        yield return Walk(-0.5f, -4.4f); yield return Tap(Key.E); yield return Speak();
        Check("Q04 aceptado", d.Quest("Q04").active);

        // Desvío alto por la cornisa: cajas como peldaño.
        yield return Walk(-2.8f, -0.8f); yield return Walk(-2.8f, 0.4f, true); yield return Walk(-2.8f, 2.2f, true);
        yield return Walk(-9.5f, 3.2f);
        Check("Ofrenda de la cornisa del andén", d.collectedStaticIds.Contains("EXP.N02S01.0"));
        yield return Walk(-10, 4.0f); yield return Tap(Key.E);
        yield return Arrival("N02S04"); if (SceneId != "N02S04") yield break;
        yield return Capture("02_Desvio_entrada");
        yield return Walk(-4.2f, -3.5f, true); yield return Walk(-0.4f, -1.8f, true); yield return Walk(2.6f, -1.6f);
        yield return Walk(5.6f, -1.6f, true);
        Check("Salto del hueco de 1,5 m hasta la cornisa lejana", Player.transform.position.y > 1.2f && Player.transform.position.x > 4.5f);
        var rival = Enemy("N02.RESISTANCE.2"); if (rival) yield return Fight(rival);
        foreach (var x in new[] { 6.3f, 7.5f, 8.7f })
        {
            yield return Walk(x, 1.1f); yield return Face(Key.UpArrow);
            yield return Tap(Key.X); yield return new WaitForSecondsRealtime(.7f);
        }
        Check("Tres rocas agrietadas rotas", d.events.Contains("N02.S04.ROCA.0") && d.events.Contains("N02.S04.ROCA.1") && d.events.Contains("N02.S04.ROCA.2"));
        yield return Walk(6.4f, 3.3f); yield return Walk(8.3f, 3.3f);
        Check("Caja de herramientas y ofrenda del nicho", d.questItems.Contains("Q04.CAJA") && d.collectedStaticIds.Contains("EXP.N02S04.0"));
        yield return Capture("03_Desvio_nicho");
        yield return Walk(5.2f, -1.6f); yield return Walk(2.2f, -1.6f, true); yield return Walk(-2.8f, -3.5f); yield return Walk(-7.5f, -4.5f);
        yield return Walk(-8.5f, -5.2f); yield return Tap(Key.E);
        yield return Arrival("N02S01"); if (SceneId != "N02S01") yield break;
        Check("Regreso del desvío a la cornisa del andén", Player.transform.position.y > 1.2f);
        yield return Walk(-6, 0.4f); yield return Walk(-0.5f, -4.4f); yield return Tap(Key.E); yield return Speak();
        Check("Q04 entregado: 1 ofrenda y 10 de dinero", d.Quest("Q04").paid && !d.questItems.Contains("Q04.CAJA"));

        // Cruce de rieles.
        yield return Walk(9.8f, 0); yield return Walk(9.8f, 2.2f); yield return Tap(Key.E);
        yield return Arrival("N02S02"); if (SceneId != "N02S02") yield break;
        yield return Capture("04_Cruce_entrada");
        foreach (var id in new[] { "N02.RESISTANCE.0", "N02.RESISTANCE.1" }) { var e = Enemy(id); if (e) yield return Fight(e); }
        yield return Walk(-8.2f, -1); yield return Walk(-8.2f, 0.5f);
        yield return PushCart(Cart("CARRO_REJA"), true);
        yield return new WaitForSecondsRealtime(1f);
        var gate = FindAnyObjectByType<M01Gate>();
        Check("Carro sobre la placa abre la reja", d.events.Contains("N02.S02.REJA_ABIERTA") && gate && !gate.blocker.enabled);
        yield return Capture("05_Cruce_reja_abierta");
        yield return Walk(-4, -2.5f); yield return Walk(-4, -4.5f);
        yield return PushCart(Cart("CARRO_Q03_1"), true);
        Check("Carro marcado enganchado en su apartadero (Q03 1/2)", d.events.Contains("Q03.CARRO1") && Cart("CARRO_Q03_1").Latched);
        yield return Walk(-8.4f, -1.4f); yield return Tap(Key.E); yield return Speak();
        Check("Palanca de topes: aviso N2.03 y la reja sigue abierta", d.events.Contains("N2.LEVER_HINT") && !gate.blocker.enabled);
        yield return Walk(-3, 0.3f); yield return Walk(-3, 1.4f, true); yield return Walk(-3, 2.8f, true); yield return Walk(-8.5f, 4);
        Check("Ofrenda de la cornisa del cruce", d.collectedStaticIds.Contains("EXP.N02S02.0"));
        yield return Walk(-8.5f, 1.2f); yield return Walk(9, -2); yield return Walk(12.4f, -2); yield return Walk(13, -0.9f); yield return Tap(Key.E);
        yield return Arrival("N02S03"); if (SceneId != "N02S03") yield break;
        yield return Capture("06_Patio_entrada");

        // Patio del farol: Hangzzer.
        var hang = FindAnyObjectByType<M01Hangzzer>(); var hangEnemy = hang.GetComponent<AfterglowEnemy>();
        float until = Time.realtimeSinceStartup + 70; bool sawDescent = false, sawCall = false, captured = false;
        yield return Walk(-2, -3);
        while (!hangEnemy.Defeated && !Player.Dead && Time.realtimeSinceStartup < until)
        {
            sawDescent |= hang.State == "Bajando" || hang.State == "Silbando"; sawCall |= hang.Summoned > 0;
            if (sawCall && !captured && hang.State == "Expuesto") { captured = true; StartCoroutine(Capture("07_Patio_Hangzzer_refuerzos")); }
            var worm = FindObjectsByType<AfterglowEnemy>().Where(e => !e.Defeated && e != hangEnemy && Vector3.Distance(e.transform.position, Player.transform.position) < 3.5f).OrderBy(e => Vector3.Distance(e.transform.position, Player.transform.position)).FirstOrDefault();
            if (worm) { yield return Fight(worm, 6, false); continue; }
            if (hang.State == "Silbando" || hang.State == "Expuesto") { yield return Fight(hangEnemy, 3, false); continue; }
            var p = Player.transform.position; var target = new Vector2(hang.ground.x - 1.1f - p.x, hang.ground.z - p.z);
            Keys(target.magnitude < .3f ? Key.None : target.x > 0 ? Key.RightArrow : Key.LeftArrow, Mathf.Abs(target.y) < .2f ? Key.None : target.y > 0 ? Key.UpArrow : Key.DownArrow);
            yield return null;
        }
        Keys();
        Check("Hangzzer baja del farol y silba", sawDescent);
        Check("Hangzzer llama refuerzos desde el túnel", sawCall);
        Check("Hangzzer derrotado (4 de vida) al golpearlo abajo", hangEnemy.Defeated);
        foreach (var e in FindObjectsByType<AfterglowEnemy>().Where(e => !e.Defeated)) yield return Fight(e, 10);
        yield return Walk(-10.4f, -0.2f); yield return Walk(-10.4f, 2.6f, true); yield return Walk(-11, 3.8f);
        Check("Ofrenda del extremo del carril alto", d.collectedStaticIds.Contains("EXP.N02S03.0"));
        yield return Walk(-9.9f, 3);
        yield return PushCart(Cart("CARRO_Q03_2"), true);
        Check("Segundo carro marcado en su apartadero (Q03 2/2)", d.events.Contains("Q03.CARRO2"));
        yield return Walk(-0.3f, 0.6f); yield return Walk(8.4f, -5.5f); yield return Walk(9.2f, -5.5f, true); yield return Walk(11, -5.5f, true);
        Check("Ofrenda de la torre de cajas", d.collectedStaticIds.Contains("EXP.N02S03.1"));
        yield return Walk(9.8f, -3.5f); yield return Walk(10, -0.3f); yield return Walk(10, 2.4f, true); yield return Walk(10, 4.1f); yield return Tap(Key.E);
        yield return Arrival("N02S05"); if (SceneId != "N02S05") yield break;
        yield return Capture("08_Galeria_entrada");

        // Galería de la polea: puente, ascenso e investigación.
        yield return Walk(3.2f, -3.6f); yield return Tap(Key.E); yield return new WaitForSecondsRealtime(1.5f);
        Check("Palanca baja el puente", d.events.Contains("N02.S05.PUENTE"));
        yield return Walk(2, -1.6f); yield return Walk(2, 4.2f);
        var last = Enemy("N02.RESISTANCE.3"); if (last) yield return Fight(last);
        yield return Walk(8.5f, 5.5f);
        Check("Ofrenda de la orilla norte", d.collectedStaticIds.Contains("EXP.N02S05.0"));
        yield return Walk(-4, 6.4f); yield return Walk(-4, 8, true); yield return Walk(-4, 10, true); yield return Walk(-2.5f, 11.8f, true);
        yield return Capture("09_Galeria_derrumbe");
        yield return Walk(-1.6f, 11.8f); yield return Tap(Key.E); yield return Speak();
        Check("Investigación de N2 registrada", d.investigated[1]);
        report += $"\nRuta N2 automatizada hasta investigar: {Time.realtimeSinceStartup - started:F1} s (incluye diálogos, capturas y combates).\n";

        // Teléfono, altar compartido y apertura de N3.
        int before = d.offerings.Count(o => !o.stolen);
        Check("Ofrendas legítimas suficientes para la cuota 5: " + before, before >= 5);
        yield return Tap(Key.T); yield return Speak();
        yield return Arrival("N01S01"); if (SceneId != "N01S01") yield break;
        Check("El teléfono vuelve al refugio sin cambiar el nivel de la expedición", d.currentLevel == 2);
        yield return Walk(4, -4.4f); yield return Tap(Key.E); yield return Speak();
        float deadline = Time.realtimeSinceStartup + 10; while (!d.opened[1] && Time.realtimeSinceStartup < deadline) { if (Dialogue && Dialogue.IsOpen) yield return Speak(); yield return null; }
        yield return Speak();
        Check("Altar compartido acredita 5 en la cuota de N2 y abre N3", d.contributions[1] == 5 && d.opened[1]);
        yield return Walk(-1, -6.6f); yield return Tap(Key.E);
        yield return Arrival("M01_MAPA");
        map = FindAnyObjectByType<M01RouteMap>();
        yield return new WaitForSecondsRealtime(.5f); if (map && map.Focused < 2) yield return Tap(Key.RightArrow);
        Check("Selector: N3 disponible y enfocado", map && map.IsOpen && map.Focused == 2 && M01RouteMap.Available(d, 2));
        yield return Capture("10_Selector_N3_abierto");
        Check("Guardado personal intacto", previousSave == (File.Exists(M01Save.Path) ? File.ReadAllText(M01Save.Path) : null));
        report += $"\nFinalizado. Fallos: {failures}\n"; SafeWrite(Evidence + "/Pruebas.md", report);
        Destroy(gameObject);
    }

    void OnDestroy() { if (Keyboard.current != null) Keys(); InputSystem.settings.backgroundBehavior = previousBackground; InputSystem.settings.editorInputBehaviorInPlayMode = previousFocus; }
}
