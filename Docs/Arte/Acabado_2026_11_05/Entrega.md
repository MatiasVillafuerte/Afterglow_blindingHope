# Acabado visual integrado — 2026-11-05

Se analizaron las diez skills instaladas y se aplicaron las de Blender al arte de producción existente. La selección y el criterio para todo el juego se encuentran en `Docs/Arte/Direccion_Arte_Afterglow.md`. Shader Graph no fue necesario; no se instalaron dependencias. El diseño de nuevos niveles queda para el encargo posterior.

## Resultado

- Dormitorio: paleta cálida, materiales de madera con veta, alfombra ornamental violeta y dorada, cortinas verdes, lámpara de mesita, estantería de minerales, libros y remates de muebles.
- Casa y patio: ventanas arqueadas con carpintería conectada al vidrio, frontón, molduras, basamentos y pilares de piedra, flores, vegetación suavizada y fuente con normales conservadas.
- Rejas: las 200 barras del recinto comparten 2,22 m de altura, incluidas 48 barras situadas sobre la línea frontal contando las esquinas. Los remates decorativos llegan a 2,47 m. El portón usa la misma altura de panel; sus pilares monumentales son distintos deliberadamente.
- Mina: roca ocre/malva en lugar de gris uniforme, masas de fondo menos afiladas, veta de madera, biseles, abrazaderas, barriles y luz cálida. Se aplica a las cuatro secciones N1 existentes.
- Mapa: conserva el selector y su distribución; la miniatura comparte la paleta y los materiales de la mina.

Fuente editable: `ArtSource/Blender/Modelado_Afterglow/Afterglow_Prologo_Nivel1.blend`. Siete FBX, materiales y seis familias de texturas horneadas en `Assets/Afterglow/Environment/Modelado`. No se cambió la lógica del juego, el tamaño del personaje, sus clips, las cámaras espaciales ni las rutas de progresión. No se ejecutó SpatialWorldBuilder.

## Comprobación final

| Modelo | Mallas de exportación | Triángulos Blender y Unity |
|---|---:|---:|
| PRO_CASA | 13 | 22.960 |
| PRO_PATIO | 19 | 173.460 |
| N01S01 | 12 | 28.988 |
| N01S02 | 12 | 40.872 |
| N01S03 | 13 | 37.064 |
| N01S04 | 12 | 42.944 |
| M01_MAPA | 12 | 16.878 |

- 2.268 objetos de origen auditados con blender-asset-qa: cero errores. Advertencias de repetición de UV, posiciones dentro de la escena y tapas rígidas documentadas en la guía. Conteo por objeto: `ArtSource/Blender/Modelado_Afterglow/Revision/TrianglesByObject.json`.
- Revisados además cinco conjuntos: lámpara/mesita, estantería/minerales, ventanas, barril/aros y soporte/abrazadera. Todos pasan. Se corrigieron barras de ventana separadas del vidrio y superficies coincidentes entre barras y travesaño. Se aceptan unas pocas caras opuestas ocultas en apoyos para conservar props cerrados e independientes.
- Siete FBX reimportados: dimensiones con diferencia menor que 0,000002 m y escala unitaria. `Revision/RoundTrip.json`. Láminas random y wire de cuatro ángulos abiertas y revisadas, además de capturas Game.
- 28 verificaciones del editor: triángulos completos, materiales URP, cámaras espaciales, referencias de scripts y escenas del flujo normal. `Inspeccion.md`.
- 64 verificaciones en Play: Nueva partida, carta, dormitorio, patio en profundidad, capitán, mapa, cuatro secciones N1, saltos, ofrendas, ataque, roca rompible, investigación, teléfono y altar. Cero fallos. El archivo de guardado personal permanece intacto. `Pruebas.md`.
- Dos verificaciones adicionales de encuadre junto a reja frontal y lateral. Capturas `Reja_Frontal.png`, `Reja_Lateral.png`; el personaje se mantiene legible detrás de las barras. La reja no se redujo para facilitar la cámara.
- Consola actual: cero errores, cero advertencias, compilación correcta. El buffer histórico conserva cuatro avisos del primer compilado sobre una API obsoleta, corregida por su sobrecarga vigente; no se borró el buffer. `Consola_Final.json` distingue historial y groundTruth.
- Unity 6000.5.5f1 y URP 17.5.0 conservados. CLI 1.0.0-beta.16 comprobada como vigente. Manifiesto, renderer URP, prefab y animaciones del personaje coinciden con sus hashes anteriores: `Versiones_Conservadas.json`.

Estado final: editor conectado y detenido en M01_MENU, escena limpia, Run In Background restaurado al valor del proyecto. No commit, push ni build. El checklist maestro y PM00–PM20 conservan su avance; esta pasada visual no marca niveles o mecánicas pendientes como completados.

## Fuentes y límites

La carpeta `Antes` conserva cuatro capturas previas y el informe anterior. Las capturas numeradas de esta carpeta corresponden al arte actualizado. `Revision` conserva auditorías, láminas e informes Blender. Se corrigió también la pérdida de normales de esquina durante el agrupado por material del exportador.

La revisión automática rechazó volver a generar todos los modelos por el riesgo de sobrescribir distribución o jugabilidad. La reparación final de las ventanas se realizó con `repair_windows.py`, limitada a las nueve ventanas y al FBX del patio; conservó el resto de objetos, escenas y JSON de colisiones.

Este pase aproxima el estilo de los conceptos; todavía no reproduce toda su densidad de detalle. Los NPC provisionales y las zonas no construidas no se presentan como arte final. No se midieron FPS ni se probó un ejecutable standalone. Los recursos creados son propios y las imágenes de referencia no se convirtieron en texturas extraídas.
