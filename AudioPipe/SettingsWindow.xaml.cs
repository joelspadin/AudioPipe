using System.Windows;

namespace AudioPipe
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        public void SetPage(Pages page)
        {
            Pivot.SelectedIndex = (int)page;
        }

        public enum Pages
        {
            Settings = 0,
            About = 1,
        }
    }
}
