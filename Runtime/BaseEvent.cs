using System;

namespace com.lokojigen.events
{
    /// <summary>
    /// Inherit from this class to create your events and transfer data (Data transfer object aka DTO).
    /// </summary>
    public abstract class BaseEvent
    {
        /// <summary>
        /// SenderTag is a string that identifies the sender of the event.
        /// It can be used to filter or identify the source of the event.
        /// </summary>
        public string SenderTag { get; private set; }

        /// <summary>
        /// Timestamp is the time when the event was created.
        /// It is set to the current UTC time by default, but can be overridden when creating the event.
        /// This can be useful for debugging or logging purposes.
        /// </summary>
        public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// BaseEvent constructor initializes the SenderTag and Timestamp properties.
        /// If SenderTag is null or empty, it defaults to "Unknown".
        /// </summary>
        public BaseEvent(string senderTag = null, DateTime? timestamp = null)
        {
            SenderTag = string.IsNullOrEmpty(senderTag) ? "Unknown" : senderTag;
            Timestamp = timestamp ?? DateTime.UtcNow;
        }

        public override string ToString()
        {
            return $"[{GetType().Name}] From: {SenderTag} @ {Timestamp:HH:mm:ss}";
        }
    }
}