"""Sequential FBX round-trip and installed skill's multi-view render checks."""
import json
import sys
from pathlib import Path
import bpy

ROOT=Path(__file__).resolve().parents[2]
SKILLS=Path.home()/".codex/skills"
for name in ("scenario-blender-expert","scenario-blender-export"):
    sys.path.insert(0,str(SKILLS/name/"scripts"))
import bx_export as BXE
import bx_audit
import bx_review

manifest=json.loads((ROOT/"Docs/Arte/KitManifest.json").read_text())
results=[]
for item in manifest["assets"]:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    path=ROOT/"Assets/Afterglow/Environment/M01Art/Models"/item["file"]
    bpy.ops.import_scene.fbx(filepath=str(path))
    parts=[ob for ob in bpy.context.scene.objects if ob.type=="MESH"]
    after=BXE.inspect_scene(parts)
    diffs=BXE.compare(item["beforeExport"],after,carries=("maps",))
    gate=bx_audit.deliverable(parts,profile="game")
    result={"name":item["name"],"diffs":diffs,"gate":gate,"inspection":after}
    results.append(result)
    (ROOT/"Docs/Arte/Verificacion_FBX.json").write_text(json.dumps(results,indent=2),encoding="utf-8")
    if diffs or not gate["pass"]:
        raise RuntimeError(f"Round trip failed for {item['name']}: {diffs}, {gate['errors']}")
bpy.ops.wm.open_mainfile(filepath=str(ROOT/"ArtSource/Blender/Afterglow_M01_MineKit.blend"))
for item in manifest["assets"]:
    ob=bpy.data.objects[item["name"]]
    sheet=bx_review.review([ob],str(ROOT/"Docs/Arte"/("Revision_"+ob.name)),
                           views=("front","threequarter"),modes=("silhouette","matcap","wire"),res=400)
    print("REVIEW "+sheet,flush=True)
print("RESULT "+json.dumps({"round_trips":len(results),"all_pass":True}),flush=True)
