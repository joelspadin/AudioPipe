using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using AudioPipe.Extensions;

namespace AudioPipe.Controls
{
    /// <summary>
    /// Panel which contains all the headers
    /// </summary>
    public class PivotHeaderPanel : ItemsControl
    {
        public event EventHandler HeaderSelected;

        private object _selectedHeader;

        public PivotHeaderPanel()
        {
            ItemContainerGenerator.StatusChanged += delegate
            {
                if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                {
                    UpdateSelectedHeader();
                }
            };
        }

        public void SelectHeader(object header)
        {
            _selectedHeader = header;
            UpdateSelectedHeader();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is PivotHeaderItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new PivotHeaderItem();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            var clickedItem = e.OriginalSource as FrameworkElement;
            if (clickedItem != null)
            {
                OnHeaderItemSelected(clickedItem.DataContext);
            }
        }

        private void OnHeaderItemSelected(object sender)
        {
            HeaderSelected?.Invoke(sender, new EventArgs());
        }

        private void UpdateSelectedHeader()
        {
            foreach (var item in ItemContainerGenerator.Items)
            {
                var container = this.GetContainer<PivotHeaderItem>(item);
                if (container != null)
                {
                    container.IsSelected = (item == _selectedHeader);
                }
            }
        }
    }
}