using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV.PowerMeter.Library
{
    public interface IImportExport : IIsQuadOrPyro
    {
        string SerialNumber { get; }

        Sensor SelectedSensor { get; }

        uint Capacity { get; }
    }
}
