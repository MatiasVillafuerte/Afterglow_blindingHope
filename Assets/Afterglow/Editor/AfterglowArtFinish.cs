using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Afterglow.M01;

namespace Afterglow.Editor
{
    public static class AfterglowArtFinish
    {
        const string Folder="Assets/Afterglow/Environment/Modelado/";
        [Serializable] sealed class Surface { public string name, family; public float[] color; }
        [Serializable] sealed class Library { public Surface[] materials; }

        [MenuItem("Afterglow/Arte/Aplicar acabado pintado")]
        public static void Apply()
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode before applying art.");
            if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Save the current scene first.");
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach(string path in Directory.GetFiles(Folder+"Textures","*.png"))
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath(path.Replace('\\','/'));
                importer.sRGBTexture=path.Contains("_BC");
                importer.alphaIsTransparency=false;
                importer.wrapMode=TextureWrapMode.Repeat;
                importer.filterMode=FilterMode.Bilinear;
                importer.anisoLevel=4;
                importer.mipmapEnabled=true;
                importer.maxTextureSize=512;
                importer.textureCompression=TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }
            var library=JsonUtility.FromJson<Library>(File.ReadAllText(Folder+"ArtStyle.json"));
            foreach(var entry in library.materials)
            {
                string path=Folder+entry.name+".mat";
                var material=AssetDatabase.LoadAssetAtPath<Material>(path);
                if(!material)
                {
                    material=new Material(Shader.Find("Universal Render Pipeline/Lit"));
                    AssetDatabase.CreateAsset(material,path);
                }
                material.color=new Color(entry.color[0],entry.color[1],entry.color[2]);
                material.SetFloat("_Smoothness",entry.name=="Water"?.65f:.22f);
                if(!string.IsNullOrEmpty(entry.family))
                {
                    material.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Folder+"Textures/T_"+entry.family+"_BC.png"));
                    material.SetTexture("_MetallicGlossMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Folder+"Textures/T_"+entry.family+"_MS.png"));
                    material.EnableKeyword("_METALLICSPECGLOSSMAP");
                }
                if(entry.name=="Lamp")
                {
                    material.EnableKeyword("_EMISSION");
                    material.SetColor("_EmissionColor",new Color(1,.53f,.13f)*1.2f);
                }
                EditorUtility.SetDirty(material);
            }
            foreach(string id in new[]{"PRO_CASA","PRO_PATIO","N01S01","N01S02","N01S03","N01S04","M01_MAPA"})
            {
                var scene=EditorSceneManager.OpenScene(M01Expedition.ScenePath(id));
                foreach(var renderer in scene.GetRootGameObjects().SelectMany(root=>root.GetComponentsInChildren<MeshRenderer>(true)))
                {
                    if(!renderer.name.StartsWith("Export_",StringComparison.Ordinal))continue;
                    string name=renderer.name.Substring(7);
                    if(name=="BreakableBoulder")name="SlateLight";
                    var material=AssetDatabase.LoadAssetAtPath<Material>(Folder+name+".mat");
                    if(material)renderer.sharedMaterial=material;
                }
                bool mine=id.StartsWith("N01",StringComparison.Ordinal);
                var key=UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include)
                    .FirstOrDefault(light=>light.type==LightType.Directional&&light.name=="Luz principal");
                if(key)
                {
                    key.intensity=mine?1.08f:1.2f;
                    key.color=mine?new Color(1,.80f,.63f):new Color(1,.95f,.84f);
                    key.shadowStrength=.68f;
                    key.shadowBias=.035f;
                    key.shadowNormalBias=.25f;
                }
                if(mine)RenderSettings.ambientLight=new Color(.54f,.43f,.44f);
                if(id=="PRO_CASA")
                {
                    RenderSettings.ambientLight=new Color(.64f,.56f,.48f);
                    var lamp=GameObject.Find("ART_BedsideLight");
                    if(!lamp)lamp=new GameObject("ART_BedsideLight");
                    lamp.transform.position=new Vector3(.45f,2,2.65f);
                    var light=lamp.GetComponent<Light>();
                    if(!light)light=lamp.AddComponent<Light>();
                    light.type=LightType.Point;light.color=new Color(1,.72f,.38f);light.intensity=2;light.range=5.5f;
                }
                EditorSceneManager.SaveScene(scene);
            }
            AssetDatabase.SaveAssets();
            EditorSceneManager.OpenScene(M01Expedition.ScenePath("M01_MENU"));
        }
    }
}
