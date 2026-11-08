using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using Afterglow;
using Afterglow.M01;
public sealed class PMPrologueChecks:MonoBehaviour
{
 string report="# PM03–08: prólogo integrado en Play Mode\n\n";
 M01Dialogue D=>FindAnyObjectByType<M01Dialogue>();AfterglowPlayer P=>FindAnyObjectByType<AfterglowPlayer>();
 InputSettings.BackgroundBehavior bg;InputSettings.EditorInputBehaviorInPlayMode focus;
 public static string Main(){var go=new GameObject("PM prologue checks");DontDestroyOnLoad(go);go.AddComponent<PMPrologueChecks>();return "Started";}
 void Check(string label,bool pass){report+="- "+label+": "+(pass?"PASS":"FAIL")+"\n";File.WriteAllText("Docs/Implementacion/PM/Pruebas_Prologo.md",report);}
 void Keys(params Key[] keys)=>InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(keys));
 IEnumerator Tap(Key k){Keys(k);yield return new WaitForSecondsRealtime(.1f);Keys();yield return new WaitForSecondsRealtime(.12f);}
 IEnumerator Settle(){yield return null;yield return null;yield return new WaitForSecondsRealtime(.5f);while(M01Expedition.Transitioning)yield return null;}
 IEnumerator Dialogue(){int i=0;while(D.IsOpen&&i++<60)yield return Tap(Key.E);Check("Dialogo terminó sin saltar IDs",!D.IsOpen);}
 IEnumerator Walk(float x,float z)
 {
  float deadline=Time.realtimeSinceStartup+15;
  while(Time.realtimeSinceStartup<deadline){var p=P.transform.position;var delta=new Vector2(x-p.x,z-p.z);if(delta.magnitude<.22f)break;Keys(Mathf.Abs(delta.x)>.16f?(delta.x>0?Key.RightArrow:Key.LeftArrow):Key.None,Mathf.Abs(delta.y)>.16f?(delta.y>0?Key.UpArrow:Key.DownArrow):Key.None);yield return null;}
  Keys();yield return new WaitForSecondsRealtime(.15f);Check("Recorrido hasta "+x+","+z,Vector2.Distance(new Vector2(P.transform.position.x,P.transform.position.z),new Vector2(x,z))<.4f);
 }
 IEnumerator Capture(string name){yield return new WaitForSecondsRealtime(1.15f);yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot("Docs/Implementacion/PM/"+name+".png");yield return null;}
 IEnumerator Start()
 {
  bg=InputSystem.settings.backgroundBehavior;focus=InputSystem.settings.editorInputBehaviorInPlayMode;InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;InputSystem.EnableDevice(Keyboard.current);
  var before=File.Exists(M01Save.Path)?File.ReadAllText(M01Save.Path):null;int frame=Time.frameCount;
  yield return Settle();Check("Arranque normal en menu",SceneManager.GetActiveScene().name=="M01_MENU");
  bool valid=M01Save.ReadFile(M01Save.Path,out _,out _);yield return Tap(Key.E);if(valid)yield return Tap(Key.DownArrow);yield return Tap(Key.E);yield return Settle();
  Check("Partida nueva muestra P01 antes de control",SceneManager.GetActiveScene().name=="PRO_CASA"&&D.CurrentId=="P01.02"&&P.GetComponent<M01PlayerRules>().BlocksControl);
  var pos=P.transform.position;Keys(Key.RightArrow,Key.E);yield return new WaitForSecondsRealtime(.8f);Keys();yield return null;Check("Mantener E no omite carta y control bloqueado",D.IsOpen&&D.LineIndex<=1&&Vector3.Distance(P.transform.position,pos)<.01f);report+="  ID tras mantener: "+D.CurrentId+"; delta posicion: "+Vector3.Distance(P.transform.position,pos)+"\n";
  yield return Dialogue();yield return new WaitForSecondsRealtime(.9f);Check("Gesto y timbre unico",M01Session.Instance.data.events.FindAll(e=>e=="P01.BELL").Count==1&&!P.GetComponent<M01PlayerRules>().BlocksControl);
  Check("Prologo sin pico ni telefono",!M01Session.Instance.data.pickaxeOwned&&!M01Session.Instance.data.phoneOwned);yield return Capture("PM_Dormitorio");
  yield return Walk(12.5f,2.9f);yield return Tap(Key.E);yield return Settle();Check("Puerta E lleva directamente a jardin",SceneManager.GetActiveScene().name=="PRO_PATIO");yield return Capture("PM_Jardin_Casa");
  yield return Tap(Key.E);yield return Settle();Check("Regreso al dormitorio sin repetir carta",SceneManager.GetActiveScene().name=="PRO_CASA"&&!D.IsOpen&&M01Session.Instance.data.events.FindAll(e=>e=="P01.BELL").Count==1);
  yield return Walk(12.5f,2.9f);yield return Tap(Key.E);yield return Settle();
  yield return Walk(16,3.8f);yield return Walk(28,3.8f);yield return Walk(28,-2.8f);yield return Walk(16,-2.8f);yield return Walk(28,-2.8f);Check("Ambos lados de fuente transitables",SceneManager.GetActiveScene().name=="PRO_PATIO");yield return Capture("PM_Jardin_Fuente");
  yield return Walk(38.5f,.5f);yield return Capture("PM_Porton");
  Keys(Key.RightArrow,Key.Space);yield return new WaitForSecondsRealtime(1.3f);Keys();yield return new WaitForSecondsRealtime(.3f);Check("Porton no se atraviesa saltando",P.transform.position.x<40);
  yield return Tap(Key.E);Check("P02 accesible desde interior",D.IsOpen&&D.CurrentId=="P02.02");yield return Dialogue();yield return Settle();Check("Encargo unico y acceso seguro",SceneManager.GetActiveScene().name=="M01_ACCESO"&&M01Session.Instance.data.events.Contains("M01.WORLD_STARTED"));
  Check("Frames realmente avanzan",Time.frameCount>frame+60);Check("No se sobrescribio guardado real",before==(File.Exists(M01Save.Path)?File.ReadAllText(M01Save.Path):null));report+="\nDONE\n";File.WriteAllText("Docs/Implementacion/PM/Pruebas_Prologo.md",report);Destroy(gameObject);
 }
 void OnDestroy(){Keys();InputSystem.settings.backgroundBehavior=bg;InputSystem.settings.editorInputBehaviorInPlayMode=focus;}
}
