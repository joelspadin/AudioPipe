using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;

namespace AudioPipe.Extensions
{
    public static class NotificationExtensions
    {
        public static XmlDocument GetXml(this ToastContent content)
        {
            var xml = new XmlDocument();
            xml.LoadXml(content.GetContent());
            return xml;
        }
    }
}
