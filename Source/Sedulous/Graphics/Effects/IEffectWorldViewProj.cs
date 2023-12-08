﻿
using System.Numerics;

namespace Sedulous.Graphics
{
    /// <summary>
    /// Represents an effect which exposes a combined world-view-projection matrix.
    /// </summary>
    public interface IEffectWorldViewProj
    {
        /// <summary>
        /// Gets or sets the effect's combined world-view-projection matrix.
        /// </summary>
		Matrix4x4 WorldViewProj
        {
            get;
            set;
        }
    }
}
