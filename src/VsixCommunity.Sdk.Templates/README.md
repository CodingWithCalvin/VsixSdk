# VsixCommunity.Sdk.Templates

Project templates for creating Visual Studio extensions using [VsixCommunity.Sdk](https://github.com/CodingWithCalvin/VsixCommunity.Sdk).

## Installation

```bash
dotnet new install VsixCommunity.Sdk.Templates
```

## Available Templates

| Template | Short Name | Description |
|----------|------------|-------------|
| VSIX Extension | `vsix` | A Visual Studio extension project using SDK-style format |

## Usage

### Create a new VSIX extension

```bash
dotnet new vsix -n MyExtension
```

### Options

| Option | Description |
|--------|-------------|
| `-n, --name` | The name for the extension (and output directory) |
| `-o, --output` | Location to place the generated output |
| `--publisher` | Publisher name for the VSIX manifest |
| `--description` | Description for the extension |

### Examples

```bash
# Create extension with custom name
dotnet new vsix -n MyAwesomeExtension

# Create extension with publisher info
dotnet new vsix -n MyExtension --publisher "My Company"

# Create in specific directory
dotnet new vsix -n MyExtension -o src/MyExtension
```

## After Creating

1. Open the solution in Visual Studio 2022+
2. Press F5 to debug in the Experimental Instance
3. Build your extension!

## Uninstall

```bash
dotnet new uninstall VsixCommunity.Sdk.Templates
```
