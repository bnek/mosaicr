namespace Mosaicr.Strategy;

public class SimpleGridStrategy : AbstractFillingStrategy
{
    public override (int X, int Y, int Width, int Height) ComputeBoundsForNextTile(
        int canvasWidth, int canvasHeight,
        int tileWidth, int tileHeight,
        int xPos, int yPos)
    {
        var blurOffset = GetBlurOffset();

        var x = (xPos * tileWidth) + blurOffset;
        var width = tileWidth;
        if (x + width > canvasWidth)
            x = canvasWidth - width;

        var y = (yPos * tileHeight) + blurOffset;
        var height = tileHeight;
        if (y + height > canvasHeight)
            y = canvasHeight - height;

        return (x, y, width, height);
    }
}
