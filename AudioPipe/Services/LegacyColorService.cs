using AudioPipe.Extensions;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace AudioPipe.Services
{
    public class LegacyColorService : IColorService
    {
        private const byte DarkThreshold = 127;

        public Color this[string colorName]
        {
            get
            {
                switch (colorName)
                {
                    case "ApplicationBackground":
                    case "DarkChromeMediumLow":
                    case "SystemAccentDark1":
                    case "SystemAccentDark2":
                        return SystemColors.WindowColor;

                    case "ApplicationTextDarkTheme":
                        return SystemColors.WindowTextColor;

                    case "DeviceItemSelectedText":
                        return SystemColors.HighlightTextColor;

                    case "GlassWindowBorder":
                        return SystemColors.WindowFrameColor;

                    case "DeviceItemSelected":
                    case "SystemAccent":
                        if (SystemParameters.HighContrast)
                        {
                            return SystemColors.HighlightColor;
                        }
                        else
                        {
                            return WindowFrameService.DwmColor;
                        }

                    case "DeviceItemHighlight":
                        byte value = Math.Max(SystemColors.WindowColor.R, Math.Max(SystemColors.WindowColor.G, SystemColors.WindowColor.B));
                        if (value > DarkThreshold)
                        {
                            return Color.FromArgb(0x19, 0x00, 0x00, 0x00);
                        }
                        else
                        {
                            return Color.FromArgb(0x19, 0xFF, 0xFF, 0xFF);
                        }

                    default:
                        return Colors.Transparent;
                }
            }
        }


    }
}
