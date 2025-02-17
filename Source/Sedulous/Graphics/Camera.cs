﻿using System.Numerics;
using Sedulous.Platform;

namespace Sedulous.Graphics
{
    /// <summary>
    /// Represents a camera which provides a view and projection matrix.
    /// </summary>
    public abstract class Camera : FrameworkResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="context">The Framework context.</param>
        protected Camera(FrameworkContext context)
            : base(context)
        {

        }

        /// <summary>
        /// Updates the camera's state.
        /// </summary>
        /// <param name="window">The window for which to update the camera. If not specified, the current window is 
        /// used, or if there is no current window, the primary window is used.</param>
        public abstract void Update(IFrameworkWindow window = null);

        /// <summary>
        /// Gets the camera's view matrix.
        /// </summary>
        /// <param name="matrix">The resulting matrix.</param>
        public abstract void GetViewMatrix(out Matrix4x4 matrix);

        /// <summary>
        /// Gets the camera's projection matrix.
        /// </summary>
        /// <param name="matrix">The resulting matrix.</param>
        public abstract void GetProjectionMatrix(out Matrix4x4 matrix);

        /// <summary>
        /// Gets the camera's combined view-projection matrix.
        /// </summary>
        /// <param name="matrix">The resulting matrix.</param>
        public abstract void GetViewProjectionMatrix(out Matrix4x4 matrix);
    }
}