using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AudioPipe.Services
{
    public static class ColorService
    {
        private static IColorService _instance;
        private static object _lock = new object();

        public static Color GetColor(string colorName)
        {
            return Instance[colorName];
        }

        /// <summary>
        /// True if using Windows 7 colors instead of immersive theme colors.
        /// </summary>
        public static bool IsLegacyTheme => Instance is LegacyColorService;

        public static IColorService Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        if (AccentColorService.IsSupported)
                        {
                            _instance = AccentColorService.ActiveSet;
                        }
                        else
                        {
                            _instance = new LegacyColorService();
                        }
                    }

                    return _instance;
                }
            }
        }
    }
}
