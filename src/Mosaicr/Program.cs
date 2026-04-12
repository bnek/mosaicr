using Mosaicr.Configuration;
using Mosaicr.Engine;

return Run(args);

static int Run(string[] args)
{
    try
    {
        if (args.Length < 2)
        {
            PrintUsage();
            return 1;
        }

        var sourceDirectory = args[0];
        var outputFile = args[1];

        // Parse optional arguments
        string? configPath = null;
        int? tileWidth = null, tileHeight = null;
        int? columns = null, rows = null;
        bool? shuffle = null, fillWithImages = null;
        string? strategy = null;
        int? blur = null;

        for (int i = 2; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--config":
                    configPath = GetNextArg(args, ref i, "--config");
                    break;
                case "--tile-width":
                    tileWidth = int.Parse(GetNextArg(args, ref i, "--tile-width"));
                    break;
                case "--tile-height":
                    tileHeight = int.Parse(GetNextArg(args, ref i, "--tile-height"));
                    break;
                case "--columns":
                    columns = int.Parse(GetNextArg(args, ref i, "--columns"));
                    break;
                case "--rows":
                    rows = int.Parse(GetNextArg(args, ref i, "--rows"));
                    break;
                case "--shuffle":
                    shuffle = bool.Parse(GetNextArg(args, ref i, "--shuffle"));
                    break;
                case "--fill-with-images":
                    fillWithImages = bool.Parse(GetNextArg(args, ref i, "--fill-with-images"));
                    break;
                case "--strategy":
                    strategy = GetNextArg(args, ref i, "--strategy");
                    break;
                case "--blur":
                    blur = int.Parse(GetNextArg(args, ref i, "--blur"));
                    break;
                default:
                    Console.Error.WriteLine($"Unknown argument: {args[i]}");
                    PrintUsage();
                    return 1;
            }
        }

        // Validate source directory
        if (!Directory.Exists(sourceDirectory))
        {
            Console.Error.WriteLine($"Error: Source directory does not exist: {sourceDirectory}");
            return 1;
        }

        var jpgFiles = Directory.GetFiles(sourceDirectory, "*.jpg");
        if (jpgFiles.Length == 0)
        {
            Console.Error.WriteLine($"Error: Source directory contains no .jpg files: {sourceDirectory}");
            return 1;
        }

        // Validate output path
        var outputDir = Path.GetDirectoryName(Path.GetFullPath(outputFile));
        if (outputDir != null && !Directory.Exists(outputDir))
        {
            Console.Error.WriteLine($"Error: Output directory does not exist: {outputDir}");
            return 1;
        }

        // Step 1: Start with defaults
        MosaicSettings settings;

        // Step 2: If config file specified, parse it
        if (configPath != null)
        {
            if (!File.Exists(configPath))
            {
                Console.Error.WriteLine($"Error: Configuration file not found: {configPath}");
                return 1;
            }
            settings = ConfigurationParser.Parse(configPath);
        }
        else
        {
            settings = new MosaicSettings();
        }

        // Step 3: Apply command-line overrides
        if (tileWidth.HasValue)
        {
            if (tileWidth.Value <= 0) { Console.Error.WriteLine("Error: --tile-width must be a positive integer."); return 1; }
            settings.TileWidth = tileWidth.Value;
        }
        if (tileHeight.HasValue)
        {
            if (tileHeight.Value <= 0) { Console.Error.WriteLine("Error: --tile-height must be a positive integer."); return 1; }
            settings.TileHeight = tileHeight.Value;
        }
        if (columns.HasValue)
        {
            if (columns.Value <= 0) { Console.Error.WriteLine("Error: --columns must be a positive integer."); return 1; }
            settings.HorizontalTileCount = columns.Value;
        }
        if (rows.HasValue)
        {
            if (rows.Value <= 0) { Console.Error.WriteLine("Error: --rows must be a positive integer."); return 1; }
            settings.VerticalTileCount = rows.Value;
        }
        if (shuffle.HasValue)
            settings.ShuffleEnabled = shuffle.Value;
        if (fillWithImages.HasValue)
            settings.FillUpMissingTilesWithImages = fillWithImages.Value;
        if (strategy != null)
            settings.ImageFillingType = strategy;
        if (blur.HasValue)
        {
            if (blur.Value < 0) { Console.Error.WriteLine("Error: --blur must be a non-negative integer."); return 1; }
            settings.PlacementBlur = blur.Value;
        }

        // Execute
        Console.WriteLine("Mosaicr - Photo Mosaic Generator");
        Console.WriteLine($"Source: {Path.GetFullPath(sourceDirectory)}");
        Console.WriteLine($"Output: {Path.GetFullPath(outputFile)}");
        Console.WriteLine($"Tiles:  {jpgFiles.Length} images found");
        Console.WriteLine();

        MosaicEngine.CreateMosaic(sourceDirectory, outputFile, settings);

        // Print summary
        var outputInfo = new FileInfo(outputFile);
        var mosaicWidth = settings.TileWidth * settings.HorizontalTileCount;
        var mosaicHeight = settings.TileHeight * settings.VerticalTileCount;
        Console.WriteLine();
        Console.WriteLine("Mosaic created successfully!");
        Console.WriteLine($"  Output:     {Path.GetFullPath(outputFile)} ({outputInfo.Length / 1024} KB)");
        Console.WriteLine($"  Tile size:  {settings.TileWidth} x {settings.TileHeight}");
        Console.WriteLine($"  Grid:       {settings.HorizontalTileCount} columns x {settings.VerticalTileCount} rows");
        Console.WriteLine($"  Tiles used: {jpgFiles.Length}");

        return 0;
    }
    catch (FormatException ex)
    {
        Console.Error.WriteLine($"Error: Invalid argument value - {ex.Message}");
        return 1;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        return 1;
    }
}

static string GetNextArg(string[] args, ref int index, string name)
{
    index++;
    if (index >= args.Length)
        throw new ArgumentException($"Missing value for {name}");
    return args[index];
}

static void PrintUsage()
{
    Console.WriteLine("Mosaicr - Photo Mosaic Generator");
    Console.WriteLine();
    Console.WriteLine("Usage: mosaicr <sourceDir> <outputFile> [options]");
    Console.WriteLine();
    Console.WriteLine("Arguments:");
    Console.WriteLine("  sourceDir              Path to directory containing tile images (.jpg)");
    Console.WriteLine("  outputFile             Path for the output mosaic JPEG");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  --config <path>        Path to properties configuration file");
    Console.WriteLine("  --tile-width <int>     Override tile width in pixels");
    Console.WriteLine("  --tile-height <int>    Override tile height in pixels");
    Console.WriteLine("  --columns <int>        Override base horizontal tile count");
    Console.WriteLine("  --rows <int>           Override base vertical tile count");
    Console.WriteLine("  --shuffle <true|false> Override shuffle setting");
    Console.WriteLine("  --fill-with-images <true|false>  Override fill behavior");
    Console.WriteLine("  --strategy <name>      Override filling strategy name");
    Console.WriteLine("  --blur <int>           Override placement blur pixels");
}
