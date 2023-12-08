using System;
using Sedulous.Audio;

namespace Sedulous.Fmod.Audio
{
    /// <inheritdoc/>
    public sealed class FmodAudioCapabilities : AudioCapabilities
    {
        /// <inheritdoc/>
        public override Boolean SupportsPitchShifting { get; } = true;
    }
}
