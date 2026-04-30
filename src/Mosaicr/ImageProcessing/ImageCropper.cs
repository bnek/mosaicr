using SkiaSharp;

namespace Mosaicr.ImageProcessing;

public static class ImageCropper
{
    public static SKBitmap CenterCrop(SKBitmap image, int tileWidth, int tileHeight)
    {
        double imageAR = (double)image.Width / image.Height;
        double tileAR = (double)tileWidth / tileHeight;

        if (Math.Abs(imageAR - tileAR) < 0.001)
        {
            // Aspect ratios match — no cropping needed
            return image;
        }

        int cropX, cropY, cropWidth, cropHeight;

        if (imageAR > tileAR)
        {
            // Image is too wide — keep full height, crop width
            cropHeight = image.Height;
            cropWidth = (int)(tileWidth * ((double)image.Height / tileHeight));
            cropX = (image.Width - cropWidth) / 2;
            cropY = 0;
        }
        else
        {
            // Image is too tall — keep full width, crop height
            cropWidth = image.Width;
            cropHeight = (int)(tileHeight * ((double)image.Width / tileWidth));
            cropX = 0;
            cropY = (image.Height - cropHeight) / 2;
        }

        var cropRect = new SKRectI(cropX, cropY, cropX + cropWidth, cropY + cropHeight);
        var cropped = new SKBitmap(cropWidth, cropHeight);
        using var canvas = new SKCanvas(cropped);
        canvas.DrawBitmap(image, cropRect, new SKRect(0, 0, cropWidth, cropHeight));
        canvas.Flush();

        image.Dispose();
        return cropped;
    }
}
