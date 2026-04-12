# 007 - Grid Calculation and Image List Preparation

## Description

Implement the grid dimension calculation algorithm and the image list preparation logic (shuffle, fill-up, re-shuffle).

## Requirements

- **Grid Dimension Calculator**:
  - Input: base columns, base rows (the configured aspect ratio), number of available tiles
  - Algorithm: start with base dimensions, increment both by their base values until `columns × rows >= tilesAvailable`
  - Output: final columns and rows
  - Example: base 19×4 with 200 tiles → 38×8 = 304 cells

- **Image List Preparator**:
  - Input: list of tile file paths, total cells needed (`columns × rows`), shuffle flag, fill-with-images flag
  - Steps:
    1. If shuffle enabled: shuffle the tile list
    2. If tiles < cells needed, fill the gap:
       - If `fillWithImages` is true: cycle through existing tiles to fill remaining slots
       - If `fillWithImages` is false: append `null` entries (placeholders for solid-color rectangles)
    3. If shuffle enabled: re-shuffle the entire list (so fill-ups are distributed evenly)
  - Output: list of exactly `tilesNeeded` entries (file paths or nulls)

## Acceptance Criteria

- [ ] Grid calculation correctly scales up: 19×4 base with 200 tiles → 38×8
- [ ] Grid calculation handles exact fit (e.g., 76 tiles with 19×4 base → 19×4)
- [ ] Grid calculation handles 0 tiles (stays at base dimensions)
- [ ] Image list has exactly `tilesNeeded` entries after preparation
- [ ] Fill-with-images mode cycles through tiles correctly
- [ ] Fill-with-placeholders mode appends nulls
- [ ] Shuffle randomizes order when enabled

## References

- [docs/algorithms.md](../../docs/algorithms.md) — Section 1: Grid Dimension Calculation, Section 5: Image List Preparation

## Dependencies

- **003-project-setup** — project must exist before adding code
