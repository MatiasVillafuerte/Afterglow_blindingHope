# Decisiones del 8 de noviembre de 2026 · ataque X, daño visible y adquisiciones

Sesión de Claude. Registro separado según Protocolo_Documentacion.md. Se distingue la decisión del diseñador, el parámetro inicial elegido por el agente y la propuesta.

## Decisiones del diseñador (encargo del 7 de noviembre, ya registradas en GDD y Encargo_Claude_Mundo_1)

- X debe atacar cuando juega una persona, con el foco en Game y tras recibir el pico.
- Cada impacto efectivo muestra la cifra sobre quien lo recibe, según DañoCausado.png.
- El timbre muestra una sola vez «Alguien tocó la puerta» (P01.04).
- Los objetos clave (teléfono, pico) tienen una presentación con pausa, manos en alto, acercamiento y brillo. Las recogidas comunes muestran una etiqueta breve sin pausa.

## Diagnóstico observado con teclado real

Se jugó desde M01_MENU, con Nueva partida, hasta que Platicio entregó el pico en N01S03. Las teclas se enviaron como pulsaciones del sistema operativo (keybd_event) a la ventana de Unity, con la configuración de entrada normal (PointersAndKeyboardsRespectGameViewFocus).

- X sí llegaba al juego: el estado pasó a `Attack`.
- El Resisworm se detenía a 1,50 m y mordía hasta 1,65 m. El pico solo alcanzaba objetivos con el centro a 1,2 m hacia delante y a 0,4 m de margen lateral. Desde la distancia de espera del enemigo, X nunca acertaba y el jugador perdía vida hasta morir.
- La prueba automática anterior acercaba el personaje a 1,1 m antes de atacar. Por eso no detectó el problema.
- La roca usaba un rayo de 1,24 m en la dirección exacta del personaje. No había respuesta visual al golpe.

## Parámetros elegidos por el agente (ajustables)

| Parámetro | Valor | Fuente |
| --- | --- | --- |
| Volumen del pico | caja de 1,2 m hacia delante, 0,8 m de ancho, 1,6 m de alto y 0,15 m detrás del centro | GDD 7.1 (alcance y anchura); altura y retroceso elegidos por el agente |
| Consulta | `Physics.OverlapBox` contra los colliders reales. Una aplicación por objetivo y acción. Línea de visión contra la capa 0 | Agente |
| Mordida del enemigo | el mismo volumen, orientado al inicio de la anticipación (0,3 s). El jugador puede salir de él | Agente |
| Distancia de espera del Resisworm | 1,05 m (antes 1,5 m), dentro del alcance del pico | Agente |
| Vida del Resisworm | 2 (antes 3 en el prefab) | GDD 7.2 |
| Visualización de volúmenes | F9 alterna cajas translúcidas: amarilla para el pico, roja para los enemigos | Encargo, prioridad 1 |
| Cifra de daño | estallido rojo de 11 puntas, número claro de 60 px con borde oscuro. Rebote de 0,16 s, lectura hasta 0,75 s y retirada en 1 s. Varios golpes seguidos sobre el mismo objetivo se apilan | GDD y DañoCausado.png |
| Respuesta del enemigo | destello rojizo de 0,14 s. Sin empuje | GDD (sin empuje) |
| Roca | se rompe al entrar en el volumen del pico y suelta 9 fragmentos físicos durante 1,4 s | Agente |
| Aviso del timbre | centrado, 40 px, 3,2 s, texto P01.04 leído desde Guion_M01.json | Guion |
| Avisos comunes | esquina superior derecha bajo el HUD, hasta 5 visibles, 2,6 s cada uno, «×N» al repetir el mismo objeto, sin pausa | Agente |
| Presentación de objeto clave | acercamiento de 0,45 s a 6,2 m del foco, brazos a 128°, espera de 2,4 s (E acorta a partir de 1,5 s) y retirada de 0,4 s. Usa el modal del diálogo, que congela tiempo, IA y botín | Agente |
| Nombres visibles | Hoja de coca, Curativo, Dinero, EXP, Casco, Pico y Teléfono de minas. «Curativo» es provisional hasta definir el catálogo de consumibles | Agente; confirmar con el diseñador |
| Icono del teléfono | provisional, generado por código. Reemplazable con `Resources/KeyItems/Teléfono de minas.png` | Encargo, prioridad 3 (iconos provisionales) |

## Hallazgo pendiente para la prioridad 2 (escenarios)

En N01S02, una línea diagonal desde la entrada (−8; −5,2) hacia las terrazas cae al vacío en (−5,6; −0,4), (−3,9; −1,1) y (−2,6; 0,2), y en (5,6; −0,9) tras el salto hacia la segunda terraza. Cada caída resta 2 y devuelve a la entrada de la sección. Cinco caídas provocan la muerte y el reinicio en N01S01 con vida completa. Hay que marcar qué bordes son caídas intencionales y cerrar los demás con barreras invisibles.
