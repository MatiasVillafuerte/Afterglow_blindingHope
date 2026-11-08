# Migración semántica del canon — esquema 4

Los esquemas 1–3 asignaban N4 a vagoneta y N5 a oscuridad. El esquema 4 usa N4 oscuridad y N5 vagoneta. No existen escenas de producción de esos niveles en esta entrega; aun así, se conserva el contenido de partidas y fixtures anteriores.

| Dato | Política |
| --- | --- |
| Investigaciones, foco y nivel actual | Se trasladan por identidad: vagoneta 4→5; oscuridad 5→4. |
| Q07/Q08 y Q09/Q10 | Conservan ID, contador, intento, estado y recompensa pagada; solo cambia su asociación de nivel. |
| Créditos de altar | Permanecen en su tramo ordinal, porque son pagos ya realizados de cuotas 9/12, no objetos del escenario. No se pierden ni intercambian ni cobran otra vez. |
| Accesos ya abiertos | Se conservan. Si estaba abierta la antigua vagoneta N4, se conserva el derecho de acceso a su nueva posición N5; N4 queda disponible como conexión. Excepción de compatibilidad única, registrada con CANON.V05.MIGRATED, no regla de partidas nuevas. No se completa investigación ni se concede dinero/ofrendas. |
| IDs espaciales/eventos/fuentes | Equivalencia explícita de prefijos N04S/N05S, N04./N05., M01N04/M01N05 y N4./N5. El resto queda intacto. Se aplica también a fuentes ya recogidas para evitar duplicación. |
| Escena futura no disponible | Regreso seguro a N01S01; conservar progreso, salud, inventario y snapshots migrados. No intentar cargar una escena inexistente. |
| Recursos, mejoras, vida, cierre | Sin cambios. Nunca se borra ni sobrescribe un guardado al leerlo. Esquema 4 evita aplicar la migración dos veces. |

La recolocación por geometría de N1 se registra por separado cuando su montaje esté integrado. Esta migración no declara construidos N2–N6. Pruebas con archivos aislados; nunca usar el guardado personal como fixture.

## Recolocación del montaje N1 — esquema 5

Los guardados de esquema 1–4 pasan primero por el canon y luego conservan su sección N1, reapareciendo en su entrada segura. El botín pendiente conserva tipo, cantidad y tiempo y se coloca junto a esa entrada. Los tres enemigos conservan salud y derrota, recolocados en sus anclas correspondientes. Eventos, fuentes, misiones y puente abierto se conservan. Esquema 5 guarda posiciones exactas del nuevo montaje y no aplica de nuevo la recolocación. Pruebas de esta segunda migración pendientes de ejecución.
