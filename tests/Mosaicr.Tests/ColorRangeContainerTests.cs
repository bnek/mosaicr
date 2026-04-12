using Mosaicr.Colors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Mosaicr.Tests;

public class ColorRangeContainerTests
{
    [Fact]
    public void Parse_ValidString_CreatesCorrectRanges()
    {
        var container = ColorRangeContainer.Parse("#FF4F00-#FFBF00,#FFA500-#FFC520");

        Assert.Equal(2, container.Count);
    }

    [Fact]
    public void Parse_SingleRange_Works()
    {
        var container = ColorRangeContainer.Parse("#FF4F00-#FFBF00");

        Assert.Equal(1, container.Count);
    }

    [Fact]
    public void Parse_Null_ReturnsEmptyContainer()
    {
        var container = ColorRangeContainer.Parse(null);

        Assert.Equal(0, container.Count);
    }

    [Fact]
    public void Parse_EmptyString_ReturnsEmptyContainer()
    {
        var container = ColorRangeContainer.Parse("");

        Assert.Equal(0, container.Count);
    }

    [Fact]
    public void Parse_WhitespaceOnly_ReturnsEmptyContainer()
    {
        var container = ColorRangeContainer.Parse("   ");

        Assert.Equal(0, container.Count);
    }

    [Fact]
    public void GetNextColor_ReturnsColorFromAvailableRanges()
    {
        var container = ColorRangeContainer.Parse("#FF4F00-#FFBF00");

        for (var i = 0; i < 50; i++)
        {
            var color = container.GetNextColor();
            var pixel = color.ToPixel<Rgba32>();

            Assert.InRange(pixel.R, 0xFF, 0xFF);
            Assert.InRange(pixel.G, 0x4F, 0xBF);
            Assert.InRange(pixel.B, 0x00, 0x00);
        }
    }

    [Fact]
    public void GetNextColor_EmptyContainer_Throws()
    {
        var container = ColorRangeContainer.Parse("");

        Assert.Throws<InvalidOperationException>(() => container.GetNextColor());
    }

    [Fact]
    public void Add_IncreasesCount()
    {
        var container = new ColorRangeContainer();
        Assert.Equal(0, container.Count);

        container.Add(new ColorRange(Color.Black, Color.White));
        Assert.Equal(1, container.Count);

        container.Add(new ColorRange(Color.Red, Color.Blue));
        Assert.Equal(2, container.Count);
    }
}
