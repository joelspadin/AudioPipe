using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.UI.Popups;

namespace AudioPipe.Settings
{
    public class AppServiceBridge
    {
        public static AppServiceBridge Instance { get; } = new AppServiceBridge();

        private AppServiceConnection _connection;
        private BackgroundTaskDeferral _appServiceDeferrral;

        private AppServiceBridge()
        {
            // TODO: split this out into an out-of-process background task that launches the main AudioPipe app.
        }

        public async Task LaunchProcess()
        {
            try
            {
                await Windows.ApplicationModel.FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message);
                await dialog.ShowAsync();
            }
        }

        public void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            var details = args.TaskInstance.TriggerDetails as AppServiceTriggerDetails;

            if (details != null)
            {
                Debug.WriteLine("Background Activated!");

                _appServiceDeferrral = args.TaskInstance.GetDeferral();
                args.TaskInstance.Canceled += OnTaskCanceled;

                _connection = details.AppServiceConnection;
            }
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (_appServiceDeferrral != null)
            {
                _appServiceDeferrral.Complete();
            }
        }
    }
}
