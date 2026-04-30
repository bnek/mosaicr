using Mosaicr.Configuration;
using SkiaSharp;

namespace Mosaicr.Tests.Configuration;

public class ConfigurationParserTests
{
    private static MosaicSettings ParseFromString(string content)
    {
        using var reader = new StringReader(content);
        return ConfigurationParser.Parse(reader);
    }

    private static SKColor ToColor(SKColor color) => color;

    [Fact]
    public void Parse_CompletePropertiesFile_AllValuesSet()
    {
        var content = """
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
            """;

        var settings = ParseFromString(content);

        Assert.Equal(200, settings.TileWidth);
        Assert.Equal(300, settings.TileHeight);
        Assert.Equal(19, settings.HorizontalTileCount);
        Assert.Equal(4, settings.VerticalTileCount);
        Assert.True(settings.ShuffleEnabled);
        Assert.False(settings.FillUpMissingTilesWithImages);
        Assert.Equal(ImageType.RGB, settings.TargetImageType);
        Assert.Equal("NORMAL", settings.ImageFillingType);
        Assert.Equal(0, settings.PlacementBlur);

        var bg = settings.BackgroundColor;
        Assert.Equal(255, bg.Red);
        Assert.Equal(0, bg.Green);
        Assert.Equal(0, bg.Blue);

        Assert.Equal(2, settings.TilesColorRanges.Count);
        Assert.Equal(2, settings.StrategyMappings.Count);
        Assert.Equal("SimpleImageFillingStrategy", settings.StrategyMappings["NORMAL_CLASS"]);
        Assert.Equal("CircleImageFillingStrategy", settings.StrategyMappings["CIRCLE_CLASS"]);
    }

    [Fact]
    public void Parse_EmptyInput_ReturnsDefaults()
    {
        var settings = ParseFromString("");

        Assert.Equal(200, settings.TileWidth);
        Assert.Equal(300, settings.TileHeight);
        Assert.Equal(19, settings.HorizontalTileCount);
        Assert.Equal(4, settings.VerticalTileCount);
        Assert.True(settings.ShuffleEnabled);
        Assert.False(settings.FillUpMissingTilesWithImages);
        Assert.Equal(ImageType.RGB, settings.TargetImageType);
        Assert.Equal("CIRCLE", settings.ImageFillingType);
        Assert.Equal(0, settings.PlacementBlur);
        Assert.Empty(settings.StrategyMappings);

        // Default color ranges should be present
        Assert.Equal(2, settings.TilesColorRanges.Count);

        // Default background color is black
        var bg = settings.BackgroundColor;
        Assert.Equal(0, bg.Red);
        Assert.Equal(0, bg.Green);
        Assert.Equal(0, bg.Blue);
    }

    [Fact]
    public void Parse_BackgroundColorHex_ParsedCorrectly()
    {
        var settings = ParseFromString("backgroundColor=#00FF00");

        var bg = settings.BackgroundColor;
        Assert.Equal(0, bg.Red);
        Assert.Equal(255, bg.Green);
        Assert.Equal(0, bg.Blue);
    }

    [Fact]
    public void Parse_TilesColorRange_ParsedCorrectly()
    {
        var settings = ParseFromString("tilesColorRange=#112233-#445566,#AABBCC-#DDEEFF");

        Assert.Equal(2, settings.TilesColorRanges.Count);

        var range1Lower = settings.TilesColorRanges[0].LowerBound;
        Assert.Equal(0x11, range1Lower.Red);
        Assert.Equal(0x22, range1Lower.Green);
        Assert.Equal(0x33, range1Lower.Blue);

        var range1Upper = settings.TilesColorRanges[0].UpperBound;
        Assert.Equal(0x44, range1Upper.Red);
        Assert.Equal(0x55, range1Upper.Green);
        Assert.Equal(0x66, range1Upper.Blue);

        var range2Lower = settings.TilesColorRanges[1].LowerBound;
        Assert.Equal(0xAA, range2Lower.Red);
        Assert.Equal(0xBB, range2Lower.Green);
        Assert.Equal(0xCC, range2Lower.Blue);

        var range2Upper = settings.TilesColorRanges[1].UpperBound;
        Assert.Equal(0xDD, range2Upper.Red);
        Assert.Equal(0xEE, range2Upper.Green);
        Assert.Equal(0xFF, range2Upper.Blue);
    }

    [Fact]
    public void Parse_StrategyClassMappings_CapturedCorrectly()
    {
        var content = """
            NORMAL_CLASS=SimpleImageFillingStrategy
            CIRCLE_CLASS=CircleImageFillingStrategy
            CUSTOM_CLASS=MyCustomStrategy
            """;

        var settings = ParseFromString(content);

        Assert.Equal(3, settings.StrategyMappings.Count);
        Assert.Equal("SimpleImageFillingStrategy", settings.StrategyMappings["NORMAL_CLASS"]);
        Assert.Equal("CircleImageFillingStrategy", settings.StrategyMappings["CIRCLE_CLASS"]);
        Assert.Equal("MyCustomStrategy", settings.StrategyMappings["CUSTOM_CLASS"]);
    }

    [Fact]
    public void Parse_MalformedLines_SkippedGracefully()
    {
        var content = """
            defaultTileWidth=200
            this line has no equals sign
            =valueWithNoKey
            defaultTileHeight=not_a_number
            defaultHorizontalTileCount=10
            """;

        var settings = ParseFromString(content);

        Assert.Equal(200, settings.TileWidth);
        Assert.Equal(300, settings.TileHeight); // default, because "not_a_number" fails parsing
        Assert.Equal(10, settings.HorizontalTileCount);
    }

    [Fact]
    public void Parse_CommentsAndBlankLines_Ignored()
    {
        var content = """
            # This is a comment
            defaultTileWidth=150

            # Another comment
            defaultTileHeight=250
            """;

        var settings = ParseFromString(content);

        Assert.Equal(150, settings.TileWidth);
        Assert.Equal(250, settings.TileHeight);
    }

    [Fact]
    public void Parse_TargetImageType_BW()
    {
        var settings = ParseFromString("targetImageType=BW");

        Assert.Equal(ImageType.BW, settings.TargetImageType);
    }

    [Fact]
    public void Parse_OverridesOnlySpecifiedValues()
    {
        var settings = ParseFromString("defaultTileWidth=100");

        Assert.Equal(100, settings.TileWidth);
        Assert.Equal(300, settings.TileHeight); // still default
        Assert.Equal(19, settings.HorizontalTileCount); // still default
    }
}
