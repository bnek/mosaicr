# =============================================================================
# Mosaicr - Example CLI Invocation
# =============================================================================
# This script demonstrates running Mosaicr with all available command-line
# parameters. Adjust the values below for your use case.
#
# Usage: .\examples\run-mosaicr.ps1
# =============================================================================

# --- Required arguments -------------------------------------------------------

$sourceDir  = "./my-photos"           # Directory containing .jpg tile images
$outputFile = "./output/mosaic.jpg"   # Output mosaic file path

# --- Optional: configuration file ---------------------------------------------

$configFile = "./mosaicr.sample.properties"

# --- Tile dimensions (pixels) -------------------------------------------------

$tileWidth  = 200
$tileHeight = 300

# --- Grid layout --------------------------------------------------------------

$columns = 19   # Base horizontal tile count
$rows    = 4    # Base vertical tile count

# --- Behavior -----------------------------------------------------------------

$shuffle        = "true"    # Randomize tile order (true|false)
$fillWithImages = "false"   # Reuse tiles to fill empty cells (true|false)

# --- Filling strategy ----------------------------------------------------------

$strategy = "CIRCLE"   # Placement strategy: NORMAL or CIRCLE
$blur     = 0          # Random pixel offset for tile position (0 = exact grid)

# --- Run Mosaicr with all parameters ------------------------------------------

dotnet run --project src/Mosaicr/Mosaicr.csproj -- `
    $sourceDir `
    $outputFile `
    --config $configFile `
    --tile-width $tileWidth `
    --tile-height $tileHeight `
    --columns $columns `
    --rows $rows `
    --shuffle $shuffle `
    --fill-with-images $fillWithImages `
    --strategy $strategy `
    --blur $blur
