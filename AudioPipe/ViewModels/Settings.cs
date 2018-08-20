using AudioPipe.Audio;
using System.Windows;

namespace AudioPipe.ViewModels
{
    /// <summary>
    /// View model for user settings.
    /// </summary>
    public class Settings : DependencyObject
    {
        /// <summary>
        /// Identifies the <see cref="Latency"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LatencyProperty =
            DependencyProperty.Register(
                nameof(Latency),
                typeof(int),
                typeof(Settings),
                new PropertyMetadata(Properties.Settings.Default.Latency, OnLatencyChanged));

        /// <summary>
        /// Identifies the <see cref="MuteSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MuteSourceProperty =
            DependencyProperty.Register(
                nameof(MuteSource),
                typeof(bool),
                typeof(Settings),
                new FrameworkPropertyMetadata(Properties.Settings.Default.MuteSource, OnMuteSourceChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            Latency = Properties.Settings.Default.Latency;
            MuteSource = Properties.Settings.Default.MuteSource;
        }

        /// <summary>
        /// Gets or sets <see cref="Pipe"/> latency in milliseconds.
        /// </summary>
        public int Latency
        {
            get => (int)GetValue(LatencyProperty);
            set => SetValue(LatencyProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the input device will be
        /// muted when the <see cref="Pipe"/> is active.
        /// </summary>
        public bool MuteSource
        {
            get => (bool)GetValue(MuteSourceProperty);
            set => SetValue(MuteSourceProperty, value);
        }

        private static void OnLatencyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            // Updating the latency setting causes the audio pipe to restart.
            // Don't update repeatedly. Use SliderValueChangedBehavior instead.
        }

        private static void OnMuteSourceChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var settings = (Settings)source;
            Properties.Settings.Default.MuteSource = settings.MuteSource;
        }
    }
}
