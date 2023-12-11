using System;
using System.IO;
using Sedulous.Content;
using Sedulous.Platform;

namespace Sedulous.Sdl2.Platform.Surface
{
    /// <summary>
    /// Imports .bmp, .png, and .jpg files.
    /// </summary>
    //[ContentImporter(".bmp")]
    //[ContentImporter(".png")]
    //[ContentImporter(".jpg")]
    //[ContentImporter(".jpeg")]
    public unsafe sealed class Sdl2PlatformNativeSurfaceImporter : ContentImporter<PlatformNativeSurface>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sdl2PlatformNativeSurfaceImporter"/> class.
        /// </summary>
        public Sdl2PlatformNativeSurfaceImporter() { }

        /// <summary>
        /// Imports the data from the specified file.
        /// </summary>
        /// <param name="metadata">The asset metadata for the asset to import.</param>
        /// <param name="stream">The stream that contains the data to import.</param>
        /// <returns>The data structure that was imported from the file.</returns>
        public override PlatformNativeSurface Import(IContentImporterMetadata metadata, Stream stream)
        {
            var data = new Byte[stream.Length];
            stream.Read(data, 0, data.Length);

            using (var mstream = new MemoryStream(data))
            using (var source = SurfaceSource.Create(mstream))
            {
                return new Sdl2PlatformNativeSurface(source);
            }
        }
    }
}
