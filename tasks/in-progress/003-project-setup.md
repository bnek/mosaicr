# 003 - Project Setup

## Description

Create a C# console application project for the Mosaicr photo mosaic generator. Set up the solution structure, add necessary NuGet packages for image processing, and create the folder structure that will house all components.

## Requirements

- Create a .NET 8+ console application project
- Create a solution file (`Mosaicr.sln`) at the repository root
- Main project: `src/Mosaicr/` (console app)
- Test project: `tests/Mosaicr.Tests/` (xUnit)
- Add NuGet package **SixLabors.ImageSharp** (cross-platform image processing library — preferred over System.Drawing.Common which is Windows-only)
- Create folder structure in the main project reflecting the architecture:
  - `Configuration/` — config parsing and settings classes
  - `Colors/` — ColorRange and ColorRangeContainer
  - `ImageProcessing/` — Image Preparator, Cropper, Resizer, Writer
  - `Strategies/` — Filling strategy interface, implementations, factory
  - `Engine/` — Mosaic Engine
- Add a minimal `Program.cs` entry point that compiles and runs (placeholder)

## Acceptance Criteria

- [ ] Solution builds successfully with `dotnet build`
- [ ] `dotnet run` executes without errors (even if it does nothing yet)
- [ ] Test project builds and `dotnet test` passes (even with no tests yet)
- [ ] ImageSharp NuGet package is referenced
- [ ] Folder structure exists with placeholder files or namespaces

## References

- [docs/architecture.md](../../docs/architecture.md) — component overview and folder structure rationale

## Dependencies

- None (this is the first implementation task)

---
## Status
- **Attempt:** 1
- **Moved to in-progress:** 2026-04-12
