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
    public static class M01FoundationBuilder
    {
        public const string Path = "Assets/Afterglow/Scenes/M01/M01_Base.unity";
        [MenuItem("Afterglow/Mundo 1/Build Foundation")]
        public static void Build()
        {
            if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play Mode first.");
            for (int i = 0; i < SceneManager.sceneCount; i++)
                if (SceneManager.GetSceneAt(i).isDirty) throw new InvalidOperationException("Save open scene changes first.");
            // Read an approved scene; never rebuild it or alter its camera.
            var source = EditorSceneManager.OpenScene("Assets/Afterglow/Scenes/Afterglow_Prototype.unity");
            var sourceCamera = UnityEngine.Object.FindAnyObjectByType<AfterglowCamera>();
            var sourceHUD = UnityEngine.Object.FindAnyObjectByType<AfterglowHUD>();
            var target = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            SceneManager.SetActiveScene(target);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Afterglow/Prefabs/Player.prefab");
            var root = (GameObject)PrefabUtility.InstantiatePrefab(prefab, target);
            root.transform.position = new Vector3(4, .05f, 0);
            var player = root.GetComponent<AfterglowPlayer>();
            M01CharacterAnimationBuilder.Configure(player);
            var rules = root.AddComponent<M01PlayerRules>();
            rules.levelEntrance = new Vector3(4, .05f, 0);
            rules.sectionEntrance = new Vector3(8, .05f, 0);
            var camera = UnityEngine.Object.Instantiate(sourceCamera.gameObject);
            SceneManager.MoveGameObjectToScene(camera, target);
            camera.GetComponent<AfterglowCamera>().player = player;
            camera.GetComponent<AfterglowCamera>().Snap();
            var hudRoot = UnityEngine.Object.Instantiate(sourceHUD.gameObject);
            SceneManager.MoveGameObjectToScene(hudRoot, target);
            var hud = hudRoot.GetComponent<AfterglowHUD>(); hud.player = player;
            var dialogue = hudRoot.AddComponent<M01Dialogue>();
            dialogue.script = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Afterglow/Data/M01/Guion_M01.json");
            var panel = Panel("Dialogue", hudRoot.transform, new Vector2(.5f, 0), new Vector2(0, 48), new Vector2(1180, 320), new Color(.98f, .97f, .91f, 1));
            dialogue.panel = panel.gameObject.AddComponent<CanvasGroup>();
            var badge=Panel("Speaker badge",panel,new Vector2(0,1),new Vector2(26,18),new Vector2(390,48),new Color(1,.78f,.35f));
            dialogue.speakerLabel=Label("Speaker",badge,new Vector2(16,-8),new Vector2(360,36),25);dialogue.speakerLabel.color=Color.black;
            dialogue.body = Label("Text", panel, new Vector2(34, -58), new Vector2(1112, 192), 27);dialogue.body.color=Color.black;
            dialogue.body.rectTransform.anchorMin=Vector2.zero;dialogue.body.rectTransform.anchorMax=Vector2.one;dialogue.body.rectTransform.offsetMin=new Vector2(34,76);dialogue.body.rectTransform.offsetMax=new Vector2(-34,-58);
            dialogue.body.overflowMode=TextOverflowModes.Page;
            var footer=Panel("Continue key",panel,new Vector2(1,0),new Vector2(-26,16),new Vector2(410,44),new Color(.88f,.87f,.78f));
            dialogue.hint = Label("Advance", footer, new Vector2(16,-8), new Vector2(380,32),22);dialogue.hint.color=Color.black;
            dialogue.choiceLabels=new TMP_Text[3];dialogue.choiceBackgrounds=new UnityEngine.UI.Image[3];
            for(int i=0;i<3;i++){var row=Panel("Choice "+i,panel,new Vector2(1,1),new Vector2(-26,-60-i*64),new Vector2(300,56),Color.white);dialogue.choiceBackgrounds[i]=row.GetComponent<UnityEngine.UI.Image>();dialogue.choiceLabels[i]=Label("Option",row,new Vector2(14,-10),new Vector2(274,40),25);dialogue.choiceLabels[i].color=Color.black;row.gameObject.SetActive(false);}
            var inventoryPhone = hudRoot.AddComponent<M01InventoryPhone>(); inventoryPhone.dialogue = dialogue; inventoryPhone.player = player;
            var countersPanel = Panel("Resources panel", hudRoot.transform, new Vector2(0, 1), new Vector2(24, -118), new Vector2(1232, 48), new Color(.05f,.07f,.1f,.95f));
            var counters = hudRoot.AddComponent<M01Counters>(); counters.text = Label("Resources", countersPanel, new Vector2(12, -10), new Vector2(1208, 32), 19);
            var black = Panel("Fade", hudRoot.transform, new Vector2(.5f, .5f), Vector2.zero, new Vector2(5000, 5000), Color.black);
            rules.fade = black.gameObject.AddComponent<CanvasGroup>(); rules.fade.alpha = 0;
            foreach (var graphic in black.GetComponentsInChildren<UnityEngine.UI.Graphic>()) graphic.raycastTarget = false;
            var help = hudRoot.GetComponentsInChildren<TMP_Text>().FirstOrDefault(text => text.name == "Controls");
            if (help) { help.text = "Flechas: mover   Espacio: saltar   X: pico en suelo   E: hablar"; help.color = Color.black; }
            var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Afterglow/Materials/Platforms.mat");
            Box("Floor A", new Vector3(10, -.5f, 0), new Vector3(20, 1, 12), mat);
            Box("Floor B", new Vector3(33.5f, -.5f, 0), new Vector3(21, 1, 12), mat);
            Box("Step", new Vector3(14, .6f, 2), new Vector3(4, 1.2f, 3), mat);
            Box("Rear wall", new Vector3(22, 6, 6), new Vector3(44, 12, .4f), mat);
            var boundary = Box("Invisible front", new Vector3(22, 10, -6), new Vector3(44, 24, .4f), mat); boundary.GetComponent<MeshRenderer>().enabled = false;
            var sun = new GameObject("Light").AddComponent<Light>(); sun.type = LightType.Directional; sun.intensity = 1.3f; sun.transform.rotation = Quaternion.Euler(45, -30, 0);
            var npc = Box("Hijo del minero (prueba de diálogo)", new Vector3(7, 1, 1), new Vector3(.9f, 2, .08f), mat);
            npc.GetComponent<Renderer>().sharedMaterial = RoleMaterial("Hijo", new Color(.25f,.75f,1));
            var point = npc.AddComponent<M01DialoguePoint>(); point.dialogue = dialogue; point.radius = 2.5f;
            point.lineIds = new[] { "N1A.05", "N1A.06", "N1A.07" };
            var enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Afterglow/Prefabs/Enemy.prefab");
            var enemy = (GameObject)PrefabUtility.InstantiatePrefab(enemyPrefab, target); enemy.transform.position = new Vector3(11, .05f, -1);
            var loot = enemy.AddComponent<M01EnemyLoot>(); loot.material = mat; loot.firstTutorialOffering = true;
            var altarObject = Box("Altar [E]", new Vector3(17, .5f, -1), new Vector3(1.2f, 1, .8f), mat);
            var altar = altarObject.AddComponent<M01Altar>(); altar.dialogue = dialogue;
            for (int i=0;i<4;i++)
            {
                var offering = GameObject.CreatePrimitive(PrimitiveType.Sphere); offering.name = "Ofrenda legítima "+i; offering.transform.position = new Vector3(25+i*2,.35f,-1);offering.transform.localScale=Vector3.one*.3f;
                UnityEngine.Object.DestroyImmediate(offering.GetComponent<Collider>()); offering.GetComponent<Renderer>().sharedMaterial=mat;
                var pickup = offering.AddComponent<M01StaticOffering>();pickup.sourceId="BASE.OFFERING."+i;
            }
            var worker = Box("Minero del bloqueo [E]",new Vector3(37,1,0),new Vector3(.9f,2,.08f),mat);worker.AddComponent<M01Investigation>().dialogue=dialogue;
            worker.GetComponent<Renderer>().sharedMaterial = RoleMaterial("Trabajador", Color.yellow);
            EditorSceneManager.MarkSceneDirty(target); EditorSceneManager.SaveScene(target, Path);
            EditorSceneManager.CloseScene(source, true);
            var scenes = EditorBuildSettings.scenes.ToList();
            if (!scenes.Any(scene => scene.path == Path)) { scenes.Add(new EditorBuildSettingsScene(Path, true)); EditorBuildSettings.scenes = scenes.ToArray(); }
            AssetDatabase.SaveAssets();
        }
        static GameObject Box(string name, Vector3 position, Vector3 scale, Material mat)
        {
            var root = GameObject.CreatePrimitive(PrimitiveType.Cube); root.name = name; root.transform.position = position; root.transform.localScale = scale; root.GetComponent<Renderer>().sharedMaterial = mat; return root;
        }
        static Material RoleMaterial(string name, Color color)
        {
            string path="Assets/Afterglow/Materials/M01_"+name+".mat";
            var material=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(!material){material=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(material,path);}
            material.color=color;return material;
        }
        static RectTransform Panel(string name, Transform parent, Vector2 anchor, Vector2 position, Vector2 size, Color color)
        {
            var root = new GameObject(name, typeof(RectTransform), typeof(UnityEngine.UI.Image)); var rect = root.GetComponent<RectTransform>();
            rect.SetParent(parent, false); rect.anchorMin = rect.anchorMax = rect.pivot = anchor; rect.anchoredPosition = position; rect.sizeDelta = size;
            var image=root.GetComponent<UnityEngine.UI.Image>();image.color=color;image.raycastTarget=false;
            if(name=="Dialogue"||name=="Speaker badge"||name=="Continue key"||name.StartsWith("Choice ")){image.sprite=AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");image.type=UnityEngine.UI.Image.Type.Sliced;image.pixelsPerUnitMultiplier=.5f;}
            return rect;
        }
        static TMP_Text Label(string name, Transform parent, Vector2 position, Vector2 size, float fontSize)
        {
            var root = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI)); var rect = root.GetComponent<RectTransform>();
            rect.SetParent(parent, false); rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1); rect.anchoredPosition = position; rect.sizeDelta = size;
            var text = root.GetComponent<TextMeshProUGUI>(); text.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Afterglow/Materials/PrototypeFont.asset"); text.fontSize = fontSize; text.color = Color.white; text.raycastTarget = false; return text;
        }
    }
}
