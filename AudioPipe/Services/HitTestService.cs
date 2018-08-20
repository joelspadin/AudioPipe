using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace AudioPipe.Services
{
    /// <summary>
    /// Handles the <see cref="MainWindow.NativeMethods.WM_NCHITTEST"/> message and prevents
    /// the aero glass border from acting as a resize handle.
    /// </summary>
    public static class HitTestService
    {
        private enum HitTest
        {
            Nowhere = 0,
            ClientArea = 1,
        }

        /// <summary>
        /// Handles the <see cref="MainWindow.NativeMethods.WM_NCHITTEST"/> message.
        /// </summary>
        /// <param name="window">The window to test.</param>
        /// <param name="lParam">The position to test, as a pointer to a { uint16 x; uint16 y; } struct.</param>
        /// <param name="handled">Set to whether the message was handled.</param>
        /// <returns>The rest of the hit test.</returns>
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

        /// <summary>
        /// Checks if the mouse position given by <paramref name="lParam"/> is
        /// inside <paramref name="window"/>.
        /// </summary>
        /// <param name="window">The window to test.</param>
        /// <param name="lParam">The position to test, as a pointer to a { uint16 x; uint16 y; } struct.</param>
        /// <returns>Whether the position in inside the window.</returns>
        public static bool IsMouseInWindow(Window window, IntPtr lParam)
        {
            var pos = new MousePosition(lParam);
            var origin = window.PointToScreen(new Point(0, 0));
            var windowPos = new Point(pos.X - origin.X, pos.Y - origin.Y);

            return windowPos.X >= 0 && windowPos.X < window.ActualWidth
                && windowPos.Y >= 0 && windowPos.Y < window.ActualHeight;
        }

        private static IntPtr IntPtr(this HitTest e)
        {
            return new IntPtr((int)e);
        }

        /// <summary>
        /// Mouse position structure.
        /// </summary>
        /// <remarks>
        /// <see cref="Value"/> overlaps with <see cref="X"/> and <see cref="Y"/>
        /// so that setting <see cref="Value"/> to the parameter to the
        /// <see cref="MainWindow.NativeMethods.WM_NCHITTEST"/> message assigns
        /// both <see cref="X"/> and <see cref="Y"/> from the packed parameter.
        /// </remarks>
        [StructLayout(LayoutKind.Explicit)]
        private struct MousePosition
        {
            [FieldOffset(0)]
            public uint Value;

            [FieldOffset(0)]
            public ushort X;

            [FieldOffset(2)]
            public ushort Y;

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
        }
    }
}
