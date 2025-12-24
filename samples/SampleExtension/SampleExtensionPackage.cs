using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace SampleExtension
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    public sealed class SampleExtensionPackage : AsyncPackage
    {
        /// <summary>
        /// SampleExtensionPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "a1b2c3d4-e5f6-7890-abcd-ef1234567890";

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited.
        /// </summary>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            // When initialized asynchronously, switch to the main thread before accessing VS services
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // TODO: Add your initialization code here
        }
    }
}
