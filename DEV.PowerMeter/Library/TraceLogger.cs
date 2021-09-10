using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Undocumented)]
    public static class TraceLogger
    {
        public static TranscriptBuffer TranscriptBuffer;
        public static bool HidePolling = true;
        public static Thread PreviousThread = (Thread)null;

        public static bool IsPolling => PollingData.IsPolling;

        public static void Trace(string format, params object[] args) => TraceLogger.Trace(string.Format(format, args));

        public static void Trace(string msg)
        {
            Thread currentThread = Thread.CurrentThread;
            if (TraceLogger.PreviousThread != currentThread)
            {
                Thread previousThread = TraceLogger.PreviousThread;
                TraceLogger.PreviousThread = currentThread;
                if (previousThread != null)
                    TraceLogger.Trace("\nThread Changed: " + (previousThread.Name ?? "Null") + " -> " + (currentThread.Name ?? "Null") + "\n");
            }
            if (TraceLogger.IsPolling && TraceLogger.HidePolling)
                return;
            if (TraceLogger.IsPolling)
                msg = "Polling: " + msg;
            TraceLogger.TraceToBuffer(msg);
        }

        [Conditional("TRACE_TO_OUTPUT")]
        public static void DebugWriteLine(string message)
        {
        }

        [Conditional("TRACE_TO_BUFFER")]
        public static void TraceToBuffer(string message)
        {
            if (TraceLogger.TranscriptBuffer == null)
                return;
            TraceLogger.TranscriptBuffer.Add(message);
        }

        [Conditional("TRACE_OPEN_CLOSE")]
        public static void TraceOpenClose(string format, params object[] args) => TraceLogger.Trace(format, args);

        [Conditional("TRACE_WRITE")]
        public static void TraceWrite(string command) => TraceLogger.Trace("Send: " + command);

        [Conditional("TRACE_WRITE")]
        public static void TraceWrite(string format, params object[] args) => TraceLogger.Trace("Send: " + string.Format(format, args));

        [Conditional("TRACE_READ")]
        public static void TraceRead(string text) => TraceLogger.Trace("Recv: " + text);

        [Conditional("TRACE_READ")]
        public static void TraceRead(string format, params object[] args) => TraceLogger.Trace("Recv: " + string.Format(format, args));

        [Conditional("TRACE_READ_BINARY")]
        public static void TraceRead(int received, byte[] data, int offset, int count) => TraceLogger.Trace(string.Format("Read( {0}, {1} ) -> {2}", (object)offset, (object)count, (object)received));

        [Conditional("TRACE_SKIP_UNUSED_DATA")]
        public static void TraceSkipUnusedData(string format, params object[] args) => TraceLogger.Trace(format, args);

        [Conditional("TRACE_SKIP_UNUSED_DATA")]
        public static void TraceSkipUnusedData(byte[] SkipBuffer, int count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < count; ++index)
                stringBuilder.AppendFormat("{0:x2} ", (object)SkipBuffer[index]);
        }

        [Conditional("TRACE_ERROR")]
        public static void TraceError(string text)
        {
            int num = TraceLogger.HidePolling ? 1 : 0;
            TraceLogger.HidePolling = false;
            TraceLogger.Trace("Error: " + text);
            TraceLogger.HidePolling = num != 0;
        }

        [Conditional("TRACE_NOTE")]
        public static void TraceNote(string text) => TraceLogger.Trace("Note: " + text);
    }
}
