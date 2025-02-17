﻿using System;
using Sedulous.Audio;
using Sedulous.Core;
using Sedulous.Fmod.Native;
using static Sedulous.Fmod.Native.FMOD_MODE;
using static Sedulous.Fmod.Native.FMOD_RESULT;
using static Sedulous.Fmod.Native.FMOD_TIMEUNIT;
using static Sedulous.Fmod.Native.FMODNative;

namespace Sedulous.Fmod.Audio
{
    /// <summary>
    /// Contains methods for playing and manipulating FMOD channels.
    /// </summary>
    internal sealed unsafe class FmodChannelPlayer : FrameworkResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FmodChannelPlayer"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        public FmodChannelPlayer(FrameworkContext context)
            : base(context)
        {

        }

        /// <inheritdoc cref="SongPlayer.Update(FrameworkTime)"/>
        public void Update(FrameworkTime time)
        {
            if (State != PlaybackState.Stopped)
            {
                UpdateSlidingVolume(time);
                UpdateSlidingPitch(time);
                UpdateSlidingPan(time);
            }
        }

        /// <inheritdoc cref="SongPlayer.Play(Song, Boolean)"/>
        public Boolean Play(FMOD_SOUND* sound, FMOD_CHANNELGROUP* channelgroup, TimeSpan duration, Boolean loop = false)
        {
            return PlayInternal(sound, channelgroup, duration, 1f, 0f, 0f,
                loop ? TimeSpan.Zero : (TimeSpan?)null, null);
        }

        /// <inheritdoc cref="SongPlayer.Play(Song, TimeSpan, TimeSpan?)"/>
        public Boolean Play(FMOD_SOUND* sound, FMOD_CHANNELGROUP* channelgroup, TimeSpan duration, TimeSpan loopStart, TimeSpan? loopLength)
        {
            return PlayInternal(sound, channelgroup, duration, 1f, 0f, 0f, 
                loopStart, loopLength ?? Duration - loopStart);
        }

        /// <inheritdoc cref="SongPlayer.Play(Song, Single, Single, Single, Boolean)"/>
        public Boolean Play(FMOD_SOUND* sound, FMOD_CHANNELGROUP* channelgroup, TimeSpan duration, Single volume, Single pitch, Single pan, Boolean loop = false)
        {
            return PlayInternal(sound, channelgroup, duration, volume, pitch, pan,
                loop ? TimeSpan.Zero : (TimeSpan?)null, null);
        }

        /// <inheritdoc cref="SongPlayer.Play(Song, Single, Single, Single, TimeSpan, TimeSpan?)"/>
        public Boolean Play(FMOD_SOUND* sound, FMOD_CHANNELGROUP* channelgroup, TimeSpan duration, Single volume, Single pitch, Single pan, TimeSpan loopStart, TimeSpan? loopLength)
        {
            return PlayInternal(sound, channelgroup, duration, volume, pitch, pan,
                loopStart, loopLength ?? Duration - loopStart);
        }

        /// <inheritdoc cref="SongPlayer.Stop"/>
        public Boolean Stop()
        {
            if (channel == null)
                return false;

            var result = FMOD_Channel_Stop(channel);
            if (!ValidateHandle(result))
                return false;

            if (result != FMOD_OK)
                throw new FmodException(result);

            StopInternal();

            return true;
        }

        /// <inheritdoc cref="SongPlayer.Pause"/>
        public Boolean Pause()
        {
            if (State == PlaybackState.Playing)
            {
                var result = FMOD_Channel_SetPaused(channel, true);
                if (!ValidateHandle(result))
                    return false;

                if (result != FMOD_OK)
                    throw new FmodException(result);

                return true;
            }
            return false;
        }

        /// <inheritdoc cref="SongPlayer.Resume"/>
        public Boolean Resume()
        {
            if (State == PlaybackState.Paused)
            {
                var result = FMOD_Channel_SetPaused(channel, false);
                if (!ValidateHandle(result))
                    return false;

                if (result != FMOD_OK)
                    throw new FmodException(result);

                return true;
            }
            return false;
        }

        /// <inheritdoc cref="SongPlayer.SlideVolume(Single, TimeSpan)"/>
        public void SlideVolume(Single volume, TimeSpan time)
        {
            if (State == PlaybackState.Stopped)
                throw new InvalidOperationException(FmodStrings.NotCurrentlyValid);

            this.isSlidingVolume = true;
            this.slideStartVolume = this.Volume;
            this.slideEndVolume = volume;
            this.slideClockVolume = 0.0;
            this.slideDurationVolume = time.TotalMilliseconds;
        }

        /// <inheritdoc cref="SongPlayer.SlidePitch(Single, TimeSpan)"/>
        public void SlidePitch(Single pitch, TimeSpan time)
        {
            if (State == PlaybackState.Stopped)
                throw new InvalidOperationException(FmodStrings.NotCurrentlyValid);

            this.isSlidingPitch = true;
            this.slideStartPitch = this.Pitch;
            this.slideEndPitch = pitch;
            this.slideClockPitch = 0.0;
            this.slideDurationPitch = time.TotalMilliseconds;
        }

        /// <inheritdoc cref="SongPlayer.SlidePan(Single, TimeSpan)"/>
        public void SlidePan(Single pan, TimeSpan time)
        {
            if (State == PlaybackState.Stopped)
                throw new InvalidOperationException(FmodStrings.NotCurrentlyValid);

            this.isSlidingPan = true;
            this.slideStartPan = this.Pan;
            this.slideEndPan = pan;
            this.slideClockPan = 0.0;
            this.slideDurationPan = time.TotalMilliseconds;
        }

        /// <inheritdoc cref="SongPlayer.State"/>
        public PlaybackState State
        {
            get
            {
                if (channel != null)
                {
                    var result = default(FMOD_RESULT);
                    var isplaying = false;
                    var ispaused = false;

                    result = FMOD_Channel_GetPaused(channel, &ispaused);
                    if (!ValidateHandle(result))
                        return PlaybackState.Stopped;

                    if (result != FMOD_OK)
                        throw new FmodException(result);

                    if (ispaused)
                        return PlaybackState.Paused;

                    result = FMOD_Channel_IsPlaying(channel, &isplaying);
                    if (!ValidateHandle(result))
                        return PlaybackState.Stopped;

                    if (result != FMOD_OK)
                        throw new FmodException(result);

                    return isplaying ? PlaybackState.Playing : PlaybackState.Stopped;
                }
                return PlaybackState.Stopped;
            }
        }

        /// <inheritdoc cref="SongPlayer.IsPlaying"/>
        public Boolean IsPlaying => State == PlaybackState.Playing;

        /// <inheritdoc cref="SongPlayer.IsLooping"/>
        public Boolean IsLooping
        {
            get
            {
                if (channel == null)
                    return false;

                var mode = default(FMOD_MODE);
                var result = FMOD_Channel_GetMode(channel, &mode);
                if (!ValidateHandle(result))
                    return false;

                if (result != FMOD_OK)
                    throw new FmodException(result);

                return 
                    (mode & FMOD_LOOP_NORMAL) != 0 ||
                    (mode & FMOD_LOOP_BIDI) != 0;
            }
            set
            {
                if (State == PlaybackState.Stopped)
                    throw new InvalidOperationException(FmodStrings.NotCurrentlyValid);

                var result = FMOD_Channel_SetMode(channel, value ? FMOD_LOOP_NORMAL : FMOD_LOOP_OFF);
                if (!ValidateHandle(result))
                    throw new InvalidOperationException(FmodStrings.NotCurrentlyValid);

                if (result != FMOD_OK)
                    throw new FmodException(result);
            }
        }

        /// <inheritdoc cref="SongPlayer.Position"/>
        public TimeSpan Position
        {
            get
            {
                if (channel == null)
                    return TimeSpan.Zero;

                var position = 0u;
                var result = FMOD_Channel_GetPosition(channel, &position, FMOD_TIMEUNIT_MS);
                if (!ValidateHandle(result))
                    return TimeSpan.Zero;

                if (result != FMOD_OK)
                    throw new FmodException(result);

                return TimeSpan.FromMilliseconds(position);
            }
            set
            {
                if (State == PlaybackState.Stopped)
                    throw new InvalidOperationException(FmodStrings.NotCurrentlyValid);

                if (value.TotalSeconds < 0 || value > Duration)
                    throw new ArgumentOutOfRangeException(nameof(value));

                var result = FMOD_Channel_SetPosition(channel, (UInt32)value.TotalMilliseconds, FMOD_TIMEUNIT_MS);
                if (!ValidateHandle(result))
                    throw new InvalidOperationException(FmodStrings.NotCurrentlyValid);

                if (result != FMOD_OK)
                    throw new FmodException(result);
            }
        }

        /// <inheritdoc cref="SongPlayer.Duration"/>
        public TimeSpan Duration
        {
            get
            {
                if (State == PlaybackState.Stopped)
                    return TimeSpan.Zero;

                return duration;
            }
        }

        /// <inheritdoc cref="SongPlayer.Volume"/>
        public Single Volume
        {
            get => (State == PlaybackState.Stopped) ? 1f : volume;
            set
            {
                if (State == PlaybackState.Stopped)
                    throw new InvalidOperationException(FmodStrings.NotCurrentlyValid);

                var clamped = MathUtility.Clamp(value, 0f, 1f);
                var result = FMOD_Channel_SetVolume(channel, clamped);
                if (!ValidateHandle(result))
                    throw new InvalidOperationException(FmodStrings.NotCurrentlyValid);

                if (result != FMOD_OK)
                    throw new FmodException(result);

                this.volume = clamped;
                this.isSlidingVolume = false;
            }
        }

        /// <inheritdoc cref="SongPlayer.Pitch"/>
        public Single Pitch
        {
            get => (State == PlaybackState.Stopped) ? 0f : pitch;
            set
            {
                if (State == PlaybackState.Stopped)
                    throw new InvalidOperationException(FmodStrings.NotCurrentlyValid);

                var clamped = MathUtility.Clamp(value, -1f, 1f);
                var result = FMOD_Channel_SetPitch(channel, 1f + clamped);
                if (!ValidateHandle(result))
                    throw new InvalidOperationException(FmodStrings.NotCurrentlyValid);

                if (result != FMOD_OK)
                    throw new FmodException(result);

                this.pitch = clamped;
                this.isSlidingPitch = false;
            }
        }

        /// <inheritdoc cref="SongPlayer.Pan"/>
        public Single Pan
        {
            get => (State == PlaybackState.Stopped) ? 0f : pan;
            set
            {
                if (State == PlaybackState.Stopped)
                    throw new InvalidOperationException(FmodStrings.NotCurrentlyValid);

                var clamped = MathUtility.Clamp(pan, -1f, 1f);
                var result = FMOD_Channel_SetPan(channel, clamped);
                if (!ValidateHandle(result))
                    throw new InvalidOperationException(FmodStrings.NotCurrentlyValid);

                if (result != FMOD_OK)
                    throw new FmodException(result);

                this.pan = clamped;
                this.isSlidingPan = false;
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (Disposed)
                return;

            if (FrameworkContext != null && !FrameworkContext.Disposed)
                Stop();

            base.Dispose(disposing);
        }

        /// <summary>
        /// Checks to see if an FMOD call returned FMOD_ERR_INVALID_HANDLE and, if it did,
        /// clears out the player's state.
        /// </summary>
        private Boolean ValidateHandle(FMOD_RESULT result)
        {
            if (result == FMOD_ERR_INVALID_HANDLE)
            {
                StopInternal();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Plays the specified sound.
        /// </summary>
        private Boolean PlayInternal(FMOD_SOUND* sound, FMOD_CHANNELGROUP* channelgroup,
            TimeSpan duration, Single volume, Single pitch, Single pan, TimeSpan? loopStart, TimeSpan? loopLength)
        {
            Stop();

            var result = default(FMOD_RESULT);

            var system = ((FmodAudioSubsystem)FrameworkContext.GetAudio()).System;
            var channel = default(FMOD_CHANNEL*);
            
            if (loopStart > TimeSpan.Zero && loopLength <= TimeSpan.Zero)
                throw new ArgumentException(nameof(loopLength));

            var looping = loopStart.HasValue || loopLength.HasValue;
            var loopStartMs = loopStart.HasValue ? (UInt32)loopStart.Value.TotalMilliseconds : 0;
            var loopEnd = (loopStart ?? TimeSpan.Zero) + loopLength;
            var loopEndMs = loopEnd.HasValue ? (UInt32)(loopEnd.Value.TotalMilliseconds - 1) : (UInt32)(duration.TotalMilliseconds - 1);

            result = FMOD_System_PlaySound(system, sound, channelgroup, true, &channel);
            if (result != FMOD_OK)
                throw new FmodException(result);

            result = FMOD_Channel_SetVolume(channel, MathUtility.Clamp(volume, 0f, 1f));
            if (result != FMOD_OK)
                throw new FmodException(result);

            result = FMOD_Channel_SetPitch(channel, 1f + MathUtility.Clamp(pitch, -1f, 1f));
            if (result != FMOD_OK)
                throw new FmodException(result);

            result = FMOD_Channel_SetPan(channel, MathUtility.Clamp(pan, -1f, 1f));
            if (result != FMOD_OK)
                throw new FmodException(result);

            result = FMOD_Channel_SetMode(channel, looping ? FMOD_LOOP_NORMAL : FMOD_LOOP_OFF);
            if (result != FMOD_OK)
                throw new FmodException(result);

            result = FMOD_Channel_SetLoopPoints(channel, loopStartMs, FMOD_TIMEUNIT_MS, loopEndMs, FMOD_TIMEUNIT_MS);
            if (result != FMOD_OK)
                throw new FmodException(result);

            result = FMOD_Channel_SetPaused(channel, false);
            if (result != FMOD_OK)
                throw new FmodException(result);

            this.channel = channel;
            this.duration = duration;
            this.volume = volume;
            this.pitch = pitch;
            this.pan = pan;
            
            return true;
        }

        /// <summary>
        /// Stops the channel.
        /// </summary>
        private Boolean StopInternal()
        {
            channel = null;

            isSlidingVolume = false;
            isSlidingPitch = false;
            isSlidingPan = false;

            return true;
        }

        /// <summary>
        /// Updates the player's volume, if its volume is sliding.
        /// </summary>
        private void UpdateSlidingVolume(FrameworkTime time)
        {
            if (!isSlidingVolume)
                return;

            slideClockVolume += time.ElapsedTime.TotalMilliseconds;

            var factor = (Single)MathUtility.Clamp(slideClockVolume / slideDurationVolume, 0.0, 1.0);
            var volume = MathUtility.Clamp(Tweening.Lerp(slideStartVolume, slideEndVolume, factor), 0f, 1f);

            var result = FMOD_Channel_SetVolume(channel, volume);
            if (!ValidateHandle(result))
                return;

            if (result != FMOD_OK)
                throw new FmodException(result);

            this.volume = volume;

            if (factor == 1f)
                isSlidingVolume = false;
        }

        /// <summary>
        /// Updates the player's pitch, if its pitch is sliding.
        /// </summary>
        private void UpdateSlidingPitch(FrameworkTime time)
        {
            if (!isSlidingPitch)
                return;

            slideClockPitch += time.ElapsedTime.TotalMilliseconds;

            var factor = (Single)MathUtility.Clamp(slideClockPitch / slideDurationPitch, 0.0, 1.0);
            var pitch = MathUtility.Clamp(Tweening.Lerp(slideStartPitch, slideEndPitch, factor), -1f, 1f);

            var result = FMOD_Channel_SetPitch(channel, 1f + pitch);
            if (!ValidateHandle(result))
                return;

            if (result != FMOD_OK)
                throw new FmodException(result);

            this.pitch = pitch;

            if (factor == 1f)
                isSlidingPitch = false;
        }

        /// <summary>
        /// Updates the player's pan, if its pan is sliding.
        /// </summary>
        private void UpdateSlidingPan(FrameworkTime time)
        {
            if (!isSlidingPan)
                return;

            slideClockPan += time.ElapsedTime.TotalMilliseconds;

            var factor = (Single)MathUtility.Clamp(slideClockPan / slideDurationPitch, 0.0, 1.0);
            var pan = MathUtility.Clamp(Tweening.Lerp(slideStartPan, slideEndPan, factor), -1f, 1f);

            var result = FMOD_Channel_SetPan(channel, pan);
            if (!ValidateHandle(result))
                return;

            if (result != FMOD_OK)
                throw new FmodException(result);

            this.pan = pan;

            if (factor == 1f)
                isSlidingPan = false;
        }        

        // State values.
        private FMOD_CHANNEL* channel;
        private Single volume;
        private Single pitch;
        private Single pan;
        private TimeSpan duration;

        // Sliding parameters.
        private Boolean isSlidingVolume;
        private Boolean isSlidingPitch;
        private Boolean isSlidingPan;
        private Single slideStartVolume;
        private Single slideStartPitch;
        private Single slideStartPan;
        private Single slideEndVolume;
        private Single slideEndPitch;
        private Single slideEndPan;
        private Double slideClockVolume;
        private Double slideClockPitch;
        private Double slideClockPan;
        private Double slideDurationVolume;
        private Double slideDurationPitch;
        private Double slideDurationPan;
    }
}
