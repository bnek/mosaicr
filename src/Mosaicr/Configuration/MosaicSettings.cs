using Mosaicr.Colors;
using SkiaSharp;

namespace Mosaicr.Configuration;

public class MosaicSettings
{
    public int TileWidth { get; set; } = 200;
    public int TileHeight { get; set; } = 300;
    public int HorizontalTileCount { get; set; } = 19;
    public int VerticalTileCount { get; set; } = 4;
    public bool ShuffleEnabled { get; set; } = true;
    public bool FillUpMissingTilesWithImages { get; set; } = false;
    public SKColor BackgroundColor { get; set; } = SKColor.Parse("#FF0000");
    public ColorRangeContainer TilesColorRanges { get; set; } = ColorRangeContainer.Parse("#FF4F00-#FFBF00,#FFA500-#FFC520");
    public ImageType TargetImageType { get; set; } = ImageType.RGB;
    public string ImageFillingType { get; set; } = "CIRCLE";
    public int PlacementBlur { get; set; } = 0;
    public Dictionary<string, string> StrategyMappings { get; set; } = new();
}
