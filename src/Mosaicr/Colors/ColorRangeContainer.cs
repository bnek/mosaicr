using SkiaSharp;

namespace Mosaicr.Colors;

public class ColorRangeContainer
{
    private readonly List<ColorRange> _ranges = new();

    public int Count => _ranges.Count;

    public ColorRange this[int index] => _ranges[index];

    public void Add(ColorRange range)
    {
        _ranges.Add(range);
    }

    public SKColor GetNextColor()
    {
        if (_ranges.Count == 0)
            throw new InvalidOperationException("No color ranges available.");

        var index = Random.Shared.Next(_ranges.Count);
        return _ranges[index].GetRandomColor();
    }

    public static ColorRangeContainer Parse(string? input)
    {
        var container = new ColorRangeContainer();

        if (string.IsNullOrWhiteSpace(input))
            return container;

        var pairs = input.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var pair in pairs)
        {
            var parts = pair.Split('-', 2);
            if (parts.Length == 2)
            {
                var lower = parts[0].Trim();
                var upper = parts[1].Trim();
                if (lower.StartsWith('#') && upper.StartsWith('#'))
                {
                    container.Add(new ColorRange(SKColor.Parse(lower), SKColor.Parse(upper)));
                }
            }
        }

        return container;
    }
}
