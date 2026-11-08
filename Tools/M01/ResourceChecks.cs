using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Afterglow;
using Afterglow.M01;
public sealed class M01ResourceChecks:MonoBehaviour
{
    string report="# Pruebas de recursos M01\n\n";
    public static string Main(){new GameObject("M01 resource checks").AddComponent<M01ResourceChecks>();return "Resource checks started";}
    void Check(string name,bool ok,string detail=""){report+="- "+name+": "+(ok?"PASS":"FAIL")+" "+detail+"\n";File.WriteAllText("Docs/Implementacion/Pruebas_Recursos_M01.md",report);}
    void Keys(params Key[] keys){InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(keys));}
    IEnumerator Press(Key key){Keys(key);yield return new WaitForSecondsRealtime(.08f);Keys();yield return new WaitForSecondsRealtime(.08f);}
    void Warp(AfterglowPlayer p,Vector3 pos){var c=p.GetComponent<CharacterController>();c.enabled=false;p.transform.position=pos;c.enabled=true;}
    M01Progress Example(int credit,int good,int stolen){var data=new M01Progress();data.contributions[1]=credit;for(int i=0;i<good;i++)data.CollectOffering("G"+i,"Coca",false);for(int i=0;i<stolen;i++)data.CollectOffering("S"+i,"Alcohol",true);return data;}
    IEnumerator Start()
    {
        var math=new M01Progress();math.experience=99;math.Active.health=2;math.AddExperience(2);
        Check("C11 99+2",math.teamLevel==2&&math.experience==1&&math.Active.gems==3&&math.historicGems==3&&math.Active.health==10);
        math.Active.gems=0;math.UnlockSon();math.members[1].health=7;math.AddExperience(250);
        Check("C11 multiple thresholds and historical son grant",math.teamLevel==4&&math.experience==51&&math.historicGems==9&&math.members[1].gems==9&&math.members[1].health==7&&math.Active.gems==6);
        var a=Example(0,6,2);var ar=a.Offer(1);Check("C14 sufficient legitimate preserves stolen",a.contributions[1]==5&&a.offerings.Count(o=>!o.stolen)==1&&a.offerings.Count(o=>o.stolen)==2&&ar.rejected==0&&!a.fraudDiscovered);
        var b=Example(1,2,1);var br=b.Offer(1);Check("C14 partial and rejection",b.contributions[1]==3&&b.offerings.Count==0&&br.rejected==1&&b.Active.health==10&&b.fraudDiscovered);
        var c=Example(3,2,1);c.Offer(1);Check("C14 remaining credit preserves stolen",c.contributions[1]==5&&c.offerings.Count==1&&c.offerings[0].stolen);
        Check("C15 quota alone cannot open",!c.TryOpen(1));c.investigated[1]=true;Check("C15 opens once after investigation",c.TryOpen(1)&&!c.TryOpen(1));int remaining=c.offerings.Count;c.Offer(1);Check("C13 no second payment",c.offerings.Count==remaining);
        var inverse=Example(0,5,0);inverse.investigated[1]=true;inverse.Offer(1);Check("C15 reverse order",inverse.TryOpen(1));
        var p=FindAnyObjectByType<AfterglowPlayer>();var dialogue=FindAnyObjectByType<M01Dialogue>();var enemy=FindAnyObjectByType<AfterglowEnemy>();enemy.enabled=false;var data=M01Session.Instance.data;
        data.experience=99;data.Active.health=2;p.SynchronizeWorldOneState();var experienceObject=new GameObject("Threshold orb test");experienceObject.transform.position=p.transform.position+Vector3.up*.35f;var experienceLoot=experienceObject.AddComponent<M01Loot>();experienceLoot.kind=M01LootKind.Experience;experienceLoot.amount=2;yield return new WaitForSeconds(.2f);
        Check("C11 actual orb updates player and shared state",data.teamLevel==2&&data.experience==1&&data.Active.gems==3&&p.Health==10&&p.Experience==1);
        var moneyObject=new GameObject("Money test");moneyObject.transform.position=p.transform.position+Vector3.up*.35f;var moneyLoot=moneyObject.AddComponent<M01Loot>();moneyLoot.kind=M01LootKind.Money;moneyLoot.amount=2;yield return new WaitForSeconds(.2f);Check("C11 money remains separate",data.money==2&&data.experience==1&&data.Active.gems==3);
        yield return new WaitForSeconds(.2f);Warp(p,new Vector3(25,.05f,-1));yield return new WaitForSeconds(.2f);Check("C10 static pickup once",data.offerings.Count==1);Warp(p,new Vector3(26,.05f,-1));yield return new WaitForSeconds(.15f);Warp(p,new Vector3(25,.05f,-1));yield return new WaitForSeconds(.15f);Check("C10 static pickup not duplicated",data.offerings.Count==1);
        yield return Press(Key.I);Check("C10 descriptive key inventory",dialogue.IsOpen&&dialogue.body.text.Contains("Teléfono de minas")&&!dialogue.body.text.Contains("robada")&&!dialogue.body.text.Contains("Usar"));yield return Press(Key.E);yield return Press(Key.E);
        Warp(p,new Vector3(30,.05f,0));yield return new WaitForSeconds(.2f);yield return Press(Key.T);Check("C16 requires confirmation",dialogue.IsOpen&&p.transform.position.x>29);yield return Press(Key.E);yield return Press(Key.E);yield return new WaitForSeconds(.2f);Check("C16 returns without losing resources or investigating",p.transform.position.x<5&&data.offerings.Count==1&&!data.investigated[0]);
        data.escortActive=true;Warp(p,new Vector3(30,.05f,0));yield return new WaitForSeconds(.2f);yield return Press(Key.T);Check("C17 escort blocks phone",dialogue.CurrentId=="N1D.07"&&p.transform.position.x>29);yield return Press(Key.E);yield return Press(Key.E);data.escortActive=false;
        var altar=FindAnyObjectByType<M01Altar>();data.offerings.Clear();for(int i=0;i<6;i++)data.offerings.Add(new M01Offering{kind="Coca",sourceId="UI.G"+i});for(int i=0;i<2;i++)data.offerings.Add(new M01Offering{kind="Alcohol",stolen=true,sourceId="UI.S"+i});
        Warp(p,new Vector3(16,.05f,-1));yield return new WaitForSeconds(.2f);yield return Press(Key.E);Check("C13 altar preview before payment",dialogue.IsOpen&&data.contributions[0]==0&&!dialogue.body.text.Contains("robada"));yield return Press(Key.E);yield return Press(Key.E);Check("C13 confirmed payment in scene",data.contributions[0]==3&&data.offerings.Count==5&&!data.fraudDiscovered);yield return Press(Key.E);yield return Press(Key.E);
        data.investigated[0]=true;yield return new WaitForSecondsRealtime(2.3f);Check("C15 delayed unique announcement",data.opened[0]&&dialogue.IsOpen&&dialogue.CurrentId=="ALT.11");yield return Press(Key.E);yield return Press(Key.E);
        altar.quotaIndex=1;data.contributions[1]=1;data.offerings.Clear();data.offerings.Add(new M01Offering{kind="Coca"});data.offerings.Add(new M01Offering{kind="Coca"});data.offerings.Add(new M01Offering{kind="Alcohol",stolen=true});int health=p.Health;
        yield return Press(Key.E);yield return Press(Key.E);yield return Press(Key.E);yield return new WaitForSecondsRealtime(.8f);Check("C14 rejection presentation has no damage",data.contributions[1]==3&&data.offerings.Count==0&&p.Health==health&&data.fraudDiscovered);yield return Press(Key.E);yield return Press(Key.E);
        // Check expiry against the same game clock paused by dialogue, without collecting this object.
        var lootObject=new GameObject("Expiry check");lootObject.transform.position=new Vector3(40,1,4);var loot=lootObject.AddComponent<M01Loot>();loot.remaining=.6f;
        dialogue.Begin(new[]{"N1A.05"});float lifetime=loot.remaining;yield return new WaitForSecondsRealtime(.4f);Check("C12 loot lifetime pauses",loot&&Mathf.Abs(loot.remaining-lifetime)<.02f);yield return Press(Key.E);yield return Press(Key.E);yield return new WaitForSeconds(.8f);Check("C12 loot expires",!lootObject);
        Keys();Check("resource verification finished",true);Destroy(gameObject);
    }
}

