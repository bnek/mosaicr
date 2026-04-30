Work out a concept to create a mosaic where the images are arranged as a circle and create a task for the implementation.

---
## Status
- **Attempt:** 1
- **Moved to in-progress:** 2026-04-13

## Concept Design: Circle Filling Strategy

The circle filling strategy arranges mosaic tiles in concentric rings around the center of the canvas, creating a circular mosaic pattern instead of a rectangular grid.

### Architecture Fit

The existing architecture uses:
- `IImageFillingStrategy.ComputeBoundsForNextTile(canvasWidth, canvasHeight, tileWidth, tileHeight, xPos, yPos)` — receives grid coordinates from the engine
- `MosaicEngine` iterates in nested loops: `for row in rows, for col in columns`
- `GridCalculator` determines how many rows/columns are needed

The circle strategy works **within the existing interface** by reinterpreting the grid-based `(xPos, yPos)` parameters.

### Key Design Decisions

1. **Reinterpret grid coordinates as sequential index**: The engine iterates in a grid (row, col). The circle strategy converts `yPos * totalColumns + xPos` into a sequential tile index and maps it to a pre-computed circular position. The grid semantics of xPos/yPos are effectively ignored — only the linear index matters.

2. **Pre-computed positions in concentric rings**:
   - Ring 0 (center): 1 tile placed at the canvas center
   - Ring k (k ≥ 1): `Nk = floor(2π · Rk / max(tileWidth, tileHeight))` tiles evenly spaced at radius `Rk`
   - `Rk = k × ringSpacing` where `ringSpacing = max(tileWidth, tileHeight)`
   - Each tile's top-left position: `(centerX + Rk · cos(2π · i / Nk) - tileWidth/2, centerY + Rk · sin(2π · i / Nk) - tileHeight/2)`
   - Continue adding rings until `Rk > min(canvasWidth, canvasHeight) / 2`

3. **Canvas sizing**: The engine sizes the canvas as `columns × tileWidth` by `rows × tileHeight`. For the circle strategy, this still works — the circle is inscribed within the rectangular canvas. The background color fills the corners outside the circle.

4. **Overflow handling**: Tiles whose sequential index exceeds the total number of pre-computed circle positions are placed off-canvas (e.g., at coordinates `-tileWidth, -tileHeight`). This effectively hides them and the background color shows through the un-tiled areas.

5. **Lazy initialization**: Positions are computed on the first call to `ComputeBoundsForNextTile` (since canvas/tile dimensions are only known at call time), then cached for subsequent calls.

6. **No interface changes needed**: The current `IImageFillingStrategy` interface supports this without modifications — the strategy simply reinterprets the existing parameters.

### Visual Example

```
         ┌────────────────────────┐
         │         ·  ·           │
         │      ·        ·       │
         │    ·    · · ·    ·    │
         │   ·   ·       ·   ·  │
         │   ·  ·  [center] ·  · │
         │   ·   ·       ·   ·  │
         │    ·    · · ·    ·    │
         │      ·        ·       │
         │         ·  ·           │
         └────────────────────────┘
         Ring 0: 1 tile (center)
         Ring 1: ~6 tiles
         Ring 2: ~12 tiles
         ...
```

### Implementation Task

See `tasks/queue/014-implement-circle-filling-strategy.md` for the implementation details.

## Completion Notes
- Completed: 2026-04-13
- Summary: Designed a circle filling strategy concept that works within the existing `IImageFillingStrategy` interface by reinterpreting grid coordinates as sequential indices mapped to pre-computed concentric ring positions. Created implementation task 014 with detailed requirements for `CircleFillingStrategy` class, `StrategyFactory` registration, configuration support, and unit tests.
