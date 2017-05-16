using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AudioPipe.Controls
{
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "PointerOver")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Pressed")]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Disabled")]
    [TemplateVisualState(GroupName = "ToggleStates", Name = "On")]
    [TemplateVisualState(GroupName = "ToggleStates", Name = "Off")]
    [TemplateVisualState(GroupName = "ContentStates", Name = "OffContent")]
    [TemplateVisualState(GroupName = "ContentStates", Name = "OnContent")]
    public class ToggleSwitch : Control
    {
        public DataTemplate OnContentTemplate
        {
            get => (DataTemplate)GetValue(OnContentTemplateProperty);
            set => SetValue(OnContentTemplateProperty, value);
        }

        // Using a DependencyProperty as the backing store for OnContentTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnContentTemplateProperty =
            DependencyProperty.Register(nameof(OnContentTemplate), typeof(DataTemplate), typeof(ToggleSwitch), new PropertyMetadata(null));

        public object OnContent
        {
            get => GetValue(OnContentProperty);
            set => SetValue(OnContentProperty, value);
        }

        // Using a DependencyProperty as the backing store for OnContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnContentProperty =
            DependencyProperty.Register(nameof(OnContent), typeof(object), typeof(ToggleSwitch), new FrameworkPropertyMetadata("On", OnOnContentChanged2));

        public DataTemplate OffContentTemplate
        {
            get => (DataTemplate)GetValue(OffContentTemplateProperty);
            set => SetValue(OffContentTemplateProperty, value);
        }

        // Using a DependencyProperty as the backing store for OffContentTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffContentTemplateProperty =
            DependencyProperty.Register(nameof(OffContentTemplate), typeof(DataTemplate), typeof(ToggleSwitch), new PropertyMetadata(null));

        public object OffContent
        {
            get => GetValue(OffContentProperty);
            set => SetValue(OffContentProperty, value);
        }

        // Using a DependencyProperty as the backing store for OffContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffContentProperty =
            DependencyProperty.Register(nameof(OffContent), typeof(object), typeof(ToggleSwitch), new FrameworkPropertyMetadata("Off", OnOffContentChanged2));

        public bool IsOn
        {
            get => (bool)GetValue(IsOnProperty);
            set => SetValue(IsOnProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsOn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register(
                nameof(IsOn),
                typeof(bool),
                typeof(ToggleSwitch),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnToggled));

        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(object), typeof(ToggleSwitch), new FrameworkPropertyMetadata(null, OnHeaderChanged2));

        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        // Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(ToggleSwitch), new PropertyMetadata(null));



        public event RoutedEventHandler Toggled;

        public ToggleSwitch()
        {
            IsEnabledChanged += ToggleSwitch_IsEnabledChanged;
        }

        private void ToggleSwitch_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateEnabledState();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UpdateCommonState(false);
            UpdateToggleState();
        }

        protected virtual void OnToggled()
        {
            UpdateToggleState();

            // TODO: event args?
            Toggled?.Invoke(this, new RoutedEventArgs());
        }

        protected virtual void OnOnContentChanged(object oldContent, object newContent)
        {
        }

        protected virtual void OnOffContentChanged(object oldContent, object newContent)
        {
        }

        protected virtual void OnHeaderChanged(object oldContent, object newContent)
        {
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            UpdateCommonState(e.LeftButton == MouseButtonState.Pressed);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            UpdateCommonState(false);
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            UpdateCommonState(true);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            UpdateCommonState(false);

            if (IsEnabled)
            {
                IsOn = !IsOn;
            }
        }

        private void UpdateCommonState(bool pressed)
        {
            string state;

            if (IsEnabled)
            {
                if (pressed)
                {
                    state = "Pressed";
                }
                else if (IsMouseOver || IsStylusOver)
                {
                    state = "PointerOver";
                }
                else
                {
                    state = "Normal";
                }
            }
            else
            {
                state = "Disabled";
            }

            VisualStateManager.GoToState(this, state, true);
        }

        private void UpdateEnabledState()
        {
            VisualStateManager.GoToState(this, IsEnabled ? "Normal" : "Disabled", true);
        }

        private void UpdateToggleState()
        {
            VisualStateManager.GoToState(this, IsOn ? "On" : "Off", true);
            VisualStateManager.GoToState(this, IsOn ? "OnContent" : "OffContent", true);
        }

        private static void OnToggled(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            toggle.OnToggled();
        }

        private static void OnOnContentChanged2(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            toggle.OnOnContentChanged(e.OldValue, e.NewValue);
        }

        private static void OnOffContentChanged2(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            toggle.OnOffContentChanged(e.OldValue, e.NewValue);
        }

        private static void OnHeaderChanged2(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            toggle.OnHeaderChanged(e.OldValue, e.NewValue);
        }
    }
}
