using System;
using System.Diagnostics;

namespace LINQToTTreeLib
{
    /// <summary>
    /// A helper class for doing event tracing.
    /// </summary>
    public static class TraceHelpers
    {
        private static Lazy<TraceSource> _gTraceSource = new Lazy<TraceSource>(() => new TraceSource("LINTToTTreeLib"));

        public static TraceSource Source
        {
            get
            {
                return _gTraceSource.Value;
            }
        }

        /// <summary>
        /// Trace a basic info message.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="message"></param>
        public static void TraceInfo(int index, string message, TraceEventType opt = TraceEventType.Verbose)
        {
            _gTraceSource.Value.TraceEvent(opt, index, message);
        }
    }
}
