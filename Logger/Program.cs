using System;
using System.Collections.Generic;
using System.IO;

namespace Logger
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var logger = new Logger();
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

            logger.WarningUnique("aaa");
            logger.WarningUnique("aaa");
            logger.DebugFormat("deveeb", 1, 2, 3, 2);
        }
    }
}