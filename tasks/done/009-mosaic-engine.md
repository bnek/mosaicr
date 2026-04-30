# 009 - Mosaic Engine

## Description

Implement the Mosaic Engine — the central orchestrator that coordinates the entire mosaic creation pipeline from image preparation through to JPEG output and cleanup.

## Requirements

- **MosaicEngine** class with a main method: `CreateMosaic(sourceDirectory, outputFile, settings)`
- The engine orchestrates these steps in order:
  1. **Prepare images**: use Image Preparator to scan source directory and process tiles to uniform dimensions
  2. **Calculate grid dimensions**: use Grid Calculator with the configured aspect ratio and tile count
  3. **Create canvas**: allocate a blank image of `(gridColumns × tileWidth) × (gridRows × tileHeight)` pixels, filled with the configured background color
  4. **Prepare image list**: shuffle and fill-up using Image List Preparator
  5. **Fill the mosaic**: iterate through each grid cell (row by row, column by column):
     - Get placement bounds from the filling strategy via `ComputeBoundsForNextTile()`
     - If tile is available: load the tile image and draw it onto the canvas at the computed bounds
     - If tile is null (placeholder): get a color from ColorRangeContainer (or use backgroundColor if no ranges configured) and fill the rectangle with that solid color
     - Handle color space mismatches when drawing tiles
  6. **Write output**: encode the canvas as JPEG and save to the output file path
  7. **Cleanup**: delete all temporary rescaled image files
- Apply the configured filling strategy (resolved via StrategyFactory)
- Set placement blur on the strategy from configuration
- Support both RGB and BW (grayscale) output modes via `targetImageType` setting

## Acceptance Criteria

- [ ] Engine runs the full pipeline end-to-end given a directory of JPEG images
- [ ] Output mosaic has correct pixel dimensions (`gridCols × tileWidth` by `gridRows × tileHeight`)
- [ ] Tiles are placed in the correct grid positions
- [ ] Empty cells are filled with colors from the configured ranges (or background color)
- [ ] Placement blur is applied when configured
- [ ] Temporary files are cleaned up after mosaic creation
- [ ] Grayscale output works when `targetImageType` is BW

## References

- [docs/overview.md](../../docs/overview.md) — high-level pipeline
- [docs/architecture.md](../../docs/architecture.md) — Mosaic Engine component description and data flow sequence diagram

## Dependencies

- **004-configuration-system** — settings must be available
- **005-color-system** — color fill for empty tiles
- **006-image-processing** — image preparation, cropping, resizing, writing
- **007-grid-calculation** — grid dimensions and image list preparation
- **008-filling-strategies** — tile placement strategy

---
## Status
- **Attempt:** 1
- **Moved to in-progress:** 2026-04-12

## Completion Notes
- Completed: 2026-04-12
- Summary: Created `src/Mosaicr/Engine/MosaicEngine.cs` with a static `CreateMosaic` method that orchestrates the full pipeline: image preparation, grid calculation, canvas creation with background color fill, image list preparation, strategy-based tile placement, color fill for empty cells, grayscale conversion support, JPEG output, and temporary file cleanup. Uses `ProcessPixelRows` for efficient pixel-level operations. Build succeeds. Committed as `5645792`.
