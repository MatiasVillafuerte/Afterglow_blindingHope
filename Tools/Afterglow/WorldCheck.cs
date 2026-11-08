using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using System.Collections;
using System.IO;
using Afterglow;
public class WorldCheck:MonoBehaviour {
 public static string Main(){new GameObject("World verification").AddComponent<WorldCheck>();return "Started";}
 void Keys(params Key[] keys){InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(keys));}
 void Warp(AfterglowPlayer p,Vector3 v){var c=p.GetComponent<CharacterController>();c.enabled=false;p.transform.position=v;c.enabled=true;}
 IEnumerator Start(){DontDestroyOnLoad(gameObject);string report="# Connected world verification\n";var p=FindAnyObjectByType<AfterglowPlayer>();Keys(Key.DownArrow);yield return new WaitForSeconds(1.5f);Keys();report+="Front boundary stops movement: "+(p.transform.position.z> -3)+" z="+p.transform.position.z+"\n";
  Warp(p,new Vector3(7,.05f,3.5f));float until=Time.time+25,next=0;while(SceneManager.GetActiveScene().name=="Afterglow_Prototype"&&Time.time<until){p=FindAnyObjectByType<AfterglowPlayer>();bool jump=p.Grounded&&Time.time>next&&Physics.Raycast(p.transform.position+Vector3.up*.6f,Vector3.right,1,1<<0);if(jump){Keys(Key.RightArrow,Key.Space);next=Time.time+.3f;}else if(p.transform.position.x>=23)Keys(Key.UpArrow);else Keys(Key.RightArrow);yield return null;}Keys();report+="Raised route and first door: "+SceneManager.GetActiveScene().name+"\n";
  for(int i=0;i<2;i++){p=FindAnyObjectByType<AfterglowPlayer>();var door=FindAnyObjectByType<AfterglowDoor>();if(!door)break;string target=door.targetScene;p.AddExperience();int xp=p.Experience;Warp(p,door.transform.position);yield return new WaitForSeconds(.5f);p=FindAnyObjectByType<AfterglowPlayer>();report+="Door -> "+target+": "+(SceneManager.GetActiveScene().name==target)+" EXP preserved="+(p.Experience==xp)+"\n";}
  File.WriteAllText("Assets/Afterglow/Docs/ConnectedWorldChecks.md",report);ScreenCapture.CaptureScreenshot("Assets/Afterglow/Docs/FinalRoom.png");Destroy(gameObject);
 }
}

