using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Media;

// https://raw.githubusercontent.com/maxtruxa/AccentColors/master/AccentColors/AccentColor.cs

namespace AudioPipe.Services
{
    public static class AccentColorService
    {
        static AccentColorSet[] _allSets;
        static AccentColorSet _activeSet;

        public static bool IsSupported => ImportService.EntryPointExists("uxtheme.dll", "GetImmersiveUserColorSetPreference");

        public static AccentColorSet[] AllSets
        {
            get
            {
                if (_allSets == null)
                {
                    UInt32 colorSetCount = NativeMethods.GetImmersiveColorSetCount();

                    var colorSets = new List<AccentColorSet>();
                    for (UInt32 i = 0; i < colorSetCount; i++)
                    {
                        colorSets.Add(new AccentColorSet(i, false));
                    }

                    AllSets = colorSets.ToArray();
                }

                return _allSets;
            }
            private set
            {
                _allSets = value;
            }
        }

        public static AccentColorSet ActiveSet
        {
            get
            {
                uint activeSet = NativeMethods.GetImmersiveUserColorSetPreference(false, false);
                ActiveSet = AllSets[Math.Min(activeSet, AllSets.Length - 1)];
                return _activeSet;
            }
            private set
            {
                if (_activeSet != null) _activeSet.Active = false;

                value.Active = true;
                _activeSet = value;
            }
        }

        public class AccentColorSet : IColorService
        {
            private uint _colorSet;

            public bool Active { get; internal set; }

            public Color this[String colorName]
            {
                get
                {
                    IntPtr name = IntPtr.Zero;
                    uint colorType;

                    try
                    {
                        name = Marshal.StringToHGlobalUni("Immersive" + colorName);
                        colorType = NativeMethods.GetImmersiveColorTypeFromName(name);
                        if (colorType == 0xFFFFFFFF)
                        {
                            throw new InvalidOperationException();
                        }
                    }
                    finally
                    {
                        if (name != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(name);
                            name = IntPtr.Zero;
                        }
                    }

                    return this[colorType];
                }
            }

            public Color this[uint colorType]
            {
                get
                {
                    uint nativeColor = NativeMethods.GetImmersiveColorFromColorSetEx(_colorSet, colorType, false, 0);
                    //if (nativeColor == 0)
                    //    throw new InvalidOperationException();
                    return Color.FromArgb(
                        (Byte)((0xFF000000 & nativeColor) >> 24),
                        (Byte)((0x000000FF & nativeColor) >> 0),
                        (Byte)((0x0000FF00 & nativeColor) >> 8),
                        (Byte)((0x00FF0000 & nativeColor) >> 16)
                        );
                }
            }

            internal AccentColorSet(uint colorSet, bool active)
            {
                _colorSet = colorSet;
                Active = active;
            }

            // HACK: GetAllColorNames collects the available color names by brute forcing the OS function.
            //   Since there is currently no known way to retrieve all possible color names,
            //   the method below just tries all indices from 0 to 0xFFF ignoring errors.
            public List<String> GetAllColorNames()
            {
                var allColorNames = new List<String>();
                for (uint i = 0; i < 0xFFF; i++)
                {
                    IntPtr typeNamePtr = NativeMethods.GetImmersiveColorNamedTypeByIndex(i);
                    if (typeNamePtr != IntPtr.Zero)
                    {
                        var typeName = (IntPtr)Marshal.PtrToStructure(typeNamePtr, typeof(IntPtr));
                        allColorNames.Add(Marshal.PtrToStringUni(typeName));
                    }
                }

                return allColorNames;
            }
        }

        internal static class NativeMethods
        {
            [DllImport("uxtheme.dll", EntryPoint = "#98", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern UInt32 GetImmersiveUserColorSetPreference(Boolean forceCheckRegistry, Boolean skipCheckOnFail);

            [DllImport("uxtheme.dll", EntryPoint = "#94", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern UInt32 GetImmersiveColorSetCount();

            [DllImport("uxtheme.dll", EntryPoint = "#95", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern UInt32 GetImmersiveColorFromColorSetEx(UInt32 immersiveColorSet, UInt32 immersiveColorType,
                Boolean ignoreHighContrast, UInt32 highContrastCacheMode);

            [DllImport("uxtheme.dll", EntryPoint = "#96", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern UInt32 GetImmersiveColorTypeFromName(IntPtr name);

            [DllImport("uxtheme.dll", EntryPoint = "#100", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
            public static extern IntPtr GetImmersiveColorNamedTypeByIndex(UInt32 index);
        }
    }
}