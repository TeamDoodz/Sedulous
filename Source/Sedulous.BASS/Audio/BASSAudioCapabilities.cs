using System;
using Sedulous.Audio;

namespace Sedulous.Bass.Audio
{
    /// <inheritdoc/>
    public sealed class BassAudioCapabilities : AudioCapabilities
    {
        /// <inheritdoc/>
        public override Boolean SupportsPitchShifting { get; } = false;
    }
}
