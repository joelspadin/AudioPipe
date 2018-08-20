using System.Drawing;
using System.Runtime.InteropServices;

namespace AudioPipe.Services
{
    /// <summary>
    /// Provides <see cref="Bitmap"/> and <see cref="Icon"/> objects from symbols.
    /// </summary>
    public static partial class IconService
    {
#pragma warning disable SA1307 // Accessible fields must begin with upper-case letter
        [StructLayout(LayoutKind.Sequential)]
        private struct ICONDIR
        {
                              /// <summary>
                              /// Reserved (must be 0).
                              /// </summary>
            public readonly ushort idReserved;

            /// <summary>
            /// Resource type (1 for icons)..
            /// </summary>
            public readonly ushort idType;

            /// <summary>
            /// Number of images in the icon.
            /// </summary>
            public ushort idCount;

            public ICONDIR(ushort count)
            {
                idReserved = 0;
                idType = 1;
                idCount = count;
            }
        }
#pragma warning restore SA1307 // Accessible fields must begin with upper-case letter
    }
}
