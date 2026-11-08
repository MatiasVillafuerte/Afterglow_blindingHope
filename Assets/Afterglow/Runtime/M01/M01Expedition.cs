using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Afterglow.M01
{
    [Serializable] public sealed class M01EnemyState
    {
        public string id;
        public Vector3 position;
        public int health;
    }
    [Serializable] public sealed class M01LootState
    {
        public Vector3 position;
        public M01LootKind kind;
        public int amount;
        public float remaining;
    }
    [Serializable] public sealed class M01RoomState
    {
        public string id;
        public List<M01EnemyState> enemies = new List<M01EnemyState>();
        public List<M01LootState> loot = new List<M01LootState>();
    }

    // Captures only authored exploration rooms; source prototype scenes remain independent.
    public sealed class M01Expedition : MonoBehaviour
    {
        public string roomId;
        public bool safeArea;
        public int level = 1;
        public Material lootMaterial;
        static bool hasArrival;
        static Vector3 arrival;
        static Vector3 savedFacing;
        static bool fading;
        public static bool Transitioning { get; private set; }
        public static string ScenePath(string id) => "Assets/Afterglow/Scenes/M01/" + id + ".unity";
        public static readonly Vector3 MineEntrance = new Vector3(-1,.05f,-5.55f);
        public static readonly Vector3 PhoneReturn = new Vector3(3,.05f,-4.6f);
        public static Vector3 SectionEntrance(string id)
        {
            switch(id)
            {
                case "PRO_PATIO": return new Vector3(0,.05f,9.4f);
                case "N01S01": return MineEntrance;
                case "N01S02": return new Vector3(-8,.05f,-5.2f);
                case "N01S03": return new Vector3(6,.05f,-6.7f);
                case "N01S04": return new Vector3(5,.05f,-5.8f);
                default: return new Vector3(4,.05f,0);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetTravel() { hasArrival = Transitioning = fading = false;savedFacing=Vector3.zero; }

        IEnumerator Start()
        {
            yield return null;
            var data = M01Session.Instance.data;
            var state = data.rooms.Find(r => r.id == roomId);
            if (state != null)
            {
                foreach (var enemy in FindObjectsByType<AfterglowEnemy>())
                {
                    var saved = state.enemies.Find(e => e.id == enemy.name);
                    if (saved != null) enemy.RestoreWorldOneState(saved.health, saved.position);
                }
                foreach (var item in state.loot)
                {
                    var root = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Destroy(root.GetComponent<Collider>());
                    root.name = "Botín restaurado"; root.transform.position = item.position;
                    root.transform.localScale = Vector3.one * .3f;
                    root.GetComponent<Renderer>().sharedMaterial = lootMaterial;
                    var loot = root.AddComponent<M01Loot>(); loot.kind = item.kind;
                    loot.amount = item.amount; loot.remaining = item.remaining;
                }
            }
            var player = FindAnyObjectByType<AfterglowPlayer>();
            if (hasArrival)
            {
                var controller = player.GetComponent<CharacterController>(); controller.enabled = false;
                player.transform.position = arrival; controller.enabled = true; hasArrival = false;
                player.ResetWorldOneBody(false); FindAnyObjectByType<AfterglowCamera>().Snap();
                if(savedFacing.sqrMagnitude>.1f){player.RestoreWorldOneFacing(savedFacing);savedFacing=Vector3.zero;}
            }
            if (!safeArea) data.currentLevel = level;
            if(fading)
            {
                var fade=player.GetComponent<M01PlayerRules>().fade;
                if(fade){fade.alpha=1;for(float t=0;t<.3f;t+=Time.unscaledDeltaTime){fade.alpha=1-t/.3f;yield return null;}fade.alpha=0;}
                fading=false;Time.timeScale=1;
            }
            Transitioning = false;
        }
        public void Capture()
        {
            var data = M01Session.Instance.data;
            data.rooms.RemoveAll(r => r.id == roomId);
            var room = new M01RoomState { id = roomId };
            foreach (var enemy in FindObjectsByType<AfterglowEnemy>())
                room.enemies.Add(new M01EnemyState { id = enemy.name, health = enemy.Health, position = enemy.transform.position });
            foreach (var loot in FindObjectsByType<M01Loot>())
                if (loot.remaining > 0) room.loot.Add(new M01LootState { position = loot.transform.position, kind = loot.kind, amount = loot.amount, remaining = loot.remaining });
            data.rooms.Add(room);
        }
        public static void Travel(string destination, Vector3 position, bool leave = false, bool enter = false, bool fade = false)
        {
            if (Transitioning) return;
            var room = FindAnyObjectByType<M01Expedition>();
            if (room) room.Capture();
            var data = M01Session.Instance.data;
            if (leave)
            {
                data.expeditionActive = false;
                foreach (var state in data.rooms) state.loot.Clear();
            }
            if (enter && !data.expeditionActive)
            {
                data.expeditionAttempt++; data.expeditionActive = true;
                foreach(var quest in data.quests)if((quest.id=="Q01"||quest.id=="Q07"||quest.id=="Q10")&&quest.attempt!=data.expeditionAttempt){quest.active=quest.paid=false;quest.count=0;quest.attempt=data.expeditionAttempt;}
                data.rooms.RemoveAll(r => r.id.StartsWith("N"));
                data.collectedStaticIds.RemoveAll(id => id.StartsWith("EXP."));
            }
            arrival = position; hasArrival = true; Transitioning = true;
            if(fade&&room){fading=true;room.StartCoroutine(room.FadeOut(destination));}
            else SceneManager.LoadScene(ScenePath(destination));
        }
        IEnumerator FadeOut(string destination)
        {
            Time.timeScale=0;var player=FindAnyObjectByType<AfterglowPlayer>();var fade=player.GetComponent<M01PlayerRules>().fade;
            if(fade){fade.transform.SetAsLastSibling();for(float t=0;t<.3f;t+=Time.unscaledDeltaTime){fade.alpha=t/.3f;yield return null;}fade.alpha=1;}
            SceneManager.LoadScene(ScenePath(destination));
        }
        public static void RespawnLevel()
        {
            var data = M01Session.Instance.data;
            foreach (var room in data.rooms) { room.enemies.Clear(); room.loot.Clear(); }
            data.escortActive = false;
            data.preparationRemaining=0;
            data.Active.health = data.Active.maxHealth;
            string entrance="N"+data.currentLevel.ToString("00")+"S01";
            Travel(entrance,SectionEntrance(entrance));
            // Travel captures the dying room; discard its enemies as well.
            foreach (var room in data.rooms) { room.enemies.Clear(); room.loot.Clear(); }
        }
        public static void LoadSavedRoom(string path,Vector3 position,Vector3 facing)
        {
            arrival=position;savedFacing=facing;hasArrival=true;Transitioning=true;
            SceneManager.LoadScene(path);
        }
    }
}
