using Mosaicr.Configuration;
using Mosaicr.Engine;
using SkiaSharp;

namespace Mosaicr.Tests.Integration;

public class MosaicEngineIntegrationTests : IDisposable
{
    private readonly string _testDir;
    private readonly string _outputFile;

    public MosaicEngineIntegrationTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), "mosaicr-test-" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(_testDir);
        _outputFile = Path.Combine(_testDir, "output.jpg");

        // Create small test JPEG images (100x100 px each, different colors)
        CreateTestImage(Path.Combine(_testDir, "tile1.jpg"), 100, 100, new SKColor(255, 0, 0));
        CreateTestImage(Path.Combine(_testDir, "tile2.jpg"), 100, 100, new SKColor(0, 255, 0));
        CreateTestImage(Path.Combine(_testDir, "tile3.jpg"), 100, 100, new SKColor(0, 0, 255));
        CreateTestImage(Path.Combine(_testDir, "tile4.jpg"), 100, 100, new SKColor(255, 255, 0));
        CreateTestImage(Path.Combine(_testDir, "tile5.jpg"), 100, 100, new SKColor(255, 0, 255));
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_testDir))
                Directory.Delete(_testDir, recursive: true);
        }
        catch
        {
            // Best-effort cleanup
        }
    }

    [Fact]
    public void CreateMosaic_FullPipeline_ProducesValidOutput()
    {
        var settings = new MosaicSettings
        {
            TileWidth = 100,
            TileHeight = 100,
            HorizontalTileCount = 3,
            VerticalTileCount = 2,
            ShuffleEnabled = false,
            FillUpMissingTilesWithImages = true,
            ImageFillingType = "NORMAL",
            PlacementBlur = 0,
            StrategyMappings = new Dictionary<string, string>
            {
                ["NORMAL_CLASS"] = "SimpleImageFillingStrategy"
            }
        };

        MosaicEngine.CreateMosaic(_testDir, _outputFile, settings);

        // Verify output file exists
        Assert.True(File.Exists(_outputFile), "Output mosaic file should exist");

        // Verify output has expected dimensions
        using var output = SKBitmap.Decode(_outputFile);
        // 5 tiles, 3x2 = 6 cells, so grid stays at 3x2
        Assert.Equal(3 * 100, output.Width);
        Assert.Equal(2 * 100, output.Height);
    }

    [Fact]
    public void CreateMosaic_WithPlaceholders_ProducesValidOutput()
    {
        var settings = new MosaicSettings
        {
            TileWidth = 100,
            TileHeight = 100,
            HorizontalTileCount = 3,
            VerticalTileCount = 2,
            ShuffleEnabled = false,
            FillUpMissingTilesWithImages = false,
            ImageFillingType = "NORMAL",
            PlacementBlur = 0,
            StrategyMappings = new Dictionary<string, string>
            {
                ["NORMAL_CLASS"] = "SimpleImageFillingStrategy"
            }
        };

        MosaicEngine.CreateMosaic(_testDir, _outputFile, settings);

        Assert.True(File.Exists(_outputFile));

        using var output = SKBitmap.Decode(_outputFile);
        Assert.Equal(300, output.Width);
        Assert.Equal(200, output.Height);
    }

    [Fact]
    public void CreateMosaic_GrayscaleMode_ProducesValidOutput()
    {
        var settings = new MosaicSettings
        {
            TileWidth = 100,
            TileHeight = 100,
            HorizontalTileCount = 3,
            VerticalTileCount = 2,
            ShuffleEnabled = false,
            FillUpMissingTilesWithImages = true,
            TargetImageType = ImageType.BW,
            ImageFillingType = "NORMAL",
            PlacementBlur = 0,
            StrategyMappings = new Dictionary<string, string>
            {
                ["NORMAL_CLASS"] = "SimpleImageFillingStrategy"
            }
        };

        MosaicEngine.CreateMosaic(_testDir, _outputFile, settings);

        Assert.True(File.Exists(_outputFile));

        using var output = SKBitmap.Decode(_outputFile);
        Assert.Equal(300, output.Width);
        Assert.Equal(200, output.Height);
    }

    [Fact]
    public void CreateMosaic_WithNonSquareTiles_CropsAndResizes()
    {
        // Test images are 100x100 but tiles are 50x75, so cropping+resizing is needed
        var settings = new MosaicSettings
        {
            TileWidth = 50,
            TileHeight = 75,
            HorizontalTileCount = 3,
            VerticalTileCount = 2,
            ShuffleEnabled = false,
            FillUpMissingTilesWithImages = true,
            ImageFillingType = "NORMAL",
            PlacementBlur = 0,
            StrategyMappings = new Dictionary<string, string>
            {
                ["NORMAL_CLASS"] = "SimpleImageFillingStrategy"
            }
        };

        MosaicEngine.CreateMosaic(_testDir, _outputFile, settings);

        Assert.True(File.Exists(_outputFile));

        using var output = SKBitmap.Decode(_outputFile);
        Assert.Equal(3 * 50, output.Width);
        Assert.Equal(2 * 75, output.Height);
    }

    private static void CreateTestImage(string path, int width, int height, SKColor color)
    {
        using var bitmap = new SKBitmap(width, height);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(color);
        canvas.Flush();

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 90);
        using var stream = File.OpenWrite(path);
        data.SaveTo(stream);
    }
}
