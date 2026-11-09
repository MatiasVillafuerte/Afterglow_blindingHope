using System.Collections;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Afterglow;
using Afterglow.M01;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Recorrido completo de Mundo 1 de una sola vez (C56, 2026-11-08): Nueva partida → N1 … N6 → «Continuará» → menú.
/// Encadena N1PlayChecks y N2–N6PlayChecks con M01RouteRunner.Chained activo: ningún fixture, cuota ni investigación se
/// concede por código; el estado de cada nivel es el que dejó el anterior. Arrancar en Play Mode desde M01_MENU.
/// </summary>
public sealed class M01FullRun : MonoBehaviour
{
    const string Evidence = "Docs/Implementacion/Recorrido_Completo";
    string report; float clock; int levelFailures;
    void LateUpdate() { clock += Time.timeScale > 0 ? Time.deltaTime / Time.timeScale : Mathf.Min(Time.unscaledDeltaTime, Time.maximumDeltaTime); }

    public static string Main() { var g = new GameObject("Recorrido completo"); DontDestroyOnLoad(g); g.AddComponent<M01FullRun>(); return "Recorrido completo iniciado"; }

    static void SafeWrite(string path, string text) { for (int i = 0; i < 8; i++) { try { File.WriteAllText(path, text); return; } catch (IOException) { System.Threading.Thread.Sleep(40); } } }
    void Write() => SafeWrite(Evidence + "/Pruebas.md", report);

    string Snapshot()
    {
        var s = M01Session.Instance ? M01Session.Instance.data : null; if (s == null) return "sin partida";
        var p = FindAnyObjectByType<AfterglowPlayer>();
        return $"vida {(p ? p.Health : s.Active.health)}/{s.Active.maxHealth} · dinero {s.money} · ofrendas {s.offerings.Count(o => !o.stolen)} · núcleos {s.restorers} · EXP {s.experience} · nivel de equipo {s.teamLevel} · abiertos {string.Join("", s.opened.Select(o => o ? "1" : "0"))}";
    }

    IEnumerator Stage(string label, string evidence, System.Func<string> start, System.Func<bool> running)
    {
        float t0 = clock; report += $"\n## {label}\n\nInicio: {Snapshot()}\n"; Write();
        start();
        yield return new WaitForSecondsRealtime(1f);
        // Tope de 20 min de juego por etapa: si un recorrido se atasca, el informe lo dice en vez de esperar sin fin.
        while (running() && clock - t0 < 1200) yield return new WaitForSecondsRealtime(.5f);
        if (running()) { report += "- FAIL Etapa detenida por tope de 20 min de juego\n"; foreach (var r in FindObjectsByType<M01RouteRunner>(FindObjectsSortMode.None)) Destroy(r.gameObject); foreach (var r in FindObjectsByType<N1PlayChecks>(FindObjectsSortMode.None)) Destroy(r.gameObject); foreach (var r in FindObjectsByType<N2PlayChecks>(FindObjectsSortMode.None)) Destroy(r.gameObject); levelFailures++; }
        yield return new WaitForSecondsRealtime(.5f);
        string text = File.Exists(evidence) ? File.ReadAllText(evidence) : "";
        var m = Regex.Match(text, @"Fallos: (\d+)"); int failures = m.Success ? int.Parse(m.Groups[1].Value) : -1;
        int pass = Regex.Matches(text, @"- PASS ").Count;
        if (failures != 0) levelFailures++;
        report += $"Fin: {Snapshot()} · escena {SceneManager.GetActiveScene().name}\n{pass} PASS · {(failures < 0 ? "sin informe final" : failures + " fallos")} · {clock - t0:F0} s de juego · informe `{evidence}`\n";
        foreach (var line in text.Split('\n').Where(l => l.StartsWith("- FAIL")).Take(12)) report += line + "\n";
        Write();
    }

    IEnumerator Start()
    {
        Directory.CreateDirectory(Evidence);
        report = "# Recorrido completo de Mundo 1\n\nNueva partida → «Continuará» de una sola vez, sin fixture (C56). Teclado simulado por el Input System. Compras de Núcleos en la tienda del pueblo con dinero ganado.\n";
        Write();
        M01RouteRunner.Chained = true;
        yield return Stage("N1 · desde Nueva partida", "Docs/Implementacion/Nivel1/Pruebas.md", N1PlayChecks.Main, () => FindAnyObjectByType<N1PlayChecks>());
        yield return Stage("Tienda antes de N2", M01ShopVisit.Path, M01ShopVisit.Main, () => FindAnyObjectByType<M01ShopVisit>());
        yield return Stage("N2", "Docs/Implementacion/Niveles/N2_Evidencias/Pruebas.md", N2PlayChecks.Main, () => FindAnyObjectByType<N2PlayChecks>());
        yield return Stage("N3", "Docs/Implementacion/Niveles/N3_Evidencias/Pruebas.md", N3PlayChecks.Main, () => FindAnyObjectByType<N3PlayChecks>());
        yield return Stage("N4", "Docs/Implementacion/Niveles/N4_Evidencias/Pruebas.md", N4PlayChecks.Main, () => FindAnyObjectByType<N4PlayChecks>());
        yield return Stage("N5", "Docs/Implementacion/Niveles/N5_Evidencias/Pruebas.md", N5PlayChecks.Main, () => FindAnyObjectByType<N5PlayChecks>());
        yield return Stage("N6 · PerforAudora y «Continuará»", "Docs/Implementacion/Niveles/N6_Evidencias/Pruebas.md", N6PlayChecks.Main, () => FindAnyObjectByType<N6PlayChecks>());
        M01RouteRunner.Chained = false;
        report += $"\nRecorrido completo finalizado en {clock / 60f:F1} min de juego. Niveles con fallos: {levelFailures}\n";
        Write(); Destroy(gameObject);
    }

    void OnDestroy() { M01RouteRunner.Chained = false; }
}

/// <summary>
/// Preparación entre N1 y N2 en el recorrido encadenado: desde el selector vuelve a N1, compra Núcleos en la tienda del
/// pueblo con el dinero ganado y regresa al selector con N2 enfocado. Entre los demás niveles la compra ocurre en PayAndOpen.
/// </summary>
public sealed class M01ShopVisit : M01RouteRunner
{
    public const string Path = "Docs/Implementacion/Recorrido_Completo/Tienda_N2.md";
    protected override string Evidence => "Docs/Implementacion/Recorrido_Completo";
    protected override string ReportFile => "Tienda_N2.md";
    protected override string Title => "Tienda del pueblo antes de N2";
    public static string Main() { var g = new GameObject("Tienda antes de N2"); DontDestroyOnLoad(g); g.AddComponent<M01ShopVisit>(); return "Tienda iniciada"; }
    protected override IEnumerator Route()
    {
        yield return new WaitForSecondsRealtime(.6f);
        var map = FindAnyObjectByType<M01RouteMap>(); if (map && !map.IsOpen) map.Open();
        yield return new WaitForSecondsRealtime(.3f);
        for (int guard = 0; map && map.Focused != 0 && guard < 4; guard++) yield return Tap(Key.LeftArrow);
        Check("Selector enfocado en N1", map && map.Focused == 0);
        yield return Tap(Key.E); yield return Arrival("N01S01"); if (SceneId != "N01S01") yield break;
        yield return Walk(4, -4.4f);
        yield return Shop();
        if (SceneId != "N01S01") yield break;
        yield return Walk(-1, -6.6f); yield return Tap(Key.E); yield return Arrival("M01_MAPA");
        map = FindAnyObjectByType<M01RouteMap>(); yield return new WaitForSecondsRealtime(.5f);
        for (int guard = 0; map && map.Focused != 1 && guard < 4; guard++) yield return Tap(Key.RightArrow);
        Check("Selector de vuelta con N2 enfocado", map && map.IsOpen && map.Focused == 1);
    }
}
