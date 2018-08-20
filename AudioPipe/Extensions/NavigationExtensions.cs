using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace AudioPipe.Extensions
{
    /// <summary>
    /// Extensions for <see cref="FrameworkElement"/> to add hyperlink navigation.
    /// </summary>
    public static class NavigationExtensions
    {
        /// <summary>
        /// Makes all Hyperlink descendants of an element open links in the
        /// default web browser.
        /// </summary>
        /// <param name="elem">The element for which hyperlinks should be enabled.</param>
        public static void EnableHyperlinks(this FrameworkElement elem)
        {
            elem.Loaded += HyperlinkContainer_Loaded;
        }

        private static void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            switch (e.Uri.HostNameType)
            {
                case UriHostNameType.Basic:
                case UriHostNameType.Dns:
                case UriHostNameType.IPv4:
                case UriHostNameType.IPv6:
                    Process.Start(e.Uri.ToString());
                    break;

                default:
                    Debug.WriteLine($"Unknown Uri type for {e.Uri}");
                    break;
            }
        }

        private static void HyperlinkContainer_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement root)
            {
                foreach (var link in root.FindLogicalDescendants<Hyperlink>())
                {
                    link.RequestNavigate -= Hyperlink_RequestNavigate;
                    link.RequestNavigate += Hyperlink_RequestNavigate;
                }
            }
        }
    }
}
