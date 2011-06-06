using System;
using System.Diagnostics;

namespace LINQToTTreeLib
{
    /// <summary>
    /// A helper class for doing event tracing.
    /// </summary>
    class TraceHelpers
    {
        private static Lazy<TraceSource> _gTraceSource = new Lazy<TraceSource>(() => new TraceSource("LINTToTTreeLib"));

        /// <summary>
        /// Trace a basic info message.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="message"></param>
        public static void TraceInfo(int index, string message)
        {
            _gTraceSource.Value.TraceEvent(TraceEventType.Information, index, message);
        }
    }
}
