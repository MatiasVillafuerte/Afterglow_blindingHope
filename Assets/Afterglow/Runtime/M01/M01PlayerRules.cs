using System.Collections;
using UnityEngine;

namespace Afterglow.M01
{
    /// <summary>World-one rules are opt-in; prototype scenes keep their original behavior.</summary>
    [RequireComponent(typeof(AfterglowPlayer))]
    public sealed class M01PlayerRules : MonoBehaviour
    {
        public bool hasPickaxe = true;
        public bool walkingAttackUnlocked;
        public bool airAttackUnlocked;
        public bool foundationTestKit = true;
        public bool restrictedActivity;
        public Vector3 sectionEntrance = new Vector3(4, .05f, 0);
        public Vector3 levelEntrance = new Vector3(4, .05f, 0);
        public float fallHeight = -8;
        public CanvasGroup fade;
        public bool Recovering { get; private set; }
        public bool scriptedControl;
        /// <summary>Modificadores temporales de la arena (efectos del Tío) y bloqueo de guardado durante jefe o minijuego.</summary>
        public static float SpeedMultiplier = 1, IntervalMultiplier = 1; public static int DamageBonus; public static bool SaveBlocked;
        /// <summary>Figura de Platicio cuando es el personaje activo (solo en el cierre de Mundo 1).</summary>
        public Sprite sonSprite; SpriteRenderer sonFigure;
        /// <summary>Aplica el personaje activo sin curar: vida propia, figura y acción de 0,9 s del hijo.</summary>
        public void ApplyMember()
        {
            bool son = Progress.activeMember == 1;
            foreach (var r in player.visual.GetComponentsInChildren<SpriteRenderer>(true)) if (r != sonFigure) r.enabled = !son;
            if (son && !sonFigure && sonSprite) { sonFigure = new GameObject("Figura de Platicio").AddComponent<SpriteRenderer>(); sonFigure.transform.SetParent(transform, false); sonFigure.sprite = sonSprite; }
            if (sonFigure) sonFigure.enabled = son;
            player.SynchronizeWorldOneState();
        }
        public static void ResetTemporary() { SpeedMultiplier = IntervalMultiplier = 1; DamageBonus = 0; SaveBlocked = false; }
        public bool BlocksControl => Recovering || scriptedControl || M01Expedition.Transitioning || Time.timeScale == 0;
        AfterglowPlayer player;
        float baseSpeed;
        float lastAttack=-10;int chain;
        /// <summary>Golpes enlazados: dentro de 0,8 s el siguiente golpe es más rápido, hasta 2 (rango 1) o 3 (rango 2) seguidos.</summary>
        public void RegisterAttack(){int linked=M01SkillTree.Rank("ENLAZADOS");chain=Time.time-lastAttack<.8f&&chain<linked?chain+1:0;lastAttack=Time.time;}
        float ChainFactor=>chain>0?.65f:1f;
        /// <summary>Daño de un golpe: base + Filo firme + efecto del Tío, con crítico doble de Golpe preciso.</summary>
        public int StrikeDamage(){int d=Mathf.Max(1,player.damage+M01SkillTree.Rank("FILO")+DamageBonus);int precise=M01SkillTree.Rank("PRECISO");if(precise>0&&Random.value<.05f*precise)d*=2;return d;}
        public float AttackInterval=>ChainFactor*(Progress.activeMember==1?.9f:.6f)*(Progress.preparationRemaining>0&&Progress.preparation=="Intervalo"?.9f:1)*IntervalMultiplier;
        public M01Progress Progress => M01Session.Instance.data;

        void Awake() { player = GetComponent<AfterglowPlayer>();baseSpeed=player.speed; if(!GetComponent<M01SkillEffects>())gameObject.AddComponent<M01SkillEffects>(); }
        void Start() { if (foundationTestKit) Progress.phoneOwned = true; else hasPickaxe = Progress.pickaxeOwned; player.SynchronizeWorldOneState(); }
        public bool CanAttack(bool grounded) => hasPickaxe && !BlocksControl && (grounded || airAttackUnlocked);
        void Update()
        {
            Progress.preparationRemaining=Mathf.Max(0,Progress.preparationRemaining-Time.deltaTime);
            player.speed=Mathf.Min(baseSpeed*1.4f,baseSpeed*(Progress.preparationRemaining>0&&Progress.preparation=="Velocidad"?1.1f:1)*SpeedMultiplier*(1+.05f*M01SkillTree.Rank("PASO")));
            if (!Recovering && !player.Dead && transform.position.y < fallHeight)
            {
                Recovering = true;
                if (player.ApplyWorldOneFallDamage(2)) StartCoroutine(Return(false));
            }
        }
        public void HandleDeath()
        {
            StopAllCoroutines();
            Recovering = true;
            StartCoroutine(Return(true));
        }
        IEnumerator Return(bool death)
        {
            // This local transition does not replace the level or erase permanent session data.
            if (fade) fade.alpha = 1;
            yield return new WaitForSecondsRealtime(.6f);
            if(death && FindAnyObjectByType<M01Expedition>() && !FindAnyObjectByType<M01Expedition>().safeArea) { M01Expedition.RespawnLevel(); yield break; }
            var controller = GetComponent<CharacterController>();
            controller.enabled = false;
            transform.position = death ? levelEntrance : sectionEntrance;
            controller.enabled = true;
            player.ResetWorldOneBody(death);
            var camera = FindAnyObjectByType<AfterglowCamera>();
            if (camera) camera.Snap();
            if (fade) fade.alpha = 0;
            Recovering = false;
        }
    }
}
