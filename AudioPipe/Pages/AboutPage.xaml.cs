using AudioPipe.Extensions;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace AudioPipe.Pages
{
    /// <summary>
    /// Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class AboutPage : UserControl
    {
        public Version AssemblyVersion => Assembly.GetEntryAssembly().GetName().Version;
        public string VersionText => string.Format(Properties.Resources.Version, AssemblyVersion);

        public AboutPage()
        {
            InitializeComponent();

            Loaded += AboutPage_Loaded;
        }

        private void AboutPage_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var link in this.FindLogicalDescendants<Hyperlink>())
            {
                link.RequestNavigate += Hyperlink_RequestNavigate;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}
