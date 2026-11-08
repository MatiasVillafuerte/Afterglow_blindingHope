using UnityEngine;
namespace Afterglow.M01
{
    public sealed class M01PickaxeVisual : MonoBehaviour
    {
        public AfterglowPlayer player;
        public SpriteRenderer sprite;
        void LateUpdate(){sprite.enabled=player && !player.Dead && player.AnimationState=="Attack";}
    }
}
