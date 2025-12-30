using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace VsixExtension
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    public sealed class VsixExtensionPackage : AsyncPackage
    {
        /// <summary>
        /// Package GUID string.
        /// </summary>
        public const string PackageGuidString = "TEMPLATE_GUID2";

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);

            // When initialized asynchronously, switch to the main thread before accessing VS services
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            // TODO: Add your initialization code here
        }
    }
}
