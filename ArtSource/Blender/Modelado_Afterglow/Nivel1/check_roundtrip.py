"""Read-only FBX round trip; compare exported topology and independent parts."""
import bpy, json
from pathlib import Path
root=Path(__file__).resolve().parents[4]
source=Path(__file__).resolve().parent
expected=json.loads((source/'Auditoria.json').read_text())
results=[]
for item in expected:
    bpy.ops.wm.read_factory_settings(use_empty=True)
    path=root/'Assets/Afterglow/Environment/Modelado/Nivel1'/(item['room']+'.fbx')
    bpy.ops.import_scene.fbx(filepath=str(path))
    meshes=[o for o in bpy.context.scene.objects if o.type=='MESH']
    tris=0
    for obj in meshes:
        obj.data.calc_loop_triangles();tris+=len(obj.data.loop_triangles)
    names=[o.name for o in meshes]
    required=['Export_BreakableBoulder'] if item['room']=='N01S03' else ['Export_BridgeDeck','Export_LeverArm','Export_Counterweight'] if item['room']=='N01S04' else []
    ok=tris==item['triangles'] and all(n in names for n in required)
    results.append({'room':item['room'],'pass':ok,'triangles':tris,'meshes':names,'requiredParts':required})
(source/'Roundtrip.json').write_text(json.dumps(results,indent=2),encoding='utf-8')
if not all(r['pass'] for r in results):raise RuntimeError('FBX roundtrip mismatch')
print('PASS roundtrip: '+str([(r['room'],r['triangles']) for r in results]))
bpy.ops.wm.open_mainfile(filepath=str(source/'N1_Final.blend'))
source_stats=[]
for scene in bpy.data.scenes:
    bpy.context.window.scene=scene
    depsgraph=bpy.context.evaluated_depsgraph_get()
    for obj in scene.objects:
        if obj.type!='MESH':continue
        evaluated=obj.evaluated_get(depsgraph);mesh=evaluated.to_mesh();mesh.calc_loop_triangles()
        source_stats.append({'room':scene.name,'object':obj.name,'triangles':len(mesh.loop_triangles)})
        evaluated.to_mesh_clear()
(source/'Triangulos_Por_Objeto.json').write_text(json.dumps(source_stats,indent=2),encoding='utf-8')
