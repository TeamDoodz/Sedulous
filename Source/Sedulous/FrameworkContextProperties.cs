﻿using System;
using Sedulous.Graphics;
using Sedulous.Platform;

namespace Sedulous
{
    /// <summary>
    /// Represents the runtime properties of an instance of the <see cref="FrameworkContext"/> class.
    /// These properties represent a combination of the requested configuration settings and the capabilities
    /// of the platform which is executing the application.
    /// </summary>
    public sealed class FrameworkContextProperties
    {
        /// <summary>
        /// Gets a value indicating whether the context supports high-density display modes
        /// such as Retina and Retina HD. This allows the application to make use of every physical pixel 
        /// on the screen, rather than being scaled to use logical pixels.
        /// </summary>
        public Boolean SupportsHighDensityDisplayModes { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether instances of the <see cref="Surface2D"/>
        /// class are treated as SRGB encoded by default.
        /// </summary>
        public Boolean SrgbDefaultForSurface2D { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether instances of the <see cref="Surface3D"/>
        /// class are treated as SRGB encoded by default.
        /// </summary>
        public Boolean SrgbDefaultForSurface3D { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether instances of the <see cref="Texture2D"/>
        /// class are treated as SRGB encoded by default.
        /// </summary>
        public Boolean SrgbDefaultForTexture2D { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether instances of the <see cref="Texture3D"/>
        /// class are treated as SRGB encoded by default.
        /// </summary>
        public Boolean SrgbDefaultForTexture3D { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether instances of the <see cref="RenderBuffer2D"/>
        /// class are treated as SRGB encoded by default.
        /// </summary>
        public Boolean SrgbDefaultForRenderBuffer2D { get; internal set; }
    }
}
