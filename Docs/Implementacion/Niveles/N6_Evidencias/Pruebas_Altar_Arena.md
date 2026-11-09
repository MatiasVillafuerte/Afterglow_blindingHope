# Altar de la arena (C44) en Play Mode

Teclado simulado (Input System). Fixture: niveles anteriores completados por sus pruebas.

- PASS Selector abierto con N6 enfocado y escena disponible
- PASS Llegada a N06S01
- PASS Paso N06S01 (10,6, -1,2) a altura 0,04
- PASS Voces de los atrapados como subtítulos sin detener al jugador
- PASS Llegada a N06S02
- PASS Presentación y explicaciones N6A.13–15
- PASS Guardado bloqueado durante el jefe
Fixture de prueba: 3 ofrendas de exploración (Coca) en el inventario para comprobar que el altar de la arena no las consume.
- PASS Altar de la arena independiente del altar de exploración (sin M01Altar en N06S02)
Ayuda de prueba: taladro detenido con `Halt()` en la fase 1 mientras se ejercita el altar; piedras y lanzamientos de Platicio siguen activos. Después se reanuda con `Begin(1)` y la pelea sigue sin ayudas.
- PASS Con 0 ofrendas de arena el altar no entrega ni consume
- PASS Contador de arena independiente: 10 de arena; ofrendas de exploración 3 → 3
- PASS Entrega exactamente 5 y conserva sobrantes: 10 → 5 · efecto Mejora · exploración 3
- PASS Con mejora activa el altar rechaza sin consumir: 5 → 5
- PASS Con mejora activa Platicio no lanza ofrendas (6 s observados)
- PASS La mejora termina sola: 14,5 s
- PASS Los lanzamientos se reanudan tras la mejora
Ofrendas de arena guardadas para la prueba del perjuicio: 5
  · 61,0 s Apuntando vida 9 · jugador (-4.4, 0.0, -4.7) · taladro (0.0, 0.0, 3.0) · delante 8,6 · lateral 2,3 · bloqueo False
  · 61,8 s Aviso vida 9 · jugador (-2.1, 0.0, -6.4) · taladro (0.0, 0.0, 3.0) · delante 9,6 · lateral 0,0 · bloqueo False
  · 63,3 s Embistiendo vida 9 · jugador (-2.1, 0.0, -6.4) · taladro (0.0, 0.0, 3.0) · delante 9,6 · lateral 0,0 · bloqueo False
  · 65,2 s Atascado vida 9 · jugador (0.7, 0.0, -9.1) · taladro (-2.4, 0.0, -8.0) · delante 0,4 · lateral -3,3 · bloqueo False
  · 69,1 s Quieto vida 9 · jugador (-3.1, 0.0, -7.6) · taladro (-2.4, 0.0, -8.0) · delante -0,3 · lateral 0,8 · bloqueo False
  · 71,2 s Apuntando vida 9 · jugador (-3.1, 0.0, -7.6) · taladro (-2.4, 0.0, -8.0) · delante 0,8 · lateral 0,1 · bloqueo False
  · 71,2 s fase 2: se retienen la entrega y el ataque a las resistencias hasta el perjuicio
  · 71,5 s Aviso vida 9 · jugador (-4.4, 0.0, -6.3) · taladro (-2.4, 0.0, -8.0) · delante 2,6 · lateral 0,0 · bloqueo False
  · 73,0 s Embistiendo vida 9 · jugador (-2.2, 0.0, -0.9) · taladro (-2.4, 0.0, -8.0) · delante 7,2 · lateral 0,0 · bloqueo False
  · 75,5 s Atascado vida 9 · jugador (2.3, 0.0, -0.9) · taladro (3.3, 0.0, 7.7) · delante -7,5 · lateral 4,1 · bloqueo False
  · 82,5 s Apuntando vida 9 · jugador (3.9, 0.0, 4.7) · taladro (2.6, 0.0, 6.7) · delante -0,9 · lateral 2,2 · bloqueo False
  · 83,3 s Aviso vida 9 · jugador (3.9, 0.0, 1.6) · taladro (2.6, 0.0, 6.7) · delante 5,2 · lateral 0,0 · bloqueo False
  · 84,5 s Aviso vida 7 · jugador (-0.3, 0.0, -2.5) · taladro (2.6, 0.0, 6.7) · delante 9,6 · lateral 0,0 · bloqueo False
  · 84,8 s Embistiendo vida 7 · jugador (-1.4, 0.0, -3.7) · taladro (2.6, 0.0, 6.7) · delante 11,1 · lateral 0,0 · bloqueo False
  · 87,0 s Atascado vida 7 · jugador (-5.3, 0.0, 0.2) · taladro (-6.5, 0.0, -5.1) · delante -4,1 · lateral 3,6 · bloqueo False
  · 94,0 s Apuntando vida 7 · jugador (-5.3, 0.0, 0.2) · taladro (-5.5, 0.0, -4.4) · delante -2,9 · lateral 3,6 · bloqueo False
  · 94,8 s Aviso vida 7 · jugador (-2.5, 0.0, 3.1) · taladro (-5.5, 0.0, -4.4) · delante 8,1 · lateral 0,0 · bloqueo False
  · 96,3 s Embistiendo vida 7 · jugador (3.6, 0.0, 5.3) · taladro (-5.5, 0.0, -4.4) · delante 13,3 · lateral 0,0 · bloqueo False
  · 98,6 s Atascado vida 7 · jugador (8.8, 0.0, 0.2) · taladro (7.7, 0.0, 3.3) · delante 0,3 · lateral 3,3 · bloqueo False
  · 105,6 s Apuntando vida 7 · jugador (2.3, 0.0, 2.5) · taladro (6.5, 0.0, 3.0) · delante -4,3 · lateral -0,5 · bloqueo False
  · 106,4 s Aviso vida 7 · jugador (-0.8, 0.0, -0.5) · taladro (6.5, 0.0, 3.0) · delante 8,0 · lateral -1,2 · bloqueo False
  · 107,9 s Embistiendo vida 7 · jugador (-5.4, 0.0, -3.6) · taladro (6.5, 0.0, 3.0) · delante 13,7 · lateral 0,0 · bloqueo False
  · 110,1 s Atascado vida 7 · jugador (-9.1, 0.0, 3.0) · taladro (-8.4, 0.0, -0.2) · delante 1,0 · lateral 3,2 · bloqueo False
- PASS Sin entrega válida en 40 s llega el perjuicio del Tío (39,5 s desde la fase 2)
  · 117,2 s Quieto vida 7 · jugador (-8.7, 0.0, -1.5) · taladro (-8.4, 0.0, -0.2) · delante 0,2 · lateral -1,3 · bloqueo False
  · 119,8 s Apuntando vida 7 · jugador (-4.8, 0.0, -4.7) · taladro (-8.4, 0.0, -0.2) · delante 4,2 · lateral -3,8 · bloqueo False
- PASS Entregar 5 con perjuicio lo elimina: 11 → 6 · efecto Neutral
  · 120,5 s Aviso vida 7 · jugador (-4.8, 0.0, -4.7) · taladro (-8.4, 0.0, -0.2) · delante 5,7 · lateral 0,0 · bloqueo False
  · 121,7 s Embistiendo vida 7 · jugador (-0.2, 0.0, -4.2) · taladro (-8.4, 0.0, -0.2) · delante 9,0 · lateral 0,0 · bloqueo False
  · 123,8 s Atascado vida 7 · jugador (6.9, 0.0, 3.0) · taladro (8.3, 0.0, 0.0) · delante -0,2 · lateral -3,2 · bloqueo False
- PASS El taladro se atascó y bajaron resistencias
Entrega en el altar de la arena observada: True · fase alcanzada: 3 · efecto: Neutral
- PASS Perjuicio observado y eliminado con una entrega
- PASS Victoria contra PerforAudora tras tres grupos
- PASS Paso N06S02 (8,267778, -1,171907) a altura 0,04
- PASS Pieza dorada registrada como objeto clave una sola vez
- PASS Capitán, Platicio se une al equipo y la vagoneta aparece
- PASS Cambiar personaje desde el menú: Platicio activo
- PASS Paso N06S02 (5, -6,5) a altura 0,04
- PASS Viaje en vagoneta y pantalla Continuará
- PASS Llegada a M01_MENU

Recorrido N6 hasta Continuará: 143,4 s.
- PASS Guardado personal intacto

Finalizado. Fallos: 0
