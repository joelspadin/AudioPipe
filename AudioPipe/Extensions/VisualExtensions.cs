using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AudioPipe.Extensions
{
    public static class VisualExtensions
    {
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

        public static ScrollViewer FindScrollViewer(this ListBox listbox)
        {
            return listbox.FindVisualDescendant<ScrollViewer>();
        }
    }
}
