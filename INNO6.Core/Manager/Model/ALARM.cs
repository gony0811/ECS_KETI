using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INNO6.Core.Manager.Model
{
    public enum eALCD
    {
        Unknown = 0,
        Light = 1,
        Heavy = 2
    }

    public enum eALST
    {
        Unknown = 0,
        SET = 1,
        RESET = 2
    }

    public enum eALED
    {
        Disable = 0,
        Enable = 1     
    }

    public class ALARM
    {
        public string ID { get; set; }
        public eALCD LEVEL { get; set; }
        public string TEXT { get; set; }
        public eALST STATUS { get; set; }
        public eALED ENABLE { get; set; }
        public string DESCRIPTION { get; set; }

        public DateTime SETTIME { get; set; }
    }
}
