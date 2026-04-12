using SixLabors.ImageSharp;

namespace Mosaicr.Configuration;

public class MosaicSettings
{
    public int TileWidth { get; set; } = 200;
    public int TileHeight { get; set; } = 300;
    public int HorizontalTileCount { get; set; } = 19;
    public int VerticalTileCount { get; set; } = 4;
    public bool ShuffleEnabled { get; set; } = true;
    public bool FillUpMissingTilesWithImages { get; set; } = false;
    public Color BackgroundColor { get; set; } = Color.ParseHex("#FF0000");
    public List<ColorRange> TilesColorRanges { get; set; } = ParseColorRanges("#FF4F00-#FFBF00,#FFA500-#FFC520");
    public ImageType TargetImageType { get; set; } = ImageType.RGB;
    public string ImageFillingType { get; set; } = "CIRCLE";
    public int PlacementBlur { get; set; } = 0;
    public Dictionary<string, string> StrategyMappings { get; set; } = new();

    internal static List<ColorRange> ParseColorRanges(string value)
    {
        var ranges = new List<ColorRange>();
        var pairs = value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var pair in pairs)
        {
            var parts = pair.Split('-', 2);
            if (parts.Length == 2)
            {
                // Each part is #RRGGBB — the second part starts with # so split on '-' with count 2
                // Handle case like "#FF4F00-#FFBF00" → ["#FF4F00", "#FFBF00"]
                var lower = parts[0].Trim();
                var upper = parts[1].Trim();
                if (lower.StartsWith('#') && upper.StartsWith('#'))
                {
                    ranges.Add(new ColorRange(Color.ParseHex(lower), Color.ParseHex(upper)));
                }
            }
        }
        return ranges;
    }
}
