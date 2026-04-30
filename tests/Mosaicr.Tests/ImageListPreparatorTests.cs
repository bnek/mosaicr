using Mosaicr.Engine;

namespace Mosaicr.Tests;

public class ImageListPreparatorTests
{
    [Fact]
    public void Prepare_ReturnsExactlyTilesNeededEntries()
    {
        var tiles = new List<string> { "a.jpg", "b.jpg", "c.jpg" };

        var result = ImageListPreparator.Prepare(tiles, 10, shuffleEnabled: false, fillWithImages: true);

        Assert.Equal(10, result.Count);
    }

    [Fact]
    public void Prepare_FillWithImages_CyclesThroughTiles()
    {
        var tiles = new List<string> { "a.jpg", "b.jpg", "c.jpg" };

        var result = ImageListPreparator.Prepare(tiles, 7, shuffleEnabled: false, fillWithImages: true);

        Assert.Equal(7, result.Count);
        // First 3 are the originals
        Assert.Equal("a.jpg", result[0]);
        Assert.Equal("b.jpg", result[1]);
        Assert.Equal("c.jpg", result[2]);
        // Fill cycles through: a, b, c, a
        Assert.Equal("a.jpg", result[3]);
        Assert.Equal("b.jpg", result[4]);
        Assert.Equal("c.jpg", result[5]);
        Assert.Equal("a.jpg", result[6]);
    }

    [Fact]
    public void Prepare_FillWithPlaceholders_AppendsNulls()
    {
        var tiles = new List<string> { "a.jpg", "b.jpg" };

        var result = ImageListPreparator.Prepare(tiles, 5, shuffleEnabled: false, fillWithImages: false);

        Assert.Equal(5, result.Count);
        Assert.Equal("a.jpg", result[0]);
        Assert.Equal("b.jpg", result[1]);
        Assert.Null(result[2]);
        Assert.Null(result[3]);
        Assert.Null(result[4]);
    }

    [Fact]
    public void Prepare_TilesMatchNeeded_NoFillNeeded()
    {
        var tiles = new List<string> { "a.jpg", "b.jpg", "c.jpg" };

        var result = ImageListPreparator.Prepare(tiles, 3, shuffleEnabled: false, fillWithImages: true);

        Assert.Equal(3, result.Count);
        Assert.Equal("a.jpg", result[0]);
        Assert.Equal("b.jpg", result[1]);
        Assert.Equal("c.jpg", result[2]);
    }

    [Fact]
    public void Prepare_ShuffleEnabled_RandomizesOrder()
    {
        var tiles = Enumerable.Range(1, 100).Select(i => $"{i}.jpg").ToList();

        var result = ImageListPreparator.Prepare(tiles, 100, shuffleEnabled: true, fillWithImages: false);

        Assert.Equal(100, result.Count);
        // With 100 items, the chance of the order being unchanged is astronomically low
        Assert.NotEqual(tiles.Cast<string?>().ToList(), result);
    }

    [Fact]
    public void Prepare_ShuffleEnabled_AllOriginalTilesPresent()
    {
        var tiles = new List<string> { "a.jpg", "b.jpg", "c.jpg" };

        var result = ImageListPreparator.Prepare(tiles, 3, shuffleEnabled: true, fillWithImages: false);

        Assert.Equal(3, result.Count);
        Assert.Contains("a.jpg", result);
        Assert.Contains("b.jpg", result);
        Assert.Contains("c.jpg", result);
    }

    [Fact]
    public void Prepare_ShuffleWithFill_DistributesFillUpsEvenly()
    {
        var tiles = new List<string> { "a.jpg" };

        var result = ImageListPreparator.Prepare(tiles, 5, shuffleEnabled: true, fillWithImages: false);

        Assert.Equal(5, result.Count);
        Assert.Single(result.Where(r => r == "a.jpg"));
        Assert.Equal(4, result.Count(r => r == null));
    }
}
