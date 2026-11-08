using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Afterglow.M01.EditorTools
{
    /// <summary>
    /// Perímetro protegido a partir de las superficies del montaje (JSON de cajas exportado desde Blender).
    /// Un borde recibe barrera invisible (3 m sobre su cota) y estacas visibles si fuera de él no hay suelo
    /// que recoja al jugador a menos de 3 m de caída. Los huecos jugables se declaran en &lt;sala&gt;_huecos.json
    /// y quedan abiertos. También escribe un plano SVG por sala con superficies, perímetro y huecos.
    /// </summary>
    public static class M01PerimeterBuilder
    {
        [Serializable] public class Box { public string name; public float[] center, size; }
        [Serializable] public class Layout { public Box[] boxes; }
        [Serializable] public class Gap { public string name, reason; public float[] min, max; }
        [Serializable] public class Gaps { public Gap[] gaps; }

        public struct Rect2 { public float x0, x1, z0, z1, top; public string name; public bool Contains(float x, float z, float pad = 0) => x >= x0 - pad && x <= x1 + pad && z >= z0 - pad && z <= z1 + pad; }
        public struct Segment { public Vector3 a, b; public float top; public Vector3 outward; }

        const float Sample = .25f, Probe = .22f, CatchDepth = 3f, WallHeight = 3.2f, Thickness = .3f;
        static readonly string[] NotWalkable = { "mass", "Back", "Neck_left", "Neck_right", "Derrumbe", "Crate", "Workbench", "Wall", "Muro" };

        public static (List<Rect2> walk, List<Rect2> solid) Surfaces(Layout layout)
        {
            var walk = new List<Rect2>(); var solid = new List<Rect2>();
            foreach (var b in layout.boxes)
            {
                var r = new Rect2 { name = b.name, x0 = b.center[0] - b.size[0] / 2, x1 = b.center[0] + b.size[0] / 2, z0 = b.center[2] - b.size[2] / 2, z1 = b.center[2] + b.size[2] / 2, top = b.center[1] + b.size[1] / 2 };
                bool walkable = b.size[0] >= 1.2f && b.size[2] >= 1.2f && !NotWalkable.Any(n => b.name.IndexOf(n, StringComparison.OrdinalIgnoreCase) >= 0);
                (walkable ? walk : solid).Add(r);
            }
            return (walk, solid);
        }

        public static List<Segment> Compute(Layout layout, Gaps gaps)
        {
            var (walk, solid) = Surfaces(layout);
            var result = new List<Segment>();
            foreach (var s in walk)
            {
                // Cuatro bordes: (inicio, fin, dirección exterior).
                var edges = new[] {
                    (new Vector2(s.x0, s.z0), new Vector2(s.x1, s.z0), new Vector2(0, -1)),
                    (new Vector2(s.x0, s.z1), new Vector2(s.x1, s.z1), new Vector2(0, 1)),
                    (new Vector2(s.x0, s.z0), new Vector2(s.x0, s.z1), new Vector2(-1, 0)),
                    (new Vector2(s.x1, s.z0), new Vector2(s.x1, s.z1), new Vector2(1, 0)) };
                foreach (var (a, b, outward) in edges)
                {
                    float length = Vector2.Distance(a, b); int steps = Mathf.Max(1, Mathf.CeilToInt(length / Sample));
                    Vector2? start = null; Vector2 last = a;
                    for (int i = 0; i <= steps; i++)
                    {
                        var p = Vector2.Lerp(a, b, (float)i / steps);
                        var q = p + outward * Probe;
                        bool supported = walk.Any(o => o.Contains(q.x, q.y) && o.top <= s.top + .75f && o.top >= s.top - CatchDepth)
                            || solid.Any(o => o.Contains(q.x, q.y) && o.top > s.top + .4f)
                            || walk.Any(o => o.Contains(q.x, q.y) && o.top > s.top + .75f) // pared de una terraza más alta
                            || (gaps?.gaps != null && gaps.gaps.Any(g => q.x >= g.min[0] && q.x <= g.max[0] && q.y >= g.min[1] && q.y <= g.max[1]));
                        if (!supported) { if (start == null) start = p; last = p; }
                        if ((supported || i == steps) && start != null)
                        {
                            if (Vector2.Distance(start.Value, last) >= .2f || i == steps)
                                result.Add(new Segment { a = new Vector3(start.Value.x, s.top, start.Value.y), b = new Vector3(last.x, s.top, last.y), top = s.top, outward = new Vector3(outward.x, 0, outward.y) });
                            start = null;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>Monta el perímetro en una escena abierta. Devuelve un resumen.</summary>
        public static string Apply(string sceneId, string layoutPath, string gapsPath, string svgPath)
        {
            var layout = JsonUtility.FromJson<Layout>(File.ReadAllText(layoutPath));
            var gaps = File.Exists(gapsPath) ? JsonUtility.FromJson<Gaps>(File.ReadAllText(gapsPath)) : new Gaps { gaps = new Gap[0] };
            var segments = Compute(layout, gaps);
            var scene = SceneManager.GetActiveScene();
            var old = scene.GetRootGameObjects().FirstOrDefault(g => g.name == "Perímetro protegido");
            if (old) Object.DestroyImmediate(old);
            var root = new GameObject("Perímetro protegido");
            var walls = new GameObject("Barreras invisibles").transform; walls.SetParent(root.transform, false);
            var posts = new GameObject("Estacas de borde").transform; posts.SetParent(root.transform, false);
            var oak = AssetDatabase.LoadAssetAtPath<Material>("Assets/Afterglow/Environment/Modelado/Oak.mat");
            int postCount = 0;
            foreach (var seg in segments)
            {
                var mid = (seg.a + seg.b) / 2; float length = Vector3.Distance(seg.a, seg.b) + Sample;
                bool alongX = Mathf.Abs(seg.outward.z) > .5f;
                var wall = new GameObject("Barrera " + mid.x.ToString("F1", CultureInfo.InvariantCulture) + "," + mid.z.ToString("F1", CultureInfo.InvariantCulture));
                wall.transform.SetParent(walls, false);
                wall.transform.position = mid + seg.outward * (Thickness / 2 + .02f) + Vector3.up * (WallHeight / 2 - .4f);
                var box = wall.AddComponent<BoxCollider>();
                box.size = alongX ? new Vector3(length, WallHeight, Thickness) : new Vector3(Thickness, WallHeight, length);
                // Estacas visibles cada 1,6 m: el borde se lee por un objeto, no solo por el collider (TOK 1:25:28).
                int count = Mathf.Max(1, Mathf.RoundToInt(length / 1.6f));
                for (int i = 0; i <= count; i++)
                {
                    var p = Vector3.Lerp(seg.a, seg.b, (float)i / count) - seg.outward * .12f;
                    var post = GameObject.CreatePrimitive(PrimitiveType.Cube); post.name = "Estaca";
                    Object.DestroyImmediate(post.GetComponent<Collider>());
                    post.transform.SetParent(posts, false);
                    post.transform.position = p + Vector3.up * .22f; post.transform.localScale = new Vector3(.12f, .44f, .12f);
                    if (oak) post.GetComponent<MeshRenderer>().sharedMaterial = oak;
                    post.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    postCount++;
                }
            }
            WriteSvg(sceneId, layout, gaps, segments, svgPath);
            EditorSceneManager.MarkSceneDirty(scene);
            return sceneId + ": " + segments.Count + " tramos protegidos, " + postCount + " estacas, " + (gaps.gaps?.Length ?? 0) + " huecos jugables.";
        }

        static void WriteSvg(string id, Layout layout, Gaps gaps, List<Segment> segments, string path)
        {
            var (walk, solid) = Surfaces(layout);
            float minX = layout.boxes.Min(b => b.center[0] - b.size[0] / 2) - 2, maxX = layout.boxes.Max(b => b.center[0] + b.size[0] / 2) + 2;
            float minZ = layout.boxes.Min(b => b.center[2] - b.size[2] / 2) - 2, maxZ = layout.boxes.Max(b => b.center[2] + b.size[2] / 2) + 2;
            const float k = 28; float W = (maxX - minX) * k, H = (maxZ - minZ) * k;
            string X(float x) => ((x - minX) * k).ToString("F1", CultureInfo.InvariantCulture);
            string Z(float z) => ((maxZ - z) * k).ToString("F1", CultureInfo.InvariantCulture);
            string L(float v) => (v * k).ToString("F1", CultureInfo.InvariantCulture);
            var sb = new StringBuilder();
            sb.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{W + 260:F0}\" height=\"{H + 60:F0}\" font-family=\"sans-serif\" font-size=\"11\">");
            sb.AppendLine($"<rect width=\"100%\" height=\"100%\" fill=\"#f6f1e7\"/><text x=\"8\" y=\"18\" font-size=\"15\" font-weight=\"bold\">{id} · perímetro protegido y huecos jugables (vista superior, +Z arriba)</text>");
            sb.AppendLine("<g transform=\"translate(0,30)\">");
            foreach (var r in walk.OrderBy(r => r.top))
            {
                int shade = (int)Mathf.Clamp(215 - r.top * 18, 120, 225);
                sb.AppendLine($"<rect x=\"{X(r.x0)}\" y=\"{Z(r.z1)}\" width=\"{L(r.x1 - r.x0)}\" height=\"{L(r.z1 - r.z0)}\" fill=\"rgb({shade},{shade - 12},{shade - 30})\" stroke=\"#6b5a45\" stroke-width=\".6\"/>");
                sb.AppendLine($"<text x=\"{X(r.x0 + .15f)}\" y=\"{Z(r.z1 - .45f)}\">{r.name} y={r.top.ToString("F2", CultureInfo.InvariantCulture)}</text>");
            }
            foreach (var r in solid) sb.AppendLine($"<rect x=\"{X(r.x0)}\" y=\"{Z(r.z1)}\" width=\"{L(r.x1 - r.x0)}\" height=\"{L(r.z1 - r.z0)}\" fill=\"#5c534b\" opacity=\".75\"/>");
            if (gaps?.gaps != null) foreach (var g in gaps.gaps)
                {
                    sb.AppendLine($"<rect x=\"{X(g.min[0])}\" y=\"{Z(g.max[1])}\" width=\"{L(g.max[0] - g.min[0])}\" height=\"{L(g.max[1] - g.min[1])}\" fill=\"#f0a030\" opacity=\".7\" stroke=\"#b06a00\" stroke-dasharray=\"4 3\"/>");
                    sb.AppendLine($"<text x=\"{X(g.max[0] + .2f)}\" y=\"{Z((g.min[1] + g.max[1]) / 2)}\" fill=\"#8a4b00\">{g.name}</text>");
                }
            foreach (var s in segments) sb.AppendLine($"<line x1=\"{X(s.a.x)}\" y1=\"{Z(s.a.z)}\" x2=\"{X(s.b.x)}\" y2=\"{Z(s.b.z)}\" stroke=\"#c0262d\" stroke-width=\"3.5\" stroke-linecap=\"round\"/>");
            sb.AppendLine("</g>");
            float lx = W + 20;
            sb.AppendLine($"<g transform=\"translate({lx:F0},50)\"><rect width=\"16\" height=\"10\" fill=\"#c8bba3\"/><text x=\"22\" y=\"9\">Superficie (más oscura = más alta)</text>");
            sb.AppendLine("<line x1=\"0\" y1=\"28\" x2=\"16\" y2=\"28\" stroke=\"#c0262d\" stroke-width=\"3.5\"/><text x=\"22\" y=\"32\">Perímetro protegido: barrera + estacas</text>");
            sb.AppendLine("<rect y=\"44\" width=\"16\" height=\"10\" fill=\"#f0a030\"/><text x=\"22\" y=\"53\">Hueco jugable: caída −2, vuelta a la entrada</text>");
            sb.AppendLine("<rect y=\"66\" width=\"16\" height=\"10\" fill=\"#5c534b\"/><text x=\"22\" y=\"75\">Masa sólida o pared</text></g>");
            sb.AppendLine("</svg>");
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, sb.ToString());
        }

        [MenuItem("Afterglow/Perímetro N1 (N01S01–N01S04)")]
        public static string BuildN1()
        {
            if (EditorApplication.isPlaying) throw new Exception("Detener Play antes de montar el perímetro.");
            if (Enumerable.Range(0, SceneManager.sceneCount).Any(i => SceneManager.GetSceneAt(i).isDirty)) throw new Exception("Hay escenas con cambios sin guardar.");
            const string data = "Assets/Afterglow/Environment/Modelado/Nivel1/";
            string report = "";
            foreach (var id in new[] { "N01S01", "N01S02", "N01S03", "N01S04" })
            {
                EditorSceneManager.OpenScene(M01Expedition.ScenePath(id));
                report += Apply(id, data + id + ".json", data + id + "_huecos.json", "Docs/Diseno/Perimetro_N1/" + id + "_perimetro.svg") + "\n";
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            }
            EditorSceneManager.OpenScene(M01Expedition.ScenePath("M01_MENU"));
            File.WriteAllText("Docs/Diseno/Perimetro_N1/Resumen.md", "# Perímetro N1\n\nGenerado por M01PerimeterBuilder.BuildN1.\n\n" + report.Replace("\n", "\n\n"));
            return report;
        }
    }
}
