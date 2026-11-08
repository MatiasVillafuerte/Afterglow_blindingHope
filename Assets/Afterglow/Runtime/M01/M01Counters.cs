using UnityEngine;
namespace Afterglow.M01
{
    public sealed class M01Counters : MonoBehaviour
    {
        public TMPro.TMP_Text text;
        void Update(){var data=M01Session.Instance.data;int i=Mathf.Clamp(data.currentLevel-1,0,4);text.text="Nivel "+data.teamLevel+"   Dinero: "+data.money+"   Gemas: "+data.Active.gems+"   Ofrendas: "+data.offerings.Count+"   Aporte: "+data.contributions[i]+" / "+M01Progress.Quotas[i]+"   [I] Inventario   [T] Teléfono";}
    }
}
