using SkiaSharp;

namespace Mosaicr.ImageProcessing;

public static class ImageWriter
{
    public static void WriteJpeg(SKBitmap bitmap, string outputPath)
    {
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 90);
        using var stream = File.OpenWrite(outputPath);
        data.SaveTo(stream);
    }
}
