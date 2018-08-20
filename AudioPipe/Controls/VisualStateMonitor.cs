using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

#if DEBUG
namespace AudioPipe.Controls
{
    /// <summary>
    /// Debugging helper that periodically logs the visual state of a control.
    /// </summary>
    public class VisualStateMonitor : DependencyObject
    {
        /// <summary>
        /// Identifies the <see cref="Interval"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.RegisterAttached(
                nameof(Interval),
                typeof(int),
                typeof(VisualStateMonitor),
                new FrameworkPropertyMetadata(0, OnIntervalChanged));

        /// <summary>
        /// Gets or sets the logging interval in milliseconds.
        /// </summary>
        public int Interval
        {
            get => (int)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }

        private static void CheckVisualState(FrameworkElement element)
        {
            var groups = GetVisualStateGroups(element);
            if (groups != null)
            {
                foreach (var group in groups)
                {
                    Debug.WriteLine($"Element: {element.Name} -> Group: {group.Name} -> State: {group.CurrentState?.Name}");
                }
            }
        }

        private static IEnumerable<DependencyObject> GetDescendants(DependencyObject reference)
        {
            if (reference == null)
            {
                yield break;
            }

            var queue = new Queue<DependencyObject>();

            do
            {
                var parent = (queue.Count == 0) ? reference : queue.Dequeue();

                var count = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);
                    queue.Enqueue(child);

                    yield return child;
                }
            }
            while (queue.Count > 0);
        }

        private static IEnumerable<VisualStateGroup> GetVisualStateGroups(FrameworkElement element)
        {
            // If the ControlTemplate has not been applied yet
            if (VisualTreeHelper.GetChildrenCount(element) <= 0)
            {
                return null;
            }

            foreach (var descendant in GetDescendants(element).OfType<FrameworkElement>())
            {
                var groups = VisualStateManager.GetVisualStateGroups(descendant)?.Cast<VisualStateGroup>();
                if (groups?.Any() == true)
                {
                    return groups;
                }
            }

            return null;
        }

        private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                var interval = (int)e.NewValue;
                if (interval <= 0)
                {
                    return;
                }

                Task.Run(async () =>
                {
                    while (true)
                    {
                        element.Dispatcher.Invoke(() => CheckVisualState(element));

                        await Task.Delay(TimeSpan.FromSeconds(interval));
                    }
                });
            }
        }
    }
}

#endif