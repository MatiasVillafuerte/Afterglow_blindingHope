using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Afterglow.M01;
public static class N1InspectFinal
{
    public static string Main()
    {
        if(EditorApplication.isPlaying)throw new Exception("Stop Play first");
        string report="# Inspección de integración final\n\n";int failures=0;
        void Check(string name,bool ok){report+=$"- {(ok?"PASS":"FAIL")} {name}\n";if(!ok)failures++;}
        int[] expected={13314,9272,12984,8590};
        for(int i=0;i<4;i++)
        {
            string id="N01S0"+(i+1), folder="Assets/Afterglow/Environment/Modelado/Nivel1/";
            var scene=EditorSceneManager.OpenScene(M01Expedition.ScenePath(id));
            var objects=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Transform>(true)).Select(t=>t.gameObject).ToArray();
            Check(id+" sin scripts perdidos",objects.All(g=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(g)==0));
            var environment=objects.Single(g=>g.name=="Nivel1_v05");
            var meshes=environment.GetComponentsInChildren<MeshFilter>(true);
            int triangles=meshes.Sum(m=>m.sharedMesh.triangles.Length/3);
            Check(id+" topología FBX/Unity "+triangles,triangles==expected[i]);
            Check(id+" materiales URP asignados",environment.GetComponentsInChildren<MeshRenderer>(true).Where(r=>!r.GetComponent<TMPro.TMP_Text>()).All(r=>r.sharedMaterials.All(m=>m&&m.shader&&m.shader.name.StartsWith("Universal Render Pipeline/"))));
            Check(id+" prefab de entorno",AssetDatabase.LoadAssetAtPath<GameObject>(folder+id+"_Environment.prefab"));
            var importer=AssetImporter.GetAtPath(folder+id+".fbx") as ModelImporter;
            Check(id+" escala de importación 1",importer&&Mathf.Approximately(importer.globalScale,1));
            if(i==3)
            {
                var bridge=UnityEngine.Object.FindAnyObjectByType<M01CounterweightBridge>();
                Check("Puente y palanca tienen mallas bajo su pivote",bridge.deck.GetComponentInChildren<MeshFilter>()&&bridge.leverArm.GetComponentInChildren<MeshFilter>());
                Check("Pivote sur del puente",Vector3.Distance(bridge.deck.position,new Vector3(2,0,0))<.001f);
            }
        }
        EditorSceneManager.OpenScene(M01Expedition.ScenePath("M01_MENU"));
        report+=$"\nFallos: {failures}\n";File.WriteAllText("Docs/Implementacion/Nivel1/Inspeccion_Final.md",report);
        if(failures>0)throw new Exception(report);return report;
    }
}
