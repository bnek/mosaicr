using SkiaSharp;

namespace Mosaicr.ImageProcessing;

public static class ImagePreparator
{
    private static readonly string[] JpegExtensions = [".jpg", ".jpeg"];

    public static (List<string> TilePaths, List<string> TempFiles) PrepareImages(
        string sourceDirectory, int tileWidth, int tileHeight)
    {
        var tilePaths = new List<string>();
        var tempFiles = new List<string>();

        var imageFiles = Directory.EnumerateFiles(sourceDirectory)
            .Where(f => JpegExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
            .OrderBy(f => f)
            .ToList();

        foreach (var filePath in imageFiles)
        {
            var image = SKBitmap.Decode(filePath);

            if (image.Width == tileWidth && image.Height == tileHeight)
            {
                image.Dispose();
                tilePaths.Add(filePath);
                continue;
            }

            image = ImageCropper.CenterCrop(image, tileWidth, tileHeight);
            image = ImageResizer.Resize(image, tileWidth, tileHeight);

            var tempPath = Path.Combine(
                Path.GetDirectoryName(filePath)!,
                Path.GetFileNameWithoutExtension(filePath) + ".rescaled.jpg");

            ImageWriter.WriteJpeg(image, tempPath);
            image.Dispose();
            tempFiles.Add(tempPath);
            tilePaths.Add(tempPath);
        }

        return (tilePaths, tempFiles);
    }
}
