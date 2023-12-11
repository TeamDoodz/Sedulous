using Sedulous.Content;
using Sedulous.Platform;
using Sedulous.Sdl2.Platform.Surface;

namespace Sedulous.Sdl2
{
    /// <summary>
    /// Loads a cursor from an image.
    /// </summary>
    //[ContentProcessor]
    public sealed class Sdl2CursorProcessor : ContentProcessor<PlatformNativeSurface, Cursor>
    {
        /// <summary>
        /// Processes the specified data structure into a game asset.
        /// </summary>
        /// <param name="manager">The content manager with which the asset is being processed.</param>
        /// <param name="metadata">The asset's metadata.</param>
        /// <param name="input">The input data structure to process.</param>
        /// <returns>The game asset that was created.</returns>
        public override Cursor Process(ContentManager manager, IContentProcessorMetadata metadata, PlatformNativeSurface input)
        {
            using (var surface = new Sdl2Surface2D(manager.FrameworkContext, input.CreateCopy(), SurfaceOptions.SrgbColor))
            {
                return new Sdl2Cursor(manager.FrameworkContext, surface, 0, 0);
            }
        }
    }
}
