// Adapted from https://gist.github.com/anonymous/4326429
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace AudioPipe.Controls
{
    /// <summary>
    /// Helps find the user-selected value of a slider only when the keyboard/mouse gesture has ended.
    /// </summary>
    public class SliderValueChangedBehavior : Behavior<Slider>
    {
        /// <summary>
        /// Identifes the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            nameof(Command),
            typeof(ICommand),
            typeof(SliderValueChangedBehavior),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(double),
            typeof(SliderValueChangedBehavior),
            new PropertyMetadata(default(double), OnValuePropertyChanged));

        private int keysDown;

        private bool mouseCaptureBound;

        /// <summary>
        /// Gets or sets a command to execute when the value changes.
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the slider value.
        /// </summary>
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            AssociatedObject.KeyUp += OnKeyUp;
            AssociatedObject.KeyDown += OnKeyDown;
            AssociatedObject.ValueChanged += OnValueChanged;

            base.OnAttached();
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.KeyUp -= OnKeyUp;
            AssociatedObject.KeyDown -= OnKeyDown;
            AssociatedObject.ValueChanged -= OnValueChanged;
        }

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = (SliderValueChangedBehavior)d;
            if (me.AssociatedObject != null)
            {
                me.Value = (double)e.NewValue;
            }
        }

        /// <summary>
        /// Applies the current value in the Value dependency property and raises the command.
        /// </summary>
        private void ApplyValue()
        {
            Value = AssociatedObject.Value;

            Command?.Execute(Value);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            keysDown++;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (keysDown-- != 0)
            {
                ApplyValue();
            }
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            mouseCaptureBound = false;
            AssociatedObject.LostMouseCapture -= OnLostMouseCapture;
            ApplyValue();
        }

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Mouse.Captured != null)
            {
                if (!mouseCaptureBound)
                {
                    AssociatedObject.LostMouseCapture += OnLostMouseCapture;
                    mouseCaptureBound = true;
                }
            }
            else if (keysDown == 0)
            {
                ApplyValue();
            }
        }
    }
}