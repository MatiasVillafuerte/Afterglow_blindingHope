using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using Afterglow;
public static class M01InspectScenes
{
 public static string Main()
 {
  var active=SceneManager.GetActiveScene();var lines=new List<string>{"# Inspección final de escenas"};
  foreach(var file in Directory.GetFiles("Assets/Afterglow/Scenes/M01","*.unity"))
  {
   string path=file.Replace('\\','/');var scene=SceneManager.GetSceneByPath(path);bool opened=!scene.IsValid()||!scene.isLoaded;if(opened)scene=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);
   var roots=scene.GetRootGameObjects();var camera=roots.SelectMany(r=>r.GetComponentsInChildren<AfterglowCamera>(true)).Single();
   bool kept=camera.offset==new Vector3(0,3.1f,-14)&&camera.minX==7&&camera.maxX==37&&Mathf.Approximately(camera.anticipation,1.5f)&&Mathf.Approximately(camera.smoothing,.25f)&&Mathf.Approximately(camera.GetComponent<Camera>().fieldOfView,32);
   int missing=roots.Sum(r=>r.GetComponentsInChildren<MonoBehaviour>(true).Count(c=>c==null));
   lines.Add("- "+scene.name+": cámara conservada="+kept+", componentes perdidos="+missing+", cámara conectada="+(camera.player!=null));
   if(opened)EditorSceneManager.CloseScene(scene,true);
  }
  SceneManager.SetActiveScene(active);File.WriteAllLines("Docs/Implementacion/Inspeccion_Escenas_M01.md",lines);return string.Join("\n",lines);
 }
}
