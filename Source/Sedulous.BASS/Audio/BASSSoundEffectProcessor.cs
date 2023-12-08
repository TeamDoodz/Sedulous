using System;
using Sedulous.Audio;
using Sedulous.Content;

namespace Sedulous.Bass.Audio
{
    /// <summary>
    /// Loads sound effect assets.
    /// </summary>
    //[ContentProcessor]
    public sealed class BassSoundEffectProcessor : ContentProcessor<BassMediaDescription, SoundEffect>
    {
        /// <inheritdoc/>
        public override SoundEffect Process(ContentManager manager, IContentProcessorMetadata metadata, BassMediaDescription input)
        {
            return input.IsFilename ?
                new BassSoundEffect(manager.FrameworkContext, (String)input.Data) :
                new BassSoundEffect(manager.FrameworkContext, (Byte[])input.Data);
        }
    }
}
