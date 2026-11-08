# Dirección de arte de Afterglow — 2026-11-05

Base visual compartida para los escenarios y props actuales y futuros. Complementa el diseño y el guion; no cambia las distribuciones, reglas ni tareas de los niveles. Sustituye el alcance limitado a la antigua muestra de `Guia_Visual_Mundo1.md` para el arte de producción.

## Referencias y criterio

Fuente: `Afterglow_Entrega_v0_3/Reference/Mundo 1/Imagenes`, especialmente Dormitorio_Concepto, Casa_Jardin_Concepto, Camara_Terrazas_Concepto, Camara_Cruce_Carretilla_Concepto y Mapa_Mundo1_Concepto. Los bocetos funcionales siguen determinando la distribución; el concepto determina colores, materiales y formas. Las imágenes de otros juegos son referencia, no texturas que extraer.

El personaje 2D tiene color cálido, silueta clara y dibujo detallado. Su entorno 3D debe acompañarlo: madera cálida, cantos redondeados, variación pintada moderada y sombras suaves. Evitar metal brillante generalizado, roca azul gris uniforme, geometría afilada gigante y ruido de alta frecuencia que compita con el personaje. No imponer un contorno negro grueso a todo el escenario.

| Zona | Formas y materiales | Acentos |
|---|---|---|
| Dormitorio | Madera tallada, paredes crema, suelo de tablones, cortinas verdes, alfombra violeta y dorada | Lámpara cálida, libros, minerales inertes de estantería |
| Casa y patio | Fachada crema, teja terracota, ventanas arqueadas, molduras doradas, hierro oscuro y basamento de piedra | Agua turquesa, flores rosas y crema, vegetación de copas suaves |
| Mina | Bloques de roca ocre con variación malva, vigas gruesas biseladas, abrazaderas oscuras | Linternas ámbar, cristales violetas y vetas doradas |
| Selector | Miniatura del mismo paisaje, madera y roca de la misma familia | Recorridos y nodos legibles; los estados de UI no dependen solo del color |

La densidad de detalle debe concentrarse en muebles, accesos, apoyos y objetos interactivos. El suelo y los fondos usan variaciones de menor contraste. El suelo caminable debe reconocerse desde la cámara de juego. Una revisión de arte no autoriza estrechar el patio ni convertir las salas en pasillos.

## Skills instaladas y uso

Se leyeron las diez skills indicadas por el usuario en `C:/Users/matia/.agents/skills` (versión 1.2 de la familia Blender, licencia MIT).

| Skill | Aplicación en esta pasada |
|---|---|
| blender-core | Blender 5.2.3 LTS por CLI, informe de entorno y operaciones bpy compatibles |
| blender-environment-kits | Escala en metros, continuidad del kit, familia común de materiales y rejas de altura uniforme; sin crear niveles nuevos |
| blender-stylized-npr | Siluetas suaves, paleta cálida y separación de materiales; se eligió acabado pintado, sin bandas de sombra ni contorno universal |
| blender-pbr-materials | Materiales mates y lectura coherente en URP; base color sRGB y datos de brillo en espacio lineal |
| blender-texturing | Variación pintada, veta de madera y dibujo ornamental de alfombra |
| blender-texture-baking | Horneado real de base color y roughness en Cycles; empaquetado de metallic/smoothness para Unity |
| blender-lighting-lookdev | Revisión de color y luz cálida en Game, luz puntual de mesita y luz principal menos dura |
| blender-export-pipelines | Conservación de escala/ejes, normales de esquina evaluadas y comprobación de vuelta desde FBX |
| blender-asset-qa | Auditoría por malla y por conjuntos de props, reparación de piezas separadas y superficies coplanares, láminas aleatorias y wire desde cuatro ángulos |
| shader-graph-best-practices | Evaluada. No fue necesario crear un Shader Graph: los materiales URP/Lit actuales admiten el acabado y evitan introducir shaders extra |

No se instalaron plugins, paquetes de Unity ni servicios de pago. Las skills de diseño de niveles se reservan para el encargo posterior.

## Producción y exportación

- Fuente editable: `ArtSource/Blender/Modelado_Afterglow/Afterglow_Prologo_Nivel1.blend`; las siete escenas conservan sus objetos individuales.
- Materiales compartidos y paleta: `Assets/Afterglow/Environment/Modelado/ArtStyle.json` y los `.mat` de esa carpeta.
- Seis familias de texturas propias de 512 × 512: Wood, Stone, Plaster, Fabric, Rug y Grass. Base color, roughness de trabajo y mapa MS con metallic en R y smoothness en alfa. Repetición intencionada, mipmaps y anisotropía 4.
- Las UV son proyecciones de repetición, no un atlas exclusivo para lightmaps. No hornear iluminación sobre estas UV. Crear UV2 independientes si en el futuro se usa iluminación horneada.
- La exportación combina por material, conserva por separado la roca rompible y conserva normales de esquina del modelo evaluado. No perder el suavizado al juntar piezas.
- Los paneles de reja y el portón comparten 2,22 m de cuerpo, con remates de hasta 0,25 m. Los pilares de piedra tienen su propio remate; su altura distinta es intencionada.
- Antes de entregar otro prop: comprobar escala contra el personaje, contactos entre piezas, UV, normales, materiales y vistas en Game. Guardar fuente y evidencia.

`AfterglowArtFinish.Apply()` remapea materiales y ajusta iluminación mediante la API del editor. No ejecuta el constructor de mundos ni cambia rutas, plataformas, diálogos, progreso o colisiones. Requiere editor fuera de Play y escena guardada. Las copias previas se conservan en `Versiones/Antes_Acabado`.

## Alcance y advertencias aceptadas

Se aplicó el acabado a PRO_CASA, PRO_PATIO, N01S01–N01S04 y M01_MAPA. Esta es una mejora del arte existente, no una reproducción terminada de todos los detalles de los conceptos ni la creación de los niveles N2–N6. Los NPC provisionales siguen pendientes de su pase de personaje.

Las advertencias de UV superpuestas se aceptan porque las texturas son repetibles y se hornearon previamente en una baldosa separada. Las posiciones elevadas de objetos se conservan porque pertenecen a una escena montada, por ejemplo libros sobre mesa y minerales sobre estante; sus contactos se revisan por conjunto. Los n-gons planos de tapas de piezas rígidas no se deforman y el FBX contiene triángulos. Se aceptan las caras ocultas opuestas en los apoyos de lámpara y minerales: mantienen cada prop cerrado y editable, no son superficies visibles que parpadeen; una optimización posterior puede eliminarlas. No se interpreta una auditoría individual como prueba de contacto entre objetos; para eso están `Assemblies.json` y las láminas de conjuntos. La pasada no incluye una medición de FPS ni prueba en un ejecutable standalone.
