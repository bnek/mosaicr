using Mosaicr.Strategy;

namespace Mosaicr.Tests.Strategy;

public class StrategyFactoryTests
{
    [Fact]
    public void Create_NormalStrategy_ReturnsSimpleGridStrategy()
    {
        var mappings = new Dictionary<string, string>
        {
            ["NORMAL_CLASS"] = "SimpleImageFillingStrategy"
        };

        var strategy = StrategyFactory.Create("NORMAL", mappings);

        Assert.IsType<SimpleGridStrategy>(strategy);
    }

    [Fact]
    public void Create_UnknownStrategyName_ThrowsArgumentException()
    {
        var mappings = new Dictionary<string, string>();

        var ex = Assert.Throws<ArgumentException>(() => StrategyFactory.Create("UNKNOWN", mappings));
        Assert.Contains("UNKNOWN", ex.Message);
        Assert.Contains("UNKNOWN_CLASS", ex.Message);
    }

    [Fact]
    public void Create_UnknownClassName_ThrowsArgumentException()
    {
        var mappings = new Dictionary<string, string>
        {
            ["FANCY_CLASS"] = "NonExistentStrategy"
        };

        var ex = Assert.Throws<ArgumentException>(() => StrategyFactory.Create("FANCY", mappings));
        Assert.Contains("NonExistentStrategy", ex.Message);
    }
}
