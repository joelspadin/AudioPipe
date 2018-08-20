using AudioPipe.Services;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace AudioPipe.Extensions
{
    /// <summary>
    /// Extensions for <see cref="Window"/> to change window accents such as
    /// borders and blur.
    /// </summary>
    internal static class WindowAccentExtensions
    {
        /// <summary>
        /// Enables or disables window frame blur.
        /// </summary>
        /// <param name="window">The window to change.</param>
        /// <param name="enabled">Whether blur should be enabled.</param>
        public static void SetBlur(this Window window, bool enabled)
        {
            NativeMethods.AccentState state;

            // Blur is not useful in high contrast mode
            if (enabled && !SystemParameters.HighContrast)
            {
                state = NativeMethods.AccentState.ACCENT_ENABLE_BLURBEHIND;
            }
            else
            {
                state = NativeMethods.AccentState.ACCENT_DISABLED;
            }

            SetAccentPolicy(window, state, GetAccentFlagsForTaskbarPosition());
        }

        private static NativeMethods.AccentFlags GetAccentFlagsForTaskbarPosition()
        {
            var flags = NativeMethods.AccentFlags.DrawAllBorders;

            switch (TaskbarService.GetTaskbarState().TaskbarPosition)
            {
                case TaskbarPosition.Top:
                    flags &= ~NativeMethods.AccentFlags.DrawTopBorder;
                    break;

                case TaskbarPosition.Bottom:
                    flags &= ~NativeMethods.AccentFlags.DrawBottomBorder;
                    break;

                case TaskbarPosition.Left:
                    flags &= ~NativeMethods.AccentFlags.DrawLeftBorder;
                    break;

                case TaskbarPosition.Right:
                    flags &= ~NativeMethods.AccentFlags.DrawRightBorder;
                    break;
            }

            return flags;
        }

        private static void SetAccentPolicy(Window window, NativeMethods.AccentState accentState, NativeMethods.AccentFlags accentFlags)
        {
            var windowHelper = new WindowInteropHelper(window);

            var accent = new NativeMethods.AccentPolicy
            {
                AccentState = accentState,
                AccentFlags = accentFlags
            };

            var structSize = Marshal.SizeOf(accent);
            var structPtr = Marshal.AllocHGlobal(structSize);

            try
            {
                Marshal.StructureToPtr(accent, structPtr, false);

                var data = new NativeMethods.WindowCompositionAttribData
                {
                    Attribute = NativeMethods.WindowCompositionAttribute.WCA_ACCENT_POLICY,
                    SizeOfData = structSize,
                    Data = structPtr
                };

                NativeMethods.SetWindowCompositionAttribute(windowHelper.Handle, ref data);
            }
            finally
            {
                Marshal.FreeHGlobal(structPtr);
            }
        }

        private static class NativeMethods
        {
            [Flags]
            internal enum AccentFlags
            {
                // ...
                DrawLeftBorder = 0x20,

                DrawTopBorder = 0x40,
                DrawRightBorder = 0x80,
                DrawBottomBorder = 0x100,
                DrawAllBorders = DrawLeftBorder | DrawTopBorder | DrawRightBorder | DrawBottomBorder

                // ...
            }

            internal enum AccentState
            {
                ACCENT_DISABLED = 0,
                ACCENT_ENABLE_GRADIENT = 1,
                ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
                ACCENT_ENABLE_BLURBEHIND = 3,
                ACCENT_INVALID_STATE = 4
            }

            internal enum WindowCompositionAttribute
            {
                // ...
                WCA_ACCENT_POLICY = 19

                // ...
            }

            [DllImport("user32.dll")]
            internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttribData data);

            [StructLayout(LayoutKind.Sequential)]
            internal struct AccentPolicy
            {
                public AccentState AccentState;
                public AccentFlags AccentFlags;
                public int GradientColor;
                public int AnimationId;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct WindowCompositionAttribData
            {
                public WindowCompositionAttribute Attribute;
                public IntPtr Data;
                public int SizeOfData;
            }
        }
    }
}
