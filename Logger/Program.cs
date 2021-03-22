using System;
using System.Collections.Generic;
using System.IO;

namespace Logger
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var logger = new Logger(new Dictionary<Level, int>
            {
                {Level.Debug, 100}
            });
            try
            {
                File.ReadAllBytes("adasasd");
            }
            catch (Exception e)
            {
                var dict = new Dictionary<object, object>
                {
                    {"test", e.Message}, {"s", "kek"}, {"dd", "kek"}, {"ede", "kek"}
                };

                logger.SystemInfo("err", dict);
            }

            for (var i = 0; i < 1000000; i++)
            {
                logger.Warning($"{i}");
                logger.Warning($"b {i}");
            }
            logger.DebugFormat("deveeb", 1, 2, 3, 2);
        }
    }
}