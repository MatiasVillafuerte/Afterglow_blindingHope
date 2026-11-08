# Prólogo y mapa: ejecución PM00–PM20

PM00 Verificado: documentos v0.4 leídos; comparación con entrega confirma que GDD y guion no cambiaron. Se conserva el checklist maestro local y su historia. Base reutilizable: diálogo paginado, inventario, guardado manual, altar, escenas N01S01–04, animación y cámara frontal aprobada. N2–N6 carecen de escenas jugables.

PM01 Verificado: 2026-11-03, Unity 6000.5.5f1, editor PID 8468, Pipeline en localhost:7800 ready, consola 0 errores/0 avisos. Se puede operar el editor, además de editar archivos. No hacen falta paquetes nuevos. URP 17.5.0 se conserva.

PM02 Verificado: documentación nueva integrada sin sobrescribir archivos existentes (inventario en Tools/PM/Integracion_Documentos.json). Hash inicial de escenas, cámara, versiones y animaciones en Tools/PM/Baseline.json. Orden: prólogo PM03–08, selector PM09–15, pruebas PM16–19, cierre PM20. Solo una tarea de implementación en curso.

Inventario previsto antes de editar: M01RouteDoor/M01Prologue/M01StoryPoint/M01PlayerRules (interacción y gestos); nuevo PMProductionBuilder (modifica PRO_CASA/PRO_PATIO mediante API del editor); orden de escenas de Build Settings; M01Dialogue (propiedad modal compartida); M01Progress/M01Save (foco de mapa/migración); nuevo selector y su constructor en M01_ACCESO. No se regeneran N01S02–04 ni la muestra de arte.

Escala: la profundidad del jardín se adapta a la cámara frontal existente con Z fija. Se conserva casa al fondo, fuente central con dos caminos y capitán fuera del portón lateral, sin modificar cámara ni ampliar personajes. Referencias conceptuales se interpretan como arquitectura, no como assets finales.

PM03–PM15 verificados: ver registros por ID en Estado_Trabajo.md y pruebas en esta carpeta. PM16–PM20 verificados: recorrido final desde Nueva partida sin sembrar progreso, pruebas, capturas y cierre registrados. Siguiente pase de N1 documentado y no iniciado.

Pruebas posteriores se registran por ID con resultado observado; código compilado por sí solo no cierra PM08 ni PM16.


