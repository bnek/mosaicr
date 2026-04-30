implement png support

---
## Status
- **Attempt:** 1
- **Moved to in-progress:** 2026-04-13

## Completion Notes
- Completed: 2026-04-13
- Summary: Added PNG support for both input and output. ImagePreparator now accepts .png files, ImageWriter detects format from output file extension (.png → PNG, else JPEG), MosaicEngine uses format-aware writing, and Program.cs validates/counts both .jpg and .png source files. All 57 tests pass.