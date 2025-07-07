namespace com.lokojigen.events
{
    /// <summary>
    /// Implement this interface in a class in your project and give the ref to EventManager.Logger.
    /// </summary>
    public interface IEventLogger
    {
        void Log(string message);
    }
}