using UnityEngine;
using UnityEngine.SceneManagement;
namespace Afterglow {
public class AfterglowDoor:MonoBehaviour {
 public string targetScene;public float entryRadius=.85f;bool travelling;
 public static bool HasTravelState;public static int TravelHealth,TravelExperience;
 void Update(){if(travelling)return;var p=FindAnyObjectByType<AfterglowPlayer>();if(!p||p.Dead)return;Vector3 delta=p.transform.position-transform.position;
 if(Mathf.Abs(delta.y)<.6f&&new Vector2(delta.x,delta.z).magnitude<entryRadius){travelling=true;TravelHealth=p.Health;TravelExperience=p.Experience;HasTravelState=true;SceneManager.LoadScene(targetScene);}}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]static void ClearTravelState(){HasTravelState=false;}
}
}

