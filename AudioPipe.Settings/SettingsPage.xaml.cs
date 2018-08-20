using Windows.UI.Xaml.Controls;

namespace AudioPipe.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsPage"/> class.
        /// </summary>
        public SettingsPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the view model for the settings page.
        /// </summary>
        public Settings Settings { get; } = new Settings();
    }
}
