using System.Collections;
using System.IO;
using System.Linq;
using Afterglow;
using Afterglow.M01;
using UnityEngine;

/// <summary>
/// Prueba dirigida (2026-11-08) de cifras de daño: invulnerabilidad sin cifra y golpe mayor que la vida restante.
/// Se ejecuta en N01S03 en Play Mode. Llama a TakeDamage directamente: verifica reglas, no la entrada.
/// </summary>
public sealed class DamageChecks : MonoBehaviour
{
    const string Report = "Docs/Implementacion/Nivel1/Pruebas_Dano_Dirigidas.md";
    string log = "# Prueba dirigida de cifras de daño\n\nN01S03 en Play Mode. TakeDamage directo (no entrada de teclado).\n\n"; int failures;
    static void SafeWrite(string path, string text) { for (int i = 0; i < 8; i++) { try { File.WriteAllText(path, text); return; } catch (IOException) { System.Threading.Thread.Sleep(40); } } }
    void Check(string label, bool ok) { if (!ok) failures++; log += $"- {(ok ? "PASS" : "FAIL")} {label}\n"; SafeWrite(Report, log); }
    public static string Main() { var g = new GameObject("Damage checks"); g.AddComponent<DamageChecks>(); return "Prueba de daño iniciada"; }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1.2f);
        var player = FindAnyObjectByType<AfterglowPlayer>();
        var rules = player.GetComponent<M01PlayerRules>();
        while (rules.BlocksControl) yield return null; // diálogo de entrega del pico
        while (M01KeyItemCelebration.Playing) yield return null;
        yield return new WaitForSeconds(1.1f);

        // 1. Invulnerabilidad: el segundo golpe dentro de 1 s no resta vida ni crea cifra.
        int before = player.Health, spawned = AfterglowDamagePopup.Spawned;
        bool first = player.TakeDamage(1);
        int afterFirst = AfterglowDamagePopup.Spawned;
        bool second = player.TakeDamage(1);
        Check("Jugador: primer golpe resta 1 y crea una cifra «1»", first && player.Health == before - 1 && afterFirst == spawned + 1 && AfterglowDamagePopup.LastAmount == 1);
        Check("Jugador: golpe absorbido por invulnerabilidad no resta vida ni crea cifra", !second && player.Health == before - 1 && AfterglowDamagePopup.Spawned == afterFirst);

        // 2. Enemigo: dos impactos sucesivos y golpe final mayor que la vida restante.
        var enemy = FindObjectsByType<AfterglowEnemy>().First(e => e.name == "N01.TUTORIAL.RESISTANCE");
        enemy.enabled = false; // aislar: sin mordidas durante la prueba
        spawned = AfterglowDamagePopup.Spawned;
        bool e1 = enemy.TakeDamage(1);
        Check("Enemigo: impacto efectivo 2→1 con cifra «1»", e1 && enemy.Health == 1 && AfterglowDamagePopup.Spawned == spawned + 1 && AfterglowDamagePopup.LastAmount == 1);
        bool e2 = enemy.TakeDamage(5);
        Check("Enemigo: golpe de 5 con 1 de vida muestra la vida efectivamente perdida («1») y lo derrota", e2 && enemy.Defeated && AfterglowDamagePopup.LastAmount == 1 && AfterglowDamagePopup.Spawned == spawned + 2);
        bool e3 = enemy.TakeDamage(1);
        Check("Enemigo derrotado no recibe más daño ni cifras", !e3 && AfterglowDamagePopup.Spawned == spawned + 2);
        yield return new WaitForEndOfFrame();
        ScreenCapture.CaptureScreenshot("Docs/Implementacion/Nivel1/Evidencias_2026_11_08/13_Dano_Excedente.png");

        // 3. Jugador: golpe mayor que la vida restante muestra la vida perdida real.
        yield return new WaitForSeconds(1.1f);
        rules.Progress.Active.health = 1; player.SynchronizeWorldOneState();
        spawned = AfterglowDamagePopup.Spawned;
        bool lethal = player.TakeDamage(4);
        Check("Jugador con 1 de vida recibe 4: cifra «1», vida 0 y muerte", lethal && AfterglowDamagePopup.LastAmount == 1 && AfterglowDamagePopup.Spawned == spawned + 1 && player.Dead);
        log += $"\nFinalizado. Fallos: {failures}\n"; SafeWrite(Report, log);
        Destroy(gameObject);
    }
}
