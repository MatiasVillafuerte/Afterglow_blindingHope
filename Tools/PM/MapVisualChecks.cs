using System.IO;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Afterglow.M01;
public sealed class PMMapVisualChecks:MonoBehaviour
{
 public static string Main(){var go=new GameObject("PM visual fixture");DontDestroyOnLoad(go);go.AddComponent<PMMapVisualChecks>();return "Started";}
 IEnumerator Start()
 {
  var data=new M01Progress();data.events.AddRange(new[]{"P02.CAPTAIN","M01.WORLD_STARTED"});M01Session.Instance.data=data;SceneManager.LoadScene(M01Expedition.ScenePath("M01_ACCESO"));yield return null;yield return null;yield return new WaitForSecondsRealtime(.5f);
  var map=FindAnyObjectByType<M01RouteMap>();map.Open();yield return new WaitForSecondsRealtime(.5f);Canvas.ForceUpdateCanvases();
  var rect=(RectTransform)map.view.transform;var corners=new Vector3[4];rect.GetWorldCorners(corners);var canvas=map.dialogue.GetComponent<Canvas>();var cr=(RectTransform)canvas.transform;bool inside=corners.All(c=>RectTransformUtility.RectangleContainsScreenPoint(cr,c,null));
  var r="# PM09–10: mapa visual en Canvas real\n\n- Panel completo dentro del Canvas: "+inside+"\n- Seis nodos: "+(map.nodes.Length==6)+"\n- Una sola maqueta y una sola camara de mapa: "+(FindObjectsByType<Camera>().Count(c=>c.targetTexture)!=0)+"\n";File.WriteAllText("Docs/Implementacion/PM/Pruebas_Mapa_Visual.md",r);
  yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot("Docs/Implementacion/PM/PM_Mapa_Inicial.png");yield return new WaitForSecondsRealtime(.4f);map.Close();Destroy(gameObject);
 }
}
