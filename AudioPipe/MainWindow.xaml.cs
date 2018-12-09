using AudioPipe.Audio;
using AudioPipe.Extensions;
using AudioPipe.Services;
using AudioPipe.ViewModels;
using NAudio.CoreAudioApi;
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

        private readonly PipeManager pipeManager = new PipeManager();
        private readonly IAppViewModel viewModel;
        private SettingsWindow settingsWindow;
        private NotifyIcon trayIcon;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
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
            DeviceService.DefaultPlaybackDeviceChanged += DeviceService_DevicesChanged;
            DeviceService.OutputDevicesChanged += DeviceService_DevicesChanged;

            viewModel = new AppViewModel();
            DataContext = viewModel;

            // If you open the tray icon's context menu before the window has
            // finished fully loading, it breaks the dispatcher. Defer it to later.
            Dispatcher.BeginInvoke(new Action(() =>
            {
                trayIcon = new NotifyIcon();
                trayIcon.Invoked += TrayIcon_Invoked;
                trayIcon.HelpClicked += TrayIcon_HelpClicked;
                trayIcon.SettingsClicked += TrayIcon_SettingsClicked;
            }));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            pipeManager.Dispose();
            if (trayIcon != null)
            {
                trayIcon.Dispose();
                trayIcon = null;
            }
        }

        /// <inheritdoc/>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            UpdateLegacyWindowBorder();

            // Install the Win32 message hook.
            var source = PresentationSource.FromVisual(this) as HwndSource;
            source?.AddHook(WindowHook);
        }

        private void CreateAndHideWindow()
        {
            // Ensure the Win32 and WPF windows are created to fix first show issues with DPI Scaling
            Opacity = 0;
            Show();
            Hide();
            Opacity = 1;
        }

        private void DeviceService_DevicesChanged(object sender, EventArgs e)
        {
            // This must be done asynchronously, or UpdateViewModel() will deadlock
            // if it tries to remove the pipe's current device from the list, which
            // then causes the pipe manager to dispose an audio stream while still
            // in the audio device enumerator's devices changed event.
            Dispatcher.BeginInvoke(new Action(() =>
            {
                UpdateViewModel();
                UpdateWindowPosition();
                UpdateTrayIcon();
            }));
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

        private void LayoutRoot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                pipeManager.OutputDevice = viewModel.SelectedDevice?.Device;
                pipeManager.Start();
            }
            catch (PipeInitException ex)
            {
                NotificationService.NotifyError(ex.Message);
                Debug.WriteLine(ex.Message);

                viewModel.SelectedDevice = viewModel.Devices.FirstOrDefault(d => d.IsDefault);
            }

            UpdateTrayIcon();
        }

        private void SelectDevice(MMDevice device)
        {
            if (device == null)
            {
                viewModel.SelectedDevice = viewModel.Devices.FirstOrDefault(d => d.IsDefault);
            }
            else
            {
                viewModel.SelectedDevice = viewModel.Devices.FirstOrDefault(d => DeviceService.Equals(d.Device, device));
            }
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

        private void SettingsWindow_Closed(object sender, EventArgs e)
        {
            settingsWindow.Closed -= SettingsWindow_Closed;
            settingsWindow = null;
        }

        private async Task ShowAsync()
        {
            if (Visibility != Visibility.Visible)
            {
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

        private void ShowSettings(SettingsWindow.Pages page)
        {
            if (settingsWindow == null)
            {
                settingsWindow = new SettingsWindow();
                settingsWindow.Closed += SettingsWindow_Closed;

                var position = GetWindowPosition(settingsWindow.Width, settingsWindow.Height, SettingsWindowPadding);
                settingsWindow.Left = position.X;
                settingsWindow.Top = position.Y;
            }

            UpdateTheme();
            settingsWindow.SetPage(page);
            settingsWindow.Show();
        }

        private void TrayIcon_HelpClicked(object sender, EventArgs e)
        {
            ShowSettings(SettingsWindow.Pages.Help);
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

        private void UpdateLatency()
        {
            pipeManager.Latency = Properties.Settings.Default.Latency;
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

        private void UpdateMuteSource()
        {
            pipeManager.MuteInputWhenPiped = Properties.Settings.Default.MuteSource;
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

        private void UpdateTrayIcon()
        {
            var pipeActive = pipeManager.OutputDevice != DeviceService.DefaultPlaybackDevice;
            trayIcon.SetPipeActive(pipeActive);
        }

        private void UpdateViewModel()
        {
            viewModel.Refresh();
            SelectDevice(pipeManager.OutputDevice);
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

        private void Window_Closing(object sender, EventArgs e)
        {
            pipeManager.Dispose();
            trayIcon.Dispose();
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

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                await HideAsync();
            }
        }

        /// <summary>
        /// Win32 message handler
        /// </summary>
        /// <param name="hwnd">The window handle.</param>
        /// <param name="msg">The message ID.</param>
        /// <param name="wParam">The message's wParam value.</param>
        /// <param name="lParam">The message's lParam value.</param>
        /// <param name="handled">A value that indicates whether the message was handled. Set the value to true if the message was handled; otherwise, false.</param>
        /// <returns>The appropriate return value depends on the particular message. See the message documentation details for the Win32 message being handled.</returns>
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
            public const int WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320;
            public const int WM_DWMCOMPOSITIONCHANGED = 0x31E;
            public const int WM_NCHITTEST = 0x0084;
            public const int WM_SETTINGCHANGE = 0x001A;
        }
    }
}
