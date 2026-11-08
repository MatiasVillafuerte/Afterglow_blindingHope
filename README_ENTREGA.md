# Afterglow Documentación de producción

Versión 0.3 · 2 de noviembre de 2026

Este paquete contiene el GDD autónomo, el diseño del mundo uno, 181 líneas de guion y acotaciones identificadas, un checklist de 61 tareas y el encargo para Codex o Claude. El proyecto Unity real debe inspeccionarse antes de implementar. Ninguna casilla se entrega como funcionalidad ya verificada.

## Dónde colocarlo

Descomprime el paquete y copia su contenido en la raíz del proyecto Unity, junto a Assets, Packages y ProjectSettings. Combina Docs y Reference y copia README_ENTREGA.md y Prompt_Codex.md. Son documentación; no reemplazan esas tres carpetas de Unity. Si ya existe documentación con esos nombres, conserva una copia de respaldo antes de sustituir sus archivos. Si el agente ya avanzó, conserva su Estado_Trabajo.md y las casillas verificadas del checklist; integra las instrucciones nuevas sin sobrescribir ese progreso con las plantillas.

El proyecto mantiene esta organización de referencia:

- Reference/General/Imagenes y Reference/General/Videos para cámara, HUD, menús y presentación compartida.
- Reference/CharacterSource para fuentes, piezas y animaciones del protagonista.

- Reference/Mundo 1/Imagenes y Reference/Mundo 1/Videos.
- Reference/Mundo 2/Imagenes y Reference/Mundo 2/Videos.
- Reference/Mundo 3/Imagenes y Reference/Mundo 3/Videos.
- Reference/Mundo 4/Imagenes y Reference/Mundo 4/Videos.
- Reference/Mundo 5/Imagenes y Reference/Mundo 5/Videos.
- Reference/Mundo 6/Imagenes y Reference/Mundo 6/Videos.

Cada mundo tiene INDICE_REFERENCIAS.md y General tiene INDICE_GENERAL.md. Mantén ahí nombre o enlace, fragmento del video, intención de diseño, nivel y estado de revisión. Los videos originales se aportarán a sus carpetas; no se incluyen ni se dan por vistos en esta entrega. La cámara actual ya está aprobada y se conserva. Los informes generados se guardan en Docs/Referencias/Analisis, y los recursos del juego ya importados permanecen en Assets.

## Documentos

| Archivo | Uso |
| --- | --- |
| Docs/GDD/Afterglow_GDD_v0_3.md | Fuente de reglas e historia para agentes |
| Docs/GDD/Afterglow_GDD_v0_3.docx | Edición de lectura del mismo GDD |
| Docs/Diseno/Mundo_1_Diseno.md | Secciones, misiones, economía, jefe y estado |
| Docs/Guion/Mundo_1_Guion.md | Líneas de producción y condiciones |
| Docs/Implementacion/Checklist_Mundo_1.md | Orden, dependencias y evidencia de ejecución |
| Docs/Implementacion/Estado_Trabajo.md | Continuidad entre sesiones y agentes |
| Docs/Referencias/Organizacion_Reference_y_Herramientas.md | Destinos de archivos existentes y revisión de herramientas |
| Docs/Implementacion/Plan_Herramientas_y_Assets.md | Informe que el agente completa antes de programar |
| Reference/General/INDICE_GENERAL.md y los índices por mundo | Qué estudiar de imágenes y videos |
| Prompt_Codex.md | Texto para iniciar el trabajo |

Lee el GDD si quieres revisar el diseño global. Consulta el archivo de diseño para cantidades y habitaciones, y el guion para las líneas exactas. El GDD Markdown y Word tienen el mismo contenido sustantivo; la imagen del mapa se integra en Word y se enlaza en Markdown.

## Cómo dárselo al agente

Abre Codex en la carpeta real del proyecto, con estas carpetas presentes. Pega el contenido de Prompt_Codex.md. El agente lee los documentos, inspecciona el prototipo y presenta primero el plan de herramientas y assets. Puede instalar opciones gratuitas compatibles si dispone de acceso; para acciones manuales informa de recursos, enlaces y pasos antes de programar. Conserva la cámara aprobada y continúa por hitos. No depende de que le resumas la historia en el chat.

Si dispones del proyecto 2D con animaciones, déjalo como fuente aparte e indica su ubicación. Preferiblemente exporta desde Unity un .unitypackage de sprites, clips, Animator, prefab y sus dependencias, conservando .meta. No copies todo el proyecto 2D encima del 3D. El código de movimiento se inspecciona antes de reutilizarlo; su física puede requerir adaptación al escenario 3D.

## Alcance

La producción inmediata abarca prólogo y mundo uno completo con recursos provisionales. Se incorpora el hijo al final, sin modo de rejugar para probarlo. Continuará lleva al menú principal. El mundo dos conserva su historia preliminar; tres al seis conservan su función narrativa. No se encarga construir esos mundos durante esta entrega.

Los números son punto de partida para pruebas. Los ajustes se registran sin cambiar por cuenta propia la historia, la persistencia ni las condiciones de apertura. El arte definitivo, nombres y emblema pueden quedar como activos sustituibles sin detener el prototipo.

## Prioridad vigente: prólogo y mapa v0.4
El encargo Prompt_Codex_Prologo_y_Mapa.md acota esta entrega a PM00–PM20: primero prólogo integrado, después selector del acceso y pruebas. No implica terminar seis niveles ni jefe en la misma sesión. Las reglas detalladas están en Docs/Diseno/Prologo_Mapa_y_Escenarios_v0_4.md; la ruta de ejecución y sus límites están en Docs/Implementacion/PM/Como_Probar_Entrega.md. Se preservan el checklist maestro y los hitos posteriores.
