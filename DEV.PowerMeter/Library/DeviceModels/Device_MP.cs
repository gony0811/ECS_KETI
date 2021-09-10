using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DEV.PowerMeter.Library.DeviceModels
{
    public class Device_MP : Device, ILegacyMeterless, IRangeSelection, IAutoRange
    {
        public const string SCPI_Handshaking = "SYST:COMM:HAND";
        public const string SCPI_SystemStatus = "SYST:STAT";
        public const string SCPI_SystemType = "SYST:INF:TYPE";
        private SensorTypeAndQualifier _SensorTypeAndQualifier;
        public const string SCPI_MeasurementMode = "CONF:MEAS";
        private Library.OperatingMode OpModeWatts;
        public const string SCPI_Speedup = "CONF:SPE";
        public const string SCPI_Wavelength = "CONF:WAVE";
        public const string SCPI_DefaultWavelengthOld = "wl";
        public const string SCPI_DefaultWavelength = "SYST:INF:WAVE";
        protected List<uint> _WavelengthTable_AsList;
        public const string SCPI_MinRangeValue = "rmi";
        public const string SCPI_MaxRangeValue = "rmx";
        public const string SCPI_GainCompensation = "CONF:GAIN:COMP";
        public const string SCPI_GainCompensationFactor = "CONF:GAIN:FACT";
        public const string SCPI_PTJM_TriggerLevel = "TRIGger:PTJ:LEVel";
        public const string SCPI_SerialNumber = "SYST:INF:SNUM";
        public const string SCPI_PartNumber = "SYST:INF:PNUM";
        public const string SCPI_ModelName = "SYST:INF:MODel";
        public const string SCPI_CalDate = "SYST:INF:CDATe";
        public const string SCPI_MfgDate = "SYST:INF:MDATe";
        public const string SCPI_ProbeType = "SYST:INF:TYPE";
        public const string SCPI_ProbeTemperature = "SYST:INF:TEMP";
        public const string SCPI_ThermistorADC = "tmp";
        public const string SCPI_ProbeDiameter = "SYST:INF:DIAM";
        public const string SCPI_FirmwareVersion = "v";
        protected string _FirmwareVersion;
        public const string SCPI_ProtocolVersion = "vp";
        public const string SCPI_ZeroSequenceId = "SYST:SYNC";
        public const string SCPI_Reset = "*RST";
        public const string SCPI_Restore = "SYSTem:RESTore";
        public const string SCPI_ConfigureZero = "CONFigure:ZERO";
        public const string SCPI_ConfigItem = "CONF:ITEM";
        public const DataFieldFlags DEFAULT_DATA_FIELDS = DataFieldFlags.Primary | DataFieldFlags.Flags | DataFieldFlags.Timestamp;
        public const DataFieldFlags LEGAL_DATA_FIELDS = DataFieldFlags.Primary | DataFieldFlags.Quad | DataFieldFlags.Flags | DataFieldFlags.Timestamp;
        private DataFieldFlags configureDataFields_AsEnum;
        protected static FlagConverter<DataFieldFlags> DataItemConverter = new FlagConverter<DataFieldFlags>(new Dictionary<string, DataFieldFlags>()
    {
      {
        "MEAS",
        DataFieldFlags.Primary
      },
      {
        "POS",
        DataFieldFlags.Quad
      },
      {
        "FLAG",
        DataFieldFlags.Flags
      },
      {
        "TST",
        DataFieldFlags.Timestamp
      }
    });
        public static EnumConverter<Device_MP.AccuracyMode> AccuracyModeConverter = new EnumConverter<Device_MP.AccuracyMode>(new Dictionary<string, Device_MP.AccuracyMode>()
    {
      {
        "DEF",
        Device_MP.AccuracyMode.Default
      },
      {
        "PONLY",
        Device_MP.AccuracyMode.PONLY
      },
      {
        "PPP",
        Device_MP.AccuracyMode.PPP
      }
    });
        public const string SCPI_ConfigureAccuracyMode = "CONF:AMODe";
        public const string SCPI_ReadNewDataRecord = "READ";
        public const string SCPI_Start = "INIT";
        public const string SCPI_Stop = "ABORT";
        public const ulong MicrosecPerMillisec = 1000;

        public string FetchBrokenMpQuery(string query)
        {
            string[] strArray = this.Communicator.SendQueryAndReceiveOneLine(query).Split("\r");
            if (strArray[1].Trim().ToUpper() != "OK")
                this.Communicator.ReportCommError("RequireOK missing OK: " + strArray[1]);
            return strArray[0];
        }

        public string FetchBrokenMpQueryNoSuffix(string query)
        {
            string[] strArray = this.Communicator.SendCommandAndReceiveOneLine(query).Split("\r");
            if (strArray[1].Trim().ToUpper() != "OK")
                this.Communicator.ReportCommError("RequireOK missing OK: " + strArray[1]);
            return strArray[0];
        }

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

        public override string SystemType => "PowerMax USB/RS";

        public SensorTypeAndQualifier SensorTypeAndQualifier => this._SensorTypeAndQualifier ?? (this._SensorTypeAndQualifier = new SensorTypeAndQualifier(this.ProbeTypeAndQualifier, SCPI.CommaChar));

        public SensorType SensorType => this.SensorTypeAndQualifier.SensorType;

        public override string MeterType
        {
            get
            {
                switch (this.SensorType)
                {
                    case SensorType.Thermo:
                        return "T";
                    case SensorType.Pyro:
                        return "P";
                    case SensorType.Optical:
                        return "O";
                    default:
                        return "?";
                }
            }
        }

        public bool MeterType_IsOptical => this.SensorType == SensorType.Optical;

        protected string OperatingMode
        {
            get => this.Communicator.SendAndReceiveQuery("CONF:MEAS");
            set => this.Communicator.SendAndReceiveCommand("CONF:MEAS", (object)value);
        }

        public override Library.OperatingMode OperatingMode_AsEnum
        {
            get => !this.MeterType_IsOptical ? SCPI.OpModeNames.FromString(this.OperatingMode) : this.OpModeWatts;
            set
            {
                if (this.MeterType_IsOptical)
                {
                    if (value != this.OpModeWatts)
                        throw new NotImplementedException("PM_USB_Device.OperatingMode_AsEnum Optical meter types cannot select non-Power mode.");
                }
                else
                    this.OperatingMode = SCPI.OpModeNames.ToString(value);
            }
        }

        protected string EnableSpeedup
        {
            get => this.Communicator.SendAndReceiveQuery("CONF:SPE");
            set => this.Communicator.SendAndReceiveCommand("CONF:SPE", (object)value);
        }

        public override bool EnableSpeedup_AsBool
        {
            get => !this.MeterType_IsOptical && FromDevice.Bool(this.EnableSpeedup);
            set
            {
                if (this.MeterType_IsOptical)
                {
                    if (value)
                        throw new NotImplementedException("PM_USB_Device.EnableSpeedup_AsBool Optical meter types cannot select Speedup.");
                }
                else
                    this.EnableSpeedup = ToDevice.Bool(value);
            }
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

        public override bool EnableAutoRange_AsBool
        {
            get => true;
            set
            {
            }
        }

        public override double SelectedRange_AsReal
        {
            get => 0.0;
            set
            {
            }
        }

        public override List<double> QueryRangeList_AsList => new List<double>()
    {
      0.0
    };

        protected string RangeMin => this.FetchBrokenMpQueryNoSuffix("rmi");

        public override double RangeMin_AsReal => FromDevice.Real(this.RangeMin);

        protected string RangeMax => this.FetchBrokenMpQueryNoSuffix("rmx");

        public override double RangeMax_AsReal => FromDevice.Real(this.RangeMax);

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
            set => this.GainCompensationFactor = ToDevice.Real(value);
        }

        protected string TriggerLevel_LPEM
        {
            get => this.Communicator.SendAndReceiveQuery("TRIGger:PTJ:LEVel");
            set => this.Communicator.SendAndReceiveCommand("TRIGger:PTJ:LEVel", (object)value);
        }

        public override Library.TriggerLevel_LPEM TriggerLevel_LPEM_AsEnum
        {
            get => SCPI.LPEM_TrigLevelNames.FromString(this.TriggerLevel_LPEM);
            set => this.TriggerLevel_LPEM = SCPI.LPEM_TrigLevelNames.ToString(value);
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

        public override string ProbeTypeAndQualifier => this.Communicator.SendAndReceiveQuery("SYST:INF:TYPE");

        public override string ProbeModel
        {
            get
            {
                string str = this.ModelName.RemoveSurroundingQuotes();
                return str.StartsWith("PM-USB") ? str.Substring("PM-USB".Length).Trim() : str;
            }
        }

        public override string ProbeSerialNumber => this.SerialNumber;

        public override string SensorTemperature => this.Communicator.SendAndReceiveQuery("SYST:INF:TEMP");

        public string ThermistorADC => this.Communicator.SendAndReceiveQuery("SYST:INF:TEMP");

        public override string ProbeDiameter => this.Communicator.SendAndReceiveQuery("SYST:INF:DIAM");

        public override double ProbeDiameter_AsReal => FromDevice.RealMaybeNA(this.ProbeDiameter);

        public override bool ProbeDiameter_IsAvailable => this.ProbeDiameter != "NA";

        public override string FirmwareVersion => this._FirmwareVersion ?? (this._FirmwareVersion = this.FetchBrokenMpQuery("v"));

        public string ProtocolVersion => this.FetchBrokenMpQuery("vp");

        public override void ClearSequenceId() => this.Communicator.SendAndReceiveCommand("SYST:SYNC");

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

        public override void ConfigureZero()
        {
            this.Communicator.SendAndReceiveCommand("CONFigure:ZERO");
            Thread.Sleep(500);
        }

        protected string ConfigureDataFields
        {
            get => this.Communicator.SendAndReceiveQuery("CONF:ITEM");
            set => this.Communicator.SendAndReceiveCommand("CONF:ITEM", (object)value);
        }

        public override DataFieldFlags ConfigureDataFields_AsEnum
        {
            get
            {
                this.configureDataFields_AsEnum = (DataFieldFlags)Device_MP.DataItemConverter.ToBitMask(this.ConfigureDataFields);
                return this.configureDataFields_AsEnum;
            }
            set
            {
                this.configureDataFields_AsEnum = (value | DataFieldFlags.Timestamp) & (DataFieldFlags.Primary | DataFieldFlags.Quad | DataFieldFlags.Flags | DataFieldFlags.Timestamp);
                this.ConfigureDataFields = Device_MP.DataItemConverter.ToStringList(this.configureDataFields_AsEnum);
            }
        }

        public string ConfigureAccuracyMode
        {
            get => this.Communicator.SendAndReceiveQuery("CONF:AMODe");
            set => this.Communicator.SendAndReceiveCommand("CONF:AMODe", (object)value);
        }

        public Device_MP.AccuracyMode ConfigureAccuracyMode_AsEnum
        {
            get => Device_MP.AccuracyModeConverter.FromString(this.ConfigureAccuracyMode);
            set
            {
                if (this.MeterType_IsOptical)
                    throw new NotImplementedException("PM_USB_Device.ConfigureAccuracyMode_AsEnum Optical meter types cannot change AMODE.");
                this.ConfigureAccuracyMode = Device_MP.AccuracyModeConverter.ToString(value);
            }
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
            if (!string.IsNullOrEmpty(line))
            {
                if (!line.IsOK())
                {
                    try
                    {
                        if (line.StartsWith("*"))
                            line = line.Substring(1);
                        string[] strArray1 = line.Split(",");
                        int index1 = 0;
                        int selectedDataFields = (int)DataRecordBase.SelectedDataFields;
                        bool isEnergy = Units.SelectedUnits.IsEnergy;
                        bool flag = (selectedDataFields & 2) != 0 && !isEnergy;
                        DataRecordSingle dataRecordSingle = flag ? (DataRecordSingle)new DataRecordQuad() : new DataRecordSingle();
                        if ((selectedDataFields & 1) != 0 && index1 < strArray1.Length)
                            dataRecordSingle.Measurement = this.DecodeReal(strArray1[index1++]);
                        if (flag)
                        {
                            if (index1 >= strArray1.Length - 1)
                                throw new MissingFieldException("x,y position fields");
                            this.DecodePosition(dataRecordSingle as DataRecordQuad, strArray1[index1], strArray1[index1 + 1]);
                            index1 += 2;
                        }
                        if ((selectedDataFields & 4) != 0 && index1 < strArray1.Length)
                            dataRecordSingle.Flags = this.DecodeFlags(strArray1[index1++]);
                        if (index1 < strArray1.Length)
                        {
                            string[] strArray2 = strArray1;
                            int index2 = index1;
                            int num1 = index2 + 1;
                            uint num2 = FromDevice.Uint(strArray2[index2]);
                            dataRecordSingle.Sequence = (ulong)num2 * 1000UL;
                        }
                        return dataRecordSingle;
                    }
                    catch (Exception ex)
                    {
                        this.Trace("Exception decoding( " + line + " ): " + ex.Message);
                    }
                    return (DataRecordSingle)null;
                }
            }
            return (DataRecordSingle)null;
        }

        public void DecodePosition(DataRecordQuad record, string x, string y)
        {
            record.X = this.DecodeReal(x);
            record.Y = this.DecodeReal(y);
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
                    case 'N':
                        measurementFlags |= MeasurementFlags.UnderRange;
                        continue;
                    case 'R':
                        measurementFlags |= MeasurementFlags.OverRange;
                        continue;
                    case 'S':
                        measurementFlags |= MeasurementFlags.SpedUp;
                        continue;
                    case 'T':
                        measurementFlags |= MeasurementFlags.OverTemp;
                        continue;
                    default:
                        this.Trace("DecodeFlags: ignoring bad flag in \"" + flags + "\"");
                        continue;
                }
            }
            return measurementFlags;
        }

        public enum AccuracyMode
        {
            Default,
            PONLY,
            PPP,
        }
    }
}
