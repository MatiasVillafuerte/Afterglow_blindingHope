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
| Lighmet | 4 de vida, disparo cada 2,4 s a 5 m/s, daño 1; espíritu y proyectil con luz |
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
- **Opciones del diálogo.** Se desplazan cuando hay más de tres.
- **Logo de PerforAudora.** Se repintó el duplicado del lado +Y del primer tramo de la punta (1.315 caras) con el dorado vecino. Aplanar el relieve se descartó porque rompe las normales del GLB. Queda un contorno tenue del relieve, visible solo de cerca.
- **Selector.** Usa `Mapa_Mundo1_Concepto.png` como fondo, con las losas pintadas como nodos y los estados de cada nodo y requisito; cada nodo entra en su escena real.

## Propuestas pendientes de decisión del diseñador

- Confirmar o ajustar el umbral de 18 minerales y los valores de prueba de Gasplode y Drilly.
- Nombre definitivo del consumible recogido como «Curativo» (el GDD nombra «Fragmento pulido»).
- Sustituir el icono provisional del teléfono, el emblema y las siglas `[SIGLAS]` de la pieza dorada.
