using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;
public class VideoReference:MonoBehaviour {
 public static string Main(){new GameObject("Temporary video inspection").AddComponent<VideoReference>();return "Preparing reference video";}
 IEnumerator Start(){var v=gameObject.AddComponent<VideoPlayer>();v.url=Path.GetFullPath("References/Guia para la camara.mp4");v.audioOutputMode=VideoAudioOutputMode.None;v.renderMode=VideoRenderMode.RenderTexture;var rt=new RenderTexture(1280,720,0);v.targetTexture=rt;v.Prepare();while(!v.isPrepared)yield return null;v.sendFrameReadyEvents=true;bool ready=false;v.frameReady+=(source,frame)=>ready=true;v.time=21;v.Play();float timeout=Time.realtimeSinceStartup+15;while(!ready&&Time.realtimeSinceStartup<timeout)yield return null;yield return new WaitForEndOfFrame();v.Pause();var old=RenderTexture.active;RenderTexture.active=rt;var t=new Texture2D(1280,720,TextureFormat.RGB24,false);t.ReadPixels(new Rect(0,0,1280,720),0,0);t.Apply();File.WriteAllBytes("References/VideoReferenceFrame.png",t.EncodeToPNG());File.WriteAllText("References/VideoInspection.txt","length="+v.length+" time="+v.time+" ready="+ready);RenderTexture.active=old;Destroy(t);Destroy(rt);Destroy(gameObject);}
}

