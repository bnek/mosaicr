using Mosaicr.Strategy;

namespace Mosaicr.Tests.Strategy;

public class CircleFillingStrategyTests
{
    [Fact]
    public void Ring0_PlacesTileAtCanvasCenter()
    {
        var strategy = new CircleFillingStrategy { PlacementBlurPixels = 0 };

        // Canvas 1000x800, tile 100x100, xPos=0 yPos=0 → index 0 → ring 0 center
        var result = strategy.ComputeBoundsForNextTile(1000, 800, 100, 100, 0, 0);

        Assert.Equal(450, result.X); // (1000/2) - (100/2)
        Assert.Equal(350, result.Y); // (800/2) - (100/2)
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Fact]
    public void Ring1_TilesArePlacedAtExpectedRadius()
    {
        var strategy = new CircleFillingStrategy { PlacementBlurPixels = 0 };

        // Canvas 1000x800, tile 100x100
        // Ring 1 radius = 100 (ringSpacing = max(100,100) = 100)
        // Center = (500, 400)
        // Index 1 = first tile in ring 1, angle = 0 → x = 500 + 100*cos(0) - 50 = 550, y = 400 + 100*sin(0) - 50 = 350
        var result = strategy.ComputeBoundsForNextTile(1000, 800, 100, 100, 1, 0);

        var centerX = 500.0;
        var centerY = 400.0;
        var tileCenterX = result.X + 50.0;
        var tileCenterY = result.Y + 50.0;
        var distFromCenter = Math.Sqrt(
            Math.Pow(tileCenterX - centerX, 2) + Math.Pow(tileCenterY - centerY, 2));

        Assert.InRange(distFromCenter, 95, 105); // approximately radius 100
    }

    [Fact]
    public void IndexExceedingCapacity_ReturnsOffCanvasBounds()
    {
        var strategy = new CircleFillingStrategy { PlacementBlurPixels = 0 };

        // Very small canvas: 200x200, tile 100x100
        // Max radius = min(200,200)/2 = 100
        // Ring 0: 1 tile, Ring 1: radius=100, exactly at boundary → included
        // Use a very high index that exceeds all positions
        // totalColumns = 200/100 = 2, so yPos=999 xPos=1 → index = 999*2+1 = 1999
        var result = strategy.ComputeBoundsForNextTile(200, 200, 100, 100, 1, 999);

        Assert.Equal(-100, result.X);
        Assert.Equal(-100, result.Y);
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);
    }

    [Fact]
    public void WithBlur_OffsetsPosition()
    {
        var strategy = new CircleFillingStrategy { PlacementBlurPixels = 50 };

        // Run many times to check blur is applied (at least some should differ from base)
        var baseStrategy = new CircleFillingStrategy { PlacementBlurPixels = 0 };
        var baseResult = baseStrategy.ComputeBoundsForNextTile(1000, 800, 100, 100, 0, 0);

        var anyDifferent = false;
        for (int i = 0; i < 100; i++)
        {
            var result = strategy.ComputeBoundsForNextTile(1000, 800, 100, 100, 0, 0);
            Assert.Equal(100, result.Width);
            Assert.Equal(100, result.Height);
            if (result.X != baseResult.X || result.Y != baseResult.Y)
                anyDifferent = true;
        }

        Assert.True(anyDifferent, "Blur should cause at least some positions to differ from the base");
    }

    [Fact]
    public void Positions_AreDeterministicWithNoBlur()
    {
        var strategy = new CircleFillingStrategy { PlacementBlurPixels = 0 };

        var result1 = strategy.ComputeBoundsForNextTile(1000, 800, 100, 100, 2, 0);
        var result2 = strategy.ComputeBoundsForNextTile(1000, 800, 100, 100, 2, 0);

        Assert.Equal(result1.X, result2.X);
        Assert.Equal(result1.Y, result2.Y);
        Assert.Equal(result1.Width, result2.Width);
        Assert.Equal(result1.Height, result2.Height);
    }

    [Fact]
    public void VerySmallCanvas_SingleTileOnly()
    {
        var strategy = new CircleFillingStrategy { PlacementBlurPixels = 0 };

        // Canvas exactly one tile: 100x100, tile 100x100
        // maxRadius = min(100,100)/2 = 50
        // Ring 0: 1 tile at center (0, 0)
        // Ring 1 radius = 100 > 50, so no more rings
        // Index 0 should be center tile
        var result = strategy.ComputeBoundsForNextTile(100, 100, 100, 100, 0, 0);

        Assert.Equal(0, result.X);  // (100/2) - (100/2) = 0
        Assert.Equal(0, result.Y);
        Assert.Equal(100, result.Width);
        Assert.Equal(100, result.Height);

        // Index 1 should be off-canvas (only 1 position in circle)
        var overflow = strategy.ComputeBoundsForNextTile(100, 100, 100, 100, 0, 1);
        Assert.Equal(-100, overflow.X);
        Assert.Equal(-100, overflow.Y);
    }
}
