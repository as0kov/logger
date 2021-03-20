#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;

namespace Logger
{
    public enum Level
    {
        Fatal,
        Error,
        Warning,
        Info,
        Debug
    }

    public record Log
    {
        private readonly DateTime _dateTime = DateTime.Now;
        private Option<object[]> _arguments;
        private Option<Exception> _exception;
        private Level _level = Level.Info;
        private Option<string> _message;
        private Option<Dictionary<object, object>> _properties;
        public bool IsMustBeUnique { get; private init; }

        public Log WithMessage(string message)
        {
            return this with
            {
                _message = message
            };
        }

        public Log WithLevel(Level level)
        {
            return this with
            {
                _level = level
            };
        }

        public Log MustBeUnique(bool unique)
        {
            return this with
            {
                IsMustBeUnique = unique
            };
        }

        public Log WithException(Exception e)
        {
            return this with
            {
                _exception = e
            };
        }

        public Log WithProperties(Dictionary<object, object> properties)
        {
            return this with
            {
                _properties = properties
            };
        }

        public override string ToString()
        {
            var logMessage = new StringBuilder();
            logMessage.Append($"{_dateTime.Day}.{_dateTime.Month}.{_dateTime.Year} "
                              + $"{_dateTime.Hour}:{_dateTime.Minute}:{_dateTime.Second} "
                              + $"({_level.ToString().ToUpper()}) : ");

            _message.IfSome(msg => logMessage.Append(msg + "\n"));
            _arguments.IfSome(args =>
                logMessage.Append($"Arguments: [{string.Join(", ", args)}]" + "\n"));
            _properties.IfSome(
                props => logMessage.Append(
                    $"Properties: [{string.Join(", ", props.Select(pair => $"{pair.Key} => {pair.Value}"))}]"
                    + "\n"));
            _exception.IfSome(ex =>
            {
                logMessage.Append(ex.Message + "\n");
                logMessage.Append(ex.StackTrace + "\n");
            });

            return logMessage.ToString();
        }

        public Log WithArguments(object[] args)
        {
            return this with
            {
                _arguments = args
            };
        }

        /// <summary>
        /// Writes log to file in logs folder
        /// </summary>
        /// <returns></returns>
        public Task WriteToLogFile()
        {
            var log = ToString();
            var logByDateFolder = $"./logs/{_dateTime.Day}-{_dateTime.Month}-{_dateTime.Year}";
            Directory.CreateDirectory(logByDateFolder);
            return File.AppendAllTextAsync($"{logByDateFolder}/{_level}.log", log);
        }

        /// <summary>
        /// Gets hash code of log content except DateTime
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            Func<Option<object>, int> GetHashCode = option => { return option.Match(o => o.GetHashCode(), () => 0); };

            return
                GetHashCode(_arguments) +
                +GetHashCode(_exception)
                + _level.GetHashCode()
                + GetHashCode(_message)
                + GetHashCode(_properties);
        }
    }
}