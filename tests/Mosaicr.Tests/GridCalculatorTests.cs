using Mosaicr.Engine;

namespace Mosaicr.Tests;

public class GridCalculatorTests
{
    [Fact]
    public void Calculate_ScalesUp_19x4With200Tiles_Returns38x8()
    {
        var (columns, rows) = GridCalculator.Calculate(19, 4, 200);

        Assert.Equal(38, columns);
        Assert.Equal(8, rows);
    }

    [Fact]
    public void Calculate_ExactFit_76TilesWith19x4_Returns19x4()
    {
        var (columns, rows) = GridCalculator.Calculate(19, 4, 76);

        Assert.Equal(19, columns);
        Assert.Equal(4, rows);
    }

    [Fact]
    public void Calculate_ZeroTiles_ReturnsBaseDimensions()
    {
        var (columns, rows) = GridCalculator.Calculate(19, 4, 0);

        Assert.Equal(19, columns);
        Assert.Equal(4, rows);
    }

    [Fact]
    public void Calculate_OneTileOverBase_ScalesUp()
    {
        var (columns, rows) = GridCalculator.Calculate(19, 4, 77);

        Assert.Equal(38, columns);
        Assert.Equal(8, rows);
    }

    [Fact]
    public void Calculate_FewerTilesThanBase_ReturnsBaseDimensions()
    {
        var (columns, rows) = GridCalculator.Calculate(19, 4, 10);

        Assert.Equal(19, columns);
        Assert.Equal(4, rows);
    }
}
