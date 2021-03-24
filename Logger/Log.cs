#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private Option<object[]> _arguments;
        private Option<Exception> _exception;
        private Option<string> _message;
        private Option<Dictionary<object, object>> _properties;
        public Level Level = Level.Info;
        public DateTime DateTime { get; } = DateTime.Now;
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
                Level = level
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

            logMessage.Append($"{DateTime.Date()} "
                              + $"{DateTime.Time()} "
                              + $"({Level.ToString().ToUpper()}) : ");

            _message.IfSome(msg => logMessage.Append(msg + "\n"));

            _arguments.IfSome(args =>
                logMessage.Append($"Arguments: [{string.Join(", ", args)}]" + "\n"));

            _properties.IfSome(
                props =>
                    logMessage.Append(
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
        ///     Gets hash code of log content except DateTime
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            Func<Option<object>, int> GetHashCode = option => { return option.Match(o => o.GetHashCode(), () => 0); };

            return
                GetHashCode(_arguments)
                + Level.GetHashCode()
                + GetHashCode(_message)
                + GetHashCode(_properties);
        }
    }
}