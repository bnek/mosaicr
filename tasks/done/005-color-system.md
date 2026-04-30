# 005 - Color System

## Description

Implement the color generation system used to fill empty tile slots with randomized solid colors. This includes `ColorRange` (a pair of RGB bounds) and `ColorRangeContainer` (a collection of ranges with random selection).

## Requirements

- **ColorRange** class:
  - Constructed from two colors: lower bound (`R_lo, G_lo, B_lo`) and upper bound (`R_hi, G_hi, B_hi`)
  - `GetRandomColor()` method generates a color where each RGB channel is independently randomized: `R = R_lo + random(0, |R_hi - R_lo|)`, same for G and B
  - Handle cases where lower > upper for a channel (use absolute difference)
- **ColorRangeContainer** class:
  - Holds a list of `ColorRange` instances
  - `GetNextColor()` selects a range at random (uniform distribution), then generates a random color from that range
  - Support adding ranges after construction
- **Parsing** (can be in ColorRangeContainer or a helper):
  - Parse a string like `#FF4F00-#FFBF00,#FFA500-#FFC520` into a list of `ColorRange` objects
  - Each comma-separated segment is one range; each range has two hex colors separated by a hyphen

## Acceptance Criteria

- [ ] `ColorRange.GetRandomColor()` produces colors within the specified bounds
- [ ] `ColorRangeContainer.GetNextColor()` randomly selects from available ranges
- [ ] Parsing `#FF4F00-#FFBF00,#FFA500-#FFC520` produces two ColorRange objects with correct bounds
- [ ] Edge case: single color range (no comma) works
- [ ] Edge case: empty/null input handled gracefully

## References

- [docs/algorithms.md](../../docs/algorithms.md) — Section 6: Color Fill for Empty Tiles
- [docs/glossary.md](../../docs/glossary.md) — Color Range, Color Range Container definitions

## Dependencies

- **003-project-setup** — project must exist before adding code

---
## Status
- **Attempt:** 1
- **Moved to in-progress:** 2026-04-12

## Completion Notes
- Completed: 2026-04-12
- Summary: Implemented ColorRange class (Colors/ColorRange.cs) with GetRandomColor() method, ColorRangeContainer class (Colors/ColorRangeContainer.cs) with GetNextColor(), Parse(), Add(), indexer, and Count. Moved ColorRange from Configuration/ to Colors/ namespace. Updated MosaicSettings and ConfigurationParser to use new types. Added 10 new tests (ColorRangeTests + ColorRangeContainerTests). All 21 tests pass.
