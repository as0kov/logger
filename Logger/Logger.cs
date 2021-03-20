using System;
using System.Collections.Generic;

namespace Logger
{
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
                properties != null ?
                log.WithProperties(properties) :
                log
            );
        }

        private static void Log(Log log)
        {
            var hashCode = log.GetHashCode();
            switch (log.IsMustBeUnique)
            {
                case true when !UniqueLogsHashes.Contains(hashCode):
                    UniqueLogsHashes.Add(hashCode);
                    Console.WriteLine(log.ToString());
                    log.WriteToLogFile();
                    break;
                case false:
                    Console.WriteLine(log.ToString());
                    log.WriteToLogFile();
                    break;
            }
        }
    }
}