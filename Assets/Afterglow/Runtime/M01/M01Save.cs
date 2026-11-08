using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Afterglow.M01
{
    [Serializable] public sealed class M01SaveFile
    {
        public int schema=5;
        public string scene;
        public Vector3 position, facing;
        public M01Progress progress;
    }
    public static class M01Save
    {
        public static string Path=>System.IO.Path.Combine(Application.persistentDataPath,"afterglow-m01.json");
        public static bool Allowed
        {
            get
            {
                var player=UnityEngine.Object.FindAnyObjectByType<AfterglowPlayer>();
                var rules=player?player.GetComponent<M01PlayerRules>():null;
                var dialogue=UnityEngine.Object.FindAnyObjectByType<M01Dialogue>();
                return player&&rules&&!player.Dead&&player.Grounded&&!rules.Recovering&&!rules.scriptedControl&&!rules.restrictedActivity&&!M01Expedition.Transitioning&&(!dialogue||!dialogue.IsOpen);
            }
        }
        public static bool TryWrite(out string error,string file=null)
        {
            error=null;
            if(!Allowed){error="El guardado no está disponible en este momento.";return false;}
            var player=UnityEngine.Object.FindAnyObjectByType<AfterglowPlayer>();
            var room=UnityEngine.Object.FindAnyObjectByType<M01Expedition>();if(room)room.Capture();
            var save=new M01SaveFile{scene=SceneManager.GetActiveScene().path,position=player.transform.position,facing=player.Facing,progress=M01Session.Instance.data};
            return WriteFile(save,file??Path,out error);
        }
        public static bool WriteFile(M01SaveFile save,string file,out string error)
        {
            error=null;
            try
            {
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(file));
                string temporary=file+".tmp";
                File.WriteAllText(temporary,JsonUtility.ToJson(save,true),new UTF8Encoding(false));
                // Validate a complete temporary file before replacing the last valid save.
                if(!ReadFile(temporary,out _,out error))return false;
                if(File.Exists(file))File.Replace(temporary,file,file+".bak");else File.Move(temporary,file);
                return true;
            }
            catch(Exception exception)when(exception is IOException||exception is UnauthorizedAccessException||exception is ArgumentException)
            {error=exception.GetType().Name+": "+exception.Message;return false;}
        }
        public static bool ReadFile(string file,out M01SaveFile save,out string error)
        {
            save=null;error=null;
            try
            {
                if(!File.Exists(file)){error="No hay una partida guardada.";return false;}
                save=JsonUtility.FromJson<M01SaveFile>(File.ReadAllText(file));
                if(save==null||save.schema<1||save.schema>5||save.progress==null||save.progress.members==null||save.progress.members.Length!=2||save.progress.contributions==null||save.progress.contributions.Length!=5||save.progress.investigated==null||save.progress.investigated.Length!=5||save.progress.opened==null||save.progress.opened.Length!=5||string.IsNullOrEmpty(save.scene)||!save.scene.StartsWith("Assets/Afterglow/Scenes/M01/",StringComparison.Ordinal))
                {error="La partida es incompatible o está incompleta.";save=null;return false;}
                if(save.progress.activeMember<0||save.progress.activeMember>=save.progress.members.Length||save.progress.members[save.progress.activeMember]==null||save.progress.members[save.progress.activeMember].maxHealth<=0){error="Datos de personaje incompatibles.";save=null;return false;}
                Migrate(save);
                MigrateN1Layout(save);
                if(!Application.CanStreamedLevelBeLoaded(save.scene)){error="La sección guardada no está disponible.";save=null;return false;}
                return true;
            }
            catch(Exception exception)when(exception is IOException||exception is UnauthorizedAccessException||exception is ArgumentException)
            {error=exception.GetType().Name+": "+exception.Message;return false;}
        }
        static void Migrate(M01SaveFile save)
        {
            var d=save.progress;
            d.events??=new System.Collections.Generic.List<string>();d.rooms??=new System.Collections.Generic.List<M01RoomState>();d.quests??=new System.Collections.Generic.List<M01QuestState>();d.questItems??=new System.Collections.Generic.List<string>();d.hintsRead??=new System.Collections.Generic.List<int>();d.offerings??=new System.Collections.Generic.List<M01Offering>();d.collectedStaticIds??=new System.Collections.Generic.List<string>();
            d.lastMapNode=Mathf.Clamp(d.lastMapNode,1,6);
            // Only an older schema needs inference. New saves preserve their exact prologue phase.
            if(save.schema==1)
            {
                bool entered=d.events.Contains("N1A.INTRO")||d.phoneOwned||d.pickaxeOwned||save.scene.Contains("/N01S")||d.currentLevel>1||Array.Exists(d.investigated,b=>b)||Array.Exists(d.opened,b=>b);
                bool started=entered||d.events.Contains("P02.CAPTAIN");
                if(started){Flag(d,"P02.CAPTAIN");Flag(d,"M01.WORLD_STARTED");}
                if(entered)Flag(d,"M01.N01_ENTERED");
                if(started||save.scene.EndsWith("/PRO_PATIO.unity",StringComparison.Ordinal))Flag(d,"P01.LETTER");
                if(d.events.Contains("P01.LETTER"))Flag(d,"P01.BELL");
            }
            if(save.schema>=3){M01CanonMigration.Apply(save);return;}

            // Pre-remodel coordinates refer to a different floor plan. Keep progress and
            // the actual section, relocating only positions that belong to the old plan.
            string section=System.IO.Path.GetFileNameWithoutExtension(save.scene);
            if(section=="M01_ACCESO")
            {
                if(d.events.Contains("M01.N01_ENTERED")||d.worldClosed)section="N01S01";
                else section=d.events.Contains("M01.WORLD_STARTED")?"M01_MAPA":"PRO_PATIO";
            }
            if(section=="PRO_PATIO"||section.StartsWith("N01S",StringComparison.Ordinal)||section=="M01_MAPA")
            {
                save.scene=M01Expedition.ScenePath(section);
                save.position=M01Expedition.SectionEntrance(section);
            }
            foreach(var room in d.rooms)
            {
                if(room.id=="M01_ACCESO")room.id="N01S01";
                if(!room.id.StartsWith("N01S",StringComparison.Ordinal))continue;
                var entrance=M01Expedition.SectionEntrance(room.id);
                if(room.loot!=null)for(int i=0;i<room.loot.Count;i++)room.loot[i].position=entrance+new Vector3(i%3*.38f,.3f,1.25f+i/3*.38f);
                if(room.enemies!=null)foreach(var enemy in room.enemies)
                {
                    if(enemy.id=="N01.TUTORIAL.RESISTANCE")enemy.position=new Vector3(1,.05f,0);
                    if(enemy.id=="N01.RESISTANCE.0")enemy.position=new Vector3(-5,.05f,3);
                    if(enemy.id=="N01.RESISTANCE.1")enemy.position=new Vector3(-5,.05f,8);
                }
            }
            save.schema=3;
            M01CanonMigration.Apply(save);
        }
        static void Flag(M01Progress d,string flag){if(!d.events.Contains(flag))d.events.Add(flag);}
        static void MigrateN1Layout(M01SaveFile save)
        {
            if(save.schema>=5)return;
            string section=System.IO.Path.GetFileNameWithoutExtension(save.scene);
            if(section.StartsWith("N01S",StringComparison.Ordinal))save.position=M01Expedition.SectionEntrance(section);
            foreach(var room in save.progress.rooms)
            {
                if(!room.id.StartsWith("N01S",StringComparison.Ordinal))continue;
                var entry=M01Expedition.SectionEntrance(room.id);
                if(room.loot!=null)for(int i=0;i<room.loot.Count;i++)room.loot[i].position=entry+new Vector3(i%3*.38f,.3f,1.25f+i/3*.38f);
                if(room.enemies!=null)foreach(var enemy in room.enemies)
                {
                    if(enemy.id=="N01.TUTORIAL.RESISTANCE")enemy.position=new Vector3(1,.05f,0);
                    if(enemy.id=="N01.RESISTANCE.0")enemy.position=new Vector3(-5,.05f,3);
                    if(enemy.id=="N01.RESISTANCE.1")enemy.position=new Vector3(-5,.05f,8);
                }
            }
            save.schema=5;
        }
        public static bool Continue(out string error,string file=null)
        {
            if(!ReadFile(file??Path,out var save,out error))return false;
            M01Session.Instance.data=save.progress;
            M01Expedition.LoadSavedRoom(save.scene,save.position,save.facing);
            return true;
        }
    }
}
