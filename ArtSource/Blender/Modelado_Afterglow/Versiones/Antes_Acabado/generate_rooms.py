"""Author the editable Afterglow rooms and the corresponding Unity static FBX files.

Run with Blender 5.2 from the Unity project root. Layout coordinates are Unity metres.
"""
import json
import math
import random
import sys
from pathlib import Path

import bmesh
import bpy
from mathutils import Vector

ROOT = Path.cwd()
SOURCE = Path(__file__).resolve().parent
EXPORT = ROOT / 'Assets/Afterglow/Environment/Modelado'
for skill in ('scenario-blender-expert', 'scenario-blender-hard-surface', 'scenario-blender-export'):
    sys.path.append(str(Path.home() / '.codex/skills' / skill / 'scripts'))
import bx_audit
import bx_export
import bx_hardsurface as hs

PALETTE = {
    'Ivory': (.84, .72, .53), 'Plaster': (.58, .44, .30), 'Trim': (.76, .54, .23),
    'Clay': (.47, .16, .085), 'ClayLight': (.62, .25, .13), 'DarkOak': (.22, .096, .047),
    'Oak': (.38, .19, .085), 'Velvet': (.27, .10, .30), 'Linen': (.9, .79, .61),
    'Glass': (.14, .35, .39), 'Water': (.11, .49, .56), 'Iron': (.075, .10, .12),
    'Path': (.62, .48, .32), 'Paver': (.72, .58, .40), 'Grass': (.22, .38, .17),
    'Leaf': (.12, .29, .14), 'LeafLight': (.28, .45, .18), 'Rose': (.69, .25, .39),
    'Lamp': (.95, .62, .23), 'Earth': (.30, .22, .16), 'Dust': (.40, .31, .24),
    'Slate': (.24, .29, .34), 'SlateLight': (.34, .40, .45), 'Deep': (.12, .16, .20),
    'Crystal': (.51, .24, .65), 'Tunnel': (.035, .042, .055), 'Canvas': (.23, .34, .39),
}


def xyz(p):
    return p[0], -p[2], p[1]


class Room:
    def __init__(self, name):
        self.name = name
        self.scene = bpy.data.scenes.new(name)
        self.scene.unit_settings.system = 'METRIC'
        bpy.context.window.scene = self.scene
        self.parts = []
        self.colliders = []
        self.random = random.Random(name)

    def mesh(self, name, vertices, faces, material, position=(0, 0, 0)):
        data = bpy.data.meshes.new(name)
        data.from_pydata(vertices, [], faces)
        data.validate()
        data.update()
        obj = bpy.data.objects.new(name, data)
        self.scene.collection.objects.link(obj)
        obj.location = xyz(position)
        obj.color = (*PALETTE[material], 1)
        mat = bpy.data.materials.get(material) or bpy.data.materials.new(material)
        mat.diffuse_color = obj.color
        data.materials.append(mat)
        self.parts.append(obj)
        return obj

    def box(self, name, position, size, material, bevel=.035, solid=False):
        x, z, y = (value / 2 for value in size)
        obj = self.mesh(name, [(-x,-y,-z),(x,-y,-z),(x,y,-z),(-x,y,-z),(-x,-y,z),(x,-y,z),(x,y,z),(-x,y,z)],
                        [(3,2,1,0),(0,1,5,4),(1,2,6,5),(2,3,7,6),(3,0,4,7),(4,5,6,7)], material, position)
        if bevel:
            modifier = obj.modifiers.new('Rounded timber or stone edge', 'BEVEL')
            modifier.width = min(bevel, min(size) * .22)
            modifier.segments = 2 if bevel > .08 else 1
            modifier.harden_normals = True
            obj.modifiers.new('Weighted corner normals', 'WEIGHTED_NORMAL')
        if solid:
            self.colliders.append({'name': name, 'center': position, 'size': size})
        return obj

    def segment(self, name, a, b, width, thickness, material):
        direction = Vector(xyz(b)) - Vector(xyz(a))
        center = tuple((a[i] + b[i]) * .5 for i in range(3))
        obj = self.box(name, center, (width, thickness, direction.length), material)
        obj.rotation_euler = direction.to_track_quat('Y', 'Z').to_euler()
        return obj

    def lathe(self, name, position, profile, material, count=28):
        verts = [(r*math.cos(i*math.tau/count), r*math.sin(i*math.tau/count), h) for r,h in profile for i in range(count)]
        faces = [tuple(reversed(range(count)))]
        for j in range(len(profile)-1):
            for i in range(count):
                k = (i+1)%count
                faces.append((j*count+i,j*count+k,(j+1)*count+k,(j+1)*count+i))
        faces.append(tuple(range((len(profile)-1)*count,len(profile)*count)))
        obj = self.mesh(name, verts, faces, material, position)
        for polygon in obj.data.polygons:
            polygon.use_smooth = len(polygon.vertices) == 4
        return obj

    def rock(self, name, position, size, material, rings=8):
        sx, sy, sz = size
        offsets = [self.random.uniform(.79, 1.19) for _ in range(rings)]
        vertices = []
        for ring, level in enumerate((-.49, -.23, .24, .49)):
            width = (.73, 1., .87, .44)[ring]
            for i in range(rings):
                angle = i * math.tau / rings + .22 * ring
                vertices.append((math.cos(angle)*sx*.5*offsets[i]*width, math.sin(angle)*sz*.5*offsets[i]*width,
                                 (level+self.random.uniform(-.06,.06))*sy))
        faces = [tuple(reversed(range(rings)))]
        for j in range(3):
            for i in range(rings):
                a=j*rings+i; b=j*rings+(i+1)%rings
                faces.extend(((a,b,b+rings),(a,b+rings,a+rings)))
        faces.append(tuple(range(3*rings,4*rings)))
        return self.mesh(name, vertices, faces, material, position)

    def boundary(self, name, center, size):
        self.colliders.append({'name': name, 'center': center, 'size': size})

    def floor(self, width, depth, middle, material, fence=False):
        self.box('Ground', (0,-.44,middle), (width,.88,depth), material, .16, True)
        for x in (-width/2,width/2):
            self.boundary('Side boundary', (x,3,middle), (.35,8,depth+.35))
        for z in (middle-depth/2,middle+depth/2):
            self.boundary('Depth boundary', (0,3,z), (width+.35,8,.35))
        if not fence:
            for x in (-width/2+.25,width/2-.25):
                for j in range(int(depth/2)):
                    z = middle-depth/2+1+j*2
                    self.rock('Side strata', (x,.55,z), (1.9,1.3+self.random.random(),2.5), 'Slate')
            for j in range(int(width/2)):
                x=-width/2+1+j*2
                self.rock('Rear strata', (x,2.65,middle+depth/2+.32), (3.4,7+self.random.random(),2.0), 'Deep' if j%3==0 else 'Slate')


def portal(room, x, y, z, style='mine'):
    width = 2.6
    room.box('Recess', (x,y+1.35,z+.16), (width,2.7,.22), 'Tunnel', .12)
    for sign in (-1,1):
        room.box('Load bearing jamb', (x+sign*1.36,y+1.55,z), (.37,3.1,.55), 'DarkOak', .08)
        room.box('Iron collar', (x+sign*1.36,y+2.55,z-.025), (.46,.22,.63), 'Iron')
        room.box('Dressed footing', (x+sign*1.36,y+.21,z), (.57,.42,.75), 'SlateLight', .05)
        room.segment('Knee brace',(x+sign*1.26,y+2.08,z-.04),(x+sign*.68,y+2.72,z-.04),.23,.4,'Oak')
    room.box('Mine lintel', (x,y+2.9,z), (3.22,.42,.61), 'Oak', .08)
    room.box('Threshold', (x,y+.018,z-.5), (2.6,.035,1.3), 'Path')
    lantern(room,x+1.48,y+2.1,z-.41)


def lantern(room,x,y,z):
    room.lathe('Lantern cap',(x,y,z),[(.24,.24),(.29,.28),(.19,.43),(.08,.48)],'Iron',8)
    room.lathe('Warm glass',(x,y,z),[(.20,-.22),(.21,.24)],'Lamp',8)
    room.lathe('Lantern foot',(x,y,z),[(.24,-.29),(.25,-.23)],'Iron',8)
    for dx in (-.19,.19):
        for dz in (-.09,.09):
            room.box('Lantern mullion',(x+dx,y,z+dz),(.042,.48,.042),'Iron')


def track(room, a, b):
    dx,dz=b[0]-a[0],b[2]-a[2]
    length=math.hypot(dx,dz)
    nx,nz=dz/length,-dx/length
    for side in (-1,1):
        room.segment('Rail head',(a[0]+side*nx*.32,a[1]+.075,a[2]+side*nz*.32),
                     (b[0]+side*nx*.32,b[1]+.075,b[2]+side*nz*.32),.06,.05,'Iron')
    for i in range(int(length/.65)+1):
        t=i/max(1,int(length/.65));x=a[0]+dx*t;z=a[2]+dz*t;y=a[1]+(b[1]-a[1])*t
        room.segment('Sleeper',(x-nx*.56,y+.035,z-nz*.56),(x+nx*.56,y+.035,z+nz*.56),.17,.07,'DarkOak')


def terrace(room, x, z, width, depth, height):
    room.box('Terrace body',(x,height*.5-.08,z),(width,height-.16,depth),'Slate',.12,False)
    room.box('Worn walking surface',(x,height-.11,z),(width,.22,depth),'SlateLight',.045)
    room.boundary('Walkable terrace',(x,height*.5,z),(width,height,depth))
    for i in range(max(2,int(width))):
        room.rock('Split rock facing',(x-width*.5+.55+i*width/max(2,int(width)),height*.42,z-depth*.5+.09),
                  (.95,height*.7,.55),'Slate')


def crate(room, x, z, y=0):
    room.box('Crate body',(x,y+.42,z),(.82,.84,.82),'DarkOak',.06,True)
    for i in (-1,0,1):
        room.box('Crate boards',(x+i*.25,y+.42,z-.422),(.23,.69,.04),'Oak')
    for height in (.12,.72):
        room.box('Crate band',(x,y+height,z-.45),(.86,.10,.045),'Trim')
    room.segment('Crate diagonal',(x-.36,y+.17,z-.46),(x+.36,y+.70,z-.46),.095,.045,'DarkOak')


def garden():
    room=Room('PRO_PATIO')
    room.box('Surrounding meadow',(0,-1.18,4),(90,1.85,80),'LeafLight',.55)
    room.segment('Road beyond the gate',(14,-.075,-7.5),(32,-.075,-9.8),3.45,.13,'Path')
    for i in range(8):
        room.rock('Distant valley slope',(-31+i*9,-.9,28+i%2*4),(14,7+i%3*2,12),'Leaf',10)
    room.floor(28,32,1,'Grass',True)
    room.box('Hacienda court foundation',(0,-.34,15),(18,.67,6),'Plaster',.10)
    room.box('Villa Albina',(0,3.55,15.15),(16,7.1,6.1),'Ivory',.14,True)
    for y in (.34,3.37,6.93):
        room.box('Facade cornice',(0,y,11.96),(16.35,.20,.31),'Trim',.05)
    for x in (-7.55,-3.95,3.95,7.55):
        room.box('Pilaster',(x,3.55,11.96),(.38,6.93,.29),'Plaster',.05)
    for level in (0,1):
        for x in (-6.1,-3,0,3,6.1):
            if not level and x==0:
                continue
            y=1.95+level*3.25
            room.box('Terracotta window surround',(x,y,12),(1.63,2.21,.28),'Clay',.12)
            room.box('Inset window glass',(x,y,11.81),(1.25,1.87,.11),'Glass',.04)
            for dx in (-.62,0,.62):
                room.box('Window vertical mullion',(x+dx,y,11.71),(.09,1.95,.14),'DarkOak')
            room.box('Window transom',(x,y+.26,11.71),(1.29,.09,.14),'DarkOak')
            room.box('Window stone sill',(x,y-1.03,11.65),(1.84,.17,.49),'Trim',.05)
            if level and x in (-3,0,3):
                room.box('Balcony sill',(x,y-1.06,11.28),(1.9,.17,.86),'Plaster',.07)
                for j in range(7):
                    room.box('Balcony baluster',(x-.84+j*.28,y-.69,10.89),(.057,.68,.059),'Iron')
                room.box('Balcony handrail',(x,y-.35,10.89),(1.86,.09,.10),'Iron')
    room.box('Door casing',(0,1.55,11.92),(2.45,3.1,.36),'Trim',.16)
    room.box('Two leaf front door',(0,1.43,11.68),(1.94,2.86,.18),'DarkOak',.09)
    for x in (-.48,.48):
        room.box('Carved door panel',(x,1.45,11.56),(.77,2.5,.09),'Oak',.1)
        room.lathe('Brass knob',(x*.38,1.28,11.43),[(.084,-.056),(.088,.04)],'Trim',12)
    for side in (-1,1):
        for row in range(5):
            for i in range(33):
                x=-8.25+i*.516+(row%2)*.08
                z=15.15+side*(3.34-row*.66)
                roof=room.box('Overlapping clay tile',(x,7.17+row*.29,z),(.56,.17,.78),'Clay' if (i+row)%3 else 'ClayLight',.07)
                roof.rotation_euler.x=side*.39
    room.box('Roof ridge',(0,8.56,15.15),(16.97,.22,.40),'Clay',.10)
    # Two wings around the basin and an approach along the garden's depth.
    for index,(a,b) in enumerate([((0,.025,11.9),(0,.025,5.3)),((0,.025,5.3),(-5,.025,3.8)),((0,.025,5.3),(5,.025,3.8)),
                ((-5,.025,3.8),(-5,.025,-3.2)),((5,.025,3.8),(5,.025,-3.2)),
                ((-5,.025,-3.2),(0,.025,-5.6)),((5,.025,-3.2),(0,.025,-5.6)),((0,.025,-5.6),(13.8,.025,-7.5))]):
        lift=index*.0012
        room.segment('Sandstone path',(a[0],a[1]+lift,a[2]),(b[0],b[1]+lift,b[2]),2.95,.045,'Path')
    for j in range(22):
        z=11.35-j*.29
        room.box('Inlaid courtyard paver',(0,.056,z),(2.46,.045,.245),'Paver' if j%3 else 'Plaster',.025)
    room.lathe('Carved fountain basin',(0,0,1),[(2.68,0),(2.82,.12),(2.82,.27),(2.62,.39),(2.62,.63),(2.79,.70),(2.79,.83),(2.51,.83),(2.48,.56)],'Ivory',48)
    room.lathe('Water in basin',(0,.65,1),[(2.49,-.03),(2.49,.015)],'Water',48)
    room.lathe('Fountain pedestal',(0,.54,1),[(.75,0),(.79,.13),(.55,.25),(.42,.55),(.28,1.2),(.43,1.39),(.58,1.52)],'Plaster',32)
    room.lathe('Scalloped upper bowl',(0,2.04,1),[(.48,0),(.84,.12),(1.07,.3),(1.12,.38),(.96,.39),(.54,.18)],'Ivory',32)
    room.lathe('Water jet',(0,2.19,1),[(.08,0),(.065,.71),(.025,.88)],'Water',10)
    room.boundary('Basin collision',(0,1.2,1),(4.8,2.4,4.8))
    # Iron enclosure. Front curb gives a clear foreground edge from the gameplay camera.
    def fence(a,b,height):
        length=math.hypot(b[0]-a[0],b[1]-a[1]);count=math.ceil(length/.63)
        for i in range(count+1):
            t=i/count;x=a[0]+(b[0]-a[0])*t;z=a[1]+(b[1]-a[1])*t
            room.box('Wrought iron bar',(x,height*.5,z),(.065,height,.065),'Iron',.014)
            if i%4==0:
                room.lathe('Fence spear finial',(x,height,z),[(.10,0),(.12,.10),(.015,.25)],'Trim',4)
        for y in (.24,height-.15):
            room.segment('Wrought rail',(a[0],y,a[1]),(b[0],y,b[1]),.085,.09,'Iron')
    fence((-14,-15),(14,-15),.86)
    fence((-14,-15),(-14,17),2.22)
    fence((-14,17),(14,17),2.22)
    fence((14,-15),(14,-9.1),2.22);fence((14,-5.9),(14,17),2.22)
    fence((14,-9.1),(14,-5.9),2.68)
    for z in (-9.22,-5.78):
        room.box('Gate pillar',(14,1.58,z),(.75,3.16,.75),'Ivory',.12)
        room.box('Gate cornice',(14,3.17,z),(.96,.21,.96),'Trim',.08)
        room.lathe('Stone urn',(14,3.24,z),[(.2,0),(.22,.12),(.35,.28),(.28,.47),(.14,.62)],'Ivory',16)
    room.box('Exterior stepping apron',(15.9,-.18,-7.5),(3.75,.35,4.0),'Path',.08)
    for x,z in [(-10.8,9),(-9.7,3.5),(10.7,8.8),(10.8,2),(-9,-7),(10.5,-1)]:
        room.lathe('Tree trunk',(x,0,z),[(.38,0),(.25,.48),(.16,2.65)],'DarkOak',9)
        room.boundary('Tree trunk',(x,1.1,z),(.48,2.2,.48))
        for dx,dy,dz in ((0,3.1,0),(-.65,2.6,.4),(.68,2.73,-.28),(0,3.75,.05)):
            room.rock('Layered crown',(x+dx,dy,z+dz),(2.3,2.1,2.2),'LeafLight' if dy>3 else 'Leaf',9)
    for x,z in [(-7,5),(7,5),(-7,-3),(7,-3),(-3,8),(3,8)]:
        room.box('Garden bed',(x,.04,z),(1.8,.08,2.5),'Earth',.16)
        for i in range(11):
            xx=x+room.random.uniform(-.72,.72);zz=z+room.random.uniform(-1,1)
            room.rock('Leaves',(xx,.17,zz),(.38,.34,.34),'Leaf')
            room.lathe('Flower',(xx,.31,zz),[(.09,0),(.16,.09),(.09,.13)],'Rose' if i%3 else 'Linen',6)
    for x,z in [(-8.3,1),(8.3,1),(-7,-5)]:
        room.box('Bench frame',(x,.36,z),(2.65,.16,.78),'Iron')
        for side in (-1,1):
            room.box('Bench foot',(x+side*.99,.22,z),(.16,.44,.72),'Iron')
            room.box('Bench back post',(x+side*1.02,.7,z+.31),(.13,1.4,.13),'Iron')
        for j in range(4):
            room.box('Bench slat',(x,.50,z-.30+j*.2),(2.55,.09,.17),'Oak',.055)
            room.box('Bench back slat',(x,.72+j*.18,z+.37),(2.55,.14,.10),'Oak',.05)
        room.boundary('Bench collision',(x,.48,z),(2.65,.95,.76))
    for x,z in [(-3.8,6.3),(3.8,6.3),(-3.8,-4.7),(3.8,-4.7),(11.8,-5.55)]:
        room.lathe('Garden lamp pole',(x,0,z),[(.25,0),(.25,.18),(.13,.31),(.09,2.25),(.15,2.42)],'Iron',10)
        lantern(room,x,2.69,z)
    return room


def bedroom():
    room=Room('PRO_CASA')
    room.box('Parquet substrate',(7,-.2,1.5),(14,.4,9),'DarkOak',.06,True)
    for i in range(28):
        for j in range(3):
            room.box('Parquet board',(.25+i*.5,.014,-1.5+j*3),(.487,.028,2.984),'Oak' if (i+j)%3 else 'DarkOak',.014)
    room.box('Wallpaper',(7,3.0,5.93),(14,6,.28),'Ivory',.06,True)
    for x in (0,14):
        room.boundary('Side wall',(x,3,1.5),(.27,8,9))
    room.boundary('Invisible theatre edge',(7,3,-3),(14,8,.27))
    for i in range(35):
        room.box('Wallpaper stripe',(.15+i*.4,3.4,5.77),(.034,4.56,.022),'Trim',.005)
    for y in (.15,1.15,5.71):
        room.box('Wood wainscot trim',(7,y,5.71),(14,.15,.14),'DarkOak')
    room.box('Rug border',(7,.043,.3),(6.4,.035,4.25),'Trim',.03)
    room.box('Violet woven rug',(7,.069,.3),(6.09,.035,3.94),'Velvet',.03)
    for x in (4.28,9.72):
        for z in (-1.42,2.02):
            ornament=room.box('Woven diamond',(x,.093,z),(.27,.02,.27),'Trim',.01)
            ornament.rotation_euler.z=.7854
    room.box('Bed base',(2.5,.55,3.1),(2.9,.65,3.85),'DarkOak',.16,True)
    room.box('Folded coverlet',(2.5,.97,2.98),(2.82,.23,3.47),'Velvet',.12)
    room.box('Coverlet hem',(2.5,1.10,1.52),(2.68,.055,.28),'Trim',.03)
    room.box('Carved headboard',(2.5,1.65,4.95),(3.15,2.35,.30),'Oak',.19,True)
    room.box('Headboard inset',(2.5,1.75,4.75),(2.44,1.45,.16),'DarkOak',.17)
    for x in (1.07,3.93):
        room.lathe('Turned bed post',(x,.16,4.83),[(.18,0),(.22,.17),(.14,.29),(.13,2.26),(.23,2.42),(.11,2.61)],'Oak',16)
    for x in (1.92,3.08):
        room.box('Pillow',(x,1.17,4.11),(1.04,.27,.77),'Linen',.13)
    room.box('Window recess',(6.9,3.44,5.70),(2.65,3.3,.16),'DarkOak',.14)
    room.box('Blue window',(6.9,3.46,5.59),(2.39,3.07,.10),'Glass',.07)
    for x in (5.7,6.9,8.1):room.box('Window frame',(x,3.46,5.5),(.13,3.2,.12),'Oak')
    room.box('Window crosspiece',(6.9,3.38,5.5),(2.4,.13,.12),'Oak')
    for sign in (-1,1):
        for i in range(8):
            room.lathe('Velvet drapery fold',(6.9+sign*(1.34+i*.12),1.69,5.28+(.05 if i%2 else 0)),[(.125,0),(.11,1.65),(.14,3.6)],'Velvet',10)
    room.segment('Curtain rod',(4.53,5.38,5.33),(9.26,5.38,5.33),.12,.12,'Trim')
    room.box('Writing desk top',(7.2,1.02,3.5),(2.9,.19,1.33),'Oak',.1,True)
    room.box('Folded letter',(7.2,1.13,3.35),(.67,.03,.45),'Linen',.015)
    room.box('Writing desk drawers',(7.2,.82,3.52),(2.77,.29,1.15),'DarkOak',.065)
    for x in (6.06,8.34):
        for z in (3.02,3.98):
            room.lathe('Turned desk leg',(x,0,z),[(.1,0),(.15,.11),(.086,.3),(.10,.65),(.16,.79)],'Oak',12)
    room.box('Family portrait frame',(4.54,3.58,5.62),(1.13,1.48,.17),'Trim',.14)
    room.box('Portrait silhouette',(4.54,3.58,5.49),(.88,1.22,.11),'Velvet',.15)
    room.box('Wardrobe body',(10.44,1.74,4.69),(1.82,3.48,1.41),'DarkOak',.14,True)
    room.box('Wardrobe cornice',(10.44,3.48,4.69),(2.05,.23,1.62),'Oak',.10)
    for x in (10.01,10.87):
        room.box('Wardrobe raised panel',(x,1.77,3.93),(.72,2.96,.15),'Oak',.13)
    room.box('Garden door',(12.7,1.48,4.58),(1.68,2.96,.19),'DarkOak',.09)
    room.box('Door moulding',(12.7,2.99,4.47),(2.16,.20,.41),'Trim',.08)
    for x in (11.80,13.60):room.box('Door jamb',(x,1.49,4.47),(.19,3,.37),'Plaster',.05)
    lantern(room,12.7,3.69,4.54)
    return room


def mine(number):
    room=Room(f'N01S{number:02d}')
    width,depth,middle={1:(22,18,1),2:(26,22,2),3:(24,24,2),4:(24,22,2)}[number]
    room.floor(width,depth,middle,'Dust')
    if number==1:
        portal(room,1,0,8.9)
        room.segment('Valley footpath',(-1,.025,-8),(-1,.025,-4.3),2.2,.045,'Path')
        room.segment('Village footpath',(-11,.025,-3.8),(-7.3,.025,-3.8),1.95,.045,'Path')
        for x in (-2.36,.36):
            room.box('Valley trail post',(x,.53,-7.56),(.21,1.06,.22),'Oak',.06)
            room.lathe('Valley marker',(x,1.08,-7.56),[(.13,0),(.15,.12),(.08,.21)],'Lamp',8)
        for x,z in [(-7,2),(7,4)]:
            room.box('Work shelter beam',(x,2.58,z+1.8),(3.6,.28,1),'DarkOak',.07)
            for dx in (-1.62,1.62):room.box('Workshop support',(x+dx,1.32,z+1.92),(.23,2.6,.27),'Oak',.05)
            crate(room,x+1.03,z+1)
            lantern(room,x-1.45,2.14,z+1.52)
        room.lathe('Carved altar plinth',(4,0,-3),[(.75,0),(.79,.12),(.61,.26),(.55,.74),(.77,.93),(.79,1.03)],'SlateLight',12)
        room.rock('Altar emblem',(4,1.31,-3),(.49,.64,.36),'Crystal')
        track(room,(1,0,-.65),(1,0,8.8))
    if number==2:
        portal(room,-8,0,-7.7);portal(room,8,2.2,10.5)
        room.rock('Central island',(0,1.35,1.6),(5.2,3.25,6.6),'Slate')
        room.boundary('Island',(0,1.36,1.6),(4.5,2.72,5.6))
        for z,h in [(1,.55),(3,1.10),(5,1.65)]:terrace(room,6,z,5,2,h)
        terrace(room,7,9,7,6,2.2)
        track(room,(-8,0,-6.5),(-8,0,6.6));track(room,(-8,0,6.6),(-3,0,9.2))
        crate(room,-7,7)
        room.box('Terrace lamp post',(10.2,2.93,8.9),(.20,1.47,.20),'Oak',.05)
        lantern(room,10.2,3.72,8.9)
    if number==3:
        portal(room,6,0,-9.2);portal(room,-6,0,11.9)
        room.rock('BreakableBoulder',(4.1,.58,2.15),(1.55,1.25,1.4),'SlateLight')
        for x,z in [(6,4),(4.5,8),(7,9.2)]:
            room.rock('Cavern pillar',(x,2,z),(3.8,4.5,3.8),'Slate')
            room.boundary('Cavern pillar',(x,1.85,z),(2.6,3.7,2.7))
        track(room,(6,0,-8.9),(4,0,-3.7));track(room,(4,0,-3.7),(-4,0,1.2));track(room,(-4,0,1.2),(-6,0,11.7))
        for x,z in [(9,-3),(-8,5),(-3,10)]:
            room.box('Timber lamp post',(x,1.52,z),(.24,3.04,.24),'Oak',.05)
            lantern(room,x,2.56,z-.21)
        crate(room,8,-4.5)
    if number==4:
        portal(room,5,0,-8.2)
        for z,h in [(1,.55),(3,1.10),(5,1.65)]:terrace(room,-4,z,5.6,2,h)
        terrace(room,-4.8,9,8.5,6,2.2)
        portal(room,-4,2.2,11.5)
        for i in range(9):
            room.rock('Caved-in boulders',(-5.15+i%3*1.12,2.57+i//3*.68,11.13), (1.4,1.24,1.13),'Slate')
        track(room,(5,0,-7.6),(5,0,5.4))
        room.box('Lower gallery lamp post',(7.65,1.05,4.1),(.23,2.10,.23),'Oak',.05)
        lantern(room,7.65,2.32,4.1)
        crate(room,8.35,5)
    for i in range(15):
        side=-1 if i%2==0 else 1;z=middle-depth/2+1+(i//2)*depth/9
        x=side*(width/2-room.random.uniform(.52,1.04))
        room.rock('Broken strata debris',(x,.13,z),(room.random.uniform(.4,1.1),.35,.68),'SlateLight')
        if i%4==0:room.rock('Mineral seam',(x,.29,z+.52),(.39,.74,.36),'Crystal',5)
    return room


def miniature():
    room=Room('M01_MAPA')
    room.box('Diorama soil',(0,-.66,1),(22,1.35,14.5),'Earth',.65)
    room.box('Diorama grass',(0,.04,1),(21.9,.13,14.4),'Grass',.42)
    points=[(-9,.21,-5),(-5,.21,-3),(0,.21,-3),(5,.21,1),(0,.21,5),(-5,.21,5),(-9,.21,5)]
    for a,b in zip(points,points[1:]):room.segment('Expedition road',a,b,.62,.05,'Paver')
    for i in (1,5,6):
        x,y,z=points[i]
        room.rock('Mine hillside',(x,.77,z+.44),(2.8,2.5,2.5),'Slate')
        room.box('Mine opening',(x,.48,z-.49),(1.13,.88,.12),'Tunnel',.10)
        for dx in (-.62,.62):room.box('Mine frame',(x+dx,.55,z-.58),(.18,1.1,.23),'Oak')
        room.box('Lintel',(x,1.12,z-.58),(1.52,.20,.25),'DarkOak')
        if i==5:room.lathe('Mine lantern',(x+.55,.88,z-.77),[(.11,0),(.14,.26)],'Lamp',7)
        if i==6:
            for j in range(4):room.rock('Blocked mouth',(x-.46+j*.29,.32,z-.73),(.49,.55,.40),'SlateLight')
    track(room,(-1.08,.15,-3),(1.08,.15,-3));track(room,(0,.15,-4),(0,.15,-2))
    room.box('Mini cart',(.55,.45,-2.81),(.66,.46,.61),'Oak',.10)
    room.box('Crossing sign post',(5,.62,1),(.12,1.23,.14),'Oak')
    room.box('Crossing sign',(5,1.16,.94),(1.4,.28,.14),'Paver',.07)
    for x in (-1.2,-.4,.4,1.2):
        room.box('Viaduct column',(x,.49,5),(.21,.98,.50),'Slate')
    room.box('Viaduct deck',(0,1.03,5),(3.0,.18,.70),'Oak',.08)
    for x,z in [(-9.8,-4.8),(-8.6,-5.6),(-10.05,-3.59)]:
        room.box('Village house',(x,.41,z),(.79,.83,.68),'Ivory',.06)
        roof=room.box('Village tiled roof',(x,.88,z),(.94,.17,.86),'Clay',.07);roof.rotation_euler.x=.25
    for i in range(24):
        x=room.random.uniform(-10.2,10.2);z=room.random.choice((-5.65,7.17))+room.random.uniform(-.3,.3)
        room.rock('Valley rim',(x,.41,z),(room.random.uniform(.6,2.3),room.random.uniform(.65,1.45),1.24),'Slate')
    return room


def evaluate(room):
    bpy.context.window.scene = room.scene
    bpy.context.view_layer.update()
    depsgraph=bpy.context.evaluated_depsgraph_get()
    grouped={}
    for obj in room.parts:
        evaluated=obj.evaluated_get(depsgraph)
        mesh=evaluated.to_mesh()
        mesh.calc_loop_triangles()
        material=obj.data.materials[0].name
        group=('BreakableBoulder' if obj.name.startswith('BreakableBoulder') else material, material)
        vertices,triangles=grouped.setdefault(group,([],[]))
        offset=len(vertices)
        for vertex in mesh.vertices:
            position=obj.matrix_world @ vertex.co
            vertices.append((-position.x,position.y,position.z))
        triangles.extend(tuple(index+offset for index in reversed(triangle.vertices)) for triangle in mesh.loop_triangles
                         if triangle.area>1e-9)
        evaluated.to_mesh_clear()
    result=[]
    for (name,material),(vertices,triangles) in grouped.items():
        obj=room.mesh('Export_'+name, vertices, triangles, material)
        bm=bmesh.new();bm.from_mesh(obj.data);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(obj.data);bm.free()
        result.append(obj)
    return result


def preview(room):
    scene=room.scene
    camera=bpy.data.objects.new('Review camera',bpy.data.cameras.new('Review camera'))
    scene.collection.objects.link(camera);scene.camera=camera
    focus=(7,1.6,2) if room.name=='PRO_CASA' else (0,1,1)
    camera.location=xyz((focus[0]+9,14,-15) if room.name=='PRO_CASA' else (16,25,-25))
    camera.rotation_euler=(Vector(xyz(focus))-camera.location).to_track_quat('-Z','Y').to_euler()
    camera.data.type='ORTHO';camera.data.ortho_scale=19 if room.name=='PRO_CASA' else 39 if room.name=='PRO_PATIO' else 32
    scene.render.engine='BLENDER_WORKBENCH'
    scene.display.shading.color_type='MATERIAL'
    scene.display.shading.show_cavity=True
    scene.display.shading.cavity_type='BOTH'
    scene.display.shading.show_shadows=True
    scene.render.resolution_x=840;scene.render.resolution_y=620;scene.render.resolution_percentage=100
    scene.render.image_settings.file_format='PNG';scene.render.filepath=str(SOURCE / 'Revision' / (room.name+'.png'))
    bpy.ops.render.render(write_still=True)


def main():
    EXPORT.mkdir(parents=True,exist_ok=True)
    (SOURCE/'Revision').mkdir(exist_ok=True)
    starter_scene=bpy.context.scene
    rooms=[bedroom(),garden(),*(mine(i) for i in range(1,5)),miniature()]
    bpy.data.scenes.remove(starter_scene)
    report=[]
    for room in rooms:
        bpy.context.window.scene=room.scene
        # Representative shaped surface is checked independently, as room parts intentionally sit apart.
        representative=next(obj for obj in room.parts if obj.modifiers and obj.dimensions.length>0)
        hs.apply_scale(representative)
        gate=bx_audit.deliverable([representative],profile='game')
        if not gate['pass']:
            raise RuntimeError((room.name,representative.name,gate['errors']))
        export_parts=evaluate(room)
        axis=bpy.data.objects.new('AxisWitness',None);room.scene.collection.objects.link(axis);axis.location=xyz((-3,2,7))
        bx_export.export(str(EXPORT/(room.name+'.fbx')),'unity',export_parts+[axis],embed_textures=False,path_mode='RELATIVE')
        triangles=sum(len(obj.data.polygons) for obj in export_parts)
        for obj in export_parts+[axis]:bpy.data.objects.remove(obj,do_unlink=True)
        (EXPORT/(room.name+'.json')).write_text(json.dumps({'boxes':room.colliders}),encoding='utf-8')
        report.append({'room':room.name,'sourceParts':len(room.parts)-len(export_parts),'triangles':triangles,'sampleGate':gate['pass']})
    bpy.context.window.scene=rooms[1].scene
    bpy.ops.wm.save_as_mainfile(filepath=str(SOURCE/'Afterglow_Prologo_Nivel1.blend'))
    for room in rooms:
        bpy.context.window.scene=room.scene
        preview(room)
    (SOURCE/'Auditoria.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
    print('RESULT '+json.dumps(report))


if __name__=='__main__':
    main()
