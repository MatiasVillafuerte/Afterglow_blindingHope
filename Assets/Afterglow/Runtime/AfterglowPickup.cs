using UnityEngine;
namespace Afterglow {
public class AfterglowPickup:MonoBehaviour {
 public AfterglowPlayer player;public float radius=.85f;bool collected;
 void Update(){if(collected||player==null||player.Dead)return;if(transform.childCount>0)transform.GetChild(0).localPosition=Vector3.up*(Mathf.Sin(Time.time*3)*.08f);if(Vector3.Distance(player.transform.position+Vector3.up*.35f,transform.position)<radius){collected=true;player.AddExperience();Destroy(gameObject);}}
}
}
