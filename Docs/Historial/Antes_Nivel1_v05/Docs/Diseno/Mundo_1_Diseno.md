# Mundo uno Diseño de niveles y datos de producción

Versión 0.3. Aplicar junto con Docs/GDD/Afterglow_GDD_v0_3.md y Docs/Guion/Mundo_1_Guion.md. Este documento define contenido del mundo uno; las cantidades son valores iniciales ajustables mediante datos. Los estados narrativos y sus condiciones permanecen obligatorios.

## 1 Identificadores y escala

M01 identifica el mundo; M01N01–M01N06 sus niveles. Las misiones usan Q01–Q11 para no confundirse con niveles. El guion conserva P01, P02, N1A, N1B, N1C, N1D, ALT, PUE, SEC, N2–N5, N6A–N6C, FIN y SYS. Cada línea tiene ID único. Los eventos se resuelven por ID y estado, no por comparar su texto.

Escala inicial: 1 unidad Unity representa aproximadamente 1 m; ajustar solo después de medir los recursos existentes. Velocidad de la bolivianita 5 m/s y altura de salto unos 2 m. Todas las plataformas obligatorias se alcanzan con el salto y movimiento básicos. Margen lateral y ancho de piso permiten desplazamiento en profundidad visible con la cámara actual aprobada, que debe conservarse. Los edificios del prólogo son proporciones de juego, no reproducción arquitectónica.

No hay valores obligatorios de longitud total ni tiempo de partida antes de medir el montaje. Construir cada sección con un objetivo reconocible y ajustar distancia para evitar caminatas sin decisiones. No estirar el escenario para cumplir una duración arbitraria.

## 2 Prólogo pueblo y acceso seguro

PRO_CASA contiene carta, mesa, zona inicial y puerta al patio. PRO_PATIO lleva al portón, donde espera el capitán. Son espacios jugables separados por una transición o continuidad física según convenga al prototipo. El timbre se reproduce al terminar la carta y no se repite indefinidamente. La salida se ve desde el inicio con cámara y composición.

La conversación P02 lleva a M01_ACCESO, una zona segura conectada con el pueblo y el inicio del nivel uno. El pueblo contiene tienda, lector de coca y habitante vendedor. El altar de exploración está en el inicio seguro del nivel uno, físicamente accesible desde el pueblo. El punto de regreso del teléfono está junto a él, sin teletransportar automáticamente al inventario o iniciar una entrega.

El acceso contiene puertas identificadas para niveles abiertos. Están cerradas las no desbloqueadas; muestran motivo sin consumir objetos. Estas puertas son acceso diegético, no un selector desde el menú. Dentro de los niveles solo se cambia de sección al cruzar puertas físicas.

## 3 Niveles y secciones

### M01N01 Entrada de la mina

| Sección | Contenido y conexión | Evento verificable |
| --- | --- | --- |
| N01S01 | Acceso seguro con altar, hijo y camino a galería | N1A presenta personajes; altar con aporte inicial 0/3 |
| N01S02 | Piso amplio y escalones bajos; salida a la derecha | Enseña movimiento y salto, sin requerir habilidades |
| N01S03 | Resistencia tutorial, roca frágil y cámara segura | N1B entrega pico antes de activar agresión; cada recompensa se da una vez |
| N01S04 | Ramal corto con casco Q02, recursos y bloqueo final elevado | N1D registra investigación; cuota 3 habilita N02 |

N1C introduce teléfono y altar durante el primer recorrido guiado. Se puede volver a S01 caminando para completar la explicación antes de la entrega. La primera resistencia deja 1 ofrenda legítima garantizada para mostrar el circuito de recursos; enemigos posteriores usan su probabilidad normal. Fuentes accesibles: 2 ofrendas fijas y recompensas Q01–Q02. El total garantizado disponible es al menos 4. No se exige aceptar ambas misiones.

### M01N02 Rieles cruzados

| Sección | Composición | Regla |
| --- | --- | --- |
| N02S01 | Entrada, hijo y primer carro en vía corta | Empujar con E mantenida y dirección; soltar E deja el carro |
| N02S02 | Reja, placa y dos apartaderos marcados | Un carro mantiene abierta la reja al permanecer en la placa |
| N02S03 | Desvío alto con herramientas y uno bajo con recursos | Rocas agrietadas rompibles; columnas estructurales no rompibles |
| N02S04 | Rampa de salida, trabajadores y bloqueo | Investigación terminada y cuota 5 abren N03 |

Las vías restringen el movimiento de carros a un eje preparado y tienen topes. Una palanca de reinicio restablece solo el puzle, no objetos, enemigos ni recompensas de misión. Se usan carros diferentes o posiciones compatibles para que completar Q03 no cierre la ruta principal. La reja no puede atrapar al personaje dentro de un collider: se mantiene abierta mientras hay alguien en su zona de seguridad. Hay al menos 6 ofrendas legítimas garantizadas entre suelo y encargos.

### M01N03 Galerías de las señales

| Sección | Función | Salida correcta y regreso |
| --- | --- | --- |
| N03S01 | Entrada del nivel, hijo y referencia visual de rescate | Lleva a S02; aquí termina Q05 |
| N03S02 | Cámara de lámparas con dos puertas de avance | Subir hacia lámpara encendida lleva a S03; la otra vuelve a S02 |
| N03S03 | Cruce de rieles y ramal opcional del NPC perdido | Derecha con riel continuo lleva a S04; error vuelve a S03 |
| N03S04 | Desnivel con postes y galería secundaria de carteles | Bajar entre dos postes lleva a S05; error vuelve a S04 |
| N03S05 | Bloqueo final | Investigación y cuota 7 abren N04 |

Cada cámara conserva además una puerta de retorno a la anterior; se distingue por un símbolo de acceso ya recorrido. No se coloca la misma señal en dos opciones contradictorias. Una salida errónea produce transición corta y reaparece en la misma cámara, sin crear una copia nueva ni restablecer estado. No cura, no mueve al acceso del nivel, no cuenta como nueva expedición y no completa por sí sola la escolta.

El NPC de Q05 espera en un ramal de S03. Tras aceptar sigue posiciones transitables del recorrido; nunca intenta atravesar paredes ni necesita navegación general de mundo abierto. En puertas y bucles se reposiciona junto al jugador sin completar el objetivo. Para cobrar se vuelve a S01 caminando por las puertas de retorno. El teléfono permanece bloqueado durante toda la escolta. Hay al menos 8 ofrendas garantizadas y dos secretos opcionales con dinero.

### M01N04 La vía suspendida

| Sección | Contenido | Condición |
| --- | --- | --- |
| N04S01 | Acceso a pie, hijo y rocas de Q07 | Se conserva movimiento y combate normal |
| N04S02 | Estación y vagoneta interactiva | E muestra controles y solicita subir |
| N04S03 | Vía de avance automático | Espacio salta; Abajo reduce velocidad temporalmente |
| N04S04 | Andén, dueño del paquete y bloqueo | Completar recorrido, investigar final y aportar 9 abre N05 |

La vagoneta usa una vía fija y encuentros diseñados, no generación aleatoria. Velocidad inicial de prueba 7 m/s; frenado al 70 % durante máximo 1,5 s seguido de 2 s de recuperación. Saltos siguen una trayectoria que supera los huecos de la ruta básica sin mejoras del árbol. Dentro no se usa pico, teléfono, cambio de personaje ni guardado. Inventario y pausa son consultables; pausa congela el minijuego y solo Salir sin guardar está disponible para abandonarlo al escritorio.

Secuencia inicial: recta segura para aprender; obstáculo bajo aislado; hueco corto anunciado por riel roto; arco de dinero que describe el salto; doble obstáculo con descanso; paquete de Q08 en trayectoria segura; hueco final y frenada hacia el andén. No se copia una secuencia de un video. La duración se ajusta después de probar lectura y velocidad; el primer montaje busca alrededor de 45–60 segundos, no un recorrido largo sin variación.

Un fallo no letal resta 2 de vida y reinicia en la estación; con cero aplica muerte completa al acceso del nivel. Dinero, ofrendas y paquete ya recogidos permanecen y no se duplican al reintentar. La investigación no se completa hasta llegar al andén. Las ofrendas obligatorias se distribuyen en exploración antes/después de la vía y no exigen recoger un salto difícil. Disponibilidad garantizada: al menos 10.

### M01N05 Galería sin luz

| Sección | Contenido | Condición |
| --- | --- | --- |
| N05S01 | Entrada con consejo, lámparas y descenso | Presenta señalización constante de puertas |
| N05S02 | Carro con lámpara y cruce | Colocarlo junto a una marca permite reconocer la salida |
| N05S03 | Refugio Q09 y enemigos de cruce Q10 | Carro y combate enseñan a preparar espacio seguro |
| N05S04 | Ascenso a compuerta y golpes al fondo | Investigación y cuota 12 abren N06 |

La oscuridad no oculta el suelo inmediato, un borde letal ni un aviso de ataque. Una iluminación ambiental mínima y una sombra simple sostienen la profundidad. El carro no consume combustible. Reiniciarlo no borra una misión ya completada. Se introducen cable saltador y, durante pulido, batería inestable, en espacios que permitan evitarlos con acciones básicas. Disponibilidad garantizada: al menos 13 ofrendas.

### M01N06 Cámara del derrumbe

N06S01 es un corredor con tres zonas de voces ambientales N6A.02–04. No contiene combates nuevos ni cuotas adicionales. La puerta al final inicia la secuencia de rescate. N06S02 contiene el espacio de rescate y la arena; la cámara cambia de encuadre al aparecer el taladro, sin teleportar al jugador a un lugar narrativamente inconexo.

La arena tiene radio inicial 10 m y paredes continuas. El altar de batalla está en un lateral y el hijo en una cornisa segura. La salida de mineros no lleva al jugador fuera del jefe. Los elementos no ocupan el único paso alrededor de la máquina. Después de la victoria, el mismo espacio permite caminar, examinar la pieza, hablar y entrar a la vagoneta escondida. No se vuelve a iniciar el jefe tras la victoria.

## 4 Misiones

| ID y nivel | Objetivo después de aceptar | Repetición | Recompensa |
| --- | --- | --- | --- |
| Q01 N01 | Derrotar 3 resistencias del área marcada | Nueva expedición voluntaria | 1 ofrenda y 5 dinero |
| Q02 N01 | Recuperar casco del ramal y entregarlo | Única | 1 ofrenda y 8 dinero |
| Q03 N02 | Colocar 2 carros marcados y avisar | Única | 2 ofrendas |
| Q04 N02 | Abrir 3 rocas opcionales y devolver caja | Única | 1 ofrenda y 10 dinero |
| Q05 N03 | Llevar minero perdido a N03S01 caminando | Única | 2 ofrendas y 10 dinero |
| Q06 N03 | Corregir 3 placas del ramal opcional | Única | 1 ofrenda; placas permanecen corregidas |
| Q07 N04 | Romper 4 rocas marcadas en acceso | Nueva expedición voluntaria | 1 ofrenda y 10 dinero |
| Q08 N04 | Recoger paquete de la vía y entregarlo al andén | Única | 2 ofrendas y 1 Fragmento pulido |
| Q09 N05 | Llevar carro de luz al refugio indicado | Única | 2 ofrendas |
| Q10 N05 | Derrotar 4 componentes del cruce | Nueva expedición voluntaria | 2 ofrendas y 12 dinero |
| Q11 Pueblo | Tras descubrir fraude, recuperar caja escondida y devolverla a su dueño | Única opcional | 8 dinero y aclaración narrativa |

Las bajas anteriores a aceptar Q01 o Q10 no cuentan. Los objetos únicos de misión pueden existir antes de aceptar y su entrega los reconoce, sin duplicarlos. Q06 solo corrige carteles secundarios; la ruta obligatoria nunca exige adivinar una mentira. El paquete Q08 permanece en inventario si se recoge y luego se falla el minijuego.

Estados de encargo: no ofrecido, disponible, activo, objetivo cumplido y entregado. Objetivo cumplido no entrega automáticamente la recompensa de un encargo que exige volver al NPC. Morir conserva progreso y entregas; una escolta activa es una excepción de posición y debe retomarse desde el NPC, sin borrar objetos ni otras misiones. Un encargo repetible entregado solo crea un nuevo intento al iniciar otra expedición voluntaria. Nunca vuelve a pagar por las mismas bajas.

Q11 se habilita al rechazar al menos una compra robada en el altar. El habitante conserva su nombre público y puede ofrecer un rumor útil antes o después. No roba dinero o inventario mediante un evento invisible. La caja no aparece como activo si no se ha generado la misión; el reembolso fijo no depende de comprar repetidas veces.

## 5 Datos de combate y experiencia

Resistencia, condensador, LED, minitaladro, cable y batería usan los valores del GDD. Los enemigos persiguen solo dentro de sus áreas transitables; su movilidad no aumenta al cambiar de nivel. Ninguno necesario para avanzar exige una habilidad comprable. Daño se registra por ID de acción y objetivo para impedir golpes múltiples del mismo collider.

| Zona | Referencia de enemigo | EXP base inicial | Dinero por enemigo inicial |
| --- | --- | --- | --- |
| N01 | 1 | 3 | 2 |
| N02 | 2 | 5 | 3 |
| N03 | 2 | 6 | 3 |
| N04 acceso y andén | 3 | 7 | 4 |
| N05 | 3 | 9 | 4 |
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

La pieza de emblema es un objeto clave descriptivo registrado una sola vez. La ruta a vagoneta queda accesible después de inspeccionarla y hablar. Viajar termina la entrega: animación, Continuará y solicitud de regreso al menú principal. La pantalla final estática permite Guardar y volver, Volver sin guardar o Cancelar; la animación de viaje no permite guardar. Registrar el estado de presentación cierre viajado: al cargarlo vuelve a Continuará sin una escena jugable de cataratas. No existe botón de repetir jefe ni selección de niveles. Continuar desde una partida guardada después de la victoria no recrea la pelea. Una partida manual anterior conserva naturalmente su progreso anterior.

## 9 Estado que debe guardarse

Usar IDs persistentes para nivel/sección, objetos estáticos, enemigo, misiones, aporte y eventos. Guardar incluye posición segura y orientación, personaje, salud por miembro, desbloqueos, EXP, nivel, histórico, saldos, nodos, recursos con procedencia y orden, cuota, investigaciones, aperturas, regalos evaluados, misiones e intentos, recogibles de expedición, escena vista, permiso de omisión, victoria, pieza y viaje.

Para un guardado exacto de exploración también se conserva estado relevante de carros, vida y posición de enemigos activos, botín suelto con tiempo restante y progreso de regeneración. No guardar en jefe, minijuego, escena, diálogo o transición. Guardar después de una muerte conserva las recogidas de la misma expedición, no repone estáticos. Beneficios externos guardados mantienen tiempo restante sin descuento de tiempo real.

Escribir a un archivo temporal y sustituir el guardado solo después de completar la escritura correctamente. Si falla, se mantiene el último válido y no se sale. El guardado no requiere una base de datos externa. Las versiones de esquema permiten reconocer un archivo incompatible y avisar sin destruirlo.

## 10 Referencias y adaptación

Leer Reference/Mundo 1/INDICE_REFERENCIAS.md y los archivos presentes. Priorizar Super Paper Mario Chapter 5-3 para estratificación, fondo y conexiones por puertas; las tres referencias de vagoneta para alternancia entre anticipación, salto y recompensa; Block Puzzles para rieles, ocupación y secuencias de colocación. Construir rutas nuevas y demostrar que sus soluciones no copian una secuencia ajena.

La ficha de Objetos clave utiliza Reference/General/Imagenes/UI_Objetos_clave.png como referencia de categoría y descripción, sin reproducir exactamente su interfaz. El teléfono no añade una pantalla de llamadas funcional dentro de esa ficha. El mapa de General orienta la continuidad hacia cataratas y no sustituye los planos de estas secciones. Las referencias de cámara se consultan para conservar la configuración existente, no para reemplazarla.
