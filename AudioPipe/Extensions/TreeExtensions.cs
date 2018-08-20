using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace AudioPipe.Extensions
{
    /// <summary>
    /// Extension methods for finding elements in a logical or visual tree.
    /// </summary>
    public static class TreeExtensions
    {
        /// <summary>
        /// Finds all logical descendants of a <see cref="DependencyObject"/> type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of element to return.</typeparam>
        /// <param name="element">The element whose descendants should be returned.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of all logical descands of type <typeparamref name="T"/>.</returns>
        public static IEnumerable<T> FindLogicalDescendants<T>(this DependencyObject element)
            where T : DependencyObject
        {
            if (element != null)
            {
                foreach (var rawChild in LogicalTreeHelper.GetChildren(element))
                {
                    if (rawChild is DependencyObject child)
                    {
                        if (child is T foundChild)
                        {
                            yield return foundChild;
                        }

                        foreach (var childOfChild in FindLogicalDescendants<T>(child))
                        {
                            yield return childOfChild;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds the first visual descendant of a <see cref="Visual"/> of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of visual to return.</typeparam>
        /// <param name="element">The element whose descendant should be returned.</param>
        /// <returns>The first visual desendant of type <typeparamref name="T"/> or null if no such descendant exists.</returns>
        public static T FindVisualDescendant<T>(this Visual element)
            where T : Visual
        {
            if (element == null)
            {
                return null;
            }

            if (element is T thisElement)
            {
                return thisElement;
            }

            (element as FrameworkElement)?.ApplyTemplate();

            var childrenCount = VisualTreeHelper.GetChildrenCount(element);

            for (var i = 0; i < childrenCount; i++)
            {
                if (VisualTreeHelper.GetChild(element, i) is Visual visual)
                {
                    var foundElement = visual.FindVisualDescendant<T>();
                    if (foundElement != null)
                    {
                        return foundElement;
                    }
                }
            }

            return null;
        }
    }
}
