using System;
using UnityEngine;
namespace Afterglow.M01
{
    public static class M01CanonMigration
    {
        public static int Level(int oldLevel)=>oldLevel==4?5:oldLevel==5?4:oldLevel;
        public static string ContentId(string id)
        {
            if(id==null)return null;
            foreach(var pair in new[]{new[]{"N04S","N05S"},new[]{"N04.","N05."},new[]{"M01N04","M01N05"},new[]{"N4.","N5."}})
            {
                if(id.StartsWith(pair[0],StringComparison.Ordinal))return pair[1]+id.Substring(pair[0].Length);
                if(id.StartsWith(pair[1],StringComparison.Ordinal))return pair[0]+id.Substring(pair[1].Length);
            }
            return id;
        }
        public static void Apply(M01SaveFile save)
        {
            if(save.schema>=4)return;
            var d=save.progress;
            bool cartAccess=d.opened[2];
            bool cartInvestigated=d.investigated[3];
            d.investigated[3]=d.investigated[4];d.investigated[4]=cartInvestigated;
            // Paid credits are ordinal ledger entries, not room identities.
            // Preserve an already earned cart access at its new position, once.
            if(cartAccess)d.opened[3]=true;
            d.currentLevel=Level(d.currentLevel);d.lastMapNode=Level(d.lastMapNode);
            for(int i=0;i<d.events.Count;i++)d.events[i]=ContentId(d.events[i]);
            for(int i=0;i<d.collectedStaticIds.Count;i++)
            {
                string id=d.collectedStaticIds[i];
                d.collectedStaticIds[i]=id.StartsWith("EXP.",StringComparison.Ordinal)?"EXP."+ContentId(id.Substring(4)):ContentId(id);
            }
            foreach(var offering in d.offerings)
            {
                string id=offering.sourceId;
                offering.sourceId=id!=null&&id.StartsWith("EXP.",StringComparison.Ordinal)?"EXP."+ContentId(id.Substring(4)):ContentId(id);
            }
            for(int i=0;i<d.hintsRead.Count;i++)d.hintsRead[i]=Level(d.hintsRead[i]);
            foreach(var room in d.rooms)
            {
                room.id=ContentId(room.id);
                if(room.enemies!=null)foreach(var enemy in room.enemies)enemy.id=ContentId(enemy.id);
            }
            var section=System.IO.Path.GetFileNameWithoutExtension(save.scene);
            if(section.StartsWith("N04S",StringComparison.Ordinal)||section.StartsWith("N05S",StringComparison.Ordinal))
            {
                string path=M01Expedition.ScenePath(ContentId(section));
                if(Application.CanStreamedLevelBeLoaded(path))save.scene=path;
                else {save.scene=M01Expedition.ScenePath("N01S01");save.position=M01Expedition.MineEntrance;}
            }
            if(!d.events.Contains("CANON.V05.MIGRATED"))d.events.Add("CANON.V05.MIGRATED");
            save.schema=4;
        }
    }
}
