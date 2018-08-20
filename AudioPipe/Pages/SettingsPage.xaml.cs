using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AudioPipe.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        /// <summary>
        /// Identifies the <see cref="IsRunAtStartupEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsRunAtStartupEnabledProperty =
            DependencyProperty.Register(nameof(IsRunAtStartupEnabled), typeof(bool), typeof(SettingsPage), new PropertyMetadata(false));

        private const string StartupTaskName = "AudioRedirectTask";

        private readonly bool isUwp = new DesktopBridge.Helpers().IsRunningAsUwp();

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsPage"/> class.
        /// </summary>
        public SettingsPage()
        {
            LatencyChangedCommand = new LatencySettingCommand(Settings);
            InitializeComponent();

            Loaded += SettingsPage_Loaded;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application should run at Windows' startup.
        /// </summary>
        public bool IsRunAtStartupEnabled
        {
            get => (bool)GetValue(IsRunAtStartupEnabledProperty);
            set => SetValue(IsRunAtStartupEnabledProperty, value);
        }

        /// <summary>
        /// Gets a command that executes when <see cref="ViewModels.Settings.Latency"/> changes.
        /// </summary>
        public ICommand LatencyChangedCommand { get; }

        /// <summary>
        /// Gets the settings view model for the page.
        /// </summary>
        public ViewModels.Settings Settings { get; } = new ViewModels.Settings();

        private Microsoft.Win32.RegistryKey GetStartupRegistryKey()
        {
            return Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        }

        private async Task InitRunAtStartupUwp()
        {
            var task = await Windows.ApplicationModel.StartupTask.GetAsync(StartupTaskName);
            switch (task.State)
            {
                case Windows.ApplicationModel.StartupTaskState.Disabled:
                    RunAtStartupToggle.IsOn = false;
                    IsRunAtStartupEnabled = true;
                    break;

                case Windows.ApplicationModel.StartupTaskState.DisabledByUser:
                    RunAtStartupToggle.IsOn = false;
                    IsRunAtStartupEnabled = false;
                    break;

                case Windows.ApplicationModel.StartupTaskState.Enabled:
                    RunAtStartupToggle.IsOn = true;
                    IsRunAtStartupEnabled = true;
                    break;
            }
        }

        private void InitRunAtStartupWin32()
        {
            var key = GetStartupRegistryKey();
            var value = key.GetValue(StartupTaskName);

            RunAtStartupToggle.IsOn = value != null;
            IsRunAtStartupEnabled = true;
        }

        private async void RunAtStartupToggle_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            await ToggleRunAtStartup();
        }

        private async void RunAtStartupToggle_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            await ToggleRunAtStartup();
        }

        private async void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (isUwp)
            {
                await InitRunAtStartupUwp();
            }
            else
            {
                InitRunAtStartupWin32();
            }
        }

        private async Task ToggleRunAtStartup()
        {
            if (isUwp)
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
                        IsRunAtStartupEnabled = false;
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

        /// <summary>
        /// Command that executes when <see cref="ViewModels.Settings.Latency"/> changes.
        /// </summary>
        public class LatencySettingCommand : ICommand
        {
            private readonly ViewModels.Settings settings;

            /// <summary>
            /// Initializes a new instance of the <see cref="LatencySettingCommand"/> class.
            /// </summary>
            /// <param name="settings">Settings view model to which this command is attached.</param>
            public LatencySettingCommand(ViewModels.Settings settings)
            {
                this.settings = settings;
            }

            /// <summary>
            /// Occurs when <see cref="CanExecute(object)"/> changes.
            /// This command can always execute, so this will never fire.
            /// </summary>
            public event EventHandler CanExecuteChanged
            {
                add { }
                remove { }
            }

            /// <inheritdoc/>
            public bool CanExecute(object parameter) => true;

            /// <inheritdoc/>
            public void Execute(object parameter)
            {
                Properties.Settings.Default.Latency = settings.Latency;
            }
        }
    }
}
