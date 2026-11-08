# Afterglow · Subchecklist de prólogo y selector

Estado de ejecución 2026-11-03: PM00–PM20 verificados para prólogo y selector integrados. Evidencia por ID en Estado_Trabajo.md y carpeta PM. El mundo completo y las escenas N2–N6 continúan pendientes en el maestro.

## Regla de trabajo

Este subchecklist detalla el encargo actual del checklist maestro `Checklist_Mundo_1.md`. Conservar C00–C60 y sus estados reales. Vincular cada tarea de aquí con los C indicados; añadir una subtarea del selector bajo el acceso, sin renumerar lo existente. Una tarea maestro solo se marca terminada cuando cumple su alcance completo, no por completar una parte de este archivo.

Estados: Pendiente, En curso, Implementado sin probar, Verificado y Bloqueado. Solo [x] significa Verificado. Mantener una tarea de implementación en curso. Tras comprobarla, registrar archivos, escena, pasos, resultado y siguiente ID en Estado_Trabajo.md.

La prioridad de esta entrega es prólogo y selector. El agente puede implementar las dependencias mínimas que falten —transiciones, diálogo, progreso o guardado—, pero no usa el orden anterior para posponer el prólogo hasta construir todo el combate ni empieza árboles, jefe o mundos posteriores. Documentar la reordenación de hitos en el checklist local y mantener las dependencias técnicas reales.

## Etapa 0 · Base real y planificación

- [x] PM00 — C00/C01: leer documentación e inspeccionar Git, escenas, arranque, cámara, controlador, animaciones, entrada y guardado. Identificar qué ya funciona y se reutiliza. Sin reescribir por defecto.
- [x] PM01 — C02/C03: verificar acceso real al editor y consola. Revisar herramientas ya instaladas, su función y compatibilidad. Instalar solo dependencias gratuitas útiles y disponibles; no repetir exploración de catálogos de skills como sustituto del trabajo. Registrar bloqueos concretos.
- [x] PM02 — C03/C60: integrar estas reglas en los documentos locales, añadir la subtarea del mapa y un plan con las etapas de este archivo. Conservar casillas e historial. Elaborar inventario de archivos/escenas a modificar antes de editarlos.

Salida: existe un plan corto basado en el proyecto real y se comienza PM03 sin otra confirmación rutinaria.

## Etapa 1 · Prólogo integrado

- [x] PM03 — C04/C19: conectar Partida nueva con la secuencia de prólogo y las escenas o zonas reales. Preservar el prototipo útil. Play desde la escena de entrada debe llegar al prólogo.
- [x] PM04 — C09/C19: carta P01, avance progresivo con E, gesto sencillo y timbre único. Control bloqueado hasta terminar la carta; no se añaden revelaciones narrativas. Mantener E no omite todo.
- [x] PM05 — C05/C19: dormitorio transitable con puerta única al jardín. Sprites y animaciones existentes; sin pico/teléfono. La cámara conserva su configuración. Probar salida y regreso sin repetir carta ni timbre.
- [x] PM06 — C19/C59: jardín rectangular con casa, fuente rodeable, caminos, vegetación y valla continua. Probar ambos lados de la fuente, límites y saltos contra la valla. Ningún adorno impide el recorrido principal.
- [x] PM07 — C09/C19: capitán fuera del portón, interacción accesible desde dentro, P02 y transición al acceso seguro una sola vez. No repetir encargo al cargar progreso posterior.
- [x] PM08 — C19/C59: acabado visual del dormitorio y jardín según los conceptos. Materiales, piezas modeladas, luz y composición reconocibles con la cámara real. Capturas jugables de interior, exterior y portón; no solo vista de Scene.

Puerta de salida de etapa: recorrido carta → casa → jardín → capitán → acceso probado desde el inicio normal. No pasar al mapa dejando el prólogo solo en una escena de prueba.

## Etapa 2 · Selector y progreso

- [x] PM09 — C20: panel de rutas en M01_ACCESO y vista modal del mapa. E abre, flechas enfocan y Esc cierra; mundo en pausa y foco de UI liberado al volver. Un altar de exploración compartido, no uno nuevo por escena.
- [x] PM10 — C20: seis nodos con IDs y nombres reales, camino, miniaturas y estados legibles. El pueblo no cuenta como nivel. Solo N1 disponible al comenzar; nodos cerrados explican el requisito pendiente.
- [x] PM11 — C13/C15/C20: conectar aperturas con investigación y créditos reales del altar. Cuotas 3/5/7/9/12. No duplicar fuente de progreso, cobrar desde el mapa ni perder aperturas al gastar inventario después.
- [x] PM12 — C04/C18/C20/C21: confirmar un nodo disponible entra en la entrada real de ese nivel una vez. Cerrar mapa no carga nada. Confirmar repetidamente durante fundido no duplica escena ni expedición. Si faltan escenas N2–N6, registrarlo sin inventar contenido terminado.
- [x] PM13 — C16/C17/C18: integrar retorno a M01_ACCESO y disponibilidad del selector allí. Teléfono conserva recursos y restricciones. No habilitar el mapa como teletransporte desde pausa o durante escolta. Volver al acceso por la puerta inicial de N1 también funciona.
- [x] PM14 — C52: preservar cierre del mundo, reintentos previos del jefe y Continuará. El selector de acceso no añade un selector después de ganar ni repetición del jefe.
- [x] PM15 — C53/C54/C55: persistir fases del prólogo y progreso de nodos mediante guardado manual existente. Cargar dentro del nivel conserva su sección. Migración de datos anterior sin borrar recursos ni abrir todo. Abrir/cerrar mapa no guarda ni cura.

Si el sistema completo de cuotas aún no existe, implementar la dependencia mínima real de C13/C15 necesaria para el selector y dejar el resto de esas tareas con su estado honesto. No simular avance mediante un botón de «completar nivel» en el juego final.

## Etapa 3 · Comprobación y cierre

- [x] PM16 — C56/C58: probar Partida nueva hasta entrada N1, volver al acceso, abrir/cerrar el mapa y repetir la entrada. Revisar compilación y consola. Evidencia de escenas reales en Play Mode.
- [x] PM17 — C15/C57: comprobar los cuatro casos de apertura: ninguna condición, solo investigar, solo cuota y ambas. Solo ambas abren el nodo siguiente; apertura y anuncio una vez. Las pruebas controladas quedan aisladas del flujo de producción.
- [x] PM18 — C53/C55/C57: guardar/cargar en dormitorio después del timbre, en jardín antes del capitán, en acceso tras el encargo y con un nivel abierto. No repetir eventos ni perder progreso. Carga fallida no destruye el último guardado.
- [x] PM19 — C05/C59: verificar cámara/controles, personaje 2D, lecturas del HUD/mapa y ausencia de salidas ilegales. La cámara de miniatura del mapa no altera la de exploración.
- [x] PM20 — C60: cerrar Estado_Trabajo.md con tareas reales, archivos, capturas, pruebas y pendientes. Documentar un plan por sección para el siguiente pase de N1, sin comenzar a remodelar seis niveles a la vez.

## Evidencia por ID

Para cada tarea: estado; escenas y archivos reales; pasos ejecutados; resultado observado; captura/log o limitación; siguiente tarea. Si no se pudo abrir Unity, escribir «Implementado sin probar» y explicar el bloqueo. No marcar PM08, PM16 o la entrega como verificados con solo archivos de código.

## Criterio de organización

Reutilizar las carpetas del proyecto. Si hace falta un área temporal, usar una sola carpeta de pruebas identificada y que el arranque normal no dependa de ella. Evitar escenas nuevas de ensayo por cada sistema, controladores duplicados y botones provisionales de desbloqueo en producción. Al final, la funcionalidad se utiliza desde la partida normal.




