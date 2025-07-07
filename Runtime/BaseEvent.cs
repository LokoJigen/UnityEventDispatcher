using System;

namespace com.lokojigen.events
{
    /// <summary>
    /// Inherit from this class to create your events and transfer data (Data transfer object aka DTO).
    /// </summary>
    public abstract class BaseEvent
    {
        public BaseEvent(string senderTag = null, DateTime? timestamp = null)
        {
            SenderTag = senderTag ?? "Unknown";
            Timestamp = timestamp ?? DateTime.UtcNow;
        }

        public string SenderTag { get; private set; }
        public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

        public virtual string DebugInfo => null;

        public override string ToString()
        {
            return $"[{GetType().Name}] From: {SenderTag} @ {Timestamp:HH:mm:ss} {(string.IsNullOrEmpty(DebugInfo) ? "" : $"→ {DebugInfo}")}";
        }
    }
}