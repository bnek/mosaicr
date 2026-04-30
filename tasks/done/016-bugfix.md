run the example PS script and troubleshoot the error

---
## Status
- **Attempt:** 1
- **Moved to in-progress:** 2026-04-13

## Completion Notes
- Completed: 2026-04-13
- Summary: Fixed `StrategyFactory.KnownStrategies` dictionary keys from legacy Java class names (`SimpleImageFillingStrategy`, `CircleImageFillingStrategy`) to actual C# class names (`SimpleGridStrategy`, `CircleFillingStrategy`) matching the config file mappings. Updated corresponding test mappings in `StrategyFactoryTests.cs` and `MosaicEngineIntegrationTests.cs`. All 57 tests pass and the example script runs successfully.