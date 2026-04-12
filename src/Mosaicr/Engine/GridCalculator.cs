namespace Mosaicr.Engine;

public static class GridCalculator
{
    public static (int Columns, int Rows) Calculate(int baseColumns, int baseRows, int tilesAvailable)
    {
        var columns = baseColumns;
        var rows = baseRows;

        while (columns * rows < tilesAvailable)
        {
            columns += baseColumns;
            rows += baseRows;
        }

        return (columns, rows);
    }
}
