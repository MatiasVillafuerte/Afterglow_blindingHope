using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Afterglow;
using Afterglow.M01;
public sealed class M01FoundationChecks : MonoBehaviour
{
    AfterglowPlayer player; AfterglowEnemy enemy; M01Dialogue dialogue; M01PlayerRules rules;
    string report;
    public static string Main(){new GameObject("M01 foundation verification").AddComponent<M01FoundationChecks>();return "Foundation checks started";}
    void Check(string id,bool pass,string detail=""){report+="- "+id+": "+(pass?"PASS":"FAIL")+" "+detail+"\n";File.WriteAllText("Docs/Implementacion/Pruebas_Base_M01.md",report);}
    void Keys(params Key[] keys){InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(keys));}
    IEnumerator Press(Key key){Keys(key);yield return new WaitForSecondsRealtime(.08f);Keys();yield return new WaitForSecondsRealtime(.08f);}
    void Warp(Vector3 pos){var c=player.GetComponent<CharacterController>();c.enabled=false;player.transform.position=pos;c.enabled=true;}
    IEnumerator Start()
    {
        report="# Pruebas en Play Mode — base M01\n\n";
        player=FindAnyObjectByType<AfterglowPlayer>();enemy=FindAnyObjectByType<AfterglowEnemy>();rules=player.GetComponent<M01PlayerRules>();dialogue=FindAnyObjectByType<M01Dialogue>();
        yield return new WaitForSeconds(.3f);int frame=Time.frameCount;yield return null;Check("C02 frames",Time.frameCount>frame);
        var start=player.transform.position;Keys(Key.RightArrow);yield return new WaitForSeconds(.3f);Keys();Check("C05 movement",player.transform.position.x>start.x+1);
        enemy.enabled=false;Warp(new Vector3(10,.05f,-1));yield return new WaitForSeconds(.2f);Keys(Key.X,Key.RightArrow);float x=player.transform.position.x;yield return new WaitForSeconds(.3f);Check("C06 stop during attack",Mathf.Abs(player.transform.position.x-x)<.08f);
        Keys(Key.X);yield return new WaitForSeconds(1);Keys();Check("C06 hold once / one damage",enemy.Health==2,"enemy health="+enemy.Health);
        Check("C07 quiet contact has no damage",player.Health==10);
        Warp(new Vector3(5,.05f,0));yield return new WaitForSeconds(.15f);Keys(Key.Space);yield return new WaitForSeconds(.12f);Keys(Key.X);yield return new WaitForSeconds(.12f);Check("C06 no unbought air attack",player.AnimationState!="Attack");Keys();yield return new WaitForSeconds(.8f);
        yield return new WaitForSeconds(1.1f);int hp=player.Health;bool damage=player.TakeDamage(1);bool duplicate=player.TakeDamage(1);Check("C07 invulnerability",damage&&!duplicate&&player.Health==hp-1);
        yield return new WaitForSeconds(1.1f);Keys(Key.X);yield return new WaitForSeconds(.05f);bool received=player.TakeDamage(1);yield return new WaitForSeconds(.04f);Check("C07 no cancel or push",received&&player.AnimationState=="Attack");Keys();yield return new WaitForSeconds(.8f);
        int before=player.Health;player.AddExperience();int exp=player.Experience;Warp(new Vector3(8,-10,0));yield return new WaitForSecondsRealtime(.9f);Check("C08 fall costs 2 and section return",player.Health==before-2&&Vector3.Distance(player.transform.position,rules.sectionEntrance)<.3f&&player.Experience==exp);
        yield return new WaitForSeconds(1.1f);player.TakeDamage(100);yield return new WaitForSecondsRealtime(.9f);Check("C08 death returns level alive",player.Health==10&&!player.Dead&&Vector3.Distance(player.transform.position,rules.levelEntrance)<.3f&&player.Experience==exp);
        yield return new WaitForSeconds(1.1f);player.TakeDamage(8);Warp(new Vector3(8,-10,0));yield return new WaitForSecondsRealtime(.9f);Check("C08 lethal fall applies full death",player.Health==10&&!player.Dead&&Vector3.Distance(player.transform.position,rules.levelEntrance)<.3f&&player.Experience==exp);
        dialogue.Begin(new[]{"N1A.05","N1A.06"});float frozen=Time.time;int characters=dialogue.VisibleCharacters;yield return new WaitForSecondsRealtime(.3f);Check("C09 game frozen / text advances",Mathf.Abs(Time.time-frozen)<.001f&&dialogue.VisibleCharacters>characters);
        yield return Press(Key.E);Check("C09 E completes current line",dialogue.LineIndex==0&&dialogue.body.maxVisibleCharacters>=dialogue.body.textInfo.characterCount);
        Keys(Key.E);yield return new WaitForSecondsRealtime(.7f);Check("C09 holding E does not skip conversation",dialogue.IsOpen&&dialogue.LineIndex==1);Keys();yield return new WaitForSecondsRealtime(.1f);
        yield return Press(Key.E);yield return Press(Key.E);Check("C09 restores game clock",!dialogue.IsOpen&&Time.timeScale==1);
        Check("camera preserved",FindAnyObjectByType<AfterglowCamera>().offset==new Vector3(0,3.1f,-14)&&FindAnyObjectByType<AfterglowCamera>().GetComponent<Camera>().fieldOfView==32);
        yield return new WaitForSeconds(1.1f);ScreenCapture.CaptureScreenshot("Docs/Implementacion/Base_M01.png");Keys();Destroy(gameObject);
    }
}
