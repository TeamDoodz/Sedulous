﻿using System;
using System.Numerics;
using Sedulous.Platform;

namespace Sedulous.Graphics.Graphics3D
{
    /// <summary>
    /// Represents a camera with perspective.
    /// </summary>
    public sealed class PerspectiveCamera : Camera
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerspectiveCamera"/> class.
        /// </summary>
        /// <param name="context">The Framework context.</param>
        private PerspectiveCamera(FrameworkContext context)
            : base(context)
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="PerspectiveCamera"/> class.
        /// </summary>
        /// <returns>The instance of <see cref="PerspectiveCamera"/> that was created.</returns>
        public static PerspectiveCamera Create() => new PerspectiveCamera(FrameworkContext.DemandCurrent());

        /// <inheritdoc/>
        public override void Update(IFrameworkWindow window = null)
        {
            var win = window ?? FrameworkContext.GetPlatform().Windows.GetCurrent() ?? FrameworkContext.GetPlatform().Windows.GetPrimary();
            var aspectRatio = win.DrawableSize.Width / (Single)win.DrawableSize.Height;

            view = Matrix4x4.CreateLookAt(Position, Target, Up);
            proj = Matrix4x4.CreatePerspectiveFieldOfView(FieldOfView, aspectRatio, NearPlaneDistance, FarPlaneDistance);
            viewproj = view * proj;
        }

        /// <inheritdoc/>
        public override void GetViewMatrix(out Matrix4x4 matrix) => matrix = view;

        /// <inheritdoc/>
        public override void GetProjectionMatrix(out Matrix4x4 matrix) => matrix = proj;

        /// <inheritdoc/>
        public override void GetViewProjectionMatrix(out Matrix4x4 matrix) => matrix = viewproj;

        /// <summary>
        /// Gets the camera's field of view in radians.
        /// </summary>
        public Single FieldOfView { get; } = (Single)Math.PI / 4f;

        /// <summary>
        /// Gets or sets the distance to the near plane.
        /// </summary>
        public Single NearPlaneDistance { get; set; } = 1f;

        /// <summary>
        /// Gets or sets the distance to the far plane.
        /// </summary>
        public Single FarPlaneDistance { get; set; } = 1000f;

        /// <summary>
        /// Gets or sets the camera's position in 3D space.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the point in 3D space at which the camera is targeted.
        /// </summary>
        public Vector3 Target { get; set; }

        /// <summary>
        /// Gets or sets the vector that denotes which direction is "up" for this camera.
        /// </summary>
        public Vector3 Up { get; set; } = Vector3.UnitY;

        // Calculated matrices.
        private Matrix4x4 view;
        private Matrix4x4 proj;
        private Matrix4x4 viewproj;
    }
}