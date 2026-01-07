// This file uses generated constants to verify compilation works correctly.
// If source generators fail, this file will not compile.

using System;

namespace E2E.SourceGenerators
{
    /// <summary>
    /// Consumes generated constants to verify source generators work.
    /// </summary>
    public static class GeneratorConsumer
    {
        // VsixInfo constants from manifest
        public static string ExtensionId => VsixInfo.Id;
        public static string ExtensionVersion => VsixInfo.Version;
        public static string ExtensionLanguage => VsixInfo.Language;
        public static string ExtensionPublisher => VsixInfo.Publisher;
        public static string ExtensionDisplayName => VsixInfo.DisplayName;
        public static string ExtensionDescription => VsixInfo.Description;
        public static string ExtensionMoreInfo => VsixInfo.MoreInfo;
        public static string ExtensionLicense => VsixInfo.License;
        public static string ExtensionGettingStartedGuide => VsixInfo.GettingStartedGuide;
        public static string ExtensionReleaseNotes => VsixInfo.ReleaseNotes;
        public static string ExtensionIcon => VsixInfo.Icon;
        public static string ExtensionPreviewImage => VsixInfo.PreviewImage;
        public static string ExtensionTags => VsixInfo.Tags;
        public static bool ExtensionIsPreview => VsixInfo.IsPreview;

        // VSCT constants - Package GUID
        public static string PackageGuidString => CommandsVsct.guidSourceGenPackageString;
        public static Guid PackageGuid => CommandsVsct.guidSourceGenPackage;

        // VSCT constants - Command Set 1
        public static string CommandSet1GuidString => CommandsVsct.guidCommandSet1.GuidString;
        public static Guid CommandSet1Guid => CommandsVsct.guidCommandSet1.Guid;
        public static int MenuGroup1 => CommandsVsct.guidCommandSet1.MenuGroup1;
        public static int Command1Id => CommandsVsct.guidCommandSet1.Command1Id;
        public static int Command2Id => CommandsVsct.guidCommandSet1.Command2Id;

        // VSCT constants - Command Set 2
        public static string CommandSet2GuidString => CommandsVsct.guidCommandSet2.GuidString;
        public static Guid CommandSet2Guid => CommandsVsct.guidCommandSet2.Guid;
        public static int MenuGroup2 => CommandsVsct.guidCommandSet2.MenuGroup2;
        public static int Command3Id => CommandsVsct.guidCommandSet2.Command3Id;

        /// <summary>
        /// Validates that all expected values are correctly generated.
        /// </summary>
        public static void ValidateGeneratedConstants()
        {
            // Validate VsixInfo values
            if (string.IsNullOrEmpty(ExtensionId))
                throw new InvalidOperationException("VsixInfo.Id is empty");
            if (string.IsNullOrEmpty(ExtensionDisplayName))
                throw new InvalidOperationException("VsixInfo.DisplayName is empty");
            if (!ExtensionIsPreview)
                throw new InvalidOperationException("VsixInfo.IsPreview should be true");

            // Validate description contains escaped content
            if (!ExtensionDescription.Contains("\""))
                throw new InvalidOperationException("VsixInfo.Description should contain escaped quotes");
            if (!ExtensionDescription.Contains("\\"))
                throw new InvalidOperationException("VsixInfo.Description should contain backslashes");

            // Validate VSCT GUIDs
            if (PackageGuid == Guid.Empty)
                throw new InvalidOperationException("Package GUID is empty");
            if (CommandSet1Guid == Guid.Empty)
                throw new InvalidOperationException("CommandSet1 GUID is empty");
            if (CommandSet2Guid == Guid.Empty)
                throw new InvalidOperationException("CommandSet2 GUID is empty");

            // Validate command IDs
            if (Command1Id != 0x0100)
                throw new InvalidOperationException("Command1Id has wrong value");
            if (Command2Id != 0x0101)
                throw new InvalidOperationException("Command2Id has wrong value");
            if (Command3Id != 0x0200)
                throw new InvalidOperationException("Command3Id has wrong value");
        }
    }
}
