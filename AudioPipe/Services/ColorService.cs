using System;
using System.Windows.Media;

namespace AudioPipe.Services
{
    /// <summary>
    /// Wraps <see cref="AccentColorService"/> or <see cref="LegacyColorService"/>
    /// depending on which OS version is used.
    /// </summary>
    public static class ColorService
    {
        private static readonly object Lock = new object();
        private static IColorService instance;

        /// <summary>
        /// Gets the singleton instance of <see cref="IColorService"/>.
        /// </summary>
        public static IColorService Instance
        {
            get
            {
                lock (Lock)
                {
                    return instance ?? (instance = InitService());
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether Windows 7 colors should be used
        /// instead of immersive theme colors.
        /// </summary>
        public static bool IsLegacyTheme => Instance is LegacyColorService;

        /// <summary>
        /// Gets the color with the given name from <see cref="Instance"/>.
        /// </summary>
        /// <param name="colorName">The name of the accent color.</param>
        /// <returns>The color associated with the given name.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the given name is invalid.</exception>
        public static Color GetColor(string colorName)
        {
            return Instance[colorName];
        }

        private static IColorService InitService()
        {
            if (AccentColorService.IsSupported)
            {
                return AccentColorService.ActiveSet;
            }
            else
            {
                return new LegacyColorService();
            }
        }
    }
}
