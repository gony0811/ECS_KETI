using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INNO6.Core.Manager.Model
{
    public class VALUE_INTERLOCK : INTERLOCK
    {
        public string LowValue { get; set; }
        public string HighValue { get; set; }
    }
}
