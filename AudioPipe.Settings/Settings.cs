namespace AudioPipe.Settings
{
    /// <summary>
    /// View model for the settings page.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Gets or sets the pipe latency in milliseconds.
        /// </summary>
        public int Latency { get; set; } = 10;

        /// <summary>
        /// Gets or sets a value indicating whether the input device will
        /// be muted when the pipe is active.
        /// </summary>
        public bool MuteSource { get; set; } = true;
    }
}
