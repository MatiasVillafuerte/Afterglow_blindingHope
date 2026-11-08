using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Afterglow;
public class AfterglowExtraChecks:MonoBehaviour {
 string file="Assets/Afterglow/Docs/ExtraChecks.md";AfterglowPlayer p;
 public static string Main(){new GameObject("Extra temporary checks").AddComponent<AfterglowExtraChecks>();return "Extra checks started";}
 void Check(string label,bool ok,string info=""){File.AppendAllText(file,"- ["+(ok?"x":" ")+"] "+label+" — "+(ok?"PASS":"FAIL")+" "+info+"\n");}
 void Warp(Component obj,Vector3 pos){var c=obj.GetComponent<CharacterController>();c.enabled=false;obj.transform.position=pos;c.enabled=true;Physics.SyncTransforms();}
 void Keys(params Key[] keys){InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(keys));}
 IEnumerator Start(){File.WriteAllText(file,"# Additional live Play Mode checks\n\n");p=FindAnyObjectByType<AfterglowPlayer>();var enemies=FindObjectsByType<AfterglowEnemy>();foreach(var e in enemies)e.enabled=false;
  Warp(p,new Vector3(5,.04f,0));Keys();yield return new WaitForSeconds(.2f);Keys(Key.Space);yield return new WaitForSeconds(.15f);Keys(Key.X);yield return new WaitForSeconds(.08f);Keys();Check("Attack animation in the air",!p.Grounded&&p.AnimationState=="Attack");ScreenCapture.CaptureScreenshot("Assets/Afterglow/Docs/AirAttack.png");yield return new WaitForSeconds(.9f);
  Check("Recovers to idle after airborne attack",p.Grounded&&p.AnimationState=="Idle");
  p.TakeDamage(1);p.AddExperience();yield return new WaitForSeconds(.1f);var hud=FindAnyObjectByType<AfterglowHUD>();Check("HUD health, fill and EXP update",hud.health.text=="9 / 10"&&hud.experience.text=="EXP: 1"&&Mathf.Abs(hud.fill.anchorMax.x-.9f)<.001f);ScreenCapture.CaptureScreenshot("Assets/Afterglow/Docs/HUDUpdate.png");
  p.enabled=false;p.GetComponent<CharacterController>().enabled=false;
  var terrace=enemies.OrderBy(e=>e.transform.position.x).ElementAt(1);Warp(terrace,new Vector3(48.25f,3.64f,0));p.transform.position=new Vector3(52,3.64f,0);terrace.enabled=true;yield return new WaitForSeconds(1.5f);Check("Enemy stops at platform edge",terrace.transform.position.x<48.8f&&terrace.transform.position.y>3.4f,"position="+terrace.transform.position);terrace.enabled=false;
  var ground=enemies.OrderBy(e=>e.transform.position.x).First();Warp(ground,new Vector3(18,.04f,0));p.transform.position=new Vector3(22,.04f,0);ground.enabled=true;yield return new WaitForSeconds(1.5f);Check("Enemy cannot pass through wall",ground.transform.position.x<19.2f,"position="+ground.transform.position);ground.enabled=false;
  p.transform.position=new Vector3(40,3.64f,0);Vector3 before=ground.transform.position;ground.enabled=true;yield return new WaitForSeconds(.4f);Check("Enemy waits for inaccessible floor",Vector3.Distance(before,ground.transform.position)<.1f);ground.enabled=false;
  Keys();UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
 }
}
