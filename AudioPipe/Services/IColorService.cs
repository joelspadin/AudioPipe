using System;
using System.Windows.Media;

namespace AudioPipe.Services
{
    /// <summary>
    /// A service that provides named colors.
    /// </summary>
    public interface IColorService
    {
        /// <summary>
        /// Gets the color with the given name.
        /// </summary>
        /// <param name="colorName">The name of the accent color.</param>
        /// <returns>The color associated with the given name.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the given name is invalid.</exception>
        Color this[string colorName] { get; }
    }
}
