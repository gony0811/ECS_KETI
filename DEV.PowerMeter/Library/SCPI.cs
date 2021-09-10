
using System;
using System.Collections.Generic;

namespace DEV.PowerMeter.Library
{
    public static class SCPI
    {
        public const string NotificiationPrefix = "#";
        public const string QuerySuffix = "?";
        public const string CommandTerminator = "\r\n";
        public const string Comma = ",";
        public const string CommaSpace = ", ";
        public const string ArgSeparator = " ";
        public const string Min = "MIN";
        public const string Max = "MAX";
        public const string Def = "DEF";
        public const string Mid = "MID";
        public const string Med = "MED";
        public const string OK = "OK";
        public const string ERR = "ERR";
        public const string NA = "NA";
        public const string ON = "ON";
        public const string OFF = "OFF";
        public const string SemiColon = ";";
        public const string VerticalBar = "|";
        public static EnumConverter<OperatingMode> OpModeNames = new EnumConverter<OperatingMode>(new Dictionary<string, OperatingMode>()
    {
      {
        "DBM",
        OperatingMode.PowerDbm
      },
      {
        "J",
        OperatingMode.EnergyJoules
      },
      {
        "W",
        OperatingMode.PowerWatts
      }
    });
        public static EnumConverter<AnalogOutputLevel> AnalogOutNames = new EnumConverter<AnalogOutputLevel>(new Dictionary<string, AnalogOutputLevel>()
    {
      {
        "1",
        AnalogOutputLevel.One
      },
      {
        "2",
        AnalogOutputLevel.Two
      },
      {
        "4",
        AnalogOutputLevel.Four
      }
    });
        public static EnumConverter<TriggerLevel_LPEM> LPEM_TrigLevelNames = new EnumConverter<TriggerLevel_LPEM>(new Dictionary<string, TriggerLevel_LPEM>()
    {
      {
        "Low",
        TriggerLevel_LPEM.Low
      },
      {
        "Medium",
        TriggerLevel_LPEM.Medium
      },
      {
        "High",
        TriggerLevel_LPEM.High
      }
    });
        public static EnumConverter<TriggerSource> TrigSourceNames = new EnumConverter<TriggerSource>(new Dictionary<string, TriggerSource>()
    {
      {
        "INT",
        TriggerSource.Internal
      },
      {
        "EXT",
        TriggerSource.External
      }
    });
        public static EnumConverter<TriggerSlope> TrigSlopeConverter = new EnumConverter<TriggerSlope>(new Dictionary<string, TriggerSlope>()
    {
      {
        "POS",
        TriggerSlope.Positive
      },
      {
        "NEG",
        TriggerSlope.Negative
      }
    });
        public static EnumConverter<DataEncoding> ReadConverter = new EnumConverter<DataEncoding>(new Dictionary<string, DataEncoding>()
    {
      {
        "BINARY",
        DataEncoding.Binary
      },
      {
        "ASCII",
        DataEncoding.Ascii
      }
    });
        public static EnumConverter<SensorType> SensorTypeConverter = new EnumConverter<SensorType>(new Dictionary<string, SensorType>()
    {
      {
        "NONE",
        SensorType.None
      },
      {
        "THERMO",
        SensorType.Thermo
      },
      {
        "PYRO",
        SensorType.Pyro
      },
      {
        "OPT",
        SensorType.Optical
      }
    });
        public static EnumConverter<SensorTypeQualifier> SensorTypeQualifierConverter = new EnumConverter<SensorTypeQualifier>(new Dictionary<string, SensorTypeQualifier>()
    {
      {
        "NONE",
        SensorTypeQualifier.None
      },
      {
        "NOSPEC",
        SensorTypeQualifier.Nospec
      },
      {
        "SINGLE",
        SensorTypeQualifier.Single
      },
      {
        "QUAD",
        SensorTypeQualifier.Quad
      },
      {
        "ENHQUAD",
        SensorTypeQualifier.EnhQuad
      }
    });
        public static EnumConverter<SystemType> SystemTypeConverter = new EnumConverter<SystemType>(new Dictionary<string, SystemType>()
    {
      {
        "SSIM",
        SystemType.SSIM
      },
      {
        "PM-Pro",
        SystemType.PmPro
      },
      {
        "PowerMax USB/RS",
        SystemType.PowerMax
      },
      {
        "EnergyMax USB/RS",
        SystemType.EnergyMax
      }
    });
        public static FlagConverter<MeasurementChannelFlags> SpeedConverter = new FlagConverter<MeasurementChannelFlags>(new Dictionary<string, MeasurementChannelFlags>()
    {
      {
        "SLOW",
        MeasurementChannelFlags.Slow
      },
      {
        "FAST",
        MeasurementChannelFlags.Fast
      },
      {
        "NONE",
        MeasurementChannelFlags.None
      }
    });
        public static FlagConverter<SystemStatusBits> StatusBitsConverter = new FlagConverter<SystemStatusBits>(new Dictionary<string, SystemStatusBits>()
    {
      {
        "Sensor",
        SystemStatusBits.SensorIsAttached
      },
      {
        "Ident",
        SystemStatusBits.BusyIdentifying
      },
      {
        "Zero",
        SystemStatusBits.BusyZeroing
      },
      {
        "Calc",
        SystemStatusBits.BusyCalculating
      },
      {
        "FPGA",
        SystemStatusBits.FpgaUpdating
      },
      {
        "Fault",
        SystemStatusBits.SystemFault
      }
    });
        public static FlagConverter<SystemFaultBits> FaultsBitsConverter = new FlagConverter<SystemFaultBits>(new Dictionary<string, SystemFaultBits>()
    {
      {
        "Missing",
        SystemFaultBits.SensorMissing
      },
      {
        "HOT",
        SystemFaultBits.SensorOvertemp
      },
      {
        "COMM",
        SystemFaultBits.SensorCommFailure
      },
      {
        "CHKSUM",
        SystemFaultBits.SensorChecksum
      },
      {
        "FW",
        SystemFaultBits.SensorFirmware
      },
      {
        "EEPROM",
        SystemFaultBits.SensorEEPROM
      },
      {
        "UNK",
        SystemFaultBits.SensorUnrecognized
      },
      {
        "INIT",
        SystemFaultBits.BadInitialization
      },
      {
        "ZERO",
        SystemFaultBits.BadZero
      },
      {
        "IPC",
        SystemFaultBits.IPCFailure
      }
    });

        public static char CommaChar => ","[0];

        public static bool IsOn(this string s) => s.Trim().Equals("ON", StringComparison.InvariantCultureIgnoreCase);

        public static bool IsOff(this string s) => s.Trim().Equals("OFF", StringComparison.InvariantCultureIgnoreCase);

        public static bool IsOK(this string s) => s.Trim().ToUpper() == "OK";

        public static bool IsErr(this string s) => !string.IsNullOrEmpty(s) && s.Trim().ToUpper().StartsWith("ERR");

        public static bool IsNA(this string s) => !string.IsNullOrEmpty(s) && s.Trim().ToUpper() == "NA";

        public static char SemiColonChar => ";"[0];

        public static char VerticalBarChar => "|"[0];
    }
}
