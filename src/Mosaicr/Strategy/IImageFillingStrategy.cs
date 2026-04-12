namespace Mosaicr.Strategy;

public interface IImageFillingStrategy
{
    int PlacementBlurPixels { get; set; }

    (int X, int Y, int Width, int Height) ComputeBoundsForNextTile(
        int canvasWidth, int canvasHeight,
        int tileWidth, int tileHeight,
        int xPos, int yPos);
}
