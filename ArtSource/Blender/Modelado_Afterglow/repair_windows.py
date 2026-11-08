"""Repair only the nine patio windows, preserving every other object and collision file."""
import sys,json,shutil
from pathlib import Path
import bpy,bmesh
HERE=Path(__file__).resolve().parent
sys.path.insert(0,str(HERE))
import generate_rooms as source
scene=bpy.data.scenes['PRO_PATIO'];bpy.context.window.scene=scene
room=source.Room.__new__(source.Room);room.name=scene.name;room.scene=scene;room.parts=[]
style=json.loads((source.EXPORT/'ArtStyle.json').read_text())
source.PALETTE.update({row['name']:row['color'] for row in style['materials']})
prefixes=('Arched glass','Window carved jamb','Arched window stone','Thin window glazing bar','Window spring transom')
for obj in list(scene.objects):
    if obj.type=='MESH' and obj.name.startswith(prefixes):bpy.data.objects.remove(obj,do_unlink=True)
for level in (0,1):
    for x in (-6.1,-3,0,3,6.1):
        if level==0 and x==0:continue
        source.finish.arch(room,x,1.95+level*3.25,11.80)
source.finish.finish_room(room)
result=source.finish.audit(room)
if any(not row['pass'] for row in result):raise RuntimeError(result)
# All bars and jambs are tested with their glazing, separately from other windows.
for glass in [o for o in room.parts if o.name.startswith('Arched glass')]:
    objects=[o.name for o in room.parts if abs(o.location.x-glass.location.x)<1 and abs(o.location.z-glass.location.z)<1.5]
    check=source.finish.qa_check.run(objects=objects,profile='game',eps=.0002,tol=.002)
    if not check['pass']:raise RuntimeError(check['errors'])
room.parts=[o for o in scene.objects if o.type=='MESH']
checks=source.finish.audit(room)
(HERE/'Revision/PRO_PATIO_QA.json').write_text(json.dumps(checks,indent=2),encoding='utf-8')
if any(not row['pass'] for row in checks):raise RuntimeError('Patio mesh audit failed')
blend=HERE/'Afterglow_Prologo_Nivel1.blend'
shutil.copy2(blend,HERE/'Versiones/Antes_Ventanas.blend')
bpy.ops.wm.save_as_mainfile(filepath=str(blend))
meshes=source.evaluate(room)
axis=bpy.data.objects.new('AxisWitness',None);scene.collection.objects.link(axis);axis.location=source.xyz((-3,2,7))
source.bx_export.export(str(source.EXPORT/'PRO_PATIO.fbx'),'unity',meshes+[axis],embed_textures=False,path_mode='RELATIVE')
report=json.loads((HERE/'Auditoria.json').read_text())
for row in report:
    if row['room']=='PRO_PATIO':
        row['sourceParts']=len(room.parts)-len(meshes)
        row['triangles']=sum(len(o.data.polygons) for o in meshes)
(HERE/'Auditoria.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
print('WINDOW_REPAIR_COMPLETE')
