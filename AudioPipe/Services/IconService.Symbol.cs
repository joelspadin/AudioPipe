using System.Drawing;

namespace AudioPipe.Services
{
    /// <summary>
    /// Provides <see cref="Bitmap"/> and <see cref="Icon"/> objects from symbols.
    /// </summary>
    public static partial class IconService
    {
        /// <summary>
        /// Glyphs from https://docs.microsoft.com/en-us/windows/uwp/style/segoe-ui-symbol-font
        /// </summary>
        public enum Symbol
        {
            /// <summary>
            /// Bluetooth icon.
            /// </summary>
            Bluetooth = 0xE702,

            /// <summary>
            /// TV monitor icon
            /// </summary>
            TVMonitor = 0xE7F4,

            /// <summary>
            /// Box speaker icon
            /// </summary>
            Speaker = 0xE7F5,

            /// <summary>
            /// Headphones icon
            /// </summary>
            Headphone = 0xE7F6,

            /// <summary>
            /// Game controller icon
            /// </summary>
            Game = 0xE7FC,

            /// <summary>
            /// Triangle with exclamation point icon
            /// </summary>
            IncidentTriangle = 0xE814,

            /// <summary>
            /// Musical notes icon
            /// </summary>
            Audio = 0xE8D6,

            /// <summary>
            /// Wireless streaming icon
            /// </summary>
            Streaming = 0xE93E,

            /// <summary>
            /// Headset with microphone icon
            /// </summary>
            Headset = 0xE95B,

            /// <summary>
            /// Circle with X icon
            /// </summary>
            Error = 0xEA39,

            /// <summary>
            /// Filled circle icon
            /// </summary>
            CircleFill = 0xEA3B,
        }
    }
}
