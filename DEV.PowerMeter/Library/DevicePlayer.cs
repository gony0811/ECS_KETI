using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV.PowerMeter.Library
{
    public class DevicePlayer : IDaqDevice, IDecodeMeasurement
    {
        public Communicator Communicator { get; protected set; }

        public bool IsOpen => this.Communicator.IsOpen;

        public Channel Channel => this.Communicator?.Channel;

        public string PortName => this.Communicator?.Channel.PortName;

        public Device BaseDevice { get; set; }

        public DataRecordSingle DecodeMeasurement(string line) => this.BaseDevice.DecodeMeasurement(line);

        public DevicePlayer(Meter meter, Channel channel)
        {
            this.BaseDevice = meter.Device;
            this.Open(channel);
        }

        public void Open(Channel channel)
        {
            this.BaseDevice.OpenRaw(channel);
            this.Communicator = this.BaseDevice.Communicator;
        }

        public void Close() => this.Communicator.Close();

        public void Start(uint count = 0)
        {
        }

        public void Stop()
        {
        }

        public void ForceTrigger()
        {
        }
    }
}
