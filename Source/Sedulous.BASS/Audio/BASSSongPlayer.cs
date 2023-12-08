﻿using System;
using System.Runtime.InteropServices;
using Sedulous.Audio;
using Sedulous.Bass.Messages;
using Sedulous.Bass.Native;
using Sedulous.Core;
using Sedulous.Core.Messages;
using static Sedulous.Bass.Native.BASSNative;

namespace Sedulous.Bass.Audio
{
    /// <summary>
    /// Represents the BASS implementation of the <see cref="SongPlayer"/> class.
    /// </summary>
    public sealed class BassSongPlayer : SongPlayer,
        IMessageSubscriber<FrameworkMessageId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BassSongPlayer"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        public BassSongPlayer(FrameworkContext context)
            : base(context)
        {
            gcHandle = GCHandle.Alloc(this, GCHandleType.Weak);

            context.Messages.Subscribe(this, BassMessages.BassDeviceChanged);
        }

        /// <inheritdoc/>
        void IMessageSubscriber<FrameworkMessageId>.ReceiveMessage(FrameworkMessageId type, MessageData data)
        {
            if (type == BassMessages.BassDeviceChanged)
            {
                if (BassUtility.IsValidHandle(stream))
                {
                    var deviceID = ((BassDeviceChangedMessageData)data).DeviceId;
                    if (!BASS_ChannelSetDevice(stream, deviceID))
                        throw new BassException();
                }
                return;
            }
        }

        /// <inheritdoc/>
        public override void Update(FrameworkTime time)
        {

        }

        /// <inheritdoc/>
        public override Boolean Play(Song song, Boolean loop = false)
        {
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.Require(song, nameof(song));

            return PlayInternal(song, 1f, 0f, 0f,
                loop ? TimeSpan.Zero : (TimeSpan?)null, null);
        }

        /// <inheritdoc/>
        public override Boolean Play(Song song, TimeSpan loopStart, TimeSpan? loopLength)
        {
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.Require(song, nameof(song));

            return PlayInternal(song, 1f, 0f, 0f, loopStart, loopLength ?? Duration - loopStart);
        }

        /// <inheritdoc/>
        public override Boolean Play(Song song, Single volume, Single pitch, Single pan, Boolean loop = false)
        {
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.Require(song, nameof(song));

            return PlayInternal(song, volume, pitch, pan,
                loop ? TimeSpan.Zero : (TimeSpan?)null, null);
        }

        /// <inheritdoc/>
        public override Boolean Play(Song song, Single volume, Single pitch, Single pan, TimeSpan loopStart, TimeSpan? loopLength)
        {
            Contract.EnsureNotDisposed(this, Disposed);
            Contract.Require(song, nameof(song));

            return PlayInternal(song, volume, pitch, pan, loopStart, loopLength ?? Duration - loopStart);
        }

        /// <inheritdoc/>
        public override void Stop()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            if (StopInternal())
            {
                OnStateChanged();
                OnSongEnded();
            }
        }

        /// <inheritdoc/>
        public override void Pause()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            if (State == PlaybackState.Playing)
            {
                if (!BASS_ChannelPause(stream))
                    throw new BassException();

                OnStateChanged();
            }
        }

        /// <inheritdoc/>
        public override void Resume()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            if (State == PlaybackState.Paused)
            {
                if (!BASS_ChannelPlay(stream, false))
                    throw new BassException();

                OnStateChanged();
            }
        }

        /// <inheritdoc/>
        public override void SlideVolume(Single volume, TimeSpan time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            EnsureChannelIsValid();

            BassUtility.SlideVolume(stream, volume, time);
        }

        /// <inheritdoc/>
        public override void SlidePitch(Single pitch, TimeSpan time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            EnsureChannelIsValid();
        }

        /// <inheritdoc/>
        public override void SlidePan(Single pan, TimeSpan time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            EnsureChannelIsValid();

            BassUtility.SlidePan(stream, pan, time);
        }

        /// <inheritdoc/>
        public override PlaybackState State
        {
            get
            {
                if (BassUtility.IsValidHandle(stream))
                {
                    switch (BASS_ChannelIsActive(stream))
                    {
                        case BASS_ACTIVE_PLAYING:
                        case BASS_ACTIVE_STALLED:
                            return PlaybackState.Playing;

                        case BASS_ACTIVE_STOPPED:
                            return PlaybackState.Stopped;

                        case BASS_ACTIVE_PAUSED:
                            return PlaybackState.Paused;
                    }
                }
                return PlaybackState.Stopped;
            }
        }

        /// <inheritdoc/>
        public override Boolean IsPlaying
        {
            get => State == PlaybackState.Playing;
        }

        /// <inheritdoc/>
        public override Boolean IsLooping
        {
            get => IsChannelValid() ? (syncLoop != 0 || BassUtility.GetIsLooping(stream)) : false;
            set
            {
                EnsureChannelIsValid();

                if (syncLoop != 0)
                {
                    if (!BASS_ChannelRemoveSync(stream, syncLoop))
                        throw new BassException();

                    syncLoop = 0;
                    syncLoopDelegate = null;
                }
                else
                {
                    BassUtility.SetIsLooping(stream, value);
                }
            }
        }

        /// <inheritdoc/>
        public override TimeSpan Position
        {
            get => IsChannelValid() ? BassUtility.GetPositionAsTimeSpan(stream) : TimeSpan.Zero;
            set 
            {
                EnsureChannelIsValid();

                if (value.TotalSeconds < 0 || value > Duration)
                    throw new ArgumentOutOfRangeException(nameof(value));

                BassUtility.SetPositionInSeconds(stream, value.TotalSeconds);
            }
        }

        /// <inheritdoc/>
        public override TimeSpan Duration
        {
            get => IsChannelValid() ? BassUtility.GetDurationAsTimeSpan(stream) : TimeSpan.Zero;
        }

        /// <inheritdoc/>
        public override Single Volume
        {
            get => IsChannelValid() ? BassUtility.GetVolume(stream) : 1f;
            set 
            {
                EnsureChannelIsValid();
                BassUtility.SetVolume(stream, MathUtility.Clamp(value, -1f, 1f));
            }
        }

        /// <inheritdoc/>
        public override Single Pitch
        {
            get => 0f;
            set
            {
                EnsureChannelIsValid();
            }
        }

        /// <inheritdoc/>
        public override Single Pan
        {
            get => IsChannelValid() ? BassUtility.GetPan(stream) : 0f;
            set
            {
                EnsureChannelIsValid();
                BassUtility.SetPan(stream, MathUtility.Clamp(value, -1f, 1f));
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (FrameworkContext != null && !FrameworkContext.Disposed)
            {
                StopInternal();
                FrameworkContext.Messages.Unsubscribe(this);
            }

            stream = 0;

            if (gcHandle.IsAllocated)
                gcHandle.Free();
            
            base.Dispose(disposing);
        }

        /// <summary>
        /// Performs custom looping when a loop range is specified.
        /// </summary>
        [MonoPInvokeCallback(typeof(SyncProc))]
        private static void SyncLoopThunk(UInt32 handle, UInt32 channel, UInt32 data, IntPtr user)
        {
            if (!BASS_ChannelSetPosition(channel, (UInt32)user, 0))
                throw new BassException();
        }

        /// <summary>
        /// Raises a callback when a song ends.
        /// </summary>
        [MonoPInvokeCallback(typeof(SyncProc))]
        private static void SyncEndThunk(UInt32 handle, UInt32 channel, UInt32 data, IntPtr user)
        {
            var gcHandle = GCHandle.FromIntPtr(user);
            ((BassSongPlayer)gcHandle.Target)?.SyncEnd(handle, channel, data, IntPtr.Zero);
        }

        /// <summary>
        /// Plays the specified song.
        /// </summary>
        private Boolean PlayInternal(Song song, Single volume, Single pitch, Single pan, TimeSpan? loopStart, TimeSpan? loopLength)
        {
            FrameworkContext.ValidateResource(song);

            Stop();

            stream = ((BassSong)song).CreateStream(0);
            if (!BassUtility.IsValidHandle(stream))
                throw new BassException();

            var autoloop = loopStart.HasValue && !loopLength.HasValue;
            var syncloop = loopStart.HasValue && !autoloop;

            BassUtility.SetIsLooping(stream, autoloop);
            BassUtility.SetVolume(stream, MathUtility.Clamp(volume, 0f, 1f));
            BassUtility.SetPan(stream, MathUtility.Clamp(pan, -1f, 1f));

            if (loopStart > TimeSpan.Zero && loopLength <= TimeSpan.Zero)
                throw new ArgumentException(nameof(loopLength));

            if (syncloop)
            {
                var loopStartInBytes = BASS_ChannelSeconds2Bytes(stream, loopStart.Value.TotalSeconds);
                var loopEndInBytes = BASS_ChannelSeconds2Bytes(stream, (loopStart + loopLength).Value.TotalSeconds);
                syncLoopDelegate = SyncLoopThunk;
                syncLoop = BASS_ChannelSetSync(stream, BASS_SYNC_POS, loopEndInBytes, syncLoopDelegate, new IntPtr((Int32)loopStartInBytes));
                if (syncLoop == 0)
                    throw new BassException();
            }

            syncEndDelegate = SyncEndThunk;
            syncEnd = BASS_ChannelSetSync(stream, BASS_SYNC_END, 0, syncEndDelegate, GCHandle.ToIntPtr(gcHandle));
            if (syncEnd == 0)
                throw new BassException();

            if (!BASS_ChannelPlay(stream, true))
                throw new BassException();

            OnStateChanged();
            OnSongStarted();

            return true;
        }

        /// <summary>
        /// Releases the memory associated with the underlying stream object.
        /// </summary>
        private Boolean StopInternal()
        {
            if (stream == 0)
                return false;

            if (!BASS_StreamFree(stream))
                throw new BassException();

            stream = 0;

            syncLoopDelegate = null;
            syncLoop = 0;

            syncEndDelegate = null;
            syncEnd = 0;

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the channel is in a valid state.
        /// </summary>
        /// <returns>true if the channel is in a valid state; otherwise, false.</returns>
        private Boolean IsChannelValid()
        {
            return State != PlaybackState.Stopped;
        }

        /// <summary>
        /// Throws an <see cref="System.InvalidOperationException"/> if the channel is not in a valid state.
        /// </summary>
        private void EnsureChannelIsValid()
        {
            if (State == PlaybackState.Stopped)
                throw new InvalidOperationException(BassStrings.NotCurrentlyValid);
        }

        /// <summary>
        /// Raises a callback when a song ends.
        /// </summary>
        private void SyncEnd(UInt32 handle, UInt32 channel, UInt32 data, IntPtr user)
        {
            if (!IsLooping)
            {
                if (StopInternal())
                {
                    OnStateChanged();
                    OnSongEnded();
                }
            }
        }

        // State values.
        private GCHandle gcHandle;
        private UInt32 stream;
        private UInt32 syncLoop;
        private UInt32 syncEnd;
        private SyncProc syncLoopDelegate;
        private SyncProc syncEndDelegate;
    }
}
