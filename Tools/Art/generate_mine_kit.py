"""Afterglow N01S02: deterministic editable mine kit. Run with Blender 5.2 -b.

Uses the installed Scenario skills for hard-surface stacks, delivery audits and
the Unity FBX preset. Own assets only; no downloaded game models or textures.
"""
import argparse
import json
import math
import random
import sys
from pathlib import Path
from array import array

import bpy
import bmesh
from mathutils import Matrix, Vector

ROOT = Path(__file__).resolve().parents[2]
SKILLS = Path.home() / ".codex" / "skills"
for folder in ("scenario-blender-expert", "scenario-blender-hard-surface", "scenario-blender-export"):
    sys.path.insert(0, str(SKILLS / folder / "scripts"))
import bx_audit
import bx_hardsurface as HS
import bx_export as BXE

OUTPUT = ROOT / "Assets/Afterglow/Environment/M01Art"
SOURCE = ROOT / "ArtSource/Blender"
REPORTS = ROOT / "Docs/Arte"


def material(name, color, roughness=.8, metallic=0):
    mat = bpy.data.materials.new(name)
    mat.diffuse_color = (*color, 1)
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    bsdf.inputs["Base Color"].default_value = (*color, 1)
    bsdf.inputs["Roughness"].default_value = roughness
    bsdf.inputs["Metallic"].default_value = metallic
    return mat


def texture(name, base, wood=False):
    size = 256
    image = bpy.data.images.new(name, width=size, height=size, alpha=False)
    image.colorspace_settings.name = "sRGB"
    pixels = array("f")
    for y in range(size):
        for x in range(size):
            u, v = x / size, y / size
            if wood:
                wave = math.sin(u * 105 + math.sin(v * 12) * 2 + math.sin(u * 22 + v * 9))
                detail = math.sin(u * 233 + math.sin(v * 6) * 3)
                factor = .90 + .08 * wave + .025 * detail
            else:
                factor = .94 + .04 * math.sin(u * 17 + math.sin(v * 23)) + .035 * math.cos(v * 27 + u * 11)
            pixels.extend((*[max(0, min(1, c * factor)) for c in base], 1))
    image.pixels.foreach_set(pixels)
    image.filepath_raw = str(OUTPUT / "Textures" / (name + ".png"))
    image.file_format = "PNG"
    image.save()
    return image


def map_texture(mat, image):
    node = next((n for n in mat.node_tree.nodes if n.type=="TEX_IMAGE"),None) or mat.node_tree.nodes.new("ShaderNodeTexImage")
    node.image = image
    mat.node_tree.links.new(node.outputs["Color"], mat.node_tree.nodes.get("Principled BSDF").inputs["Base Color"])


def mesh_object(name, verts, faces, mat):
    mesh = bpy.data.meshes.new(name)
    mesh.from_pydata(verts, [], faces)
    mesh.validate()
    mesh.update()
    ob = bpy.data.objects.new(name, mesh)
    bpy.context.collection.objects.link(ob)
    ob.data.materials.append(mat)
    ob["afterglow_generated"] = True
    return ob


def beam(name, a, b, width, depth, mat):
    a, b = Vector(a), Vector(b)
    direction = b - a
    transform = Matrix.Translation((a+b)/2) @ direction.to_track_quat("Z", "Y").to_matrix().to_4x4()
    verts = [transform @ Vector((x*width/2, y*depth/2, z*direction.length/2))
             for x, y, z in ((-1,-1,-1),(-1,-1,1),(-1,1,-1),(-1,1,1),
                            (1,-1,-1),(1,-1,1),(1,1,-1),(1,1,1))]
    faces = [(0,4,6,2),(1,3,7,5),(0,1,5,4),(2,6,7,3),(0,2,3,1),(4,5,7,6)]
    return mesh_object(name, verts, faces, mat)


def merge_solids(parts, name):
    """Exact unions remove internal faces; source base remains editable polygon mesh."""
    ob = parts[0]
    bpy.context.view_layer.objects.active = ob
    ob.select_set(True)
    for other in parts[1:]:
        mod = ob.modifiers.new("Joint union", "BOOLEAN")
        mod.operation = "UNION"
        mod.solver = "EXACT"
        mod.material_mode = "TRANSFER"
        mod.object = other
        with bpy.context.temp_override(object=ob, active_object=ob, selected_objects=[ob]):
            result = bpy.ops.object.modifier_apply(modifier=mod.name)
        assert result == {"FINISHED"}, result
        bpy.data.objects.remove(other, do_unlink=True)
    ob.name = name
    bm = bmesh.new()
    bm.from_mesh(ob.data)
    bmesh.ops.recalc_face_normals(bm, faces=list(bm.faces))
    bm.to_mesh(ob.data)
    bm.free()
    return ob


def unwrap(ob):
    bpy.ops.object.select_all(action="DESELECT")
    ob.select_set(True)
    bpy.context.view_layer.objects.active = ob
    bpy.ops.object.mode_set(mode="EDIT")
    bpy.ops.mesh.select_all(action="SELECT")
    result = bpy.ops.uv.smart_project(angle_limit=math.radians(66), island_margin=.025)
    assert result == {"FINISHED"}
    bpy.ops.object.mode_set(mode="OBJECT")


def rock(name, size, seed, mat):
    rng = random.Random(seed)
    bm = bmesh.new()
    sx, sy, sz = size
    for z, radius in ((0,.72),(.18,1),(.65,.89),(1,.43)):
        for i in range(8):
            angle = i*math.tau/8 + (.13 if z>.3 else 0)
            variation = rng.uniform(.84,1.10)
            bm.verts.new((math.cos(angle)*sx*.5*radius*variation,
                          math.sin(angle)*sy*.5*radius*rng.uniform(.9,1.05),
                          sz*z*(rng.uniform(.94,1.04) if z else 1)))
    bmesh.ops.convex_hull(bm, input=list(bm.verts), use_existing_faces=False)
    loose = [v for v in bm.verts if not v.link_faces]
    if loose:
        bmesh.ops.delete(bm, geom=loose, context="VERTS")
    bmesh.ops.recalc_face_normals(bm, faces=list(bm.faces))
    mesh = bpy.data.meshes.new(name)
    bm.to_mesh(mesh)
    bm.free()
    ob = bpy.data.objects.new(name, mesh)
    bpy.context.collection.objects.link(ob)
    ob.data.materials.append(mat)
    ob["afterglow_seed"] = seed
    return ob


def rail_profile(name, y, mat):
    outline = [(-.105,.14),(.105,.14),(.105,.21),(.037,.21),(.037,.33),(.075,.33),
               (.075,.40),(-.075,.40),(-.075,.33),(-.037,.33),(-.037,.21),(-.105,.21)]
    n = len(outline)
    verts = [(x, y+u, z) for x in (-1.5,1.5) for u,z in outline]
    faces = [tuple(reversed(range(n))),tuple(range(n,2*n))]
    faces += [(i,(i+1)%n,(i+1)%n+n,i+n) for i in range(n)]
    ob = mesh_object(name,verts,faces,mat)
    bm=bmesh.new();bm.from_mesh(ob.data);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(ob.data);bm.free()
    return ob


def main():
    for folder in (OUTPUT/"Models", OUTPUT/"Textures", SOURCE, REPORTS):
        folder.mkdir(parents=True, exist_ok=True)
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)
    bpy.context.scene.unit_settings.system="METRIC"
    bpy.context.scene.unit_settings.scale_length=1
    stone=material("M01_Stone",(.32,.41,.47))
    wood=material("M01_Timber",(.57,.37,.21))
    iron=material("M01_Iron",(.19,.23,.27),.5,.55)
    map_texture(stone,texture("Stone_BaseColor",(.47,.56,.62)))
    map_texture(wood,texture("Timber_BaseColor",(.30,.15,.07),True))
    assets=[rock("Rock_A",(1.8,1.25,1.05),40201,stone),
            rock("Rock_B",(1.3,1.05,2.1),40202,stone),
            rock("Rock_C",(2.15,1.6,1.25),40203,stone)]
    support=merge_solids([
        beam("PostL",(-1.95,0,0),(-1.76,0,4.25),.40,.48,wood),
        beam("PostR",(1.95,0,0),(1.76,0,4.25),.40,.48,wood),
        beam("Crown",(-2.15,0,4.15),(2.15,0,4.15),.42,.53,wood),
        beam("BraceL",(-1.8,0,2.95),(-.70,0,4.15),.24,.34,wood),
        beam("BraceR",(1.8,0,2.95),(.70,0,4.15),.24,.34,wood)],"TimberSupport")
    HS.hard_surface_stack(support,micro_width=.025,micro_segments=2)
    assets.append(support)
    ties=[beam("Tie",(x,-.74,.085),(x,.74,.085),.17,.22,wood) for x in (-1.2,-.4,.4,1.2)]
    rail=merge_solids([*ties,rail_profile("RailL",-.46,iron),rail_profile("RailR",.46,iron)],"RailModule")
    HS.hard_surface_stack(rail,micro_width=.009,micro_segments=1)
    assets.append(rail)
    report={"blender":bpy.app.version_string,"seed":40201,"assets":[]}
    for ob in assets:
        unwrap(ob)
        BXE.ground([ob])
        bpy.context.view_layer.update()
        source_gate=bx_audit.deliverable([ob],profile="game")
        export_ob,_=HS.game_version(ob,name=ob.name+"_Export",micro_segments=2 if ob==support else 1)
        gate=bx_audit.deliverable([export_ob],profile="game")
        evaluation=export_ob.evaluated_get(bpy.context.evaluated_depsgraph_get()).to_mesh()
        evaluation.calc_loop_triangles()
        dimensions=list(ob.dimensions)
        item={"name":ob.name,"file":ob.name+".fbx","dimensionsBlender":dimensions,
              "triangles":len(evaluation.loop_triangles),"vertices":len(evaluation.vertices),
              "uvCount":len(evaluation.uv_layers),"materials":[m.name for m in ob.data.materials],
              "sourceGate":source_gate,"gate":gate,"beforeExport":BXE.inspect_scene([export_ob])}
        export_ob.evaluated_get(bpy.context.evaluated_depsgraph_get()).to_mesh_clear()
        report["assets"].append(item)
        (REPORTS/"KitManifest.json").write_text(json.dumps(report,indent=2),encoding="utf-8")
        if not gate["pass"]:
            raise RuntimeError(f"Asset gate failed: {ob.name}: {gate['errors']}")
        BXE.export(str(OUTPUT/"Models"/item["file"]),"unity",[export_ob],embed_textures=False,path_mode="RELATIVE")
        bpy.data.objects.remove(export_ob,do_unlink=True)
    # Artist-friendly layout, with editable base meshes and live bevel/normal stacks.
    for ob,x in zip(assets,(-5.6,-3.0,-.5,4.0,8.5)):
        ob.location.x=x
    world=bpy.data.worlds.new("MineKit studio")
    world.color=(.22,.26,.31)
    bpy.context.scene.world=world
    lamp_data=bpy.data.lights.new("Studio key","SUN");lamp_data.energy=3
    lamp=bpy.data.objects.new("Studio key",lamp_data);bpy.context.collection.objects.link(lamp);lamp.rotation_euler=(.6,-.4,-.4)
    camera_data=bpy.data.cameras.new("Kit camera");camera=bpy.data.objects.new("Kit camera",camera_data);bpy.context.collection.objects.link(camera)
    camera.location=(11,-17,10);camera.rotation_euler=(Vector((1.5,0,1.8))-camera.location).to_track_quat("-Z","Y").to_euler();camera_data.type="ORTHO";camera_data.ortho_scale=18;bpy.context.scene.camera=camera
    bpy.context.scene.render.resolution_x=1280;bpy.context.scene.render.resolution_y=720;bpy.context.scene.render.resolution_percentage=100
    for mat,name in ((stone,"Stone_BaseColor"),(wood,"Timber_BaseColor")):
        image=bpy.data.images.load(str(OUTPUT/"Textures"/(name+".png")),check_existing=True)
        map_texture(mat,image)
        image.pack()
    bpy.ops.wm.save_as_mainfile(filepath=str(SOURCE/"Afterglow_M01_MineKit.blend"))
    print("RESULT "+json.dumps({"assets":len(assets),"all_gates_pass":True,"blend":str(SOURCE/"Afterglow_M01_MineKit.blend")}))


if __name__=="__main__":
    main()
