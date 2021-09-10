
using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Measurement, "Delegate for PhoenixMeter's Meter Status Changed events. ")]
    public delegate void MeterStatusChanged(object sender, MeterStatus state);
}
