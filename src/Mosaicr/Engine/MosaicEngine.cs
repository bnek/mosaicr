using Mosaicr.Configuration;
using Mosaicr.ImageProcessing;
using Mosaicr.Strategy;
using SkiaSharp;

namespace Mosaicr.Engine;

public static class MosaicEngine
{
    public static void CreateMosaic(string sourceDirectory, string outputFile, MosaicSettings settings)
    {
        // Step 1: Prepare images
        var (tilePaths, tempFiles) = ImagePreparator.PrepareImages(
            sourceDirectory, settings.TileWidth, settings.TileHeight);

        try
        {
            // Step 2: Calculate grid dimensions
            var (columns, rows) = GridCalculator.Calculate(
                settings.HorizontalTileCount, settings.VerticalTileCount, tilePaths.Count);

            var canvasWidth = columns * settings.TileWidth;
            var canvasHeight = rows * settings.TileHeight;

            // Step 3: Create canvas filled with background color
            using var canvas = new SKBitmap(canvasWidth, canvasHeight);
            using var skCanvas = new SKCanvas(canvas);
            skCanvas.Clear(settings.BackgroundColor);

            // Step 4: Prepare image list
            var imageList = ImageListPreparator.Prepare(
                tilePaths, columns * rows, settings.ShuffleEnabled, settings.FillUpMissingTilesWithImages);

            // Step 5: Resolve filling strategy
            var strategy = StrategyFactory.Create(settings.ImageFillingType, settings.StrategyMappings);
            strategy.PlacementBlurPixels = settings.PlacementBlur;

            // Step 6: Fill the mosaic
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    var index = row * columns + col;
                    var tilePath = imageList[index];

                    var bounds = strategy.ComputeBoundsForNextTile(
                        canvasWidth, canvasHeight,
                        settings.TileWidth, settings.TileHeight,
                        col, row);

                    if (tilePath != null)
                    {
                        using var tile = SKBitmap.Decode(tilePath);
                        skCanvas.DrawBitmap(tile, new SKPoint(bounds.X, bounds.Y));
                    }
                    else
                    {
                        var fillColor = settings.TilesColorRanges.Count > 0
                            ? settings.TilesColorRanges.GetNextColor()
                            : settings.BackgroundColor;

                        using var paint = new SKPaint { Color = fillColor };
                        skCanvas.DrawRect(bounds.X, bounds.Y, bounds.Width, bounds.Height, paint);
                    }
                }
            }

            skCanvas.Flush();

            // Step 7 & 8: Write output (with grayscale conversion if needed)
            if (settings.TargetImageType == ImageType.BW)
            {
                using var grayscale = new SKBitmap(canvasWidth, canvasHeight);
                using var gsCanvas = new SKCanvas(grayscale);
                using var paint = new SKPaint();
                paint.ColorFilter = SKColorFilter.CreateColorMatrix(new float[]
                {
                    0.2126f, 0.7152f, 0.0722f, 0, 0,
                    0.2126f, 0.7152f, 0.0722f, 0, 0,
                    0.2126f, 0.7152f, 0.0722f, 0, 0,
                    0, 0, 0, 1, 0
                });
                gsCanvas.DrawBitmap(canvas, 0, 0, paint);
                gsCanvas.Flush();
                ImageWriter.Write(grayscale, outputFile);
            }
            else
            {
                ImageWriter.Write(canvas, outputFile);
            }
        }
        finally
        {
            // Step 9: Cleanup temporary files
            foreach (var tempFile in tempFiles)
            {
                try
                {
                    File.Delete(tempFile);
                }
                catch
                {
                    // Best-effort cleanup
                }
            }
        }
    }
}
