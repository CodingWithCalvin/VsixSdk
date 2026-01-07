# Template Support

This document explains how to include Visual Studio project templates and item templates in your VSIX extension using CodingWithCalvin.VsixSdk.

## Background

The built-in VSIX Manifest Designer in Visual Studio cannot enumerate SDK-style projects when adding template assets. This is because the designer uses legacy DTE extenders that are registered for the old project system, not the Common Project System (CPS) used by SDK-style projects.

Additionally, SDK-style projects don't define the `TemplateProjectOutputGroup` and `ItemTemplateOutputGroup` MSBuild output groups that VSSDK expects for template assets.

This SDK provides MSBuild-based template support that bypasses these limitations entirely.

## Item Types

The SDK provides four item types for including templates:

| Item Type | Description |
|-----------|-------------|
| `VsixProjectTemplate` | A folder containing a `.vstemplate` file for a project template |
| `VsixItemTemplate` | A folder containing a `.vstemplate` file for an item template |
| `VsixTemplateZip` | A pre-built template zip file |
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
2. Zip each template folder during build
3. Include the zips in the VSIX at `ProjectTemplates/` and `ItemTemplates/`

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

## Manual Template Configuration

### Folder-Based Templates

If your templates are in non-standard locations, add them explicitly:

```xml
<ItemGroup>
  <VsixProjectTemplate Include="MyTemplates\ConsoleApp" />
  <VsixItemTemplate Include="MyTemplates\NewClass" />
</ItemGroup>
```

### Pre-Built Template Zips

If you have pre-built template zip files:

```xml
<ItemGroup>
  <VsixTemplateZip Include="Templates\MyTemplate.zip" TemplateType="Project" />
  <VsixTemplateZip Include="Templates\MyItem.zip" TemplateType="Item" />
</ItemGroup>
```

### Template References

To include a template from another project in your solution:

```xml
<ItemGroup>
  <VsixTemplateReference Include="..\MyTemplateProject\MyTemplateProject.csproj"
                         TemplateType="Project"
                         TemplatePath="Templates\MyProjectTemplate" />
</ItemGroup>
```

The `TemplatePath` is relative to the referenced project's directory.

## Manifest Configuration

Visual Studio requires `<Content>` entries in your `.vsixmanifest` to register templates. Add these to your manifest:

```xml
<Content>
  <ProjectTemplate Path="ProjectTemplates" />
  <ItemTemplate Path="ItemTemplates" />
</Content>
```

The SDK will emit warnings if you have templates defined but missing manifest entries:
- **VSIXSDK011**: Project templates defined but no `<ProjectTemplate>` in manifest
- **VSIXSDK012**: Item templates defined but no `<ItemTemplate>` in manifest

## Target Subfolders

To organize templates into subfolders within the VSIX:

```xml
<ItemGroup>
  <!-- Will be placed at ProjectTemplates/CSharp/ -->
  <VsixProjectTemplate Include="ProjectTemplates\MyTemplate" TargetSubPath="CSharp" />

  <!-- Will be placed at ItemTemplates/Web/ -->
  <VsixItemTemplate Include="ItemTemplates\MyItem" TargetSubPath="Web" />
</ItemGroup>
```

## Complete Example

### Project File

```xml
<Project Sdk="CodingWithCalvin.VsixSdk/1.0.0">

  <PropertyGroup>
    <Version>1.0.0</Version>
  </PropertyGroup>

  <!-- Templates are auto-discovered, but you can add more explicitly -->
  <ItemGroup>
    <VsixTemplateReference Include="..\SharedTemplates\SharedTemplates.csproj"
                           TemplateType="Project"
                           TemplatePath="Templates\SharedProject" />
  </ItemGroup>

</Project>
```

### Manifest File

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
  <Content>
    <ProjectTemplate Path="ProjectTemplates" />
    <ItemTemplate Path="ItemTemplates" />
  </Content>
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
| VSIXSDK010 | `VsixTemplateZip` item missing `TemplateType` metadata |
| VSIXSDK011 | Project templates defined but no `<ProjectTemplate>` in manifest |
| VSIXSDK012 | Item templates defined but no `<ItemTemplate>` in manifest |
| VSIXSDK013 | `VsixTemplateReference` item missing `TemplateType` metadata |
| VSIXSDK014 | `VsixTemplateReference` item missing `TemplatePath` metadata |

## Troubleshooting

### Templates not appearing in Visual Studio

1. Ensure your manifest has the appropriate `<Content>` entries
2. Check that the template zip files are included in the VSIX (open the .vsix as a zip)
3. Verify the `.vstemplate` file has correct `<ProjectType>` or `<TemplateGroupID>`
4. Reset the Visual Studio template cache: delete `%LocalAppData%\Microsoft\VisualStudio\<version>\ComponentModelCache`

### Build errors about missing template folders

Ensure the template folder exists and contains a `.vstemplate` file. For `VsixTemplateReference`, verify the `TemplatePath` is correct relative to the referenced project.

### Templates in wrong location in VSIX

Check the `TargetSubPath` metadata if you're using custom paths. By default, templates are placed directly in `ProjectTemplates/` or `ItemTemplates/`.
