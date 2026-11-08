# Character clarity

The original sheet is 1024 x 1536 and was imported as DXT5. It now imports at the same full resolution as RGBA32, uncompressed (including Standalone override), retaining sprite slicing, GUIDs and original art. Bilinear filtering is retained for this illustrated, non-pixel-art character; mipmaps are off. No generated detail or external dependency was added.

All four Afterglow cameras use FOV 32 and offset (0,3.1,-14), retaining the frontal 5-degree orientation. At the starting position, character height in the 1280x720 capture increases from approximately 110 to 170 pixels. Dynamic resolution, camera post-processing and image-space antialiasing are disabled locally on these cameras. Global URP assets and editor version are unchanged. Physics, rig scale and level geometry remain unchanged.

Reproducible through Afterglow > Improve Character Clarity; Build Connected World also reapplies this configuration. Only saved Afterglow scenes are updated; unsaved scene changes block the operation. The source reference files are unchanged.

Verified in Game with CharacterClarity.png, confirmed RGBA32/full source resolution, and final console with 0 errors and 0 warnings. Elevated-platform head and feet remain inside the camera viewport. No standalone build was tested. Game view should be viewed at native resolution / 1x when assessing the image; editor preview downscaling can soften any raster output.
