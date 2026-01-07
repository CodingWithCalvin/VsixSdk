// This file uses generated constants to verify all generators work together.

using System;

namespace E2E.AllFeatures
{
    /// <summary>
    /// Consumes generated constants to verify all features work together.
    /// </summary>
    public static class FeatureConsumer
    {
        // VsixInfo constants
        public static string ExtensionId => VsixInfo.Id;
        public static string ExtensionDisplayName => VsixInfo.DisplayName;

        // VSCT constants
        public static Guid PackageGuid => AllCommandsVsct.guidAllFeaturesPackage;
        public static int CommandId => AllCommandsVsct.guidAllFeaturesCommandSet.AllFeaturesCommand;

        /// <summary>
        /// Validates that all generated constants are available.
        /// </summary>
        public static void ValidateAllFeatures()
        {
            if (string.IsNullOrEmpty(ExtensionId))
                throw new InvalidOperationException("VsixInfo.Id is empty");

            if (PackageGuid == Guid.Empty)
                throw new InvalidOperationException("Package GUID is empty");

            if (CommandId != 0x0100)
                throw new InvalidOperationException("Command ID has wrong value");
        }
    }
}
