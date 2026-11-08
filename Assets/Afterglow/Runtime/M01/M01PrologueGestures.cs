using UnityEngine;
namespace Afterglow.M01
{
    // Minimal silent responses, limited to P02. The sprite rig and its clips remain intact.
    public sealed class M01PrologueGestures:MonoBehaviour
    {
        AfterglowPlayer player;M01Dialogue dialogue;Quaternion initial;bool posing;
        void Start(){player=FindAnyObjectByType<AfterglowPlayer>();dialogue=FindAnyObjectByType<M01Dialogue>();}
        void LateUpdate()
        {
            bool active=dialogue&&dialogue.IsOpen&&(dialogue.CurrentId=="P02.04"||dialogue.CurrentId=="P02.07");
            if(active){if(!posing){initial=player.visual.localRotation;posing=true;}float angle=Mathf.Sin(Time.unscaledTime*5)*6;player.visual.localRotation=initial*Quaternion.Euler(0,0,angle);}
            else if(posing){player.visual.localRotation=initial;posing=false;}
        }
    }
}
