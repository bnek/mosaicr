# 008 - Filling Strategies (Strategy Pattern)

## Description

Implement the tile placement strategy system using the Strategy Pattern. This includes the interface, abstract base class, the Simple Grid Strategy, and a Strategy Factory for resolving strategy names to implementations.

## Requirements

- **IImageFillingStrategy** interface:
  - `PlacementBlurPixels` property (get/set): max random pixel offset
  - `ComputeBoundsForNextTile(canvasWidth, canvasHeight, tileWidth, tileHeight, xPos, yPos)` → returns a rectangle (x, y, width, height) defining where the tile should be drawn

- **AbstractFillingStrategy** base class:
  - Implements `PlacementBlurPixels` property
  - Protected `GetBlurOffset()` method: returns `random(0, PlacementBlurPixels)` or 0 if blur is disabled

- **SimpleGridStrategy** (strategy name: `NORMAL`):
  - Computes tile placement as:
    ```
    blurOffset = GetBlurOffset()
    x = (xPos × tileWidth) + blurOffset
    y = (yPos × tileHeight) + blurOffset
    width = tileWidth
    height = tileHeight
    // Clamp to canvas bounds
    if (x + width > canvasWidth): x = canvasWidth - width
    if (y + height > canvasHeight): y = canvasHeight - height
    ```

- **StrategyFactory**:
  - Takes the strategy mappings dictionary (from configuration: `{NAME}_CLASS → implementation`)
  - Given a strategy name (e.g., `NORMAL`), looks up `NORMAL_CLASS` in the mappings, resolves to the concrete type, and instantiates it
  - For this task, only `SimpleGridStrategy` needs to be resolvable
  - Note: `CircleStrategy` was not completed in the legacy version — skip it

## Acceptance Criteria

- [ ] `IImageFillingStrategy` interface defined with required members
- [ ] `SimpleGridStrategy` computes correct rectangle for grid positions
- [ ] Placement blur adds random offset and clamps to canvas bounds
- [ ] `StrategyFactory` resolves `NORMAL` to `SimpleGridStrategy`
- [ ] Factory handles unknown strategy names gracefully (throw descriptive exception)
- [ ] Blur of 0 produces exact grid positions with no offset

## References

- [docs/architecture.md](../../docs/architecture.md) — Strategy Pattern class diagram and Strategy Factory description
- [docs/algorithms.md](../../docs/algorithms.md) — Section 4: Tile Placement, Simple Grid Strategy
- [docs/configuration.md](../../docs/configuration.md) — Strategy resolution mechanism

## Dependencies

- **003-project-setup** — project must exist before adding code
