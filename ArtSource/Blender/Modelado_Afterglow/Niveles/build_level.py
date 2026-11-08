"""Montaje genérico de un nivel de Mundo 1 a partir de su plano editable (Docs/Diseno/Niveles/N?_Plano.json).

Ejecutar desde la raíz del proyecto Unity con Blender 5.2:
  blender -b --python-exit-code 1 -P ArtSource/Blender/Modelado_Afterglow/Niveles/build_level.py -- N2
Salida: Assets/Afterglow/Environment/Modelado/Nivel<n>/<sala>.fbx, <sala>.json (colliders) y <sala>_huecos.json,
fuente editable ArtSource/Blender/Modelado_Afterglow/Niveles/N<n>_Montaje.blend y revisiones en Niveles/Revision_N<n>.
Las piezas interactivas (carros, reja, puente, palanca, contrapeso, rocas, farol) se exportan por separado como Export_<id>.
"""
from pathlib import Path
import sys, json, math
import bpy

ROOT = Path.cwd(); SOURCE = Path(__file__).resolve().parent
sys.path.insert(0, str(SOURCE.parent))
import generate_rooms as kit
import painterly_finish as finish
sys.path.insert(0, str(SOURCE.parent / 'Nivel1'))
import build_n1 as n1  # reutiliza join_terrain (unión exacta y biselado del terreno)

LEVEL = sys.argv[sys.argv.index('--') + 1] if '--' in sys.argv else 'N2'
PLAN = json.loads((ROOT / 'Docs/Diseno/Niveles' / (LEVEL + '_Plano.json')).read_text(encoding='utf-8'))
NUMBER = PLAN['level']
OUT = ROOT / 'Assets/Afterglow/Environment/Modelado' / ('Nivel' + str(NUMBER)); OUT.mkdir(parents=True, exist_ok=True)
REVIEW = SOURCE / ('Revision_' + LEVEL); REVIEW.mkdir(parents=True, exist_ok=True)
for key, color in {'BridgeDeck': (.52, .31, .12), 'LeverArm': (.78, .46, .12), 'Counterweight': (.28, .25, .31),
                   'CartBody': (.36, .22, .12), 'CartMark': (.95, .72, .18), 'Plate': (.62, .5, .2), 'GateIron': (.2, .22, .25),
                   'Cracked': (.55, .5, .44)}.items():
    kit.PALETTE[key] = color


def slab(r, name, x, z, w, d, top=0, mat='Dust'):
    height = top + 3
    obj = r.box(name, (x, top - height / 2, z), (w, height, d), mat, .035, True)
    obj['terrain'] = True
    return obj


def wall(r, name, x, z, w, d, h):
    return r.box(name, (x, h / 2, z), (w, h, d), 'Slate', .08, True)


def tagged(obj, export_id):
    obj['export_id'] = export_id
    return obj


def return_markers(r, x, y, z):
    for dx in (-1.6, 1.6):
        r.box('Return_marker', (x + dx, y + .45, z), (.23, .9, .23), 'Oak', .04)
        r.box('Return_marker_cap', (x + dx, y + .95, z), (.32, .1, .32), 'Trim', .02)


def cart(r, export_id, x, y, z, marked):
    parts = [r.box('Cart_tub', (x, y + .62, z), (1.25, .62, .95), 'CartBody', .06),
             r.box('Cart_rim', (x, y + .95, z), (1.35, .08, 1.05), 'Iron', .02)]
    for dx in (-.42, .42):
        for dz in (-.5, .5):
            parts.append(r.lathe('Cart_wheel', (x + dx, y + .22, z + dz), [(.2, -.05), (.2, .05)], 'Iron', 10))
    if marked:
        parts.append(r.box('Cart_mark', (x, y + .66, z - .49), (1.27, .2, .04), 'CartMark', .01))
    for p in parts: tagged(p, export_id)


def decorate(r, room):
    props = room.get('props', {})
    # Fondo de caverna detrás de la silueta jugable (sin techo ni pared frontal).
    xs = [s[1] for s in room['slabs']]; zs = [s[2] + s[4] / 2 for s in room['slabs']]
    x0, x1, zb = min(xs) - 8, max(xs) + 8, max(zs) + 4
    i = 0; x = x0
    while x < x1:
        r.rock('Cavern_backdrop', (x, 2.4, zb + (i % 2) * .7), (3.4, 9 + (i % 3) * .8, 3), 'Deep' if i % 3 == 0 else 'Slate')
        x += 2.3; i += 1
    for w in room.get('walls', []):
        name, x, z, ww, d, h = w
        for k in range(max(2, int(ww / 1.8))):
            r.rock('Strata_' + name, (x - ww / 2 + .9 + k * 1.6, h * .5, z - d * .45), (2.0, h + .2, 1.2), 'SlateLight' if k % 3 == 0 else 'Slate')
    for s in room['slabs']:
        name, x, z, w, d, top, mat = s
        if top > .3:  # canto de madera en el frente de cada cota elevada (lectura del borde)
            r.box('Ledge_trim_' + name, (x, top - .14, z - d / 2 - .06), (w - .2, .24, .16), 'Oak', .04)
    for x, z, y in props.get('crates', []):
        kit.crate(r, x, z, y)
    for lamp in props.get('lamps', []):
        x, y, z = lamp[:3]; base = lamp[3] if len(lamp) > 3 else 0
        r.box('Lamp_post', (x, base + y / 2, z), (.22, y, .22), 'Oak', .04); kit.lantern(r, x, base + y, z - .23)
    for a, b in props.get('tracks', []):
        kit.track(r, tuple(a), tuple(b))
    if 'landmark' in props:  # torre de poleas: objetivo visible desde el andén
        x, y, z = props['landmark']
        for dx in (-.9, .9):
            r.segment('Pulley_leg', (x + dx, y, z), (x + dx * .3, y + 8.2, z), .34, .34, 'DarkOak')
        for h in (2.5, 5.0, 7.2):
            r.box('Pulley_brace', (x, y + h, z), (1.9 - h * .14, .22, .24), 'Oak', .03)
        r.lathe('Pulley_wheel', (x, y + 8.3, z - .2), [(.95, -.12), (1.0, -.06), (1.0, .06), (.95, .12)], 'Iron', 20)
        r.box('Pulley_hub', (x, y + 8.3, z - .2), (.3, .3, .5), 'Trim', .04)
        r.segment('Pulley_rope', (x + .95, y + 8.3, z - .2), (x + .95, y + 3.0, z - .2), .05, .05, 'Linen')
    if 'hangPost' in props:  # farol del Hangzzer: poste alto con brazo
        x, y, z = props['hangPost']
        tagged(r.box('Hang_post', (x, y + 2.1, z), (.3, 4.2, .3), 'DarkOak', .05), 'HangPost')
        tagged(r.box('Hang_arm', (x + .55, y + 4.0, z), (1.3, .18, .2), 'Oak', .04), 'HangPost')
        kit.lantern(r, x - .3, y + 3.6, z - .25)
    if 'rescueDoor' in props:
        x, y, z = props['rescueDoor']; kit.portal(r, x, y, z)
        r.box('Rescue_planks', (x, y + 1.3, z + .05), (2.4, 2.4, .1), 'DarkOak', .02)
    if 'tunnel' in props:
        x, y, z = props['tunnel']; kit.portal(r, x, y, z)
    for door in room.get('doors', []):
        x, y, z = door['pos']
        if door.get('portal'): kit.portal(r, x, y, z)
        else: return_markers(r, x, y, z)
        n = door.get('lights', 0)  # código de N3: faroles sobre el dintel
        for i in range(n):
            lx = x + (i - (n - 1) / 2) * .75
            r.box('Code_lamp_hook', (lx, y + 3.45, z - .3), (.06, .3, .06), 'Iron')
            kit.lantern(r, lx, y + 3.15, z - .38)
    for sx, sy, sz, count, _text in room.get('signs', []):  # señal del cruce: tablero con N faroles
        width = max(1.2, count * .62 + .5)
        for dx in (-width / 2 + .1, width / 2 - .1):
            r.box('Sign_post', (sx + dx, sy + .8, sz), (.14, 1.6, .14), 'Oak', .03)
        r.box('Sign_board', (sx, sy + 1.75, sz), (width, .75, .12), 'DarkOak', .04)
        for i in range(count):
            r.lathe('Sign_lamp', (sx + (i - (count - 1) / 2) * .62, sy + 1.75, sz - .1), [(.16, -.16), (.17, .16)], 'Lamp', 10)
    if 'banner' in props:  # marca del equipo minero (confirmación de avance)
        x, y, z = props['banner']
        for dx in (-.9, .9):
            r.box('Banner_pole', (x + dx, y + 1.4, z), (.12, 2.8, .12), 'DarkOak', .03)
        r.box('Banner_cloth', (x, y + 2.0, z - .05), (1.6, 1.2, .05), 'Rose', .01)
        r.box('Banner_emblem', (x, y + 2.05, z - .09), (.6, .6, .03), 'Lamp', .01)


def mechanisms(r, room):
    for m in room.get('mechanisms', []):
        if m['type'] == 'cart':
            y = m.get('y', 0)
            cart(r, 'Cart_' + m['id'], m['start'], y, m['z'], m.get('marked', False))
            tagged(r.box('Plate_' + m['id'], (m['plate'], y + .03, m['z']), (1.1, .06, 1.1), 'CartMark' if m.get('marked') else 'Plate', .02), 'Plate_' + m['id'])
            for stop in (m['min'] - .75, m['max'] + .75):
                r.box('Rail_stop', (stop, y + .25, m['z']), (.25, .5, 1.1), 'Iron', .03)
        elif m['type'] == 'gate':
            x, y, z = m['pos']; sx, sy, sz = m['size']
            for k in range(int(sz / .45) + 1):
                tagged(r.box('Gate_bar', (x, y + sy / 2, z - sz / 2 + k * .45), (.12, sy, .12), 'GateIron', .02), 'Gate')
            for h in (.4, sy - .3):
                tagged(r.box('Gate_rail', (x, y + h, z), (.16, .14, sz), 'GateIron', .02), 'Gate')
        elif m['type'] == 'bridge':
            px, py, pz = m['pivot']; cx, cy, cz = m['deckCenter']; sx, sy, sz = m['deckSize']
            n = int(sz / .4)
            for k in range(n):
                tagged(r.box('Deck_plank', (px + cx, py + cy, pz + .2 + k * .4), (sx - .04, .2, .34), 'BridgeDeck', .025), 'BridgeDeck')
            lx, ly, lz = m['lever']
            r.box('Lever_base', (lx, ly + .35, lz), (.8, .7, .8), 'Iron', .08)
            tagged(r.box('Lever_arm', (lx, ly + .98, lz), (.14, .9, .14), 'LeverArm', .03), 'LeverArm')
            wx, wy, wz = m['counterweight']
            r.segment('Counterweight_frame', (wx, 0, wz), (wx, wy + 1.4, wz), .2, .22, 'Oak')
            tagged(r.box('Counterweight', (wx, wy, wz), (.85, .9, .7), 'Counterweight', .08), 'Counterweight')
        elif m['type'] == 'signPlate':
            x, y, z = m['pos']
            r.box('Plate_post', (x, y + .7, z), (.14, 1.4, .14), 'Oak', .03)
            tagged(r.box('Plate_board', (x, y + 1.45, z - .08), (1.1, .5, .08), 'Trim', .02), 'SignBoard_' + m['id'])
        elif m['type'] == 'resetLever':
            x, y, z = m['pos']
            r.box('Reset_base', (x, y + .3, z), (.7, .6, .7), 'Iron', .06)
            tagged(r.box('Reset_arm', (x, y + .9, z), (.12, .8, .12), 'LeverArm', .03), 'ResetArm')
    for entry in room.get('breakables', []):
        bid, x, z = entry[:3]
        top = next(s[5] for s in room['slabs'] if abs(s[1] - x) <= s[3] / 2 + .2 and abs(s[2] - z) <= s[4] / 2 + .4)
        tagged(r.rock('Cracked_' + bid, (x, top + .7, z), (1.3, 1.4, .9), 'Cracked'), 'Rock_' + bid)


def track_ride(r, room):
    """Vía de la vagoneta (N5): tablero sólido por tramo, rieles y caballetes cada 4 m."""
    for x0, x1, y in room.get('track', []):
        length = x1 - x0
        r.box('Track_deck_%g' % x0, ((x0 + x1) / 2, y - .15, 0), (length, .3, 1.5), 'Oak', .03, True)
        kit.track(r, (x0, y, 0), (x1, y, 0))
        k = x0 + 1
        while k < x1:
            for dz in (-.6, .6):
                r.segment('Trestle_leg', (k, y - .3, dz), (k + (.4 if dz < 0 else -.4), y - 7, dz * 1.6), .22, .22, 'DarkOak')
            r.box('Trestle_beam', (k, y - 1.6, 0), (.22, .22, 2.2), 'Oak', .03)
            k += 4
    xs = [t[0] for t in room.get('track', [])] + [t[1] for t in room.get('track', [])]
    if xs:  # roca de fondo a lo largo de la vía (sin pared frontal)
        x = min(xs) - 6; i = 0
        while x < max(xs) + 6:
            r.rock('Ride_backdrop', (x, 1.5, 9 + (i % 2)), (5, 12 + (i % 3), 3), 'Deep' if i % 3 == 0 else 'Slate'); x += 4.2; i += 1


def arena(r, room):
    """Arena circular de N6: suelo en disco, pared de rocas continua, cornisa de Platicio, altar de batalla y vía oculta."""
    a = room['arena']; cx, cy, cz = a['center']; R = a['radius']
    r.lathe('Arena_floor', (cx, cy - 3, cz), [(R + .4, 0), (R + .4, 2.97), (R + .2, 3)], 'Dust', 48)
    n = 30
    for i in range(n):
        ang = i / n * math.tau
        x, z = cx + math.cos(ang) * (R + 1.0), cz + math.sin(ang) * (R + 1.0)
        if math.sin(ang) > -0.35:  # mitad del fondo: pared alta de roca
            r.rock('Arena_wall', (x, 2.2, z), (2.6, 4.8 + (i % 3) * .5, 1.8), 'Slate' if i % 2 else 'SlateLight')
        else:  # frente de cámara: bordillo bajo que marca el límite sin tapar la pelea
            r.rock('Arena_curb', (cx + math.cos(ang) * (R + .5), .2, cz + math.sin(ang) * (R + .5)), (2.3, .45, .9), 'SlateLight')
    lx, ly, lz = a['ledge']
    r.box('Son_ledge', (lx, ly - .2, lz), (4, .4, 1.8), 'Oak', .05)
    for dx in (-1.6, 1.6): r.box('Ledge_post', (lx + dx, ly / 2 - .2, lz), (.25, ly, .25), 'DarkOak', .04)
    ax, ay, az = a['altar']
    r.lathe('Battle_altar', (ax, ay, az), [(.75, 0), (.79, .12), (.61, .26), (.55, .74), (.79, 1.03)], 'SlateLight', 12)
    r.rock('Battle_altar_emblem', (ax, ay + 1.31, az), (.49, .64, .36), 'Crystal')
    x0, y0, z0 = a['cart']; x1, y1, z1 = a['cartEnd']
    kit.track(r, (x0 - 2, y0, z0), (x1, y1, z1))
    r.box('Tarp', (x0, 1.2, z0 + .1), (2.6, 2.4, .12), 'Canvas', .02)
    for k in range(8):
        r.rock('Backdrop', (cx - 16 + k * 4.5, 3, cz + R + 5), (4, 10, 3), 'Deep' if k % 3 == 0 else 'Slate')


def evaluate(room):
    """Como kit.evaluate, pero las piezas con export_id salen como malla propia (Export_<id>)."""
    bpy.context.window.scene = room.scene; bpy.context.view_layer.update()
    depsgraph = bpy.context.evaluated_depsgraph_get(); grouped = {}
    for obj in room.parts:
        evaluated = obj.evaluated_get(depsgraph); mesh = evaluated.to_mesh(); mesh.calc_loop_triangles()
        material = obj.data.materials[0].name
        key = (obj.get('export_id') or material, material)
        vertices, triangles, normals = grouped.setdefault(key, ([], [], []))
        offset = len(vertices)
        for vertex in mesh.vertices:
            p = obj.matrix_world @ vertex.co; vertices.append((-p.x, p.y, p.z))
        nm = obj.matrix_world.to_3x3().inverted().transposed()
        for tri in mesh.loop_triangles:
            if tri.area <= 1e-9: continue
            triangles.append(tuple(i + offset for i in reversed(tri.vertices)))
            for index in reversed(tri.loops):
                n = (nm @ mesh.corner_normals[index].vector).normalized(); normals.append((-n.x, n.y, n.z))
        evaluated.to_mesh_clear()
    result = []
    for (name, material), (vertices, triangles, normals) in grouped.items():
        obj = room.mesh('Export_' + name, vertices, triangles, material)
        for polygon in obj.data.polygons: polygon.use_smooth = True
        obj.data.normals_split_custom_set(normals)
        finish.planar_uv(obj, .65); result.append(obj)
    return result


def main():
    starter = bpy.context.scene
    rooms = []
    for room in PLAN['rooms']:
        r = kit.Room(room['id'])
        for s in room['slabs']: slab(r, *s)
        for w in room.get('walls', []): wall(r, *w)
        rooms.append((r, room))
    bpy.data.scenes.remove(starter)
    report = []
    for r, room in rooms:
        bpy.context.window.scene = r.scene
        if any(o.get('terrain') for o in r.parts): n1.join_terrain(r)
        if room.get('ride'): track_ride(r, room)
        elif room.get('arena'): arena(r, room)
        else: decorate(r, room)
        mechanisms(r, room)
        for obj in r.parts: finish.planar_uv(obj, .65)
        parts = evaluate(r)
        axis = bpy.data.objects.new('AxisWitness', None); r.scene.collection.objects.link(axis); axis.location = kit.xyz((-3, 2, 7))
        kit.bx_export.export(str(OUT / (r.name + '.fbx')), 'unity', parts + [axis], embed_textures=False, path_mode='RELATIVE')
        report.append({'room': r.name, 'triangles': sum(len(o.data.polygons) for o in parts), 'pieces': [o.name for o in parts if not o.name.startswith('Export_') or '_' in o.name[7:]]})
        for obj in parts + [axis]: bpy.data.objects.remove(obj, do_unlink=True)
        r.parts = [o for o in r.scene.objects if o.type == 'MESH']
        (OUT / (r.name + '.json')).write_text(json.dumps({'boxes': r.colliders}), encoding='utf-8')
        (OUT / (r.name + '_huecos.json')).write_text(json.dumps({'gaps': room.get('gaps', [])}, ensure_ascii=False), encoding='utf-8')
        try:
            kit_shots = __import__('review_shots')
        except ImportError:
            sys.path.insert(0, str(Path.home() / '.agents/skills/blender-asset-qa/scripts')); kit_shots = __import__('review_shots')
        kit_shots.run(objects=[o.name for o in r.parts], out=str(REVIEW / (r.name + '_random.png')), mode='random', res=360)
    bpy.context.window.scene = rooms[0][0].scene
    bpy.ops.wm.save_as_mainfile(filepath=str(SOURCE / (LEVEL + '_Montaje.blend')))
    (SOURCE / (LEVEL + '_Auditoria.json')).write_text(json.dumps(report, indent=2), encoding='utf-8')
    print('RESULT ' + json.dumps(report))


if __name__ == '__main__':
    main()
