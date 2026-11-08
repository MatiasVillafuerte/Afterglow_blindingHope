using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using Afterglow;
using Afterglow.M01;
public sealed class PMMapChecks:MonoBehaviour
{
 string report="# PM09–14/16: selector integrado en Play Mode\n\n";
 M01Dialogue D=>FindAnyObjectByType<M01Dialogue>();AfterglowPlayer P=>FindAnyObjectByType<AfterglowPlayer>();M01RouteMap Map=>FindAnyObjectByType<M01RouteMap>();
 InputSettings.BackgroundBehavior bg;InputSettings.EditorInputBehaviorInPlayMode focus;
 public static string Main(){var go=new GameObject("PM map checks");DontDestroyOnLoad(go);go.AddComponent<PMMapChecks>();return "Started";}
 void Check(string label,bool pass){report+="- "+label+": "+(pass?"PASS":"FAIL")+"\n";File.WriteAllText("Docs/Implementacion/PM/Pruebas_Mapa.md",report);}
 void Keys(params Key[] keys)=>InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(keys));
 IEnumerator Tap(Key k){Keys(k);yield return new WaitForSecondsRealtime(.09f);Keys();yield return new WaitForSecondsRealtime(.14f);}
 IEnumerator Settle(){yield return null;yield return null;yield return new WaitForSecondsRealtime(.5f);while(M01Expedition.Transitioning)yield return null;}
 IEnumerator Dialogue(){int i=0;while(D.IsOpen&&i++<80)yield return Tap(Key.E);Check("Dialogo termina",!D.IsOpen);}
 IEnumerator Walk(float x,float z)
 {
  float deadline=Time.realtimeSinceStartup+15;
  while(Time.realtimeSinceStartup<deadline){var p=P.transform.position;var delta=new Vector2(x-p.x,z-p.z);if(delta.magnitude<.22f)break;Keys(Mathf.Abs(delta.x)>.16f?(delta.x>0?Key.RightArrow:Key.LeftArrow):Key.None,Mathf.Abs(delta.y)>.16f?(delta.y>0?Key.UpArrow:Key.DownArrow):Key.None);yield return null;}
  Keys();yield return new WaitForSecondsRealtime(.15f);
 }
 IEnumerator Capture(string name){yield return new WaitForSecondsRealtime(1.15f);yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot("Docs/Implementacion/PM/"+name+".png");yield return null;}
 IEnumerator Start()
 {
  bg=InputSystem.settings.backgroundBehavior;focus=InputSystem.settings.editorInputBehaviorInPlayMode;InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;InputSystem.EnableDevice(Keyboard.current);
  yield return Settle();var data=M01Session.Instance.data;
  string before=File.Exists(M01Save.Path)?File.ReadAllText(M01Save.Path):null;
  Check("Acceso tras prologo sin adelantar N1A ni telefono",SceneManager.GetActiveScene().name=="M01_ACCESO"&&data.events.Contains("P02.CAPTAIN")&&!data.events.Contains("N1A.INTRO")&&!data.phoneOwned);
  yield return Walk(5.7f,-1.6f);var pos=P.transform.position;int health=P.Health,money=data.money,attempt=data.expeditionAttempt;var cam=FindAnyObjectByType<AfterglowCamera>();var offset=cam.offset;
  yield return Tap(Key.E);Check("E abre mapa y pausa simulacion",Map.IsOpen&&Time.timeScale==0&&D.IsOpen);Check("N1 inicial, cinco cerrados",Map.Focused==0&&M01RouteMap.Available(data,0)&&!M01RouteMap.Available(data,1));yield return Capture("PM_Mapa_Inicial");
  yield return Tap(Key.RightArrow);Check("Flecha enfoca vecino cerrado sin viaje",Map.Focused==1&&SceneManager.GetActiveScene().name=="M01_ACCESO");yield return Tap(Key.E);Check("E no abre nodo cerrado",Map.IsOpen&&data.expeditionAttempt==attempt);yield return Capture("PM_Mapa_Cerrado");
  yield return Tap(Key.Escape);Check("Esc restaura posicion vida y foco libre",!Map.IsOpen&&!D.IsOpen&&Time.timeScale==1&&Vector3.Distance(P.transform.position,pos)<.03f&&P.Health==health&&data.money==money);
  yield return Tap(Key.E);Check("Recuerda ultimo nodo enfocado",Map.IsOpen&&Map.Focused==1);yield return Tap(Key.LeftArrow);yield return Tap(Key.E);Keys(Key.E);yield return new WaitForSecondsRealtime(.7f);Keys();yield return Settle();
  Check("Confirma N1 una sola expedicion en entrada compartida",SceneManager.GetActiveScene().name=="M01_ACCESO"&&data.expeditionAttempt==attempt+1&&data.expeditionActive&&data.events.Contains("M01.N01_ENTERED"));Check("N1A empieza al iniciar visita guiada",D.IsOpen&&D.CurrentId.StartsWith("N1A."));yield return Dialogue();
  yield return Walk(12,0);yield return Settle();Check("Entrega telefono en N1 tras presentacion",D.IsOpen&&D.CurrentId.StartsWith("N1C."));yield return Dialogue();Check("Telefono recibido, pico aun no",data.phoneOwned&&!data.pickaxeOwned);yield return Capture("PM_Entrada_N1");
  float deadline=Time.realtimeSinceStartup+13;while(SceneManager.GetActiveScene().name=="M01_ACCESO"&&Time.realtimeSinceStartup<deadline){Keys(Key.RightArrow);yield return null;}Keys();yield return Settle();Check("Puerta fisica lleva a galeria, misma expedicion",SceneManager.GetActiveScene().name=="N01S02"&&data.expeditionAttempt==attempt+1);yield return Capture("PM_N1_Galeria");
  yield return Tap(Key.Escape);Check("Pausa no ofrece mapa en galeria",D.IsOpen&&!D.body.text.Contains("mapa"));yield return Dialogue();
  // Walk back through the real initial portal, then enter again from the shared board.
  deadline=Time.realtimeSinceStartup+8;while(SceneManager.GetActiveScene().name=="N01S02"&&Time.realtimeSinceStartup<deadline){Keys(Key.LeftArrow);yield return null;}Keys();yield return Settle();Check("Retorno andando al acceso termina expedicion",SceneManager.GetActiveScene().name=="M01_ACCESO"&&!data.expeditionActive);
  yield return Walk(5.7f,-1.6f);yield return Tap(Key.E);yield return Tap(Key.E);yield return Settle();Check("Reentrada voluntaria nueva expedicion sin repetir N1A",data.expeditionAttempt==attempt+2&&!D.IsOpen);
  deadline=Time.realtimeSinceStartup+13;while(SceneManager.GetActiveScene().name=="M01_ACCESO"&&Time.realtimeSinceStartup<deadline){Keys(Key.RightArrow);yield return null;}Keys();yield return Settle();
  yield return Tap(Key.T);Check("Telefono confirma regreso sin mapa",D.IsOpen&&SceneManager.GetActiveScene().name=="N01S02"&&!FindAnyObjectByType<M01RouteMap>());yield return Tap(Key.E);yield return Tap(Key.E);yield return Settle();Check("Telefono vuelve al acceso conservando recursos",SceneManager.GetActiveScene().name=="M01_ACCESO"&&data.money==money&&!data.expeditionActive);
  Check("Una sola fuente de altar",FindObjectsByType<M01Altar>().Length==1);Check("Camara exploracion conserva offset",FindAnyObjectByType<AfterglowCamera>().offset==offset);Check("Mapa y viajes no escriben guardado",before==(File.Exists(M01Save.Path)?File.ReadAllText(M01Save.Path):null));
  yield return Walk(5.7f,-1.6f);data.worldClosed=true;yield return Tap(Key.E);Check("Mundo cerrado no permite selector ni replay",!Map.IsOpen&&!M01RouteMap.Available(data,0));data.worldClosed=false;
  report+="\nDONE\n";File.WriteAllText("Docs/Implementacion/PM/Pruebas_Mapa.md",report);Destroy(gameObject);
 }
 void OnDestroy(){Keys();InputSystem.settings.backgroundBehavior=bg;InputSystem.settings.editorInputBehaviorInPlayMode=focus;}
}
