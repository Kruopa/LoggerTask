
using System;
using System.IO;
using System.Threading.Tasks;

namespace AppLogger
{
    public class FileLogWriter : ILogWriter
    {
        private readonly string logFileDirectory;
        private string LogFilePath => GetLogFilePath(DateTime.Now);

        public FileLogWriter(string logFileDirectory)
        {
            this.logFileDirectory = logFileDirectory;
            if (!Directory.Exists(logFileDirectory))
            {
                Directory.CreateDirectory(logFileDirectory);
            }
        }

        public async Task WriteAsync(string logMessage)
        {
            await WriteToFileAsync(logMessage, LogFilePath);

        }

        public async Task WriteAsync(string logMessage, DateTime timestamp)
        {
            await WriteToFileAsync(logMessage, GetLogFilePath(timestamp));
        }

        private async Task WriteToFileAsync(string logMessage, string filePath)
        {
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(fileStream))
                {
                    await writer.WriteLineAsync(logMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Can not write log message into the file: {LogFilePath}. { ex.Message}");
            }

        }

        private string GetLogFilePath(DateTime dateTime)
        {
            string fileName = dateTime.ToString("yyyyMMdd") + ".log";
            string filePath = Path.Combine(logFileDirectory, fileName);
            return filePath;
        }
    }
}
