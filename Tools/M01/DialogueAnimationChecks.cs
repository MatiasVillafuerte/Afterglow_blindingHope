using System.IO;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Afterglow;
using Afterglow.M01;
public class M01DialogueAnimationChecks:MonoBehaviour
{
 string report="# Correcciones visuales\n";
 public static string Main(){new GameObject("UI and animation checks").AddComponent<M01DialogueAnimationChecks>();return "Started";}
 void Check(string label,bool ok){report+="- "+label+": "+ok+"\n";File.WriteAllText("Docs/Implementacion/Pruebas_Correcciones_Visuales.md",report);}
 IEnumerator Key(Key key){InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(key));yield return new WaitForSecondsRealtime(.06f);InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());yield return new WaitForSecondsRealtime(.06f);}
 IEnumerator Close(M01Dialogue d){int guard=0;while(d.IsOpen&&guard++<80)yield return Key(UnityEngine.InputSystem.Key.E);}
 IEnumerator Start(){var d=FindAnyObjectByType<M01Dialogue>();yield return Close(d);
  d.BeginText("Inventario",string.Join("\n",Enumerable.Range(1,30).Select(i=>"Objeto de prueba "+i+": descripción extensa del inventario.")));yield return new WaitForSecondsRealtime(.1f);Check("Texto largo paginado",d.body.textInfo.pageCount>1);int page=d.body.pageToDisplay;yield return Key(UnityEngine.InputSystem.Key.E);yield return Key(UnityEngine.InputSystem.Key.E);Check("E avanza página sin cerrar inventario",d.IsOpen&&d.body.pageToDisplay>page);var corners=new Vector3[4];d.body.rectTransform.GetWorldCorners(corners);var hint=new Vector3[4];d.hint.rectTransform.GetWorldCorners(hint);Check("Ayuda separada del cuerpo",hint[1].y<corners[0].y);yield return Close(d);
  int selected=-1;d.BeginChoice("¿Llamar para volver al acceso con lo conseguido?",new[]{"Volver","Seguir explorando"},i=>selected=i);yield return Key(UnityEngine.InputSystem.Key.DownArrow);yield return Key(UnityEngine.InputSystem.Key.E);ScreenCapture.CaptureScreenshot("Docs/Implementacion/Opciones_Dialogo.png");Check("Opciones en filas independientes",d.choiceLabels[0].rectTransform.position.y>d.choiceLabels[1].rectTransform.position.y&&d.choiceLabels[0].text=="Volver");yield return Key(UnityEngine.InputSystem.Key.E);Check("Selección conservada",selected==1);
  var p=FindAnyObjectByType<AfterglowPlayer>();var enemy=FindAnyObjectByType<AfterglowEnemy>();if(enemy)enemy.enabled=false;var tool=p.GetComponentInChildren<M01PickaxeVisual>();yield return new WaitForEndOfFrame();Check("Pico oculto en reposo",!tool.sprite.enabled);var idle=p.animator.runtimeAnimatorController.animationClips.First(c=>c.name=="M01_Idle");Check("Idle lento 2.8 segundos",Mathf.Abs(idle.length-2.8f)<.02f);
  yield return Key(UnityEngine.InputSystem.Key.X);yield return new WaitForSeconds(.06f);yield return new WaitForEndOfFrame();Check("Pico visible durante golpe",tool.sprite.enabled&&p.AnimationState=="Attack");Time.timeScale=0;yield return new WaitForSecondsRealtime(.1f);ScreenCapture.CaptureScreenshot("Docs/Implementacion/Ataque_Pico.png");yield return new WaitForSecondsRealtime(.15f);Time.timeScale=1;yield return new WaitForSeconds(.8f);yield return new WaitForEndOfFrame();Check("Pico guardado al finalizar",!tool.sprite.enabled);Destroy(gameObject);
 }
}

