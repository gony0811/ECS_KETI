

namespace DEV.PowerMeter.Library
{
    public interface ILicensedMeter : ILicensedItem
    {
        bool IsOpen { get; }

        bool IsLegacyMeterlessPower { get; }

        bool IsLegacyMeterlessEnergy { get; }
    }
}
