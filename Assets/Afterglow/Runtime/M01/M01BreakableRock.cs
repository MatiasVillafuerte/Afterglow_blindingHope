using UnityEngine;

namespace Afterglow.M01
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class M01BreakableRock : MonoBehaviour
    {
        const string Broken = "N01.S03.ROCA_ROTA";
        void Start()
        {
            if (M01Session.Instance.data.events.Contains(Broken)) gameObject.SetActive(false);
        }

        public void TryStrike(Vector3 origin, Vector3 facing)
        {
            var collision = GetComponent<BoxCollider>();
            if (!Physics.Raycast(origin, facing, out var hit, 1.24f, ~0, QueryTriggerInteraction.Ignore) || hit.collider != collision)
                return;
            Break();
        }

        /// <summary>Lo invoca el volumen del pico (AfterglowHitbox) cuando alcanza el collider de la roca.</summary>
        public void Break()
        {
            if (!gameObject.activeInHierarchy) return;
            var events = M01Session.Instance.data.events;
            if (!events.Contains(Broken)) events.Add(Broken);
            Debris();
            gameObject.SetActive(false);
        }

        void Debris()
        {
            var bounds = GetComponent<BoxCollider>().bounds;
            var source = GetComponentInChildren<Renderer>();
            var random = new System.Random(7);
            for (int i = 0; i < 9; i++)
            {
                var chunk = GameObject.CreatePrimitive(PrimitiveType.Cube);
                chunk.name = "Fragmento de roca";
                float s = .18f + (float)random.NextDouble() * .22f;
                chunk.transform.localScale = Vector3.one * s;
                chunk.transform.position = bounds.center + new Vector3((float)(random.NextDouble() - .5) * bounds.size.x * .7f, (float)(random.NextDouble() - .3) * bounds.size.y * .6f, (float)(random.NextDouble() - .5) * bounds.size.z * .5f);
                chunk.transform.rotation = Random.rotation;
                chunk.layer = 2;
                var player = FindAnyObjectByType<AfterglowPlayer>();
                if (player) Physics.IgnoreCollision(chunk.GetComponent<Collider>(), player.GetComponent<CharacterController>());
                if (source) chunk.GetComponent<Renderer>().sharedMaterial = source.sharedMaterial;
                var body = chunk.AddComponent<Rigidbody>();
                body.linearVelocity = new Vector3((float)(random.NextDouble() - .5) * 5, 2.5f + (float)random.NextDouble() * 3, (float)(random.NextDouble() - .5) * 3);
                body.angularVelocity = Random.insideUnitSphere * 8;
                Destroy(chunk, 1.4f);
            }
        }
    }
}
