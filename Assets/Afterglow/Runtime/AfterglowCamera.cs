using UnityEngine;

namespace Afterglow
{
    public class AfterglowCamera : MonoBehaviour
    {
        public AfterglowPlayer player;
        public Vector3 offset = new Vector3(0, 3.8f, -17);
        public float anticipation = 1.5f, smoothing = .25f, minX = 10, maxX = 34;

        [Header("Framing for rooms with depth")]
        public bool followDepth;
        public Vector2 focusX = new Vector2(-12, 12), focusZ = new Vector2(-10, 14);
        public Vector2 depthBlend = new Vector2(-5, 10);
        public Vector2 pitch = new Vector2(16, 22), yaw = new Vector2(0, -10);
        public Vector2 distance = new Vector2(11, 13);
        public float focusHeight = 1.25f;
        Vector3 velocity;

        void Start() => Snap();

        public void Snap()
        {
            if (!player) return;
            Target(out var position, out var rotation);
            transform.SetPositionAndRotation(position, rotation);
            velocity = Vector3.zero;
        }

        void Target(out Vector3 position, out Quaternion rotation)
        {
            var actor = player.transform.position;
            var lead = player.Facing * anticipation;
            if (!followDepth)
            {
                position = new Vector3(Mathf.Clamp(actor.x + lead.x, minX, maxX), Mathf.Max(0, actor.y - 1) * .65f, 0) + offset;
                rotation = Quaternion.Euler(5, 0, 0);
                return;
            }

            float depth = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(depthBlend.x, depthBlend.y, actor.z));
            float elevation = Mathf.InverseLerp(0, 3, actor.y);
            var focus = new Vector3(Mathf.Clamp(actor.x + lead.x, focusX.x, focusX.y),
                actor.y + focusHeight, Mathf.Clamp(actor.z + lead.z, focusZ.x, focusZ.y));
            rotation = Quaternion.Euler(Mathf.Lerp(pitch.x, pitch.y, depth) + elevation * 3,
                Mathf.Lerp(yaw.x, yaw.y, depth), 0);
            position = focus - rotation * Vector3.forward * (Mathf.Lerp(distance.x, distance.y, depth) + elevation * .7f);
        }

        void LateUpdate()
        {
            if (!player || Time.deltaTime <= 0) return;
            Target(out var position, out var rotation);
            transform.position = Vector3.SmoothDamp(transform.position, position, ref velocity, smoothing);
            if (followDepth)
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1 - Mathf.Exp(-Time.deltaTime * 6));
        }
    }
}
