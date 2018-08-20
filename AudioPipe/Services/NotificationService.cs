using AudioPipe.Extensions;
using AudioPipe.Properties;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Windows.UI.Notifications;

namespace AudioPipe.Services
{
    /// <summary>
    /// Provides an interface to either UWP or Win32 notifications,
    /// depending on how the application is being run.
    /// </summary>
    public static class NotificationService
    {
        private const string ApplicationId = "AudioRedirect";
        private const string ErrorImageFileName = "NotifyError.png";

        private static string DataDirectory => Path.GetTempPath();

        private static string ErrorImagePath => Path.Combine(DataDirectory, ErrorImageFileName);

        private static bool IsUwp => new DesktopBridge.Helpers().IsRunningAsUwp();

        /// <summary>
        /// Displays an error notification with a given message.
        /// </summary>
        /// <param name="message">The text to display.</param>
        public static void NotifyError(string message)
        {
            if (IsUwp)
            {
                NotifyErrorUwp(message);
            }
            else
            {
                NotifyErrorWin32(message);
            }
        }

        private static void CreateErrorIcon()
        {
            // TODO: DPI aware?
            const int imageSize = 44;
            const int symbolSize = 30;

            var iconInfo = new IconService.IconInfo
            {
                Symbols = new List<IconService.SymbolInfo>
                {
                    new IconService.SymbolInfo
                    {
                        CharCode = (int)IconService.Symbol.IncidentTriangle,
                        Color = System.Drawing.Color.Yellow,
                    }
                },
                Background = System.Drawing.Color.Transparent,
                ImageSize = imageSize,
                SymbolSize = symbolSize,
            };

            using (var bitmap = IconService.CreateBitmap(iconInfo))
            {
                bitmap.Save(ErrorImagePath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private static void NotifyErrorUwp(string message)
        {
            if (!File.Exists(ErrorImagePath))
            {
                CreateErrorIcon();
            }

            var content = new ToastContent()
            {
                Duration = ToastDuration.Short,
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = Resources.ErrorTitleCannotCreatePipe,
                                HintMaxLines = 1,
                            },
                            new AdaptiveText()
                            {
                                Text = message,
                            }
                        },

                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = $"file:///{ErrorImagePath}",
                        }
                    }
                }
            };

            var toast = new ToastNotification(content.GetXml());
            ToastNotificationManager.CreateToastNotifier(ApplicationId).Show(toast);
        }

        private static void NotifyErrorWin32(string message)
        {
            MessageBox.Show(message, Resources.ErrorTitleCannotCreatePipe, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}
