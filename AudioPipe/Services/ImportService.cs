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
            [DllImport("kernel32", SetLastError = true)]
            public static extern IntPtr LoadLibrary(string lpFileName);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool FreeLibrary(IntPtr hModule);

            [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            public static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);
        }
    }
}
