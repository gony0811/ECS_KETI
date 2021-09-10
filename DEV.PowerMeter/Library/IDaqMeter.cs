using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV.PowerMeter.Library
{
    public interface IDaqMeter
    {
        OperatingMode OperatingMode { get; }

        uint PreTrigger { get; }

        bool OperatingMode_IsTrueEnergy { get; }

        bool QuadMode_IsSelected { get; }

        uint Capacity { get; }

        uint SnapshotMaxCapacity { get; }

        bool ContinuousMode { get; }

        DataFieldFlags SelectedDataFields { get; }

        bool SnapshotMode_IsSelected { get; }

        long SamplePeriod { get; }

        bool Sensor_IsPyro { get; }

        bool Sensor_IsQuad { get; }

        void SetPollingMode();
    }
}
