using Mosaicr.Strategy;

namespace Mosaicr.Tests.Strategy;

public class SimpleGridStrategyTests
{
    [Fact]
    public void ComputeBounds_NoBlur_ReturnsExactGridPosition()
    {
        var strategy = new SimpleGridStrategy { PlacementBlurPixels = 0 };

        var result = strategy.ComputeBoundsForNextTile(1000, 800, 200, 300, 2, 1);

        Assert.Equal(400, result.X);
        Assert.Equal(300, result.Y);
        Assert.Equal(200, result.Width);
        Assert.Equal(300, result.Height);
    }

    [Fact]
    public void ComputeBounds_NoBlur_OriginPosition_ReturnsZeroZero()
    {
        var strategy = new SimpleGridStrategy { PlacementBlurPixels = 0 };

        var result = strategy.ComputeBoundsForNextTile(1000, 800, 200, 300, 0, 0);

        Assert.Equal(0, result.X);
        Assert.Equal(0, result.Y);
    }

    [Fact]
    public void ComputeBounds_WithBlur_ClampsToCanvasBounds()
    {
        var strategy = new SimpleGridStrategy { PlacementBlurPixels = 50 };

        var result = strategy.ComputeBoundsForNextTile(200, 300, 200, 300, 0, 0);

        Assert.True(result.X >= 0);
        Assert.True(result.X + result.Width <= 200);
        Assert.True(result.Y >= 0);
        Assert.True(result.Y + result.Height <= 300);
    }

    [Fact]
    public void ComputeBounds_NoBlur_LastCell_FitsWithinCanvas()
    {
        var strategy = new SimpleGridStrategy { PlacementBlurPixels = 0 };
        int canvasW = 5 * 200;
        int canvasH = 3 * 300;

        var result = strategy.ComputeBoundsForNextTile(canvasW, canvasH, 200, 300, 4, 2);

        Assert.Equal(800, result.X);
        Assert.Equal(600, result.Y);
        Assert.Equal(200, result.Width);
        Assert.Equal(300, result.Height);
    }

    [Fact]
    public void ComputeBounds_DimensionsAlwaysMatchTileSize()
    {
        var strategy = new SimpleGridStrategy { PlacementBlurPixels = 10 };

        for (int i = 0; i < 100; i++)
        {
            var result = strategy.ComputeBoundsForNextTile(1000, 800, 200, 300, 3, 2);
            Assert.Equal(200, result.Width);
            Assert.Equal(300, result.Height);
        }
    }
}
