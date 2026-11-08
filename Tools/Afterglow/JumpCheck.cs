using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Afterglow;
public class AfterglowJumpCheck:MonoBehaviour {
 public static string Main(){new GameObject("Jump trajectory check").AddComponent<AfterglowJumpCheck>();return "Jump check started";}
 void Keys(params Key[] keys){InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(keys));}
 IEnumerator Start(){var p=FindAnyObjectByType<AfterglowPlayer>();Keys();yield return new WaitForSeconds(.5f);while(!p.Grounded)yield return null;float start=p.transform.position.y;bool rise=false,fall=false,extra=false,walk=false;float max=start,previous=0;float began=Time.time;Keys(Key.Space);int phase=0;
  while(Time.time-began<1.5f){rise|=p.AnimationState=="Jump";fall|=p.AnimationState=="Falling";max=Mathf.Max(max,p.transform.position.y);if(phase==0&&Time.time-began>.08f){Keys();phase=1;}else if(phase==1&&p.transform.position.y>start+.8f){Keys(Key.Space);previous=p.VerticalSpeed;phase=2;}else if(phase==2){extra=p.VerticalSpeed>previous+.5f;Keys();phase=3;}yield return null;}
  Keys(Key.RightArrow);yield return new WaitForSeconds(.2f);walk=p.AnimationState=="Walk";Keys();yield return new WaitForSeconds(.1f);
  File.WriteAllText("Assets/Afterglow/Docs/JumpTrajectory.md","# Frame-observed jump verification\n\n- Jump animation: "+rise+"\n- Falling animation: "+fall+"\n- Apex height: "+(max-start)+"\n- Extra jump detected: "+extra+"\n- Walking animation: "+walk+"\n- Idle after movement: "+(p.AnimationState=="Idle")+"\n- Observed each player frame, rather than sampling the short Jump/Falling clips at fixed wall-clock offsets.\n");UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
 }
}
