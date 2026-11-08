using UnityEngine;

namespace Afterglow.M01
{
    /// <summary>
    /// Regalo único de Platicio por nivel (GDD 8.4): 25 % una ofrenda, 25 % 5 de dinero, 50 % sin regalo.
    /// El resultado se registra aunque no haya regalo y no se vuelve a tirar al conversar, morir o recargar.
    /// Lo invoca el punto de historia de Platicio al terminar su conversación de presentación.
    /// </summary>
    public static class M01PlaticioGift
    {
        public static string Roll(string level)
        {
            var data = M01Session.Instance.data;
            string prefix = "PLATICIO.REGALO." + level + ".";
            foreach (var e in data.events) if (e.StartsWith(prefix)) return e.Substring(prefix.Length);
            float r = Random.value;
            string result = r < .25f ? "OFRENDA" : r < .5f ? "DINERO" : "NADA";
            data.events.Add(prefix + result);
            if (result == "OFRENDA") data.CollectOffering("PLATICIO." + level, "Coca", false);
            else if (result == "DINERO") { data.money += 5; M01Progress.Announce("Dinero", 5); }
            return result;
        }
    }
}
