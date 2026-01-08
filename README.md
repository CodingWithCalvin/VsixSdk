# CodingWithCalvin.VsixSdk

[![NuGet](https://img.shields.io/nuget/v/CodingWithCalvin.VsixSdk?style=for-the-badge)](https://www.nuget.org/packages/CodingWithCalvin.VsixSdk)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow?style=for-the-badge)](https://opensource.org/licenses/MIT)

An MSBuild SDK that brings modern SDK-style `.csproj` files to Visual Studio extension development. No more XML soup! 🎉

## 💡 Why This Exists

Visual Studio extension projects are stuck in the past. They still use the old, verbose project format while the rest of .NET has moved on to clean SDK-style projects.

**This SDK fixes that.**

✨ Write clean `.csproj` files
✨ Get source generators for compile-time constants
✨ Ship fully functional VSIX packages

## 🚀 Getting Started

### 📦 Using the Template (Recommended)

The easiest way to create a new VSIX project is with the dotnet template:

```bash
# Install the template
dotnet new install CodingWithCalvin.VsixSdk.Templates

# Create a new extension
dotnet new vsix -n MyExtension --publisher "Your Name" --description "My awesome extension"

# Build it
cd MyExtension
dotnet build
```

#### Template Parameters

| Parameter | Short | Description | Default |
|-----------|-------|-------------|---------|
| `--extensionName` | `-e` | Display name in VS extension manager | Project name |
| `--publisher` | `-p` | Publisher name in VSIX manifest | MyPublisher |
| `--description` | `-de` | Extension description | A Visual Studio extension |
| `--tags` | `-ta` | Comma-separated tags for discoverability | extension |

**Examples:**

```bash
# Basic - uses project name as display name
dotnet new vsix -n MyExtension

# With custom extension name (different from project name)
dotnet new vsix -n MyExtension.Vsix --extensionName "My Cool Extension"

# With all parameters
dotnet new vsix -n MyExtension \
  --extensionName "My Cool Extension" \
  --publisher "Acme Corp" \
  --description "Adds productivity features to Visual Studio" \
  --tags "productivity, tools, editor"
```

### 🔄 Migrating from Legacy Projects

Have an existing non-SDK style VSIX project? Here's how to modernize it:

#### Step 1: Back Up Your Project

Before making changes, ensure your project is committed to source control or backed up.

#### Step 2: Replace the .csproj Content

Replace your entire `.csproj` file with the SDK-style format:

**Before (Legacy):**
```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('...')" />
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <ProjectGuid>{YOUR-GUID}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>MyExtension</RootNamespace>
    <AssemblyName>MyExtension</AssemblyName>
    <TargetFrameworkVersion>v4.7.2</TargetFrameworkVersion>
    <!-- ... many more lines ... -->
  </PropertyGroup>
  <!-- ... hundreds of lines of XML ... -->
  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
  <Import Project="$(VSToolsPath)\VSSDK\Microsoft.VsSDK.targets" Condition="'$(VSToolsPath)' != ''" />
</Project>
```

**After (SDK-style):**
```xml
<Project Sdk="CodingWithCalvin.VsixSdk/0.3.0">
  <PropertyGroup>
    <TargetFramework>net472</TargetFramework>
    <RootNamespace>MyExtension</RootNamespace>
    <AssemblyName>MyExtension</AssemblyName>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.VisualStudio.SDK" Version="17.*" />
    <PackageReference Include="Microsoft.VSSDK.BuildTools" Version="17.*" PrivateAssets="all" />
  </ItemGroup>
</Project>
```

#### Step 3: Update the VSIX Manifest for VS 2022+

Add `<ProductArchitecture>amd64</ProductArchitecture>` to each `InstallationTarget`:

```xml
<Installation>
  <InstallationTarget Id="Microsoft.VisualStudio.Community" Version="[17.0, 19.0)">
    <ProductArchitecture>amd64</ProductArchitecture>
  </InstallationTarget>
  <!-- Repeat for Professional, Enterprise if needed -->
</Installation>
```

#### Step 4: Remove Unnecessary Files

Delete these files if they exist (the SDK handles them automatically):
- 📄 `packages.config` - Use `PackageReference` instead
- 📄 `Properties/AssemblyInfo.cs` - SDK generates this automatically
- 📄 `app.config` - Usually not needed

#### Step 5: Update Package References

Convert from `packages.config` to `PackageReference` format in your `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.VisualStudio.SDK" Version="17.*" />
  <PackageReference Include="Microsoft.VSSDK.BuildTools" Version="17.*" PrivateAssets="all" />
  <!-- Add other packages your extension uses -->
</ItemGroup>
```

#### Step 6: Handle VSCT Files

If you have `.vsct` files, they're automatically included. Remove any explicit `<VSCTCompile>` items unless you need custom metadata.

#### Step 7: Build and Test

```bash
dotnet build
```

Fix any errors that arise. Common issues:
- ❌ **Missing types**: Add the appropriate `PackageReference`
- ❌ **Duplicate files**: Remove explicit includes that conflict with auto-inclusion
- ❌ **Resource files**: Ensure `VSPackage.resx` files are in the project

#### ✅ Migration Checklist

- [ ] Replaced `.csproj` with SDK-style format
- [ ] Added `<ProductArchitecture>amd64</ProductArchitecture>` to manifest
- [ ] Converted `packages.config` to `PackageReference`
- [ ] Removed `Properties/AssemblyInfo.cs`
- [ ] Removed explicit file includes (now auto-included)
- [ ] Updated version range to `[17.0, 19.0)` for VS 2022+
- [ ] Build succeeds with `dotnet build`
- [ ] F5 debugging works in Visual Studio

### 🛠️ Manual Setup

If you prefer to set up manually, create a `.csproj` file:

```xml
<Project Sdk="CodingWithCalvin.VsixSdk/0.3.0">
  <PropertyGroup>
    <TargetFramework>net472</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.VisualStudio.SDK" Version="17.*" />
    <PackageReference Include="Microsoft.VSSDK.BuildTools" Version="17.*" PrivateAssets="all" />
  </ItemGroup>
</Project>
```

Then create `source.extension.vsixmanifest`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<PackageManifest Version="2.0.0" xmlns="http://schemas.microsoft.com/developer/vsx-schema/2011" xmlns:d="http://schemas.microsoft.com/developer/vsx-schema-design/2011">
  <Metadata>
    <Identity Id="MyExtension.YOUR-GUID-HERE" Version="1.0.0" Language="en-US" Publisher="Your Name" />
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

## ✨ Features

### 🔮 Source Generators

The SDK includes source generators that create compile-time constants from your manifest files.

#### VsixInfo - VSIX Manifest Constants

A `VsixInfo` class is automatically generated from your `source.extension.vsixmanifest`:

```csharp
// Auto-generated from your manifest
public static class VsixInfo
{
    public const string Id = "MyExtension.a1b2c3d4-...";
    public const string Version = "1.0.0";
    public const string Publisher = "Your Name";
    public const string DisplayName = "My Extension";
    public const string Description = "Description of your extension";
    public const string Tags = "your, tags, here";
    // ... and more
}
```

Use it in your code:

```csharp
// Display version in your extension
MessageBox.Show($"Version: {VsixInfo.Version}");

// Use in attributes
[Guid(VsixInfo.Id)]
public sealed class MyPackage : AsyncPackage { }
```

#### VSCT GUIDs and IDs

If you have `.vsct` files, constants are generated for your GUIDs and command IDs:

```csharp
// Auto-generated from MyCommands.vsct
public static class MyCommandsVsct
{
    public static readonly Guid guidMyPackage = new Guid("...");

    public static class guidMyCommandSet
    {
        public const string GuidString = "...";
        public static readonly Guid Guid = new Guid(GuidString);
        public const int MyCommandId = 0x0100;
        public const int MyMenuGroup = 0x1020;
    }
}
```

#### 📁 Generated Files Location

Generated source files are written to the `Generated/` folder in your project and are visible in Solution Explorer. They're marked as auto-generated so you know not to edit them directly.

### 🏷️ Version Override

Update the VSIX version at build time without manually editing the manifest:

```bash
dotnet build -p:SetVsixVersion=2.0.0
```

This updates the `source.extension.vsixmanifest` file with the new version, rebuilds with the correct version in all outputs (including the generated `VsixInfo.Version` constant), and produces the VSIX with the specified version.

Perfect for CI/CD pipelines:

```yaml
# GitHub Actions example
- name: Build Release
  run: dotnet build -c Release -p:SetVsixVersion=${{ github.ref_name }}
```

### 📂 Auto-Inclusion

The SDK automatically includes common VSIX files:

- 📄 `*.vsct` files as `VSCTCompile` items
- 📄 `VSPackage.resx` files with proper metadata
- 📄 `source.extension.vsixmanifest` as an `AdditionalFile` for source generators
- 📄 `*.imagemanifest` files as `ImageManifest` items (for VS Image Service)
- 📄 `ContentManifest.json` files as `Content` items (for VS content registration)

### 🐛 F5 Debugging

Press F5 to launch the Visual Studio Experimental Instance with your extension loaded. This works automatically when:
- Building in Debug configuration
- Building inside Visual Studio (not `dotnet build`)

### 🚀 Smart Deployment

Extensions are only deployed to the Experimental Instance when building inside Visual Studio. This prevents errors when building from the command line.

### 📦 VS Project and Item Templates

The SDK provides comprehensive support for including Visual Studio project and item templates in your VSIX package.

#### Auto-Discovery

Place templates in `ProjectTemplates/` or `ItemTemplates/` folders and they'll be automatically discovered and packaged:

```
MyExtension/
├── ProjectTemplates/
│   └── MyProjectTemplate/
│       ├── MyProject.csproj
│       ├── Class1.cs
│       └── MyTemplate.vstemplate
├── ItemTemplates/
│   └── MyItemTemplate/
│       ├── MyItem.cs
│       └── MyItem.vstemplate
└── source.extension.vsixmanifest
```

The SDK automatically:
1. Discovers `.vstemplate` files in these folders
2. Injects the required `<Content>` entries into the manifest (via intermediate file)
3. Includes all template files in the VSIX

#### Cross-Project Template References

Reference templates from other SDK-style projects using `VsixTemplateReference`:

```xml
<ItemGroup>
  <VsixTemplateReference Include="..\MyTemplateProject\MyTemplateProject.csproj"
                         TemplateType="Project"
                         TemplatePath="Templates\MyProjectTemplate" />
</ItemGroup>
```

This is useful when the VSIX Manifest Designer cannot enumerate SDK-style projects.

#### Disabling Auto-Injection

If you prefer to manage manifest Content entries manually:

```xml
<PropertyGroup>
  <AutoInjectVsixTemplateContent>false</AutoInjectVsixTemplateContent>
</PropertyGroup>
```

When disabled, build warnings (VSIXSDK011, VSIXSDK012) will alert you if templates are discovered but the manifest lacks the corresponding `<Content>` entries.

## ⚙️ Configuration

### Properties

| Property | Default | Description |
|----------|---------|-------------|
| `TargetFramework` | `net472` | Target framework (must be .NET Framework 4.7.2+) |
| `Platform` | `AnyCPU` | Target platform |
| `GeneratePkgDefFile` | `true` | Generate .pkgdef registration file |
| `DeployExtension` | `true`* | Deploy to experimental instance |
| `EnableDefaultVsixItems` | `true` | Auto-include VSIX-related files |
| `EnableDefaultVsixTemplateItems` | `true` | Auto-discover templates in ProjectTemplates/ItemTemplates folders |
| `AutoInjectVsixTemplateContent` | `true` | Auto-inject Content entries into manifest for discovered templates |
| `VsixProjectTemplatesFolder` | `ProjectTemplates` | Folder for project templates |
| `VsixItemTemplatesFolder` | `ItemTemplates` | Folder for item templates |
| `EmitCompilerGeneratedFiles` | `true` | Write generated source files to disk |
| `CompilerGeneratedFilesOutputPath` | `Generated/` | Location for generated source files |

> \* Only when `Configuration=Debug` AND building inside Visual Studio

### Disabling Auto-Inclusion

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
  <EnableDefaultVSPackageResourceItems>false</EnableDefaultVSPackageResourceItems>
  <EnableDefaultImageManifestItems>false</EnableDefaultImageManifestItems>
  <EnableDefaultContentManifestItems>false</EnableDefaultContentManifestItems>
  <EnableDefaultVsixTemplateItems>false</EnableDefaultVsixTemplateItems>
</PropertyGroup>
```

### Custom Debugging Arguments

```xml
<PropertyGroup>
  <VsixDebuggingArguments>/rootSuffix Exp /log</VsixDebuggingArguments>
</PropertyGroup>
```

## 📋 Requirements

- Visual Studio 2022 or later
- .NET Framework 4.7.2+ target framework

### NuGet Central Package Management (CPM)

The SDK is fully compatible with [NuGet Central Package Management](https://learn.microsoft.com/nuget/consume-packages/central-package-management). When using CPM, define your package versions in `Directory.Packages.props`:

```xml
<!-- Directory.Packages.props -->
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <PackageVersion Include="Microsoft.VisualStudio.SDK" Version="17.*" />
    <PackageVersion Include="Microsoft.VSSDK.BuildTools" Version="17.*" />
  </ItemGroup>
</Project>
```

Then reference packages without versions in your project:

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.VisualStudio.SDK" />
  <PackageReference Include="Microsoft.VSSDK.BuildTools" PrivateAssets="all" />
</ItemGroup>
```

## 🏗️ Building from Source

```bash
# Clone the repository
git clone https://github.com/CodingWithCalvin/VsixSdk.git
cd VsixSdk

# Build the SDK package
dotnet build src/CodingWithCalvin.VsixSdk/CodingWithCalvin.VsixSdk.csproj -c Release

# Build the template package
dotnet pack src/CodingWithCalvin.VsixSdk.Templates/CodingWithCalvin.VsixSdk.Templates.csproj -c Release

# Packages output to artifacts/packages/
```

## 👥 Contributors

<!-- readme: contributors -start -->
[![CalvinAllen](https://avatars.githubusercontent.com/u/41448698?v=4&s=64)](https://github.com/CalvinAllen) [![rahul7720](https://avatars.githubusercontent.com/u/4676251?v=4&s=64)](https://github.com/rahul7720) 
<!-- readme: contributors -end -->

## 📄 License

MIT License - see [LICENSE](LICENSE) for details.

---

Made with ❤️ by [Coding With Calvin](https://codingwithcalvin.net)
