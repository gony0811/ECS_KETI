using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INNO6.Core
{

    public abstract class SECSDriverBase
    {
        public string IpAddress { get; protected set; }
        public int Port { get; protected set; }

        public bool Active { get; protected set; }
        public bool IsConnected { get; protected set; }
        public string FileteringRegularExp { get; set; }
        public bool UseFilteringAsciiValue { get; set; }
        public string SecsConfigFilePath { get; set; }
        public string DeviceID { get; set; }
        public abstract void Initialize(string secsConfigFilePath);

        public abstract void Start();

        public abstract void Stop();

        public abstract void Send(object message);

        public abstract void WriteLogAndSendMessage(object message, object arg);
    }
}
