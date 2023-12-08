using System;
using System.Runtime.InteropServices;
using Sedulous.Audio;
using Sedulous.Bass.Native;
using Sedulous.Core;
using Sedulous.Platform;
using static Sedulous.Bass.Native.BASSNative;

namespace Sedulous.Bass.Audio
{
    /// <summary>
    /// Represents the BASS implementation of the <see cref="SoundEffect"/> class.
    /// </summary>
    public sealed class BassSoundEffect : SoundEffect
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BassSoundEffect"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="filename">The filename of the sample to load.</param>
        public BassSoundEffect(FrameworkContext context, String filename)
            : base(context)
        {
            Contract.Require(filename, nameof(filename));

            var fileSystemService = FileSystemService.Create();
            var fileData = default(Byte[]);

            using (var stream = fileSystemService.OpenRead(filename))
            {
                fileData = new Byte[stream.Length];
                stream.Read(fileData, 0, fileData.Length);
            }

            InitializeSampleData(fileData, out this.sample, out this.sampleInfo, out this.sampleData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BassSoundEffect"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="fileData">An array containing the sample data to load.</param>
        public BassSoundEffect(FrameworkContext context, Byte[] fileData)
            : base(context)
        {
            Contract.Require(fileData, nameof(fileData));

            InitializeSampleData(fileData, out this.sample, out this.sampleInfo, out this.sampleData);
        }
        
        /// <summary>
        /// Gets the sound effect's sample information.
        /// </summary>
        /// <param name="data">The sound effect's raw PCM sample data.</param>
        /// <param name="info">The sound effect's sample info.</param>
        /// <returns>The handle to the sound effect's BASS sample.</returns>
        public UInt32 GetSampleInfo(out IntPtr data, out BASS_SAMPLE info)
        {
            data = this.sampleData;
            info = this.sampleInfo;
            return sample;
        }

        /// <inheritdoc/>
        public override void Play()
        {
            Contract.EnsureNotDisposed(this, Disposed);

            var channel = BASS_SampleGetChannel(sample, false);
            if (!BassUtility.IsValidHandle(channel))
                throw new BassException();

            if (!BASS_ChannelPlay(channel, true))
                throw new BassException();
        }

        /// <inheritdoc/>
        public override void Play(Single volume, Single pitch, Single pan)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            var channel = BASS_SampleGetChannel(sample, false);
            if (!BassUtility.IsValidHandle(channel))
                throw new BassException();

            BassUtility.SetVolume(channel, MathUtility.Clamp(volume, 0f, 1f));
            BassUtility.SetPan(channel, MathUtility.Clamp(pan, -1f, 1f));

            if (!BASS_ChannelPlay(channel, false))
                throw new BassException();
        }

        /// <inheritdoc/>
        public override TimeSpan Duration => BassUtility.IsValidHandle(sample) ? BassUtility.GetDurationAsTimeSpan(sample) : TimeSpan.Zero;

        /// <inheritdoc/>
        protected override void Dispose(Boolean disposing)
        {
            if (Disposed)
                return;

            if (!BASS_SampleFree(sample))
                throw new BassException();

            if (this.sampleData != IntPtr.Zero)
                Marshal.FreeHGlobal(this.sampleData);

            this.sample = 0;
            this.sampleData = IntPtr.Zero;

            base.Dispose(disposing);
        }

        /// <summary>
        /// Initializes a <see cref="BassSoundEffect"/> instance from the specified data.
        /// </summary>
        private static void InitializeSampleData(Byte[] fileData, out UInt32 sample, out BASS_SAMPLE sampleInfo, out IntPtr sampleData)
        {
            sample = BASS_SampleLoad(fileData, 0, (UInt32)fileData.Length, UInt16.MaxValue, 0);
            if (!BassUtility.IsValidHandle(sample))
                throw new BassException();

            if (!BASS_SampleGetInfo(sample, out sampleInfo))
                throw new BassException();

            sampleData = Marshal.AllocHGlobal((Int32)sampleInfo.length);
            if (!BASS_SampleGetData(sample, sampleData))
                throw new BassException();
        }

        // The sound effect's sample data.
        private IntPtr sampleData;
        private UInt32 sample;
        private BASS_SAMPLE sampleInfo;
    }
}