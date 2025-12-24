# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

VsixCommunity.Sdk is an MSBuild SDK that enables SDK-style `.csproj` files for Visual Studio extension (VSIX) development. It wraps `Microsoft.NET.Sdk` and `Microsoft.VSSDK.BuildTools` to provide a modern project format for VS 2022+ extensions.

## Build Commands

```bash
# Build the SDK package
dotnet build src/VsixCommunity.Sdk/VsixCommunity.Sdk.csproj -c Release

# Build the template package
dotnet pack src/VsixCommunity.Sdk.Templates/VsixCommunity.Sdk.Templates.csproj -c Release

# Build the sample extension (tests the SDK locally)
dotnet build samples/SampleExtension/SampleExtension.csproj

# Packages output to artifacts/packages/
```

## Testing Templates Locally

```bash
# Install template from local package
dotnet new install artifacts/packages/VsixCommunity.Sdk.Templates.1.0.0.nupkg

# Test creating a new project
dotnet new vsix -n TestExtension --publisher "Test"

# Uninstall when done
dotnet new uninstall VsixCommunity.Sdk.Templates
```

## Architecture

### SDK Structure (`src/VsixCommunity.Sdk/Sdk/`)

The SDK follows MSBuild SDK conventions with props/targets pairs:

- **Sdk.props** → Imports `Microsoft.NET.Sdk` props, then `Sdk.Vsix.props`
- **Sdk.targets** → Imports `Microsoft.NET.Sdk` targets, then `Sdk.Vsix.targets`
- **Sdk.Vsix.props** → VSIX-specific properties (defaults, F5 debugging, deployment settings)
- **Sdk.Vsix.targets** → VSIX-specific targets (auto-include items, validation, VSSDK import)

The modular design allows `Sdk.Vsix.props`/`Sdk.Vsix.targets` to be imported standalone for local development (see `samples/Directory.Build.props`).

### Local Development Pattern

Sample projects use `Sdk="Microsoft.NET.Sdk"` directly, with `samples/Directory.Build.props` and `samples/Directory.Build.targets` importing the VSIX-specific files from source. This allows testing SDK changes without rebuilding the NuGet package.

### Template Structure (`src/VsixCommunity.Sdk.Templates/`)

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
