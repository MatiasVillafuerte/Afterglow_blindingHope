using System.IO;
using Afterglow.M01;
public static class PMProgressChecks
{
 public static string Main()
 {
  string report="# PM11/17: fixtures aislados de progreso\n\nSin alterar la partida activa ni escribir su guardado.\n\n";int failures=0;
  for(int index=0;index<5;index++)for(int flags=0;flags<4;flags++)
  {
   var data=new M01Progress();data.events.AddRange(new[]{"P02.CAPTAIN","M01.WORLD_STARTED"});data.investigated[index]=(flags&1)!=0;data.contributions[index]=(flags&2)!=0?M01Progress.Quotas[index]:0;
   bool opened=data.TryOpen(index),expected=flags==3;bool pass=opened==expected&&M01RouteMap.Available(data,index+1)==expected&&!data.TryOpen(index);
   report+="- N"+(index+1)+" cuota "+M01Progress.Quotas[index]+", investigar="+((flags&1)!=0)+", cuota="+((flags&2)!=0)+": "+(pass?"PASS":"FAIL")+"\n";if(!pass)failures++;
   if(expected){data.offerings.Clear();if(!M01RouteMap.Available(data,index+1))failures++;}
  }
  var credits=new M01Progress();credits.investigated[0]=true;for(int i=0;i<4;i++)credits.CollectOffering("FIXTURE."+i,"Coca",false);credits.Offer(0);bool credit=credits.contributions[0]==3&&credits.offerings.Count==1&&credits.TryOpen(0);credits.SpendCoca(1);credit&=credits.contributions[0]==3&&credits.opened[0];report+="- Gastar inventario no revierte credito ni apertura: "+(credit?"PASS":"FAIL")+"\n";if(!credit)failures++;
  report+="\nFallos: "+failures+"\n";File.WriteAllText("Docs/Implementacion/PM/Pruebas_Progreso.md",report);return report;
 }
}
