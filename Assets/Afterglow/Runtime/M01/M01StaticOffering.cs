using UnityEngine;
namespace Afterglow.M01
{
    public sealed class M01StaticOffering : MonoBehaviour
    {
        public string sourceId, kind = "Coca";
        public bool stolen;
        void Start() { if (M01Session.Instance.data.collectedStaticIds.Contains(sourceId)) gameObject.SetActive(false); }
        void Update()
        {
            if (Time.timeScale == 0) return;
            var player = FindAnyObjectByType<AfterglowPlayer>();
            if (!player || player.Dead || Vector3.Distance(transform.position, player.transform.position + Vector3.up * .35f) > .8f) return;
            M01Session.Instance.data.CollectOffering(sourceId, kind, stolen); gameObject.SetActive(false);
        }
    }
}
