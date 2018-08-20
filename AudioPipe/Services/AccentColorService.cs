// Adapted from https://raw.githubusercontent.com/maxtruxa/AccentColors/master/AccentColors/AccentColor.cs
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace AudioPipe.Services
{
    /// <summary>
    /// Interface to Windows' accent colors.
    /// </summary>
    public static partial class AccentColorService
    {
        private static AccentColorSet activeSet;
        private static AccentColorSet[] allSets;

        /// <summary>
        /// Gets the active <see cref="AccentColorSet"/>.
        /// </summary>
        public static AccentColorSet ActiveSet
        {
            get
            {
                uint activeSet = NativeMethods.GetImmersiveUserColorSetPreference(false, false);
                ActiveSet = AllSets[Math.Min(activeSet, AllSets.Length - 1)];
                return AccentColorService.activeSet;
            }

            private set
            {
                if (activeSet != null)
                {
                    activeSet.Active = false;
                }

                value.Active = true;
                activeSet = value;
            }
        }

        /// <summary>
        /// Gets the list of <see cref="AccentColorSet"/>s.
        /// </summary>
        public static AccentColorSet[] AllSets
        {
            get
            {
                if (allSets == null)
                {
                    uint colorSetCount = NativeMethods.GetImmersiveColorSetCount();

                    var colorSets = new List<AccentColorSet>();
                    for (uint i = 0; i < colorSetCount; i++)
                    {
                        colorSets.Add(new AccentColorSet(i, false));
                    }

                    AllSets = colorSets.ToArray();
                }

                return allSets;
            }

            private set
            {
                allSets = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Windows supports accent colors.
        /// </summary>
        public static bool IsSupported => ImportService.EntryPointExists("uxtheme.dll", "GetImmersiveUserColorSetPreference");

        /// <summary>
        /// Implements <see cref="IColorService"/> for accessing one set
        /// of accent colors.
        /// </summary>
        public class AccentColorSet : IColorService
        {
            private readonly uint colorSet;

            /// <summary>
            /// Initializes a new instance of the <see cref="AccentColorSet"/> class.
            /// </summary>
            /// <param name="colorSet">The index of the color set.</param>
            /// <param name="active">Whether the set is active.</param>
            internal AccentColorSet(uint colorSet, bool active)
            {
                this.colorSet = colorSet;
                Active = active;
            }

            /// <summary>
            /// Gets a value indicating whether the set is active.
            /// </summary>
            public bool Active { get; internal set; }

            /// <inheritdoc/>
            public Color this[string colorName]
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

            /// <summary>
            /// Gets the accent color with the given color type index.
            /// </summary>
            /// <param name="colorType">The color type index.</param>
            /// <returns>The color associated with the given color type.</returns>
            public Color this[uint colorType]
            {
                get
                {
                    uint nativeColor = NativeMethods.GetImmersiveColorFromColorSetEx(colorSet, colorType, false, 0);

                    // if (nativeColor == 0)
                    //     throw new InvalidOperationException();
                    return Color.FromArgb(
                        (byte)((0xFF000000 & nativeColor) >> 24),
                        (byte)((0x000000FF & nativeColor) >> 0),
                        (byte)((0x0000FF00 & nativeColor) >> 8),
                        (byte)((0x00FF0000 & nativeColor) >> 16));
                }
            }

            /// <summary>
            /// Gets a list of all accent color names.
            /// </summary>
            /// <returns>A list of all accent color names.</returns>
            public List<string> GetAllColorNames()
            {
                // HACK: GetAllColorNames collects the available color names by brute forcing the OS function.
                //   Since there is currently no known way to retrieve all possible color names,
                //   the method below just tries all indices from 0 to 0xFFF ignoring errors.
                var allColorNames = new List<string>();
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
    }
}