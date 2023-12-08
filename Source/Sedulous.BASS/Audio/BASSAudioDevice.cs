using System;
using Sedulous.Audio;
using Sedulous.Core;

namespace Sedulous.Bass.Audio
{
    /// <summary>
    /// Represents the BASS implementation of the <see cref="IAudioDevice"/> interface.
    /// </summary>
    public sealed class BassAudioDevice : FrameworkResource, IAudioDevice
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BassAudioDevice"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="id">The device's BASS identifier.</param>
        /// <param name="name">The device's name.</param>
        public BassAudioDevice(FrameworkContext context, UInt32 id, String name)
            : base(context)
        {
            Contract.Require(name, nameof(name));

            this.Id = id;
            this.Name = name;
        }

        /// <summary>
        /// Gets the device's BASS identifier.
        /// </summary>
        public UInt32 Id { get; }

        /// <inheritdoc/>
        public String Name { get; }

        /// <inheritdoc/>
        public Boolean IsDefault { get; internal set; }

        /// <inheritdoc/>
        public Boolean IsValid { get; internal set; }
    }
}
