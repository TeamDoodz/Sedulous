using System;
using Sedulous.Audio;
using Sedulous.Content;

namespace Sedulous.Fmod.Audio
{
    /// <summary>
    /// Loads song assets.
    /// </summary>
    //[ContentProcessor]
    public sealed class FmodSongProcessor : ContentProcessor<FmodMediaDescription, Song>
    {
        /// <inheritdoc/>
        public override Song Process(ContentManager manager, IContentProcessorMetadata metadata, FmodMediaDescription input)
        {
            if (!input.IsFilename)
                throw new NotSupportedException();

            return new FmodSong(manager.FrameworkContext, (String)input.Data);
        }
    }
}
