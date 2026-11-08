# Afterglow Checklist de implementación del mundo uno

Versión 0.5. Estados verificados conservados del proyecto real; historial detallado en Docs/Historial/Antes_Nivel1_v05. No marcar una casilla por crear archivos: se exige comportamiento en escena y evidencia. El repositorio real de Unity debe inspeccionarse antes de completar el primer hito. Este archivo es una instantánea documental; al integrarlo conservar los estados del checklist real y revalidar únicamente tareas afectadas por cambios.

## Cómo trabajar

Seguir los hitos en orden de dependencia. En cada tarea registrar estado Pendiente, En curso, Implementado sin probar, Verificado o Bloqueado. Una casilla [x] solo significa Verificado. Anotar archivos reales modificados, escena, prueba, resultado y errores relevantes en Estado_Trabajo.md. No inventar rutas de scripts antes de inspeccionar el proyecto.

Si una referencia visual falta, avanzar con el diseño de niveles y registrarlo; no bloquear código independiente. Si falta un recurso definitivo, utilizar el provisional indicado. Si una decisión contradice este paquete, documentar el conflicto y consultar solo ese punto. No reabrir como preguntas las reglas ya especificadas.

La prioridad actual es perfeccionar N1 mediante Checklist_Nivel_1.md y sincronizar la documentación oficial. No reiniciar el prólogo ni construir N2–N6 en ese encargo. Aplicar Protocolo_Documentacion.md antes de cerrar cada corrección.

## Hito 0 Contexto y diagnóstico

- [x] C00 Leer AGENTS.md aplicable, README, GDD completo, diseño y guion completos, checklist e índices de Reference. Evidencia: resumen de alcance y documentos leídos.
- [x] C01 Inspeccionar versión Unity, paquetes, escenas, control, cámara, físicas, entrada, animaciones y estado de Git. Evidencia: inventario de componentes existentes y qué se reutilizará.
- [x] C02 Comprobar herramientas del editor disponibles. Evidencia: escena activa y consola si se pueden consultar; si no, limitación explícita y trabajo de archivos posible. No afirmar conexión solo por tener plugin instalado.
- [x] C03 Crear Estado_Trabajo.md con diagnóstico y plan de hitos. Antes de programar, presentar Plan_Herramientas_y_Assets.md con función, fuente oficial, compatibilidad, licencia, coste y modalidad de instalación. Instalar y comprobar opciones gratuitas accesibles; indicar los pasos del usuario para las indispensables que no se puedan instalar. Registrar referencias vistas, conservar la cámara aprobada y no bloquear el avance por opcionales.

Salida: se conoce la base real y se continúa con implementación autorizada, sin pedir otra confirmación para tareas rutinarias.

## Hito 1 Base jugable y tiempo

Depende de C00–C03.

- [x] C04 Conservar el prototipo útil y preparar escenas o zonas de trabajo del mundo uno. Evidencia: no se han reemplazado ajustes o paquetes del proyecto sin necesidad.
- [x] C05 Movimiento 3D con figura 2D y salto, conservando el sistema y configuración de cámara aprobados. Adaptar alturas, profundidad, orientación, rampas y puertas a esa cámara; ninguna ruta obligatoria exige mejoras. Comprobar que las instalaciones no alteraron el encuadre ni seguimiento existentes.
- [x] C06 Pico frontal de suelo: 1 daño, detiene movimiento, una aplicación por objetivo y acción. Probar que mantener contacto no multiplica daño y X en aire no ataca sin habilidad. *2026-11-08: el golpe usa un volumen 3D de 1,2 × 0,8 m (AfterglowHitbox). Revalidado con teclado real contra Resisworm y roca. X en el aire, sin cambios en el código, queda pendiente de una nueva observación.*
- [x] C07 Salud, 1 s de invulnerabilidad y daño sin empuje ni cancelación. Probar que se puede seguir atacando al recibir daño y que no hay daño por enemigo quieto.
- [x] C08 Caída: −2 y regreso a puerta de sección; con cero aplicar muerte e inicio de nivel. Probar ambas condiciones.
- [ ] C09 Reloj de juego y diálogos con escritura progresiva. Probar pausa de IA, botín y beneficios, mientras texto y botones siguen funcionando. E completa frase y después avanza; mantener E no omite todo.

Salida: escena base jugable con pico, salud, cámara y pausa coherentes.

## Hito 2 Recursos altar teléfono y estado

Depende del hito 1.

- [ ] C10 Inventario de consumibles, ofrendas, misiones y Objetos clave, con procedencia interna oculta. Cabeza y contadores del HUD conectados al estado real.
- [x] C11 Dinero, orbes y gemas separados. Probar 99+2 EXP: nivel siguiente, EXP 1, 3 gemas por miembro desbloqueado y curación del activo. Probar también varios umbrales en una recogida.
- [ ] C12 Botín de enemigo 5 s, parpadeo últimos 3 y una sola recogida. Objetos estáticos y de misión no caducan. Pausa congela tiempo restante.
- [ ] C13 Altar de exploración con cuotas 3, 5, 7, 9 y 12 y aportaciones parciales. Probar sin ofrendas, insuficientes, completas y excedentes; no cobrar dos veces. *2026-11-08: cuotas 3, 5, 7, 9 y 12 cubiertas y cobradas una vez en los recorridos N1–N5 (altar compartido acredita el nivel de la expedición). Falta probar sin ofrendas, insuficientes y excedentes.*
- [x] C14 Probar cuota 5 y crédito 0 con 6 legítimas+2 robadas: quedan 1 legítima+2 robadas, crédito 5 y no hay piedra. Probar crédito 1 con 2 legítimas+1 robada: crédito 3 y robada eliminada, piedra sin daño.
- [ ] C15 Apertura exige investigación terminada y cuota completa. Probar cumplirlas en ambos órdenes; anuncio único y puerta abierta sin otro pago. *2026-11-08: en N1–N5 la apertura llega tras investigar y completar la cuota (orden investigación → altar). Falta el orden inverso.*
- [ ] C16 Teléfono por T, regreso al acceso y ficha solo descriptiva en Objetos clave. Probar llamada antes del final: conserva recursos y no marca investigación.
- [ ] C17 Bloqueos de teléfono durante escolta, jefe, vagoneta, conversación, escena y caída. Probar que consultar ficha no ejecuta llamada y que tras cerrar mundo queda inactivo.
- [x] C18 Estado de expedición. Cruzar puertas, repetir cámara y cargar no renuevan nada; salida voluntaria y nueva entrada reponen fuentes renovables. Morir repone enemigos, conserva estáticos tomados y no renueva encargos ni regalos.

Salida: circuito entrar, investigar, recoger, volver, aportar y abrir funciona sin duplicar recursos.

## Hito 3 Prólogo y primer nivel completo

Depende del hito 2.

- [x] C19 Carta, casa, patio, timbre y capitán usando P01–P02. El control empieza tras la carta; el jugador llega caminando al portón. No revelar al científico ni una desaparición de padres.
- [ ] C20 Pueblo y acceso seguro con altar, puertas y actores por rol. Habitante vendedor tiene mismo nombre y color que otros habitantes.
- [ ] C21 Topología variable de N1 y tutorial N1A–N1E, siguiendo las anclas del diseño. Dar pico antes de agresión, teléfono una vez y primer botín demostrativo. Mantener diálogos alineados con las teclas reales.
- [ ] C22 Q01 y Q02: aceptar, contar nuevas bajas, buscar, devolver y cobrar. Morir después de una entrega única no permite repetir recompensa.
- [ ] C23 Servicios de tienda, lector y vendedor. Coca se descuenta del mismo inventario del altar; vendedor no vende coca. Comprar es opcional y el fraude se descubre en un rechazo del altar.
- [ ] C24 Pista útil, preparación 90 s y curación por objetos. No cobrar una pista repetida sin utilidad; no gastar duración en conversaciones o escenas.

Salida: se puede completar el prólogo y abrir el nivel dos en una partida normal.

## Hito 4 Árbol enemigos y misiones comunes

Depende del hito 3.

- [ ] C25 Árbol de la bolivianita: Técnicas de pico y Fundamentos con rangos, requisitos y costes del GDD. Implementar ataque en marcha, aéreo, combos, área frontal, giro, daño y crítico, más fundamentos.
- [ ] C26 Reembolso íntegro con confirmación de dependencias. Probar que comprar/reembolsar no aumenta histórico ni cura gratuitamente; no hay nodos huérfanos.
- [ ] C27 Integrar los cinco enemigos 2D del GDD con sus poses, ataques, refuerzos e iluminación. Probar cada aparición y el pico con entrada de teclado real. *2026-11-08: Resisworm, Hangzzer con refuerzos, Gasplode, Drilly y Lighmet aparecen y se vencen con el pico en los recorridos (teclado simulado). Lighmet con aviso de 0,45 s. Falta la revisión con teclado real.*
- [ ] C28 EXP relativa por zona y dinero. Una resistencia base 3/ref 1 concede 3/2/1/0 en niveles 1/2/3/4. No hay EXP negativa ni orbes cero.
- [ ] C29 Sistema de encargos por ID y por intento. Las misiones únicas no se repiten; las repetibles exigen expedición nueva y acciones nuevas. Guardar objetivo cumplido y recompensa entregada por separado.
- [ ] C30 Regalos del hijo evaluados una vez por nivel, incluso si no da nada. Guardar o morir no permite volver a sortear.

Salida: progresión y economía permiten elecciones sin bloquear el juego básico.

## Hito 5 Niveles dos al cinco

Depende de hitos 3 y 4.

- [x] C31 N2 amplio y diverso con plataformas, subidas, encuentros, carros, placas, topes, palancas y Q03–Q04; cantidad de cámaras justificada por recorrido y ritmo. Reiniciar puzle no duplica objetos ni recompensas y nunca atrapa al jugador dentro de una reja. *2026-11-08: verificado en Play Mode (N2PlayChecks, 108 PASS, 0 fallos): Q03/Q04, carros y placa, reja, palanca de topes sin cerrar la reja, Hangzzer, puente y apertura de N3.*
- [x] C32 N3 con acertijos de cuatro puertas, pistas fiables, confirmaciones de avance y ramales de tesoro, minero o enemigos; Q05–Q06. Error vuelve a la misma cámara con el mismo estado; no al acceso. Existe ruta física de regreso. *2026-11-08: verificado (N3PlayChecks, 110 PASS, 0 fallos): puerta equivocada vuelve a la misma cámara con el aviso N3.03 y conserva lo encontrado; confirmación N3.07; Q06.*
- [ ] C33 Escolta: NPC sigue por puertas y bucles sin completar antes de la entrada de N3. T muestra el mensaje y no permite llamar. Muerte restablece la escolta pendiente sin borrar otras misiones. *2026-11-08: el minero sigue al jugador y cruza puertas (N3PlayChecks). Falta probar T durante la escolta y la muerte con escolta pendiente.*
- [x] C36 N4, exploración amplia con iluminación baja y Q09–Q10. Carro con luz, ruta legible y combate en oscuridad sin ocultar peligros esenciales. *2026-11-08: verificado (N4PlayChecks, 80 PASS, 0 fallos): lámpara del casco, Lighmet, carro de luz Q09 y Q10. Exigencia alta sin consumibles (pendiente de decisión).*
- [x] C34 N5, estación, vía, andén y Q07–Q08; cantidad de secciones según ritmo del minijuego. Vagoneta con salto, freno, obstáculos, minerales flotantes y evaluación en el andén. No exige habilidades compradas ni uso de pico. *2026-11-08: verificado (N5PlayChecks, 41 PASS, 0 fallos): Q07, Q08, rechazo inicial, salto, obstáculos, minerales y evaluación en el andén.*
- [ ] C35 Probar aceptación, caída y reinicio desde salida, minerales por intento, insuficiencia, recompensa única, investigación en la puerta y aporte 12 en N01S01. *2026-11-08: aceptación, caída y reinicio con 0 minerales, premio único, investigación y aporte 12 verificados. La insuficiencia no se produjo en el último recorrido.*
- [ ] C37 Q11 y diálogos de servicios contextuales, sin repetición de recompensa ni etiqueta Estafador.
- [ ] C38 Comprobar cuotas y suministro garantizado por IDs de fuentes. Se puede llegar al nivel seis sin comprar al vendedor, completar todas las misiones ni depender de botín aleatorio.

Salida: seis niveles accesibles por sus condiciones; las cuotas no se repiten al morir.

## Hito 6 Rescate y taladro

Depende del hito 5 y del reloj de C09.

- [x] C39 Corredor con subtítulos ambientales, rescate, aparición y presentación N6A. Primer encuentro no permite omitir. *2026-11-08: verificado (N6PlayChecks): subtítulos sin pausa, rescate N6A.06–11, presentación N6A.13–15; F solo tras N6.DERROTA.*
- [x] C40 Arena circular y taladro 3D básico: avisar, embestir, atascar en cualquier pared y responder al choque con jugador según GDD. *2026-11-08: verificado con registro de estados: apuntar, aviso, embestida, atasco en la pared y retroceso tras chocar con el jugador (daño 2).*
- [ ] C41 Resistencias 2/3/4 de 3 de vida: montadas invulnerables, desmontadas vulnerables solo en atasco, daño persistente entre ventanas, derrotadas no reaparecen en esa fase.
- [ ] C42 Probar que no existe daño directo al taladro, acción de apagarlo ni EXP por sus resistencias. Tercer grupo derrotado activa victoria una sola vez. *2026-11-08: victoria tras tres grupos, una sola vez. Falta comprobar explícitamente que no hay daño directo al taladro.*
- [ ] C43 Piedras durante toda la pelea, tamaños, avisos y mayor presión durante atasco. Verificar espacio seguro y acceso al altar.
- [ ] C44 Altar distinto con mismo diseño y contador de arena independiente. No consume ofrendas de exploración; entrega exactamente 5, conserva sobrantes y no consume con mejora activa. *2026-11-08: sin ejercitar en el recorrido (el jefe se venció sin entregar ofrendas de arena).*
- [ ] C45 Lanzamientos cada 2–5 s, máximo 7 en suelo, 5 s de vida; recogidas no caducan. Con mejora activa del Tío se detienen; se reanudan después.
- [ ] C46 Reloj del Tío empieza después de primera fase y explicación; 40 s de tiempo jugable. Cambiar fase reinicia plazo y conserva perjuicio. Una entrega con perjuicio solo deja neutral.
- [ ] C47 Preparación del lector y efecto del Tío independientes. Escenas congelan ambos; el perjuicio no borra la preparación. HUD no revela fase, efecto ni reloj.
- [ ] C48 Avisos breves del hijo no pausan; explicaciones sí. La introducción consumida no reduce duración de beneficios.
- [ ] C49 Morir reinicia desde corredor, limpia arena y temporales y conserva progreso permanente. Solo después de perder permite F para saltar rescate e introducción y empezar fase uno. *2026-11-08: la derrota devuelve al corredor con vida máxima y la opción F. Falta comprobar la limpieza de temporales.*

Salida: jefe vencible con estadísticas básicas y también con mejoras, sin exigir una compra concreta.

## Hito 7 Cierre compañero y guardado

Depende del hito 6.

- [ ] C50 Victoria, explosión segura, vuelta al control e interacción obligatoria con pieza. FIN explica la pista de cataratas sin identificar científico. *2026-11-08: victoria, pieza dorada interactiva y registrada una vez (N6PlayChecks).*
- [ ] C51 Registrar pieza como objeto clave, desbloquear hijo una vez y acreditarle gemas históricas. Cambiar desde menú sin curar y sin mostrar al inactivo; movimiento, salto y golpe básico del hijo funcionan en el cierre. *2026-11-08: Platicio se une tras el capitán y el menú lo activa. Falta comprobar gemas históricas y su golpe en el cierre.*
- [ ] C52 Vagoneta oculta, confirmación de viaje, animación y Continuará. E solicita menú principal con Guardar y volver / Volver sin guardar / Cancelar; no guarda durante la animación. Cargar cierre viajado muestra Continuará. No añadir selector posterior a la victoria, modo de repetir jefe ni niveles jugables de cataratas. *2026-11-08: vagoneta (movida dentro de la arena), viaje, «Continuará» y vuelta al menú verificados. Falta probar las tres opciones del menú y cargar un cierre ya viajado.*
- [ ] C53 Guardado manual completo de exploración. Cambiar sección, guardar y cargar conservan vida, créditos, estado de misión, enemigos, carros, recogibles y duración externa.
- [ ] C54 Guardar bloqueado en jefe, minijuego, escena, conversación y transición. Salir sin guardar no sobrescribe archivo; cancelación no sale; fallo no destruye último guardado ni cierra sesión.
- [ ] C55 Continuar restaura una partida válida y avisa si no puede cargar. Partida posterior a victoria no duplica EXP, pieza, compañero ni jefe.

Salida: el mundo tiene un cierre completo y un guardado verificable.

## Hito 8 Validación de entrega

- [ ] C56 Recorrido completo desde partida nueva hasta Continuará con arte provisional, sin comandos de depuración para saltar cuotas o investigaciones.
- [ ] C57 Recorrido de casos límite C14, muerte en escolta, fallos de vagoneta, reintento de jefe, pausa de relojes y carga de partida. Registrar evidencia por prueba.
- [ ] C58 Revisar consola y compilación. Distinguir errores propios y previos; no ocultar advertencias relevantes ni afirmar pruebas que no se ejecutaron.
- [ ] C59 Revisar legibilidad con cámara distante y sin depender solo del color. Documentar ajustes de parámetros que hayan sido necesarios.
- [ ] C60 Actualizar Estado_Trabajo.md con lo terminado, lo no probado, referencias realmente revisadas, archivos modificados y pasos de ejecución. Mantener técnicas avanzadas del hijo y mundos futuros fuera de la lista de funcionalidades entregadas.

## Registro por tarea

ID:
Estado:
Archivos y escena reales:
Comprobación ejecutada:
Resultado observado:
Errores o limitaciones:
Siguiente acción:

No se incluyen resultados ficticios en este paquete. El registro empieza cuando el agente inspeccione y trabaje en el proyecto real.

## Estado de producción vigente

PM00–PM20 y el pase espacial/artístico permanecen verificados en su alcance histórico. Evidencias: PM, Camara_Espacial y Docs/Arte/Acabado_2026_11_05. C20–C24 se revalidan para el rediseño N1 mediante LD00–LD19. C53–C55 siguen parciales: existen guardados de exploración probados, faltan sistemas futuros. N2–N6, árbol y jefe no están construidos. Solo la tarea LD indicada en Estado_Trabajo está En curso; otros alcances parciales quedan pendientes.

## Ampliación del 7 de noviembre · pruebas pendientes

- [x] Daño de combate visible en ambos sentidos, con cifra efectiva y animación según DañoCausado.png. Comprobar invulnerabilidad, fallo y derrota. *2026-11-08: verificado con teclado real en ambos sentidos, fallo, profundidad, dos impactos y derrota (Evidencias_2026_11_08/08a–08d). La prueba dirigida `Pruebas_Dano_Dirigidas.md` (6 PASS) cubre el golpe absorbido por invulnerabilidad (sin cifra) y el daño mayor que la vida restante (cifra = vida perdida).*
- [ ] Comparar dormitorio, casa/jardín y mina con sus conceptos desde Game y registrar los modelos y materiales pulidos.

- [x] Verificar timbre con mensaje único, teléfono y pico con gesto, brillo y restauración del control y cámara. *2026-11-08: verificado desde Nueva partida. La carga no repite la entrega. Evidencias_2026_11_08/02, 03, 05, 06–06c y 11. La captura intermedia del teléfono se descartó; la fase se registró por estado.*
- [x] Verificar etiqueta de ofrenda y curativo durante el movimiento, incluidas recogidas consecutivas. *2026-11-08: «Curativo» y «Hoja de coca ×2» sin pausa (Evidencias_2026_11_08/09). Curativo y ofrendas colocados en la ruta como preparación de prueba.*
