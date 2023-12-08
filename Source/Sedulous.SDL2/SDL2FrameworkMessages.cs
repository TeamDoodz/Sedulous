
namespace Sedulous.Sdl2
{
    /// <summary>
    /// Contains the implementation's Sedulous engine events.
    /// </summary>
    public static class Sdl2FrameworkMessages
    {
        /// <summary>
        /// An event indicating that an SDL event was raised.
        /// </summary>
        public static readonly FrameworkMessageId SdlEvent = FrameworkMessageId.Acquire(nameof(SdlEvent));
    }
}
