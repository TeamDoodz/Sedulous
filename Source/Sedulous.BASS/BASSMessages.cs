namespace Sedulous.Bass
{
    /// <summary>
    /// Contains the implementation's Sedulous engine events.
    /// </summary>
    public static class BassMessages
    {
        /// <summary>
        /// An event indicating that the current BASS device has changed.
        /// </summary>
        public static readonly FrameworkMessageId BassDeviceChanged = FrameworkMessageId.Acquire(nameof(BassDeviceChanged));
    }
}
