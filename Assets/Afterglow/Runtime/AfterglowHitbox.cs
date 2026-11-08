using System.Collections.Generic;
using UnityEngine;

namespace Afterglow
{
    /// <summary>
    /// Volumen 3D frontal compartido por el pico y por los ataques de los enemigos.
    /// GDD 0.5: alcance frontal aproximado 1,2 m y anchura 0,8 m, medidos desde el centro de quien golpea.
    /// Se compara contra los colliders reales del objetivo, de modo que la profundidad (eje Z) y la altura cuentan.
    /// </summary>
    public static class AfterglowHitbox
    {
        public const float Reach = 1.2f, Width = .8f, Height = 1.6f, Back = .15f;
        /// <summary>F9 durante el juego alterna la visualización de los volúmenes de golpe.</summary>
        public static bool ShowVolumes;
        static readonly Collider[] buffer = new Collider[32];

        public static void Box(Vector3 feet, Vector3 facing, out Vector3 center, out Vector3 halfExtents, out Quaternion rotation)
        {
            facing.y = 0;
            if (facing.sqrMagnitude < .0001f) facing = Vector3.right;
            facing.Normalize();
            rotation = Quaternion.LookRotation(facing, Vector3.up);
            halfExtents = new Vector3(Width * .5f, Height * .5f, (Reach + Back) * .5f);
            center = feet + Vector3.up * (Height * .5f + .05f) + facing * ((Reach - Back) * .5f);
        }

        /// <summary>Colliders únicos dentro del volumen, excluyendo a quien golpea.</summary>
        public static List<Collider> Query(Vector3 feet, Vector3 facing, Transform self, Color debugColor)
        {
            Box(feet, facing, out var center, out var half, out var rotation);
            int count = Physics.OverlapBoxNonAlloc(center, half, buffer, rotation, ~0, QueryTriggerInteraction.Ignore);
            var result = new List<Collider>(count);
            for (int i = 0; i < count; i++)
                if (buffer[i] && (!self || !buffer[i].transform.IsChildOf(self))) result.Add(buffer[i]);
            if (ShowVolumes) AfterglowHitboxView.Show(center, half, rotation, debugColor, .25f);
            return result;
        }
    }
}
