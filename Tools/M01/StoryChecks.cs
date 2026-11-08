using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using Afterglow;
using Afterglow.M01;
public class M01StoryChecks:MonoBehaviour
{
 string report="# Prólogo y transición de salas\n";
 public static string Main(){var o=new GameObject("M01 Story checks");DontDestroyOnLoad(o);o.AddComponent<M01StoryChecks>();return "Started";}
 void Check(string name,bool pass){report+="- "+name+": "+pass+"\n";File.WriteAllText("Docs/Implementacion/Pruebas_Prologo_y_Salas.md",report);}
 IEnumerator Key(Key key){InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(key));yield return new WaitForSecondsRealtime(.08f);InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());yield return new WaitForSecondsRealtime(.08f);}
 IEnumerator Ready(){yield return null;while(M01Expedition.Transitioning)yield return null;yield return null;}
 IEnumerator Talk(){var d=FindAnyObjectByType<M01Dialogue>();for(int i=0;i<90&&d&&d.IsOpen;i++)yield return Key(UnityEngine.InputSystem.Key.E);}
 IEnumerator Walk(float x){string room=SceneManager.GetActiveScene().name;float end=Time.realtimeSinceStartup+16;while(Time.realtimeSinceStartup<end){if(SceneManager.GetActiveScene().name!=room)yield break;var p=FindAnyObjectByType<AfterglowPlayer>();var d=FindAnyObjectByType<M01Dialogue>();if(d&&d.IsOpen){if(p)p.testMove=Vector2.zero;yield return Talk();continue;}if(!p)yield break;if(Mathf.Abs(p.transform.position.x-x)<.15f){p.testMove=Vector2.zero;yield break;}p.testControl=true;p.testMove=new Vector2(Mathf.Sign(x-p.transform.position.x),0);yield return null;}Check("Caminar hasta "+x,false);}
 IEnumerator Start()
 {
  M01Session.Instance.data=new M01Progress();SceneManager.LoadScene(M01Expedition.ScenePath("PRO_CASA"));yield return new WaitForSecondsRealtime(.4f);yield return Ready();
  var d=FindAnyObjectByType<M01Dialogue>();var p=FindAnyObjectByType<AfterglowPlayer>();var initial=p.transform.position;Check("Carta literal y control bloqueado",d.IsOpen&&d.CurrentId=="P01.02"&&Time.timeScale==0&&!p.GetComponent<M01PlayerRules>().hasPickaxe);yield return Key(UnityEngine.InputSystem.Key.E);yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot("Docs/Implementacion/Prologo_Carta.png");yield return Talk();Check("Carta completa y timbre único",M01Session.Instance.data.events.Contains("P01.LETTER")&&FindObjectsByType<AudioSource>().Length==1&&Vector3.Distance(p.transform.position,initial)<.05f);
  p.testControl=true;p.testMove=Vector2.right;float end=Time.realtimeSinceStartup+8;while(SceneManager.GetActiveScene().name=="PRO_CASA"&&Time.realtimeSinceStartup<end)yield return null;yield return new WaitForSecondsRealtime(.3f);Check("Puerta caminando casa a patio",SceneManager.GetActiveScene().name=="PRO_PATIO");
  yield return Walk(9.5f);yield return Key(UnityEngine.InputSystem.Key.E);yield return Talk();yield return new WaitForSecondsRealtime(.4f);yield return Ready();Check("Capitán lleva al acceso",SceneManager.GetActiveScene().name=="M01_ACCESO"&&M01Session.Instance.data.events.Contains("P02.CAPTAIN"));yield return Talk();
  yield return Walk(12);yield return Talk();Check("Teléfono una vez y pico aún no entregado",M01Session.Instance.data.phoneOwned&&!M01Session.Instance.data.pickaxeOwned);
  yield return Walk(40);yield return new WaitForSecondsRealtime(.4f);yield return Ready();Check("Puerta física inicia expedición",SceneManager.GetActiveScene().name=="N01S02"&&M01Session.Instance.data.expeditionActive);
  yield return Walk(40);yield return new WaitForSecondsRealtime(.4f);yield return Ready();yield return Walk(10);yield return Talk();Check("Pico antes de agresión",SceneManager.GetActiveScene().name=="N01S03"&&M01Session.Instance.data.pickaxeOwned&&FindAnyObjectByType<M01PlayerRules>().hasPickaxe);
  // Focused state regression: alter one hit through the production damage path, cross rooms and return.
  var enemy=System.Array.Find(FindObjectsByType<AfterglowEnemy>(),e=>e.name=="N01.TUTORIAL.RESISTANCE");enemy.enabled=false;enemy.TakeDamage(1);var position=enemy.transform.position;FindAnyObjectByType<M01Expedition>().Capture();int attempt=M01Session.Instance.data.expeditionAttempt;
  M01Expedition.Travel("N01S04",new Vector3(4,.05f,0));yield return new WaitForSecondsRealtime(.4f);yield return Ready();M01Expedition.Travel("N01S03",new Vector3(4,.05f,0));yield return new WaitForSecondsRealtime(.4f);yield return Ready();enemy=System.Array.Find(FindObjectsByType<AfterglowEnemy>(),e=>e.name=="N01.TUTORIAL.RESISTANCE");Check("Regreso conserva vida y posición enemiga",enemy.Health==2&&Vector3.Distance(enemy.transform.position,position)<.2f);Check("Cruzar salas no renueva expedición",M01Session.Instance.data.expeditionAttempt==attempt);
  enemy.enabled=false;enemy.TakeDamage(2);yield return new WaitForSecondsRealtime(.1f);yield return Talk();M01Expedition.Travel("N01S04",new Vector3(4,.05f,0));yield return new WaitForSecondsRealtime(.4f);yield return Ready();M01Expedition.Travel("N01S03",new Vector3(4,.05f,0));yield return new WaitForSecondsRealtime(.4f);yield return Ready();Check("Enemigo derrotado sigue derrotado",System.Array.Find(FindObjectsByType<AfterglowEnemy>(),e=>e.name=="N01.TUTORIAL.RESISTANCE").Defeated);
  M01Expedition.Travel("M01_ACCESO",new Vector3(4,.05f,0),true);yield return new WaitForSecondsRealtime(.4f);yield return Ready();M01Expedition.Travel("N01S03",new Vector3(4,.05f,0),false,true);yield return new WaitForSecondsRealtime(.4f);yield return Ready();Check("Expedición voluntaria renueva enemigos",System.Array.Find(FindObjectsByType<AfterglowEnemy>(),e=>e.name=="N01.TUTORIAL.RESISTANCE").Health==3&&M01Session.Instance.data.expeditionAttempt==attempt+1);
  Destroy(gameObject);
 }
}





