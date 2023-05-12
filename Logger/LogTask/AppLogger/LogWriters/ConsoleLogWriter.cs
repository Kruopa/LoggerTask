using System;
using System.Threading.Tasks;

namespace AppLogger
{
    public class ConsoleLogWriter : ILogWriter
    {
        public ConsoleLogWriter() { }
        public async Task WriteAsync(string logMessage)
        {
            await Console.Out.WriteLineAsync(logMessage);
        }
    }
}
