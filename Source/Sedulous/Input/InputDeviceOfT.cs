﻿using System;
using Sedulous.Core;

namespace Sedulous.Input
{
    /// <summary>
    /// Represents any input device.
    /// </summary>
    /// <typeparam name="T">The type of button exposed by the device.</typeparam>
    public abstract class InputDevice<T> : InputDevice
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputDevice{T}"/> class.
        /// </summary>
        /// <param name="context">The Framework context.</param>
        internal InputDevice(FrameworkContext context)
            : base(context)
        {

        }
        
        /// <summary>
        /// Gets a value indicating whether the specified button is currently down.
        /// </summary>
        /// <param name="button">The button to evaluate.</param>
        /// <returns><see langword="true"/> if the button is down; otherwise, <see langword="false"/>.</returns>
        public abstract Boolean IsButtonDown(T button);

        /// <summary>
        /// Gets a value indicating whether the specified button is currently up.
        /// </summary>
        /// <param name="button">The button to evaluate.</param>
        /// <returns><see langword="true"/> if the button is up; otherwise, <see langword="false"/>.</returns>
        public abstract Boolean IsButtonUp(T button);

        /// <summary>
        /// Gets a value indicating whether the specified button is currently pressed.
        /// </summary>
        /// <param name="button">The button to evaluate.</param>
        /// <param name="ignoreRepeats">A value indicating whether to ignore repeated button press events on devices which support them.</param>
        /// <returns><see langword="true"/> if the button is pressed; otherwise, <see langword="false"/>.</returns>        
        public abstract Boolean IsButtonPressed(T button, Boolean ignoreRepeats = true);

        /// <summary>
        /// Gets a value indicating whether the specified button is currently released.
        /// </summary>
        /// <param name="button">The button to evaluate.</param>
        /// <returns><see langword="true"/> if the button is released; otherwise, <see langword="false"/>.</returns>
        public abstract Boolean IsButtonReleased(T button);

        /// <summary>
        /// Gets the current state of the specified button.
        /// </summary>
        /// <param name="button">The button for which to retrieve a state.</param>
        /// <returns>The current state of the specified button.</returns>
        public virtual ButtonState GetButtonState(T button)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            var state = IsButtonDown(button) ? ButtonState.Down : ButtonState.Up;

            if (IsButtonPressed(button))
                state |= ButtonState.Pressed;

            if (IsButtonReleased(button))
                state |= ButtonState.Released;

            return state;
        }
    }
}