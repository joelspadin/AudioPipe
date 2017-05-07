using AudioPipe.Services;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace AudioPipe.Extensions
{
    internal static class WindowExtensions
    {
        private const int ShiftAmount = 66;
        private const double ShowDuration = 0.2;
        private const double HideDuration = 0.1;

        //private static Storyboard storyboard = null;

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

        public static Matrix CalculateDpiFactors(this Window window)
        {
            var mainWindowPresentationSource = PresentationSource.FromVisual(window);
            return mainWindowPresentationSource == null ? new Matrix() { M11 = 1, M22 = 1 } : mainWindowPresentationSource.CompositionTarget.TransformToDevice;
        }

        public static double DpiHeightFactor(this Window window)
        {
            var m = CalculateDpiFactors(window);
            return m.M22;
        }

        public static double DpiWidthFactor(this Window window)
        {
            var m = CalculateDpiFactors(window);
            return m.M11;
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

    }
}