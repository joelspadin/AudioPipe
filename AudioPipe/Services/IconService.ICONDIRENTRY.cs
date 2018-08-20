using System;
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
#pragma warning disable SA1214 // Readonly fields must appear before non-readonly fields
        [StructLayout(LayoutKind.Sequential)]
        private struct ICONDIRENTRY
        {
            /// <summary>
            /// Width, in pixels, of the image.
            /// </summary>
            public byte bWidth;

            /// <summary>
            /// Height, in pixels, of the image.
            /// </summary>
            public byte bHeight;

            /// <summary>
            /// Number of colors in the image (0 if >= 8bpp)
            /// </summary>
            public byte bColorCount;

            /// <summary>
            /// Reserved (must be 0).
            /// </summary>
            public readonly byte bReserved;

            /// <summary>
            /// Color planes.
            /// </summary>
            public ushort wPlanes;

            /// <summary>
            /// Bits per pixel.
            /// </summary>
            public ushort wBitCount;

            /// <summary>
            /// How many bytes in this resource?
            /// </summary>
            public uint dwBytesInRes;

            /// <summary>
            /// Where in the file is this image?
            /// </summary>
            public readonly uint dwImageOffset;

            public ICONDIRENTRY(int width, int height, int pngBytes, int offsetBytes)
            {
                bWidth = Convert.ToByte(width >= 256 ? 0 : width);
                bHeight = Convert.ToByte(height >= 256 ? 0 : height);
                bColorCount = 0;
                bReserved = 0;
                wPlanes = 1;
                wBitCount = 24; // 8-bit RGBA
                dwBytesInRes = Convert.ToUInt32(pngBytes);
                dwImageOffset = Convert.ToUInt16(offsetBytes);
            }
        }
#pragma warning restore SA1214 // Readonly fields must appear before non-readonly fields
#pragma warning restore SA1307 // Accessible fields must begin with upper-case letter
    }
}
