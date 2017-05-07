using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;

namespace AudioPipe.Services
{
    public static class ThemeService
    {
        public static bool IsWindowTransparencyEnabled
        {
            get { return !SystemParameters.HighContrast && UserSystemPreferencesService.IsTransparencyEnabled; }
        }

        public static void UpdateThemeResources(ResourceDictionary dictionary)
        {
            dictionary["WindowBackground"] = new SolidColorBrush(GetWindowBackgroundColor());

            ReplaceBrush(dictionary, "WindowForeground", "ApplicationTextDarkTheme");
            ReplaceBrushWithOpacity(dictionary, "ItemSelected", "SystemAccent", 0.8);

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