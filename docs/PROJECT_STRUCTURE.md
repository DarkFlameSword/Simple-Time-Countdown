# Project Structure

This repository follows a simple GitHub-friendly layout:

- `src/SimpleTimeCountdown.App/`
  - WPF desktop application source code and app assets
- `src/SimpleTimeCountdown.Setup/`
  - Windows installer source code
- `scripts/`
  - Local packaging and asset-generation scripts
- `packaging/msix/`
  - MSIX manifest and visual assets
- `docs/`
  - Maintainer-facing documentation
- `.github/workflows/`
  - CI build pipeline
- `artifacts/`
  - Generated outputs only

Generated content such as `bin/`, `obj/`, `artifacts/publish/`, `artifacts/packages/`, and installer payload zips should stay out of version control.
