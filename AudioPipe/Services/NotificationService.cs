using AudioPipe.Extensions;
using AudioPipe.Properties;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Collections.Generic;
using Windows.UI.Notifications;

namespace AudioPipe.Services
{
    public static class NotificationService
    {
        private const string ApplicationId = "AudioPipe";
        private const string ErrorImagePath = "NotifyError.png";

        private static bool _iconCreated = false;

        public static void NotifyError(string message)
        {
            if (!_iconCreated)
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
                            Source = $"file:///{System.IO.Directory.GetCurrentDirectory()}/{ErrorImagePath}",
                        }
                    }
                }
            };

            var toast = new ToastNotification(content.GetXml());
            ToastNotificationManager.CreateToastNotifier(ApplicationId).Show(toast);
        }

        private static void CreateErrorIcon()
        {
            // TODO: DPI aware?
            int imageSize = 44;
            int symbolSize = 30;

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
    }
}
