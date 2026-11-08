"""N1 authored layout in Unity metres. Blender 5.2; does not rebuild prologue/map.
Run: blender -b --python-exit-code 1 -P .../build_n1.py [-- --detail]
"""
from pathlib import Path
import sys,json
import bpy
import bmesh
ROOT=Path.cwd(); SOURCE=Path(__file__).resolve().parent
sys.path.insert(0,str(SOURCE.parent))
import generate_rooms as kit
import painterly_finish as finish
sys.path.insert(0,str(Path.home()/'.agents/skills/blender-asset-qa/scripts'))
import qa_check,review_shots
DETAIL='--detail' in sys.argv
OUT=ROOT/'Assets/Afterglow/Environment/Modelado/Nivel1'
REVIEW=SOURCE/('Revision_Final' if DETAIL else 'Revision_Blockout')
OUT.mkdir(parents=True,exist_ok=True);REVIEW.mkdir(exist_ok=True)
for key,color in {'BridgeDeck':(.52,.31,.12),'LeverArm':(.78,.46,.12),'Counterweight':(.28,.25,.31)}.items():kit.PALETTE[key]=color

def slab(r,name,x,z,w,d,top=0,mat='Dust'):
    height=top+3
    obj=r.box(name,(x,top-height/2,z),(w,height,d),mat,.035,True)
    obj['terrain']=True
    return obj
def wall(r,name,x,z,w,d,h=3.6):
    return r.box(name,(x,h/2,z),(w,h,d),'Slate',.08,True)
def portal(r,x,y,z):
    if z<0:
        for dx in [-1.6,1.6]:r.box('Return_marker',(x+dx,y+.45,z),(.23,.9,.23),'Oak',.04)
        return
    kit.portal(r,x,y,z)
def room1():
    r=kit.Room('N01S01')
    slab(r,'Entry_apron',0,-4,12,8)
    slab(r,'Left_work_bay',-7,1,8,7)
    slab(r,'Right_work_bay',6,3,8,6)
    slab(r,'Central_refuge',0,2,8,6)
    slab(r,'Tunnel_neck',1,6.75,5,3.5)
    # Keep the existing village trail and interaction coordinates.
    slab(r,'Village_path',-8,-3.8,6,2.5)
    wall(r,'Back_left',-5,6,7,1);wall(r,'Back_right',7,6.8,6,1)
    portal(r,1,0,8.5)
    r.lathe('Altar_plinth',(4,0,-3),[(.75,0),(.79,.12),(.61,.26),(.55,.74),(.79,1.03)],'SlateLight',12)
    r.rock('Altar_emblem',(4,1.31,-3),(.49,.64,.36),'Crystal')
    return r
def room2():
    r=kit.Room('N01S02')
    slab(r,'Arrival_shelf',-8,-5.1,6,5.8)
    slab(r,'Lateral_approach',-2.8,-3.5,4.4,3)
    slab(r,'Safe_jump_base',2,-2,5,5,-.35)
    slab(r,'First_terrace',2,-2,3,4,.65,'SlateLight')
    slab(r,'Second_terrace',5,1,4,3,1.3,'SlateLight')
    slab(r,'Third_terrace',6,4.5,4,3,1.95,'SlateLight')
    slab(r,'High_arrival',7.5,8.5,7,5,2.2,'SlateLight')
    wall(r,'Back_mass',7.5,11.5,8,1,5)
    portal(r,-8,0,-7.7);portal(r,8,2.2,10.5)
    return r
def room3():
    r=kit.Room('N01S03')
    slab(r,'Arrival_lane',6,-6.2,5,6)
    slab(r,'Combat_fan',2,-.6,16,8)
    slab(r,'Optional_work_bay',-5,5,7,8)
    slab(r,'Exit_neck',-6,10.3,3,3.6)
    wall(r,'Ore_mass',1.4,5.5,4,4,3.8)
    wall(r,'Neck_left',-8.5,10.5,2,4)
    wall(r,'Neck_right',-3.5,10.5,2,4)
    # Kept separate by the exporter's existing BreakableBoulder group.
    r.rock('BreakableBoulder',(-6,.68,9),(2.65,1.4,1.3),'SlateLight')
    portal(r,6,0,-9.2);portal(r,-6,0,11.9)
    return r
def room4():
    r=kit.Room('N01S04')
    slab(r,'South_bank',3,-4,12,8)
    slab(r,'North_bank',2,7.5,14,5)
    slab(r,'West_turn',-5,6.5,4,3)
    slab(r,'Final_step_1',-4,6,4,2,.6,'SlateLight')
    slab(r,'Final_step_2',-4,8,4,2,1.2,'SlateLight')
    slab(r,'Investigation_ledge',-4,10,6,2,1.8,'SlateLight')
    slab(r,'Helmet_recess',8,8,2,4)
    wall(r,'Derrumbe',-4,11.6,8,1.1,4.7)
    # Deck exported open, pivot in Unity at (2,0,0). Its collider is dynamic.
    r.box('Bridge_deck',(2,-.12,2.5),(3,.24,5.2),'BridgeDeck',.04)
    r.box('Lever_base',(4,.35,-1.2),(.8,.7,.8),'Iron',.08)
    r.box('Lever_arm',(4,.98,-1.2),(.14,.9,.14),'LeverArm',.03)
    r.box('Counterweight',(4,2,0),(.85,.9,.7),'Counterweight',.08)
    portal(r,5,0,-8.2)
    return r

def decorate(r):
    # Art is added only after the blockout has passed the Unity route gate.
    if not DETAIL:return
    # Backdrop beyond the playable silhouette; no ceiling or foreground wall.
    for i in range(12):
        x=-12+i*2.2
        r.rock('Cavern_backdrop',(x,2.2,15.8+(i%2)*.65),(3.3,9+(i%3)*.7,3.1),'Deep' if i%3==0 else 'Slate')
    for box in list(r.colliders):
        if 'Back_' in box['name'] or box['name'] in ('Back_mass','Derrumbe','Ore_mass'):
            x,y,z=box['center'];w,h,d=box['size']
            for i in range(max(2,int(w/1.8))):
                r.rock('Stratum_'+box['name'],(x-w/2+1+i*1.5,y,z-.45),(2.1,h+.3,1.3),'SlateLight' if i%3==0 else 'Slate')
    lamps={'N01S01':[(-4,2.7,5),(4,2.5,5)],'N01S02':[(9,4.6,9)],'N01S03':[(2,2.7,4),(-8,2.8,6)],'N01S04':[(4,2.8,-2),(-7,4.2,10)]}
    for x,y,z in lamps[r.name]:
        r.box('Lamp_post',(x,y/2,z),(.22,y,.22),'Oak',.04);kit.lantern(r,x,y,z-.23)
    if r.name=='N01S01':
        for x,z in [(-7,3.9),(7,5.3)]:
            r.box('Workbench_top',(x,.85,z),(2.7,.16,.8),'Oak',.06,True)
            for dx in [-1.1,1.1]:r.box('Workbench_leg',(x+dx,.4,z),(.18,.8,.55),'DarkOak',.04)
            r.box('Tool_rack',(x,1.7,z+.35),(2.4,.15,.12),'DarkOak',.03)
            for dx in [-.7,0,.7]:
                r.box('Tool_handle',(x+dx,1.5,z+.26),(.07,.65,.08),'Oak',.02)
                r.box('Tool_head',(x+dx,1.78,z+.23),(.38,.09,.13),'Iron',.02)
        kit.track(r,(1,.025,1),(1,.025,7.9))
        for x,z in [(-4,-5),(4,3)]:r.rock('Mineral_outcrop',(x,.35,z),(.55,.7,.55),'Crystal',6)
    if r.name=='N01S02':
        for x,z,h in [(2,-2,.65),(5,1,1.3),(6,4.5,1.95),(7.5,8.5,2.2)]:
            r.box('Terrace_front_trim',(x,h-.16,z-1.42),(2.8,.25,.18),'Oak',.05)
            for dx in [-1.1,1.1]:r.box('Terrace_timber',(x+dx,(h-3)/2,z-1.4),(.2,h+3,.22),'DarkOak',.035)
        kit.track(r,(-9,0,-5),(-6,0,-4))
    if r.name=='N01S03':
        kit.track(r,(6,.025,-7),(4,.025,-4));kit.track(r,(4,.025,-4),(-1,.025,0))
        kit.crate(r,-7.6,4.2);kit.crate(r,7,-3.7)
        for i in range(4):r.rock('Ore_on_workbench',(-1+i*.38,.25,2.8),(.4,.5,.38),'Crystal',6)
    if r.name=='N01S04':
        for z in [i*.4 for i in range(13)]:r.box('Deck_plank',(2,.025,z),(2.96,.08,.32),'BridgeDeck',.025)
        r.segment('Counterweight_frame',(4,0,0),(4,3.4,0),.2,.22,'Oak')
        r.segment('Counterweight_cable',(4,2.45,0),(4,3.2,0),.04,.04,'Iron')

def join_terrain(r):
    """Exact union removes coincident floor surfaces, retaining the collider plan."""
    parts=[o for o in r.parts if o.get('terrain')]
    for obj in parts:obj.modifiers.clear()
    base=parts[0];base.name='Authored_terrain'
    for other in parts[1:]:
        mod=base.modifiers.new('Union_walkable_mass','BOOLEAN');mod.operation='UNION';mod.solver='EXACT';mod.object=other
        with bpy.context.temp_override(object=base,active_object=base,selected_objects=[base],selected_editable_objects=[base]):bpy.ops.object.modifier_apply(modifier=mod.name)
        r.parts.remove(other);bpy.data.objects.remove(other,do_unlink=True)
    bm=bmesh.new();bm.from_mesh(base.data);bmesh.ops.remove_doubles(bm,verts=list(bm.verts),dist=.00001);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(base.data);bm.free()
    bevel=base.modifiers.new('Rounded_rock_edges','BEVEL');bevel.width=.045;bevel.segments=2;bevel.limit_method='ANGLE'
    with bpy.context.temp_override(object=base,active_object=base,selected_objects=[base],selected_editable_objects=[base]):bpy.ops.object.modifier_apply(modifier=bevel.name)
    bm=bmesh.new();bm.from_mesh(base.data);bmesh.ops.remove_doubles(bm,verts=list(bm.verts),dist=.0001);bmesh.ops.dissolve_degenerate(bm,edges=list(bm.edges),dist=.0001);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(base.data);bm.free()
    base.modifiers.new('Weighted_corner_normals','WEIGHTED_NORMAL')
    # Jump-separated terrain masses are separate assets, not detached pieces of one prop.
    mesh=base.data;adj={v.index:set() for v in mesh.vertices}
    for e in mesh.edges:a,b=e.vertices;adj[a].add(b);adj[b].add(a)
    unseen=set(adj);islands=[]
    while unseen:
        todo=[unseen.pop()];group=set(todo)
        while todo:
            for v in adj[todo.pop()]:
                if v in unseen:unseen.remove(v);group.add(v);todo.append(v)
        islands.append(group)
    if len(islands)>1:
        for i,group in enumerate(islands):
            indices=sorted(group);mapping={old:new for new,old in enumerate(indices)}
            vertices=[tuple(mesh.vertices[j].co) for j in indices]
            faces=[tuple(mapping[j] for j in f.vertices) for f in mesh.polygons if all(j in group for j in f.vertices)]
            obj=r.mesh('Terrain_mass_'+str(i),vertices,faces,'Dust');obj.matrix_world=base.matrix_world.copy()
            obj.modifiers.new('Weighted_corner_normals','WEIGHTED_NORMAL')
        r.parts.remove(base);bpy.data.objects.remove(base,do_unlink=True)

def main():
    starter=bpy.context.scene
    if DETAIL:
        source=SOURCE.parent/'Afterglow_Prologo_Nivel1.blend'
        with bpy.data.libraries.load(str(source),link=False) as (available,target):target.materials=[n for n in available.materials if n in kit.PALETTE]
        for name in kit.PALETTE:
            mat=bpy.data.materials.get(name)
            if mat:kit.PALETTE[name]=tuple(mat.diffuse_color[:3])
    rooms=[room1(),room2(),room3(),room4()]
    bpy.data.scenes.remove(starter)
    report=[]
    for r in rooms:
        bpy.context.window.scene=r.scene
        join_terrain(r)
        decorate(r)
        for obj in r.parts:finish.planar_uv(obj,.65)
        checks=finish.audit(r)
        (REVIEW/(r.name+'_QA.json')).write_text(json.dumps(checks,indent=2),encoding='utf-8')
        if any(not c['pass'] for c in checks):raise RuntimeError('QA failed '+r.name)
        parts=kit.evaluate(r)
        axis=bpy.data.objects.new('AxisWitness',None);r.scene.collection.objects.link(axis);axis.location=kit.xyz((-3,2,7))
        kit.bx_export.export(str(OUT/(r.name+'.fbx')),'unity',parts+[axis],embed_textures=False,path_mode='RELATIVE')
        report.append({'room':r.name,'triangles':sum(len(o.data.polygons) for o in parts),'sourceObjects':len(r.parts)-len(parts),'stage':'detail' if DETAIL else 'blockout'})
        for obj in parts+[axis]:bpy.data.objects.remove(obj,do_unlink=True)
        r.parts=[o for o in r.scene.objects if o.type=='MESH']
        (OUT/(r.name+'.json')).write_text(json.dumps({'boxes':r.colliders}),encoding='utf-8')
        for mode in ['random','wire']:
            review_shots.run(objects=[o.name for o in r.parts],out=str(REVIEW/(r.name+'_'+mode+'.png')),mode=mode,res=320)
        collections={name:bpy.data.collections.new(r.name+'_'+name) for name in ['Terrain','Architecture','Mechanisms','Breakable','Decoration']}
        for collection in collections.values():r.scene.collection.children.link(collection)
        for obj in r.parts:
            kind='Terrain' if obj.name.startswith(('Authored_terrain','Terrain_mass')) else 'Breakable' if obj.name.startswith('BreakableBoulder') else 'Mechanisms' if obj.data.materials[0].name in ['BridgeDeck','LeverArm','Counterweight'] else 'Architecture' if any(s in obj.name for s in ['Back_','Neck_','Ore_mass','Derrumbe','jamb','footing']) else 'Decoration'
            collections[kind].objects.link(obj)
            for collection in list(obj.users_collection):
                if collection!=collections[kind]:collection.objects.unlink(obj)
    bpy.context.window.scene=rooms[0].scene
    bpy.ops.wm.save_as_mainfile(filepath=str(SOURCE/('N1_Final.blend' if DETAIL else 'N1_Blockout.blend')))
    (SOURCE/'Auditoria.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
    print('RESULT '+json.dumps(report))
if __name__=='__main__':main()
