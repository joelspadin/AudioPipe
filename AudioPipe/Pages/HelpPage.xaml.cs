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
    public partial class HelpPage : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HelpPage"/> class.
        /// </summary>
        public HelpPage()
        {
            InitializeComponent();

            this.EnableHyperlinks();
        }
    }
}
