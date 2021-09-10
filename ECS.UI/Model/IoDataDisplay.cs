using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.UI.Model
{
    public class IoDataDisplay
    {
        public string Name { get; set; }

        public string DeviceModule { get; set; }

        public string DriverName { get; set; }

        public string Direction { get; set; }

        public string Type { get; set; }

        public string DefaultValue { get; set; }

        public string Use { get; set; }
        public int PollingTime { get; set; }

        public int DataResetTimeout { get; set; }

        public string Min { get; set; }
        public string Max { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }
    }
}
