using System;

namespace Sedulous.Sdl2.Platform.Surface
{
    /// <summary>
    /// Contains metadata for <see cref="Sdl2Surface3DProcessor"/>.
    /// </summary>
    internal sealed class Sdl2Surface3DProcessorMetadata
    {
        /// <summary>
        /// Gets or sets a value indicating whether the surface is SRGB encoded. If <see langword="null"/>, the
        /// value specified by the <see cref="FrameworkContextProperties.SrgbDefaultForSurface3D"/> property is used.
        /// </summary>
        public Boolean? SrgbEncoded { get; set; }
    }
}
