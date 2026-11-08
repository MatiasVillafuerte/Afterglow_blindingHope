# Base técnica M01 — punto de trabajo, no entrega del mundo completo

Abrir `Assets/Afterglow/Scenes/M01/M01_Base.unity` y pulsar Play. El constructor reproducible es **Afterglow > Mundo 1 > Build Foundation**. Rechaza escenas abiertas con cambios sin guardar. Reutiliza Player.prefab y copia la cámara y HUD de Afterglow_Prototype sin guardar cambios en ella. No invocar los constructores anteriores para generar el mundo narrativo.

Esta escena da pico y teléfono como kit técnico para probar sistemas. No sustituye la carta, casa, patio, capitán, tutorial ni la entrega narrativa de objetos.

## Recorrido de prueba manual

1. Flechas: movimiento XZ. Espacio: salto básico; el hueco central se supera sin mejoras. X: pico en suelo, con parada durante la acción. El ataque aéreo y en marcha están deshabilitados por defecto en M01PlayerRules.
2. Acercarse al actor celeste y pulsar E: tres líneas literales N1A.05–07. Texto progresivo; E completa primero la frase y luego avanza. Mantener E no recorre la conversación. El juego y sus temporizadores se pausan; el texto usa tiempo de interfaz.
3. La resistencia está cerca de (11,0,-1). Tres golpes válidos la derrotan; el botín concede EXP, dinero y una ofrenda tutorial garantizada. El botín caduca tras 5 s jugables y parpadea desde el segundo 2. No se puede farmear por contacto.
4. Tras saltar el hueco, recoger las ofrendas fijas entre x=25 y x=31. No caducan. I abre el inventario: los tipos y la ficha del teléfono son visibles; la procedencia interna no se muestra.
5. T solicita confirmación con flechas y E. Volver mantiene recursos y no completa investigación. La ficha de teléfono nunca ejecuta llamada. En esta base el regreso es a una posición del mismo escenario; la conexión al acceso narrativo entre escenas está pendiente.
6. Altar cerca de (17,0,-1): E presenta las unidades que se tomarán; confirmar aplica la transacción una sola vez. Las robadas se rechazan solo si no alcanzan las legítimas. El rechazo muestra una piedra visual sin collider y no causa daño.
7. Hablar con el trabajador amarillo al final marca investigación tras terminar N1D.01–03. Cuota más investigación permiten un anuncio único tras 2 s jugables. Esta base registra apertura, pero aún no genera la puerta física al siguiente nivel.
8. Caer por el hueco resta 2 y devuelve a la entrada de sección. Una caída letal o muerte restaura la vida máxima y vuelve al inicio del nivel sin borrar EXP/inventario de sesión. El restablecimiento de enemigos y las expediciones completas están pendientes de C18.

## Pruebas automatizadas reproducibles

En Play Mode, tras `unity status --until-ready --project-path . --format json`, usar `unity command script run --caller plugin --skill unity-cli --project-path . --format json -- --file Tools/M01/FoundationChecks.cs --entry M01FoundationChecks.Main`. Ejecutar cada driver sobre una sesión nueva para no mezclar escenarios de prueba.

ResourceChecks.cs / M01ResourceChecks.Main prueba transacciones, umbrales, orbes/dinero reales, inventario, teléfono, confirmación, rechazo y caducidad. TraverseBase.cs / M01TraverseBase.Main aísla plataformas. Los drivers colocan actores o datos para casos concretos; no equivalen a una partida narrativa completa. Los informes están junto a este documento y diferencian esas condiciones.

No hay todavía guardado manual, misiones, servicios, árbol, seis niveles, minijuego, jefe o cierre. El estado en memoria se descarta al terminar Play. El siguiente trabajo debe completar C09–C18 pendientes y después continuar C19–C24 según Estado_Trabajo.md.
