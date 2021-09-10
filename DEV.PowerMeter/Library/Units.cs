
using SharedLibrary;
using System;
using System.Collections.Generic;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Formatting, "Represents the physical units relating to a series of Measurement Values.\r\nUnits tracks the relevant units independently of a Measurement's ScaleFactor and numeric value.\r\nCompliating the matter is that the meter can apply Area Correction,\r\nwhich means normalizing the power or energy measurement values,\r\nby dividing them by the area of the laser beam.\r\nThis produces measurements qualified by the suffix for per square centimeter.\r\n")]
    public class Units : IEquatable<Units>
    {
        [API("AreaCorrectionSuffix as a string")]
        private const string AreaCorrectionSuffix = "/cm\u00B2";
        [API("static instance of Units for Watts measurements")]
        public static readonly Units Watts = new Units(PhysicalUnits.Watts, false);
        [API("static instance of Units for Joules measurements")]
        public static readonly Units Joules = new Units(PhysicalUnits.Joules, false);
        [API("static instance of Units for Seconds")]
        public static readonly Units Seconds = new Units(PhysicalUnits.Seconds, false);
        [API("static instance of Units for Hertz")]
        public static readonly Units Hertz = new Units(PhysicalUnits.Hertz, false);
        [API("Current Units selected by PhoenixMeter object per data acquisition mode")]
        public static Units SelectedUnits = Units.Watts;
        [API("PhysicalUnits for this instance of Units")]
        public PhysicalUnits PhysicalUnits;
        [API("AreaCorrected setting for this instance of Units")]
        public bool AreaCorrected;
        [API("Convert Abbreviations to PhysicalUnits")]
        public static EnumConverter<PhysicalUnits> PhysicalUnitsConverter = new EnumConverter<PhysicalUnits>(new Dictionary<string, PhysicalUnits>()
    {
      {
        "J",
        PhysicalUnits.Joules
      },
      {
        "W",
        PhysicalUnits.Watts
      },
      {
        "dBm",
        PhysicalUnits.Dbm
      },
      {
        "",
        PhysicalUnits.ADC_Counts
      },
      {
        "s",
        PhysicalUnits.Seconds
      },
      {
        "Hz",
        PhysicalUnits.Hertz
      },
      {
        "Ts",
        PhysicalUnits.Timestamp
      },
      {
        "Pid",
        PhysicalUnits.PulseId
      },
      {
        "Psi",
        PhysicalUnits.PressurePSI
      }
    });
        [API("Convert verbose Names to PhysicalUnits")]
        public static EnumConverter<PhysicalUnits> VerbosePhysicalUnitsConverter = new EnumConverter<PhysicalUnits>(new Dictionary<string, PhysicalUnits>()
    {
      {
        "Energy Joules",
        PhysicalUnits.Joules
      },
      {
        "Power Watts",
        PhysicalUnits.Watts
      },
      {
        "Power dBm",
        PhysicalUnits.Dbm
      },
      {
        nameof (Seconds),
        PhysicalUnits.Seconds
      },
      {
        nameof (Hertz),
        PhysicalUnits.Hertz
      },
      {
        "Timestamp",
        PhysicalUnits.Timestamp
      },
      {
        "Pulse Id",
        PhysicalUnits.PulseId
      },
      {
        "Pressure Psi",
        PhysicalUnits.PressurePSI
      }
    });
        [API("List of Names")]
        public static readonly string[] Names = new string[9]
        {
      "Energy Joules",
      "Power Watts",
      "Power dBm",
      "ADC Counts",
      nameof (Seconds),
      nameof (Hertz),
      "Timestamp",
      "Pulse Id",
      "Pressure Psi"
        };
        [API("List of ShortNames")]
        public static readonly string[] ShortNames = new string[9]
        {
      nameof (Joules),
      nameof (Watts),
      "dBm",
      "ADC",
      nameof (Seconds),
      nameof (Hertz),
      "Ts",
      "Pid",
      "Psi"
        };
        [API("Convert OperatingMode to PhysicalUnits")]
        public static readonly Dictionary<OperatingMode, PhysicalUnits> ModeToPhysical = new Dictionary<OperatingMode, PhysicalUnits>()
    {
      {
        OperatingMode.EnergyJoules,
        PhysicalUnits.Joules
      },
      {
        OperatingMode.PowerWatts,
        PhysicalUnits.Watts
      },
      {
        OperatingMode.PowerDbm,
        PhysicalUnits.Dbm
      }
    };
        [API("Convert PhysicalUnits to nominal OperatingMode (excluding AreaCorrection mode)")]
        public static readonly Dictionary<PhysicalUnits, OperatingMode> PhysicalToMode = new Dictionary<PhysicalUnits, OperatingMode>()
    {
      {
        PhysicalUnits.Joules,
        OperatingMode.EnergyJoules
      },
      {
        PhysicalUnits.Watts,
        OperatingMode.PowerWatts
      },
      {
        PhysicalUnits.Dbm,
        OperatingMode.PowerDbm
      }
    };

        static Units() => Units.SelectedUnits = Units.Watts;

        public bool IsConsistentWith(OperatingMode operatingMode) => operatingMode == OperatingMode.EnergyJoules == this.IsEnergy;

        [API("IEquatable for comparing Units")]
        public bool Equals(Units other) => other != null && this.AreaCorrected == other.AreaCorrected && this.PhysicalUnits == other.PhysicalUnits;

        [API("Long name for this instance (e.g. Power Watts or Energy Joules).")]
        public string Name => Units.Names[(int)this.PhysicalUnits];

        [API("Short name for this instance (e.g. Watts or Joules).")]
        public string ShortName => Units.ShortNames[(int)this.PhysicalUnits];

        [API("Abbreviated symbol for this instance (e.g. W or J).")]
        public string Abbreviation => Units.Abbreviations[(int)this.PhysicalUnits];

        [API("Suffix (Abbreviation + Modifier) for numbers.")]
        public string Suffix => this.Abbreviation + this.Modifier;

        [API("Verbose (longer Name + Modifier) for labels and titles.")]
        public string Verbose => this.Name + this.Modifier;

        [API("Optional AreaCorrectionSuffix, if applicable.")]
        public string Modifier => !this.AreaCorrected ? "" : "/cm\u00B2";

        [API("True iff this instance represents an Energy measurement.")]
        public bool IsEnergy => this.PhysicalUnits == PhysicalUnits.Joules;

        [API("True iff this instance represents a Power measurement.")]
        public bool IsPower => this.PhysicalUnits == PhysicalUnits.Watts || this.PhysicalUnits == PhysicalUnits.Dbm;

        [API("Create specific instance for given PhysicalUnits and AreaCorrection setting.")]
        public Units(PhysicalUnits units, bool corrected)
        {
            this.PhysicalUnits = units;
            this.AreaCorrected = corrected;
        }

        [API("Create specific instance for given PhysicalUnits with AreaCorrection off.")]
        public Units(PhysicalUnits units)
          : this(units, false)
        {
        }

        [API("Create specific instance based on a system OperatingMode with AreaCorrection off.")]
        public Units(OperatingMode mode)
          : this(Units.ModeToPhysical[mode], false)
        {
        }

        [API("Create specific instance based on a system OperatingMode and AreaCorrection setting.")]
        public Units(OperatingMode mode, bool corrected)
          : this(Units.ModeToPhysical[mode], corrected)
        {
        }

        [API("Create a new Units which is the energy equivalant of this instance (preserves AreaCorrected setting)")]
        public static Units ToEnergy(Units units)
        {
            if (units.PhysicalUnits == PhysicalUnits.Joules)
                return units;
            return units.PhysicalUnits == PhysicalUnits.Watts ? new Units(PhysicalUnits.Joules, units.AreaCorrected) : throw new Exception("Units.ToEnergy only works with Power units");
        }

        [API("Encoding of Units for use in Export files.")]
        public string Encode() => this.Suffix;

        [API("Decoding of Units for use in Import files.")]
        public static Units Decode(string suffix)
        {
            string[] strArray = suffix.Split('/');
            bool corrected = strArray.Length > 1;
            PhysicalUnits units;
            try
            {
                units = Units.PhysicalUnitsConverter.FromString(strArray[0]);
            }
            catch
            {
                units = Units.Watts.PhysicalUnits;
            }
            return new Units(units, corrected);
        }

        [API("NOT USED")]
        private static Units FromString(string value)
        {
            if (value.StartsWith("Units("))
                value = value.Replace("Units(", "").Replace(")", "").Trim();
            string[] strArray = value.Split('/');
            bool corrected = strArray.Length > 1;
            PhysicalUnits units;
            try
            {
                units = Units.VerbosePhysicalUnitsConverter.FromString(strArray[0]);
            }
            catch
            {
                units = Units.Watts.PhysicalUnits;
            }
            return new Units(units, corrected);
        }

        public override string ToString() => "Units( " + this.Verbose + " )";

        [API("List of Abbreviations")]
        public static string[] Abbreviations => Units.PhysicalUnitsConverter.Keys;
    }
}
