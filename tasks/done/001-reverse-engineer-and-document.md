This is an old java app that I wrote to create mosaics. it's in the 'java-legacy-version' directory.
I want to migrate this to C# because I'm more fluent in C# nowadays.

The first step is reverse engineering and documenting the concept from the existing java code. Explore the app and produce detailed documentation in different markdown files that is platform agnostic and can later be used to implement the app in different languages.


Constraints:
- ignore the FLickr integration as it's no longer relevant.

---
## Status
- **Attempt:** 1
- **Moved to in-progress:** 2026-04-12

## Completion Notes
- Completed: 2026-04-12
- Summary: Created 5 platform-agnostic documentation files in `docs/`: overview.md (pipeline & purpose), configuration.md (all parameters with types/defaults), architecture.md (component design with mermaid diagrams & strategy pattern), algorithms.md (detailed algorithms with math formulas for center-crop, grid calculation, tile placement, color fill), glossary.md (key terms). Flickr integration excluded as requested. Committed as 311cf71.