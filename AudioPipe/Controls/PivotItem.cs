using System.Windows;
using System.Windows.Controls;

namespace AudioPipe.Controls
{
    public class PivotItem : ContentControl
    {
        public object Header
        {
            get => GetValue(PivotHeaderProperty);
            set => SetValue(PivotHeaderProperty, value);
        }

        public static readonly DependencyProperty PivotHeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(object), typeof(PivotItem),
                new FrameworkPropertyMetadata(null));
    }
}