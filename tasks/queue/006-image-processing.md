# 006 - Image Processing

## Description

Implement the image processing pipeline: scanning a directory for JPEG files, center-cropping images to match the target tile aspect ratio, resizing to exact tile dimensions, writing JPEG output, and handling color space conversion.

## Requirements

- **Image Preparator**:
  - Scan a source directory for `.jpg` / `.jpeg` files
  - For each image that doesn't match the required tile dimensions:
    1. Center-crop to match target aspect ratio
    2. Resize to exact tile dimensions
    3. Save as a temporary file (`.rescaled.jpg` suffix)
  - Return a list of file paths (original if already correct size, temp file otherwise)
  - Track temporary files for later cleanup

- **Image Cropper** (center-crop algorithm):
  - Compare image aspect ratio vs tile aspect ratio
  - **Image too wide** (`imageAR > tileAR`): keep full height, crop width from center. `W_new = tileWidth × (H_i / tileHeight)`, `x_cutout = (W_i / 2) - (W_new / 2)`
  - **Image too tall** (`imageAR < tileAR`): keep full width, crop height from center. `H_new = tileHeight × (W_i / tileWidth)`, `y_cutout = (H_i / 2) - (H_new / 2)`
  - **Aspect ratios match**: no cropping needed

- **Image Resizer**:
  - Resize a cropped image to exact `(tileWidth, tileHeight)` pixel dimensions
  - Use bicubic interpolation for quality

- **Image Writer**:
  - Encode an in-memory image as JPEG and write to disk

- **Color Space Conversion**:
  - Detect when a loaded image's color space doesn't match the canvas (e.g., CMYK vs RGB)
  - Convert to RGB as needed

## Acceptance Criteria

- [ ] Directory scanning finds all `.jpg` and `.jpeg` files
- [ ] Center-crop correctly handles images that are too wide, too tall, or already correct
- [ ] Resized images are exactly the target tile dimensions
- [ ] JPEG output is written successfully
- [ ] Temporary files are tracked and can be listed for cleanup
- [ ] CMYK images are converted to RGB without errors

## References

- [docs/algorithms.md](../../docs/algorithms.md) — Section 2: Center-Crop, Section 3: Resize, Section 7: Color Space Conversion
- [docs/architecture.md](../../docs/architecture.md) — Image Preparator, Image Cropper, Image Resizer, Image Writer components

## Dependencies

- **003-project-setup** — project and ImageSharp package must exist
