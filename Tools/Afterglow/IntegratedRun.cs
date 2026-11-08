using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Afterglow;
public class AfterglowIntegratedRun:MonoBehaviour {
 public static string Main(){new GameObject("Temporary integrated run").AddComponent<AfterglowIntegratedRun>();return "Integrated run started";}
 void Keys(params Key[] keys){InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(keys));}
 IEnumerator Start(){var p=FindAnyObjectByType<AfterglowPlayer>();float deadline=Time.time+75,nextAttack=0,nextJump=0;int defeated=0;bool died=false;Vector3 final=Vector3.zero;
  while(p&&Time.time<deadline&&p.transform.position.x<98){
   if(p.Dead){died=true;break;}
   var nearby=FindObjectsByType<AfterglowEnemy>().Where(e=>!e.Defeated&&Mathf.Abs(e.transform.position.y-p.transform.position.y)<.8f&&Vector3.Distance(e.transform.position,p.transform.position)<4f).OrderBy(e=>Vector3.Distance(e.transform.position,p.transform.position)).FirstOrDefault();
   if(nearby){var delta=nearby.transform.position-p.transform.position;var keys=new System.Collections.Generic.List<Key>();if(delta.x>.15f)keys.Add(Key.RightArrow);if(delta.x<-.15f)keys.Add(Key.LeftArrow);if(delta.z>.15f)keys.Add(Key.UpArrow);if(delta.z<-.15f)keys.Add(Key.DownArrow);if(new Vector2(delta.x,delta.z).magnitude>1.3f)Keys(keys.ToArray());else if(Time.time>nextAttack){keys.Add(Key.X);Keys(keys.ToArray());nextAttack=Time.time+.75f;}else Keys();}
   else {var coin=FindObjectsByType<AfterglowPickup>().Where(c=>Mathf.Abs(c.transform.position.y-p.transform.position.y)<.8f&&Vector3.Distance(c.transform.position,p.transform.position)<4).OrderBy(c=>Vector3.Distance(c.transform.position,p.transform.position)).FirstOrDefault();
    if(coin){var delta=coin.transform.position-p.transform.position;var keys=new System.Collections.Generic.List<Key>();if(delta.x>.15f)keys.Add(Key.RightArrow);if(delta.x<-.15f)keys.Add(Key.LeftArrow);if(delta.z>.15f)keys.Add(Key.UpArrow);if(delta.z<-.15f)keys.Add(Key.DownArrow);Keys(keys.ToArray());}
    else {bool jump=p.Grounded&&Time.time>nextJump&&Physics.Raycast(p.transform.position+Vector3.up*.55f,Vector3.right,1.1f,1<<0);if(jump){Keys(Key.RightArrow,Key.Space);nextJump=Time.time+.3f;}else Keys(Key.RightArrow);}}
   defeated=FindObjectsByType<AfterglowEnemy>().Count(e=>e.Defeated);final=p.transform.position;yield return null;
  }
  Keys();yield return new WaitForSeconds(.2f);if(p)final=p.transform.position;string result="# Integrated live run\n\n"+"- "+(!died&&final.x>=98&&defeated==4&&p.Experience==12?"PASS":"FAIL")+"\n- Final position: "+final+"\n- Enemies defeated: "+defeated+" / 4\n- EXP: "+(p?p.Experience:0)+" / 12\n- Health: "+(p?p.Health:0)+"\n- Teleports, disabled enemies and direct damage calls: none. Normal arrow, jump and X input only.\n";File.WriteAllText("Assets/Afterglow/Docs/IntegratedRun.md",result);ScreenCapture.CaptureScreenshot("Assets/Afterglow/Docs/IntegratedFinish.png");Destroy(gameObject);
 }
}
