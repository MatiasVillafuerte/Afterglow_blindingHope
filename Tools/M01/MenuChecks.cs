using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using Afterglow.M01;
public class M01MenuChecks:MonoBehaviour
{
 string report="# Inicio y pausa\n";
 M01Dialogue D=>FindAnyObjectByType<M01Dialogue>();
 public static string Main(){var o=new GameObject("M01 Menu checks");DontDestroyOnLoad(o);o.AddComponent<M01MenuChecks>();return "Started";}
 void Check(string label,bool pass){report+="- "+label+": "+pass+"\n";File.WriteAllText("Docs/Implementacion/Pruebas_Menu_M01.md",report);}
 IEnumerator Key(Key key){InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(key));yield return new WaitForSecondsRealtime(.1f);InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState());yield return new WaitForSecondsRealtime(.1f);}
 IEnumerator Choose(int index){yield return Key(UnityEngine.InputSystem.Key.E);for(int i=0;i<index;i++)yield return Key(UnityEngine.InputSystem.Key.DownArrow);yield return Key(UnityEngine.InputSystem.Key.E);}
 IEnumerator Start()
 {
  InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;InputSystem.EnableDevice(Keyboard.current);
  string before=File.Exists(M01Save.Path)?File.ReadAllText(M01Save.Path):null;bool valid=M01Save.ReadFile(M01Save.Path,out _,out _);
  SceneManager.LoadScene(M01Expedition.ScenePath("M01_MENU"));yield return null;yield return null;yield return new WaitForSecondsRealtime(.3f);Check("Inicio ofrece solo opciones disponibles",D.IsOpen&&D.choiceLabels[0].text==(valid?"Continuar":"Nueva partida"));yield return Key(UnityEngine.InputSystem.Key.E);yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot("Docs/Implementacion/Menu_Inicio_M01.png");if(valid)yield return Key(UnityEngine.InputSystem.Key.DownArrow);yield return Key(UnityEngine.InputSystem.Key.E);yield return null;yield return null;yield return new WaitForSecondsRealtime(.2f);
  Check("Nueva partida abre la carta",SceneManager.GetActiveScene().name=="PRO_CASA"&&D.CurrentId=="P01.02");yield return Key(UnityEngine.InputSystem.Key.E);yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot("Docs/Implementacion/Prologo_Carta.png");
  int guard=0;while(D.IsOpen&&guard++<15)yield return Key(UnityEngine.InputSystem.Key.E);yield return new WaitForSeconds(.15f);
  yield return Key(UnityEngine.InputSystem.Key.Escape);Check("Esc pausa el juego",D.IsOpen&&Time.timeScale==0);yield return Key(UnityEngine.InputSystem.Key.E);yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot("Docs/Implementacion/Menu_Pausa_M01.png");yield return Key(UnityEngine.InputSystem.Key.DownArrow);yield return Key(UnityEngine.InputSystem.Key.DownArrow);yield return Key(UnityEngine.InputSystem.Key.E);
  yield return Choose(1);yield return Choose(1);Check("Cancelar salida conserva sesión abierta",UnityEditor.EditorApplication.isPlaying&&!D.IsOpen&&Time.timeScale==1);string after=File.Exists(M01Save.Path)?File.ReadAllText(M01Save.Path):null;Check("Nueva partida y cancelar no sobrescriben guardado",before==after);
  Destroy(gameObject);
 }
}
