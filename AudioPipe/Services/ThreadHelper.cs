using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;

namespace AudioPipe.Services
{
    /// <summary>
    /// Provides helper functions to threading.
    /// </summary>
    public static class ThreadHelper
    {
        /// <summary>
        /// Asserts that the call is being made on the UI thread.
        /// </summary>
        /// <param name="callerMemberName">Name of the calling function.</param>
        [Conditional("DEBUG")]
        public static void AssertOnUIThread([CallerMemberName] string callerMemberName = "")
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }

            Debug.Assert(
                Thread.CurrentThread == Application.Current.Dispatcher.Thread,
                string.Format("{0} must be called on the UI thread.", callerMemberName));
        }
    }
}