using System;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace DEV.PowerMeter.Library.ImportExport
{
    [API(APICategory.Measurement)]
    public class Header : IIsQuadOrPyro
    {
        public const string CommaSeparator = ", ";
        [API("Separator for TSV/TXT format files")]
        public const string TabSeparator = "\t";
        [API("Optional Selection bounds associated with this buffer")]
        public BufferBounds BufferBounds;
        [API("Optional Selection position associated with this buffer")]
        public BufferPosition BufferPosition;
        public const string DefaultSeparator = ", ";
        public const string Meter_FilePrefix = "Meter";
        public const string MeterSerial_PropertyName = "Meter S/N:";
        public const string Sensor_PropertyName = "Sensor";
        public const string Date_PropertyName = "Date";
        public const string Resolution_PropertyName = "TimeStampResolution";
        public const string Units_PropertyName = "Units";
        public const string Cursor_PropertyPrefix = "Cursor.";
        public const string CursorLeft_PropertyName = "Cursor.Left";
        public const string CursorRight_PropertyName = "Cursor.Right";
        public const string CursorUpper_PropertyName = "Cursor.Upper";
        public const string CursorLower_PropertyName = "Cursor.Lower";
        public const string Tracker_PropertyPrefix = "Tracker.";
        public const string TrackerX_PropertyName = "Tracker.X";
        public const string TrackerY_PropertyName = "Tracker.Y";
        public const string Cursor_PropertyName = "Cursor";
        public const string Tracker_PropertyName = "Tracker";
        public const string MeterNone_PropertyName = "[None]";
        public const string SensorNone_PropertyName = "None, None";
        public const string Timestamp_ColumnLabel = "Timestamp";
        public const string Measurement_ColumnLabel = "Measurement";
        public const string PulsePeriod_uSec_ColumnLabel = "PulsePeriod(uSec)";
        public const string X_ColumnLabel = "X";
        public const string Y_ColumnLabel = "Y";
        public const string SequenceColumnLabel = "Sequence";
        public const string FlagsColumnLabel = "Flags";
        public const string FlagsInterpretedColumnLabel = "FlagsInterpreted";
        public const string PeriodColumnLabel = "Period";
        public const string BinaryColumnLabel = "Binary Data";
        public static string[] SingleColumnHeadings = new string[6]
        {
      "Timestamp",
      "Measurement",
      "Sequence",
      "Flags",
      "FlagsInterpreted",
      "Period"
        };
        public static string[] QuadColumnHeadings = new string[8]
        {
      "Timestamp",
      "Measurement",
      "X",
      "Y",
      "Sequence",
      "Flags",
      "FlagsInterpreted",
      "Period"
        };
        public static string[] HistogramColumnHeadings = new string[4]
        {
      "Lower",
      "Upper",
      "Count",
      "Percent"
        };
        private static string[] LegacyColumnHeadings1 = new string[2]
        {
      "Measurement",
      "PulsePeriod(usec)"
        };
        private static string[] LegacyColumnHeadings2 = new string[3]
        {
      "Timestamp",
      "Measurement",
      "PulsePeriod(usec)"
        };
        protected int RowCount;
        private int LinesWritten;

        public string Separator { get; protected set; }

        public char SeparatorChar => this.Separator[0];

        public CultureInfo Culture { get; protected set; }

        [API("Filename for reading and writing")]
        public string Filename { get; set; }

        [API("File extension")]
        public string Extension => Path.GetExtension(this.Filename);

        public string MeterSerial { get; set; }

        public string SensorSerial { get; set; }

        public string SensorTypeAndQualifierString => this.SensorTypeString + this.Separator + this.SensorTypeQualifierString;

        public string SensorTypeString { get; set; }

        public string SensorTypeQualifierString { get; set; }

        [API("Associated Date, as string")]
        public string Date { get; set; }

        [API("Model name of associated sensor")]
        public string SensorModel { get; set; }

        public bool Sensor_IsQuad { get; set; }

        public bool Sensor_IsPyro { get; set; }

        public bool IsLegacy { get; protected set; }

        public bool HasPeriod { get; protected set; }

        public bool HasBinary { get; protected set; }

        [API("Units associated with these measurements")]
        public Units Units { get; set; }

        [API("Associated Date, as DateTime")]
        public DateTime DateTime { get; protected set; }

        public void SetBufferBounds(BufferBounds bounds)
        {
            if (this.BufferBounds == null)
                this.BufferBounds = new BufferBounds();
            this.BufferBounds.Set(bounds);
        }

        public void ClearBufferBounds() => this.BufferBounds = (BufferBounds)null;

        public void SetBufferPosition(BufferPosition position) => this.BufferPosition = position;

        public string[] ColummHeadings { get; set; }

        public Dictionary<string, int> ColumnIndicies { get; protected set; }

        public void SetColummHeadings(string[] headings)
        {
            this.ColummHeadings = headings;
            this.ColumnIndicies = new Dictionary<string, int>((IEqualityComparer<string>)StringComparer.InvariantCultureIgnoreCase);
            for (int index = 0; index < this.ColummHeadings.Length; ++index)
                this.ColumnIndicies[this.ColummHeadings[index]] = index;
        }

        public void SetColummHeadings(bool sensor_IsQuad) => this.SetColummHeadings(Header.SingleOrQuadColumnHeadings(this.Sensor_IsQuad));

        public string RowHeader { get; protected set; }

        public string[] RowHeaderFields { get; protected set; }

        public bool RetainBinary { get; protected set; }

        public int Columns => this.RowHeaderFields == null ? 0 : this.RowHeaderFields.Length;

        public Header(string separator = ", ")
        {
            this.Separator = separator;
            this.BufferBounds = (BufferBounds)null;
            this.RetainBinary = false;
            this.MeterSerial = "None";
            this.SensorTypeString = "None";
            this.SensorTypeQualifierString = "None";
            this.SensorModel = "";
            this.SensorSerial = "";
            this.Sensor_IsQuad = false;
            this.Sensor_IsPyro = false;
            this.Units = Units.Watts;
            this.Clear();
        }

        public Header(Units units) => this.Units = units;

        public Header(Meter meter, Units units)
          : this(units)
        {
            if (meter != null)
            {
                this.MeterSerial = meter.SerialNumber;
                this.Sensor_IsQuad = meter.Sensor_IsQuad;
                this.Sensor_IsPyro = meter.Sensor_IsPyro || meter.SlowEnergyMode_IsSelected;
                ISensor selectedSensor = (ISensor)meter.SelectedSensor;
                if (selectedSensor != null)
                {
                    this.SensorSerial = selectedSensor.SerialNumber;
                    this.SensorTypeString = selectedSensor.SensorType.ToString();
                    this.SensorTypeQualifierString = selectedSensor.SensorTypeQualifier.ToString();
                    this.SensorModel = selectedSensor.ModelName;
                }
            }
            this.SetColummHeadings(this.Sensor_IsQuad);
        }

        public Header(Units units, bool Sensor_IsQuad, bool Sensor_IsPyro)
          : this(units)
        {
            this.Sensor_IsQuad = Sensor_IsQuad;
            this.Sensor_IsPyro = Sensor_IsPyro;
            this.SetColummHeadings(Sensor_IsQuad);
        }

        public Header(
          string filename,
          Meter meter,
          Units units,
          BufferBounds bufferBounds = null,
          BufferPosition bufferPosition = null)
          : this(meter, units)
        {
            this.SetFilename(filename);
            this.BufferBounds = bufferBounds;
            this.BufferPosition = bufferPosition;
        }

        [API("set filename for import/export; extension determines Separator and default ColumnHeadings")]
        public void SetFilename(string filename)
        {
            this.Filename = filename;
            this.Separator = Header.SeparatorFromFilename(filename);
            this.SetColummHeadings(Header.SingleOrQuadColumnHeadings(this.Sensor_IsQuad));
            this.Culture = Header.CultureFromExtension(this.Extension);
        }

        [API("set optional buffer bounds for exporting slices")]
        public void SetBounds(BufferBounds bufferBounds) => this.BufferBounds = bufferBounds;

        public void Clear()
        {
            this.DateTime = DateTime.Now;
            this.Date = DateTime.Now.ToString();
            this.Filename = (string)null;
        }

        public static string SeparatorFromFilename(string filename) => Header.SeparatorFromExtension(Path.GetExtension(filename));

        public static string SeparatorFromExtension(string extension) => Header.IsCsvExtension(extension) ? ", " : "\t";

        public static CultureInfo CultureFromFilename(string filename) => Header.CultureFromExtension(Path.GetExtension(filename));

        public static CultureInfo CultureFromExtension(string extension) => Header.IsCsvExtension(extension) ? CultureInfo.InvariantCulture : CultureInfo.CurrentCulture;

        public static bool IsCsvExtension(string extension) => extension.ToLower().CompareTo(".csv") == 0;

        public static bool IsCsvFilename(string filename) => Header.IsCsvExtension(Path.GetExtension(filename));

        public static string[] SingleOrQuadColumnHeadings(bool IsQuad) => !IsQuad ? Header.SingleColumnHeadings : Header.QuadColumnHeadings;

        public int Read(StreamReader reader)
        {
            this.Units = Units.Watts;
            this.Date = (string)null;
            if (!this.ReadRow(reader) || !this.Match("Meter S/N:"))
                throw new Exception(string.Format("File {0} not LabMax-Pro format", (object)this.Filename));
            this.ParseMeterSerial();
            while (this.ReadRow(reader) && !this.Match("Timestamp"))
            {
                if (this.Match("Measurement"))
                {
                    this.IsLegacy = true;
                    break;
                }
                if (this.Match("Meter S/N:"))
                    throw new HeaderException(string.Format("Multiple Meter Desription Rows", (object)this.Filename));
                if (this.Match("Sensor"))
                    this.ParseSensorFields();
                else if (this.Match("Date"))
                    this.Date = this.RowHeaderFields.Length <= 1 ? "[None]" : this.RowHeaderFields[1];
                else if (this.Match("Units"))
                {
                    if (this.RowHeaderFields.Length > 1)
                        this.Units = Units.Decode(this.RowHeaderFields[1]);
                }
                else if (this.Match("Cursor"))
                    this.ParseCursorProperties();
                else if (this.Match("Tracker"))
                    this.ParseTrackerProperties();
                else if (this.MatchPrefix("Cursor."))
                    this.ParseCursorProperty();
                else if (this.MatchPrefix("Tracker."))
                    this.ParseTrackerProperty();
            }
            this.SetColummHeadings(this.RowHeaderFields);
            this.Sensor_IsPyro = this.Units.IsEnergy;
            this.Sensor_IsQuad = this.ColumnIndicies.ContainsKey("X") && this.ColumnIndicies.ContainsKey("Y");
            this.HasPeriod = this.ColumnIndicies.ContainsKey("Period");
            this.HasBinary = this.ColumnIndicies.ContainsKey("Binary Data");
            return this.RowCount;
        }

        private bool ReadRow(StreamReader reader)
        {
            this.RowHeader = reader.ReadLine();
            ++this.RowCount;
            if (this.RowHeader == null)
                return false;
            this.RowHeaderFields = this.RowHeader.Split(this.Separator[0]);
            for (int index = 0; index < this.RowHeaderFields.Length; ++index)
                this.RowHeaderFields[index] = this.RowHeaderFields[index].Trim();
            this.RowHeaderFields = this.RowHeaderFields.ElideTrailingEmpty();
            return true;
        }

        private bool Match(string name) => this.RowHeaderFields.Length != 0 && string.Compare(this.RowHeaderFields[0], name, StringComparison.InvariantCultureIgnoreCase) == 0;

        private bool MatchPrefix(string name)
        {
            if (this.RowHeaderFields.Length < 1)
                return false;
            string rowHeaderField = this.RowHeaderFields[0];
            return name.Length <= rowHeaderField.Length && string.Compare(rowHeaderField.Substring(0, name.Length), name, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        private void ParseMeterSerial()
        {
            this.MeterSerial = this.RowHeaderFields.Length < 2 ? "None" : this.RowHeaderFields[1];
            this.MeterSerial = this.MeterSerial.RemoveSurroundingQuotes();
        }

        private void ParseSensorFields()
        {
            switch (this.RowHeaderFields.Length)
            {
                case 2:
                    this.SensorTypeString = this.RowHeaderFields[1];
                    this.Sensor_IsPyro = SCPI.SensorTypeConverter.FromString(this.SensorTypeString, SensorType.None) == SensorType.Pyro;
                    break;
                case 3:
                    this.SensorTypeQualifierString = this.RowHeaderFields[2];
                    goto case 2;
                case 4:
                    this.SensorSerial = this.RowHeaderFields[3];
                    goto case 3;
                case 5:
                    this.SensorModel = this.RowHeaderFields[4];
                    goto case 4;
            }
        }

        public void ParseCursorProperties()
        {
            if (this.RowHeaderFields.Length < 2)
                return;
            if (this.BufferBounds == null)
                this.BufferBounds = new BufferBounds();
            switch (this.RowHeaderFields.Length)
            {
                case 2:
                    this.BufferBounds.First = this.DecodeTimestamp(this.RowHeaderFields[1]);
                    break;
                case 3:
                    this.BufferBounds.Last = this.DecodeTimestamp(this.RowHeaderFields[2]);
                    goto case 2;
                case 4:
                    this.BufferBounds.Upper = this.DecodeMeasurementValue(this.RowHeaderFields[3]);
                    goto case 3;
                default:
                    this.BufferBounds.Lower = this.DecodeMeasurementValue(this.RowHeaderFields[4]);
                    goto case 4;
            }
        }

        public void ParseTrackerProperties()
        {
            if (this.RowHeaderFields.Length < 2)
                return;
            if (this.BufferPosition == null)
                this.BufferPosition = new BufferPosition();
            if (this.RowHeaderFields.Length != 2)
                this.BufferPosition.Y = this.DecodeMeasurementValue(this.RowHeaderFields[2]);
            this.BufferPosition.X = this.DecodeTimestamp(this.RowHeaderFields[1]);
        }

        public void ParseCursorProperty()
        {
            if (this.RowHeaderFields.Length < 2)
                return;
            if (this.BufferBounds == null)
                this.BufferBounds = new BufferBounds();
            string rowHeaderField = this.RowHeaderFields[0];
            if (!(rowHeaderField == "Cursor.Left"))
            {
                if (!(rowHeaderField == "Cursor.Right"))
                {
                    if (!(rowHeaderField == "Cursor.Upper"))
                    {
                        if (!(rowHeaderField == "Cursor.Lower"))
                            return;
                        this.BufferBounds.Lower = this.DecodeMeasurementValue(this.RowHeaderFields[1]);
                    }
                    else
                        this.BufferBounds.Upper = this.DecodeMeasurementValue(this.RowHeaderFields[1]);
                }
                else
                    this.BufferBounds.Last = this.DecodeTimestamp(this.RowHeaderFields[1]);
            }
            else
                this.BufferBounds.First = this.DecodeTimestamp(this.RowHeaderFields[1]);
        }

        public void ParseTrackerProperty()
        {
            if (this.RowHeaderFields.Length < 2)
                return;
            if (this.BufferPosition == null)
                this.BufferPosition = new BufferPosition();
            string rowHeaderField = this.RowHeaderFields[0];
            if (!(rowHeaderField == "Tracker.X"))
            {
                if (!(rowHeaderField == "Tracker.Y"))
                    return;
                this.BufferPosition.Y = this.DecodeMeasurementValue(this.RowHeaderFields[1]);
            }
            else
                this.BufferPosition.X = this.DecodeTimestamp(this.RowHeaderFields[1]);
        }

        public DateTime DecodeTimestamp(string timestamp) => DataRecordSingle.DecodeDateTime(timestamp, this.Culture);

        public double DecodeMeasurementValue(string measurement) => DataRecordSingle.DecodeReal(measurement, this.Culture);

        public int Write(StreamWriter writer)
        {
            this.LinesWritten = 1;
            this.WriteFields(writer, "Meter S/N:", this.MeterSerial);
            this.WriteFields(writer, "Sensor", this.SensorTypeAndQualifierString, this.SensorSerial, this.SensorModel);
            this.WriteFields(writer, "Date", this.Date);
            if (this.Units != null)
                this.WriteFields(writer, "Units", this.Units.Suffix);
            if (this.BufferBounds != null)
                this.WriteFields(writer, "Cursor", this.BufferBounds.First.ToStringMicrosec(this.Culture), this.BufferBounds.Last.ToStringMicrosec(this.Culture), this.BufferBounds.Upper.ToString("f8", (IFormatProvider)this.Culture), this.BufferBounds.Lower.ToString("f8", (IFormatProvider)this.Culture));
            if (this.BufferPosition != null)
                this.WriteFields(writer, "Tracker", this.BufferPosition.X.ToStringMicrosec(this.Culture), this.BufferPosition.Y.ToString("f8", (IFormatProvider)this.Culture));
            this.WriteFieldArray(writer, this.ColummHeadings);
            if (this.RetainBinary)
                writer.Write(this.Separator + "Binary Data");
            writer.WriteLine();
            return this.LinesWritten;
        }

        public void WriteFields(StreamWriter writer, params string[] fields)
        {
            this.WriteFieldArray(writer, fields);
            writer.WriteLine();
            ++this.LinesWritten;
        }

        public void WriteFieldArray(StreamWriter writer, string[] fields)
        {
            fields = fields.ElideTrailingEmpty();
            writer.Write(string.Join(this.Separator, fields));
        }

        public int WriteEmptyHeader(StreamWriter writer)
        {
            this.WriteFields(writer, "Meter S/N:", "[None]");
            this.WriteFields(writer, "Sensor", "None, None");
            this.WriteFields(writer, "Date", DateTime.Now.ToString());
            this.WriteFields(writer, Header.SingleOrQuadColumnHeadings(false));
            return 4;
        }

        public int WriteHeader(StreamWriter writer, Header header) => header == null ? this.WriteEmptyHeader(writer) : header.Write(writer);
    }
}
