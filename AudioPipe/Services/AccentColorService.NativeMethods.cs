// Adapted from https://raw.githubusercontent.com/maxtruxa/AccentColors/master/AccentColors/AccentColor.cs
using System;
using System.Runtime.InteropServices;

namespace AudioPipe.Services
{
    /// <summary>
    /// Interface to Windows' accent colors.
    /// </summary>
    public static partial class AccentColorService
    {
#pragma warning disable SA1600 // Elements must be documented
        internal static class NativeMethods
        {
            [DllImport("uxtheme.dll", EntryPoint = "#95", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern uint GetImmersiveColorFromColorSetEx(
                uint immersiveColorSet,
                uint immersiveColorType,
                bool ignoreHighContrast,
                uint highContrastCacheMode);

            [DllImport("uxtheme.dll", EntryPoint = "#100", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern IntPtr GetImmersiveColorNamedTypeByIndex(uint index);

            [DllImport("uxtheme.dll", EntryPoint = "#94", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern uint GetImmersiveColorSetCount();

            [DllImport("uxtheme.dll", EntryPoint = "#96", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern uint GetImmersiveColorTypeFromName(IntPtr name);

            [DllImport("uxtheme.dll", EntryPoint = "#98", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern uint GetImmersiveUserColorSetPreference(bool forceCheckRegistry, bool skipCheckOnFail);
        }
#pragma warning restore SA1600 // Elements must be documented
    }
}