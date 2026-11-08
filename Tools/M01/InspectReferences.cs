using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;
public class M01ReferenceInspection:MonoBehaviour {
 public static string Main(){new GameObject("Temporary reference inspection").AddComponent<M01ReferenceInspection>();return "Reference sampling started";}
 IEnumerator Start(){
  string dir="Docs/Referencias/Analisis";Directory.CreateDirectory(dir);
  var files=Directory.GetFiles("Reference/Mundo 1/Videos","*.mp4").OrderBy(f=>f).ToArray();
  string log="# Muestreo de referencias — 2026-11-02\n\nSe inspeccionan fotogramas identificados; no se afirma reproducción íntegra ni análisis de audio.\n";
  for(int i=0;i<files.Length;i++){
   var g=new GameObject("Video reader");var v=g.AddComponent<VideoPlayer>();v.playOnAwake=false;v.url=Path.GetFullPath(files[i]);v.audioOutputMode=VideoAudioOutputMode.None;v.renderMode=VideoRenderMode.RenderTexture;var rt=new RenderTexture(960,540,0);v.targetTexture=rt;v.Prepare();float timeout=Time.realtimeSinceStartup+20;
   while(!v.isPrepared&&Time.realtimeSinceStartup<timeout)yield return null;
   if(v.isPrepared){bool ready=false;v.sendFrameReadyEvents=true;v.frameReady+=(source,frame)=>ready=true;double sample=i==0?100:i==1?95:System.Math.Min(40,v.length*.5);v.time=sample;v.Play();timeout=Time.realtimeSinceStartup+20;while(!ready&&Time.realtimeSinceStartup<timeout)yield return null;
    if(ready){yield return new WaitForEndOfFrame();v.Pause();var old=RenderTexture.active;RenderTexture.active=rt;var tex=new Texture2D(960,540,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,960,540),0,0);tex.Apply();string image=dir+"/M01_REF_"+(i+1)+".png";File.WriteAllBytes(image,tex.EncodeToPNG());log+="- "+files[i]+": segundo decodificado "+v.time+", duración "+v.length+", fotograma "+image+".\n";RenderTexture.active=old;Destroy(tex);}
   }else log+="- Falló preparación: "+files[i]+"\n";
   Destroy(g);Destroy(rt);File.WriteAllText(dir+"/Muestreo_M01.md",log);yield return null;
  }Destroy(gameObject);
 }
}

