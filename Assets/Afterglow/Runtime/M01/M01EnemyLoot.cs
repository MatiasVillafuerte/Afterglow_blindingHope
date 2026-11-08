using UnityEngine;
namespace Afterglow.M01
{
    public sealed class M01EnemyLoot : MonoBehaviour
    {
        public int referenceLevel = 1, baseExperience = 3, money = 2;
        public bool firstTutorialOffering;
        public Material material;
        bool dropped;
        public void Drop()
        {
            if (dropped) return; dropped = true;
            M01QuestNPC.RecordKill(name);
            var data = M01Session.Instance.data;
            int experience = Mathf.Max(0, baseExperience - Mathf.Max(0, data.teamLevel - referenceLevel));
            if (experience > 0) Spawn(M01LootKind.Experience, experience, -.5f);
            Spawn(M01LootKind.Money, Mathf.RoundToInt(money * (1 + .1f * M01SkillTree.Rank("BOLSILLOS"))), .5f);
            if (firstTutorialOffering || Random.value < .25f) Spawn(M01LootKind.Offering, 1, 0);
            if (Random.value < .1f) Spawn(M01LootKind.Healing, 2, 1);
        }
        void Spawn(M01LootKind kind, int value, float x)
        {
            Vector3 position = transform.position + new Vector3(x, .6f, .45f);
            if (Physics.Raycast(position, Vector3.down, out var floor, 3, 1 << 0)) position.y = floor.point.y + .35f;
            var root = GameObject.CreatePrimitive(PrimitiveType.Sphere);root.name = "Botín " + kind;
            Destroy(root.GetComponent<Collider>());root.transform.position = position;root.transform.localScale = Vector3.one * .3f;
            root.GetComponent<Renderer>().sharedMaterial = material;
            var loot = root.AddComponent<M01Loot>(); loot.kind = kind; loot.amount = value;
        }
    }
}
