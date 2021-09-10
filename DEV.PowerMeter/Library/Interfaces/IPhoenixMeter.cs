using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV.PowerMeter.Library.Interfaces
{
    public interface IPhoenixMeter
    {
        bool IsOpen { get; }

        bool IsRunning { get; }

        Statistics Statistics { get; }

        Computations Computations { get; }

        Meter Meter { get; }

        CaptureBuffer CaptureBuffer { get; }

        DataLogger DataLogger { get; }

        bool Polling_Enabled { get; set; }

        bool Updates_Enabled { get; set; }

        bool Continuous { get; set; }

        OperatingMode OperatingMode { get; set; }

        PreviewBufferController PreviewBufferController { get; }

        void Close();

        void Open(string portName);

        void Start();

        void PrepareToStart();

        event Action<PollingData> MeasurementUpdate;

        event Library.MeterStatusChanged MeterStatusChanged;

        event Library.DAQ_StateChanged DAQ_StateChanged;

        void Stop();

        void InitializeBuffers();

        bool HasSensor { get; }
    }
}
