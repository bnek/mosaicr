using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Mosaicr.ImageProcessing;

public static class ImageResizer
{
    public static void Resize(Image image, int tileWidth, int tileHeight)
    {
        image.Mutate(ctx => ctx.Resize(new ResizeOptions
        {
            Size = new Size(tileWidth, tileHeight),
            Mode = ResizeMode.Stretch,
            Sampler = KnownResamplers.Bicubic
        }));
    }
}
