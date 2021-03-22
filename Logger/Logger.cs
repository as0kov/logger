using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt;

namespace Logger
{
    public class Logger : ILog
    {
        private Dictionary<Level, Channel<Log>> _logBuffers = new();

        private static readonly System.Collections.Generic.HashSet<int> UniqueLogsHashes = new();

        public Logger()
        {
                
        }
        
        public Logger(Dictionary<Level, int> maxLogBufferSizes)
        {
            maxLogBufferSizes.Iter(logBufferSize =>
            {
                var (logLevel, logSize) = logBufferSize;
                _logBuffers.Add(logLevel, Channel.CreateBounded<Log>(logSize));
            });
        }

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

        /// <summary>
        /// Add log to buffer and returns ValueTask if buffer not full else returns None
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        private Task addLogToBuffer(Log log)
        {
            var isLogChExists = _logBuffers.ContainsKey(log.Level);
            if (isLogChExists)
            {
                var logCh = _logBuffers[log.Level];
                if (logCh)
            }
        }
        
        private static void Log(Log log)
        {
            var hashCode = log.GetHashCode();
            switch (log.IsMustBeUnique)
            {
                case true when !UniqueLogsHashes.Contains(hashCode):
                    UniqueLogsHashes.Add(hashCode);
                    Task.Factory.StartNew(WriteLogToFile(log));
                    break;
                case false:
                    Task.Factory.StartNew(log);
                    break;
            }
        }
        
        /// <summary>
        /// Writes log to file in logs folder
        /// </summary>
        /// <returns></returns>
        public static void WriteLogToFile(Log log)
        {
            var logByDateFolder = $"./logs/{log.Date}";
            Directory.CreateDirectory(logByDateFolder);
            File.AppendAllText($"{logByDateFolder}/{log.Level}.log", log.ToString());
        }
    }
}