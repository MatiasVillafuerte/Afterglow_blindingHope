using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Afterglow.M01
{
    /// <summary>
    /// Árbol por personaje (GDD 8.2–8.3): ramas Técnicas de pico y Fundamentos. Coste por rango en gemas del personaje
    /// activo; requisitos; reembolso completo que retira también los descendientes (con confirmación). Compras y reembolsos
    /// solo en exploración fuera de combate (no durante jefe, minijuego ni escena). Se guarda en M01Member.skills.
    /// </summary>
    public static class M01SkillTree
    {
        public sealed class Node { public string id, name, effect, requires; public int[] costs; public bool technique; }
        public static readonly Node[] Catalog = {
            new Node { id = "ENLAZADOS", name = "Golpes enlazados", costs = new[] { 3, 4 }, effect = "Encadena 2 y luego 3 golpes más rápidos", technique = true },
            new Node { id = "MARCHA", name = "Pico en marcha", costs = new[] { 3 }, effect = "Atacar caminando al 70 % de velocidad", technique = true },
            new Node { id = "AEREO", name = "Golpe aéreo", costs = new[] { 3 }, effect = "Golpe frontal en el aire", technique = true },
            new Node { id = "BARRIDO", name = "Barrido frontal", costs = new[] { 4 }, requires = "ENLAZADOS", effect = "Mantener X 0,5 s amplía el golpe", technique = true },
            new Node { id = "GIRO", name = "Giro minero", costs = new[] { 5 }, requires = "BARRIDO", effect = "Z: giro de área; enfriamiento 3 s", technique = true },
            new Node { id = "FILO", name = "Filo firme", costs = new[] { 3, 5 }, effect = "+1 de daño por rango", technique = true },
            new Node { id = "PRECISO", name = "Golpe preciso", costs = new[] { 3, 4 }, requires = "FILO", effect = "Crítico 5 % y 10 %: daño doble", technique = true },
            new Node { id = "VITALIDAD", name = "Vitalidad", costs = new[] { 2, 3, 4 }, effect = "+2 de vida máxima por rango" },
            new Node { id = "PASO", name = "Paso seguro", costs = new[] { 2, 3 }, effect = "+5 % de velocidad por rango" },
            new Node { id = "SALTO", name = "Salto ligero", costs = new[] { 3, 4 }, effect = "+5 % de altura de salto por rango" },
            new Node { id = "ESQUIVA", name = "Esquiva breve", costs = new[] { 3 }, effect = "Shift: impulso corto; enfriamiento 2 s" },
            new Node { id = "OJO", name = "Ojo entrenado", costs = new[] { 2 }, effect = "Ver la vida de enemigos normales" },
            new Node { id = "RECUPERACION", name = "Recuperación", costs = new[] { 5 }, effect = "1 de vida cada 20 s tras 5 s sin daño" },
            new Node { id = "BOLSILLOS", name = "Bolsillos atentos", costs = new[] { 2, 3 }, effect = "+10 % de dinero de enemigos por rango" },
            new Node { id = "AFINIDAD", name = "Afinidad de EXP", costs = new[] { 3 }, effect = "Mayor radio de recogida de orbes" } };

        static M01Progress Data => M01Session.Instance.data;
        static M01Member Member => Data.Active;
        public static int Rank(string id) => Rank(Member, id);
        public static int Rank(M01Member m, string id)
        {
            if (m == null || m.skills == null) return 0;
            var e = m.skills.FirstOrDefault(s => s.StartsWith(id + ":")); return e == null ? 0 : int.Parse(e.Substring(id.Length + 1));
        }
        static void SetRank(string id, int rank) { Member.skills ??= new List<string>(); Member.skills.RemoveAll(s => s.StartsWith(id + ":")); if (rank > 0) Member.skills.Add(id + ":" + rank); }

        static bool CanEdit(out string reason)
        {
            var player = Object.FindAnyObjectByType<AfterglowPlayer>(); var rules = player ? player.GetComponent<M01PlayerRules>() : null;
            reason = M01PlayerRules.SaveBlocked ? "No se puede cambiar el árbol durante el jefe o un minijuego." : rules && (rules.Recovering || rules.restrictedActivity) ? "Espera a estar fuera de combate." : null;
            return reason == null;
        }

        public static void Open(M01Dialogue dialogue, bool techniques = true)
        {
            string who = Data.activeMember == 0 ? "La bolivianita" : "Platicio";
            var branch = Catalog.Where(n => n.technique == techniques).ToArray();
            var options = branch.Select(n => { int r = Rank(n.id); string next = r < n.costs.Length ? " · " + n.costs[r] + " gemas" : " · máximo"; return n.name + " " + r + "/" + n.costs.Length + next; }).ToList();
            options.Add(techniques ? "Ver Fundamentos" : "Ver Técnicas de pico"); options.Add("Reembolsar…"); options.Add("Volver");
            string title = who + " · " + (techniques ? "Técnicas de pico" : "Fundamentos");
            dialogue.BeginChoice("Gemas disponibles: " + Member.gems + ". Elige un nodo para mejorarlo. Muestra coste, rango y requisito.", options.ToArray(), choice =>
            {
                if (choice == branch.Length) { Open(dialogue, !techniques); return; }
                if (choice == branch.Length + 1) { Refund(dialogue); return; }
                if (choice > branch.Length + 1) return;
                Buy(dialogue, branch[choice], techniques);
            }, title);
        }

        static void Buy(M01Dialogue dialogue, Node n, bool techniques)
        {
            if (!CanEdit(out var reason)) { dialogue.BeginText("Habilidades", reason); return; }
            int r = Rank(n.id);
            if (r >= n.costs.Length) { dialogue.BeginText(n.name, n.effect + ". Ya está al máximo."); return; }
            if (n.requires != null && Rank(n.requires) == 0) { dialogue.BeginText(n.name, "Requiere " + Catalog.First(c => c.id == n.requires).name + " 1."); return; }
            int cost = n.costs[r];
            if (Member.gems < cost) { dialogue.BeginText(n.name, n.effect + ". Cuesta " + cost + " gemas; tienes " + Member.gems + "."); return; }
            dialogue.BeginChoice(n.name + " " + (r + 1) + "/" + n.costs.Length + ": " + n.effect + ". Coste " + cost + " gemas.", new[] { "Aprender", "Cancelar" }, c =>
            {
                if (c != 0) return; Member.gems -= cost; SetRank(n.id, r + 1); Apply();
                M01Toast.Message(n.name + " " + (r + 1) + "/" + n.costs.Length, 2.2f);
            }, "Habilidades");
        }

        static IEnumerable<Node> Descendants(string id) { foreach (var c in Catalog.Where(c => c.requires == id)) { yield return c; foreach (var d in Descendants(c.id)) yield return d; } }

        static void Refund(M01Dialogue dialogue)
        {
            if (!CanEdit(out var reason)) { dialogue.BeginText("Habilidades", reason); return; }
            var owned = Catalog.Where(n => Rank(n.id) > 0).ToArray();
            if (owned.Length == 0) { dialogue.BeginText("Reembolso", "No hay nodos aprendidos."); return; }
            dialogue.BeginChoice("¿Qué nodo retiras? El reembolso devuelve el coste completo.", owned.Select(n => n.name + " " + Rank(n.id)).Append("Volver").ToArray(), choice =>
            {
                if (choice >= owned.Length) return;
                var n = owned[choice]; var chain = new[] { n }.Concat(Descendants(n.id).Where(d => Rank(d.id) > 0)).ToArray();
                int total = chain.Sum(x => x.costs.Take(Rank(x.id)).Sum());
                string list = string.Join(", ", chain.Select(x => x.name));
                dialogue.BeginChoice("Se retirarán: " + list + ". Recuperas " + total + " gemas.", new[] { "Reembolsar", "Cancelar" }, c =>
                {
                    if (c != 0) return; foreach (var x in chain) SetRank(x.id, 0); Member.gems += total; Apply();
                    M01Toast.Message("Reembolso: " + total + " gemas", 2.2f);
                }, "Reembolso");
            }, "Reembolso");
        }

        /// <summary>Recalcula vida máxima (Vitalidad) y estado del jugador sin curar.</summary>
        public static void Apply()
        {
            foreach (var m in Data.members)
            {
                int baseHealth = m.id == "HIJO" ? 14 : 10;
                m.maxHealth = baseHealth + 2 * Rank(m, "VITALIDAD");
                m.health = Mathf.Min(m.health, m.maxHealth);
            }
            var player = Object.FindAnyObjectByType<AfterglowPlayer>(); if (player) player.SynchronizeWorldOneState();
        }
    }
}
