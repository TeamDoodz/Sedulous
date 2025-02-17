﻿using System;

namespace Sedulous.Graphics
{
    /// <summary>
    /// Represents one of an effect's techniques, which contains all of the state necessary
    /// to render a particular material.
    /// </summary>
    public abstract class EffectTechnique : FrameworkResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EffectTechnique"/> class.
        /// </summary>
        /// <param name="context">The Framework context.</param>
        public EffectTechnique(FrameworkContext context)
            : base(context)
        {

        }

        /// <summary>
        /// Gets the effect technique's name.
        /// </summary>
        public abstract String Name
        {
            get;
        }
        
        /// <summary>
        /// Gets the effect technique's collection of passes.
        /// </summary>
        public abstract EffectPassCollection Passes
        {
            get;
        }
    }
}
