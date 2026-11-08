using System.Collections;
using UnityEngine;
namespace Afterglow.M01
{
    public sealed class M01Prologue:MonoBehaviour
    {
        public M01Dialogue dialogue;
        IEnumerator Start()
        {
            yield return null;
            while(M01Expedition.Transitioning)yield return null;
            if(M01Session.Instance.data.events.Contains("P01.LETTER"))yield break;
            var rect=(RectTransform)dialogue.panel.transform;
            var hidden=new System.Collections.Generic.List<GameObject>();
            foreach(Transform child in rect.parent)if(child!=rect && child.name!="Fade" && child.gameObject.activeSelf){hidden.Add(child.gameObject);child.gameObject.SetActive(false);}
            var position=rect.anchoredPosition;var size=rect.sizeDelta;
            rect.anchoredPosition=new Vector2(0,60);rect.sizeDelta=new Vector2(1180,600);
            dialogue.Begin(new[]{"P01.02","P01.03"},()=>{
                M01Session.Instance.data.events.Add("P01.LETTER");
                rect.anchoredPosition=position;rect.sizeDelta=size;
                foreach(var child in hidden)if(child)child.SetActive(true);
                StartCoroutine(SetDownLetter());
            });
        }
        IEnumerator SetDownLetter()
        {
            var player=FindAnyObjectByType<AfterglowPlayer>();
            var rules=player.GetComponent<M01PlayerRules>();rules.scriptedControl=true;
            var visual=player.visual;var origin=visual.localPosition;
            for(float t=0;t<.7f;t+=Time.unscaledDeltaTime){visual.localPosition=origin+Vector3.down*(Mathf.Sin(t/.7f*Mathf.PI)*.12f);yield return null;}
            visual.localPosition=origin;
            var data=M01Session.Instance.data;
            if(!data.events.Contains("P01.BELL")){data.events.Add("P01.BELL");RingBell();M01Toast.Message(dialogue.LookupText("P01.04")??"Alguien tocó la puerta");}
            rules.scriptedControl=false;
        }
        void RingBell()
        {
            const int rate=22050;var samples=new float[rate];
            for(int i=0;i<samples.Length;i++){float t=(float)i/rate;float f=t<.42f?880:660;float local=t<.42f?t:t-.42f;samples[i]=Mathf.Sin(t*f*Mathf.PI*2)*.15f*Mathf.Exp(-local*7);}
            var clip=AudioClip.Create("Afterglow doorbell",rate,1,rate,false);clip.SetData(samples,0);
            var source=gameObject.AddComponent<AudioSource>();source.clip=clip;source.Play();
        }
    }
}
