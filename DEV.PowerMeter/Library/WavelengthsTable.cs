using SharedLibrary;
using System;
using System.Linq;
using System.Runtime.Serialization;


namespace DEV.PowerMeter.Library
{
    [DataContract]
    [API(APICategory.Measurement, "Wavelengths Table contains a list of all the ranges for a given Sensor.There are two kinds of Wavelengths included:(a) there is a list of wavelengths from the Sensor's EEPROM, which corelates with certain calibration information; and(b) additional wavelengths that the user uses frequently.The user can pick any wavelngth for Wavelength Correction, so long as it's within the given Minimum and Maximum.")]
    public class WavelengthsTable : ExtendableTable<uint>
    {
        [API("placeholder for missing/optional (min/max) values")]
        public const uint MissingWavelength = 0;

        [API("Construct an empty table")]
        public WavelengthsTable()
        {
        }

        [API("Maximum value for Wavelength Correction.")]
        public uint Maximum => !this.IsEmpty ? this.Originals.Max<uint>() : 0U;

        [API("Minimum value for Wavelength Correction.")]
        public uint Minimum => !this.IsEmpty ? this.Originals.Min<uint>() : 0U;

        [API("Test if a value is within range.")]
        public bool ValidRange(uint value) => this.Minimum <= value && value <= this.Maximum;

        [API("Add a new wavelength. Throws ArgumentOutOfRangeException for duplicates and out of range values.")]
        public override void Add(uint value)
        {
            if (!this.ValidRange(value) || this.Contains(value))
                throw new ArgumentOutOfRangeException();
            base.Add(value);
        }

        [API("find an entry in the wavelength table that is closest to the requested one. Returns the actual value, since we don't need (or actally have) indicies. Returns 0 if a match cannot be found. ")]
        public uint FindClosest(uint wavelength)
        {
            uint num1 = 0;
            if (!this.IsEmpty)
            {
                int num2 = int.MaxValue;
                foreach (uint num3 in this.Unordered)
                {
                    int num4 = Math.Abs((int)num3 - (int)wavelength);
                    if (num2 > num4)
                    {
                        num2 = num4;
                        num1 = num3;
                    }
                }
            }
            return num1;
        }

        public override bool TryParse(string s, out uint value) => uint.TryParse(s, out value);
    }
}
