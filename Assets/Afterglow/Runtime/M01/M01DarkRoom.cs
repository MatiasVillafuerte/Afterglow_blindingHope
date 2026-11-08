using UnityEngine;

namespace Afterglow.M01
{
    /// <summary>
    /// Sala con luz escasa (N4). Oscurece el ambiente y la luz principal y enciende la lámpara del casco de la
    /// protagonista: un círculo corto que mantiene legibles el suelo inmediato y los bordes cercanos (N4.02).
    /// Los faroles fijos, el carro de luz y los Lighmet aportan el resto de la orientación.
    /// </summary>
    public sealed class M01DarkRoom : MonoBehaviour
    {
        public Color ambient = new Color(.05f, .05f, .07f);
        public float sunIntensity = .06f, helmetRange = 4.2f, helmetIntensity = 2.4f;
        void Start()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat; RenderSettings.ambientLight = ambient;
            foreach (var l in FindObjectsByType<Light>(FindObjectsSortMode.None)) if (l.type == LightType.Directional) l.intensity = sunIntensity;
            var player = FindAnyObjectByType<AfterglowPlayer>(); if (!player) return;
            var lamp = new GameObject("Lámpara del casco").AddComponent<Light>(); lamp.transform.SetParent(player.transform, false);
            lamp.transform.localPosition = new Vector3(0, 2.1f, -.6f); lamp.type = LightType.Point; lamp.range = helmetRange; lamp.intensity = helmetIntensity;
            lamp.color = new Color(1f, .9f, .7f); lamp.shadows = LightShadows.None;
        }
    }
}
