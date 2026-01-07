using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace SampleExtensionWithTemplates
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    public sealed class SampleExtensionWithTemplatesPackage : AsyncPackage
    {
        /// <summary>
        /// SampleExtensionWithTemplatesPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "b2c3d4e5-f6a7-8901-bcde-f23456789012";

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited.
        /// </summary>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            // When initialized asynchronously, switch to the main thread before accessing VS services
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // This extension provides project and item templates
            // Templates are automatically discovered from ProjectTemplates/ and ItemTemplates/ folders
        }
    }
}
