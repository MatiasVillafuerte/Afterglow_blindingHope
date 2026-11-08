using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Afterglow;
public class AfterglowPlaytest:MonoBehaviour {
 List<string> report=new List<string>();AfterglowPlayer p;string dir="Assets/Afterglow/Docs";
 public static string Main(){new GameObject("Afterglow verification (temporary)").AddComponent<AfterglowPlaytest>();return "Playtest started";}
 void Check(string name,bool ok,string detail=""){report.Add("- ["+(ok?"x":" ")+"] "+name+" — "+(ok?"PASS":"FAIL")+" "+detail);File.WriteAllText(dir+"/PlaytestResults.md","# Live Play Mode verification\n\n"+string.Join("\n",report));}
 void Keys(params Key[] keys){InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(keys));}
 void Warp(Vector3 pos){var c=p.GetComponent<CharacterController>();c.enabled=false;p.transform.position=pos;c.enabled=true;Physics.SyncTransforms();}
 IEnumerator Wait(float seconds){yield return new WaitForSeconds(seconds);}
 IEnumerator Tap(Key key,float duration=.06f){Keys(key);yield return Wait(duration);Keys();yield return null;}
 IEnumerator Start(){
  DontDestroyOnLoad(gameObject);p=FindAnyObjectByType<AfterglowPlayer>();var enemies=FindObjectsByType<AfterglowEnemy>();
  Check("Play loop advances",Time.frameCount>10,"frame="+Time.frameCount);yield return Wait(.2f);
  Vector3 start=p.transform.position;Keys(Key.RightArrow);yield return Wait(.5f);Keys();Check("Right arrow",p.transform.position.x>start.x+2);
  start=p.transform.position;Keys(Key.LeftArrow);yield return Wait(.5f);Keys();Check("Left arrow",p.transform.position.x<start.x-2);
  start=p.transform.position;Keys(Key.UpArrow);yield return Wait(.35f);Keys();Check("Up arrow depth",p.transform.position.z>start.z+1);
  start=p.transform.position;Keys(Key.DownArrow);yield return Wait(.35f);Keys();Check("Down arrow depth",p.transform.position.z<start.z-1);
  start=p.transform.position;Keys(Key.RightArrow,Key.UpArrow);yield return Wait(.3f);Keys();float diagonal=Vector3.Distance(start,p.transform.position);Check("Normalized diagonal",Mathf.Abs(diagonal-1.5f)<.15f,"distance="+diagonal);
  Warp(new Vector3(5,.04f,0));Keys();yield return Wait(.3f);while(!p.Grounded)yield return null;
  float jumpBegin=Time.time,jumpBase=p.transform.position.y,previousSpeed=0;bool sawJump=false,sawFall=false,doubleBoost=false;int jumpPhase=0;Keys(Key.Space);
  while(Time.time-jumpBegin<1.5f){sawJump|=p.AnimationState=="Jump";sawFall|=p.AnimationState=="Falling";if(jumpPhase==0&&Time.time-jumpBegin>.08f){Keys();jumpPhase=1;}else if(jumpPhase==1&&p.transform.position.y>jumpBase+.8f){Keys(Key.Space);previousSpeed=p.VerticalSpeed;jumpPhase=2;}else if(jumpPhase==2){doubleBoost=p.VerticalSpeed>previousSpeed+.5f;Keys();jumpPhase=3;}yield return null;}
  Check("No double jump",!doubleBoost&&jumpPhase==3);Check("Jump animation",sawJump);Check("Falling animation",sawFall);Check("Lands and idles",p.Grounded&&p.AnimationState=="Idle");
  // Traverse the entire route using normal input and collisions. Enemies disabled only to isolate jumps.
  foreach(var e in enemies){e.enabled=false;e.GetComponent<CharacterController>().enabled=false;}
  Warp(new Vector3(4,.04f,0));yield return Wait(.1f);float timeout=Time.time+40;int jumps=0;float nextJump=0;float maxHeight=0;bool visible=true;
  while(p.transform.position.x<95&&Time.time<timeout){
   bool obstacle=Physics.Raycast(p.transform.position+Vector3.up*.55f,Vector3.right,1.1f,1<<0);
   bool jump=obstacle&&p.Grounded&&Time.time>nextJump;
   if(jump){nextJump=Time.time+.3f;jumps++;Keys(Key.RightArrow,Key.Space);}else Keys(Key.RightArrow);
   maxHeight=Mathf.Max(maxHeight,p.transform.position.y);
   var vp=Camera.main.WorldToViewportPoint(p.transform.position+Vector3.up);visible &= vp.z>0&&vp.x>.02f&&vp.x<.98f&&vp.y>.02f&&vp.y<.98f;
   yield return null;
  }
  Keys();Check("Full route with single jumps",p.transform.position.x>=95,"x="+p.transform.position.x+" jumps="+jumps+" maxY="+maxHeight);Check("Camera keeps player visible",visible);
  ScreenCapture.CaptureScreenshot(dir+"/Finish.png");yield return Wait(.2f);
  // Controlled combat setup; no damage methods called for enemy death test.
  var enemy=enemies.OrderBy(e=>e.transform.position.x).First();enemy.GetComponent<CharacterController>().enabled=true;Warp(new Vector3(enemy.transform.position.x-1.4f,.04f,0));yield return Wait(.15f);Keys(Key.RightArrow);yield return Wait(.02f);Keys();
  Keys(Key.X);yield return Wait(1.3f);Keys();Check("Holding X attacks only once",enemy.Health==2,"health="+enemy.Health);yield return Wait(.1f);
  yield return Tap(Key.X);yield return Wait(.7f);Check("Second hit",enemy.Health==1,"health="+enemy.Health);
  yield return Tap(Key.X);yield return Wait(.7f);Check("Third hit defeats enemy",enemy.Defeated&&enemy.HitsTaken==3);Check("Exactly three EXP drops",FindObjectsByType<AfterglowPickup>().Length+p.Experience==3,"world + already collected = "+(FindObjectsByType<AfterglowPickup>().Length+p.Experience));
  foreach(var coin in FindObjectsByType<AfterglowPickup>()){if(!coin)continue;Warp(new Vector3(coin.transform.position.x,.04f,coin.transform.position.z));yield return Wait(.15f);}
  Check("Collect three once",p.Experience==3,"EXP="+p.Experience);yield return Wait(.2f);Check("No repeated EXP",p.Experience==3&&FindObjectsByType<AfterglowPickup>().Length==0);
  // Range, direction and floor rejection against another enemy, kept stationary.
  var target=enemies.OrderBy(e=>e.transform.position.x).Last();int hp=target.Health;
  Warp(target.transform.position+Vector3.left*3);yield return Tap(Key.X);yield return Wait(.7f);Check("Out of range does no damage",target.Health==hp);
  Warp(target.transform.position+Vector3.left*1.3f);Keys(Key.LeftArrow);yield return Wait(.02f);Keys();yield return Tap(Key.X);yield return Wait(.7f);Check("Behind player does no damage",target.Health==hp);
  float gravity=p.gravity;p.gravity=0;typeof(AfterglowPlayer).GetField("velocityY",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).SetValue(p,0f);
  Warp(target.transform.position+Vector3.left*1.2f+Vector3.up*3f);Keys(Key.RightArrow);yield return Wait(.02f);Keys();yield return Tap(Key.X);yield return Wait(.7f);Check("Different height does no damage",target.Health==hp);p.gravity=gravity;
  // Enemy AI and timed damage, on an ample upper terrace.
  Warp(target.transform.position+Vector3.left*4);target.GetComponent<CharacterController>().enabled=true;target.enabled=true;Vector3 old=target.transform.position;yield return Wait(.5f);Check("Enemy detects and pursues",target.transform.position.x<old.x-.2f);
  Warp(target.transform.position+Vector3.left*1.1f);int health=p.Health;float limit=Time.time+3;bool tilted=false;while(p.Health==health&&Time.time<limit){tilted|=Quaternion.Angle(target.visual.localRotation,Quaternion.identity)>5;yield return null;}
  Check("Enemy leans and hits",tilted&&p.Health==health-1);health=p.Health;yield return Wait(.3f);Check("Contact is not per-frame damage",p.Health==health);
  target.enabled=false;yield return Wait(1.1f);bool first=p.TakeDamage(1);bool second=p.TakeDamage(1);Check("One second invulnerability",first&&!second);
  ScreenCapture.CaptureScreenshot(dir+"/CombatHUD.png");yield return Wait(.2f);
  p.TakeDamage(100);yield return Wait(1.1f);p.TakeDamage(100);yield return Wait(2);
  p=FindAnyObjectByType<AfterglowPlayer>();Check("Death resets complete state",p!=null&&!p.Dead&&p.Health==10&&p.Experience==0&&p.transform.position.x<5&&FindObjectsByType<AfterglowEnemy>().Length==4&&FindObjectsByType<AfterglowPickup>().Length==0);
  Warp(new Vector3(4,-10,0));yield return Wait(2);p=FindAnyObjectByType<AfterglowPlayer>();Check("Fall resets complete state",p!=null&&!p.Dead&&p.Health==10&&p.Experience==0&&p.transform.position.x<5);
  Check("No duplicate camera or HUD",FindObjectsByType<Camera>().Length==1&&FindObjectsByType<AfterglowHUD>().Length==1);
  Keys();ScreenCapture.CaptureScreenshot(dir+"/VerifiedInitial.png");Check("Verification complete",true);Destroy(gameObject);
 }
}
