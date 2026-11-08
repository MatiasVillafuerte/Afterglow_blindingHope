using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Afterglow;
using Afterglow.M01;
public class M01DeathStateChecks:MonoBehaviour
{
 string report="# Muerte y renovación de expedición\n";
 public static string Main(){var o=new GameObject("M01 Death checks");DontDestroyOnLoad(o);o.AddComponent<M01DeathStateChecks>();return "Started";}
 void Check(string label,bool pass){report+="- "+label+": "+pass+"\n";File.WriteAllText("Docs/Implementacion/Pruebas_Muerte_Expedicion.md",report);}
 IEnumerator Ready(){yield return null;while(M01Expedition.Transitioning)yield return null;yield return null;}
 IEnumerator Start()
 {
  var data=M01Session.Instance.data;data.events.Add("N1A.INTRO");data.events.Add("N1C.PHONE");data.events.Add("N1B.LOOT");data.Quest("Q02").paid=true;data.Quest("Q01").count=2;data.Quest("Q01").active=true;
  int money=data.money,items=data.offerings.Count,attempt=data.expeditionAttempt;data.preparationRemaining=30;
  foreach(var e in FindObjectsByType<AfterglowEnemy>())e.enabled=false;
  yield return new WaitForSeconds(1.1f);FindAnyObjectByType<AfterglowPlayer>().TakeDamage(999);
  float deadline=Time.realtimeSinceStartup+12;while((SceneManager.GetActiveScene().name!="M01_ACCESO"||M01Expedition.Transitioning)&&Time.realtimeSinceStartup<deadline)yield return null;yield return Ready();
  var player=FindAnyObjectByType<AfterglowPlayer>();Check("Muerte vuelve a entrada y restaura vida",SceneManager.GetActiveScene().name=="M01_ACCESO"&&player.Health==data.Active.maxHealth);
  Check("Muerte conserva recursos, encargo y pago único",data.money==money&&data.offerings.Count==items&&data.Quest("Q02").paid&&data.Quest("Q01").count==2);Check("Muerte elimina preparación sin nueva expedición",data.preparationRemaining==0&&data.expeditionAttempt==attempt);
  M01Expedition.Travel("N01S03",new Vector3(4,.05f,0));yield return Ready();Check("Muerte repone enemigos",System.Array.TrueForAll(FindObjectsByType<AfterglowEnemy>(),e=>e.Health==3));Check("Muerte conserva estáticos tomados",data.collectedStaticIds.Contains("EXP.N01S03.0")&&!FindObjectsByType<M01StaticOffering>().AnyActive("EXP.N01S03.0"));
  M01Expedition.Travel("M01_ACCESO",new Vector3(4,.05f,0),true);yield return Ready();M01Expedition.Travel("N01S03",new Vector3(4,.05f,0),false,true);yield return Ready();Check("Nueva expedición renueva estáticos",FindObjectsByType<M01StaticOffering>().AnyActive("EXP.N01S03.0"));Check("Nueva expedición renueva intento sin renovar misión única",data.Quest("Q01").count==0&&!data.Quest("Q01").paid&&data.Quest("Q02").paid);
  Destroy(gameObject);
 }
}
public static class M01DeathOfferingCheck
{
 public static bool AnyActive(this M01StaticOffering[] items,string id){foreach(var item in items)if(item.sourceId==id&&item.gameObject.activeInHierarchy)return true;return false;}
}
