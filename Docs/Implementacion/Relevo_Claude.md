# Relevo de Afterglow para Claude · 7 de noviembre de 2026

Trabaja conmigo en **Afterglow: Blinding Hope**. Soy el diseñador. Este relevo resume lo comprobado hasta el 6 de noviembre. Si algo cambió después, usa el proyecto y sus evidencias para determinar el estado actual.

## Antes de tomar el control

En tu primera respuesta pregúntame, en un solo mensaje, qué tarea quiero priorizar, qué resultado espero y qué herramientas te autorizo a operar en esta sesión. Pregunta si hay escenas abiertas con cambios sin guardar o una partida personal que deba preservarse. Confirma si puedo intervenir en un inicio de sesión, una licencia o un diálogo que requiera atención personal. Espera mi respuesta antes de operar el equipo. Resuelve las dudas de diseño pendientes conmigo y consulta en los documentos lo que ya está decidido.

Después de mi respuesta, empieza con una inspección de lectura. La raíz operativa es:

`C:\Users\matia\Documents\Universidad\04_Cuarto Semestre\Game Development\Afterglow_blindingHope`

Confirma Assets, Packages y ProjectSettings. Comprueba por separado acceso a archivos, conexión con el editor de ese proyecto, escena activa, consola, capacidad de ejecutar Blender y skills efectivamente disponibles en tu entorno. El último entorno comprobado usó Unity 6000.5.5f1, URP 17.5.0, Unity CLI 1.0.0-beta.17, Pipeline 0.9.1-exp.1 y Blender 5.2.3 LTS. Conserva las versiones del motor y del renderizado. Usa pruebas con guardados aislados. Consulta conmigo antes de descartar trabajo abierto, publicar o hacer push.

## Qué juego recibes

Aventura para PC con personajes 2D en escenarios mineros 3D. La protagonista, una bolivianita, recibe una carta en casa y conoce en el patio al capitán que pide ayuda tras un derrumbe. Se explora lateralmente y en profundidad, se salta, se combate con pico y se interactúa con E. La cámara aprobada sigue al personaje y su altura. El patio es rectangular y permite recorrerlo en profundidad. El trabajo previo incluye habitación, casa y patio, selector, presentación y animaciones del personaje, ataque con pico, menús, inventario, diálogos y opciones, misiones, teléfono, altar y persistencia. Los informes distinguen la cobertura de cada sistema.

El flujo probado parte del menú, pasa por PRO_CASA y PRO_PATIO, y el capitán conduce a M01_MAPA. Seleccionar N1 inicia una expedición en N01S01. Hay cuatro cámaras conectadas por puertas físicas:

1. **N01S01, refugio.** El hijo entrega el teléfono. Aquí están el altar compartido y los encargos Q01 y Q02.
2. **N01S02, cantera.** Las terrazas enseñan desplazamiento lateral, profundidad y salto con una ofrenda fija en la ruta.
3. **N01S03, taller.** El jugador recibe el pico antes de la agresión, combate, consigue botín y rompe la roca del paso. Un ramal permite completar las bajas de Q01.
4. **N01S04, garganta.** La palanca baja el puente. El casco de Q02 y el minero que registra la investigación están en ramales distintos. El teléfono permite regresar al acceso. La investigación y tres ofrendas aportadas abren N2 en el selector. La ruta básica obtiene las tres sin comprar ni cobrar ambas misiones.

El mundo uno reserva N2 para exploración amplia, N3 para el laberinto, N4 para oscuridad (Q09/Q10), N5 para vagoneta (Q07/Q08) y N6 para el jefe. Las cuotas son 3, 5, 7, 9 y 12. En esta entrega N2–N6 tienen diseño y nodos previstos. El alcance jugable de niveles es N1. El guardado usa esquema 5 y migra por significado para conservar créditos, recursos y recompensas.

## Qué leer antes de codificar

Lee primero `AGENTS.md`, `INDICE_DOCUMENTACION.md`, `Docs/Implementacion/Estado_Trabajo.md` y `Docs/Implementacion/Protocolo_Documentacion.md`. Lee el encargo vigente `Prompt_Codex_Nivel1.md`. Continúa en este orden:

- **Canon e historia.** `Docs/Diseno/Mundo_1_Canon.md`, `Docs/GDD/Afterglow_GDD_v0_5.md`, `Docs/Diseno/Mundo_1_Diseno.md` y `Docs/Guion/Mundo_1_Guion.md`. El Word v0.5 es una copia de lectura del GDD.
- **Diseño y mapa.** En `Docs/Diseno/`, lee `Metodo_Diseno_Niveles.md`, `Prologo_Mapa_y_Escenarios_v0_5.md`, `Plan_Nivel1.md`, `Plano_Nivel1.svg` y `Mapa_Mundo1_Datos_Referencia.json`.
- **Avance real.** En `Docs/Implementacion/`, lee `Checklist_Nivel_1.md`, `Checklist_Mundo_1.md` y `Checklist_Prologo_y_Mapa_v0_4.md`. En su subcarpeta `Nivel1/`, consulta `Metricas.md`, `Pruebas_Final.md`, `Pruebas_Seguridad.md`, `Migracion_Canon.md`, `Migracion_Pruebas.md` e `Inspeccion_Final.md`. Allí están también las capturas de Game y la consola histórica.
- **Referencias y herramientas.** Lee `Docs/Referencias/Analisis/Mapas_Nivel1.md`, los índices de `Reference` y `Docs/Implementacion/Plan_Herramientas_y_Assets.md`. El material original de cámara, personajes, mapas y estilo visual está en `Afterglow_Entrega_v0_3/Reference/`. Las carpetas de entrega son fuentes de entrada. La documentación activa está en `Docs/`. Para trabajo de niveles comprueba y lee la skill efectiva `level-design`. Para arte comprueba las skills Blender de modelado, exportación y QA y la aplicación ejecutable.

## Archivos de implementación y arte que conviene localizar

- `Assets/Afterglow/Scenes/` contiene las escenas. `Runtime/AfterglowPlayer.cs` y `Runtime/AfterglowCamera.cs`, dentro de `Assets/Afterglow/`, contienen el controlador y la cámara. `Assets/Afterglow/Character/` y `Prefabs/` bajo ese mismo directorio contienen recursos del personaje y prefabs.
- `Assets/Afterglow/Runtime/M01/` reúne la lógica. Empieza por `M01RouteMap.cs`, `M01Expedition.cs`, `M01QuestNPC.cs`, `M01Save.cs`, `M01CanonMigration.cs`, `M01CounterweightBridge.cs` y `M01WorldHint.cs` según la tarea. El guion consumido en Unity está en `Assets/Afterglow/Data/M01/Guion_M01.json`.
- `ArtSource/Blender/Modelado_Afterglow/` conserva las fuentes editables. `Afterglow_Prologo_Nivel1.blend` pertenece al montaje anterior. En `Nivel1/` están `N1_Blockout.blend`, `N1_Final.blend`, `build_n1.py`, `check_roundtrip.py` y las auditorías. En `Assets/Afterglow/Environment/Modelado/Nivel1/` están los cuatro FBX, sus manifiestos de colisión y los prefabs de entorno integrados.
- `Tools/PM/` contiene `N1LayoutBuilder.cs` para reconstruir el montaje y las pruebas `N1PlayChecks.cs`, `N1SafetyChecks.cs`, `N1CanonChecks.cs` y `N1InspectFinal.cs`. El constructor escribe escenas. Para una inspección basta leerlas. En `Tools/Documentacion/` están `Validar_Canon.py`, `Generar_GDD.py` y `Plano_N1.py`.

## Punto exacto para retomar

El recorrido de partida nueva hasta desbloquear N2 pasó en Play Mode. También pasaron caída, muerte, retorno físico, teléfono, persistencia del puente, guardado/carga y migración. Se corrigieron los pivotes móviles, el cierre del selector y el aviso «Bajar puente [E]», que ahora se lee delante del farol y desaparece al activar. La última consulta de consola dio cero errores y cero advertencias. Son resultados registrados en el editor. La comprensión de un jugador nuevo sigue sin evaluación humana.

Al cerrar el hito de N1, retoma **LD19 como única tarea en curso**. Quedan abiertas LD02, LD12, LD18 y LD19. Para LD02 contrasta canon, misiones, cuotas, datos y migración. Para LD12 usa el tiempo y las observaciones de la ruta automatizada y consigna el alcance de esa medición. Para LD18 compara el plano y la geometría sin decoración con los eventos y anclas reales. Para LD19 sincroniza índice, plan, checklist maestro y estado. Hay frases provisionales en el plan y una instrucción final de repetir N1SafetyChecks en Estado_Trabajo que precede al resultado que ya consta como verificado. Corrige esas referencias usando las evidencias. Pregúntame si quiero cambiar la prioridad antes de empezar otra fase.

## Cómo devolverme el proyecto para seguir con Codex

Por cada corrección, registra fecha y decisión en `Docs/Historial/`. Actualiza en su lugar los documentos afectados de canon, GDD, diseño, guion, mapa, datos, índices y prompts. Si cambia el GDD, regenera su Word o documenta el bloqueo. Mantén una sola tarea de implementación en curso. Conserva las casillas verificadas que sigan siendo válidas. Cuando un cambio invalide una prueba, identifica cuál, ejecútala en el proyecto y registra el resultado observado.

Al cerrar cada sesión, actualiza `Docs/Implementacion/Estado_Trabajo.md` y los checklists correspondientes. Deja fecha, IDs, decisiones, archivos y escenas tocados, qué ocurrió en Play Mode, rutas a capturas e informes, errores pendientes y la próxima acción concreta. Distingue diseño, modelado en Blender, importación en Unity y verificación jugable. Entrégame un resumen corto con enlaces y las preguntas que aún necesiten mi decisión. Así Codex podrá continuar desde el repositorio.

## Producción vigente desde el 7 de noviembre de 2026

Leer `Docs/Implementacion/Encargo_Claude_Mundo_1.md`. El encargo de N1 conserva su alcance histórico. El nuevo encargo a Claude abarca correcciones del prototipo y producción de N2–N6 hasta el cierre.
