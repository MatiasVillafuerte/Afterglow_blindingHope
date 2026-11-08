using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
namespace Afterglow.Editor {
public static class AfterglowVisualQuality {
 [MenuItem("Afterglow/Improve Character Clarity")]
 public static void Apply(){
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode first.");
  for(int i=0;i<SceneManager.sceneCount;i++)if(SceneManager.GetSceneAt(i).isDirty)throw new InvalidOperationException("Save open scene changes first.");
  string original=SceneManager.GetActiveScene().path;
  string sheet=AssetDatabase.GUIDToAssetPath("7634fb7576e2b764eb2186c001c4c82b");
  var importer=(TextureImporter)AssetImporter.GetAtPath(sheet);
  importer.textureCompression=TextureImporterCompression.Uncompressed;importer.crunchedCompression=false;
  importer.mipmapEnabled=false;importer.filterMode=FilterMode.Bilinear;importer.maxTextureSize=2048;importer.alphaIsTransparency=true;
  var desktop=importer.GetPlatformTextureSettings("Standalone");desktop.overridden=true;desktop.maxTextureSize=2048;desktop.format=TextureImporterFormat.RGBA32;desktop.textureCompression=TextureImporterCompression.Uncompressed;importer.SetPlatformTextureSettings(desktop);importer.SaveAndReimport();
  foreach(string guid in AssetDatabase.FindAssets("t:Scene",new[]{"Assets/Afterglow/Scenes"})){
   var scene=EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(guid));
   foreach(var follow in UnityEngine.Object.FindObjectsByType<AfterglowCamera>()){
    // Larger screen footprint without changing jump distances, rig scale or collision geometry.
    follow.offset=new Vector3(0,3.1f,-14);float oldMin=follow.minX;follow.minX=7;follow.maxX+=oldMin-7;
    var cam=follow.GetComponent<Camera>();cam.fieldOfView=32;cam.allowDynamicResolution=false;
    var data=cam.GetUniversalAdditionalCameraData();data.antialiasing=AntialiasingMode.None;data.renderPostProcessing=false;
    follow.Snap();EditorUtility.SetDirty(follow);EditorUtility.SetDirty(cam);EditorUtility.SetDirty(data);
   }
   EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);
  }
  if(!string.IsNullOrEmpty(original))EditorSceneManager.OpenScene(original);AssetDatabase.SaveAssets();
 }
}
}
