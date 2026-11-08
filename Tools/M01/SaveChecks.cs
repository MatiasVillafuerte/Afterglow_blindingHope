using System.IO;
using System.Collections;
using UnityEngine;
using Afterglow;
using Afterglow.M01;
public class M01SaveChecks:MonoBehaviour
{
 string report="# Guardado manual y estado de expedición\n";
 string file;
 public static string Main(){var o=new GameObject("M01 Save checks");DontDestroyOnLoad(o);o.AddComponent<M01SaveChecks>();return "Started";}
 void Check(string label,bool result){report+="- "+label+": "+result+"\n";File.WriteAllText("Docs/Implementacion/Pruebas_Guardado_M01.md",report);}
 IEnumerator Ready(){yield return null;while(M01Expedition.Transitioning)yield return null;yield return null;yield return new WaitForSeconds(.15f);}
 IEnumerator Start()
 {
  file=System.IO.Path.GetFullPath("Tools/M01/TestData/save.json");var data=M01Session.Instance.data;data.events.Add("N1B.PICKAXE");data.events.Add("N1B.LOOT");data.pickaxeOwned=true;data.Active.health=7;data.money=42;data.preparation="Velocidad";data.preparationRemaining=73;
  M01Expedition.Travel("N01S03",new Vector3(4,.05f,0));yield return Ready();
  var player=FindAnyObjectByType<AfterglowPlayer>();var enemy=System.Array.Find(FindObjectsByType<AfterglowEnemy>(),e=>e.name=="N01.TUTORIAL.RESISTANCE");foreach(var e in FindObjectsByType<AfterglowEnemy>())e.enabled=false;enemy.TakeDamage(1);data.CollectOffering("EXP.N01S03.0","Coca",false);
  var orb=GameObject.CreatePrimitive(PrimitiveType.Sphere);Destroy(orb.GetComponent<Collider>());orb.transform.position=new Vector3(12,.4f,-2);var loot=orb.AddComponent<M01Loot>();loot.kind=M01LootKind.Money;loot.amount=7;loot.remaining=4;
  Check("Guardado solo bajo orden manual",M01Save.TryWrite(out var error,file));M01Save.ReadFile(file,out var saved,out error);float remaining=saved.progress.preparationRemaining;Check("Incluye salud, enemigos y botín",saved.progress.Active.health==7&&saved.progress.rooms.Exists(r=>r.id=="N01S03"&&r.enemies.Exists(e=>e.id==enemy.name&&e.health==2)&&r.loot.Exists(l=>l.amount==7)));
  string original=File.ReadAllText(file);data.money=999;yield return new WaitForSeconds(.1f);Check("Cambiar sesión no sobrescribe archivo",File.ReadAllText(file)==original);
  Check("Continuar acepta partida válida",M01Save.Continue(out error,file));yield return Ready();data=M01Session.Instance.data;player=FindAnyObjectByType<AfterglowPlayer>();enemy=System.Array.Find(FindObjectsByType<AfterglowEnemy>(),e=>e.name=="N01.TUTORIAL.RESISTANCE");Check("Carga conserva vida, dinero y enemigo",player.Health==7&&data.money==42&&enemy.Health==2);Check("Carga conserva recogida estática",data.collectedStaticIds.Contains("EXP.N01S03.0")&&!FindObjectsByType<M01StaticOffering>().AnyActive("EXP.N01S03.0"));Check("Preparación no descuenta tiempo cerrado",remaining-data.preparationRemaining<1);Check("Carga conserva botín suelto",System.Array.Exists(FindObjectsByType<M01Loot>(),l=>l.amount==7&&l.remaining>2));
  foreach(var e in FindObjectsByType<AfterglowEnemy>())e.enabled=false;
  FindAnyObjectByType<M01PlayerRules>().restrictedActivity=true;Check("Contexto restringido bloquea escritura",!M01Save.TryWrite(out error,file)&&File.ReadAllText(file)==original);FindAnyObjectByType<M01PlayerRules>().restrictedActivity=false;
  var attrs=File.GetAttributes(file);bool failed=false;try{File.SetAttributes(file,attrs|FileAttributes.ReadOnly);failed=!M01Save.TryWrite(out error,file);}finally{File.SetAttributes(file,attrs);}Check("Fallo de escritura conserva último válido",failed&&File.ReadAllText(file)==original);
  File.WriteAllText(file+".invalid","{broken");Check("Carga inválida no reemplaza sesión",!M01Save.Continue(out error,file+".invalid")&&M01Session.Instance.data.money==42);
  Destroy(gameObject);
 }
}
public static class M01OfferingCheckExtensions
{
 public static bool AnyActive(this M01StaticOffering[] items,string id){foreach(var item in items)if(item.sourceId==id&&item.gameObject.activeInHierarchy)return true;return false;}
}
