using Sedulous.Core.Messages;
using Sedulous.Sdl2.Native;

namespace Sedulous.Sdl2.Messages
{
    /// <summary>
    /// Represents the message data for an SDL2Event message.
    /// </summary>
    public sealed class Sdl2EventMessageData : MessageData
    {
        /// <summary>
        /// Gets or sets the SDL event data.
        /// </summary>
        public SDL_Event Event
        {
            get;
            set;
        }
    }
}
