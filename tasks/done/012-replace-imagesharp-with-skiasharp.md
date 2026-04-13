replace imagesharp with skiasharp as it's open source

---
## Status
- **Attempt:** 1
- **Moved to in-progress:** 2026-04-13

## Completion Notes
- Completed: 2026-04-13
- Summary: Replaced SixLabors.ImageSharp with SkiaSharp across all 10 source files and 5 test files. Updated package reference in csproj. Adapted APIs (Color→SKColor, Image→SKBitmap, in-place mutation→return new objects). All 50 tests pass. Committed as "Replace SixLabors.ImageSharp with SkiaSharp".