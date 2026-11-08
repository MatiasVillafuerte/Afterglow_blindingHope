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
        public bool BlocksControl => Recovering || scriptedControl || M01Expedition.Transitioning || Time.timeScale == 0;
        AfterglowPlayer player;
        float baseSpeed;
        public float AttackInterval=>.6f*(Progress.preparationRemaining>0&&Progress.preparation=="Intervalo"?.9f:1);
        public M01Progress Progress => M01Session.Instance.data;

        void Awake() { player = GetComponent<AfterglowPlayer>();baseSpeed=player.speed; }
        void Start() { if (foundationTestKit) Progress.phoneOwned = true; else hasPickaxe = Progress.pickaxeOwned; player.SynchronizeWorldOneState(); }
        public bool CanAttack(bool grounded) => hasPickaxe && !BlocksControl && (grounded || airAttackUnlocked);
        void Update()
        {
            Progress.preparationRemaining=Mathf.Max(0,Progress.preparationRemaining-Time.deltaTime);
            player.speed=baseSpeed*(Progress.preparationRemaining>0&&Progress.preparation=="Velocidad"?1.1f:1);
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
