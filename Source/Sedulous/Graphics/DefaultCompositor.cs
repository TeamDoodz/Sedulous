﻿using System.Drawing;
using Sedulous.Core;
using Sedulous.Platform;

namespace Sedulous.Graphics
{
    /// <summary>
    /// Represents Sedulous's default compositor, which renders
    /// directly to the back buffer.
    /// </summary>
    public sealed class DefaultCompositor : Compositor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCompositor"/> class.
        /// </summary>
        /// <param name="context">The Framework context.</param>
        /// <param name="window">The window with which this compositor is associated.</param>
        public DefaultCompositor(FrameworkContext context, IFrameworkWindow window)
            : base(context, window)
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="DefaultCompositor"/> class.
        /// </summary>
        /// <param name="window">The window with which the created compositor is associated.</param>
        /// <returns>The instance of <see cref="DefaultCompositor"/> that was created.</returns>
        public static DefaultCompositor Create(IFrameworkWindow window)
        {
            var uv = FrameworkContext.DemandCurrent();
            return new DefaultCompositor(uv, window);
        }

        /// <inheritdoc/>
        public override RenderTarget2D GetRenderTarget()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            return null;
        }

        /// <inheritdoc/>
        public override void BeginFrame()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            var window = FrameworkContext.GetPlatform().Windows.GetCurrent();
            var graphics = FrameworkContext.GetGraphics();
            graphics.SetRenderTarget(null);
            graphics.SetViewport(new Viewport(0, 0, window.DrawableSize.Width, window.DrawableSize.Height));
            graphics.Clear(Color.CornflowerBlue, 1.0f, 0);
        }

        /// <inheritdoc/>
        public override Size Size => Window.DrawableSize;
    }
}
