using UnityEngine;
using UnityEngine.InputSystem;
namespace Afterglow.M01
{
    public sealed class M01QuestNPC:MonoBehaviour
    {
        public M01Dialogue dialogue;
        public string questId="Q01", startLine="SEC.01", endLine="SEC.02";
        public string requiredItem;
        /// <summary>Encargos de colocación (Q03): se cumplen cuando todos estos eventos existen.</summary>
        public string[] requiredEvents;
        public string npcName="Minero";
        public int requiredCount=3, offeringReward=1, moneyReward=5;
        InputAction interact;
        bool requested;
        void OnEnable(){interact=new InputAction("Quest",InputActionType.Button,"<Keyboard>/e");interact.performed+=_=>{if(!dialogue.IsOpen)requested=true;};interact.Enable();}
        void OnDisable(){interact?.Dispose();}
        void Update()
        {
            if(!requested)return;requested=false;var player=FindAnyObjectByType<AfterglowPlayer>();
            if(!player||player.Dead||dialogue.IsOpen||Vector3.Distance(player.transform.position,transform.position)>2.4f)return;
            var data=M01Session.Instance.data;var quest=data.Quest(questId);
            if(quest.paid){dialogue.BeginText(npcName,"Gracias por la ayuda. Ya recibiste la recompensa de este encargo.");return;}
            if(!quest.active)
            {
                dialogue.Begin(new[]{startLine},()=>dialogue.BeginChoice("¿Aceptar el encargo "+questId+"?",new[]{"Aceptar","Ahora no"},choice=>{
                    if(choice!=0)return;quest.active=true;quest.count=0;quest.attempt=data.expeditionActive?data.expeditionAttempt:data.expeditionAttempt+1;
                },"Encargo "+questId));return;
            }
            bool complete=requiredEvents!=null&&requiredEvents.Length>0?System.Array.TrueForAll(requiredEvents,e=>data.events.Contains(e)):string.IsNullOrEmpty(requiredItem)?quest.count>=requiredCount:data.questItems.Contains(requiredItem);
            if(!complete){dialogue.BeginText("Encargo "+questId,requiredEvents!=null&&requiredEvents.Length>0?"Carros en su apartadero: "+System.Array.FindAll(requiredEvents,e=>data.events.Contains(e)).Length+" / "+requiredEvents.Length+".":string.IsNullOrEmpty(requiredItem)?"Progreso: "+quest.count+" / "+requiredCount+". Solo cuentan las acciones posteriores a aceptar.":"Busca el objeto y vuelve para entregarlo.");return;}
            quest.paid=true;quest.active=false;
            if(!string.IsNullOrEmpty(requiredItem))data.questItems.Remove(requiredItem);
            data.money+=moneyReward;
            for(int i=0;i<offeringReward;i++)data.CollectOffering(questId+".REWARD."+quest.attempt+"."+i,"Coca",false);
            dialogue.Begin(new[]{endLine});
        }
        public static void RecordKill(string enemyId)
        {
            var data=M01Session.Instance.data;string id=enemyId.StartsWith("N01.")?"Q01":enemyId.StartsWith("N04.")?"Q10":null;
            if(id==null)return;var quest=data.Quest(id);if(quest.active&&!quest.paid)quest.count++;
        }
    }
}
