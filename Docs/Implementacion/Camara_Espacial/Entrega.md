# Cámara espacial, prólogo y nivel uno

La partida comienza en `M01_MENU`. La carta y el timbre abren el dormitorio. La puerta del dormitorio lleva al patio de 28 × 32 m. Los dos senderos rodean la fuente y vuelven a unirse antes del portón. E permite hablar con el capitán desde el interior. Su encargo abre directamente `M01_MAPA` con N1 enfocado. E inicia la expedición en `N01S01`.

La cámara acompaña al personaje en X, Y y Z. Cada habitación tiene límites de seguimiento, distancia e inclinación propios. En las salas profundas, la cámara cambia suavemente la distancia y el ángulo al avanzar. Se conservan las flechas de movimiento en los ejes del escenario y Espacio para saltar. Los pasos entre salas se activan con E, tienen fundido y dejan al personaje a distancia de la puerta de regreso.

| Escena | Recorrido |
| --- | --- |
| `N01S01` | Entrada segura con el hijo como único guía, teléfono, altar y dos puestos de encargos. La boca de mina lleva a las terrazas. Los senderos laterales permiten regresar al mapa y al pueblo. |
| `N01S02` | Galería ancha con isla de roca y terrazas de 0,55 m hasta la boca elevada a 2,2 m. Se puede rodear la isla y recoger una ofrenda en el ascenso. |
| `N01S03` | Cámara de trabajo con rieles en zigzag. El hijo entrega el pico, se activan las resistencias y se puede romper una roca opcional. La salida queda al fondo, en el lado opuesto a la entrada. |
| `N01S04` | Ramal inferior para el casco y terrazas para llegar al minero del derrumbe. El teléfono devuelve a N01S01, cerca del altar. |

Se prueban tres ofrendas garantizadas, la investigación y el crédito de 3 que abre N2. Sus escenas siguen pendientes. El selector vuelve a la entrada con Esc después de la primera expedición. Confirmar N1 de nuevo comienza otra expedición. Las puertas internas mantienen el intento en curso.

Las siete maquetas editables están en `ArtSource/Blender/Modelado_Afterglow/Afterglow_Prologo_Nivel1.blend`. La carpeta incluye los renders de revisión, el generador de geometría y la auditoría. Los FBX, materiales URP y planos de colisiones están en `Assets/Afterglow/Environment/Modelado`. Se usaron Blender 5.2.3, las skills de Scenario para modelado y exportación, y la skill de diseño de niveles. La búsqueda de `blender hard surface environment` devolvió complementos con 1–5 instalaciones. La selección se apoyó en las herramientas de Scenario ya probadas en el proyecto. [Scenario Skills](https://github.com/scenario-labs/skills)

El archivo de guardado usa esquema 3. Los esquemas anteriores conservan recursos, salud, encargos y progreso. Las posiciones de las salas rediseñadas se recolocan en una entrada segura de la misma sección. El acceso antiguo migra al mapa si el capitán ya habló y N1 aún no empezó. Si la expedición ya había empezado, migra a N01S01. Los guardados nuevos mantienen la posición exacta.

Las pruebas están en `Pruebas.md`, `Inspeccion.md` y `Guardados.md`. Las capturas numeradas son de Game View. La prueba normal usa teclas para caminar, saltar e interactuar. Para dirigir los golpes de combate usa la entrada de prueba del mismo controlador y el ataque básico real. El progreso se obtiene dentro del recorrido. Las pruebas de migración escriben en `Tools/PM/TestData`.
