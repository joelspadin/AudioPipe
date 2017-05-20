using AudioPipe.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace AudioPipe.Services
{
    public static class WindowFrameService
    {
        private const int DefaultDwmColor = 0x6b74b8fc;

        private static class NativeMethods
        {
            public const int GWL_STYLE = -16;
            public const int GWL_EXSTYLE = -20;

            public const int WS_OVERLAPPED = 0x00000000;
            public const int WS_CHILD = 0x40000000;
            public const int WS_MINIMIZE = 0x20000000;
            public const int WS_VISIBLE = 0x10000000;
            public const int WS_DISABLED = 0x08000000;
            public const int WS_CLIPSIBLINGS = 0x04000000;
            public const int WS_CLIPCHILDREN = 0x02000000;
            public const int WS_MAXIMIZE = 0x01000000;
            public const int WS_CAPTION = 0x00C00000;     /* WS_BORDER | WS_DLGFRAME  */
            public const int WS_BORDER = 0x00800000;
            public const int WS_DLGFRAME = 0x00400000;
            public const int WS_VSCROLL = 0x00200000;
            public const int WS_HSCROLL = 0x00100000;
            public const int WS_SYSMENU = 0x00080000;
            public const int WS_THICKFRAME = 0x00040000;
            public const int WS_GROUP = 0x00020000;
            public const int WS_TABSTOP = 0x00010000;

            public const int WS_MINIMIZEBOX = 0x00020000;
            public const int WS_MAXIMIZEBOX = 0x00010000;

            public const int WS_EX_DLGMODALFRAME = 0x0001;
            public const int WS_EX_TOPMOST = 0x00000008;
            public const int WS_EX_TOOLWINDOW = 0x00000080;
            public const int WS_EX_WINDOWEDGE = 0x00000100;
            public const int WS_EX_CLIENTEDGE = 0x00000200;
            public const int WS_EX_STATICEDGE = 0x00020000;
            public const int WS_EX_APPWINDOW = 0x00040000;
            public const int WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
            public const int WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);

            public const int SM_CXSIZEFRAME = 32;
            public const int SM_CXPADDEDBORDER = 92;

            [DllImport("dwmapi.dll", PreserveSig = false)]
            public static extern bool DwmIsCompositionEnabled();

            [DllImport("dwmapi.dll", PreserveSig = false)]
            public static extern void DwmGetColorizationColor(out uint ColorizationColor, [MarshalAs(UnmanagedType.Bool)]out bool ColorizationOpaqueBlend);

            [DllImport("user32.dll")]
            public static extern int GetSystemMetrics(int smIndex);

            [DllImport("user32.dll")]
            public static extern int GetWindowLong(IntPtr hwnd, int index);

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            public static extern int SetWindowLong(IntPtr hwnd, int index, int dwNewLong);

            [DllImport("user32.dll")]
            public static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);
        }

        public static bool IsDwmCompositionEnabled => NativeMethods.DwmIsCompositionEnabled();

        public static Color DwmColor
        {
            get
            {
                //NativeMethods.DwmGetColorizationColor(out uint color, out bool opaque);
                var color = (int)Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorizationColor", DefaultDwmColor);

                return Color.FromArgb(
                    0xFF,
                    (byte)((color >> 16) & 0xFF),
                    (byte)((color >> 8) & 0xFF),
                    (byte)(color & 0xFF));
            }
        }

        public static int GlassBorderWidth => NativeMethods.GetSystemMetrics(NativeMethods.SM_CXSIZEFRAME)
                                            + NativeMethods.GetSystemMetrics(NativeMethods.SM_CXPADDEDBORDER);

        public static void HideIcon(Window window)
        {
            AddFlags(window, NativeMethods.GWL_EXSTYLE, NativeMethods.WS_EX_DLGMODALFRAME);
        }

        public static void ShowGlassBorder(Window window)
        {
            AddFlags(window, NativeMethods.GWL_STYLE, NativeMethods.WS_BORDER | NativeMethods.WS_THICKFRAME);
        }

        public static void HideGlassBorder(Window window)
        {
            RemoveFlags(window, NativeMethods.GWL_STYLE, NativeMethods.WS_BORDER | NativeMethods.WS_THICKFRAME);
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
