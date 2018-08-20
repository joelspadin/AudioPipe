using System.Drawing;
using System.Runtime.InteropServices;

namespace AudioPipe.Services
{
#pragma warning disable SA1307 // Accessible fields must begin with upper-case letter
    /// <summary>
    /// Defines the coordinates of the upper-left and lower-right corners of a rectangle
    /// (https://msdn.microsoft.com/en-us/9439cb6c-f2f7-4c27-b1d7-8ddf16d81fe8).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        /// <summary>
        /// The x-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public int left;

        /// <summary>
        /// The y-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public int top;

        /// <summary>
        /// The x-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public int right;

        /// <summary>
        /// The y-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public int bottom;

        /// <summary>
        /// Converts the <see cref="RECT"/> struct to a <see cref="Rectangle"/>.
        /// </summary>
        /// <returns>The rectangle converted to a <see cref="Rectangle"/>.</returns>
        public Rectangle ToRectangle()
        {
            return new Rectangle(new Point(left, top), new Size(right - left, bottom - top));
        }
    }
#pragma warning restore SA1307 // Accessible fields must begin with upper-case letter
}