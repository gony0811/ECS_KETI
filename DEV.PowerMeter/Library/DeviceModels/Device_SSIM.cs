using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;


namespace DEV.PowerMeter.Library.DeviceModels
{
    public class Device_SSIM : Device, IRangeSelection, IAutoRange, IConfigureBaudRate
    {
        public const string SCPI_Handshaking = "SYST:COMM:HAND";
        public readonly string Handshaking_OFF = "SYST:COMM:HAND OFF ;SYST:COMM:HAND";
        public const string SCPI_ConfigureBaudRate = "SYST:COMM:SER:BAUD";
        public const string SCPI_SystemStatus = "SYST:STAT";
        public static uint FakeSystemStatus_AsUInt = 0;
        public const string SCPI_SystemFaults = "SYST:FAULt";
        public static uint FakeSystemFaults_AsUInt = 0;
        public const string SCPI_SystemType = "SYST:TYPE";
        public const string SCPI_MeasurementMode = "CONFigure:MEASure:MODe";
        public const string SCPI_MeasurementSpeed = "CONFigure:MEASure:SOUR:SEL";
        public const string SCPI_QuerySpeedList = "CONFigure:MEAS:SOUR:LIST";
        public const string SCPI_Speedup = "CONFigure:SPEedup";
        public const string SCPI_AreaCorrection = "CONFigure:AREA:CORRection";
        public const string SCPI_ApertureArea = "CONFigure:AREA:APERture";
        public const float DefaultAreaCorrectionArea = 1f;
        public const string SCPI_AnalogOutLevel = "CONFigure:AOUT:FSCale";
        public const string SCPI_Smoothing = "CONFigure:AVERage:TIME";
        public const string SCPI_WavelengthCorrection = "CONF:WAVE:CORR";
        public const string SCPI_Wavelength = "CONF:WAVE:WAVE";
        public const string SCPI_WavelengthTable = "CONFigure:WAVElength:LIST";
        public const string SCPI_RangeAuto = "CONFigure:RANGE:AUTO";
        public const string SCPI_RangeValue = "CONFigure:RANGE:SELect";
        public const string SCPI_QueryRangeList = "CONFigure:RANGe:LIST";
        public const string SCPI_GainCompensation = "CONFigure:GAIN:COMPensation";
        public const string SCPI_GainCompensationFactor = "CONFigure:GAIN:FACTor";
        public const string SCPI_Decimation = "CONFigure:DECimation";
        public const uint DecimationRate_Disabled = 1;
        public const string SCPI_TriggerSource = "TRIGger:SOURce";
        public const string SCPI_TriggerLevel = "TRIGger:LEVEL";
        public const string SCPI_TriggerLevelPercent = "TRIGger:PERCENT:LEVEL";
        public const string SCPI_TriggerSlope = "TRIGger:SLOpe";
        public const string SCPI_TriggerDelay = "TRIGger:DELay";
        public const string SCPI_TriggerSequenceId = "TRIGger:SEQuence";
        public const string SCPI_PTJM_TriggerLevel = "TRIGger:PTJ:LEVel";
        public const string SCPI_SerialNumber = "SYST:INF:INST:SNUM";
        public const string SCPI_PartNumber = "SYST:INF:INST:PNUM";
        public const string SCPI_ModelName = "SYST:INF:INST:MODel";
        public const string SCPI_CalDate = "SYST:INF:INST:CDATe";
        public const string SCPI_MfgDate = "SYST:INF:INST:MDATe";
        public const string SCPI_ProbeType = "SYST:INF:PROBe:TYPE";
        public const string SCPI_ProbeModel = "SYST:INF:PROBe:MODel";
        public const string SCPI_ProbeSerial = "SYST:INF:PROBe:SNUM";
        public const string SCPI_ProbeResponsivity = "SYST:INF:PROBe:RESPonsivity";
        public const string SCPI_ProbeCalDate = "SYST:INF:PROBe:CDATE";
        public const string SCPI_ProbeMfgDate = "SYST:INF:PROBe:MDATe";
        public const string SCPI_ProbeTemperature = "SYST:INF:PROBe:TEMPerature";
        public const string SCPI_ProbeDiameter = "SYST:INF:PROBe:DIAMeter";
        public const string SCPI_SetPassword = "CALIBration:PASSword";
        public const string SCPI_CalCommit = "CALIBration:COMMit";
        public const string SCPI_PersistentMemoryWipe = "CALIBration:WIPE";
        public const string SCPI_ProbeTypeOverride = "CALIBration:INST:PROB";
        public const string SCPI_HighSpeedThermal_ProbeType = "STHER,1.06E-2";
        public const string SCPI_FpgaAveragingId = "CALIB:INST:FPGA:AVER";
        public const string SCPI_FpgaDecimation = "CALIB:INST:FPGA:DEC";
        public const string SCPI_ZeroSequenceId = "SYST:SYNC";
        public const string SCPI_FirmwareVersion = "syst:inf:inst:fver";
        protected string FirmwareVersion_Cached;
        public const string SCPI_FpgaFver = "syst:inf:fpga:fver";
        public const string SCPI_FpgaHver = "syst:inf:fpga:hver";
        public const string SCPI_ProbeRomVer = "syst:inf:prob:rev";
        public const string MinimumFPGA_FirmwareVersion = "20140303";
        public const string MinimumFPGA_HardwareVersion = "20140303";
        public const string MinimumFirmwareVersion = "01.00.18";
        public const string MinimumFirmwareVersionMeterless = "03.00.06";
        public readonly VersionMoniker FirmwareVersion_FirstWithRomVersion = new VersionMoniker("0.00.33");
        public readonly VersionMoniker FirmwareVersion_FirstWithForceCommand = new VersionMoniker("01.00.12");
        public readonly VersionMoniker FirmwareVersion_FirstWithWorkingZero = new VersionMoniker("01.00.15");
        public readonly VersionMoniker FirmwareVersion_FirstWith_EEPEOM_UpgradeAllowed = new VersionMoniker("01.00.18");
        public readonly VersionMoniker FirmwareVersion_FirstWith_V2_Release = new VersionMoniker("02.00.00");
        public readonly VersionMoniker FirmwareVersion_FirstWithHeisenbergSensorType = new VersionMoniker("01.00.20");
        public readonly VersionMoniker FirmwareVersion_FirstWithWorkingQuadBinary = new VersionMoniker("02.00.06");
        public readonly VersionMoniker FirmwareVersion_FirstWithFastEnergy = new VersionMoniker("02.00.10");
        public readonly VersionMoniker FirmwareVersion_FirstWithMeterType = new VersionMoniker("02.00.23");
        public readonly VersionMoniker FirmwareVersion_FirstWithDynamicTriggerLevels = new VersionMoniker("03.00.22");
        public readonly VersionMoniker FirmwareVersion_FirstWithConfigBaudRate = new VersionMoniker("03.00.19");
        public const string SCPI_Reset = "*RST";
        public const string SCPI_Restore = "SYSTem:RESTore";
        public const int WaitConfigureZero_Timeout = 15000;
        public const int WaitConfigureZero_MinElapsed = 2000;
        public const int WaitConfigureZero_SleepInterval = 500;
        public const string SCPI_ConfigureZero = "CONFigure:ZERO";
        public const string SCPI_DataEncodingSelect = "CONFigure:READings:MODe";
        public const string SCPI_ConfigItem = "CONFigure:ITEM";
        public const DataFieldFlags LEGAL_DATA_FIELDS = DataFieldFlags.Primary | DataFieldFlags.Quad | DataFieldFlags.Flags | DataFieldFlags.Sequence | DataFieldFlags.Period;
        protected static FlagConverter<DataFieldFlags> DataItemConverter = new FlagConverter<DataFieldFlags>(new Dictionary<string, DataFieldFlags>()
    {
      {
        "PRI",
        DataFieldFlags.Primary
      },
      {
        "QUAD",
        DataFieldFlags.Quad
      },
      {
        "FLAG",
        DataFieldFlags.Flags
      },
      {
        "SEQ",
        DataFieldFlags.Sequence
      },
      {
        "PER",
        DataFieldFlags.Period
      }
    });
        public const string SCPI_SnapshotMode = "CONF:MEAS:SNAP:SEL";
        public const string SCPI_PreTriggerDelay = "CONF:MEAS:SNAP:PRE";
        public const string SCPI_ForceTrigger = "FORCe";
        public const string SCPI_MeasurementWindow = "CONF:MEAS:WIN";
        public const string SCPI_QueryMeterType = "SYST:INF:INST:TYPE";
        public const string SCPI_MeterType_TOP = "TOP";
        public const string SCPI_MeterType_TO = "TO";
        public const string SCPI_MeterType_T = "T";
        public const string SCPI_ReadNewDataRecord = "READ";
        public const string SCPI_Start = "STARt";
        public const string SCPI_Stop = "STOP";

        protected override void EnableHandshaking() => this.Communicator.SendAndReceiveCommand("SYST:COMM:HAND", (object)"ON");

        protected override void DisableHandshaking()
        {
            string oneLine = this.Communicator.SendQueryAndReceiveOneLine(this.Handshaking_OFF);
            if (!oneLine.IsOn())
                return;
            this.Trace("WARNING: DisableHandshaking Query.IsOn: " + oneLine);
        }

        public override string Identification => this.Communicator.SendAndReceiveQuery("*IDN");

        public string ConfigureBaudRate
        {
            get
            {
                if (!this.FirmwareVersion_HasConfigBaudRate)
                    return "NA";
                string oneLine = this.Communicator.SendQueryAndReceiveOneLine("SYST:COMM:SER:BAUD");
                if (oneLine.IsErr())
                    return oneLine;
                this.Communicator.Channel.ReadLine();
                return oneLine;
            }
            set => this.Communicator.SendAndReceiveCommand("SYST:COMM:SER:BAUD", (object)value);
        }

        protected string SystemStatus => this.Communicator.SendAndReceiveQuery("SYST:STAT");

        protected uint SystemStatus_AsUInt => (uint)((int)FromDevice.Hex(this.SystemStatus) | (int)Device_SSIM.FakeSystemStatus_AsUInt | (Device_SSIM.FakeSystemFaults_AsUInt != 0U ? int.MinValue : 0));

        public override SystemStatusBits SystemStatus_AsEnum => (SystemStatusBits)this.SystemStatus_AsUInt;

        protected string SystemFaults => this.Communicator.SendAndReceiveQuery("SYST:FAULt");

        protected uint SystemFaults_AsUInt => FromDevice.Hex(this.SystemFaults) | Device_SSIM.FakeSystemFaults_AsUInt;

        public override SystemFaultBits SystemFaults_AsEnum => (SystemFaultBits)this.SystemFaults_AsUInt;

        public override string SystemType => this.Communicator.SendAndReceiveQuery("SYST:TYPE");

        protected string OperatingMode
        {
            get => this.Communicator.SendAndReceiveQuery("CONFigure:MEASure:MODe");
            set => this.Communicator.SendAndReceiveCommand("CONFigure:MEASure:MODe", (object)value);
        }

        public override Library.OperatingMode OperatingMode_AsEnum
        {
            get => SCPI.OpModeNames.FromString(this.OperatingMode);
            set => this.OperatingMode = SCPI.OpModeNames.ToString(value);
        }

        protected string MeasurementSpeed
        {
            get => this.Communicator.SendAndReceiveQuery("CONFigure:MEASure:SOUR:SEL");
            set => this.Communicator.SendAndReceiveCommand("CONFigure:MEASure:SOUR:SEL", (object)value);
        }

        public override MeasurementChannelFlags MeasurementSpeed_AsEnum
        {
            get => SCPI.SpeedConverter.FromString(this.MeasurementSpeed);
            set => this.MeasurementSpeed = SCPI.SpeedConverter.ToString(value);
        }

        protected string QuerySpeedList => this.Communicator.SendAndReceiveQuery("CONFigure:MEAS:SOUR:LIST");

        public override MeasurementChannelFlags QuerySpeedList_AsEnum => (MeasurementChannelFlags)SCPI.SpeedConverter.ToBitMask(this.QuerySpeedList);

        public string EnableSpeedup
        {
            get => this.Communicator.SendAndReceiveQuery("CONFigure:SPEedup");
            set => this.Communicator.SendAndReceiveCommand("CONFigure:SPEedup", (object)value);
        }

        public override bool EnableSpeedup_AsBool
        {
            get => FromDevice.Bool(this.EnableSpeedup);
            set => this.EnableSpeedup = ToDevice.Bool(value);
        }

        public string EnableAreaCorrection
        {
            get => this.Communicator.SendAndReceiveQuery("CONFigure:AREA:CORRection");
            set => this.Communicator.SendAndReceiveCommand("CONFigure:AREA:CORRection", (object)value);
        }

        public override bool EnableAreaCorrection_AsBool
        {
            get => FromDevice.Bool(this.EnableAreaCorrection);
            set => this.EnableAreaCorrection = ToDevice.Bool(value);
        }

        public override string AreaCorrectionValue
        {
            get => this.Communicator.SendAndReceiveQuery("CONFigure:AREA:APERture");
            set => this.Communicator.SendAndReceiveCommand("CONFigure:AREA:APERture", (object)value);
        }

        public override double AreaCorrectionValue_AsReal
        {
            get => FromDevice.Real(this.AreaCorrectionValue);
            set => this.AreaCorrectionValue = ToDevice.Real(value);
        }

        public override string AnalogOutputLevel
        {
            get => this.Communicator.SendAndReceiveQuery("CONFigure:AOUT:FSCale");
            set => this.Communicator.SendAndReceiveCommand("CONFigure:AOUT:FSCale", (object)value);
        }

        public override Library.AnalogOutputLevel AnalogOutputLevel_AsEnum
        {
            get => SCPI.AnalogOutNames.FromString(this.AnalogOutputLevel);
            set => this.AnalogOutputLevel = SCPI.AnalogOutNames.ToString(value);
        }

        protected string EnableSmoothing
        {
            get => this.Communicator.SendAndReceiveQuery("CONFigure:AVERage:TIME");
            set => this.Communicator.SendAndReceiveCommand("CONFigure:AVERage:TIME", (object)value);
        }

        public override bool EnableSmoothing_AsBool
        {
            get => FromDevice.Bool(this.EnableSmoothing);
            set => this.EnableSmoothing = ToDevice.Bool(value);
        }

        public string EnableWavelengthCorrection
        {
            get => this.Communicator.SendAndReceiveQuery("CONF:WAVE:CORR");
            set => this.Communicator.SendAndReceiveCommand("CONF:WAVE:CORR", (object)value);
        }

        public override bool EnableWavelengthCorrection_AsBool
        {
            get => FromDevice.Bool(this.EnableWavelengthCorrection);
            set => this.EnableWavelengthCorrection = ToDevice.Bool(value);
        }

        public string WavelengthCorrectionValue
        {
            get => this.Communicator.SendAndReceiveQuery("CONF:WAVE:WAVE");
            set => this.Communicator.SendAndReceiveCommand("CONF:WAVE:WAVE", (object)value);
        }

        public override uint WavelengthCorrectionValue_AsUint
        {
            get => FromDevice.Uint(this.WavelengthCorrectionValue);
            set => this.WavelengthCorrectionValue = value.ToString();
        }

        public string WavelengthCorrectionMin => this.Communicator.SendAndReceiveQuery("CONF:WAVE:WAVE", (object)"MIN");

        public override uint WavelengthCorrectionMin_AsUint => FromDevice.Uint(this.WavelengthCorrectionMin);

        public string WavelengthCorrectionMax => this.Communicator.SendAndReceiveQuery("CONF:WAVE:WAVE", (object)"MAX");

        public override uint WavelengthCorrectionMax_AsUint => FromDevice.Uint(this.WavelengthCorrectionMax);

        public string WavelengthTable => this.Communicator.SendAndReceiveQuery("CONFigure:WAVElength:LIST");

        public override List<uint> WavelengthTable_AsList
        {
            get
            {
                string wavelengthTable = this.WavelengthTable;
                if (wavelengthTable.IsNullOrWhiteSpace())
                    return (List<uint>)null;
                return ((IEnumerable<string>)wavelengthTable.Split(',')).Select<string, uint>((Func<string, uint>)(item => FromDevice.Uint(item))).Where<uint>((Func<uint, bool>)(item => item > 0U)).ToList<uint>();
            }
        }

        protected string EnableAutoRange
        {
            get => this.Communicator.SendAndReceiveQuery("CONFigure:RANGE:AUTO");
            set => this.Communicator.SendAndReceiveCommand("CONFigure:RANGE:AUTO", (object)value);
        }

        public override bool EnableAutoRange_AsBool
        {
            get => FromDevice.Bool(this.EnableAutoRange);
            set => this.EnableAutoRange = ToDevice.Bool(value);
        }

        protected string SelectedRange
        {
            get => this.Communicator.SendAndReceiveQuery("CONFigure:RANGE:SELect");
            set => this.Communicator.SendAndReceiveCommand("CONFigure:RANGE:SELect", (object)value);
        }

        public override double SelectedRange_AsReal
        {
            get => FromDevice.Real(this.SelectedRange);
            set => this.SelectedRange = ToDevice.Real(value);
        }

        protected string QueryRangeList => this.Communicator.SendAndReceiveQuery("CONFigure:RANGe:LIST");

        public override List<double> QueryRangeList_AsList
        {
            get
            {
                string queryRangeList = this.QueryRangeList;
                if (queryRangeList.IsNullOrEmpty())
                    return (List<double>)null;
                return ((IEnumerable<string>)queryRangeList.Split(',')).Select<string, double>((Func<string, double>)(item => FromDevice.Range(item))).ToList<double>();
            }
        }

        protected string RangeMin => this.Communicator.SendAndReceiveQuery("CONFigure:RANGE:SELect", (object)"MIN");

        public override double RangeMin_AsReal => FromDevice.Real(this.RangeMin);

        protected string RangeMax => this.Communicator.SendAndReceiveQuery("CONFigure:RANGE:SELect", (object)"MAX");

        public override double RangeMax_AsReal => FromDevice.Real(this.RangeMax);

        protected string EnableGainCompensation
        {
            get => this.Communicator.SendAndReceiveQuery("CONFigure:GAIN:COMPensation");
            set => this.Communicator.SendAndReceiveCommand("CONFigure:GAIN:COMPensation", (object)value);
        }

        public override bool EnableGainCompensation_AsBool
        {
            get => FromDevice.Bool(this.EnableGainCompensation);
            set => this.EnableGainCompensation = ToDevice.Bool(value);
        }

        protected string GainCompensationFactor
        {
            get => this.Communicator.SendAndReceiveQuery("CONFigure:GAIN:FACTor");
            set => this.Communicator.SendAndReceiveCommand("CONFigure:GAIN:FACTor", (object)value);
        }

        public override double GainCompensationFactor_AsReal
        {
            get => FromDevice.Real(this.GainCompensationFactor);
            set => this.GainCompensationFactor = ToDevice.Real(value);
        }

        public override string DecimationRate
        {
            get => this.Communicator.SendAndReceiveQuery("CONFigure:DECimation");
            set => this.Communicator.SendAndReceiveCommand("CONFigure:DECimation", (object)value);
        }

        public override uint DecimationRate_AsUint
        {
            get => FromDevice.Uint(this.DecimationRate, 1U);
            set => this.DecimationRate = value.ToString();
        }

        public string TriggerSource
        {
            get => this.Communicator.SendAndReceiveQuery("TRIGger:SOURce");
            set => this.Communicator.SendAndReceiveCommand("TRIGger:SOURce", (object)value);
        }

        public override Library.TriggerSource TriggerSource_AsEnum
        {
            get => SCPI.TrigSourceNames.FromString(this.TriggerSource);
            set => this.TriggerSource = SCPI.TrigSourceNames.ToString(value);
        }

        public string TriggerLevel
        {
            get => this.Communicator.SendAndReceiveQuery("TRIGger:LEVEL");
            set => this.Communicator.SendAndReceiveCommand("TRIGger:LEVEL", (object)value);
        }

        public override double TriggerLevel_AsReal
        {
            get => FromDevice.Real(this.TriggerLevel);
            set => this.TriggerLevel = ToDevice.Real(value);
        }

        public override void SetTriggerLevel(double level) => this.Communicator.SendCommandWhileRunning("TRIGger:LEVEL", (object)ToDevice.Real(level));

        public string TriggerLevelMinimum => this.Communicator.SendAndReceiveQuery("TRIGger:LEVEL", (object)"MIN");

        public override double TriggerLevelMinimum_AsReal => FromDevice.Real(this.TriggerLevelMinimum);

        public string TriggerLevelMaximum => this.Communicator.SendAndReceiveQuery("TRIGger:LEVEL", (object)"MAX");

        public override double TriggerLevelMaximum_AsReal => FromDevice.Real(this.TriggerLevelMaximum);

        public string TriggerLevelPercent => this.Communicator.SendAndReceiveQuery("TRIGger:PERCENT:LEVEL");

        public override double TriggerLevelPercent_AsReal => FromDevice.Real(this.TriggerLevelPercent);

        public string TriggerLevelPercentMinimum => this.Communicator.SendAndReceiveQuery("TRIGger:PERCENT:LEVEL", (object)"MIN");

        public override double TriggerLevelPercentMinimum_AsReal => FromDevice.Real(this.TriggerLevelPercentMinimum);

        public string TriggerLevelPercentMaximum => this.Communicator.SendAndReceiveQuery("TRIGger:PERCENT:LEVEL", (object)"MAX");

        public override double TriggerLevePercentMaximum_AsReal => FromDevice.Real(this.TriggerLevelPercentMaximum);

        public string TriggerSlope
        {
            get => this.Communicator.SendAndReceiveQuery("TRIGger:SLOpe");
            set => this.Communicator.SendAndReceiveCommand("TRIGger:SLOpe", (object)value);
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
            get => this.Communicator.SendAndReceiveQuery("SYST:INF:INST:SNUM");
            set => this.Communicator.SendAndReceiveCommand("SYST:INF:INST:SNUM", (object)value);
        }

        public override string PartNumber
        {
            get => this.Communicator.SendAndReceiveQuery("SYST:INF:INST:PNUM");
            set => this.Communicator.SendAndReceiveCommand("SYST:INF:INST:PNUM", (object)value);
        }

        public override string ModelName
        {
            get => this.Communicator.SendAndReceiveQuery("SYST:INF:INST:MODel");
            set => this.Communicator.SendAndReceiveCommand("SYST:INF:INST:MODel", (object)value);
        }

        public override string CalibrationDate
        {
            get => this.Communicator.SendAndReceiveQuery("SYST:INF:INST:CDATe");
            set => this.Communicator.SendAndReceiveCommand("SYST:INF:INST:CDATe", (object)value);
        }

        public override string ManufactureDate
        {
            get => this.Communicator.SendAndReceiveQuery("SYST:INF:INST:MDATe");
            set => this.Communicator.SendAndReceiveCommand("SYST:INF:INST:MDATe", (object)value);
        }

        public override string ProbeTypeAndQualifier => this.Communicator.SendAndReceiveQuery("SYST:INF:PROBe:TYPE");

        public override string ProbeModel => this.Communicator.SendAndReceiveQuery("SYST:INF:PROBe:MODel");

        public override string ProbeSerialNumber => this.Communicator.SendAndReceiveQuery("SYST:INF:PROBe:SNUM");

        protected string ProbeResponsivity => this.Communicator.SendAndReceiveQuery("SYST:INF:PROBe:RESPonsivity");

        public override double ProbeResponsivity_AsReal => FromDevice.Real(this.ProbeResponsivity);

        public override string ProbeCalDate => this.Communicator.SendAndReceiveQuery("SYST:INF:PROBe:CDATE");

        public override string ProbeManDate => this.Communicator.SendAndReceiveQuery("SYST:INF:PROBe:MDATe");

        public override string SensorTemperature => this.Communicator.SendAndReceiveQuery("SYST:INF:PROBe:TEMPerature");

        public override string ProbeDiameter => this.Communicator.SendAndReceiveQuery("SYST:INF:PROBe:DIAMeter");

        public override double ProbeDiameter_AsReal => FromDevice.RealMaybeNA(this.ProbeDiameter);

        public override bool ProbeDiameter_IsAvailable => this.ProbeDiameter != "NA";

        public override void SetCalibrationPassword(string password) => this.Communicator.SendAndReceiveCommand("CALIBration:PASSword", (object)password);

        public override void CalibrationCommit() => this.Communicator.SendAndReceiveCommand("CALIBration:COMMit");

        public override void PersistentMemoryWipe() => this.Communicator.SendAndReceiveCommand("CALIBration:WIPE");

        protected override void ProbeTypeOverride(string probe) => this.Communicator.SendAndReceiveCommand("CALIBration:INST:PROB", (object)probe);

        public override void ImpersonateHighSpeedProbe()
        {
            this.SetCalibrationPassword("rosebud");
            this.ProbeTypeOverride("STHER,1.06E-2");
        }

        public override string FpgaAveragingId
        {
            get => this.Communicator.SendAndReceiveQuery("CALIB:INST:FPGA:AVER");
            set => this.Communicator.SendAndReceiveCommand("CALIB:INST:FPGA:AVER", (object)value);
        }

        public override uint FpgaAveragingId_AsUint
        {
            get => FromDevice.Uint(this.FpgaAveragingId);
            set => this.FpgaAveragingId = value.ToString();
        }

        public override string FpgaDecimation
        {
            get => this.Communicator.SendAndReceiveQuery("CALIB:INST:FPGA:DEC");
            set => this.Communicator.SendAndReceiveCommand("CALIB:INST:FPGA:DEC", (object)value);
        }

        public override uint FpgaDecimation_AsUint
        {
            get => FromDevice.Uint(this.FpgaDecimation, 1U);
            set => this.FpgaDecimation = value.ToString();
        }

        public override void ClearSequenceId() => this.Communicator.SendAndReceiveCommand("SYST:SYNC");

        public override string FirmwareVersion
        {
            get
            {
                if (this.FirmwareVersion_Cached == null)
                {
                    string query = this.Communicator.SendAndReceiveQuery("syst:inf:inst:fver");
                    if (query != null)
                        this.FirmwareVersion_Cached = VersionMoniker.StripVersionCrap(query);
                }
                return this.FirmwareVersion_Cached;
            }
        }

        public override string FpgaFirmwareVersion => this.Communicator.SendAndReceiveQuery("syst:inf:fpga:fver");

        public override string FpgaHardwareVersion => this.Communicator.SendAndReceiveQuery("syst:inf:fpga:hver");

        public override bool FirmwareVersion_HasProbeRomVersion => this.FirmwareVersion != null && this.FirmwareVersion_FirstWithRomVersion <= this.FirmwareVersion;

        public override string ProbeRomVersion => !this.FirmwareVersion_HasProbeRomVersion ? "None" : this.Communicator.SendAndReceiveQuery("syst:inf:prob:rev");

        public override bool FirmwareVersion_HasForceCommand => this.FirmwareVersion != null && this.FirmwareVersion_FirstWithForceCommand <= this.FirmwareVersion;

        public override bool FirmwareVersion_HasWorkingZero => this.FirmwareVersion != null && this.FirmwareVersion_FirstWithWorkingZero <= this.FirmwareVersion;

        public override bool FirmwareVersion_EEPEOM_UpgradeAllowed => this.FirmwareVersion != null && this.FirmwareVersion_FirstWith_EEPEOM_UpgradeAllowed <= this.FirmwareVersion;

        public override bool FirmwareVersion_V2_Release => this.FirmwareVersion != null && this.FirmwareVersion_FirstWith_V2_Release <= this.FirmwareVersion;

        public override bool FirmwareVersion_HasHeisenbergSensorType => this.FirmwareVersion != null && this.FirmwareVersion_FirstWithHeisenbergSensorType <= this.FirmwareVersion;

        public override bool FirmwareVersion_HasWorkingQuadBinary => this.FirmwareVersion != null && this.FirmwareVersion_FirstWithWorkingQuadBinary <= this.FirmwareVersion;

        public override bool FirmwareVersion_HasFastEnergy => this.FirmwareVersion != null && this.FirmwareVersion_FirstWithFastEnergy <= this.FirmwareVersion;

        public override bool FirmwareVersion_HasMeterType => this.FirmwareVersion != null && this.FirmwareVersion_FirstWithMeterType <= this.FirmwareVersion;

        public override bool FirmwareVersion_HasDynamicTriggerLevels => this.FirmwareVersion != null && this.FirmwareVersion_FirstWithDynamicTriggerLevels <= this.FirmwareVersion;

        public override bool FirmwareVersion_HasConfigBaudRate => this.FirmwareVersion != null && this.FirmwareVersion_FirstWithDynamicTriggerLevels <= this.FirmwareVersion;

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
            this.ConfigureZero_Wait();
        }

        public void ConfigureZero_Wait()
        {
            Stopwatch stopwatch = new Stopwatch();
            int num = 0;
            stopwatch.Start();
            if ((this.SystemStatus_AsEnum & SystemStatusBits.BusyZeroing) == (SystemStatusBits)0)
            {
                while (stopwatch.ElapsedMilliseconds <= 15000L && (this.SystemStatus_AsEnum & SystemStatusBits.BusyZeroing) == (SystemStatusBits)0)
                    Thread.Sleep(10);
            }
            while (stopwatch.ElapsedMilliseconds <= 15000L && (this.SystemStatus_AsEnum & SystemStatusBits.BusyZeroing) != (SystemStatusBits)0)
            {
                ++num;
                Thread.Sleep(500);
            }
        }

        public string QueryZeroBaseline() => this.Communicator.SendAndReceiveQuery("CONFigure:ZERO");

        public double QueryZeroBaseline_AsReal() => FromDevice.Real(this.QueryZeroBaseline());

        protected string DataEncoding
        {
            get => this.Communicator.SendAndReceiveQuery("CONFigure:READings:MODe");
            set => this.Communicator.SendAndReceiveCommand("CONFigure:READings:MODe", (object)value);
        }

        public override Library.DataEncoding DataEncoding_AsEnum
        {
            get => SCPI.ReadConverter.FromString(this.DataEncoding);
            set => this.DataEncoding = SCPI.ReadConverter.ToString(value);
        }

        protected string ConfigureDataFields
        {
            get => this.Communicator.SendAndReceiveQuery("CONFigure:ITEM");
            set => this.Communicator.SendAndReceiveCommand("CONFigure:ITEM", (object)value);
        }

        public override DataFieldFlags ConfigureDataFields_AsEnum
        {
            get => (DataFieldFlags)Device_SSIM.DataItemConverter.ToBitMask(this.ConfigureDataFields);
            set => this.ConfigureDataFields = Device_SSIM.DataItemConverter.ToStringList(value & (DataFieldFlags.Primary | DataFieldFlags.Quad | DataFieldFlags.Flags | DataFieldFlags.Sequence | DataFieldFlags.Period));
        }

        public override string EnableSnapshotMode
        {
            get => this.Communicator.SendAndReceiveQuery("CONF:MEAS:SNAP:SEL");
            set => this.Communicator.SendAndReceiveCommand("CONF:MEAS:SNAP:SEL", (object)value);
        }

        public override bool EnableSnapshotMode_AsBool
        {
            get => FromDevice.Bool(this.EnableSnapshotMode);
            set => this.EnableSnapshotMode = ToDevice.Bool(value);
        }

        public override string PreTriggerDelay
        {
            get => this.Communicator.SendAndReceiveQuery("CONF:MEAS:SNAP:PRE");
            set => this.Communicator.SendAndReceiveCommand("CONF:MEAS:SNAP:PRE", (object)value);
        }

        public override uint PreTriggerDelay_AsUint
        {
            get => FromDevice.Uint(this.PreTriggerDelay);
            set => this.PreTriggerDelay = value.ToString();
        }

        public override void ForceTrigger() => this.Communicator.SendCommandWhileRunning("FORCe");

        public string MeasurementWindow
        {
            get => this.Communicator.SendAndReceiveQuery("CONF:MEAS:WIN");
            set => this.Communicator.SendAndReceiveCommand("CONF:MEAS:WIN", (object)value);
        }

        public override uint MeasurementWindow_AsUint
        {
            get => !this.FirmwareVersion_HasFastEnergy ? 0U : FromDevice.Uint(this.MeasurementWindow);
            set
            {
                if (!this.FirmwareVersion_HasFastEnergy)
                    return;
                this.MeasurementWindow = value.ToString();
            }
        }

        public override string MeterType => !this.FirmwareVersion_HasMeterType ? "None" : this.Communicator.SendAndReceiveQuery("SYST:INF:INST:TYPE");

        public override bool MeterHasPyro => this.FirmwareVersion_HasMeterType && this.MeterType.Equals("TOP", StringComparison.InvariantCultureIgnoreCase);

        public override string ReadNewDataRecord() => this.Communicator.SendAndReceiveQuery("READ");

        public override void Start(uint count = 0)
        {
            this.DisableHandshaking();
            this.Communicator.SendCommandNoWaitReply(count == 0U ? "STARt" : string.Format("{0} {1}", (object)"STARt", (object)count));
        }

        public override void Stop()
        {
            this.Communicator.SendStopCommand("STOP");
            this.EnableHandshaking();
        }

        [API(APICategory.Unclassified)]
        public override DataRecordSingle DecodeMeasurement(string measurement)
        {
            if (!string.IsNullOrEmpty(measurement))
            {
                if (!measurement.IsOK())
                {
                    try
                    {
                        return (DataRecordBase.SelectedDataFields & DataFieldFlags.Quad) != (DataFieldFlags)0 ? (DataRecordSingle)new DataRecordQuad(measurement) : new DataRecordSingle(measurement);
                    }
                    catch (Exception ex)
                    {
                        this.Trace("DecodeMeasurement Exception: {0}", (object)ex.Message);
                    }
                    return (DataRecordSingle)null;
                }
            }
            return (DataRecordSingle)null;
        }
    }
}
