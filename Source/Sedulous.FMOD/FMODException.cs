using System;
using Sedulous.Fmod.Native;
using static Sedulous.Fmod.Native.FMODNative;

namespace Sedulous.Fmod
{
    /// <summary>
    /// Represents an exception thrown as a result of an FMOD API error.
    /// </summary>
    [Serializable]
    public sealed class FmodException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FmodException"/> class.
        /// </summary>
        public FmodException(FMOD_RESULT result)
            : base(FMOD_ErrorString(result))
        {

        }
    }
}
