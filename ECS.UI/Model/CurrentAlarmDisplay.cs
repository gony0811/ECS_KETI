using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.UI.Model
{
    public class CurrentAlarmDisplay
    {
        public string ID { get; set; }
        public string NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string LEVEL { get; set; }
        public DateTime SET_TIME { get; set; }
    }
}
