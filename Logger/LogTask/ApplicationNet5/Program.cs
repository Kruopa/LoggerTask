using System;
using System.Collections.Generic;
using System.Threading;
using AppLogger;

namespace ApplicationNet5
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start logging...");

            ILogger logger;
            ILogWriter filelogWriterFlush = new FileLogWriter("logsNoFlush/");

            var logWriter = new List<ILogWriter>() { filelogWriterFlush };
            logger = new Logger(logWriter);


            for (int i = 0; i < 15; i++)
            {
                logger.Log("Number with Flush: " + i.ToString());
                Thread.Sleep(50);
            }

            logger.Stop(flush: false);

            ILogWriter filelogWriterNoFlush = new FileLogWriter("logsWithFlush/");

            ILogger logger2 = new Logger(filelogWriterNoFlush);

            for (int i = 50; i > 0; i--)
            {
                logger2.Log("Number with No flush: " + i.ToString());
                Thread.Sleep(50);
            }

            logger2.Stop(flush: false);

            Console.WriteLine("Done.");
        }
    }
}
