using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Afterglow.M01
{
    [Serializable] public sealed class M01Offering
    {
        public string kind, sourceId;
        public bool stolen;
    }
    [Serializable] public sealed class M01Member
    {
        public string id;
        public bool unlocked;
        public int health, maxHealth, gems;
        /// <summary>Árbol de habilidades: "ID:rango" (M01SkillTree).</summary>
        public List<string> skills = new List<string>();
    }
    [Serializable] public sealed class M01QuestState
    {
        public string id;
        public bool active, paid;
        public int count, attempt;
    }
    [Serializable] public sealed class M01Progress
    {
        public int teamLevel = 1, experience, historicGems, money, activeMember;
        public int[] contributions = new int[5];
        public bool[] investigated = new bool[5];
        public bool[] opened = new bool[5];
        public bool phoneOwned, escortActive, worldClosed, fraudDiscovered;
        public List<M01Offering> offerings = new List<M01Offering>();
        public List<string> collectedStaticIds = new List<string>();
        public M01Member[] members = {
            new M01Member { id = "BOLIVIANITA", unlocked = true, health = 10, maxHealth = 10 },
            new M01Member { id = "HIJO", health = 14, maxHealth = 14 }
        };
        public M01Member Active => members[activeMember];
        public static readonly int[] Quotas = { 3, 5, 7, 9, 12 };
        public int currentLevel = 1;
        public int lastMapNode=1;
        public bool pickaxeOwned, expeditionActive;
        public int expeditionAttempt;
        public List<string> events = new List<string>();
        public List<M01RoomState> rooms = new List<M01RoomState>();
        public List<M01QuestState> quests = new List<M01QuestState>();
        public List<string> questItems = new List<string>();
        public List<int> hintsRead = new List<int>();
        public int fragments, restorers;
        public string preparation;
        public float preparationRemaining;
        /// <summary>Adquisición confirmada por el inventario: nombre visible, cantidad y si es objeto clave.</summary>
        public static event Action<string, int, bool> Acquired;
        public static void Announce(string displayName, int count = 1, bool keyItem = false) { if (count > 0) Acquired?.Invoke(displayName, count, keyItem); }
        public static string OfferingName(string kind) => kind == "Coca" ? "Hoja de coca" : kind;
        public static string KeyItemName(string id) => id == "PICO" ? "Pico" : id == "TELEFONO" ? "Teléfono de minas" : id == "PIEZA" ? "Pieza dorada del taladro" : id;
        /// <summary>Entrega única de un objeto clave. Devuelve false si ya se poseía (no repite la presentación).</summary>
        public bool GrantKeyItem(string id)
        {
            bool owned = id == "PICO" ? pickaxeOwned : id == "TELEFONO" ? phoneOwned : events.Contains("KEY." + id);
            if (owned) return false;
            if (id == "PICO") pickaxeOwned = true; else if (id == "TELEFONO") phoneOwned = true; else events.Add("KEY." + id);
            Announce(KeyItemName(id), 1, true);
            return true;
        }
        public void AddDroppedOffering(string kind = "Coca")
        {
            offerings.Add(new M01Offering { kind = kind, sourceId = "DROP." + Guid.NewGuid().ToString("N"), stolen = false });
            Announce(OfferingName(kind));
        }
        public M01QuestState Quest(string id)
        {
            var quest=quests.Find(q=>q.id==id);
            if(quest==null){quest=new M01QuestState{id=id};quests.Add(quest);}return quest;
        }
        public bool SpendCoca(int count)
        {
            var coca=offerings.Where(o=>o.kind=="Coca").Take(count).ToList();
            if(coca.Count<count)return false;foreach(var leaf in coca)offerings.Remove(leaf);return true;
        }
        public void AddExperience(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            experience += amount;
            while (experience >= 100)
            {
                experience -= 100; teamLevel++; historicGems += 3;
                foreach (var member in members) if (member.unlocked) member.gems += 3;
                Active.health = Active.maxHealth;
            }
        }
        public void UnlockSon()
        {
            if (members[1].unlocked) return;
            members[1].unlocked = true; members[1].gems = historicGems;
        }
        public bool CollectOffering(string sourceId, string kind, bool stolen)
        {
            if (collectedStaticIds.Contains(sourceId)) return false;
            collectedStaticIds.Add(sourceId);
            offerings.Add(new M01Offering { sourceId = sourceId, kind = kind, stolen = stolen });
            Announce(OfferingName(kind));
            return true;
        }
        public M01AltarResult Offer(int index)
        {
            if (index < 0 || index >= Quotas.Length) throw new ArgumentOutOfRangeException(nameof(index));
            var result = new M01AltarResult();
            if (worldClosed) { result.lineId = "ALT.12"; return result; }
            int missing = Math.Max(0, Quotas[index] - contributions[index]);
            if (missing == 0) { result.lineId = "ALT.09"; result.canOpen = investigated[index] && !opened[index]; return result; }
            var legitimate = offerings.Where(item => !item.stolen).ToList();
            int count = Math.Min(missing, legitimate.Count);
            for (int i = 0; i < count; i++) offerings.Remove(legitimate[i]);
            contributions[index] += count; result.accepted = count;
            if (count < missing)
            {
                result.rejected = offerings.RemoveAll(item => item.stolen);
                if (result.rejected > 0) fraudDiscovered = true;
            }
            result.missing = Quotas[index] - contributions[index];
            result.canOpen = result.missing == 0 && investigated[index] && !opened[index];
            result.lineId = result.rejected > 0 ? (count > 0 ? "ALT.07" : "ALT.05") : result.missing > 0 ? (count > 0 ? "ALT.02" : "ALT.01") : offerings.Count > 0 ? "ALT.08" : "ALT.03";
            return result;
        }
        public bool TryOpen(int index)
        {
            if (worldClosed || opened[index] || !investigated[index] || contributions[index] < Quotas[index]) return false;
            opened[index] = true; return true;
        }
    }
    public sealed class M01AltarResult { public string lineId; public int accepted, rejected, missing; public bool canOpen; }
}
