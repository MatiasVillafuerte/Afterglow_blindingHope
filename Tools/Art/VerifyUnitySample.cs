using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using Afterglow;

public static class AfterglowArtVerify
{
 [Serializable] sealed class Manifest {public Asset[] assets=Array.Empty<Asset>();}
 [Serializable] sealed class Asset {public string name="";public float[] dimensionsBlender=Array.Empty<float>();public int triangles=0;}
 static string PathOf(Transform t){return t.parent?PathOf(t.parent)+"/"+t.name:t.name;}
 static string[] Collision(Scene scene)=>scene.GetRootGameObjects().Where(g=>g.name!="ART_N01S02").SelectMany(g=>g.GetComponentsInChildren<Collider>(true)).Select(c=>PathOf(c.transform)+"|"+c.GetType().Name+"|"+c.transform.position.ToString("F5")+"|"+c.transform.eulerAngles.ToString("F5")+"|"+c.transform.lossyScale.ToString("F5")+"|"+c.enabled+"|"+(c is BoxCollider b?b.center.ToString("F5")+b.size.ToString("F5"):c is CharacterController cc?cc.radius+","+cc.height+","+cc.stepOffset+","+cc.center.ToString("F5"):"" )).OrderBy(s=>s).ToArray();
 public static string Main()
 {
  var active=SceneManager.GetActiveScene();var source=EditorSceneManager.OpenScene("Assets/Afterglow/Scenes/M01/N01S02.unity",OpenSceneMode.Additive);
  bool same=Collision(active).SequenceEqual(Collision(source));EditorSceneManager.CloseScene(source,true);SceneManager.SetActiveScene(active);
  var lines=new List<string>{"# Verificación de importación y escena","- Colisiones originales idénticas: "+same};
  var cam=active.GetRootGameObjects().Select(g=>g.GetComponent<AfterglowCamera>()).First(c=>c);
  bool camera=cam.offset==new Vector3(0,3.1f,-14)&&cam.minX==7&&cam.maxX==37&&cam.anticipation==1.5f&&cam.smoothing==.25f&&cam.GetComponent<Camera>().fieldOfView==32;
  lines.Add("- Cámara aprobada conservada: "+camera);
  var manifest=Newtonsoft.Json.JsonConvert.DeserializeObject<Manifest>(File.ReadAllText("Docs/Arte/KitManifest.json"));bool all=same&&camera;
  foreach(var asset in manifest.assets)
  {
   var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Afterglow/Environment/M01Art/Prefabs/"+asset.name+".prefab");
   int tris=0;bool normals=true,uv=true,mats=true;var bounds=new Bounds();bool first=true;
   foreach(var filter in prefab.GetComponentsInChildren<MeshFilter>())
   {
    var mesh=filter.sharedMesh;tris+=mesh.triangles.Length/3;normals&=mesh.normals.Length==mesh.vertexCount&&mesh.normals.All(n=>!float.IsNaN(n.x)&&n.sqrMagnitude>.9f);uv&=mesh.uv.Length==mesh.vertexCount&&mesh.uv.All(v=>!float.IsNaN(v.x)&&!float.IsNaN(v.y));
    var renderer=filter.GetComponent<Renderer>();mats&=renderer.sharedMaterials.All(m=>m&&m.shader.name=="Universal Render Pipeline/Lit");if(first){bounds=renderer.bounds;first=false;}else bounds.Encapsulate(renderer.bounds);
   }
   var expected=new Vector3(asset.dimensionsBlender[0],asset.dimensionsBlender[2],asset.dimensionsBlender[1]);bool scale=Vector3.Distance(bounds.size,expected)<.003f&&prefab.transform.localScale==Vector3.one&&Mathf.Abs(bounds.min.y)<.005f;
   bool pass=tris==asset.triangles&&normals&&uv&&mats&&scale;all&=pass;
   lines.Add("- "+asset.name+": PASS="+pass+", triángulos="+tris+", escala/pivote="+scale+", normales="+normals+", UV="+uv+", materiales URP="+mats);
   if(asset.name=="RailModule")
   {
    int iron=0,wrong=0;foreach(var filter in prefab.GetComponentsInChildren<MeshFilter>()){var mesh=filter.sharedMesh;var materials=filter.GetComponent<Renderer>().sharedMaterials;for(int sub=0;sub<mesh.subMeshCount;sub++){var indices=mesh.GetTriangles(sub);var vertices=mesh.vertices;var ns=mesh.normals;for(int i=0;i<indices.Length;i+=3){int a=indices[i],b=indices[i+1],c=indices[i+2];float y=(vertices[a].y+vertices[b].y+vertices[c].y)/3;float normalY=(ns[a].y+ns[b].y+ns[c].y)/3;if(y>.35f&&normalY>.8f){if(materials[sub].name=="M01_Iron")iron++;else wrong++;}}}}
    bool correct=iron>0&&wrong==0;all&=correct;lines.Add("- Cabezas de ambos rieles con hierro: "+correct+" (triángulos correctos "+iron+", incorrectos "+wrong+")");
   }
  }
  int missing=active.GetRootGameObjects().Sum(g=>g.GetComponentsInChildren<MonoBehaviour>(true).Count(c=>c==null));all&=missing==0;lines.Add("- Componentes perdidos: "+missing);lines.Add("- Resultado conjunto: "+all);File.WriteAllLines("Docs/Arte/Verificacion_Unity.md",lines);return string.Join("\n",lines);
 }
}
