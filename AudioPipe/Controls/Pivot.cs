using AudioPipe.Extensions;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AudioPipe.Controls
{
    /// <summary>
    /// WPF implementation of <see cref="Windows.UI.Xaml.Controls.Pivot"/>.
    /// </summary>
    public class Pivot : ItemsControl
    {
        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(PivotHeaderPanel), typeof(Pivot), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="HeaderTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(Pivot), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="SelectedIndex"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                nameof(SelectedIndex),
                typeof(int),
                typeof(Pivot),
                new FrameworkPropertyMetadata(-1, OnSelectedIndexChanged));

        /// <summary>
        /// Identifies the <see cref="SelectedItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(Pivot), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="Pivot"/> class.
        /// </summary>
        public Pivot()
        {
            Header = new PivotHeaderPanel();
            Header.HeaderSelected += Header_HeaderSelected;
            UpdateSelectedIndex();
        }

        /// <summary>
        /// Gets or sets the panel which contains the headers of all <see cref="PivotItem"/> children.
        /// </summary>
        public PivotHeaderPanel Header
        {
            get => (PivotHeaderPanel)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        /// <summary>
        /// Gets or sets the template for the <see cref="PivotItem.Header"/> property of <see cref="PivotItem"/> children.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the zero-based index of the currently selected item in the <see cref="Pivot"/>.
        /// </summary>
        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        /// <summary>
        /// Gets or sets the currently selected item in the <see cref="Pivot"/>.
        /// </summary>
        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        /// <inheritdoc/>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new PivotItem();
        }

        /// <inheritdoc/>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is PivotItem;
        }

        /// <inheritdoc/>
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

        private void Header_HeaderSelected(object sender, EventArgs e)
        {
            // Identify the PivotItem to which the newly-selected header belongs.
            for (int i = 0; i < Items.Count; i++)
            {
                var container = this.GetContainer<PivotItem>(Items[i]);

                if (container?.Header == sender)
                {
                    SelectedIndex = i;
                    SelectedItem = Items[i];
                    break;
                }
            }

            UpdateSelectedHeader();
        }

        private void UpdateSelectedHeader()
        {
            var container = this.GetContainer<PivotItem>(SelectedItem);
            Header.SelectHeader(container?.Header);
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
    }
}
