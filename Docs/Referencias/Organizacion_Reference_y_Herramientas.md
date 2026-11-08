# Organización de Reference y preparación de herramientas

## Estructura

Reference/General/Imagenes y Reference/General/Videos contienen decisiones compartidas: cámara aprobada, HUD, menús, árbol y mapa general. Reference/CharacterSource conserva el material fuente del personaje. Reference/Mundo 1 hasta Mundo 6 mantienen Imagenes y Videos para escenarios, niveles, puzles y mecánicas locales. Un material general no se duplica en cada mundo: sus índices pueden enlazarlo con una ruta relativa.

Docs/Referencias/Analisis contiene informes técnicos sobre los materiales. No es un destino para scripts o prefabs del juego. El código, escenas, prefabs y sprites ya importados permanecen en Assets y conservan sus referencias y .meta. La autoría de Codex no determina la carpeta; la determina la función del archivo.

## Destinos de los archivos existentes

| Archivo o grupo observado | Destino desde la raíz del proyecto | Motivo |
| --- | --- | --- |
| Carpeta CharacterSource | Reference/CharacterSource/ | Conservar fuentes del personaje y su organización interna |
| Animaciones.zip | Reference/CharacterSource/Animaciones.zip | Archivo fuente para inspeccionar e importar selectivamente |
| PersonajeSeparado.png | Reference/CharacterSource/PersonajeSeparado.png | Fuente de las piezas del protagonista |
| Guia para la camara.mp4 | Reference/General/Videos/ | Explica una decisión compartida y ya aprobada |
| Video largo de Paper Mario The Thousand Year Door usado para cámara y movimiento | Reference/General/Videos/ | Referencia general; registrar fragmentos concretos |
| Screenshot 2026-10-30 042243.png del HUD | Reference/General/Imagenes/ | Referencia de interfaz compartida |
| VideoReferenceFrame.png | Reference/General/Imagenes/ si deriva del video general | Registrar el video y segundo de origen; si es un nivel local, usar Imagenes de ese mundo |
| VideoInspection.txt | Docs/Referencias/Analisis/VideoInspection.txt | Informe generado; actualizar las rutas mencionadas después de mover las referencias |
| Super Paper Mario Chapter 5-3 | Reference/Mundo 1/Videos/ | Mina, alturas, composición y plataformas |
| Donkey Kong Country Returns 1-6 Crazy Cart | Reference/Mundo 1/Videos/ | Minijuego de vagoneta del primer mundo |
| Mario and Luigi Superstar Saga Mysterious Mine Carts | Reference/Mundo 1/Videos/ | Recorrido de carritos del primer mundo |
| Donkey Kong Country Part 27 Mine Cart y Super Mario RPG de carritos | Reference/Mundo 1/Videos/ | Referencias locales de la vagoneta |
| Mario plus Rabbids Kingdom Battle Block Puzzles | Reference/Mundo 1/Videos/ o Imagenes/ | Puzles de carros locales |
| ZIP Afterglow_Documentacion_v0_3 | Fuera de Reference; conservar la descarga aparte | Descomprimir y copiar Docs, Reference, README y prompt a la raíz, junto a Assets |

Los nombres se conservan salvo que un renombrado ayude a identificarlos y se actualicen sus índices. No sobrescribir archivos del mismo nombre sin comparar su contenido. La captura permite identificar grupos, pero no demuestra el contenido de un ZIP, informe o video; el agente debe inspeccionarlos en el proyecto real.

La cámara actual ya está aprobada. Sus videos documentan ese comportamiento; no constituyen una tarea para rehacerla. El agente adapta niveles y plataformas a su sistema y conserva configuración, seguimiento, controles y encuadre. No instala una cámara alternativa como parte de una lista genérica de paquetes.

## Fuentes frente a recursos importados

CharacterSource es la fuente. Assets contiene la copia o importación que usa Unity. Inspeccionar Animaciones.zip y CharacterSource antes de importar; mantener clips, Animator, sprites y dependencias necesarios, incluidos .meta. No copiar ProjectSettings o Packages de un proyecto 2D encima del actual. No mover fuera de Assets recursos que las escenas ya referencian.

Si Codex generó una captura de juego o un fotograma de análisis, se guarda como referencia general o del mundo pertinente. Si generó un informe, se guarda en Analisis. Si creó un script, material o prefab que usa el juego, permanece en la estructura real de Assets. Los documentos del agente nunca sustituyen una prueba en el editor.

## Evaluación antes de programar

Leer el GDD, diseño, guion, checklist, índices generales y del mundo uno; después inspeccionar el proyecto y producir Plan_Herramientas_y_Assets.md. El informe se presenta al usuario antes de escribir sistemas nuevos. Se distinguen plugins de Codex o Claude, paquetes de Unity, herramientas del editor y assets de contenido: instalar un plugin del agente no importa automáticamente sprites o modelos al juego.

Para cada candidato registrar utilidad concreta, necesidad, fuente original, versión compatible, dependencias, licencia, coste y método. Empezar por lo que ya existe, especialmente integración Unity, cámara y animaciones. Evaluar herramientas para geometría de bloques, edición o reutilización de sprites, interfaz, diálogo y audio solo si aportan algo al prototipo. No hace falta instalar un paquete para cada categoría.

La autorización cubre descargar e instalar opciones gratuitas compatibles que el entorno permita y cuya licencia sea aplicable. No cubre compras ni suscripciones. Preferir paquetes oficiales, repositorios originales y assets con origen y licencia identificables. Usar versiones verificadas con el proyecto, no asumir que la última es compatible. Importar solo lo que se vaya a usar, preservando materiales fuente.

Si puede instalarlo, el agente lo hace y comprueba importación, compilación y que no se modificó la cámara. Si no puede, informa antes de codificar: nombre exacto, enlace oficial, utilidad, coste verificado, pasos manuales, destino y comprobación posterior. No solicita contraseñas en el chat. La adquisición en una cuenta, autenticación o una interfaz no disponible pueden requerir intervención del usuario; se informa del bloqueo observado, no de una suposición.

Solo una dependencia indispensable impide su parte del trabajo. Para opciones decorativas o de comodidad se continúa con figuras y geometría provisionales. Después de presentar el informe no se solicita otra confirmación para cada instalación gratuita ya autorizada que sea técnicamente posible.

## Documentación consultada

OpenAI Docs sobre permisos de archivos y red: https://learn.chatgpt.com/docs/permissions

Unity sobre fuentes de paquetes: https://docs.unity.com/en-us/engine/6000.6/manual/packages-list/packages/upm-concepts

Unity sobre importación desde Asset Store: https://docs.unity.com/en-us/asset-store/downloads/asset-store-packages

Plugin oficial de Unity para agentes: https://github.com/Unity-Technologies/unity-agent-plugin

Estas fuentes explican mecanismos; no certifican las versiones del proyecto real ni que el agente tenga acceso a una cuenta o descarga concreta. La comprobación se realiza en el diagnóstico local.
