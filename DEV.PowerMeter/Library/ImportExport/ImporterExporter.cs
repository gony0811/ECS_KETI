using System.Diagnostics;

namespace DEV.PowerMeter.Library.ImportExport
{
    public abstract class ImporterExporter
    {
        public const string CsvExtension = ".csv";
        public const string TsvExtension = ".tsv";
        public const string TxtExtension = ".txt";
        public static readonly string[] ValidExtensions = new string[3]
        {
      ".csv",
      ".tsv",
      ".txt"
        };
        public static string DefaultExtension = ".csv";
        public static int DefaultExtension_index = 1;
        public const int CsvExtension_index = 1;
        public const int TsvExtension_index = 2;
        public const int TxtExtension_index = 3;
        public const int AnyExtension_index = 5;
        public const string ImportExportFilter = "Comma-Separated Files (*.csv)|*.csv|Tab-Separated Files (*.tsv)|*.tsv|Tab-Separated Files (*.txt)|*.txt|All Files (*.*)|*.*";
        public const string CsvOnlyFilter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
        public const int TxtOnlyExtension_index = 1;
        public const string TxtOnlyFilter = "TXT Files (*.txt)|*.txt|All Files (*.*)|*.*";
        public int LinesRead;
        public int LinesWritten;

        public Header Header { get; protected set; }

        public string Filename
        {
            protected set => this.Header.Filename = value;
            get => this.Header.Filename;
        }

        public CaptureBuffer Buffer { get; protected set; }

        protected BufferBounds BufferBounds => this.Header.BufferBounds;

        protected BufferPosition BufferPosition
        {
            get => this.Header.BufferPosition;
            set => this.Header.BufferPosition = value;
        }

        public ImporterExporter(string filename, CaptureBuffer buffer = null)
        {
            this.Buffer = buffer;
            this.Header = this.Buffer == null ? new Header() : this.Buffer.Header;
            this.Header.SetFilename(filename);
            this.LinesRead = 0;
        }

        public void Trace(string format, params object[] args)
        {
        }

        [Conditional("TRACE_GENERATE")]
        public static void TraceGenerate(string fmt, params object[] args)
        {
        }

        [Conditional("TRACE_EXPORT")]
        public static void TraceExport(string fmt, params object[] args)
        {
        }

        [Conditional("TRACE_EXPORT")]
        public static void TraceExportTiming(string fmt, params object[] args)
        {
        }
    }
}
