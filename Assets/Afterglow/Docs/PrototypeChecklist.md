# Afterglow prototype

Implementation and verification are recorded separately. Source: References/Animaciones.zip, extracted outside Assets into References/CharacterSource (read-only source).

| Stage | Implementation | Verification |
|---|---|---|
| 1. Destination, character, tools | Inspected Unity 6000.5.5f1, URP 17.5.0, Input System 1.19.0; Pipeline ready. All five clips exist. Source has no player scripts or prefab, uses articulated SpriteRenderers. | Live connection and source hierarchy inspected. |
| 2. Character and controller | Complete: original articulated hierarchy, original 5 clips/controller, transparent unlit sprites; CharacterController with Input System actions. | Play Mode: all four arrows, normalized diagonal, grounded jump, no double jump, air steering. JumpTrajectory.md observes Idle/Walk/Jump/Falling every frame; ExtraChecks.md observes Attack in the air and return to Idle. |
| 3. Level and camera | Complete: 100 x 10m, safe base, steps up to 1.2m, upper terraces, lower fall limit, stable perspective camera. | Full route traversed with normal collisions and single jumps; camera viewport monitored. Constructor executed twice with 22 roots both times. |
| 4. Combat and enemies | Complete: 4 planar enemies, directional melee, per-attack damage, health, wind-up, invulnerability, edge and wall guards. | Three hits kill a 3HP enemy; holding X produces one strike; wrong direction, range and fixed different height rejected. Enemy detection, pursuit, tilt and paced damage passed. ExtraChecks verifies edges, walls, inaccessible floors and airborne attack. |
| 5. EXP, HUD, restart | Complete: 3 circles per kill, one-shot pickup, head portrait, responsive Canvas, health bar/text and EXP, death/fall resets. | World pickups + already collected EXP = exactly 3; collection results in EXP 3 once. HUDUpdate screenshot and ExtraChecks verify health text 9/10, bar .9 and EXP 1. Death and fall reload restore all state without duplicate camera/HUD. |
| 6. Full Play Mode verification | Live automated checks and visual inspection completed; integrated run tracked separately in IntegratedRun.md. | See PlaytestResults.md, JumpTrajectory.md, ExtraChecks.md and IntegratedRun.md. Final console and structural validation recorded below. |

The source project's Packages, ProjectSettings, 2D lighting and floor are not imported. No additional package is required for transform animation. Original sprite and animation metadata preserved. Runtime logic is namespaced Afterglow and uses CharacterController exclusively.

## Interpretation of verification evidence

The two fixed-time animation samples in PlaytestResults.md missed short states on a slow frame. These are superseded by JumpTrajectory.md, which observes every frame: Jump=True, Falling=True, walking=True, idle=True, apex 1.976m, extra jump=False. The test driver now uses this observation method too. These were sampling failures, not missing clips. Initial driver mistakes (colliders left enabled during isolated traversal; reading already-collected pickups) were corrected. Reward count includes circles that are collected immediately, preventing a false failure when a drop is already within collection radius.

ExtraChecks.md passed every check. No GUID duplicates were found among Assets metadata. Original SampleScene hash is unchanged. Scene is included alongside the original scene in EditorBuildSettings. Unity 6000.5.5f1 and URP 17.5.0 are preserved.

The video was inspected through a temporary VideoPlayer and a decoded frame outside Assets; only camera/layout reference was used. MediaFoundation emitted format warnings during this inspection; the temporary reader was removed and the game has no video dependency. Initial font setup warnings and one temporary test-driver MissingReferenceException were fixed before final checks. No logs were transmitted externally.

Pending: a human usability pass (feel/difficulty) and standalone build/platform testing. These are distinct from the executed automated Play Mode and visual verification; no standalone build is claimed.

## Final result

- IntegratedRun.md: PASS. Normal keyboard input only, no teleports, disabled enemies or direct damage calls. Reached (99.01, 4.84, 0.63), defeated 4/4 enemies, collected 12/12 EXP, finished with 2/10 health.
- Final console ground truth: compiling=false, compilationFailed=false, consoleErrors=0, consoleWarnings=0.
- Live reference inspection: 14 sprite pieces, zero missing sprites, all 5 original clips present, all HUD references assigned, zero missing script components.
- Builder repeated twice: 22 roots -> 22 roots. Original scene remains in build settings and its hash is unchanged.
- Final editor state: stopped, Afterglow_Prototype loaded and active, isDirty=false, 22 roots. Press Play to start a fresh 10/10 health, EXP 0 session.
- Captures: Initial.png, AirAttack.png, HUDUpdate.png, Finish.png, IntegratedFinish.png. These show actual Play Mode output; the final integrated screenshot shows 2/10 health and EXP 12.
