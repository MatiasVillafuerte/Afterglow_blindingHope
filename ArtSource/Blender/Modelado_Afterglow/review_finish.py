"""Multi-angle review and FBX round-trip; does not save over the source blend."""
import sys,json
from pathlib import Path
import bpy
from mathutils import Vector
ROOT=Path(__file__).resolve().parents[3]
HERE=Path(__file__).resolve().parent
sys.path.insert(0,str(Path.home()/'.agents/skills/blender-asset-qa/scripts'))
import review_shots

def bounds(objects):
    bpy.context.view_layer.update()
    points=[o.matrix_world@Vector(v) for o in objects for v in o.bound_box]
    return [[min(p[i] for p in points),max(p[i] for p in points)] for i in range(3)]

report=[]
patio=bpy.data.scenes['PRO_PATIO']
bpy.context.window.scene=patio
bars=[o for o in patio.objects if o.name.startswith('Wrought iron bar')]
fence={'bars':len(bars),'heights':sorted(set(round(o.dimensions.z,4) for o in bars)),
       'frontBars':sum(abs(o.location.y-15)<.01 for o in bars)}
(HERE/'Revision/FenceDimensions.json').write_text(json.dumps(fence,indent=2),encoding='utf-8')
if fence['heights']!=[2.22]:raise RuntimeError(fence)
if '--metrics-only' in sys.argv:
    counts=[]
    for scene in bpy.data.scenes:
        bpy.context.window.scene=scene
        deps=bpy.context.evaluated_depsgraph_get()
        for obj in scene.objects:
            if obj.type!='MESH':continue
            evaluated=obj.evaluated_get(deps);mesh=evaluated.to_mesh();mesh.calc_loop_triangles()
            counts.append({'scene':scene.name,'object':obj.name,'triangles':len(mesh.loop_triangles)})
            evaluated.to_mesh_clear()
    (HERE/'Revision/TrianglesByObject.json').write_text(json.dumps(counts,indent=2),encoding='utf-8')
    print('FENCE_DIMENSIONS '+json.dumps(fence))
    sys.exit(0)
for scene in list(bpy.data.scenes):
    if scene.name not in ['PRO_CASA','PRO_PATIO','N01S01','N01S02','N01S03','N01S04','M01_MAPA']:continue
    bpy.context.window.scene=scene
    objects=[o for o in scene.objects if o.type=='MESH']
    for mode in ('random','wire'):
        review_shots.run(objects=[o.name for o in objects],out=str(HERE/'Revision'/f'{scene.name}_{mode}.png'),mode=mode,res=320)
    original=bounds(objects)
    # Export intentionally mirrors X before Unity axis conversion, as in the approved baseline.
    expected=[[-original[0][1],-original[0][0]],original[1],original[2]]
    imported=bpy.data.scenes.new('RoundTrip');bpy.context.window.scene=imported
    bpy.ops.import_scene.fbx(filepath=str(ROOT/'Assets/Afterglow/Environment/Modelado'/f'{scene.name}.fbx'))
    meshes=[o for o in imported.objects if o.type=='MESH']
    actual=bounds(meshes)
    difference=max(abs(actual[i][j]-expected[i][j]) for i in range(3) for j in range(2))
    result={'room':scene.name,'maxBoundsError':difference,'pass':difference<.015,
            'meshCount':len(meshes),'triangles':sum(len(o.data.polygons) for o in meshes),
            'unitScale':all(max(abs(s-1) for s in o.scale)<.001 for o in meshes)}
    report.append(result)
    for obj in list(imported.objects):bpy.data.objects.remove(obj,do_unlink=True)
    bpy.data.scenes.remove(imported)
(HERE/'Revision/RoundTrip.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
if any(not row['pass'] or not row['unitScale'] for row in report):raise RuntimeError(report)
print('REVIEW_COMPLETE '+json.dumps(report))
