using UnityEngine;

namespace Afterglow
{
    /// <summary>Caja translúcida temporal que muestra un volumen de golpe (solo depuración, F9).</summary>
    public sealed class AfterglowHitboxView : MonoBehaviour
    {
        static Material material;
        float until;

        public static void Show(Vector3 center, Vector3 halfExtents, Quaternion rotation, Color color, float duration)
        {
            if (!material) material = new Material(Shader.Find("Sprites/Default"));
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Hitbox (debug)";
            Destroy(cube.GetComponent<Collider>());
            cube.transform.SetPositionAndRotation(center, rotation);
            cube.transform.localScale = halfExtents * 2;
            var renderer = cube.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            var block = new MaterialPropertyBlock();
            block.SetColor("_Color", color);
            renderer.SetPropertyBlock(block);
            cube.AddComponent<AfterglowHitboxView>().until = Time.time + duration;
        }

        void Update() { if (Time.time >= until) Destroy(gameObject); }
    }
}
