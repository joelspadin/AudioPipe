using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AudioPipe.Extensions
{
    public static class ItemsControlExtensions
    {
        public static T GetContainer<T>(this ItemsControl control, object item)
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

            if (container != null && container is T)
            {
                return (T)container;
            }
            else
            {
                return default(T);
            }
        }
    }
}
