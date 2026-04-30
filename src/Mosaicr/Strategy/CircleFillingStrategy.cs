namespace Mosaicr.Strategy;

public class CircleFillingStrategy : AbstractFillingStrategy
{
    private List<(int X, int Y)>? _positions;
    private int _cachedCanvasWidth;
    private int _cachedCanvasHeight;
    private int _cachedTileWidth;
    private int _cachedTileHeight;

    public override (int X, int Y, int Width, int Height) ComputeBoundsForNextTile(
        int canvasWidth, int canvasHeight,
        int tileWidth, int tileHeight,
        int xPos, int yPos)
    {
        EnsurePositionsComputed(canvasWidth, canvasHeight, tileWidth, tileHeight);

        var totalColumns = canvasWidth / tileWidth;
        var index = yPos * totalColumns + xPos;

        var blurOffset = GetBlurOffset();

        if (index < 0 || index >= _positions!.Count)
            return (-tileWidth, -tileHeight, tileWidth, tileHeight);

        var pos = _positions[index];
        return (pos.X + blurOffset, pos.Y + blurOffset, tileWidth, tileHeight);
    }

    private void EnsurePositionsComputed(int canvasWidth, int canvasHeight, int tileWidth, int tileHeight)
    {
        if (_positions != null
            && _cachedCanvasWidth == canvasWidth
            && _cachedCanvasHeight == canvasHeight
            && _cachedTileWidth == tileWidth
            && _cachedTileHeight == tileHeight)
            return;

        _cachedCanvasWidth = canvasWidth;
        _cachedCanvasHeight = canvasHeight;
        _cachedTileWidth = tileWidth;
        _cachedTileHeight = tileHeight;

        _positions = ComputeCirclePositions(canvasWidth, canvasHeight, tileWidth, tileHeight);
    }

    private static List<(int X, int Y)> ComputeCirclePositions(
        int canvasWidth, int canvasHeight, int tileWidth, int tileHeight)
    {
        var positions = new List<(int X, int Y)>();

        var centerX = canvasWidth / 2.0;
        var centerY = canvasHeight / 2.0;
        var maxTileDim = Math.Max(tileWidth, tileHeight);
        var ringSpacing = maxTileDim;
        var maxRadius = Math.Min(canvasWidth, canvasHeight) / 2.0;

        // Ring 0: center tile
        positions.Add(((int)(centerX - tileWidth / 2.0), (int)(centerY - tileHeight / 2.0)));

        // Ring k (k >= 1)
        for (var k = 1; ; k++)
        {
            var rk = k * ringSpacing;
            if (rk > maxRadius)
                break;

            var nk = (int)Math.Floor(2 * Math.PI * rk / maxTileDim);
            if (nk < 1) nk = 1;

            for (var i = 0; i < nk; i++)
            {
                var angle = 2 * Math.PI * i / nk;
                var x = (int)(centerX + rk * Math.Cos(angle) - tileWidth / 2.0);
                var y = (int)(centerY + rk * Math.Sin(angle) - tileHeight / 2.0);
                positions.Add((x, y));
            }
        }

        return positions;
    }
}
