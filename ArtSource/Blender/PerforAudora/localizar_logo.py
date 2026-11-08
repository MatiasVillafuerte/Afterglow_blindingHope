"""Localiza los logos color coltán de la punta de PerforAudora y escribe la máscara UV del duplicado.

Blender 5.2: blender -b -P ArtSource/Blender/PerforAudora/localizar_logo.py -- <glb> <carpeta_salida>
Salida: base_color.png, normal.png, metal_rough.png (texturas originales), mascara_duplicado.png y logos.json.
Criterio: caras de la punta (x < -0.2) cuyo color de textura es gris oscuro poco saturado (el emblema).
Se agrupan por lado (y > 0 / y < 0). Se conserva el logo del lado -Y (frente, como en PerforAudora.png).
"""
import bpy, sys, json
import numpy as np
from pathlib import Path
args = sys.argv[sys.argv.index('--') + 1:]
glb, out = args[0], Path(args[1]); out.mkdir(parents=True, exist_ok=True)
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.gltf(filepath=glb)
obj = [o for o in bpy.context.scene.objects if o.type == 'MESH'][0]
mesh = obj.data
images = {}
for img in bpy.data.images:
    w, h = img.size; px = np.array(img.pixels[:], dtype=np.float32).reshape(h, w, 4)
    images[img.name] = px
    key = 'base_color' if img.name == 'texture_pbr_20250901' else 'normal' if img.name.endswith('normal') else 'metal_rough'
    img.filepath_raw = str(out / (key + '.png')); img.file_format = 'PNG'; img.save()
base = images['texture_pbr_20250901']; H, W = base.shape[:2]
uv = mesh.uv_layers.active.data
logo_faces = {1: [], -1: []}
mw = obj.matrix_world
for poly in mesh.polygons:
    c = mw @ poly.center
    if c.x > -0.2: continue
    us = [uv[i].uv for i in poly.loop_indices]
    u = sum(p.x for p in us) / len(us); v = sum(p.y for p in us) / len(us)
    r, g, b, _ = base[min(H - 1, int(v * H)), min(W - 1, int(u * W))]
    lum = (r + g + b) / 3; sat = max(r, g, b) - min(r, g, b)
    if lum < .32 and sat < .08:
        logo_faces[1 if c.y > 0 else -1].append(poly.index)
report = {}
for side, faces in logo_faces.items():
    pts = [mw @ mesh.polygons[i].center for i in faces]
    uvs = [uv[li].uv for i in faces for li in mesh.polygons[i].loop_indices]
    report['y+' if side > 0 else 'y-'] = {
        'faces': len(faces),
        'centro': [round(sum(p[k] for p in pts) / max(1, len(pts)), 4) for k in range(3)] if pts else None,
        'uv_min': [round(min(p.x for p in uvs), 4), round(min(p.y for p in uvs), 4)] if uvs else None,
        'uv_max': [round(max(p.x for p in uvs), 4), round(max(p.y for p in uvs), 4)] if uvs else None}
# Máscara del duplicado (lado +Y): rasteriza los triángulos UV de sus caras.
mask = np.zeros((H, W), dtype=np.uint8)
import mathutils
for i in logo_faces[1]:
    loops = mesh.polygons[i].loop_indices
    pts = [(uv[li].uv.x * W, uv[li].uv.y * H) for li in loops]
    xs = [p[0] for p in pts]; ys = [p[1] for p in pts]
    for yy in range(int(min(ys)), int(max(ys)) + 1):
        for xx in range(int(min(xs)), int(max(xs)) + 1):
            mask[min(H - 1, yy), min(W - 1, xx)] = 255
mask_img = bpy.data.images.new('mask', W, H, alpha=False)
rgba = np.repeat((mask.astype(np.float32) / 255)[:, :, None], 4, axis=2); rgba[:, :, 3] = 1
mask_img.pixels[:] = rgba.ravel(); mask_img.filepath_raw = str(out / 'mascara_duplicado.png'); mask_img.file_format = 'PNG'; mask_img.save()
(out / 'logos.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
print('LOGOS', json.dumps(report))
