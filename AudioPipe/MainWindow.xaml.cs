using AudioPipe.Audio;
using AudioPipe.Extensions;
using AudioPipe.Services;
using AudioPipe.ViewModels;
using CSCore.CoreAudioAPI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace AudioPipe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window, IDisposable
    {
        private const double SettingsWindowPadding = 40;

        private readonly IAppViewModel _viewModel;
        private readonly PipeManager _pipeManager = new PipeManager();

        private TrayIcon _trayIcon;
        private SettingsWindow _settingsWindow;

        public MainWindow()
        {
            InitializeComponent();

            if (ColorService.IsLegacyTheme)
            {
                AllowsTransparency = false;
            }

            CreateAndHideWindow();

            UpdateTheme();
            UpdateLatency();
            UpdateMuteSource();

            Properties.Settings.Default.PropertyChanged += Settings_PropertyChanged;
            DeviceService.DefaultCaptureDeviceChanged += DeviceService_DevicesChanged;
            DeviceService.OutputDevicesChanged += DeviceService_DevicesChanged;

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

                if (ColorService.IsLegacyTheme)
                {
                    Show();
                }
                else
                {
                    var scrollviewer = LayoutRoot.FindVisualDescendant<ScrollViewer>();

                    Topmost = false;
                    SizeToContent = SizeToContent.Manual;
                    scrollviewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

                    await this.ShowWithAnimation();

                    Topmost = true;
                    SizeToContent = SizeToContent.WidthAndHeight;
                    scrollviewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                }
            }
        }

        private async Task HideAsync()
        {
            if (Visibility == Visibility.Visible)
            {
                if (ColorService.IsLegacyTheme)
                {
                    Hide();
                }
                else
                {
                    await this.HideWithAnimation();
                }
            }
        }

        private void UpdateLatency()
        {
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

        private async void TrayIcon_Invoked(object sender, EventArgs e)
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

        private void TrayIcon_SettingsClicked(object sender, EventArgs e)
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

        /// <summary>
        /// Updates whether the aero glass window border is shown.
        /// </summary>
        private void UpdateLegacyWindowBorder()
        {
            if (ColorService.IsLegacyTheme)
            {
                if (WindowFrameService.IsDwmCompositionEnabled)
                {
                    WindowFrameService.ShowGlassBorder(this);
                }
                else
                {
                    WindowFrameService.HideGlassBorder(this);
                }
            }
        }

        private Point GetWindowPosition(double width, double height, double padding)
        {
            double left;
            double top;
            double barOffset = 0;
            double edgeOffset = 0;

            if (ColorService.IsLegacyTheme)
            {
                if (WindowFrameService.IsDwmCompositionEnabled)
                {
                    barOffset = 3 * WindowFrameService.GlassBorderWidth;
                    edgeOffset = WindowFrameService.GlassBorderWidth;
                }
                else
                {
                    barOffset = 2 * BorderThickness.Left;
                    if (SystemParameters.HighContrast)
                    {
                        // For some reason the border changes horizontal positioning
                        // depending on high contrast mode.
                        edgeOffset = barOffset;
                    }
                }
            }

            var taskbarState = TaskbarService.GetTaskbarState();
            switch (taskbarState.TaskbarPosition)
            {
                case TaskbarPosition.Left:
                    left = (taskbarState.TaskbarBounds.Right / this.DpiWidthFactor()) + padding + barOffset;
                    top = (taskbarState.TaskbarBounds.Bottom / this.DpiHeightFactor()) - width - padding - edgeOffset;
                    break;
                case TaskbarPosition.Right:
                    left = (taskbarState.TaskbarBounds.Left / this.DpiWidthFactor()) - width - padding - barOffset;
                    top = (taskbarState.TaskbarBounds.Bottom / this.DpiHeightFactor()) - height - padding - edgeOffset;
                    break;
                case TaskbarPosition.Top:
                    left = (taskbarState.TaskbarBounds.Right / this.DpiWidthFactor()) - width - padding - edgeOffset;
                    top = (taskbarState.TaskbarBounds.Bottom / this.DpiHeightFactor()) + padding + barOffset;
                    break;
                case TaskbarPosition.Bottom:
                    left = (taskbarState.TaskbarBounds.Right / this.DpiWidthFactor()) - width - padding - edgeOffset;
                    top = (taskbarState.TaskbarBounds.Top / this.DpiHeightFactor()) - height - padding - barOffset;
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

        private void DeviceService_DevicesChanged(object sender, EventArgs e)
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

        public void Dispose()
        {
            _pipeManager.Dispose();
            if (_trayIcon != null)
            {
                _trayIcon.Dispose();
                _trayIcon = null;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            UpdateLegacyWindowBorder();

            // Install the Win32 message hook.
            var source = PresentationSource.FromVisual(this) as HwndSource;
            source?.AddHook(WindowHook);
        }

        /// <summary>
        /// Win32 message handler
        /// </summary>
        private IntPtr WindowHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeMethods.WM_DWMCOMPOSITIONCHANGED:
                    // Windows 7 aero glass enabled or disabled
                    UpdateLegacyWindowBorder();
                    handled = true;
                    break;

                case NativeMethods.WM_DWMCOLORIZATIONCOLORCHANGED:
                    if (ColorService.IsLegacyTheme)
                    {
                        // Windows 7 aero glass color changed
                        UpdateTheme();
                        handled = true;
                    }
                    break;

                case NativeMethods.WM_SETTINGCHANGE:
                    if (lParam != IntPtr.Zero && Marshal.PtrToStringAuto(lParam) == "ImmersiveColorSet")
                    {
                        // Windows 8+ theme changed
                        UpdateTheme();
                        handled = true;
                    }
                    break;

                case NativeMethods.WM_NCHITTEST:
                    return HitTestService.DoHitTest(this, lParam, out handled);
            }

            return IntPtr.Zero;
        }

        private static class NativeMethods
        {
            public const int WM_SETTINGCHANGE = 0x001A;
            public const int WM_NCHITTEST = 0x0084;
            public const int WM_DWMCOMPOSITIONCHANGED = 0x31E;
            public const int WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320;
        }
    }
}
