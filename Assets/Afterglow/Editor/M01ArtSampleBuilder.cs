using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Afterglow.Editor
{
    public static class M01ArtSampleBuilder
    {
        public const string ScenePath="Assets/Afterglow/Scenes/Art/M01_N01S02_Arte.unity";
        const string Root="Assets/Afterglow/Environment/M01Art";
        const string Owned="ART_N01S02";
        [Serializable] sealed class Manifest { public Asset[] assets; }
        [Serializable] sealed class Asset { public string name,file; public string[] materials; public int triangles; }
        static readonly Dictionary<string,Material> Materials=new Dictionary<string,Material>();
        static readonly Dictionary<string,GameObject> Prefabs=new Dictionary<string,GameObject>();

        [MenuItem("Afterglow/Arte/Build N01S02 Sample")]
        public static void Build()
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode before building the art sample.");
            var scene=SceneManager.GetActiveScene();
            if(scene.path!=ScenePath)throw new InvalidOperationException("Open the separate art sample first: "+ScenePath);
            if(scene.isDirty)throw new InvalidOperationException("Save scene edits before regenerating the owned art group.");
            Directory.CreateDirectory(Root+"/Materials");Directory.CreateDirectory(Root+"/Prefabs");Directory.CreateDirectory(Root+"/Meshes");
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            Materials.Clear();Prefabs.Clear();
            foreach(string path in Directory.GetFiles(Root+"/Textures","*.png"))
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath(path.Replace('\\','/'));
                importer.textureType=TextureImporterType.Default;importer.sRGBTexture=true;importer.mipmapEnabled=true;
                importer.textureCompression=TextureImporterCompression.Uncompressed;importer.wrapMode=TextureWrapMode.Repeat;importer.anisoLevel=4;importer.SaveAndReimport();
            }
            var rock=Mat("M01_Stone",new Color(.69f,.80f,.89f),"Stone_BaseColor",.09f);
            Mat("M01_Timber",Color.white,"Timber_BaseColor",.12f);
            Mat("M01_Iron",new Color(.25f,.31f,.36f),null,.35f,.55f);
            var ground=Mat("Ground",new Color(.87f,.68f,.49f),"Stone_BaseColor",.05f);
            ground.mainTextureScale=new Vector2(16,5);
            var platform=Mat("Platform",new Color(.72f,.73f,.68f),"Stone_BaseColor",.09f);
            var platformTop=Mat("PlatformTop",new Color(.96f,.93f,.84f),"Stone_BaseColor",.07f);
            var deep=Mat("DeepRock",new Color(.055f,.10f,.14f),null,.02f);
            var emission=Mat("Lamp",new Color(1,.73f,.34f),null,.1f);
            emission.EnableKeyword("_EMISSION");emission.SetColor("_EmissionColor",new Color(1,.52f,.12f)*2);
            var manifest=JsonUtility.FromJson<Manifest>(File.ReadAllText("Docs/Arte/KitManifest.json"));
            foreach(var item in manifest.assets)MakePrefab(item);
            var previous=scene.GetRootGameObjects().FirstOrDefault(g=>g.name==Owned);
            if(previous)UnityEngine.Object.DestroyImmediate(previous);
            var owner=new GameObject(Owned);
            var random=new System.Random(40201);
            foreach(var root in scene.GetRootGameObjects())
            {
                if(root.name=="Ground")root.GetComponent<Renderer>().sharedMaterial=ground;
                if(root.name=="Rear")root.GetComponent<Renderer>().enabled=false;
                if(root.name=="Side")root.GetComponent<Renderer>().sharedMaterial=deep;
                if(root.name.StartsWith("Escalón")||root.name=="Plataforma superior")
                {
                    root.GetComponent<Renderer>().sharedMaterials=new[]{platform,platformTop};
                    root.GetComponent<MeshFilter>().sharedMesh=BeveledBlock();
                }
            }
            // All dressing is non-blocking; the original floor, steps and boundaries own collision.
            Box("Deep wall",owner.transform,new Vector3(22,4.8f,8.8f),new Vector3(48,11,.6f),deep);
            for(int row=0;row<2;row++)for(int i=0;i<13;i++)
                Place("Rock_C",owner.transform,new Vector3(i*3.8f-1,row*3.1f-.15f,8.1f),new Vector3(2.3f,2.6f,.9f),new Vector3(0,i*17%35,0));
            for(int row=0;row<3;row++)for(int i=0;i<19;i++)
            {
                string variant=new[]{"Rock_A","Rock_B","Rock_C"}[(i+row)%3];
                float x=i*2.55f-1+(row%2)*1.1f;
                var instance=Place(variant,owner.transform,new Vector3(x,row*2.65f-.35f,7.15f+row*.4f),new Vector3(1.9f,1.65f,1.2f),new Vector3(0,(float)random.NextDouble()*50-25,0));
                instance.name="Wall "+row+"."+i+" · "+variant;
            }
            for(int i=0;i<7;i++)
            {
                float x=3+i*6.2f;
                Place("TimberSupport",owner.transform,new Vector3(x,0,5.15f),Vector3.one,Vector3.zero);
                Lamp(owner.transform,x-1.25f,emission);
            }
            for(int i=0;i<14;i++)
                Place("RailModule",owner.transform,new Vector3(1.5f+i*3,0,-3.6f),new Vector3(1,.22f,1),Vector3.zero);
            // Small rubble remains at the wall; nothing is scattered onto the authored stair route.
            for(int i=0;i<23;i++)
                Place(i%2==0?"Rock_A":"Rock_C",owner.transform,new Vector3(i*1.85f,.01f,5.25f),new Vector3(.45f,.35f,.4f),new Vector3(0,i*37%180,0));
            for(int i=0;i<9;i++)
                Place("Rock_A",owner.transform,new Vector3(i*5.1f,-.2f,-5.8f),new Vector3(.75f,.43f,.65f),new Vector3(0,i*27,0));
            var existingLight=scene.GetRootGameObjects().Select(g=>g.GetComponent<Light>()).FirstOrDefault(l=>l&&l.type==LightType.Directional);
            if(existingLight){existingLight.color=new Color(1,.87f,.70f);existingLight.intensity=1.35f;existingLight.shadows=LightShadows.Soft;existingLight.shadowStrength=.65f;existingLight.transform.rotation=Quaternion.Euler(48,-35,0);}
            RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor=new Color(.29f,.38f,.48f);
            RenderSettings.ambientEquatorColor=new Color(.23f,.28f,.31f);
            RenderSettings.ambientGroundColor=new Color(.20f,.16f,.12f);
            RenderSettings.fog=false;
            var controls=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<TMPro.TMP_Text>(true)).FirstOrDefault(t=>t.name=="Controls");
            if(controls)controls.color=new Color(.95f,.95f,.92f);
            // Camera's transform, FOV and follow parameters are deliberately never assigned here.
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
        }

        static Material Mat(string name,Color color,string texture,float smooth,float metallic=0)
        {
            string path=Root+"/Materials/"+name+".mat";
            var material=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(!material){material=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(material,path);}
            material.color=color;material.SetFloat("_Smoothness",smooth);material.SetFloat("_Metallic",metallic);material.enableInstancing=true;
            if(texture!=null)material.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Textures/"+texture+".png");
            Materials[name]=material;return material;
        }
        static void MakePrefab(Asset item)
        {
            string path=Root+"/Models/"+item.file;
            var importer=(ModelImporter)AssetImporter.GetAtPath(path);
            importer.globalScale=1;importer.useFileScale=true;importer.importAnimation=false;importer.importCameras=false;importer.importLights=false;
            importer.materialImportMode=ModelImporterMaterialImportMode.ImportStandard;importer.importNormals=ModelImporterNormals.Import;
            foreach(string name in item.materials)importer.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material),name),Materials[name]);
            importer.isReadable=true;importer.meshCompression=ModelImporterMeshCompression.Off;importer.SaveAndReimport();
            var model=AssetDatabase.LoadAssetAtPath<GameObject>(path);var go=(GameObject)PrefabUtility.InstantiatePrefab(model);
            PrefabUtility.UnpackPrefabInstance(go,PrefabUnpackMode.Completely,InteractionMode.AutomatedAction);go.name=item.name;
            foreach(var renderer in go.GetComponentsInChildren<MeshRenderer>())
            {
                renderer.sharedMaterials=renderer.sharedMaterials.Select(material=>material&&Materials.ContainsKey(material.name)?Materials[material.name]:material).ToArray();
                if(renderer.sharedMaterials.Any(material=>!material||material.shader.name!="Universal Render Pipeline/Lit"))throw new InvalidOperationException("Material remap failed: "+item.name);
            }
            // Reusable props have simple colliders; placed scenery explicitly disables them.
            if(item.name=="TimberSupport")
            {
                ColliderBox(go,new Vector3(-1.85f,2.1f,0),new Vector3(.5f,4.25f,.55f));
                ColliderBox(go,new Vector3(1.85f,2.1f,0),new Vector3(.5f,4.25f,.55f));
                ColliderBox(go,new Vector3(0,4.16f,0),new Vector3(4.35f,.45f,.6f));
            }
            else
            {
                var bounds=new Bounds();bool first=true;
                foreach(var filter in go.GetComponentsInChildren<MeshFilter>()){var mesh=filter.sharedMesh;var b=new Bounds(go.transform.InverseTransformPoint(filter.transform.TransformPoint(mesh.bounds.center)),Vector3.Scale(mesh.bounds.size,filter.transform.localScale));if(first){bounds=b;first=false;}else bounds.Encapsulate(b);}
                ColliderBox(go,bounds.center,bounds.size);
            }
            Prefabs[item.name]=PrefabUtility.SaveAsPrefabAsset(go,Root+"/Prefabs/"+item.name+".prefab");UnityEngine.Object.DestroyImmediate(go);
        }
        static void ColliderBox(GameObject go,Vector3 center,Vector3 size){var collider=go.AddComponent<BoxCollider>();collider.center=center;collider.size=size;}
        static GameObject Place(string key,Transform parent,Vector3 position,Vector3 scale,Vector3 angles)
        {
            var go=(GameObject)PrefabUtility.InstantiatePrefab(Prefabs[key]);go.transform.SetParent(parent,false);go.transform.position=position;go.transform.localScale=scale;go.transform.eulerAngles=angles;
            foreach(var collider in go.GetComponentsInChildren<Collider>())collider.enabled=false;
            return go;
        }
        static void Lamp(Transform parent,float x,Material emission)
        {
            var shell=GameObject.CreatePrimitive(PrimitiveType.Sphere);shell.name="Work lamp";shell.transform.SetParent(parent,false);shell.transform.position=new Vector3(x,3.25f,4.55f);shell.transform.localScale=new Vector3(.28f,.4f,.28f);UnityEngine.Object.DestroyImmediate(shell.GetComponent<Collider>());shell.GetComponent<Renderer>().sharedMaterial=emission;
            var light=new GameObject("Warm pool").AddComponent<Light>();light.transform.SetParent(parent,false);light.transform.position=shell.transform.position+Vector3.back*.3f;light.type=LightType.Point;light.color=new Color(1,.72f,.40f);light.intensity=2.4f;light.range=8;light.shadows=LightShadows.None;
        }
        static GameObject Box(string name,Transform parent,Vector3 position,Vector3 size,Material material)
        {
            var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=name;go.transform.SetParent(parent,false);go.transform.position=position;go.transform.localScale=size;UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());go.GetComponent<Renderer>().sharedMaterial=material;return go;
        }
        static Mesh BeveledBlock()
        {
            string path=Root+"/Meshes/Platform_Beveled.asset";var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(path);bool fresh=!mesh;
            var outline=new[]{new Vector2(-.46f,-.5f),new Vector2(.46f,-.5f),new Vector2(.5f,-.46f),new Vector2(.5f,.46f),new Vector2(.46f,.5f),new Vector2(-.46f,.5f),new Vector2(-.5f,.46f),new Vector2(-.5f,-.46f)};
            var vertices=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();
            var topTriangles=new List<int>();
            Action<Vector3[],bool> face=(points,flip)=>{int start=vertices.Count;vertices.AddRange(points);var n=Vector3.Cross(points[1]-points[0],points[2]-points[0]).normalized;var target=n.y>.6f?topTriangles:triangles;foreach(var p in points)uv.Add(Mathf.Abs(n.z)>.6f?new Vector2(p.x+.5f,p.y+.5f):Mathf.Abs(n.x)>.6f?new Vector2(p.z+.5f,p.y+.5f):new Vector2(p.x+.5f,p.z+.5f));for(int i=1;i<points.Length-1;i++){target.Add(start);target.Add(start+(flip?i+1:i));target.Add(start+(flip?i:i+1));}};
            face(outline.Select(p=>new Vector3(p.x,p.y,-.5f)).ToArray(),true);face(outline.Select(p=>new Vector3(p.x,p.y,.5f)).ToArray(),false);
            for(int i=0;i<8;i++){var a=outline[i];var b=outline[(i+1)%8];face(new[]{new Vector3(a.x,a.y,-.5f),new Vector3(b.x,b.y,-.5f),new Vector3(b.x,b.y,.5f),new Vector3(a.x,a.y,.5f)},false);}
            if(!mesh)mesh=new Mesh{name="Platform beveled editable"};else mesh.Clear();mesh.SetVertices(vertices);mesh.SetUVs(0,uv);mesh.subMeshCount=2;mesh.SetTriangles(triangles,0);mesh.SetTriangles(topTriangles,1);mesh.RecalculateNormals();mesh.RecalculateBounds();if(fresh)AssetDatabase.CreateAsset(mesh,path);else EditorUtility.SetDirty(mesh);return mesh;
        }
    }
}
