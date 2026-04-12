# Glossary

## Tile
A single small image that serves as one building block of the mosaic. All tiles are cropped and resized to identical dimensions before placement.

## Mosaic
The final composite image created by arranging many tiles in a grid pattern. The mosaic's pixel dimensions are determined by `gridColumns × tileWidth` and `gridRows × tileHeight`.

## Grid
The row-and-column structure that defines where tiles are placed on the mosaic canvas. The grid dimensions are a scaled-up multiple of the configured aspect ratio, large enough to accommodate all available tiles.

## Aspect Ratio (Grid)
The base ratio of columns to rows (e.g., 19:4). The actual grid dimensions are always a whole-number multiple of this ratio, scaled up until there are enough cells for all available tiles.

## Canvas
The blank output image onto which tiles are placed. Created at the full mosaic resolution and initially filled with the background color.

## Tile Dimensions
The fixed width and height (in pixels) that every tile is normalized to. Source images that don't match these dimensions are center-cropped and resized.

## Center-Crop
The process of cropping a source image to match the tile aspect ratio by removing equal amounts from opposite edges. This preserves the center of the image, which typically contains the main subject.

## Bicubic Interpolation
A high-quality image resampling algorithm used when resizing tiles. It produces smoother results than nearest-neighbor or bilinear interpolation, especially when downscaling.

## Background Color
A solid color (specified as a hex value like `#FF0000`) used to fill the canvas initially and to fill any empty tile slots when color ranges are not configured.

## Color Range
A pair of colors defining a lower and upper bound in RGB space. Random colors can be generated within this range by independently randomizing each color channel (R, G, B) between the two bounds.

## Color Range Container
A collection of one or more color ranges. When a random color is needed, a range is selected at random from the container, then a color is generated from that range. This allows defining multiple color palettes that are sampled randomly.

## Placement Blur
A configurable pixel offset that introduces randomness into tile positioning. When enabled, each tile is shifted by a random number of pixels (from 0 up to the configured maximum), creating a less uniform, more organic look. Tiles may slightly overlap as a result.

## Image Filling Strategy
A pluggable algorithm that determines how tiles are positioned on the canvas. The strategy computes the pixel bounds (x, y, width, height) for each tile based on its grid position. Different strategies can produce different visual layouts (e.g., a simple grid vs. a circular arrangement).

## Strategy Factory
A mechanism for resolving a strategy name (from configuration) to a concrete implementation at runtime. New strategies can be added by implementing the filling strategy interface and registering the mapping in configuration.

## Fill-Up
The process of padding the tile list when there are fewer tiles than grid cells. Two modes: **image fill-up** (reuse existing tiles cyclically) or **color fill-up** (insert placeholders that become solid-color rectangles).

## Shuffle
Randomizing the order of tiles before placement so that the mosaic arrangement is non-deterministic. When combined with fill-up, a second shuffle distributes any repeated or placeholder tiles evenly across the grid.

## Temporary Files
Intermediate rescaled image files created during the preparation step. These are saved to disk (with a `.rescaled.jpg` suffix) so the placement step can load them. All temporary files are deleted after the mosaic is complete.

## Color Space Conversion
The process of converting a tile's pixel data from one color model to another (e.g., CMYK to RGB) so it can be correctly composited onto the mosaic canvas. This is performed automatically when a mismatch is detected.
