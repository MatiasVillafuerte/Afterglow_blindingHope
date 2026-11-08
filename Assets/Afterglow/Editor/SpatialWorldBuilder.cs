using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Afterglow.M01;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Afterglow.Editor
{
    public static class SpatialWorldBuilder
    {
        const string Models = "Assets/Afterglow/Environment/Modelado/";
        const string Backup = "Tools/PM/Backups/Antes_Camara_Espacial/";
        static readonly string[] Sections = { "PRO_CASA", "PRO_PATIO", "N01S01", "N01S02", "N01S03", "N01S04" };
        static readonly Dictionary<string, Color> Colors = new Dictionary<string, Color>
        {
            ["Ivory"]=new Color(.84f,.72f,.53f), ["Plaster"]=new Color(.58f,.44f,.30f), ["Trim"]=new Color(.76f,.54f,.23f),
            ["Clay"]=new Color(.47f,.16f,.085f), ["ClayLight"]=new Color(.62f,.25f,.13f), ["DarkOak"]=new Color(.22f,.096f,.047f),
            ["Oak"]=new Color(.38f,.19f,.085f), ["Velvet"]=new Color(.27f,.10f,.30f), ["Linen"]=new Color(.9f,.79f,.61f),
            ["Glass"]=new Color(.14f,.35f,.39f), ["Water"]=new Color(.11f,.49f,.56f), ["Iron"]=new Color(.075f,.10f,.12f),
            ["Path"]=new Color(.62f,.48f,.32f), ["Paver"]=new Color(.72f,.58f,.40f), ["Grass"]=new Color(.22f,.38f,.17f),
            ["Leaf"]=new Color(.12f,.29f,.14f), ["LeafLight"]=new Color(.28f,.45f,.18f), ["Rose"]=new Color(.69f,.25f,.39f),
            ["Lamp"]=new Color(.95f,.62f,.23f), ["Earth"]=new Color(.30f,.22f,.16f), ["Dust"]=new Color(.40f,.31f,.24f),
            ["Slate"]=new Color(.24f,.29f,.34f), ["SlateLight"]=new Color(.34f,.40f,.45f), ["Deep"]=new Color(.12f,.16f,.20f),
            ["Crystal"]=new Color(.51f,.24f,.65f), ["Tunnel"]=new Color(.035f,.042f,.055f), ["Canvas"]=new Color(.23f,.34f,.39f)
        };

        [Serializable] sealed class BoxLayout { public string name; public float[] center, size; }
        [Serializable] sealed class Layout { public BoxLayout[] boxes; }

        static Transform roomRoot;
        static M01Dialogue dialogue;

        public static void Build()
        {
            if (EditorApplication.isPlaying || Enumerable.Range(0,SceneManager.sceneCount).Any(i=>SceneManager.GetSceneAt(i).isDirty))
                throw new InvalidOperationException("Stop Play and save any unsaved scene before building.");

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach (string id in Sections.Append("M01_MAPA"))
                if (!AssetDatabase.LoadAssetAtPath<GameObject>(Models+id+".fbx")) throw new FileNotFoundException(id+".fbx");

            Directory.CreateDirectory(Backup);
            foreach (string id in Sections.Append("M01_PUEBLO").Append("M01_MAPA"))
            {
                string source=M01Expedition.ScenePath(id);
                if(File.Exists(source)&&!File.Exists(Backup+id+".unity"))File.Copy(source,Backup+id+".unity");
            }

            CopyTemplate("N01S01", "M01_ACCESO");
            CopyTemplate("M01_MAPA", "M01_ACCESO");
            foreach(string id in Sections)BuildRoom(id);
            BuildMap();
            var village=EditorSceneManager.OpenScene(M01Expedition.ScenePath("M01_PUEBLO"));
            foreach(var door in Object.FindObjectsByType<M01RouteDoor>())
                if(door.destination=="M01_ACCESO") { door.destination="N01S01";door.arrival=new Vector3(-8,.05f,-3.8f);door.fade=true; }
            EditorSceneManager.SaveScene(village);

            var scenes=EditorBuildSettings.scenes.Where(s=>s.path!=M01Expedition.ScenePath("M01_ACCESO")).ToList();
            foreach(string id in new[]{"M01_MAPA","N01S01"})
            {
                string path=M01Expedition.ScenePath(id);
                if(!scenes.Any(s=>s.path==path))scenes.Add(new EditorBuildSettingsScene(path,true));
            }
            EditorBuildSettings.scenes=scenes.ToArray();
            AssetDatabase.SaveAssets();
            EditorSceneManager.OpenScene(M01Expedition.ScenePath("M01_MENU"));
        }

        static void CopyTemplate(string target, string template)
        {
            string path=M01Expedition.ScenePath(target);
            if(!File.Exists(path)&&!AssetDatabase.CopyAsset(M01Expedition.ScenePath(template),path))
                throw new IOException("Could not create "+path);
        }

        static bool Core(GameObject go) => go.GetComponent<AfterglowPlayer>() || go.GetComponent<Camera>() ||
            go.GetComponent<AfterglowHUD>() || go.GetComponent<M01Expedition>() || go.GetComponent<M01Prologue>();

        static void BuildRoom(string id)
        {
            var scene=EditorSceneManager.OpenScene(M01Expedition.ScenePath(id));
            dialogue=Object.FindAnyObjectByType<M01Dialogue>();
            foreach(var go in scene.GetRootGameObjects())if(!Core(go))Object.DestroyImmediate(go);
            foreach(Transform child in dialogue.transform)
                if(child.name=="PM_MapView"){Object.DestroyImmediate(child.gameObject);break;}

            roomRoot=new GameObject("Escenario · "+id).transform;
            ImportModel(id,roomRoot,Vector3.zero);
            var layout=JsonUtility.FromJson<Layout>(File.ReadAllText(Models+id+".json"));
            foreach(var box in layout.boxes)
            {
                var collision=Anchor(box.name,Vector3From(box.center)).AddComponent<BoxCollider>();
                collision.size=Vector3From(box.size);
            }

            var player=Object.FindAnyObjectByType<AfterglowPlayer>();
            var entrance=id=="PRO_CASA"?new Vector3(7,.05f,0):M01Expedition.SectionEntrance(id);
            player.transform.position=entrance;
            var rules=player.GetComponent<M01PlayerRules>();
            rules.levelEntrance=rules.sectionEntrance=entrance;
            var expedition=Object.FindAnyObjectByType<M01Expedition>();
            expedition.roomId=id;
            expedition.safeArea=id.StartsWith("PRO_")||id=="N01S01";
            expedition.lootMaterial=Material("Crystal");
            ConfigureCamera(id,player);
            SetupLighting(id);
            var controls=dialogue.transform.Find("Controls")?.GetComponent<TMP_Text>();
            if(controls)
            {
                controls.color=Color.white;
                controls.text=id.StartsWith("PRO_")?"Flechas · Caminar     Espacio · Saltar     E · Interactuar":"Flechas · Caminar     Espacio · Saltar     X · Pico     E · Interactuar";
            }
            switch(id)
            {
                case "PRO_CASA": Bedroom(); break;
                case "PRO_PATIO": Garden(); break;
                case "N01S01": MineEntry(); break;
                case "N01S02": Terraces(); break;
                case "N01S03": ResistanceRoom(); break;
                case "N01S04": Collapse(); break;
            }
            EditorSceneManager.SaveScene(scene);
        }

        static void BuildMap()
        {
            var scene=EditorSceneManager.OpenScene(M01Expedition.ScenePath("M01_MAPA"));
            var map=Object.FindAnyObjectByType<M01RouteMap>();
            foreach(var go in scene.GetRootGameObjects())
                if(!Core(go)&&go!=map.gameObject&&go!=map.miniature)Object.DestroyImmediate(go);
            foreach(var child in map.transform.Cast<Transform>().ToArray())Object.DestroyImmediate(child.gameObject);
            foreach(var child in map.miniature.transform.Cast<Transform>().ToArray())
                if(!child.GetComponent<Camera>()&&!child.GetComponent<Light>())Object.DestroyImmediate(child.gameObject);
            var model=ImportModel("M01_MAPA",map.miniature.transform,new Vector3(1000,0,0));
            foreach(var child in model.GetComponentsInChildren<Transform>(true))child.gameObject.layer=30;
            map.gameObject.name="Selector de expediciones";
            map.standalone=true;
            map.dialogue.GetComponent<M01PauseMenu>().enabled=false;
            map.dialogue.GetComponent<M01InventoryPhone>().enabled=false;
            foreach(Transform child in map.dialogue.transform)
                if(child!=map.view.transform&&child!=map.dialogue.panel.transform&&child.name!="Fade")child.gameObject.SetActive(false);
            var player=Object.FindAnyObjectByType<AfterglowPlayer>();
            player.enabled=false;
            player.visual.gameObject.SetActive(false);
            var camera=Object.FindAnyObjectByType<AfterglowCamera>().GetComponent<Camera>();
            camera.clearFlags=CameraClearFlags.SolidColor;
            camera.backgroundColor=new Color(.04f,.065f,.075f);
            var state=Object.FindAnyObjectByType<M01Expedition>();state.roomId="M01_MAPA";state.safeArea=true;
            EditorSceneManager.SaveScene(scene);
        }

        static GameObject ImportModel(string id, Transform parent, Vector3 position)
        {
            var asset=AssetDatabase.LoadAssetAtPath<GameObject>(Models+id+".fbx");
            var instance=(GameObject)PrefabUtility.InstantiatePrefab(asset);
            instance.transform.SetParent(parent,false);
            instance.transform.localPosition=position;
            var witness=instance.GetComponentsInChildren<Transform>(true).First(t=>t.name=="AxisWitness");
            var axes=instance.transform.InverseTransformPoint(witness.position);
            if(Vector3.Distance(axes,new Vector3(3,2,7))>.03f)
            {
                if(Mathf.Abs(Mathf.Abs(axes.x)-3)>.03f||Mathf.Abs(axes.y-2)>.03f||Mathf.Abs(Mathf.Abs(axes.z)-7)>.03f)
                    throw new InvalidOperationException("Unexpected FBX axes in "+id+": "+axes);
                instance.transform.localScale=new Vector3(Mathf.Sign(axes.x),1,Mathf.Sign(axes.z));
            }
            foreach(var renderer in instance.GetComponentsInChildren<MeshRenderer>(true))
            {
                string name=renderer.name.Replace("Export_", "");
                renderer.sharedMaterial=Material(name=="BreakableBoulder"?"SlateLight":name);
            }
            return instance;
        }

        static void ConfigureCamera(string id, AfterglowPlayer player)
        {
            var camera=Object.FindAnyObjectByType<AfterglowCamera>();
            camera.player=player;
            camera.followDepth=true;
            camera.anticipation=1.12f;
            camera.smoothing=.32f;
            camera.GetComponent<Camera>().fieldOfView=38;
            camera.focusX=new Vector2(-10,10);
            camera.focusZ=new Vector2(-8,13);
            camera.depthBlend=new Vector2(-6,11);
            camera.yaw=new Vector2(8,-9);
            camera.pitch=new Vector2(16,23);
            camera.distance=new Vector2(11.3f,12.4f);
            if(id=="PRO_CASA")
            {
                camera.focusX=new Vector2(5,10);
                camera.focusZ=new Vector2(-1,2.2f);
                camera.depthBlend=new Vector2(-2,4);
                camera.yaw=new Vector2(3,-4);
                camera.pitch=new Vector2(12,15);
                camera.distance=new Vector2(10,10.5f);
                camera.anticipation=.7f;
            }
            if(id=="PRO_PATIO")
            {
                camera.focusX=new Vector2(-12.8f,13);
                camera.focusZ=new Vector2(-13.8f,14.8f);
                camera.yaw=new Vector2(-13,10);
                camera.pitch=new Vector2(19,24);
                camera.distance=new Vector2(13.4f,15.1f);
            }
            if(id=="N01S02") { camera.yaw=new Vector2(13,-11);camera.pitch=new Vector2(18,24); }
            if(id=="N01S03") { camera.focusZ=new Vector2(-9,13);camera.yaw=new Vector2(-14,13); }
            if(id=="N01S04") { camera.yaw=new Vector2(8,15);camera.pitch=new Vector2(18,24); }
            camera.GetComponent<Camera>().clearFlags=CameraClearFlags.SolidColor;
            camera.GetComponent<Camera>().backgroundColor=id=="PRO_PATIO"?new Color(.40f,.54f,.62f):new Color(.075f,.10f,.14f);
            camera.Snap();
        }

        static void SetupLighting(string id)
        {
            bool outside=id=="PRO_PATIO";
            RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight=outside?new Color(.57f,.61f,.64f):new Color(.48f,.49f,.54f);
            var light=Anchor("Luz principal",Vector3.zero).AddComponent<Light>();
            light.type=LightType.Directional;
            light.intensity=outside?1.65f:1.05f;
            light.color=outside?new Color(1,.94f,.83f):new Color(.98f,.88f,.77f);
            light.transform.rotation=Quaternion.Euler(46,-32,0);
            light.shadows=LightShadows.Soft;
            if(id=="PRO_CASA") Glow(new Vector3(12.5f,3.3f,3.65f),5,2.3f);
            if(id=="N01S01") Glow(new Vector3(2,2.3f,8.2f),7,2.2f);
            if(id=="N01S02") Glow(new Vector3(8.9f,4.2f,9.7f),7,2.2f);
            if(id=="N01S03") { Glow(new Vector3(6,2,-4),8,2.1f);Glow(new Vector3(-6,2.4f,11),8,2.3f); }
            if(id=="N01S04") Glow(new Vector3(-4,4.3f,9.6f),8,2.7f);
        }

        static void Glow(Vector3 position,float range,float intensity)
        {
            var light=Anchor("Luz de farol",position).AddComponent<Light>();
            light.type=LightType.Point;light.range=range;light.intensity=intensity;light.color=new Color(1,.66f,.29f);
        }

        static void Bedroom()
        {
            var letter=Anchor("Carta",new Vector3(7.2f,1.13f,3.35f));
            var point=letter.AddComponent<M01DialoguePoint>();point.dialogue=dialogue;point.lineIds=new[]{"P01.06"};
            Door("Jardín",new Vector3(12.7f,0,4.22f),"PRO_PATIO",M01Expedition.SectionEntrance("PRO_PATIO"));
        }

        static void Garden()
        {
            Door("Dormitorio",new Vector3(0,0,11.25f),"PRO_CASA",new Vector3(11,.05f,2.3f));
            Actor("Capitán",new Vector3(15.45f,0,-7.5f),"Capitan",new Color(.18f,.35f,.53f));
            var anchor=Anchor("Capitán desde el portón",new Vector3(12.8f,0,-7.5f));
            var captain=Story(anchor,"P02.CAPTAIN",false,2.3f,"P02.02","P02.04","P02.05","P02.07");
            captain.destination="M01_MAPA";
            captain.arrival=Vector3.zero;
            anchor.AddComponent<M01PrologueGestures>();
            Label("Capitán  [E]",new Vector3(13,3.72f,-7.5f));
        }

        static void MineEntry()
        {
            var son=Actor("Hijo del minero",new Vector3(-1,0,-3.9f),"Hijo",new Color(.18f,.48f,.53f));
            var intro=Story(son,"N1A.INTRO",true,3.7f,"N1A.01","N1A.02","N1A.03","N1A.05","N1A.06","N1A.07");
            intro.prerequisiteEvent="M01.N01_ENTERED";
            var phone=Story(son,"N1C.PHONE",true,3.7f,"N1C.01","N1C.02","N1C.03","N1C.04","N1C.05","N1C.06","N1C.07");
            phone.prerequisiteEvent="N1A.INTRO";phone.givePhone=true;
            var altar=Anchor("Altar del Tío",new Vector3(4,.65f,-3)).AddComponent<M01Altar>();altar.dialogue=dialogue;altar.interactionRadius=2.25f;
            var obstacle=altar.gameObject.AddComponent<BoxCollider>();obstacle.size=new Vector3(1.2f,1.25f,1.2f);
            Label("Altar del Tío  [E]",new Vector3(4,2.13f,-3));
            var onward=Door("Galería de terrazas",new Vector3(1,0,8.1f),"N01S02",M01Expedition.SectionEntrance("N01S02"));
            onward.requiredEvent="N1C.PHONE";onward.enterExpedition=true;
            Door("Sendero al mapa",new Vector3(-1,0,-7.45f),"M01_MAPA",Vector3.zero).blockedWhenWorldClosed=true;
            Door("Sendero al pueblo",new Vector3(-10,0,-3.8f),"M01_PUEBLO",new Vector3(37,.05f,0));
            Quest("Encargo · resistencias",new Vector3(-7,0,2),"Q01","SEC.01","SEC.02",null);
            Quest("Encargo · casco",new Vector3(7,0,4),"Q02","SEC.03","SEC.04","Q02.CASCO");
        }

        static void Terraces()
        {
            Door("Entrada de la mina",new Vector3(-8,0,-7.1f),"N01S01",new Vector3(1,.05f,5.8f));
            Door("Cámara de trabajo",new Vector3(8,2.2f,10),"N01S03",M01Expedition.SectionEntrance("N01S03"));
            Offering("EXP.N01S02.0",new Vector3(6,1.46f,3));
        }

        static void ResistanceRoom()
        {
            var rock=roomRoot.GetComponentsInChildren<MeshFilter>().First(m=>m.name=="Export_BreakableBoulder");
            var rockCollider=rock.gameObject.AddComponent<BoxCollider>();
            rockCollider.center=rock.sharedMesh.bounds.center;rockCollider.size=rock.sharedMesh.bounds.size;
            rock.gameObject.AddComponent<M01BreakableRock>();
            Door("Galería de terrazas",new Vector3(6,0,-8.65f),"N01S02",new Vector3(8,2.25f,8));
            Door("Ramal del derrumbe",new Vector3(-6,0,11.4f),"N01S04",M01Expedition.SectionEntrance("N01S04"));
            var son=Actor("Hijo del minero",new Vector3(4.7f,0,-4.8f),"Hijo",new Color(.18f,.48f,.53f));
            var tutorial=Story(son,"N1B.PICKAXE",true,3.1f,"N1B.02","N1B.04","N1B.05","N1B.06");
            tutorial.givePickaxe=true;
            tutorial.wakeEnemies=new[]{Enemy("N01.TUTORIAL.RESISTANCE",new Vector3(1,.05f,0),true),
                Enemy("N01.RESISTANCE.0",new Vector3(-5,.05f,3),false),Enemy("N01.RESISTANCE.1",new Vector3(-5,.05f,8),false)};
            var loot=Story(Anchor("Primer botín",Vector3.zero),"N1B.LOOT",true,100,"N1B.08","N1B.09");
            loot.prerequisiteEvent="N1B.PICKAXE";loot.defeatedEnemy=tutorial.wakeEnemies[0];
            Offering("EXP.N01S03.0",new Vector3(8,.35f,1.6f));
        }

        static void Collapse()
        {
            Door("Cámara de trabajo",new Vector3(5,0,-7.65f),"N01S03",new Vector3(-6,.05f,9));
            var helmet=Part("Casco Q02",new Vector3(7,.35f,3),new Vector3(.57f,.30f,.52f),"Trim",PrimitiveType.Sphere);
            helmet.AddComponent<M01QuestObject>();
            Label("Casco",new Vector3(7,1.35f,3));
            var miner=Actor("Minero del derrumbe",new Vector3(-4,2.2f,9.4f),"Minero",new Color(.46f,.37f,.24f));
            miner.AddComponent<M01Investigation>().dialogue=dialogue;
            Label("Derrumbe  [E]",new Vector3(-4,4.88f,10));
        }

        static M01RouteDoor Door(string label,Vector3 position,string destination,Vector3 arrival)
        {
            var door=Anchor(label,position).AddComponent<M01RouteDoor>();
            door.requiresInteract=true;door.interactionRadius=1.7f;door.verticalTolerance=.85f;
            door.destination=destination;door.arrival=arrival;door.fade=true;
            Label(label+"  [E]",position+Vector3.up*3.24f);
            return door;
        }

        static M01StoryPoint Story(GameObject anchor,string flag,bool automatic,float range,params string[] lines)
        {
            var point=anchor.AddComponent<M01StoryPoint>();point.dialogue=dialogue;point.eventId=flag;
            point.lineIds=lines;point.automatic=automatic;point.radius=range;return point;
        }

        static void Quest(string label,Vector3 position,string id,string start,string end,string item)
        {
            var npc=Actor(label,position,"Minero",new Color(.46f,.37f,.24f)).AddComponent<M01QuestNPC>();
            npc.dialogue=dialogue;npc.questId=id;npc.startLine=start;npc.endLine=end;npc.requiredItem=item;
            if(item!=null)npc.moneyReward=8;
            Label(label+"  [E]",position+Vector3.up*2.62f);
        }

        static AfterglowEnemy Enemy(string id,Vector3 position,bool tutorial)
        {
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Afterglow/Prefabs/Enemy.prefab");
            var enemy=(GameObject)PrefabUtility.InstantiatePrefab(prefab);
            enemy.name=id;enemy.transform.position=position;
            var loot=enemy.AddComponent<M01EnemyLoot>();loot.material=Material("Crystal");loot.firstTutorialOffering=tutorial;
            return enemy.GetComponent<AfterglowEnemy>();
        }

        static void Offering(string id,Vector3 position)
        {
            var offering=Part("Ofrenda · Coca",position,Vector3.one*.32f,"LeafLight",PrimitiveType.Sphere);
            offering.AddComponent<M01StaticOffering>().sourceId=id;
        }

        static GameObject Actor(string name,Vector3 position,string style,Color vest)
        {
            var actor=Anchor(name,position);
            var renderer=actor.AddComponent<SpriteRenderer>();
            renderer.sprite=MinerSprite(style,vest);
            return actor;
        }

        static Sprite MinerSprite(string style,Color vest)
        {
            string path=Models+"Actor_"+style+".asset";
            var old=AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().FirstOrDefault();
            if(old)return old;
            const int width=128,height=192;
            var image=new Texture2D(width,height,TextureFormat.RGBA32,false){name=style,filterMode=FilterMode.Bilinear};
            var pixels=new Color32[width*height];
            void Shape(float x,float y,float rx,float ry,Color color)
            {
                for(int j=0;j<height;j++)for(int i=0;i<width;i++)
                {
                    float d=(i-x)*(i-x)/(rx*rx)+(j-y)*(j-y)/(ry*ry);
                    if(d<=1)pixels[j*width+i]=color;
                }
            }
            var line=new Color(.12f,.10f,.12f);var ochre=new Color(.76f,.48f,.23f);var skin=new Color(.65f,.51f,.67f);
            Shape(45,33,15,27,line);Shape(81,33,15,27,line);
            Shape(46,36,11,23,ochre*.72f);Shape(80,36,11,23,ochre*.72f);
            Shape(38,9,22,9,line);Shape(88,9,22,9,line);
            Shape(62,80,40,39,line);Shape(62,80,35,35,vest);
            Shape(32,72,11,23,vest*.78f);Shape(93,72,11,23,vest*.78f);
            Shape(63,122,33,34,line);Shape(63,122,29,30,skin);
            Shape(51,128,3.4f,5,line);Shape(77,129,3.4f,5,line);Shape(64,109,9,2.1f,line);
            Shape(61,155,38,24,line);Shape(61,157,34,20,style=="Capitan"?vest:ochre);
            Shape(62,148,44,8,line);Shape(62,151,40,4,ochre);
            Shape(66,162,13,13,line);Shape(66,162,9,9,new Color(1,.85f,.44f));
            Shape(65,164,4,5,new Color(1,.98f,.78f));
            image.SetPixels32(pixels);image.Apply();
            AssetDatabase.CreateAsset(image,path);
            var sprite=Sprite.Create(image,new Rect(0,0,width,height),new Vector2(.5f,0),88);
            sprite.name=style;AssetDatabase.AddObjectToAsset(sprite,path);
            return sprite;
        }

        static Material Material(string name)
        {
            string path=Models+name+".mat";
            var material=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(material)return material;
            material=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name};
            material.color=Colors.TryGetValue(name,out var color)?color:Color.gray;
            material.SetFloat("_Smoothness",name=="Water"?.76f:name=="Iron"?.28f:.17f);
            if(name=="Lamp")
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor",new Color(.95f,.42f,.10f)*.8f);
            }
            AssetDatabase.CreateAsset(material,path);
            return material;
        }

        static GameObject Anchor(string name,Vector3 position)
        {
            var go=new GameObject(name);go.transform.SetParent(roomRoot);go.transform.position=position;return go;
        }

        static GameObject Part(string name,Vector3 position,Vector3 scale,string material,PrimitiveType type=PrimitiveType.Cube)
        {
            var go=GameObject.CreatePrimitive(type);go.name=name;go.transform.SetParent(roomRoot);
            go.transform.position=position;go.transform.localScale=scale;
            Object.DestroyImmediate(go.GetComponent<Collider>());go.GetComponent<MeshRenderer>().sharedMaterial=Material(material);
            return go;
        }

        static void Label(string label,Vector3 position)
        {
            var text=Anchor(label,position).AddComponent<TextMeshPro>();
            text.font=dialogue.body.font;text.text=label;text.alignment=TextAlignmentOptions.Center;
            text.fontSize=2.6f;text.color=new Color(1,.9f,.69f);text.rectTransform.sizeDelta=new Vector2(9,1);
        }

        static Vector3 Vector3From(float[] a)=>new Vector3(a[0],a[1],a[2]);
    }
}
