# 🚀 CodingWithCalvin.VsixSdk

[![NuGet](https://img.shields.io/nuget/v/CodingWithCalvin.VsixSdk.svg)](https://www.nuget.org/packages/CodingWithCalvin.VsixSdk)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

> ✨ **Finally!** Modern SDK-style projects for Visual Studio extensions!

An MSBuild SDK that brings the clean, modern `.csproj` format to VSIX development. No more XML soup! 🎉

---

## 🤔 Why This Exists

Visual Studio extension projects are stuck in 2010. They still use the old, verbose project format while the rest of .NET has moved on to beautiful SDK-style projects.

**This SDK fixes that.**

Write clean `.csproj` files. Get all the modern tooling. Ship fully functional VSIX packages. 💪

---

## 📦 Installation

```
dotnet add package CodingWithCalvin.VsixSdk
```

Or reference it directly in your project file (recommended):

```xml
<Project Sdk="CodingWithCalvin.VsixSdk/1.0.0">
```

---

## ⚡ Quick Start

### 1️⃣ Create the Project File

```xml
<Project Sdk="CodingWithCalvin.VsixSdk/1.0.0">

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

That's it. Seriously. 😎

### 2️⃣ Create the VSIX Manifest

Create `source.extension.vsixmanifest`:

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

> 💡 **Pro tip:** The `Version="|%CurrentProject%;GetVsixVersion|"` syntax automatically syncs with your project's `Version` property!

### 3️⃣ Create Your Package Class

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

            // Your initialization code here 🎨
        }
    }
}
```

### 4️⃣ Build and Debug

| Action | Command |
|--------|---------|
| 🔨 Build | `dotnet build` or build in Visual Studio |
| 🐛 Debug | Press **F5** → launches the Experimental Instance |

---

## ✅ Features

| Feature | Description |
|---------|-------------|
| 📝 **SDK-style projects** | Clean, minimal `.csproj` files |
| 🐛 **F5 debugging** | Works out of the box with VS Experimental Instance |
| 📁 **Auto-inclusion** | VSCT, VSIX manifests, and VSPackage resources included automatically |
| 🔄 **Version sync** | VSIX version derived from project `Version` property |
| ⚙️ **Sensible defaults** | Correct settings for VS 2022+ (x64, .NET Framework 4.7.2+) |
| 🚀 **Smart deployment** | Only deploys to Experimental Instance when building in VS |

---

## 📋 Requirements

- 🖥️ Visual Studio 2022 or later
- 🎯 .NET Framework 4.7.2+ target framework

---

## 🔧 Configuration

### Properties

| Property | Default | Description |
|----------|---------|-------------|
| `TargetFramework` | `net472` | Target framework (must be .NET Framework 4.7.2+) |
| `Platform` | `x64` | Target platform (VS 2022+ is 64-bit) |
| `UseCodebase` | `true` | Use codebase for assembly loading |
| `GeneratePkgDefFile` | `true` | Generate .pkgdef registration file |
| `VsixVersion` | `$(Version)` | VSIX manifest version |
| `DeployExtension` | `true`* | Deploy to experimental instance |
| `EnableDefaultVsixItems` | `true` | Auto-include VSIX-related files |
| `EnableDefaultVsixDebugging` | `true` | Configure F5 debugging |

> \* Only when `Configuration=Debug` AND building inside Visual Studio

### 🎛️ Disabling Auto-Inclusion

Take full control over which files are included:

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

### 🐛 Custom Debugging Arguments

```xml
<PropertyGroup>
  <VsixDebuggingArguments>/rootSuffix Exp /log</VsixDebuggingArguments>
</PropertyGroup>
```

---

## 🔄 Migration from Legacy Projects

Migrating from the old project format? Here's how:

1. 📝 Replace your old `.csproj` content with the SDK-style format above
2. 🗑️ Remove unnecessary `<Import>` statements — the SDK handles them
3. 📁 Keep your `source.extension.vsixmanifest`, `.vsct`, and resource files
4. ➕ Add `<ProductArchitecture>amd64</ProductArchitecture>` to your manifest for VS 2022+
5. 🔨 Build and test!

---

## 🏗️ Building from Source

```bash
# Clone the repository
git clone https://github.com/CodingWithCalvin/VsixSdk.git
cd VsixSdk

# Build the SDK package
dotnet build src/CodingWithCalvin.VsixSdk/CodingWithCalvin.VsixSdk.csproj -c Release

# 📦 Package outputs to artifacts/packages/
```

---

## 📄 License

MIT License - see [LICENSE](LICENSE) for details.

---

Made with ❤️ by [Coding With Calvin](https://codingwithcalvin.net)
