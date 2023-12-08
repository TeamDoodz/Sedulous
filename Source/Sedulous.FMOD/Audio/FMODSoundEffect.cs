using System;
using System.Runtime.InteropServices;
using Sedulous.Audio;
using Sedulous.Core;
using Sedulous.Fmod.Native;
using static Sedulous.Fmod.Native.FMOD_MODE;
using static Sedulous.Fmod.Native.FMOD_RESULT;
using static Sedulous.Fmod.Native.FMODNative;

namespace Sedulous.Fmod.Audio
{
    /// <summary>
    /// Represents the FMOD implementation of the <see cref="SoundEffect"/> class.
    /// </summary>
    public sealed unsafe class FmodSoundEffect : SoundEffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FmodSoundEffect"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="file">The path to the file from which to load the sound effect.</param>
        public FmodSoundEffect(FrameworkContext context, String file)
            : base(context)
        {
            Contract.RequireNotEmpty(file, nameof(file));

            var result = default(FMOD_RESULT);
            var system = ((FmodAudioSubsystem)context.GetAudio()).System;

            fixed (FMOD_SOUND** psound = &sound)
            {
                var exinfo = new FMOD_CREATESOUNDEXINFO();
                exinfo.cbsize = Marshal.SizeOf(exinfo);

                result = FMOD_System_CreateStream(system, file, FMOD_DEFAULT, &exinfo, psound);
                if (result != FMOD_OK)
                    throw new FmodException(result);
            }
            
            var durationInMilliseconds = 0u;

            result = FMOD_Sound_GetLength(sound, &durationInMilliseconds, FMOD_TIMEUNIT.FMOD_TIMEUNIT_MS);
            if (result != FMOD_OK)
                throw new FmodException(result);

            this.duration = TimeSpan.FromMilliseconds(durationInMilliseconds);
        }

        /// <inheritdoc/>
        public override void Play()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            var result = default(FMOD_RESULT);

            var system = ((FmodAudioSubsystem)FrameworkContext.GetAudio()).System;
            var channel = default(FMOD_CHANNEL*);
            var channelgroup = ChannelGroup;

            result = FMOD_System_PlaySound(system, sound, channelgroup, false, &channel);
            if (result != FMOD_OK)
                throw new FmodException(result);
        }

        /// <inheritdoc/>
        public override void Play(Single volume, Single pitch, Single pan)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            var result = default(FMOD_RESULT);

            var system = ((FmodAudioSubsystem)FrameworkContext.GetAudio()).System;
            var channel = default(FMOD_CHANNEL*);
            var channelgroup = ChannelGroup;

            result = FMOD_System_PlaySound(system, sound, channelgroup, true, &channel);
            if (result != FMOD_OK)
                throw new FmodException(result);

            result = FMOD_Channel_SetVolume(channel, MathUtility.Clamp(volume, 0f, 1f));
            if (result != FMOD_OK)
                throw new FmodException(result);

            result = FMOD_Channel_SetPitch(channel, 1f + MathUtility.Clamp(volume, -1f, 1f));
            if (result != FMOD_OK)
                throw new FmodException(result);

            result = FMOD_Channel_SetPan(channel, MathUtility.Clamp(volume, -1f, 1f));
            if (result != FMOD_OK)
                throw new FmodException(result);

            result = FMOD_Channel_SetPaused(channel, false);
            if (result != FMOD_OK)
                throw new FmodException(result);
        }

        /// <inheritdoc/>
        public override TimeSpan Duration => duration;
        
        /// <summary>
        /// Gets the FMOD sound pointer for this object.
        /// </summary>
        internal FMOD_SOUND* Sound => sound;

        /// <summary>
        /// Gets the FMOD channel group for this object.
        /// </summary>
        internal FMOD_CHANNELGROUP* ChannelGroup => ((FmodAudioSubsystem)FrameworkContext.GetAudio()).ChannelGroupSoundEffects;

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (Disposed)
                return;

            var result = FMOD_Sound_Release(sound);
            if (result != FMOD_OK)
                throw new FmodException(result);

            base.Dispose(disposing);
        }

        // FMOD state variables.
        private readonly FMOD_SOUND* sound;
        private readonly TimeSpan duration;
    }
}
