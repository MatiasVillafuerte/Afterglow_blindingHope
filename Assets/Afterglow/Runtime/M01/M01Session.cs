using UnityEngine;
namespace Afterglow.M01
{
    public sealed class M01Session : MonoBehaviour
    {
        static M01Session instance;
        public M01Progress data = new M01Progress();
        public static M01Session Instance
        {
            get { if (!instance) instance = new GameObject("M01 Session").AddComponent<M01Session>(); return instance; }
        }
        void Awake() { if (instance && instance != this) { Destroy(gameObject); return; } instance = this; DontDestroyOnLoad(gameObject); }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] static void ClearSession() { instance = null; }
    }
}
