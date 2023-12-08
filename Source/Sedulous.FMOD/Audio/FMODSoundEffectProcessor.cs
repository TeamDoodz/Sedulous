using System;
using Sedulous.Audio;
using Sedulous.Content;

namespace Sedulous.Fmod.Audio
{
    /// <summary>
    /// Loads sound effect assets.
    /// </summary>
    //[ContentProcessor]
    public sealed class FmodSoundEffectProcessor : ContentProcessor<FmodMediaDescription, SoundEffect>
    {
        /// <inheritdoc/>
        public override SoundEffect Process(ContentManager manager, IContentProcessorMetadata metadata, FmodMediaDescription input)
        {
            if (!input.IsFilename)
                throw new NotSupportedException();

            return new FmodSoundEffect(manager.FrameworkContext, (String)input.Data);
        }
    }
}
