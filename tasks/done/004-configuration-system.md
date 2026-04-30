# 004 - Configuration System

## Description

Implement the configuration loading system. Parse a Java-style properties file (key=value format), map values to a strongly-typed settings class, and support all parameters documented in the configuration reference.

## Requirements

- Create a `MosaicSettings` class with strongly-typed properties for all configuration parameters:
  - `TileWidth` (int, default 200)
  - `TileHeight` (int, default 300)
  - `HorizontalTileCount` (int, default 19)
  - `VerticalTileCount` (int, default 4)
  - `ShuffleEnabled` (bool, default true)
  - `FillUpMissingTilesWithImages` (bool, default false)
  - `BackgroundColor` (Color, default #FF0000)
  - `TilesColorRanges` (list of color range pairs, parsed from string like `#FF4F00-#FFBF00,#FFA500-#FFC520`)
  - `TargetImageType` (enum: RGB or BW, default RGB)
  - `ImageFillingType` (string, default "CIRCLE")
  - `PlacementBlur` (int, default 0)
  - Strategy class mappings (dictionary of strategy name → implementation name)
- Create a `ConfigurationParser` that:
  - Reads a properties file (lines of `key=value`, ignoring `#` comments and blank lines)
  - Maps keys to `MosaicSettings` properties
  - Parses hex color strings (`#RRGGBB`) to Color objects
  - Parses color range strings (`#LOW-#HIGH,#LOW-#HIGH`) into structured data
  - Detects `{NAME}_CLASS` keys and stores them in the strategy mappings dictionary
- Apply defaults for any missing properties

## Acceptance Criteria

- [ ] Can parse a complete properties file matching the example in docs/configuration.md
- [ ] All default values are applied when a property is missing
- [ ] Hex color parsing works for backgroundColor and color ranges
- [ ] Strategy class mappings (e.g., `NORMAL_CLASS=SimpleImageFillingStrategy`) are captured
- [ ] Invalid/malformed lines are handled gracefully (skipped or logged)

## References

- [docs/configuration.md](../../docs/configuration.md) — full parameter reference with types, defaults, and format details

## Dependencies

- **003-project-setup** — project must exist before adding code

---
## Status
- **Attempt:** 1
- **Moved to in-progress:** 2026-04-12

## Completion Notes
- Completed: 2026-04-12
- Summary: Implemented MosaicSettings, ColorRange, ImageType, and ConfigurationParser. Created 9 tests (plus 1 existing placeholder) — all 10 pass. Committed as af633a0.
