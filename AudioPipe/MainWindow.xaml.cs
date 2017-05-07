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

namespace AudioPipe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppViewModel _viewModel;


        private TrayIcon _trayIcon = new TrayIcon();
        private PipeManager _pipeManager = new PipeManager();

        public MainWindow()
        {
            InitializeComponent();

            _trayIcon.Invoked += TrayIcon_Invoked;

            _viewModel = new AppViewModel();
            DataContext = _viewModel;

            CreateAndHideWindow();
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

        private void UpdateViewModel()
        {
            _viewModel.Refresh();
            _viewModel.SelectedDevice = _viewModel.Devices.FirstOrDefault(d => DeviceService.Equals(d.Device, _pipeManager.CurrentOutput));
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

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                await HideAsync();
            }
        }

        private void UpdateTheme()
        {
            foreach (var dict in Resources.MergedDictionaries)
            {
                if (dict.Source.OriginalString.EndsWith("Brushes.xaml"))
                {
                    ThemeService.UpdateThemeResources(dict);
                }
            }

            this.SetBlur(ThemeService.IsWindowTransparencyEnabled);
        }

        private void UpdateWindowPosition()
        {
            LayoutRoot.UpdateLayout();
            LayoutRoot.Measure(new Size(double.PositiveInfinity, LayoutRoot.MaxHeight));
            Height = LayoutRoot.DesiredSize.Height;

            var taskbarState = TaskbarService.GetTaskbarState();
            switch (taskbarState.TaskbarPosition)
            {
                case TaskbarPosition.Left:
                    Left = (taskbarState.TaskbarBounds.Right / this.DpiWidthFactor());
                    Top = (taskbarState.TaskbarBounds.Bottom / this.DpiHeightFactor()) - Height;
                    break;
                case TaskbarPosition.Right:
                    Left = (taskbarState.TaskbarBounds.Left / this.DpiWidthFactor()) - Width;
                    Top = (taskbarState.TaskbarBounds.Bottom / this.DpiHeightFactor()) - Height;
                    break;
                case TaskbarPosition.Top:
                    Left = (taskbarState.TaskbarBounds.Right / this.DpiWidthFactor()) - Width;
                    Top = (taskbarState.TaskbarBounds.Bottom / this.DpiHeightFactor());
                    break;
                case TaskbarPosition.Bottom:
                    Left = (taskbarState.TaskbarBounds.Right / this.DpiWidthFactor()) - Width;
                    Top = (taskbarState.TaskbarBounds.Top / this.DpiHeightFactor()) - Height;
                    break;
            }
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

            var pipeActive = _pipeManager.CurrentOutput != DeviceService.GetDefaultCaptureDevice();
            _trayIcon.SetPipeActive(pipeActive);
        }
    }
}
