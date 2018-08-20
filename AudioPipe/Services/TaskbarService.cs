using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AudioPipe.Services
{
    /// <summary>
    /// Provides access to the position and style of the taskbar.
    /// </summary>
    public static class TaskbarService
    {
        private const string NotifyClassName = "TrayNotifyWnd";
        private const string TaskbarClassName = "Shell_TrayWnd";

        private enum ABEdge
        {
            ABE_LEFT = 0,
            ABE_TOP = 1,
            ABE_RIGHT = 2,
            ABE_BOTTOM = 3
        }

        private enum ABMsg
        {
            ABM_NEW = 0,
            ABM_REMOVE,
            ABM_QUERYPOS,
            ABM_SETPOS,
            ABM_GETSTATE,
            ABM_GETTASKBARPOS,
            ABM_ACTIVATE,
            ABM_GETAUTOHIDEBAR,
            ABM_SETAUTOHIDEBAR,
            ABM_WINDOWPOSCHANGED,
            ABM_SETSTATE
        }

        /// <summary>
        /// Gets the position and size of the notification area.
        /// </summary>
        /// <returns>The bounds of the notification area.</returns>
        public static Rectangle GetNotificationAreaBounds()
        {
            NativeMethods.GetWindowRect(FindNotificationArea(), out var rect);

            // TODO: is this high DPI aware?
            return rect.ToRectangle();
        }

        /// <summary>
        /// Gets the position and size of the taskbar.
        /// </summary>
        /// <returns>The bounds of the taskbar.</returns>
        public static Rectangle GetTaskbarBounds()
        {
            var appbar = default(APPBARDATA);
            var hwnd = FindTaskbar();

            appbar.cbSize = Marshal.SizeOf(appbar);
            appbar.uEdge = 0;
            appbar.hWnd = hwnd;
            appbar.lParam = 1;

            NativeMethods.GetWindowRect(hwnd, out var scaledTaskbarRect);

            var taskbarNonDPIAwareSize = NativeMethods.SHAppBarMessage((int)ABMsg.ABM_GETTASKBARPOS, ref appbar);

            var scalingAmount = (double)(scaledTaskbarRect.bottom - scaledTaskbarRect.top) / (appbar.rc.bottom - appbar.rc.top);

            var taskbarRect = default(RECT);
            taskbarRect.top = (int)(appbar.rc.top * scalingAmount);
            taskbarRect.bottom = (int)(appbar.rc.bottom * scalingAmount);
            taskbarRect.left = (int)(appbar.rc.left * scalingAmount);
            taskbarRect.right = (int)(appbar.rc.right * scalingAmount);

            return taskbarRect.ToRectangle();
        }

        /// <summary>
        /// Gets the bounds and orientation of the taskbar.
        /// </summary>
        /// <returns>A <see cref="TaskbarState"/> describing the taskbar.</returns>
        public static TaskbarState GetTaskbarState()
        {
            var retState = new TaskbarState
            {
                TaskbarBounds = GetTaskbarBounds(),
                TaskbarPosition = TaskbarPosition.Bottom
            };

            var screen = Screen.AllScreens.FirstOrDefault(x => x.Bounds.Contains(retState.TaskbarBounds));
            if (screen != null)
            {
                if (retState.TaskbarBounds.Bottom == screen.Bounds.Bottom && retState.TaskbarBounds.Top == screen.Bounds.Top)
                {
                    retState.TaskbarPosition = (retState.TaskbarBounds.Left == screen.Bounds.Left) ? TaskbarPosition.Left : TaskbarPosition.Right;
                }

                if (retState.TaskbarBounds.Right == screen.Bounds.Right && retState.TaskbarBounds.Left == screen.Bounds.Left)
                {
                    retState.TaskbarPosition = (retState.TaskbarBounds.Top == screen.Bounds.Top) ? TaskbarPosition.Top : TaskbarPosition.Bottom;
                }
            }

            return retState;
        }

        /// <summary>
        /// Gets whether a <see cref="TaskbarPosition"/> is on the leading edge (top or left) of the screen.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>Whether <paramref name="position"/> is on the leading edge of the screen.</returns>
        public static bool IsLeadingEdge(TaskbarPosition position)
        {
            return position == TaskbarPosition.Top || position == TaskbarPosition.Left;
        }

        /// <summary>
        /// Gets whether a <see cref="TaskbarPosition"/> uses a vertical taskbar.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>Whether <paramref name="position"/> uses a vertical taskbar.</returns>
        public static bool IsVertical(TaskbarPosition position)
        {
            return position == TaskbarPosition.Left || position == TaskbarPosition.Right;
        }

        private static IntPtr FindNotificationArea()
        {
            IntPtr taskbarHandle = FindTaskbar();
            if (taskbarHandle == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            return NativeMethods.FindWindowEx(taskbarHandle, IntPtr.Zero, NotifyClassName, null);
        }

        private static IntPtr FindTaskbar()
        {
            return NativeMethods.FindWindow(TaskbarClassName, null);
        }

#pragma warning disable SA1307 // Accessible fields must begin with upper-case letter
        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public int cbSize; // initialize this field using: Marshal.SizeOf(typeof(APPBARDATA));
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public uint uEdge;
            public RECT rc;
            public int lParam;
        }
#pragma warning restore SA1307 // Accessible fields must begin with upper-case letter

        private static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

            [DllImport("shell32.dll")]
            public static extern IntPtr SHAppBarMessage(uint dwMessage, [In] ref APPBARDATA pData);
        }
    }
}