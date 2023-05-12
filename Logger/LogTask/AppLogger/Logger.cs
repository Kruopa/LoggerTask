
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Based on .NET Standart 2.1 Framework for better compatibility between multiple versions
/// NuGet package build is enabled as well
/// </summary>
namespace AppLogger
{
    public class Logger : ILogger
    {
        private readonly BlockingCollection<string> logQueue;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly Task loggingTask;
        private readonly List<ILogWriter> writers;
        private readonly ILogFormatter logFormatter;


        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class with the specified collection of log writers
        /// and log formatter.
        /// </summary>
        /// <param name="writers">The collection of log writers to use.</param>
        /// <param name="logFormatter">The log formatter to use.</param>
        public Logger(IEnumerable<ILogWriter> writers, ILogFormatter logFormatter)
        {
            this.writers = (List<ILogWriter>)(writers ?? throw new ArgumentNullException(nameof(writers)));
            this.logFormatter = logFormatter ?? throw new ArgumentNullException(nameof(logFormatter));
            this.logQueue =  new BlockingCollection<string>();
            this.cancellationTokenSource = new CancellationTokenSource();
            this.loggingTask = Task.Run(() => WriteLogsAsync(cancellationTokenSource.Token));
        }
        
        public void Log(string message)
        {
            try
            {
                logQueue.Add(message);
            }
            catch (InvalidOperationException ex)
            {
                // This can happen if Stop method has been called.
                Console.WriteLine($"Failed to add message '{message}' to the log queue.", ex);
            }
            catch (Exception ex)
            {
                // Catching general Exception is not a good practice, but we can log it for debugging purposes.
                Console.WriteLine($"An error occurred while adding message '{message}' to the log queue.", ex);
            }
        }

        /// <summary>
        /// Stops the logger by canceling the background task that writes log messages and optionally waits for the task to finish writing all queued messages.
        /// </summary>
        /// <param name="flush">If false, waits for the background task to finish writing all queued messages. If true, the task is stopped abruptly.</param>
        public void Stop(bool flush)
        {
            try
            {
                cancellationTokenSource.Cancel();
                if (flush) loggingTask.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to stop the logger.", ex);
            }
        }


        private async Task WriteLogsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                string message;
                try
                {
                    message = logQueue.Take(cancellationToken);
                }
                catch (OperationCanceledException)
                {
#if DEBUG           
                    Console.WriteLine("Stopping logger");
#endif
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Logger exception when taking logs from queue. {ex}");
                    break;
                }

                if (!string.IsNullOrEmpty(message))
                {
                    foreach (var writer in writers)
                    {
                        try
                        {
                            await writer.WriteAsync(logFormatter.Format(message));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Logger exception when writing logs. {ex}");
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class with a custom <paramref name="logFormatter"/> and default <see cref="ConsoleLogWriter"/> as the only log writer.
        /// </summary>
        /// <param name="logFormatter">The custom log formatter to use.</param>
        public Logger(ILogFormatter logFormatter) : this(new List<ILogWriter> { new ConsoleLogWriter() }, logFormatter)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class with default <see cref="ConsoleLogWriter"/> as the only log writer and default <see cref="DefaultLogFormatter"/> as the log formatter.
        /// </summary>
        public Logger() : this(new List<ILogWriter> { new ConsoleLogWriter() }, new DefaultLogFormatter())
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class with the specified collection of <paramref name="writers"/> as the log writers and default <see cref="DefaultLogFormatter"/> as the log formatter.
        /// </summary>
        /// <param name="writers">The collection of log writers to use.</param>
        public Logger(IEnumerable<ILogWriter> writers) : this(writers, new DefaultLogFormatter())
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class with the specified <paramref name="writer"/> as the only log writer and default <see cref="DefaultLogFormatter"/> as the log formatter.
        /// </summary>
        /// <param name="writer">The log writer to use.</param>
        public Logger(ILogWriter writer) : this(new List<ILogWriter> { writer }, new DefaultLogFormatter())
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class with the specified <paramref name="writer"/> as the only log writer class with a custom <paramref name="logFormatter"/>.
        /// </summary>
        /// <param name="writer">The log writer to use.</param>
        /// <param name="logFormatter">The log formatter to use.</param>
        public Logger(ILogWriter writer, ILogFormatter logFormatter) : this(new List<ILogWriter> { writer }, logFormatter)
        {
        }
    }
}
