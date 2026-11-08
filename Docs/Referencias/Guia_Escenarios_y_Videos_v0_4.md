# Referencias espaciales y análisis de videos

## Qué estudiar

Las referencias N64/GameCube explican niveles formados por secciones con entradas/salidas distintas, suelo con profundidad, habitaciones amplias, plataformas elevadas y objetivos que permiten abrir otra puerta. La selección de niveles toma la idea de nodos y caminos de los mapas aportados. Son dos capas diferentes: selector entre niveles y puertas entre secciones.

No importar combate por turnos, mundo hecho de papel, plegados, habilidad de planeador ni transporte por tuberías a Afterglow. El diseño debe combinar composición propia con las reglas del GDD.

El arte de minas incluido es una propuesta visual. Los mecanismos ilustrados se resuelven con geometría y estados reales: una carretilla debe llegar a su placa por rieles transitables, una reja debe abrir sin atrapar y una puerta elevada debe poder alcanzarse con el salto básico. La especificación funcional tiene prioridad sobre detalles inconsistentes de una ilustración.

## Organización

| Material | Carpeta |
| --- | --- |
| Cámara y apariencia general ya aprobadas | Reference/General/Imagenes o Videos |
| Fuentes del personaje y animaciones | Reference/CharacterSource |
| Dormitorio, jardín y boceto original de la casa | Reference/Mundo 1/Imagenes/Prologo |
| Cámaras de mina y diagrama de conexiones | Reference/Mundo 1/Imagenes/Escenarios |
| Concepto y esquema funcional del selector | Reference/Mundo 1/Imagenes/Mapa_Seleccion |
| Gameplays de minas, carro y composición de M1 | Reference/Mundo 1/Videos |
| Fotogramas extraídos e informes del análisis | Docs/Referencias/Analisis/Mundo_1 |

## Gameplays largos

Se pueden aportar archivos largos si el agente tiene herramientas para acceder a ellos. No se presupone que pueda reproducirlos o comprender diez horas de video directamente. La alternativa verificable es consultar capítulos, extraer fotogramas y revisar fragmentos relevantes. FFmpeg permite buscar un instante y limitar los fotogramas exportados.

Preparar una fila por fragmento en `Reference/Mundo 1/Videos/INDICE_FRAGMENTOS_v0_4.csv`. La tabla empieza sin tiempos inventados. Añadir nombre de archivo exacto, título/juego, inicio y fin HH:MM:SS, objetivo y prioridad. Idealmente seleccionar de tres a cinco fragmentos de unos minutos que cubran: puerta lateral a cámara profunda; bifurcación alta/baja; puzle que abre salida; transición ida/vuelta; minijuego de carro. El prólogo no depende de completar esta investigación.

Para cada fragmento útil, extraer solo unas pocas imágenes representativas y registrar lo observado. Un fotograma estático permite estudiar composición; para afirmar cómo funciona un mecanismo o transición hacen falta estados antes/después u otro contexto que lo demuestre. No adivinar comportamiento solo por la imagen.

Ejemplo técnico para un archivo local, adaptando el ejecutable instalado y las rutas reales:

```text
ffmpeg -ss 00:12:30 -i "gameplay.mp4" -frames:v 1 "fotograma_001.png"
```

No extraer un fotograma por segundo de un gameplay de diez horas. Si los tiempos no están indicados, empezar por capítulos o una exploración acotada y refinarlos. No afirmar haber visto capítulos no examinados. Si falta la herramienta, el agente puede instalar una opción gratuita accesible o pedir exclusivamente los timestamps/capturas necesarios para ese bloqueo; el resto del encargo continúa.

## Registro de observaciones

Por cada fuente: archivo o URL; tramo o imagen consultada; observación comprobada; idea que se adapta; diferencia propia; sección de Afterglow a la que sirve. Guardar el informe en Docs/Referencias/Analisis/Mundo_1. No copiar geometría o assets de los juegos de referencia.

## Fuentes consultadas para orientar este paquete

- Nintendo, manual oficial de Paper Mario N64: https://www.nintendo.com/eu/media/downloads/games_8/emanuals/nintendo_8/Manual_Nintendo64_PaperMario_EN.pdf — movimiento libre en campo y transiciones entre pantallas. No se adopta su sistema de batalla.
- Nintendo, guía de The Thousand-Year Door: https://www.nintendo.com/au/news-and-articles/a-beginners-guide-to-paper-mario-the-thousand-year-door/ — exploración, regreso a lugares y pistas del compañero. La versión moderna no sustituye la investigación visual específica de GameCube.
- FFmpeg, documentación: https://ffmpeg.org/ffmpeg.html — búsqueda por tiempo y extracción de un número limitado de fotogramas.
- Fundación Patiño, Villa Albina: https://patino.org/es/lugares/villa-albina/ — arquitectura y entorno del prólogo.
- Los Tiempos, interiores de Villa Albina: https://www.lostiempos.com/oh/actualidad/20190506/villa-albina-abren-tesoro-mejor-guardado — referencias decorativas del dormitorio.

Este paquete no afirma haber analizado un gameplay completo. Los seis archivos del usuario y los conceptos sí se inspeccionaron al preparar la propuesta.
