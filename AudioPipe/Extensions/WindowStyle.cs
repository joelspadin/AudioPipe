using AudioPipe.Services;
using System;
using System.Windows;

namespace AudioPipe.Extensions
{
    /// <summary>
    /// Adds an attachable property to <see cref="Window"/> to control whether the
    /// window icon is shown.
    /// </summary>
    public class WindowStyle : DependencyObject
    {
        /// <summary>
        /// Identifies the <see cref="ShowIcon"/> attached dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowIconProperty =
            DependencyProperty.RegisterAttached(
                nameof(ShowIcon),
                typeof(bool),
                typeof(WindowStyle),
                new FrameworkPropertyMetadata(true, new PropertyChangedCallback((d, _) => UpdateIcon(d))));

        /// <summary>
        /// Gets or sets a value indicating whether a window's icon should be shown.
        /// </summary>
        public bool ShowIcon
        {
            get => (bool)GetValue(ShowIconProperty);
            set => SetValue(ShowIconProperty, value);
        }

        /// <summary>
        /// Gets the <see cref="ShowIcon"/> attached property.
        /// </summary>
        /// <param name="obj">The object to get.</param>
        /// <returns>The <see cref="ShowIcon"/> property of the object.</returns>
        public static bool GetShowIcon(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowIconProperty);
        }

        /// <summary>
        /// Sets the <see cref="ShowIcon"/> attached property.
        /// </summary>
        /// <param name="obj">The object to set.</param>
        /// <param name="value">The new value.</param>
        public static void SetShowIcon(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowIconProperty, value);
        }

        private static void UpdateIcon(DependencyObject obj)
        {
            if (GetShowIcon(obj))
            {
                // Not implemented.
            }
            else
            {
                if (obj is Window window)
                {
                    window.SourceInitialized += (object sender, EventArgs e) =>
                    {
                        WindowFrameService.HideIcon(window);
                    };
                }
            }
        }
    }
}
