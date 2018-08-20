using System.Drawing;

namespace AudioPipe.Services
{
    /// <summary>
    /// Provides <see cref="Bitmap"/> and <see cref="Icon"/> objects from symbols.
    /// </summary>
    public static partial class IconService
    {
        /// <summary>
        /// Describes one symbol in an <see cref="IconInfo"/>.
        /// </summary>
        public struct SymbolInfo
        {
            /// <summary>
            /// The character code of a Segoe MDL2 Assets glyph.
            /// </summary>
            public int CharCode;

            /// <summary>
            /// The color of the symbol.
            /// </summary>
            public Color Color;

            /// <summary>
            /// Initializes a new instance of the <see cref="SymbolInfo"/> struct.
            /// </summary>
            /// <param name="charCode">The character code of a Segoe MDL2 Assets glyph.</param>
            public SymbolInfo(int charCode)
            {
                CharCode = charCode;
                Color = GetForegroundColor();
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SymbolInfo"/> struct.
            /// </summary>
            /// <param name="symbol">The character code of a Segoe MDL2 Assets glyph.</param>
            public SymbolInfo(Symbol symbol)
                : this((int)symbol)
            {
            }
        }
    }
}
