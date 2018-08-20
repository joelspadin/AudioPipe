using AudioPipe.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AudioPipe.Extensions
{
    /// <summary>
    /// Extensions for <see cref="Window"/>.
    /// </summary>
    internal static class WindowExtensions
    {
        private const double HideDuration = 0.1;
        private const int ShiftAmount = 66;
        private const double ShowDuration = 0.2;

        /// <summary>
        /// Gets a matrix describing the 2D scaling factors for the screen's DPI.
        /// </summary>
        /// <param name="window">The window to query.</param>
        /// <returns>A matrix describing the 2D scaling factors for the screen's DPI.</returns>
        public static Matrix CalculateDpiFactors(this Window window)
        {
            var mainWindowPresentationSource = PresentationSource.FromVisual(window);
            return mainWindowPresentationSource?.CompositionTarget.TransformToDevice ?? new Matrix() { M11 = 1, M22 = 1 };
        }

        /// <summary>
        /// Gets the height factor of <see cref="CalculateDpiFactors(Window)"/>.
        /// </summary>
        /// <param name="window">The window to query.</param>
        /// <returns>The height factor of <see cref="CalculateDpiFactors(Window)"/>.</returns>
        public static double DpiHeightFactor(this Window window)
        {
            var m = CalculateDpiFactors(window);
            return m.M22;
        }

        /// <summary>
        /// Gets the width factor of <see cref="CalculateDpiFactors(Window)"/>.
        /// </summary>
        /// <param name="window">The window to query.</param>
        /// <returns>The width factor of <see cref="CalculateDpiFactors(Window)"/>.</returns>
        public static double DpiWidthFactor(this Window window)
        {
            var m = CalculateDpiFactors(window);
            return m.M11;
        }

        /// <summary>
        /// Hides the window with an animation of sliding into the taskbar.
        /// </summary>
        /// <param name="window">The window to hide.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task HideWithAnimation(this Window window)
        {
            var duration = new Duration(TimeSpan.FromSeconds(HideDuration));
            var ease = new ExponentialEase { EasingMode = EasingMode.EaseIn };

            var fadeAnimation = new DoubleAnimation
            {
                Duration = duration,
                FillBehavior = FillBehavior.HoldEnd,
                EasingFunction = ease
            };

            fadeAnimation.From = 1;
            fadeAnimation.To = 0;

            var storyboard = new Storyboard();
            storyboard.Children.Add(fadeAnimation);

            Storyboard.SetTarget(fadeAnimation, window);
            Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath("(Opacity)"));

            await PlayStoryboard(storyboard, window);

            window.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Shows the window with an animation of sliding from the taskbar.
        /// </summary>
        /// <param name="window">The window to show.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task ShowWithAnimation(this Window window)
        {
            var originalTop = window.Top;
            var originalLeft = window.Left;
            var originalWidth = window.Width;
            var originalHeight = window.Height;

            window.Visibility = Visibility.Visible;
            window.Activate();

            var duration = new Duration(TimeSpan.FromSeconds(ShowDuration));
            var ease = new ExponentialEase { EasingMode = EasingMode.EaseOut };

            var moveAnimation = new DoubleAnimation
            {
                Duration = duration,
                FillBehavior = FillBehavior.HoldEnd,
                EasingFunction = ease,
            };
            var clipAnimation = new DoubleAnimation
            {
                Duration = duration,
                FillBehavior = FillBehavior.HoldEnd,
                EasingFunction = new AntiExponentialEase(ease),
            };
            var fadeAnimation = new DoubleAnimation
            {
                Duration = duration,
                FillBehavior = FillBehavior.HoldEnd,
                EasingFunction = ease,
            };

            fadeAnimation.From = 0;
            fadeAnimation.To = 1;

            var taskbarPosition = TaskbarService.GetTaskbarState().TaskbarPosition;

            if (TaskbarService.IsVertical(taskbarPosition))
            {
                moveAnimation.To = window.Left;
                clipAnimation.To = window.ActualWidth;
            }
            else
            {
                moveAnimation.To = window.Top;
                clipAnimation.To = window.ActualHeight;
            }

            if (TaskbarService.IsLeadingEdge(taskbarPosition))
            {
                moveAnimation.From = moveAnimation.To - ShiftAmount;
            }
            else
            {
                moveAnimation.From = moveAnimation.To + ShiftAmount;
            }

            clipAnimation.From = clipAnimation.To - ShiftAmount;

            var storyboard = new Storyboard();
            storyboard.Children.Add(moveAnimation);
            storyboard.Children.Add(clipAnimation);
            storyboard.Children.Add(fadeAnimation);

            Storyboard.SetTarget(fadeAnimation, window);
            Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath("(Opacity)"));

            if (TaskbarService.IsVertical(taskbarPosition))
            {
                Storyboard.SetTarget(moveAnimation, window);
                Storyboard.SetTargetProperty(moveAnimation, new PropertyPath("(Left)"));

                Storyboard.SetTarget(clipAnimation, window);
                Storyboard.SetTargetProperty(clipAnimation, new PropertyPath("(Width)"));
            }
            else
            {
                Storyboard.SetTarget(moveAnimation, window);
                Storyboard.SetTargetProperty(moveAnimation, new PropertyPath("(Top)"));

                Storyboard.SetTarget(clipAnimation, window);
                Storyboard.SetTargetProperty(clipAnimation, new PropertyPath("(Height)"));
            }

            await PlayStoryboard(storyboard, window);

            window.Top = originalTop;
            window.Left = originalLeft;
            window.Width = originalWidth;
            window.Height = originalHeight;

            window.Focus();
        }

        private static async Task PlayStoryboard(Storyboard board, FrameworkElement containingObject)
        {
            var tcs = new TaskCompletionSource<bool>();
            EventHandler onCompleted = (s, e) => tcs.SetResult(true);
            board.Completed += onCompleted;

            board.Begin(containingObject, HandoffBehavior.SnapshotAndReplace, true);
            await tcs.Task;

            board.Completed -= onCompleted;
            board.Remove(containingObject);
        }

        private class AntiExponentialEase : ExponentialEase
        {
            public AntiExponentialEase()
            {
            }

            public AntiExponentialEase(ExponentialEase baseEase)
            {
                Exponent = baseEase.Exponent;
                EasingMode = baseEase.EasingMode;
            }

            protected override double EaseInCore(double normalizedTime)
            {
                return 1 - base.EaseInCore(normalizedTime);
            }
        }
    }
}