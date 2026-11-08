using System.Collections;
using System.Linq;
using Afterglow.M01;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Recorrido de N4 (Galería sin luz) desde el selector hasta abrir N5.</summary>
public sealed class N4PlayChecks : M01RouteRunner
{
    protected override string Evidence => "Docs/Implementacion/Niveles/N4_Evidencias";
    protected override string Title => "Recorrido N4 en Play Mode";
    public static string Main() { var g = new GameObject("N4 route checks"); DontDestroyOnLoad(g); g.AddComponent<N4PlayChecks>(); return "Recorrido N4 iniciado"; }

    protected override IEnumerator Route()
    {
        CompleteBefore(4);
        yield return EnterFromMap(4); if (SceneId != "N04S01") yield break;
        Check("Sala oscura con lámpara del casco", FindAnyObjectByType<M01DarkRoom>() && Player.GetComponentsInChildren<Light>().Any(l => l.name == "Lámpara del casco"));
        yield return Capture("01_Anden_oscuro");
        yield return Walk(-4.5f, -3.2f); yield return Speak();
        Check("Platicio orienta (N4.01–02) y evalúa su regalo", Data.events.Contains("N4.INTRO"));
        yield return Walk(-8, 0.4f); yield return Walk(-8, 2f, true); yield return Walk(-8, 3.5f); yield return Walk(-8, 0.5f); yield return Walk(9, -6);
        Check("Ofrendas del andén oscuro", Data.collectedStaticIds.Contains("EXP.N04S01.0") && Data.collectedStaticIds.Contains("EXP.N04S01.1"));
        yield return Walk(6, 0.0f); yield return Door("N04S02"); if (SceneId != "N04S02") yield break;
        yield return Capture("02_Descenso_rellanos");
        yield return Walk(-5.6f, -3); yield return Walk(-3, -3); yield return Walk(-3, -5);
        Check("Ofrenda del segundo rellano", Data.collectedStaticIds.Contains("EXP.N04S02.0"));
        yield return Walk(-1.4f, -3); yield return Walk(1, -3);
        var lighmet = Enemy("S02.LIGHMET.N4"); if (lighmet) yield return Fight(lighmet, 25);
        yield return Capture("03_Lighmet_ilumina");
        yield return Walk(5, -3); yield return Walk(9, -2);
        var worm = Enemy("S02.RESISTANCE.N4"); if (worm) yield return Fight(worm);
        yield return Walk(12, -5);
        yield return Walk(10, 3.0f); yield return Door("N04S03"); if (SceneId != "N04S03") yield break;
        yield return Capture("04_Cruce_oscuro");
        // Encargos del refugio primero: las bajas de Q10 solo cuentan tras aceptar.
        yield return Walk(9.6f, 1.2f); yield return Walk(9.6f, 3.4f, true); yield return Walk(10, 3.6f); yield return Tap(Key.E); yield return Speak();
        yield return Walk(12, 3.2f); yield return Tap(Key.E); yield return Speak();
        Check("Q09 y Q10 aceptados en el refugio", Data.Quest("Q09").active && Data.Quest("Q10").active);
        foreach (var id in new[] { "N04.RESISTANCE.2", "N04.RESISTANCE.1", "N04.LIGHMET.1", "N04.RESISTANCE.0" }) { var e = Enemy(id); if (e) yield return Fight(e, 25); }
        Check("Q10: cuatro componentes del cruce", Data.Quest("Q10").count >= 4);
        yield return Walk(0, -5); yield return Walk(-6, 1.5f);
        Check("Ofrendas del cruce", Data.collectedStaticIds.Contains("EXP.N04S03.0") && Data.collectedStaticIds.Contains("EXP.N04S03.1"));
        var cart = FindObjectsByType<M01RailCart>(FindObjectsSortMode.None).First(c => c.cartId == "CARRO_LUZ");
        yield return Walk(-11.8f, -1); yield return Walk(-11.8f, 0.6f);
        float end = Now + 22; while (!cart.OnPlate && Now < end) { if (Dialogue.IsOpen) { Keys(); yield return Speak(); } Keys(Key.RightArrow); yield return null; }
        Keys(); yield return new WaitForSecondsRealtime(.3f);
        Check("Q09: el carro de luz llega al refugio y lo ilumina", Data.events.Contains("Q09.CARRO_LUZ"));
        yield return Capture("05_Carro_de_luz");
        yield return Walk(9.6f, 1.2f); yield return Walk(9.6f, 3.4f, true); yield return Walk(10, 3.6f); yield return Tap(Key.E); yield return Speak();
        yield return Walk(12, 3.2f); yield return Tap(Key.E); yield return Speak();
        Check("Q09 y Q10 entregados", Data.Quest("Q09").paid && Data.Quest("Q10").paid);
        yield return Walk(12, 5.0f); yield return Door("N04S04"); if (SceneId != "N04S04") yield break;
        yield return Walk(-4, -2.2f); yield return Walk(-4, 0, true);
        var l2 = Enemy("S04.LIGHMET.N4"); if (l2) yield return Fight(l2, 25);
        yield return Walk(-4, 0); yield return Walk(-2.6f, 0.6f); yield return Walk(0, 1, true); yield return Walk(1.4f, 1.6f); yield return Walk(4, 2.2f, true); yield return Walk(4, 2.8f);
        yield return Walk(5.4f, 3); yield return Walk(8, 3.2f, true); yield return Walk(9, 5);
        Check("Ofrendas de la grieta alta", new[] { "EXP.N04S04.0", "EXP.N04S04.1", "EXP.N04S04.2" }.All(Data.collectedStaticIds.Contains));
        yield return Walk(8, 5.0f); yield return Door("N04S05"); if (SceneId != "N04S05") yield break;
        var w = Enemy("S05.RESISTANCE.N4"); if (w) yield return Fight(w);
        yield return Walk(6, -1); yield return Walk(0, 2.4f); yield return Walk(0, 3.4f, true); yield return Tap(Key.E); yield return Speak();
        Check("Investigación de N4 registrada", Data.investigated[3]);
        yield return Capture("06_Estacion_carga");
        Note($"\nRuta N4 automatizada hasta investigar: {Now - started:F1} s.");
        yield return PayAndOpen(3, 9);
    }
}
