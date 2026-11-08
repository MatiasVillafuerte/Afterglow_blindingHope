using System.Collections;
using System.Linq;
using Afterglow.M01;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Recorrido de N3 (Galerías de las señales) desde el selector hasta abrir N4.</summary>
public sealed class N3PlayChecks : M01RouteRunner
{
    protected override string Evidence => "Docs/Implementacion/Niveles/N3_Evidencias";
    protected override string Title => "Recorrido N3 en Play Mode";
    public static string Main() { var g = new GameObject("N3 route checks"); DontDestroyOnLoad(g); g.AddComponent<N3PlayChecks>(); return "Recorrido N3 iniciado"; }

    IEnumerator CrossS03Forward()
    {
        yield return Walk(-4.6f, -1.6f); yield return Walk(-1.5f, -1.4f, true); yield return Walk(-1.5f, -1f);
        yield return Walk(-0.4f, -2.4f); yield return Walk(2.4f, -2.4f, true); yield return Walk(4.6f, -2.6f);
        yield return Walk(7f, -2.6f, true); yield return Walk(8.5f, 2.4f);
    }

    IEnumerator CrossS03Back()
    {
        yield return Walk(6.4f, -2.6f); yield return Walk(4.2f, -2.6f, true); yield return Walk(1.4f, -2.4f);
        yield return Walk(-1.0f, -2.0f, true); yield return Walk(-3.4f, -2f); yield return Walk(-6f, -3f, true); yield return Walk(-7, -5.2f);
    }

    protected override IEnumerator Route()
    {
        CompleteBefore(3);
        yield return EnterFromMap(3); if (SceneId != "N03S01") yield break;
        yield return Capture("01_Bocamina");
        yield return Walk(-4.5f, -3.2f); yield return Speak();
        Check("Platicio explica el código de faroles (N3.01–02) y evalúa su regalo", Data.events.Contains("N3.INTRO") && Data.events.Any(e => e.StartsWith("PLATICIO.REGALO.N3.")));
        yield return Walk(-4, 0.4f); yield return Walk(-4, 2f, true); yield return Walk(-4, 3.5f);
        Check("Ofrenda de la terraza de la bocamina", Data.collectedStaticIds.Contains("EXP.N03S01.0"));
        yield return Walk(0, 4.0f); yield return Door("N03S02"); if (SceneId != "N03S02") yield break;
        yield return Capture("02_Cruce_tres_bocas");
        var worm = Enemy("N03.RESISTANCE.0"); if (worm) yield return Fight(worm);
        yield return Walk(8, -5);
        // Error ordinario: la boca central (1 farol) devuelve a la misma cámara con su estado.
        yield return Walk(0, 2.0f); yield return Door("N03S02"); yield return new WaitForSecondsRealtime(.4f);
        Check("Puerta equivocada: misma cámara lógica y aviso N3.03", SceneId == "N03S02" && Data.events.Contains("N3.ERROR") && Dialogue.IsOpen && Dialogue.CurrentId == "N3.03");
        yield return Speak();
        Check("El error conserva lo encontrado (ofrenda recogida y enemigo derrotado)", Data.collectedStaticIds.Contains("EXP.N03S02.0") && (Enemy("N03.RESISTANCE.0") == null || Enemy("N03.RESISTANCE.0").Defeated));
        yield return Walk(-6, 2.0f); yield return Door("N03S03"); if (SceneId != "N03S03") yield break;
        yield return CrossS03Forward();
        Check("Marca del equipo confirma el avance (N3.07)", Data.events.Contains("N3.CONFIRM1"));
        Check("Ofrendas de la confirmación", Data.collectedStaticIds.Contains("EXP.N03S03.0"));
        yield return Capture("03_Marca_del_equipo");
        yield return Walk(8.5f, 3.2f); yield return Door("N03S04"); if (SceneId != "N03S04") yield break;
        yield return Capture("04_Cruce_escalonado");
        // Ramal especial: la puerta baja izquierda lleva al taller de placas.
        yield return Walk(-7, 4.0f); yield return Door("N03S05"); if (SceneId != "N03S05") yield break;
        yield return Capture("05_Taller_placas");
        var w2 = Enemy("N03.RESISTANCE.1"); if (w2) yield return Fight(w2);
        yield return Walk(2, -3.2f); yield return Tap(Key.E); yield return Speak();
        Check("Q06 aceptado", Data.Quest("Q06").active);
        foreach (var p in new[] { new Vector2(-5, 0.4f), new Vector2(0, 1.4f), new Vector2(5, 1.4f) }) { yield return Walk(p.x, p.y); yield return Tap(Key.E); yield return new WaitForSecondsRealtime(.3f); }
        Check("Tres placas enderezadas", Enumerable.Range(0, 3).All(i => Data.events.Contains("Q06.PLACA." + i)));
        yield return Walk(2, -3.2f); yield return Tap(Key.E); yield return Speak();
        Check("Q06 entregado", Data.Quest("Q06").paid);
        yield return Walk(7, -6); yield return Walk(-7, 2.4f); yield return Walk(-7, 4f, true); yield return Walk(-7, 4.3f);
        Check("Ofrendas del taller", Data.collectedStaticIds.Contains("EXP.N03S05.0") && Data.collectedStaticIds.Contains("EXP.N03S05.1"));
        yield return Walk(-5.6f, 4f); yield return Tap(Key.E); yield return Speak();
        Check("Escolta Q05 activa: el minero perdido sigue al jugador", Data.escortActive && Data.Quest("Q05").active);
        yield return Walk(-5, 1.5f); yield return Walk(0, -6.2f); yield return Door("N03S04"); if (SceneId != "N03S04") yield break;
        Check("El minero cruza la puerta con el jugador", M01EscortNPC.Follower != null);
        // Regreso caminando a la bocamina con la escolta.
        yield return Walk(0, -6.2f); yield return Door("N03S03"); if (SceneId != "N03S03") yield break;
        yield return CrossS03Back(); yield return Door("N03S02"); if (SceneId != "N03S02") yield break;
        yield return Walk(0, -6.2f); yield return Door("N03S01");
        yield return new WaitForSecondsRealtime(.6f);
        Check("Q05: el minero llega caminando a la entrada", Data.events.Contains("Q05.ENTREGADO") && !Data.escortActive);
        yield return Walk(3.4f, -4.4f); yield return Tap(Key.E); yield return Speak();
        Check("Q05 entregado: 2 ofrendas y 10 de dinero", Data.Quest("Q05").paid);
        // De nuevo al frente por la ruta correcta.
        yield return Walk(0, 0.3f); yield return Walk(0, 2f, true); yield return Walk(0, 4.0f); yield return Door("N03S02");
        yield return Walk(-6, 2.0f); yield return Door("N03S03"); yield return CrossS03Forward(); yield return Walk(8.5f, 3.2f); yield return Door("N03S04");
        yield return Walk(-1, 0.4f); yield return Walk(-1, 2.2f, true); yield return Walk(3.4f, 2.6f); yield return Walk(5.2f, 2.6f, true); yield return Walk(9, 3.8f);
        Check("Ofrenda de la terraza alta", Data.collectedStaticIds.Contains("EXP.N03S04.0"));
        yield return Walk(7, 4.0f); yield return Door("N03S06"); if (SceneId != "N03S06") yield break;
        yield return Capture("06_Galeria_pozo");
        yield return Walk(0, -3.4f); yield return Walk(0, 0); yield return Walk(0, 3.0f);
        Check("Pasarela del pozo sin caer y sin activar los Gasplode", Player.transform.position.y > -0.2f && FindObjectsByType<M01Gasplode>(FindObjectsSortMode.None).All(g => g.State == "Quieto"));
        yield return Door("N03S07"); if (SceneId != "N03S07") yield break;
        yield return Capture("07_Fin_senales");
        var w3 = Enemy("N03.RESISTANCE.2"); if (w3) yield return Fight(w3);
        yield return Walk(6, 0.2f); yield return Walk(0, 2.4f); yield return Walk(0, 3.4f, true);
        yield return Tap(Key.E); yield return Speak();
        Check("Investigación de N3 registrada", Data.investigated[2]);
        Note($"\nRuta N3 automatizada hasta investigar: {Now - started:F1} s.");
        yield return PayAndOpen(2, 7);
    }
}
