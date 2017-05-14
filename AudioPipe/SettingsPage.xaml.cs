using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            public event EventHandler CanExecuteChanged;

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
