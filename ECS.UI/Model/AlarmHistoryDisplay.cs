using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.UI.Model
{
    public class AlarmHistoryDisplay
    {
        public string ID { get; set; }
        public string LEVEL { get; set; }
        public string ALARM_TEXT { get; set; }
        public string STATUS { get; set; }
        public string ENABLE { get; set; }
        public string DESCRIPTION { get; set; }
        public DateTime UPDATETIME { get; set; } 
    }
}
