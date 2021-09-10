using System.Collections.Generic;

namespace DEV.PowerMeter.Library.DeviceModels
{
    public interface IRangeSelection
    {
        double SelectedRange_AsReal { get; set; }

        List<double> QueryRangeList_AsList { get; }

        double RangeMax_AsReal { get; }

        double RangeMin_AsReal { get; }
    }
}
