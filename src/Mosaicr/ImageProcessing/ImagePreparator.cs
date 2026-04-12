using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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
            using var image = Image.Load<Rgb24>(filePath);

            if (image.Width == tileWidth && image.Height == tileHeight)
            {
                tilePaths.Add(filePath);
                continue;
            }

            ImageCropper.CenterCrop(image, tileWidth, tileHeight);
            ImageResizer.Resize(image, tileWidth, tileHeight);

            var tempPath = Path.Combine(
                Path.GetDirectoryName(filePath)!,
                Path.GetFileNameWithoutExtension(filePath) + ".rescaled.jpg");

            ImageWriter.WriteJpeg(image, tempPath);
            tempFiles.Add(tempPath);
            tilePaths.Add(tempPath);
        }

        return (tilePaths, tempFiles);
    }
}
