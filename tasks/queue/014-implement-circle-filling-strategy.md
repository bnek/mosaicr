# Implement Circle Filling Strategy

## Description
Implement a `CircleFillingStrategy` that arranges mosaic tiles in concentric rings around the center of the canvas, creating a circular mosaic pattern instead of a rectangular grid.

## Background
See `tasks/done/013-circle-image-filling strategy.md` for the full concept design. The strategy works within the existing `IImageFillingStrategy` interface by reinterpreting grid-based `(xPos, yPos)` coordinates as a sequential tile index mapped to pre-computed circular positions.

## Requirements

### 1. Create `CircleFillingStrategy` class
- File: `src/Mosaicr/Strategy/CircleFillingStrategy.cs`
- Extends `AbstractFillingStrategy`
- Pre-computes tile positions in concentric rings on first call to `ComputeBoundsForNextTile`:
  - Ring 0: 1 tile at canvas center (`centerX - tileWidth/2, centerY - tileHeight/2`)
  - Ring k (k >= 1): `Nk = floor(2 * pi * Rk / max(tileWidth, tileHeight))` tiles evenly spaced
  - `Rk = k * ringSpacing` where `ringSpacing = max(tileWidth, tileHeight)`
  - Each tile position: top-left corner at `(centerX + Rk * cos(2 * pi * i / Nk) - tileWidth/2, centerY + Rk * sin(2 * pi * i / Nk) - tileHeight/2)`
  - Continue adding rings until the ring radius exceeds `min(canvasWidth, canvasHeight) / 2`
- Maps the grid-based `(xPos, yPos)` to a sequential index: `yPos * (canvasWidth / tileWidth) + xPos`
- Returns pre-computed position for that index
- If index exceeds circle capacity, returns off-canvas position (e.g., `-tileWidth, -tileHeight, tileWidth, tileHeight`)
- Supports placement blur via inherited `GetBlurOffset()`
- Positions are lazily computed on first call and cached

### 2. Register in `StrategyFactory`
- Add `["CircleImageFillingStrategy"] = typeof(CircleFillingStrategy)` to `KnownStrategies` dictionary in `src/Mosaicr/Strategy/StrategyFactory.cs`

### 3. Add configuration support
- Ensure `CIRCLE_CLASS=CircleImageFillingStrategy` can be set in config files
- The `MosaicSettings.ImageFillingType` already defaults to `"CIRCLE"`, so setting `CIRCLE_CLASS=CircleImageFillingStrategy` in the config will work with the existing `StrategyFactory` lookup logic

### 4. Write unit tests
- File: `tests/Mosaicr.Tests/Strategy/CircleFillingStrategyTests.cs`
- Test that ring 0 (index 0) places a tile at/near canvas center
- Test that ring 1 tiles are placed at the expected radius from center
- Test that sequential indices exceeding circle capacity return off-canvas bounds
- Test that blur offset is applied when `PlacementBlurPixels > 0`
- Test that positions are deterministic (consistent across multiple calls with same parameters)
- Test edge case: very small canvas (single tile only)

### 5. Update `StrategyFactoryTests`
- File: `tests/Mosaicr.Tests/Strategy/StrategyFactoryTests.cs`
- Add a test that `StrategyFactory.Create("CIRCLE", mappings)` returns `CircleFillingStrategy` when `CIRCLE_CLASS=CircleImageFillingStrategy` is in mappings

## Acceptance Criteria
- [ ] `CircleFillingStrategy` class exists and compiles
- [ ] Strategy is registered in `StrategyFactory` as `CircleImageFillingStrategy`
- [ ] Tiles are arranged in concentric rings when the circle strategy is used
- [ ] All new and existing tests pass (`dotnet test`)
- [ ] The strategy handles edge cases (single tile, canvas smaller than one ring, index overflow)
- [ ] No changes to `IImageFillingStrategy` interface are required
