# Mundo uno Diseño de niveles y datos de producción

Versión 0.5. Aplicar junto con Docs/GDD/Afterglow_GDD_v0_5.md y Docs/Guion/Mundo_1_Guion.md. Este documento define contenido del mundo uno; las cantidades son valores iniciales ajustables mediante datos. Los estados narrativos y sus condiciones permanecen obligatorios.

## 1 Identificadores y escala

M01 identifica el mundo y M01N01–M01N06 sus niveles en el orden de Mundo_1_Canon.md. Q01–Q11 son misiones y sus IDs conservan el significado. El guion conserva sus eventos por ID. Las secciones y sus puertas reciben IDs persistentes cuando se aprueba y construye la topología; su cantidad no está fijada por este documento.

Las antiguas referencias S01, S02, etc. se tratan como anclas de contenido al migrar el proyecto. Se pueden fusionar o dividir cámaras conservando los eventos, las misiones y las fuentes. Mantener una tabla entre las ubicaciones anteriores y las nuevas. Nunca borrar una recompensa cobrada ni repetir una introducción por cambiar la geometría.

La escala inicial es aproximadamente 1 unidad Unity por metro, velocidad base de referencia 5 m/s y salto de referencia 2 m. Antes de diseñar medir el controlador real y su margen de salto, colisión, pendientes y profundidad visible con la cámara aprobada. Las rutas obligatorias usan acciones básicas. No imponer una longitud o duración artificial para llenar espacio.

## 2 Prólogo pueblo y acceso seguro

PRO_CASA contiene carta, escritorio, cama, zona inicial y puerta directa al jardín. PRO_PATIO es el recinto rectangular vallado con mansión al fondo, fuente central rodeable, vegetación y portón. El capitán está fuera y se habla desde dentro. Conservar P01–P02 y las reglas de Prologo_Mapa_y_Escenarios_v0_5.md. El rediseño de N1 no modifica esta entrega ya existente.

El capitán conduce a M01_MAPA. N01S01 es el acceso seguro conectado al pueblo, con un único altar y el regreso del teléfono. La salida física al mapa abre sus seis nodos. M01_ACCESO queda como escena legada fuera del arranque. El teléfono no aporta ofrendas ni abre automáticamente el mapa. La entrada al recorrido de N1 inicia su expedición. Las puertas internas no abren el selector.

## 3 Identidad y contenido de los seis niveles

### M01N01 Entrada de la mina

Tutorial corto, conciso y completo en sus bases. Debe introducir movimiento X/Z, saltos y subidas en Y, lectura de caminos, enemigo con ataque anunciado, pico, roca agrietada, un mecanismo ambiental sencillo y el circuito de ofrendas, investigación, teléfono y altar. Las acciones se presentan en situaciones seguras y después se combinan brevemente. No convertir N1 en seis minijuegos ni enseñar por completo el laberinto, la oscuridad o la vagoneta.

Anclas obligatorias de contenido, que pueden repartirse en un número variable de cámaras:

| Ancla | Contenido que se conserva | Verificación |
| --- | --- | --- |
| Acceso y orientación | N1A, altar seguro, hijo y regreso | Un solo altar e introducción, rutas comprensibles |
| Movimiento | Piso que permita usar profundidad, cambios de dirección, salto seguro y subida | La forma exige movimientos distintos sin cambiar cámara |
| Pico y combate | N1B, primera resistencia y roca agrietada | Pico antes de agresión, una entrega, enemigo y roca con lectura distinta |
| Entorno interactivo | Mecanismo breve de causa y efecto | Accionar o desplazar modifica físicamente el camino; no es decoración |
| Exploración opcional | Ramal de Q02 y recursos | Se distingue del camino principal y permite volver |
| Registro final | Trabajadores, N1D y cuota 3 | Investigación solo al interactuar y N2 solo con investigación más cuota |

N1C entrega el teléfono una vez y explica el altar con las teclas reales. La primera resistencia deja una ofrenda legítima garantizada como demostración. Hay dos ofrendas estáticas y recompensas de Q01–Q02: al menos cuatro legítimas garantizadas en la expedición, con tres necesarias. No obligar a completar ambas misiones ni a comprar al vendedor. Mantener Q01 fuera de la ruta obligatoria para que el tutorial no exija una sesión de combate repetitiva.

La topología se diseña desde estas funciones. Puede haber un espacio amplio con varias situaciones y transiciones breves, o varias cámaras compactas con razones para separarlas. Una transición no debe existir solo para interrumpir un recorrido que funciona mejor continuo. El plano final se documenta después de elegirlo y probarlo; los conceptos ilustrados no fijan su forma.

### M01N02 Rieles cruzados

Exploración semejante a N1 en su base, mucho más amplia y diversa. Combina desniveles, suelo profundo, cruces, ramales altos y bajos, enemigos y varias situaciones ambientales. Los carros, placas y topes son parte del vocabulario, no la única estructura de todas sus salas. Q03–Q04 conservan colocación de carros y recuperación de herramientas.

Introducir un problema, variarlo y combinarlo con otro ya conocido. Ejemplos disponibles: carro que mantiene una placa, palanca que mueve un puente o plataforma que conecta alturas. Elegir solo los que aporten decisiones al recorrido. Los rieles tienen topes; el reinicio restablece el puzle, no recompensas ni enemigos. Una reja no encierra al jugador en su collider. Hay al menos seis ofrendas legítimas garantizadas; investigación y aporte 5 abren N3.

**Construcción del 8 de noviembre.** El plano editable es `Docs/Diseno/Niveles/N2_Plano.json`, fuente única para Blender (`build_level.py`) y Unity (`M01LevelBuilder`). Tiene cinco secciones:

- **N02S01 · Andén de los rieles.** Centro del nivel. La torre de poleas que se ve sobre el muro marca el destino final. Platicio orienta y entrega su regalo único. Q03 lo da el Capataz Ramos y Q04 la Minera Quispe; el Minero Ticona conversa por humor. Unas cajas suben a la cornisa del desvío.
- **N02S02 · Cruce de rieles.** El carro empujado hasta su placa abre la reja del pasaje, que queda abierta para siempre. El carro marcado de Q03 se engancha en su apartadero. La palanca de topes devuelve los carros. Hay dos Resisworm y una cornisa con ofrenda.
- **N02S03 · Patio del farol.** Dos carriles en profundidad. Hangzzer cuelga del farol central, baja, silba y llama hasta 2 Resisworm sin botín desde el túnel del fondo; solo es vulnerable abajo. El segundo carro de Q03 está en el carril alto y una torre de cajas guarda una ofrenda.
- **N02S04 · Desvío alto (opcional).** Terrazas de 0,65 m y un hueco jugable de 1,5 m. Tres rocas agrietadas (las columnas lisas no se rompen) tapan la caja de Q04 y una ofrenda.
- **N02S05 · Galería de la polea.** Un puente de contrapeso (mecánica conocida) cruza la garganta. Peldaños de 0,65 m llevan a la cornisa del derrumbe, donde el minero registra la investigación (N2.04, N2D.01–02).

Hay seis ofrendas estáticas garantizadas, más 2 de Q03, 1 de Q04, el regalo eventual de Platicio y el botín. El grafo de puertas y las pulsaciones del ritmo están en el mismo JSON.

### M01N03 Galerías de las señales

Laberinto con cámaras de acertijo de cuatro puertas: regreso, continuación principal y dos alternativas. La pista de cada acertijo identifica una continuación principal de forma fiable. Puede usar una relación entre señales, lámparas o rieles que se explique en el propio nivel, sin adivinación ni información externa.

| Tipo de sección | Función | Estado y conexión |
| --- | --- | --- |
| Entrada | Presentar el código de pistas y el punto de rescate Q05 | El minero escoltado se entrega aquí caminando |
| Acertijo de cuatro puertas | Observar y elegir la continuación | Error ordinario vuelve a esa misma cámara lógica |
| Confirmación de progreso | Cambiar composición y mostrar un hito reconocible | Conecta una tanda de acertijos con otra |
| Ramal especial | Tesoro, minero perdido o encuentro de enemigos | Vuelve a la cámara de origen; no completa el avance principal |
| Final | Localizar el bloqueo y hablar con trabajador | Investigación más aporte 7 abren N4 |

La cantidad y alternancia se justifican por el ritmo, sin asignar un total fijo de cámaras. No repetir un mismo cuarto con puertas permutadas en todo el nivel. Las cámaras de confirmación aportan plataformas, descanso o un encuentro breve, conservando la lectura de avance. Los ramales especiales son excepciones explícitas al bucle erróneo; la ruta principal siempre responde a su pista.

Un error no repone enemigos, regalos, tesoros ni recursos, no cambia la solución ni regresa al acceso. La puerta de regreso se distingue. Q05 sigue por puertas y se completa solo al volver caminando al inicio de N3; T está bloqueado durante escolta. Q06 afecta señales secundarias y no vuelve mentirosa la ruta obligatoria. Hay al menos ocho ofrendas legítimas garantizadas.

### M01N04 Galería sin luz

Exploración amplia con plataformas, subidas, enemigos y puzles, cuya condición principal es la iluminación baja. El jugador debe buscar medios para reconocer y recorrer el espacio: lámparas fijas, un carro con luz o mecanismos ambientales sencillos que permitan iluminar una zona. La luz tiene una función de navegación y preparación del camino, no solo un filtro oscuro.

Contenido: aproximación legible, descenso o profundidad, cruce con luz móvil, refugio Q09, encuentro Q10 y ascenso o recorrido final. La cantidad de cámaras es variable. El carro con lámpara no consume combustible y su reinicio no borra misiones completadas. No ocultar suelo inmediato, bordes letales, aterrizajes ni avisos de ataque. Debe poder orientarse con la cámara existente.

Q09 conserva el encargo de llevar luz al refugio y Q10 el de despejar enemigos del cruce. Hay al menos diez ofrendas legítimas garantizadas. Investigación y aporte 9 abren N5. El final anuncia la vía suspendida como continuación; todavía no confirma la puerta exacta de los atrapados.

### M01N05 La vía suspendida

La identidad es el minijuego de vagoneta. Aproximación a pie y estación son breves; el cuerpo del nivel es subir al carro, anticipar obstáculos y evitar caer. El andén final devuelve el control normal y permite investigar el final. Q07 es el encargo de rocas del acceso y Q08 el paquete del recorrido.

Vía diseñada con segmentos legibles y descansos, sin copiar el orden de obstáculos de un video. Parámetros iniciales: 7 m/s, Espacio salta y Abajo frena al 70 por ciento durante un máximo de 1,5 s con 2 s de recuperación. Confirmar estos valores con el controlador específico del minijuego. No exige habilidades compradas ni ataque de pico. Durante la conducción no se usa teléfono, cambio de personaje ni guardado. Pausa congela la acción; el menú mantiene las restricciones existentes.

Secuencia de referencia funcional: aprendizaje seguro, obstáculo aislado, hueco anunciado, recogibles que sugieren un arco, variación con descanso y llegada al andén. El diseño concreto debe ser propio y puede cambiar. Los objetos de progreso necesarios se sitúan antes o después de la vía o en posiciones seguras; no exigir un salto opcional difícil para pagar el altar.

En N5, un minero explica el objetivo y pide aceptación. Los minerales suspendidos se recogen por contacto. Una caída reinicia el recorrido desde el inicio. Al llegar, el minero cuenta los minerales del intento y pide repetir si faltan. Con éxito entrega directamente las ofrendas necesarias para la cuota de N5. El minero de la puerta bloqueada registra la investigación al conversar tras el éxito. Se regresa a N01S01 para entregar al Tío (cuota 12) y abrir N6. Reintentos y cargas conservan el progreso permanente sin duplicar recompensa. El umbral de minerales queda por definir.

### M01N06 Cámara del derrumbe

Nivel del jefe. Conservar corredor con voces N6A, puerta de rescate, celebración y aparición del taladro, arena circular y cierre. El contraste entre pasillo narrativo y arena es deliberado. No añadir un laberinto, minijuego o cuota extra para aumentar su número de cámaras.

La arena tiene radio inicial de 10 m, paredes continuas, altar de batalla independiente y cornisa segura para el hijo. El mismo lugar permite caminar, inspeccionar la pieza, hablar y entrar en la vagoneta de viaje después de ganar. No se reinicia el jefe tras la victoria.

## 4 Misiones

| ID y nivel | Objetivo después de aceptar | Repetición | Recompensa |
| --- | --- | --- | --- |
| Q01 N01 | Derrotar 3 resistencias del área marcada | Nueva expedición voluntaria | 1 ofrenda y 5 dinero |
| Q02 N01 | Recuperar casco del ramal y entregarlo | Única | 1 ofrenda y 8 dinero |
| Q03 N02 | Colocar 2 carros marcados y avisar | Única | 2 ofrendas |
| Q04 N02 | Abrir 3 rocas opcionales y devolver caja | Única | 1 ofrenda y 10 dinero |
| Q05 N03 | Llevar minero perdido a la entrada de N3 caminando | Única | 2 ofrendas y 10 dinero |
| Q06 N03 | Corregir 3 placas del ramal opcional | Única | 1 ofrenda; placas permanecen corregidas |
| Q07 N05 | Romper 4 rocas marcadas en acceso | Nueva expedición voluntaria | 1 ofrenda y 10 dinero |
| Q08 N05 | Recoger paquete de la vía y entregarlo al andén | Única | 2 ofrendas y 1 Fragmento pulido |
| Q09 N04 | Llevar carro de luz al refugio indicado | Única | 2 ofrendas |
| Q10 N04 | Derrotar 4 componentes del cruce | Nueva expedición voluntaria | 2 ofrendas y 12 dinero |
| Q11 Pueblo | Tras descubrir fraude, recuperar caja escondida y devolverla a su dueño | Única opcional | 8 dinero y aclaración narrativa |

Las bajas anteriores a aceptar Q01 o Q10 no cuentan. Los objetos únicos de misión pueden existir antes de aceptar y su entrega los reconoce, sin duplicarlos. Q06 solo corrige carteles secundarios; la ruta obligatoria nunca exige adivinar una mentira. El paquete Q08 permanece en inventario si se recoge y luego se falla el minijuego.

Estados de encargo: no ofrecido, disponible, activo, objetivo cumplido y entregado. Objetivo cumplido no entrega automáticamente la recompensa de un encargo que exige volver al NPC. Morir conserva progreso y entregas; una escolta activa es una excepción de posición y debe retomarse desde el NPC, sin borrar objetos ni otras misiones. Un encargo repetible entregado solo crea un nuevo intento al iniciar otra expedición voluntaria. Nunca vuelve a pagar por las mismas bajas.

Q11 se habilita al rechazar al menos una compra robada en el altar. El habitante conserva su nombre público y puede ofrecer un rumor útil antes o después. No roba dinero o inventario mediante un evento invisible. La caja no aparece como activo si no se ha generado la misión; el reembolso fijo no depende de comprar repetidas veces.

## 5 Datos de combate y experiencia

Resisworm (N1), Hangzzer (N2), Gasplode y Drilly (N3) y Lighmet (N4) usan los valores del catálogo 7.2 del GDD. Los enemigos persiguen solo dentro de sus áreas transitables; su movilidad no aumenta al cambiar de nivel. Ninguno necesario para avanzar exige una habilidad comprable. Daño se registra por ID de acción y objetivo para impedir golpes múltiples del mismo collider. Implementación del 8 de noviembre: el pico y las mordidas usan un volumen 3D compartido de 1,2 m × 0,8 m × 1,6 m contra los colliders reales (F9 lo muestra). El Resisworm espera a 1,05 m, dentro del alcance del pico, y tiene 2 de vida. La espera y la altura del volumen son parámetros de prueba ajustables.

| Zona | Referencia de enemigo | EXP base inicial | Dinero por enemigo inicial |
| --- | --- | --- | --- |
| N01 | 1 | 3 | 2 |
| N02 | 2 | 5 | 3 |
| N03 | 2 | 6 | 3 |
| N04 galería oscura | 3 | 9 | 4 |
| N05 acceso y andén | 3 | 7 | 4 |
| Taladro vencido | No escala | 100 una sola vez | 20 una sola vez |

EXP normal = max(0, baseEXP − max(0, nivelEquipo − referencia)). La recogida aplica el valor calculado al derrotar al enemigo, guardado en el orbe; no cambia retroactivamente si se sube de nivel antes de recogerlo. No se crea un orbe con valor cero. Los tres recursos son dinero, puntos de EXP y gemas de habilidades, con usos separados; los dos recursos gastables principales son dinero y gemas.

Cada umbral de 100 concede 3 gemas, aumenta el contador histórico del equipo en 3 y acredita 3 a cada personaje desbloqueado. El hijo recibe ese histórico al incorporarse una sola vez. Vida actual, ramas y saldos son individuales. La subida cura al activo. El total histórico nunca incluye devoluciones ni regalos de dinero.

Las ofrendas son equiprobables entre los tipos permitidos para una fuente, excepto vendedor, que solo usa alcohol y cigarro. Los regalos del hijo se evalúan una vez por nivel del mundo, incluso si no hay regalo. Las fuentes estáticas garantizadas se anotan con IDs en cada sección para medir disponibilidad y no depender de una simulación aleatoria.

## 6 Transacciones del altar

Para el próximo paso, faltante = cuota − aporteActual, con mínimo 0. Registrar el estado de investigación de ese nivel y su apertura. El inventario conserva los tipos aunque el contador rápido los agrupe.

1. Si ya terminó el mundo: solo mensaje ALT.12; no transacción.
2. Si faltante es 0: no consumir; abrir si investigación está completa y el paso aún no fue abierto; de lo contrario ALT.09.
3. Si legítimas disponibles cubren faltante: tomar exactamente esas legítimas por orden de adquisición; conservar legítimas sobrantes y todas las robadas; completar aporte y resolver apertura o espera.
4. Si no cubren: tomar todas las legítimas como aporte; rechazar y eliminar las robadas presentadas; mostrar pendientes. Si hay rechazo, escena cómica sin daño.
5. Si inventario no contiene ofrendas: informar faltante sin transacción.

Las operaciones de inventario y crédito se aplican juntas, una sola vez, antes de permitir otra entrada. La escena de apertura no vuelve a procesar inventario. La piedra no modifica salud, invulnerabilidad, gemas ni misiones. Una entrega válida que ignora robadas no activa el indicador de fraude descubierto.

Ejemplos obligatorios: cuota 5, crédito 0, inventario 6 legítimas+2 robadas deja 1 legítima+2 robadas y crédito 5. Cuota 5, crédito 1, inventario 2 legítimas+1 robada deja crédito 3 y elimina la robada. Cuota 5, crédito 3, inventario 2 legítimas+1 robada completa y conserva la robada. Cuota cubierta sin investigación no abre y no vuelve a cobrar.

## 7 Jefe y reloj de juego

Estados de máquina: apuntar, aviso, embestir, recuperación por choque con jugador, atasco, transición, victoria. El daño al taladro siempre se ignora. Resistencias montadas no son objetivos dañables. Las desmontadas solo reciben durante atasco; se retiran las derrotadas y se conserva la vida de supervivientes.

En fases dos y tres, el giro es limitado y se bloquea al estimar los últimos 0,5 s antes de la pared. Si el radio de giro o velocidad impiden evitar el ataque con 5 m/s, se ajustan los parámetros antes de exigir dash. Movimiento del jugador no se empuja por el choque; quien retrocede es el modelo de máquina.

Piedras: avisos cada 3 s, 2,7 s y 2,4 s según fase. En atasco se permiten dos por tanda. Radio 0,6/1,1 m, daño 1/2 y aviso mínimo 1,2 s. La selección combina posiciones recientes y áreas libres; garantiza paso al altar y espacio junto a una resistencia.

El hijo lanza cada intervalo uniforme entre 2 y 5 s jugables cuando el estado del Tío no sea mejora. Si hay 7 unidades en suelo, espera al siguiente intervalo sin crear otra. Caducidad 5 s, parpadeo los últimos 3; las recogidas no caducan. No se usan objetos de exploración en este sistema.

Estado del Tío: neutral, mejora o perjuicio. Solo uno. La preparación externa del lector es un modificador distinto. Cinco ofrendas neutrales conceden mejora 15 s; cinco en perjuicio lo quitan y dejan neutral; cinco con mejora no se consumen. Reiniciar fase no elimina perjuicio. El reloj de 40 s comienza tras la explicación al terminar el primer grupo y se reinicia al empezar fase o entregar válidamente. Con perjuicio espera entrega sin sortear otros.

Las tres mejoras y los perjuicios siguen el GDD. El daño final es entero, redondeado hacia abajo con mínimo 1; no se sortea reducción de daño si el valor previo es 1. Los modificadores de lector y Tío se aplican por separado sobre las estadísticas con habilidades. Movimiento final se limita al 70–130 % de la velocidad con habilidades, para evitar que efectos temporales rompan el recorrido. La preparación externa conserva sus 90 s restantes al entrar y no corre durante rescate ni introducción.

Un único reloj de tiempo jugable, o un mecanismo equivalente coherente, gobierna movimiento de juego, IA, ataques, enfriamientos, beneficios, botín, ofrendas y minijuego. Pausa, explicación y escena lo detienen. La escritura del diálogo y su avance siguen funcionando mediante tiempo de interfaz. Subtítulos de reacción no pausan. Los objetos recogidos antes de una transición conservan sus datos; la escena no duplica recogidas.

Victoria detiene peligros antes de la explosión, limpia inventario y efectos de arena y acredita EXP/dinero una sola vez. Muerte limpia arena y todos los beneficios temporales y reinicia sus grupos. No se concede EXP por resistencias del jefe. El permiso de saltar introducción solo aparece después de haberla visto y haber perdido.

## 8 Hijo jugable y cierre

El indicador de incorporación se aplica tras la conversación del capitán. Se registra el saldo histórico, vida máxima inicial y árbol independiente del hijo. El menú permite cambiar en la zona segura antes del viaje; no hace falta un nivel de demostración ni un combate nuevo. Su ataque básico usa barra de prospección, 1 de daño y ciclo 0,9 s; vida 14.

Diseño futuro de técnicas propias: Golpe medido de mayor alcance y preparación; Apoyo firme para reducir daño durante una postura; Estudio del terreno para ralentizar un enemigo marcado; Barrido de seguridad y Golpe de palanca. Mantenerlas como contenido de diseño, sin botones que prometan acciones inexistentes en esta entrega. Fundamentos está disponible por personaje con el catálogo del GDD.

La pieza de emblema es un objeto clave descriptivo registrado una sola vez. La ruta a vagoneta queda accesible después de inspeccionarla y hablar. Viajar termina la entrega: animación, Continuará y solicitud de regreso al menú principal. La pantalla final estática permite Guardar y volver, Volver sin guardar o Cancelar; la animación de viaje no permite guardar. Registrar el estado de presentación cierre viajado: al cargarlo vuelve a Continuará sin una escena jugable de cataratas. No existe botón de repetir jefe ni selección de niveles posterior a esta victoria. El selector del acceso funciona durante la exploración anterior al cierre. Continuar desde una partida guardada después de la victoria no recrea la pelea. Una partida manual anterior conserva naturalmente su progreso anterior.

## 9 Estado que debe guardarse

Usar IDs persistentes para nivel/sección, objetos estáticos, enemigo, misiones, aporte y eventos. Guardar incluye posición segura y orientación, personaje, salud por miembro, desbloqueos, EXP, nivel, histórico, saldos, nodos, recursos con procedencia y orden, cuota, investigaciones, aperturas, regalos evaluados, misiones e intentos, recogibles de expedición, escena vista, permiso de omisión, victoria, pieza y viaje.

Para un guardado exacto de exploración también se conserva estado relevante de carros, puentes, palancas y plataformas ambientales, vida y posición de enemigos activos, botín suelto con tiempo restante y progreso de regeneración. No guardar en jefe, minijuego, escena, diálogo o transición. Guardar después de una muerte conserva las recogidas de la misma expedición, no repone estáticos. Beneficios externos guardados mantienen tiempo restante sin descuento de tiempo real.

Escribir a un archivo temporal y sustituir el guardado solo después de completar la escritura correctamente. Si falla, se mantiene el último válido y no se sale. El guardado no requiere una base de datos externa. Las versiones de esquema permiten reconocer un archivo incompatible y avisar sin destruirlo.

## 10 Referencias y adaptación

Leer Reference/Mundo 1/INDICE_REFERENCIAS.md y los archivos presentes. Priorizar Super Paper Mario Chapter 5-3 para estratificación, fondo y conexiones por puertas; las tres referencias de vagoneta para alternancia entre anticipación, salto y recompensa; Block Puzzles para rieles, ocupación y secuencias de colocación. Construir rutas nuevas y demostrar que sus soluciones no copian una secuencia ajena.

La ficha de Objetos clave utiliza Reference/General/Imagenes/UI_Objetos_clave.png como referencia de categoría y descripción, sin reproducir exactamente su interfaz. El teléfono no añade una pantalla de llamadas funcional dentro de esa ficha. El mapa de General orienta la continuidad hacia cataratas y no sustituye los planos de estas secciones. Las referencias de cámara se consultan para conservar la configuración existente, no para reemplazarla.


## 11 Método de construcción y continuidad

Aplicar Metodo_Diseno_Niveles.md, Checklist_Nivel_1.md y Protocolo_Documentacion.md. El encargo actual implementa N1 y actualiza todos los documentos afectados por el canon; no construye N2–N6. Las cámaras se modelan como fuentes editables en Blender, se exportan por piezas y se validan en Unity. Una cámara no se considera diferente por cambiar adornos o puertas.

## Dirección de producción del mundo uno

Cada plano distingue perímetro protegido mediante paredes invisibles y caídas intencionales dentro del recorrido. La cámara sigue al jugador en profundidad y altura. Los límites exteriores impiden abandonar el escenario. Platicio, hijo del minero, aparece en cada nivel para explicar su identidad, ofrecer consejos y aplicar las reglas de regalos únicos. Otros mineros permiten conversaciones ambientales breves y divertidas.

El bocadillo ocupa una franja superior compacta y deja visible el escenario. El hablante se anima mientras habla. Mantener nombre, contraste, opciones y tecla de avance. El menú e inventario usan los vídeos de Paper Mario como referencia de navegación y composición. Los iconos provisionales del teléfono y otros objetos podrán sustituirse por arte del diseñador. El selector sigue el mapa conceptual y el funcional de Mundo 1.

El jefe es PerforAudora. La fuente 3D se suministra en FBX y GLB. En la primera sección de la punta debe quedar una sola marca color coltán. Los Resisworm montados siguen las estadísticas de cada fase de la batalla.

## Lectura del impacto y unidad artística

Cada golpe efectivo muestra la salud perdida sobre el personaje afectado mediante una cifra animada en un estallido rojo según DañoCausado.png. Dormitorio, casa y mina comparten escala de detalle, texturas pictóricas y sombreado suave. Las referencias de cada sala guían la silueta, los materiales y la iluminación.
