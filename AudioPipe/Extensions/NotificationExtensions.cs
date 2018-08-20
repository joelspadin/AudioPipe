using Microsoft.Toolkit.Uwp.Notifications;
using Windows.Data.Xml.Dom;

namespace AudioPipe.Extensions
{
    /// <summary>
    /// Extensions for UWP notifications.
    /// </summary>
    public static class NotificationExtensions
    {
        /// <summary>
        /// Gets an XML document describing a <see cref="ToastContent"/>.
        /// </summary>
        /// <param name="content">The toast to get.</param>
        /// <returns>The content as an <see cref="XmlDocument"/>.</returns>
        public static XmlDocument GetXml(this ToastContent content)
        {
            var xml = new XmlDocument();
            xml.LoadXml(content.GetContent());
            return xml;
        }
    }
}
