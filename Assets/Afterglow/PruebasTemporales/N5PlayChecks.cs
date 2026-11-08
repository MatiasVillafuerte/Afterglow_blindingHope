using System.Collections;
using System.Linq;
using Afterglow.M01;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Recorrido de N5 (La vía suspendida) desde el selector hasta abrir N6, con caída, insuficiencia y éxito.</summary>
public sealed class N5PlayChecks : M01RouteRunner
{
    protected override string Evidence => "Docs/Implementacion/Niveles/N5_Evidencias";
    protected override string Title => "Recorrido N5 en Play Mode";
    public static string Main() { var g = new GameObject("N5 route checks"); DontDestroyOnLoad(g); g.AddComponent<N5PlayChecks>(); return "Recorrido N5 iniciado"; }
    // Pulsaciones de salto antes de cada hueco y obstáculo (x de la vagoneta).
    static readonly float[] Jumps = { 26.8f, 43.0f, 53.8f, 79.1f, 95.9f, 119.3f, 133.3f, 140.3f, 148.6f };

    IEnumerator Ride(bool unused)
    {
        var ride = FindAnyObjectByType<M01MineCartRide>(); int next = 0; float lastX = 0, lastY = 0;
        float end = Now + 80;
        while (ride && Now < end && SceneId == "N05S02")
        {
            float x = ride.cart.position.x;
            if (x < 5 && lastX > 10) Note($"- Caída en x {lastX:F1} (altura {lastY:F2}), saltos enviados {next}, minerales {ride.Collected}");
            lastX = x; lastY = ride.cart.position.y;
            if (x < 5 && next > 0) next = 0; // reinicio tras una caída
            if (next < Jumps.Length && x >= Jumps[next]) { yield return Tap(Key.Space); next++; continue; }
            yield return null;
        }
    }

    protected override IEnumerator Route()
    {
        CompleteBefore(5);
        yield return EnterFromMap(5); if (SceneId != "N05S01") yield break;
        yield return Capture("01_Estacion");
        yield return Walk(-7.6f, -3.4f); yield return Speak();
        yield return Walk(-10, -2.4f); yield return Tap(Key.E); yield return Speak();
        Check("Q07 aceptado", Data.Quest("Q07").active);
        // Cuarta roca (−4, −1,2): se golpea desde abajo, mirando hacia arriba.
        foreach (var (r, face) in new[] { (new Vector2(-8, -4.6f), Key.DownArrow), (new Vector2(-5, -4.8f), Key.DownArrow), (new Vector2(-2, -4.4f), Key.DownArrow), (new Vector2(-4, -2.4f), Key.UpArrow) })
        { yield return Walk(r.x, r.y); yield return Face(face); yield return Tap(Key.X); yield return new WaitForSecondsRealtime(.7f); }
        Check("Q07: cuatro rocas marcadas", Data.Quest("Q07").count >= 4);
        yield return Walk(-10, -2.4f); yield return Tap(Key.E); yield return Speak();
        Check("Q07 entregado", Data.Quest("Q07").paid);
        yield return Walk(-11, -6);
        yield return Walk(-0.6f, -3.8f); yield return Walk(1.4f, -3.8f, true); yield return Walk(2.2f, -3.8f); yield return Tap(Key.E); yield return Speak();
        Check("Q08 aceptado", Data.Quest("Q08").active);
        yield return Walk(11, -4.5f);
        Check("Ofrendas de la estación", Data.collectedStaticIds.Contains("EXP.N05S01.0") && Data.collectedStaticIds.Contains("EXP.N05S01.1"));
        // Rechazo inicial: «Todavía no» no inicia el viaje.
        yield return Walk(7.2f, -2.4f); yield return Tap(Key.E);
        while (Dialogue.IsOpen && Dialogue.CurrentId == "N5.02") yield return Tap(Key.E);
        yield return Tap(Key.DownArrow); yield return Tap(Key.E); yield return new WaitForSecondsRealtime(.6f);
        Check("Rechazo inicial: sigue en la estación", SceneId == "N05S01");
        for (int attempt = 0; attempt < 3 && SceneId == "N05S01"; attempt++)
        {
            yield return new WaitForSecondsRealtime(.6f); yield return Tap(Key.E); yield return Speak();
            float wait = Now + 4; while (SceneId == "N05S01" && Now < wait) yield return null;
        }
        yield return Arrival("N05S02"); if (SceneId != "N05S02") yield break;
        var ride = FindAnyObjectByType<M01MineCartRide>();
        yield return Capture("02_Via_inicio");
        // Primera pasada: sin saltar el primer hueco → caída y reinicio con el contador a cero.
        float t0 = Now; while (ride.Attempts == 0 && Now - t0 < 15) yield return null;
        Check("Caer reinicia la carrera desde el inicio con 0 minerales", ride.Attempts == 1 && ride.Collected == 0 && ride.cart.position.x < 10);
        yield return Ride(false);
        yield return Arrival("N05S03"); if (SceneId != "N05S03") yield break;
        int tries = 0;
        while (!Data.events.Contains("N5.PREMIO") && tries++ < 3)
        {
            yield return new WaitForSecondsRealtime(1f);
            if (Dialogue.IsOpen && !Data.events.Contains("N5.PREMIO")) { yield return Capture("03_Andén_insuficiente"); Note("Intento insuficiente: el minero pide repetir."); yield return Speak(); yield return Arrival("N05S02"); yield return Ride(false); yield return Arrival("N05S03"); }
        }
        yield return Speak();
        Check("Minerales suficientes: el minero entrega las ofrendas una sola vez (N5.04)", Data.events.Contains("N5.PREMIO"));
        Check("El paquete de la vía quedó en el inventario", Data.questItems.Contains("Q08.PAQUETE") || Data.Quest("Q08").paid);
        yield return Capture("04_Andén_exito");
        yield return Walk(3, -3.2f); yield return Tap(Key.E); yield return Speak();
        Check("Q08 entregado en el andén", Data.Quest("Q08").paid);
        yield return Walk(6, -0.6f); yield return Walk(0, 1.6f); yield return Walk(0, 3.2f, true); yield return Tap(Key.E); yield return Speak();
        Check("Investigación de N5 registrada tras el éxito", Data.investigated[4]);
        Note($"\nRuta N5 automatizada hasta investigar: {Now - started:F1} s.");
        yield return PayAndOpen(4, 12);
    }
}
