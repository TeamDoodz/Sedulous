﻿using System.Drawing;

namespace Sedulous.Graphics
{
    /// <summary>
    /// Represents an effect which exposes a texture size parameter.
    /// </summary>
    public interface IEffectTextureSize
    {
        /// <summary>
        /// Gets or sets the dimensions of the texture being rendered.
        /// </summary>
        Size TextureSize
        {
            get;
            set;
        }
    }
}
