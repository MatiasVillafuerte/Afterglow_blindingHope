using System.Collections;
using System.IO;
using Afterglow;
using Afterglow.M01;
using UnityEngine;
using UnityEngine.SceneManagement;

// Temporary visual probe: no scene writes and no save operations.
public sealed class ArtBoundaryProbe : MonoBehaviour
{
    public static string Main()
    {
        var runner=new GameObject("Revisión visual de rejas");
        DontDestroyOnLoad(runner);runner.AddComponent<ArtBoundaryProbe>();
        return "Revisión iniciada";
    }
    IEnumerator Start()
    {
        yield return SceneManager.LoadSceneAsync(M01Expedition.ScenePath("PRO_PATIO"));
        yield return new WaitForSecondsRealtime(.7f);
        var player=FindAnyObjectByType<AfterglowPlayer>();
        player.testControl=true;player.testMove=Vector2.zero;
        string report="# Rejas desde la cámara Game\n\n";
        foreach(var point in new[]{new Vector3(-2,.05f,-13.6f),new Vector3(-12.5f,.05f,-6)})
        {
            player.GetComponent<CharacterController>().enabled=false;
            player.transform.position=point;
            player.GetComponent<CharacterController>().enabled=true;
            FindAnyObjectByType<AfterglowCamera>().Snap();
            yield return new WaitForSecondsRealtime(1.2f);
            var viewport=Camera.main.WorldToViewportPoint(player.transform.position+Vector3.up);
            bool framed=viewport.z>0&&viewport.x>.05f&&viewport.x<.95f&&viewport.y>.05f&&viewport.y<.95f;
            report+=$"- {(framed?"PASS":"FAIL")} Personaje encuadrado cerca de reja en {point}; viewport {viewport}\n";
            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot("Docs/Arte/Acabado_2026_11_05/"+(point.z< -10?"Reja_Frontal":"Reja_Lateral")+".png");
            yield return null;
        }
        File.WriteAllText("Docs/Arte/Acabado_2026_11_05/Rejas_Game.md",report);
        player.testControl=false;Destroy(gameObject);
    }
}
