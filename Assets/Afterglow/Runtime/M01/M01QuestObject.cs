using UnityEngine;
namespace Afterglow.M01
{
    public sealed class M01QuestObject:MonoBehaviour
    {
        public string itemId="Q02.CASCO";
        public string displayName="Casco";
        void Start(){if(M01Session.Instance.data.collectedStaticIds.Contains(itemId))gameObject.SetActive(false);}
        void Update()
        {
            if(Time.timeScale==0)return;var player=FindAnyObjectByType<AfterglowPlayer>();
            if(!player||player.Dead||Vector3.Distance(transform.position,player.transform.position+Vector3.up*.35f)>.8f)return;
            var data=M01Session.Instance.data;
            if(!data.collectedStaticIds.Contains(itemId)){data.collectedStaticIds.Add(itemId);data.questItems.Add(itemId);M01Progress.Announce(displayName);}
            gameObject.SetActive(false);
        }
    }
}
