# Estado de trabajo — 2026-11-06

Última sesión: ver «2026-11-08 · Segunda sesión Claude» al final.

Raíz activa: este proyecto Unity; documentación vigente en Docs e INDICE_DOCUMENTACION.md. N2–N6 pendientes de producción. Avance previo C00–C60 y PM00–PM20 conservado.

## Implementado y comprobado

- Unity 6000.5.5f1, URP 17.5.0, Pipeline 0.9.1-exp.1; CLI 1.0.0-beta.17. Editor reabierto, PID 5740, ready.
- Canon 0.5 integrado: N4 oscuridad/Q09–Q10, N5 vagoneta/Q07–Q08. Guardado esquema 5: migración semántica y recolocación segura de N1; 26 comprobaciones sin fallos en Nivel1/Migracion_Pruebas.md.
- N1_Blockout.blend y N1_Final.blend ejecutados en Blender 5.2.3 LTS, bajo ArtSource/Blender/Modelado_Afterglow/Nivel1. Cuatro FBX y prefabs en Assets/Afterglow/Environment/Modelado/Nivel1. Triángulos: 13314, 9272, 12984, 8590. Roundtrip.json y Triangulos_Por_Objeto.json registran la auditoría.
- Recorrido real desde menú/prólogo/mapa, cuatro cámaras, pico, enemigos, roca, puente, investigación, teléfono, altar cuota 3 y N2 disponible: Nivel1/Pruebas_Final.md, cero fallos. Q01 y Q02 cobradas sin duplicación. Guardado personal intacto.
- Seguridad: caída, muerte, retorno por puertas, carga abierta/cerrada y migración de posición probadas. Informe Nivel1/Pruebas_Seguridad.md. Última revalidación del aviso visual finalizada sin fallos.
- Pivotes de puente y palanca corregidos desempaquetando la instancia FBX antes de reorganizarla. Mallas móviles comprobadas físicamente. Selector corregido para tolerar destrucción de su UI al descargar escena.
- Inspeccion_Final.md: materiales URP, escala, scripts y prefabs comprobados. Consola_Final.json conserva incidencias históricas corregidas; última lectura anterior al ajuste del aviso: cero errores y advertencias actuales.
- GDD Word v0.5 regenerado; 17 páginas inspeccionadas tras corregir cabecera huérfana. PDF y páginas en Nivel1/Word_QA. Plan y plano editable en Docs/Diseno.

## Cierre pendiente

LD15: aviso delante del farol, legible en captura y oculto tras activar; revalidación sin fallos. Consola actual: cero errores y advertencias. Play detenido. Única tarea En curso: LD19, cierre documental y sincronización final de las casillas restantes con sus evidencias. No interpretar casillas pendientes como pérdida de los resultados anteriores.

QA Blender: sin errores por objeto. Advertencias aceptadas: UV superpuestas por texturas repetibles (no atlas de horneado), bases bajo cota cero intencionales y n-gons planos estáticos triangulados al exportar. Ocho hojas random/wire inspeccionadas. Las pruebas automatizadas no sustituyen una evaluación de comprensión con jugadores nuevos.


## 2026-11-07 · Nuevo encargo documental

Decisiones vigentes en Encargo_Claude_Mundo_1.md y GDD. Las pruebas anteriores de recorrido se conservan. Ataque manual con X, perímetro de N1 y presentación quedan pendientes de revalidación por el reporte del diseñador. Próximo paso técnico: reproducir X tras obtener el pico en Game. Los cinco enemigos tienen arte fuente identificado. Su integración, N5 y PerforAudora están pendientes.

## 7 de noviembre · ampliación de diseño

Se añaden al encargo vigente los indicadores de daño y la revisión artística de modelos. Estado: especificado, pendiente de implementación y prueba en Unity. Ver Encargo_Claude_Mundo_1 y Checklist_Mundo_1.

Ampliación de presentación, pendiente de implementación en Game. P01.04 incorpora el aviso del timbre. En N1C y N1B se especifica la celebración de teléfono y pico. Las recogidas comunes usan avisos sin pausa. Primera intervención técnica: reproducir y corregir X desde la entrada normal, documentar el resultado y avanzar por el checklist maestro hasta el cierre de Mundo 1.

## 2026-11-08 · Sesión Claude · ataque X, cifras de daño, timbre y adquisiciones

Entorno observado: Unity 6000.5.5f1, URP 17.5.0, Unity CLI 1.0.0-beta.17 y Pipeline 0.9.1-exp.1. El editor estaba cerrado al conectarse y se abrió con M01_MENU sin cambios sin guardar. Las skills del diseñador están en `C:\Users\matia\.agents\skills` (level-design, blender-* y shader-graph-best-practices). Caveman no figura entre ellas ni se instaló.

### Método de prueba

Teclado real: pulsaciones del sistema operativo (keybd_event) enviadas a la ventana de Unity, con el foco en Game y la configuración de entrada normal (PointersAndKeyboardsRespectGameViewFocus). No se usaron `testAttack` ni `testControl`. Las lecturas de estado se hicieron con `unity command script eval`. Ayudas de prueba declaradas:

- En los combates de precisión se usó `Time.timeScale` 0,25 y 0,1, porque la latencia del agente entre pasos permitía que el enemigo atacara.
- La vida se restauró una vez antes de ir a la roca.
- N01S03 se alcanzó una vez con `M01Expedition.Travel` (la misma llamada que hace la puerta) tras tres intentos fallidos en las terrazas de N01S02. Ver hallazgos.
- Se colocaron un curativo y dos ofrendas en la ruta para probar recogidas seguidas. Los curativos solo aparecen al 10 %.

### Implementado y verificado en Game (8 de noviembre)

| Prueba | Resultado observado | Evidencia (`Docs/Implementacion/Nivel1/Evidencias_2026_11_08/`) |
| --- | --- | --- |
| Diagnóstico inicial desde el menú normal | X llegaba al juego (`Attack`), pero el Resisworm esperaba a 1,50 m, fuera del alcance de 1,2 m. Se perdió vida hasta morir y reaparecer en N01S01 | Docs/Historial/Decisiones_2026_11_08_Ataque_y_Adquisiciones.md |
| Primer X tras recibir el pico | Golpe registrado (StrikeCount 1) | 07_Primer_X.png |
| X lateral contra un Resisworm a 1,04 m | Impacto, vida 2→1, cifra «1» sobre el enemigo | 08a_Golpe_Volumenes_F9.png |
| Segundo impacto y derrota | Vida 1→0, cifra visible al caer. La mordida al jugador muestra su propia cifra | 08b_Cifras_Ambos_Sentidos.png |
| Golpe en profundidad (−Z, desfase lateral de 0,52 m) | Impacto | 08c_Golpe_Profundidad.png |
| Golpe fuera de alcance (1,58 m) | Sin objetivo ni cifra | Registro de estado |
| Tres Resisworm de N01S03 derrotados con X real | Correcto. El enemigo también daña al jugador | Registro de estado |
| Roca | Se rompe con X real y suelta fragmentos. Se pasa a N01S04 | 08d_Roca_Rota.png |
| Timbre | «Alguien tocó la puerta» aparece una sola vez, centrado, y desaparece a los 3,2 s. Queda registrado P01.BELL | 02_Timbre_Aviso.png |
| Teléfono (N1C) | Presentación completa (fase Hold observada, played=1). Vuelven la cámara y el control | 03_Telefono_Antes.png y 05_Telefono_Despues.png. La captura intermedia llegó tarde y se descartó |
| Pico (N1B) | Pausa, manos en V, acercamiento, pico brillante y «Conseguiste Pico». Vuelven el encuadre y timeScale 1 | 06_Pico_Antes.png, 06b_Pico_Durante_Zoom.png y 06c_Pico_Despues.png |
| Recogidas seguidas caminando | «Conseguiste Curativo» y «Conseguiste Hoja de coca ×2» sin pausa. Vida 7→9 | 09_Recogidas_Consecutivas.png |
| Guardar (Esc → Guardar) y Continuar desde M01_MENU | Se carga N01S03 con pico, teléfono, 4 ofrendas y EXP 3. Sin presentaciones repetidas (played=0). El Resisworm derrotado sigue derrotado. X funciona tras la carga | 10_Guardado.png y 11_Menu_Continuar.png |
| Compilación y consola | `unity recompile`: 0 errores y 0 advertencias. Consola en Play: 0 errores y 0 advertencias | — |

### Archivos modificados o creados

- Nuevos: `Assets/Afterglow/Runtime/AfterglowHitbox.cs`, `AfterglowHitboxView.cs` (F9), `AfterglowDamagePopup.cs`, `Runtime/M01/M01Toast.cs` y `Runtime/M01/M01KeyItemCelebration.cs`.
- Modificados: `AfterglowPlayer.cs` (golpe con volumen, cifra al recibir daño, F9 y contadores de diagnóstico), `AfterglowEnemy.cs` (mordida con volumen, espera a 1,05 m, destello, cifra y `TakeDamage` devuelve bool), `M01BreakableRock.cs` (`Break` y fragmentos), `M01Progress.cs` (evento `Acquired`, `GrantKeyItem` y `AddDroppedOffering`), `M01Loot.cs`, `M01QuestObject.cs`, `M01StoryPoint.cs` y `M01Prologue.cs`.
- Datos: `Assets/Afterglow/Data/M01/Guion_M01.json` (nueva línea P01.04) y `Assets/Afterglow/Prefabs/Enemy.prefab` (maxHealth 2, reach 1.05).
- Herramientas: `Tools/PM/N1PlayChecks.cs`, `N1SafetyChecks.cs` y `SpatialPlayChecks.cs`. `Speak()` ahora espera a que termine la presentación de objetos clave.
- Escenas: ninguna escena se guardó modificada. Los Resisworm de N01S03 heredan del prefab.
- Documentos: este archivo, Checklist_Mundo_1.md, Checklist_Nivel_1.md, Mundo_1_Diseno.md y Docs/Historial/Decisiones_2026_11_08_Ataque_y_Adquisiciones.md.

### Hallazgos y pendientes

- **N01S02, perímetro.** Hay caídas al vacío en recorridos diagonales y tras el salto a la segunda terraza (coordenadas en el historial). Cinco caídas reinician el nivel en N01S01. Corresponde a la prioridad 2 del encargo.
- **Pruebas automáticas.** N1PlayChecks, N1SafetyChecks y SpatialPlayChecks no se volvieron a ejecutar tras el cambio de combate y del modal. Quedan pendientes de revalidación.
- **Cifras de daño.** No se observaron dos casos: el golpe absorbido por invulnerabilidad (el código no genera cifra) y el daño mayor que la vida restante (todo el daño actual es 1).
- **«Curativo».** Es un nombre provisional, igual que el icono del teléfono.
- **LD02, LD12, LD18 y LD19.** Siguen abiertas.

### Siguiente tarea concreta

Prioridad 2 del encargo en N01S02: marcar en el plano el perímetro protegido y las caídas intencionales, añadir barreras invisibles en los bordes exteriores transitables y volver a probar con teclado real el recorrido diagonal y los saltos de las terrazas. Después, volver a ejecutar N1PlayChecks y N1SafetyChecks.

## 2026-11-08 · Segunda sesión Claude · N2–N6 jugables y recorridos sin fallos

Encargo: terminar Mundo 1 desde Nueva partida hasta la victoria contra PerforAudora y «Continuará». Unity 6000.5.5f1, URP 17.5.0, cámara de exploración y esquema de guardado sin cambios. Commits locales `1801636`, `136ad23`, `27795d7` y el de cierre de esta sección. Sin push.

### Qué se puede jugar desde el menú

Nueva partida → prólogo → N1 → selector → N2 → … → N6 → PerforAudora → pieza dorada → Platicio en el equipo → vagoneta → «Continuará» → menú. Cada nivel se entra desde su nodo del selector y se abre el siguiente por sus condiciones reales: investigación más cuota en el altar compartido (3, 5, 7, 9 y 12).

### Pruebas ejecutadas (teclado simulado por el Input System, la misma ruta que el teclado real)

| Prueba | Alcance | Resultado | Informe |
| --- | --- | --- | --- |
| N1PlayChecks | Nueva partida → N1 completo → N2 abierto | 92 PASS · 0 fallos | Nivel1/Pruebas.md |
| N1SafetyChecks | Caída, muerte, puertas, carga y migración | 35 PASS · 0 fallos | Nivel1/Pruebas_Seguridad.md |
| N2PlayChecks | Selector → Q03/Q04, carros, reja, Hangzzer, puente → investigación → altar 5 → N3 enfocado | 108 PASS · 0 fallos | Niveles/N2_Evidencias/ |
| N3PlayChecks | Puertas con error y regreso, Q05 escolta, Q06, Gasplode, Drilly → altar 7 → N4 | 110 PASS · 0 fallos | Niveles/N3_Evidencias/ |
| N4PlayChecks | Oscuridad, lámpara, Lighmet, Q09 carro de luz, Q10 → altar 9 → N5 | 80 PASS · 0 fallos | Niveles/N4_Evidencias/ |
| N5PlayChecks | Q07, Q08, rechazo, caída y reinicio, umbral 18 y premio único → altar 12 → N6 | 41 PASS · 0 fallos | Niveles/N5_Evidencias/ |
| N6PlayChecks | Corredor, rescate, jefe en tres fases, victoria, pieza, capitán, cambio de personaje, vagoneta, «Continuará», menú | 0 fallos (197,6 s) | Niveles/N6_Evidencias/ |
| Compilación | `unity recompile` tras cada cambio | 0 errores · 0 advertencias | — |

Las pruebas N2–N6 parten de un fixture declarado (niveles anteriores completados por sus propias pruebas). No se ha jugado aún el recorrido completo de una sola vez (C56).

### Correcciones de juego de esta sesión

- **Golpe del pico.** La dirección pulsada en el mismo fotograma que X orienta el golpe (antes salía hacia la orientación previa: 8 fallos seguidos en diagonal en N02S04). `AfterglowPlayer.cs`.
- **Selector.** La flecha elige el vecino mejor alineado: desde N4 no había forma de llegar a N5. El aviso de escena pendiente usa `HasScene`. `M01RouteMap.cs`.
- **Lighmet.** Aviso de 0,45 s con el sprite de lanzamiento; un golpe durante el aviso cancela el disparo. `M01Lighmet.cs`. El de N04S04 flotaba a 1,35 m (plano corregido).
- **Guion.** Faltaban N4D.01, N4M.01 y N4M.02: la investigación de N4 lanzaba una excepción y no se registraba. Redactadas en `Guion_M01.json` y `Mundo_1_Guion.md` (tono pendiente de revisión).
- **Cierre de N6.** La vagoneta estaba fuera del anillo de paredes de la arena y sin suelo; ahora está dentro (plano N6).
- **Escenarios.** Luz principal en N2–N6 y barandas del puente de N02S05 (`M01CounterweightBridge.rails`). N2–N6 reconstruidos desde sus planos.

### Ayudas de prueba (no forman parte del juego)

- Recorridos `Tools/PM/N?PlayChecks.cs` y `M01RouteRunner.cs`, copiados a `Assets/Afterglow/PruebasTemporales/` para ejecutarlos.
- Reloj de juego para los plazos y `Time.maximumDeltaTime = 1/30` mientras corre una prueba, restaurado al terminar. El editor sin foco baja de fotogramas y los plazos en tiempo real fallaban al azar.
- Los recorridos luchan contra un Resisworm que corta el paso, recogen botín cercano y usan un consumible con 4 de vida o menos.
- El fixture da un Núcleo por nivel completado, como compra prevista en la tienda del pueblo.
- `M01Expedition.Travel` y `testControl` solo se usaron en diagnósticos aislados (`Docs/Implementacion/Niveles/Diagnostico/`), no en los recorridos que cuentan.
- Nuevo `Tools/Documentacion/Validar_Lineas.py`: comprueba que todas las líneas de planos y código existan en el guion.

### Archivos

- **Runtime:** `AfterglowPlayer.cs`, `M01RouteMap.cs`, `M01Lighmet.cs` y `M01CounterweightBridge.cs`.
- **Editor:** `M01LevelBuilder.cs` (luz principal y barandas).
- **Datos:** `Guion_M01.json`, `N4_Plano.json` y `N6_Plano.json`.
- **Escenas:** N02S01–N06S02 reconstruidas.
- **Pruebas:** N1PlayChecks (reloj de juego), N2–N6PlayChecks y M01RouteRunner.
- **Documentos:** `Docs/Historial/Decisiones_2026_11_08_Niveles_N2_N6.md`, `Mundo_1_Guion.md`, `Checklist_Mundo_1.md` y este archivo.

### Sin verificar o pendiente de decisión

- C56: recorrido completo de una sola vez desde Nueva partida hasta «Continuará».
- C44: el altar de la arena (entrega de 5 ofrendas de arena) no se ejercitó; el jefe se venció sin mejoras del Tío.
- Insuficiencia de minerales en N5: el recorrido superó el umbral en el primer viaje completo.
- Guardado y carga en mitad de N2–N6, árbol de habilidades y regalos de Platicio en N6: no probados en esta sesión.
- Decisiones del diseñador:
  - exigencia de N4 sin consumibles (llega con 1 de vida);
  - umbral de 18 minerales;
  - tono de N4D.01, N4M.01 y N4M.02;
  - nombre «Curativo».

### Siguiente tarea concreta

Escribir un recorrido encadenado que juegue de una sola vez desde Nueva partida hasta «Continuará» sin fixture, reutilizando las rutas N1–N6. Registrar vida, dinero, ofrendas y tiempo por nivel. Después, probar el altar de la arena y guardar y cargar en cada nivel.
