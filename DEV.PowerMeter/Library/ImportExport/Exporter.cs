using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


namespace DEV.PowerMeter.Library.ImportExport
{
    public class Exporter : ImporterExporter, IDisposable
    {
        protected StreamWriter Writer;
        protected Stopwatch Stopwatch;

        public Exporter(string filename, CaptureBuffer buffer)
          : base(filename, buffer)
        {
        }

        public static void Export(string filename, CaptureBuffer buffer, bool sliceToBounds = false) => new Exporter(filename, buffer).Export(sliceToBounds);

        public virtual void Create(DestinationCollisionRemedy remedy = DestinationCollisionRemedy.Overwrite)
        {
            this.Buffer.Header.SetFilename(this.Filename);
            this.Buffer.Header.SetBounds(this.BufferBounds);
            this.Writer = this.CreateWriter(remedy);
            this.LinesWritten = this.Header.WriteHeader(this.Writer, this.Header);
        }

        public bool IsOpen => this.Writer != null;

        public void Close()
        {
            if (this.Writer == null)
                return;
            this.Writer.Close();
            this.Writer = (StreamWriter)null;
        }

        public void Dispose() => this.Close();

        public void Export(bool sliceToBounds = false)
        {
            this.Stopwatch = Stopwatch.StartNew();
            this.Create();
            if (sliceToBounds)
                this.Export(this.Buffer.Slice(this.BufferBounds));
            else
                this.Export(this.Buffer.PowerAndEnergy);
        }

        public void Export(IEnumerable<DataRecordSingle> enumerator)
        {
            try
            {
                try
                {
                    foreach (IDataRecordSingle dataRecordSingle in enumerator)
                    {
                        this.Export(dataRecordSingle);
                        ++this.LinesWritten;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Line {0}: {1}", (object)this.LinesWritten, (object)ex.Message));
                }
                if (this.Buffer.SkipBuffer == null || !DataRecordBase.RetainBinary)
                    return;
                this.Writer.WriteLine(this.Buffer.SkipBuffer.ToSeparatedValues(this.Header.Separator));
            }
            finally
            {
                this.Close();
            }
        }

        public void Export(IDataRecordSingle item)
        {
            this.Writer.WriteLine(string.Join(this.Header.Separator, item.Encode(this.Header.Culture)));
            ++this.LinesWritten;
        }

        public StreamWriter CreateWriter(DestinationCollisionRemedy remedy)
        {
            StandardPathnames.EnsureFolderExists(this.Filename);
            if (remedy != DestinationCollisionRemedy.Overwrite && File.Exists(this.Filename))
            {
                switch (remedy)
                {
                    case DestinationCollisionRemedy.Append:
                        StreamWriter streamWriter = FileX.AppendTextUTF8(this.Filename);
                        streamWriter.WriteLine();
                        return streamWriter;
                    case DestinationCollisionRemedy.Rename:
                        this.Filename = DestinationPathname.GenerateUniqueFilename(this.Filename);
                        break;
                }
            }
            return FileX.CreateTextUTF8(this.Filename);
        }

        public void Flush()
        {
            this.Close();
            this.Writer = FileX.AppendTextUTF8(this.Filename);
        }
    }
}
