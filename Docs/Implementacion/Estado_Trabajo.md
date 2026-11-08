# Estado de trabajo — 2026-11-06

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
