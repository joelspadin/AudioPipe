using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace AudioPipe.Services
{
    /// <summary>
    /// Handles the WM_NCHITTEST message and prevents the aero glass border
    /// from acting as a resize handle.
    /// </summary>
    public static class HitTestService
    {
        private enum HitTest
        {
            Nowhere = 0,
            ClientArea = 1,
        }

        private static IntPtr IntPtr(this HitTest e)
        {
            return new IntPtr((int)e);
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct MousePosition
        {
            public MousePosition(uint val)
            {
                X = Y = 0;
                Value = val;
            }

            public MousePosition(IntPtr ptr)
            {
                X = Y = 0;
                Value = (uint)ptr.ToInt32();
            }

            [FieldOffset(0)]
            public UInt32 Value;

            [FieldOffset(0)]
            public UInt16 X;

            [FieldOffset(2)]
            public UInt16 Y;
        }

        public static bool IsMouseInWindow(Window window, IntPtr lParam)
        {
            var pos = new MousePosition(lParam);
            var origin = window.PointToScreen(new Point(0, 0));
            var windowPos = new Point(pos.X - origin.X, pos.Y - origin.Y);

            return windowPos.X >= 0 && windowPos.X < window.ActualWidth &&
                windowPos.Y >= 0 && windowPos.Y < window.ActualHeight;
        }

        public static IntPtr DoHitTest(Window window, IntPtr lParam, out bool handled)
        {
            if (IsMouseInWindow(window, lParam))
            {
                handled = true;
                return HitTest.ClientArea.IntPtr();
            }
            else
            {
                // The window shouldn't be resizable. Hide the fact that the
                // mouse may be over the window border.
                handled = true;
                return HitTest.Nowhere.IntPtr();
            }
        }
    }
}
