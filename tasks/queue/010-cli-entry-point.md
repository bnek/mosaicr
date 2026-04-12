# 010 - CLI Entry Point

## Description

Implement the command-line interface entry point that accepts arguments, loads configuration, merges defaults with overrides, and invokes the Mosaic Engine.

## Requirements

- **Command-line arguments**:
  - Source directory (required): path to the directory containing tile images
  - Output file (required): path for the output mosaic JPEG
  - `--config <path>`: path to the properties configuration file (optional, uses defaults if omitted)
  - `--tile-width <int>`: override tile width
  - `--tile-height <int>`: override tile height
  - `--columns <int>`: override base horizontal tile count
  - `--rows <int>`: override base vertical tile count
  - `--shuffle <true|false>`: override shuffle setting
  - `--fill-with-images <true|false>`: override fill behavior
  - `--strategy <name>`: override filling strategy name
  - `--blur <int>`: override placement blur pixels
- **Argument merging**:
  1. Start with built-in defaults
  2. If config file specified, parse it and override defaults
  3. Apply any command-line overrides on top
- **Validation**:
  - Source directory must exist
  - Source directory must contain at least one `.jpg` file
  - Output file path's parent directory must exist
  - Tile dimensions must be positive integers
- **Execution**:
  - Construct `MosaicSettings` from merged configuration
  - Instantiate and invoke `MosaicEngine.CreateMosaic()`
  - Print summary on completion (output file path, mosaic dimensions, tile count)
  - Return non-zero exit code on error with descriptive message

## Acceptance Criteria

- [ ] `dotnet run -- <sourceDir> <outputFile>` works with all defaults
- [ ] `dotnet run -- <sourceDir> <outputFile> --config settings.properties` loads config file
- [ ] Command-line arguments override config file values
- [ ] Invalid arguments produce helpful error messages
- [ ] Missing source directory or empty directory produces clear error
- [ ] Successful run prints summary information

## References

- [docs/architecture.md](../../docs/architecture.md) — Application Entry Point component
- [docs/configuration.md](../../docs/configuration.md) — all configuration parameters and defaults

## Dependencies

- **004-configuration-system** — config parsing
- **009-mosaic-engine** — engine to invoke
