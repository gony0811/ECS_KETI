using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INNO6.IO
{
    public class DeviceInfo
    {
        public string DeviceName { get; set; }
        public string InstanceName { get; set; }
        public string FileName { get; set; }
        public bool Use { get; set; }
        public List<string> Arguments = new List<string>();
        public string Description { get; set; }

        public DeviceHandler DeviceHandler { get; set; }
        public DevicePollingBase DevicePolling { get; set; }
    }
}
