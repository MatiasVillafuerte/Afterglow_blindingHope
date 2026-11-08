# Plan de N1 Entrada de la mina

Versión de trabajo 2026-11-06. Estado: diseño medido; nuevo montaje pendiente. Fuente del encargo: Prompt_Codex_Nivel1.md y canon 0.5. Este documento no declara jugable la geometría propuesta.

## Medidas y alternativas

Métricas reales en `Docs/Implementacion/Nivel1/Metricas.md`: velocidad 5 m/s; cápsula 2 m de alto y radio 0,33; salto observado 1,70 m y 3,89 m de alcance durante 37 frames. Cámara existente FOV 38 y seguimiento XYZ. El valor configurado jumpHeight=2 no equivale al arco observado con integración por frame. Huecos obligatorios ≤2 m, peldaños ≤0,8 m y recepción ≥2×2 m. No se cambia el controlador ni su cámara.

Alternativa A: tres espacios amplios. Acceso y movimiento juntos; taller con combate y mecanismo; cuenca final con investigación. Menos transiciones, pero varias instrucciones compiten en el taller y el primer espacio conserva trayectos largos entre servicios.

Alternativa B elegida: cuatro cámaras de tamaños distintos. Refugio de acceso, cantera de terrazas con recorrido quebrado, ensanchamiento de trabajo para combate y garganta con puente/descanso/final. Cada cambio separa una función y conserva la orientación del regreso. Cuatro resulta de esas funciones, no de una obligación de reutilizar S01–S04. Se mantienen sus IDs para conservar anclas y recompensas; cambia la superficie recorrible.

## Ruta y ritmo

M01_MAPA → N01S01 ↔ N01S02 ↔ N01S03 ↔ N01S04. Teléfono desde exploración válida → N01S01. Pueblo conectado solo al acceso. Regreso físico por las mismas puertas; sin atajo que complete investigación o altar. Ritmo propuesto 0,1 / 0,35 / 0,55 / 0,2 / 0,45 / 0,1: orientación, saltos, primer combate, descanso, mecanismo y subida, conversación final. Duración medida el 8 de noviembre: la ruta automatizada (N1PlayChecks) tarda 100,7 s desde la entrada a N01S01 hasta la apertura de N2. Incluye diálogos, capturas y combate opcional, con entrada simulada. No es una estimación para jugadores nuevos, que aún no se ha evaluado.

## Cámaras y acciones

| ID | Función y forma transitable | Secuencia, decisión y altura | Entradas, salida y recompensa |
| --- | --- | --- | --- |
| N01S01 Refugio | Atrio central con dos ensanchamientos de trabajo y cuello posterior; aprox. 22×18 m, Y=0. | Reconocer hijo, altar y luz del túnel. Caminar X/Z; elegir encargos opcionales sin bloquear salida. Sin enemigos. | Aparición (-1,0,-5,55); mapa delante, pueblo izquierda; avance (1,0,8,1) requiere teléfono. N1A/N1C únicos, altar cuota 3, Q01/Q02. |
| N01S02 Cantera escalonada | Suelo quebrado con acceso al sudoeste, desplazamiento lateral y ascenso al nordeste; aprox. 23×20 m. Vacíos visibles entre masas, sin una losa rectangular bajo todo. | Ver salientes anchos y puerta alta. Primer salto con base de recuperación; cambiar de X a Z; peldaños 0,65/1,30/1,95 y recepción final 2,20. Aterrizajes anchos; ningún salto de precisión obligatorio. Perímetro (8 de noviembre): todos los bordes exteriores tienen barrera invisible de 3,2 m y estacas visibles. El hueco de 0,5 m entre las terrazas 2 y 3 queda abierto, pero es más estrecho que la cápsula del personaje (0,66 m) y funciona como peldaño. La caída intencional del nivel es la garganta de N01S04. Plano: `Docs/Diseno/Perimetro_N1/`. | Entrada (-8,0,-5,2); regreso (-8,0,-7,1); salida (8,2,2,10). EXP.N01S02.0 pasa a la terraza intermedia. |
| N01S03 Taller abierto | Ensanchamiento de combate al sur y cuello minero al noroeste; ramal lateral para dos resistencias opcionales; aprox. 20×22 m, Y=0. | Hijo entrega pico antes de despertar enemigos. Espacio libre para leer el primer ataque; derrotar resistencia, recoger ofrenda; roca agrietada separada bloquea el cuello. Elegir ramal Q01 o continuar. | Entrada (6,0,-6,7), salida (-6,0,11,4). Se conservan tres IDs de enemigo, EXP.N01S03.0 y N01.S03.ROCA_ROTA. Roca se recoloca en el cuello, sin una segunda recompensa. |
| N01S04 Garganta del contrapeso | Dos orillas separadas por un corte de 5 m, puente de 3 m de ancho; orilla norte en L y terraza final. Aprox. 20×20 m. | Ver palanca y puente desde orilla segura; E baja el puente; cruzar, elegir casco a derecha o subida a izquierda. Tres peldaños de 0,6 m combinan caminar en profundidad y saltar; final a Y=1,8. | Entrada (5,0,-5,8), regreso (5,0,-7,65); casco Q02 en ramal norte (7,0,8), minero final (-4,1,8,10). N1D registra investigación, nunca entrar o accionar puente. |

Sin decoración: atrio con ramales / cadena de terrazas y vacíos / ensanchamiento para combate con cuello / dos orillas y cruce. No basta cambiar color ni girar un rectángulo. Esta diferencia debe comprobarse de nuevo en la geometría exportada.

## Mecanismo único

Palanca de contrapeso en la orilla sur de S04. Acción E, cerca y sobre suelo, con controles libres. El tablero gira desde posición levantada hasta horizontal en 1 s. Contrapeso y brazo se mueven en sentido opuesto; el efecto queda a pocos metros y dentro del encuadre desde el mando. Mensaje N1E.02 enseña la relación antes de usarla.

El puente no cierra: elimina atrapamiento, necesidad de reset y pérdida de ruta al volver. Collider del paso solo se habilita al terminar de bajar; durante el movimiento no se puede guardar ni cambiar de sala desde el mando. El borde y el desnivel se muestran; caída aplica las reglas existentes. Evento persistente `N01.S04.BRIDGE_OPEN` registrado al activar: muerte/cambio de sala/carga lo restauran ya horizontal sin repetir premios ni animación. Guardado previo cerrado vuelve a cerrado. No consume ofrendas ni otorga investigación. No exige nuevas habilidades. Una sola palanca, sin añadir carros, placas o plataformas móviles independientes.

## Anclas, recursos y conservación

N1A/N1C siguen en S01; N1B y tres resistencias en S03; N1E junto a palanca S04; N1D en final S04. Q01/Q02 se aceptan y cobran en S01. Q02 casco se conserva por ID aunque cambie su posición. Dos fuentes fijas más primera resistencia garantizada más Q02 dan cuatro ofrendas legítimas disponibles; Q01 añade una quinta. La ruta básica obtiene tres sin comprar ni exigir ambas misiones. Cuota 3 e investigación deben abrir N2 en el mapa, cuya escena permanece pendiente.

No se reemplaza prólogo, selector, UI, animaciones o combate. Se reutilizan AfterglowPlayer, AfterglowCamera, M01RouteDoor, M01Expedition, M01StoryPoint, M01Investigation, M01Altar, M01QuestNPC y guardado. N4/N5 migran por la política en Nivel1/Migracion_Canon.md. La remodelación N1 deberá recolocar guardados y snapshots sobre puntos seguros sin regenerar recursos ni enemigos derrotados.

## Producción y pruebas

Fuentes en ArtSource/Blender/Modelado_Afterglow/Nivel1; exportación bajo Assets/Afterglow/Environment/Modelado/Nivel1. Separar suelo, adornos, roca, puente, brazo y contrapeso. Primero blockout Blender y escala/pivote en Unity; después recorrido real y correcciones; solo entonces acabado con los materiales del juego. Las fuentes del prólogo y mapa no se regeneran.

LD00–LD07 son contexto/diseño; LD08–LD12 montaje y pruebas; LD13–LD18 arte y verificación; LD19 cierre documental. Plano editable: Plano_Nivel1.svg. Evidencia actual: métricas y migración; todavía faltan nuevo montaje, recorrido, mecanismo y pruebas de cierre.
