using Microsoft.Win32;

namespace AudioPipe.Services
{
    /// <summary>
    /// Accesses Windows' user preferences data in the registry.
    /// </summary>
    public static class UserSystemPreferencesService
    {
        private const string SubKeyName = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        /// <summary>
        /// Gets a value indicating whether window transparency is enabled.
        /// </summary>
        public static bool IsTransparencyEnabled => GetIntValue("EnableTransparency", 0) > 0;

        /// <summary>
        /// Gets a value indicating whether window frames use the accent color.
        /// </summary>
        public static bool UseAccentColor => GetIntValue("ColorPrevalence", 0) > 0;

        private static int GetIntValue(string name, int defaultValue)
        {
            using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
            {
                return (int?)baseKey?.OpenSubKey(SubKeyName)?.GetValue(name, defaultValue) ?? defaultValue;
            }
        }
    }
}