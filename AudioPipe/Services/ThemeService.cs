using System.Windows;
using System.Windows.Media;

namespace AudioPipe.Services
{
    public static class ThemeService
    {
        public static bool IsWindowTransparencyEnabled
        {
            get { return !SystemParameters.HighContrast && UserSystemPreferencesService.IsTransparencyEnabled; }
        }

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

        public static void UpdateThemeColors(ResourceDictionary dictionary)
        {
            dictionary["SystemAccentColor"] = AccentColorService.ActiveSet["SystemAccent"];

            dictionary["GlassWindowBackground"] = new SolidColorBrush(GetWindowBackgroundColor());
            ReplaceBrush(dictionary, "GlassWindowForeground", "ApplicationTextDarkTheme");
            ReplaceBrush(dictionary, "DeviceItemHighlight", "DarkListLow");

            //System.IO.File.WriteAllLines(@"C:\Users\Joel\Desktop\ThemeColors.txt",
            //    AccentColorService.ActiveSet.GetAllColorNames().Select((name) => $"{AccentColorService.ActiveSet[name]}, {name}")
            //    );
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

            var color = AccentColorService.ActiveSet[resource];
            color.A = (byte)(IsWindowTransparencyEnabled ? 0xCC : 0xFF);
            return color;
        }

        private static void SetBrushWithOpacity(ResourceDictionary dictionary, string name, string immersiveAccentName, double opacity)
        {
            var color = AccentColorService.ActiveSet[immersiveAccentName];
            color.A = (byte)(opacity * 255);
            ((SolidColorBrush)dictionary[name]).Color = color;
        }

        private static void SetBrush(ResourceDictionary dictionary, string name, string immersiveAccentName)
        {
            SetBrushWithOpacity(dictionary, name, immersiveAccentName, 1.0);
        }

        private static void ReplaceBrush(ResourceDictionary dictionary, string name, string immersiveAccentName)
        {
            dictionary[name] = new SolidColorBrush(AccentColorService.ActiveSet[immersiveAccentName]);
        }

        private static void ReplaceBrushWithOpacity(ResourceDictionary dictionary, string name, string immersiveAccentName, double opacity)
        {
            var color = AccentColorService.ActiveSet[immersiveAccentName];
            color.A = (byte)(opacity * 255);
            dictionary[name] = new SolidColorBrush(color);
        }
    }
}