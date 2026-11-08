# Estudio de construcción de mundos · Paper Mario (TTYD y The Origami King)

Sesión de Claude, 8 de noviembre de 2026. Los fotogramas se extrajeron con `Tools/Documentacion/Hoja_Contactos_Video.py` (OpenCV, Python 3.11) en hojas de contactos con marca de tiempo. Las hojas están en `Docs/Referencias/Analisis/Estudio_Niveles_2026_11_08/`. Se observaron fotogramas espaciados (cada 10–30 s), no el vídeo continuo. Las afirmaciones se limitan a lo visible en esos fotogramas. No se copian salas, gráficos ni secuencias: se extraen principios y se adaptan a la mina.

Fuentes (`Afterglow_Entrega_v0_5/Reference/General/Videos/`):

- **TTYD**: `Paper Mario_ The Thousand-Year Door - Full Game (Switch).mp4` (12:00:00).
- **TOK**: `Paper Mario_ The Origami King - Full Game Walkthrough.mp4` (13:07:30).

## Secuencias observadas

| Archivo y tiempo | Qué ve el jugador al entrar | Cómo entiende adónde ir | Principio | Aplicación concreta en Afterglow |
| --- | --- | --- | --- | --- |
| TTYD 1:03:00–1:03:41 | Campo abierto con el castillo en el horizonte; luego el puente levadizo y la muralla | El destino se ve antes de poder alcanzarlo. La ruta lo enfrenta | Objetivo visible desde la entrada | N2S01: desde la puerta se ve, al fondo y en alto, la torre de poleas de la salida principal |
| TTYD 1:09:51–1:11:19 (sala de bloques) | Bloques de piedra que suben en escalones hasta un balcón, con la puerta a la derecha y arriba | La puerta del balcón se ve al entrar. La escalera de bloques y el bloque de color señalan el mecanismo. La cámara sube con el personaje | Cambio de altura legible: escalones visibles, el mecanismo cerca de su efecto y la cámara acompaña la cota | N2S02 (Cruce de rieles): plataforma de mineral que la palanca sube junto a la escalera de cajas. La puerta alta se ve desde la entrada |
| TTYD 1:10:35–1:10:46 | Pasarela superior larga, con ventanas que marcan el ritmo y la misma sala abajo | Se recorre la sala por arriba y se vuelve al suelo | Una misma sala con dos alturas: la segunda pasada cambia la lectura | N2S04: galería con pasarela de vigas sobre el cruce. Ramal superior hacia el cofre y regreso por escalera |
| TTYD 1:14:02–1:18:10 | Sala con suelo visto en 3/4 y obstáculos en profundidad; después, sala oscura con balcones | Barreras físicas y luz de ventana marcan el camino | La profundidad se usa como dimensión de recorrido, no como decorado | N2S03: patio ancho con dos carriles en Z y un carro en el de atrás para la placa de Q03 |
| TTYD 0:51:12–0:56:53 (subsuelo) | Cámara con agua y un pasillo de arcos repetidos | Los arcos encuadran la línea a seguir. El cambio de iluminación marca la sala nueva | La repetición rítmica guía. Un cambio de luz o color anuncia que cambió la sección | N3: las cámaras de confirmación cambian el color de los faroles respecto de las de acertijo |
| TOK 1:19:36 | Torre alta y una cinta roja que la rodea, vista desde abajo | Hito vertical permanente | El hito vertical orienta durante todo el nivel | N2: torre de poleas visible desde S01, S03 y S04 |
| TOK 1:20:08–1:22:48 | Sendero que sube por terrazas, con salientes laterales y un arco de piedra a otra altura | El sendero claro contrasta con el césped. Los ramales salen del sendero principal y lo dejan a la vista | Separar ruta principal y ramal por material de suelo | Rieles y tablones para la ruta principal; roca suelta y gravilla para los ramales |
| TOK 1:24:24–1:24:56 | Saliente pegado a la pared que se alcanza con una acción; recompensa con presentación (manos en alto, brillo y cartel) | El objeto opcional está a la vista pero exige una acción | Un ramal corto recompensa la observación. Confirma nuestra presentación de objeto clave | N2: rocas opcionales de Q04 con la caja visible tras la tercera |
| TOK 1:25:28–1:26:00 | Pasillo de estacas de madera en ambos bordes y terrazas con vallas | El borde caminable se lee por objetos, no por un muro invisible sin forma | Perímetro protegido con señal física (valla, estaca o roca) más un collider invisible | N01S02 y N2–N4: vallas de madera y bordes de roca en los perímetros, con BoxCollider invisible más alto que el salto |
| TOK 1:39:13–1:42:52 (templo oscuro) | Cueva casi negra, antorchas en la pared y bordes de plataforma iluminados; la salida se ilumina desde arriba | Cada luz marca un rellano. El brillo dibuja el borde. El destino tiene la luz más fuerte | En la oscuridad, la luz dibuja el recorrido: suelo inmediato, bordes y destino siempre legibles | N4: faroles fijos en cada rellano, bordes con emisivo tenue y el carro de luz como faro móvil. Los proyectiles del Lighmet iluminan al pasar |
| TOK 1:36:05–1:37:39 | Arena circular con un borde claramente distinto | El círculo contrasta con la sala de acceso | Contraste pasillo/arena | N6: corredor estrecho de vigas que se abre a la arena circular de 10 m |

## Mapas y conceptos propios de Mundo 1

- **`Distribucion_Secciones.png`**: un nivel en varias secciones conectadas por puertas. La ruta principal va de A a B y C; D es un ramal opcional que regresa por la misma puerta; E es el final investigable. Este grafo es la base de N2, que lo amplía con dos ramales y un patio.
- **`Camara_Cruce_Carretilla_Concepto.png`**: estilo de referencia para N2. Roca facetada cálida, rampas escalonadas con baranda de madera, faroles en cada portal, reja con polea, carro sobre riel con placa amarilla y amatistas como acento. De aquí salen el vocabulario de N2 (polea, reja, placa y carro) y el uso de barandas como perímetro legible.
- **Once mapas de `MapasNiveles`**: ya analizados en `Mapas_Nivel1.md`. Para N2 se toman tres principios: un centro con varios enlaces y ramales laterales (images (2).jpg), un vacío central que organiza el recorrido y muestra el objetivo al otro lado (Hoohoo Mountain) y la alternancia de salas anchas y pasos estrechos (Joke's End).

## Situaciones propias derivadas

1. **Placa y carro.** Empujar el carro sobre el riel hasta la placa mantiene abierta la reja. El carro no puede salir del riel: los topes lo impiden. Al salir y volver a la sala, el puzle se restablece.
2. **Hangzzer colgado de un farol.** Baja, silba y llama a dos Resisworm desde un túnel visible. Se ve de dónde baja (el farol) y por dónde llegan los refuerzos (la boca del túnel). Se coloca en N2S03, sobre el cruce, cuando el jugador ya conoce al Resisworm de N1.
3. **Puente de vigas.** Una palanca baja el puente. Ya se aprendió en N1, así que en N2 se combina con el carro (problema conocido + uno nuevo).
4. **Mineros ambientales.** Diálogos breves con humor local, independientes de las misiones (por ejemplo, el minero que discute con su carro).
