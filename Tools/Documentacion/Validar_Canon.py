"""Comprueba referencias compartidas del canon; no sustituye pruebas de Unity."""
from pathlib import Path
import json
import re
import sys

root = Path(__file__).resolve().parents[2]
errors = []

def require(relative, fragment):
    path = root / relative
    if not path.is_file():
        errors.append(f'Falta {relative}')
    elif fragment not in path.read_text(encoding='utf-8'):
        errors.append(f'{relative}: falta referencia vigente {fragment!r}')

gdd = 'Docs/GDD/Afterglow_GDD_v0_5.md'
design = 'Docs/Diseno/Mundo_1_Diseno.md'
script = 'Docs/Guion/Mundo_1_Guion.md'
master = 'Docs/Implementacion/Checklist_Mundo_1.md'
require(gdd, '| 4 Galería sin luz |')
require(gdd, '| 5 La vía suspendida |')
require(design, '### M01N04 Galería sin luz')
require(design, '### M01N05 La vía suspendida')
require(script, '### Escena N4 Oscuridad')
require(script, '### Escena N5 Vagoneta')
for quest, level in [('Q07','N05'),('Q08','N05'),('Q09','N04'),('Q10','N04')]:
    require(design, f'| {quest} {level} |')
require(master, 'C34 N5,')
require(master, 'C36 N4,')

path = root/'Docs/Diseno/Mapa_Mundo1_Datos_Referencia.json'
if path.is_file():
    nodes=json.loads(path.read_text(encoding='utf-8'))['nodos']
    if len(nodes)!=6: errors.append('El mapa debe contener seis niveles')
    elif nodes[3]['nombre']!='Galería sin luz' or nodes[4]['nombre']!='La vía suspendida':
        errors.append('El mapa no aplica N4 oscuridad y N5 vagoneta')
    if [n['cuota_para_siguiente'] for n in nodes] != [3,5,7,9,12,None]:
        errors.append('Cuotas del mapa incompatibles')
else:
    errors.append('Faltan datos del mapa')

text=(root/script).read_text(encoding='utf-8')
ids=re.findall(r'^([A-Z][A-Za-z0-9]*\.\d+)\s*·',text,re.M)
if len(ids)!=len(set(ids)): errors.append('IDs de diálogo duplicados')
tasks=(root/'Docs/Implementacion/Checklist_Nivel_1.md').read_text(encoding='utf-8')
ids_tasks=re.findall(r'^- \[[ xX]\] (LD\d\d) ',tasks,re.M)
if ids_tasks != [f'LD{i:02}' for i in range(20)]:errors.append('Checklist LD incompleto o duplicado')

for p in [root/x for x in [gdd,design,script,master,'Docs/Diseno/Mundo_1_Canon.md','Docs/Diseno/Metodo_Diseno_Niveles.md','Docs/Diseno/Prologo_Mapa_y_Escenarios_v0_5.md','Reference/Mundo 1/INDICE_REFERENCIAS.md','INDICE_DOCUMENTACION.md']]:
    t=p.read_text(encoding='utf-8')
    if t.count('```')%2: errors.append(f'Bloque de código sin cerrar: {p.relative_to(root)}')
    for old in ['| 4 La vía suspendida |','| 5 Galería sin luz |','### M01N04 La vía suspendida','### M01N05 Galería sin luz','Vagoneta N4','Carretillas N2 y N5']:
        if old in t:errors.append(f'Referencia obsoleta en {p.relative_to(root)}: {old}')

if errors:
    print('\n'.join(errors))
    sys.exit(1)
print(f'Canon comprobado: seis niveles, cuotas, misiones, guion ({len(ids)} IDs) y 20 tareas LD.')
print('Esta comprobación valida documentación, no modelado ni comportamiento de Unity.')
