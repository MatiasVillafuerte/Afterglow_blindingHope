using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Afterglow.M01
{
    public enum M01Service { Shop, Reader, Vendor }
    public sealed class M01ServiceNPC:MonoBehaviour
    {
        public M01Dialogue dialogue;
        public M01Service service;
        InputAction interact;
        bool requested;
        M01Progress Data=>M01Session.Instance.data;
        void OnEnable(){interact=new InputAction("Service",InputActionType.Button,"<Keyboard>/e");interact.performed+=_=>{if(!dialogue.IsOpen)requested=true;};interact.Enable();}
        void OnDisable(){interact?.Dispose();}
        void Update()
        {
            if(!requested)return;requested=false;var player=FindAnyObjectByType<AfterglowPlayer>();
            if(!player||player.Dead||dialogue.IsOpen||Vector3.Distance(player.transform.position,transform.position)>2.4f)return;
            if(service==M01Service.Shop)dialogue.Begin(new[]{"PUE.12"},Shop);
            else if(service==M01Service.Reader)dialogue.Begin(new[]{"PUE.07"},Reader);
            else dialogue.Begin(new[]{Data.fraudDiscovered?"PUE.04":"PUE.01"},Vendor);
        }
        void Message(string title,string text)=>dialogue.BeginText(title,text);
        void Shop()
        {
            dialogue.BeginChoice("Dinero disponible: "+Data.money+". Los objetos se usan desde el inventario.",new[]{"Fragmento · 5","Núcleo · 12","Volver"},choice=>{
                if(choice==2)return;int price=choice==0?5:12;
                if(Data.money<price){Message("Tienda","No tienes suficiente dinero.");return;}
                Data.money-=price;if(choice==0)Data.fragments++;else Data.restorers++;
                Message("Tienda","Objeto añadido al inventario.");
            },"Habitante del pueblo");
        }
        void Vendor()
        {
            dialogue.BeginChoice("Comprar paquete de 3 ofrendas por 8 de dinero. Dinero: "+Data.money+".",new[]{"Comprar","No, gracias"},choice=>{
                if(choice!=0){dialogue.Begin(new[]{"PUE.03"});return;}
                if(Data.money<8){Message("Habitante del pueblo","No tienes suficiente dinero.");return;}
                Data.money-=8;for(int i=0;i<3;i++)Data.offerings.Add(new M01Offering{kind=i%2==0?"Alcohol":"Cigarro",sourceId="VENDOR."+System.Guid.NewGuid().ToString("N"),stolen=true});
                Message("Habitante del pueblo","Recibiste tres ofrendas.");
            },"Habitante del pueblo");
        }
        void Reader()
        {
            dialogue.BeginChoice("Hojas de coca: "+Data.offerings.Count(o=>o.kind=="Coca")+". Preparación actual: "+(Data.preparationRemaining>0?Data.preparation+" · "+Mathf.CeilToInt(Data.preparationRemaining)+" s":"ninguna")+".",new[]{"Pista · 1 hoja","Preparación · 2 hojas","Volver"},choice=>{
                if(choice==0)Hint();else if(choice==1)Preparation();
            },"Lector de coca");
        }
        void Hint()
        {
            int level=Data.currentLevel;
            if(Data.worldClosed||Data.investigated[Mathf.Clamp(level-1,0,4)]||Data.hintsRead.Contains(level)){dialogue.Begin(new[]{"PUE.10"});return;}
            if(!Data.SpendCoca(1)){Message("Lector de coca","Necesitas una hoja de coca.");return;}
            Data.hintsRead.Add(level);
            string[] hints={"Los escalones del primer tramo llevan a una ofrenda. No necesitas ninguna mejora para alcanzarla.",dialogue.LookupText("PUE.09"),"Sigue la lámpara encendida, después el riel continuo y por último baja entre los dos postes.","La vía rota anuncia un salto. El dinero dibuja la trayectoria segura.","Acerca el carro con lámpara a la marca del cruce para reconocer la salida.","Espera a que el taladro quede atascado. Entonces podrás golpear sus resistencias."};
            Message("Lector de coca",hints[Mathf.Clamp(level-1,0,5)]);
        }
        void Preparation()
        {
            dialogue.BeginChoice("Duración: 90 segundos de juego. Sustituye la preparación anterior; no consume tiempo durante conversaciones.",new[]{"Velocidad +10 %","Intervalo −10 %","Cancelar"},choice=>{
                if(choice==2)return;
                if(Data.preparationRemaining>0)dialogue.BeginChoice("¿Sustituir la preparación actual por dos hojas de coca?",new[]{"Sustituir","Cancelar"},confirm=>{if(confirm==0)ApplyPreparation(choice);},"Lector de coca");
                else ApplyPreparation(choice);
            },"Lector de coca");
        }
        void ApplyPreparation(int kind)
        {
            if(!Data.SpendCoca(2)){Message("Lector de coca","Necesitas dos hojas de coca.");return;}
            Data.preparation=kind==0?"Velocidad":"Intervalo";Data.preparationRemaining=90;
            dialogue.Begin(new[]{"PUE.11"});
        }
    }
}
