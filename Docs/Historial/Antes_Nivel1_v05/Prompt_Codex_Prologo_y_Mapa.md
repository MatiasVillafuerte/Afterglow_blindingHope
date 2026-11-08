# Encargo para Codex · Prólogo y selección de niveles

Trabaja en el proyecto Unity abierto de **Afterglow: Blinding Hope**. Debes entregar funciones integradas en la partida real y seguir el checklist. El objetivo actual es terminar el prólogo y el mapa de selección del mundo uno. Las referencias de minas explican cómo se diseñarán secciones variadas posteriormente; no implementes seis niveles a la vez ni conviertas la sesión en pruebas desconectadas.

## 1. Leer y diagnosticar antes de editar

Lee AGENTS.md aplicable, README, GDD actual completo, `Docs/Diseno/Mundo_1_Diseno.md`, `Docs/Guion/Mundo_1_Guion.md`, `Docs/Implementacion/Checklist_Mundo_1.md`, `Estado_Trabajo.md` y el plan de herramientas existente. Después lee íntegros:

- `Docs/Diseno/Prologo_Mapa_y_Escenarios_v0_4.md`.
- `Docs/Implementacion/Checklist_Prologo_y_Mapa_v0_4.md`.
- `Docs/Referencias/Guia_Escenarios_y_Videos_v0_4.md`.
- `Reference/Mundo 1/INDICE_PROLOGO_MAPA_v0_4.md` y los índices de General/CharacterSource.

Esta especificación define el prólogo, el selector de acceso y la composición de secciones para este encargo. Integra sus reglas en los documentos locales, preservando IDs e historial. Para el resto aplica el GDD. El selector se utiliza en el acceso seguro durante el mundo; el cierre después de vencer al jefe sigue siendo viaje y Continuará. Las cuotas y el teléfono conservan sus reglas.

Inspecciona Unity/URP, paquetes, escenas, arranque/build, controlador, cámara, sprites/Animator, entrada, diálogos, transiciones y guardado reales. No supongas que una función está implementada por existir en el documento o en una escena de ensayo. Antes de modificar archivos, presenta un resumen breve: qué existe, qué reutilizarás y los hitos de esta entrega. Continúa con el trabajo autorizado sin pedirme otra aprobación para pasos rutinarios.

## 2. Herramientas y referencias

Verifica las herramientas Unity ya instaladas y la conexión real al editor. Si hay Blender o ProBuilder útil, úsalo para geometría editable; las instrucciones de una skill no sustituyen el acceso a la aplicación. Reutiliza herramientas operativas. Solo instala opciones gratuitas, compatibles, de fuente verificable y necesarias para este trabajo. Registra licencia y versión; no hagas compras, suscripciones ni actualizaciones del Editor/URP. Si una dependencia exige una acción manual, indica el paso exacto y avanza con tareas independientes. Una dependencia opcional no retrasa el prólogo.

Inspecciona realmente los conceptos, originales, diagrama de secciones y mapa funcional. Investiga ejemplos visuales de **Paper Mario N64 y The Thousand-Year Door de GameCube**, especialmente el encadenado de habitaciones, entradas al fondo, plataformas altas, cruces y puertas. Son inspiración espacial; no copies mapas, materiales, personajes, combate por turnos ni transformaciones de papel.

Si hay gameplays completos, usa sus capítulos o la tabla de timestamps. Con herramientas locales de video puedes extraer fotogramas de fragmentos seleccionados y examinarlos, aunque el video dure más de diez horas. No recorras todas las horas a fotogramas ni afirmes haber visto el video completo. Si no puedes abrirlo, registra la limitación y utiliza imágenes; no bloquees el trabajo por una referencia opcional. Documenta archivo/URL, tiempo observado, idea útil y adaptación propia en `Docs/Referencias/Analisis/`. Prioriza fragmentos de N64/GameCube para variedad espacial y Super Paper Mario 5-3 para ambientación minera.

## 3. Orden obligatorio de ejecución

Sigue PM00–PM20 del subchecklist y enlaza el avance con C00–C60 del checklist maestro sin renumerar ni desmarcar tareas ya verificadas. Mantén una sola tarea de implementación en curso. Puedes implementar dependencias mínimas que falten; no expandas este encargo al árbol, al jefe o a otros mundos.

**Primero: prólogo completo.** Conecta Partida nueva con P01: carta de los padres, habitación, timbre único y control. Construye el dormitorio con decoración inspirada en Villa Albina y una puerta que lleve directamente al jardín. Construye casa grande al fondo, recinto rectangular vallado, fuente central rodeable, senderos y vegetación. El capitán queda fuera del portón y puede hablarse desde dentro con E. Usa P02; al terminar, transiciona a M01_ACCESO. Reutiliza el personaje 2D y sus animaciones. Todavía no recibe pico ni teléfono. No añadas interiores completos, enemigos o puzles a la casa.

Termina materiales, iluminación y composición suficientes para reconocer los conceptos. La caja gris es una fase interna de montaje; el criterio de esta entrega incluye apariencia coherente. Comprueba el recorrido desde el inicio normal y toma capturas jugables antes de pasar al selector.

**Después: mapa funcional.** En M01_ACCESO, un panel de rutas abre una vista de selección. Presenta terreno/miniaturas, camino y seis nodos identificados; no una lista de seis botones desconectados. Flechas cambian foco, E confirma y Esc vuelve al mismo punto. Solo N1 está abierto al principio. Cada nodo siguiente exige investigación y aporte del anterior: 3, 5, 7, 9 y 12. El mapa consulta el estado del altar, no acepta ni gasta ofrendas.

El teléfono regresa al acceso, donde se puede aportar al altar o abrir el mapa. No abrirlo desde pausa dentro del nivel. La selección entra por la entrada real del nivel; nunca elige sección, final o jefe directamente. Las puertas internas mantienen el recorrido físico y el retorno. Reutiliza el acceso/altar de N01S01 sin duplicarlos.

Conecta las escenas que existan de verdad. Si N2–N6 aún no están construidas, deja la falta claramente documentada y conserva su bloqueo inicial; no crees un pasillo vacío para declararlas completas ni cambies los requisitos narrativos para ocultar una ausencia. Implementa y prueba la lógica de nodos mediante datos/fixtures de prueba aislados cuando sea necesario, sin dar desbloqueos gratuitos en el flujo final.

**Finalmente: comprobar, guardar y registrar.** Prueba Partida nueva → carta → dormitorio → jardín → capitán → acceso → mapa → N1. Comprueba retorno, foco, doble confirmación, las cuatro combinaciones de investigación/cuota y persistencia. Cargar una partida en una galería conserva su sección. No se guardan cambios automáticamente al viajar o cerrar el mapa. No se añade selector posterior a la victoria del mundo ni repetición del jefe.

## 4. Restricciones de diseño

- Conserva la cámara de exploración aprobada, su seguimiento y los controles. Las vistas de concepto no son instrucciones para reemplazarla. La vista de mapa puede tener una cámara propia.
- Entornos en 3D; protagonista, enemigos y NPC en 2D. Importa solo las fuentes/animaciones necesarias, con sus dependencias y archivos .meta. No conviertas la física del mundo a 2D ni reemplaces ajustes del proyecto por los de otro proyecto.
- En niveles posteriores, X es lateral, Z profundidad y Y altura. Alterna secciones con profundidad, terrazas, cruces, rocas rompibles y puzles, conservando objetivos e IDs. No basta con invertir un pasillo o redistribuir cubos. N6 conserva su corredor narrativo.
- Ninguna ruta obligatoria exige habilidades comprables ni una mecánica aún inexistente. Mide alturas y distancias con el controlador real; el arte conceptual no define colisiones.
- No crees sistemas paralelos de inventario, diálogo, guardado o progreso. Integra los que funcionen; amplíalos si hace falta.
- Conserva cambios ajenos y el historial de tareas. No hagas push, publiques ni envíes mensajes externos.

## 5. Evidencia y forma de trabajar

Después de cada tarea registra ID, estado, archivos/escenas, prueba y resultado en Estado_Trabajo.md. Solo marca [x] lo comprobado. No confundas código escrito con comportamiento visto en Unity. Usa compilación, consola y Play Mode disponibles; si el editor está bloqueado, deja «Implementado sin probar» y explica el bloqueo concreto.

Las pruebas deben respaldar una función integrada. Evita multiplicar escenas temporales o scripts de demo que no utiliza la partida. Al finalizar indica la escena de inicio real y cómo recorrer la entrega. Incluye capturas del dormitorio, jardín/portón, mapa y entrada N1; casos de desbloqueo y guardado; errores pendientes y siguiente ID concreto.

Continúa hasta cerrar prólogo y selector, salvo bloqueo real o límite de sesión. Si debes detenerte, conserva un estado coherente con la siguiente tarea documentada. No me pidas decisiones ya resueltas en estos archivos y no anuncies acabado lo que solo esté planteado.
