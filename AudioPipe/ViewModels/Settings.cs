using System.Windows;

namespace AudioPipe.ViewModels
{
    public class Settings : DependencyObject
    {
        public int Latency
        {
            get => (int)GetValue(LatencyProperty);
            set => SetValue(LatencyProperty, value);
        }

        // Using a DependencyProperty as the backing store for Latency.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LatencyProperty =
            DependencyProperty.Register(
                nameof(Latency),
                typeof(int),
                typeof(Settings),
                new PropertyMetadata(Properties.Settings.Default.Latency, OnLatencyChanged));

        public bool MuteSource
        {
            get => (bool)GetValue(MuteSourceProperty);
            set => SetValue(MuteSourceProperty, value);
        }

        // Using a DependencyProperty as the backing store for MuteSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MuteSourceProperty =
            DependencyProperty.Register(
                nameof(MuteSource),
                typeof(bool),
                typeof(Settings),
                new FrameworkPropertyMetadata(Properties.Settings.Default.MuteSource, OnMuteSourceChanged));

        private static void OnLatencyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            // Updating latency setting causes audio pipe to restart.
            // Don't update repeatedly. Use SliderValueChangedBehavior instead.
        }

        private static void OnMuteSourceChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var settings = (Settings)source;
            Properties.Settings.Default.MuteSource = settings.MuteSource;
        }
    }
}
