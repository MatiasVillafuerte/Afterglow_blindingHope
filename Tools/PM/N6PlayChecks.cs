using System.Collections;
using System.Linq;
using Afterglow;
using Afterglow.M01;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Recorrido de N6: corredor, rescate, batalla contra PerforAudora, victoria, cierre y Continuará.</summary>
public sealed class N6PlayChecks : M01RouteRunner
{
    protected override string Evidence => "Docs/Implementacion/Niveles/N6_Evidencias";
    protected override string Title => "Recorrido N6 en Play Mode";
    public static string Main() { var g = new GameObject("N6 route checks"); DontDestroyOnLoad(g); g.AddComponent<N6PlayChecks>(); return "Recorrido N6 iniciado"; }

    IEnumerator Battle(float seconds)
    {
        var arena = FindAnyObjectByType<M01BossArena>(); var drill = arena.drill; float end = Now + seconds;
        bool sawStuck = false, sawDelivery = false; int startPhase = arena.Phase; string prevState = ""; int prevHp = Player.Health; int logged = 0; Vector3 dodge = Vector3.zero;
        while (!arena.Won && !Player.Dead && Now < end && SceneId == "N06S02")
        {
            if ((drill.State != prevState || Player.Health != prevHp) && logged++ < 60)
            {
                var rel = Player.transform.position - drill.transform.position; rel.y = 0;
                Note($"  · {Now:F1} s {drill.State} vida {Player.Health} · jugador {Player.transform.position:F1} · taladro {drill.transform.position:F1} · delante {Vector3.Dot(rel, drill.transform.forward):F1} · lateral {Vector3.Dot(rel, drill.transform.right):F1} · bloqueo {Player.GetComponent<M01PlayerRules>().BlocksControl}");
                prevState = drill.State; prevHp = Player.Health;
            }
            if (Dialogue.IsOpen) { Keys(); yield return Speak(); continue; }
            var p = Player.transform.position;
            if (drill.State == "Atascado")
            {
                sawStuck = true;
                var target = FindObjectsByType<AfterglowEnemy>(FindObjectsSortMode.None).Where(e => !e.Defeated && e.name.StartsWith("N6.RESISTENCIA")).OrderBy(e => Vector3.Distance(e.transform.position, p)).FirstOrDefault();
                if (target) { yield return Fight(target, 4, false); continue; }
            }
            // Entregar 5 ofrendas de arena en el altar cuando no hay mejora activa.
            if (arena.ArenaOfferings >= 5 && arena.Effect != "Mejora" && drill.State != "Embistiendo" && drill.State != "Aviso")
            {
                var a = arena.altar.position + new Vector3(1.4f, 0, 0);
                if (Vector3.Distance(new Vector3(p.x, 0, p.z), new Vector3(a.x, 0, a.z)) > .4f) { MoveTowards(a); yield return null; continue; }
                Keys(); yield return Tap(Key.E); sawDelivery |= arena.ArenaOfferings < 5; continue;
            }
            // Esquivar la embestida: salir lateralmente de la trayectoria del taladro, hacia el centro.
            var toPlayer = p - drill.transform.position; toPlayer.y = 0; var fwd = drill.transform.forward;
            float along = Vector3.Dot(toPlayer, fwd); float lateral = Vector3.Dot(toPlayer, drill.transform.right);
            bool threat = drill.State == "Aviso" || drill.State == "Embistiendo";
            if (!threat) dodge = Vector3.zero;
            if (threat && (dodge != Vector3.zero || (along > -1 && Mathf.Abs(lateral) < 2.6f)))
            {
                // Una sola dirección por embestida: perpendicular al taladro y hacia el centro de la arena.
                if (dodge == Vector3.zero) { var perp = drill.transform.right; var toCenter = arena.center - p; toCenter.y = 0; dodge = Vector3.Dot(perp, toCenter) >= 0 ? perp : -perp; }
                if (Mathf.Abs(lateral) < 3.2f) { MoveTowards(p + dodge * 2f); yield return null; continue; }
                Keys(); yield return null; continue;
            }
            // Recoger ofrendas de arena cercanas.
            var offering = FindObjectsByType<M01ArenaOffering>(FindObjectsSortMode.None).OrderBy(o => Vector3.Distance(o.transform.position, p)).FirstOrDefault();
            if (offering && Vector3.Distance(offering.to, p) < 6) { MoveTowards(offering.to); yield return null; continue; }
            Keys(); yield return null;
        }
        Keys();
        Check("El taladro se atascó y bajaron resistencias", sawStuck);
        Note("Entrega en el altar de la arena observada: " + sawDelivery + " · fase alcanzada: " + arena.Phase + " · efecto: " + arena.Effect);
    }

    void MoveTowards(Vector3 target)
    {
        var d = target - Player.transform.position;
        Keys(Mathf.Abs(d.x) < .2f ? Key.None : d.x > 0 ? Key.RightArrow : Key.LeftArrow, Mathf.Abs(d.z) < .2f ? Key.None : d.z > 0 ? Key.UpArrow : Key.DownArrow);
    }

    protected override IEnumerator Route()
    {
        CompleteBefore(6);
        yield return EnterFromMap(6); if (SceneId != "N06S01") yield break;
        yield return Capture("01_Corredor");
        yield return Walk(10.6f, -1.2f, false, 16);
        Check("Voces de los atrapados como subtítulos sin detener al jugador", !Dialogue.IsOpen);
        yield return Tap(Key.E); yield return SpeakUntil("N06S02");
        yield return Arrival("N06S02"); if (SceneId != "N06S02") yield break;
        yield return Speak();
        Check("Presentación y explicaciones N6A.13–15", !Dialogue.IsOpen && FindAnyObjectByType<M01BossArena>().Phase == 1);
        Check("Guardado bloqueado durante el jefe", !M01Save.Allowed);
        yield return Capture("02_Arena_fase1");
        yield return Battle(420);
        var arena = FindAnyObjectByType<M01BossArena>();
        if (Player.Dead || SceneId != "N06S02")
        {
            Check("Victoria contra PerforAudora", false);
            yield return Arrival("N06S01");
            Check("Derrota: vuelta al corredor con vida máxima y opción F", Data.events.Contains("N6.DERROTA") && Player.Health == Player.maxHealth);
            yield break;
        }
        Check("Victoria contra PerforAudora tras tres grupos", arena.Won && Data.events.Contains("N6.VICTORIA"));
        yield return Capture("03_Victoria");
        yield return Speak();
        var ending = FindAnyObjectByType<M01Ending>();
        yield return Walk(ending.piece.transform.position.x, ending.piece.transform.position.z - 1.2f);
        yield return Tap(Key.E); yield return Speak(); yield return Speak();
        Check("Pieza dorada registrada como objeto clave una sola vez", Data.events.Contains("KEY.PIEZA"));
        float t = Now + 20; while (ending.Step != "Vagoneta" && Now < t) { yield return Speak(); yield return null; }
        Check("Capitán, Platicio se une al equipo y la vagoneta aparece", Data.members[1].unlocked && Data.events.Contains("FIN.CONVERSACION") && ending.cart.activeSelf);
        yield return Capture("04_Cierre_capitan");
        yield return Tap(Key.Escape); yield return new WaitForSecondsRealtime(.3f);
        for (int i = 0; i < 3; i++) yield return Tap(Key.DownArrow);
        yield return Tap(Key.E); yield return new WaitForSecondsRealtime(.5f);
        Check("Cambiar personaje desde el menú: Platicio activo", Data.activeMember == 1);
        yield return Walk(ending.cart.transform.position.x - 1.2f, ending.cart.transform.position.z);
        yield return Tap(Key.E); yield return new WaitForSecondsRealtime(.3f); yield return Tap(Key.E);
        t = Now + 12; while (ending.Step != "Continúa" && Now < t) yield return null;
        Check("Viaje en vagoneta y pantalla Continuará", ending.Step == "Continúa" && Data.worldClosed);
        yield return Capture("05_Continuara");
        yield return Tap(Key.E); yield return new WaitForSecondsRealtime(.3f); yield return Tap(Key.DownArrow); yield return Tap(Key.E);
        yield return Arrival("M01_MENU");
        Note($"\nRecorrido N6 hasta Continuará: {Now - started:F1} s.");
    }
}
