using System;
using System.IO;
using System.Text;
using CarRental.Application.Interfaces.Infrastructure;
using CarRental.Infrastructure.Configuration;

namespace CarRental.Infrastructure.Logging
{
    /// <summary>
    /// Very simple file-based logger.
    /// </summary>
    public class FileLogger : ILogger
    {
        private readonly string _logFilePath;
        private static readonly object FileLock = new();

        public FileLogger(AppConfiguration configuration)
        {
            _logFilePath = configuration.LogFilePath;
        }

        public void Info(string message) =>
            WriteLog("INFO", message, null);

        public void Warning(string message) =>
            WriteLog("WARN", message, null);

        public void Error(string message, Exception? exception = null) =>
            WriteLog("ERROR", message, exception);

        private void WriteLog(string level, string message, Exception? exception)
        {
            var sb = new StringBuilder();
            sb.Append(DateTime.UtcNow.ToString("o"));
            sb.Append(" [").Append(level).Append("] ");
            sb.Append(message);

            if (exception != null)
            {
                sb.AppendLine();
                sb.Append(exception);
            }

            var line = sb.ToString();

            lock (FileLock)
            {
                File.AppendAllText(_logFilePath, line + Environment.NewLine);
            }
        }
    }
}
