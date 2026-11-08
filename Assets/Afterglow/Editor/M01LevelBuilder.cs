using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Afterglow.M01.EditorTools
{
    /// <summary>
    /// Monta las escenas de un nivel desde su plano editable (Docs/Diseno/Niveles/N?_Plano.json) y el FBX exportado por
    /// ArtSource/Blender/Modelado_Afterglow/Niveles/build_level.py. Usa N01S03 como plantilla (jugador, cámara aprobada,
    /// HUD, diálogo, menú y teléfono) y rehace todo el contenido de la sala en cada ejecución.
    /// </summary>
    public static class M01LevelBuilder
    {
        const string Template = "Assets/Afterglow/Scenes/M01/N01S03.unity";
        const string Modelado = "Assets/Afterglow/Environment/Modelado/";
        static JObject plan; static string level;

        static Vector3 V(JToken t) => new Vector3((float)t[0], (float)t[1], (float)t[2]);

        public static string Build(string id)
        {
            if (EditorApplication.isPlaying) throw new Exception("Detener Play antes de montar el nivel.");
            if (Enumerable.Range(0, SceneManager.sceneCount).Any(i => SceneManager.GetSceneAt(i).isDirty)) throw new Exception("Hay escenas con cambios sin guardar.");
            level = id; plan = JObject.Parse(File.ReadAllText("Docs/Diseno/Niveles/" + id + "_Plano.json"));
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            PrepareEnemySprites();
            string report = "";
            foreach (JObject room in plan["rooms"]) report += BuildRoom(room) + "\n";
            AddToBuildSettings(plan["rooms"].Select(r => M01Expedition.ScenePath((string)r["id"])));
            EditorSceneManager.OpenScene("Assets/Afterglow/Scenes/M01/M01_MENU.unity");
            Directory.CreateDirectory("Docs/Implementacion/Niveles");
            File.WriteAllText("Docs/Implementacion/Niveles/" + id + "_Montaje.md", "# Montaje " + id + "\n\nGenerado por M01LevelBuilder.Build(\"" + id + "\").\n\n" + report.Replace("\n", "\n\n"));
            return report;
        }

        static string BuildRoom(JObject room)
        {
            string id = (string)room["id"]; string path = M01Expedition.ScenePath(id);
            AssetDatabase.DeleteAsset(path); AssetDatabase.CopyAsset(Template, path);
            var scene = EditorSceneManager.OpenScene(path);
            var roots = scene.GetRootGameObjects();
            var player = Object.FindAnyObjectByType<AfterglowPlayer>();
            var dialogue = Object.FindAnyObjectByType<M01Dialogue>();
            var stage = roots.First(g => g.name.StartsWith("Escenario"));
            var offeringProto = Object.FindAnyObjectByType<M01StaticOffering>();
            var font = dialogue.body.font;
            var enemyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Afterglow/Prefabs/Enemy.prefab");
            var lootMat = Object.FindAnyObjectByType<M01EnemyLoot>()?.material;
            var offerMesh = offeringProto.GetComponent<MeshFilter>().sharedMesh; var offerMat = offeringProto.GetComponent<MeshRenderer>().sharedMaterial;
            foreach (var g in roots) if (g.GetComponent<AfterglowEnemy>() || g.name == "Perímetro protegido") Object.DestroyImmediate(g);
            foreach (Transform child in stage.transform.Cast<Transform>().ToArray()) Object.DestroyImmediate(child.gameObject);
            stage.name = "Escenario · " + id;
            var state = Object.FindAnyObjectByType<M01Expedition>(); state.name = "Room state " + id; state.roomId = id; state.level = (int)plan["level"]; state.safeArea = false;

            // Geometría exportada desde Blender.
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(Modelado + "Nivel" + plan["level"] + "/" + id + ".fbx") ?? throw new Exception("Falta FBX " + id);
            var model = (GameObject)PrefabUtility.InstantiatePrefab(asset); model.name = "Montaje " + id; model.transform.SetParent(stage.transform, false);
            PrefabUtility.UnpackPrefabInstance(model, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            var axis = model.GetComponentsInChildren<Transform>().First(t => t.name == "AxisWitness");
            if (Vector3.Distance(axis.position, new Vector3(3, 2, 7)) > .03f) throw new Exception("Ejes FBX incorrectos " + id + axis.position);
            foreach (var mr in model.GetComponentsInChildren<MeshRenderer>())
            {
                string mat = mr.sharedMaterial ? mr.sharedMaterial.name : "Dust";
                mr.sharedMaterial = Material(mat.Split(' ')[0]);
            }
            Transform Piece(string prefix) { var parts = model.GetComponentsInChildren<Transform>().Where(t => t.name.StartsWith("Export_" + prefix)).ToArray(); if (parts.Length == 0) return null; var holder = new GameObject(prefix).transform; holder.SetParent(stage.transform, false); foreach (var p in parts) p.SetParent(holder, true); return holder; }

            // Colliders del plano y perímetro protegido.
            string data = Modelado + "Nivel" + plan["level"] + "/";
            var layout = JObject.Parse(File.ReadAllText(data + id + ".json"));
            var colliders = new GameObject("Colisión del plano").transform; colliders.SetParent(stage.transform, false);
            foreach (var box in layout["boxes"]) { var go = new GameObject((string)box["name"]); go.transform.SetParent(colliders, false); go.transform.position = V(box["center"]); go.AddComponent<BoxCollider>().size = V(box["size"]); }
            if (!((bool?)room["ride"] ?? false) && room["arena"] == null) M01PerimeterBuilder.Apply(id, data + id + ".json", data + id + "_huecos.json", "Docs/Diseno/Perimetro_" + level + "/" + id + "_perimetro.svg");

            // Luces cálidas de farol.
            foreach (var l in room["lights"] ?? new JArray())
            {
                var light = new GameObject("Luz de farol").AddComponent<Light>(); light.transform.SetParent(stage.transform, false);
                light.transform.position = new Vector3((float)l[0], (float)l[1], (float)l[2]); light.type = LightType.Point; light.range = 9; light.intensity = (float)l[3]; light.color = new Color(1f, .74f, .45f); light.shadows = LightShadows.None;
            }

            if ((bool?)room["dark"] ?? false) stage.AddComponent<M01DarkRoom>();
            // Cámara aprobada: solo se ajustan los límites de encuadre a la sala.
            var cam = Object.FindAnyObjectByType<AfterglowCamera>(); var c = room["camera"];
            if ((bool?)c["side"] ?? false) { cam.followDepth = false; cam.minX = (float)c["minX"]; cam.maxX = (float)c["maxX"]; if (c["offset"] != null) cam.offset = V(c["offset"]); }
            else { cam.followDepth = true; cam.focusX = new Vector2((float)c["focusX"][0], (float)c["focusX"][1]); cam.focusZ = new Vector2((float)c["focusZ"][0], (float)c["focusZ"][1]); cam.depthBlend = new Vector2((float)c["depthBlend"][0], (float)c["depthBlend"][1]); }

            // Jugador y entradas seguras.
            var entry = V(room["entry"]); player.transform.position = entry;
            var rules = player.GetComponent<M01PlayerRules>(); rules.sectionEntrance = entry; rules.levelEntrance = M01Expedition.SectionEntrance(level.Replace("N", "N0") + "S01");
            rules.foundationTestKit = false; rules.hasPickaxe = false;

            TextMeshPro Label(string text, Vector3 p)
            {
                var t = new GameObject(text).AddComponent<TextMeshPro>(); t.transform.SetParent(stage.transform, false); t.transform.position = p;
                t.font = font; t.fontSize = 2.1f; t.alignment = TextAlignmentOptions.Center; t.text = text; t.color = new Color(1f, .93f, .78f);
                t.rectTransform.sizeDelta = new Vector2(8, 1.2f);
                t.gameObject.AddComponent<M01WorldHint>(); return t;
            }

            foreach (var d in room["doors"])
            {
                var door = new GameObject((string)d["name"]).AddComponent<M01RouteDoor>(); door.transform.SetParent(stage.transform, false);
                door.transform.position = V(d["trigger"]); door.destination = (string)d["to"]; door.arrival = V(d["arrival"]);
                door.requiresInteract = true; door.interactionRadius = 1.8f; door.verticalTolerance = .85f; door.fade = true; door.blockedWhenWorldClosed = door.destination == "M01_MAPA";
                door.wrongDoor = (bool?)d["wrong"] ?? false;
                if (door.destination == id && !door.wrongDoor) throw new Exception("Puerta circular sin marcar en " + id);
                Label((string)d["name"] + "  [E]", V(d["trigger"]) + Vector3.up * ((int?)d["lights"] > 0 ? 4.0f : 3.1f));
            }

            foreach (var sg in room["signs"] ?? new JArray()) Label((string)sg[4] + " · " + (int)sg[3] + (((int)sg[3]) == 1 ? " farol" : " faroles"), new Vector3((float)sg[0], (float)sg[1] + 2.6f, (float)sg[2]));
            foreach (var o in room["offerings"] ?? new JArray())
            {
                var go = new GameObject("Ofrenda · Coca", typeof(MeshFilter), typeof(MeshRenderer)); go.transform.SetParent(stage.transform, false);
                go.GetComponent<MeshFilter>().sharedMesh = offerMesh; go.GetComponent<MeshRenderer>().sharedMaterial = offerMat; go.transform.localScale = Vector3.one * .32f;
                go.transform.position = new Vector3((float)o[1], (float)o[2], (float)o[3]); go.AddComponent<M01StaticOffering>().sourceId = (string)o[0];
            }
            foreach (var q in room["questItems"] ?? new JArray())
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube); Object.DestroyImmediate(go.GetComponent<Collider>()); go.name = (string)q[1];
                go.transform.SetParent(stage.transform, false); go.transform.position = new Vector3((float)q[2], (float)q[3], (float)q[4]); go.transform.localScale = new Vector3(.6f, .4f, .4f);
                go.GetComponent<MeshRenderer>().sharedMaterial = Material("Trim");
                var item = go.AddComponent<M01QuestObject>(); item.itemId = (string)q[0]; item.displayName = (string)q[1];
            }
            foreach (var b in room["breakables"] ?? new JArray())
            {
                var holder = Piece("Rock_" + (string)b[0]); if (!holder) continue;
                var mesh = holder.GetComponentInChildren<MeshFilter>(); var bounds = mesh.GetComponent<MeshRenderer>().bounds;
                holder.position = Vector3.zero; var col = holder.gameObject.AddComponent<BoxCollider>(); col.center = holder.InverseTransformPoint(bounds.center); col.size = bounds.size;
                var breakable = holder.gameObject.AddComponent<M01BreakableRock>(); breakable.eventId = (string)b[0];
                if (b.Count() > 3) { breakable.questId = (string)b[3]; breakable.perAttempt = true; Label("Roca marcada", bounds.center + Vector3.up * 1.4f); }
            }

            var carts = new Dictionary<string, M01RailCart>();
            foreach (JObject m in room["mechanisms"] ?? new JArray())
            {
                string type = (string)m["type"];
                if (type == "cart")
                {
                    var holder = Piece("Cart_" + (string)m["id"]); float y = (float?)m["y"] ?? 0;
                    var root = new GameObject("Carro " + (string)m["id"]); root.transform.SetParent(stage.transform, false); root.transform.position = new Vector3((float)m["start"], y, (float)m["z"]);
                    if (holder) holder.SetParent(root.transform, true);
                    var box = root.AddComponent<BoxCollider>(); box.center = new Vector3(0, .62f, 0); box.size = new Vector3(1.36f, 1.0f, 1.0f);
                    var cart = root.AddComponent<M01RailCart>(); cart.cartId = (string)m["id"]; cart.eventId = (string)m["event"];
                    cart.min = (float)m["min"]; cart.max = (float)m["max"]; cart.start = (float)m["start"]; cart.plate = (float)m["plate"]; cart.marked = (bool?)m["marked"] ?? false;
                    carts[cart.cartId] = cart;
                    if ((bool?)m["light"] ?? false) { var lamp = new GameObject("Luz del carro").AddComponent<Light>(); lamp.transform.SetParent(root.transform, false); lamp.transform.localPosition = new Vector3(0, 1.6f, 0); lamp.type = LightType.Point; lamp.range = 7.5f; lamp.intensity = 3.2f; lamp.color = new Color(1f, .82f, .5f); lamp.shadows = LightShadows.None; }
                    Piece("Plate_" + (string)m["id"]);
                }
                else if (type == "gate")
                {
                    var bars = Piece("Gate"); var root = new GameObject((string)m["id"]); root.transform.SetParent(stage.transform, false);
                    var p = V(m["pos"]); var size = V(m["size"]);
                    if (bars) bars.SetParent(root.transform, true);
                    var blocker = new GameObject("Bloqueo de la reja").AddComponent<BoxCollider>(); blocker.transform.SetParent(root.transform, false); blocker.transform.position = p + Vector3.up * size.y / 2; blocker.size = size + new Vector3(.2f, 0, 0);
                    var gate = root.AddComponent<M01Gate>(); gate.eventId = (string)m["event"]; gate.bars = bars; gate.blocker = blocker;
                }
                else if (type == "signPlate")
                {
                    var board = Piece("SignBoard_" + (string)m["id"]); var p = V(m["pos"]);
                    var pivot = new GameObject("Placa " + (string)m["id"]).transform; pivot.SetParent(stage.transform, false); pivot.position = p + new Vector3(0, 1.45f, -.08f);
                    if (board) board.SetParent(pivot, true);
                    var plate = new GameObject("Interacción " + (string)m["id"]).AddComponent<M01SignPlate>(); plate.transform.SetParent(stage.transform, false); plate.transform.position = p;
                    plate.dialogue = dialogue; plate.eventId = (string)m["id"]; plate.board = pivot;
                    Label("Placa torcida  [E]", p + Vector3.up * 2.3f);
                }
                else if (type == "resetLever")
                {
                    var arm = Piece("ResetArm"); var lever = new GameObject("Palanca de topes").AddComponent<M01CartResetLever>(); lever.transform.SetParent(stage.transform, false); lever.transform.position = V(m["pos"]);
                    if (arm) { var pivot = new GameObject("Pivote palanca de topes").transform; pivot.SetParent(stage.transform, false); pivot.position = V(m["pos"]) + Vector3.up * .55f; arm.SetParent(pivot, true); lever.arm = pivot; }
                    lever.dialogue = dialogue; lever.lineId = (string)m["line"];
                    lever.carts = ((JArray)m["carts"]).Select(x => carts.TryGetValue((string)x, out var cc) ? cc : null).Where(x => x).ToArray();
                    Label("Palanca de topes  [E]", V(m["pos"]) + Vector3.up * 2.2f);
                }
                else if (type == "bridge")
                {
                    var pivot = new GameObject("Puente · pivote").transform; pivot.SetParent(stage.transform, false); pivot.position = V(m["pivot"]);
                    var deck = Piece("BridgeDeck"); if (deck) deck.SetParent(pivot, true);
                    var crossing = pivot.gameObject.AddComponent<BoxCollider>(); crossing.center = V(m["deckCenter"]); crossing.size = V(m["deckSize"]);
                    var leverGo = new GameObject("Palanca del contrapeso"); leverGo.transform.SetParent(stage.transform, false); leverGo.transform.position = V(m["lever"]);
                    var armPivot = new GameObject("Pivote palanca").transform; armPivot.SetParent(stage.transform, false); armPivot.position = V(m["lever"]) + Vector3.up * .55f;
                    var arm = Piece("LeverArm"); if (arm) arm.SetParent(armPivot, true);
                    var weight = Piece("Counterweight");
                    var bridge = leverGo.AddComponent<M01CounterweightBridge>(); bridge.deck = pivot; bridge.crossing = crossing; bridge.leverArm = armPivot; bridge.counterweight = weight; bridge.dialogue = dialogue;
                    bridge.openEvent = (string)m["event"]; bridge.hintEvent = "";
                    bridge.prompt = Label("Bajar puente  [E]", V(m["lever"]) + new Vector3(-1.4f, 1.9f, -.6f)).gameObject;
                }
                else if (type == "hangzzer")
                {
                    var go = (GameObject)PrefabUtility.InstantiatePrefab(enemyPrefab); go.name = (string)m["id"]; go.transform.position = V(m["anchor"]);
                    var enemy = go.GetComponent<AfterglowEnemy>(); enemy.maxHealth = 4;
                    var loot = go.AddComponent<M01EnemyLoot>(); loot.material = lootMat; loot.referenceLevel = 2; loot.baseExperience = 6; loot.money = 4;
                    var figure = go.GetComponentInChildren<M01EnemySprite>(); var body = figure ? figure.body : null; if (figure) Object.DestroyImmediate(figure);
                    var h = go.AddComponent<M01Hangzzer>(); h.anchor = V(m["anchor"]); h.ground = V(m["ground"]); h.tunnel = V(m["tunnel"]);
                    h.detect = (float)m["detect"]; h.interval = (float)m["interval"]; h.maxAlive = (int)m["maxAlive"]; h.maxPerCycle = (int)m["maxPerCycle"];
                    h.reinforcementPrefab = enemyPrefab; h.lootMaterial = lootMat; h.body = body;
                    h.hanging = Sprite("Hangzzer_1"); h.whistling = Sprite("Hangzzer_0");
                    if (body) { body.sprite = h.hanging; body.transform.localPosition = Vector3.zero; }
                    var wire = GameObject.CreatePrimitive(PrimitiveType.Cylinder); Object.DestroyImmediate(wire.GetComponent<Collider>()); wire.name = "Cable del Hangzzer"; wire.transform.SetParent(stage.transform, false);
                    wire.GetComponent<MeshRenderer>().sharedMaterial = Material("Iron"); h.wire = wire.transform;
                }
            }

            foreach (var e in room["enemies"] ?? new JArray())
            {
                var go = (GameObject)PrefabUtility.InstantiatePrefab(enemyPrefab); go.name = (string)e["id"]; go.transform.position = V(e["pos"]);
                var loot = go.AddComponent<M01EnemyLoot>(); loot.material = lootMat; loot.referenceLevel = (int)plan["level"];
                string kind = (string)e["type"] ?? "Resisworm";
                if (kind != "Resisworm")
                {
                    var figure = go.GetComponentInChildren<M01EnemySprite>(); var body = figure ? figure.body : null; if (figure) Object.DestroyImmediate(figure);
                    var enemy = go.GetComponent<AfterglowEnemy>();
                    if (kind == "Gasplode") { enemy.maxHealth = 1; loot.baseExperience = 2; var g = go.AddComponent<M01Gasplode>(); g.body = body; g.idle = Sprite("Gasplode_0"); g.alert = Sprite("Gasplode_1"); g.blast = Sprite("Gasplode_2"); if (body) body.sprite = g.idle; }
                    else if (kind == "Lighmet") { enemy.maxHealth = 4; loot.baseExperience = 5; var lm = go.AddComponent<M01Lighmet>(); lm.body = body; lm.idle = Sprite("Lighmet_0"); lm.cast = Sprite("Lighmet_2"); lm.fireball = Sprite("Lighmet_1"); if (body) { body.sprite = lm.idle; body.transform.localScale *= .8f; } }
                    else if (kind == "Drilly")
                    {
                        enemy.maxHealth = 3; loot.baseExperience = 4; var dr = go.AddComponent<M01Drilly>(); dr.body = body;
                        if (body) { body.sprite = Sprite("Drilly_0"); body.transform.localScale *= .5f; }
                        var mound = GameObject.CreatePrimitive(PrimitiveType.Sphere); Object.DestroyImmediate(mound.GetComponent<Collider>()); mound.name = "Montículo"; mound.transform.SetParent(go.transform, false);
                        mound.transform.localPosition = new Vector3(0, .05f, 0); mound.transform.localScale = new Vector3(1.1f, .35f, .9f); mound.GetComponent<MeshRenderer>().sharedMaterial = Material("Earth"); dr.mound = mound.transform;
                    }
                }
            }

            foreach (JObject n in room["npcs"] ?? new JArray())
            {
                string kind = (string)n["kind"];
                var go = new GameObject((string)n["label"]); go.transform.SetParent(stage.transform, false); go.transform.position = V(n["pos"]);
                var sr = go.AddComponent<SpriteRenderer>(); sr.sprite = (string)n["id"] == "PLATICIO" ? Actor("Actor_Hijo") : Actor("Actor_Minero");
                string[] lines = n["lines"]?.Select(x => (string)x).ToArray();
                if (kind == "story")
                {
                    var sp = go.AddComponent<M01StoryPoint>(); sp.dialogue = dialogue; sp.eventId = (string)n["event"]; sp.lineIds = lines; sp.automatic = (bool?)n["automatic"] ?? false; sp.radius = (float?)n["radius"] ?? 2.4f; sp.giftLevel = (string)n["gift"];
                    var talk = go.AddComponent<M01DialoguePoint>(); talk.dialogue = dialogue; talk.lineIds = new[] { lines.Last() }; talk.radius = 2.2f;
                }
                else if (kind == "quest")
                {
                    var q = go.AddComponent<M01QuestNPC>(); q.dialogue = dialogue; q.questId = (string)n["quest"]; q.startLine = (string)n["start"]; q.endLine = (string)n["end"]; q.npcName = (string)n["label"];
                    q.requiredEvents = n["requiredEvents"]?.Select(x => (string)x).ToArray(); q.requiredItem = (string)n["requiredItem"]; q.offeringReward = (int?)n["offerings"] ?? 1; q.moneyReward = (int?)n["money"] ?? 0; q.requiredCount = (int?)n["requiredCount"] ?? 3;
                }
                else if (kind == "talk") { var t = go.AddComponent<M01DialoguePoint>(); t.dialogue = dialogue; t.lineIds = lines; t.radius = 2.2f; }
                else if (kind == "escort") { var es = go.AddComponent<M01EscortNPC>(); es.dialogue = dialogue; es.questId = (string)n["quest"]; es.deliverScene = (string)n["deliverScene"]; es.deliverEvent = (string)n["event"]; es.lineIds = lines; es.homeScene = id; }
                else if (kind == "investigation") { var inv = go.AddComponent<M01Investigation>(); inv.dialogue = dialogue; inv.index = (int)n["index"]; inv.lineIds = lines; inv.requiredEvent = (string)n["requiredEvent"]; }
                else if (kind == "cartStart" || kind == "cartArrival") { var cm = go.AddComponent<M01CartMiner>(); cm.dialogue = dialogue; cm.arrivalRole = kind == "cartArrival"; cm.threshold = (int?)n["threshold"] ?? 18; }
                if ((bool?)n["hidden"] ?? false) { sr.enabled = false; continue; }
                var npcLabel = Label((string)n["label"] + (kind == "story" && ((bool?)n["automatic"] ?? false) ? "" : "  [E]"), V(n["pos"]) + Vector3.up * 2.6f);
                if (n["visibleEvent"] != null) { var vis = go.AddComponent<M01EventVisible>(); vis.eventId = (string)n["visibleEvent"]; vis.extra = new[] { npcLabel.gameObject }; }
                if (kind == "escort") go.GetComponent<M01EscortNPC>().label = npcLabel.gameObject;
            }

            if ((bool?)room["ride"] ?? false) BuildRide(room, stage.transform, player);
            if (room["arena"] != null) BuildBoss(room, stage.transform, player, dialogue, enemyPrefab, lootMat);
            if (room["props"]?["rescueDoor"] != null) { var rd = new GameObject("Puerta de los atrapados").AddComponent<M01RescueDoor>(); rd.transform.SetParent(stage.transform, false); rd.transform.position = V(room["props"]["rescueDoor"]) + new Vector3(0, 0, -.8f); rd.dialogue = dialogue; Label("Puerta de los atrapados  [E]", V(room["props"]["rescueDoor"]) + Vector3.up * 3.2f); }
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            return id + ": montado (" + layout["boxes"].Count() + " colliders, " + (room["doors"]?.Count() ?? 0) + " puertas, " + (room["enemies"]?.Count() ?? 0) + " enemigos, " + (room["npcs"]?.Count() ?? 0) + " personajes).";
        }

        static void BuildBoss(JObject room, Transform stage, AfterglowPlayer player, M01Dialogue dialogue, GameObject enemyPrefab, Material lootMat)
        {
            var a = room["arena"]; var center = V(a["center"]); float radius = (float)a["radius"];
            TextMeshPro Label(string text, Vector3 p)
            {
                var t = new GameObject(text).AddComponent<TextMeshPro>(); t.transform.SetParent(stage, false); t.transform.position = p;
                t.font = dialogue.body.font; t.fontSize = 2.1f; t.alignment = TextAlignmentOptions.Center; t.text = text; t.color = new Color(1f, .93f, .78f);
                t.rectTransform.sizeDelta = new Vector2(8, 1.2f); t.gameObject.AddComponent<M01WorldHint>(); return t;
            }
            // Suelo y pared circular continua (colliders); el visual viene del FBX de la arena.
            var floor = new GameObject("Suelo de la arena").AddComponent<BoxCollider>(); floor.transform.SetParent(stage, false); floor.transform.position = center + Vector3.down * 1.5f; floor.size = new Vector3(radius * 2 + 4, 3, radius * 2 + 4);
            for (int i = 0; i < 40; i++)
            {
                float ang = i / 40f * Mathf.PI * 2; var w = new GameObject("Pared " + i).AddComponent<BoxCollider>(); w.transform.SetParent(stage, false);
                w.transform.position = center + new Vector3(Mathf.Cos(ang), 0, Mathf.Sin(ang)) * (radius + .3f) + Vector3.up * 2; w.transform.rotation = Quaternion.LookRotation(new Vector3(Mathf.Cos(ang), 0, Mathf.Sin(ang))); w.size = new Vector3(1.8f, 4.5f, .6f);
            }
            // PerforAudora: FBX corregido, escala ×4, punta hacia +Z del pivote.
            var drillRoot = new GameObject("PerforAudora"); drillRoot.transform.SetParent(stage, false); drillRoot.transform.position = V(a["drill"]);
            var modelAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Bosses/Mundo 1 - PerforAudora/PerforAudora_Corregido.fbx");
            var model = (GameObject)PrefabUtility.InstantiatePrefab(modelAsset); model.transform.SetParent(drillRoot.transform, false);
            var mf = model.GetComponentInChildren<MeshFilter>(); var verts = mf.sharedMesh.vertices; var b = mf.sharedMesh.bounds;
            // La punta es el extremo del eje largo con menor sección.
            int axis = b.size.x >= b.size.y && b.size.x >= b.size.z ? 0 : b.size.y >= b.size.z ? 1 : 2;
            float lo = float.MaxValue, hi = float.MinValue; foreach (var v in verts) { lo = Mathf.Min(lo, v[axis]); hi = Mathf.Max(hi, v[axis]); }
            float spreadLo = 0, spreadHi = 0; foreach (var v in verts) { var o = v - b.center; o[axis] = 0; if (v[axis] < lo + (hi - lo) * .08f) spreadLo = Mathf.Max(spreadLo, o.magnitude); if (v[axis] > hi - (hi - lo) * .08f) spreadHi = Mathf.Max(spreadHi, o.magnitude); }
            var tipLocal = Vector3.zero; tipLocal[axis] = spreadLo < spreadHi ? -1 : 1;
            var tipWorld = mf.transform.TransformDirection(tipLocal); tipWorld.y = 0;
            model.transform.localScale = Vector3.one * 4f;
            model.transform.localRotation = Quaternion.FromToRotation(tipWorld.normalized, Vector3.forward) * model.transform.localRotation;
            var bounds = model.GetComponentInChildren<MeshRenderer>().bounds; model.transform.position += new Vector3(drillRoot.transform.position.x - bounds.center.x, drillRoot.transform.position.y - bounds.min.y + .1f, drillRoot.transform.position.z - bounds.center.z);
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Bosses/Mundo 1 - PerforAudora/PerforAudora_Corregido_Color.png");
            var drillMat = AssetDatabase.LoadAssetAtPath<Material>(Modelado + "PerforAudora.mat");
            if (!drillMat) { drillMat = new Material(Shader.Find("Universal Render Pipeline/Lit")); drillMat.SetTexture("_BaseMap", tex); drillMat.SetFloat("_Smoothness", .55f); drillMat.SetFloat("_Metallic", .5f); AssetDatabase.CreateAsset(drillMat, Modelado + "PerforAudora.mat"); }
            foreach (var r in model.GetComponentsInChildren<MeshRenderer>()) r.sharedMaterial = drillMat;
            var warning = GameObject.CreatePrimitive(PrimitiveType.Cylinder); Object.DestroyImmediate(warning.GetComponent<Collider>()); warning.name = "Aviso de embestida"; warning.transform.SetParent(drillRoot.transform, false);
            warning.transform.localPosition = new Vector3(0, .02f, 4); warning.transform.localScale = new Vector3(1.6f, .01f, 6f); warning.GetComponent<MeshRenderer>().sharedMaterial = Material("Rose");
            var mounted = new GameObject("Resistencias montadas").transform; mounted.SetParent(drillRoot.transform, false);
            for (int i = 0; i < 4; i++) { var s = new GameObject("Resistencia " + i).AddComponent<SpriteRenderer>(); s.transform.SetParent(mounted, false); s.transform.localPosition = new Vector3(-.9f + i * .6f, 2.0f, -.6f); s.sprite = Sprite("Resisworm_1"); s.transform.localScale = Vector3.one * .7f; }
            var drill = drillRoot.AddComponent<M01BossDrill>(); drill.model = model.transform; drill.warning = warning.transform; drill.arenaCenter = center; drill.arenaRadius = radius - .4f;
            // Altar de batalla, cornisa de Platicio y cierre.
            var altar = new GameObject("Altar de la arena").transform; altar.SetParent(stage, false); altar.position = V(a["altar"]);
            var altarCol = altar.gameObject.AddComponent<BoxCollider>(); altarCol.center = new Vector3(0, .5f, 0); altarCol.size = new Vector3(1.4f, 1f, 1.4f);
            Label("Altar de la arena  [E]", V(a["altar"]) + Vector3.up * 2.4f);
            var ledge = new GameObject("Platicio en la cornisa").AddComponent<SpriteRenderer>(); ledge.transform.SetParent(stage, false); ledge.transform.position = V(a["ledge"]); ledge.sprite = Actor("Actor_Hijo");
            Label("Platicio", V(a["ledge"]) + Vector3.up * 2.6f);
            var ending = new GameObject("Cierre del mundo").AddComponent<M01Ending>(); ending.transform.SetParent(stage, false); ending.dialogue = dialogue; ending.sonSprite = Actor("Actor_Hijo");
            var piece = GameObject.CreatePrimitive(PrimitiveType.Cube); Object.DestroyImmediate(piece.GetComponent<Collider>()); piece.name = "Pieza dorada"; piece.transform.SetParent(stage, false); piece.transform.position = V(a["drill"]) + Vector3.up * .5f; piece.transform.localScale = new Vector3(.8f, .5f, .5f); piece.GetComponent<MeshRenderer>().sharedMaterial = Material("Lamp");
            var glow = new GameObject("Brillo de la pieza").AddComponent<Light>(); glow.transform.SetParent(piece.transform, false); glow.type = LightType.Point; glow.range = 4; glow.intensity = 2.5f; glow.color = new Color(1, .8f, .35f);
            var pieceLabel = Label("Pieza dorada  [E]", V(a["drill"]) + Vector3.up * 1.8f); pieceLabel.transform.SetParent(piece.transform, true); ending.piece = piece;
            var captain = new GameObject("Capitán de minas").AddComponent<SpriteRenderer>(); captain.transform.SetParent(stage, false); captain.transform.position = V(a["captain"]); captain.sprite = Actor("Actor_Minero"); captain.transform.localScale = Vector3.one * 1.1f; ending.captain = captain.gameObject;
            var cart = new GameObject("Vagoneta escondida").transform; cart.SetParent(stage, false); cart.position = V(a["cart"]);
            foreach (var (pos, scale, mat) in new[] { (new Vector3(0, .55f, 0), new Vector3(1.4f, .6f, 1f), "CartBody"), (new Vector3(0, .88f, 0), new Vector3(1.5f, .08f, 1.1f), "Iron") }) { var g = GameObject.CreatePrimitive(PrimitiveType.Cube); Object.DestroyImmediate(g.GetComponent<Collider>()); g.transform.SetParent(cart, false); g.transform.localPosition = pos; g.transform.localScale = scale; g.GetComponent<MeshRenderer>().sharedMaterial = Material(mat); }
            var cartLabel = Label("Vagoneta  [E]", V(a["cart"]) + Vector3.up * 2.4f); cartLabel.transform.SetParent(cart, true); ending.cart = cart.gameObject;
            var end = new GameObject("Destino de la vagoneta").transform; end.SetParent(stage, false); end.position = V(a["cartEnd"]); ending.cartPath = end;
            var miner = new GameObject("Minero de la lona").AddComponent<SpriteRenderer>(); miner.transform.SetParent(stage, false); miner.transform.position = V(a["cartMiner"]); miner.sprite = Actor("Actor_Minero");
            var talk = miner.gameObject.AddComponent<M01DialoguePoint>(); talk.dialogue = dialogue; talk.lineIds = new[] { "FIN.16" }; var minerLabel = Label("Minero  [E]", V(a["cartMiner"]) + Vector3.up * 2.6f); minerLabel.transform.SetParent(miner.transform, true); ending.cartMiner = miner.gameObject;
            var arena = stage.gameObject.AddComponent<M01BossArena>(); arena.dialogue = dialogue; arena.drill = drill; arena.altar = altar; arena.sonLedge = ledge.transform; arena.ending = ending;
            arena.resistancePrefab = enemyPrefab; arena.warnMaterial = Material("Rose"); arena.rockMaterial = Material("Slate"); arena.offeringMaterial = Material("LeafLight"); arena.lootMaterial = lootMat; arena.center = center; arena.radius = radius - 1;
            player.GetComponent<M01PlayerRules>().sonSprite = Actor("Actor_Hijo");
        }

        static void BuildRide(JObject room, Transform stage, AfterglowPlayer player)
        {
            var track = room["track"].Select(t => new Vector3((float)t[0], (float)t[1], (float)t[2])).ToList();
            float TrackY(float x) { foreach (var t in track) if (x >= t.x && x <= t.y) return t.z; return 0; }
            var entry = V(room["entry"]);
            var cart = new GameObject("Vagoneta").transform; cart.SetParent(stage, false); cart.position = new Vector3(entry.x, TrackY(entry.x), 0);
            void Part(PrimitiveType type, Vector3 pos, Vector3 scale, string mat) { var g = GameObject.CreatePrimitive(type); Object.DestroyImmediate(g.GetComponent<Collider>()); g.transform.SetParent(cart, false); g.transform.localPosition = pos; g.transform.localScale = scale; if (type == PrimitiveType.Cylinder) g.transform.localRotation = Quaternion.Euler(90, 0, 0); g.GetComponent<MeshRenderer>().sharedMaterial = Material(mat); }
            Part(PrimitiveType.Cube, new Vector3(0, .55f, 0), new Vector3(1.4f, .6f, 1f), "CartBody"); Part(PrimitiveType.Cube, new Vector3(0, .88f, 0), new Vector3(1.5f, .08f, 1.1f), "Iron");
            foreach (var dx in new[] { -.45f, .45f }) foreach (var dz in new[] { -.5f, .5f }) Part(PrimitiveType.Cylinder, new Vector3(dx, .22f, dz), new Vector3(.42f, .05f, .42f), "Iron");
            var ride = stage.gameObject.AddComponent<M01MineCartRide>(); ride.cart = cart; ride.start = cart.position;
            var r = room["rideSettings"]; ride.endX = (float)r["endX"]; ride.arrivalScene = (string)r["arrivalScene"]; ride.arrival = V(r["arrival"]);
            var crystal = Material("Crystal");
            foreach (var m in room["minerals"]) { var g = GameObject.CreatePrimitive(PrimitiveType.Cube); Object.DestroyImmediate(g.GetComponent<Collider>()); g.name = "Mineral suspendido"; g.transform.SetParent(stage, false); g.transform.position = new Vector3((float)m[0], (float)m[1] + .9f, 0); g.transform.localScale = Vector3.one * .38f; g.transform.rotation = Quaternion.Euler(45, 0, 45); g.GetComponent<MeshRenderer>().sharedMaterial = crystal; ride.minerals.Add(g.transform); }
            foreach (var o in room["obstacles"]) { float x = (float)o; var g = GameObject.CreatePrimitive(PrimitiveType.Cube); Object.DestroyImmediate(g.GetComponent<Collider>()); g.name = "Obstáculo"; g.transform.SetParent(stage, false); g.transform.position = new Vector3(x, TrackY(x) + .35f, 0); g.transform.localScale = new Vector3(.8f, .7f, 1.2f); g.GetComponent<MeshRenderer>().sharedMaterial = Material("Slate"); ride.obstacles.Add(g.transform); }
            if (room["package"] != null) { var g = GameObject.CreatePrimitive(PrimitiveType.Cube); Object.DestroyImmediate(g.GetComponent<Collider>()); g.name = "Paquete de la vía"; g.transform.SetParent(stage, false); g.transform.position = new Vector3((float)room["package"][0], (float)room["package"][1] + .4f, 0); g.transform.localScale = new Vector3(.6f, .45f, .45f); g.GetComponent<MeshRenderer>().sharedMaterial = Material("Trim"); ride.package = g.transform; }
            player.transform.position = cart.position + new Vector3(0, .55f, 0);
        }

        static Material Material(string name)
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>(Modelado + name + ".mat");
            if (mat) return mat;
            var colors = new Dictionary<string, Color> { { "BridgeDeck", new Color(.52f, .31f, .12f) }, { "LeverArm", new Color(.78f, .46f, .12f) }, { "Counterweight", new Color(.28f, .25f, .31f) }, { "CartBody", new Color(.36f, .22f, .12f) }, { "CartMark", new Color(.95f, .72f, .18f) }, { "Plate", new Color(.62f, .5f, .2f) }, { "GateIron", new Color(.2f, .22f, .25f) }, { "Cracked", new Color(.62f, .55f, .46f) } };
            var reference = AssetDatabase.LoadAssetAtPath<Material>(Modelado + "Oak.mat");
            mat = reference ? new Material(reference) : new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = colors.TryGetValue(name, out var c) ? c : new Color(.5f, .5f, .5f);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", mat.color);
            AssetDatabase.CreateAsset(mat, Modelado + name + ".mat");
            return mat;
        }

        static Sprite Actor(string asset) => AssetDatabase.LoadAllAssetsAtPath(Modelado + asset + ".asset").OfType<Sprite>().FirstOrDefault();
        static Sprite Sprite(string pose) => AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Afterglow/Character/Enemigos/" + pose + ".png");

        /// <summary>Importa las poses como sprites (pivote en los pies; el Hangzzer colgado, en el cable) y viste el prefab Enemy con Resisworm.</summary>
        public static void PrepareEnemySprites()
        {
            foreach (var file in Directory.GetFiles("Assets/Afterglow/Character/Enemigos", "*.png"))
            {
                var importer = (TextureImporter)AssetImporter.GetAtPath(file.Replace('\\', '/'));
                string name = Path.GetFileNameWithoutExtension(file);
                bool top = name == "Hangzzer_1";
                if (importer.textureType == TextureImporterType.Sprite && importer.spritePixelsPerUnit == 520) continue;
                importer.textureType = TextureImporterType.Sprite; importer.spriteImportMode = SpriteImportMode.Single; importer.mipmapEnabled = false; importer.alphaIsTransparency = true;
                importer.spritePixelsPerUnit = 520;
                var settings = new TextureImporterSettings(); importer.ReadTextureSettings(settings);
                settings.spriteAlignment = (int)(top ? SpriteAlignment.TopCenter : SpriteAlignment.BottomCenter); importer.SetTextureSettings(settings);
                importer.SaveAndReimport();
            }
            const string prefabPath = "Assets/Afterglow/Prefabs/Enemy.prefab";
            var root = PrefabUtility.LoadPrefabContents(prefabPath);
            var enemy = root.GetComponent<AfterglowEnemy>();
            var figure = root.GetComponentInChildren<M01EnemySprite>(true);
            if (!figure)
            {
                foreach (var mr in enemy.visual.GetComponentsInChildren<MeshRenderer>()) mr.enabled = false;
                var go = new GameObject("Figura 2D"); go.transform.SetParent(enemy.visual, false);
                var scale = enemy.visual.lossyScale; go.transform.localScale = new Vector3(1 / scale.x, 1 / scale.y, 1 / scale.z);
                go.transform.position = root.transform.position;
                var sr = go.AddComponent<SpriteRenderer>(); figure = go.AddComponent<M01EnemySprite>(); figure.body = sr; figure.enemy = enemy;
            }
            figure.crouch = Sprite("Resisworm_0"); figure.stand = Sprite("Resisworm_1"); figure.hop = Sprite("Resisworm_2"); figure.body.sprite = figure.crouch;
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath); PrefabUtility.UnloadPrefabContents(root);
        }

        static void AddToBuildSettings(IEnumerable<string> paths)
        {
            var scenes = EditorBuildSettings.scenes.ToList();
            foreach (var p in paths) if (!scenes.Any(s => s.path == p)) scenes.Add(new EditorBuildSettingsScene(p, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
