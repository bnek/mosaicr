using Mosaicr.Colors;
using SkiaSharp;

namespace Mosaicr.Tests;

public class ColorRangeTests
{
    [Fact]
    public void GetRandomColor_ReturnsColorWithinBounds()
    {
        var lower = new SKColor(50, 100, 150, 255);
        var upper = new SKColor(200, 200, 200, 255);
        var range = new ColorRange(lower, upper);

        for (var i = 0; i < 100; i++)
        {
            var color = range.GetRandomColor();

            Assert.InRange(color.Red, 50, 200);
            Assert.InRange(color.Green, 100, 200);
            Assert.InRange(color.Blue, 150, 200);
        }
    }

    [Fact]
    public void GetRandomColor_HandlesReversedBounds()
    {
        var lower = new SKColor(200, 200, 200, 255);
        var upper = new SKColor(50, 100, 150, 255);
        var range = new ColorRange(lower, upper);

        for (var i = 0; i < 100; i++)
        {
            var color = range.GetRandomColor();

            Assert.InRange(color.Red, 50, 200);
            Assert.InRange(color.Green, 100, 200);
            Assert.InRange(color.Blue, 150, 200);
        }
    }

    [Fact]
    public void GetRandomColor_SameColorBounds_ReturnsSameColor()
    {
        var color = new SKColor(128, 64, 32, 255);
        var range = new ColorRange(color, color);

        var result = range.GetRandomColor();

        Assert.Equal(128, result.Red);
        Assert.Equal(64, result.Green);
        Assert.Equal(32, result.Blue);
    }
}
