using System;
using System.Runtime.InteropServices;

namespace AudioPipe.Services
{
    /// <summary>
    /// Provides utilities for importing DLL methods.
    /// </summary>
    public static class ImportService
    {
        /// <summary>
        /// Checks whether the DLL named <paramref name="dllName"/> has a method named <paramref name="entryPoint"/>.
        /// </summary>
        /// <param name="dllName">The name of the DLL to check.</param>
        /// <param name="entryPoint">The name of the method to find.</param>
        /// <returns>Whether <paramref name="dllName"/> has a method named <paramref name="entryPoint"/>.</returns>
        public static bool EntryPointExists(string dllName, string entryPoint)
        {
            var library = NativeMethods.LoadLibrary(dllName);
            if (library == IntPtr.Zero)
            {
                return false;
            }

            try
            {
                return NativeMethods.GetProcAddress(library, entryPoint) != UIntPtr.Zero;
            }
            finally
            {
                NativeMethods.FreeLibrary(library);
            }
        }

        private static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool FreeLibrary(IntPtr hModule);

            [DllImport("kernel32", ExactSpelling = true, SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
            public static extern UIntPtr GetProcAddress(
                IntPtr hModule,
                [MarshalAs(UnmanagedType.LPStr)] string procName);

            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern IntPtr LoadLibrary(string lpFileName);
        }
    }
}
