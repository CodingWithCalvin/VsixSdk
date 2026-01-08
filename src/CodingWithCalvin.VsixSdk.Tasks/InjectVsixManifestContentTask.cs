using System;
using System.IO;
using System.Xml;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CodingWithCalvin.VsixSdk.Tasks;

/// <summary>
/// MSBuild task that injects Content entries into a VSIX manifest for discovered templates.
/// Creates an intermediate manifest file without modifying the source.
/// </summary>
public class InjectVsixManifestContentTask : Task
{
    private const string VsixNamespace = "http://schemas.microsoft.com/developer/vsx-schema/2011";

    /// <summary>
    /// Path to the source VSIX manifest file.
    /// </summary>
    [Required]
    public string SourceManifestPath { get; set; } = string.Empty;

    /// <summary>
    /// Path where the modified manifest will be written.
    /// </summary>
    [Required]
    public string OutputManifestPath { get; set; } = string.Empty;

    /// <summary>
    /// Whether project templates were discovered.
    /// </summary>
    public bool HasProjectTemplates { get; set; }

    /// <summary>
    /// Whether item templates were discovered.
    /// </summary>
    public bool HasItemTemplates { get; set; }

    /// <summary>
    /// The folder path for project templates (default: "ProjectTemplates").
    /// </summary>
    public string ProjectTemplatesPath { get; set; } = "ProjectTemplates";

    /// <summary>
    /// The folder path for item templates (default: "ItemTemplates").
    /// </summary>
    public string ItemTemplatesPath { get; set; } = "ItemTemplates";

    public override bool Execute()
    {
        try
        {
            if (!File.Exists(SourceManifestPath))
            {
                Log.LogError("VSIXSDK020", null, null, null, 0, 0, 0, 0,
                    "Source manifest not found: {0}", SourceManifestPath);
                return false;
            }

            var doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(SourceManifestPath);

            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("vsix", VsixNamespace);

            var modified = false;

            var packageManifest = doc.SelectSingleNode("/vsix:PackageManifest", nsmgr);
            if (packageManifest == null)
            {
                Log.LogError("VSIXSDK021", null, null, SourceManifestPath, 0, 0, 0, 0,
                    "Invalid manifest: PackageManifest element not found");
                return false;
            }

            var contentElement = doc.SelectSingleNode("/vsix:PackageManifest/vsix:Content", nsmgr);
            if (contentElement == null && (HasProjectTemplates || HasItemTemplates))
            {
                contentElement = doc.CreateElement("Content", VsixNamespace);
                packageManifest.AppendChild(contentElement);
                modified = true;
                Log.LogMessage(MessageImportance.Normal, "Created Content element in manifest");
            }

            if (contentElement != null)
            {
                if (HasProjectTemplates)
                {
                    var existingProjectTemplate = contentElement.SelectSingleNode(
                        "vsix:ProjectTemplate", nsmgr);
                    if (existingProjectTemplate == null)
                    {
                        var projectTemplateElement = doc.CreateElement("ProjectTemplate", VsixNamespace);
                        projectTemplateElement.SetAttribute("Path", ProjectTemplatesPath);
                        contentElement.AppendChild(projectTemplateElement);
                        modified = true;
                        Log.LogMessage(MessageImportance.Normal,
                            "Added ProjectTemplate entry with Path='{0}'", ProjectTemplatesPath);
                    }
                    else
                    {
                        Log.LogMessage(MessageImportance.Low,
                            "ProjectTemplate entry already exists, skipping injection");
                    }
                }

                if (HasItemTemplates)
                {
                    var existingItemTemplate = contentElement.SelectSingleNode(
                        "vsix:ItemTemplate", nsmgr);
                    if (existingItemTemplate == null)
                    {
                        var itemTemplateElement = doc.CreateElement("ItemTemplate", VsixNamespace);
                        itemTemplateElement.SetAttribute("Path", ItemTemplatesPath);
                        contentElement.AppendChild(itemTemplateElement);
                        modified = true;
                        Log.LogMessage(MessageImportance.Normal,
                            "Added ItemTemplate entry with Path='{0}'", ItemTemplatesPath);
                    }
                    else
                    {
                        Log.LogMessage(MessageImportance.Low,
                            "ItemTemplate entry already exists, skipping injection");
                    }
                }
            }

            var outputDir = Path.GetDirectoryName(OutputManifestPath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            doc.Save(OutputManifestPath);

            if (modified)
            {
                Log.LogMessage(MessageImportance.High,
                    "Injected template Content entries into manifest: {0}", OutputManifestPath);
            }
            else
            {
                Log.LogMessage(MessageImportance.Normal,
                    "No template Content injection needed, copied manifest to: {0}", OutputManifestPath);
            }

            return true;
        }
        catch (Exception ex)
        {
            Log.LogErrorFromException(ex, showStackTrace: true);
            return false;
        }
    }
}
