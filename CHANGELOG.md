# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Template support for project and item templates with auto-discovery in `ProjectTemplates/` and `ItemTemplates/` folders
- Auto-injection of `<Content>` entries into vsixmanifest for discovered templates (`AutoInjectVsixTemplateContent` property)
- Cross-project template references via `VsixTemplateReference` item type
- Auto-inclusion of `.imagemanifest` files as `ImageManifest` items
- Auto-inclusion of `ContentManifest.json` files as `Content` items
- NuGet Central Package Management (CPM) support
- Build validation warnings for missing manifest Content entries (VSIXSDK011-014)

### Changed

- Removed implicit `PackageReference` for `Microsoft.VSSDK.BuildTools` - users must now explicitly add this reference

## [0.3.0] - 2025-12-30

### Added

- String constant generation for top-level VSCT GUIDs
- Implicit Microsoft.VSSDK.BuildTools package reference

### Changed

- Removed publish manifest generation (refactored for simplicity)

## [0.2.0] - 2025-12-30

### Added

- Publish manifest generation and source generators
- Analyzer reference for source generators

### Fixed

- Release workflow updated for moved solution location

### Changed

- README refreshed with emojis and reorganized sections
- README badges updated to for-the-badge style

[Unreleased]: https://github.com/CodingWithCalvin/VsixSdk/compare/v0.3.0...HEAD
[0.3.0]: https://github.com/CodingWithCalvin/VsixSdk/compare/v0.2.0...v0.3.0
[0.2.0]: https://github.com/CodingWithCalvin/VsixSdk/releases/tag/v0.2.0
