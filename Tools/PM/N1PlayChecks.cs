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

public sealed class N1PlayChecks : MonoBehaviour
{
    // 2026-11-08: otro proceso puede mapear el informe (error Win32 1224); se reintenta sin abortar la prueba.
    static void SafeWrite(string path,string text){for(int i=0;i<8;i++){try{System.IO.File.WriteAllText(path,text);return;}catch(System.IO.IOException){System.Threading.Thread.Sleep(40);}}}

    const string Evidence = "Docs/Implementacion/Nivel1";
    string report = "# Recorrido N1 v0.5 en Play Mode\n\n";
    int failures;
    float n1Started;
    InputSettings.BackgroundBehavior previousBackground;
    InputSettings.EditorInputBehaviorInPlayMode previousFocus;
    AfterglowPlayer Player => FindAnyObjectByType<AfterglowPlayer>();
    M01Dialogue Dialogue => FindAnyObjectByType<M01Dialogue>();
    M01Progress Data => M01Session.Instance.data;
    string SceneId => SceneManager.GetActiveScene().name;

    public static string Main()
    {
        var runner = new GameObject("Prueba de cámaras espaciales");
        DontDestroyOnLoad(runner);
        runner.AddComponent<N1PlayChecks>();
        return "Prueba iniciada";
    }

    bool Check(string label, bool result)
    {
        if (!result) failures++;
        report += $"- {(result?"PASS":"FAIL")} {label}\n";
        SafeWrite(Evidence+"/Pruebas.md", report);
        return result;
    }

    void Keys(params Key[] keys) => InputSystem.QueueStateEvent(Keyboard.current, new KeyboardState(keys));

    IEnumerator Tap(Key key)
    {
        Keys(key);
        yield return new WaitForSecondsRealtime(.1f);
        Keys();
        yield return new WaitForSecondsRealtime(.13f);
    }

    IEnumerator Arrival(string id)
    {
        float end = Now+12;
        while ((SceneId!=id||M01Expedition.Transitioning)&&Now<end)yield return null;
        yield return new WaitForSecondsRealtime(.42f);
        Check("Llegada a "+id,SceneId==id&&!M01Expedition.Transitioning);
    }

    IEnumerator Speak()
    {
        int presses=0;
        while(Dialogue&&Dialogue.IsOpen&&presses++<75)yield return Tap(Key.E);
        Check("Secuencia de diálogo termina",Dialogue&&!Dialogue.IsOpen);
        // 2026-11-08: la presentación de objetos clave pausa tras el diálogo; esperar a que devuelva el control.
        for(float t=0;t<.2f||M01KeyItemCelebration.Playing;t+=Time.unscaledDeltaTime){if(t>8)break;yield return null;}
    }

    IEnumerator Walk(float x,float z,bool jump=false)
    {
        float end=Now+15;
        bool reached=false;
        bool jumpSent=!jump;
        while(Now<end && !Player.Dead)
        {
            if(Dialogue&&Dialogue.IsOpen){Keys();yield return Speak();end=Now+15;}
            Vector3 p=Player.transform.position;
            var delta=new Vector2(x-p.x,z-p.z);
            if(delta.magnitude<.23f){reached=true;break;}
            Key horizontal=Mathf.Abs(delta.x)<.15f?Key.None:delta.x>0?Key.RightArrow:Key.LeftArrow;
            Key depth=Mathf.Abs(delta.y)<.15f?Key.None:delta.y>0?Key.UpArrow:Key.DownArrow;
            if(!jumpSent&&Player.Grounded){Keys(horizontal,depth,Key.Space);jumpSent=true;}
            else Keys(horizontal,depth);
            yield return null;
        }
        Keys();
        float settle=Now+2.5f;
        while(!Player.Grounded&&Now<settle)yield return null;
        yield return new WaitForSecondsRealtime(.1f);
        Check($"Paso {SceneId} ({x}, {z}) a altura {Player.transform.position.y:F2}",reached);
    }

    IEnumerator Enter(string destination)
    {
        yield return Tap(Key.E);
        yield return Arrival(destination);
    }

    IEnumerator Capture(string name)
    {
        yield return new WaitForSecondsRealtime(1.15f);
        yield return new WaitForEndOfFrame();
        ScreenCapture.CaptureScreenshot(Evidence+"/"+name+".png");
        yield return null;
    }

    IEnumerator Fight(AfterglowEnemy enemy)
    {
        var player=Player;
        player.testControl=true;
        float end=Now+18;
        while(enemy&&!enemy.Defeated&&!player.Dead&&Now<end)
        {
            Vector3 delta=enemy.transform.position-player.transform.position;
            var flat=new Vector2(delta.x,delta.z);
            player.testMove=flat.normalized*(flat.magnitude>1.08f?1:.16f);
            player.testAttack=flat.magnitude<=1.1f;
            yield return null;
        }
        player.testControl=false;
        player.testMove=Vector2.zero;
        player.testAttack=false;
        Check("Resistencia derrotada mediante ataque básico",enemy&&enemy.Defeated);
    }

    // Reloj de juego para los plazos (2026-11-08): un fotograma lento del editor cuenta como un paso, no como segundos reales.
    float clock, previousMaxDelta = .3333f; float Now => clock;
    void LateUpdate() { clock += Time.timeScale > 0 ? Time.deltaTime / Time.timeScale : Mathf.Min(Time.unscaledDeltaTime, Time.maximumDeltaTime); }

    IEnumerator Start()
    {
        Directory.CreateDirectory(Evidence);
        previousMaxDelta = Time.maximumDeltaTime; Time.maximumDeltaTime = 1f / 30f;
        previousBackground=InputSystem.settings.backgroundBehavior;
        previousFocus=InputSystem.settings.editorInputBehaviorInPlayMode;
        InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
        InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        InputSystem.EnableDevice(Keyboard.current);
        string previousSave=File.Exists(M01Save.Path)?File.ReadAllText(M01Save.Path):null;
        yield return Arrival("M01_MENU");
        bool hasSave=M01Save.ReadFile(M01Save.Path,out _,out _);
        if(hasSave)yield return Tap(Key.DownArrow);
        for(int i=0;i<5&&SceneId=="M01_MENU";i++)yield return Tap(Key.E);
        yield return Arrival("PRO_CASA");
        if(SceneId!="PRO_CASA")yield break;
        Check("Carta antes de movimiento",Dialogue.IsOpen&&Dialogue.CurrentId=="P01.02");
        yield return Speak();
        yield return new WaitForSecondsRealtime(.8f);
        Check("Timbre y prólogo sin herramientas",Data.events.Contains("P01.BELL")&&!Data.pickaxeOwned&&!Data.phoneOwned);
        yield return Capture("01_Dormitorio");
        yield return Walk(12.5f,3.15f);
        yield return Enter("PRO_PATIO");
        if(SceneId!="PRO_PATIO")yield break;
        var camera=FindAnyObjectByType<AfterglowCamera>();
        var cameraStart=camera.transform.position;
        yield return Capture("02_Patio_Fachada");
        yield return Walk(-5,8.3f);
        yield return Walk(-5,-5.1f);
        yield return Capture("03_Patio_Profundidad");
        Check("Patio recorrible en profundidad y cámara acompaña Z",Mathf.Abs(camera.transform.position.z-cameraStart.z)>10);
        yield return Walk(5,-5.1f);
        yield return Walk(5,4);
        yield return Capture("04_Patio_Fuente");
        yield return Walk(5,-5.1f);
        yield return Walk(12.5f,-7.5f);
        yield return Capture("05_Porton");
        yield return Tap(Key.E);
        Check("Capitán interactúa desde dentro",Dialogue.IsOpen&&Dialogue.CurrentId=="P02.02");
        yield return Speak();
        yield return Arrival("M01_MAPA");
        if(SceneId!="M01_MAPA")yield break;
        var map=FindAnyObjectByType<M01RouteMap>();
        Check("Capitán abre el selector independiente con N1 enfocado",map.IsOpen&&map.standalone&&map.Focused==0&&Time.timeScale==0);
        yield return Capture("06_Mapa");
        yield return Tap(Key.RightArrow);
        yield return Tap(Key.E);
        Check("Nodo cerrado conserva selector",map.IsOpen&&map.Focused==1);
        yield return Tap(Key.LeftArrow);
        yield return Enter("N01S01");
        if(SceneId!="N01S01")yield break;
        Check("N1 tiene entrada propia y un altar",!FindAnyObjectByType<M01RouteMap>()&&FindObjectsByType<M01Altar>().Length==1&&Data.expeditionAttempt==1);
        yield return Speak();
        Check("Presentación y teléfono del mismo guía",Data.events.Contains("N1A.INTRO")&&Data.phoneOwned&&!Data.pickaxeOwned);
        n1Started=Now;
        yield return Walk(-5.8f,2);yield return Tap(Key.E);yield return Speak();
        Check("Q01 se acepta antes de las bajas",Data.Quest("Q01").active&&Data.Quest("Q01").count==0);
        yield return Capture("07_N01S01_Entrada");
        yield return Walk(1,2);
        yield return Walk(1,7.3f);
        yield return Enter("N01S02");
        if(SceneId!="N01S02")yield break;
        yield return Walk(-6,-3.5f);yield return Walk(-1.3f,-3.5f);yield return Walk(2,-2,true);
        camera=FindAnyObjectByType<AfterglowCamera>();cameraStart=camera.transform.position;
        yield return Walk(3,-1);yield return Walk(5,1,true);yield return Walk(5,2);yield return Walk(6,4.3f,true);yield return Walk(6,5.6f);yield return Walk(7,7,true);
        yield return Capture("08_N01S02_Terrazas");
        Check("Salto base y seguimiento vertical hasta 2,2 m",Player.transform.position.y>2&&camera.transform.position.y>cameraStart.y+.8f);
        Check("Ofrenda de la terraza recogida",Data.collectedStaticIds.Contains("EXP.N01S02.0"));
        yield return Walk(8,9.2f);
        yield return Enter("N01S03");
        if(SceneId!="N01S03")yield break;
        yield return Speak();
        Check("Pico recibido en cámara de trabajo",Data.pickaxeOwned);
        yield return Capture("09_N01S03_Trabajo");
        var enemy=FindObjectsByType<AfterglowEnemy>().First(e=>e.name=="N01.TUTORIAL.RESISTANCE");
        yield return Fight(enemy);
        yield return Speak();
        var offering=FindObjectsByType<M01Loot>().FirstOrDefault(o=>o.kind==M01LootKind.Offering);
        if(offering)yield return Walk(offering.transform.position.x,offering.transform.position.z);
        yield return Walk(8,1.6f);
        Check("Tres ofrendas reales disponibles",Data.offerings.Count>=3);
        foreach(var rival in FindObjectsByType<AfterglowEnemy>().Where(e=>!e.Defeated))yield return Fight(rival);
        yield return Walk(-6,7.5f);
        Keys(Key.UpArrow);yield return new WaitForSecondsRealtime(.12f);Keys();yield return Tap(Key.X);
        yield return new WaitForSecondsRealtime(.4f);
        Check("Pico abre roca del cuello",Data.events.Contains("N01.S03.ROCA_ROTA"));
        yield return Walk(-6,10.65f);
        yield return Enter("N01S04");
        if(SceneId!="N01S04")yield break;
        yield return Walk(4,-3.7f);yield return Speak();
        yield return Walk(4,-2.2f);
        var bridge=FindAnyObjectByType<M01CounterweightBridge>();
        // Un jugador vuelve a pulsar si la palanca no respondió (la E justo después de un diálogo puede caer en su espera).
        for(int attempt=0;attempt<3&&!Data.events.Contains(M01CounterweightBridge.OpenEvent);attempt++){yield return Tap(Key.E);yield return Speak();float until=Now+1.5f;while(Now<until&&!Data.events.Contains(M01CounterweightBridge.OpenEvent))yield return null;}
        float lowered=Now+2f;while(Now<lowered&&!bridge.crossing.enabled)yield return null;
        Check("Palanca baja paso y habilita collider",Data.events.Contains(M01CounterweightBridge.OpenEvent)&&bridge.crossing.enabled);
        yield return Capture("10a_Puente");
        yield return Walk(2,-.8f);yield return Walk(2,6);yield return Walk(8,8);
        Check("Casco accesible en ramal norte",Data.questItems.Contains("Q02.CASCO"));
        yield return Walk(2,6);yield return Walk(-1.3f,6);yield return Walk(-4,6,true);
        yield return Walk(-4,8,true);yield return Walk(-4,9.8f,true);
        yield return Capture("10_N01S04_Derrumbe");
        yield return Tap(Key.E);yield return Speak();
        Check("Investigación en el derrumbe elevado",Data.investigated[0]);
        yield return Tap(Key.T);yield return Speak();yield return Arrival("N01S01");
        Check("Teléfono devuelve a la entrada real",SceneId=="N01S01"&&Data.offerings.Count>=3);
        yield return Tap(Key.E);yield return Speak();
        float deadline=Now+12;
        while(!Data.opened[0]&&Now<deadline)yield return null;
        Check("Altar acredita 3 y abre N2 tras investigar",Data.contributions[0]==3&&Data.opened[0]);
        report+=$"\nRuta N1 automatizada hasta apertura: {Now-n1Started:F1} segundos, incluye diálogos/capturas y combate opcional; no es duración estimada para jugadores.\n";
        yield return Speak();
        yield return Walk(1,1);yield return Walk(6,3.7f);yield return Tap(Key.E);yield return Speak();yield return Tap(Key.E);yield return Speak();
        Check("Q02 paga la cuarta fuente garantizada",Data.Quest("Q02").paid&&Data.collectedStaticIds.Any(s=>s.StartsWith("Q02.REWARD.")));
        int rewards=Data.offerings.Count,money=Data.money;yield return Tap(Key.E);yield return Speak();
        Check("Q02 no paga dos veces",Data.offerings.Count==rewards&&Data.money==money);
        yield return Walk(1,1);yield return Walk(-5.8f,2);yield return Tap(Key.E);yield return Speak();
        Check("Q01 cuenta tres bajas aceptadas y recompensa una vez",Data.Quest("Q01").paid&&Data.Quest("Q01").count==3&&Data.collectedStaticIds.Any(s=>s.StartsWith("Q01.REWARD.")));
        yield return Walk(-1,-6.5f);yield return Enter("M01_MAPA");yield return Tap(Key.RightArrow);
        map=FindAnyObjectByType<M01RouteMap>();
        Check("Selector normal muestra N2 abierto sin construirlo",map&&map.IsOpen&&map.Focused==1&&M01RouteMap.Available(Data,1));
        yield return Capture("11_Mapa_N2_Abierto");
        Check("Guardado personal intacto",previousSave==(File.Exists(M01Save.Path)?File.ReadAllText(M01Save.Path):null));
        report+=$"\nFinalizado. Fallos: {failures}\n";
        SafeWrite(Evidence+"/Pruebas.md",report);
        Destroy(gameObject);
    }

    void OnDestroy()
    { Time.maximumDeltaTime = previousMaxDelta;
        if(Keyboard.current!=null)Keys();
        InputSystem.settings.backgroundBehavior=previousBackground;
        InputSystem.settings.editorInputBehaviorInPlayMode=previousFocus;
    }
}
