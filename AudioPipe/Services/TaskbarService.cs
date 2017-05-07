using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AudioPipe.Services
{
    public static class TaskbarService
    {
        private const string TaskbarClassName = "Shell_TrayWnd";
        private const string NotifyClassName = "TrayNotifyWnd";

        public static TaskbarState GetTaskbarState()
        {
            var retState = new TaskbarState();
            retState.TaskbarBounds = GetTaskbarBounds();
            retState.TaskbarPosition = TaskbarPosition.Bottom;

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

        public static Rectangle GetTaskbarBounds()
        {
            var ABD = new APPBARDATA();
            var hwnd = FindTaskbar();

            ABD.cbSize = Marshal.SizeOf(ABD);
            ABD.uEdge = 0;
            ABD.hWnd = hwnd;
            ABD.lParam = 1;

            User32.GetWindowRect(hwnd, out var scaledTaskbarRect);

            var taskbarNonDPIAwareSize = Shell32.SHAppBarMessage((int)ABMsg.ABM_GETTASKBARPOS, ref ABD);

            var scalingAmount = (double)(scaledTaskbarRect.bottom - scaledTaskbarRect.top) / (ABD.rc.bottom - ABD.rc.top);

            var taskbarRect = default(RECT);
            taskbarRect.top = (int)(ABD.rc.top * scalingAmount);
            taskbarRect.bottom = (int)(ABD.rc.bottom * scalingAmount);
            taskbarRect.left = (int)(ABD.rc.left * scalingAmount);
            taskbarRect.right = (int)(ABD.rc.right * scalingAmount);

            return taskbarRect.ToRectangle();
        }

        public static Rectangle GetNotificationAreaBounds()
        {
            User32.GetWindowRect(FindNotificationArea(), out var rect);
            // TODO: is this high DPI aware?
            return rect.ToRectangle();
        }

        public static bool IsVertical(TaskbarPosition position)
        {
            return position == TaskbarPosition.Left || position == TaskbarPosition.Right;
        }

        public static bool IsLeadingEdge(TaskbarPosition position)
        {
            return position == TaskbarPosition.Top || position == TaskbarPosition.Left;
        }

        private static IntPtr FindTaskbar()
        {
            return User32.FindWindow(TaskbarClassName, null);
        }

        private static IntPtr FindNotificationArea()
        {
            IntPtr taskbarHandle = FindTaskbar();
            if (taskbarHandle == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            return User32.FindWindowEx(taskbarHandle, IntPtr.Zero, NotifyClassName, null);
        }
    }

    public static class User32
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public Rectangle ToRectangle()
        {
            return new Rectangle(new Point(left, top), new Size(right - left, bottom - top));
        }
    }

    public static class Shell32
    {
        [DllImport("shell32.dll")]
        public static extern IntPtr SHAppBarMessage(uint dwMessage, [In] ref APPBARDATA pData);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct APPBARDATA
    {
        public int cbSize; // initialize this field using: Marshal.SizeOf(typeof(APPBARDATA));
        public IntPtr hWnd;
        public uint uCallbackMessage;
        public uint uEdge;
        public RECT rc;
        public int lParam;
    }

    public enum ABMsg
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

    public enum ABEdge
    {
        ABE_LEFT = 0,
        ABE_TOP = 1,
        ABE_RIGHT = 2,
        ABE_BOTTOM = 3
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TaskbarState
    {
        public TaskbarPosition TaskbarPosition;
        public Rectangle TaskbarBounds;
    }

    public enum TaskbarPosition
    {
        Top,
        Left,
        Right,
        Bottom
    }
}