using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AudioPipe.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="ObservableCollection{T}"/>.
    /// </summary>
    public static class ObservableCollectionExtensions
    {
        /// <summary>
        /// Inserts an object into an <see cref="ObservableCollection{T}"/> sorted
        /// based on a comparison function.
        /// </summary>
        /// <typeparam name="T">The type of the collection.</typeparam>
        /// <param name="collection">The collection into which an item should be inserted.</param>
        /// <param name="item">The item to insert.</param>
        /// <param name="comparer">A comparison function determining the sort order.</param>
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
