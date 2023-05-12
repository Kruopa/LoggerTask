using AppLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.AppLogger
{
    public class FileLogWriterTests
    {
        [Fact]
        public async Task TestFileCreationAfterMidnight()
        {
            // Arrange
            string logDirectory = "logs";
            if (Directory.Exists(logDirectory)) Directory.Delete(logDirectory, true);
            DateTime BeforeMidnightTime = new DateTime(2020, 5, 11, 23, 59, 59, 999);
            var AfterMidnightTime = BeforeMidnightTime.AddMilliseconds(2);

            string logFilePathBeforeMidnight = Path.Combine(logDirectory, BeforeMidnightTime.ToString("yyyyMMdd") + ".log");
            string logFilePathAfterMidnight = Path.Combine(logDirectory, AfterMidnightTime.ToString("yyyyMMdd") + ".log");

            // Create an instance of the FileLogWriter
            FileLogWriter fileLogWriter = new FileLogWriter(logDirectory);

            try
            {
                // Act
                await fileLogWriter.WriteAsync("Test message before midnight", BeforeMidnightTime);
                await fileLogWriter.WriteAsync("Test message after midnight", AfterMidnightTime);

                // Assert
                Assert.True(File.Exists(logFilePathBeforeMidnight), $"Log file {logFilePathBeforeMidnight} should exist.");
                Assert.True(File.Exists(logFilePathAfterMidnight), $"Log file {logFilePathAfterMidnight} should exist.");
            }
            finally
            {
                // Cleanup
                if (File.Exists(logFilePathBeforeMidnight)) File.Delete(logFilePathBeforeMidnight);
                if (File.Exists(logFilePathAfterMidnight)) File.Delete(logFilePathAfterMidnight);
                if (Directory.Exists(logDirectory)) Directory.Delete(logDirectory);
            }
        }
    }
}
