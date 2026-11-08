# Afterglow Blinding Hope
## Documento de diseño del juego

Versión 0.3 · 2 de noviembre de 2026

Diseño de Matias Andres Villafuerte Ruiz

Afterglow: Blinding Hope es una aventura de plataformas, puzles y combate en tiempo real para PC. Sus habitantes son minerales humanoides representados en 2D dentro de escenarios 3D. El juego combina personajes caricaturescos y humor con una historia sobre explotación, promesas de prosperidad y responsabilidad frente a los privilegios.

Este documento explica el universo, la historia completa que está definida y los sistemas necesarios para construir el prólogo y el mundo uno. El mundo dos tiene un argumento y recorrido preliminares. Los mundos tres al seis conservan sus temas y su función en la historia; sus mecánicas se desarrollarán posteriormente. El documento es autónomo y no exige conocer conversaciones ni otros GDD.

La duración total, los requisitos mínimos de PC y el equilibrio final se estimarán después de medir el prototipo. La prioridad actual es demostrar la aventura local completa y conservar una base que pueda ampliarse sin depender de la memoria de un agente.

## 1 Alcance y lectura del paquete

### 1 1 Entrega jugable

La entrega construye un prólogo, un pueblo de apoyo y seis niveles del mundo uno: entrada de la mina, rieles y carretillas, laberinto, vagoneta, galerías oscuras y rescate con taladro. Termina con la incorporación del hijo del minero y un viaje de presentación hacia cataratas. No incluye niveles jugables del mundo dos ni de los siguientes mundos.

Se utiliza geometría simple para escenario y taladro; figuras planas con colores por rol para los actores que todavía no tengan arte. Las animaciones existentes de la bolivianita pueden reutilizarse tras inspeccionar sus dependencias. La entrega debe demostrar el recorrido completo, guardado manual, muerte, combate, progresión y cierre; la decoración definitiva no es requisito.

### 1 2 Documentos y autoridad

El GDD define reglas, historia y alcance. Docs/Diseno/Mundo_1_Diseno.md detalla secciones, encuentros, misiones, cifras y estados. Docs/Guion/Mundo_1_Guion.md contiene diálogos con identificadores y condiciones. Docs/Implementacion/Checklist_Mundo_1.md ordena la producción y sus comprobaciones. Prompt_Codex.md es el encargo inicial para el agente. README_ENTREGA.md explica la instalación documental.

Reference contiene General para recursos compartidos, CharacterSource para las fuentes del personaje y una carpeta por mundo con Imagenes y Videos. Sus índices explican qué estudiar de cada material. Las referencias orientan composición, ritmo y lenguaje visual; no sustituyen las reglas del GDD ni autorizan copiar mapas o activos.

Las decisiones de historia y de progreso son requisitos. Los números de equilibrio se consideran parámetros iniciales de prueba: se pueden ajustar de forma documentada después de probarlos, sin cambiar las condiciones de desbloqueo, persistencia o victoria. Los nombres definitivos de personajes y el emblema del científico no son necesarios para construir el prototipo; se usan identificadores estables y recursos provisionales.

## 2 Experiencia del jugador

### 2 1 Pilares

La exploración combina rutas principales legibles con ramales que contienen recursos, habitantes y encargos. Cada comunidad tiene una historia local que merece atención por sí misma. Los conflictos y las máquinas van conectando esas experiencias con un problema de alcance mayor.

El combate se desarrolla directamente en el escenario. No existe cambio a combate por turnos ni pantalla separada de batalla. Saltar, moverse en profundidad y leer las señales de los enemigos son suficientes para completar las rutas obligatorias. Las habilidades compradas permiten especializarse y cambiar acciones, sin convertirse en requisitos ocultos para avanzar.

El tono permite situaciones cómicas, como una piedra que reprende sin causar daño, sin trivializar el sufrimiento de las comunidades. Los compañeros aportan perspectivas y personalidades propias. El protagonista habla poco; sus decisiones, gestos y ayuda a otros expresan su carácter.

### 2 2 Espacio cámara y controles básicos

El jugador se desplaza sobre un suelo 3D en los ejes lateral y de profundidad con las flechas. Espacio salta y X ejecuta el ataque básico. Los actores se representan como sprites o figuras planas, con colisiones y movimiento compatibles con escenarios 3D. La referencia a Paper Mario corresponde a presentación y exploración; el universo no está hecho de papel.

La cámara general del prototipo ya está aprobada y debe conservarse. Su configuración existente define encuadre, seguimiento y controles; los videos explican esa referencia. Los nuevos escenarios se adaptan a ella. No se sustituye el sistema ni se reajusta por una recomendación de otro paquete. Suelo, sombra simple y separación de planos permiten interpretar la profundidad.

## 3 Universo e historia general

### 3 1 Habitantes y materiales

Existen minerales vivos, que son personas, y minerales inanimados, que se extraen y usan como materiales. El dinero, el oro trabajado y los objetos restauradores no implican partes de habitantes. Los trabajadores del mundo uno extraen materiales sin vida. La explotación de personas es un conflicto del argumento, no la definición de toda actividad minera.

Los enemigos normales tienen formas de componentes electrónicos: resistencias, condensadores, LEDs, cables y otros dispositivos. Los jefes son máquinas más complejas. La historia cuestiona el uso de personas y territorios como recursos, sin establecer que toda tecnología sea malvada.

### 3 2 Protagonista y familia

El protagonista es una bolivianita humanoide, amable, perseverante y de pocas palabras. Conoce la minería gracias a sus padres, pero todavía aprende. Sus padres son mineros famosos y han viajado para ayudar a un amigo. El protagonista está acostumbrado a sus ausencias y al comienzo no cree que estén desaparecidos.

La casa familiar se inspira libremente en Villa Albina de Cochabamba. En el prototipo basta una habitación, una puerta, un patio y un portón que sostengan el inicio jugable. No se exige reproducir el edificio real.

### 3 3 Inicio de la aventura

Una carta avisa que la estancia de los padres con su amigo se ha alargado, pero que están bien. Tras leerla, el jugador ve al protagonista en casa. Suena el timbre y recibe control. Sale, cruza el patio y llega al capitán de minas, amigo de sus padres.

Un derrumbe dejó atrapados a varios trabajadores. El capitán supone que el hijo de mineros tan famosos domina el oficio y le pide revisar las galerías mientras los trabajadores abren paso. El protagonista acepta. Este rescate es la línea principal del mundo uno y pone en marcha el viaje.

El capitán es el padre del futuro compañero. Su hijo es tímido, metódico y observador; el capitán es frío, estricto, gritón y suele desmerecerlo. La colaboración durante el rescate permite que el hijo reconozca el valor de su propia ayuda sin cambiar de personalidad de golpe.

### 3 4 Conflicto de fondo y desenlace previsto

El amigo de los padres es un científico. Al principio considera que la tecnología y el coltán pueden servir para desarrollar algo beneficioso. Cuando crece la demanda, su prioridad pasa a ser ganar dinero. Recurre a promesas de mejores oportunidades, engaña a habitantes y termina sometiéndolos a explotación.

Los padres contribuyen indirectamente a ese sistema. Su relación con el científico y el alcance de esa contribución se revelan en el mundo seis. Sigue por definir cuánto sabían, desde cuándo y qué hacen cuando el protagonista los confronta. Estas incertidumbres pertenecen al desarrollo futuro de la historia y no deben resolverse mediante diálogos inventados en el mundo uno.

El final contempla una decisión entre conservar los privilegios familiares tolerando la explotación o rebelarse frente a los padres y el científico. La segunda alternativa conduce al conflicto con el jefe final. Las consecuencias concretas de cada desenlace, la pelea y el destino de los personajes requieren desarrollo posterior.

### 3 5 Orden de los mundos y geografía

| Mundo | Identidad y conflicto | Desarrollo actual |
| --- | --- | --- |
| 1 Minas bolivianas | Rescate, trabajo comunitario, reciprocidad con el Tío y llegada de una máquina desconocida | Historia y sistemas preparados para producción del prototipo |
| 2 Cataratas | Pueblo de minerales de oro, fiebre de riqueza y rumor de una cueva de oro sólido | Argumento y seis niveles preliminares |
| 3 Galaxia | Belleza luminosa y habitantes que se sienten opacados o ignorados | Tema narrativo; mecánicas y escenas pendientes |
| 4 Abandonada | Agotamiento de recursos, desplazamiento y recuperación de la naturaleza | Tema narrativo; intereses ocultos y desarrollo pendientes |
| 5 Fábrica | Producción de componentes enemigos, dependencia laboral y protestas silenciadas | Conflicto general; recorrido y personajes pendientes |
| 6 Coltán | Territorio inspirado en las minas de la República Democrática del Congo, explotación y revelación familiar | Núcleo del desenlace; decisiones y mecánicas futuras pendientes |

Los mundos uno y dos comparten una masa de tierra; tres y cuatro otra; cinco y seis una tercera. Hay mar entre esas masas. Una vía de vagoneta conecta minas bolivianas con cataratas. Los transportes entre continentes aún no están definidos. La condición de Galaxia como espacio literal, sobrenatural o representación visual sigue pendiente.

![Mapa conceptual](<../../Reference/General/Imagenes/Mapa_conceptual.png>)

El mapa es una referencia conceptual de agrupación e identidad visual, no un plano de niveles ni una reproducción de lugares reales.

### 3 6 Distribución de información

Al inicio el jugador sabe lo que dice la carta. En el mundo uno descubre una máquina desconocida y una marca sin identificar. El capitán no reconoce el logo. Solo el material y la capacidad de fabricación sugieren investigar cataratas. No se revela el nombre del científico, su relación familiar ni la complicidad de los padres en diálogos tempranos.

## 4 Personajes del mundo uno

| Personaje o rol | Función | Voz y presentación |
| --- | --- | --- |
| Bolivianita | Protagonista seleccionable desde el comienzo | Habla poco; responde sobre todo con gestos |
| Capitán de minas | Pide el rescate y organiza a los trabajadores | Seco, estricto y despectivo hacia su hijo; no conoce la marca |
| Hijo del minero | Tutorial, consejos, regalos puntuales y apoyo en el jefe | Tímido, metódico y analítico; seleccionable después de la victoria |
| Minero | Trabaja, bloquea temporalmente un paso o solicita ayuda | Su contexto distingue rescatados, exploradores y trabajadores que abren camino |
| Habitante del pueblo | Servicios, rumores o encargos | El vendedor de ofrendas robadas utiliza este mismo nombre público |
| Lector de coca | Pistas y preparación temporal a cambio de coca | Explica precio y servicio antes de cobrar |
| Tío | Se manifiesta mediante altar, aceptación, ayuda o perjuicio | No tiene voz directa; los mensajes del altar son narración del juego |

Los nombres propios, minerales y diseños definitivos de varios personajes están por crear. Se usan estos roles sin anticipar su identidad. El habitante que engaña no se etiqueta como Estafador en el HUD, diálogos, objetivos ni nombres visibles.

## 5 Estructura del mundo uno

### 5 1 Pueblo acceso y expediciones

El prólogo y el pueblo no cuentan entre los seis niveles. El pueblo ofrece tienda, lector de coca y habitantes. Se conecta con la zona segura al principio del nivel uno, donde está el altar de exploración. El regreso por teléfono deja al protagonista en esa zona segura. Desde allí se accede por puertas identificadas a las expediciones ya abiertas. El menú de pausa no teletransporta a secciones.

Cada nivel contiene secciones conectadas por puertas. Se puede retroceder físicamente. Una expedición empieza al entrar a un nivel desde el acceso y termina al abandonarlo. Cruzar puertas internas, cargar partida o repetir una habitación del laberinto no inicia una expedición.

### 5 2 Recorrido principal

| Nivel | Composición | Objetivo local | Cuota para el siguiente |
| --- | --- | --- | --- |
| 1 Entrada de la mina | Altar seguro, saltos, primer enemigo, rocas y desvío corto | Aprender y alcanzar al equipo del bloqueo | 3 |
| 2 Rieles cruzados | Camino casi recto y desvíos alto y bajo | Colocar carros, abrir paso y llegar al bloqueo | 5 |
| 3 Galerías de las señales | Cámaras por puertas, cruces y ramales secretos | Leer señales, salir del laberinto y llegar al bloqueo | 7 |
| 4 La vía suspendida | Acceso a pie, estación, minijuego y andén final | Completar la vagoneta y revisar el andén | 9 |
| 5 Galería sin luz | Descenso, carros con lámpara, cruces y ascenso final | Orientarse y localizar la cámara de los atrapados | 12 |
| 6 Cámara del derrumbe | Corredor con voces, puerta de rescate y arena circular | Rescatar y derrotar al taladro | Sin cuota nueva |

En niveles uno al cinco, alcanzar e interactuar con el punto final registra Investigación terminada. Abrir el siguiente requiere ese registro y completar su cuota en el altar. Se puede aportar antes de terminar la investigación; el crédito permanece, pero no abre hasta cumplir ambas condiciones. Cada apertura consume su propia cuota y sucede una sola vez. Las cinco suman 36 ofrendas.

Después de completar las condiciones, transcurren 2 segundos de escena segura y un minero anuncia que se ha abierto el paso. No se exige otro pago. No hay atajos diseñados para regresar al inicio; el teléfono resuelve ese regreso.

### 5 3 Teléfono de minas y objetos clave

El hijo entrega el teléfono en el tutorial. Aparece en Inventario, apartado Objetos clave, con icono, nombre y descripción. Esa ficha es descriptiva: no contiene un botón Usar. La acción de llamar se realiza con T mientras el personaje juega en la exploración del mundo uno.

Se puede llamar antes de investigar el final; se conserva lo recogido y la investigación sigue pendiente. La llamada está bloqueada durante jefe, minijuego, cinemática, conversación, caída o escolta activa. Si ya está en la zona de regreso, no inicia una transición adicional. Se permite llamar en combate normal de exploración y una confirmación evita pulsaciones accidentales.

Al terminar el mundo, el teléfono permanece como objeto clave descriptivo y deja de ofrecer llamadas. Otros objetos vinculados a un mundo también pueden conservar su ficha cuando ya no sean necesarios. El menú separa objetos clave de consumibles y ofrendas.

### 5 4 Laberinto y escoltas

Una salida equivocada conduce de nuevo a la misma habitación lógica, formando un bucle hasta encontrar la salida correcta. No devuelve al acceso, no resta vida ni objetos y no repone enemigos o recogibles. La puerta de regreso físico a la cámara anterior se distingue de las salidas del acertijo. Las pistas y los letreros de la ruta obligatoria son fiables.

Un NPC perdido puede seguir al jugador. El rescate termina al llevarlo caminando al inicio de ese nivel, no al pueblo ni al altar. Mientras acompaña, T muestra: «El rescate por teléfono solo admite a una persona. Esta vez tendrán que volver por su cuenta». No se inicia una llamada ni se completa la misión. Tras la muerte, la escolta pendiente se restablece en su punto seguro original; una escolta ya terminada permanece completada.

## 6 Inventario y economía de ofrendas

### 6 1 Recursos

El dinero sirve para compras y se encuentra como botín o en rutas que orientan la exploración. Los orbes de EXP solo se obtienen de enemigos y se aplican al recogerlos. Las gemas de habilidades se conceden al completar cada 100 EXP y se gastan en el árbol. No son dinero y no se compran en tiendas.

El inventario contiene consumibles, ofrendas, objetos de misión y objetos clave. Cada ofrenda registra internamente su tipo y procedencia legítima o robada. La interfaz no revela la procedencia antes de que el altar la evalúe. Las ofrendas de arena tienen un contador temporal separado.

### 6 2 Procesamiento del altar de exploración

Alcohol, cigarro y hoja de coca valen una unidad. No se exige una combinación particular. Al acercarse se muestra Interactuar [E]. La transacción se ejecuta una sola vez, se guarda en el estado de sesión y no puede duplicarse por mantener la tecla.

Para una cuota activa se calcula lo que falta después de los aportes previos. Si hay suficientes unidades legítimas, solo se consume la cantidad legítima necesaria. Se conservan todos los excedentes legítimos y todas las robadas, sin evaluarlas ni descartarlas. La cuota queda satisfecha y se abre el paso cuando también esté terminada la investigación. Esta entrega no provoca una reprimenda por las robadas que siguen en inventario.

Si no hay suficientes legítimas, todas las legítimas disponibles se depositan como crédito parcial. Las robadas presentadas en esa entrega se rechazan y desaparecen; no generan crédito. Se informa de la cantidad pendiente y, cuando hay rechazo, se presenta la piedra cómica sin daño. Una entrega solo robada rechaza sus unidades y no aporta progreso. Sin ofrendas, se informa de lo que falta sin reprimenda.

Cuando el aporte ya está completo y falta investigar, no se procesa otra entrega ni se consumen unidades. Si solo falta confirmar la apertura después de investigar, el evento abre con el crédito existente. Tras cerrar el mundo, el altar muestra su mensaje de tranquilidad y no procesa inventario.

El consumo de legítimas usa un orden interno estable de adquisición, sin preferencia oculta por tipo. Esto hace que los costes de coca sean previsibles; antes de entregar, la interfaz muestra qué unidades legítimas se tomarán sin revelar las ignoradas de origen dudoso. El detalle de procedencia sigue siendo interno.

### 6 3 Fuentes y reaparición

Los enemigos normales sueltan dinero y la EXP correspondiente a su dificultad relativa. Probabilidades iniciales independientes: 25 % de ofrenda legítima y 10 % de curación. El botín en suelo caduca a los 5 segundos y parpadea desde el segundo 2. Recogerlo cancela su caducidad. Abandonar la expedición elimina botín no recogido.

Las ofrendas estáticas y los objetos de misión no caducan. El diseño ofrece al menos 4, 6, 8, 10 y 13 ofrendas garantizadas accesibles en los niveles uno al cinco, entre objetos fijos y recompensas. La ruta principal puede financiarse con fuentes legítimas, sin comprar al vendedor ni depender del azar. Los encargos repetibles y las expediciones renovadas cubren recursos gastados en servicios.

Al iniciar una nueva expedición voluntaria reaparecen enemigos, dinero de guía y ofrendas estáticas renovables. Las recompensas únicas no reaparecen. Morir repone enemigos y devuelve a la entrada del nivel, pero conserva los recogibles estáticos ya tomados en esa expedición. No renueva encargos repetibles ni regalos. Cargar una partida conserva los registros de la expedición guardada.

### 6 4 Servicios

El vendedor se presenta como Habitante del pueblo y ofrece solo alcohol o cigarros robados. Primera oferta: 3 unidades por 8 de dinero. Comprar es opcional. Puede aportar rumores y una misión única para recuperar herramientas escondidas, descritos en el diseño y guion. No bloquea ningún camino obligatorio.

Una hoja de coca es el mismo objeto tanto para el altar como para el lector: gastar una en un servicio reduce en una las disponibles para ofrendar. Pista local cuesta 1 hoja; preparación cuesta 2. Si no hay pista útil nueva, se informa antes de cobrar. La preparación concede durante 90 segundos jugables velocidad +10 % o intervalo de ataque −10 %, a elección en el servicio. Comprar otra sustituye la anterior previa confirmación.

Fragmento pulido recupera 2 de vida y cuesta 5 de dinero. Núcleo restaurador recupera 5 y cuesta 12. Se usan desde Inventario y no exceden la vida máxima. Pueden aparecer en rocas, regalos o botín. Son materiales ficticios inanimados. La tienda se encuentra en el pueblo.

## 7 Combate enemigos y salud

### 7 1 Bolivianita

Vida base 10; daño básico 1; acción de pico 0,6 segundos; preparación de 0,15 segundos; alcance frontal aproximado 1,2 m y anchura de 0,8 m. El ataque básico detiene el movimiento y solo está disponible en suelo. El pico es su única arma durante este mundo. Las mejoras desbloquean ataque en marcha, aéreo, cadenas y técnicas.

Cada acción aplica daño una sola vez por objetivo. El daño recibido no empuja, no cancela ataques y concede 1 segundo de invulnerabilidad. No hay daño por simple contacto con un enemigo quieto: lo provocan sus acciones activas señaladas o peligros del escenario. Ataques, telegráficos y colisiones deben ser legibles con la cámara distante.

### 7 2 Catálogo del mundo uno

| Enemigo | Vida y movilidad | Ataque y respuesta |
| --- | --- | --- |
| Resistencia | 3; lenta; permanece en una plataforma | Inclina 0,7 s y golpea por 1. Apartarse y atacar en recuperación |
| Condensador | 4; baja pero no sube | Carga 1,1 s y descarga en círculo por 1. Salir del área marcada |
| LED centinela | 2; fijo en lugar accesible | Apunta 1 s y dispara pulso recto por 1. Cambiar profundidad o saltar |
| Minitaladro | 3; excava en suelo permitido, sin cruzar huecos | Estela y marcador 0,8 s; emerge por 1 y queda expuesto 1,5 s |
| Cable saltador | 2; sube escalones preparados y baja | Se comprime 0,6 s y salta a posición fijada por 1. Evitar aterrizaje |
| Batería inestable | 5; lenta; solo baja | Marca mancha 1,2 s antes de activarla por 1. Rodear la zona |

El carácter defensivo de la resistencia se expresa mediante su ritmo; el ataque básico siempre sirve. El minitaladro emerge aunque el jugador se quede quieto. Los enemigos obligatorios pueden alcanzarse sin comprar ataque aéreo. La batería es una ampliación del catálogo y puede incorporarse al final del pulido; resistencia, condensador y minitaladro forman el primer corte técnico.

### 7 3 Caída muerte y recuperación

Una caída resta 2 de vida, funde a negro y devuelve a la puerta de entrada de la sección. Si deja la vida en cero, se aplica muerte completa. Morir restaura la vida máxima del personaje activo y devuelve al inicio del nivel. Se mantienen dinero, inventario, EXP, gemas, habilidades, misiones completadas, aportes y puertas abiertas; se eliminan efectos temporales.

Cambiar de sección, salir de expedición o cargar no cura. La vida actual de cada personaje es independiente. Cambiar de personaje tampoco cura. Al subir de nivel se restaura la vida del personaje activo; los no seleccionados conservan su vida actual. Esta regla se representa como parámetro de alcance de curación en los datos de progresión.

## 8 EXP habilidades y equipo

### 8 1 Progresión

El nivel y la EXP pertenecen al equipo. El nivel inicial es 1 y su medidor comienza en 0/100. Cada 100 EXP incrementan un nivel, conceden 3 gemas a cada personaje desbloqueado y curan al activo. El exceso permanece: 99+2 termina en el nivel siguiente con 1 EXP. Se procesan también múltiples umbrales en una sola recogida.

Las gemas y las compras son propias de cada personaje. Al desbloquear al hijo se le concede la cantidad histórica total ganada por el equipo, aunque la bolivianita la haya gastado. Si el total histórico es 20, recibe 20. Comprar y reembolsar no modifican ese total. Las ganancias futuras se acreditan a todos los miembros desbloqueados.

La EXP de enemigos fáciles disminuye con el nivel del equipo. Parámetro inicial: máximo de 0 y EXP base menos la diferencia positiva entre nivel del equipo y referencia del enemigo. Una resistencia de referencia 1 y base 3 concede 3, 2, 1 y 0 para niveles 1, 2, 3 y 4. Dinero y ofrendas no disminuyen con esa regla. Los encargos no conceden EXP directamente. El taladro concede 100 EXP una sola vez al vencer; sus resistencias no permiten acumular EXP durante la batalla.

### 8 2 Árbol por personaje

La pantalla sitúa al personaje a la izquierda y el árbol a la derecha. Hay dos ramas independientes por personaje: Técnicas de pico, o el nombre de su arma, y Fundamentos. Fundamentos comparte catálogo entre personajes, pero no compras ni efectos. Se muestran coste, rango, requisito, saldo y reembolso. La bolivianita utiliza este árbol completo en el mundo uno.

| Técnica de pico | Coste por rango | Resultado |
| --- | --- | --- |
| Golpes enlazados | 3 y 4 | Dos y luego tres golpes encadenables, con ventanas propias |
| Pico en marcha | 3 | Atacar caminando al 70 % de velocidad |
| Golpe aéreo | 3 | Golpe frontal en el aire, sin salto adicional |
| Barrido frontal | 4 | Requiere Enlazados 1; mantener X 0,5 s amplía el golpe |
| Giro minero | 5 | Requiere Barrido; Z ejecuta giro de área con enfriamiento 3 s |
| Filo firme | 3 y 5 | +1 daño por rango; base final con ambos rangos 3 |
| Golpe preciso | 3 y 4 | Requiere Filo 1; crítico 5 % y 10 % total; daño doble |

| Fundamento | Coste por rango | Resultado |
| --- | --- | --- |
| Vitalidad | 2, 3 y 4 | +2 de vida máxima por rango |
| Paso seguro | 2 y 3 | +5 % de velocidad por rango |
| Salto ligero | 3 y 4 | +5 % de altura por rango; opcional |
| Esquiva breve | 3 | Dash con Shift, enfriamiento 2 s, sin invulnerabilidad |
| Ojo entrenado | 2 | Ver vida de enemigos normales |
| Recuperación | 5 | 1 de vida cada 20 s jugables tras 5 s sin causar ni recibir daño |
| Bolsillos atentos | 2 y 3 | +10 % de dinero de enemigos por rango |
| Afinidad de EXP | 3 | Mayor radio de recogida de orbes, sin aumentar su valor |

La técnica especial se selecciona en el menú y usa Z; X sigue reservado para ataque y cadenas. Compras y reembolsos se hacen en exploración fuera de combate, no durante jefe, minijuego o escena. No se añaden doble salto, gancho libre ni reflejo de daño a esta entrega. El objetivo de equilibrio es permitir unas dos o tres elecciones útiles antes del jefe, no completar el árbol; las ganancias se ajustan al recorrido real.

### 8 3 Reembolso

El reembolso devuelve el coste completo. Retirar un requisito muestra una confirmación que enumera los descendientes que también se quitarán y la suma a devolver. No puede quedar un nodo activo sin su requisito. Quitar vida máxima limita la vida actual al nuevo máximo; recomprarla no cura. Las acciones en ejecución terminan antes de retirar su habilidad. No se crean gemas históricas por reembolsar.

### 8 4 Hijo del minero

Antes de cada nivel da consejos. Evalúa un único regalo por nivel: 25 % de una ofrenda, 25 % de 5 de dinero y 50 % sin regalo. El resultado se registra incluso si no da nada. No se vuelve a tirar al conversar, morir o recargar.

En el jefe ocupa una cornisa segura y lanza ofrendas. Tras la victoria se desbloquea y puede seleccionarse desde el menú en la zona segura del cierre. El personaje inactivo no aparece durante exploración; sí puede aparecer cuando el guion lo requiera. No hay compañero autónomo de combate siguiendo al jugador.

Su rol es prospector metódico, de mayor vida y menor daño por segundo. Base inicial: 14 de vida, daño 1 y acción de 0,9 s. Usa una barra de prospección distinta del pico. Esta entrega incluye selección, movimiento, salto, golpe básico y su saldo independiente; las técnicas avanzadas candidatas se conservan como diseño para su desarrollo posterior, no como requisito de rejugabilidad. El mundo termina sin exigir probarlas.

## 9 Jefe del mundo uno

### 9 1 Rescate y presentación

El nivel seis comienza con un corredor. Se oyen mensajes de los mineros mientras se avanza. Al interactuar con la puerta se presenta el rescate: celebran al ver al protagonista y al hijo, pero un taladro atraviesa una pared antes de que terminen de agradecer. Los mineros huyen por un paso seguro. Se presenta el nombre Taladro aurífero y la cámara encuadra una arena circular.

El hijo explica las resistencias y las ofrendas antes de devolver el control. Solo después de perder contra el jefe aparece una opción de mantener F para omitir rescate y presentación. La muerte reinicia desde el corredor del nivel seis, con la batalla completa desde su primera fase y sin repetir cuotas del altar de exploración.

### 9 2 Ciclo y fases

El taladro apunta, anuncia una embestida y avanza. En la primera fase sigue una recta; en segunda y tercera puede corregir su dirección con giro limitado. Cualquier pared circular puede atascarlo. Al atascarse, las resistencias bajan de la máquina e intentan liberarla: solo entonces reciben daño.

| Fase interna | Grupo de resistencias | Embestida y atasco iniciales |
| --- | --- | --- |
| 1 | 2 con 3 de vida cada una | 6 m/s, aviso 1,5 s, recta, atasco 7 s |
| 2 | 3 con 3 de vida cada una | 7 m/s, giro hasta 15 grados/s, atasco 7 s |
| 3 | 4 con 3 de vida cada una | 8 m/s, giro hasta 25 grados/s, atasco 8 s |

Las dos últimas fases fijan dirección durante los últimos 0,5 segundos de la embestida para permitir evitarla con movimiento básico. Si alcanza al jugador, inflige 2 de daño, se detiene, retrocede mecánicamente y gira ruedas durante 1 segundo antes de volver a apuntar. Ese choque no es un atasco ni expone resistencias.

Las supervivientes conservan su daño entre atascos. Las derrotadas no vuelven dentro de esa fase. Derrotar a todo el grupo provoca una escena de refuerzos y el siguiente grupo. Eliminar el tercero vence al taladro. No recibe daño directo del pico ni tiene una barra de vida adicional. En fase dos las resistencias alternan lado de bajada; en tercera bajan por dos lados. No aparecen enemigos infinitos para farmear EXP.

### 9 3 Piedras

Caen durante toda la pelea y aumentan durante los atascos. Un círculo rojo y una sombra avisan al menos 1,2 segundos antes. Pequeñas: radio 0,6 m y daño 1; grandes: radio 1,1 m y daño 2. Solo dañan una vez al impactar. No cubren toda la arena ni el único acceso al altar; se mantiene espacio para acercarse a por lo menos una resistencia expuesta.

### 9 4 Altar y ofrendas de arena

El altar de la arena es distinto del de exploración, con el mismo diseño. Su estado e inventario son independientes. El hijo lanza una unidad cada 2–5 segundos jugables, salvo durante una mejora activa del Tío. Hay como máximo 7 en suelo; caducan a los 5 segundos y parpadean los últimos 3. Las recogidas no caducan y se acumulan sin ese límite de suelo.

El inventario anterior no sirve en la arena. Cada entrega procesa exactamente 5 unidades de arena, conserva sobrantes y aplica una sola transición. Neutral recibe una mejora temporal aleatoria. Con perjuicio, la entrega elimina ese perjuicio y deja neutral: no concede también una mejora. Con mejora activa, se rechaza sin consumir nada.

### 9 5 Tiempo y efectos

La primera fase no tiene reloj de castigo. Después de acabarla, el hijo da una explicación con pausa; el plazo empieza al recuperar el control. En las fases dos y tres hay 40 segundos jugables desde el inicio de fase o la última entrega válida. Al vencer el plazo se activa un único perjuicio aleatorio que sustituye una mejora del Tío si la hubiera. No acumula otros perjuicios mientras espera una entrega.

Cambiar de fase reinicia el plazo pero conserva el perjuicio existente. Una entrega válida reinicia el plazo y, si había perjuicio, lo elimina. La mejora del Tío dura inicialmente 15 segundos. Sus candidatos son daño +1, movimiento +15 % o intervalo de ataque −20 %. Perjuicios: daño −25 % con mínimo de 1, movimiento −15 % o intervalo +20 %. Si el daño actual es 1, el sorteo excluye su reducción para que el perjuicio no resulte imperceptible o anule el ataque.

La preparación del lector puede coexistir independientemente con el efecto del Tío. Este último no la elimina. La configuración limita la velocidad final para conservar la jugabilidad. Morir elimina todas las mejoras temporales. Ganar elimina ofrendas y efectos propios de arena; una preparación externa puede conservar el tiempo restante durante el cierre jugable.

Durante explicaciones, escenas y pausa no avanzan embestidas, piedras, enfriamientos, duración de beneficios, reloj del Tío, lanzamientos ni caducidad del botín. Las frases de reacción son subtítulos breves no interactivos: la batalla y su tiempo continúan.

## 10 Cierre del mundo uno

Al vencer, el taladro explota sin causar daño al jugador y queda una pieza dorada con logo y siglas. Vuelve el control; el jugador debe acercarse e interactuar. El logo definitivo no está diseñado: se usa un símbolo neutro y el texto [SIGLAS]. La pieza se registra como objeto clave y el evento ocurre una sola vez.

El capitán llega, reconoce el trabajo del protagonista y de su hijo y examina la marca. No la reconoce. Explica que no existe una fábrica o equipo local capaz de crear semejante máquina de oro. Sugiere consultar en cataratas, donde viven minerales de oro. Es una hipótesis de investigación, no una identificación del fabricante.

El hijo propone continuar para averiguar de dónde salió una máquina tan grande. Se incorpora al equipo y se habilita Cambiar personaje. La relación con el padre muestra un pequeño reconocimiento, sin resolverse totalmente. Una vía de carga y una vagoneta escondidas junto a la arena permiten continuar por tierra hacia cataratas.

Después de examinar la pieza y completar la conversación se puede interactuar con la vagoneta. Viajar inicia una animación con ambos personajes. Aparece Continuará y E solicita volver al menú principal, ofreciendo Guardar y volver, Volver sin guardar o Cancelar. Esta pantalla estática ya no es una cinemática activa: admite esa orden manual de guardar el cierre, sin guardar automáticamente. Cargar un cierre ya viajado muestra Continuará; no requiere construir una escena jugable de cataratas. No hay selector de niveles, modo de repetir al jefe ni desafío posterior a la victoria. La victoria y la entrega del emblema se protegen con indicadores para no duplicar recompensas al cargar una partida posterior al combate.

## 11 Mundo dos Cataratas

### 11 1 Historia local

Un pueblo al pie de las cataratas reúne habitantes y extranjeros buscando pepitas en el río. La fiebre de riqueza ocupa su tiempo y alimenta rivalidades. Muchos habitantes están hechos de oro, lo que introduce una ironía visual. Circula el rumor de una cueva de oro sólido en las profundidades.

El protagonista llega por la pista de la máquina dorada. El futuro compañero local quiere comprobar el rumor para cuestionar la obsesión colectiva, no para enriquecerse. La relación concreta entre investigar la marca y emprender las pruebas de la cueva sigue pendiente de escritura.

Un habitante ofrece un gancho a cambio de una pepita de oro. El protagonista encuentra pirita y la entrega como si fuera oro. El gancho sirve en una animación puntual para entrar detrás de la catarata; no es una herramienta general de balanceo o escalada.

### 11 2 Recorrido preliminar

| Nivel | Contenido | Condición de avance |
| --- | --- | --- |
| 1 Pueblo del río | Rumor, aliado, pirita y trueque | Obtener el gancho |
| 2 Exterior de cataratas | Ascenso, plataformas y entrada oculta | Llegar y usar el gancho en la animación |
| 3 Prueba de combate | Grupo definido de enemigos | Derrotarlo |
| 4 Prueba de plataformas | Tres mecanismos en distintas alturas | Activarlos |
| 5 Laberinto interior | Buscar una llave y su puerta | Abrir la salida |
| 6 Cámara del supuesto tesoro | Arena de la serpiente de cables | Vencer al jefe |

Una voz acompaña las pruebas: primero plantea desafíos con seguridad y después se irrita y amenaza conforme el jugador progresa. En la cámara final se descubre que pertenece a la serpiente. No existe la cueva de oro sólido que prometía el rumor.

### 11 3 Serpiente y cierre

La arena circular tiene charcos y goteras que permiten anticipar chorros. El jugador atrae a la serpiente hacia esos puntos para que el agua la aturda y pueda acercarse a atacar. El ciclo alterna persecución, pausa, ataque y reposicionamiento. Tiene tres fases; los chorros salen de forma espaciada y pueden dañar al jugador. Su exclusividad como condición de vulnerabilidad, intervalos y ataques exactos siguen pendientes.

La intención del cierre es regresar al pueblo, demostrar que no hay cueva de oro sólido y provocar una reflexión sobre el tiempo perdido persiguiendo esa promesa. Faltan la prueba concreta, reacciones, arco del aliado y pista hacia Galaxia. No se afirma que la serpiente inventó el rumor; su motivación y conexión con el científico necesitan desarrollo. No se implementa ese contenido en la entrega del mundo uno.

## 12 Interfaz diálogo y presentación

### 12 1 Menús y HUD

HUD de exploración: cabeza del personaje activo, vida actual/máxima, EXP/100, dinero, saldo de gemas propio, ofrendas transportadas, aporte de la cuota y objetivos secundarios activos. El inventario muestra detalle de consumibles, ofrendas, misiones y Objetos clave. No exige mantener el objetivo principal Llega al final como texto permanente.

En el jefe se muestran vida, resistencias restantes del grupo y ofrendas de arena, con coste de entrega de 5. No se muestra número de fase, efecto activo del Tío ni temporizador de castigo. Las escenas de refuerzos explican el cambio de dificultad y las frases del hijo comunican sus intervenciones de forma general. Ojo entrenado solo revela vida de enemigos normales.

El menú ofrece Continuar, Guardar, Inventario, Árbol de habilidades, Cambiar personaje cuando corresponda y Salir. Solo se muestran personajes desbloqueados. La vida actual y el saldo de gemas se actualizan según la selección sin curar ni transferir puntos.

### 12 2 Conversaciones y escenas

El texto aparece progresivamente. E completa primero la frase actual y una pulsación posterior avanza. Mantener la tecla no salta toda la conversación. Hablar detiene movimiento y combate y pausa los temporizadores de juego. La carta y las escenas iniciales no se pueden omitir; únicamente el rescate y presentación del jefe admiten omisión después de perder contra él.

Las voces ambientales y reacciones breves durante la pelea aparecen como subtítulos sin bloquear control. No se superponen; tienen prioridad y límite de repetición por evento. Las explicaciones iniciales y la advertencia tras el primer grupo son conversaciones con pausa.

### 12 3 Teclado

| Tecla | Acción | Restricción |
| --- | --- | --- |
| Flechas | Movimiento lateral y de profundidad | En vagoneta, Abajo frena brevemente |
| Espacio | Saltar | No avanza diálogos |
| X | Pico y cadena | Ataque aéreo o en marcha requieren compra |
| Z | Técnica especial seleccionada | Sin acción hasta desbloquearla |
| E | Interactuar y avanzar texto | Mensajes muestran la tecla efectiva |
| T | Llamar por teléfono | Solo exploración del mundo uno y sin escolta |
| I | Menú de personaje e inventario | Pausa juego |
| K | Árbol | Compra o reembolso fuera de combate |
| Shift izquierdo | Dash | Requiere Esquiva breve |
| Esc | Pausa o regresar | No omite escenas |
| F mantenida 1 s | Omitir rescate e introducción | Solo en reintento tras derrota |

### 12 4 Recursos y sonido

El entorno usa bloques, rampas y plataformas grises; los rieles y carros pueden ser geometría básica. Colores provisionales: bolivianita morado; capitán azul oscuro; hijo celeste; habitantes verde; atrapados naranja; trabajadores que abren paso amarillo; lector turquesa; enemigos rojo. El vendedor usa el mismo color de los habitantes. Formas, etiquetas y movimiento complementan al color.

Se necesitan sonidos simples de salto, pico, daño, recogida, puerta, llamada, altar, ruedas y avisos del jefe. Pueden ser recursos de prueba propios o con licencia verificable. Debe poder ajustarse música y efectos desde opciones. Las señales de peligro siguen siendo visuales aunque no haya audio definitivo. La implementación inicial no requiere voces grabadas.

## 13 Guardado y estado de sesión

El guardado es manual desde pausa. Se deshabilita durante jefe, minijuego y cinemática; tampoco se escribe a mitad de una transición o conversación. Salir ofrece Guardar y salir, Salir sin guardar y Cancelar. En contextos donde no se puede guardar, se explica la restricción y se permite salir sin guardar o cancelar. Un fallo de escritura mantiene la sesión abierta.

En exploración se conserva mundo, nivel, sección, posición segura y orientación; vida por personaje; personaje activo y desbloqueos; dinero; nivel, EXP, total histórico, saldos y árboles; inventario y procedencia; aportes; investigaciones terminadas; puertas; encargos; regalos evaluados; recogibles de expedición; escenas vistas; permiso de omitir introducción; emblema, victoria y acceso al viaje. Se conservan efectos externos y sus segundos restantes cuando el guardado sea válido. El reloj no avanza con el juego cerrado.

Guardar no inicia expedición, repone objetos ni cura. Los cambios de sesión pueden mantenerse en memoria durante el juego, pero no sobrescriben el archivo sin una orden manual. Salir sin guardar vuelve al último guardado cuando se carga de nuevo. Continuar solo se habilita si existe una partida válida. El archivo se escribe de forma que un fallo no destruya el último guardado correcto.

## 14 Producción y referencias

### 14 1 Proyecto de Unity

El proyecto existente contiene un prototipo de plataformas. El agente debe inspeccionarlo antes de editar, identificar escenas y código reutilizables y mantener sus versiones de Unity y paquetes salvo necesidad justificada. Los datos informados son Unity 6000.5.5f1 y URP 17.5.0; se verifican en ProjectVersion.txt y el manifiesto. No se presupone acceso al editor por tener instalado un plugin.

La implementación separa presentación visual de movimiento, daño, inventario, misiones y estado. Los parámetros de combate y economía se configuran como datos. Las animaciones importadas conservan .meta y dependencias; no se copia ProjectSettings ni Packages desde el proyecto 2D. Se evita mezclar cuerpos y colisiones 2D con resolución de plataformas 3D.

El orden de producción y las pruebas están en el checklist. El agente informa por separado lo escrito, lo compilado y lo probado en Play Mode. La existencia de un script no demuestra que su flujo funcione en la escena.

### 14 2 Referencias por mundo

Reference/General/Imagenes y Videos reúnen cámara, HUD, objetos clave y presentación compartida. Reference/CharacterSource conserva sprites y fuentes de animación. Reference/Mundo 1/Imagenes y Videos contienen mina, niveles y puzles locales; los mundos dos al seis tienen la misma estructura. Los índices registran archivo o enlace, fragmento, intención y nivel. Los informes generados se guardan en Docs/Referencias/Analisis.

Para el mundo uno se registran las referencias indicadas por el diseñador: Super Paper Mario Chapter 5 3, para fondo, alturas, plataformas y secciones; Donkey Kong Country Returns 1 6 Crazy Cart, Mario and Luigi Superstar Saga Mysterious Mine Carts y la referencia de carritos de Super Mario RPG indicada como Docaty Mountain, para ritmo y composición del minijuego; Mario plus Rabbids Kingdom Battle Block Puzzles, para carros y ocupación de espacios. El clip exacto de Super Mario RPG se identifica cuando se coloque el material, sin asumir un título diferente.

La adaptación mezcla esas ideas con objetivos, rutas, recompensas y personajes propios. El agente debe ver los archivos disponibles y registrar cuáles estudió. Los títulos no significan que se haya inspeccionado un video. Antes de programar presenta el plan de plugins y assets, instala las opciones gratuitas compatibles que pueda gestionar y detalla las acciones manuales necesarias. El procedimiento está en Prompt_Codex.md. Si falta un clip o asset opcional, avanza con el diseño y los recursos provisionales.

## 15 Desarrollo futuro

El mundo uno queda especificado para construir un prototipo completo con arte provisional. El diseño detallado define sus contenidos verificables y los datos iniciales. El arte definitivo, nombres, música y siglas pueden sustituirse mediante los identificadores existentes.

Pendientes futuros: en Cataratas, serpiente, rumor e investigación, aliado, diálogos, pruebas, combate y cierre; en Galaxia, Abandonada y Fábrica, personajes, conflictos, recorridos, mecánicas y conexiones; en Coltán, conocimiento de los padres, confrontación, alternativas y jefe. No se implementan ni se resuelven por interpretación durante el mundo uno.

