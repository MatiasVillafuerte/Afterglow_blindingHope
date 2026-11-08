using UnityEngine;

namespace Afterglow.M01
{
    /// <summary>Piedra de la arena (GDD 9.3): círculo rojo y sombra al menos 1,2 s antes; daña una sola vez al impactar.</summary>
    public sealed class M01FallingRock : MonoBehaviour
    {
        public float radius = .6f, warn = 1.3f; public int damage = 1;
        Transform marker, rock; float born; bool landed;
        public static M01FallingRock Spawn(Vector3 ground, bool large, Material warnMat, Material rockMat)
        {
            var go = new GameObject(large ? "Piedra grande" : "Piedra pequeña"); go.transform.position = ground;
            var r = go.AddComponent<M01FallingRock>(); r.radius = large ? 1.1f : .6f; r.damage = large ? 2 : 1;
            var m = GameObject.CreatePrimitive(PrimitiveType.Cylinder); Destroy(m.GetComponent<Collider>()); m.name = "Aviso"; m.transform.SetParent(go.transform, false);
            m.transform.localPosition = Vector3.up * .03f; m.transform.localScale = new Vector3(r.radius * 2, .01f, r.radius * 2); m.GetComponent<MeshRenderer>().sharedMaterial = warnMat; r.marker = m.transform;
            var k = GameObject.CreatePrimitive(PrimitiveType.Sphere); Destroy(k.GetComponent<Collider>()); k.name = "Roca"; k.transform.SetParent(go.transform, false);
            k.transform.localScale = new Vector3(r.radius * 1.3f, r.radius * 1.1f, r.radius * 1.3f); k.GetComponent<MeshRenderer>().sharedMaterial = rockMat; k.SetActive(false); r.rock = k.transform;
            return r;
        }
        void Start() { born = Time.time; }
        void Update()
        {
            float t = Time.time - born;
            if (marker) marker.localScale = new Vector3(radius * 2 * (1 + Mathf.Sin(t * 12) * .05f), .01f, radius * 2 * (1 + Mathf.Sin(t * 12) * .05f));
            if (t > warn - .3f && !landed) { rock.gameObject.SetActive(true); rock.localPosition = Vector3.up * Mathf.Lerp(7, radius * .5f, Mathf.Clamp01((t - (warn - .3f)) / .3f)); }
            if (t >= warn && !landed)
            {
                landed = true;
                var p = FindAnyObjectByType<AfterglowPlayer>();
                if (p) { var d = p.transform.position - transform.position; d.y = 0; if (d.magnitude <= radius + .3f) p.TakeDamage(damage); }
                if (marker) Destroy(marker.gameObject);
            }
            if (t > warn + .8f) Destroy(gameObject);
        }
    }
}
