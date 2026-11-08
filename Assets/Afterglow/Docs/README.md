# AFTERGLOW: BLINDING HOPE — prototype

Open `Assets/Afterglow/Scenes/Afterglow_Prototype.unity` and press Play. Click the Game view if physical keyboard input is not focused.

## Controls

- Arrow keys: move on XZ; Up goes away from the camera. Diagonals are normalized.
- Space: single grounded jump, with air steering.
- X: one melee attack per press; works in the air. Last movement direction on XZ determines the attack cone, independently of visual facing.

## Adjustable components

- Player / AfterglowPlayer: speed 5, jump height 2, gravity -25, max health 10, damage 1, cooldown .65s, range 1.65m, height tolerance 1.1m.
- Enemy / AfterglowEnemy: health 3, damage 1, detection 6m, reach 1.5m, interval 1.2s, speed 1.6m/s. Wind-up .3s with impact checked once; edge and wall probes stop pursuit.
- Main Camera / AfterglowCamera: perspective FOV 38, offset (0,9,-21), angle 18 degrees, anticipation 2.5m and smoothing .25s.

## Rebuild

Use **Afterglow > Build Prototype**, outside Play Mode. It refuses to proceed while any open scene has unsaved changes. It replaces only the generated prototype scene and its own prefabs/materials, preserving other scene files. Existing prototype edits must be saved first and will then be replaced by the reproducible layout. Keep authored customizations in the builder when rebuilding.

The generated scene is enabled in EditorBuildSettings, preserving other entries. Reset loads its own build index, restoring player, camera, enemies and EXP without static game state.

## Character provenance

`References/Animaciones.zip` was extracted to `References/CharacterSource` outside Assets. This source is left unchanged after extraction. The character had no prefab or gameplay script: its articulated Transform/SpriteRenderer hierarchy lives under Player/Body in its SampleScene. The builder reconstructs only Body and its descendants from that source, using Unity APIs, and preserves local transforms, names, sprite local IDs, draw order and Animator Controller. It does not import the reference floor, camera, light, project settings, packages or 2D collider.

Imported: the original sprite sheet and its slicing metadata, PlayerAC.controller, and Idle, Walk, Jump, Falling, Attack clips, with their original .meta GUIDs. No GUID collision was found. No source class names conflict (there were no source gameplay scripts). All five states exist. No new view or Nintendo artwork is used. The HUD portrait is the existing transparent head sprite.

Visual hierarchy: CharacterController root > VisualFacing > RigOffset > Body > original pieces. Normalization and reflection happen outside the animated hierarchy. Original root curves remain confined to Body, never the moving controller. Original clips are unchanged, contain no animation events, and attack lasts .5s. Sprites use Sprites/Default for unlit transparent rendering in the existing 3D URP renderer. No 2D animation, lighting, PSD or Aseprite packages are required.

The UI TrueType file is Roboto Regular, reused from the existing Unity Searcher package (license in Docs). No external software or paid dependency was installed. The generated font asset is scoped under Afterglow. TMP resources/examples also appeared in Assets/TextMesh Pro during the session; they are left intact. The temporary duplicate TMP settings under Afterglow were removed; the existing global TMP settings are used. The builder creates local settings only if none exist.

## Verification

See `PrototypeChecklist.md` and `PlaytestResults.md`. `Tools/Afterglow/Playtest.cs` is a reproducible, ephemeral Pipeline test driver outside Assets, executed with `unity command script run --file Tools/Afterglow/Playtest.cs --entry AfterglowPlaytest.Main` after entering Play Mode. It queues real Input System keyboard states and drives the normal controller. Temporary controlled combat positions and disabled enemy controllers isolate individual tests; those changes are not saved. The full route is traversed with normal collision, movement and grounded jumps, without teleporting along the route. This is automated Play Mode validation, not a claim of a human playthrough.

Final integrated run: PASS, 4 enemies defeated, 12 EXP collected, 2/10 health remaining, end of route reached with normal controls only. See IntegratedRun.md and IntegratedFinish.png. JumpTrajectory.md supersedes two missed short animation samples in the earlier PlaytestResults.md; all five states were observed live. ExtraChecks.md passed airborne attack, HUD update and enemy edge/wall/inaccessible-floor checks. Final console: zero errors and warnings; zero missing sprites, scripts or required references. Builder repeated twice without duplicates. Left in Edit Mode with the saved prototype scene active.

An initial editor closure during script reload was recovered by reopening the same version. Initial font setup and simulated edge detection were corrected before the final validation. The original Assets/Scenes/SampleScene.unity retains SHA256 07FCE8B04804DDCCB3026DB5B43D8DC41BA138E49D50D36A5C24877081EACDCA.
