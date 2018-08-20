using System;
using System.Diagnostics;
using System.Windows;
using VioletTape.WpfExceptionViewer;

namespace AudioPipe
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            if (!Debugger.IsAttached)
            {
                RegisterGlobalExceptionHandling();
            }
        }

        private void RegisterGlobalExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => ShowException(e.ExceptionObject as Exception);

            // DispatcherUnhandledException += (sender, e) => ShowException(e.Exception);
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (sender, e) => ShowException(e.Exception);
        }

        private void ShowException(Exception ex)
        {
            var viewer = new ExceptionViewer("An unhandled exception occurred.", ex, MainWindow)
            {
                Title = "Audio Redirect"
            };
            viewer.ShowDialog();
        }
    }
}
