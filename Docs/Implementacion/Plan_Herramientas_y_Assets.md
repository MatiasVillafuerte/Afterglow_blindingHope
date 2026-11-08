# Plan de herramientas y assets — 2026-11-02

Evaluación presentada antes de programar. Versiones obtenidas de manifiesto y PackageCache. Proyecto Unity 6000.5.5f1 / URP 17.5.0. No se reinstala ni actualiza por rutina.

| Nombre y tipo | Función en Afterglow / necesidad | Fuente oficial / versión compatible | Licencia y coste | Estado / puedo instalar |
|---|---|---|---|---|
| Unity agent plugin / agente | Skills CLI y UI; imprescindible | https://github.com/Unity-Technologies/unity-agent-plugin ; 0.1.8-beta local | Unity Companion License; sin compra adicional | Ya instalado y skills leídas; no reinstalar |
| Unity CLI y Pipeline / conexión | Escenas, consola y pruebas; imprescindible | https://docs.unity.com/unity-cli ; CLI 1.0.0-beta.15, Pipeline 0.9.1-exp.1 requiere Unity 6000.0 según package.json | Pipeline: Unity Terms of Service según LICENSE.md local; sin compra adicional | Ya instalado; carga del editor pendiente. Puedo usar CLI; no actualizar |
| Input System / Unity | Controles existentes; imprescindible | https://docs.unity3d.com/Packages/com.unity.inputsystem@1.19/manual/index.html ; 1.19.0, mínimo 2022.3 declarado | Unity Companion License local; sin coste adicional | Ya instalado; conservar |
| uGUI/TMP / Unity | HUD, diálogo y menús; imprescindible | https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/index.html ; 2.5.0 local requiere 6000.5 | Unity Companion License local; sin coste adicional | Ya instalado; reutilizar fuente y retrato |
| CharacterController y primitivas / motor | Movimiento 3D, montaje gris y taladro; imprescindible | API editor 6000.5.5f1 local | Licencia del motor existente; sin coste nuevo | Ya instalado; sin descarga |
| Sprites, rig por piezas, PlayerAC y 5 clips / contenido | Protagonista; imprescindible | Reference/CharacterSource/Animaciones.zip y Assets/Afterglow/Character | Aportados por usuario para este proyecto; no se presume licencia de redistribución separada | Ya importados; conservar GUID y copias originales |
| ProBuilder / geometría | Modelado gris manual; opcional | https://github.com/Unity-Technologies/com.unity.probuilder ; 6.x candidata, revisión concreta no validada aquí | Unity Companion License en repositorio; gratuito | Descartado por ahora: primitivas suficientes. No instalado |
| Framework de diálogo externo / paquete | Conversaciones; opcional | Sin candidato necesario: guion por ID y TMP existentes | Código propio, sin coste nuevo | Descartado: evitar dependencia innecesaria |
| AudioClip.Create / sonidos propios | Timbre y señales; útil | API del motor existente | Sonidos procedurales propios; sin coste | Propuesto; no implementado aún |
| Cinemachine / cámara | Cámara alternativa; opcional | Ninguna revisión elegida | No aplica | Descartado: cámara aprobada no se sustituye |

No hay dependencia imprescindible que requiera adquisición, login, pago o acción manual del usuario. No se compra ni se suscribe nada. No se cambia Unity, URP, Packages ni ProjectSettings para importar personaje. No se descargan assets decorativos. Las imágenes de terceros son referencias de composición, no recursos del juego.

Cada incorporación se comprobará con compilación y consola; la cámara se contrastará con el baseline. Encontrar un paquete no equivale a haberlo instalado. No hay instalaciones nuevas realizadas en esta evaluación.

Actualización PM v0.4: conexión operativa con editor 6000.5.5f1/Pipeline 0.9.1-exp.1; CLI actualizada desde beta.15 a 1.0.0-beta.16 mediante self-update oficial. URP 17.5.0/manifiesto conservados. Prologo y maqueta de mapa usan geometría nativa editable, materiales propios y uGUI existente; no hubo compras ni nuevos paquetes. Timbre procedural ya integrado y probado. Blender 5.2.3/skills Scenario se conservan para futuros pases; esta integración no vuelve a buscar ni instalar catálogos. Ver PM/Plan_y_evidencia.md y PM/Como_Probar_Entrega.md.
