using System.Windows;

namespace AudioPipe
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsWindow"/> class.
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Enumeration of settings pages.
        /// </summary>
        public enum Pages
        {
            /// <summary>
            /// The main settings page.
            /// </summary>
            Settings = 0,

            /// <summary>
            /// A page with information about the application.
            /// </summary>
            About = 1,

            /// <summary>
            /// A page with information on how to use the application.
            /// </summary>
            Help = 2,
        }

        /// <summary>
        /// Changes the active page.
        /// </summary>
        /// <param name="page">The page to activate.</param>
        public void SetPage(Pages page)
        {
            Pivot.SelectedIndex = (int)page;
        }
    }
}
