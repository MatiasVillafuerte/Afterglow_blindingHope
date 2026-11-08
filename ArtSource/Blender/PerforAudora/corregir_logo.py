"""Elimina la copia duplicada del logo color coltán (lado +Y) del primer tramo de la punta de PerforAudora.

Blender 5.2: blender -b -P ArtSource/Blender/PerforAudora/corregir_logo.py -- <glb_original> <raiz_proyecto>
- No modifica PerforAudora.fbx ni PerforAudora.glb originales.
- Repinta únicamente los texeles de las caras oscuras del emblema a menos de 4,5 cm de su centro (+Y) con la mediana
  del dorado de las caras vecinas del mismo tramo; normal plana y metal/rugosidad de la misma vecindad.
- Guarda la fuente editable ArtSource/Blender/PerforAudora/PerforAudora_corregido.blend, las texturas corregidas y
  exporta Assets/Bosses/Mundo 1 - PerforAudora/PerforAudora_Corregido.fbx (+ PNG) para Unity.
"""
import bpy, sys, json
import numpy as np
from pathlib import Path
from mathutils import Vector
args = sys.argv[sys.argv.index('--') + 1:]
glb, root = args[0], Path(args[1])
src = root / 'ArtSource/Blender/PerforAudora'; dst = root / 'Assets/Bosses/Mundo 1 - PerforAudora'
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath=glb)
obj = [o for o in bpy.context.scene.objects if o.type == 'MESH'][0]; mesh = obj.data; mw = obj.matrix_world
imgs = {('base' if i.name == 'texture_pbr_20250901' else 'normal' if i.name.endswith('normal') else 'mr'): i for i in bpy.data.images}
px = {k: np.array(v.pixels[:], dtype=np.float32).reshape(v.size[1], v.size[0], 4) for k, v in imgs.items()}
H, W = px['base'].shape[:2]; uv = mesh.uv_layers.active.data

def texel(poly):
    us = [uv[i].uv for i in poly.loop_indices]; u = sum(p.x for p in us) / len(us); v = sum(p.y for p in us) / len(us)
    return min(H - 1, int(v * H)), min(W - 1, int(u * W))

def dark(poly):
    r, g, b, _ = px['base'][texel(poly)]; return (r + g + b) / 3 < .32 and max(r, g, b) - min(r, g, b) < .08

centers = {'y+': Vector((-0.3172, 0.0937, 0.1315)), 'y-': Vector((-0.3007, -0.0991, 0.1291))}
# Refina el centro del duplicado con las caras oscuras cercanas.
near = [p for p in mesh.polygons if (mw @ p.center - centers['y+']).length < .07 and dark(p)]
c = sum((mw @ p.center for p in near), Vector()) / max(1, len(near))
def emblem(poly):
    r, g, b, _ = px['base'][texel(poly)]; return max(r, g, b) - min(r, g, b) < .45 or (r + g + b) / 3 < .45  # gris, blanco, negro o sombra del relieve; el dorado es claro y saturado
logo = [p for p in mesh.polygons if (mw @ p.center - c).length < .07 and (mw @ p.center).x < -0.262 and emblem(p)]
ring = [p for p in mesh.polygons if .07 < (mw @ p.center - c).length < .1 and not emblem(p) and (mw @ p.center).x < -0.2]
gold = np.median(np.array([px['base'][texel(p)] for p in ring]), axis=0)
mr = np.median(np.array([px['mr'][texel(p)] for p in ring]), axis=0)
flat = np.array([.5, .5, 1., 1.], dtype=np.float32)

def fill(poly):
    pts = [(uv[i].uv.x * W, uv[i].uv.y * H) for i in poly.loop_indices]
    for k in range(1, len(pts) - 1):
        tri = np.array([pts[0], pts[k], pts[k + 1]])
        x0, y0 = np.floor(tri.min(0)).astype(int) - 2; x1, y1 = np.ceil(tri.max(0)).astype(int) + 2
        xs, ys = np.meshgrid(np.arange(max(0, x0), min(W, x1 + 1)), np.arange(max(0, y0), min(H, y1 + 1)))
        a, b, cc = tri
        def edge(p, q, x, y): return (q[0] - p[0]) * (y - p[1]) - (q[1] - p[1]) * (x - p[0])
        area = edge(a, b, cc[0], cc[1])
        if abs(area) < 1e-9: continue
        w0 = edge(b, cc, xs + .5, ys + .5) / area; w1 = edge(cc, a, xs + .5, ys + .5) / area; w2 = 1 - w0 - w1
        inside = (w0 >= -.08) & (w1 >= -.08) & (w2 >= -.08)  # margen de ~2 px contra costuras
        yy, xx = ys[inside], xs[inside]
        px['base'][yy, xx] = gold; px['normal'][yy, xx] = flat; px['mr'][yy, xx] = mr

for p in logo: fill(p)

# El relieve geométrico del emblema se conserva: aplanarlo rompe las normales personalizadas del GLB (probado 2026-11-08).
moved = 0
for k, img in imgs.items():
    img.pixels[:] = px[k].ravel(); img.update()
    name = {'base': 'PerforAudora_Corregido_Color.png', 'normal': 'PerforAudora_Corregido_Normal.png', 'mr': 'PerforAudora_Corregido_MetalRugosidad.png'}[k]
    img.filepath_raw = str(dst / name); img.file_format = 'PNG'; img.save()
    fresh = bpy.data.images.load(str(dst / name), check_existing=False); fresh.name = 'PerforAudora_' + k; fresh.pack()
    if k != 'base': fresh.colorspace_settings.name = 'Non-Color'
    for m in bpy.data.materials:
        if m.node_tree:
            for node in m.node_tree.nodes:
                if node.type == 'TEX_IMAGE' and node.image == img: node.image = fresh
obj.name = 'PerforAudora'
bpy.ops.wm.save_as_mainfile(filepath=str(src / 'PerforAudora_corregido.blend'))
bpy.ops.export_scene.fbx(filepath=str(dst / 'PerforAudora_Corregido.fbx'), use_selection=False, path_mode='RELATIVE', embed_textures=False, apply_scale_options='FBX_SCALE_ALL', axis_forward='-Z', axis_up='Y')
report = {'vertices_aplanados': moved, 'centro_duplicado': [round(v, 4) for v in c], 'caras_repintadas': len(logo), 'caras_de_referencia': len(ring), 'dorado_mediana': [round(float(v), 3) for v in gold[:3]]}
(src / 'Correccion_Logo.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
print('CORRECCION', json.dumps(report))
