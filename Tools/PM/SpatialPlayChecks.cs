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

public sealed class SpatialPlayChecks : MonoBehaviour
{
    const string Evidence = "Docs/Implementacion/Camara_Espacial";
    string report = "# Recorrido espacial en Play Mode\n\n";
    int failures;
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
        runner.AddComponent<SpatialPlayChecks>();
        return "Prueba iniciada";
    }

    bool Check(string label, bool result)
    {
        if (!result) failures++;
        report += $"- {(result?"PASS":"FAIL")} {label}\n";
        File.WriteAllText(Evidence+"/Pruebas.md", report);
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
        float end = Time.realtimeSinceStartup+12;
        while ((SceneId!=id||M01Expedition.Transitioning)&&Time.realtimeSinceStartup<end)yield return null;
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
        float end=Time.realtimeSinceStartup+15;
        bool reached=false;
        bool jumpSent=!jump;
        while(Time.realtimeSinceStartup<end && !Player.Dead)
        {
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
        float settle=Time.realtimeSinceStartup+2.5f;
        while(!Player.Grounded&&Time.realtimeSinceStartup<settle)yield return null;
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
        float end=Time.realtimeSinceStartup+18;
        while(enemy&&!enemy.Defeated&&!player.Dead&&Time.realtimeSinceStartup<end)
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

    IEnumerator Start()
    {
        Directory.CreateDirectory(Evidence);
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
        yield return Capture("07_N01S01_Entrada");
        yield return Walk(1,7.3f);
        yield return Enter("N01S02");
        if(SceneId!="N01S02")yield break;
        yield return Walk(6,-1.4f);
        camera=FindAnyObjectByType<AfterglowCamera>();cameraStart=camera.transform.position;
        foreach(float z in new[]{1f,3f,5f,7.8f})yield return Walk(6,z,true);
        yield return Capture("08_N01S02_Terrazas");
        Check("Salto base y seguimiento vertical hasta 2,2 m",Player.transform.position.y>2&&camera.transform.position.y>cameraStart.y+1.5f);
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
        yield return Walk(4.1f,.54f);
        Keys(Key.UpArrow);yield return new WaitForSecondsRealtime(.13f);Keys();yield return Tap(Key.X);
        yield return new WaitForSecondsRealtime(.36f);
        Check("Pico abre la roca opcional",Data.events.Contains("N01.S03.ROCA_ROTA"));
        foreach(var rival in FindObjectsByType<AfterglowEnemy>().Where(e=>!e.Defeated))yield return Fight(rival);
        yield return Walk(-6,10.65f);
        yield return Enter("N01S04");
        if(SceneId!="N01S04")yield break;
        yield return Walk(7,3);
        Check("Casco accesible en ramal inferior",Data.questItems.Contains("Q02.CASCO"));
        yield return Walk(6,-1.45f);yield return Walk(-4,-1.45f);
        foreach(float z in new[]{1f,3f,5f,7.8f})yield return Walk(-4,z,true);
        yield return Walk(-4,8.75f);
        yield return Capture("10_N01S04_Derrumbe");
        yield return Tap(Key.E);yield return Speak();
        Check("Investigación en el derrumbe elevado",Data.investigated[0]);
        yield return Tap(Key.T);yield return Speak();yield return Arrival("N01S01");
        Check("Teléfono devuelve a la entrada real",SceneId=="N01S01"&&Data.offerings.Count>=3);
        yield return Tap(Key.E);yield return Speak();
        float deadline=Time.realtimeSinceStartup+12;
        while(!Data.opened[0]&&Time.realtimeSinceStartup<deadline)yield return null;
        Check("Altar acredita 3 y abre N2 tras investigar",Data.contributions[0]==3&&Data.opened[0]);
        Check("Guardado personal intacto",previousSave==(File.Exists(M01Save.Path)?File.ReadAllText(M01Save.Path):null));
        report+=$"\nFinalizado. Fallos: {failures}\n";
        File.WriteAllText(Evidence+"/Pruebas.md",report);
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if(Keyboard.current!=null)Keys();
        InputSystem.settings.backgroundBehavior=previousBackground;
        InputSystem.settings.editorInputBehaviorInPlayMode=previousFocus;
    }
}
