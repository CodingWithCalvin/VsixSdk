# VsixCommunity.Sdk

An MSBuild SDK for building Visual Studio extensions (VSIX) using modern SDK-style projects.

## Why?

Visual Studio extension projects still use the legacy (non-SDK) project format. This SDK enables you to use the clean, modern SDK-style `.csproj` format while still producing fully functional VSIX packages.

## Installation

Install the SDK from NuGet:

```
dotnet add package VsixCommunity.Sdk
```

Or reference it directly in your project file using the `Sdk` attribute (see Quick Start below).

## Quick Start

### 1. Create the project file

Create a new `.csproj` file with the SDK reference:

```xml
<Project Sdk="VsixCommunity.Sdk/1.0.0">

  <PropertyGroup>
    <TargetFramework>net472</TargetFramework>
    <Version>1.0.0</Version>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.VisualStudio.SDK" Version="17.*" />
    <PackageReference Include="Microsoft.VSSDK.BuildTools" Version="17.*" PrivateAssets="all" />
  </ItemGroup>

</Project>
```

### 2. Create the VSIX manifest

Create a `source.extension.vsixmanifest` file:

```xml
<?xml version="1.0" encoding="utf-8"?>
<PackageManifest Version="2.0.0" xmlns="http://schemas.microsoft.com/developer/vsx-schema/2011" xmlns:d="http://schemas.microsoft.com/developer/vsx-schema-design/2011">
  <Metadata>
    <Identity Id="MyExtension.YOUR-GUID-HERE" Version="|%CurrentProject%;GetVsixVersion|" Language="en-US" Publisher="Your Name" />
    <DisplayName>My Extension</DisplayName>
    <Description>Description of your extension</Description>
    <Tags>your, tags, here</Tags>
  </Metadata>
  <Installation>
    <InstallationTarget Id="Microsoft.VisualStudio.Community" Version="[17.0, 19.0)">
      <ProductArchitecture>amd64</ProductArchitecture>
    </InstallationTarget>
  </Installation>
  <Dependencies>
    <Dependency Id="Microsoft.Framework.NDP" DisplayName="Microsoft .NET Framework" d:Source="Manual" Version="[4.7.2,)" />
  </Dependencies>
  <Prerequisites>
    <Prerequisite Id="Microsoft.VisualStudio.Component.CoreEditor" Version="[17.0,19.0)" DisplayName="Visual Studio core editor" />
  </Prerequisites>
  <Assets>
    <Asset Type="Microsoft.VisualStudio.VsPackage" d:Source="Project" d:ProjectName="%CurrentProject%" Path="|%CurrentProject%;PkgDefProjectOutputGroup|" />
  </Assets>
</PackageManifest>
```

> **Note:** The `Version="|%CurrentProject%;GetVsixVersion|"` syntax automatically syncs the VSIX version with your project's `Version` property.

### 3. Create your package class

Create a C# file with your package implementation:

```csharp
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace MyExtension
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid("YOUR-GUID-HERE")]
    public sealed class MyExtensionPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // Your initialization code here
        }
    }
}
```

### 4. Build and debug

- **Build:** `dotnet build` or build in Visual Studio
- **Debug:** Press F5 in Visual Studio to launch the Experimental Instance

## Features

- **SDK-style projects** - Clean, minimal `.csproj` files
- **F5 debugging** - Works out of the box with the VS Experimental Instance
- **Auto-inclusion** - VSCT, VSIX manifests, and VSPackage resources are included automatically
- **Version sync** - VSIX version derived from the project `Version` property
- **Sensible defaults** - Correct settings for VS 2022+ (x64, .NET Framework 4.7.2+)
- **Smart deployment** - Only deploys to Experimental Instance when building in Visual Studio

## Requirements

- Visual Studio 2022 or later
- .NET Framework 4.7.2 or later target framework

## Configuration

### Properties

| Property | Default | Description |
|----------|---------|-------------|
| `TargetFramework` | `net472` | Target framework (must be .NET Framework 4.7.2+) |
| `Platform` | `x64` | Target platform (VS 2022+ is 64-bit) |
| `UseCodebase` | `true` | Use codebase for assembly loading |
| `GeneratePkgDefFile` | `true` | Generate .pkgdef registration file |
| `VsixVersion` | `$(Version)` | VSIX manifest version |
| `DeployExtension` | `true` (Debug in VS only) | Deploy to experimental instance |
| `EnableDefaultVsixItems` | `true` | Auto-include VSIX-related files |
| `EnableDefaultVsixDebugging` | `true` | Configure F5 debugging |

### Disabling Auto-Inclusion

To manually control which files are included:

```xml
<PropertyGroup>
  <EnableDefaultVsixItems>false</EnableDefaultVsixItems>
</PropertyGroup>
```

Or disable specific categories:

```xml
<PropertyGroup>
  <EnableDefaultVsctItems>false</EnableDefaultVsctItems>
  <EnableDefaultVsixManifestItems>false</EnableDefaultVsixManifestItems>
  <EnableDefaultVSPackageResourceItems>false</EnableDefaultVSPackageResourceItems>
</PropertyGroup>
```

### Custom Debugging Arguments

```xml
<PropertyGroup>
  <VsixDebuggingArguments>/rootSuffix Exp /log</VsixDebuggingArguments>
</PropertyGroup>
```

## Migration from Legacy Projects

1. Replace your old `.csproj` content with the SDK-style format shown above
2. Remove unnecessary `<Import>` statements - they're handled by the SDK
3. Keep your `source.extension.vsixmanifest`, `.vsct`, and resource files
4. Update the VSIX manifest to include `<ProductArchitecture>amd64</ProductArchitecture>` for VS 2022+
5. Build and test

## Building from Source

```bash
# Clone the repository
git clone https://github.com/VsixCommunity/VsixCommunity.Sdk.git
cd VsixCommunity.Sdk

# Build the SDK package
dotnet build src/VsixCommunity.Sdk/VsixCommunity.Sdk.csproj -c Release

# The package will be in artifacts/packages/
```

## Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## License

MIT License - see [LICENSE](LICENSE) for details.
