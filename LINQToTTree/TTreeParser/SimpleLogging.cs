using System;
using System.Collections.Generic;
using System.IO;

namespace TTreeParser
{
    /// <summary>
    /// Simple class to help with "global" output for logging
    /// </summary>
    public static class SimpleLogging
    {
        private static List<TextWriter> _outputs = new List<TextWriter>();

        static SimpleLogging()
        {
            _outputs.Add(Console.Out);
        }

        /// <summary>
        /// Add a stream to log
        /// </summary>
        /// <param name="writer"></param>
        public static void AddStream(TextWriter writer)
        {
            _outputs.Add(writer);
        }

        /// <summary>
        /// Log a message to the output, uses standard .NET formatting
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message, params object[] args)
        {
            foreach (var writer in _outputs)
            {
                writer.WriteLine(message, args);
            }
        }
    }
}
