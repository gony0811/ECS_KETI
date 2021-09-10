using SharedLibrary;
using System;
using System.IO;

namespace DEV.PowerMeter.Library.ImportExport
{
    public class Importer : ImporterExporter
    {
        protected DataRecordSingle PreviousItem;

        public Importer(string filename, CaptureBuffer buffer = null)
          : base(filename, buffer)
        {
        }

        public void ImportHeader(string filename)
        {
            this.Buffer = (CaptureBuffer)new CaptureBufferUnbounded();
            this.Header.SetFilename(this.Filename);
            using (StreamReader reader = File.OpenText(this.Filename))
                this.LinesRead = this.Header.Read(reader);
        }

        public void Import()
        {
            this.Trace("Importing " + this.Filename);
            this.Buffer = (CaptureBuffer)new CaptureBufferUnbounded(this.Header);
            this.ImportSeparatedValues();
        }

        protected void ImportSeparatedValues()
        {
            this.PreviousItem = (DataRecordSingle)null;
            using (StreamReader reader = File.OpenText(this.Filename))
            {
                this.LinesRead = this.Header.Read(reader);
                while (true)
                {
                    string str = reader.ReadLine();
                    if (str != null)
                    {
                        if (!str.IsNullOrEmpty())
                        {
                            ++this.LinesRead;
                            DataRecordSingle dataRecordSingle;
                            try
                            {
                                dataRecordSingle = this.Decode(str);
                                if (dataRecordSingle == null)
                                    break;
                            }
                            catch (Exception ex)
                            {
                                throw new DecodeException(string.Format("Line {0}: {1}", (object)this.LinesRead, (object)ex.Message), (System.Exception)ex);
                            }
                            this.Buffer.AddTimestampedItem(dataRecordSingle);
                        }
                        else
                            break;
                    }
                    else
                        break;
                }
            }
            if (this.Buffer.Count == 0U)
                throw new Exception("No Data Records");
        }

        public DataRecordSingle Decode(string line)
        {
            string[] fields = line.Split(this.Header.SeparatorChar).Truncate(this.Header.Columns);
            if (fields.AllEmptyFields())
                return (DataRecordSingle)null;
            if (fields.HasAnEmptyField())
                throw new DecodeException(string.Format("Empty field: {0}", (object)line));
            DataRecordSingle dataRecordSingle;
            if (this.Header.Sensor_IsQuad)
            {
                dataRecordSingle = (DataRecordSingle)new DataRecordQuad();
                dataRecordSingle.Decode(fields, this.Header.Culture);
            }
            else if (this.Header.IsLegacy)
            {
                dataRecordSingle = new DataRecordSingle();
                dataRecordSingle.DecodeLegacy(fields, this.Header.Culture);
            }
            else
            {
                dataRecordSingle = new DataRecordSingle();
                dataRecordSingle.Decode(fields, this.Header.Culture);
            }
            this.PreviousItem = dataRecordSingle;
            return dataRecordSingle;
        }
    }
}
