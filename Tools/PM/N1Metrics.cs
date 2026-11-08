using System.Collections;
using System.IO;
using Afterglow;
using Afterglow.M01;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class N1Metrics : MonoBehaviour
{
    public static string Main(){var go=new GameObject("LD measurement fixture");DontDestroyOnLoad(go);go.AddComponent<N1Metrics>();return "Medición iniciada; fixture aislado, sin guardar escenas ni partida.";}
    IEnumerator Start()
    {
        Directory.CreateDirectory("Docs/Implementacion/Nivel1");
        M01Session.Instance.data=new M01Progress();
        M01Session.Instance.data.events.AddRange(new[]{"N1A.INTRO","N1C.PHONE","P02.CAPTAIN","M01.WORLD_STARTED","M01.N01_ENTERED"});
        yield return SceneManager.LoadSceneAsync(M01Expedition.ScenePath("N01S01"));
        yield return null;
        foreach(var story in FindObjectsByType<M01StoryPoint>())story.enabled=false;
        var p=FindAnyObjectByType<AfterglowPlayer>();var cc=p.GetComponent<CharacterController>();
        var cam=FindAnyObjectByType<AfterglowCamera>();var lens=cam.GetComponent<Camera>();
        p.testControl=true;p.testMove=Vector2.zero;
        yield return new WaitForSeconds(1);
        cam.Snap();
        string report="# Métricas observadas en Play Mode\n\nFixture sin guardado; controlador y cámara de producción. Plataforma temporal a Y=30 para medir sin obstáculos. No demuestra recorrido N1.\n\n";
        report+=$"- Cápsula: altura {cc.height:F3}, radio {cc.radius:F3}, stepOffset {cc.stepOffset:F3}, slopeLimit {cc.slopeLimit:F1}, skinWidth {cc.skinWidth:F3}.\n";
        report+=$"- Cámara: FOV {lens.fieldOfView}, resolución {lens.pixelWidth}×{lens.pixelHeight}, seguimiento XYZ {cam.followDepth}, distancia {cam.distance}, pitch {cam.pitch}.\n";
        var plane=new Plane(Vector3.up,Vector3.zero);
        foreach(float v in new[]{.1f,.5f,.8f}){
            var a=lens.ViewportPointToRay(new Vector3(.1f,v,0));var b=lens.ViewportPointToRay(new Vector3(.9f,v,0));
            if(plane.Raycast(a,out float da)&&plane.Raycast(b,out float db))report+=$"- Suelo visible fila {v:F1}: {a.GetPoint(da)} a {b.GetPoint(db)} (80% central).\n";
        }
        var floor=GameObject.CreatePrimitive(PrimitiveType.Cube);floor.name="Temporary metrics floor";floor.transform.position=new Vector3(0,29.5f,0);floor.transform.localScale=new Vector3(60,1,60);
        cc.enabled=false;p.transform.position=new Vector3(0,30.1f,0);cc.enabled=true;p.ResetWorldOneBody(false);
        yield return new WaitForSeconds(.5f);
        float startTime=Time.time;var start=p.transform.position;p.testMove=Vector2.right;
        yield return new WaitForSeconds(1);
        p.testMove=Vector2.zero;report+=$"- Velocidad medida: {(p.transform.position.x-start.x)/(Time.time-startTime):F3} m/s.\n";
        yield return new WaitForSeconds(.2f);
        start=p.transform.position;startTime=Time.time;float peak=start.y;int firstFrame=Time.frameCount;
        p.testJump=true;p.testMove=Vector2.right;
        yield return null;
        while(Time.time-startTime<2){peak=Mathf.Max(peak,p.transform.position.y);if(Time.frameCount>firstFrame+3&&p.Grounded)break;yield return null;}
        p.testMove=Vector2.zero;
        report+=$"- Salto real: altura {peak-start.y:F3} m; alcance {p.transform.position.x-start.x:F3} m; vuelo {Time.time-startTime:F3} s; frames {Time.frameCount-firstFrame}; aterrizó {p.Grounded}.\n";
        report+="- Diseño conservador: huecos obligatorios ≤2 m, ascensos individuales ≤0,8 m, aterrizajes ≥2×2 m, pasarelas ≥2,4 m. Confirmar después en el montaje Blender.\n";
        File.WriteAllText("Docs/Implementacion/Nivel1/Metricas.md",report);
        Destroy(gameObject);
    }
}
