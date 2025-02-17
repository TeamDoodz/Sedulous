﻿using System;

namespace Sedulous.Graphics
{
    /// <summary>
    /// Represents one render pass in an effect technique.
    /// </summary>
    public abstract class EffectPass : FrameworkResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EffectPass"/> class.
        /// </summary>
        /// <param name="context">The Framework context.</param>
        public EffectPass(FrameworkContext context)
            : base(context)
        {

        }

        /// <summary>
        /// Applies the effect pass state to the device.
        /// </summary>
        public abstract void Apply();

        /// <summary>
        /// Gets the effect pass's name.
        /// </summary>
        public abstract String Name
        {
            get;
        }
    }
}
