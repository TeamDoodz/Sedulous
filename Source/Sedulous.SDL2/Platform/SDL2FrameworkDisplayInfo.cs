﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sedulous.Core;
using Sedulous.Platform;
using static Sedulous.Sdl2.Native.SDLNative;

namespace Sedulous.Sdl2.Platform
{
    /// <summary>
    /// Represents the SDL2 implementation of the <see cref="IFrameworkDisplayInfo"/> interface.
    /// </summary>
    public sealed class Sdl2FrameworkDisplayInfo : IFrameworkDisplayInfo
    {
        /// <summary>
        /// Initializes a new instance of the OpenGLSedulousDisplayInfo class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        public Sdl2FrameworkDisplayInfo(FrameworkContext context)
        {
            Contract.Require(context, nameof(context));

            this.displays = Enumerable.Range(0, SDL_GetNumVideoDisplays())
                .Select(x => new Sdl2FrameworkDisplay(context, x))
                .ToList<IFrameworkDisplay>();
        }

        /// <summary>
        /// Updates the state of the application's displays.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Update(FrameworkTime)"/>.</param>
        public void Update(FrameworkTime time)
        {
            foreach (var display in displays)
                ((Sdl2FrameworkDisplay)display).Update(time);
        }

        /// <inheritdoc/>
        public IFrameworkDisplay this[Int32 ix]
        {
            get { return displays[ix]; }
        }

        /// <inheritdoc/>
        public IFrameworkDisplay PrimaryDisplay
        {
            get { return displays.Count == 0 ? null : displays[0]; }
        }

        /// <inheritdoc/>
        public Int32 Count
        {
            get { return displays.Count; }
        }

        /// <inheritdoc/>
        public List<IFrameworkDisplay>.Enumerator GetEnumerator()
        {
            return displays.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator<IFrameworkDisplay> IEnumerable<IFrameworkDisplay>.GetEnumerator()
        {
            return GetEnumerator();
        }

        // The list of display devices.  SDL2 never updates its device info, even if
        // devices are added or removed, so we only need to create this once.
        private readonly List<IFrameworkDisplay> displays;
    }
}
