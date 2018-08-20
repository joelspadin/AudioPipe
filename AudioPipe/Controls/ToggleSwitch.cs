using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AudioPipe.Controls
{
    /// <summary>
    /// WPF port of <see cref="Windows.UI.Xaml.Controls.ToggleSwitch"/>.
    /// </summary>
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
        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(object), typeof(ToggleSwitch), new FrameworkPropertyMetadata(null, OnHeaderChanged2));

        /// <summary>
        /// Identifies the <see cref="HeaderTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(ToggleSwitch), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IsOn"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register(
                nameof(IsOn),
                typeof(bool),
                typeof(ToggleSwitch),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnToggled));

        /// <summary>
        /// Identifies the <see cref="OffContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OffContentProperty =
            DependencyProperty.Register(
                nameof(OffContent),
                typeof(object),
                typeof(ToggleSwitch),
                new FrameworkPropertyMetadata("Off", OnOffContentChanged2));

        /// <summary>
        /// Identifies the <see cref="OffContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OffContentTemplateProperty =
            DependencyProperty.Register(nameof(OffContentTemplate), typeof(DataTemplate), typeof(ToggleSwitch), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="OnContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OnContentProperty =
            DependencyProperty.Register(
                nameof(OnContent),
                typeof(object),
                typeof(ToggleSwitch),
                new FrameworkPropertyMetadata("On", OnOnContentChanged2));

        /// <summary>
        /// Identifies the <see cref="OnContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OnContentTemplateProperty =
            DependencyProperty.Register(nameof(OnContentTemplate), typeof(DataTemplate), typeof(ToggleSwitch), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleSwitch"/> class.
        /// </summary>
        public ToggleSwitch()
        {
            IsEnabledChanged += ToggleSwitch_IsEnabledChanged;
        }

        /// <summary>
        /// Occurs when the On/Off state changes for this <see cref="ToggleSwitch"/>.
        /// </summary>
        public event RoutedEventHandler Toggled;

        /// <summary>
        /// Gets or sets the header content.
        /// </summary>
        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display the control's header.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValue(HeaderTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the state of the <see cref="ToggleSwitch"/> is "On".
        /// </summary>
        public bool IsOn
        {
            get => (bool)GetValue(IsOnProperty);
            set => SetValue(IsOnProperty, value);
        }

        /// <summary>
        /// Gets or sets the object's content that should be displayed using the <see cref="OffContentTemplate"/>
        /// when this <see cref="ToggleSwitch"/> is in the "Off" state.
        /// </summary>
        public object OffContent
        {
            get => GetValue(OffContentProperty);
            set => SetValue(OffContentProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display the control's content while in the "Off" state.
        /// </summary>
        public DataTemplate OffContentTemplate
        {
            get => (DataTemplate)GetValue(OffContentTemplateProperty);
            set => SetValue(OffContentTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the object's content that should be displayed using the <see cref="OnContentTemplate"/>
        /// when this <see cref="ToggleSwitch"/> is in the "On" state.
        /// </summary>
        public object OnContent
        {
            get => GetValue(OnContentProperty);
            set => SetValue(OnContentProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> used to display the control's content while in the "On" state.
        /// </summary>
        public DataTemplate OnContentTemplate
        {
            get => (DataTemplate)GetValue(OnContentTemplateProperty);
            set => SetValue(OnContentTemplateProperty, value);
        }

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UpdateCommonState(false);
            UpdateToggleState();
        }

        /// <summary>
        /// Invoked when the content for <see cref="Header"/> changes.
        /// </summary>
        /// <param name="oldContent">The string or object content of the old content.</param>
        /// <param name="newContent">The string or object content of the new content.</param>
        protected virtual void OnHeaderChanged(object oldContent, object newContent)
        {
        }

        /// <inheritdoc/>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            UpdateCommonState(e.LeftButton == MouseButtonState.Pressed);
        }

        /// <inheritdoc/>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            UpdateCommonState(false);
        }

        /// <summary>
        /// Invoked when the content for <see cref="OffContent"/> changes.
        /// </summary>
        /// <param name="oldContent">The string or object content of the old content.</param>
        /// <param name="newContent">The string or object content of the new content.</param>
        protected virtual void OnOffContentChanged(object oldContent, object newContent)
        {
        }

        /// <summary>
        /// Invoked when the content for <see cref="OnContent"/> changes.
        /// </summary>
        /// <param name="oldContent">The string or object content of the old content.</param>
        /// <param name="newContent">The string or object content of the new content.</param>
        protected virtual void OnOnContentChanged(object oldContent, object newContent)
        {
        }

        /// <inheritdoc/>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            UpdateCommonState(true);
        }

        /// <inheritdoc/>
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            UpdateCommonState(false);

            if (IsEnabled)
            {
                IsOn = !IsOn;
            }
        }

        /// <summary>
        /// Invoked before the <see cref="Toggled"/> event is raised.
        /// </summary>
        protected virtual void OnToggled()
        {
            UpdateToggleState();

            // TODO: event args?
            Toggled?.Invoke(this, new RoutedEventArgs());
        }

        private static void OnHeaderChanged2(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            toggle.OnHeaderChanged(e.OldValue, e.NewValue);
        }

        private static void OnOffContentChanged2(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            toggle.OnOffContentChanged(e.OldValue, e.NewValue);
        }

        private static void OnOnContentChanged2(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            toggle.OnOnContentChanged(e.OldValue, e.NewValue);
        }

        private static void OnToggled(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            toggle.OnToggled();
        }

        private void ToggleSwitch_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateEnabledState();
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
    }
}
