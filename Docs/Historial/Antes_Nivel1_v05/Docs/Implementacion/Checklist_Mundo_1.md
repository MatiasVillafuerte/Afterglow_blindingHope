# Afterglow Checklist de implementación del mundo uno

Versión 0.3. Estado inicial de este archivo: ninguna implementación verificada. No marcar una casilla por crear archivos: se exige comportamiento en escena y evidencia. El repositorio real de Unity debe inspeccionarse antes de completar el primer hito.

## Cómo trabajar

Seguir los hitos en orden de dependencia. En cada tarea registrar estado Pendiente, En curso, Implementado sin probar, Verificado o Bloqueado. Una casilla [x] solo significa Verificado. Anotar archivos reales modificados, escena, prueba, resultado y errores relevantes en Estado_Trabajo.md. No inventar rutas de scripts antes de inspeccionar el proyecto.

Si una referencia visual falta, avanzar con el diseño de niveles y registrarlo; no bloquear código independiente. Si falta un recurso definitivo, utilizar el provisional indicado. Si una decisión contradice este paquete, documentar el conflicto y consultar solo ese punto. No reabrir como preguntas las reglas ya especificadas.

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
- [x] C06 Pico frontal de suelo: 1 daño, detiene movimiento, una aplicación por objetivo y acción. Probar que mantener contacto no multiplica daño y X en aire no ataca sin habilidad.
- [x] C07 Salud, 1 s de invulnerabilidad y daño sin empuje ni cancelación. Probar que se puede seguir atacando al recibir daño y que no hay daño por enemigo quieto.
- [x] C08 Caída: −2 y regreso a puerta de sección; con cero aplicar muerte e inicio de nivel. Probar ambas condiciones.
- [ ] C09 Reloj de juego y diálogos con escritura progresiva. Probar pausa de IA, botín y beneficios, mientras texto y botones siguen funcionando. E completa frase y después avanza; mantener E no omite todo.

Salida: escena base jugable con pico, salud, cámara y pausa coherentes.

## Hito 2 Recursos altar teléfono y estado

Depende del hito 1.

- [ ] C10 Inventario de consumibles, ofrendas, misiones y Objetos clave, con procedencia interna oculta. Cabeza y contadores del HUD conectados al estado real.
- [x] C11 Dinero, orbes y gemas separados. Probar 99+2 EXP: nivel siguiente, EXP 1, 3 gemas por miembro desbloqueado y curación del activo. Probar también varios umbrales en una recogida.
- [ ] C12 Botín de enemigo 5 s, parpadeo últimos 3 y una sola recogida. Objetos estáticos y de misión no caducan. Pausa congela tiempo restante.
- [ ] C13 Altar de exploración con cuotas 3, 5, 7, 9 y 12 y aportaciones parciales. Probar sin ofrendas, insuficientes, completas y excedentes; no cobrar dos veces.
- [x] C14 Probar cuota 5 y crédito 0 con 6 legítimas+2 robadas: quedan 1 legítima+2 robadas, crédito 5 y no hay piedra. Probar crédito 1 con 2 legítimas+1 robada: crédito 3 y robada eliminada, piedra sin daño.
- [ ] C15 Apertura exige investigación terminada y cuota completa. Probar cumplirlas en ambos órdenes; anuncio único y puerta abierta sin otro pago.
- [ ] C16 Teléfono por T, regreso al acceso y ficha solo descriptiva en Objetos clave. Probar llamada antes del final: conserva recursos y no marca investigación.
- [ ] C17 Bloqueos de teléfono durante escolta, jefe, vagoneta, conversación, escena y caída. Probar que consultar ficha no ejecuta llamada y que tras cerrar mundo queda inactivo.
- [x] C18 Estado de expedición. Cruzar puertas, repetir cámara y cargar no renuevan nada; salida voluntaria y nueva entrada reponen fuentes renovables. Morir repone enemigos, conserva estáticos tomados y no renueva encargos ni regalos.

Salida: circuito entrar, investigar, recoger, volver, aportar y abrir funciona sin duplicar recursos.

## Hito 3 Prólogo y primer nivel completo

Depende del hito 2.

- [x] C19 Carta, casa, patio, timbre y capitán usando P01–P02. El control empieza tras la carta; el jugador llega caminando al portón. No revelar al científico ni una desaparición de padres.
- [ ] C20 Pueblo y acceso seguro con altar, puertas y actores por rol. Habitante vendedor tiene mismo nombre y color que otros habitantes.
- [ ] C21 N01S01–S04 y tutorial N1A–N1D. Dar pico antes de agresión, teléfono una vez y primer botín demostrativo. Mantener diálogos alineados con las teclas reales.
- [ ] C22 Q01 y Q02: aceptar, contar nuevas bajas, buscar, devolver y cobrar. Morir después de una entrega única no permite repetir recompensa.
- [ ] C23 Servicios de tienda, lector y vendedor. Coca se descuenta del mismo inventario del altar; vendedor no vende coca. Comprar es opcional y el fraude se descubre en un rechazo del altar.
- [ ] C24 Pista útil, preparación 90 s y curación por objetos. No cobrar una pista repetida sin utilidad; no gastar duración en conversaciones o escenas.

Salida: se puede completar el prólogo y abrir el nivel dos en una partida normal.

## Hito 4 Árbol enemigos y misiones comunes

Depende del hito 3.

- [ ] C25 Árbol de la bolivianita: Técnicas de pico y Fundamentos con rangos, requisitos y costes del GDD. Implementar ataque en marcha, aéreo, combos, área frontal, giro, daño y crítico, más fundamentos.
- [ ] C26 Reembolso íntegro con confirmación de dependencias. Probar que comprar/reembolsar no aumenta histórico ni cura gratuitamente; no hay nodos huérfanos.
- [ ] C27 Resistencia, condensador y minitaladro completos; después LED y cable. Batería en el pulido. Probar aviso, daño, rango, movilidad y recuperación con acciones básicas.
- [ ] C28 EXP relativa por zona y dinero. Una resistencia base 3/ref 1 concede 3/2/1/0 en niveles 1/2/3/4. No hay EXP negativa ni orbes cero.
- [ ] C29 Sistema de encargos por ID y por intento. Las misiones únicas no se repiten; las repetibles exigen expedición nueva y acciones nuevas. Guardar objetivo cumplido y recompensa entregada por separado.
- [ ] C30 Regalos del hijo evaluados una vez por nivel, incluso si no da nada. Guardar o morir no permite volver a sortear.

Salida: progresión y economía permiten elecciones sin bloquear el juego básico.

## Hito 5 Niveles dos al cinco

Depende de hitos 3 y 4.

- [ ] C31 N02S01–S04, carros, placas, topes, palanca y Q03–Q04. Reiniciar puzle no duplica objetos ni recompensas y nunca atrapa al jugador dentro de una reja.
- [ ] C32 N03S01–S05 y Q05–Q06. Error vuelve a la misma cámara con el mismo estado; no al acceso. Existe ruta física de regreso.
- [ ] C33 Escolta: NPC sigue por puertas y bucles sin completar antes de N03S01. T muestra el mensaje y no permite llamar. Muerte restablece la escolta pendiente sin borrar otras misiones.
- [ ] C34 N04S01–S04 y Q07–Q08. Vagoneta con salto, freno, obstáculos, guía de dinero y andén. No exige habilidades compradas ni uso de pico.
- [ ] C35 Fallar vagoneta: −2 y estación o muerte completa con cero. Lo recogido no se duplica, guardar y teléfono siguen bloqueados. Llegar al andén marca investigación, fallar no.
- [ ] C36 N05S01–S04 y Q09–Q10. Carro con luz, ruta legible y combate en oscuridad sin ocultar peligros esenciales.
- [ ] C37 Q11 y diálogos de servicios contextuales, sin repetición de recompensa ni etiqueta Estafador.
- [ ] C38 Comprobar cuotas y suministro garantizado por IDs de fuentes. Se puede llegar al nivel seis sin comprar al vendedor, completar todas las misiones ni depender de botín aleatorio.

Salida: seis niveles accesibles por sus condiciones; las cuotas no se repiten al morir.

## Hito 6 Rescate y taladro

Depende del hito 5 y del reloj de C09.

- [ ] C39 Corredor con subtítulos ambientales, rescate, aparición y presentación N6A. Primer encuentro no permite omitir.
- [ ] C40 Arena circular y taladro 3D básico: avisar, embestir, atascar en cualquier pared y responder al choque con jugador según GDD.
- [ ] C41 Resistencias 2/3/4 de 3 de vida: montadas invulnerables, desmontadas vulnerables solo en atasco, daño persistente entre ventanas, derrotadas no reaparecen en esa fase.
- [ ] C42 Probar que no existe daño directo al taladro, acción de apagarlo ni EXP por sus resistencias. Tercer grupo derrotado activa victoria una sola vez.
- [ ] C43 Piedras durante toda la pelea, tamaños, avisos y mayor presión durante atasco. Verificar espacio seguro y acceso al altar.
- [ ] C44 Altar distinto con mismo diseño y contador de arena independiente. No consume ofrendas de exploración; entrega exactamente 5, conserva sobrantes y no consume con mejora activa.
- [ ] C45 Lanzamientos cada 2–5 s, máximo 7 en suelo, 5 s de vida; recogidas no caducan. Con mejora activa del Tío se detienen; se reanudan después.
- [ ] C46 Reloj del Tío empieza después de primera fase y explicación; 40 s de tiempo jugable. Cambiar fase reinicia plazo y conserva perjuicio. Una entrega con perjuicio solo deja neutral.
- [ ] C47 Preparación del lector y efecto del Tío independientes. Escenas congelan ambos; el perjuicio no borra la preparación. HUD no revela fase, efecto ni reloj.
- [ ] C48 Avisos breves del hijo no pausan; explicaciones sí. La introducción consumida no reduce duración de beneficios.
- [ ] C49 Morir reinicia desde corredor, limpia arena y temporales y conserva progreso permanente. Solo después de perder permite F para saltar rescate e introducción y empezar fase uno.

Salida: jefe vencible con estadísticas básicas y también con mejoras, sin exigir una compra concreta.

## Hito 7 Cierre compañero y guardado

Depende del hito 6.

- [ ] C50 Victoria, explosión segura, vuelta al control e interacción obligatoria con pieza. FIN explica la pista de cataratas sin identificar científico.
- [ ] C51 Registrar pieza como objeto clave, desbloquear hijo una vez y acreditarle gemas históricas. Cambiar desde menú sin curar y sin mostrar al inactivo; movimiento, salto y golpe básico del hijo funcionan en el cierre.
- [ ] C52 Vagoneta oculta, confirmación de viaje, animación y Continuará. E solicita menú principal con Guardar y volver / Volver sin guardar / Cancelar; no guarda durante la animación. Cargar cierre viajado muestra Continuará. No añadir selector, modo de repetir jefe ni niveles jugables de cataratas.
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


## Registro real — 2026-11-02

C00–C08 y C14: Verificados en el alcance de base M01. Evidencia: Estado_Trabajo.md, Pruebas_Base_M01.md, Prueba_Ruta_Base.md, Pruebas_Recursos_M01.md y Verificacion_Baseline.json. Las escenas antiguas, cámara, manifiesto y versión conservan hashes iniciales. No se consideran verificados el prólogo ni los niveles narrativos.

C09: Implementado y probado texto, E/hold, reloj compartido y pausa de botín. Pendiente regresión del beneficio del lector y de las IA futuras cuando existan.
C10: En curso; HUD, ofrendas y ficha descriptiva de teléfono conectados. Faltan consumibles y misiones.
C11: En curso; umbrales/histórico probados; recogida integrada de EXP y dinero en última pasada de ResourceChecks.
C12: En curso; 5 s de vida, parpadeo y reloj pausado implementados. Falta abandono de expedición y restauración de botín guardado.
C13: En curso; confirmación, aporte parcial y suficiente, excedentes y rechazo sin daño probados. Falta integrar todos los altares de niveles.
C15: En curso; condiciones en ambos órdenes y anuncio único probados. Falta puerta física al siguiente nivel.
C16: En curso; confirmación y retorno sin perder recursos probado en base; falta regreso entre escenas al acceso real.
C17: En curso; escolta y diálogo bloquean llamadas. Falta probar jefe/vagoneta/cierre integrados.
C18–C60: Pendientes salvo documentación de continuidad parcial. No existe todavía el recorrido narrativo completo ni guardado manual.

C11 final: Verificado también con orbe real (99+2), curación del activo, actualización del jugador y dinero separado en escena. Ver Pruebas_Recursos_M01.md.


C18 y C19: verificados para escenas actuales; Pruebas_Prologo_y_Salas.md, Pruebas_Muerte_Expedicion.md y Pruebas_Guardado_M01.md. Registro 2026-11-03 en Estado_Trabajo.md. C20–C24 en curso; servicios y Q01/Q02 probados, faltan recorrido integrado, puerta N02 y casos adicionales. C53–C55 tienen guardado inicial probado pero pendientes de sistemas futuros. Las correcciones visuales están verificadas por separado, no dan por terminado el mundo.

## Subtarea C20 — selector de acceso, entrega v0.4
PM00–PM08 verificados para prólogo y base actual; evidencia en PM/Pruebas_Prologo.md. PM09 en curso: selector modal compartiendo progreso/altar existentes. Se mantiene el estado global de C20 y C53–C60 hasta completar su alcance completo. Esta reordenación da prioridad al prólogo y selector antes de árbol/jefe/seis niveles; no desmarca casillas anteriores.

Cierre PM v0.4: PM00–PM20 verificados dentro del alcance prólogo + selector. Evidencia detallada en Estado_Trabajo.md y carpeta PM. Se preservan todas las casillas anteriores del maestro; los niveles y sistemas posteriores continúan pendientes. C20 incorpora selector funcional en acceso, con N2–N6 declarados sin escena. Las verificaciones de guardado y cierre del selector no completan jefe/cierre/vagonetas futuros. Siguiente: pase de N01S02 según PM/Siguiente_Pase_N1.md.

El pase de cámara espacial rediseñó N01S01–N01S04 y probó la expedición completa con el salto básico, combate, tres ofrendas, investigación y crédito del altar. La evidencia está en Camara_Espacial/Entrega.md. Se conservan las casillas históricas del maestro. La apertura de N2 queda verificada como progreso y sus escenas siguen pendientes de producción.
