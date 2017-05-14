using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace AudioPipe.Extensions
{
    public class WindowStyle : DependencyObject
    {


        public static bool GetShowIcon(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowIconProperty);
        }

        public static void SetShowIcon(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowIconProperty, value);
        }

        // Using a DependencyProperty as the backing store for ShowIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowIconProperty =
            DependencyProperty.RegisterAttached(
                "ShowIcon",
                typeof(bool),
                typeof(WindowStyle),
                new FrameworkPropertyMetadata(true, new PropertyChangedCallback((d, e) => UpdateIcon(d))));

        private static void UpdateIcon(DependencyObject obj)
        {
            if (GetShowIcon(obj))
            {
                // Not implemented.
            }
            else
            {
                var window = obj as Window;
                if (window != null)
                {
                    window.SourceInitialized += delegate
                    {
                        HideIcon(window);
                    };
                }
            }
        }

        private static void HideIcon(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;

            var extStyle = GetWindowLong(hwnd, GwlExstyle);
            SetWindowLong(hwnd, GwlExstyle, extStyle | WsExDlgmodalframe);
        }


        private const int GwlExstyle = -20;
        private const int WsExDlgmodalframe = 0x0001;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hwnd, uint msg,
          IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);
    }
}
