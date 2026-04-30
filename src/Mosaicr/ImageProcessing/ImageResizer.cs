using SkiaSharp;

namespace Mosaicr.ImageProcessing;

public static class ImageResizer
{
    public static SKBitmap Resize(SKBitmap image, int tileWidth, int tileHeight)
    {
        var resized = image.Resize(new SKImageInfo(tileWidth, tileHeight), SKSamplingOptions.Default);
        image.Dispose();
        return resized;
    }
}
