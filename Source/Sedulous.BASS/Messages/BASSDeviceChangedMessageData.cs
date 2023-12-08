using System;
using Sedulous.Core.Messages;

namespace Sedulous.Bass.Messages
{
    /// <summary>
    /// Represents the data for an event which indicates that the BASS device has changed.
    /// </summary>
    public sealed class BassDeviceChangedMessageData : MessageData
    {
        /// <summary>
        /// Gets or sets the identifier of the new device.
        /// </summary>
        public UInt32 DeviceId { get; set; }
    }
}
