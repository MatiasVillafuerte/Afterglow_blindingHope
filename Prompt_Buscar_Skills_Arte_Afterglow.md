# Prompt para buscar instalar y comprobar skills de arte en Afterglow

Usa la skill de búsqueda de skills que tengo activada en esta sesión; identifica su nombre exacto. Quiero que busques, selecciones, instales y compruebes las capacidades más útiles para mejorar visualmente mi juego. No termines solo con una lista de enlaces: deja un conjunto pequeño de capacidades utilizables y demuestra qué pueden ejecutar en mi proyecto real.

## Contexto y objetivo

El juego es Afterglow: Blinding Hope, una aventura de plataformas, puzles y combate en tiempo real, con personajes y enemigos 2D dentro de escenarios 3D. Tiene seis mundos mineros con identidad propia. La producción actual se concentra en las minas bolivianas del mundo uno.

Ya hay un prototipo funcional construido principalmente con figuras básicas. Esta etapa autoriza mejorar su arte: modelado de entorno, composición, materiales, iluminación y acabado de una muestra. El GDD sigue definiendo historia y jugabilidad; la mejora visual debe respetarlas. La cámara actual está aprobada: conserva su configuración, seguimiento y controles. Conserva también los personajes 2D y las animaciones existentes.

## Primero lee e inspecciona

Lee las instrucciones AGENTS.md aplicables y estos documentos, si existen en la raíz del proyecto:

- Docs/GDD/Afterglow_GDD_v0_3.md.
- Docs/Diseno/Mundo_1_Diseno.md y Docs/Guion/Mundo_1_Guion.md.
- Docs/Implementacion/Estado_Trabajo.md, Checklist_Mundo_1.md y Plan_Herramientas_y_Assets.md.
- Docs/Referencias/Organizacion_Reference_y_Herramientas.md.
- Reference/General/INDICE_GENERAL.md y Reference/Mundo 1/INDICE_REFERENCIAS.md, junto con las imágenes y fragmentos de video disponibles.
- Reference/CharacterSource, para comprender qué recursos del protagonista ya están disponibles.

Inspecciona Unity, URP, paquetes, escenas, scripts, integración del agente, permisos de ejecución y disponibilidad real de Blender. No presupongas conexión al editor por tener un plugin instalado ni la disponibilidad de Blender por instalar una skill. Determina qué puede explicar una skill y qué puede ejecutar realmente mediante scripts, CLI, API o MCP.

## Capacidades prioritarias

### 1 Modelado de escenarios y props

Es la prioridad principal. Busca flujos que permitan crear mallas editables, variaciones, UV, materiales, escala y exportación. Deben servir para rocas, paredes de mina, soportes de madera, entradas, rieles, carretillas y detalles reutilizables.

Compara dos opciones: modelado mediante Python bpy en Blender, con exportación a Unity; y geometría editable o procedural dentro de Unity, incluida la posibilidad de usar ProBuilder. Elige la combinación más sencilla que pueda ejecutarse y verificarse con las herramientas disponibles. Una integración MCP de Blender es una posibilidad, no un requisito si la ejecución por Python y línea de comandos ya funciona.

Busca con términos como blender python bpy procedural modeling, stylized environment, modular environment kit, game ready assets, Unity ProBuilder scripting y Blender Unity export. Prefiere capacidades que produzcan archivos de modelos y prefabs reales. Una skill que solo explique C# de gameplay no cubre esta prioridad.

### 2 Diseño de niveles y dirección de arte

Busca capacidades que conviertan el GDD, el plano del nivel y las referencias en decisiones aplicables: composición, jerarquía visual, siluetas, materiales, paleta, contraste entre planos, puntos de interés y distribución de props. Deben trabajar con la cámara aprobada, las alturas y las rutas jugables existentes.

Para el mundo uno deben integrar la mina estilizada y caricaturesca, sus rieles, madera, rocas, puzles y atmósfera. Combina principios de las referencias de Super Paper Mario, las vagonetas de Donkey Kong y Mario y los Block Puzzles de Mario + Rabbids, creando composiciones propias. No copies habitaciones completas ni inventes mecánicas o historia para justificar el arte.

Consulta los temas de los demás mundos para que el flujo pueda reutilizarse después, sin construir ahora los mundos dos al seis. Si no existe una skill adecuada de dirección de arte, registra la carencia y crea una guía visual del mundo uno como documento del proyecto; no inventes una skill existente ni lo presentes como una instalación.

### 3 Materiales iluminación y acabado en Unity

Busca capacidades de environment art, set dressing, materiales compatibles con la URP instalada, texturas estilizadas, iluminación, contraste y revisión visual desde la cámara de juego. Shader Graph es opcional si hay una necesidad concreta; no es obligatorio crear shaders nuevos.

La mejora debe notarse en formas, superficies, composición y luz. Pintar de colores los mismos cubos no basta para demostrar modelado. Mantén legibles personajes 2D, saltos, bordes, puertas, objetos y avisos enemigos. No impongas fotorrealismo ni sacrifiques jugabilidad por decoración.

### 4 Integración y comprobación visual

Necesito inspeccionar y modificar escenas, importar modelos, configurar materiales y colisiones, crear prefabs y capturar resultados desde Unity. Comprueba primero lo que aporta el plugin oficial de Unity ya instalado. Evita instalar otro puente redundante si el existente funciona.

## Cómo seleccionar e instalar

Busca primero entre lo instalado y en fuentes oficiales o repositorios originales verificables. Usa la herramienta de búsqueda, pero abre y lee el SKILL.md y sus recursos antes de recomendar. Si su catálogo no cubre modelado, amplía la búsqueda a fuentes originales y documentación de Unity o Blender; no repitas indefinidamente la misma consulta genérica.

Selecciona como máximo cinco skills principales, pudiendo usar menos. Las integraciones indispensables se identifican aparte. Para cada candidata registra: nombre exacto, enlace original, función, tipo de capacidad, scripts o herramientas que utiliza, requisito de conexión, compatibilidad con Windows y versiones reales, licencia, coste, limitaciones y criterio de elección. Distingue skill, plugin del agente, paquete de Unity, integración MCP y asset.

La autorización cubre instalar skills gratuitas compatibles y las herramientas gratuitas justificadas necesarias para utilizarlas, siempre que tu entorno lo permita. No compres assets ni actives servicios de pago. No actualices Unity ni sustituyas cámara, input o renderizado para hacer funcionar una candidata. Evita instalar bibliotecas enteras cuando solo necesitas una capacidad, y no reinstales lo que ya funciona.

Antes de instalar, presenta un resumen breve de la selección y la vía elegida para modelar. Después continúa con las instalaciones gratuitas autorizadas sin esperar otra confirmación rutinaria. Si una requiere intervención manual, identifica el bloqueo observado, enlace, coste verificado y pasos exactos. Continúa con las opciones independientes cuando sea posible. Si ninguna candidata sirve, explica la búsqueda realizada y ofrece un flujo ejecutable con las herramientas existentes, sin fabricar nombres de skills.

## Prueba práctica obligatoria

Tras instalar, crea una escena de prueba separada o una copia de trabajo de un pequeño tramo del nivel uno. Usa una localización identificada del diseño, no una escena genérica desconectada del juego. La cámara de producción permanece intacta; la copia utiliza sus mismos parámetros para evaluar el arte.

1. Captura el estado inicial e identifica las habilidades y colisiones que deben conservarse.
2. Escribe una guía visual breve con paleta, materiales, siluetas, luz y referencias aplicadas. Las decisiones visuales aún no fijadas se presentan como propuestas para esa muestra.
3. Genera tres variantes de roca, un soporte de madera y un módulo de riel, mediante mallas editables. Reutilízalos para dar identidad al tramo.
4. Si utilizas Blender, entrega la fuente .blend en una carpeta de fuentes y exportaciones compatibles con el importador real de Unity. Si utilizas Unity, guarda mallas y prefabs editables. No presupongas que los nodos de materiales de Blender se transfieren automáticamente a URP.
5. Integra materiales y luz, conserva el volumen jugable y usa colisiones sencillas. Comprueba escala, normales, UV cuando correspondan y referencias de materiales.
6. Prueba movimiento, salto y recorrido en Play Mode, y captura el resultado desde el mismo encuadre inicial. Señala cualquier comprobación que no puedas ejecutar.

La prueba demuestra que las capacidades instaladas producen una mejora visible. No exige rehacer todos los niveles, crear personajes 3D ni terminar el arte definitivo. Los recursos provisionales de NPC no se convierten automáticamente en un encargo de ilustración o animación.

## Entrega de esta tarea

Deja un informe con la selección final, las descartadas relevantes, instalación y versiones, dependencias, pruebas y limitaciones. Separa Instalado de Instalado y probado. Guarda el informe en Docs/Implementacion/Seleccion_Skills_Arte.md y la guía en Docs/Arte/Guia_Visual_Mundo1.md, reutilizando documentos equivalentes si ya existen.

Enumera los modelos, materiales, prefabs, scripts de generación y escenas realmente producidos. Incluye las capturas de antes y después y cómo abrir la muestra. Explica cómo invocar cada skill en las siguientes sesiones. Actualiza Estado_Trabajo.md con los resultados reales, sin sobrescribir avances anteriores ni marcar tareas de gameplay que no hayas probado.

No cierres esta tarea con consejos de diseño únicamente. El resultado esperado es una selección mínima instalada cuando sea posible y una muestra visual comprobable, o el bloqueo concreto que impide ejecutarla con sus pasos de resolución.
