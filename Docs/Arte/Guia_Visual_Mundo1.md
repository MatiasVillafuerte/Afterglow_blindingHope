# Guía visual propuesta — Mundo 1, muestra N01S02

Esta propuesta se aplica solo a M01_N01S02_Arte, copia de la sección N01S02 «piso amplio y escalones bajos» del diseño. No fija el arte definitivo del mundo. Conserva los personajes 2D, animaciones, cámara y geometría de colisión de producción.

## Lenguaje y jerarquía
Mina artesanal estilizada, con formas robustas y facetadas. Tres tamaños: masas de roca en fondo; pórticos de madera y rieles como ritmo; juntas y cantos biselados como detalle. Las rocas no deben parecer cubos pintados. El soporte tiene patas inclinadas, travesaño y tornapuntas; el riel tiene perfil metálico real y durmientes.

El personaje morado/ámbar es el punto más saturado. El suelo transitable conserva un valor medio cálido y una superficie tranquila. El fondo usa pizarra azul, menor contraste y masas grandes. La zona de subida y su ofrenda mantienen bordes claros; ninguna decoración invade el volumen de salto ni oculta la salida.

## Paleta inicial
| Uso | Color propuesto | Acabado |
|---|---|---|
| Roca principal | #405463 | Rugosa, facetas anchas, variación gris azul |
| Aristas de roca | #6C7C87 | Luz suave, sin brillo plástico |
| Suelo | #92745B | Tierra cálida mate, poco ruido |
| Madera | #93643E | Ocre oscuro, biseles desgastados y veta discreta |
| Metal | #414B52 | Hierro apagado, canto legible y suavidad baja |
| Luz de trabajo | #FFD39A | Ámbar moderado, puntos separados |
| Fondo profundo | #182A37 | Azul petróleo; conserva siluetas |

Estas decisiones son propuestas para comparar en Game. Evitar el negro puro y la niebla que oculte bordes. Sin desenfoque de profundidad, sin cambiar render scale, sin recolorear los sprites.

## Composición
Se conserva el piso de 44 × 12 m, escalones de 0,3 m y plataforma de 1,2 m. Los pórticos y rocas grandes se colocan tras la ruta, cerca del muro posterior. Rieles y durmientes son decoración de suelo fuera de la subida; no introducen un puzle ni una vagoneta. Decoración frontal baja y espaciada. El área de paso y salto se reserva manualmente; la semilla solo varía la decoración.

## Materiales e iluminación
Fuentes modeladas en metros, pivote al suelo, escalas aplicadas, UV y normales verificadas. Exportación FBX con el preset Unity de scenario-blender-export. Materiales Blender usados para previsualización; Unity reconstruye URP/Lit y asigna las texturas expresamente. No se confía en la transferencia automática de nodos. Luz direccional suave, relleno azul y lámparas ámbar locales; sin cambiar las URP Assets de producción. Colisiones simples de la sección original; adornos sin colisiones de gameplay.

## Referencias realmente inspeccionadas
- Super Paper Mario Chapter 5-3: fotograma 99,9999 s, Docs/Referencias/Analisis/M01_REF_1.png. Principio: capas de roca y madera, actor separado del fondo. No se copia habitación ni textura.
- Donkey Kong Country Returns Crazy Cart: fotograma 94,9949 s, M01_REF_2.png. Principio: estructura portante, riel legible y luz cálida/fría. No se copia secuencia de obstáculos.
- PuzzleInspiration.jpeg: separación de zonas y marcas claras. Esta muestra no cambia la lógica de puzles.
- Personaje_bolivianita.png y rig importado: reserva de contraste alrededor de cabeza, brazos y botas.

No se afirma haber visto los videos completos en esta tarea. El registro de muestreo anterior identifica los fragmentos. Los otros mundos se consultaron solo por su identidad del GDD: Cataratas, Galaxia, Abandonada, Fábrica y Coltán. No se producen aquí.

## Aceptación de la muestra
Capturas antes/después con jugador en (20, 0,05, 0), misma cámara/FOV y pose Idle. Tres rocas distintas, soporte y riel con fuente .blend y FBX; mallas/prefabs reutilizables. Revisar triángulos, escala, UV, normales y referencias de materiales. Probar caminata, movimiento en profundidad, salto, escalones y subida sin habilidades compradas. Comparar colisiones y hashes de producción. Registrar fallos y límites de la prueba, no marcar gameplay ajeno.
