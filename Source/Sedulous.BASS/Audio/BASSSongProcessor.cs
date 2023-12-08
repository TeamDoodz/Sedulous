using System;
using Sedulous.Audio;
using Sedulous.Content;

namespace Sedulous.Bass.Audio
{
    /// <summary>
    /// Loads song assets.
    /// </summary>
    //[ContentProcessor]
    public sealed class BassSongProcessor : ContentProcessor<BassMediaDescription, Song>
    {
        /// <inheritdoc/>
        public override Song Process(ContentManager manager, IContentProcessorMetadata metadata, BassMediaDescription input)
        {
            if (!input.IsFilename)
                throw new NotSupportedException();

            return new BassSong(manager.FrameworkContext, (String)input.Data);
        }
    }
}
