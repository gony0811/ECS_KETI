
using SharedLibrary;
using System;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Utility)]
    public class ErrorLogger : IErrorReporter
    {
        [API(APICategory.Unclassified)]
        public TranscriptBuffer TranscriptBuffer { get; protected set; }

        [API(APICategory.Unclassified)]
        public ErrorLogger()
          : this(new TranscriptBuffer())
        {
        }

        [API(APICategory.Unclassified)]
        public ErrorLogger(TranscriptBuffer transcriptBuffer) => this.TranscriptBuffer = transcriptBuffer;

        public void ReportError(string msg)
        {
            if (this.TranscriptBuffer == null)
                throw new Exception(msg);
            this.TranscriptBuffer.Add(msg);
        }

        [API(APICategory.Unclassified)]
        public void ReportError(string format, params object[] args) => this.ReportError(string.Format(format, args));
    }
}
