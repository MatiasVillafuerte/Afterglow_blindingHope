using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Afterglow;
using Afterglow.M01;
public sealed class PMBoundaryAndAltarChecks:MonoBehaviour
{
 string report="# PM17/19: límites, altar e inputs restringidos\n\nFixtures aislados: se recoloca el actor junto a cada límite, sin desbloqueos de producción.\n\n";
 AfterglowPlayer P=>FindAnyObjectByType<AfterglowPlayer>();M01Dialogue D=>FindAnyObjectByType<M01Dialogue>();
 InputSettings.BackgroundBehavior bg;InputSettings.EditorInputBehaviorInPlayMode focus;
 public static string Main(){var go=new GameObject("PM boundary fixture");DontDestroyOnLoad(go);go.AddComponent<PMBoundaryAndAltarChecks>();return "Started";}
 void Check(string label,bool pass){report+="- "+label+": "+(pass?"PASS":"FAIL")+"\n";File.WriteAllText("Docs/Implementacion/PM/Pruebas_Limites_Altar.md",report);}
 void Keys(params Key[] k)=>InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(k));
 IEnumerator Tap(Key k){Keys(k);yield return new WaitForSecondsRealtime(.1f);Keys();yield return new WaitForSecondsRealtime(.12f);}
 IEnumerator Settle(){yield return null;yield return null;yield return new WaitForSecondsRealtime(.6f);while(M01Expedition.Transitioning)yield return null;}
 void Place(Vector3 v){var cc=P.GetComponent<CharacterController>();cc.enabled=false;P.transform.position=v;cc.enabled=true;}
 IEnumerator Start()
 {
  bg=InputSystem.settings.backgroundBehavior;focus=InputSystem.settings.editorInputBehaviorInPlayMode;InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;InputSystem.EnableDevice(Keyboard.current);
  var data=new M01Progress();data.events.AddRange(new[]{"P01.LETTER","P01.BELL","P02.CAPTAIN","M01.WORLD_STARTED","N1A.INTRO","N1C.PHONE"});M01Session.Instance.data=data;SceneManager.LoadScene(M01Expedition.ScenePath("PRO_PATIO"));yield return Settle();
  Vector3[] starts={new Vector3(28,.05f,-4),new Vector3(26,.05f,5),new Vector3(3,.05f,0),new Vector3(39,.05f,0),new Vector3(18.8f,.05f,.5f)};Key[] dirs={Key.DownArrow,Key.UpArrow,Key.LeftArrow,Key.RightArrow,Key.RightArrow};
  for(int i=0;i<starts.Length;i++){Place(starts[i]);yield return new WaitForSecondsRealtime(.2f);Keys(dirs[i],Key.Space);yield return new WaitForSecondsRealtime(1.8f);Keys();yield return new WaitForSecondsRealtime(.25f);var p=P.transform.position;bool safe=i==0?p.z>-5:i==1?p.z<7:i==2?p.x>2:i==3?p.x<40:p.x<19.9f;Check("Salto contra limite "+i,safe&&!P.Dead);}
  SceneManager.LoadScene(M01Expedition.ScenePath("M01_ACCESO"));yield return Settle();data.phoneOwned=true;Place(new Vector3(20,.05f,0));yield return new WaitForSecondsRealtime(.2f);
  data.escortActive=true;yield return Tap(Key.T);Check("Escolta bloquea llamada y mapa",D.IsOpen&&!FindAnyObjectByType<M01RouteMap>().Open());int guard=0;while(D.IsOpen&&guard++<15)yield return Tap(Key.E);data.escortActive=false;
  P.GetComponent<M01PlayerRules>().restrictedActivity=true;yield return Tap(Key.T);Check("Actividad restringida bloquea llamada",D.IsOpen);guard=0;while(D.IsOpen&&guard++<15)yield return Tap(Key.E);P.GetComponent<M01PlayerRules>().restrictedActivity=false;
  Place(new Vector3(20,3,0));yield return Tap(Key.T);Check("En el aire no llama",!D.IsOpen);yield return new WaitForSecondsRealtime(1);
  data.investigated[0]=true;data.contributions[0]=3;float deadline=Time.realtimeSinceStartup+15;while(!data.opened[0]&&Time.realtimeSinceStartup<deadline)yield return null;Check("Altar abre con ambas condiciones y anuncia",data.opened[0]&&D.IsOpen&&D.CurrentId=="ALT.11");guard=0;while(D.IsOpen&&guard++<15)yield return Tap(Key.E);yield return new WaitForSecondsRealtime(2.5f);Check("Anuncio no se repite",!D.IsOpen&&!data.TryOpen(0));
  var map=FindAnyObjectByType<M01RouteMap>();data.lastMapNode=2;map.Open();yield return null;Check("Nodo abierto ausente informa produccion pendiente",map.Focused==1&&map.details.text.Contains("pendiente de producción"));int attempt=data.expeditionAttempt;yield return Tap(Key.E);Check("Ausencia de N2 no carga escena ficticia",map.IsOpen&&data.expeditionAttempt==attempt&&SceneManager.GetActiveScene().name=="M01_ACCESO");yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot("Docs/Implementacion/PM/PM_Mapa_N2_Pendiente.png");map.Close();
  report+="\nDONE\n";File.WriteAllText("Docs/Implementacion/PM/Pruebas_Limites_Altar.md",report);Destroy(gameObject);
 }
 void OnDestroy(){Keys();InputSystem.settings.backgroundBehavior=bg;InputSystem.settings.editorInputBehaviorInPlayMode=focus;}
}
