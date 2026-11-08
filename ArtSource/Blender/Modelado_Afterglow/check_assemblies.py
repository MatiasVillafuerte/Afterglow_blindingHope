import sys,json
from pathlib import Path
import bpy
HERE=Path(__file__).resolve().parent
sys.path.insert(0,str(Path.home()/'.agents/skills/blender-asset-qa/scripts'))
import qa_check,review_shots
report=[]
def check(scene,name,objects):
    bpy.context.window.scene=scene
    result=qa_check.run(objects=[o.name for o in objects],profile='game',eps=.0002,tol=.002)
    report.append({'assembly':name,**result})
    for mode in ('random','wire'):
        review_shots.run(objects=[o.name for o in objects],out=str(HERE/'Revision'/f'{name}_{mode}.png'),mode=mode,res=360)

scene=bpy.data.scenes['PRO_CASA']
check(scene,'Bedside',[o for o in scene.objects if o.name.startswith(('Bedside','Cream lampshade'))])
check(scene,'MineralShelf',[o for o in scene.objects if o.name.startswith(('Mineral display shelf','Inanimate mineral','Shelf bracket'))])
scene=bpy.data.scenes['PRO_PATIO']
check(scene,'ArchedWindow',[o for o in scene.objects if o.name.startswith(('Arched glass','Window carved jamb','Arched window stone','Thin window glazing bar')) and abs(o.location.x+6.1)<1])
scene=bpy.data.scenes['N01S01']
check(scene,'Barrel',[o for o in scene.objects if o.name.startswith(('Coopered barrel','Barrel iron hoop')) and o.location.x>0])
check(scene,'TimberBrace',[o for o in scene.objects if o.name.startswith(('Gallery support post','Gallery supporting bracket','Forged support collar')) and o.location.x>0])
(HERE/'Revision/Assemblies.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
print('ASSEMBLIES '+json.dumps([{'name':r['assembly'],'pass':r['pass'],'errors':r['errors']} for r in report]))
