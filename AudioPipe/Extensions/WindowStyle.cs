using AudioPipe.Services;
using System;
using System.Runtime.InteropServices;
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
                        WindowFrameService.HideIcon(window);
                    };
                }
            }
        }
    }
}
