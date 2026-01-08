# Template Support

This document explains how to include Visual Studio project templates and item templates in your VSIX extension using CodingWithCalvin.VsixSdk.

## Background

The built-in VSIX Manifest Designer in Visual Studio cannot enumerate SDK-style projects when adding template assets. This is because the designer uses legacy DTE extenders that are registered for the old project system, not the Common Project System (CPS) used by SDK-style projects.

This SDK provides MSBuild-based template support that works around these limitations by:
- Auto-discovering templates in standard folders
- Supporting cross-project template references
- Providing validation warnings for missing manifest Content entries

## How It Works

This SDK handles template packaging by:

1. **Auto-discovering** templates in `ProjectTemplates/` and `ItemTemplates/` folders
2. **Including template files** in the VSIX package automatically
3. **Auto-injecting manifest Content entries** so you don't need to manually edit the manifest
4. **Supporting cross-project template references** for including templates from other SDK-style projects

Visual Studio requires `<Content><ProjectTemplate/></Content>` entries in the manifest to register and display templates. The SDK automatically injects these entries when templates are discovered, so you don't need to add them manually.

## Item Types

The SDK provides the following item types for templates:

| Item Type | Description |
|-----------|-------------|
| `VsixProjectTemplate` | (Auto-discovered) A folder containing a `.vstemplate` file for a project template |
| `VsixItemTemplate` | (Auto-discovered) A folder containing a `.vstemplate` file for an item template |
| `VsixTemplateReference` | Reference a template folder from another project |

## Auto-Discovery

By default, the SDK automatically discovers templates in your project:

- **Project templates**: Any subfolder in `ProjectTemplates/` containing a `.vstemplate` file
- **Item templates**: Any subfolder in `ItemTemplates/` containing a `.vstemplate` file

### Example Project Structure

```
MyExtension/
  MyExtension.csproj
  source.extension.vsixmanifest
  ProjectTemplates/
    MyProjectTemplate/
      MyProjectTemplate.vstemplate
      MyProject.csproj
      Class1.cs
  ItemTemplates/
    MyItemTemplate/
      MyItemTemplate.vstemplate
      MyClass.cs
```

With this structure, no additional configuration is needed. The SDK will:
1. Find the templates automatically
2. Include all template files in the VSIX
3. Inject the required `<Content>` entries into the manifest automatically

### Disabling Auto-Discovery

To disable automatic template discovery:

```xml
<PropertyGroup>
  <EnableDefaultVsixTemplateItems>false</EnableDefaultVsixTemplateItems>
</PropertyGroup>
```

### Changing Default Folders

To use different folder names:

```xml
<PropertyGroup>
  <VsixProjectTemplatesFolder>Templates\Projects</VsixProjectTemplatesFolder>
  <VsixItemTemplatesFolder>Templates\Items</VsixItemTemplatesFolder>
</PropertyGroup>
```

## Cross-Project Template References

When you have templates in a separate SDK-style project that the VSIX Manifest Designer cannot enumerate, use `VsixTemplateReference`:

```xml
<ItemGroup>
  <VsixTemplateReference Include="..\MyTemplateProject\MyTemplateProject.csproj"
                         TemplateType="Project"
                         TemplatePath="Templates\MyProjectTemplate" />
</ItemGroup>
```

The SDK will:
1. Copy the template folder from the referenced project to your local `ProjectTemplates/` or `ItemTemplates/` folder
2. Include the copied template files in the VSIX

The `TemplatePath` is relative to the referenced project's directory.

## Manifest Configuration

Visual Studio requires `<Content>` entries in your `.vsixmanifest` to register templates. **The SDK automatically injects these entries** when templates are discovered, so you typically don't need to add them manually.

### Automatic Content Injection (Default)

When `AutoInjectVsixTemplateContent` is enabled (the default), the SDK:
1. Creates an intermediate manifest in the `obj` folder
2. Injects `<ProjectTemplate Path="ProjectTemplates"/>` if project templates are discovered
3. Injects `<ItemTemplate Path="ItemTemplates"/>` if item templates are discovered
4. Uses the intermediate manifest for VSIX packaging (your source manifest is never modified)

This means your source `.vsixmanifest` does not need a `<Content>` section at all when using templates.

### Disabling Auto-Injection

If you prefer to manage the manifest Content entries manually, disable auto-injection:

```xml
<PropertyGroup>
  <AutoInjectVsixTemplateContent>false</AutoInjectVsixTemplateContent>
</PropertyGroup>
```

When auto-injection is disabled, you must add the Content entries to your manifest manually:

```xml
<Content>
  <ProjectTemplate Path="ProjectTemplates" />
  <ItemTemplate Path="ItemTemplates" />
</Content>
```

The SDK will emit warnings if you have templates but missing manifest entries:
- **VSIXSDK011**: Project templates defined but no `<ProjectTemplate>` in manifest
- **VSIXSDK012**: Item templates defined but no `<ItemTemplate>` in manifest

These warnings only appear when `AutoInjectVsixTemplateContent` is set to `false`.

## Complete Example

### Project File

```xml
<Project Sdk="CodingWithCalvin.VsixSdk/1.0.0">

  <PropertyGroup>
    <Version>1.0.0</Version>
  </PropertyGroup>

  <!-- Templates in ProjectTemplates/ and ItemTemplates/ are auto-discovered -->

  <!-- Include templates from another SDK-style project -->
  <ItemGroup>
    <VsixTemplateReference Include="..\SharedTemplates\SharedTemplates.csproj"
                           TemplateType="Project"
                           TemplatePath="Templates\SharedProject" />
  </ItemGroup>

</Project>
```

### Manifest File

The SDK automatically injects Content entries, so your manifest doesn't need them:

```xml
<?xml version="1.0" encoding="utf-8"?>
<PackageManifest Version="2.0.0" xmlns="http://schemas.microsoft.com/developer/vsx-schema/2011">
  <Metadata>
    <Identity Id="MyExtension" Version="1.0.0" Language="en-US" Publisher="MyCompany" />
    <DisplayName>My Extension</DisplayName>
    <Description>Extension with templates</Description>
  </Metadata>
  <Installation>
    <InstallationTarget Id="Microsoft.VisualStudio.Community" Version="[17.0,19.0)">
      <ProductArchitecture>amd64</ProductArchitecture>
    </InstallationTarget>
  </Installation>
  <!-- Content entries are auto-injected by the SDK -->
</PackageManifest>
```

### Template File (.vstemplate)

```xml
<?xml version="1.0" encoding="utf-8"?>
<VSTemplate Version="3.0.0" Type="Project" xmlns="http://schemas.microsoft.com/developer/vstemplate/2005">
  <TemplateData>
    <Name>My Project Template</Name>
    <Description>A sample project template</Description>
    <Icon>__TemplateIcon.ico</Icon>
    <ProjectType>CSharp</ProjectType>
    <DefaultName>MyProject</DefaultName>
    <ProvideDefaultName>true</ProvideDefaultName>
  </TemplateData>
  <TemplateContent>
    <Project File="MyProject.csproj" ReplaceParameters="true">
      <ProjectItem ReplaceParameters="true">Class1.cs</ProjectItem>
    </Project>
  </TemplateContent>
</VSTemplate>
```

## Validation Warnings

| Code | Description |
|------|-------------|
| VSIXSDK011 | Project templates defined but no `<ProjectTemplate>` in manifest |
| VSIXSDK012 | Item templates defined but no `<ItemTemplate>` in manifest |
| VSIXSDK013 | `VsixTemplateReference` item missing `TemplateType` metadata |
| VSIXSDK014 | `VsixTemplateReference` item missing `TemplatePath` metadata |

## Troubleshooting

### Templates not appearing in Visual Studio

1. Check that the template folders are included in the VSIX (open the .vsix as a zip)
2. Verify the intermediate manifest (`obj/*/source.extension.vsixmanifest`) contains the `<Content>` entries
3. Verify the `.vstemplate` file has correct `<ProjectType>` or `<TemplateGroupID>`
4. Reset the Visual Studio template cache: delete `%LocalAppData%\Microsoft\VisualStudio\<version>\ComponentModelCache`
5. If using `AutoInjectVsixTemplateContent=false`, ensure your source manifest has the `<Content>` entries

### Build errors about missing template folders

Ensure the template folder exists and contains a `.vstemplate` file. For `VsixTemplateReference`, verify the `TemplatePath` is correct relative to the referenced project.

### Cross-project templates not working

1. Verify the referenced project path is correct
2. Check that `TemplateType` is set to `Project` or `Item`
3. Ensure `TemplatePath` points to a folder containing a `.vstemplate` file
4. The template folder will be copied to your local `ProjectTemplates/` or `ItemTemplates/` folder during build
