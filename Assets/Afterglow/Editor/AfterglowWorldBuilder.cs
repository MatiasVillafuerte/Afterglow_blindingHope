using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
namespace Afterglow.Editor {
public static class AfterglowWorldBuilder {
 const string Root="Assets/Afterglow/";
 public static readonly string[] Names={"Afterglow_Prototype","Afterglow_Courtyard","Afterglow_Gallery","Afterglow_Summit"};
 [MenuItem("Afterglow/Build Connected World")]
 public static void Build(){
  AfterglowBuilder.Build();
  for(int room=0;room<4;room++){
   if(room>0)EditorSceneManager.OpenScene(Root+"Scenes/Afterglow_Prototype.unity");
   var scene=SceneManager.GetActiveScene();
   foreach(var go in scene.GetRootGameObjects())if(go.GetComponent<MeshRenderer>()||go.GetComponent<AfterglowEnemy>()||go.GetComponent<AfterglowDoor>())UnityEngine.Object.DestroyImmediate(go);
   float length=new[]{44f,38f,50f,32f}[room];
   // Adjoining floor sections change depth while the camera-side boundary stays invisible.
   float[] depths=room==0?new[]{8f,13f,9f}:room==1?new[]{14f,9f,16f}:room==2?new[]{9f,16f,11f}:new[]{12f,15f,10f};
   for(int i=0;i<3;i++){float w=length/3;float d=depths[i];Box("Floor section "+i,new Vector3(w*(i+.5f),-.5f,-30+(d+27)/2),new Vector3(w,1,d+27));Box("Rear wall "+i,new Vector3(w*(i+.5f),7,-3+d),new Vector3(w,14,.35f));
    if(i>0){float previous=depths[i-1];Box("Depth return "+i,new Vector3(w*i,4,-3+(previous+d)/2),new Vector3(.35f,8,Mathf.Abs(previous-d)+.35f));}}
   Box("Left wall",new Vector3(-.25f,4,3),new Vector3(.5f,8,13));Box("Right wall",new Vector3(length+.25f,4,4),new Vector3(.5f,8,16));
   var boundary=Box("Invisible camera-side boundary",new Vector3(length/2,12,-3.35f),new Vector3(length+2,30,.7f));boundary.GetComponent<MeshRenderer>().enabled=false;
   // The foreground lane remains flat; the rear lane offers a separate climb.
   float start=room==2?18:11;
   for(int i=0;i<4;i++)Box("Raised route step "+i,new Vector3(start+i*3,(i+1)*.6f,3.5f),new Vector3(3,(i+1)*1.2f,3));
   Box("Upper landing",new Vector3(start+12,2.4f,3.5f),new Vector3(3,4.8f,4));
   var p=UnityEngine.Object.FindAnyObjectByType<AfterglowPlayer>();p.transform.position=new Vector3(4,.05f,0);
   var follow=UnityEngine.Object.FindAnyObjectByType<AfterglowCamera>();follow.offset=new Vector3(0,3.8f,-17);follow.minX=10;follow.maxX=length-10;follow.GetComponent<Camera>().fieldOfView=40;follow.Snap();
   if(room<3){Vector3 door=room==0?new Vector3(start+12,4.8f,5f):room==1?new Vector3(25,0,2):new Vector3(15,0,5);
    Door(door,Names[room+1]);}
   var enemyPrefab=AssetDatabase.LoadAssetAtPath<GameObject>(Root+"Prefabs/Enemy.prefab");var enemy=(GameObject)PrefabUtility.InstantiatePrefab(enemyPrefab);enemy.transform.position=new Vector3(length-9,.04f,-.5f);
   var path=Root+"Scenes/"+Names[room]+".unity";EditorSceneManager.SaveScene(scene,path);
   var entries=EditorBuildSettings.scenes.ToList();if(!entries.Any(e=>e.path==path))entries.Add(new EditorBuildSettingsScene(path,true));else entries.First(e=>e.path==path).enabled=true;EditorBuildSettings.scenes=entries.ToArray();
  }
  EditorSceneManager.OpenScene(Root+"Scenes/Afterglow_Prototype.unity");AssetDatabase.SaveAssets();AfterglowVisualQuality.Apply();
 }
 static GameObject Box(string name,Vector3 pos,Vector3 size){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.position=pos;g.transform.localScale=size;g.GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(Root+"Materials/Platforms.mat");return g;}
 static void Door(Vector3 pos,string target){var root=new GameObject("Door to "+target);root.transform.position=pos;root.AddComponent<AfterglowDoor>().targetScene=target;
  var opening=Box("Dark opening",pos+Vector3.up*1.3f+Vector3.forward*.1f,new Vector3(1.6f,2.6f,.12f));opening.GetComponent<Renderer>().sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(Root+"Materials/Enemy.mat");UnityEngine.Object.DestroyImmediate(opening.GetComponent<Collider>());opening.transform.SetParent(root.transform,true);
  for(int side=-1;side<=1;side+=2){var frame=Box("Door jamb",pos+new Vector3(side*.95f,1.4f,.1f),new Vector3(.25f,2.8f,.35f));frame.transform.SetParent(root.transform,true);}var top=Box("Door lintel",pos+new Vector3(0,2.85f,.1f),new Vector3(2.15f,.25f,.35f));top.transform.SetParent(root.transform,true);
 }
}
}


