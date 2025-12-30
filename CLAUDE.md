# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Critical Rules

**These rules override all other instructions:**

1. **NEVER commit directly to main** - Always create a feature branch and submit a pull request
2. **Conventional commits** - Format: `type(scope): description`
3. **GitHub Issues for TODOs** - Use `gh` CLI to manage issues, no local TODO files. Use conventional commit format for issue titles
4. **Pull Request titles** - Use conventional commit format (same as commits)
5. **Branch naming** - Use format: `type/scope/short-description` (e.g., `feat/sdk/version-sync`)
6. **Working an issue** - Always create a new branch from an updated main branch
7. **Check branch status before pushing** - Verify the remote tracking branch still exists. If a PR was merged/deleted, create a new branch from main instead

---

### GitHub CLI Commands

```bash
gh issue list                    # List open issues
gh issue view <number>           # View details
gh issue create --title "type(scope): description" --body "..."
gh issue close <number>
```

### Conventional Commit Types

| Type | Description |
|------|-------------|
| `feat` | New feature |
| `fix` | Bug fix |
| `docs` | Documentation only |
| `refactor` | Code change that neither fixes a bug nor adds a feature |
| `test` | Adding or updating tests |
| `chore` | Maintenance tasks |

---

## Project Overview

CodingWithCalvin.VsixSdk is an MSBuild SDK that enables SDK-style `.csproj` files for Visual Studio extension (VSIX) development. It wraps `Microsoft.NET.Sdk` and `Microsoft.VSSDK.BuildTools` to provide a modern project format for VS 2022+ extensions.

## Build Commands

```bash
# Build the SDK package
dotnet build src/CodingWithCalvin.VsixSdk/CodingWithCalvin.VsixSdk.csproj -c Release

# Build the template package
dotnet pack src/CodingWithCalvin.VsixSdk.Templates/CodingWithCalvin.VsixSdk.Templates.csproj -c Release

# Build the sample extension (tests the SDK locally)
dotnet build samples/SampleExtension/SampleExtension.csproj

# Packages output to artifacts/packages/
```

## Testing Templates Locally

```bash
# Install template from local package
dotnet new install artifacts/packages/CodingWithCalvin.VsixSdk.Templates.1.0.0.nupkg

# Test creating a new project
dotnet new vsix -n TestExtension --publisher "Test"

# Uninstall when done
dotnet new uninstall CodingWithCalvin.VsixSdk.Templates
```

## Architecture

### SDK Structure (`src/CodingWithCalvin.VsixSdk/Sdk/`)

The SDK follows MSBuild SDK conventions with props/targets pairs:

- **Sdk.props** → Imports `Microsoft.NET.Sdk` props, then `Sdk.Vsix.props`
- **Sdk.targets** → Imports `Microsoft.NET.Sdk` targets, then `Sdk.Vsix.targets`
- **Sdk.Vsix.props** → VSIX-specific properties (defaults, F5 debugging, deployment settings)
- **Sdk.Vsix.targets** → VSIX-specific targets (auto-include items, validation, VSSDK import)

The modular design allows `Sdk.Vsix.props`/`Sdk.Vsix.targets` to be imported standalone for local development (see `samples/Directory.Build.props`).

### Local Development Pattern

Sample projects use `Sdk="Microsoft.NET.Sdk"` directly, with `samples/Directory.Build.props` and `samples/Directory.Build.targets` importing the VSIX-specific files from source. This allows testing SDK changes without rebuilding the NuGet package.

### Template Structure (`src/CodingWithCalvin.VsixSdk.Templates/`)

Uses `dotnet new` template engine with:
- `template.json` defining parameters (`publisher`, `description`) and generated GUIDs
- `sourceName: "VsixExtension"` for automatic name substitution
- Template placeholders: `TEMPLATE_PUBLISHER`, `TEMPLATE_DESCRIPTION`, `TEMPLATE_GUID1`, `TEMPLATE_GUID2`

## Key MSBuild Properties

| Property | Purpose |
|----------|---------|
| `DeployExtension` | Only `true` when `BuildingInsideVisualStudio=true` (prevents `dotnet build` errors) |
| `EnableDefaultVsixItems` | Auto-includes `.vsixmanifest`, `.vsct`, `VSPackage.resx` files |
| `VsixVersion` | Derived from `$(Version)` for manifest sync |
| `GeneratePkgDefFile` | Must be `true` for VS registration |

## VSIX Manifest Requirements

VS 2022+ requires `<ProductArchitecture>amd64</ProductArchitecture>` in each `InstallationTarget`. Version ranges should be `[17.0, 19.0)` to support VS 2022-2026.
