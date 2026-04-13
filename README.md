# Mosaicr

A photo mosaic generator that takes a collection of JPEG images (tiles) from a directory and assembles them into a single large mosaic image. Built as a .NET 8 CLI application using SkiaSharp.

## Features

- Configurable tile dimensions and grid layout
- Pluggable filling strategies (NORMAL grid, CIRCLE arrangement)
- Automatic center-crop and resize of source images to uniform tile size
- Shuffle tiles for randomized placement
- Fill empty grid cells by reusing tiles or with solid-color rectangles
- Configurable color ranges for empty tile fills
- Grayscale (BW) output support
- Placement blur/jitter for a less rigid look

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Building

```shell
dotnet build src/Mosaicr/Mosaicr.csproj
```

## Running Tests

```shell
dotnet test tests/Mosaicr.Tests/Mosaicr.Tests.csproj
```

## Usage

```
mosaicr <sourceDir> <outputFile> [options]
```

| Argument | Description |
|---|---|
| `sourceDir` | Path to directory containing tile images (`.jpg`) |
| `outputFile` | Path for the output mosaic JPEG |

### Options

| Option | Type | Default | Description |
|---|---|---|---|
| `--config <path>` | string | — | Path to a properties configuration file |
| `--tile-width <int>` | int | 200 | Tile width in pixels |
| `--tile-height <int>` | int | 300 | Tile height in pixels |
| `--columns <int>` | int | 19 | Base horizontal tile count |
| `--rows <int>` | int | 4 | Base vertical tile count |
| `--shuffle <true\|false>` | bool | true | Randomize tile placement order |
| `--fill-with-images <true\|false>` | bool | false | Reuse tiles to fill empty grid cells |
| `--strategy <name>` | string | CIRCLE | Filling strategy name (`NORMAL` or `CIRCLE`) |
| `--blur <int>` | int | 0 | Random pixel offset for tile placement |

### Example

```shell
dotnet run --project src/Mosaicr/Mosaicr.csproj -- ./my-photos ./output/mosaic.jpg \
    --config ./mosaicr.sample.properties \
    --tile-width 200 --tile-height 300 \
    --columns 19 --rows 4 \
    --shuffle true --fill-with-images false \
    --strategy CIRCLE --blur 0
```

## Configuration File

Mosaicr reads settings from a properties file (key=value format). Lines starting with `#` are comments. Command-line options override values from the configuration file.

See [mosaicr.sample.properties](mosaicr.sample.properties) for a complete example.

| Key | Type | Default | Description |
|---|---|---|---|
| `defaultTileWidth` | int | 200 | Width of each tile in pixels |
| `defaultTileHeight` | int | 300 | Height of each tile in pixels |
| `defaultHorizontalTileCount` | int | 19 | Base number of columns in the grid |
| `defaultVerticalTileCount` | int | 4 | Base number of rows in the grid |
| `shuffleEnabled` | bool | true | Randomize tile placement order |
| `fillUpMissingTilesWithImages` | bool | false | Reuse tiles to fill empty cells |
| `backgroundColor` | hex | #FF0000 | Background color for canvas and empty tiles (`#RRGGBB`) |
| `tilesColorRange` | string | #FF4F00-#FFBF00,#FFA500-#FFC520 | Color ranges for empty tile fills |
| `targetImageType` | string | RGB | Output color mode: `RGB` or `BW` |
| `imageFillingType` | string | CIRCLE | Tile placement strategy name |
| `placementBlur` | int | 0 | Max random pixel offset (0 = exact grid) |
| `{NAME}_CLASS` | string | — | Maps a strategy name to its implementation class |

### Color Range Format

Color ranges are comma-separated pairs of `#LOWER-#UPPER` hex colors:

```
tilesColorRange=#FF4F00-#FFBF00,#FFA500-#FFC520
```

When filling an empty tile, one range is chosen at random, then each RGB channel is independently randomized between the lower and upper bounds.

## How It Works

1. **Read configuration** — Load settings from a properties file and apply CLI overrides.
2. **Scan source directory** — Find all `.jpg` files.
3. **Prepare tiles** — Center-crop and resize each image to the configured tile dimensions.
4. **Calculate grid** — Scale the base grid ratio up until there are enough cells for all tiles.
5. **Create canvas** — Allocate a blank image filled with the background color.
6. **Place tiles** — Optionally shuffle, then place tiles using the selected filling strategy. Empty cells are filled with solid colors from the configured ranges.
7. **Write output** — Encode the completed mosaic as a JPEG file and clean up temporary files.

## Project Structure

```
src/Mosaicr/              Main application
  Configuration/          Settings parsing and models
  Colors/                 Color range system
  Engine/                 Mosaic engine and grid calculation
  ImageProcessing/        Crop, resize, and write images
  Strategy/               Pluggable tile placement strategies
tests/Mosaicr.Tests/      Unit and integration tests
docs/                     Documentation (overview, architecture, algorithms, configuration)
java-legacy-version/      Legacy Java implementation
examples/                 Example scripts
```

## License

TBD
