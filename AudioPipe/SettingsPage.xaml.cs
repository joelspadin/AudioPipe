using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AudioPipe
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        private const string StartupTaskName = "AudioRedirectTask";

        public ViewModels.Settings Settings { get; } = new ViewModels.Settings();

        public ICommand LatencyChangedCommand { get; }

        public bool RunAtStartupEnabled
        {
            get => (bool)GetValue(RunAtStartupEnabledProperty);
            set => SetValue(RunAtStartupEnabledProperty, value);
        }

        // Using a DependencyProperty as the backing store for RunAtStartupEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RunAtStartupEnabledProperty =
            DependencyProperty.Register(nameof(RunAtStartupEnabled), typeof(bool), typeof(SettingsPage), new PropertyMetadata(false));

        private readonly bool _isUwp = new DesktopBridge.Helpers().IsRunningAsUwp();

        public SettingsPage()
        {
            LatencyChangedCommand = new LatencySettingCommand(Settings);
            InitializeComponent();

            Loaded += SettingsPage_Loaded;
        }

        private async void SettingsPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_isUwp)
            {
                await InitRunAtStartupUwp();
            }
            else
            {
                InitRunAtStartupWin32();
            }
        }

        private async void RunAtStartupToggle_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            await ToggleRunAtStartup();
        }

        private async void RunAtStartupToggle_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            await ToggleRunAtStartup();
        }

        private async Task InitRunAtStartupUwp()
        {
            var task = await Windows.ApplicationModel.StartupTask.GetAsync(StartupTaskName);
            switch (task.State)
            {
                case Windows.ApplicationModel.StartupTaskState.Disabled:
                    RunAtStartupToggle.IsOn = false;
                    RunAtStartupEnabled = true;
                    break;

                case Windows.ApplicationModel.StartupTaskState.DisabledByUser:
                    RunAtStartupToggle.IsOn = false;
                    RunAtStartupEnabled = false;
                    break;

                case Windows.ApplicationModel.StartupTaskState.Enabled:
                    RunAtStartupToggle.IsOn = true;
                    RunAtStartupEnabled = true;
                    break;
            }
        }

        private void InitRunAtStartupWin32()
        {
            var key = GetStartupRegistryKey();
            var value = key.GetValue(StartupTaskName);

            RunAtStartupToggle.IsOn = value != null;
            RunAtStartupEnabled = true;
        }

        private async Task ToggleRunAtStartup()
        {
            if (_isUwp)
            {
                await ToggleRunAtStartupUwp();
            }
            else
            {
                ToggleRunAtStartupWin32();
            }
        }

        private async Task ToggleRunAtStartupUwp()
        {
            var task = await Windows.ApplicationModel.StartupTask.GetAsync(StartupTaskName);
            if (task.State == Windows.ApplicationModel.StartupTaskState.Enabled)
            {
                task.Disable();
                RunAtStartupToggle.IsOn = false;
            }
            else
            {
                switch (await task.RequestEnableAsync())
                {
                    case Windows.ApplicationModel.StartupTaskState.DisabledByUser:
                        RunAtStartupToggle.IsOn = false;
                        RunAtStartupEnabled = false;
                        break;

                    case Windows.ApplicationModel.StartupTaskState.Enabled:
                        RunAtStartupToggle.IsOn = true;
                        break;
                }
            }
        }

        private void ToggleRunAtStartupWin32()
        {
            var key = GetStartupRegistryKey();
            var value = key.GetValue(StartupTaskName);

            if (value == null)
            {
                key.SetValue(StartupTaskName, System.Reflection.Assembly.GetExecutingAssembly().Location);
                RunAtStartupToggle.IsOn = true;
            }
            else
            {
                key.DeleteValue(StartupTaskName);
                RunAtStartupToggle.IsOn = false;
            }
        }

        private Microsoft.Win32.RegistryKey GetStartupRegistryKey()
        {
            return Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
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
