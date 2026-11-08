using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Afterglow;
public sealed class AfterglowArtTraverse:MonoBehaviour
{
 InputSettings.BackgroundBehavior previousBackground;InputSettings.EditorInputBehaviorInPlayMode previousFocus;
 public static string Main(){new GameObject("Art sample route verification").AddComponent<AfterglowArtTraverse>();return "Route started";}
 void Keys(params Key[] keys)=>InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(keys));
 IEnumerator Start()
 {
  previousBackground=InputSystem.settings.backgroundBehavior;previousFocus=InputSystem.settings.editorInputBehaviorInPlayMode;
  InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;InputSystem.EnableDevice(Keyboard.current);
  var p=FindAnyObjectByType<AfterglowPlayer>();p.testControl=false;p.animator.speed=1;var cc=p.GetComponent<CharacterController>();cc.enabled=false;p.transform.position=new Vector3(9,.05f,0);cc.enabled=true;yield return new WaitForSeconds(.2f);
  float deadline=Time.time+15;while(p.transform.position.z<2.4f&&Time.time<deadline){Keys(Key.UpArrow);yield return null;}Keys();bool depth=p.transform.position.z>=2.4f;yield return null;
  bool jumped=false,landed=false,visible=true;float maxY=p.transform.position.y;int jumpFrames=0;
  while(p.transform.position.x<31&&Time.time<deadline)
  {
   if(!jumped&&p.Grounded&&p.transform.position.x>=12){jumped=true;jumpFrames=4;}
   if(jumpFrames-->0)Keys(Key.RightArrow,Key.Space);else Keys(Key.RightArrow);
   maxY=Mathf.Max(maxY,p.transform.position.y);if(p.Grounded&&p.transform.position.x>19&&p.transform.position.y>1.1f)landed=true;
   var camera=Camera.main;var head=camera.WorldToViewportPoint(p.transform.position+Vector3.up*2);var feet=camera.WorldToViewportPoint(p.transform.position);visible&=head.z>0&&head.x>0&&head.x<1&&head.y<1&&feet.y>0;
   yield return null;
  }
  Keys();yield return new WaitForSeconds(.15f);
  bool complete=p.transform.position.x>=31;string report="# Recorrido de la muestra en Play Mode\n\n- Movimiento en profundidad con flecha: "+depth+"\n- Salto con Espacio: "+(jumped&&maxY>1.5f)+"\n- Aterrizaje sobre plataforma original: "+landed+"\n- Recorrido completo sin mejoras: "+complete+"\n- Personaje dentro del encuadre: "+visible+"\n- Llegada: "+p.transform.position.ToString("F3")+"\n- Altura máxima: "+maxY+"\n- Se recolocó solo la salida inicial. El recorrido usa teclas simuladas y el controlador real. No se teletransportó entre puntos ni se compraron habilidades.\n";
  File.WriteAllText("Docs/Arte/Prueba_Recorrido.md",report);yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot("Docs/Arte/Recorrido_Final.png");Destroy(gameObject);
 }
 void OnDestroy(){Keys();InputSystem.settings.backgroundBehavior=previousBackground;InputSystem.settings.editorInputBehaviorInPlayMode=previousFocus;}
}
