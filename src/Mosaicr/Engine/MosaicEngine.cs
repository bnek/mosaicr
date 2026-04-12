using Mosaicr.Configuration;
using Mosaicr.ImageProcessing;
using Mosaicr.Strategy;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

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
            using var canvas = new Image<Rgb24>(canvasWidth, canvasHeight);
            var bgPixel = settings.BackgroundColor.ToPixel<Rgb24>();
            canvas.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    row.Fill(bgPixel);
                }
            });

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
                        using var tile = Image.Load<Rgb24>(tilePath);
                        canvas.Mutate(ctx => ctx.DrawImage(tile, new Point(bounds.X, bounds.Y), 1f));
                    }
                    else
                    {
                        var fillColor = settings.TilesColorRanges.Count > 0
                            ? settings.TilesColorRanges.GetNextColor()
                            : settings.BackgroundColor;

                        var pixel = fillColor.ToPixel<Rgb24>();
                        FillRectangle(canvas, bounds.X, bounds.Y, bounds.Width, bounds.Height, pixel);
                    }
                }
            }

            // Step 7 & 8: Write output (with grayscale conversion if needed)
            if (settings.TargetImageType == ImageType.BW)
            {
                using var grayscale = canvas.CloneAs<L8>();
                ImageWriter.WriteJpeg(grayscale, outputFile);
            }
            else
            {
                ImageWriter.WriteJpeg(canvas, outputFile);
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

    private static void FillRectangle(Image<Rgb24> canvas, int x, int y, int width, int height, Rgb24 color)
    {
        var maxX = Math.Min(x + width, canvas.Width);
        var maxY = Math.Min(y + height, canvas.Height);
        var startX = Math.Max(x, 0);
        var startY = Math.Max(y, 0);

        canvas.ProcessPixelRows(accessor =>
        {
            for (int py = startY; py < maxY; py++)
            {
                var row = accessor.GetRowSpan(py);
                for (int px = startX; px < maxX; px++)
                {
                    row[px] = color;
                }
            }
        });
    }
}
