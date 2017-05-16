using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace AudioPipe
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        public ViewModels.Settings Settings { get; } = new ViewModels.Settings();

        public ICommand LatencyChangedCommand { get; }

        public SettingsPage()
        {
            LatencyChangedCommand = new LatencySettingCommand(Settings);
            InitializeComponent();
        }

        public class LatencySettingCommand : ICommand
        {
            // Command can always execute, so even will never fire.
            public event EventHandler CanExecuteChanged { add { } remove { } }

            private ViewModels.Settings _settings;

            public LatencySettingCommand(ViewModels.Settings settings)
            {
                _settings = settings;
            }

            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                Properties.Settings.Default.Latency = _settings.Latency;
            }
        }
    }
}
