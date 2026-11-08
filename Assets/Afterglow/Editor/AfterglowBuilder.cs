using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor.Animations;
using TMPro;
namespace Afterglow.Editor {
public static class AfterglowBuilder {
 const string Root="Assets/Afterglow", ScenePath=Root+"/Scenes/Afterglow_Prototype.unity";
 static Material ground,platform,sprite,enemyMaterial,reward;static TMP_FontAsset font;
 [MenuItem("Afterglow/Build Prototype")]
 public static void Build(){
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode before building.");
  for(int i=0;i<SceneManager.sceneCount;i++)if(SceneManager.GetSceneAt(i).isDirty)throw new InvalidOperationException("Save unsaved scene changes before building. No scene was replaced.");
  Directory.CreateDirectory(Root+"/Materials");Directory.CreateDirectory(Root+"/Prefabs");Directory.CreateDirectory(Root+"/Scenes");AssetDatabase.Refresh();
  ground=Mat("Ground",new Color(.64f,.66f,.68f));platform=Mat("Platforms",new Color(.83f,.84f,.85f));enemyMaterial=Mat("Enemy",new Color(.23f,.25f,.28f));reward=Mat("Experience",Color.white);sprite=Mat("Character",Color.white,"Sprites/Default");
  var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
  RenderSettings.ambientLight=new Color(.7f,.73f,.78f);RenderSettings.fog=false;
  var sun=new GameObject("Directional Light").AddComponent<Light>();sun.type=LightType.Directional;sun.intensity=1.4f;sun.transform.rotation=Quaternion.Euler(45,-35,0);
  Box("Base 100m",new Vector3(50,-.5f,0),new Vector3(100,1,10),ground);
  Box("Back limit",new Vector3(50,1,5.3f),new Vector3(100,2,.6f),ground);
  Box("Front limit",new Vector3(50,.25f,-5.3f),new Vector3(100,.5f,.6f),ground);
  Box("Start limit",new Vector3(-.3f,1,0),new Vector3(.6f,2,10),ground);Box("End limit",new Vector3(100.3f,4,0),new Vector3(.6f,8,10),ground);
  // Continuous safe floor, with a main stair route rising in steps of 1.2m.
  Box("Step A",new Vector3(23,.6f,1),new Vector3(7,1.2f,6),platform);
  Box("Step B",new Vector3(30.75f,1.2f,1),new Vector3(8.5f,2.4f,6),platform);
  Box("Combat terrace",new Vector3(42,1.8f,0),new Vector3(14,3.6f,8),platform);
  Box("Descent B",new Vector3(53,1.2f,1),new Vector3(8,2.4f,6),platform);
  Box("Descent A",new Vector3(60,.6f,1),new Vector3(6,1.2f,6),platform);
  Box("Final step A",new Vector3(70,.6f,0),new Vector3(6,1.2f,8),platform);
  Box("Final step B",new Vector3(77,1.2f,0),new Vector3(8,2.4f,8),platform);
  Box("Final step C",new Vector3(84,1.8f,0),new Vector3(6,3.6f,8),platform);
  Box("Finish terrace",new Vector3(93.5f,2.4f,0),new Vector3(13,4.8f,8),platform);
  var player=new GameObject("Player");player.layer=2;player.transform.position=new Vector3(4,.05f,0);
  var cc=player.AddComponent<CharacterController>();cc.height=2;cc.radius=.33f;cc.center=Vector3.up;cc.stepOffset=.25f;cc.skinWidth=.04f;
  var p=player.AddComponent<AfterglowPlayer>();
  var wrapper=new GameObject("VisualFacing").transform;wrapper.SetParent(player.transform,false);p.visual=wrapper;
  var normalize=new GameObject("RigOffset").transform;normalize.SetParent(wrapper,false);
  var body=ImportCharacter();body.transform.SetParent(normalize,false);
  var animator=body.GetComponent<Animator>();p.animator=animator;
  AssetDatabase.LoadAssetAtPath<AnimationClip>(Root+"/Character/Animations/Idle.anim").SampleAnimation(body,0);
  var renderers=body.GetComponentsInChildren<SpriteRenderer>();Bounds bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
  float scale=2/bounds.size.y;normalize.localPosition-=new Vector3(bounds.center.x,bounds.min.y,bounds.center.z)-wrapper.position;wrapper.localScale=Vector3.one*scale;
  // The rig's animated root stays below a normalization wrapper, separate from movement.
  foreach(var t in player.GetComponentsInChildren<Transform>())t.gameObject.layer=2;
  PrefabUtility.SaveAsPrefabAsset(player,Root+"/Prefabs/Player.prefab");
  Enemy(new Vector3(17,.02f,0));Enemy(new Vector3(40,3.62f,0));Enemy(new Vector3(58,1.22f,1));Enemy(new Vector3(94,4.82f,0));
  var camera=new GameObject("Main Camera");camera.tag="MainCamera";var cam=camera.AddComponent<Camera>();cam.fieldOfView=38;cam.nearClipPlane=.1f;cam.farClipPlane=180;cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=new Color(.15f,.18f,.22f);camera.AddComponent<AudioListener>();var follow=camera.AddComponent<AfterglowCamera>();follow.player=p;follow.Snap();
  BuildHUD(p,renderers.First(r=>r.name=="Player_Head").sprite);
  EditorSceneManager.SaveScene(scene,ScenePath);
  var scenes=EditorBuildSettings.scenes.ToList();if(!scenes.Any(s=>s.path==ScenePath))scenes.Add(new EditorBuildSettingsScene(ScenePath,true));else scenes.First(s=>s.path==ScenePath).enabled=true;EditorBuildSettings.scenes=scenes.ToArray();
  AssetDatabase.SaveAssets();Selection.activeGameObject=player;
 }
 static Material Mat(string name,Color color,string shader="Universal Render Pipeline/Lit"){
  string path=Root+"/Materials/"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(!m){m=new Material(Shader.Find(shader));AssetDatabase.CreateAsset(m,path);}m.color=color;return m;
 }
 static void Box(string name,Vector3 pos,Vector3 size,Material mat){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.position=pos;g.transform.localScale=size;g.GetComponent<Renderer>().sharedMaterial=mat;}
 static void Enemy(Vector3 pos){var g=new GameObject("Enemy");g.layer=2;g.transform.position=pos;var cc=g.AddComponent<CharacterController>();cc.height=1.8f;cc.radius=.35f;cc.center=Vector3.up*.9f;cc.stepOffset=.15f;var e=g.AddComponent<AfterglowEnemy>();e.rewardMaterial=reward;
  var v=GameObject.CreatePrimitive(PrimitiveType.Cube);v.name="Flat enemy visual";v.layer=2;UnityEngine.Object.DestroyImmediate(v.GetComponent<Collider>());v.transform.SetParent(g.transform,false);v.transform.localPosition=Vector3.up*.9f;v.transform.localScale=new Vector3(.95f,1.8f,.08f);v.GetComponent<Renderer>().sharedMaterial=enemyMaterial;e.visual=v.transform;
  PrefabUtility.SaveAsPrefabAsset(g,Root+"/Prefabs/Enemy.prefab");}
 static string Field(string text,string name){return Regex.Match(text,@"(?m)^  "+name+@": (.*)$").Groups[1].Value.Trim();}
 static long Ref(string text,string name){var v=Regex.Match(Field(text,name),@"fileID: (-?\d+)");return v.Success?long.Parse(v.Groups[1].Value):0;}
 static float[] Values(string text,string name){return Regex.Matches(Field(text,name),@"[xyzw]: ([-\d.eE+]+)").Cast<Match>().Select(m=>float.Parse(m.Groups[1].Value,CultureInfo.InvariantCulture)).ToArray();}
 static GameObject ImportCharacter(){
  string legacySource="References/CharacterSource/Assets/00_Scenes/SampleScene.unity";
  string yaml=File.ReadAllText(File.Exists(legacySource)?legacySource:"Reference/CharacterSource/UnitySource/Assets/00_Scenes/SampleScene.unity");
  var blocks=Regex.Matches(yaml,@"(?ms)^--- !u!(\d+) &(-?\d+)\r?\n(.*?)(?=^--- !u!|\z)").Cast<Match>().ToArray();
  var gos=blocks.Where(m=>m.Groups[1].Value=="1").ToDictionary(m=>long.Parse(m.Groups[2].Value),m=>Field(m.Groups[3].Value,"m_Name"));
  var ts=blocks.Where(m=>m.Groups[1].Value=="4").ToDictionary(m=>long.Parse(m.Groups[2].Value),m=>m.Groups[3].Value);
  long root=ts.First(k=>gos[Ref(k.Value,"m_GameObject")]=="Body").Key;var objects=new Dictionary<long,Transform>();
  Transform Make(long id){if(objects.TryGetValue(id,out var found))return found;var data=ts[id];var g=new GameObject(gos[Ref(data,"m_GameObject")]);objects[id]=g.transform;
   long parent=Ref(data,"m_Father");if(id!=root)g.transform.SetParent(Make(parent),false);
   var p=Values(data,"m_LocalPosition");var r=Values(data,"m_LocalRotation");var s=Values(data,"m_LocalScale");g.transform.localPosition=new Vector3(p[0],p[1],p[2]);g.transform.localRotation=new Quaternion(r[0],r[1],r[2],r[3]);g.transform.localScale=new Vector3(s[0],s[1],s[2]);return g.transform;}
  bool Descendant(long id){if(id==root)return true;long father=Ref(ts[id],"m_Father");return father!=0&&ts.ContainsKey(father)&&Descendant(father);}
  foreach(long id in ts.Keys.Where(Descendant))Make(id);
  var sprites=AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GUIDToAssetPath("7634fb7576e2b764eb2186c001c4c82b")).OfType<Sprite>().ToDictionary(s=>{AssetDatabase.TryGetGUIDAndLocalFileIdentifier(s,out string guid,out long id);return id;},s=>s);
  foreach(var block in blocks.Where(m=>m.Groups[1].Value=="212")){string data=block.Groups[3].Value;long go=Ref(data,"m_GameObject");var entry=objects.FirstOrDefault(o=>Ref(ts[o.Key],"m_GameObject")==go);if(entry.Value==null)continue;var sr=entry.Value.gameObject.AddComponent<SpriteRenderer>();sr.sprite=sprites[Ref(data,"m_Sprite")];sr.sortingOrder=int.Parse(Field(data,"m_SortingOrder"));sr.sharedMaterial=sprite;}
  var body=objects[root].gameObject;var anim=body.AddComponent<Animator>();anim.runtimeAnimatorController=AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(Root+"/Character/Animations/PlayerAC.controller");anim.applyRootMotion=false;anim.cullingMode=AnimatorCullingMode.AlwaysAnimate;
  return body;
 }
 static RectTransform Rect(string name,Transform parent,Vector2 anchor,Vector2 pos,Vector2 size){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=anchor;r.anchoredPosition=pos;r.sizeDelta=size;return r;}
 static TMPro.TextMeshProUGUI Text(string name,Transform parent,string value,Vector2 anchor,Vector2 pos,Vector2 size,int fontSize){var r=Rect(name,parent,anchor,pos,size);var t=r.gameObject.AddComponent<TMPro.TextMeshProUGUI>();t.font=font;t.text=value;t.fontSize=fontSize;t.color=Color.white;t.raycastTarget=false;return t;}
 static UnityEngine.UI.Image Image(string name,Transform parent,Vector2 anchor,Vector2 pos,Vector2 size,Color color){var r=Rect(name,parent,anchor,pos,size);var i=r.gameObject.AddComponent<UnityEngine.UI.Image>();i.color=color;i.raycastTarget=false;return i;}
 static void BuildHUD(AfterglowPlayer player,Sprite head){
  if(!Resources.Load<TMP_Settings>("TMP Settings")){Directory.CreateDirectory(Root+"/Resources");AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TMP_Settings>(),Root+"/Resources/TMP Settings.asset");AssetDatabase.SaveAssets();}
  string fontPath=Root+"/Materials/PrototypeFont.asset";font=AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontPath);
  if(!font){font=TMP_FontAsset.CreateFontAsset(AssetDatabase.LoadAssetAtPath<Font>(Root+"/Character/PrototypeUI.ttf"));font.name="PrototypeFont";font.TryAddCharacters("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 /:·…↑↓←→áí");AssetDatabase.CreateAsset(font,fontPath);AssetDatabase.AddObjectToAsset(font.material,font);foreach(var atlas in font.atlasTextures)AssetDatabase.AddObjectToAsset(atlas,font);}
  var canvas=new GameObject("HUD",typeof(Canvas),typeof(UnityEngine.UI.CanvasScaler),typeof(UnityEngine.UI.GraphicRaycaster));canvas.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;var scaler=canvas.GetComponent<UnityEngine.UI.CanvasScaler>();scaler.uiScaleMode=UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1280,720);scaler.matchWidthOrHeight=.5f;
  var hud=canvas.AddComponent<AfterglowHUD>();hud.player=player;
  var panel=Image("Health panel",canvas.transform,new Vector2(0,1),new Vector2(24,-24),new Vector2(340,86),new Color(.07f,.09f,.12f,.95f));
  var portrait=Image("Head",panel.transform,new Vector2(0,1),new Vector2(6,-4),new Vector2(78,78),Color.white);portrait.sprite=head;portrait.preserveAspect=true;
  var bar=Image("Health background",panel.transform,new Vector2(0,1),new Vector2(96,-45),new Vector2(224,18),new Color(.25f,.26f,.29f));
  var fill=Image("Health fill",bar.transform,Vector2.zero,Vector2.zero,Vector2.zero,new Color(.92f,.38f,.4f));fill.rectTransform.anchorMax=Vector2.one;fill.rectTransform.offsetMin=fill.rectTransform.offsetMax=Vector2.zero;hud.fill=fill.rectTransform;
  hud.health=Text("Health",panel.transform,"10 / 10",new Vector2(0,1),new Vector2(96,-9),new Vector2(220,32),26);
  var xp=Image("EXP panel",canvas.transform,new Vector2(0,1),new Vector2(380,-24),new Vector2(190,60),new Color(.07f,.09f,.12f,.95f));hud.experience=Text("Experience",xp.transform,"EXP: 0",new Vector2(0,1),new Vector2(18,-12),new Vector2(165,40),26);
  Text("Controls",canvas.transform,"FLECHAS  Mover     ESPACIO  Saltar     X  Atacar",new Vector2(.5f,0),new Vector2(0,18),new Vector2(720,30),19).alignment=TextAlignmentOptions.Center;
  hud.message=Text("Restart",canvas.transform,"",new Vector2(.5f,.5f),Vector2.zero,new Vector2(650,80),36);hud.message.alignment=TextAlignmentOptions.Center;
 }
}
}

