# Configuration Reference

All settings are read from a properties file (key=value format). Parameters are grouped by function below.

## Tile Settings

| Parameter | Type | Default | Description |
|---|---|---|---|
| `defaultTileWidth` | integer | `200` | Width of each tile in pixels. Source images are resized to this width. |
| `defaultTileHeight` | integer | `300` | Height of each tile in pixels. Source images are resized to this height. |

## Grid / Layout

| Parameter | Type | Default | Description |
|---|---|---|---|
| `defaultHorizontalTileCount` | integer | `19` | Base number of columns in the mosaic grid. The actual column count will be a multiple of this value, scaled up to fit all available tiles. |
| `defaultVerticalTileCount` | integer | `4` | Base number of rows in the mosaic grid. The actual row count will be a multiple of this value, scaled up to fit all available tiles. |

The final mosaic dimensions in pixels are:
- **Width** = `actualColumns × defaultTileWidth`
- **Height** = `actualRows × defaultTileHeight`

## Behavior

| Parameter | Type | Default | Description |
|---|---|---|---|
| `shuffleEnabled` | boolean | `true` | When `true`, tiles are placed in random order. When `false`, tiles are placed in the order they are read from the directory. |
| `fillUpMissingTilesWithImages` | boolean | `false` | Controls what happens when there are fewer tiles than grid cells. When `true`, tiles are reused (cycled) to fill all cells. When `false`, empty cells are filled with solid-color rectangles. |

## Colors

| Parameter | Type | Default | Description |
|---|---|---|---|
| `backgroundColor` | hex color | `#FF0000` | The background color used to fill the canvas and any empty tiles (when `fillUpMissingTilesWithImages` is `false` and no `tilesColorRange` is set). Format: `#RRGGBB`. |
| `tilesColorRange` | string | `#FF4F00-#FFBF00,#FFA500-#FFC520` | Comma-separated list of color ranges for filling empty tiles. Each range is two hex colors separated by a hyphen: `#LOWER-#UPPER`. When filling an empty tile, a range is randomly chosen, then a random color within that range is generated. If not set, `backgroundColor` is used instead. |
| `targetImageType` | string | `RGB` | The color mode for the output image. Accepted values: `RGB` (full color) or `BW` (grayscale). |

### Color Range Format

```
tilesColorRange=#FF4F00-#FFBF00,#FFA500-#FFC520
```

This defines two color ranges:
1. From `#FF4F00` (dark orange) to `#FFBF00` (golden yellow)
2. From `#FFA500` (orange) to `#FFC520` (amber)

When a color is needed, one range is chosen at random. Within that range, each RGB channel is independently randomized between the lower and upper bounds.

## Image Filling Strategy

| Parameter | Type | Default | Description |
|---|---|---|---|
| `imageFillingType` | string | `CIRCLE` | The name of the tile placement strategy. This is resolved to a concrete implementation via a naming convention (see below). |
| `placementBlur` | integer | `0` | Maximum random pixel offset applied to each tile's position. A value of `0` means tiles are placed exactly on the grid. Higher values introduce a random jitter effect. |
| `{STRATEGY_NAME}_CLASS` | string | — | Maps a strategy name to its implementation. For example, `NORMAL_CLASS=SimpleImageFillingStrategy` and `CIRCLE_CLASS=CircleImageFillingStrategy`. |

### Strategy Resolution

The strategy is resolved as follows:
1. Read the `imageFillingType` value (e.g., `CIRCLE`)
2. Append `_CLASS` to form a lookup key (e.g., `CIRCLE_CLASS`)
3. Read the value of that key to get the implementation name (e.g., `CircleImageFillingStrategy`)
4. Instantiate the strategy

### Known Strategies

| Name | Description |
|---|---|
| `NORMAL` | Simple grid placement. Each tile is placed at its exact grid position with an optional blur offset. |
| `CIRCLE` | Circular arrangement (referenced in configuration but implementation was not completed in the legacy version). |

## Example Configuration

```properties
# Tile dimensions
defaultTileWidth=200
defaultTileHeight=300

# Grid layout
defaultHorizontalTileCount=19
defaultVerticalTileCount=4

# Behavior
shuffleEnabled=true
fillUpMissingTilesWithImages=false

# Colors
backgroundColor=#FF0000
tilesColorRange=#FF4F00-#FFBF00,#FFA500-#FFC520
targetImageType=RGB

# Filling strategy
imageFillingType=NORMAL
placementBlur=0
NORMAL_CLASS=SimpleImageFillingStrategy
CIRCLE_CLASS=CircleImageFillingStrategy
```
