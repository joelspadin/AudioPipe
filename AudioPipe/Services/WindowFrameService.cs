using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace AudioPipe.Services
{
    /// <summary>
    /// Provides utilities for manipulating the window frame.
    /// </summary>
    public static partial class WindowFrameService
    {
        private const int DefaultDwmColor = 0x6b74b8fc;

        /// <summary>
        /// Gets the current window frame color.
        /// </summary>
        public static Color DwmColor
        {
            get
            {
                // NativeMethods.DwmGetColorizationColor(out uint color, out bool opaque);
                var color = (int)Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorizationColor", DefaultDwmColor);

                return Color.FromArgb(
                    0xFF,
                    (byte)((color >> 16) & 0xFF),
                    (byte)((color >> 8) & 0xFF),
                    (byte)(color & 0xFF));
            }
        }

        /// <summary>
        /// Gets the width in pixels of the window frame glass border.
        /// </summary>
        public static int GlassBorderWidth => NativeMethods.GetSystemMetrics(NativeMethods.SM_CXSIZEFRAME)
                                            + NativeMethods.GetSystemMetrics(NativeMethods.SM_CXPADDEDBORDER);

        /// <summary>
        /// Gets a value indicating whether DWM composition is enabled,
        /// which enables the Aero glass theme on Windows 7.
        /// </summary>
        public static bool IsDwmCompositionEnabled => NativeMethods.DwmIsCompositionEnabled();

        /// <summary>
        /// Removes the glass border from the window frame.
        /// </summary>
        /// <param name="window">The window to modify.</param>
        public static void HideGlassBorder(Window window)
        {
            RemoveFlags(window, NativeMethods.GWL_STYLE, NativeMethods.WS_BORDER | NativeMethods.WS_THICKFRAME);
        }

        /// <summary>
        /// Hides the application icon from the window frame.
        /// </summary>
        /// <param name="window">The window to modify.</param>
        public static void HideIcon(Window window)
        {
            AddFlags(window, NativeMethods.GWL_EXSTYLE, NativeMethods.WS_EX_DLGMODALFRAME);
        }

        /// <summary>
        /// Enables the glass border for the window frame.
        /// </summary>
        /// <param name="window">The window to modify.</param>
        public static void ShowGlassBorder(Window window)
        {
            AddFlags(window, NativeMethods.GWL_STYLE, NativeMethods.WS_BORDER | NativeMethods.WS_THICKFRAME);
        }

        private static void AddFlags(Window window, int index, int newFlags)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            var flags = NativeMethods.GetWindowLong(hwnd, index);
            NativeMethods.SetWindowLong(hwnd, index, flags | newFlags);
        }

        private static void RemoveFlags(Window window, int index, int newFlags)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            var flags = NativeMethods.GetWindowLong(hwnd, index);
            NativeMethods.SetWindowLong(hwnd, index, flags & ~newFlags);
        }
    }
}
