﻿using System;
using Sedulous.Core;
using Sedulous.Input;
using static Sedulous.Sdl2.Native.SDLNative;

namespace Sedulous.Sdl2.Input
{
    /// <summary>
    /// Manages the Sedulous context's connected touch devices.
    /// </summary>
    internal sealed class TouchDeviceInfo : FrameworkResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TouchDeviceInfo"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        public TouchDeviceInfo(FrameworkContext context)
            : base(context)
        {
            var count = SDL_GetNumTouchDevices();
            devices = new Sdl2TouchDevice[count];

            for (int i = 0; i < count; i++)
            {
                devices[i] = new Sdl2TouchDevice(context, i);
            }
        }

        /// <summary>
        /// Resets the states of the connected devices in preparation for the next frame.
        /// </summary>
        public void ResetDeviceStates()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            foreach (var device in devices)
            {
                device.ResetDeviceState();
            }
        }

        /// <summary>
        /// Updates the states of the connected touch devices.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Update(FrameworkTime)"/>.</param>
        public void Update(FrameworkTime time)
        {
            foreach (var device in devices)
            {
                device.Update(time);
            }
        }

        /// <summary>
        /// Gets the touch device with the specified device index.
        /// </summary>
        /// <param name="index">The index of the device to retrieve.</param>
        /// <returns>The touch device with the specified device index, or <see langword="null"/> if no such device exists.</returns>
        public TouchDevice GetTouchDeviceByIndex(Int32 index)
        {
            return devices[index];
        }

        /// <summary>
        /// Gets the number of available touch devices.
        /// </summary>
        public Int32 Count
        {
            get { return devices.Length; }
        }

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                foreach (var device in devices)
                {
                    SafeDispose.Dispose(device);
                }
            }
            base.Dispose(disposing);
        }

        // Connected touch devices.
        private Sdl2TouchDevice[] devices;
    }
}
