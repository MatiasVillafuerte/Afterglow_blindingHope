using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Afterglow.M01;

namespace Afterglow.Editor
{
    // Focused production pass: never regenerates galleries or the independent art sample.
    public static class PMProductionBuilder
    {
        const string Folder="Assets/Afterglow/Environment/Prologue";
        static Transform root;
        static Material cream,wood,gold,roof,grass,stone,iron,paper,purple,leaf,water;
        public static void BuildPrologue()
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode first");
            if(Enumerable.Range(0,SceneManager.sceneCount).Any(i=>SceneManager.GetSceneAt(i).isDirty))throw new InvalidOperationException("Unsaved scene changes; save first");
            Directory.CreateDirectory(Folder);Directory.CreateDirectory("Tools/PM/Backups");AssetDatabase.Refresh();
            cream=Mat("Marfil",new Color(.88f,.81f,.66f));wood=Mat("Nogal",new Color(.25f,.105f,.055f));gold=Mat("Laton",new Color(.62f,.39f,.12f));roof=Mat("Teja",new Color(.43f,.13f,.075f));grass=Mat("Cesped",new Color(.22f,.37f,.17f));stone=Mat("Arenisca",new Color(.62f,.56f,.43f));iron=Mat("Hierro",new Color(.055f,.085f,.085f));paper=Mat("Papel",new Color(1,.96f,.8f));purple=Mat("Terciopelo",new Color(.27f,.12f,.31f));leaf=Mat("Follaje",new Color(.12f,.29f,.12f));water=Mat("Agua",new Color(.18f,.57f,.59f));
            foreach(var id in new[]{"PRO_CASA","PRO_PATIO"})
            {
                string path=M01Expedition.ScenePath(id);string backup="Tools/PM/Backups/"+id+".unity";if(!File.Exists(backup))File.Copy(path,backup);
                var scene=EditorSceneManager.OpenScene(path);
                foreach(var go in scene.GetRootGameObjects())
                {
                    if(go.GetComponent<AfterglowPlayer>()||go.GetComponent<Camera>()||go.GetComponent<AfterglowHUD>()||go.GetComponent<M01Expedition>()||go.GetComponent<M01Prologue>())continue;
                    UnityEngine.Object.DestroyImmediate(go);
                }
                root=new GameObject("ART_PROLOGO_"+id).transform;
                var player=UnityEngine.Object.FindAnyObjectByType<AfterglowPlayer>();
                var rules=player.GetComponent<M01PlayerRules>();rules.foundationTestKit=false;rules.hasPickaxe=false;
                player.transform.position=id=="PRO_CASA"?new Vector3(7,.05f,0):new Vector3(9,.05f,3.8f);
                rules.sectionEntrance=rules.levelEntrance=player.transform.position;
                var dialogue=UnityEngine.Object.FindAnyObjectByType<M01Dialogue>();
                var controls=dialogue.transform.Find("Controls");if(controls){controls.GetComponent<TMP_Text>().text="Flechas: caminar    Espacio: saltar    E: leer / abrir puerta / hablar";controls.GetComponent<TMP_Text>().color=Color.white;}
                var light=new GameObject("Sol del prologo").AddComponent<Light>();light.transform.SetParent(root);light.type=LightType.Directional;light.intensity=id=="PRO_CASA"?1.05f:1.6f;light.transform.rotation=Quaternion.Euler(48,-28,0);
                RenderSettings.ambientLight=new Color(.65f,.65f,.68f);RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;
                if(id=="PRO_CASA")Bedroom(dialogue);else Garden(dialogue);
                UnityEngine.Object.FindAnyObjectByType<AfterglowCamera>().Snap();
                EditorSceneManager.SaveScene(scene);
            }
            var menu=M01Expedition.ScenePath("M01_MENU");
            EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(menu,true)}.Concat(EditorBuildSettings.scenes.Where(s=>s.path!=menu)).ToArray();
            EditorSceneManager.playModeStartScene=AssetDatabase.LoadAssetAtPath<SceneAsset>(menu);
            AssetDatabase.SaveAssets();EditorSceneManager.OpenScene(menu);
            Debug.Log("PM03–08 production prologue built; galleries and camera preserved");
        }
        static Material Mat(string name,Color color){string path=Folder+"/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(!m){m=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(m,path);}m.color=color;m.SetFloat("_Smoothness",name=="Agua"?.75f:.25f);return m;}
        static GameObject Part(string name,Vector3 pos,Vector3 size,Material material,PrimitiveType type=PrimitiveType.Cube,bool collision=true)
        {
            var go=GameObject.CreatePrimitive(type);go.name=name;go.transform.SetParent(root);go.transform.position=pos;go.transform.localScale=size;go.GetComponent<Renderer>().sharedMaterial=material;
            if(!collision)UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());return go;
        }
        static void Boundary(string name,Vector3 pos,Vector3 size){Part(name,pos,size,iron).GetComponent<Renderer>().enabled=false;}
        static void Label(string text,Vector3 pos,float size=2.2f){var go=new GameObject(text);go.transform.SetParent(root);go.transform.position=pos;var t=go.AddComponent<TextMeshPro>();t.font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Afterglow/Materials/PrototypeFont.asset");t.text=text;t.color=new Color(.16f,.1f,.05f);t.fontSize=size;t.alignment=TextAlignmentOptions.Center;t.rectTransform.sizeDelta=new Vector2(9,1);}
        static void Door(string name,Vector3 pos,string dest,Vector3 arrival,M01Dialogue dialogue)
        {
            var p=Part(name,pos,new Vector3(1.55f,2.9f,.12f),wood,collision:false);
            for(int side=-1;side<=1;side+=2)Part("Jamba",pos+new Vector3(side*.88f,0,-.08f),new Vector3(.19f,3.25f,.22f),cream,falseType,false);
            Part("Dintel",pos+Vector3.up*1.6f,new Vector3(2, .22f,.28f),gold,collision:false);
            Part("Manilla",pos+new Vector3(.5f,-.1f,-.15f),Vector3.one*.12f,gold,PrimitiveType.Sphere,false);
            var d=p.AddComponent<M01RouteDoor>();d.requiresInteract=true;d.interactionRadius=2.4f;d.destination=dest;d.arrival=arrival;
            Label("[ E ] "+name,pos+new Vector3(0,2,-.3f),1.9f);
        }
        const PrimitiveType falseType=PrimitiveType.Cube;
        static void Turned(string name,Vector3 position,Vector2[] profile,Material material)
        {
            string path=Folder+"/"+name+".asset";var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if(!mesh){mesh=new Mesh{name=name};int n=32;var vertices=new Vector3[profile.Length*(n+1)];var triangles=new System.Collections.Generic.List<int>();
                for(int row=0;row<profile.Length;row++)for(int i=0;i<=n;i++){float a=i*Mathf.PI*2/n;vertices[row*(n+1)+i]=new Vector3(Mathf.Cos(a)*profile[row].x,profile[row].y,Mathf.Sin(a)*profile[row].x);if(row>0&&i<n){int b=row*(n+1)+i;triangles.AddRange(new[]{b,b+1,b-n-1,b+1,b-n,b-n-1});}}
                mesh.vertices=vertices;mesh.triangles=triangles.ToArray();mesh.RecalculateNormals();mesh.RecalculateBounds();AssetDatabase.CreateAsset(mesh,path);}
            var go=new GameObject(name);go.transform.SetParent(root);go.transform.position=position;go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=material;
        }
        static void Bedroom(M01Dialogue dialogue)
        {
            Part("Tarima",new Vector3(7,-.2f,1.7f),new Vector3(14,.4f,8.6f),wood);
            for(int i=0;i<27;i++)Part("Junta de parquet",new Vector3(.25f+i*.5f,.012f,1.7f),new Vector3(.018f,.012f,8.5f),gold,collision:false);
            Part("Papel pintado",new Vector3(7,3,6),new Vector3(14,6,.25f),cream);
            for(int i=0;i<35;i++)Part("Liston decorativo",new Vector3(i*.4f,2.8f,5.84f),new Vector3(.025f,5.5f,.035f),gold,collision:false);
            foreach(float y in new[]{.25f,1.3f,5.6f})Part("Moldura",new Vector3(7,y,5.73f),new Vector3(14,.15f,.2f),wood,collision:false);
            Boundary("Limite frontal",new Vector3(7,4,-2.6f),new Vector3(14,9,.2f));Boundary("Pared izquierda",new Vector3(0,4,1.7f),new Vector3(.25f,9,8.6f));Boundary("Pared derecha",new Vector3(14,4,1.7f),new Vector3(.25f,9,8.6f));
            Part("Alfombra",new Vector3(7,.025f,.5f),new Vector3(6,.035f,3.7f),purple,collision:false);
            foreach(float z in new[]{-1.25f,2.25f})Part("Borde tejido",new Vector3(7,.047f,z),new Vector3(5.8f,.015f,.12f),gold,collision:false);
            Part("Cama tallada",new Vector3(2.4f,.65f,3.1f),new Vector3(2.7f,1,3.9f),wood);Part("Colcha",new Vector3(2.4f,1.23f,2.8f),new Vector3(2.6f,.25f,3.2f),purple);
            Part("Cabecero",new Vector3(2.4f,1.9f,4.9f),new Vector3(3,2.3f,.22f),wood);
            Part("Almohada",new Vector3(2.4f,1.47f,4),new Vector3(2,.22f,.7f),paper,PrimitiveType.Sphere,false);
            foreach(float x in new[]{.9f,3.9f}){Part("Poste torneado",new Vector3(x,1.45f,4.9f),new Vector3(.18f,2.7f,.18f),gold,PrimitiveType.Cylinder);Part("Remate",new Vector3(x,2.9f,4.9f),Vector3.one*.3f,wood,PrimitiveType.Sphere,false);}
            Part("Ventana azul",new Vector3(7,3.3f,5.72f),new Vector3(2.5f,3.2f,.08f),water,collision:false);
            for(int i=-1;i<=1;i++)Part("Marco ventana",new Vector3(7+i*1.25f,3.3f,5.61f),new Vector3(.12f,3.4f,.1f),wood,collision:false);
            Part("Travesano ventana",new Vector3(7,3.3f,5.6f),new Vector3(2.6f,.12f,.1f),wood,collision:false);
            foreach(float side in new[]{-1f,1f})for(int j=0;j<4;j++)Part("Pliegue cortina",new Vector3(7+side*(1.35f+j*.13f),3.2f,5.4f-j%2*.07f),new Vector3(.22f,3.5f,.16f),purple,PrimitiveType.Cylinder,false);
            Part("Armario",new Vector3(10.3f,1.7f,4.8f),new Vector3(1.6f,3.4f,1.2f),wood);Part("Panel armario",new Vector3(10.3f,1.8f,4.15f),new Vector3(1.25f,2.7f,.08f),gold,collision:false);
            Part("Escritorio",new Vector3(7.2f,1,3.5f),new Vector3(2.9f,.16f,1.25f),wood);
            foreach(float x in new[]{6f,8.4f})foreach(float z in new[]{3.05f,3.95f})Part("Pata escritorio",new Vector3(x,.5f,z),new Vector3(.13f,1,.13f),wood);
            var letter=Part("Carta",new Vector3(7.2f,1.12f,3.35f),new Vector3(.65f,.025f,.45f),paper,collision:false);
            var read=letter.AddComponent<M01DialoguePoint>();read.dialogue=dialogue;read.lineIds=new[]{"P01.06"};
            Part("Retrato familiar marco",new Vector3(4.6f,3.7f,5.7f),new Vector3(1.1f,1.4f,.15f),gold,collision:false);Part("Retrato mineral",new Vector3(4.6f,3.7f,5.57f),new Vector3(.7f,1,.08f),purple,PrimitiveType.Sphere,false);
            Door("Jardin",new Vector3(12.7f,1.45f,4.5f),"PRO_PATIO",new Vector3(9,.05f,3.8f),dialogue);
        }
        static void Fence(Vector3 a,Vector3 b)
        {
            var v=b-a;int count=Mathf.CeilToInt(v.magnitude/.65f);
            bool foreground=a.z==-5&&b.z==-5;float height=foreground?.35f:2.8f;
            for(int i=0;i<=count;i++){var p=Vector3.Lerp(a,b,(float)i/count);Part("Barrote",p+Vector3.up*height*.5f,new Vector3(.075f,height,.075f),iron,collision:false);Part("Punta",p+Vector3.up*(height+.1f),new Vector3(.13f,.2f,.13f),gold,PrimitiveType.Sphere,false);}
            var mid=(a+b)*.5f;var size=new Vector3(Mathf.Abs(v.x)+.12f,.12f,Mathf.Abs(v.z)+.12f);foreach(float y in foreground?new[]{.2f}:new[]{.6f,2.2f})Part("Travesano valla",mid+Vector3.up*y,size,iron,collision:false);
            Boundary("Valla fisica continua",mid+Vector3.up*4,new Vector3(Mathf.Abs(v.x)+.2f,9,Mathf.Abs(v.z)+.2f));
        }
        static void Garden(M01Dialogue dialogue)
        {
            Part("Prado rectangular",new Vector3(22,-.2f,1),new Vector3(40,.4f,12),grass);
            Part("Camino porton",new Vector3(24,.016f,.5f),new Vector3(35,.03f,2.2f),stone,collision:false);
            Part("Camino casa",new Vector3(9,.02f,3),new Vector3(2,.035f,6),stone,collision:false);
            foreach(float z in new[]{-2.8f,3.8f})Part("Rodeo de fuente",new Vector3(22,.02f,z),new Vector3(9,.035f,1.6f),stone,collision:false);
            foreach(float x in new[]{17.5f,26.5f})Part("Union senderos",new Vector3(x,.022f,.5f),new Vector3(1.6f,.035f,7.8f),stone,collision:false);
            Part("Cimiento mansion",new Vector3(9,.35f,7.9f),new Vector3(15,.7f,4),stone);
            Part("Villa Albina fachada",new Vector3(9,4.1f,8.6f),new Vector3(14,7.6f,4),cream);
            for(int floor=0;floor<2;floor++)for(int i=0;i<6;i++)
            {
                float x=3.3f+i*2.3f,y=2.2f+floor*3.2f;
                Part("Marco terracota",new Vector3(x,y,6.49f),new Vector3(1.35f,2.1f,.22f),roof,collision:false);
                Part("Cristal",new Vector3(x,y,6.34f),new Vector3(1.06f,1.8f,.08f),water,collision:false);
                Part("Antepecho",new Vector3(x,y-.95f,6.22f),new Vector3(1.65f,.17f,.4f),gold,collision:false);
                Part("Montante",new Vector3(x,y,6.23f),new Vector3(.08f,1.8f,.08f),cream,collision:false);
            }
            foreach(float y in new[]{.7f,3.75f,7.8f})Part("Cornisa mansion",new Vector3(9,y,6.36f),new Vector3(14.5f,.2f,.5f),gold,collision:false);
            var roofGo=Part("Tejado inclinado",new Vector3(9,8.2f,8),new Vector3(15,.32f,5.8f),roof,collision:false);roofGo.transform.rotation=Quaternion.Euler(-18,0,0);
            for(int i=0;i<30;i++)Part("Tejas",new Vector3(1.8f+i*.5f,7.65f,5.4f),new Vector3(.45f,.15f,.4f),roof,PrimitiveType.Cylinder,false);
            Door("Dormitorio",new Vector3(9,1.45f,5.8f),"PRO_CASA",new Vector3(11,.05f,2.3f),dialogue);
            Turned("Cuenco tallado",new Vector3(22,0,.5f),new[]{new Vector2(0,0),new Vector2(2.05f,0),new Vector2(2.25f,.12f),new Vector2(2.25f,.24f),new Vector2(2.1f,.3f),new Vector2(2.1f,.53f),new Vector2(2.2f,.57f),new Vector2(2.2f,.68f),new Vector2(1.95f,.68f),new Vector2(1.9f,.38f),new Vector2(0,.38f)},stone);
            Part("Agua de la fuente",new Vector3(22,.43f,.5f),new Vector3(3.9f,.035f,3.9f),water,PrimitiveType.Cylinder,false);
            Turned("Pedestal torneado",new Vector3(22,.4f,.5f),new[]{new Vector2(.5f,0),new Vector2(.5f,.1f),new Vector2(.35f,.18f),new Vector2(.23f,.35f),new Vector2(.18f,.9f),new Vector2(.32f,1.05f),new Vector2(.32f,1.2f),new Vector2(.2f,1.3f)},cream);
            Turned("Copa labrada",new Vector3(22,1.7f,.5f),new[]{new Vector2(.15f,0),new Vector2(.4f,.08f),new Vector2(.75f,.2f),new Vector2(1,.4f),new Vector2(1,.5f),new Vector2(.86f,.5f),new Vector2(.7f,.32f),new Vector2(.2f,.2f),new Vector2(0,.2f)},gold);
            Part("Remate fuente",new Vector3(22,2.5f,.5f),new Vector3(.35f,.9f,.35f),stone,PrimitiveType.Sphere,false);
            Boundary("Cuenco no transitable",new Vector3(22,1.5f,.5f),new Vector3(4.2f,3,4.2f));
            Fence(new Vector3(2,0,-5),new Vector3(40,0,-5));Fence(new Vector3(2,0,7),new Vector3(40,0,7));Fence(new Vector3(2,0,-5),new Vector3(2,0,7));
            Fence(new Vector3(40,0,-5),new Vector3(40,0,-1));Fence(new Vector3(40,0,2),new Vector3(40,0,7));
            Fence(new Vector3(40,0,-1),new Vector3(40,0,2));
            foreach(float z in new[]{-1.2f,2.2f}){Part("Pilar porton",new Vector3(40,1.7f,z),new Vector3(.6f,3.4f,.6f),cream);Part("Remate pilar",new Vector3(40,3.55f,z),Vector3.one*.65f,gold,PrimitiveType.Sphere,false);}
            for(int i=0;i<9;i++)
            {
                float x=5+i*4;float z=5.8f;
                if(x<13)continue;
                Part("Tronco",new Vector3(x,1.25f,z),new Vector3(.3f,1.25f,.3f),wood,PrimitiveType.Cylinder);
                Part("Copa arbol",new Vector3(x,3.1f,z),new Vector3(2.3f,2.6f,1.8f),leaf,PrimitiveType.Sphere,false);
                for(int j=0;j<6;j++)Part("Flores",new Vector3(x-1+j*.38f,.22f,z+.75f),new Vector3(.23f,.35f,.23f),j%2==0?paper:purple,PrimitiveType.Sphere,false);
            }
            foreach(float x in new[]{14f,31f})
            {
                Part("Banco asiento",new Vector3(x,.6f,4.9f),new Vector3(2.5f,.16f,.7f),wood);Part("Banco respaldo",new Vector3(x,1.05f,5.2f),new Vector3(2.5f,.75f,.13f),wood);
                foreach(float dx in new[]{-.9f,.9f})Part("Banco pie",new Vector3(x+dx,.3f,4.9f),new Vector3(.13f,.6f,.6f),iron);
                Part("Farola",new Vector3(x,1.5f,-3.9f),new Vector3(.12f,1.5f,.12f),iron,PrimitiveType.Cylinder);Part("Farol",new Vector3(x,3.2f,-3.9f),new Vector3(.45f,.65f,.45f),paper,collision:false);
            }
            // The speaker remains outside. A separate interaction anchor is reachable inside.
            Part("Capitan silueta 2D",new Vector3(42,1,.5f),new Vector3(.8f,2,.08f),iron,collision:false);
            Part("Gorra capitan",new Vector3(42,2.1f,.45f),new Vector3(.95f,.22f,.1f),gold,collision:false);
            var anchor=new GameObject("Capitan [E] desde dentro");anchor.transform.SetParent(root);anchor.transform.position=new Vector3(38.8f,.05f,.5f);
            var story=anchor.AddComponent<M01StoryPoint>();story.dialogue=dialogue;story.eventId="P02.CAPTAIN";story.lineIds=new[]{"P02.02","P02.04","P02.05","P02.07"};story.radius=2;story.destination="M01_ACCESO";
            anchor.AddComponent<M01PrologueGestures>();
            Label("[ E ] Capitan",new Vector3(38.7f,3.7f,.5f),2);
        }
    }
}
