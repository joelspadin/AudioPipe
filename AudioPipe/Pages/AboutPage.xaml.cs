using AudioPipe.Extensions;
using System;
using System.Reflection;
using System.Windows.Controls;

namespace AudioPipe.Pages
{
    /// <summary>
    /// Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class AboutPage : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutPage"/> class.
        /// </summary>
        public AboutPage()
        {
            InitializeComponent();

            this.EnableHyperlinks();
        }

        /// <summary>
        /// Gets the application's assembly version.
        /// </summary>
        public Version AssemblyVersion => Assembly.GetEntryAssembly().GetName().Version;

        /// <summary>
        /// Gets a string describing the application's assembly version.
        /// </summary>
        public string VersionText => string.Format(Properties.Resources.Version, AssemblyVersion);
    }
}
