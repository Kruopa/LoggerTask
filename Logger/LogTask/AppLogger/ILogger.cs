namespace AppLogger
{
    /// <summary>
    ///  Component for logging internal application meessages
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs given messages
        /// </summary>
        /// <param name="message">message that will be logged</param>
        void Log(string message);

        /// <summary>
        /// Stops the logger by canceling the background task that writes log messages and optionally waits for the task to finish writing all queued messages.
        /// </summary>
        /// <param name="flush">If true, waits for the background task to finish writing all queued messages</param>
        void Stop(bool flush);
    }
}