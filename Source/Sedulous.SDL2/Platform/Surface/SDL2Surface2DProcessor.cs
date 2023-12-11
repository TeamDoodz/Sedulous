﻿using Sedulous.Content;
using Sedulous.Platform;

namespace Sedulous.Sdl2.Platform.Surface
{
    /// <summary>
    /// Loads 2D surface assets.
    /// </summary>
    //[ContentProcessor]
    public sealed class Sdl2Surface2DProcessor : ContentProcessor<PlatformNativeSurface, Surface2D>
    {
        /// <summary>
        /// Processes the specified data structure into a game asset.
        /// </summary>
        /// <param name="manager">The content manager with which the asset is being processed.</param>
        /// <param name="metadata">The asset's metadata.</param>
        /// <param name="input">The input data structure to process.</param>
        /// <returns>The game asset that was created.</returns>
        public override Surface2D Process(ContentManager manager, IContentProcessorMetadata metadata, PlatformNativeSurface input)
        {
            var mdat = metadata.As<Sdl2Surface2DProcessorMetadata>();
            var srgbEncoded = mdat.SrgbEncoded ?? manager.FrameworkContext.Properties.SrgbDefaultForSurface2D;
            var surfOptions = srgbEncoded ? SurfaceOptions.SrgbColor : SurfaceOptions.LinearColor;

            var copy = input.CreateCopy();
            var result = new Sdl2Surface2D(manager.FrameworkContext, copy, surfOptions);

            return result;
        }
    }
}
