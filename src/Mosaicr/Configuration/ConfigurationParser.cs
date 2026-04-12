using SixLabors.ImageSharp;

namespace Mosaicr.Configuration;

public static class ConfigurationParser
{
    public static MosaicSettings Parse(string filePath)
    {
        using var reader = new StreamReader(filePath);
        return Parse(reader);
    }

    public static MosaicSettings Parse(TextReader reader)
    {
        var settings = new MosaicSettings();

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            line = line.Trim();

            if (string.IsNullOrEmpty(line) || line.StartsWith('#'))
                continue;

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex < 0)
                continue;

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim();

            if (string.IsNullOrEmpty(key))
                continue;

            try
            {
                ApplyProperty(settings, key, value);
            }
            catch
            {
                // Skip malformed values gracefully
            }
        }

        return settings;
    }

    private static void ApplyProperty(MosaicSettings settings, string key, string value)
    {
        switch (key)
        {
            case "defaultTileWidth":
                settings.TileWidth = int.Parse(value);
                break;
            case "defaultTileHeight":
                settings.TileHeight = int.Parse(value);
                break;
            case "defaultHorizontalTileCount":
                settings.HorizontalTileCount = int.Parse(value);
                break;
            case "defaultVerticalTileCount":
                settings.VerticalTileCount = int.Parse(value);
                break;
            case "shuffleEnabled":
                settings.ShuffleEnabled = bool.Parse(value);
                break;
            case "fillUpMissingTilesWithImages":
                settings.FillUpMissingTilesWithImages = bool.Parse(value);
                break;
            case "backgroundColor":
                settings.BackgroundColor = Color.ParseHex(value);
                break;
            case "tilesColorRange":
                settings.TilesColorRanges = MosaicSettings.ParseColorRanges(value);
                break;
            case "targetImageType":
                settings.TargetImageType = Enum.Parse<ImageType>(value, ignoreCase: true);
                break;
            case "imageFillingType":
                settings.ImageFillingType = value;
                break;
            case "placementBlur":
                settings.PlacementBlur = int.Parse(value);
                break;
            default:
                if (key.EndsWith("_CLASS"))
                {
                    settings.StrategyMappings[key] = value;
                }
                break;
        }
    }
}
