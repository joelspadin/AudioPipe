using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AudioPipe.Controls
{
    //[TemplateVisualState(GroupName = "SelectionStates", Name = "Disabled")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "Unselected")]
    //[TemplateVisualState(GroupName = "SelectionStates", Name = "UnselectedLockled")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "UnselectedPointerOver")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "UnselectedPressed")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "Selected")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "SelectedPointerOver")]
    [TemplateVisualState(GroupName = "SelectionStates", Name = "SelectedPressed")]
    [TemplateVisualState(GroupName = "FocusStates", Name = "Unfocused")]
    [TemplateVisualState(GroupName = "FocusStates", Name = "Focused")]
    public class PivotHeaderItem : ContentControl
    {
        private bool _isSelected;
        private bool _isPressed;

        internal bool IsPressed
        {
            get => _isPressed;
            set
            {
                if (value != _isPressed)
                {
                    _isPressed = value;
                    UpdateSelectionState();
                }
            }
        }


        internal bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    UpdateSelectionState();
                }
            }
        }

        public PivotHeaderItem()
        {
            UpdateFocusState();
            UpdateSelectionState();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            UpdateSelectionState();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            UpdateSelectionState();
        }

        protected override void OnStylusEnter(StylusEventArgs e)
        {
            base.OnStylusEnter(e);
            UpdateSelectionState();
        }

        protected override void OnStylusLeave(StylusEventArgs e)
        {
            base.OnStylusLeave(e);
            UpdateSelectionState();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            IsPressed = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            IsPressed = false;
        }

        protected override void OnStylusDown(StylusDownEventArgs e)
        {
            base.OnStylusDown(e);
            IsPressed = true;
        }

        protected override void OnStylusUp(StylusEventArgs e)
        {
            base.OnStylusUp(e);
            IsPressed = false;
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            UpdateFocusState();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            UpdateFocusState();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateFocusState();
            UpdateDefaultStyle();
            UpdateSelectionState();
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


        private enum SelectionStates
        {
            Unselected,
            UnselectedPointerOver,
            UnselectedPressed,
            Selected,
            SelectedPointerOver,
            SelectedPressed,
        }
    }
}
