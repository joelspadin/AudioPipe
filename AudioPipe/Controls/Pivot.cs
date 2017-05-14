using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using AudioPipe.Extensions;

namespace AudioPipe.Controls
{
    public class Pivot : ItemsControl
    {
        public PivotHeaderPanel Header
        {
            get => (PivotHeaderPanel)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(PivotHeaderPanel), typeof(Pivot), new PropertyMetadata(null));

        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        // Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(Pivot), new PropertyMetadata(null));

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(Pivot), new PropertyMetadata(null));

        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        // Using a DependencyProperty as the backing store for SelectedIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                nameof(SelectedIndex),
                typeof(int),
                typeof(Pivot),
                new FrameworkPropertyMetadata(-1, OnSelectedIndexChanged));




        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is PivotItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new PivotItem();
        }



        public Pivot()
        {
            Header = new PivotHeaderPanel();
            Header.HeaderSelected += Header_HeaderSelected;
            UpdateSelectedIndex();
        }

        private void UpdateSelectedIndex()
        {
            if (Items.Count > 0)
            {
                if (SelectedIndex < 0)
                {
                    SelectedIndex = 0;
                }

                SelectedItem = Items[SelectedIndex];
            }
            else
            {
                SelectedItem = null;
            }

            UpdateSelectedHeader();
        }

        private void UpdateSelectedHeader()
        {
            var container = this.GetContainer<PivotItem>(SelectedItem);
            Header.SelectHeader(container?.Header);
        }

        private void Header_HeaderSelected(object sender, EventArgs e)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var container = this.GetContainer<PivotItem>(Items[i]);

                if (container != null && container.Header == sender)
                {
                    SelectedIndex = i;
                    SelectedItem = Items[i];
                    break;
                }
            }

            UpdateSelectedHeader();
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            Header.Items.Clear();
            foreach (var item in Items.OfType<PivotItem>())
            {
                Header.Items.Add(item.Header);
            }

            UpdateSelectedIndex();
        }

        private static void OnSelectedIndexChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var pivot = (Pivot)sender;
            pivot.UpdateSelectedIndex();
        }
    }
}
