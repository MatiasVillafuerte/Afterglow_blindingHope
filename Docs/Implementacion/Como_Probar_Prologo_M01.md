# Probar el avance del prólogo

Abre Assets/Afterglow/Scenes/M01/M01_MENU.unity y pulsa Play. Selecciona Nueva partida con E. Continúa con E en la carta; después usa flechas para caminar hacia la puerta de la derecha. En el patio, acércate al capitán y pulsa E. La conversación lleva al acceso.

En el acceso se presentan el hijo, el teléfono y el altar. Avanza caminando hacia la puerta N01. Las tres salas N01S02–S04 tienen puertas de ida y regreso. El pico se entrega antes de activar el enemigo tutorial. X ataca, Espacio salta, I abre categorías del inventario, T solicita retorno y Esc abre pausa. El casco de Q02 está en el ramal de N01S04. Los encargos se aceptan y cobran junto a sus mineros en el acceso. Desde la puerta izquierda puedes visitar el pueblo y sus servicios.

Esto es un avance con geometría y actores provisionales. Todavía no existe la puerta jugable a N02 ni el resto de niveles, árbol, vagoneta, taladro o cierre. No se presenta como mundo uno terminado. N01S04 permite registrar investigación y regresar al altar, pero el desbloqueo del nivel siguiente todavía no tiene destino físico implementado.

El guardado es manual en Esc > Guardar. Continuar solo aparece en el inicio si hay una partida válida. Las pruebas automáticas usaron un archivo aparte en Tools/M01/TestData, sin crear una partida personal ni sobrescribirla. Salir en Play Mode termina Play; en un ejecutable cierra la aplicación. No existe guardado automático.

Pruebas reales: Pruebas_Correcciones_Visuales.md, Pruebas_Prologo_y_Salas.md, Pruebas_Servicios_M01.md, Pruebas_Guardado_M01.md, Pruebas_Muerte_Expedicion.md y Pruebas_Menu_M01.md. Hay pruebas con recursos sembrados y recolocación de jugador; falta el recorrido completo sin depuración de C56.
