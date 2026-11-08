# Checklist de perfeccionamiento del nivel uno

Versión 0.5. Integración iniciada el 2026-11-05. LD19 En curso; LD02, LD12 y LD18 pendientes de cierre documental. Evidencias actualizadas en Estado_Trabajo.md y Nivel1. El avance previo del checklist maestro C00–C60 y PM00–PM20 se conserva. Las casillas de contexto y diseño se respaldan en Plan_Nivel1.md, Metricas.md y Mapas_Nivel1.md; no equivalen a nivel terminado.

## Etapa A Contexto y sincronización

- [x] LD00 Leer documentación activa, estado real y la skill level-design instalada en el entorno del agente. Identificar su ubicación efectiva y utilizar sus instrucciones aplicables; no afirmar que se leyó por conocer el nombre.
- [x] LD01 Inspeccionar N1 real, controlador, cámara, puertas, progreso, arte, Blender y herramientas. Registrar qué se reutiliza y qué geometría debe rediseñarse. Conservar prólogo y selector funcionales.
- [ ] LD02 Integrar el canon oficial N1–N6 en GDD, diseño, guion, checklist, mapa y datos, aplicando Protocolo_Documentacion.md. Verificar N4 oscuridad y N5 vagoneta, con misiones y cuotas correctas.

## Etapa B Referencias y diseño propio

- [x] LD03 Analizar los once mapas de MapasNiveles y recortes de los atlas detallados. Distinguir observaciones comprobadas de hipótesis y miniaturas insuficientes. Registrar principios que se adaptan sin copiar salas.
- [x] LD04 Medir salto, velocidad, tamaño y visibilidad reales. Dibujar dos alternativas de estructura de N1, elegir una y justificarla brevemente por enseñanza, ritmo, profundidad y retorno. Continuar sin pedir aprobación por cámara.
- [x] LD05 Elaborar Plan_Nivel1.md y plano editable con cantidad variable de cámaras, ruta principal, ramales, puertas, apariciones, retos, anclas, recursos y final. No fijar el número por las filas del documento anterior.
- [x] LD06 Definir cada cámara por función, forma, alturas, recorrido, decisión y diferencia respecto a las vecinas. Comparar sin decoración y corregir copias del mismo cuarto o de las tres ilustraciones.
- [x] LD07 Seleccionar un mecanismo ambiental breve para el tutorial. Documentar interacción, efecto visible, retorno/reinicio, persistencia y relación con lo que se enseña. No añadir todas las mecánicas propuestas.

## Etapa C Montaje y validación temprana

- [x] LD08 Crear montaje básico en Blender con escala real, colecciones y partes separadas. Conservar .blend y automatización reproducible si se utiliza. Exportar una muestra y verificar escala, orientación y pivotes en Unity antes de extenderla.
- [x] LD09 Integrar el recorrido provisional completo de N1 con controlador/cámara existentes y puertas físicas. Probar entrada, salida y regreso sin cargas duplicadas ni puntos dentro de colliders.
- [x] LD10 Integrar tutorial de pico antes de agresión, roca rompible y mecanismo con causa/efecto. El enemigo y los diálogos conservan sus eventos. Probar el mecanismo y su reinicio en Play Mode. *2026-11-08: el pico y la roca se revalidaron con X real tras corregir el alcance (ver Estado_Trabajo).*
- [x] LD11 Comprobar ofrendas garantizadas, Q01/Q02, teléfono, investigación final y cuota 3. Llegar a abrir N2 por sus condiciones reales sin un botón de depuración. No implementar el recorrido de N2 en este encargo.
- [ ] LD12 Revisar ritmo y comprensión (2026-11-08: X legible y efectivo con teclado real; el perímetro de N01S02 provoca caídas en diagonal, pendiente de la prioridad 2): el jugador encuentra la ruta, aprende antes de ser presionado y usa X/Z/Y de forma legible. Eliminar caminatas vacías y transiciones sin función. Medir el recorrido en vez de inventar una duración.

## Etapa D Modelado y acabado

- [x] LD13 Modelar en Blender las cámaras ya validadas y sus piezas reutilizables: terreno con forma, roca, soportes, rieles y el mecanismo elegido. Conservar partes interactivas separadas y fuentes editables.
- [x] LD14 Exportar/importar geometría, preparar materiales compatibles con URP, prefabs y colliders sencillos. Confirmar que suelo, obstáculos y partes móviles coinciden con lo visible. Mantener personajes/NPC/enemigos 2D.
- [x] LD15 Aplicar luz y ambientación propias de N1 con la cámara aprobada. Ningún adorno oculta saltos, puerta o señales importantes. Capturas de juego muestran salas distinguibles por composición y función.

## Etapa E Integración y cierre

- [x] LD16 Recorrer N1 completo desde el selector normal, regresar al acceso y verificar que el prólogo y el mapa no se alteraron. Revisar compilación y consola.
- [x] LD17 Probar caída/muerte, regreso por puerta, teléfono, persistencia de mecanismo y guardado/carga. Mantener recursos, eventos, misiones y recompensas, sin duplicaciones al cambiar las cámaras.
- [ ] LD18 Revisar nuevamente la variedad sin decoración y la correspondencia entre plano, guion y geometría final. Registrar observaciones y correcciones concretas.
- [ ] LD19 Actualizar documentación vigente, checklist maestro y Estado_Trabajo.md con archivos reales, versión, pruebas, capturas, limitaciones y siguiente tarea. No declarar N2–N6 construidos ni marcar verificadas tareas no ejecutadas.

Una sola tarea de implementación En curso. Estados permitidos: Pendiente, En curso, Implementado sin probar, Verificado, Bloqueado y Pendiente de revalidación. Solo Verificado permite [x]. Si Unity no se pudo ejecutar, explicitarlo y conservar las tareas correspondientes sin marcar.


