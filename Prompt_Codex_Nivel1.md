# Encargo para Codex y Claude Perfeccionar el nivel uno

Trabaja en el proyecto Unity existente de **Afterglow: Blinding Hope**. La tarea es perfeccionar N1 como tutorial breve, variado y jugable, modelando su entorno en Blender e integrándolo en la partida real. Actualiza también toda la documentación afectada por el canon oficial de los seis niveles. No construyas N2–N6 en este encargo ni reinicies el prólogo o selector que ya funcionen.

## 1 Usar el contexto y la skill realmente instalados

Lee AGENTS.md aplicable y la skill **level-design** instalada en tu entorno. Localiza y lee su SKILL.md efectivo y los recursos pertinentes; no afirmes que la usaste por conocer su nombre. Aplica sus métodos de diseño dentro de las decisiones del diseñador. Si falta en esa sesión, informa del hecho y continúa con el método y las herramientas disponibles sin inventar capacidades.

Localiza el proyecto por Assets, Packages y ProjectSettings y la documentación que ya utilizas. Puede estar dentro de Afterglow_Entrega_v0_3; no crees otra raíz operativa paralela. Lee el GDD actual completo, diseño de M1, guion, checklist maestro, Estado_Trabajo real y los índices. Integra la edición 0.5 leyendo INDICE_DOCUMENTACION.md, Mundo_1_Canon.md, Metodo_Diseno_Niveles.md, Protocolo_Documentacion.md y Checklist_Nivel_1.md. Conserva el avance real y cualquier contenido reciente compatible.

Inspecciona N1, cámara, controlador, salto, puertas, escenas de arranque, selector, inventario, misiones, diálogo, progreso y guardado reales. Identifica qué se reutiliza. Verifica acceso al editor y Blender; una skill de instrucciones no garantiza que puedas ejecutar la aplicación. Reutiliza las herramientas instaladas y no conviertas este encargo en otra búsqueda de plugins. Si falta una dependencia indispensable, informa del bloqueo concreto y avanza con tareas independientes. No hagas compras ni cambies Unity/URP.

## 2 Canon oficial que debes sincronizar

Estos son los seis niveles del mundo uno; su identidad ya está decidida:

1. **N1 Tutorial:** corto y conciso; movimiento lateral y en profundidad, plataformas, subidas, enemigo, pico, roca, mecanismo ambiental básico y circuito de ofrendas/investigación/retorno. Enseña acciones comunes y después las combina brevemente. No contiene versiones completas de los otros cinco niveles.
2. **N2 Exploración amplia:** misma base que N1, mucho más amplia y diversa. Plataformas, enemigos, ramales y puzles combinados; no reducirlo a una sucesión idéntica de salas de carros.
3. **N3 Laberinto:** cámaras de acertijo con cuatro puertas, una de regreso. Las pistas indican una continuación correcta. Un error ordinario devuelve a la misma cámara lógica conservando estado. Entre acertijos hay cámaras diferentes que confirman avance. Algunos ramales pueden contener tesoro, minero perdido o una sala de enemigos y tienen retorno al origen.
4. **N4 Oscuridad:** exploración de plataformas, subidas y puzles con iluminación baja. Buscar medios del entorno para ver y orientarse, manteniendo legibles suelo, bordes y avisos esenciales.
5. **N5 Vagoneta:** minijuego de montar en un carro y superar obstáculos/huecos para evitar caer, inspirado en principios de los recorridos de mine cart aportados. Aproximación y andén son breves; la conducción es su identidad.
6. **N6 Jefe:** corredor narrativo, rescate y taladro, con las reglas de batalla y cierre del GDD.

El prólogo y el pueblo son espacios adicionales. Mantén el mapa de acceso, las puertas internas y el desbloqueo por investigación más cuota: 3, 5, 7, 9 y 12. N4 es oscuridad y N5 vagoneta en todos los documentos y datos. Q09/Q10 pertenecen a oscuridad; Q07/Q08 a vagoneta. Si hay contenido/guardados con asignación anterior, usa una migración semántica documentada; no intercambies números a ciegas ni borres progreso.

## 3 Entender las referencias sin copiarlas

Analiza los once mapas en **Reference/General/Imagenes/MapasNiveles**, que el diseñador ya colocó bajo Afterglow_blindingHope/Afterglow_Entrega_v0_3. Si la documentación activa está en otra ruta, resuelve su ubicación real sin duplicar la colección. Inspecciona imágenes y recorta regiones a resolución original de los atlas grandes, especialmente Joke's End y Woohoo Hooniversity. No uses solo una vista en miniatura de todo el atlas.

Estudia dos escalas: estructura del nivel y composición interna de las cámaras. Observa forma/tamaño del suelo, vacíos, plataformas, profundidad, desniveles, relación de puertas con objetivos, ramales, visibilidad y cambios de función. Distingue mapas de conexiones de atlas detallados. Registra archivo y región consultada, observación verificable, principio útil, adaptación propia y limitaciones; no inventes el funcionamiento de un puzle que una imagen no demuestra.

**Las tres ilustraciones generadas anteriormente son referencias visuales, no plantillas.** No las repliques como las únicas cámaras del nivel ni construyas versiones de ellas con otras puertas o materiales. Tampoco copies una sala de Mario y Luigi sustituyendo sus gráficos por rocas. Extrae principios y diseña situaciones de Afterglow a partir de sus acciones, objetivos y contexto minero.

## 4 Diseñar antes de detallar la geometría

El objetivo no es añadir más cámaras, más assets ni más obstáculos por cantidad. La cantidad de cámaras es variable: si son pocas, deben concentrar espacios amplios con varias situaciones; si son muchas, pueden ser recorridos más breves, pero con intención y retos. Mezcla tamaños si mejora el ritmo. No mantengas obligatoriamente cuatro cámaras porque un documento anterior enumeraba S01–S04.

Primero mide salto, velocidad, tamaño y profundidad visible del controlador real con la cámara aprobada. Después presenta dos alternativas breves de estructura de N1, distintas en recorrido y distribución de retos. Elige la más clara para tutorial, justifica brevemente y continúa sin pedirme aprobación por sala.

Mantén un Plan_Nivel1.md y un plano editable que reúnan ruta principal, ramales, puertas, apariciones seguras, retos, mecanismos, fuentes, eventos y final. Reutiliza documentos equivalentes que ya existan. El número de cámaras debe resultar de ese recorrido. Define por cámara: función, forma/dimensiones aproximadas, alturas, acción predominante, decisión, entrada/salida, condición, recompensa y diferencia respecto a las vecinas.

Justifica la forma desde lo que hace el jugador: qué reconoce al entrar, qué quiere alcanzar, qué condiciona el camino y qué acción cambia la situación. Una sala de combate necesita espacio para leer ataques; una de plataformas organiza aterrizajes; una de manipulación relaciona un objeto con su efecto. No pongas el mismo suelo rectangular con una caja, un enemigo y una puerta en todas.

Comprueba la diversidad ocultando decoración: deben distinguirse forma transitable, distribución de alturas, secuencia de acciones, decisiones o función. Girar una sala, cambiar materiales o mover el portal no basta. Reutilizar piezas y repetir con propósito de aprendizaje es válido si se desarrolla la situación y no se convierte en monotonía.

## 5 Pequeñas mecánicas ambientales autorizadas

Puedes añadir pequeñas mecánicas que mejoren el diseño: palancas, puentes con contrapesos, placas/carros o plataformas móviles sencillas. No son una lista que debas implementar completa. Para N1 elige una interacción breve con causa/efecto visible y una combinación final suave. Reserva aplicaciones extensas para niveles posteriores.

Toda incorporación define activación, efecto espacial, retorno/reinicio, seguridad, persistencia, muerte y carga. Mantén las acciones básicas del personaje y los controles actuales; ninguna ruta obligatoria necesita dash, doble salto, gancho libre, escalada, ataque aéreo, arma nueva ni mejora comprada. No cambies historia, cuotas, sistema de combate o progresión para justificar un puzle.

## 6 Blender para construir y Unity para comprobar

Modela las cámaras y piezas en Blender, conservando .blend editables y scripts/parametrización si automatizas. Organiza por cámara y función. Separa terreno y decoración de puertas, rocas rompibles, carros y partes móviles. No unas toda una cámara en una malla que impida integrar sus interacciones.

Primero monta formas sencillas en Blender, exporta por piezas y verifica escala/orientación/pivotes en Unity. Prueba el recorrido con el personaje y cámara existentes antes de detallar. Tras corregirlo, realiza el pase de modelado, materiales, UV donde sea necesario y ambientación. Conserva fuentes fuera de Assets y exporta recursos a la organización real de arte. No importes un render como sustituto del escenario 3D.

Unity integra colliders, prefabs, puertas, luces, NPC/enemigos 2D, mecanismos, estados y guardado. No asumas que materiales o lógica de Blender se transfieren automáticamente. Si no tienes acceso real a Blender, explica el bloqueo; no anuncies modelado en Blender a partir de un script no ejecutado.

Preserva la cámara de exploración aprobada, seguimiento y controles. Las imágenes elevadas explican distribución, no autorizan reemplazarla. Los escenarios deben adaptarse a ella y mantener legibles profundidad, aterrizajes y señales.

## 7 Integración de N1 y criterio de terminado

Conserva N1A–N1D, Q01/Q02, sus recompensas, la primera entrega del pico antes de agresión, la entrega única del teléfono y el altar compartido del acceso. Adapta N1E y las pistas al mecanismo real. Redistribuye anclas si cambian las cámaras, sin duplicar diálogos, fuentes o recompensas. N1 mantiene al menos cuatro ofrendas legítimas garantizadas y cuota 3. No exige comprar al vendedor ni completar todas las misiones.

El jugador debe entrar desde el selector normal, recorrer N1, aprender, investigar el final, volver y abrir N2 por sus condiciones reales. No construyas el recorrido de N2 en este encargo. No uses un botón de completar nivel o una escena desconectada como sustituto de esa integración.

Sigue LD00–LD19 de Checklist_Nivel_1.md y enlaza con C00–C60, sin reiniciar estados reales ni dispersarte en otros sistemas. Una sola tarea de implementación en curso. Comprueba en Play Mode ruta, retorno, puertas, mecanismo, enemigo, caída/muerte, teléfono y guardado/carga. Corrige puntos de aparición y geometría antes de declarar completado el nivel.

Al terminar entrega fuentes .blend, exportaciones y prefabs utilizados, plano final, capturas de juego, resultados de consola/compilación, pruebas y tareas pendientes. Distingue propuesto, modelado, importado y verificado. Un archivo de código, una imagen o un render no demuestra una función jugable.

## 8 Mantener la documentación al día en cada corrección

Esta obligación es parte del trabajo: aplica Protocolo_Documentacion.md después de cada corrección del diseñador y al finalizar. Actualiza canon, GDD, diseño, guion/pistas, checklist, mapa/nodos, datos, índices y prompts afectados. Conserva una única raíz activa para que Claude u otro agente pueda continuar sin leer este chat.

Integra esa regla en el AGENTS.md del proyecto, preservando sus otras instrucciones. Guarda el historial separado y redacta la documentación de producción como una versión vigente completa. No acumules contradicciones o notas que obliguen al siguiente agente a reconstruir qué corrección ganó. Regenera la copia Word si la mantienes; si hay un bloqueo real, señala cuál es la fuente vigente y qué copia quedó desactualizada.

No marques una implementación verificada por cambiar texto. Conserva pruebas no afectadas y revalida las invalidadas. Actualiza Estado_Trabajo.md con IDs, archivos, escenas, pruebas observadas, bloqueos y siguiente tarea. Si la sesión termina, deja continuidad concreta. No hagas push ni publiques sin instrucción explícita.

## Producción vigente desde el 7 de noviembre de 2026

Leer `Docs/Implementacion/Encargo_Claude_Mundo_1.md`. El encargo de N1 conserva su alcance histórico. El nuevo encargo a Claude abarca correcciones del prototipo y producción de N2–N6 hasta el cierre.
