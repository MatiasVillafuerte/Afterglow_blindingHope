using TMPro;
using UnityEngine;
namespace Afterglow.M01
{
    [RequireComponent(typeof(TextMeshPro))]
    public sealed class M01WorldHint:MonoBehaviour
    {
        TextMeshPro label;Camera view;AfterglowPlayer player;
        void Start(){label=GetComponent<TextMeshPro>();view=Camera.main;player=FindAnyObjectByType<AfterglowPlayer>();}
        void LateUpdate()
        {
            if(!view||!player)return;
            var delta=transform.position-player.transform.position;delta.y=0;
            var screen=view.WorldToViewportPoint(transform.position);
            label.enabled=delta.magnitude<6&&screen.z>0&&screen.y<.72f&&screen.y>.12f;
            transform.localScale=Vector3.one*Mathf.Clamp(screen.z/12,.25f,1.1f);
        }
    }
}
