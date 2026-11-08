# Protocolo de actualización documental

Toda corrección explícita del diseñador se incorpora a los documentos de trabajo durante el mismo encargo. Esta obligación acompaña a Codex, Claude y cualquier agente posterior.

## Una raíz vigente

Localizar la raíz real del proyecto Unity por Assets, Packages y ProjectSettings. Localizar por separado la documentación activa: puede estar en la raíz o dentro de Afterglow_Entrega_v0_3. No crear una segunda copia operativa de Docs y Reference solo por recibir un paquete nuevo. INDICE_DOCUMENTACION.md identifica la única raíz vigente, versión y encargo actual.

El paquete v0_5 es una instantánea actualizada para integrar. El agente debe compararlo con los archivos del proyecto y conservar contenido y avance reales más recientes que no entren en conflicto. No sobrescribir Estado_Trabajo.md ni casillas verificadas con las plantillas de esta entrega. Las versiones antiguas se archivan fuera de la lectura activa, conservando sus enlaces si hace falta.

## Secuencia ante una corrección

1. Registrar brevemente la decisión y la fecha en un historial separado. Distinguir decisión del diseñador, parámetro inicial y propuesta del agente.
2. Actualizar el canon y el texto vigente del GDD. El documento de producción explica la versión actual; no relata cómo era cada borrador anterior.
3. Propagar a tablas de niveles, diseño, guion y pistas, misiones, checklist, mapas/nodos, datos de progreso, fuentes de recursos, referencias y prompts activos afectados.
4. Buscar referencias literales y semánticas obsoletas, incluyendo contenido dentro de DOCX y JSON. Un reemplazo global de números no basta.
5. Revisar impacto en código, escenas, IDs, guardados y pruebas. Documentar migraciones antes de aplicar cambios a partidas existentes; no perder recursos o misiones.
6. Conservar verificaciones no afectadas. Una prueba invalidada vuelve a Pendiente de revalidación, con motivo. Una actualización de texto no significa implementación completa.
7. Actualizar el índice y Estado_Trabajo.md con la versión, archivos afectados, verificación y siguiente tarea. Regenerar Word desde la fuente vigente si se mantiene esa copia; si no es posible, marcarla como desactualizada en el índice y dejar el bloqueo explícito.

Integrar una regla breve con esta obligación en el AGENTS.md aplicable si no existe, preservando el resto de sus instrucciones. No modificar otros proyectos ni instrucciones globales del usuario.

## Migración de contenido al orden oficial 0.5

El contenido de oscuridad pertenece a M01N04 y el de vagoneta a M01N05. Q09/Q10 acompañan a la oscuridad; Q07/Q08 a la vagoneta. Los IDs de misión conservan su significado. Los prefijos de diálogo N4 y N5 corresponden al nivel actual. Las cuotas 9 y 12 corresponden a la posición actual N4 y N5, respectivamente.

Si el proyecto ya tiene escenas o guardados con la asignación anterior, elaborar una tabla de equivalencias por significado antes de cambiarlos. Mantener misiones completadas, recursos, daño, mejoras y recompensas ya cobradas. No intercambiar ciegamente dos enteros de progreso: el nuevo orden cambia requisitos y puede existir una partida en medio de una galería. Se necesita una política explícita de migración para esa situación. Si no puede restaurarse la posición exacta en una geometría remodelada, usar un punto seguro válido y conservar el progreso permanente, dejando constancia.

Las pruebas C34/C35 siguen significando vagoneta y C36 oscuridad; se actualiza el nivel al que apuntan y el orden de ejecución, sin reciclar su identidad. Para escenas de N1 que cambien de número o tamaño, mapear los eventos y fuentes anteriores a los nuevos IDs de cámara.

## Diferencia entre diseño y estado real

El GDD define qué debe existir. El plan define qué se construirá. El checklist y Estado_Trabajo registran qué existe y qué se comprobó. Ninguno se utiliza como sustituto de los otros. El siguiente agente debe poder retomar el trabajo con los archivos, sin necesitar memoria de esta conversación.
