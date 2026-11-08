using System;
using System.IO;
using System.Linq;
using Afterglow;
using Afterglow.M01;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;
using Object=UnityEngine.Object;
public static class N1LayoutBuilder
{
    const string Base="Assets/Afterglow/Environment/Modelado/";
    [Serializable] class Box{public string name=null;public float[] center=null,size=null;}
    [Serializable] class Layout{public Box[] boxes=null;}
    static Vector3 V(float[] a)=>new Vector3(a[0],a[1],a[2]);
    public static string Main()=>Build(false);
    public static string Detail()=>Build(true);
    static string Build(bool detail)
    {
        if(EditorApplication.isPlaying||Enumerable.Range(0,SceneManager.sceneCount).Any(i=>SceneManager.GetSceneAt(i).isDirty))throw new Exception("Detener Play y conservar cambios de escena antes de montar N1.");
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        Directory.CreateDirectory("Tools/PM/Backups/Antes_N1_v05");
        var gray=AssetDatabase.LoadAssetAtPath<Material>(Base+"Nivel1/Blockout.mat");
        if(!gray){gray=new Material(Shader.Find("Universal Render Pipeline/Lit")){color=new Color(.5f,.53f,.55f)};AssetDatabase.CreateAsset(gray,Base+"Nivel1/Blockout.mat");}
        string report="";
        foreach(string id in new[]{"N01S01","N01S02","N01S03","N01S04"})
        {
            var path=M01Expedition.ScenePath(id);string backup="Tools/PM/Backups/Antes_N1_v05/"+id+".unity";
            if(!File.Exists(backup))File.Copy(path,backup);
            var scene=EditorSceneManager.OpenScene(path);
            var root=scene.GetRootGameObjects().First(g=>g.name.StartsWith("Escenario"));
            var oldBoxes=Newtonsoft.Json.JsonConvert.DeserializeObject<Layout>(File.ReadAllText(Base+id+".json")).boxes.Select(b=>b.name).ToArray();
            foreach(Transform child in root.transform.Cast<Transform>().ToArray())
                if(child.name=="Nivel1_v05"||PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(child.gameObject)==Base+id+".fbx"||(child.GetComponent<BoxCollider>()&&oldBoxes.Contains(child.name)))Object.DestroyImmediate(child.gameObject);
            var environment=new GameObject("Nivel1_v05");environment.transform.SetParent(root.transform,false);
            var asset=AssetDatabase.LoadAssetAtPath<GameObject>(Base+"Nivel1/"+id+".fbx");if(!asset)throw new Exception("Falta FBX "+id);
            var model=(GameObject)PrefabUtility.InstantiatePrefab(asset);model.transform.SetParent(environment.transform,false);
            // Interactive meshes need independent pivots; imported model instances forbid reparenting.
            PrefabUtility.UnpackPrefabInstance(model,PrefabUnpackMode.Completely,InteractionMode.AutomatedAction);
            var axis=model.GetComponentsInChildren<Transform>().First(t=>t.name=="AxisWitness");
            if(Vector3.Distance(axis.position,new Vector3(3,2,7))>.03f)throw new Exception("Ejes FBX incorrectos "+id+axis.position);
            foreach(var mr in model.GetComponentsInChildren<MeshRenderer>())
            {
                string material=mr.name.Replace("Export_","");
                material=material=="BreakableBoulder"?"SlateLight":material=="BridgeDeck"?"Oak":material=="LeverArm"?"Trim":material=="Counterweight"?"Iron":material;
                mr.sharedMaterial=detail?AssetDatabase.LoadAssetAtPath<Material>(Base+material+".mat"):gray;
                if(!mr.sharedMaterial)throw new Exception("Falta material "+material);
            }
            var layout=Newtonsoft.Json.JsonConvert.DeserializeObject<Layout>(File.ReadAllText(Base+"Nivel1/"+id+".json"));
            foreach(var box in layout.boxes){var go=new GameObject(box.name);go.transform.SetParent(environment.transform,false);go.transform.position=V(box.center);go.AddComponent<BoxCollider>().size=V(box.size);}
            if(id=="N01S02")Find<M01StaticOffering>().First(o=>o.sourceId=="EXP.N01S02.0").transform.position=new Vector3(5,1.65f,1);
            if(id=="N01S03")
            {
                var rock=model.GetComponentsInChildren<MeshFilter>().First(f=>f.name=="Export_BreakableBoulder");
                var c=rock.gameObject.AddComponent<BoxCollider>();c.center=rock.sharedMesh.bounds.center;c.size=rock.sharedMesh.bounds.size;rock.gameObject.AddComponent<M01BreakableRock>();
                Find<M01StaticOffering>().First(o=>o.sourceId=="EXP.N01S03.0").transform.position=new Vector3(8,.35f,1.6f);
            }
            if(id=="N01S04")
            {
                Find<M01QuestObject>().Single().transform.position=new Vector3(8,.35f,8);
                Find<M01Investigation>().Single().transform.position=new Vector3(-4,1.8f,10);
                foreach(var label in Find<TextMeshPro>())if(label.text.StartsWith("Casco"))label.transform.position=new Vector3(8,1.35f,8);else if(label.text.StartsWith("Derrumbe"))label.transform.position=new Vector3(-4,4.5f,10);
                var dialogue=Object.FindAnyObjectByType<M01Dialogue>();
                Transform Mesh(string name)=>model.GetComponentsInChildren<Transform>().First(t=>t.name=="Export_"+name);
                var pivot=new GameObject("Puente · pivote sur").transform;pivot.SetParent(environment.transform,false);pivot.position=new Vector3(2,0,0);
                Mesh("BridgeDeck").SetParent(pivot,true);
                var c=pivot.gameObject.AddComponent<BoxCollider>();c.center=new Vector3(0,-.12f,2.5f);c.size=new Vector3(3,.24f,5.2f);
                var lever=new GameObject("Palanca del contrapeso");lever.transform.SetParent(environment.transform,false);lever.transform.position=new Vector3(4,0,-1.2f);
                var armPivot=new GameObject("Pivote palanca").transform;armPivot.SetParent(environment.transform,false);armPivot.position=new Vector3(4,.55f,-1.2f);Mesh("LeverArm").SetParent(armPivot,true);
                var bridge=lever.AddComponent<M01CounterweightBridge>();bridge.deck=pivot;bridge.crossing=c;bridge.leverArm=armPivot;bridge.counterweight=Mesh("Counterweight");bridge.dialogue=dialogue;
                var hint=lever.AddComponent<M01StoryPoint>();hint.dialogue=dialogue;hint.eventId="N1E.BRIDGE_HINT";hint.automatic=true;hint.radius=3.4f;hint.lineIds=new[]{"N1E.02"};
                var sign=new GameObject("Palanca E").AddComponent<TextMeshPro>();sign.transform.SetParent(environment.transform,false);sign.transform.position=new Vector3(1.2f,1.8f,-2.2f);sign.font=dialogue.body.font;sign.fontSize=2.6f;sign.alignment=TextAlignmentOptions.Center;sign.text="Bajar puente [E]";bridge.prompt=sign.gameObject;
            }
            var player=Object.FindAnyObjectByType<AfterglowPlayer>();player.transform.position=M01Expedition.SectionEntrance(id);
            foreach(var label in Find<TextMeshPro>())
            {
                label.fontSize=2.1f;
                if(!label.GetComponent<M01WorldHint>())label.gameObject.AddComponent<M01WorldHint>();
            }
            // Camera fields and all prologue/map scenes remain untouched.
            PrefabUtility.SaveAsPrefabAsset(environment,Base+"Nivel1/"+id+"_Environment.prefab");
            EditorSceneManager.SaveScene(scene);report+=id+": "+layout.boxes.Length+" colliders; escala y ejes correctos.\n";
        }
        AssetDatabase.SaveAssets();EditorSceneManager.OpenScene(M01Expedition.ScenePath("M01_MENU"));
        File.WriteAllText("Docs/Implementacion/Nivel1/Importacion_"+(detail?"Final":"Blockout")+".md",report);return report;
    }
    static T[] Find<T>()where T:Object=>Object.FindObjectsByType<T>();
}

