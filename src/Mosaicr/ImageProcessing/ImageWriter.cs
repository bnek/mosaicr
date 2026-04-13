using SkiaSharp;

namespace Mosaicr.ImageProcessing;

public static class ImageWriter
{
    public static void Write(SKBitmap bitmap, string outputPath)
    {
        var ext = Path.GetExtension(outputPath).ToLowerInvariant();
        var (format, quality) = ext == ".png"
            ? (SKEncodedImageFormat.Png, 100)
            : (SKEncodedImageFormat.Jpeg, 90);

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(format, quality);
        using var stream = File.OpenWrite(outputPath);
        data.SaveTo(stream);
    }
}
