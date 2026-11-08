using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Afterglow;
using Afterglow.M01;
public static class PMInspectProduction
{
 public static string Main()
 {
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop first");string report="# PM19: inspección de producción\n\n";int failures=0;string reference=null;
  foreach(string id in new[]{"M01_Base","M01_MENU","PRO_CASA","PRO_PATIO","M01_ACCESO","M01_PUEBLO","N01S02","N01S03","N01S04"})
  {
   var scene=EditorSceneManager.OpenScene(M01Expedition.ScenePath(id));var camera=UnityEngine.Object.FindAnyObjectByType<AfterglowCamera>();var p=UnityEngine.Object.FindAnyObjectByType<AfterglowPlayer>();
   string parameters=camera.offset+"/"+camera.anticipation+"/"+camera.smoothing+"/"+camera.minX+"/"+camera.maxX+"/"+camera.GetComponent<Camera>().fieldOfView;if(reference==null)reference=parameters;
   int missing=scene.GetRootGameObjects().Sum(go=>go.GetComponentsInChildren<Transform>(true).Sum(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)));
   bool pass=parameters==reference&&missing==0&&p.visual.GetComponentsInChildren<SpriteRenderer>(true).Length>0&&p.animator.runtimeAnimatorController;
   if(!pass)failures++;report+="- "+id+": "+(pass?"PASS":"FAIL")+"; cámara "+parameters+"; scripts ausentes "+missing+"\n";
   if(id=="M01_ACCESO")
   {
    var roots=scene.GetRootGameObjects();int mini=roots.Count(go=>go.name=="PM_Miniature"),board=roots.Count(go=>go.name=="PM_RouteBoard");int views=roots.SelectMany(go=>go.GetComponentsInChildren<Transform>(true)).Count(t=>t.name=="PM_MapView");
    bool unique=mini==1&&board==1&&views==1&&UnityEngine.Object.FindObjectsByType<M01Altar>().Length==1;if(!unique)failures++;report+="- Un solo tablero, mapa, maqueta y altar: "+(unique?"PASS":"FAIL")+"\n";
   }
  }
  string menu=M01Expedition.ScenePath("M01_MENU");bool startup=EditorBuildSettings.scenes.First(s=>s.enabled).path==menu&&EditorSceneManager.playModeStartScene&&AssetDatabase.GetAssetPath(EditorSceneManager.playModeStartScene)==menu;if(!startup)failures++;report+="- Arranque de build y Play en menú real: "+(startup?"PASS":"FAIL")+"\n\nFallos: "+failures+"\n";File.WriteAllText("Docs/Implementacion/PM/Inspeccion_Produccion.md",report);EditorSceneManager.OpenScene(menu);return report;
 }
}
