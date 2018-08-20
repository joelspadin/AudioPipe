using System.Windows;
using System.Windows.Controls;

namespace AudioPipe.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="ItemsControl"/>.
    /// </summary>
    public static class ItemsControlExtensions
    {
        /// <summary>
        /// Finds the <see cref="ItemsControl"/>'s container for an item.
        /// </summary>
        /// <typeparam name="T">The type of the container.</typeparam>
        /// <param name="control">The <see cref="ItemsControl"/> to query.</param>
        /// <param name="item">The item whose container should be returned.</param>
        /// <returns><paramref name="item"/>'s container, if it exists and is of type <typeparamref name="T"/>, or null otherwise.</returns>
        public static T GetContainer<T>(this ItemsControl control, object item)
            where T : UIElement
        {
            object container = control.ItemContainerGenerator.ContainerFromItem(item);
            if (container == null)
            {
                var index = control.Items.IndexOf(item);
                if (index >= 0)
                {
                    container = control.Items[index];
                }
            }

            if (container is T foundContainer)
            {
                return foundContainer;
            }
            else
            {
                return default;
            }
        }
    }
}
