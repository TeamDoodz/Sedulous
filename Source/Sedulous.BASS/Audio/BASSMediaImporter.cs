using System;
using System.IO;
using Sedulous.Content;

namespace Sedulous.Bass.Audio
{
    /// <summary>
    /// Imports .mp3, .ogg, and .wav files.
    /// </summary>
    //[ContentImporter(".mp3")]
    //[ContentImporter(".ogg")]
    //[ContentImporter(".wav")]
    public sealed class BassMediaImporter : ContentImporter<BassMediaDescription>
    {
        /// <inheritdoc/>
        public override BassMediaDescription Import(IContentImporterMetadata metadata, Stream stream)
        {
            if (metadata.IsFile)
                return new BassMediaDescription(metadata.AssetFilePath);

            var buffer = new Byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return new BassMediaDescription(buffer);
        }
    }
}
