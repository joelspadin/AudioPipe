using System.Windows;
using System.Windows.Controls;

namespace AudioPipe.Controls
{
    /// <summary>
    /// WPF port of <see cref="Windows.UI.Xaml.Controls.PivotItem"/>.
    /// </summary>
    public class PivotItem : ContentControl
    {
        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PivotHeaderProperty =
            DependencyProperty.Register(
                nameof(Header),
                typeof(object),
                typeof(PivotItem),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the header for the <see cref="PivotItem"/>.
        /// </summary>
        public object Header
        {
            get => GetValue(PivotHeaderProperty);
            set => SetValue(PivotHeaderProperty, value);
        }
    }
}