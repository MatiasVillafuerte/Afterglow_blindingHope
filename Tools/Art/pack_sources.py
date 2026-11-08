"""Refresh and pack authored texture images in the editable source only."""
from pathlib import Path
import json
import bpy
ROOT=Path(__file__).resolve().parents[2]
path=ROOT/"ArtSource/Blender/Afterglow_M01_MineKit.blend"
bpy.ops.wm.open_mainfile(filepath=str(path))
for name,filename in (("M01_Stone","Stone_BaseColor"),("M01_Timber","Timber_BaseColor")):
    material=bpy.data.materials[name]
    image=bpy.data.images.load(str(ROOT/"Assets/Afterglow/Environment/M01Art/Textures"/(filename+".png")),check_existing=True)
    node=next((n for n in material.node_tree.nodes if n.type=="TEX_IMAGE"),None) or material.node_tree.nodes.new("ShaderNodeTexImage")
    node.image=image
    material.node_tree.links.new(node.outputs["Color"],material.node_tree.nodes.get("Principled BSDF").inputs["Base Color"])
    image.pack()
bpy.ops.wm.save_as_mainfile(filepath=str(path))
bpy.ops.wm.open_mainfile(filepath=str(path))
checks={"packed_images":[im.name for im in bpy.data.images if im.packed_file],"models":[ob.name for ob in bpy.data.objects if ob.type=="MESH"]}
assert len(checks["packed_images"])>=2 and len(checks["models"])==5,checks
(ROOT/"Docs/Arte/Verificacion_Fuente_Blend.json").write_text(json.dumps(checks,indent=2))
print("RESULT "+json.dumps(checks))
