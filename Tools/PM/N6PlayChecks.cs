using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Afterglow;
using Afterglow.M01;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Recorrido de N6: corredor, rescate, batalla contra PerforAudora, victoria, cierre y Continuará.</summary>
public sealed class N6PlayChecks : M01RouteRunner
{
    protected override string Evidence => "Docs/Implementacion/Niveles/N6_Evidencias";
    protected override string Title => ArenaMode ? "Altar de la arena (C44) en Play Mode" : "Recorrido N6 en Play Mode";
    protected override string ReportFile => ArenaMode ? "Pruebas_Altar_Arena.md" : "Pruebas.md";
    public static string Main() { ArenaMode = false; var g = new GameObject("N6 route checks"); DontDestroyOnLoad(g); g.AddComponent<N6PlayChecks>(); return "Recorrido N6 iniciado"; }
    /// <summary>C44: contador de arena, entrega de exactamente 5, rechazo con mejora activa, lanzamientos detenidos y perjuicio.</summary>
    public static string MainArena() { ArenaMode = true; var g = new GameObject("N6 arena altar checks"); DontDestroyOnLoad(g); g.AddComponent<N6PlayChecks>(); return "Prueba del altar de la arena iniciada"; }
    static bool ArenaMode;
    bool holdForDebuff, debuffDelivery;

    IEnumerator CollectArenaOfferings(M01BossArena arena, int goal, float seconds)
    {
        float end = Now + seconds;
        while (arena.ArenaOfferings < goal && Now < end && !Player.Dead)
        {
            if (Dialogue.IsOpen) { Keys(); yield return Speak(); continue; }
            if (Player.Health <= 4) { Keys(); yield return HealIfLow(); }
            var p = Player.transform.position;
            var o = FindObjectsByType<M01ArenaOffering>(FindObjectsSortMode.None).OrderBy(x => Vector3.Distance(x.to, p)).FirstOrDefault();
            if (o) MoveTowards(o.to); else MoveTowards(arena.center);
            yield return null;
        }
        Keys();
    }

    IEnumerator GoToAltar(M01BossArena arena)
    {
        var a = arena.altar.position + new Vector3(1.4f, 0, 0);
        for (float end = Now + 8; Now < end;)
        {
            var p = Player.transform.position;
            if (Vector3.Distance(new Vector3(p.x, 0, p.z), new Vector3(a.x, 0, a.z)) <= .4f) break;
            MoveTowards(a); yield return null;
        }
        Keys(); yield return new WaitForSecondsRealtime(.2f);
    }

    HashSet<M01ArenaOffering> OfferingIds() => new HashSet<M01ArenaOffering>(FindObjectsByType<M01ArenaOffering>(FindObjectsSortMode.None));

    IEnumerator AltarChecks()
    {
        var arena = FindAnyObjectByType<M01BossArena>(); var drill = arena.drill;
        if (!Chained && Data.offerings.Count == 0)
        {
            for (int i = 0; i < 3; i++) Data.CollectOffering("PRUEBA.C44." + i, "Coca", false);
            Note("Fixture de prueba: 3 ofrendas de exploración (Coca) en el inventario para comprobar que el altar de la arena no las consume.");
        }
        int explore = Data.offerings.Count;
        Check("Altar de la arena independiente del altar de exploración (sin M01Altar en N06S02)", arena.altar && !FindAnyObjectByType<M01Altar>());
        drill.Halt();
        Note("Ayuda de prueba: taladro detenido con `Halt()` en la fase 1 mientras se ejercita el altar; piedras y lanzamientos de Platicio siguen activos. Después se reanuda con `Begin(1)` y la pelea sigue sin ayudas.");
        // Menos de 5: no consume.
        yield return GoToAltar(arena);
        yield return Capture("C44_01_Altar_arena");
        int before = arena.ArenaOfferings; yield return Tap(Key.E); yield return new WaitForSecondsRealtime(.3f);
        Check($"Con {before} ofrendas de arena el altar no entrega ni consume", arena.ArenaOfferings == before && arena.Effect == "Neutral");
        yield return CollectArenaOfferings(arena, 10, 150);
        Check($"Contador de arena independiente: {arena.ArenaOfferings} de arena; ofrendas de exploración {explore} → {Data.offerings.Count}", arena.ArenaOfferings >= 10 && Data.offerings.Count == explore);
        yield return GoToAltar(arena);
        before = arena.ArenaOfferings; yield return Tap(Key.E); yield return new WaitForSecondsRealtime(.3f);
        int after = arena.ArenaOfferings;
        Check($"Entrega exactamente 5 y conserva sobrantes: {before} → {after} · efecto {arena.Effect} · exploración {Data.offerings.Count}", after == before - 5 && arena.Effect == "Mejora" && Data.offerings.Count == explore);
        float buffStart = Now;
        yield return Capture("C44_02_Mejora_del_Tio");
        yield return Tap(Key.E); yield return new WaitForSecondsRealtime(.3f);
        Check($"Con mejora activa el altar rechaza sin consumir: {after} → {arena.ArenaOfferings}", arena.ArenaOfferings == after && arena.Effect == "Mejora");
        var ids = OfferingIds(); float watch = Now + 6; bool newThrow = false;
        while (Now < watch) { newThrow |= OfferingIds().Except(ids).Any(); yield return null; }
        Check("Con mejora activa Platicio no lanza ofrendas (6 s observados)", !newThrow && arena.Effect == "Mejora");
        while (arena.Effect == "Mejora" && Now < buffStart + 20) yield return null;
        float lasted = Now - buffStart;
        Check($"La mejora termina sola: {lasted:F1} s", arena.Effect == "Neutral" && lasted > 13.5f && lasted < 17f);
        ids = OfferingIds(); watch = Now + 7; newThrow = false;
        while (Now < watch && !newThrow) { newThrow |= OfferingIds().Except(ids).Any(); yield return null; }
        Check("Los lanzamientos se reanudan tras la mejora", newThrow);
        if (arena.ArenaOfferings < 5) yield return CollectArenaOfferings(arena, 5, 60);
        Note($"Ofrendas de arena guardadas para la prueba del perjuicio: {arena.ArenaOfferings}");
        holdForDebuff = true;
        drill.Begin(1);
    }

    IEnumerator Battle(float seconds)
    {
        var arena = FindAnyObjectByType<M01BossArena>(); var drill = arena.drill; float end = Now + seconds;
        bool sawStuck = false, sawDelivery = false; float phaseTwoAt = -1; int startPhase = arena.Phase; string prevState = ""; int prevHp = Player.Health; int logged = 0; Vector3 dodge = Vector3.zero;
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
            if (holdForDebuff && arena.Phase >= 2 && phaseTwoAt < 0) { phaseTwoAt = Now; Note($"  · {Now:F1} s fase 2: se retienen la entrega y el ataque a las resistencias hasta el perjuicio"); }
            if (holdForDebuff && arena.Effect == "Perjuicio")
            {
                holdForDebuff = false; float waited = Now - phaseTwoAt;
                Check($"Sin entrega válida en 40 s llega el perjuicio del Tío ({waited:F1} s desde la fase 2)", waited > 38f && waited < 44f);
                debuffDelivery = true;
            }
            bool holding = holdForDebuff && arena.Phase >= 2;
            if (drill.State == "Atascado" && !holding)
            {
                sawStuck = true;
                var target = FindObjectsByType<AfterglowEnemy>(FindObjectsSortMode.None).Where(e => !e.Defeated && e.name.StartsWith("N6.RESISTENCIA")).OrderBy(e => Vector3.Distance(e.transform.position, p)).FirstOrDefault();
                if (target) { yield return Fight(target, 4, false); continue; }
            }
            // Entregar 5 ofrendas de arena en el altar cuando no hay mejora activa.
            if (!holdForDebuff && arena.ArenaOfferings >= 5 && arena.Effect != "Mejora" && drill.State != "Embistiendo" && drill.State != "Aviso")
            {
                var a = arena.altar.position + new Vector3(1.4f, 0, 0);
                if (Vector3.Distance(new Vector3(p.x, 0, p.z), new Vector3(a.x, 0, a.z)) > .4f) { MoveTowards(a); yield return null; continue; }
                int had = arena.ArenaOfferings; string effect = arena.Effect;
                Keys(); yield return Tap(Key.E); yield return new WaitForSecondsRealtime(.2f);
                sawDelivery |= arena.ArenaOfferings < had;
                if (debuffDelivery && effect == "Perjuicio" && arena.ArenaOfferings < had)
                {
                    debuffDelivery = false;
                    Check($"Entregar 5 con perjuicio lo elimina: {had} → {arena.ArenaOfferings} · efecto {arena.Effect}", arena.ArenaOfferings == had - 5 && arena.Effect == "Neutral" && M01PlayerRules.SpeedMultiplier == 1 && M01PlayerRules.IntervalMultiplier == 1);
                    yield return Capture("C44_03_Perjuicio_eliminado");
                }
                continue;
            }
            // Esquivar la embestida: salir lateralmente de la trayectoria del taladro, hacia el centro.
            var toPlayer = p - drill.transform.position; toPlayer.y = 0; var fwd = drill.transform.forward;
            float along = Vector3.Dot(toPlayer, fwd); float lateral = Vector3.Dot(toPlayer, drill.transform.right);
            // Desde la fase 2 el taladro sigue al jugador durante el aviso: moverse de lado entonces no sirve. Mientras
            // apunta o avisa, el jugador se aleja hacia el lado opuesto de la arena para que la embestida empiece lejos.
            if (drill.State == "Apuntando" || drill.State == "Aviso" || drill.State == "Retrocediendo")
            {
                dodge = Vector3.zero;
                var away = toPlayer.sqrMagnitude > .01f ? toPlayer.normalized : (p - arena.center).normalized;
                var spot = drill.transform.position + away * 30; var fromCenter = spot - arena.center; fromCenter.y = 0;
                spot = arena.center + Vector3.ClampMagnitude(fromCenter, arena.radius - 2.2f);
                if (Vector3.Distance(new Vector3(p.x, 0, p.z), new Vector3(spot.x, 0, spot.z)) > .5f) { MoveTowards(spot); yield return null; continue; }
                Keys(); yield return null; continue;
            }
            bool threat = drill.State == "Embistiendo";
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
        if (ArenaMode) yield return AltarChecks();
        yield return Battle(420);
        if (ArenaMode) Check("Perjuicio observado y eliminado con una entrega", !holdForDebuff && !debuffDelivery);
        var arena = FindAnyObjectByType<M01BossArena>();
        if (Player.Dead || SceneId != "N06S02" || !arena || !arena.Won)
        {
            Check("Victoria contra PerforAudora", false);
            yield return Arrival("N06S01");
            Check("Derrota: vuelta al corredor con vida máxima y opción F", Data.events.Contains("N6.DERROTA") && Player.Health == Player.maxHealth);
            yield break;
        }
        Check("Victoria contra PerforAudora tras tres grupos", arena.Won && Data.events.Contains("N6.VICTORIA"));
        yield return Capture("03_Victoria");
        // Won se marca al inicio de la explosión; la pieza se coloca 1,2 s después (M01Ending.Begin). Esperar a que exista.
        var ending = FindAnyObjectByType<M01Ending>();
        for (float pieceWait = Now + 5; ending.Step != "Pieza" && Now < pieceWait;) yield return null;
        yield return Speak();
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
