# Core Algorithms

## 1. Grid Dimension Calculation

### Problem

Given a desired grid aspect ratio (e.g., 19 columns × 4 rows) and the actual number of available tiles, calculate the final grid dimensions such that:
- The grid has enough cells to hold all tiles
- The grid proportions remain a multiple of the base aspect ratio

### Algorithm

```
Input:
  baseColumns, baseRows  — the configured aspect ratio (e.g., 19, 4)
  tilesAvailable         — number of tile images found

Output:
  finalColumns, finalRows — the scaled-up grid dimensions

Procedure:
  columns = baseColumns
  rows = baseRows

  while (columns × rows) < tilesAvailable:
      columns += baseColumns
      rows += baseRows

  return (columns, rows)
```

### Example

- Base ratio: 19×4 (76 cells)
- Available tiles: 200
- Iteration 1: 38×8 = 304 cells → 304 ≥ 200 ✓
- Final grid: **38 columns × 8 rows** (304 cells, 104 empty)

### Notes

- The grid always ends up with `cells ≥ tilesAvailable`
- Empty cells are handled by the fill-up logic (see section 5)
- Both dimensions scale together, preserving the original ratio

---

## 2. Image Preparation: Center-Crop (Aspect Ratio Correction)

### Problem

Source images may have arbitrary dimensions and aspect ratios. Before they can be used as tiles, they must be cropped to match the target tile aspect ratio, then resized to exact tile dimensions. The crop should be centered to preserve the most important part of the image.

### Algorithm

```
Input:
  image                  — source image (width: W_i, height: H_i)
  tileWidth, tileHeight  — target tile dimensions (W_t, H_t)

Output:
  croppedImage           — center-cropped image matching tile aspect ratio
```

Calculate the aspect ratios:

$$\text{imageAspectRatio} = \frac{W_i}{H_i}$$

$$\text{tileAspectRatio} = \frac{W_t}{H_t}$$

#### Case 1: Image is too wide (imageAspectRatio > tileAspectRatio)

The image is wider than needed relative to its height. Keep the full height, crop the width.

$$H_{new} = H_i$$

$$W_{new} = W_t \times \frac{H_{new}}{H_t}$$

$$x_{cutout} = \frac{W_i}{2} - \frac{W_{new}}{2}$$

Crop region: `(x_cutout, 0, W_new, H_new)`

```
┌──────────────────────────┐
│  ┊      image      ┊    │
│  ┊   ┌──────────┐  ┊    │
│  ┊   │  cropped  │  ┊    │
│  ┊   │  region   │  ┊    │
│  ┊   └──────────┘  ┊    │
│  ┊                  ┊    │
└──────────────────────────┘
   ↑                  ↑
   x_cutout     x_cutout + W_new
```

#### Case 2: Image is too tall (imageAspectRatio < tileAspectRatio)

The image is taller than needed relative to its width. Keep the full width, crop the height.

$$W_{new} = W_i$$

$$H_{new} = H_t \times \frac{W_{new}}{W_t}$$

$$y_{cutout} = \frac{H_i}{2} - \frac{H_{new}}{2}$$

Crop region: `(0, y_cutout, W_new, H_new)`

```
┌──────────┐
│          │  ← y_cutout
├──────────┤
│  cropped │
│  region  │
├──────────┤
│          │
└──────────┘
```

#### Case 3: Aspect ratios match

No cropping needed.

---

## 3. Image Resize

After cropping, the image is resized to the exact tile dimensions.

```
Input:
  croppedImage           — output from center-crop
  tileWidth, tileHeight  — target dimensions

Output:
  resizedImage           — image at exactly (tileWidth × tileHeight) pixels
```

- Create a new blank image at the target dimensions
- Draw the cropped image scaled to fill the new image
- Use **bicubic interpolation** for high-quality scaling

---

## 4. Tile Placement

### Simple Grid Strategy (NORMAL)

Tiles are placed on a row-by-column grid. Each tile occupies one cell.

```
Input:
  canvas                 — the mosaic canvas image
  tileWidth, tileHeight  — tile dimensions
  xPos, yPos             — grid position (column, row), 0-indexed
  placementBlurPixels    — max random offset (0 = no blur)

Output:
  rectangle              — pixel bounds (x, y, width, height) for the tile
```

```
blurOffset = random(0, placementBlurPixels)   // 0 if blur disabled

x = (xPos × tileWidth) + blurOffset
width = tileWidth

// Clamp to canvas bounds
if (x + width) > canvasWidth:
    x = canvasWidth - width

y = (yPos × tileHeight) + blurOffset
height = tileHeight

// Clamp to canvas bounds
if (y + height) > canvasHeight:
    y = canvasHeight - height

return Rectangle(x, y, width, height)
```

### Placement Blur Effect

When `placementBlur > 0`, each tile's position is shifted by a small random amount. This creates a slightly irregular, hand-placed look instead of a perfectly rigid grid. The offset is always non-negative (shifts right/down) and clamped so tiles never extend beyond the canvas edges.

Note: with blur, tiles may slightly overlap each other since later tiles are drawn on top of earlier ones.

---

## 5. Image List Preparation (Shuffle & Fill-Up)

### Problem

The number of available tiles may not match the number of grid cells. The list needs to be prepared to have exactly as many entries as grid cells.

### Algorithm

```
Input:
  tileFiles[]            — list of tile file references
  tilesNeeded            — gridColumns × gridRows
  shuffleEnabled         — whether to randomize order
  fillWithImages         — whether to reuse tiles or use placeholders

Output:
  preparedList[]         — list of exactly tilesNeeded entries
```

```
if shuffleEnabled:
    shuffle(tileFiles)

index = 0
while length(tileFiles) < tilesNeeded:
    if fillWithImages:
        tileFiles.append(tileFiles[index])   // cycle through existing tiles
        index++
    else:
        tileFiles.append(NULL)               // placeholder → colored rectangle

if shuffleEnabled:
    shuffle(tileFiles)                       // re-shuffle so fill-ups are distributed
```

### Fill Behavior

| `fillWithImages` | Behavior |
|---|---|
| `true` | Tiles are reused cyclically. The mosaic is fully filled with images (some repeated). |
| `false` | Empty cells become `NULL` entries. During placement, these are rendered as solid-color rectangles using the color system. |

---

## 6. Color Fill for Empty Tiles

When a tile slot has no image (NULL placeholder), it is filled with a solid color.

### With Color Ranges Configured

1. The color range container holds one or more `ColorRange` objects
2. A range is selected at random (uniform distribution)
3. Within the selected range, a color is generated:

```
Input:
  lowerColor (R_lo, G_lo, B_lo)
  upperColor (R_hi, G_hi, B_hi)

Output:
  randomColor

R = R_lo + random(0, |R_hi - R_lo|)
G = G_lo + random(0, |G_hi - G_lo|)
B = B_lo + random(0, |B_hi - B_lo|)

return Color(R, G, B)
```

Each channel is randomized independently, producing colors within the "box" defined by the two corners in RGB space.

### Without Color Ranges

The configured `backgroundColor` is used as a solid fill for all empty tiles.

---

## 7. Color Space Conversion

When placing a tile onto the canvas, the tile's color space may differ from the canvas color space (e.g., a CMYK image being placed into an RGB canvas). In this case:

1. Detect the color space mismatch
2. Convert the tile from its source color space to the canvas color space
3. Use bicubic interpolation during conversion for quality

This ensures all tiles are visually consistent regardless of their original color profile.
