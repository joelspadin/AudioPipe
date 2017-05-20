using System;
using System.Runtime.InteropServices;

namespace AudioPipe.Services
{
    public static class ImportService
    {
        public static bool EntryPointExists(string dllName, string entryPoint)
        {
            var library = NativeMethods.LoadLibrary(dllName);
            if (library == IntPtr.Zero)
            {
                return false;
            }

            try
            {
                var proc = NativeMethods.GetProcAddress(library, entryPoint);
                return (proc != UIntPtr.Zero);
            }
            finally
            {
                NativeMethods.FreeLibrary(library);
            }
        }

        private static class NativeMethods
        {
            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern IntPtr LoadLibrary(string lpFileName);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool FreeLibrary(IntPtr hModule);

            [DllImport("kernel32", ExactSpelling = true, SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
            public static extern UIntPtr GetProcAddress(
                IntPtr hModule,
                [MarshalAs(UnmanagedType.LPStr)] string procName);
        }
    }
}
