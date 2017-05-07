using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPipe.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static void InsertSorted<T>(this ObservableCollection<T> collection, T item, IComparer<T> comparer)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                if (comparer.Compare(item, collection[i]) < 0)
                {
                    collection.Insert(i, item);
                    return;
                }
            }

            collection.Add(item);
        }
    }
}
