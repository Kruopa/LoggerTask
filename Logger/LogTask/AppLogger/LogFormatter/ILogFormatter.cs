namespace AppLogger
{
    public interface ILogFormatter
    {
        /// <summary>
        /// Formats given message
        /// </summary>
        /// <param name="message">string that will be formatted</param>
        /// <returns>formated message string</returns>
        string Format(string message);
    }
}