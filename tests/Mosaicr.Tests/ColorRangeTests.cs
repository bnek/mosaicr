using Mosaicr.Colors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Mosaicr.Tests;

public class ColorRangeTests
{
    [Fact]
    public void GetRandomColor_ReturnsColorWithinBounds()
    {
        var lower = Color.FromRgba(50, 100, 150, 255);
        var upper = Color.FromRgba(200, 200, 200, 255);
        var range = new ColorRange(lower, upper);

        for (var i = 0; i < 100; i++)
        {
            var color = range.GetRandomColor();
            var pixel = color.ToPixel<Rgba32>();

            Assert.InRange(pixel.R, 50, 200);
            Assert.InRange(pixel.G, 100, 200);
            Assert.InRange(pixel.B, 150, 200);
        }
    }

    [Fact]
    public void GetRandomColor_HandlesReversedBounds()
    {
        var lower = Color.FromRgba(200, 200, 200, 255);
        var upper = Color.FromRgba(50, 100, 150, 255);
        var range = new ColorRange(lower, upper);

        for (var i = 0; i < 100; i++)
        {
            var color = range.GetRandomColor();
            var pixel = color.ToPixel<Rgba32>();

            Assert.InRange(pixel.R, 50, 200);
            Assert.InRange(pixel.G, 100, 200);
            Assert.InRange(pixel.B, 150, 200);
        }
    }

    [Fact]
    public void GetRandomColor_SameColorBounds_ReturnsSameColor()
    {
        var color = Color.FromRgba(128, 64, 32, 255);
        var range = new ColorRange(color, color);

        var result = range.GetRandomColor();
        var pixel = result.ToPixel<Rgba32>();

        Assert.Equal(128, pixel.R);
        Assert.Equal(64, pixel.G);
        Assert.Equal(32, pixel.B);
    }
}
