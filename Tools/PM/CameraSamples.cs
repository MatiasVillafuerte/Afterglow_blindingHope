using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;
public sealed class CameraSamples : MonoBehaviour
{
 public static string Main(){new GameObject("Camera reference sampler").AddComponent<CameraSamples>();return "Sampling";}
 IEnumerator Start(){
 var files=Directory.GetFiles(Path.Combine(Path.GetTempPath(),"AfterglowCameraInput"),"*.mp4").OrderBy(x=>x).ToArray();
 const string output="Docs/Referencias/Analisis/Camara";Directory.CreateDirectory(output);
 var rt=new RenderTexture(960,540,0);var img=new Texture2D(960,540,TextureFormat.RGB24,false);string log="";
 for(int i=0;i<files.Length;i++){
 var v=gameObject.AddComponent<VideoPlayer>();v.url=Path.GetFullPath(files[i]);v.audioOutputMode=VideoAudioOutputMode.None;v.renderMode=VideoRenderMode.RenderTexture;v.targetTexture=rt;v.Prepare();float end=Time.realtimeSinceStartup+12;
 while(!v.isPrepared&&Time.realtimeSinceStartup<end)yield return null;
 log+=files[i]+" duration="+v.length+"\n";
 if(v.isPrepared){v.sendFrameReadyEvents=true;bool ready=false;v.frameReady+=(_,f)=>ready=true;
 foreach(float fraction in new[]{.22f,.55f,.82f}){ready=false;v.time=v.length*fraction;v.Play();end=Time.realtimeSinceStartup+5;while(!ready&&Time.realtimeSinceStartup<end)yield return null;v.Pause();yield return null;
 if(!ready)continue;var old=RenderTexture.active;RenderTexture.active=rt;img.ReadPixels(new Rect(0,0,960,540),0,0);img.Apply();RenderTexture.active=old;File.WriteAllBytes($"{output}/{i}_{fraction:F2}.png",img.EncodeToPNG());log+=$"  {fraction:F2}: {v.time:F2}s\n";}}
 v.Stop();Destroy(v);yield return null;File.WriteAllText(output+"/Muestreo.txt",log);
 }
 Destroy(rt);Destroy(img);Destroy(gameObject);}
}
