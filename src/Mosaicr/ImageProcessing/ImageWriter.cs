using SixLabors.ImageSharp;

namespace Mosaicr.ImageProcessing;

public static class ImageWriter
{
    public static void WriteJpeg(Image image, string outputPath)
    {
        image.SaveAsJpeg(outputPath);
    }
}
