using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace AudioPipe.Services
{
    public static class IconService
    {
        private const string SegoeSymbolFont = "Segoe MDL2 Assets";

        public static Bitmap CreateBitmap(IconInfo icon)
        {
            var bitmap = new Bitmap(icon.ImageSize, icon.ImageSize);
            var g = Graphics.FromImage(bitmap);
            g.Clear(icon.Background);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            foreach (var symbol in icon.Symbols)
            {
                var text = char.ConvertFromUtf32(symbol.CharCode);
                var font = new Font(SegoeSymbolFont, icon.SymbolSize, FontStyle.Regular, GraphicsUnit.Pixel);
                var brush = new SolidBrush(symbol.Color);

                var format = new StringFormat(StringFormat.GenericTypographic);
                var offset = (icon.ImageSize - icon.SymbolSize) / 2;
                var origin = new PointF(offset, offset);

                g.DrawString(text, font, brush, origin, format);
            }

            return bitmap;
        }

        /// <summary>
        /// Creates a bitmap using a Segoe MDL2 Assets character.
        /// </summary>
        /// <param name="charCode">The character code of a Segoe MDL2 Assets character</param>
        /// <param name="imageSize">The size of the image in pixels</param>
        /// <param name="symbolSize">The size of the symbol in pixels. Use 0 to match the image size.</param>
        /// <returns></returns>
        public static Bitmap CreateBitmap(int charCode, int imageSize = 16, int symbolSize = 0)
        {
            if (symbolSize == 0)
            {
                symbolSize = imageSize;
            }

            return CreateBitmap(new IconInfo
            {
                Symbols = new List<SymbolInfo>
                {
                    new SymbolInfo(charCode),
                },
                Background = Color.Transparent,
                ImageSize = imageSize,
                SymbolSize = symbolSize,
            });
        }

        public static Icon CreateIcon(IconInfo icon)
        {
            using (var bitmap = CreateBitmap(icon))
            {
                return CreateIconFromBitmap(bitmap);
            }
        }

        /// <summary>
        /// Creates an icon using a Segoe MDL2 Assets character.
        /// </summary>
        /// <param name="charCode">The character code of a Segoe MDL2 Assets character</param>
        /// <param name="imageSize">The size of the image in pixels</param>
        /// <param name="symbolSize">The size of the symbol in pixels. Use 0 to match the image size.</param>
        /// <returns></returns>
        public static Icon CreateIcon(int charCode, int imageSize = 16, int symbolSize = 0)
        {
            using (var bitmap = CreateBitmap(charCode, imageSize, symbolSize))
            {
                return CreateIconFromBitmap(bitmap);
            }
        }

        private static Icon CreateIconFromBitmap(Bitmap bitmap)
        {
            if (bitmap.Width != bitmap.Height)
            {
                throw new ArgumentException($"Cannot create icon of size ({bitmap.Width}, {bitmap.Height}). Bitmap must be square.", nameof(bitmap));
            }
            if (bitmap.Width < 16)
            {
                throw new ArgumentException("Bitmap size must be >= 16", nameof(bitmap));
            }
            if (bitmap.Width > 256)
            {
                throw new ArgumentException("Bitmap size must be <= 256", nameof(bitmap));
            }

            byte[] png;
            using (var fs = new MemoryStream())
            {
                bitmap.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                fs.Position = 0;
                png = fs.ToArray();
            }

            using (var fs = new MemoryStream())
            {
                // .ico format begins with a header indicating the number of frames,
                // then one entry header per frame. Image data follows at a location
                // specified by the entry header's offset.
                var header = new ICONDIR(1);
                var offset = Marshal.SizeOf(typeof(ICONDIR)) + Marshal.SizeOf(typeof(ICONDIRENTRY));
                var entry = new ICONDIRENTRY(bitmap.Width, bitmap.Height, png.Length, offset);

                using (var writer = new BinaryWriter(fs))
                {
                    writer.Write(header);
                    writer.Write(entry);
                    writer.Write(png);

                    fs.Position = 0;
                    return new Icon(fs);
                }
            }
        }

        public static Color GetForegroundColor()
        {
            var color = AccentColorService.ActiveSet["ApplicationTextDarkTheme"];
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        // https://docs.microsoft.com/en-us/windows/uwp/style/segoe-ui-symbol-font
        public enum Symbol
        {
            Speaker = 0xE7F5,
            Headphone = 0xE7F6,
            IncidentTriangle = 0xE814,
            Error = 0xEA39,
            CircleFill = 0xEA3B,
        }

        public struct SymbolInfo
        {
            public int CharCode;
            public Color Color;

            public SymbolInfo(int charCode)
            {
                CharCode = charCode;
                Color = GetForegroundColor();
            }

            public SymbolInfo(Symbol symbol)
                : this((int)symbol)
            {
            }
        }

        public struct IconInfo
        {
            public List<SymbolInfo> Symbols;
            public Color Background;
            public int ImageSize;
            public int SymbolSize;

            public IconInfo(int size = 16)
            {
                Symbols = new List<SymbolInfo>();
                Background = Color.Transparent;
                ImageSize = size;
                SymbolSize = size;
            }

            public IconInfo(int charCode, int size = 16)
                : this(size)
            {
                Symbols.Add(new SymbolInfo(charCode));
            }

            public IconInfo(Symbol symbol, int size = 16)
                : this((int)symbol, size)
            {
            }
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern int SetTextCharacterExtra(
            IntPtr hdc,    // DC handle
            int nCharExtra // extra-space value
        );

        #region Icon Structures
        [StructLayout(LayoutKind.Sequential)]
        private struct ICONDIR
        {
            public readonly ushort idReserved;  // Reserved (must be 0)
            public readonly ushort idType;      // Resource type (1 for icons)
            public ushort idCount;              // How many images?

            public ICONDIR(ushort count)
            {
                idReserved = 0;
                idType = 1;
                idCount = count;
            }
        }

        private static void Write(this BinaryWriter writer, ICONDIR dir)
        {
            writer.Write(dir.idReserved);
            writer.Write(dir.idType);
            writer.Write(dir.idCount);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ICONDIRENTRY
        {
            public byte bWidth;                 // Width, in pixels, of the image
            public byte bHeight;                // Height, in pixels, of the image
            public byte bColorCount;            // Number of colors in image (0 if >= 8bpp)
            public readonly byte bReserved;     // Reserved (must be 0)
            public ushort wPlanes;              // Color planes
            public ushort wBitCount;            // Bits per pixel
            public uint dwBytesInRes;           // How many bytes in this resource?
            public readonly uint dwImageOffset; // Where in the file is this image?

            public ICONDIRENTRY(int width, int height, int pngBytes, int offsetBytes)
            {
                bWidth = Convert.ToByte(width >= 256 ? 0 : width);
                bHeight = Convert.ToByte(height >= 256 ? 0 : height);
                bColorCount = 0;
                bReserved = 0;
                wPlanes = 1;
                wBitCount = 24; // 8-bit RGBA
                dwBytesInRes = Convert.ToUInt32(pngBytes);
                dwImageOffset = Convert.ToUInt16(offsetBytes);// 22;
            }
        }

        private static void Write(this BinaryWriter writer, ICONDIRENTRY entry)
        {
            writer.Write(entry.bWidth);
            writer.Write(entry.bHeight);
            writer.Write(entry.bColorCount);
            writer.Write(entry.bReserved);
            writer.Write(entry.wPlanes);
            writer.Write(entry.wBitCount);
            writer.Write(entry.dwBytesInRes);
            writer.Write(entry.dwImageOffset);
        }
        #endregion
    }
}
