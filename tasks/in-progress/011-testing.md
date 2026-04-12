# 011 - Testing

## Description

Add unit tests for core algorithms and an integration test that creates a mosaic from test images end-to-end.

## Requirements

- **Unit Tests** (in `tests/Mosaicr.Tests/`):

  - **Grid Dimension Calculation**:
    - Base 19×4 with 200 tiles → 38×8
    - Exact fit: 76 tiles with 19×4 → 19×4
    - Fewer tiles than base: e.g., 10 tiles with 19×4 → 19×4 (base is already enough)
    - Edge case: 0 tiles

  - **Center-Crop Algorithm**:
    - Image too wide: verify correct crop region (centered horizontally)
    - Image too tall: verify correct crop region (centered vertically)
    - Image already matches aspect ratio: no cropping

  - **Color Range**:
    - Generated colors are within specified bounds
    - Single range generates valid colors
    - ColorRangeContainer selects from multiple ranges

  - **Color Range Parsing**:
    - Parse `#FF4F00-#FFBF00,#FFA500-#FFC520` → two ranges with correct bounds
    - Parse single range (no comma)
    - Handle empty/null input

  - **Image List Preparation**:
    - Correct length after fill-up (equals cells needed)
    - Fill-with-images mode cycles tiles
    - Fill-with-placeholders mode inserts nulls
    - Shuffle changes order (statistical — run multiple times, verify not always same order)

  - **Configuration Parsing**:
    - Parse a complete example config file
    - Default values applied for missing keys
    - Hex color parsing
    - Color range string parsing
    - Strategy mapping keys detected

  - **Simple Grid Strategy**:
    - Correct rectangle for position (0,0), (1,0), (0,1)
    - Blur offset within expected range
    - Clamping at canvas edges

- **Integration Test**:
  - Include a small set of test JPEG images (3-5 small images, e.g., 100×100 px) in the test project
  - Run the full pipeline: config → prepare → grid → canvas → fill → write
  - Verify output file exists and has expected dimensions
  - Clean up output file after test

## Acceptance Criteria

- [ ] All unit tests pass with `dotnet test`
- [ ] Integration test produces a valid mosaic JPEG
- [ ] Tests cover the core algorithms listed above
- [ ] Test images are included in the test project (small file size)
- [ ] No tests depend on external resources or network access

## References

- [docs/algorithms.md](../../docs/algorithms.md) — all algorithm specifications to test against
- [docs/configuration.md](../../docs/configuration.md) — example config for parsing tests

## Dependencies

- **003-project-setup** — test project must exist
- **All implementation tasks (004–010)** — code must exist to test

---
## Status
- **Attempt:** 1
- **Moved to in-progress:** 2026-04-13
