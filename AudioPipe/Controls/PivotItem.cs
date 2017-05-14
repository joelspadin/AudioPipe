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
                new FrameworkPropertyMetadata(OnPivotHeaderChanged));


        private static void OnPivotHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = (PivotItem)d;
            var oldPivotHeader = e.OldValue;
            var newPivotHeader = item.Header;
            item.OnPivotHeaderChanged(oldPivotHeader, newPivotHeader);
        }

        protected void OnPivotHeaderChanged(object oldPivotHeader, object newPivotHeader)
        {
        }
    }
}