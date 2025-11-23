using System;
using System.IO;
using CarRental.Infrastructure.Configuration;
using CarRental.Infrastructure.Logging;
using Xunit;

namespace CarRental.Tests.Infrastructure
{
    public class FileLoggerTests
    {
        [Fact]
        public void InfoWarningError_WriteLinesToFile()
        {
            var tempLog = Path.Combine(Path.GetTempPath(), $"CarRental_Log_{Guid.NewGuid():N}.log");

            try
            {
                // Arrange
                Environment.SetEnvironmentVariable("CAR_RENTAL_LOG_FILE", tempLog);

                var config = new AppConfiguration();
                var logger = new FileLogger(config);

                // Act
                logger.Info("Test info");
                logger.Warning("Test warning");
                logger.Error("Test error", new InvalidOperationException("Boom"));

                // Assert
                Assert.True(File.Exists(tempLog));

                var content = File.ReadAllText(tempLog);

                Assert.Contains("INFO", content);
                Assert.Contains("Test info", content);

                Assert.Contains("WARN", content);
                Assert.Contains("Test warning", content);

                Assert.Contains("ERROR", content);
                Assert.Contains("Test error", content);
                Assert.Contains("Boom", content);
            }
            finally
            {
                if (File.Exists(tempLog))
                {
                    File.Delete(tempLog);
                }
            }
        }
    }
}
