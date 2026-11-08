using System.IO;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using Afterglow;
using Afterglow.M01;
public class M01ServiceChecks:MonoBehaviour
{
 string report="# Servicios, consumibles y encargos iniciales\n";
 M01Dialogue D=>FindAnyObjectByType<M01Dialogue>();
 M01Progress Data=>M01Session.Instance.data;
 public static string Main(){var o=new GameObject("M01 Service checks");DontDestroyOnLoad(o);o.AddComponent<M01ServiceChecks>();return "Started";}
 void Check(string label,bool result){report+="- "+label+": "+result+"\n";File.WriteAllText("Docs/Implementacion/Pruebas_Servicios_M01.md",report);}
 IEnumerator Key(Key key){InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(key));yield return new WaitForSecondsRealtime(.08f);InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());yield return new WaitForSecondsRealtime(.08f);}
 bool Choices=>D.choiceLabels.Any(t=>t.transform.parent.gameObject.activeSelf);
 IEnumerator FinishText(){for(int i=0;i<90&&D.IsOpen&&!Choices;i++)yield return Key(UnityEngine.InputSystem.Key.E);}
 IEnumerator Choose(int index){yield return Key(UnityEngine.InputSystem.Key.E);for(int i=0;i<index;i++)yield return Key(UnityEngine.InputSystem.Key.DownArrow);yield return Key(UnityEngine.InputSystem.Key.E);}
 void Place(float x,float z=0){var p=FindAnyObjectByType<AfterglowPlayer>();var c=p.GetComponent<CharacterController>();c.enabled=false;p.transform.position=new Vector3(x,.05f,z);c.enabled=true;p.testControl=true;p.testMove=Vector2.zero;FindAnyObjectByType<AfterglowCamera>().Snap();}
 IEnumerator Start()
 {
  yield return FinishText();Data.money=100;Data.offerings.Clear();for(int i=0;i<6;i++)Data.offerings.Add(new M01Offering{kind="Coca",sourceId="CHECK.COCA."+i});
  SceneManager.LoadScene(M01Expedition.ScenePath("M01_PUEBLO"));yield return new WaitForSecondsRealtime(.4f);
  Place(9);yield return Key(UnityEngine.InputSystem.Key.E);yield return FinishText();yield return Choose(0);yield return FinishText();Check("Tienda cobra cinco y añade fragmento",Data.money==95&&Data.fragments==1);
  Place(29);yield return Key(UnityEngine.InputSystem.Key.E);yield return FinishText();yield return Choose(0);yield return FinishText();Check("Vendedor vende solo alcohol/cigarro",Data.money==87&&Data.offerings.Count(o=>o.stolen)==3&&Data.offerings.Where(o=>o.stolen).All(o=>o.kind=="Alcohol"||o.kind=="Cigarro"));
  Place(19);yield return Key(UnityEngine.InputSystem.Key.E);yield return FinishText();yield return Choose(0);yield return FinishText();Check("Pista gasta la coca del altar",Data.offerings.Count(o=>o.kind=="Coca")==5);yield return Key(UnityEngine.InputSystem.Key.E);yield return FinishText();yield return Choose(0);yield return FinishText();Check("Pista repetida no cobra",Data.offerings.Count(o=>o.kind=="Coca")==5);
  yield return Key(UnityEngine.InputSystem.Key.E);yield return FinishText();yield return Choose(1);yield return Choose(0);float remaining=Data.preparationRemaining;yield return new WaitForSecondsRealtime(.5f);Check("Preparación cuesta dos hojas y pausa duración",Data.offerings.Count(o=>o.kind=="Coca")==3&&Mathf.Abs(Data.preparationRemaining-remaining)<.02f);yield return FinishText();yield return new WaitForSeconds(.2f);Check("Preparación afecta velocidad",FindAnyObjectByType<AfterglowPlayer>().speed>5.4f&&Data.preparationRemaining<remaining);
  Data.Active.health=7;FindAnyObjectByType<AfterglowPlayer>().SynchronizeWorldOneState();yield return Key(UnityEngine.InputSystem.Key.I);yield return Choose(1);yield return Choose(0);yield return FinishText();Check("Fragmento cura dos sin superar máximo",Data.Active.health==9&&Data.fragments==0);
  Data.events.Add("N1A.INTRO");Data.events.Add("N1C.PHONE");SceneManager.LoadScene(M01Expedition.ScenePath("M01_ACCESO"));yield return new WaitForSecondsRealtime(.4f);Place(23);yield return Key(UnityEngine.InputSystem.Key.E);yield return FinishText();yield return Choose(0);Check("Aceptar Q01 no cuenta bajas anteriores",Data.Quest("Q01").active&&Data.Quest("Q01").count==0);
  for(int i=0;i<3;i++)M01QuestNPC.RecordKill("N01.CHECK."+i);int money=Data.money;yield return Key(UnityEngine.InputSystem.Key.E);yield return FinishText();Check("Q01 entrega una vez",Data.Quest("Q01").paid&&Data.money==money+5);yield return Key(UnityEngine.InputSystem.Key.E);yield return FinishText();Check("Q01 no paga otra vez",Data.money==money+5);
  Place(29);yield return Key(UnityEngine.InputSystem.Key.E);yield return FinishText();yield return Choose(0);SceneManager.LoadScene(M01Expedition.ScenePath("N01S04"));yield return new WaitForSecondsRealtime(.4f);Place(13,3);yield return new WaitForSecondsRealtime(.2f);Check("Casco recogible persistente",Data.questItems.Contains("Q02.CASCO"));SceneManager.LoadScene(M01Expedition.ScenePath("M01_ACCESO"));yield return new WaitForSecondsRealtime(.4f);Place(29);money=Data.money;yield return Key(UnityEngine.InputSystem.Key.E);yield return FinishText();Check("Q02 devuelve casco y recompensa única",Data.Quest("Q02").paid&&!Data.questItems.Contains("Q02.CASCO")&&Data.money==money+8);
  Destroy(gameObject);
 }
}
