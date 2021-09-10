using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    public class WavelengthOption : NamedValue<uint>
    {
        public const uint CorrectionDisabled_Value = 0;
        public const string DisabledLabel = "[Specify Wavelength]";

        public bool IsDisabled => this.Value == 0U;

        public WavelengthOption(uint value)
          : base(value, WavelengthOption.ToString(value))
        {
        }

        public override string ToString() => WavelengthOption.ToString(this.Value);

        public static string ToString(uint value) => value == 0U ? "[Specify Wavelength]" : string.Format("{0} nm", (object)value);
    }
}
