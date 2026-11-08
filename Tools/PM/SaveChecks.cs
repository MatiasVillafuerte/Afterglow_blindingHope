using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Afterglow;
using Afterglow.M01;
public sealed class PMSaveChecks:MonoBehaviour
{
 string report="# PM15/18: guardado y migración en escenas reales\n\nFixtures bajo Tools/PM/TestData, sin sobrescribir guardado personal.\n\n";
 const string FileName="Tools/PM/TestData/manual.json";
 public static string Main(){var go=new GameObject("PM save fixture");DontDestroyOnLoad(go);go.AddComponent<PMSaveChecks>();return "Started";}
 void Check(string label,bool pass){report+="- "+label+": "+(pass?"PASS":"FAIL")+"\n";File.WriteAllText("Docs/Implementacion/PM/Pruebas_Guardado.md",report);}
 IEnumerator Settle(){yield return null;yield return null;yield return new WaitForSecondsRealtime(.8f);while(M01Expedition.Transitioning)yield return null;}
 IEnumerator Start()
 {
  Directory.CreateDirectory("Tools/PM/TestData");var personal=File.Exists(M01Save.Path)?File.ReadAllText(M01Save.Path):null;
  string[] scenes={"PRO_CASA","PRO_PATIO","M01_ACCESO","M01_ACCESO","N01S02"};
  for(int i=0;i<scenes.Length;i++)
  {
   var d=new M01Progress();d.events.AddRange(new[]{"P01.LETTER","P01.BELL"});d.money=37;d.Active.health=7;d.lastMapNode=i>=3?2:1;
   if(i>=2)d.events.AddRange(new[]{"P02.CAPTAIN","M01.WORLD_STARTED"});if(i>=3){d.investigated[0]=true;d.contributions[0]=3;d.opened[0]=true;}if(i==4){d.phoneOwned=true;d.pickaxeOwned=true;d.expeditionActive=true;d.expeditionAttempt=4;d.events.AddRange(new[]{"N1A.INTRO","N1C.PHONE","M01.N01_ENTERED"});}
   M01Session.Instance.data=d;SceneManager.LoadScene(M01Expedition.ScenePath(scenes[i]));yield return Settle();var player=FindAnyObjectByType<AfterglowPlayer>();
   var cc=player.GetComponent<CharacterController>();cc.enabled=false;player.transform.position=new Vector3(scenes[i]=="PRO_PATIO"?28:6,.05f,0);cc.enabled=true;player.SynchronizeWorldOneState();yield return new WaitForSecondsRealtime(.3f);var savedPosition=player.transform.position;
   bool written=M01Save.TryWrite(out string error,FileName);Check("Guardado manual "+scenes[i]+" caso "+i,written);if(!written){report+=error+"\n";continue;}
   d.money=0;d.Active.health=1;bool loaded=M01Save.Continue(out error,FileName);yield return Settle();d=M01Session.Instance.data;player=FindAnyObjectByType<AfterglowPlayer>();
   Check("Carga conserva seccion/vida/recursos caso "+i,loaded&&SceneManager.GetActiveScene().name==scenes[i]&&Vector3.Distance(player.transform.position,savedPosition)<.05f&&player.Health==7&&d.money==37);
   Check("No repite carta/timbre/encargo caso "+i,!FindAnyObjectByType<M01Dialogue>().IsOpen&&d.events.FindAll(e=>e=="P01.BELL").Count==1&&(i<2?!d.events.Contains("P02.CAPTAIN"):d.events.Contains("P02.CAPTAIN")));
   if(i>=3)Check("Nodo abierto y foco persistente caso "+i,d.lastMapNode==2&&d.opened[0]&&d.contributions[0]==3&&d.investigated[0]);
   if(i==4)Check("Carga galeria no reinicia expedicion ni abre mapa",d.expeditionAttempt==4&&d.expeditionActive&&!FindAnyObjectByType<M01RouteMap>());
  }
  string prior=File.ReadAllText(FileName);var bad=new M01SaveFile{scene="missing",progress=new M01Progress()};Check("Fallo al guardar conserva ultimo fichero",!M01Save.WriteFile(bad,FileName,out _)&&File.ReadAllText(FileName)==prior);File.WriteAllText(FileName+".invalid","{invalid");Check("Carga invalida conserva sesion",!M01Save.Continue(out _,FileName+".invalid")&&M01Session.Instance.data.money==37&&File.ReadAllText(FileName)==prior);
  var old=new M01SaveFile{schema=1,scene=M01Expedition.ScenePath("N01S02"),position=new Vector3(6,.05f,0),progress=new M01Progress()};old.progress.phoneOwned=true;old.progress.money=71;old.progress.contributions[0]=2;string oldPath="Tools/PM/TestData/schema1.json";File.WriteAllText(oldPath,JsonUtility.ToJson(old));bool migrated=M01Save.ReadFile(oldPath,out var migratedSave,out _);Check("Schema1 infiere avance real y conserva recursos",migrated&&migratedSave.schema==2&&migratedSave.progress.events.Contains("P02.CAPTAIN")&&migratedSave.progress.events.Contains("P01.BELL")&&migratedSave.progress.money==71&&migratedSave.progress.contributions[0]==2&&!Array.Exists(migratedSave.progress.opened,b=>b));
  old.scene=M01Expedition.ScenePath("PRO_CASA");old.progress.phoneOwned=false;old.progress.contributions[0]=0;old.progress.events.Clear();File.WriteAllText(oldPath,JsonUtility.ToJson(old));M01Save.ReadFile(oldPath,out migratedSave,out _);Check("Schema1 ambiguo no inventa prologo ni aperturas",!migratedSave.progress.events.Contains("P02.CAPTAIN")&&!migratedSave.progress.events.Contains("P01.LETTER")&&!Array.Exists(migratedSave.progress.opened,b=>b));
  var active=M01Session.Instance.data;SceneManager.LoadScene(M01Expedition.ScenePath("M01_ACCESO"));yield return Settle();var map=FindAnyObjectByType<M01RouteMap>();var pos=FindAnyObjectByType<AfterglowPlayer>().transform.position;int health=active.Active.health;map.Open();Check("Guardado bloqueado en mapa modal",map.IsOpen&&!M01Save.Allowed&&!M01Save.TryWrite(out _,FileName));map.Close();yield return Settle();Check("Cerrar mapa no cura ni guarda",active.Active.health==health&&File.ReadAllText(FileName)==prior&&Vector3.Distance(pos,FindAnyObjectByType<AfterglowPlayer>().transform.position)<.05f);
  Check("Guardado personal intacto",personal==(File.Exists(M01Save.Path)?File.ReadAllText(M01Save.Path):null));report+="\nDONE\n";File.WriteAllText("Docs/Implementacion/PM/Pruebas_Guardado.md",report);Destroy(gameObject);
 }
}
