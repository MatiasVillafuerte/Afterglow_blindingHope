using System.Collections;
using System.IO;
using UnityEngine;
using Afterglow;
public sealed class AfterglowArtCapture:MonoBehaviour
{
 public string stage;
 public static string Before()=>Begin("Antes");
 public static string After()=>Begin("Despues");
 static string Begin(string stage){var go=new GameObject("Temporary art capture");go.AddComponent<AfterglowArtCapture>().stage=stage;return "Capture scheduled: "+stage;}
 IEnumerator Start()
 {
  var p=FindAnyObjectByType<AfterglowPlayer>();var controller=p.GetComponent<CharacterController>();controller.enabled=false;p.transform.position=new Vector3(20,.05f,0);controller.enabled=true;p.testControl=true;p.testMove=Vector2.zero;
  yield return new WaitForSeconds(1.2f);
  p.animator.Play("Idle",0,0);p.animator.Update(0);p.animator.speed=0;
  var follow=FindAnyObjectByType<AfterglowCamera>();follow.Snap();yield return new WaitForEndOfFrame();
  var c=follow.GetComponent<Camera>();var text="Stage: "+stage+"\nPlayer: "+p.transform.position.ToString("F5")+"\nCamera position: "+c.transform.position.ToString("F5")+"\nCamera rotation: "+c.transform.eulerAngles.ToString("F5")+"\nFOV: "+c.fieldOfView+"\nOffset: "+follow.offset+"\nLimits: "+follow.minX+", "+follow.maxX+"\nAnticipation: "+follow.anticipation+"\nSmoothing: "+follow.smoothing+"\n";
  Directory.CreateDirectory("Docs/Arte");File.WriteAllText("Docs/Arte/Camara_"+stage+".txt",text);ScreenCapture.CaptureScreenshot("Docs/Arte/N01S02_"+stage+".png");yield return new WaitForEndOfFrame();p.animator.speed=1;Destroy(gameObject);
 }
}
