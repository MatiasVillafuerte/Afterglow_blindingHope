# Camera and connected-world revision

This revision supersedes the original 100m single-scene layout. The original playtest reports describe that earlier layout. Unity 6000.5.5f1 and URP 17.5.0 are unchanged.

Use **Afterglow > Build Connected World** to reproduce the current world. The old Build Prototype menu produces the earlier single-room layout; use the connected-world menu for this revision. Both refuse unsaved open scenes. Only generated Afterglow scenes are replaced.

Scenes: Afterglow_Prototype -> Afterglow_Courtyard -> Afterglow_Gallery -> Afterglow_Summit. Doors activate automatically when the player enters the dark opening. Health and EXP carry across doors; death/fall resets the current room. A new Play session starts without saved travel state.

Room lengths are 44, 38, 50 and 32m. Depth varies within each room: 8/13/9, 14/9/16, 9/16/11 and 12/15/10m. Foreground lanes remain flat while rear lanes have 1.2m steps and a 4.8m landing. The first door is on the upper landing; the second is an interior ground-level opening, and the third is towards the rear of the gallery, before the elevated route. Four temporary enemies remain, one per room.

Camera: stable perspective, 5 degree pitch, offset (0,3.8,-17), FOV 40, smooth lateral tracking and limited vertical tracking. The camera-facing boundary is a tall, invisible collider at z=-3.35; the player cannot walk or jump past it. The visual floor extends beneath the camera to avoid exposing a front fascia/abyss. Rear walls fill the backdrop.

Reference inspected: References/Guia para la camara.mp4 at second 21, decoded locally to References/VideoReferenceFrame.png. No reference graphics are imported into the game.

Validation is recorded in ConnectedWorldChecks.md. That test isolates the boundary with keyboard input, places the player at the start of the raised route, then climbs using keyboard movement and grounded jumps. The remaining doors are tested by placing the player at each entrance to isolate transition and EXP preservation; this is not a claim of a full uninterrupted playthrough of all new rooms.
`nFinal verification: front boundary stopped player at z=-2.63; raised route reached Courtyard; Gallery and Summit transitions passed with EXP preserved. Final console: 0 errors, 0 warnings. Initial scene left open in Edit Mode. All four scenes are enabled alongside SampleScene in build settings.
