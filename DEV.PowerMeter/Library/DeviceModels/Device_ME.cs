
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace DEV.PowerMeter.Library.DeviceModels
{
    public class Device_ME : Device, ILegacyMeterless, IRangeSelection, IAutoRange
    {
        public const string SCPI_Handshaking = "SYST:COMM:HAND";
        public const string SCPI_SystemStatus = "SYST:STAT";
        private SensorTypeAndQualifier _SensorTypeAndQualifier;
        public const string SCPI_MeasurementMode = "CONF:MEAS:TYPE";
        public const string SCPI_Decimation = "CONF:DEC";
        public const uint DecimationRate_Disabled = 1;
        public const string SCPI_Wavelength = "CONF:WAVE";
        public const string SCPI_DefaultWavelengthOld = "wl";
        public const string SCPI_DefaultWavelength = "SYST:INF:WAVE";
        protected List<uint> _WavelengthTable_AsList;
        public const string SCPI_RangeValue = "CONFigure:RANGE:SELect";
        public double? CurrentSelectedRange_AsReal;
        public const string SCPI_ConfRangeSel = "conf:range:sel";
        protected double? _RangeMin_AsReal;
        protected double? _RangeMax_AsReal;
        public const string SCPI_GainCompensation = "CONF:GAIN:COMP";
        public const string SCPI_GainCompensationFactor = "CONF:GAIN:FACT";
        public const string SCPI_TriggerSource = "TRIG:SOUR";
        public const string SCPI_TriggerLevel_EM = "TRIG:LEVEL";
        public const string SCPI_TriggerSlope = "TRIG:SLOpe";
        public const string SCPI_TriggerDelay = "TRIGger:DELay";
        public const string SCPI_TriggerSequenceId = "TRIGger:SEQuence";
        public const string SCPI_SerialNumber = "SYST:INF:SNUM";
        public const string SCPI_PartNumber = "SYST:INF:PNUM";
        public const string SCPI_ModelName = "SYST:INF:MODel";
        public const string SCPI_CalDate = "SYST:INF:CDATe";
        public const string SCPI_MfgDate = "SYST:INF:MDATe";
        public const string SCPI_ProbeDiameter = "CONF:DIAM";
        protected string _FirmwareVersion;
        public const string SCPI_ZeroTriggerSequenceId = "TRIG:SEQ";
        public const string SCPI_Reset = "*RST";
        public const string SCPI_Restore = "SYSTem:RESTore";
        public const string SCPI_ConfigureZero = "CONFigure:ZERO";
        public const string SCPI_ConfigItem = "CONF:ITEM";
        public const DataFieldFlags LEGAL_DATA_FIELDS = DataFieldFlags.Primary | DataFieldFlags.Flags | DataFieldFlags.Sequence | DataFieldFlags.Period;
        protected static FlagConverter<DataFieldFlags> DataItemConverter = new FlagConverter<DataFieldFlags>(new Dictionary<string, DataFieldFlags>()
    {
      {
        "PULS",
        DataFieldFlags.Primary
      },
      {
        "PER",
        DataFieldFlags.Period
      },
      {
        "FLAG",
        DataFieldFlags.Flags
      },
      {
        "SEQ",
        DataFieldFlags.Sequence
      }
    });
        public const string SCPI_ConfigStatsItem = "CONF:STAT:ITEM";
        protected static FlagConverter<Device_ME.StatsFieldFlags> StatsItemConverter = new FlagConverter<Device_ME.StatsFieldFlags>(new Dictionary<string, Device_ME.StatsFieldFlags>()
    {
      {
        "MEAN",
        Device_ME.StatsFieldFlags.MEAN
      },
      {
        "MIN",
        Device_ME.StatsFieldFlags.MIN
      },
      {
        "MAX",
        Device_ME.StatsFieldFlags.MAX
      },
      {
        "STDV",
        Device_ME.StatsFieldFlags.STDV
      },
      {
        "DOSE",
        Device_ME.StatsFieldFlags.DOSE
      },
      {
        "MISS",
        Device_ME.StatsFieldFlags.MISS
      },
      {
        "SEQ",
        Device_ME.StatsFieldFlags.Sequence
      }
    });
        public const string SCPI_ReadNewDataRecord = "READ";
        public const string SCPI_Start = "INIT";
        public const string SCPI_Stop = "ABORT";
        private const string MeterlessPyroFormat = "E9";

        protected override void EnableHandshaking() => this.Communicator.SendAndReceiveCommand("SYST:COMM:HAND", (object)"ON");

        protected override void DisableHandshaking()
        {
            this.Communicator.SendCommandNoWaitReply("SYST:COMM:HAND", (object)"OFF");
            Thread.Sleep(50);
            if (this.Communicator.SendQueryAndReceiveOneLine("SYST:COMM:HAND").IsOn())
                throw new DeviceDisableHandshakingException();
        }

        public override string Identification => this.Communicator.SendAndReceiveQuery("*IDN");

        protected string SystemStatus => this.Communicator.SendAndReceiveQuery("SYST:STAT");

        public override SystemStatusBits SystemStatus_AsEnum => (SystemStatusBits)(4 | (this.SystemStatus.Contains("T") ? 2 : 0));

        public override SystemFaultBits SystemFaults_AsEnum => (SystemFaultBits)0;

        public override string SystemType => "EnergyMax USB/RS";

        public SensorTypeAndQualifier SensorTypeAndQualifier => this._SensorTypeAndQualifier ?? (this._SensorTypeAndQualifier = new SensorTypeAndQualifier(this.ProbeTypeAndQualifier, SCPI.CommaChar));

        public SensorType SensorType => this.SensorTypeAndQualifier.SensorType;

        public override string MeterType => "P";

        protected string OperatingMode
        {
            get => this.Communicator.SendAndReceiveQuery("CONF:MEAS:TYPE");
            set => this.Communicator.SendAndReceiveCommand("CONF:MEAS:TYPE", (object)value);
        }

        public override Library.OperatingMode OperatingMode_AsEnum
        {
            get => SCPI.OpModeNames.FromString(this.OperatingMode);
            set => this.OperatingMode = SCPI.OpModeNames.ToString(value);
        }

        public override bool EnableSpeedup_AsBool
        {
            get => false;
            set
            {
            }
        }

        public override string DecimationRate
        {
            get => this.Communicator.SendAndReceiveQuery("CONF:DEC");
            set => this.Communicator.SendAndReceiveCommand("CONF:DEC", (object)value);
        }

        public override uint DecimationRate_AsUint
        {
            get => FromDevice.Uint(this.DecimationRate, 1U);
            set => this.DecimationRate = value.ToString();
        }

        public override bool EnableWavelengthCorrection_AsBool
        {
            get => true;
            set
            {
            }
        }

        public string WavelengthCorrectionValue
        {
            get => this.Communicator.SendAndReceiveQuery("CONF:WAVE");
            set => this.Communicator.SendAndReceiveCommand("CONF:WAVE", (object)value);
        }

        public override uint WavelengthCorrectionValue_AsUint
        {
            get => FromDevice.Uint(this.WavelengthCorrectionValue);
            set => this.WavelengthCorrectionValue = value.ToString();
        }

        public string WavelengthCorrectionMin => this.Communicator.SendAndReceiveQuery("CONF:WAVE", (object)"MIN");

        public override uint WavelengthCorrectionMin_AsUint => FromDevice.Uint(this.WavelengthCorrectionMin);

        public string WavelengthCorrectionMax => this.Communicator.SendAndReceiveQuery("CONF:WAVE", (object)"MAX");

        public override uint WavelengthCorrectionMax_AsUint => FromDevice.Uint(this.WavelengthCorrectionMax);

        public string DefaultWavelength => this.Communicator.SendAndReceiveQuery("SYST:INF:WAVE", (object)"DEF");

        public uint DefaultWavelength_AsUint => FromDevice.Uint(this.DefaultWavelength);

        public override List<uint> WavelengthTable_AsList
        {
            get
            {
                List<uint> wavelengthTableAsList = this._WavelengthTable_AsList;
                if (wavelengthTableAsList != null)
                    return wavelengthTableAsList;
                List<uint> uintList1 = new List<uint>();
                uintList1.Add(this.WavelengthCorrectionMin_AsUint);
                uintList1.Add(this.DefaultWavelength_AsUint);
                uintList1.Add(this.WavelengthCorrectionMax_AsUint);
                List<uint> uintList2 = uintList1;
                this._WavelengthTable_AsList = uintList1;
                return uintList2;
            }
        }

        protected string SelectedRange
        {
            get => this.Communicator.SendAndReceiveQuery("CONFigure:RANGE:SELect");
            set => this.Communicator.SendAndReceiveCommand("CONFigure:RANGE:SELect", (object)value);
        }

        public override double SelectedRange_AsReal
        {
            get
            {
                if (!this.CurrentSelectedRange_AsReal.HasValue)
                    this.CurrentSelectedRange_AsReal = new double?(FromDevice.Real(this.SelectedRange));
                return this.CurrentSelectedRange_AsReal.Value;
            }
            set
            {
                this.CurrentSelectedRange_AsReal = new double?(value);
                this.SelectedRange = this.ToDevice_Real(value);
            }
        }

        public override List<double> QueryRangeList_AsList => new List<double>()
    {
      this.RangeMin_AsReal,
      this.RangeMax_AsReal
    };

        protected string RangeMin => this.Communicator.SendAndReceiveQuery("conf:range:sel", (object)"MIN");

        public override double RangeMin_AsReal
        {
            get
            {
                if (!this._RangeMin_AsReal.HasValue)
                    this._RangeMin_AsReal = new double?(FromDevice.Real(this.RangeMin));
                return this._RangeMin_AsReal.Value;
            }
        }

        protected string RangeMax => this.Communicator.SendAndReceiveQuery("conf:range:sel", (object)"MAX");

        public override double RangeMax_AsReal
        {
            get
            {
                if (!this._RangeMax_AsReal.HasValue)
                    this._RangeMax_AsReal = new double?(FromDevice.Real(this.RangeMax));
                return this._RangeMax_AsReal.Value;
            }
        }

        protected string EnableGainCompensation
        {
            get => this.Communicator.SendAndReceiveQuery("CONF:GAIN:COMP");
            set => this.Communicator.SendAndReceiveCommand("CONF:GAIN:COMP", (object)value);
        }

        public override bool EnableGainCompensation_AsBool
        {
            get => FromDevice.Bool(this.EnableGainCompensation);
            set => this.EnableGainCompensation = ToDevice.Bool(value);
        }

        protected string GainCompensationFactor
        {
            get => this.Communicator.SendAndReceiveQuery("CONF:GAIN:FACT");
            set => this.Communicator.SendAndReceiveCommand("CONF:GAIN:FACT", (object)value);
        }

        public override double GainCompensationFactor_AsReal
        {
            get => FromDevice.Real(this.GainCompensationFactor);
            set => this.GainCompensationFactor = this.ToDevice_Real(value);
        }

        public string TriggerSource
        {
            get => this.Communicator.SendAndReceiveQuery("TRIG:SOUR");
            set => this.Communicator.SendAndReceiveCommand("TRIG:SOUR", (object)value);
        }

        public override Library.TriggerSource TriggerSource_AsEnum
        {
            get => SCPI.TrigSourceNames.FromString(this.TriggerSource);
            set => this.TriggerSource = SCPI.TrigSourceNames.ToString(value);
        }

        protected double TriggerPercentToLevel(double percent) => this.TriggerLevePercentMaximum_AsReal == 0.0 ? 0.0 : percent / this.TriggerLevePercentMaximum_AsReal * this.ME_TriggerLevelMaximum_AsReal;

        protected double TriggerLevelToPercent(double level) => this.ME_TriggerLevelMaximum_AsReal == 0.0 ? 0.0 : level / this.ME_TriggerLevelMaximum_AsReal * this.TriggerLevePercentMaximum_AsReal;

        public double ME_TriggerLevelMaximum_AsReal => this.SelectedRange_AsReal * this.TriggerLevePercentMaximum_AsReal / 100.0;

        public override double TriggerLevelMinimum_AsReal => this.RangeMax_AsReal * this.TriggerLevelPercentMinimum_AsReal / 100.0;

        public override double TriggerLevelMaximum_AsReal => this.RangeMax_AsReal * this.TriggerLevePercentMaximum_AsReal / 100.0;

        public override double TriggerLevelPercentMinimum_AsReal => 0.01;

        public override double TriggerLevePercentMaximum_AsReal => 30.0;

        public override double TriggerLevel_AsReal
        {
            get => this.TriggerPercentToLevel(this.TriggerLevelPercent_AsReal);
            set => this.TriggerLevelPercent_AsReal = this.TriggerLevelToPercent(value);
        }

        public override void SetTriggerLevel(double level) => this.SetTriggerLevelPercent(this.TriggerLevelToPercent(level));

        public string TriggerLevelPercent
        {
            get => this.Communicator.SendAndReceiveQuery("TRIG:LEVEL");
            set => this.Communicator.SendAndReceiveCommand("TRIG:LEVEL", (object)value);
        }

        public override double TriggerLevelPercent_AsReal
        {
            get => FromDevice.Real(this.TriggerLevelPercent);
            set => this.TriggerLevelPercent = this.ToDevice_Real(value);
        }

        public void SetTriggerLevelPercent(double percent) => this.Communicator.SendCommandWhileRunning("TRIG:LEVEL", (object)this.ToDevice_Real(percent));

        public string TriggerSlope
        {
            get => this.Communicator.SendAndReceiveQuery("TRIG:SLOpe");
            set => this.Communicator.SendAndReceiveCommand("TRIG:SLOpe", (object)value);
        }

        public override Library.TriggerSlope TriggerSlope_AsEnum
        {
            get => SCPI.TrigSlopeConverter.FromString(this.TriggerSlope);
            set => this.TriggerSlope = SCPI.TrigSlopeConverter.ToString(value);
        }

        public string TriggerDelay
        {
            get => this.Communicator.SendAndReceiveQuery("TRIGger:DELay");
            set => this.Communicator.SendAndReceiveCommand("TRIGger:DELay", (object)value);
        }

        public override uint TriggerDelay_AsUint
        {
            get => FromDevice.Uint(this.TriggerDelay);
            set => this.TriggerDelay = value.ToString();
        }

        protected string TriggerSequenceId
        {
            get => this.Communicator.SendAndReceiveQuery("TRIGger:SEQuence");
            set => this.Communicator.SendAndReceiveCommand("TRIGger:SEQuence", (object)value);
        }

        public override string SerialNumber
        {
            get => this.Communicator.SendAndReceiveQuery("SYST:INF:SNUM").RemoveSurroundingQuotes();
            set => this.Communicator.SendAndReceiveCommand("SYST:INF:SNUM", (object)value);
        }

        public override string PartNumber
        {
            get => this.Communicator.SendAndReceiveQuery("SYST:INF:PNUM").RemoveSurroundingQuotes();
            set => this.Communicator.SendAndReceiveCommand("SYST:INF:PNUM", (object)value);
        }

        public override string ModelName
        {
            get => this.Communicator.SendAndReceiveQuery("SYST:INF:MODel").RemoveSurroundingQuotes();
            set => this.Communicator.SendAndReceiveCommand("SYST:INF:MODel", (object)value);
        }

        public override string CalibrationDate
        {
            get => this.Communicator.SendAndReceiveQuery("SYST:INF:CDATe").RemoveSurroundingQuotes();
            set => this.Communicator.SendAndReceiveCommand("SYST:INF:CDATe", (object)value);
        }

        public override string ManufactureDate
        {
            get => this.Communicator.SendAndReceiveQuery("SYST:INF:MDATe").RemoveSurroundingQuotes();
            set => this.Communicator.SendAndReceiveCommand("SYST:INF:MDATe", (object)value);
        }

        public override string ProbeTypeAndQualifier => "PYRO,NOSPEC";

        public override string ProbeModel
        {
            get
            {
                string str = this.ModelName.RemoveSurroundingQuotes();
                return str.StartsWith("PM-USB") ? str.Substring("PM-USB".Length).Trim() : str;
            }
        }

        public override string ProbeSerialNumber => this.SerialNumber;

        public override string SensorTemperature => "NA";

        public override string ProbeDiameter => this.Communicator.SendAndReceiveQuery("CONF:DIAM");

        public override double ProbeDiameter_AsReal => FromDevice.RealMaybeNA(this.ProbeDiameter);

        public override bool ProbeDiameter_IsAvailable => this.ProbeDiameter != "NA";

        public override string FirmwareVersion
        {
            get
            {
                if (this._FirmwareVersion == null)
                {
                    string identification = this.Identification;
                    int num = identification.IndexOf("- V");
                    if (num >= 0)
                        this._FirmwareVersion = identification.Substring(num + 2);
                }
                return this._FirmwareVersion ?? "[FirmwareVersion]";
            }
        }

        public override void ClearSequenceId() => this.Communicator.SendAndReceiveCommand("TRIG:SEQ", (object)"0");

        public override void SystemReset() => this.Communicator.SendAndReceiveCommand("*RST");

        public override void SystemRestore() => this.Communicator.LockAndDelegate((Action)(() =>
        {
            this.Communicator.SendCommandNoWaitReply("SYSTem:RESTore");
            this.EnableHandshaking();
        }));

        public override void SendCommand(string command)
        {
            if (!this.IsOpen)
                return;
            this.Communicator.SendCommandWaitTimeout(command);
        }

        public override void ConfigureZero() => this.Communicator.SendAndReceiveCommand("CONFigure:ZERO");

        protected string ConfigureDataFields
        {
            get => this.Communicator.SendAndReceiveQuery("CONF:ITEM");
            set => this.Communicator.SendAndReceiveCommand("CONF:ITEM", (object)value);
        }

        public override DataFieldFlags ConfigureDataFields_AsEnum
        {
            get => (DataFieldFlags)Device_ME.DataItemConverter.ToBitMask(this.ConfigureDataFields);
            set => this.ConfigureDataFields = Device_ME.DataItemConverter.ToStringList(value & (DataFieldFlags.Primary | DataFieldFlags.Flags | DataFieldFlags.Sequence | DataFieldFlags.Period));
        }

        protected string ConfigureStatsFields
        {
            get => this.Communicator.SendAndReceiveQuery("CONF:ITEM");
            set => this.Communicator.SendAndReceiveCommand("CONF:ITEM", (object)value);
        }

        public Device_ME.StatsFieldFlags ConfigureStatsFields_Default => Device_ME.StatsFieldFlags.All;

        public Device_ME.StatsFieldFlags ConfigureStatsFields_AsEnum
        {
            get => (Device_ME.StatsFieldFlags)Device_ME.StatsItemConverter.ToBitMask(this.ConfigureDataFields);
            set => this.ConfigureDataFields = Device_ME.StatsItemConverter.ToStringList(value & Device_ME.StatsFieldFlags.All);
        }

        public override string ReadNewDataRecord() => this.Communicator.SendAndReceiveQuery("READ");

        public override void Start(uint count = 0)
        {
            this.DisableHandshaking();
            this.Communicator.SendCommandNoWaitReply("INIT");
        }

        public override void Stop()
        {
            this.Communicator.SendStopCommand("ABORT");
            this.EnableHandshaking();
        }

        public override DataRecordSingle DecodeMeasurement(string line)
        {
            if (string.IsNullOrEmpty(line) || line.IsOK())
                return (DataRecordSingle)null;
            DataFieldFlags selectedDataFields = DataRecordBase.SelectedDataFields;
            try
            {
                if (line.StartsWith("*"))
                    line = line.Substring(1);
                string[] strArray1 = line.Split(",");
                if ((selectedDataFields & DataFieldFlags.Quad) != (DataFieldFlags)0)
                    throw new NotSupportedException("Meter_ME quad record");
                DataRecordSingle dataRecordSingle = new DataRecordSingle();
                int num1 = 0;
                if ((selectedDataFields & DataFieldFlags.Primary) != (DataFieldFlags)0 && num1 < strArray1.Length)
                    dataRecordSingle.Measurement = FromDevice.Real(strArray1[num1++]);
                if ((selectedDataFields & DataFieldFlags.Period) != (DataFieldFlags)0 && num1 < strArray1.Length)
                    dataRecordSingle.Period = FromDevice.Uint(strArray1[num1++]);
                if ((selectedDataFields & DataFieldFlags.Flags) != (DataFieldFlags)0 && num1 < strArray1.Length)
                    dataRecordSingle.Flags = this.DecodeFlags(strArray1[num1++]);
                if ((selectedDataFields & DataFieldFlags.Sequence) != (DataFieldFlags)0 && num1 < strArray1.Length)
                {
                    string[] strArray2 = strArray1;
                    int index = num1;
                    int num2 = index + 1;
                    uint num3 = FromDevice.Uint(strArray2[index]);
                    dataRecordSingle.Sequence = (ulong)num3;
                }
                return dataRecordSingle;
            }
            catch (Exception ex)
            {
                this.Trace("Exception decoding( " + line + " ): " + ex.Message);
            }
            return (DataRecordSingle)null;
        }

        public MeasurementFlags DecodeFlags(string flags)
        {
            MeasurementFlags measurementFlags = (MeasurementFlags)0;
            foreach (char ch in flags.ToUpper())
            {
                switch (ch)
                {
                    case '0':
                    case 'C':
                        continue;
                    case 'B':
                        measurementFlags |= MeasurementFlags.BaselineClip;
                        continue;
                    case 'D':
                        measurementFlags |= MeasurementFlags.DirtyBatch;
                        continue;
                    case 'M':
                        measurementFlags |= MeasurementFlags.MissingPulse;
                        continue;
                    case 'P':
                        measurementFlags |= MeasurementFlags.OverRange;
                        continue;
                    default:
                        this.Trace("DecodeFlags: ignoring bad flag in \"" + flags + "\"");
                        continue;
                }
            }
            return measurementFlags;
        }

        public string ToDevice_Real(double value) => value.ToString("E9", (IFormatProvider)CultureInfo.InvariantCulture).Replace("+", "");

        [System.Flags]
        public enum StatsFieldFlags
        {
            MEAN = 1,
            MIN = 2,
            MAX = MIN | MEAN, // 0x00000003
            STDV = 8,
            DOSE = 16, // 0x00000010
            MISS = 32, // 0x00000020
            Sequence = 64, // 0x00000040
            All = 127, // 0x0000007F
        }
    }
}
