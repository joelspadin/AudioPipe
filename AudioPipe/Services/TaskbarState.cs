using System.Drawing;
using System.Runtime.InteropServices;

namespace AudioPipe.Services
{
    /// <summary>
    /// Describes the bounds and orientation of the taskbar.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TaskbarState
    {
        /// <summary>
        /// The side of the screen to which the taskbar is docked.
        /// </summary>
        public TaskbarPosition TaskbarPosition;

        /// <summary>
        /// The position and size of the taskbar on the screen.
        /// </summary>
        public Rectangle TaskbarBounds;
    }
}