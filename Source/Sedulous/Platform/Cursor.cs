﻿using System;
using Sedulous.Core;
using Sedulous.Graphics;
using Sedulous.Platform;

namespace Sedulous
{
    /// <summary>
    /// Represents a factory method which constructs instances of the <see cref="Cursor"/> class.
    /// </summary>
    /// <param name="context">The Framework context.</param>
    /// <param name="surface">The surface that contains the cursor image.</param>
    /// <param name="hx">The x-coordinate of the cursor's hotspot.</param>
    /// <param name="hy">The y-coordinate of the cursor's hotspot.</param>
    /// <returns>The instance of <see cref="Cursor"/> that was created.</returns>
    public delegate Cursor CursorFactory(FrameworkContext context, Surface2D surface, Int32 hx, Int32 hy);

    /// <summary>
    /// Represents a mouse cursor.
    /// </summary>
    public abstract class Cursor : FrameworkResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cursor"/> class.
        /// </summary>
        /// <param name="context">The Framework context.</param>
        public Cursor(FrameworkContext context)
            : base(context)
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="Cursor"/> class.
        /// </summary>
        /// <param name="surface">The <see cref="Surface2D"/> that contains the cursor image.</param>
        /// <returns>The instance of <see cref="Cursor"/> that was created.</returns>
        public static Cursor Create(Surface2D surface)
        {
            Contract.Require(surface, nameof(surface));

            var uv = FrameworkContext.DemandCurrent();
            return uv.GetFactoryMethod<CursorFactory>()(uv, surface, 0, 0);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Cursor"/> class.
        /// </summary>
        /// <param name="surface">The <see cref="Surface2D"/> that contains the cursor image.</param>
        /// <param name="hx">The x-coordinate of the cursor's hotspot.</param>
        /// <param name="hy">The y-coordinate of the cursor's hotspot.</param>
        /// <returns>The instance of <see cref="Cursor"/> that was created.</returns>
        public static Cursor Create(Surface2D surface, Int32 hx, Int32 hy)
        {
            Contract.Require(surface, nameof(surface));

            var uv = FrameworkContext.DemandCurrent();
            return uv.GetFactoryMethod<CursorFactory>()(uv, surface, hx, hy);
        }

        /// <summary>
        /// Gets the width of the cursor in pixels.
        /// </summary>
        public abstract Int32 Width
        {
            get;
        }

        /// <summary>
        /// Gets the height of the cursor in pixels.
        /// </summary>
        public abstract Int32 Height
        {
            get;
        }

        /// <summary>
        /// Gets the x-coordinate of the cursor's hotspot.
        /// </summary>
        public abstract Int32 HotspotX
        {
            get;
        }

        /// <summary>
        /// Gets the y-coordinate of the cursor's hotspot.
        /// </summary>
        public abstract Int32 HotspotY
        {
            get;
        }
    }
}
