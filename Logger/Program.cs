using System;

namespace Logger
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var logger = new Logger();

            for (var i = 0; i < 1000; i++) logger.ErrorUnique("a", new Exception());
        }
    }
}