using Sedulous.Audio;
using Sedulous.Core;
using System.IO;
using System.Reflection;
using System;
using Sedulous.Fmod.Audio;
using System.Linq;
using Sedulous.Content;

namespace Sedulous.Fmod
{
    /// <summary>
    /// Represents an Sedulous plugin which registers FMOD as the audio subsystem implementation.
    /// </summary>
    public class FmodAudioPlugin : FrameworkPlugin
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
            factory.SetFactoryMethod<SongPlayerFactory>((uv) => new FmodSongPlayer(uv));
            factory.SetFactoryMethod<SoundEffectPlayerFactory>((uv) => new FmodSoundEffectPlayer(uv));

            try
            {
                if (FrameworkPlatformInfo.CurrentPlatform == FrameworkPlatform.Android)
                {
                    var shim = Assembly.Load("Sedulous.Shims.Android.FMOD.dll");
                    var type = shim.GetTypes().Where(x => x.IsClass && !x.IsAbstract && typeof(FmodPlatformSpecificImplementationDetails).IsAssignableFrom(x)).SingleOrDefault();
                    if (type == null)
                        throw new InvalidOperationException(FmodStrings.CannotFindPlatformShimClass);

                    factory.SetFactoryMethod<FMODPlatformSpecificImplementationDetailsFactory>(
                        (uv) => (FmodPlatformSpecificImplementationDetails)Activator.CreateInstance(type));
                }
            }
            catch (FileNotFoundException e)
            {
                throw new Exception(FrameworkStrings.MissingCompatibilityShim.Format(e.FileName));
            }

            factory.SetFactoryMethod<FrameworkAudioFactory>((uv, configuration) => new FmodAudioSubsystem(uv, configuration));

            base.Configure(context, factory);
        }

        /// <inheritdoc/>
        public override void Initialize(FrameworkContext context, FrameworkFactory factory)
        {
            var importers = context.GetContent().Importers;
            {
                importers.RegisterImporter<FmodMediaImporter>(".aif");
                importers.RegisterImporter<FmodMediaImporter>(".aiff");
                importers.RegisterImporter<FmodMediaImporter>(".flac");
                importers.RegisterImporter<FmodMediaImporter>(".it");
                importers.RegisterImporter<FmodMediaImporter>(".m3u");
                importers.RegisterImporter<FmodMediaImporter>(".mid");
                importers.RegisterImporter<FmodMediaImporter>(".mod");
                importers.RegisterImporter<FmodMediaImporter>(".mp2");
                importers.RegisterImporter<FmodMediaImporter>(".mp3");
                importers.RegisterImporter<FmodMediaImporter>(".ogg");
                importers.RegisterImporter<FmodMediaImporter>(".s3m");
                importers.RegisterImporter<FmodMediaImporter>(".wav");
            }

            var processors = context.GetContent().Processors;
            {
                processors.RegisterProcessor<FmodSongProcessor>();
                processors.RegisterProcessor<FmodSoundEffectProcessor>();
            }

            base.Initialize(context, factory);
        }
    }
}
