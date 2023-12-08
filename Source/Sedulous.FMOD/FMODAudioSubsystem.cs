using System;
using System.Collections.Generic;
using System.Text;
using Sedulous.Audio;
using Sedulous.Core;
using Sedulous.Core.Messages;
using Sedulous.Fmod.Audio;
using Sedulous.Fmod.Native;
using Sedulous.Platform;
using static Sedulous.Fmod.Native.FMOD_DEBUG_FLAGS;
using static Sedulous.Fmod.Native.FMOD_DEBUG_MODE;
using static Sedulous.Fmod.Native.FMOD_INITFLAGS;
using static Sedulous.Fmod.Native.FMOD_RESULT;
using static Sedulous.Fmod.Native.FMODNative;

namespace Sedulous.Fmod
{
    /// <summary>
    /// Represents the FMOD implementation of the Sedulous audio subsystem.
    /// </summary>
    public sealed unsafe partial class FmodAudioSubsystem : FrameworkResource, IAudioSubsystem, IMessageSubscriber<FrameworkMessageId>
    {
        /// <summary>
        /// Initializes a new instance of the FMODSedulousAudio class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="configuration">The Sedulous configuration.</param>
        public FmodAudioSubsystem(FrameworkContext context, FrameworkConfiguration configuration)
            : base(context)
        {
            platformSpecificImpl = FmodPlatformSpecificImplementationDetails.Create();
            platformSpecificImpl.OnInitialized();

            this.Capabilities = new FmodAudioCapabilities();

            InitializeLogging(configuration);

            FMOD_RESULT result;
            fixed (FMOD_SYSTEM** psystem = &system)
            {
                result = FMOD_System_Create(psystem);
                if (result != FMOD_OK)
                    throw new FmodException(result);
            }

            var version = 0u;
            result = FMOD_System_GetVersion(system, &version);
            if (result != FMOD_OK)
                throw new FmodException(result);
            
            if (version < FMOD_VERSION)
                throw new Exception(FmodStrings.FMODVersionMismatch.Format(FMOD_VERSION, version));

            var extradriverdata = default(void*);
            result = FMOD_System_Init(system, 256, FMOD_INIT_NORMAL, extradriverdata);
            if (result != FMOD_OK)
                throw new FmodException(result);

            fixed (FMOD_CHANNELGROUP** pcgroupSongs = &cgroupSongs)
            {
                result = FMOD_System_CreateChannelGroup(system, "Songs", pcgroupSongs);
                if (result != FMOD_OK)
                    throw new FmodException(result);
            }

            fixed (FMOD_CHANNELGROUP** pcgroupSoundEffects = &cgroupSoundEffects)
            {
                result = FMOD_System_CreateChannelGroup(system, "Sound Effects", pcgroupSoundEffects);
                if (result != FMOD_OK)
                    throw new FmodException(result);
            }

            UpdateFileSource();
            UpdateAudioDevices();
            PlaybackDevice = GetDefaultDevice();
            
            systemDeviceCallbackFMOD = DeviceCallback;
            
            result = FMOD_System_SetCallback(system, systemDeviceCallbackFMOD, FMOD_SYSTEM_CALLBACK_TYPE.DEVICELISTCHANGED | FMOD_SYSTEM_CALLBACK_TYPE.DEVICELOST);
            if (result != FMOD_OK)
                throw new FmodException(result);

            context.Messages.Subscribe(this, FrameworkMessages.ApplicationCreated);
            context.Messages.Subscribe(this, FrameworkMessages.ApplicationTerminating);
            context.Messages.Subscribe(this, FrameworkMessages.ApplicationSuspending);
            context.Messages.Subscribe(this, FrameworkMessages.ApplicationResumed);
            context.Messages.Subscribe(this, FrameworkMessages.FileSourceChanged);
        }

        /// <inheritdoc/>
        void IMessageSubscriber<FrameworkMessageId>.ReceiveMessage(FrameworkMessageId type, MessageData data)
        {
            if (type == FrameworkMessages.ApplicationCreated)
            {
                platformSpecificImpl.OnApplicationCreated();
                return;
            }

            if (type == FrameworkMessages.ApplicationTerminating)
            {
                platformSpecificImpl.OnApplicationTerminating();
                return;
            }

            if (type == FrameworkMessages.ApplicationSuspending)
            {
                if (!suspended)
                {
                    awaitingResume = true;
                    Suspend();
                }
                return;
            }

            if (type == FrameworkMessages.ApplicationResumed)
            {
                if (awaitingResume)
                {
                    awaitingResume = false;
                    Resume();
                }
                return;
            }

            if (type == FrameworkMessages.FileSourceChanged)
            {
                UpdateFileSource();
                return;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<IAudioDevice> EnumerateAudioDevices()
        {
            Contract.EnsureNotDisposed(this, Disposed);
            
            return knownAudioDevices;
        }

        /// <inheritdoc/>
        public IAudioDevice FindAudioDeviceByName(String name)
        {
            Contract.Require(name, nameof(name));
            Contract.EnsureNotDisposed(this, Disposed);

            foreach (var device in knownAudioDevices)
            {
                if (device.IsValid && String.Equals(name, device.Name, StringComparison.Ordinal))
                    return device;
            }

            return null;
        }

        /// <inheritdoc/>
        public void Update(FrameworkTime time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            var result = default(FMOD_RESULT);

            result = FMOD_System_Update(system);
            if (result != FMOD_OK)
                throw new FmodException(result);
            
            Updating?.Invoke(this, time);
        }

        /// <inheritdoc/>
        public void Suspend()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            var result = default(FMOD_RESULT);

            result = FMOD_System_MixerSuspend(system);
            if (result != FMOD_OK)
                throw new FmodException(result);

            suspended = true;
        }

        /// <inheritdoc/>
        public void Resume()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            var result = default(FMOD_RESULT);

            result = FMOD_System_MixerResume(system);
            if (result != FMOD_OK)
                throw new FmodException(result);

            suspended = false;
        }

        /// <inheritdoc/>
        public IAudioDevice PlaybackDevice
        {
            get => playbackDevice;
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                var val = value ?? GetDefaultDevice();
                if (val == null)
                {
                    playbackDevice = null;
                }
                else
                {
                    if (val is FmodAudioDevice device)
                    {
                        FrameworkContext.ValidateResource(device);

                        var result = default(FMOD_RESULT);
                        var olddriver = 0;

                        result = FMOD_System_GetDriver(system, &olddriver);
                        if (result != FMOD_OK)
                            throw new FmodException(result);

                        result = FMOD_System_SetDriver(system, device.Id);
                        if (result == FMOD_ERR_OUTPUT_INIT)
                        {
                            result = FMOD_System_SetDriver(system, olddriver);
                            if (result != FMOD_OK)
                                throw new FmodException(result);

                            return;
                        }

                        if (result != FMOD_OK)
                            throw new FmodException(result);

                        playbackDevice = device;
                    }
                    else throw new ArgumentException(nameof(value));
                }
            }
        }

        /// <inheritdoc/>
        public AudioCapabilities Capabilities { get; }

        /// <inheritdoc/>
        public Single AudioMasterVolume
        {
            get => audioMasterVolume;
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                var audioMasterVolumeClamped = MathUtility.Clamp(value, 0f, 1f);
                if (audioMasterVolumeClamped != audioMasterVolume)
                {
                    audioMasterVolume = audioMasterVolumeClamped;

                    UpdateSoundVolume();
                    UpdateSongsVolume();
                }
            }
        }

        /// <inheritdoc/>
        public Single SongsMasterVolume
        {
            get => songsMasterVolume;
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                var songsMasterVolumeClamped = MathUtility.Clamp(value, 0f, 1f);
                if (songsMasterVolumeClamped != songsMasterVolume)
                {
                    songsMasterVolume = songsMasterVolumeClamped;

                    UpdateSongsVolume();
                }
            }
        }

        /// <inheritdoc/>
        public Single SoundEffectsMasterVolume
        {
            get => soundEffectsMasterVolume;
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                var soundEffectsMasterVolumeClamped = MathUtility.Clamp(value, 0f, 1f);
                if (soundEffectsMasterVolumeClamped != soundEffectsMasterVolume)
                {
                    soundEffectsMasterVolume = soundEffectsMasterVolumeClamped;

                    UpdateSoundVolume();
                }
            }
        }

        /// <inheritdoc/>
        public Boolean AudioMuted
        {
            get => audioMuted;
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                if (audioMuted != value)
                {
                    audioMuted = value;

                    UpdateSongsVolume();
                    UpdateSoundVolume();
                }
            }
        }

        /// <inheritdoc/>
        public Boolean SongsMuted
        {
            get => songsMuted;
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                if (songsMuted != value)
                {
                    songsMuted = value;

                    UpdateSongsVolume();
                }
            }
        }

        /// <inheritdoc/>
        public Boolean SoundEffectsMuted
        {
            get => soundEffectsMuted;
            set
            {
                Contract.EnsureNotDisposed(this, Disposed);

                if (soundEffectsMuted != value)
                {
                    soundEffectsMuted = value;

                    UpdateSoundVolume();
                }
            }
        }

        /// <inheritdoc/>
        public event FrameworkSubsystemUpdateEventHandler Updating;

        /// <summary>
        /// Gets a pointer to the FMOD system object.
        /// </summary>
        internal FMOD_SYSTEM* System => system;

        /// <summary>
        /// Gets a pointer to the FMOD channel group for songs.
        /// </summary>
        internal FMOD_CHANNELGROUP* ChannelGroupSongs => cgroupSongs;

        /// <summary>
        /// Gets a pointer to the FMOD channel group for sound effects.
        /// </summary>
        internal FMOD_CHANNELGROUP* ChannelGroupSoundEffects => cgroupSoundEffects;

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            var result = default(FMOD_RESULT);

            result = FMOD_ChannelGroup_Release(cgroupSongs);
            if (result != FMOD_OK)
                throw new FmodException(result);

            result = FMOD_ChannelGroup_Release(cgroupSoundEffects);
            if (result != FMOD_OK)
                throw new FmodException(result);

            result = FMOD_System_Release(system);
            if (result != FMOD_OK)
                throw new FmodException(result);

            if (disposing && !FrameworkContext.Disposed)
            {
                FrameworkContext.Messages.Unsubscribe(this);
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Represents a thunk which allows FMOD to call into the managed debug callback.
        /// </summary>
        [MonoPInvokeCallback(typeof(FMOD_DEBUG_CALLBACK))]
        private static FMOD_RESULT DebugCallbackThunk(FMOD_DEBUG_FLAGS flags, String file, Int32 line, String func, String message)
        {
            var messageString = $"FMOD: {message.Trim()}";
            var messageLevel = DebugLevels.Info;
            switch (flags)
            {
                case FMOD_DEBUG_LEVEL_WARNING:
                    messageLevel = DebugLevels.Warning;
                    break;

                case FMOD_DEBUG_LEVEL_ERROR:
                    messageLevel = DebugLevels.Error;
                    break;
            }
            
            if (FrameworkContext.RequestCurrent()?.GetAudio() is FmodAudioSubsystem audio)
                audio.debugCallback?.Invoke(audio.FrameworkContext, messageLevel, messageString);

            return FMOD_OK;
        }

        /// <summary>
        /// Callback fired by FMOD in update() method when the device list change or when a device is lost
        /// </summary>
        [MonoPInvokeCallback(typeof(FMOD_SYSTEM_CALLBACK))]
        private FMOD_RESULT DeviceCallback(void* system1, FMOD_SYSTEM_CALLBACK_TYPE type, void* commanddata1, void* commanddata2, void* userdata)
        {
            UpdateAudioDevices();
            return FMOD_OK;
        }
        
        /// <summary>
        /// Initializes FMOD's logging system.
        /// </summary>
        private void InitializeLogging(FrameworkConfiguration configuration)
        {
            var result = default(FMOD_RESULT);
            
            debugCallback = configuration.DebugCallback;
            debugCallbackFMOD = DebugCallbackThunk;

            var flags = FMOD_DEBUG_LEVEL_NONE;
            if ((configuration.DebugLevels & DebugLevels.Error) == DebugLevels.Error)
                flags |= FMOD_DEBUG_LEVEL_ERROR;
            if ((configuration.DebugLevels & DebugLevels.Warning) == DebugLevels.Warning)
                flags |= FMOD_DEBUG_LEVEL_WARNING;
            if ((configuration.DebugLevels & DebugLevels.Info) == DebugLevels.Info)
                flags |= FMOD_DEBUG_LEVEL_LOG;

            var mode = (configuration.DebugCallback != null) ? FMOD_DEBUG_MODE_CALLBACK : FMOD_DEBUG_MODE_FILE;

            result = FMOD_Debug_Initialize(flags, mode, debugCallbackFMOD, "fmod.log");
            if (result == FMOD_ERR_UNSUPPORTED)
                return;
            if (result != FMOD_OK)
                throw new FmodException(result);
        }

        /// <summary>
        /// Updates the global volume of the Songs channel group to match the subsystem's current settings.
        /// </summary>
        private void UpdateSongsVolume()
        {
            var volumeSongs = (audioMuted || songsMuted) ? 0f : audioMasterVolume * songsMasterVolume;

            var result = FMOD_ChannelGroup_SetVolume(cgroupSongs, volumeSongs);
            if (result != FMOD_OK)
                throw new FmodException(result);
        }

        /// <summary>
        /// Updates the global volume of the Sound Effects channel group to match the subsystem's current settings.
        /// </summary>
        private void UpdateSoundVolume()
        {
            var volumeSoundEffects = (audioMuted || soundEffectsMuted) ? 0f : audioMasterVolume * soundEffectsMasterVolume;

            var result = FMOD_ChannelGroup_SetVolume(cgroupSoundEffects, volumeSoundEffects);
            if (result != FMOD_OK)
                throw new FmodException(result);
        }

        /// <summary>
        /// Updates the list of audio devices.
        /// </summary>
        private void UpdateAudioDevices()
        {
            var numdrivers = 0;
            var result = FMOD_System_GetNumDrivers(system, &numdrivers);
            if (result != FMOD_OK)
                throw new FmodException(result);
            
            if (numdrivers != knownAudioDevices.Count)
            {
                foreach (var device in knownAudioDevices)
                    device.IsValid = false;

                knownAudioDevices.Clear();

                var namebuf = new StringBuilder(256);
                var namelen = namebuf.Capacity;

                for (var i = 0; i < numdrivers; i++)
                {
                    result = FMOD_System_GetDriverInfo(system, i, namebuf, namelen, null, null, null, null);
                    if (result != FMOD_OK)
                        throw new FmodException(result);

                    var device = new FmodAudioDevice(FrameworkContext, i, namebuf.ToString())
                    {
                        IsValid = true,
                        IsDefault = i == 0
                    };

                    knownAudioDevices.Add(device);
                }
            }

            if (playbackDevice != null && !playbackDevice.IsValid)
                this.PlaybackDevice = FindAudioDeviceByName(PlaybackDevice.Name) ?? GetDefaultDevice();
        }

        /// <summary>
        /// Updates the file source from which FMOD loads files.
        /// </summary>
        private void UpdateFileSource()
        {
            var result = default(FMOD_RESULT);

            this.fileSource = FileSystemService.Source;

            if (fileSource == null)
            {
                result = FMOD_System_SetFileSystem(system, null, null, null, null, null, null, -1);
                if (result != FMOD_OK)
                    throw new FmodException(result);
            }
            else
            {
                result = FMOD_System_SetFileSystem(system,
                    new FMOD_FILE_OPEN_CALLBACK(FmodFileSystem.UserOpen),
                    new FMOD_FILE_CLOSE_CALLBACK(FmodFileSystem.UserClose),
                    new FMOD_FILE_READ_CALLBACK(FmodFileSystem.UserRead),
                    new FMOD_FILE_SEEK_CALLBACK(FmodFileSystem.UserSeek), null, null, -1);
                if (result != FMOD_OK)
                    throw new FmodException(result);
            }
        }

        /// <summary>
        /// Gets the default audio device.
        /// </summary>
        private FmodAudioDevice GetDefaultDevice()
        {
            return knownAudioDevices.Count == 0 ? null : knownAudioDevices[0];
        }

        // FMOD state variables.
        private readonly FMOD_SYSTEM* system;
        private readonly FMOD_CHANNELGROUP* cgroupSongs;
        private readonly FMOD_CHANNELGROUP* cgroupSoundEffects;

        // Property values.
        private Single audioMasterVolume = 1.0f;
        private Single songsMasterVolume = 1.0f;
        private Single soundEffectsMasterVolume = 1.0f;
        private Boolean audioMuted;
        private Boolean songsMuted;
        private Boolean soundEffectsMuted;

        // State values.
        private FileSource fileSource;
        private Boolean suspended;
        private Boolean awaitingResume;

        // Audio device cache.
        private FmodAudioDevice playbackDevice;
        private List<FmodAudioDevice> knownAudioDevices = new List<FmodAudioDevice>();
        
        // Debug output callbacks.
        private DebugCallback debugCallback;
        private FMOD_DEBUG_CALLBACK debugCallbackFMOD;

        // System callbacks.
        private FMOD_SYSTEM_CALLBACK systemDeviceCallbackFMOD;
        
        // Platform-specific details.
        private readonly FmodPlatformSpecificImplementationDetails platformSpecificImpl;
    }
}
