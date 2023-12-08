using Sedulous.Audio;
using Sedulous.Bass.Audio;
using Sedulous.Core;

namespace Sedulous.Bass
{
    /// <summary>
    /// Represents an Sedulous plugin which registers BASS as the audio subsystem implementation.
    /// </summary>
    public class BassAudioPlugin : FrameworkPlugin
    {
        /// <inheritdoc/>
        public override void Register(FrameworkConfiguration configuration)
        {
            Contract.Require(configuration, nameof(configuration));

            base.Register(configuration);
        }

        /// <inheritdoc/>
        public override void Configure(FrameworkContext context, FrameworkFactory factory)
        {
            factory.SetFactoryMethod<SongPlayerFactory>((uv) => new BassSongPlayer(uv));
            factory.SetFactoryMethod<SoundEffectPlayerFactory>((uv) => new BassSoundEffectPlayer(uv));

            factory.SetFactoryMethod<FrameworkAudioFactory>((uv, configuration) => new BassAudioSubsystem(uv));

            base.Configure(context, factory);
        }

        /// <inheritdoc/>
        public override void Initialize(FrameworkContext context, FrameworkFactory factory)
        {
            var importers = context.GetContent().Importers;
            {
                importers.RegisterImporter<BassMediaImporter>(".mp3");
                importers.RegisterImporter<BassMediaImporter>(".ogg");
                importers.RegisterImporter<BassMediaImporter>(".wav");
            }

            var processors = context.GetContent().Processors;
            {
                processors.RegisterProcessor<BassSongProcessor>();
                processors.RegisterProcessor<BassSoundEffectProcessor>();
            }

            base.Initialize(context, factory);
        }
    }
}
