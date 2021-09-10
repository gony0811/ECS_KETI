using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV.PowerMeter.Library.DeviceModels
{
    public interface IAutoRange : IRangeSelection
    {
        bool EnableAutoRange_AsBool { get; set; }
    }
}
