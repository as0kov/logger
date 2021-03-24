using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Logger
{
    public static class DateTimeExt
    {
        public static string Date(this DateTime dateTime)
        {
            return string.Join(
                "-",
                dateTime.Day.ToString().PadLeft(2, '0'),
                dateTime.Month.ToString().PadLeft(2, '0'),
                dateTime.Year.ToString().PadLeft(2, '0'));
        }

        public static string Time(this DateTime dateTime)
        {
            return string.Join(
                ":",
                dateTime.Hour.ToString().PadLeft(2, '0'),
                dateTime.Minute.ToString().PadLeft(2, '0'),
                dateTime.Second.ToString().PadLeft(2, '0'));
        }
    }

    public class Logger : ILog
    {
        private static readonly HashSet<int> UniqueLogsHashes = new();

        public void Fatal(string message)
        {
            Log(
                new Log()
                    .WithLevel(Level.Fatal)
                    .WithMessage(message)
            );
        }

        public void Fatal(string message, Exception e)
        {
            Log(
                new Log()
                    .WithLevel(Level.Fatal)
                    .WithMessage(message)
                    .WithException(e)
            );
        }

        public void Error(string message)
        {
            Log(
                new Log()
                    .WithLevel(Level.Error)
                    .WithMessage(message)
            );
        }

        public void Error(string message, Exception e)
        {
            Log(
                new Log()
                    .WithLevel(Level.Error)
                    .WithMessage(message)
                    .WithException(e)
            );
        }

        public void Error(Exception ex)
        {
            Log(
                new Log()
                    .WithLevel(Level.Error)
                    .WithException(ex)
            );
        }

        public void ErrorUnique(string message, Exception e)
        {
            Log(
                new Log()
                    .WithLevel(Level.Error)
                    .WithMessage(message)
                    .MustBeUnique(true)
                    .WithException(e)
            );
        }

        public void Warning(string message)
        {
            Log(
                new Log()
                    .WithLevel(Level.Warning)
                    .WithMessage(message)
            );
        }

        public void Warning(string message, Exception e)
        {
            Log(
                new Log()
                    .WithLevel(Level.Warning)
                    .WithMessage(message)
                    .WithException(e)
            );
        }

        public void WarningUnique(string message)
        {
            Log(
                new Log()
                    .WithLevel(Level.Warning)
                    .WithMessage(message)
                    .MustBeUnique(true)
            );
        }

        public void Info(string message)
        {
            Log(
                new Log()
                    .WithLevel(Level.Info)
                    .WithMessage(message)
            );
        }

        public void Info(string message, Exception e)
        {
            Log(
                new Log()
                    .WithLevel(Level.Info)
                    .WithMessage(message)
                    .WithException(e)
            );
        }

        public void Info(string message, params object[] args)
        {
            Log(
                new Log()
                    .WithLevel(Level.Info)
                    .WithMessage(message)
                    .WithArguments(args)
            );
        }

        public void Debug(string message)
        {
            Log(
                new Log()
                    .WithLevel(Level.Debug)
                    .WithMessage(message)
            );
        }

        public void Debug(string message, Exception e)
        {
            Log(
                new Log()
                    .WithLevel(Level.Debug)
                    .WithMessage(message)
                    .WithException(e)
            );
        }

        public void DebugFormat(string message, params object[] args)
        {
            Log(
                new Log()
                    .WithLevel(Level.Debug)
                    .WithMessage(message)
                    .WithArguments(args)
            );
        }

        public void SystemInfo(string message, Dictionary<object, object> properties = null)
        {
            var log = new Log()
                .WithLevel(Level.Info)
                .WithMessage(message);
            Log(
                properties != null ? log.WithProperties(properties) : log
            );
        }

        private void Log(Log log)
        {
            var hashCode = log.GetHashCode();

            if (log.IsMustBeUnique && !UniqueLogsHashes.Contains(hashCode))
            {
                UniqueLogsHashes.Add(hashCode);
                WriteLogToFile(log);
            }
            else if (log.IsMustBeUnique == false)
            {
                WriteLogToFile(log);
            }
        }

        /// <summary>
        ///     Writes log to file in logs folder
        /// </summary>
        /// <returns></returns>
        private static void WriteLogToFile(Log log)
        {
            Task.Factory.StartNew(
                    async () => await WriteToLogfileAsync(log.DateTime, log.Level, log.ToString())
                )
                .RunSynchronously();
        }

        private static async Task WriteToLogfileAsync(DateTime dateTime, Level level, string text)
        {
            var logByDateFolder = $"./logs/{dateTime.Date()}";
            Directory.CreateDirectory(logByDateFolder);
            await File.AppendAllTextAsync($"{logByDateFolder}/{level}.log", text);
        }
    }
}