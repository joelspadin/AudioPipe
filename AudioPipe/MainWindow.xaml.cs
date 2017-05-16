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
using AudioPipe.Extensions;
using AudioPipe.Services;
using AudioPipe.ViewModels;
using System.Diagnostics;
using CSCore.CoreAudioAPI;

namespace AudioPipe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double SettingsWindowPadding = 40;

        private readonly AppViewModel _viewModel;
        private readonly PipeManager _pipeManager = new PipeManager();

        private TrayIcon _trayIcon;
        private SettingsWindow _settingsWindow;

        public MainWindow()
        {
            InitializeComponent();
            CreateAndHideWindow();

            UpdateTheme();
            UpdateLatency();
            UpdateMuteSource();

            Properties.Settings.Default.PropertyChanged += Settings_PropertyChanged;
            DeviceService.DefaultCaptureDeviceChanged += DeviceService_DefaultCaptureDeviceChanged;

            _viewModel = new AppViewModel();
            DataContext = _viewModel;

            // If you open the tray icon's context menu before the window has
            // finished fully loading, it breaks the dispatcher. Defer it to later.
            Dispatcher.BeginInvoke(new Action(() =>
            {
                _trayIcon = new TrayIcon();
                _trayIcon.Invoked += TrayIcon_Invoked;
                _trayIcon.SettingsClicked += TrayIcon_SettingsClicked;
            }));
        }

        private void CreateAndHideWindow()
        {
            // Ensure the Win32 and WPF windows are created to fix first show issues with DPI Scaling
            Opacity = 0;
            Show();
            Hide();
            Opacity = 1;
        }

        private async Task ShowAsync()
        {
            if (Visibility != Visibility.Visible)
            {
                UpdateTheme();
                UpdateViewModel();
                UpdateWindowPosition();

                var scrollviewer = LayoutRoot.FindScrollViewer();

                Topmost = false;
                SizeToContent = SizeToContent.Manual;
                scrollviewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

                await this.ShowWithAnimation();

                Topmost = true;
                SizeToContent = SizeToContent.WidthAndHeight;
                scrollviewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
        }

        private async Task HideAsync()
        {
            if (Visibility == Visibility.Visible)
            {
                await this.HideWithAnimation();
            }
        }

        private void UpdateLatency()
        {
            // TODO: may need to throttle updates to prevent things from breaking.
            _pipeManager.Latency = Properties.Settings.Default.Latency;
        }

        private void UpdateMuteSource()
        {
            _pipeManager.MuteSource = Properties.Settings.Default.MuteSource;
        }

        private void UpdateViewModel()
        {
            _viewModel.Refresh();
            SelectDevice(_pipeManager.CurrentOutput);
        }

        private void SelectDevice(MMDevice device)
        {
            if (device == null)
            {
                _viewModel.SelectedDevice = _viewModel.Devices.FirstOrDefault(d => d.IsDefault);
            }
            else
            {
                _viewModel.SelectedDevice = _viewModel.Devices.FirstOrDefault(d => DeviceService.Equals(d.Device, device));
            }
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            _pipeManager.Dispose();
            _trayIcon.Dispose();
        }

        private async void Window_Deactivated(object sender, EventArgs e)
        {
            // TODO: find a better way to keep the window from reopening if you click down
            // on the tray icon (causes deactivate) and then release (causes invoke).
            var cursorPos = System.Windows.Forms.Control.MousePosition;
            if (!TaskbarService.GetNotificationAreaBounds().Contains(cursorPos))
            {
                await HideAsync();
            }
        }

        private async void TrayIcon_Invoked()
        {
            if (Visibility == Visibility.Visible)
            {
                await HideAsync();
            }
            else
            {
                await ShowAsync();
            }
        }

        private void TrayIcon_SettingsClicked()
        {
            ShowSettings(SettingsWindow.Pages.Settings);
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                await HideAsync();
            }
        }

        private void UpdateTheme()
        {
            var globalTheme = ThemeService.FindResourceDictionary(Application.Current.Resources, "Default.xaml");
            if (globalTheme != null)
            {
                ThemeService.UpdateThemeColors(globalTheme);
            }

            this.SetBlur(ThemeService.IsWindowTransparencyEnabled);
        }

        private Point GetWindowPosition(double width, double height, double padding)
        {
            double left;
            double top;

            var taskbarState = TaskbarService.GetTaskbarState();
            switch (taskbarState.TaskbarPosition)
            {
                case TaskbarPosition.Left:
                    left = (taskbarState.TaskbarBounds.Right / this.DpiWidthFactor()) + padding;
                    top = (taskbarState.TaskbarBounds.Bottom / this.DpiHeightFactor()) - width - padding;
                    break;
                case TaskbarPosition.Right:
                    left = (taskbarState.TaskbarBounds.Left / this.DpiWidthFactor()) - width - padding;
                    top = (taskbarState.TaskbarBounds.Bottom / this.DpiHeightFactor()) - height - padding;
                    break;
                case TaskbarPosition.Top:
                    left = (taskbarState.TaskbarBounds.Right / this.DpiWidthFactor()) - width - padding;
                    top = (taskbarState.TaskbarBounds.Bottom / this.DpiHeightFactor()) + padding;
                    break;
                case TaskbarPosition.Bottom:
                    left = (taskbarState.TaskbarBounds.Right / this.DpiWidthFactor()) - width - padding;
                    top = (taskbarState.TaskbarBounds.Top / this.DpiHeightFactor()) - height - padding;
                    break;
                default:
                    left = 0;
                    top = 0;
                    break;
            }

            return new Point(left, top);
        }

        private void UpdateWindowPosition()
        {
            LayoutRoot.UpdateLayout();
            LayoutRoot.Measure(new Size(double.PositiveInfinity, LayoutRoot.MaxHeight));
            Height = LayoutRoot.DesiredSize.Height;

            var position = GetWindowPosition(Width, Height, 0);
            Left = position.X;
            Top = position.Y;
        }

        private void LayoutRoot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _pipeManager.SetOutputDevice(_viewModel.SelectedDevice?.Device);
                _pipeManager.Start();
            }
            catch (PipeInitException ex)
            {
                NotificationService.NotifyError(ex.Message);
                Debug.WriteLine(ex.Message);

                _viewModel.SelectedDevice = _viewModel.Devices.FirstOrDefault(d => d.IsDefault);
            }

            UpdateTrayIcon();
        }

        private void DeviceService_DefaultCaptureDeviceChanged()
        {
            Dispatcher.Invoke(() =>
            {
                UpdateViewModel();
                UpdateTrayIcon();
            });
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Properties.Settings.Latency):
                    UpdateLatency();
                    break;

                case nameof(Properties.Settings.MuteSource):
                    UpdateMuteSource();
                    break;
            }

            Properties.Settings.Default.Save();
        }

        private void ShowSettings(SettingsWindow.Pages page)
        {
            if (_settingsWindow == null)
            {
                _settingsWindow = new SettingsWindow();
                _settingsWindow.Closed += SettingsWindow_Closed;

                var position = GetWindowPosition(_settingsWindow.Width, _settingsWindow.Height, SettingsWindowPadding);
                _settingsWindow.Left = position.X;
                _settingsWindow.Top = position.Y;
            }

            UpdateTheme();
            _settingsWindow.SetPage(page);
            _settingsWindow.Show();
        }

        private void SettingsWindow_Closed(object sender, EventArgs e)
        {
            _settingsWindow.Closed -= SettingsWindow_Closed;
            _settingsWindow = null;
        }

        private void UpdateTrayIcon()
        {
            var pipeActive = _pipeManager.CurrentOutput != DeviceService.DefaultCaptureDevice;
            _trayIcon.SetPipeActive(pipeActive);
        }
    }
}
