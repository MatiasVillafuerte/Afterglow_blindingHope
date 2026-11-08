using System;
using System.IO;
using System.Linq;
using Afterglow.M01;
using UnityEngine;
public static class N1CanonChecks
{
    public static string Main()
    {
        const string folder="Tools/PM/TestData/Canon05";
        Directory.CreateDirectory(folder);Directory.CreateDirectory("Docs/Implementacion/Nivel1");
        int failures=0;string report="# Migración de canon — pruebas aisladas\n\nNo se escribe la partida personal.\n\n";
        void Check(string label,bool value){report+=$"- {(value?"PASS":"FAIL")} {label}\n";if(!value)failures++;}
        for(int schema=1;schema<=3;schema++)
        {
            var original=new M01SaveFile{schema=schema,scene=M01Expedition.ScenePath("N01S01"),position=M01Expedition.MineEntrance,facing=Vector3.right};
            var d=original.progress=new M01Progress();d.money=37;d.experience=18;d.members[0].health=7;d.currentLevel=4;d.lastMapNode=5;
            d.contributions=new[]{3,5,7,9,6};d.investigated=new[]{true,true,true,true,false};d.opened=new[]{true,true,true,false,false};
            d.events.AddRange(new[]{"N4.04","N5.02","N1A.INTRO"});d.Quest("Q08").paid=true;d.Quest("Q10").count=2;
            d.collectedStaticIds.Add("EXP.N04S01.0");d.offerings.Add(new M01Offering{kind="Coca",sourceId="EXP.N05S02.0"});
            string path=folder+"/schema"+schema+".json";File.WriteAllText(path,JsonUtility.ToJson(original));string before=File.ReadAllText(path);
            bool read=M01Save.ReadFile(path,out var loaded,out var error);Check("Schema "+schema+" legible: "+error,read);if(!read)continue;
            var a=loaded.progress;
            Check("Investigación y foco por significado "+schema,!a.investigated[3]&&a.investigated[4]&&a.currentLevel==5&&a.lastMapNode==4);
            Check("Créditos, vida y recursos intactos "+schema,a.contributions.SequenceEqual(d.contributions)&&a.money==37&&a.experience==18&&a.members[0].health==7&&a.offerings.Count==1);
            Check("Misiones pagadas y contador intactos "+schema,a.Quest("Q08").paid&&a.Quest("Q10").count==2);
            Check("Fuentes y eventos equivalentes "+schema,a.collectedStaticIds.Contains("EXP.N05S01.0")&&a.offerings[0].sourceId=="EXP.N04S02.0"&&a.events.Contains("N5.04")&&a.events.Contains("N4.02"));
            Check("Derecho de acceso antiguo conservado "+schema,a.opened[2]&&a.opened[3]&&!a.opened[4]);
            Check("Lectura no sobrescribe original "+schema,File.ReadAllText(path)==before);
            string once=JsonUtility.ToJson(loaded);M01CanonMigration.Apply(loaded);Check("Idempotente "+schema,JsonUtility.ToJson(loaded)==once&&loaded.schema==5);
        }
        var future=new M01SaveFile{schema=3,scene=M01Expedition.ScenePath("N04S03"),progress=new M01Progress()};
        M01CanonMigration.Apply(future);Check("Escena antigua ausente vuelve a suelo seguro",future.scene==M01Expedition.ScenePath("N01S01")&&future.position==M01Expedition.MineEntrance);
        var fresh=new M01SaveFile{progress=new M01Progress()};M01CanonMigration.Apply(fresh);
        Check("Partida nueva no recibe excepciones",fresh.progress.opened.All(x=>!x)&&fresh.progress.events.Count==0);
        report+=$"\nFallos: {failures}\n";File.WriteAllText("Docs/Implementacion/Nivel1/Migracion_Pruebas.md",report);
        if(failures>0)throw new Exception(report);return report;
    }
}
