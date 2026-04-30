namespace Mosaicr.Strategy;

public abstract class AbstractFillingStrategy : IImageFillingStrategy
{
    private static readonly Random RandomInstance = new();

    public int PlacementBlurPixels { get; set; }

    public abstract (int X, int Y, int Width, int Height) ComputeBoundsForNextTile(
        int canvasWidth, int canvasHeight,
        int tileWidth, int tileHeight,
        int xPos, int yPos);

    protected int GetBlurOffset()
    {
        if (PlacementBlurPixels <= 0)
            return 0;

        return RandomInstance.Next(0, PlacementBlurPixels + 1);
    }
}
