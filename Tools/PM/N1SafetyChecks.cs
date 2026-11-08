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
public sealed class N1SafetyChecks:MonoBehaviour
{

    // 2026-11-08: otro proceso puede mapear el informe (error Win32 1224); se reintenta sin abortar la prueba.
    static void SafeWrite(string path,string text){for(int i=0;i<8;i++){try{System.IO.File.WriteAllText(path,text);return;}catch(System.IO.IOException){System.Threading.Thread.Sleep(40);}}}

    const string Dir="Tools/PM/TestData/N1Safety/";
    const string Report="Docs/Implementacion/Nivel1/Pruebas_Seguridad.md";
    string log="# Seguridad y persistencia N1\n\nFixture aislado: estado inicial sembrado, desplazamiento y E/T reales. No sustituye Pruebas.md desde Nueva partida.\n\n";int failures;
    string personal;InputSettings.BackgroundBehavior background;InputSettings.EditorInputBehaviorInPlayMode focus;
    AfterglowPlayer P=>FindAnyObjectByType<AfterglowPlayer>();
    M01Progress D=>M01Session.Instance.data;M01Dialogue Talk=>FindAnyObjectByType<M01Dialogue>();
    M01CounterweightBridge Bridge=>FindAnyObjectByType<M01CounterweightBridge>();
    public static string Main(){var g=new GameObject("N1 safety fixture");DontDestroyOnLoad(g);g.AddComponent<N1SafetyChecks>();return "Pruebas de seguridad iniciadas";}
    void Check(string name,bool ok){if(!ok)failures++;log+=$"- {(ok?"PASS":"FAIL")} {name}\n";SafeWrite(Report,log);}
    void Keys(params Key[] k)=>InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(k));
    IEnumerator Tap(Key k){Keys(k);yield return new WaitForSecondsRealtime(.08f);Keys();yield return new WaitForSecondsRealtime(.1f);}
    IEnumerator Speak(){for(int i=0;Talk&&Talk.IsOpen&&i<55;i++)yield return Tap(Key.E);    // 2026-11-08: la presentación de objetos clave pausa tras el diálogo; esperar a que devuelva el control.
        for(float t=0;t<.2f||M01KeyItemCelebration.Playing;t+=Time.unscaledDeltaTime){if(t>8)break;yield return null;}
    }
    IEnumerator Capture(string name){yield return new WaitForSecondsRealtime(.7f);yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot("Docs/Implementacion/Nivel1/"+name+".png");yield return null;}
    IEnumerator Ready(string id){float t=Time.realtimeSinceStartup+12;while((SceneManager.GetActiveScene().name!=id||M01Expedition.Transitioning)&&Time.realtimeSinceStartup<t)yield return null;yield return new WaitForSecondsRealtime(.5f);Check("Llegada "+id,SceneManager.GetActiveScene().name==id&&!M01Expedition.Transitioning);}
    IEnumerator Walk(float x,float z)
    {
        P.testControl=true;float end=Time.realtimeSinceStartup+12;
        while(Time.realtimeSinceStartup<end){var a=P.transform.position;var d=new Vector2(x-a.x,z-a.z);if(d.magnitude<.2f)break;P.testMove=d.normalized;yield return null;}
        P.testMove=Vector2.zero;yield return new WaitForSecondsRealtime(.15f);
        Check("Caminar "+x+","+z,Vector2.Distance(new Vector2(P.transform.position.x,P.transform.position.z),new Vector2(x,z))<.3f);
    }
    IEnumerator Start()
    {
        Directory.CreateDirectory(Dir);personal=File.Exists(M01Save.Path)?File.ReadAllText(M01Save.Path):null;
        background=InputSystem.settings.backgroundBehavior;focus=InputSystem.settings.editorInputBehaviorInPlayMode;
        InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;InputSystem.EnableDevice(Keyboard.current);
        var d=new M01Progress{phoneOwned=true,pickaxeOwned=true,expeditionActive=true,expeditionAttempt=1,money=19};
        d.events.AddRange(new[]{"P01.LETTER","P01.BELL","P02.CAPTAIN","M01.WORLD_STARTED","M01.N01_ENTERED","N1A.INTRO","N1C.PHONE","N1B.PICKAXE","N1B.LOOT","N1E.BRIDGE_HINT","N01.S03.ROCA_ROTA"});
        d.CollectOffering("FIXTURE.OFFERING","Coca",false);d.Quest("Q02").paid=true;M01Session.Instance.data=d;
        M01Expedition.LoadSavedRoom(M01Expedition.ScenePath("N01S04"),M01Expedition.SectionEntrance("N01S04"),Vector3.forward);yield return Ready("N01S04");
        Check("Puente inicialmente cerrado",Bridge&&!Bridge.crossing.enabled&&!D.events.Contains(M01CounterweightBridge.OpenEvent));
        var deckMesh=Bridge.deck.GetComponentInChildren<MeshFilter>();
        var armMesh=Bridge.leverArm.GetComponentInChildren<MeshFilter>();
        Check("Mallas móviles unidas a sus pivotes",deckMesh&&armMesh);
        var closedCenter=deckMesh?deckMesh.transform.TransformPoint(deckMesh.sharedMesh.bounds.center):Vector3.zero;
        yield return Walk(4,-2.2f);
        yield return Capture("12_Puente_Cerrado");
        Check("Guardar cerrado",M01Save.TryWrite(out var error,Dir+"closed.json"));
        yield return Tap(Key.E);
        Check("Durante movimiento guardar bloqueado",D.events.Contains(M01CounterweightBridge.OpenEvent)&&!M01Save.Allowed&&!Bridge.crossing.enabled);
        yield return new WaitForSecondsRealtime(1.2f);
        Check("Puente firme y control de guardado liberado",Bridge.crossing.enabled&&M01Save.Allowed);
        Check("Aviso de palanca se retira al usarla",Bridge.prompt&&!Bridge.prompt.activeSelf);
        Check("Malla visible baja y coincide con paso",deckMesh&&Vector3.Distance(closedCenter,deckMesh.transform.TransformPoint(deckMesh.sharedMesh.bounds.center))>2&&Mathf.Abs(deckMesh.GetComponent<Renderer>().bounds.center.y+.12f)<.1f);
        yield return Capture("13_Puente_Abierto");
        Check("Guardar abierto",M01Save.TryWrite(out error,Dir+"open.json"));
        yield return Tap(Key.E);yield return new WaitForSecondsRealtime(.2f);
        Check("Activación única sin recompensa ni investigación",D.events.Count(e=>e==M01CounterweightBridge.OpenEvent)==1&&D.money==19&&D.offerings.Count==1&&!D.investigated[0]);
        yield return Walk(5,-6.4f);yield return Tap(Key.E);yield return Ready("N01S03");
        Check("Puerta de vuelta mantiene roca abierta",!FindObjectsByType<M01BreakableRock>().Any(r=>r.gameObject.activeInHierarchy));
        yield return Walk(-6,10.65f);yield return Tap(Key.E);yield return Ready("N01S04");
        Check("Regreso conserva puente y recursos",Bridge.crossing.enabled&&D.money==19&&D.offerings.Count==1);
        yield return Walk(-1,-.5f);
        int hp=P.Health;P.testMove=Vector2.up;float timeout=Time.realtimeSinceStartup+7;
        while(P.Health==hp&&Time.realtimeSinceStartup<timeout)yield return null;P.testMove=Vector2.zero;
        yield return new WaitForSecondsRealtime(1);
        Check("Caída resta 2 y vuelve a entrada segura",P.Health==hp-2&&Vector3.Distance(P.transform.position,M01Expedition.SectionEntrance("N01S04"))<.5f&&Bridge.crossing.enabled);
        yield return new WaitForSecondsRealtime(1.1f);P.TakeDamage(99);yield return Ready("N01S01");
        Check("Muerte conserva progreso y cura al activo",P.Health==P.maxHealth&&D.events.Contains(M01CounterweightBridge.OpenEvent)&&D.Quest("Q02").paid&&D.money==19&&D.offerings.Count==1);
        yield return Walk(1,1);yield return Walk(6,3.7f);yield return Tap(Key.E);yield return Speak();
        Check("Q02 no vuelve a pagar después de muerte",D.Quest("Q02").paid&&D.money==19&&D.offerings.Count==1);
        Check("Cargar abierto",M01Save.Continue(out error,Dir+"open.json"));yield return Ready("N01S04");
        Check("Carga restaura puente y posición exacta",Bridge.crossing.enabled&&Mathf.Abs(P.transform.position.x-4)<.3f&&Mathf.Abs(P.transform.position.z+2.2f)<.3f&&D.offerings.Count==1);
        Check("Cargar cerrado",M01Save.Continue(out error,Dir+"closed.json"));yield return Ready("N01S04");
        Check("Guardado anterior restaura cerrado sin perder recursos",!Bridge.crossing.enabled&&D.money==19&&D.offerings.Count==1&&D.Quest("Q02").paid);
        var legacy=new M01SaveFile{schema=4,scene=M01Expedition.ScenePath("N01S04"),position=new Vector3(-4,2.2f,9.4f),progress=D,facing=Vector3.forward};
        SafeWrite(Dir+"legacy.json",JsonUtility.ToJson(legacy));
        Check("Esquema 4 recoloca sobre entrada nueva",M01Save.ReadFile(Dir+"legacy.json",out var migrated,out error)&&migrated.schema==5&&migrated.position==M01Expedition.SectionEntrance("N01S04")&&migrated.progress.money==19&&migrated.progress.Quest("Q02").paid);
        P.testControl=true;P.testJump=true;yield return new WaitForSecondsRealtime(.15f);yield return Tap(Key.T);
        Check("Teléfono bloqueado en salto",!Talk.IsOpen&&SceneManager.GetActiveScene().name=="N01S04");
        yield return new WaitForSecondsRealtime(1);yield return Tap(Key.T);yield return Speak();yield return Ready("N01S01");
        Check("Teléfono conserva recursos y no aporta",D.offerings.Count==1&&D.contributions[0]==0&&D.money==19);
        Check("Guardado personal intacto",personal==(File.Exists(M01Save.Path)?File.ReadAllText(M01Save.Path):null));
        log+=$"\nFinalizado. Fallos: {failures}\n";SafeWrite(Report,log);Destroy(gameObject);
    }
    void OnDestroy(){Keys();InputSystem.settings.backgroundBehavior=background;InputSystem.settings.editorInputBehaviorInPlayMode=focus;}
}
