using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using Afterglow.M01;

namespace Afterglow.Editor
{
    public static class M01StoryBuilder
    {
        static Scene source, target;
        static AfterglowPlayer sourcePlayer, player;
        static AfterglowCamera sourceCamera;
        static AfterglowHUD sourceHud;
        static M01Dialogue dialogue;
        static Material stone;
        static readonly string[] Rooms={"M01_MENU","PRO_CASA","PRO_PATIO","M01_ACCESO","M01_PUEBLO","N01S02","N01S03","N01S04"};
        [MenuItem("Afterglow/Mundo 1/Build Prologue and Level One")]
        public static void Build()
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode first.");
            for(int i=0;i<SceneManager.sceneCount;i++)if(SceneManager.GetSceneAt(i).isDirty)throw new InvalidOperationException("Save scene changes first.");
            source=EditorSceneManager.OpenScene(M01FoundationBuilder.Path);
            sourcePlayer=UnityEngine.Object.FindAnyObjectByType<AfterglowPlayer>();
            sourceCamera=UnityEngine.Object.FindAnyObjectByType<AfterglowCamera>();
            sourceHud=UnityEngine.Object.FindAnyObjectByType<AfterglowHUD>();
            stone=AssetDatabase.LoadAssetAtPath<Material>("Assets/Afterglow/Materials/Platforms.mat");
            foreach(string id in Rooms)
            {
                target=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);SceneManager.SetActiveScene(target);
                Setup(id);
                if(id=="M01_MENU")MainMenu();
                else if(id=="PRO_CASA")House();
                else if(id=="PRO_PATIO")Patio();
                else if(id=="M01_ACCESO")Access();
                else if(id=="M01_PUEBLO")Village();
                else Gallery(id);
                EditorSceneManager.SaveScene(target,M01Expedition.ScenePath(id));EditorSceneManager.CloseScene(target,true);
            }
            var scenes=EditorBuildSettings.scenes.ToList();
            foreach(string id in Rooms){string path=M01Expedition.ScenePath(id);if(!scenes.Any(s=>s.path==path))scenes.Add(new EditorBuildSettingsScene(path,true));}
            EditorBuildSettings.scenes=scenes.ToArray();
            EditorSceneManager.OpenScene(M01Expedition.ScenePath("PRO_CASA"));AssetDatabase.SaveAssets();
        }
        static void Setup(string id)
        {
            var p=UnityEngine.Object.Instantiate(sourcePlayer.gameObject);SceneManager.MoveGameObjectToScene(p,target);p.transform.position=new Vector3(4,.05f,0);player=p.GetComponent<AfterglowPlayer>();
            var rules=p.GetComponent<M01PlayerRules>();rules.foundationTestKit=false;rules.hasPickaxe=false;rules.levelEntrance=rules.sectionEntrance=p.transform.position;
            var cam=UnityEngine.Object.Instantiate(sourceCamera.gameObject);SceneManager.MoveGameObjectToScene(cam,target);cam.GetComponent<AfterglowCamera>().player=player;cam.GetComponent<AfterglowCamera>().Snap();
            var ui=UnityEngine.Object.Instantiate(sourceHud.gameObject);SceneManager.MoveGameObjectToScene(ui,target);ui.GetComponent<AfterglowHUD>().player=player;dialogue=ui.GetComponent<M01Dialogue>();
            var phone=ui.GetComponent<M01InventoryPhone>();phone.player=player;phone.exploration=!id.StartsWith("PRO_");
            ui.AddComponent<M01PauseMenu>().dialogue=dialogue;
            rules.fade=ui.transform.Find("Fade").GetComponent<CanvasGroup>();
            var room=new GameObject("Room state "+id).AddComponent<M01Expedition>();room.roomId=id;room.safeArea=id.StartsWith("PRO_")||id.StartsWith("M01_");room.level=1;room.lootMaterial=stone;
            Box("Ground",new Vector3(22,-.5f,0),new Vector3(44,1,12),stone);
            Box("Rear",new Vector3(22,4.5f,6),new Vector3(44,9,.35f),stone);
            foreach(float x in new[]{0f,44f})Box("Side",new Vector3(x,4.5f,0),new Vector3(.35f,9,12),stone);
            var front=Box("Invisible front boundary",new Vector3(22,5,-6),new Vector3(44,12,.3f),stone);front.GetComponent<Renderer>().enabled=false;
            var light=new GameObject("Light").AddComponent<Light>();light.type=LightType.Directional;light.intensity=1.3f;light.transform.rotation=Quaternion.Euler(45,-30,0);
            var header=ui.transform.Find("Controls");if(header)header.GetComponent<TMP_Text>().text="Flechas: mover   Espacio: saltar   X: pico   E: hablar / interactuar";
            WorldLabel(id,new Vector3(7,4.1f,5.7f),Color.white,3);
        }
        static void House()
        {
            Box("Table",new Vector3(4,.55f,1.8f),new Vector3(2,1.1f,1),Role("Madera",new Color(.4f,.24f,.12f)));
            var letter=Box("Carta [E]",new Vector3(4,1.14f,1.8f),new Vector3(.65f,.04f,.45f),Role("Papel",Color.white));
            Story(letter,"",new[]{"P01.06"},false,2);
            new GameObject("Letter and bell").AddComponent<M01Prologue>().dialogue=dialogue;
            Door("Salida al patio",10,"PRO_PATIO",new Vector3(4,.05f,0));
            WorldLabel("Casa · sal al patio",new Vector3(7,3.1f,3),Color.white,2.5f);
            // Frame the small house without touching the approved camera's settings.
            Box("House end wall",new Vector3(12,4,2),new Vector3(.4f,8,8),stone);
        }
        static void MainMenu()
        {
            player.enabled=false;player.GetComponent<M01PlayerRules>().enabled=false;player.visual.gameObject.SetActive(false);
            dialogue.GetComponent<M01InventoryPhone>().enabled=false;dialogue.GetComponent<M01PauseMenu>().enabled=false;
            dialogue.gameObject.AddComponent<M01MainMenu>().dialogue=dialogue;
        }
        static void Patio()
        {
            Box("Garden",new Vector3(15,.08f,3),new Vector3(22,.16f,3),Role("Jardin",new Color(.25f,.42f,.28f)));
            var captain=Actor("Capitán de minas",new Vector3(11,1,0),new Color(.12f,.22f,.4f));
            var point=Story(captain,"P02.CAPTAIN",new[]{"P02.02","P02.04","P02.05","P02.07"},false,2.5f);point.destination="M01_ACCESO";
            WorldLabel("Portón · [E] Hablar",new Vector3(11,3.1f,.2f),Color.white,2.5f);
            Door("Casa",2,"PRO_CASA",new Vector3(8,.05f,0));
            Box("Gate",new Vector3(13,1.4f,1),new Vector3(.5f,2.8f,5),stone);
        }
        static void Access()
        {
            var son=Actor("Hijo del minero",new Vector3(7,1,1),new Color(.25f,.75f,1));
            Story(son,"N1A.INTRO",new[]{"N1A.01","N1A.02","N1A.03","N1A.05","N1A.06","N1A.07"},true,4);
            var phone=Actor("Hijo · teléfono y altar",new Vector3(12,1,1),new Color(.25f,.75f,1));
            var tutorial=Story(phone,"N1C.PHONE",new[]{"N1C.01","N1C.02","N1C.03","N1C.04","N1C.05","N1C.06","N1C.07"},true,3);tutorial.givePhone=true;
            var altar=Box("Altar [E]",new Vector3(16,.5f,-1),new Vector3(1.2f,1,.8f),Role("Altar",new Color(.75f,.55f,.3f))).AddComponent<M01Altar>();altar.dialogue=dialogue;
            WorldLabel("Altar [E]",new Vector3(16,2.3f,-1),Color.black,2.5f);
            Door("Pueblo",2,"M01_PUEBLO",new Vector3(37,.05f,0));
            var door=Door("N01 · Galería",40,"N01S02",new Vector3(4,.05f,0));door.enterExpedition=true;door.requiredEvent="N1C.PHONE";
            var q1=Actor("Minero · resistencias [E]",new Vector3(23,1,1),Color.yellow).AddComponent<M01QuestNPC>();q1.dialogue=dialogue;
            var q2=Actor("Minero · casco [E]",new Vector3(29,1,1),Color.yellow).AddComponent<M01QuestNPC>();q2.dialogue=dialogue;q2.questId="Q02";q2.startLine="SEC.03";q2.endLine="SEC.04";q2.requiredItem="Q02.CASCO";q2.moneyReward=8;
        }
        static void Village()
        {
            var colors=new[]{new Color(.35f,.75f,.45f),new Color(.25f,.75f,.65f),new Color(.35f,.75f,.45f)};
            var names=new[]{"Tienda","Lector de coca","Habitante del pueblo"};
            for(int i=0;i<3;i++){var service=Actor(i==1?names[i]:"Habitante del pueblo",new Vector3(9+i*10,1,1),colors[i]).AddComponent<M01ServiceNPC>();service.dialogue=dialogue;service.service=(M01Service)i;WorldLabel(names[i]+" · [E]",new Vector3(9+i*10,3,1),Color.black,2);}
            Door("Acceso de la mina",40,"M01_ACCESO",new Vector3(4,.05f,0));
        }
        static void Gallery(string id)
        {
            if(id=="N01S02")
            {
                Door("Acceso",2,"M01_ACCESO",new Vector3(37,.05f,0)).leaveExpedition=true;
                Door("Cámara de la resistencia",40,"N01S03",new Vector3(4,.05f,0));
                for(int i=0;i<4;i++)Box("Escalón "+i,new Vector3(13+i*1.5f,.15f*(i+1),2.5f),new Vector3(1.5f,.3f*(i+1),3),stone);
                Box("Plataforma superior",new Vector3(23,.6f,2.5f),new Vector3(10,1.2f,3),stone);
                Offering(id,0,new Vector3(21,1.55f,2.5f));
            }
            if(id=="N01S03")
            {
                Door("Escalones",2,"N01S02",new Vector3(37,.05f,0));Door("Ramal del derrumbe",40,"N01S04",new Vector3(4,.05f,0));
                var enemyPrefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Afterglow/Prefabs/Enemy.prefab");
                var enemy=(GameObject)PrefabUtility.InstantiatePrefab(enemyPrefab,target);enemy.name="N01.TUTORIAL.RESISTANCE";enemy.transform.position=new Vector3(20,.05f,0);
                var loot=enemy.AddComponent<M01EnemyLoot>();loot.material=stone;loot.firstTutorialOffering=true;
                var son=Actor("Hijo del minero",new Vector3(10,1,1),new Color(.25f,.75f,1));
                var tutorial=Story(son,"N1B.PICKAXE",new[]{"N1B.02","N1B.04","N1B.05","N1B.06"},true,3);tutorial.givePickaxe=true;tutorial.wakeEnemies=new[]{enemy.GetComponent<AfterglowEnemy>()};
                var enemies=new System.Collections.Generic.List<AfterglowEnemy>(tutorial.wakeEnemies);
                for(int i=0;i<2;i++){var other=(GameObject)PrefabUtility.InstantiatePrefab(enemyPrefab,target);other.name="N01.RESISTANCE."+i;other.transform.position=new Vector3(28+i*6,.05f,i==0?2:-2);var otherLoot=other.AddComponent<M01EnemyLoot>();otherLoot.material=stone;enemies.Add(other.GetComponent<AfterglowEnemy>());}
                tutorial.wakeEnemies=enemies.ToArray();
                var reward=Story(new GameObject("Primer botín"),"N1B.LOOT",new[]{"N1B.08","N1B.09"},true,100);reward.prerequisiteEvent="N1B.PICKAXE";reward.defeatedEnemy=enemy.GetComponent<AfterglowEnemy>();
                Offering(id,0,new Vector3(30,.35f,2));
            }
            if(id=="N01S04")
            {
                Door("Cámara anterior",2,"N01S03",new Vector3(37,.05f,0));
                var helmet=GameObject.CreatePrimitive(PrimitiveType.Sphere);helmet.name="Casco Q02";helmet.transform.position=new Vector3(13,.4f,3);helmet.transform.localScale=new Vector3(.6f,.35f,.5f);UnityEngine.Object.DestroyImmediate(helmet.GetComponent<Collider>());helmet.GetComponent<Renderer>().sharedMaterial=Role("Casco",Color.yellow);helmet.AddComponent<M01QuestObject>();WorldLabel("Casco",new Vector3(13,1.4f,3),Color.black,2);
                for(int i=0;i<4;i++)Box("Ascenso "+i,new Vector3(26+i*1.5f,.15f*(i+1),1.5f),new Vector3(1.5f,.3f*(i+1),4),stone);
                Box("Bloqueo elevado",new Vector3(36,.6f,1.5f),new Vector3(10,1.2f,4),stone);
                var miner=Actor("Minero · investigación [E]",new Vector3(37,2.2f,1),Color.yellow);miner.AddComponent<M01Investigation>().dialogue=dialogue;
                WorldLabel("Derrumbe · [E] Hablar",new Vector3(37,4,1),Color.black,2.5f);
            }
        }
        static void Offering(string room,int index,Vector3 position)
        {
            var root=GameObject.CreatePrimitive(PrimitiveType.Sphere);root.name="Ofrenda "+room+"."+index;root.transform.position=position;root.transform.localScale=Vector3.one*.35f;UnityEngine.Object.DestroyImmediate(root.GetComponent<Collider>());root.GetComponent<Renderer>().sharedMaterial=Role("Ofrenda",new Color(.4f,.85f,.35f));root.AddComponent<M01StaticOffering>().sourceId="EXP."+room+"."+index;
        }
        static M01StoryPoint Story(GameObject root,string flag,string[] lines,bool automatic,float radius)
        {
            var point=root.AddComponent<M01StoryPoint>();point.dialogue=dialogue;point.eventId=flag;point.lineIds=lines;point.automatic=automatic;point.radius=radius;return point;
        }
        static M01RouteDoor Door(string label,float x,string destination,Vector3 arrival)
        {
            var root=Box("Puerta · "+label,new Vector3(x,1.4f,0),new Vector3(1.6f,2.8f,.2f),Role("Puerta",new Color(.08f,.1f,.14f)));UnityEngine.Object.DestroyImmediate(root.GetComponent<Collider>());
            WorldLabel(label,new Vector3(x,3.3f,0),Color.black,2.4f);
            var door=root.AddComponent<M01RouteDoor>();door.destination=destination;door.arrival=arrival;return door;
        }
        static GameObject Actor(string name,Vector3 position,Color color)
        {
            var actor=Box(name,position,new Vector3(.9f,2,.08f),Role(name,color));WorldLabel(name,position+Vector3.up*1.4f,Color.black,2);return actor;
        }
        static void WorldLabel(string text,Vector3 position,Color color,float size)
        {
            var root=new GameObject(text);root.transform.position=position;var label=root.AddComponent<TextMeshPro>();label.font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Afterglow/Materials/PrototypeFont.asset");label.text=text;label.fontSize=size;label.color=color;label.alignment=TextAlignmentOptions.Center;label.rectTransform.sizeDelta=new Vector2(10,1.5f);
        }
        static Material Role(string name,Color color)
        {
            foreach(char c in System.IO.Path.GetInvalidFileNameChars())name=name.Replace(c,'_');string path="Assets/Afterglow/Materials/M01_"+name+".mat";var mat=AssetDatabase.LoadAssetAtPath<Material>(path);if(!mat){mat=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(mat,path);}mat.color=color;return mat;
        }
        static GameObject Box(string name,Vector3 position,Vector3 scale,Material mat)
        {
            var root=GameObject.CreatePrimitive(PrimitiveType.Cube);root.name=name;root.transform.position=position;root.transform.localScale=scale;root.GetComponent<Renderer>().sharedMaterial=mat;return root;
        }
    }
}
