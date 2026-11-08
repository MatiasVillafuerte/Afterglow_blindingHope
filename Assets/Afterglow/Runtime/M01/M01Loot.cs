using UnityEngine;
namespace Afterglow.M01
{
    public enum M01LootKind { Experience, Money, Offering, Healing }
    public sealed class M01Loot : MonoBehaviour
    {
        public M01LootKind kind;
        public int amount = 1;
        public float remaining = 5;
        bool collected;
        void Update()
        {
            if (collected || Time.timeScale == 0) return;
            remaining -= Time.deltaTime;
            if (remaining <= 0) { Destroy(gameObject); return; }
            foreach (var renderer in GetComponentsInChildren<Renderer>()) renderer.enabled = remaining > 3 || (int)(remaining * 10) % 2 == 0;
            var player = FindAnyObjectByType<AfterglowPlayer>();
            if (!player || player.Dead || Vector3.Distance(transform.position, player.transform.position + Vector3.up * .35f) > .85f) return;
            collected = true;
            var data = M01Session.Instance.data;
            if (kind == M01LootKind.Experience) { data.AddExperience(amount); M01Progress.Announce("EXP", amount); }
            else if (kind == M01LootKind.Money) { data.money += amount; M01Progress.Announce("Dinero", amount); }
            else if (kind == M01LootKind.Healing) { data.Active.health = Mathf.Min(data.Active.maxHealth, data.Active.health + amount); M01Progress.Announce("Curativo"); }
            else data.AddDroppedOffering("Coca");
            player.SynchronizeWorldOneState(); Destroy(gameObject);
        }
    }
}
