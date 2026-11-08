using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Afterglow;
public sealed class M01TraverseBase:MonoBehaviour
{
 public static string Main(){new GameObject("M01 basic route check").AddComponent<M01TraverseBase>();return "Route check started";}
 IEnumerator Start(){var p=FindAnyObjectByType<AfterglowPlayer>();var enemy=FindAnyObjectByType<AfterglowEnemy>();enemy.enabled=false;var controller=p.GetComponent<CharacterController>();controller.enabled=false;p.transform.position=new Vector3(4,.05f,0);controller.enabled=true;yield return new WaitForSeconds(.2f);float timeout=Time.time+15,next=0;int jumps=0;bool visible=true;
  while(p.transform.position.x<36&&Time.time<timeout){bool gap=!Physics.Raycast(p.transform.position+Vector3.right*.9f+Vector3.up*.5f,Vector3.down,1,1<<0);if(p.Grounded&&gap&&Time.time>next){InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(Key.RightArrow,Key.Space));next=Time.time+.3f;jumps++;}else InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(Key.RightArrow));var head=Camera.main.WorldToViewportPoint(p.transform.position+Vector3.up*2);var foot=Camera.main.WorldToViewportPoint(p.transform.position);visible&=head.z>0&&head.x>0&&head.x<1&&head.y>0&&head.y<1&&foot.y>0;yield return null;}
  InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());File.WriteAllText("Docs/Implementacion/Prueba_Ruta_Base.md","# Ruta básica con entrada normal\n\n- Llegada: "+p.transform.position+"\n- Saltos: "+jumps+"\n- Llegó sin mejoras: "+(p.transform.position.x>=36)+"\n- Personaje visible: "+visible+"\n- Solo se colocó al jugador en la salida inicial; recorrido con flecha y salto normal, sin teletransportes intermedios. Enemigo desactivado para aislar plataformas.\n");Destroy(gameObject);
 }
}
