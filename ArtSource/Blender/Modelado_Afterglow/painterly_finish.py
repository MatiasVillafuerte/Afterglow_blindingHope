"""Reusable Afterglow surface library, ornamental props and asset review.

The same baked tiles and palette are used by Blender and Unity URP.
"""
import json
import math
import sys
from pathlib import Path

import bmesh
import bpy
import numpy as np
from mathutils import Vector

SKILLS = Path.home() / '.agents/skills'
for skill in ('blender-texture-baking', 'blender-asset-qa'):
    sys.path.insert(0, str(SKILLS / skill / 'scripts'))
import bake_maps
import qa_check
import review_shots

HERE = Path(__file__).resolve().parent
TEXTURES = Path.cwd() / 'Assets/Afterglow/Environment/Modelado/Textures'

FAMILIES = {
    'Wood': ('Oak', 'DarkOak'), 'Stone': ('Slate', 'SlateLight', 'Deep', 'Earth', 'Dust', 'Path', 'Paver'),
    'Plaster': ('Ivory', 'Plaster', 'Trim', 'Clay', 'ClayLight'), 'Fabric': ('Linen', 'Canvas'),
    'Rug': ('Velvet',), 'Grass': ('Grass', 'Leaf', 'LeafLight'),
}


def planar_uv(obj, density=1.0):
    """Continuous tiling UVs. Stacked faces intentionally reuse painted surface tiles."""
    mesh = obj.data
    layer = mesh.uv_layers.get('SurfaceTile') or mesh.uv_layers.new(name='SurfaceTile')
    for polygon in mesh.polygons:
        axis = max(range(3), key=lambda i: abs(polygon.normal[i]))
        axes = ((1, 2), (0, 2), (0, 1))[axis]
        for loop in polygon.loop_indices:
            point = mesh.vertices[mesh.loops[loop].vertex_index].co
            layer.data[loop].uv = (point[axes[0]] * density, point[axes[1]] * density)


def prepare(palette):
    palette.update({
        'Ivory': (.91,.83,.64), 'Plaster': (.69,.54,.37), 'Trim': (.84,.59,.25),
        'Clay': (.61,.25,.13), 'ClayLight': (.74,.34,.19), 'DarkOak': (.31,.14,.066),
        'Oak': (.48,.25,.105), 'Velvet': (.43,.22,.46), 'Linen': (.96,.86,.66),
        'Canvas': (.38,.44,.24), 'Glass': (.22,.46,.51), 'Water': (.22,.61,.64),
        'Grass': (.36,.49,.20), 'Leaf': (.20,.37,.15), 'LeafLight': (.42,.57,.22),
        'Rose': (.76,.32,.49), 'Earth': (.34,.24,.15), 'Dust': (.54,.40,.27),
        'Slate': (.54,.38,.29), 'SlateLight': (.70,.51,.34), 'Deep': (.35,.27,.32),
        'Crystal': (.61,.32,.71), 'Lamp': (1,.74,.32), 'Iron': (.12,.16,.15),
    })
    TEXTURES.mkdir(parents=True, exist_ok=True)
    bake_scene = bpy.data.scenes.new('Surface authoring')
    bpy.context.window.scene = bake_scene
    families = {}
    for family, members in FAMILIES.items():
        tile = bake_surface(family, bake_scene)
        for name in members:
            families[name] = family
            material = bpy.data.materials.get(name) or bpy.data.materials.new(name)
            base = next(n for n in material.node_tree.nodes if n.type == 'BSDF_PRINCIPLED')
            nodes, links = material.node_tree.nodes, material.node_tree.links
            texture = nodes.new('ShaderNodeTexImage')
            texture.image = tile
            texture.label = 'Baked brushwork shared with Unity'
            multiply = nodes.new('ShaderNodeMixRGB')
            multiply.blend_type = 'MULTIPLY'
            multiply.inputs[0].default_value = 1
            multiply.inputs[2].default_value = (*palette[name], 1)
            links.new(texture.outputs['Color'], multiply.inputs[1])
            links.new(multiply.outputs[0], base.inputs['Base Color'])
            base.inputs['Roughness'].default_value = .76
            material.diffuse_color = (*palette[name],1)
    for name in palette:
        material = bpy.data.materials.get(name) or bpy.data.materials.new(name)
        material.diffuse_color = (*palette[name],1)
        base = next(n for n in material.node_tree.nodes if n.type == 'BSDF_PRINCIPLED')
        if name not in families:
            base.inputs['Base Color'].default_value = (*palette[name],1)
        if name == 'Water':
            base.inputs['Roughness'].default_value = .22
            base.inputs['IOR'].default_value = 1.33
        if name == 'Lamp':
            base.inputs['Emission Color'].default_value = (1,.46,.12,1)
            base.inputs['Emission Strength'].default_value = .65
    entries = [{'name':name, 'color':list(color), 'family':families.get(name,'')}
               for name,color in palette.items()]
    (TEXTURES.parent/'ArtStyle.json').write_text(json.dumps({'materials':entries},indent=2),encoding='utf-8')
    return bake_scene


def bake_surface(family, scene):
    mesh = bpy.data.meshes.new('Bake tile '+family)
    mesh.from_pydata([(-1,-1,0),(1,-1,0),(1,1,0),(-1,1,0)],[],[(0,1,2,3)])
    mesh.uv_layers.new(name='UVMap')
    for data,uv in zip(mesh.uv_layers.active.data,((0,0),(1,0),(1,1),(0,1))):
        data.uv = uv
    plane = bpy.data.objects.new('Bake_'+family,mesh)
    scene.collection.objects.link(plane)
    material = bpy.data.materials.new('Procedural_'+family)
    mesh.materials.append(material)
    nodes,links = material.node_tree.nodes,material.node_tree.links
    base = next(n for n in nodes if n.type=='BSDF_PRINCIPLED')
    coord=nodes.new('ShaderNodeTexCoord')
    noise=nodes.new('ShaderNodeTexNoise')
    noise.inputs['Scale'].default_value=7 if family!='Grass' else 16
    noise.inputs['Detail'].default_value=2
    noise.inputs['Roughness'].default_value=.65
    links.new(coord.outputs['UV'],noise.inputs['Vector'])
    ramp=nodes.new('ShaderNodeValToRGB')
    ramp.color_ramp.elements[0].position=.13
    ramp.color_ramp.elements[0].color=(.53,.48,.42,1) if family=='Stone' else (.65,.63,.57,1)
    ramp.color_ramp.elements[1].position=.85
    ramp.color_ramp.elements[1].color=(.98,.95,.84,1)
    links.new(noise.outputs['Fac'],ramp.inputs[0])
    output=ramp.outputs['Color']
    if family in ('Wood','Fabric','Stone'):
        wave=nodes.new('ShaderNodeTexWave')
        wave.wave_type='BANDS'
        wave.bands_direction='X' if family!='Stone' else 'Y'
        wave.inputs['Scale'].default_value=13 if family=='Wood' else 28 if family=='Fabric' else 3.5
        wave.inputs['Distortion'].default_value=4 if family=='Wood' else 1.5
        wave.inputs['Detail Scale'].default_value=1.2
        links.new(coord.outputs['UV'],wave.inputs['Vector'])
        mix=nodes.new('ShaderNodeMixRGB')
        mix.blend_type='MULTIPLY';mix.inputs[0].default_value=.20 if family=='Wood' else .085
        links.new(output,mix.inputs[1]);links.new(wave.outputs['Color'],mix.inputs[2]);output=mix.outputs[0]
    links.new(output,base.inputs['Base Color'])
    base.inputs['Roughness'].default_value=.79
    result=bake_maps.run(low=plane.name,maps=['basecolor','roughness'],res=512,samples=8,
                         margin_px=8,device='CPU',out=str(TEXTURES),name=family)
    # The packed data texture keeps Unity's smoothness in alpha.
    rough=bpy.data.images.get('T_'+family+'_R')
    if rough is None:
        rough=bpy.data.images.load(str(TEXTURES/('T_'+family+'_R.png')),check_existing=True)
    pixels=np.empty(512*512*4,dtype=np.float32);rough.pixels.foreach_get(pixels)
    rgba=np.zeros((512*512,4),dtype=np.float32)
    rgba[:,3]=1-pixels.reshape(-1,4)[:,0]
    packed=bpy.data.images.new('T_'+family+'_MS',512,512,alpha=True,is_data=True)
    packed.pixels.foreach_set(rgba.ravel());packed.filepath_raw=str(TEXTURES/(packed.name+'.png'))
    packed.file_format='PNG';packed.save();packed.pack()
    tile=bpy.data.images.get('T_'+family+'_BC')
    if tile is None:
        tile=bpy.data.images.load(str(TEXTURES/('T_'+family+'_BC.png')),check_existing=True)
    if family=='Rug':
        # A woven medallion and repeating flower border, authored in the baked tile.
        p=np.empty(512*512*4,dtype=np.float32);tile.pixels.foreach_get(p)
        p=p.reshape(512,512,4)
        yy,xx=np.mgrid[0:512,0:512]/511
        x,y=(xx-.5)*2,(yy-.5)*2
        radius=np.sqrt(x*x+y*y);theta=np.arctan2(y,x)
        medallion=(radius<.43+.07*np.cos(theta*8))&(radius>.24+.04*np.cos(theta*8))
        border=(np.maximum(abs(x),abs(y))>.86)&(np.maximum(abs(x),abs(y))<.94)
        flowers=(np.cos(xx*math.tau*8)*np.cos(yy*math.tau*8)>.56)&(np.maximum(abs(x),abs(y))>.65)
        p[medallion|border|flowers,:3]=(.99,.80,.40)
        tile.pixels.foreach_set(p.ravel());tile.save()
    tile.pack()
    bpy.data.objects.remove(plane,do_unlink=True)
    return tile


def arch(room,x,y,z,width=1.65,height=2.25):
    r=width*.5
    outline=[(-r,-height*.5),(r,-height*.5)]
    outline += [(math.cos(a)*r,height*.5-r+math.sin(a)*r) for a in np.linspace(0,math.pi,17)]
    count=len(outline)
    vertices=[(a,-d,b) for d in (-.10,.10) for a,b in outline]
    faces=[tuple(reversed(range(count))),tuple(range(count,2*count))]
    faces += [(i,(i+1)%count,(i+1)%count+count,i+count) for i in range(count)]
    glass=room.mesh('Arched glass',vertices,faces,'Glass',(x,y,z))
    for sign in (-1,1):
        room.box('Window carved jamb',(x+sign*r,y-r*.5,z-.15),(.12,height-r,.18),'Ivory',.03)
    for a,b in zip(np.linspace(0,math.pi,17)[:-1],np.linspace(0,math.pi,17)[1:]):
        room.segment('Arched window stone',(x+r*math.cos(a),y+height*.5-r+r*math.sin(a),z-.16),
                     (x+r*math.cos(b),y+height*.5-r+r*math.sin(b),z-.16),.13,.18,'Ivory')
    for dx in (-.31,0,.31):
        bar_height=height-r+math.sqrt(r*r-dx*dx)-.04
        room.box('Thin window glazing bar',(x+dx,y-height*.5+bar_height*.5,z-.12),(.045,bar_height,.06),'Linen',.01)
    room.box('Window spring transom',(x,y+height*.5-r,z-.15),(width,.05,.06),'Linen',.01)


def crown(room,x,y,z,width,material='Trim'):
    # Thick extruded triangular pediment with a recessed, contrasting tympanum.
    vertices=[(-width/2,-.17,0),(width/2,-.17,0),(0,-.17,.76),(-width/2,.17,0),(width/2,.17,0),(0,.17,.76)]
    room.mesh('Classical pediment',vertices,[(2,1,0),(3,4,5),(0,1,4,3),(1,2,5,4),(2,0,3,5)],material,(x,y,z))
    room.segment('Pediment left cornice',(x-width*.5,y,z-.21),(x,y+.76,z-.21),.14,.15,'Ivory')
    room.segment('Pediment right cornice',(x,y+.76,z-.21),(x+width*.5,y,z-.21),.14,.15,'Ivory')


def decorate(rooms):
    for room in rooms:
        bpy.context.window.scene=room.scene
        if room.name=='PRO_PATIO':
            # Replace the rectangular glazing by arched windows at the same positions.
            for obj in list(room.parts):
                if obj.name.startswith(('Terracotta window surround','Inset window glass','Window vertical mullion','Window transom')):
                    room.parts.remove(obj);bpy.data.objects.remove(obj,do_unlink=True)
            for level in (0,1):
                for x in (-6.1,-3,0,3,6.1):
                    if level==0 and x==0:continue
                    arch(room,x,1.95+level*3.25,11.80)
            crown(room,0,7.05,11.76,5.4)
            for i in range(26):
                for x in (-13.9,13.9):
                    if x>0 and -9.6<(-14+i*1.24)<-5.3:continue
                    room.box('Fence foundation',(x,.20,-14+i*1.24),(.40,.4,1.26),'Plaster',.04)
            room.box('Front fence foundation',(0,.2,-15),(28,.4,.4),'Plaster',.04)
            for x,z in [(-14,-15),(-7,-15),(0,-15),(7,-15),(14,-15),(-14,1),(-14,9),(14,1),(14,9)]:
                room.box('Limestone fence pier',(x,1.20,z),(.47,2.40,.47),'Ivory',.08)
                room.box('Pier cap',(x,2.42,z),(.67,.16,.67),'Trim',.04)
            for x,z in [(-10.8,9),(10.7,8.8),(-9,-7)]:
                for i in range(9):
                    angle=i*math.tau/9
                    room.rock('Blossoming crown',(x+math.cos(angle)*.9,3.0+(i%3)*.28,z+math.sin(angle)*.8),(.65,.65,.65),'Rose',10)
        if room.name=='PRO_CASA':
            room.box('Bedside cabinet',(.45,.58,2.65),(.83,1.16,.76),'Oak',.09)
            room.box('Bedside top',(.45,1.17,2.65),(.92,.12,.87),'Ivory',.06)
            room.lathe('Bedside brass lamp',(.45,1.23,2.65),[(.19,0),(.23,.06),(.10,.15),(.06,.56),(.11,.61)],'Trim',20)
            room.lathe('Cream lampshade',(.45,1.77,2.65),[(.36,0),(.20,.48)],'Linen',24)
            room.box('Mineral display shelf',(10.4,4.4,5.55),(2.28,.14,.48),'Oak',.04)
            for x,y in [(9.7,4.47),(10.4,4.47),(11.0,4.47)]:
                room.lathe('Inanimate mineral',(x,y,5.44),[(.18,0),(.21,.12),(.15,.43),(.025,.66)],'Crystal',6)
            for x in (9.7,11.1):
                room.segment('Shelf bracket',(x,4.01,5.78),(x,4.34,5.40),.11,.11,'Trim')
            crown(room,10.44,3.56,4.48,2.10,'Oak')
            for x in (1.4,3.6):
                room.lathe('Headboard carved finial',(x,2.8,4.82),[(.11,0),(.19,.13),(.17,.27),(.05,.40)],'Trim',16)
            for i in range(4):
                room.box('Bound book',(6.33+i*.16,1.27,3.77),(.12,.27+(i%2)*.08,.33),'Canvas' if i%2 else 'Clay',.02)
        if room.name.startswith('N01S'):
            for x,z in [(-8,1),(8,6)]:
                room.box('Gallery support post',(x,1.7,z),(.32,3.4,.36),'Oak',.065)
                room.segment('Gallery supporting bracket',(x,3,z),(x-1.1 if x>0 else x+1.1,3.5,z),.27,.32,'DarkOak')
                room.box('Forged support collar',(x,2.7,z),(.39,.19,.42),'Iron',.025)
            for x,z in [(8,-4),(-8,5)]:
                room.lathe('Coopered barrel',(x,0,z),[(.37,0),(.45,.18),(.49,.55),(.45,.95),(.37,1.1)],'Oak',18)
                for h in (.15,.94):
                    room.lathe('Barrel iron hoop',(x,h,z),[(.455,0),(.466,.10)],'Iron',20)


def finish_room(room):
    for obj in room.parts:
        mesh=obj.data
        bm=bmesh.new();bm.from_mesh(mesh)
        bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(mesh);bm.free()
        planar_uv(obj, .65 if obj.data.materials[0].name not in ('Velvet',) else .22)
    room.scene.view_settings.view_transform='Standard'


def audit(room):
    """Each loose room prop is a separate asset. Check topology on every authored mesh."""
    bpy.context.window.scene=room.scene
    results=[]
    for obj in room.parts:
        result=qa_check.run(objects=[obj.name],profile='game',eps=.0002,tol=.002)
        results.append({'object':obj.name,'pass':result['pass'],'errors':result['errors'],
                        'warnings':result['warnings']})
    return results
