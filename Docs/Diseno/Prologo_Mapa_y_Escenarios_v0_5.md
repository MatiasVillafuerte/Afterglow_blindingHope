# Afterglow · Diseño del prólogo, selector y secciones

Especificación de producción · 5 de noviembre de 2026.

## 1. Objetivo y términos

Afterglow: Blinding Hope es una aventura con personajes minerales 2D en entornos 3D. El control usa las flechas para movimiento lateral y en profundidad, Espacio para saltar y E para interactuar. La cámara de exploración ya aprobada se conserva. El combate ocurre en el propio escenario. El mundo no es de papel.

Un **mundo** reúne una comunidad y su arco de historia. Un **nivel** es una expedición seleccionable, como Entrada de la mina o Rieles cruzados. Una **sección** es una cámara concreta dentro de ese nivel, con sus puertas y estado. El **selector** presenta los seis niveles del mundo uno; no es un mapa de las seis regiones del juego. El prólogo y el pueblo no cuentan como niveles.

Esta especificación describe el prólogo y el selector integrados en la partida real. El encargo vigente conserva esos sistemas y perfecciona N1. La remodelación de N1–N6 seguirá después, por los hitos del checklist maestro. Este documento también fija cómo evitar que todas sus secciones se conviertan en pasillos laterales.

## 2. Reglas confirmadas y decisiones de montaje

Son requisitos: dormitorio conectado directamente con jardín; mansión inspirada en Villa Albina; recinto rectangular cercado; fuente, vegetación y portón; capitán fuera; secciones con profundidad y alturas; selección de niveles; progresión con investigación y cuota; cámara existente; actores 2D.

Son decisiones iniciales de montaje para poder avanzar: selector en M01_MAPA tras el capitán, navegación entre nodos con flechas y E y jardín existente de 28 × 32 unidades; conservar las dimensiones y fuentes editables ya verificadas del dormitorio. El agente puede adaptar estas dimensiones a la escala real. El criterio es recorrido legible con la cámara y el salto existentes. No son medidas del edificio histórico.

## 3. Prólogo jugable

### 3.1 Secuencia e historia

Partida nueva muestra la carta P01 antes de entregar control. Los padres avisan que su estancia donde un amigo se ha prolongado y que están bien. El protagonista todavía no cree que hayan desaparecido. Reutilizar las líneas P01.02–P01.03 del guion; no inventar una urgencia, una amenaza ni una mención al científico.

Al terminar la carta se ve la bolivianita en su dormitorio. La deja en el escritorio mediante la animación o gesto sencillo disponible. Suena el timbre una vez, aparece «Alguien tocó la puerta» y comienza el control. Los diálogos usan texto progresivo: E termina la frase y una pulsación posterior avanza. Mantener E no omite toda la escena. No añadir una función de saltar cinemáticas al prólogo.

El jugador camina hasta la puerta, interactúa y aparece en el jardín. Recorre el patio hasta el portón; allí habla con el capitán situado fuera. Aplicar P02.02–P02.07: derrumbe, mineros atrapados, experiencia que el capitán atribuye al hijo de mineros famosos y encargo de rescate. La bolivianita responde con los gestos del guion. No revelar la identidad ni la relación del científico con la familia.

Al terminar P02 se registra el prólogo como completado y un fundido lleva directamente a M01_MAPA. No se añade una ruta jugable por carretera. Desde el mapa se entra en N01S01. Los eventos N1A de presentación del hijo conservan sus IDs y ocurren una sola vez al iniciar la visita guiada al nivel uno; no se adelanta aquí la entrega del pico ni del teléfono.

### 3.2 Dormitorio — PRO_CASA

Habitación rectangular de una planta jugable, con una única salida directa al jardín. No construir un vestíbulo, escaleras interiores ni otras habitaciones obligatorias. La fachada puede representar dos plantas mientras esta habitación de juego está conectada a la planta baja.

La decoración toma inspiración de los interiores de Villa Albina: muebles de madera tallada, papel tapiz, cortinas, alfombra y luminaria de época. Una cama, mesita, armario, escritorio con carta, retrato familiar y minerales inanimados bastan. Usar detalles violetas y dorados discretos relacionados con la bolivianita. El retrato es provisional y no fija el diseño definitivo de los padres.

Los muebles dejan un recorrido central despejado. La salida se distingue con encuadre, luz y contraste. El personaje usa sus sprites y animaciones existentes; el concepto no debe convertirse en un personaje modelado en 3D. Todavía no tiene pico ni teléfono. Golpear, llamar y abrir el mapa de niveles no funcionan aquí.

La carta puede releerse con P01.06. Es una interacción secundaria sin recompensa. La puerta tiene un punto de aparición propio a ambos lados, colocado fuera del volumen de transición y sobre suelo seguro. Volver del jardín no vuelve a mostrar la carta inicial ni activa de nuevo el timbre.

### 3.3 Jardín — PRO_PATIO

Terreno rectangular completamente delimitado por vallas, incluyendo laterales y parte posterior de la propiedad. El portón es el único acceso al exterior. La casa ocupa el fondo y conserva los rasgos de Villa Albina: fachada clara, ventanas altas, marcos de ladrillo rojizo, molduras cálidas, balcones pequeños y tejado de tejas. No modelar todo el interior de la mansión.

Una fuente central constituye el punto visual del jardín. El camino desde la casa se divide a sus dos lados y vuelve a unirse antes del portón. Ambos lados son transitables. Árboles, flores, bancos y faroles se sitúan principalmente en los márgenes, con zonas de césped que se pueden recorrer. No hay enemigos ni puzles obligatorios en este espacio.

La fuente y las vallas tienen colisiones coherentes. El jugador no puede salir saltando la valla, entrar al agua por accidente ni atravesar la fachada. La altura y el ancho de los colliders se ajustan al controlador real. No colocar un muro invisible en medio de un camino que parece abierto. El jardín no requiere saltos precisos ni habilidades del árbol.

El capitán espera fuera del portón. Un punto de interacción en su lado interior permite hablar con él sin atravesar las rejas. E inicia P02; no se activa una y otra vez por mantenerse cerca. Durante el diálogo se detiene el control; después del encargo se realiza la transición a M01_MAPA. Si falta su arte definitivo, es una figura plana de color identificable por rol.

### 3.4 Cámara y acabado

Las vistas elevadas de Reference explican distribución; no sustituyen la cámara de juego. Adaptar posiciones, tamaño de árboles, altura de muebles y márgenes a la cámara aprobada. No cambiar su seguimiento ni los ejes del control para que coincidan con una ilustración. Una vista específica del mapa puede tener su propia cámara sin modificar la de exploración.

El prólogo debe tener formas, materiales, vegetación y luz suficientes para reconocer la propuesta. Una caja sin material puede servir de montaje temporal durante la misma tarea, pero no cierra la entrega de arte. Mantener un acabado caricaturesco coherente, con geometría editable y piezas reutilizables. Conservar los colliders sencillos aunque la malla visible tenga detalle.

## 4. Acceso seguro y mapa de selección

### 4.1 Acceso y retorno

N01S01 es la zona segura junto al pueblo y al altar único al comienzo de la mina. Su salida física conduce a M01_MAPA, escena de selección que conserva su cámara propia y foco modal. M01_ACCESO no forma parte de la ruta de arranque actual.

El teléfono T vuelve a N01S01 con los recursos, sin entregar ofrendas ni completar investigación. Mantiene sus bloqueos de jefe, minijuego, escena, conversación, caída y escolta. Consultar su ficha no llama. No se abre el selector desde la pausa en una galería.

N1A/N1C conservan introducción y teléfono únicos. Entrar desde el mapa inicia la expedición; las puertas internas y el retorno no duplican el intento. Esc en M01_MAPA vuelve a N01S01 después de la primera visita; antes de ella se confirma N1.

El selector reemplaza el conjunto de puertas entre niveles en el acceso. Las puertas **internas de cada nivel** siguen siendo físicas y permiten retroceder. El jugador no elige una sección ni aparece directamente junto al objetivo final.

### 4.2 Presentación y controles

Mapa visual con relieve de un valle minero, senderos y seis nodos. Cada nodo tiene una miniatura reconocible, número y nombre. El pueblo/acceso es una referencia de retorno, no el nivel cero ni una expedición adicional. No construir una copia de los mapas de Mario.

Las flechas recorren nodos adyacentes por las conexiones del mapa. Se permite enfocar un nodo cerrado para leer por qué no se puede entrar. E confirma un nodo disponible; Esc cierra el mapa y devuelve el control al mismo lugar del acceso. No añadir movimiento libre, salto ni combate a esta vista. El ratón es opcional y debe utilizar las mismas validaciones que el teclado.

Al abrirlo, enfocar el último nodo visitado si sigue siendo válido; en una partida nueva, N1. El nodo seleccionado tiene marco o marcador y nombre, no solo un cambio de color. El mapa muestra como mínimo: nombre, disponible/bloqueado/investigado, investigación del nivel seleccionado y aporte de ese nivel para el siguiente. El nodo final indica que no exige cuota nueva. Distinguir progreso de investigación y progreso de apertura.

Al seleccionar un nivel cerrado no se carga nada ni se descuenta inventario. Un mensaje concreto explica lo pendiente: por ejemplo «Para abrir Rieles cruzados: termina la investigación de Entrada de la mina y completa su aporte al Tío (1/3)». Si una de las dos condiciones ya se cumplió, mencionarla como completada y destacar la restante.

Cada nivel disponible se carga en su entrada, con una sola transición y una sola expedición. Deshabilitar confirmación mientras se procesa la carga. Al cerrar el mapa se libera el foco; una pulsación de E no debe confirmar un nivel y activar también una interacción en el destino.

El mapa no tiene botón de guardar propio. Se cierra con Esc para utilizar el guardado manual del menú existente en el acceso. Abrir/cerrar el mapa no guarda, cura, repone enemigos ni concede recursos. La vista puede ser una interfaz con terreno ilustrado y nodos, o una miniatura 3D con interfaz; debe integrarse con las herramientas existentes, sin reescribir el proyecto para crearla.

### 4.3 Condiciones exactas

| Nodo | Nombre | Miniatura | Requisito de acceso |
| --- | --- | --- | --- |
| M01N01 | Entrada de la mina | Boca de mina | Prólogo completado y mundo uno iniciado |
| M01N02 | Rieles cruzados | Cruce de rieles y carros | N1 investigado y aporte N1 de 3 completado |
| M01N03 | Galerías de las señales | Cruce con letreros | N2 investigado y aporte N2 de 5 completado |
| M01N04 | Galería sin luz | Galería con lámpara | N3 investigado y aporte N3 de 7 completado |
| M01N05 | La vía suspendida | Viaducto de vagoneta | N4 investigado y aporte N4 de 9 completado |
| M01N06 | Cámara del derrumbe | Entrada bloqueada por derrumbe | N5 investigado y aporte N5 de 12 completado |

El registro de investigación procede de interactuar con el final real del nivel. El crédito procede del altar y respeta la procedencia de las ofrendas. El mapa solo consulta ese progreso; nunca consume ni acepta ofrendas. Cada apertura sucede una vez y conserva el anuncio del minero. Al abrir N2, el mapa refleja la nueva conexión sin obligar a entrar automáticamente.

Cumplir solo la investigación o solo el aporte no abre el siguiente. Haber abierto un nivel no se revierte cuando se gasta coca o se conserva menos inventario: su cuota ya pagada se registra como crédito. No aplicar una cuota global de 36 a cada apertura; son cinco aportes separados que suman 36.

Los niveles N1–N5 disponibles se pueden volver a visitar antes de terminar el mundo para recoger recursos o atender encargos. Salir voluntariamente y volver a entrar inicia una nueva expedición según el GDD. Pasar una puerta interna, morir, abrir el mapa o cargar partida no inicia una expedición nueva. Una interfaz no debe ofrecer un nivel aún sin escena implementada como si estuviera terminado; registrar el faltante en el estado de producción sin inventar una condición narrativa.

N6 conserva sus reintentos antes de ganar. Después de la victoria continúa la secuencia de la pieza, incorporación del hijo, viaje y Continuará. No añadir un selector posterior a la victoria, repetición del jefe ni niveles jugables de cataratas como parte de esta entrega.

### 4.4 Datos y persistencia

Utilizar los sistemas reales de progreso y guardado. Datos mínimos: fase del prólogo, timbre consumido, conversación del capitán terminada, mundo iniciado, investigación por nivel, aporte por nivel, aperturas ya anunciadas y último nodo enfocado. Los IDs del nivel son estables; el nombre visible puede cambiar sin romper partidas.

Guardar en PRO_CASA o PRO_PATIO conserva ubicación segura, vida y fase de la secuencia. Cargar después del timbre no lo repite; cargar después de P02 no vuelve a pedir el encargo. Una partida de antes del encargo continúa desde su estado guardado. No se produce guardado automático al terminarlo.

Al migrar un guardado anterior, conservar todo lo existente. Inferir las nuevas banderas solo desde progreso real que ya las pruebe; una partida con N1 iniciado no debe repetir el prólogo. No desbloquear todos los nodos por faltar un campo. Si el guardado es ambiguo, aplicar un valor conservador y documentar esa migración concreta.

La carga de un guardado dentro de una galería restaura esa sección y no abre el mapa. Los estados de los nodos se calculan o sincronizan con la fuente única de progreso. No mantener otro inventario ni otra lógica de cuotas en la interfaz.

## 5. Composición dinámica de secciones

### 5.1 Ejes y puertas

«Arriba y abajo» puede significar dos cosas distintas. En esta especificación, **profundidad Z** es acercarse o alejarse sobre el suelo con las flechas; **altura Y** es subir mediante saltos, rampas o escalones. El desplazamiento lateral es X. No confundir una salida situada al fondo con una plataforma físicamente elevada.

Un nivel se representa como una red de secciones conectadas. Cada puerta/portal tiene ID estable, sección destino, punto de aparición y puerta de regreso si corresponde. La cámara y el control conservan sus ejes al entrar; la transición no rota silenciosamente los controles. Puede cambiar la orientación de la ruta sobre el suelo sin cambiar la cámara.

El sistema de puertas existente puede reutilizarse. Para interacciones nuevas, E abre o atraviesa una puerta cercana; si el proyecto ya tiene portales de cruce automático estables, conservar ese comportamiento de forma consistente y reflejarlo en ayudas. En ambos casos impedir rebotes inmediatos entre portales, cargas duplicadas y apariciones dentro de colliders.

Cada sección tiene una intención, una ruta principal, una entrada, al menos una salida o retorno y, cuando proceda, un ramal opcional. Diseñar el plano y la conexión antes de decorar. Una puerta en altura necesita plataforma de llegada; un puzle debe tener solución y reinicio sin duplicar recursos ni atrapar al jugador.

### 5.2 Vocabulario espacial

| Composición | Cambio de recorrido | Uso posible |
| --- | --- | --- |
| Galería lateral amplia | X con profundidad para rodear obstáculos | Presentar entrada, enemigo o señal |
| Cámara en profundidad | Del primer plano al fondo, con desvío lateral | Entrada y salida en ejes distintos |
| Terrazas | Alternar X/Z y subir por Y con saltos básicos | Puerta alta y recurso opcional bajo |
| Cruce de alturas | Ruta inferior y superior visibles | Elegir ramal y regresar sin atajo al acceso |
| Cámara de trabajo | Carro, topes y placa abren un paso | Puzle de N2, no añadido obligatorio al prólogo |
| Bucle de señales | Varias puertas y regreso identificable | Laberinto de N3 con misma habitación lógica |
| Estación y vía | Paso a minijuego y andén posterior | N5, con reglas de control propias ya documentadas |
| Galería con luz móvil | Ruta en profundidad, descenso y ascenso | N4, manteniendo visibles suelo y peligros |

Variar la función y el recorrido, no solo invertir el mismo pasillo ni cambiar el lugar de las cajas. Evitar cadenas de tres cámaras consecutivas con idéntico eje dominante y estructura de reto. Esta es una guía de composición, no una obligación de añadir un puzle a cada habitación. N6 conserva su pasillo narrativo deliberado antes del jefe.

Las plataformas obligatorias se miden con el salto y movimiento básicos del controlador real, dejando margen. No exigir dash, doble salto, ataque aéreo o aumento de velocidad para completar el mundo. Si falta una mecánica de escalera, no dibujar una como única ruta y asumir que ya existe.

### 5.3 Aplicación a los niveles

Aplicar el orden y la identidad de Mundo_1_Canon.md. N1 es tutorial breve, N2 exploración amplia y diversa, N3 laberinto de cuatro puertas con confirmaciones y secretos, N4 exploración con poca luz, N5 minijuego de vagoneta y N6 jefe. Las anclas no fijan cantidad de cámaras. El plano se desarrolla con Metodo_Diseno_Niveles.md y mantiene misiones, fuentes, cuotas y eventos.

El prólogo y el selector se conservan durante el encargo actual de N1. El checklist de prólogo sigue siendo un registro de esa entrega; las tareas actuales están en Checklist_Nivel_1.md.

## 6. Condiciones de entrega

La ruta de Partida nueva debe conducir a carta, dormitorio, jardín, capitán, M01_MAPA y entrada real N01S01 sin abrir escenas de prueba manualmente. Las escenas nuevas deben formar parte del arranque/build/Addressables efectivo según la arquitectura existente.

La prueba de progresión cubre: ninguna condición, solo investigación, solo cuota, ambas y guardar/cargar después del desbloqueo. Un test de estado controlado sirve para esta lógica, pero no sustituye el recorrido jugable ni se expone al usuario como desbloqueo gratuito.

Registrar capturas del prólogo y mapa, ruta de escenas reales, consola, pasos comprobados, tareas por ID y limitaciones. No afirmar que se implementó el mundo entero. Los materiales sin color, la falta de conexión al arranque y los botones que solo cambian un texto no cierran esta entrega.

## Revisión del selector

Comparar la composición con Mapa_Mundo1_Concepto.png y conexiones con Mapa_Mundo1_Funcional.svg de Mapa_Seleccion en las referencias v0_5. Verificar los estados de foco y desbloqueo en Game.
