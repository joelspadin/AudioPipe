using AudioPipe.Extensions;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace AudioPipe.Controls
{
    /// <summary>
    /// Panel which contains all the headers of a <see cref="Pivot"/>.
    /// </summary>
    public class PivotHeaderPanel : ItemsControl
    {
        private object selectedHeader;

        /// <summary>
        /// Initializes a new instance of the <see cref="PivotHeaderPanel"/> class.
        /// </summary>
        public PivotHeaderPanel()
        {
            ItemContainerGenerator.StatusChanged += (object sender, EventArgs e) =>
            {
                if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                {
                    UpdateSelectedHeader();
                }
            };
        }

        /// <summary>
        /// Occurs when a header item is selected.
        /// </summary>
        public event EventHandler HeaderSelected;

        /// <summary>
        /// Changes the selected header of the <see cref="Pivot"/>.
        /// </summary>
        /// <param name="header">The header to select.</param>
        public void SelectHeader(object header)
        {
            selectedHeader = header;
            UpdateSelectedHeader();
        }

        /// <inheritdoc/>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new PivotHeaderItem();
        }

        /// <inheritdoc/>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is PivotHeaderItem;
        }

        /// <inheritdoc/>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (e.OriginalSource is FrameworkElement clickedItem)
            {
                OnHeaderItemSelected(clickedItem.DataContext);
            }
        }

        private void OnHeaderItemSelected(object sender)
        {
            HeaderSelected?.Invoke(sender, EventArgs.Empty);
        }

        private void UpdateSelectedHeader()
        {
            foreach (var item in ItemContainerGenerator.Items)
            {
                var container = this.GetContainer<PivotHeaderItem>(item);
                if (container != null)
                {
                    container.IsSelected = item == selectedHeader;
                }
            }
        }
    }
}