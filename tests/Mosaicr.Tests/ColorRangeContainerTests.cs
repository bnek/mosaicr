using Mosaicr.Colors;
using SkiaSharp;

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

            Assert.InRange(color.Red, 0xFF, 0xFF);
            Assert.InRange(color.Green, 0x4F, 0xBF);
            Assert.InRange(color.Blue, 0x00, 0x00);
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

        container.Add(new ColorRange(SKColors.Black, SKColors.White));
        Assert.Equal(1, container.Count);

        container.Add(new ColorRange(SKColors.Red, SKColors.Blue));
        Assert.Equal(2, container.Count);
    }
}
