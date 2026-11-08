using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Afterglow.Editor
{
 public static class M01CharacterAnimationBuilder
 {
  const string Root="Assets/Afterglow/Character/Animations/";
  static void Curve(AnimationClip clip,string path,string property,params float[] values){var keys=new List<Keyframe>();for(int i=0;i<values.Length;i+=2)keys.Add(new Keyframe(values[i],values[i+1]));AnimationUtility.SetEditorCurve(clip,EditorCurveBinding.FloatCurve(path,typeof(Transform),property),new AnimationCurve(keys.ToArray()));}
  static string Path(Transform root,Transform child){return AnimationUtility.CalculateTransformPath(child,root);}
  public static void Configure(AfterglowPlayer player)
  {
   var animator=player.animator;var body=animator.gameObject;
   var idleSource=AssetDatabase.LoadAssetAtPath<AnimationClip>(Root+"Idle.anim");idleSource.SampleAnimation(body,0);
   var idle=new AnimationClip{name="M01_Idle",frameRate=30};var attack=new AnimationClip{name="M01_Attack",frameRate=60};
   foreach(var transform in body.GetComponentsInChildren<Transform>()){
    string path=Path(body.transform,transform);var p=transform.localPosition;var r=transform.localEulerAngles;var s=transform.localScale;
    foreach(var clip in new[]{idle,attack}){float duration=clip==idle?2.8f:.6f;
     for(int axis=0;axis<3;axis++){string a="xyz".Substring(axis,1);Curve(clip,path,"m_LocalPosition."+a,0,p[axis],duration,p[axis]);Curve(clip,path,"localEulerAnglesRaw."+a,0,r[axis],duration,r[axis]);Curve(clip,path,"m_LocalScale."+a,0,s[axis],duration,s[axis]);}}
    if(transform.name=="Player_LeftArmA"||transform.name=="Player_RightArmA"){
     float direction=transform.name.Contains("Left")?1:-1;Curve(idle,path,"localEulerAnglesRaw.z",0,r.z, .7f,r.z+7*direction,1.4f,r.z,2.1f,r.z-7*direction,2.8f,r.z);
     float lift=transform.name.Contains("Right")?125:35;Curve(attack,path,"localEulerAnglesRaw.z",0,r.z,.08f,r.z+lift,.15f,r.z-50,.3f,r.z-38,.6f,r.z);
    }
    if(transform.name.EndsWith("LegC"))Curve(idle,path,"localEulerAnglesRaw.z",0,r.z,.7f,r.z-4,1.4f,r.z,2.1f,r.z-4,2.8f,r.z);
   }
   var rootPos=body.transform.localPosition;float rise=.045f/body.transform.parent.lossyScale.y;
   Curve(idle,"","m_LocalPosition.y",0,rootPos.y,.7f,rootPos.y+rise,1.4f,rootPos.y,2.1f,rootPos.y+rise,2.8f,rootPos.y);
   var settings=AnimationUtility.GetAnimationClipSettings(idle);settings.loopTime=true;AnimationUtility.SetAnimationClipSettings(idle,settings);
   var idleAsset=Save(idle,Root+"M01_Idle.anim");var attackAsset=Save(attack,Root+"M01_Attack.anim");
   var original=AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(Root+"PlayerAC.controller");
   string controllerPath=Root+"M01_Controller.overrideController";var controller=AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(controllerPath);if(!controller){controller=new AnimatorOverrideController(original);AssetDatabase.CreateAsset(controller,controllerPath);}controller["Idle"]=idleAsset;controller["Attack"]=attackAsset;animator.runtimeAnimatorController=controller;
   string texturePath="Assets/Afterglow/Character/Tools/Pickaxe.png";var importer=(TextureImporter)AssetImporter.GetAtPath(texturePath);importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;var spriteSettings=new TextureImporterSettings();importer.ReadTextureSettings(spriteSettings);spriteSettings.spriteAlignment=(int)SpriteAlignment.Custom;spriteSettings.spritePivot=new Vector2(.23f,.12f);importer.SetTextureSettings(spriteSettings);importer.textureCompression=TextureImporterCompression.Uncompressed;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.SaveAndReimport();
   var hand=System.Array.Find(body.GetComponentsInChildren<Transform>(),t=>t.name=="Player_RightArmC");
   var tool=new GameObject("Pickaxe grip");tool.transform.SetParent(hand,false);var sprite=tool.AddComponent<SpriteRenderer>();sprite.sprite=AssetDatabase.LoadAssetAtPath<Sprite>(texturePath);sprite.sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>("Assets/Afterglow/Materials/Character.mat");sprite.sortingOrder=20;
   var handRenderer=hand.GetComponent<SpriteRenderer>();tool.transform.localPosition=hand.InverseTransformPoint(handRenderer.bounds.center);float scale=.8f/(sprite.sprite.bounds.size.y*hand.lossyScale.y);tool.transform.localScale=Vector3.one*scale;tool.transform.localRotation=Quaternion.Euler(0,0,30);sprite.enabled=false;var visual=tool.AddComponent<Afterglow.M01.M01PickaxeVisual>();visual.player=player;visual.sprite=sprite;
   idleAsset.SampleAnimation(body,0);AssetDatabase.SaveAssets();
  }
  static AnimationClip Save(AnimationClip value,string path){var asset=AssetDatabase.LoadAssetAtPath<AnimationClip>(path);if(!asset){AssetDatabase.CreateAsset(value,path);return value;}EditorUtility.CopySerialized(value,asset);Object.DestroyImmediate(value);return asset;}
 }
}
