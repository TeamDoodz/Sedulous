using System;
using System.IO;
using Sedulous.Content;

namespace Sedulous.Fmod.Audio
{
    /// <summary>
    /// Imports .mp3, .ogg, and .wav files.
    /// </summary>
    //[ContentImporter(".aif")]
    //[ContentImporter(".aiff")]
    //[ContentImporter(".flac")]
    //[ContentImporter(".it")]
    //[ContentImporter(".m3u")]
    //[ContentImporter(".mid")]
    //[ContentImporter(".mod")]
    //[ContentImporter(".mp2")]
    //[ContentImporter(".mp3")]
    //[ContentImporter(".ogg")]
    //[ContentImporter(".s3m")]
    //[ContentImporter(".wav")]
    public sealed class FmodMediaImporter : ContentImporter<FmodMediaDescription>
    {
        /// <inheritdoc/>
        public override FmodMediaDescription Import(IContentImporterMetadata metadata, Stream stream)
        {
            if (metadata.IsFile)
                return new FmodMediaDescription(metadata.AssetFilePath);

            var buffer = new Byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return new FmodMediaDescription(buffer);
        }
    }
}
