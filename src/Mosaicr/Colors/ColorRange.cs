using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Mosaicr.Colors;

public class ColorRange
{
    public Color LowerBound { get; }
    public Color UpperBound { get; }

    public ColorRange(Color lowerBound, Color upperBound)
    {
        LowerBound = lowerBound;
        UpperBound = upperBound;
    }

    public Color GetRandomColor()
    {
        var lo = LowerBound.ToPixel<Rgba32>();
        var hi = UpperBound.ToPixel<Rgba32>();

        var r = RandomChannel(lo.R, hi.R);
        var g = RandomChannel(lo.G, hi.G);
        var b = RandomChannel(lo.B, hi.B);

        return Color.FromRgba(r, g, b, 255);
    }

    private static byte RandomChannel(byte a, byte b)
    {
        var min = Math.Min(a, b);
        var max = Math.Max(a, b);
        return (byte)Random.Shared.Next(min, max + 1);
    }
}
