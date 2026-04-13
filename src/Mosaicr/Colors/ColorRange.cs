using SkiaSharp;

namespace Mosaicr.Colors;

public class ColorRange
{
    public SKColor LowerBound { get; }
    public SKColor UpperBound { get; }

    public ColorRange(SKColor lowerBound, SKColor upperBound)
    {
        LowerBound = lowerBound;
        UpperBound = upperBound;
    }

    public SKColor GetRandomColor()
    {
        var r = RandomChannel(LowerBound.Red, UpperBound.Red);
        var g = RandomChannel(LowerBound.Green, UpperBound.Green);
        var b = RandomChannel(LowerBound.Blue, UpperBound.Blue);

        return new SKColor(r, g, b, 255);
    }

    private static byte RandomChannel(byte a, byte b)
    {
        var min = Math.Min(a, b);
        var max = Math.Max(a, b);
        return (byte)Random.Shared.Next(min, max + 1);
    }
}
