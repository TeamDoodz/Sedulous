using System;
using static Sedulous.Sdl2.Native.SDLNative;

namespace Sedulous.Sdl2
{
    /// <summary>
    /// Represents an exception thrown as a result of an SDL2 API error.
    /// </summary>
    [Serializable]
    public sealed class Sdl2Exception : Exception
    {
        /// <summary>
        /// Initializes a new instance of the SDL2Exception class.
        /// </summary>
        public Sdl2Exception()
            : base(SDL_GetError())
        {

        }
    }
}
