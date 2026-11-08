using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
namespace Afterglow {
[RequireComponent(typeof(CharacterController))]
public class AfterglowPlayer : MonoBehaviour {
 public float speed=5, jumpHeight=2, gravity=-25, attackCooldown=.65f, attackRange=1.65f, verticalTolerance=1.1f;
 public int maxHealth=10, damage=1;
 public Transform visual;
 public Animator animator;
 public int Health {get;private set;} public int Experience {get;private set;}
 public Vector3 Facing {get;private set;}=Vector3.right;
 public float VerticalSpeed=>velocityY; public bool Grounded=>controller.isGrounded;
 public string AnimationState {get;private set;}
 public bool Dead {get;private set;}
 CharacterController controller; SpriteRenderer[] sprites; float velocityY,nextAttack,attackEnd,impactAt,invulnerableUntil; bool impactPending,jumpRequested,attackRequested;
 InputAction jumpAction,attackAction;
 Afterglow.M01.M01PlayerRules worldOne;
 // The verification driver uses the same movement/attack path as the keyboard.
 [HideInInspector] public bool testControl; [HideInInspector] public Vector2 testMove; [HideInInspector] public bool testJump,testAttack;
 void Awake(){worldOne=GetComponent<Afterglow.M01.M01PlayerRules>();controller=GetComponent<CharacterController>();sprites=visual.GetComponentsInChildren<SpriteRenderer>();Health=AfterglowDoor.HasTravelState?AfterglowDoor.TravelHealth:maxHealth;Experience=AfterglowDoor.HasTravelState?AfterglowDoor.TravelExperience:0;AfterglowDoor.HasTravelState=false;}
 void OnEnable(){jumpAction=new InputAction("Jump",InputActionType.Button,"<Keyboard>/space");attackAction=new InputAction("Attack",InputActionType.Button,"<Keyboard>/x");jumpAction.performed+=_=>jumpRequested=true;attackAction.performed+=_=>attackRequested=true;jumpAction.Enable();attackAction.Enable();}
 void OnDisable(){jumpAction?.Dispose();attackAction?.Dispose();jumpRequested=attackRequested=false;}
 void Update(){
  if(Dead)return;
  if(worldOne && worldOne.BlocksControl){jumpRequested=attackRequested=false;return;}
  var k=Keyboard.current; if(k!=null&&k.f9Key.wasPressedThisFrame)AfterglowHitbox.ShowVolumes=!AfterglowHitbox.ShowVolumes;
  Vector2 move=Vector2.zero; bool jump=false,attack=false;
  if(testControl){move=testMove;jump=testJump;attack=testAttack;testJump=testAttack=false;}
  else if(k!=null){move=new Vector2((k.rightArrowKey.isPressed?1:0)-(k.leftArrowKey.isPressed?1:0),(k.upArrowKey.isPressed?1:0)-(k.downArrowKey.isPressed?1:0));jump=jumpRequested;attack=attackRequested;}
  jumpRequested=attackRequested=false;
  move=Vector2.ClampMagnitude(move,1); Vector3 horizontal=new Vector3(move.x,0,move.y);
  if(worldOne && attack && Time.time>=nextAttack && worldOne.CanAttack(controller.isGrounded)){nextAttack=Time.time+worldOne.AttackInterval;attackEnd=Time.time+worldOne.AttackInterval;impactAt=Time.time+.15f;impactPending=true;Play("Attack");}
  if(worldOne && Time.time<attackEnd && !worldOne.walkingAttackUnlocked){horizontal=Vector3.zero;move=Vector2.zero;jump=false;}
  if(horizontal.sqrMagnitude>.01f)Facing=horizontal.normalized;
  if(controller.isGrounded && velocityY<0)velocityY=-2;
  if(jump && controller.isGrounded)velocityY=Mathf.Sqrt(-2*gravity*jumpHeight);
  velocityY+=gravity*Time.deltaTime;
  controller.Move((horizontal*speed+Vector3.up*velocityY)*Time.deltaTime);
  if(Mathf.Abs(move.x)>.01f){var s=visual.localScale;s.x=Mathf.Abs(s.x)*Mathf.Sign(move.x);visual.localScale=s;}
  if(!worldOne && attack && Time.time>=nextAttack){nextAttack=Time.time+attackCooldown;attackEnd=Time.time+.6f;impactAt=Time.time+.22f;impactPending=true;Play("Attack");}
  if(impactPending && Time.time>=impactAt){impactPending=false;StrikeCount++;Strike();}
  if(Time.time>=attackEnd)Play(controller.isGrounded?(move.sqrMagnitude>.01f?"Walk":"Idle"):(velocityY>0?"Jump":"Falling"));
  bool visible=Time.time>=invulnerableUntil || ((int)(Time.time*14)%2==0);foreach(var s in sprites)s.enabled=visible;
  if(!worldOne && transform.position.y < -8)Die();
 }
 void Play(string state){if(AnimationState==state)return;AnimationState=state;animator.CrossFadeInFixedTime(state,.06f,0);}
 void Strike(){
  if(worldOne){
   // GDD 0.5: volumen frontal 1,2 m x 0,8 m contra los colliders reales; una sola aplicación por objetivo y acción.
   var struck=new System.Collections.Generic.HashSet<Object>();
   foreach(var col in AfterglowHitbox.Query(transform.position,Facing,transform,new Color(1f,.85f,.2f,.35f))){
    var rock=col.GetComponentInParent<Afterglow.M01.M01BreakableRock>();
    if(rock&&struck.Add(rock)){rock.Break();continue;}
    var enemy=col.GetComponentInParent<AfterglowEnemy>();
    if(enemy&&!enemy.Defeated&&struck.Add(enemy)&&!Physics.Linecast(transform.position+Vector3.up*.8f,enemy.transform.position+Vector3.up*.8f,1<<0,QueryTriggerInteraction.Ignore))enemy.TakeDamage(damage);
   }
   LastStrikeTargets=struck.Count;return;}
  foreach(var enemy in FindObjectsByType<AfterglowEnemy>()){
   Vector3 d=enemy.transform.position-transform.position;Vector3 flat=new Vector3(d.x,0,d.z);
   bool inRange=flat.magnitude<=attackRange&&Vector3.Dot(Facing,flat.normalized)>.25f;
   if(!enemy.Defeated && Mathf.Abs(d.y)<=verticalTolerance && inRange && !Physics.Linecast(transform.position+Vector3.up*.8f,enemy.transform.position+Vector3.up*.8f,1<<0))enemy.TakeDamage(damage);
  }}
 /// <summary>Objetivos alcanzados por el último golpe (diagnóstico de pruebas).</summary>
 public int LastStrikeTargets {get;private set;}
 public int StrikeCount {get;private set;}
 public bool TakeDamage(int amount){if(Dead||(worldOne&&worldOne.BlocksControl)||Time.time<invulnerableUntil||amount<=0)return false;int before=Health;Health=Mathf.Max(0,Health-amount);AfterglowDamagePopup.Spawn(transform,before-Health);if(worldOne)worldOne.Progress.Active.health=Health;invulnerableUntil=Time.time+1;if(Health==0)Die();return true;}
 public void AddExperience(){if(Dead)return;if(worldOne){worldOne.Progress.AddExperience(1);Experience=worldOne.Progress.experience;Health=worldOne.Progress.Active.health;}else Experience++;}
 public bool ApplyWorldOneFallDamage(int amount){if(!worldOne)return false;Health=Mathf.Max(0,Health-amount);worldOne.Progress.Active.health=Health;if(Health==0)Die();return Health>0;}
 public void SynchronizeWorldOneState(){if(worldOne){maxHealth=worldOne.Progress.Active.maxHealth;Health=worldOne.Progress.Active.health;Experience=worldOne.Progress.experience;}}
 public void RestoreWorldOneFacing(Vector3 facing){Facing=facing.normalized;if(Mathf.Abs(Facing.x)>.01f){var scale=visual.localScale;scale.x=Mathf.Abs(scale.x)*Mathf.Sign(Facing.x);visual.localScale=scale;}}
 public void ResetWorldOneBody(bool restoreHealth){Dead=false;velocityY=-2;impactPending=false;attackEnd=nextAttack=0;jumpRequested=attackRequested=false;if(restoreHealth)Health=maxHealth;if(worldOne)worldOne.Progress.Active.health=Health;invulnerableUntil=Time.time+1;foreach(var sprite in sprites)sprite.enabled=true;}
 void Die(){if(Dead)return;Dead=true;impactPending=false;if(worldOne){worldOne.HandleDeath();return;}Invoke(nameof(Restart),1.5f);}
 void Restart(){AfterglowDoor.HasTravelState=false;SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);}
}
}

