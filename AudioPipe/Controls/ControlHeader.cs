using System.Windows;
using System.Windows.Controls;

namespace AudioPipe.Controls
{
    /// <summary>
    /// Wraps a control with a text header.
    /// </summary>
    public class ControlHeader : ContentControl
    {
        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(object), typeof(ControlHeader), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="HeaderTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(ControlHeader), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the header content.
        /// </summary>
        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        /// <summary>
        /// Gets or sets a data template for <see cref="Header"/>.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }
    }
}
