using AppLogger;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.AppLogger
{
    public class LoggerTests
    {
        private readonly IFixture fixture;
        private readonly Mock<ILogWriter> logWriterMock;

        public LoggerTests()
        {
            fixture = new Fixture().Customize(new AutoMoqCustomization());
            logWriterMock = fixture.Freeze<Mock<ILogWriter>>();
        }

        [Fact]
        public async Task Logger_WritesSomething()
        {
            logWriterMock
                .Setup(x => x.WriteAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var logger = new Logger(logWriterMock.Object);
            string message = "Test";
            // Act
            logger.Log(message);
            await Task.Delay(50);
            logger.Stop(false);

            // Assert
            logWriterMock.Verify(x => x.WriteAsync(It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData(20)]
        [InlineData(30)]
        public void LoggerStopWithFlush_InLogQueue(int logRecordCount)
        {
            // Arrange
            var logWriterMock = new Mock<ILogWriter>();
            logWriterMock
                .Setup(x => x.WriteAsync(It.IsAny<string>()))
                .Returns(() =>
                {
                    //Deploys task completion for 50ms
                    Thread.Sleep(50);
                    return Task.CompletedTask;
                });
            var logger = new Logger(logWriterMock.Object);

            // Act
            for (int i = 0; i < logRecordCount; i++)
            {
                logger.Log("Number with Flush: " + i.ToString());
                Thread.Sleep(10);
            }
            logger.Stop(flush: false);


            // Assert that ILogWriter method WriteAsync were invoked at least once
            logWriterMock.Verify(x => x.WriteAsync(It.IsAny<string>()), Times.AtLeastOnce);
            // Assert that ILogWriter method WriteAsync were called less times then original amount of log messages
            logWriterMock.Verify(x => x.WriteAsync(It.IsAny<string>()), Times.AtMost(logRecordCount - 1));
        }


    }
}
