using UnityEngine;
using TMPro;
namespace Afterglow {
public class AfterglowHUD:MonoBehaviour {
 public AfterglowPlayer player;public TMP_Text health,experience,message;public RectTransform fill;
 void Update(){health.text=player.Health+" / "+player.maxHealth;experience.text="EXP: "+player.Experience+(player.GetComponent<Afterglow.M01.M01PlayerRules>()?" / 100":"");fill.anchorMax=new Vector2((float)player.Health/player.maxHealth,1);message.text=player.Dead?"Has caído · Reiniciando…":"";}
}
}
