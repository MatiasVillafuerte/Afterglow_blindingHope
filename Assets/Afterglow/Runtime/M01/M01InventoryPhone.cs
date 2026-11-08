using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Afterglow.M01
{
    public sealed class M01InventoryPhone : MonoBehaviour
    {
        public M01Dialogue dialogue;
        public AfterglowPlayer player;
        public Vector3 accessPosition = new Vector3(4, .05f, 0);
        public bool exploration = true;
        InputAction inventory, phone;
        bool showInventory, call;
        void OnEnable()
        {
            inventory = new InputAction("Inventory", InputActionType.Button, "<Keyboard>/i");
            phone = new InputAction("Phone", InputActionType.Button, "<Keyboard>/t");
            inventory.performed += _ => showInventory = true; phone.performed += _ => call = true;
            inventory.Enable(); phone.Enable();
        }
        void OnDisable() { inventory?.Dispose(); phone?.Dispose(); }
        void Update()
        {
            if (dialogue.IsOpen) { showInventory = call = false; return; }
            if(player.GetComponent<M01PlayerRules>().BlocksControl){showInventory=call=false;return;}
            var data = M01Session.Instance.data;
            if (showInventory)
            {
                showInventory = false;
                dialogue.BeginChoice("Dinero: "+data.money+". Selecciona una categoría.",new[]{"Ofrendas / claves","Consumibles","Encargos"},category=>{
                    if(category==0){
                        string items=string.Join("\n",data.offerings.GroupBy(item=>item.kind).Select(group=>group.Key+": "+group.Count()));
                        // Provenance is deliberately not exposed by the inventory presentation.
                        string phoneText=data.phoneOwned?dialogue.LookupText(data.worldClosed?"SYS2.02":"SYS2.01"):"Ningún objeto clave.";
                        dialogue.BeginText("Inventario","Ofrendas\n"+(items.Length>0?items:"Ninguna.")+"\n\nObjetos clave\n"+phoneText);
                    }else if(category==1)ShowConsumables();
                    else dialogue.BeginText("Encargos",data.quests.Count==0?"No tienes encargos.":string.Join("\n",data.quests.Select(q=>q.id+": "+(q.paid?"Entregado":q.active?"Activo · "+q.count:"Disponible")))+"\nObjetos: "+string.Join(", ",data.questItems));
                },"Inventario");
            }
            if (!call) return; call = false;
            if (!data.phoneOwned || player.Dead) return;
            if(player.GetComponent<M01PlayerRules>().restrictedActivity){dialogue.Begin(new[]{"SYS.05"});return;}
            if (data.escortActive) { dialogue.Begin(new[] { "N1D.07" }); return; }
            if (data.worldClosed || !exploration) { dialogue.Begin(new[] { "SYS2.06" }); return; }
            if (!player.Grounded || player.VerticalSpeed>.1f || !Physics.Raycast(player.transform.position+Vector3.up*.1f,Vector3.down,.4f,~0,QueryTriggerInteraction.Ignore)) return;
            var room=FindAnyObjectByType<M01Expedition>();
            if ((!room || room.roomId=="N01S01" || room.roomId=="M01_ACCESO") && Vector3.Distance(player.transform.position, room&&room.roomId=="N01S01"?M01Expedition.PhoneReturn:accessPosition) < 1) return;
            int index = Mathf.Clamp(data.currentLevel - 1, 0, 4);
            dialogue.BeginChoice(dialogue.LookupText(data.investigated[index] ? "N1D.05" : "N1D.04"), new[] { "Volver", "Seguir explorando" }, selected =>
            {
                if (selected != 0 || data.escortActive || data.worldClosed || !exploration || player.Dead) return;
                if (FindAnyObjectByType<M01Expedition>()) { M01Expedition.Travel("N01S01",M01Expedition.PhoneReturn,true,fade:true); return; }
                var controller = player.GetComponent<CharacterController>(); controller.enabled = false;
                player.transform.position = accessPosition; controller.enabled = true;
                player.ResetWorldOneBody(false); FindAnyObjectByType<AfterglowCamera>().Snap();
            });
        }
        void ShowConsumables()
        {
            var data=M01Session.Instance.data;
            dialogue.BeginChoice("Vida: "+data.Active.health+" / "+data.Active.maxHealth+". No se gastan objetos con la vida llena.",new[]{"Fragmento: "+data.fragments+" · +2","Núcleo: "+data.restorers+" · +5","Volver"},choice=>{
                if(choice==2)return;
                if(data.Active.health>=data.Active.maxHealth){dialogue.BeginText("Consumibles","Ya tienes la vida completa.");return;}
                if((choice==0?data.fragments:data.restorers)<=0){dialogue.BeginText("Consumibles","No tienes ese objeto.");return;}
                if(choice==0)data.fragments--;else data.restorers--;
                data.Active.health=Mathf.Min(data.Active.maxHealth,data.Active.health+(choice==0?2:5));player.SynchronizeWorldOneState();
                dialogue.BeginText("Consumibles","Vida recuperada: "+data.Active.health+" / "+data.Active.maxHealth+".");
            },"Consumibles");
        }
    }
}
