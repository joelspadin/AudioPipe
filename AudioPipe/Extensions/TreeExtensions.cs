using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace AudioPipe.Extensions
{
    public static class TreeExtensions
    {
        public static IEnumerable<T> FindLogicalDescendants<T>(this DependencyObject element) where T : DependencyObject
        {
            if (element != null)
            {
                foreach (var rawChild in LogicalTreeHelper.GetChildren(element))
                {
                    if (rawChild is DependencyObject)
                    {
                        var child = (DependencyObject)rawChild;
                        if (child is T)
                        {
                            yield return (T)child;
                        }

                        foreach (T childOfChild in FindLogicalDescendants<T>(child))
                        {
                            yield return childOfChild;
                        }
                    }
                }
            }
        }

        public static T FindVisualDescendant<T>(this Visual element) where T : Visual
        {
            if (element == null)
            {
                return null;
            }

            var e = element as T;

            if (e != null)
            {
                return e;
            }

            (element as FrameworkElement)?.ApplyTemplate();

            var childrenCount = VisualTreeHelper.GetChildrenCount(element);

            for (var i = 0; i < childrenCount; i++)
            {
                var visual = VisualTreeHelper.GetChild(element, i) as Visual;
                var foundElement = visual.FindVisualDescendant<T>();

                if (foundElement != null)
                {
                    return foundElement;
                }
            }

            return null;
        }
    }
}
