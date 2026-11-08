"""Integración local reproducible; conserva originales y estados verificados."""
from pathlib import Path
import re, shutil, json
R=Path(__file__).resolve().parents[2]
incoming=R/'Afterglow_Entrega_v0_5'
history=R/'Docs/Historial/Antes_Nivel1_v05'
def backup(rel):
    p=R/rel; target=history/rel
    if p.exists() and not target.exists():
        target.parent.mkdir(parents=True,exist_ok=True); shutil.copy2(p,target)
def put(rel,text):
    backup(rel); p=R/rel;p.parent.mkdir(parents=True,exist_ok=True);p.write_text(text,encoding='utf-8')
def source(rel): return (incoming/rel).read_text(encoding='utf-8-sig')
for rel in ['Docs/Diseno/Mundo_1_Canon.md','Docs/Diseno/Metodo_Diseno_Niveles.md','Docs/Implementacion/Protocolo_Documentacion.md','Prompt_Codex_Nivel1.md']:
    put(rel,source(rel))
gdd=source('Docs/GDD/Afterglow_GDD_v0_5.md')
gdd=gdd.replace('M01_ACCESO es la zona segura al principio de la mina, junto al altar de exploración, conectada con el pueblo.', 'N01S01 es la zona segura al principio de la mina, junto al único altar de exploración y conectada con el pueblo. M01_ACCESO es un identificador legado, fuera del arranque activo.')
gdd=gdd.replace('Un panel de rutas abre el mapa de selección del mundo uno: flechas enfocan nodos, E entra en un nivel abierto y Esc vuelve al mismo acceso.', 'El capitán conduce mediante fundido a M01_MAPA, la escena de selección del mundo uno. Flechas enfocan nodos y E entra en un nivel abierto. Tras visitar N1, Esc regresa a N01S01; su salida física permite volver al mapa.')
gdd=gdd.replace('El acceso y N01S01 pueden compartir el mismo espacio seguro y altar, sin duplicarlos.', 'N01S01 reúne acceso seguro y altar, sin duplicarlos; el selector permanece en M01_MAPA.')
put('Docs/GDD/Afterglow_GDD_v0_5.md',gdd)
design=source('Docs/Diseno/Mundo_1_Diseno.md')
design=design.replace('M01_ACCESO se conecta con el pueblo y el altar de exploración al principio de la mina. El panel de rutas abre el mapa con seis nodos; el teléfono vuelve al acceso y no aporta ofrendas. El acceso seguro de N1 puede compartir zona y datos con este espacio, sin duplicar altar o recompensas.', 'El capitán conduce a M01_MAPA. N01S01 es el acceso seguro conectado al pueblo, con un único altar y el regreso del teléfono. La salida física al mapa abre sus seis nodos. M01_ACCESO queda como escena legada fuera del arranque. El teléfono no aporta ofrendas ni abre automáticamente el mapa.')
put('Docs/Diseno/Mundo_1_Diseno.md',design)
script=source('Docs/Guion/Mundo_1_Guion.md')
script=re.sub(r'^(P02\.08[^\n]*)$', 'P02.08 · Acotación: fundido a M01_MAPA; el jugador selecciona N1 y entra en N01S01, acceso seguro de la mina. No repetir la carta ni adelantar N1A antes de entrar.',script,flags=re.M)
put('Docs/Guion/Mundo_1_Guion.md',script)
prologue=source('Docs/Diseno/Prologo_Mapa_y_Escenarios_v0_5.md')
prologue=prologue.replace('selector abierto desde un panel de rutas en el acceso seguro, navegación entre nodos con flechas y E, jardín de aproximadamente 24 × 36 unidades y dormitorio de aproximadamente 8 × 6', 'selector en M01_MAPA tras el capitán, navegación entre nodos con flechas y E y jardín existente de 28 × 32 unidades; conservar las dimensiones y fuentes editables ya verificadas del dormitorio')
prologue=prologue.replace('un fundido lleva al acceso seguro del pueblo minero. No se añade una ruta jugable por carretera. Desde este acceso se utiliza el mapa para entrar en N1.', 'un fundido lleva directamente a M01_MAPA. No se añade una ruta jugable por carretera. Desde el mapa se entra en N01S01.')
prologue=prologue.replace('después del encargo se realiza la transición al acceso minero', 'después del encargo se realiza la transición a M01_MAPA')
start=prologue.index('M01_ACCESO sigue siendo');end=prologue.index('El selector reemplaza',start)
prologue=prologue[:start]+'''N01S01 es la zona segura junto al pueblo y al altar único al comienzo de la mina. Su salida física conduce a M01_MAPA, escena de selección que conserva su cámara propia y foco modal. M01_ACCESO no forma parte de la ruta de arranque actual.

El teléfono T vuelve a N01S01 con los recursos, sin entregar ofrendas ni completar investigación. Mantiene sus bloqueos de jefe, minijuego, escena, conversación, caída y escolta. Consultar su ficha no llama. No se abre el selector desde la pausa en una galería.

N1A/N1C conservan introducción y teléfono únicos. Entrar desde el mapa inicia la expedición; las puertas internas y el retorno no duplican el intento. Esc en M01_MAPA vuelve a N01S01 después de la primera visita; antes de ella se confirma N1.

'''+prologue[end:]
prologue=prologue.replace('capitán, acceso seguro, mapa y entrada real de N1', 'capitán, M01_MAPA y entrada real N01S01')
put('Docs/Diseno/Prologo_Mapa_y_Escenarios_v0_5.md',prologue)
masterrel='Docs/Implementacion/Checklist_Mundo_1.md'
previous=(R/masterrel).read_text(encoding='utf-8-sig')
checked=set(re.findall(r'^- \[[xX]\] (C\d\d) ',previous,re.M))
master=source(masterrel)
master=re.sub(r'^- \[ \] (C\d\d) ',lambda m: '- [x] '+m[1]+' ' if m[1] in checked else m[0],master,flags=re.M)
master=master.replace('Estado inicial de este archivo: ninguna implementación verificada.', 'Estados verificados conservados del proyecto real; historial detallado en Docs/Historial/Antes_Nivel1_v05.')
master+='\n## Estado de producción vigente\n\nPM00–PM20 y el pase espacial/artístico permanecen verificados en su alcance histórico. Evidencias: PM, Camara_Espacial y Docs/Arte/Acabado_2026_11_05. C20–C24 se revalidan para el rediseño N1 mediante LD00–LD19. C53–C55 siguen parciales: existen guardados de exploración probados, faltan sistemas futuros. N2–N6, árbol y jefe no están construidos. Solo la tarea LD indicada en Estado_Trabajo está En curso; otros alcances parciales quedan pendientes.\n'
put(masterrel,master)
nodes=json.loads(source('Docs/Diseno/Mapa_Mundo1_Datos_Referencia.json'))
nodes['acceso']='N01S01';nodes['selector']='M01_MAPA'
nodes['nodos'][0]['entrada']='Assets/Afterglow/Scenes/M01/N01S01.unity'
for node in nodes['nodos'][1:]:node['entrada']=None;node['estado_escena']='Pendiente de producción, fuera del encargo N1'
put('Docs/Diseno/Mapa_Mundo1_Datos_Referencia.json',json.dumps(nodes,ensure_ascii=False,indent=2)+'\n')
for rel in ['Reference/Mundo 1/INDICE_REFERENCIAS.md','Reference/General/INDICE_GENERAL.md']:
    text=(R/rel).read_text(encoding='utf-8-sig').replace('Vagoneta N4','Vagoneta N5').replace('Carretillas N2 y N5','Carretillas N2 y N4').replace('Niveles 1, 2, 3 y 5','Niveles 1, 2, 3 y 4')
    if 'MapasNiveles' not in text:text+='\nMapas para N1: colección original en `Afterglow_Entrega_v0_3/Reference/General/Imagenes/MapasNiveles` desde la raíz Unity. No duplicar imágenes. Registro de inspección en Docs/Referencias/Analisis/Mapas_Nivel1.md.\n'
    put(rel,text)
index=source('INDICE_DOCUMENTACION.md').replace('Reference/General/Imagenes/MapasNiveles/INDICE_ANALISIS.md', 'Docs/Referencias/Analisis/Mapas_Nivel1.md (colección original: Afterglow_Entrega_v0_3/Reference/General/Imagenes/MapasNiveles)').replace('Estado_Trabajo.md real del proyecto','Docs/Implementacion/Estado_Trabajo.md')
index+='\n## Raíz efectiva y conservación\n\nEsta carpeta Unity es la única raíz operativa. Docs contiene especificaciones vigentes; Afterglow_Entrega_v0_3 y v0_5 son paquetes de entrada, no copias activas. El GDD vigente es Markdown v0.5. La copia Word v0.3 queda archivada y desactualizada; regeneración v0.5 pendiente. Los informes PM, Camara_Espacial y Arte conservan pruebas históricas: no sustituyen las revalidaciones LD.\n'
put('INDICE_DOCUMENTACION.md',index)
# Old entry points become pointers; their full originals remain in history.
for rel in ['Docs/GDD/Afterglow_GDD_v0_3.md','Docs/Diseno/Prologo_Mapa_y_Escenarios_v0_4.md','Prompt_Codex.md','Prompt_Codex_Prologo_y_Mapa.md']:
    if (R/rel).exists():put(rel,'# Documento sustituido\n\nConsultar INDICE_DOCUMENTACION.md en la raíz Unity para la única especificación vigente. Original íntegro conservado en Docs/Historial/Antes_Nivel1_v05/'+rel+'.\n')
backup('Docs/GDD/Afterglow_GDD_v0_3.docx')
put('Docs/Historial/Decisiones_Nivel1_v05.md','# Decisiones — 2026-11-05\n\nSe integra canon oficial 0.5: N4 oscuridad/Q09–Q10; N5 vagoneta/Q07–Q08. Se conserva el flujo espacial aprobado capitán → M01_MAPA → N01S01 y jardín 28 × 32, más reciente que la plantilla de acceso del paquete. No se reinician casillas reales ni PM. Documentación actualizada no demuestra implementación. Migración y geometría N1 pendientes de prueba.\n')
validator=source('Tools/Documentacion/Validar_Canon.py').replace("for p in root.rglob('*.md'):","for p in [root/x for x in [gdd,design,script,master,'Docs/Diseno/Mundo_1_Canon.md','Docs/Diseno/Metodo_Diseno_Niveles.md','Docs/Diseno/Prologo_Mapa_y_Escenarios_v0_5.md','Reference/Mundo 1/INDICE_REFERENCIAS.md','INDICE_DOCUMENTACION.md']]:")
put('Tools/Documentacion/Validar_Canon.py',validator)
print('Especificaciones integradas; casillas conservadas:', ', '.join(sorted(checked)))
