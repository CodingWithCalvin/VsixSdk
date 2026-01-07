using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;

namespace E2E.Templates.ReferenceManual
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid("00000000-0000-0000-0000-000000000021")]
    public sealed class E2ETemplatesReferenceManualPackage : AsyncPackage
    {
    }
}
