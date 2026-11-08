using System;
using System.IO;
using System.Linq;
using Afterglow;
using Afterglow.M01;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class InspectSpatialWorld
{
    public static string Main()
    {
        const string evidence="Docs/Implementacion/Camara_Espacial";
        string report="# Modelos importados, escenas y guardados anteriores\n\n";
        int failures=0;
        void Check(string name,bool pass)
        {
            if(!pass)failures++;
            report+=$"- {(pass?"PASS":"FAIL")} {name}\n";
        }

        string json=File.ReadAllText("ArtSource/Blender/Modelado_Afterglow/Auditoria.json");
        foreach(var model in JArray.Parse(json))
        {
            string id=(string)model["room"];
            int expected=(int)model["triangles"];
            var scene=EditorSceneManager.OpenScene(M01Expedition.ScenePath(id));
            var meshes=scene.GetRootGameObjects().SelectMany(o=>o.GetComponentsInChildren<MeshFilter>(true))
                .Where(m=>m.name.StartsWith("Export_",StringComparison.Ordinal)).ToArray();
            long triangles=meshes.Sum(m=>Enumerable.Range(0,m.sharedMesh.subMeshCount).Sum(i=>(long)m.sharedMesh.GetIndexCount(i)/3));
            Check(id+" conserva "+expected+" triángulos de Blender (Unity: "+triangles+")",triangles==expected);
            Check(id+" usa materiales URP",meshes.Length>0&&meshes.All(m=>m.GetComponent<MeshRenderer>().sharedMaterial.shader.name=="Universal Render Pipeline/Lit"));
            var main=UnityEngine.Object.FindAnyObjectByType<AfterglowCamera>();
            if(id!="M01_MAPA")Check(id+" tiene seguimiento espacial",main.followDepth);
            int missing=scene.GetRootGameObjects().SelectMany(o=>o.GetComponentsInChildren<Transform>(true)).Sum(t=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject));
            Check(id+" tiene referencias de componentes válidas",missing==0);
        }

        var paths=EditorBuildSettings.scenes.Where(s=>s.enabled).Select(s=>s.path).ToArray();
        Check("Arranque normal y mapa/N1 independientes",paths[0]==M01Expedition.ScenePath("M01_MENU")&&paths.Contains(M01Expedition.ScenePath("M01_MAPA"))&&paths.Contains(M01Expedition.ScenePath("N01S01"))&&!paths.Contains(M01Expedition.ScenePath("M01_ACCESO")));

        EditorSceneManager.OpenScene(M01Expedition.ScenePath("M01_MENU"));
        report+=$"\nFallos: {failures}\n";
        File.WriteAllText(evidence+"/Inspeccion.md",report);
        return report;
    }

    public static string SaveChecks()
    {
        if(!Application.isPlaying)throw new InvalidOperationException("Run save validation in Play Mode.");
        string report="# Migración de posiciones y progreso en Play Mode\n\n";
        int failures=0;
        void Check(string name,bool pass)
        {
            if(!pass)failures++;
            report+=$"- {(pass?"PASS":"FAIL")} {name}\n";
        }
        Directory.CreateDirectory("Tools/PM/TestData");
        const string fixture="Tools/PM/TestData/spatial_schema2.json";
        var progress=new M01Progress { money=41 };
        progress.Active.health=6;
        progress.contributions[0]=2;
        progress.events.AddRange(new[]{"P01.LETTER","P01.BELL","P02.CAPTAIN","M01.WORLD_STARTED"});
        var save=new M01SaveFile{schema=2,scene=M01Expedition.ScenePath("M01_ACCESO"),position=new Vector3(31,.04f,0),progress=progress};
        bool Read(out M01SaveFile migrated)
        {
            File.WriteAllText(fixture,JsonUtility.ToJson(save));
            return M01Save.ReadFile(fixture,out migrated,out _);
        }
        Check("Guardado antiguo tras capitán carga mapa",Read(out var afterCaptain)&&afterCaptain.scene==M01Expedition.ScenePath("M01_MAPA"));
        progress.events.Add("M01.N01_ENTERED");
        progress.phoneOwned=true;
        Check("Acceso antiguo migra a entrada real",Read(out var afterEntry)&&afterEntry.scene==M01Expedition.ScenePath("N01S01")&&afterEntry.position==M01Expedition.MineEntrance);
        Check("Migración conserva salud, dinero y crédito",afterEntry!=null&&afterEntry.progress.Active.health==6&&afterEntry.progress.money==41&&afterEntry.progress.contributions[0]==2&&!afterEntry.progress.opened[0]);
        save.scene=M01Expedition.ScenePath("N01S03");
        Check("Guardado antiguo de galería conserva sección y se recoloca en suelo seguro",Read(out var gallery)&&gallery.scene==save.scene&&gallery.position==M01Expedition.SectionEntrance("N01S03"));
        save.schema=3;save.position=new Vector3(-3,0,2);
        Check("Guardado actual conserva coordenadas exactas",Read(out var current)&&current.position==save.position);

        report+=$"\nFallos: {failures}\n";
        File.WriteAllText("Docs/Implementacion/Camara_Espacial/Guardados.md",report);
        return report;
    }
}
