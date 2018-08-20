using System.Windows;
using System.Windows.Media;

namespace AudioPipe.Services
{
    /// <summary>
    /// Provides utilities for accessing Windows' themes.
    /// </summary>
    public static class ThemeService
    {
        /// <summary>
        /// Gets a value indicating whether the window background may be transparent.
        /// </summary>
        public static bool IsWindowTransparencyEnabled => !SystemParameters.HighContrast && UserSystemPreferencesService.IsTransparencyEnabled;

        /// <summary>
        /// Finds a descendant of a <see cref="ResourceDictionary"/> with a given name.
        /// </summary>
        /// <param name="root">The root <see cref="ResourceDictionary"/> for the search.</param>
        /// <param name="filename">The name of the dictionary to find.</param>
        /// <returns>The found dictionary, or null if no such dictionary could be found.</returns>
        public static ResourceDictionary FindResourceDictionary(ResourceDictionary root, string filename)
        {
            if (root.Source?.OriginalString.EndsWith(filename) ?? false)
            {
                return root;
            }

            foreach (var dict in root.MergedDictionaries)
            {
                var found = FindResourceDictionary(dict, filename);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        /// <summary>
        /// Updates theme color resources in the given <see cref="ResourceDictionary"/>
        /// to match Windows' current colors.
        /// </summary>
        /// <param name="dictionary">The theme dictionary to update.</param>
        public static void UpdateThemeColors(ResourceDictionary dictionary)
        {
            dictionary["SystemAccentColor"] = ColorService.GetColor("SystemAccent");

            if (ColorService.IsLegacyTheme)
            {
                if (WindowFrameService.IsDwmCompositionEnabled)
                {
                    // If aero glass is enabled, it provides its own border so
                    // don't add a border inside the window.
                    dictionary["GlassWindowBorderThickness"] = new Thickness(0);
                    dictionary["GlassWindowBorder"] = Brushes.Transparent;
                }
                else
                {
                    dictionary["GlassWindowBorderThickness"] = new Thickness(1);
                    dictionary["GlassWindowBorder"] = new SolidColorBrush(ColorService.GetColor("GlassWindowBorder"));
                }

                ReplaceBrush(dictionary, "GlassWindowBackground", "ApplicationBackground");
                ReplaceBrush(dictionary, "GlassWindowForeground", "ApplicationTextDarkTheme");

                ReplaceBrush(dictionary, "DeviceItemHighlight", "DeviceItemHighlight");
                ReplaceBrush(dictionary, "DeviceItemSelected", "DeviceItemSelected");
                ReplaceBrush(dictionary, "DeviceItemSelectedText", "DeviceItemSelectedText");
            }
            else
            {
                dictionary["GlassWindowBackground"] = new SolidColorBrush(GetWindowBackgroundColor());
                ReplaceBrush(dictionary, "GlassWindowForeground", "ApplicationTextDarkTheme");

                ReplaceBrush(dictionary, "DeviceItemHighlight", "DarkListLow");
                ReplaceBrush(dictionary, "DeviceItemSelected", "SystemAccent", 0.8);
                ReplaceBrush(dictionary, "DeviceItemSelectedText", "ApplicationTextDarkTheme");
            }
        }

        private static Color GetWindowBackgroundColor()
        {
            string resource;
            if (SystemParameters.HighContrast)
            {
                resource = "ApplicationBackground";
            }
            else if (UserSystemPreferencesService.UseAccentColor)
            {
                resource = IsWindowTransparencyEnabled ? "SystemAccentDark2" : "SystemAccentDark1";
            }
            else
            {
                resource = "DarkChromeMediumLow";
            }

            var color = ColorService.GetColor(resource);
            color.A = (byte)(IsWindowTransparencyEnabled ? 0xCC : 0xFF);
            return color;
        }

        private static void ReplaceBrush(ResourceDictionary dictionary, string name, string immersiveAccentName)
        {
            ReplaceBrush(dictionary, name, immersiveAccentName, 1);
        }

        private static void ReplaceBrush(ResourceDictionary dictionary, string name, string immersiveAccentName, double opacity)
        {
            var color = ColorService.GetColor(immersiveAccentName);
            dictionary[name] = new SolidColorBrush(color)
            {
                Opacity = opacity
            };
        }
    }
}