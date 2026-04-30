using Mosaicr.ImageProcessing;
using SkiaSharp;

namespace Mosaicr.Tests.ImageProcessing;

public class ImageCropperTests
{
    [Fact]
    public void CenterCrop_ImageTooWide_CropsHorizontally()
    {
        // Image is 800x400 (AR=2.0), tile is 200x300 (AR=0.667)
        // Image is too wide → keep full height, crop width
        // cropHeight = 400, cropWidth = 200 * (400/300) = 266
        // cropX = (800 - 266) / 2 = 267
        using var image = new SKBitmap(800, 400);

        using var result = ImageCropper.CenterCrop(image, 200, 300);

        // After crop: width should match tile aspect ratio
        var expectedWidth = (int)(200 * (400.0 / 300));
        Assert.Equal(expectedWidth, result.Width);
        Assert.Equal(400, result.Height);
    }

    [Fact]
    public void CenterCrop_ImageTooTall_CropsVertically()
    {
        // Image is 400x800 (AR=0.5), tile is 200x300 (AR=0.667)
        // Image is too tall → keep full width, crop height
        // cropWidth = 400, cropHeight = 300 * (400/200) = 600
        // cropY = (800 - 600) / 2 = 100
        using var image = new SKBitmap(400, 800);

        using var result = ImageCropper.CenterCrop(image, 200, 300);

        Assert.Equal(400, result.Width);
        var expectedHeight = (int)(300 * (400.0 / 200));
        Assert.Equal(expectedHeight, result.Height);
    }

    [Fact]
    public void CenterCrop_AspectRatioMatches_NoCropping()
    {
        // Image is 400x600 (AR=0.667), tile is 200x300 (AR=0.667)
        var image = new SKBitmap(400, 600);

        var result = ImageCropper.CenterCrop(image, 200, 300);

        Assert.Equal(400, result.Width);
        Assert.Equal(600, result.Height);
        result.Dispose();
    }

    [Fact]
    public void CenterCrop_TooWide_CropIsCentered()
    {
        // Verify crop is centered by checking pixel content
        // Create a 600x300 image, fill center column with green, rest with red
        var image = new SKBitmap(600, 300);
        var red = new SKColor(255, 0, 0);
        var green = new SKColor(0, 255, 0);

        for (int y = 0; y < 300; y++)
        {
            for (int x = 0; x < 600; x++)
            {
                // Mark center pixel column (x=300) as green
                image.SetPixel(x, y, (x == 300) ? green : red);
            }
        }

        // Crop to 1:1 aspect ratio (tile 100x100) → should keep 300x300 centered
        using var result = ImageCropper.CenterCrop(image, 100, 100);

        Assert.Equal(300, result.Width);
        Assert.Equal(300, result.Height);

        // The center of the original image (x=300) is now at x=150 in the cropped image
        var centerPixel = result.GetPixel(150, 150);
        Assert.Equal(green, centerPixel);
    }

    [Fact]
    public void CenterCrop_TooTall_CropIsCentered()
    {
        // Create a 300x600 image, fill center row with green, rest with red
        var image = new SKBitmap(300, 600);
        var red = new SKColor(255, 0, 0);
        var green = new SKColor(0, 255, 0);

        for (int y = 0; y < 600; y++)
        {
            for (int x = 0; x < 300; x++)
            {
                // Mark center pixel row (y=300) as green
                image.SetPixel(x, y, (y == 300) ? green : red);
            }
        }

        // Crop to 1:1 aspect ratio → should keep 300x300 centered
        using var result = ImageCropper.CenterCrop(image, 100, 100);

        Assert.Equal(300, result.Width);
        Assert.Equal(300, result.Height);

        // The center of the original (y=300) is now at y=150 in the cropped image
        var centerPixel = result.GetPixel(150, 150);
        Assert.Equal(green, centerPixel);
    }

    [Fact]
    public void CenterCrop_PreservesAspectRatioPrecisely()
    {
        // 1000x500 image, tile 200x300 (AR=0.667)
        // Image AR = 2.0 > tile AR → crop width
        using var image = new SKBitmap(1000, 500);

        using var result = ImageCropper.CenterCrop(image, 200, 300);

        // Result should have the same aspect ratio as the tile
        double resultAR = (double)result.Width / result.Height;
        double tileAR = 200.0 / 300.0;
        Assert.True(Math.Abs(resultAR - tileAR) < 0.01,
            $"Expected aspect ratio ~{tileAR:F3}, got {resultAR:F3}");
    }
}
