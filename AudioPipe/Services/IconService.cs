using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace AudioPipe.Services
{
    /// <summary>
    /// Provides <see cref="Bitmap"/> and <see cref="Icon"/> objects from symbols.
    /// </summary>
    public static partial class IconService
    {
        private const string SegoeSymbolFont = "Segoe MDL2 Assets";

        /// <summary>
        /// Pre-rendered icons to use on Windows 7.
        /// </summary>
        private static readonly Dictionary<int, string> LegacyIcons = new Dictionary<int, string>
        {
            [(int)Symbol.Headphone] = "Resources/Headphone.ico",
            [(int)Symbol.Speaker] = "Resources/Speaker.ico",
        };

        private static bool IsSymbolFontInstalled => new InstalledFontCollection().Families.Any(family => family.Name == SegoeSymbolFont);

        /// <summary>
        /// Creates a bitmap from one or more Segoe MDL2 Assets glyphs.
        /// </summary>
        /// <param name="icon">An object describing the icon to create.</param>
        /// <returns>The created bitmap.</returns>
        public static Bitmap CreateBitmap(IconInfo icon)
        {
            var bitmap = new Bitmap(icon.ImageSize, icon.ImageSize);
            var g = Graphics.FromImage(bitmap);
            g.Clear(icon.Background);
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

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
        /// Creates a bitmap using a Segoe MDL2 Assets glpyh.
        /// </summary>
        /// <param name="charCode">The character code of a Segoe MDL2 Assets character</param>
        /// <param name="imageSize">The size of the image in pixels</param>
        /// <param name="symbolSize">The size of the symbol in pixels. Use 0 to match the image size.</param>
        /// <returns>The created bitmap.</returns>
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

        /// <summary>
        /// Creates an icon from one or more Segoe MDL2 Assets glyphs.
        /// </summary>
        /// <param name="icon">An object describing the icon to create.</param>
        /// <returns>The created icon.</returns>
        public static Icon CreateIcon(IconInfo icon)
        {
            using (var bitmap = CreateBitmap(icon))
            {
                return CreateIconFromBitmap(bitmap);
            }
        }

        /// <summary>
        /// Creates an icon using a Segoe MDL2 Assets glyph.
        /// </summary>
        /// <param name="charCode">The character code of a Segoe MDL2 Assets character</param>
        /// <param name="imageSize">The size of the image in pixels</param>
        /// <param name="symbolSize">The size of the symbol in pixels. Use 0 to match the image size.</param>
        /// <returns>The created icon.</returns>
        public static Icon CreateIcon(int charCode, int imageSize = 16, int symbolSize = 0)
        {
            if (UseLegacyIcon(charCode, out var filename))
            {
                return Icon.ExtractAssociatedIcon(filename);
            }

            using (var bitmap = CreateBitmap(charCode, imageSize, symbolSize))
            {
                return CreateIconFromBitmap(bitmap);
            }
        }

        /// <summary>
        /// The the application's foreground color.
        /// </summary>
        /// <returns>The foreground color.</returns>
        public static Color GetForegroundColor()
        {
            var color = ColorService.GetColor("ApplicationTextDarkTheme");
            return Color.FromArgb(color.A, color.R, color.G, color.B);
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

        /// <summary>
        /// Checks whether there is a legacy icon for the given symbol.
        /// </summary>
        /// <param name="charCode">The character code of a Segoe MDL2 Assets character</param>
        /// <param name="filename">If successful, the filename of the icon.</param>
        /// <returns>true if successful</returns>
        private static bool UseLegacyIcon(int charCode, out string filename)
        {
            filename = null;
            if (IsSymbolFontInstalled)
            {
                return false;
            }

            return LegacyIcons.TryGetValue(charCode, out filename);
        }

        private static void Write(this BinaryWriter writer, ICONDIR dir)
        {
            writer.Write(dir.idReserved);
            writer.Write(dir.idType);
            writer.Write(dir.idCount);
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
    }
}
