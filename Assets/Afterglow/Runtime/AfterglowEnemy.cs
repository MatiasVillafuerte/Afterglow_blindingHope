using UnityEngine;
namespace Afterglow {
[RequireComponent(typeof(CharacterController))]
public class AfterglowEnemy:MonoBehaviour {
 // reach = distancia a la que se detiene y prepara su golpe. Debe quedar dentro del alcance del pico (AfterglowHitbox).
 public int maxHealth=2,damage=1;public float detection=6,reach=1.05f,interval=1.2f,speed=1.6f;
 public Transform visual;public Material rewardMaterial;
 public int Health {get;private set;} public bool Defeated {get;private set;} public int HitsTaken {get;private set;}
 AfterglowPlayer player;CharacterController controller;float nextAttack,attackStart=-10,flashUntil;bool hit,flashing;float vertical;Vector3 attackDirection=Vector3.right;
 Renderer[] renderers;MaterialPropertyBlock block;
 void Awake(){Health=maxHealth;controller=GetComponent<CharacterController>();player=FindAnyObjectByType<AfterglowPlayer>();renderers=visual?visual.GetComponentsInChildren<Renderer>(true):new Renderer[0];block=new MaterialPropertyBlock();}
 public void RestoreWorldOneState(int health,Vector3 position){controller.enabled=false;transform.position=position;Health=Mathf.Clamp(health,0,maxHealth);Defeated=Health==0;visual.gameObject.SetActive(!Defeated);controller.enabled=!Defeated;}
 void Update(){
  if(flashing&&Time.time>=flashUntil)SetFlash(false);
  if(Time.timeScale==0||Defeated||player==null||player.Dead)return;
  Vector3 delta=player.transform.position-transform.position;Vector3 flat=new Vector3(delta.x,0,delta.z);float distance=flat.magnitude;
  float phase=Time.time-attackStart;
  if(phase<.7f){float tilt=phase<.28f?-12*phase/.28f:phase<.4f?35:Mathf.Lerp(35,0,(phase-.4f)/.3f);visual.localRotation=Quaternion.Euler(0,0,-Mathf.Sign(attackDirection.x==0?1:attackDirection.x)*tilt);
   // El golpe usa la dirección fijada al empezar la anticipación: el jugador puede salir del volumen.
   if(!hit&&phase>=.3f){hit=true;foreach(var col in AfterglowHitbox.Query(transform.position,attackDirection,transform,new Color(1f,.25f,.2f,.35f)))
     if(col.GetComponentInParent<AfterglowPlayer>()==player&&!Physics.Linecast(transform.position+Vector3.up*.8f,player.transform.position+Vector3.up*.8f,1<<0,QueryTriggerInteraction.Ignore)){player.TakeDamage(damage);break;}}
   return;}
  visual.localRotation=Quaternion.identity;
  Vector3 movement=Vector3.zero;
  if(Mathf.Abs(delta.y)<.8f && distance<=detection){if(distance<=reach){if(Time.time>=nextAttack){attackStart=Time.time;nextAttack=Time.time+interval;hit=false;attackDirection=flat.sqrMagnitude>.0001f?flat.normalized:Vector3.right;}}
   else {Vector3 dir=flat.normalized;Vector3 ahead=transform.position+dir*.8f+Vector3.up*.4f;
    if(Physics.Raycast(ahead,Vector3.down,out var ground,1,1<<0)&&Mathf.Abs(ground.point.y-transform.position.y)<.35f&&!Physics.Raycast(transform.position+Vector3.up*.7f,dir,.8f,1<<0))movement=dir*speed;}}
  if(controller.isGrounded)vertical=-2;else vertical-=25*Time.deltaTime;controller.Move((movement+Vector3.up*vertical)*Time.deltaTime);
 }
 void SetFlash(bool on){flashing=on;foreach(var r in renderers){if(!r)continue;if(!on){r.SetPropertyBlock(null);continue;}
   var m=r.sharedMaterial;Color c=m&&m.HasProperty("_BaseColor")?m.GetColor("_BaseColor"):Color.white;r.GetPropertyBlock(block);block.SetColor("_BaseColor",Color.Lerp(c,new Color(1f,.35f,.3f),.65f));block.SetColor("_Color",new Color(1f,.45f,.4f));r.SetPropertyBlock(block);}}
 /// <summary>Aplica daño una vez; devuelve true si restó vida. Muestra la cifra efectiva aunque el objetivo caiga.</summary>
 public bool TakeDamage(int amount){if(Defeated||amount<=0)return false;HitsTaken++;int before=Health;Health=Mathf.Max(0,Health-amount);AfterglowDamagePopup.Spawn(transform,before-Health);
  if(Health>0){SetFlash(true);flashUntil=Time.time+.14f;return true;}
  Defeated=true;SetFlash(false);controller.enabled=false;visual.gameObject.SetActive(false);
  var worldLoot=GetComponent<Afterglow.M01.M01EnemyLoot>();if(worldLoot){worldLoot.Drop();return true;}
  for(int i=0;i<3;i++){Vector3 p=transform.position+new Vector3((i-1)*.55f,.6f,.45f);if(!Physics.Raycast(p,Vector3.down,out var floor,3,1<<0)){p=transform.position+Vector3.up*.6f;Physics.Raycast(p,Vector3.down,out floor,3,1<<0);}p.y=floor.point.y+.35f;
   var coin=new GameObject("EXP +1");coin.transform.position=p;var pickup=coin.AddComponent<AfterglowPickup>();pickup.player=player;
   var disc=GameObject.CreatePrimitive(PrimitiveType.Cylinder);disc.name="Flat circle";disc.transform.SetParent(coin.transform,false);disc.transform.localScale=new Vector3(.3f,.025f,.3f);disc.transform.localRotation=Quaternion.Euler(70,0,0);Destroy(disc.GetComponent<Collider>());disc.GetComponent<Renderer>().sharedMaterial=rewardMaterial;}
  return true;
 }
}
}
