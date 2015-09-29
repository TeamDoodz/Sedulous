﻿using System;
using TwistedLogik.Ultraviolet.Input;
using TwistedLogik.Ultraviolet.UI.Presentation.Input;

namespace TwistedLogik.Ultraviolet.UI.Presentation.Controls
{
    /// <summary>
    /// Represents a button that raises its <see cref="Primitives.ButtonBase.Click"/> event repeatedly while it is pressed.
    /// </summary>
    [UvmlKnownType(null, "TwistedLogik.Ultraviolet.UI.Presentation.Controls.Templates.RepeatButton.xml")]
    public class RepeatButton : Button
    {
        /// <summary>
        /// Initializes the <see cref="RepeatButton"/> type.
        /// </summary>
        static RepeatButton()
        {
            ClickModeProperty.OverrideMetadata(typeof(RepeatButton), new PropertyMetadata<ClickMode>(ClickMode.Press));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatButton"/> class.
        /// </summary>
        /// <param name="uv">The Ultraviolet context.</param>
        /// <param name="name">The element's identifying name within its namescope.</param>
        public RepeatButton(UltravioletContext uv, String name)
            : base(uv, name)
        {

        }

        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, that the button waits prior to 
        /// beginning its repetitions.
        /// </summary>
        public Double Delay
        {
            get { return GetValue<Double>(DelayProperty); }
            set { SetValue<Double>(DelayProperty, value); }
        }

        /// <summary>
        /// Gets or sets the interval between repeated <see cref="Primitives.ButtonBase.Click"/> events, in milliseconds.
        /// </summary>
        public Double Interval
        {
            get { return GetValue<Double>(IntervalProperty); }
            set { SetValue<Double>(IntervalProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Delay"/> dependency property.
        /// </summary>
        /// <remarks>The styling name of this dependency property is 'delay'.</remarks>
        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register("Delay", typeof(Double), typeof(RepeatButton),
            new PropertyMetadata<Double>(SystemParameters.KeyboardDelay));
        
        /// <summary>
        /// Identifies the <see cref="Interval"/> dependency property.
        /// </summary>
        /// <remarks>The styling name of this dependency property is 'interval'.</remarks>
        public static readonly DependencyProperty IntervalProperty = DependencyProperty.Register("Interval", typeof(Double), typeof(RepeatButton),
            new PropertyMetadata<Double>(SystemParameters.KeyboardSpeed));

        /// <inheritdoc/>
        protected override void OnUpdating(UltravioletTime time)
        {
            UpdateRepetitions(time);

            base.OnUpdating(time);
        }

        /// <inheritdoc/>
        protected override void OnLostMouseCapture(ref RoutedEventData data)
        {
            repeating   = false;
            repeatTimer = 0;

            base.OnLostMouseCapture(ref data);
        }

        /// <inheritdoc/>
        protected override void OnMouseDown(MouseDevice device, MouseButton button, ref RoutedEventData data)
        {
            if (button == MouseButton.Left)
            {
                repeating   = false;
                repeatTimer = 0;

                data.Handled = true;
            }
            base.OnMouseDown(device, button, ref data);
        }
        
        /// <summary>
        /// Updates the button's repetition state.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="UltravioletContext.Draw(UltravioletTime)"/>.</param>
        private void UpdateRepetitions(UltravioletTime time)
        {
            if (!IsPressed)
                return;

            var input = Ultraviolet.GetInput();
            if (input.IsMouseSupported())
            {
                var position = Mouse.GetPosition(this);
                if (!Bounds.Contains(position))
                {
                    return;
                }
            }

            repeatTimer += time.ElapsedTime.TotalMilliseconds;
            if (repeating)
            {
                var interval = Interval;
                if (repeatTimer >= interval)
                {
                    repeatTimer %= interval;
                    OnClick();
                }
            }
            else
            {
                var delay = Delay;
                if (repeatTimer >= delay)
                {
                    repeatTimer %= delay;
                    repeating = true;
                    OnClick();
                }
            }
        }

        // State values.
        private Double repeatTimer;
        private Boolean repeating;
    }
}
