"""Editable final plan generated from the same collider manifest imported into Unity."""
from pathlib import Path
import json, html
root=Path(__file__).resolve().parents[2]
titles=['Refugio · orientación y retorno','Cantera · desplazamiento y salto','Taller · pico, combate y ramal','Garganta · mecanismo y combinación']
routes=[[(-1,-5.55),(-1,-3.9),(1,2),(1,8.1)],[(-8,-5.2),(-6,-3.5),(-1.3,-3.5),(2,-2),(5,1),(6,4.3),(7,7),(8,10)],[(6,-6.7),(1,0),(8,1.6),(-5,3),(-5,8),(-6,11.4)],[(5,-5.8),(4,-2.2),(2,-.8),(2,6),(-1.3,6),(-4,6),(-4,8),(-4,10)]]
anchors=[[("A",-1,-5.55),("H",-1,-3.9),("T",4,-3),("Q1",-7,2),("Q2",7,4),("P",-10,-3.8),("D",1,8.1)],[("A",-8,-5.2),("O",5,1),("D",8,10)],[("A",6,-6.7),("E1",1,0),("E2",-5,3),("E3",-5,8),("O",8,1.6),("R",-6,9),("D",-6,11.4)],[("A",5,-5.8),("L",4,-1.2),("Q2",8,8),("F",-4,10)]]
notes=[['A: aparición segura; D: puerta S02','H: N1A/N1C · teléfono único','T: altar compartido · cuota 3','Q1/Q2: aceptar y devolver','P: pueblo; mapa al sur del acceso'],['O: EXP.N01S02.0 · fuente fija','Cotas: 0 / 0,65 / 1,30 / 1,95 / 2,20','Saltos y profundidad alternados','Caída: retorno a A, daño 2'],['N1B entrega pico antes de agresión','E1: resistencia y ofrenda garantizada','E2/E3: ramal opcional para Q01','O: EXP.N01S03.0; R: roca con X','D: salida a S04 tras cuello minero'],['L: palanca E · N1E.02','Puente: N01.S04.BRIDGE_OPEN','Q2: casco opcional a derecha','F: N1D · investigación tras subir','T teléfono: volver a S01 sin aportar']]
svg=['<svg xmlns="http://www.w3.org/2000/svg" width="1600" height="1240" viewBox="0 0 1600 1240">','<rect width="1600" height="1240" fill="#17232d"/>','<style>text{font-family:Arial;fill:#f5ecd4}.title{font-size:22px;font-weight:bold}.note{font-size:15px}.small{font-size:13px}</style>','<text x="35" y="38" class="title">N1 · plano final editable · X lateral / Z profundidad / Y altura (metros)</text>']
for i in range(4):
    ox=20+(i%2)*790;oy=60+(i//2)*550;cx=ox+390;cy=oy+320;s=17
    xy=lambda x,z:(cx+x*s,cy-z*s)
    svg.append(f'<g id="N01S0{i+1}"><rect x="{ox}" y="{oy}" width="770" height="530" rx="12" fill="#223541"/><text x="{ox+20}" y="{oy+32}" class="title">S0{i+1} · {titles[i]}</text>')
    boxes=json.loads((root/f'Assets/Afterglow/Environment/Modelado/Nivel1/N01S0{i+1}.json').read_text())['boxes']
    for b in boxes:
        x,y,z=b['center'];w,h,d=b['size'];px,pz=xy(x-w/2,z+d/2);top=y+h/2
        color='#9e9179' if b['name'].startswith(('S1_','S2_','S3_','S4_')) else '#4b5561'
        if any(k in b['name'] for k in ['Back','Neck_left','Neck_right','Ore_mass','Derrumbe','Crate']):color='#4b5561'
        svg.append(f'<rect id="{html.escape(b["name"])}" x="{px}" y="{pz}" width="{w*s}" height="{d*s}" fill="{color}" stroke="#ddd" stroke-width="1"><title>{html.escape(b["name"])} · cota {top:.2f} m</title></rect>')
    if i==3:
        px,pz=xy(.5,5.1);svg.append(f'<rect id="Puente" x="{px}" y="{pz}" width="51" height="88.4" fill="#c8a24d"/>')
    points=' '.join(f'{xy(x,z)[0]},{xy(x,z)[1]}' for x,z in routes[i])
    svg.append(f'<polyline points="{points}" fill="none" stroke="#b5ffe0" stroke-width="4" stroke-dasharray="7 5"/>')
    for label,x,z in anchors[i]:
        px,pz=xy(x,z);svg.append(f'<circle cx="{px}" cy="{pz}" r="11" fill="#163d51" stroke="#ffe2a0"/><text x="{px-7}" y="{pz+5}" class="small">{label}</text>')
    for j,n in enumerate(notes[i]):svg.append(f'<text x="{ox+20}" y="{oy+415+j*20}" class="note">{html.escape(n)}</text>')
    svg.append('</g>')
svg+=['<text x="35" y="1190" class="note">Ruta: M01_MAPA → S01 ↔ S02 ↔ S03 ↔ S04. Línea verde: recorrido comprobado; puertas internas bidireccionales.</text>','<text x="35" y="1215" class="note">Geometría importada del manifiesto final. A: aparición; D: salida. N2 solo se desbloquea; su recorrido queda fuera de esta entrega.</text></svg>']
(root/'Docs/Diseno/Plano_Nivel1.svg').write_text('\n'.join(svg),encoding='utf-8')
print('Plano final actualizado')
