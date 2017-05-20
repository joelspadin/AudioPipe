using Microsoft.Win32;

namespace AudioPipe.Services
{
    public static class UserSystemPreferencesService
    {
        private const string SubKeyName = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        private static int GetIntValue(string name, int defaultValue)
        {
            using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
            {
                return (int?)baseKey?.OpenSubKey(SubKeyName)?.GetValue(name, defaultValue) ?? defaultValue;
            }
        }

        public static bool IsTransparencyEnabled => GetIntValue("EnableTransparency", 0) > 0;

        public static bool UseAccentColor => GetIntValue("ColorPrevalence", 0) > 0;
    }
}