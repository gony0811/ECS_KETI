using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV.PowerMeter.Library.DeviceModels
{
    public interface IMeter : IIsQuadOrPyro, ISamplePeriod, IImportExport, ISequenceIds
    {
    }
}
