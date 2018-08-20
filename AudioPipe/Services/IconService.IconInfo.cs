using System.Collections.Generic;
using System.Drawing;

namespace AudioPipe.Services
{
    /// <summary>
    /// Provides <see cref="Bitmap"/> and <see cref="Icon"/> objects from symbols.
    /// </summary>
    public static partial class IconService
    {
        /// <summary>
        /// Describes one or more Segoe MDL2 Assets glyphs to layer to create an icon.
        /// </summary>
        public struct IconInfo
        {
            /// <summary>
            /// The icon's background color.
            /// </summary>
            public Color Background;

            /// <summary>
            /// The size, in pixels, of the image.
            /// </summary>
            public int ImageSize;

            /// <summary>
            /// List of glyphs to layer.
            /// </summary>
            public List<SymbolInfo> Symbols;

            /// <summary>
            /// The size, in pixels of the glyph(s).
            /// </summary>
            public int SymbolSize;

            /// <summary>
            /// Initializes a new instance of the <see cref="IconInfo"/> struct.
            /// </summary>
            /// <param name="size">Size, in pixels, of the icon.</param>
            public IconInfo(int size = 16)
            {
                Symbols = new List<SymbolInfo>();
                Background = Color.Transparent;
                ImageSize = size;
                SymbolSize = size;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="IconInfo"/> struct
            /// with a single glyph of the application's foreground color.
            /// </summary>
            /// <param name="charCode">The character code of a Segoe MDL2 Assets character</param>
            /// <param name="size">The size of the glyph in pixels</param>
            public IconInfo(int charCode, int size = 16)
                : this(size)
            {
                Symbols.Add(new SymbolInfo(charCode));
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="IconInfo"/> struct
            /// with a single glyph of the application's foreground color.
            /// </summary>
            /// <param name="symbol">The character code of a Segoe MDL2 Assets character</param>
            /// <param name="size">The size of the glyph in pixels</param>
            public IconInfo(Symbol symbol, int size = 16)
                : this((int)symbol, size)
            {
            }
        }
    }
}
