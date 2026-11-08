# Método de diseño de niveles

Aplicar junto con Mundo_1_Canon.md y el GDD vigente. Un nivel se diseña desde sus situaciones de juego y su recorrido. La malla y la decoración materializan ese diseño.

## 1 Leer la función antes de dibujar

Para el nivel actual definir identidad, objetivo, acciones disponibles, conocimientos previos, qué enseña o desarrolla y qué debe sentir el jugador. Para N1: aprendizaje breve, seguridad para probar y una combinación final sencilla. No reutilizar una plantilla de mina ni copiar una de las tres ilustraciones generadas.

Medir en Unity el personaje real: tamaño, salto, distancia segura, ancho de paso, pendientes, velocidad y profundidad visible. Conservar la cámara aprobada. Una cámara de nivel puede ser más grande que la pantalla y recorrerse con el seguimiento existente; no obliga a cambiarlo ni a mostrar todo a la vez.

## 2 Analizar las referencias por separado

Revisar los once mapas de General/Imagenes/MapasNiveles. Recortar a resolución original las regiones de los atlas grandes; no limitarse a una miniatura de la imagen entera. Registrar archivo, zona o coordenadas, observación visible, principio que interesa, adaptación propia y limitación. Distinguir atlas con geometría de esquemas de conexiones. No inferir la solución de un puzle por su dibujo si no está demostrada.

Estudiar topología global y composición local: proporciones del suelo, vacíos, plataformas, profundidad, desniveles, relaciones entre puertas y objetivos, hitos, zonas seguras y ramales. La paleta común de un mapa no impide que sus salas tengan funciones distintas. No adoptar capacidades de Mario y Luigi que el protagonista de Afterglow no posee.

## 3 Diseñar el recorrido completo

Proponer dos estructuras breves y realmente distintas para N1. Compararlas por claridad de tutorial, uso del espacio, ritmo, retorno y coste de implementación. Elegir una con una explicación breve y continuar sin pedir aprobación por cámara. No presentar dos variantes de la misma sala con puertas cambiadas.

Dibujar un plano de conjunto y una red de secciones: ruta principal, ramales, puertas, apariciones, retos, hitos y final. Una arista representa una transición; no colocar cámaras contiguas físicamente solo porque el atlas de referencia las dibuja así. Las relaciones espaciales y el retorno deben poder comprenderse.

La cantidad de cámaras resulta de ese recorrido. Un corte de escena se justifica por una intención, cambio de composición o necesidad técnica. El tamaño se justifica por acciones y decisiones; no por alargar una caminata. Se permite repetición con propósito de aprendizaje y aumento de complejidad, conservando identidad propia.

## 4 Ficha funcional por cámara

Cada cámara final tiene ID, papel en el nivel, forma y dimensiones aproximadas, entrada/salida y puntos seguros, acción predominante, secuencia de decisiones, reto, ruta principal y opcional, recompensa, condiciones y estado persistente. Añadir motivo para su geometría y diferencia respecto de sus vecinas. Separar X lateral, Z profundidad y Y altura.

Explicar qué ve primero el jugador, qué identifica como objetivo, por qué el camino directo está condicionado, qué acción resuelve la situación y qué cambia al resolverla. El desafío puede ser movimiento, observación, combate, manipulación o combinación; no colocar los cinco de forma automática en todas las habitaciones.

Una cámara amplia puede alojar aproximación, reto y resolución sin puertas entre cada acción. Una cámara pequeña puede cumplir una función breve y aportar un cambio de ritmo. No usar un suelo rectangular vacío como base fija para todos los montajes. Vacíos, bordes, masas rocosas y plataformas deben formar el recorrido, no ser adornos puestos encima.

## 5 Seleccionar mecánicas ambientales

Se permiten palancas, puentes con contrapesos, placas y carros, plataformas móviles sencillas y recursos de iluminación del entorno. Elegir lo mínimo que mejore el reto. No imponer todas estas ideas ni convertir cada cámara en un interruptor que abre la puerta.

Para N1, introducir una interacción ambiental breve y visible: por ejemplo una palanca que mueve un puente. Es un ejemplo sustituible por una solución original con igual claridad. Mostrar la causa y el efecto; después combinarla suavemente con un movimiento ya aprendido. Las aplicaciones extensas corresponden a N2 y N4.

Cada mecanismo nuevo define activación, efecto, retorno o reinicio, advertencia, interacción con el jugador, persistencia y conducta al morir/cargar. No introducir doble salto, gancho libre, escalada, nueva arma o consumible obligatorio para justificar un diseño. La base debe ser completable con acciones existentes.

## 6 Construcción en Blender y validación en Unity

Blender es la herramienta principal para modelar las cámaras y piezas del entorno. Conservar fuentes .blend editables y, si se usa automatización, los scripts de generación con parámetros comprensibles. Organizar por cámara y por función: suelo, roca, soportes, decoración y mecanismos. Mantener las partes interactivas separadas; no exportar una única malla que una suelo, puerta, roca rompible y carro.

Guardar fuentes fuera de Assets, siguiendo la organización real del proyecto, y exportar las piezas necesarias como FBX a la zona de arte existente. Verificar escala, orientación, pivotes, normales y materiales con una prueba de ida y vuelta. No asumir que nodos de material o lógica de Blender se convierten en comportamiento de Unity.

Primero un montaje geométrico sencillo en Blender; exportar, montar y probar en Unity con controlador y cámara reales. Corregir espacio y distancias antes del acabado. Después modelar siluetas, materiales, UV cuando corresponda y piezas de ambientación. Unity integra colliders sencillos, prefabs, portales, enemigos, NPC, luces, estados, guardado y la lógica del puzle.

Un render de Blender o un archivo FBX no demuestra que el nivel sea jugable. Probar la ruta normal, un regreso, la caída, puertas, mecanismo y persistencia. No considerar terminado un pase de arte que deje rutas invisibles o aterrizajes ocultos. Los actores permanecen 2D.

## 7 Evaluar variedad y aprendizaje

Comparar el plano de las cámaras sin decoración: forma del suelo transitable, reparto de alturas, secuencia de acciones, decisiones, visibilidad y función en el nivel. Si solo cambian orientación, recursos gráficos o posición de puertas, revisar el diseño. Reutilizar piezas y materiales es válido; reutilizar sin adaptación la misma situación es lo que hay que corregir.

No exigir que todas las cámaras sean totalmente inéditas. Una repetición breve puede consolidar una acción si cambia de forma significativa la exigencia o su combinación. La variedad debe mejorar orientación y ritmo, no crear complejidad arbitraria.

En N1 comprobar que cada acción se entiende antes de exigirla bajo presión, que el enemigo no ataca antes de entregar el pico y que las rutas obligatorias no necesitan habilidades del árbol. El primer recorrido debe enseñar a investigar y volver sin recurrir a instrucciones de depuración.

## 8 Archivos de producción

Mantener un Plan_Nivel1.md que reúna decisión de estructura, red de cámaras, fichas, mecanismos, fuentes y justificación del tamaño/cantidad. Añadir un plano editable SVG o equivalente y el registro de pruebas en Estado_Trabajo.md. Reutilizar archivos equivalentes si ya existen; no crear una colección paralela de documentos que nadie actualice.

Al cerrar, aplicar Protocolo_Documentacion.md para sincronizar GDD, guion, checklist, mapa y datos. El registro debe distinguir lo propuesto, modelado, importado y verificado en Play Mode.


El paquete incluye Tools/Documentacion/Generar_GDD.py para regenerar Word desde Markdown y Validar_Canon.py para comprobar orden, cuotas y referencias principales. La comprobación documental no reemplaza pruebas en Play Mode. Actualizar también estos verificadores cuando el diseñador cambie de nuevo el canon.
