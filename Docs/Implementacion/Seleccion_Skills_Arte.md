# Selección y prueba de skills de arte — Afterglow

Fecha: 2026-11-03, America/La_Paz. Encargo leído completo: Prompt_Buscar_Skills_Arte_Afterglow.md. Muestra terminada y probada; no es el acabado definitivo de todos los niveles.

## Diagnóstico y decisión

La skill de búsqueda solicitada se llama **find-skills**; ya existía en C:/Users/matia/.codex/skills/find-skills. Se leyó su SKILL.md, se consultó el leaderboard de skills.sh y se ejecutó `npx --yes skills find` con `blender python bpy`, `unity probuilder`, `stylized environment` y `game art direction`. Se ampliaron los resultados a repositorios originales y documentación de los fabricantes. Los contadores de catálogo son orientativos y cambian; no se eligió por el nombre únicamente.

Unity: 6000.5.5f1; URP: 17.5.0; CLI: 1.0.0-beta.15; Pipeline: 0.9.1-exp.1. Plugin oficial unity-agent-plugin 0.1.8-beta ya disponible. Se abrió este proyecto y se comprobó Pipeline ready, puerto local 7800, escena y consola. No se instaló otro puente. Poder leer/escribir C# no se tomó como prueba de control del editor: se ejecutaron órdenes reales de escena, importación, compilación, Play y captura.

Blender estaba instalado, aunque no figuraba en PATH: C:/Program Files/Blender Foundation/Blender 5.2/blender.exe. Se ejecutó realmente `--background --factory-startup` y bpy confirmó **5.2.3 LTS**, Python y exportación FBX operativos. No hizo falta instalar Blender, pip bpy, un complemento MCP ni una cuenta de Scenario. La licencia del programa está en copyright.txt de la instalación (GNU GPL); no añade coste a esta tarea.

Se presentó la selección y la vía de modelado en el chat antes de instalar. La combinación elegida es Blender/bpy local para las fuentes, el preset FBX Unity para entrega y la integración oficial existente para materiales URP, prefabs y revisión en Game.

## Conjunto principal, máximo cinco

Las tres skills nuevas proceden de [scenario-labs/skills](https://github.com/scenario-labs/skills), licencia [MIT](https://github.com/scenario-labs/skills/blob/e24bc7d40be97b428e7b9fd467c415366e1dd974/LICENSE), revisión fijada **e24bc7d40be97b428e7b9fd467c415366e1dd974**. Se leyeron los SKILL.md y los recursos aplicables antes de emplearlas. El repositorio tenía 1.964 estrellas al consultar la API; el catálogo mostraba aproximadamente 1K instalaciones para la skill principal. Esto no sustituye las pruebas locales.

| Nombre exacto y origen | Tipo y función | Herramientas / conexión / compatibilidad | Licencia, coste y estado |
|---|---|---|---|
| [scenario-blender-expert](https://github.com/scenario-labs/skills/tree/e24bc7d40be97b428e7b9fd467c415366e1dd974/skills/dcc/blender/scenario-blender-expert) | Skill; protocolo de construcción, revisión visual y auditoría | bpy, bx_audit.py y bx_review.py; Blender CLI local, sin servidor. Target 5.2; ejecutada en Windows con 5.2.3 | MIT, gratuito. **Instalado y probado**: cinco gates y láminas de silueta/matcap/wire |
| [scenario-blender-hard-surface](https://github.com/scenario-labs/skills/tree/e24bc7d40be97b428e7b9fd467c415366e1dd974/skills/dcc/blender/scenario-blender-hard-surface) | Skill; soporte y riel de geometría editable, juntas, biseles y normales | bx_hardsurface.py, bmesh y Blender nativo. Requiere expert a su lado; no requiere servicios externos | MIT, gratuito. **Instalado y probado**: pila Bevel/WeightedNormal y copia de exportación triangulada |
| [scenario-blender-export](https://github.com/scenario-labs/skills/tree/e24bc7d40be97b428e7b9fd467c415366e1dd974/skills/dcc/blender/scenario-blender-export) | Skill; archivos de modelos y control de escala/pivote/materiales | bx_export.py, preset unity y comparador de reimportación; requiere expert. FBX importado realmente en Unity 6000.5.5f1 | MIT, gratuito. **Instalado y probado**: cinco FBX reimportados, dimensiones y triángulos conservados |
| procedural-world-generation, local | Skill existente; semilla reproducible y zonas reservadas para decoración | C:/Users/matia/.codex/skills/procedural-world-generation/SKILL.md. Se aplican sus reglas de decoración secundaria, sin adoptar la narrativa de Huellas de luz | Origen local del usuario, sin licencia pública declarada. Sin instalación ni coste nuevo. **Reutilizado y probado**: semilla 40201; ruta y colisiones preservadas |
| [unity:unity-cli](https://github.com/Unity-Technologies/unity-agent-plugin) | Skill del plugin oficial; integración con el editor | Unity CLI + Pipeline ya instalados; localhost. Importa, edita escenas por API, consulta consola y captura Game | Licencias de la integración existente; sin compra nueva. **Ya instalado y probado** en este proyecto |

No se copió una biblioteca completa de skills. Se instalaron solo las tres carpetas de Scenario mediante el instalador de skills de Codex, con sus recursos y scripts. Rutas y hashes: Docs/Arte/Skills_Instaladas.json. Las tres carpetas están bajo C:/Users/matia/.codex/skills/. Estarán disponibles para detección automática en el siguiente turno; sus instrucciones y scripts ya se leyeron y usaron en este encargo.

## Comparación del modelado

**Blender/bpy, elegido:** produce mallas editables, UV, materiales y fuentes .blend. Ya funcionaba en la máquina. Permite biselar y comprobar normales/manifold antes de exportar. El precio es un paso explícito de importación y reconstrucción de materiales URP. Se verificó ese paso: los nodos Blender no se presentan como materiales URP automáticamente transferidos.

**Mesh API de Unity, complemento usado:** genera la pieza biselada de plataforma como Mesh .asset editable mediante API, manteniendo el BoxCollider. No requiere paquetes. Es útil para encajar geometría sencilla con precisión en el nivel; no sustituye las herramientas de edición de Blender para todos los props.

**ProBuilder, alternativa no instalada:** la [documentación oficial de ProBuilder 6](https://docs.unity3d.com/Packages/com.unity.probuilder@6.0/manual/index.html) ofrece edición de geometría, UV y API C#. La página del editor 6000.0 documenta 6.0.8. No se instaló ni se declara una prueba de esa versión en 6000.5.5f1: Blender y Mesh API cubrieron la muestra sin modificar Packages. No hacía falta instalar un paquete adicional solo para demostrar que existe.

## Candidatas descartadas relevantes

- [besty0728/unity-skills, unity-probuilder](https://skills.sh/besty0728/unity-skills/unity-probuilder): catálogo 82 instalaciones; repositorio original aproximadamente 1.916 estrellas, MIT. Su flujo usa UnitySkills y REST local 8090. Se contrastaron las instrucciones del módulo y la guía del repositorio. No se agregó ese puente, pues Pipeline 7800 ya opera el editor. ProBuilder puede usarse por su API sin ese puente si se necesitara después.
- [mattjamo/bpy_skill, nombre bpy](https://github.com/mattjamo/bpy_skill): SKILL.md leído; licencia Apache-2.0, Blender CLI y verificadores. Tenía 0 estrellas al comprobarlo. Es una alternativa ejecutable de propósito general; se prefirió la familia Scenario por su revisión de mallas, exportador y objetivo 5.2. No instalado ni ejecutado.
- [art-direction__defining-game-art](https://github.com/erikhazzard/vasir/tree/main/.agents/skills/art-direction__defining-game-art): SKILL.md leído, repositorio MIT y 1 estrella. Sus referencias internas principales son pixel RPG y hero-action 3D. No se verificó un flujo de arte 2.5D apropiado para esta muestra, por lo que no se instaló. No se atribuye a esa skill una dirección de arte que no se usó.
- fantasy-3d-art-director y level-designer locales: se inspeccionaron. Contienen contexto narrativo de Huellas de luz. Se conservan instaladas, pero no se importan sus instrucciones de animales, restauración mágica o historia a Afterglow. La carencia de una dirección específica se cubre con Docs/Arte/Guia_Visual_Mundo1.md, que es un documento del proyecto, **no una skill instalada**.
- Otro MCP Blender, Cinemachine, packs completos de assets y servicios de generación por créditos: no necesarios para esta muestra. No se instalaron ni se activaron pagos. Los fotogramas de referencia solo orientaron composición; no se extrajeron texturas ni modelos de esos juegos.

## Qué se produjo

Fuente editable y portátil, con dos texturas empaquetadas: **ArtSource/Blender/Afterglow_M01_MineKit.blend**. Contiene cinco modelos y sus materiales; soporte y riel conservan las bases editables y modificadores de bisel/normales. Los FBX usan copias trianguladas. El modelo de origen se dispone en una mesa de revisión; cada exportación conserva pivote al suelo y escala de metros.

| Modelo y prefab | Triángulos exportados | Uso |
|---|---:|---|
| Rock_A.fbx / Rock_A.prefab | 60 | Roca baja facetada |
| Rock_B.fbx / Rock_B.prefab | 60 | Roca vertical |
| Rock_C.fbx / Rock_C.prefab | 60 | Masa ancha con otra silueta y semilla |
| TimberSupport.fbx / TimberSupport.prefab | 660 | Dos patas inclinadas, travesaño y tornapuntas |
| RailModule.fbx / RailModule.prefab | 1.032 | Dos perfiles metálicos y cuatro durmientes |

Modelos: Assets/Afterglow/Environment/M01Art/Models/. Prefabs: la carpeta Prefabs/ del mismo directorio. Dos texturas propias de 256 × 256 en Textures/: Stone_BaseColor.png y Timber_BaseColor.png. Ocho materiales URP/Lit en Materials/: M01_Stone, M01_Timber, M01_Iron, Ground, Platform, PlatformTop, DeepRock y Lamp. Mesh adicional de plataforma biselada en Meshes/Platform_Beveled.asset. No se descargaron modelos ni imágenes de otros juegos.

Escena: **Assets/Afterglow/Scenes/Art/M01_N01S02_Arte.unity**, copia de N01S02, la sección de escalones y ofrenda de nivel uno. No es una habitación genérica. Conserva sus puertas, rutas, jugador y objetos. Los pórticos, roca y rieles reutilizan los prefabs. La decoración no bloquea la ruta: sus colliders están desactivados en las instancias; los prefabs conservan colisiones simples para otros usos. Los BoxCollider originales de suelo, límites y plataformas permanecen idénticos. El riel decorativo está bajo en el suelo, sin introducir mecánica de vagoneta.

Scripts reales:
- Tools/Art/generate_mine_kit.py: generación determinista y exportación.
- Tools/Art/review_mine_kit.py: reimportación secuencial de FBX, auditoría y láminas.
- Tools/Art/pack_sources.py: empaquetado explícito y reapertura de la fuente .blend.
- Assets/Afterglow/Editor/M01ArtSampleBuilder.cs: importadores, remapeo de materiales por nombre, prefabs y montaje de la copia.
- Tools/Art/CaptureSample.cs, VerifyUnitySample.cs y TraverseSample.cs: captura, inspección del editor y recorrido real.

## Pruebas observadas

- Blender 5.2.3 ejecutó los scripts, guardó la fuente y exportó cinco FBX. Gate final de cada copia de exportación: correcto, sin errores ni advertencias. Las bases editables de soporte/riel conservan n-gons planos, registrados como advertencia en sourceGate; la exportación es triangulada.
- Cinco FBX reimportados en Blender: cero diferencias de tamaño/triángulos/mapas según bx_export.compare; gates correctos. Se abrieron las cinco láminas de silueta, matcap y wire. Las facetas duras de las rocas son deliberadas; no se presentan como un error de suavizado.
- Fuente .blend reabierta: cinco modelos y dos imágenes empaquetadas, Verificacion_Fuente_Blend.json.
- Unity: cinco prefabs con escala/pivote correctos, UV y normales válidos, materiales URP presentes y triángulos completos. Ambas cabezas del riel usan hierro: cuatro triángulos superiores correctos, cero incorrectos. Cero componentes perdidos. Verificacion_Unity.md.
- Comparación de colisiones con N01S02 original: idénticas. Cámara conservada: offset (0,3.1,-14), FOV 32, pitch 5, minX 7, maxX 37, anticipación 1.5 y suavizado .25.
- Capturas Antes/Después: jugador (20,0.04,0), cámara (21.5,3.1,-14), misma pose Idle. Camara_Antes.txt y Camara_Despues.txt coinciden salvo el nombre de etapa.
- Recorrido con flechas y Espacio, controlador real, sin habilidades compradas: profundidad, salto, aterrizaje en la plataforma y llegada x31.231 correctos. Se recolocó solo la salida inicial; no hubo teletransportes entre puntos. El personaje permaneció en el encuadre. Prueba_Recorrido.md.
- Catorce hashes protegidos conservados: escena N01S02 de producción, cámara, prefab del jugador, manifiesto, versión, assets URP y clips. Verificacion_Produccion.json.
- Compilación y consola final de Play: cero errores y cero advertencias. No se marcaron tareas de gameplay como consecuencia de esta muestra.

Incidente corregido: Unity descartó polígonos del primer riel FBX pese a un round-trip válido en Blender. Se exportaron copias trianguladas y se verificó el número de triángulos en el importador real. El historial está en Incidente_Importacion_Resuelto.json. También se remapearon materiales por nombre y se verificó la asignación de hierro. Un verificador temporal falló al deserializar clases dinámicas con JsonUtility; se corrigió usando Newtonsoft ya incluido, sin instalar dependencias. Pipeline estuvo ausente brevemente al recargar scripts; se esperó ready en el mismo editor antes de continuar.

## Abrir, reproducir e invocar después

1. Abre Assets/Afterglow/Scenes/Art/M01_N01S02_Arte.unity y pulsa Play. Usa flechas y Espacio para recorrer la sección. La escena de producción no fue sustituida.
2. Para comparar el centro del tramo, avanza hasta x20. Capturas: Docs/Arte/N01S02_Antes.png y N01S02_Despues.png; recorrido final en Recorrido_Final.png. La guía de decisiones propuestas está en Docs/Arte/Guia_Visual_Mundo1.md.
3. Abre ArtSource/Blender/Afterglow_M01_MineKit.blend para editar los modelos. Para generar de nuevo, ejecuta Blender con `--background --factory-startup --python-exit-code 1 --python Tools/Art/generate_mine_kit.py`. Después ejecuta review_mine_kit.py. En Unity, abre la copia y usa Afterglow > Arte > Build N01S02 Sample. La regeneración reemplaza el grupo propio ART_N01S02 y sus recursos generados; no modifica el nivel de producción.
4. En próximas sesiones puedes pedir: «Usa $scenario-blender-expert para revisar este prop», «Usa $scenario-blender-hard-surface para modelar un soporte con bpy» y «Usa $scenario-blender-export para exportar y comprobar este FBX en Unity». Los nombres no son comandos de Unity ni paquetes UPM. `find-skills` sigue disponible para otras búsquedas.

## Límites de esta comprobación

Es una muestra de arte del tramo, no el arte definitivo del mundo completo. Las decisiones de paleta están propuestas para esta copia. No se cambiaron personajes 2D ni animaciones; los NPC provisionales no se ilustraron. No se construyeron otros mundos ni sistemas de gameplay nuevos. No se probó un build standalone, rendimiento objetivo, guardado de la escena de muestra ni un recorrido completo del juego. La prueba de iluminación es en tiempo real, sin bake ni postprocesado nuevo; no se cambió el renderizado del proyecto. No hay dependencia pendiente de login, pago o acción manual para usar este flujo.


## Comparación desde la cámara de juego

![Antes](<C:/Users/matia/Documents/Universidad/04_Cuarto Semestre/Game Development/Afterglow_blindingHope/Docs/Arte/N01S02_Antes.png>)

![Después](<C:/Users/matia/Documents/Universidad/04_Cuarto Semestre/Game Development/Afterglow_blindingHope/Docs/Arte/N01S02_Despues.png>)

