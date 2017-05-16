using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

// https://gist.github.com/anonymous/4326429

namespace AudioPipe.Controls
{
    /// <summary>
    /// Helps find the user-selected value of a slider only when the keyboard/mouse gesture has ended.
    /// </summary>
    public class SliderValueChangedBehavior : Behavior<Slider>
    {
        private int _keysDown;

        private bool _mouseCaptureBound;

        #region Dependency property Value

        /// <summary>
        /// DataBindable value.
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(double),
            typeof(SliderValueChangedBehavior),
            new PropertyMetadata(default(double), OnValuePropertyChanged));

        #endregion

        #region Dependency property Value

        /// <summary>
        /// DataBindable Command
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(SliderValueChangedBehavior),
            new PropertyMetadata(null));

        #endregion

        /// <summary>
        /// On behavior attached.
        /// </summary>
        protected override void OnAttached()
        {
            AssociatedObject.KeyUp += OnKeyUp;
            AssociatedObject.KeyDown += OnKeyDown;
            AssociatedObject.ValueChanged += OnValueChanged;

            base.OnAttached();
        }

        /// <summary>
        /// On behavior detaching.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.KeyUp -= OnKeyUp;
            AssociatedObject.KeyDown -= OnKeyDown;
            AssociatedObject.ValueChanged -= OnValueChanged;
        }

        /// <summary>
        /// On Value dependency property change.
        /// </summary>
        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var me = (SliderValueChangedBehavior)d;
            if (me.AssociatedObject != null)
            {
                me.Value = (double)e.NewValue;
            }
        }

        /// <summary>
        /// Occurs when the slider's value change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Mouse.Captured != null)
            {
                if (!_mouseCaptureBound)
                {
                    AssociatedObject.LostMouseCapture += OnLostMouseCapture;
                    _mouseCaptureBound = true;
                }
            }
            else if (_keysDown == 0)
            {
                ApplyValue();
            }
        }

        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            _mouseCaptureBound = false;
            AssociatedObject.LostMouseCapture -= OnLostMouseCapture;
            ApplyValue();
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (_keysDown-- != 0)
            {
                ApplyValue();
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            _keysDown++;
        }

        /// <summary>
        /// Applies the current value in the Value dependency property and raises the command.
        /// </summary>
        private void ApplyValue()
        {
            this.Value = this.AssociatedObject.Value;

            this.Command?.Execute(this.Value);
        }
    }
}