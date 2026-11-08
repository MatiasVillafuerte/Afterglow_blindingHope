"""Re-export evaluated surfaces from the saved source without rebuilding rooms."""
import sys,json
from pathlib import Path
import bpy
sys.path.insert(0,str(Path(__file__).resolve().parent))
import generate_rooms as source
for scene in list(bpy.data.scenes):
    if scene.name not in ['PRO_CASA','PRO_PATIO','N01S01','N01S02','N01S03','N01S04','M01_MAPA']:continue
    bpy.context.window.scene=scene
    room=source.Room.__new__(source.Room)
    room.name=scene.name;room.scene=scene
    room.parts=[o for o in scene.objects if o.type=='MESH']
    meshes=source.evaluate(room)
    axis=bpy.data.objects.new('AxisWitness',None);scene.collection.objects.link(axis);axis.location=source.xyz((-3,2,7))
    result=source.bx_export.export(str(source.EXPORT/(scene.name+'.fbx')),'unity',meshes+[axis],embed_textures=False,path_mode='RELATIVE')
    print(scene.name,result)
    for obj in meshes+[axis]:bpy.data.objects.remove(obj,do_unlink=True)
print('NORMALS_REEXPORTED')
