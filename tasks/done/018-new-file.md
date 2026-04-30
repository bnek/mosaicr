if the output file exists, create one with a timestamp attached to the name so that we dont overwrite the existing.

---
## Status
- **Attempt:** 1
- **Moved to in-progress:** 2026-04-13

## Completion Notes
- Completed: 2026-04-13
- Summary: Added logic in Program.cs to check if the output file exists before creating the mosaic. If it does, a timestamp (yyyyMMdd_HHmmss) is appended to the filename to avoid overwriting. Build succeeds, all 57 tests pass. Committed as dbaec03.