using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AudioPipe.Controls
{
    // [TemplateVisualState(GroupName = "SelectionStates", Name = "Disabled")]
    // [TemplateVisualState(GroupName = "SelectionStates", Name = "UnselectedLockled")]

    /// <summary>
    /// Represents one header of a <see cref="Pivot"/>.
    /// </summary>
    [TemplateVisualState(GroupName = "SelectionStates", Name = "Unselected")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "UnselectedPointerOver")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "UnselectedPressed")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "Selected")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "SelectedPointerOver")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "SelectedPressed")]
    [TemplateVisualState(GroupName = "FocusStates", Name = "Unfocused")]
    [TemplateVisualState(GroupName = "FocusStates", Name = "Focused")]
    public class PivotHeaderItem : ContentControl
    {
        private bool isPressed;
        private bool isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="PivotHeaderItem"/> class.
        /// </summary>
        public PivotHeaderItem()
        {
            UpdateFocusState();
            UpdateSelectionState();
        }

        private enum SelectionStates
        {
            Unselected,
            UnselectedPointerOver,
            UnselectedPressed,
            Selected,
            SelectedPointerOver,
            SelectedPressed,
        }

        /// <summary>
        /// Gets or sets a value indicating whether the header is currently being pressed.
        /// </summary>
        internal bool IsPressed
        {
            get => isPressed;
            set
            {
                if (value != isPressed)
                {
                    isPressed = value;
                    UpdateSelectionState();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this header is for the selected
        /// item in a <see cref="Pivot"/>.
        /// </summary>
        internal bool IsSelected
        {
            get => isSelected;
            set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                    UpdateSelectionState();
                }
            }
        }

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateFocusState();
            UpdateDefaultStyle();
            UpdateSelectionState();
        }

        /// <inheritdoc/>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            UpdateFocusState();
        }

        /// <inheritdoc/>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            UpdateFocusState();
        }

        /// <inheritdoc/>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            UpdateSelectionState();
        }

        /// <inheritdoc/>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            UpdateSelectionState();
        }

        /// <inheritdoc/>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            IsPressed = true;
        }

        /// <inheritdoc/>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            IsPressed = false;
        }

        /// <inheritdoc/>
        protected override void OnStylusDown(StylusDownEventArgs e)
        {
            base.OnStylusDown(e);
            IsPressed = true;
        }

        /// <inheritdoc/>
        protected override void OnStylusEnter(StylusEventArgs e)
        {
            base.OnStylusEnter(e);
            UpdateSelectionState();
        }

        /// <inheritdoc/>
        protected override void OnStylusLeave(StylusEventArgs e)
        {
            base.OnStylusLeave(e);
            UpdateSelectionState();
        }

        /// <inheritdoc/>
        protected override void OnStylusUp(StylusEventArgs e)
        {
            base.OnStylusUp(e);
            IsPressed = false;
        }

        private void UpdateFocusState()
        {
            VisualStateManager.GoToState(this, IsFocused ? "Focused" : "Unfocused", true);
        }

        private void UpdateSelectionState()
        {
            SelectionStates newState;

            if (IsSelected)
            {
                if (IsPressed)
                {
                    newState = SelectionStates.SelectedPressed;
                }
                else if (IsMouseOver || IsStylusOver)
                {
                    newState = SelectionStates.SelectedPointerOver;
                }
                else
                {
                    newState = SelectionStates.Selected;
                }
            }
            else
            {
                if (IsPressed)
                {
                    newState = SelectionStates.UnselectedPressed;
                }
                else if (IsMouseOver || IsStylusOver)
                {
                    newState = SelectionStates.UnselectedPointerOver;
                }
                else
                {
                    newState = SelectionStates.Unselected;
                }
            }

            VisualStateManager.GoToState(this, newState.ToString(), true);
        }
    }
}
