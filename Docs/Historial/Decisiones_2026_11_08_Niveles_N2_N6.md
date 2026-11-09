# Decisiones del 8 de noviembre de 2026 (segunda sesión) · niveles N2–N6

Sesión de Claude. Se distingue la decisión del diseñador, el parámetro inicial elegido por el agente y la propuesta.

## Decisiones del diseñador (mensaje del 8 de noviembre)

- El foco es crear niveles.
- El hito jugable es atravesar N1 sin salir del escenario, volver al selector, entrar en N2, investigar y abrir N3 por condiciones reales.
- Después hay que seguir con N3–N6 sin pedir aprobación por cada sala.
- Cada nivel parte de un plano editable y un montaje en Blender, con los `.blend` fuera de Assets.

## Método adoptado (agente)

- **Una fuente por nivel.** `Docs/Diseno/Niveles/N?_Plano.json` contiene superficies, paredes, puertas, huecos jugables, accesorios, personajes, enemigos, mecanismos, pulsaciones de ritmo y grafo de puertas.
- **Blender.** `ArtSource/Blender/Modelado_Afterglow/Niveles/build_level.py` (Blender 5.2) genera `N?_Montaje.blend` y exporta un FBX por sala, con las piezas interactivas separadas.
- **Unity.** `Assets/Afterglow/Editor/M01LevelBuilder.cs` monta la escena desde la plantilla N01S03 (jugador, cámara aprobada, HUD y diálogo), con colliders del plano, perímetro protegido, luces, puertas, personajes y enemigos.
- **Entradas seguras.** `Tools/Documentacion/Generar_Entradas.py` genera `M01LevelEntrances.cs`.

## Parámetros de prueba (abiertos en el GDD, ajustables)

| Parámetro | Valor |
| --- | --- |
| Intervalo de mordida del Resisworm | 1,6 s (antes 1,2): el jugador gana el intercambio si ataca primero |
| Refuerzos del Hangzzer | intervalo 6 s, 2 vivos como máximo, 2 por ciclo, 4 por vida del Hangzzer; sin botín |
| Gasplode | 1 de vida, aviso a 2,6 m, mecha de 0,9 s, explosión de 1,9 m y daño 2 |
| Drilly | 3 de vida, sigue 2,4 s, aviso de 0,6 s, fuera 1,6 s, golpe a 1,25 m y daño 1 |
| Lighmet | 4 de vida, disparo cada 2,4 s a 5 m/s, daño 1; espíritu y proyectil con luz. Aviso de 0,45 s con el sprite de lanzamiento; un golpe durante el aviso cancela el disparo (antes disparaba sin aviso y a quemarropa era inevitable: 3–4 de vida por Lighmet) |
| Vagoneta | 7 m/s, salto de 1,6 m, freno al 70 % durante 1,5 s; umbral de 18 de 28 minerales |
| Lámpara del casco en N4 | 4,2 m |
| PerforAudora | escala ×4; los valores de fase, atasco, piedras, altar y Tío siguen el GDD 9 |

## Correcciones aplicadas

- **Perímetro.** Hay barreras invisibles y estacas en los bordes sin suelo debajo. Los huecos jugables se declaran por sala. El hueco de 0,5 m de N01S02 resultó ser un peldaño (la cápsula del personaje mide 0,66 m).
- **Altar compartido.** Acredita la cuota del nivel de la expedición actual (`currentLevel`).
- **Línea de visión.** El golpe del pico y la mordida la calculan a 1,2 m: el canto de un desnivel de 0,65 m ya no bloquea el ataque. Los enemigos pueden bajar desniveles de hasta 1 m, pero no subirlos.
- **Orientación del golpe.** Pulsar una dirección y X en el mismo fotograma iniciaba el golpe con la orientación anterior, porque el ataque detiene el paso antes de actualizarla. En la cornisa de N02S04 el Resisworm llega en diagonal: el pico fallaba 8 golpes seguidos (0 objetivos) mientras la mordida restaba vida. Ahora la dirección pulsada orienta el golpe y el sprite. Diagnóstico: `PruebasTemporales/FightDiag.cs` (ayuda temporal), registro en `Docs/Implementacion/Niveles/Diagnostico/Diag_combate.txt`. Tras el cambio: 2 golpes, enemigo derrotado, 1 de vida perdida.
- **Barandas del puente de N02S05.** Colliders laterales que solo bloquean con el puente bajado, más postes y pasamanos visibles. La garganta sigue siendo una caída intencional con el puente alzado.
- **Luz principal de N2–N6.** El montaje borraba la luz direccional de la plantilla; ahora cada escena crea «Luz principal» (M01DarkRoom la atenúa en N4).
- **Selector.** Desde N4 ninguna flecha llevaba a N5: Derecha elegía N3 por estar más cerca. Ahora gana el vecino mejor alineado con la flecha. Recorrido: N1 → N2 con Derecha, N2 → N3 con Arriba, N3 → N4 con Izquierda o Arriba, N4 → N5 con Derecha y N5 → N6 con Derecha. El aviso de «escena pendiente de producción» usa `HasScene`.
- **Vagoneta del cierre.** El plano la situaba en (8, 0, −12,5), fuera del anillo de paredes de la arena (radio 10) y sin suelo: el cierre era inalcanzable. Ahora está dentro, en (6,2, 0, −6,5), con el minero en (4,4, 0, −7,4); el viaje cruza la arena hasta (−7, 0, −6,5) antes de «Continuará».
- **Opciones del diálogo.** Se desplazan cuando hay más de tres.
- **Logo de PerforAudora.** Se repintó el duplicado del lado +Y del primer tramo de la punta (1.315 caras) con el dorado vecino. Aplanar el relieve se descartó porque rompe las normales del GLB. Queda un contorno tenue del relieve, visible solo de cerca.
- **Selector.** Usa `Mapa_Mundo1_Concepto.png` como fondo, con las losas pintadas como nodos y los estados de cada nodo y requisito; cada nodo entra en su escena real.

## Propuestas pendientes de decisión del diseñador

- Exigencia de N4. Sin consumibles, cinco Resisworm y tres Lighmet consumen los 10 de vida: el recorrido automatizado moría en el último Resisworm con 1 de vida. Solo el 10 % de las bajas suelta un curativo (+2). El diseño ya prevé la tienda del pueblo (Fragmento +2 por 5, Núcleo +5 por 12, usados desde el inventario). Por eso no se añadió ningún punto de descanso: el recorrido compra un Núcleo por nivel completado (fixture) y lo usa con 4 de vida o menos. Decidir si conviene que Platicio o un cartel recuerde la tienda antes de N4.
- Líneas ausentes. N4D.01, N4M.01 y N4M.02 no existían en `Guion_M01.json`: la investigación de N4 lanzaba una excepción y no se registraba. Se redactaron las tres líneas (pendientes de revisión de tono) y se añadió `Tools/Documentacion/Validar_Lineas.py`.
- Lighmet de N04S04. El plano lo situaba a 1,35 m sobre el suelo, fuera del alcance del pico. Bajado a 0,05.
- Confirmar o ajustar el umbral de 18 minerales y los valores de prueba de Gasplode y Drilly.
- Nombre definitivo del consumible recogido como «Curativo» (el GDD nombra «Fragmento pulido»).
- Sustituir el icono provisional del teléfono, el emblema y las siglas `[SIGLAS]` de la pieza dorada.


## 2026-11-09 · Recorrido completo C56 (agente)

- Se verificó C56 con el recorrido encadenado `M01FullRun` (vuelta 6, 0 fallos). Evidencia en `Docs/Implementacion/Recorrido_Completo/`.
- **Decisión de prueba.** Entre N1 y N2 el recorrido vuelve al pueblo y compra un Núcleo con el dinero ganado, en vez de recibirlo del fixture. Sin esa compra, la vuelta 1 murió en N2.
- **Correcciones del recorrido, no del juego:**
  - espera de texto escrito antes de elegir en menús;
  - ruta de la tienda que evita al vendedor;
  - puerta del pueblo pisada en su centro;
  - reintento de la palanca de N1;
  - táctica de N6 contra el reapuntado del taladro;
  - espera a que la pieza dorada aparezca tras la explosión.
- **Observación para el diseñador.** La vida no se regenera entre niveles. En la vuelta 6 se llegó a N4 con 6/10 y a N5 con 5/10, y solo los Núcleos comprados la repusieron. Se suma a la propuesta pendiente sobre la exigencia de N4.
