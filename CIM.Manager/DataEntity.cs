using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core;
using INNO6.IO;

namespace CIM.Manager
{


    public class ALARM
    {
        public enum eALCD
        {
            Unknown = 0,
            Light = 1,
            Serious = 2
        }

        public enum eALST
        {
            Unknown = 0,
            SET = 1,
            RESET = 2
        }

        public enum eALED
        {
            Enable = 0,
            Disable = 1,
        }

        public string ID { get; set; }
        public string INDEX { get; set; }
        public string MODULE { get; set; }
        public eALCD LEVEL { get; set; }
        public string TEXT { get; set; }
        public eALST STATUS { get; set; }
        public eALED ENABLE { get; set; }
        public string DESCRIPTION { get; set; }
    }

    public class ALARM_HISTORY : ALARM
    {
        DateTime UPDATETIME { get; set; }
    }

    public class EC
    {
        public string INDEX { get; set; }
        public string ECID { get; set; }
        public string ECNAME { get; set; }
        public string MODULE { get; set; }
        public string UNIT { get; set; }
        public string DESCRIPTION { get; set; }
    }

    public class ECValue
    {
        public string ECDEF { get; set; }
        public string ECSUL { get; set; }
        public string ECSLL { get; set; }
        public string ECWUL { get; set; }
        public string ECWLL { get; set; }
    }

    public class SV
    {
        public string INDEX { get; set; }
        public string SVID { get; set; }
        public string SVNAME { get; set; }
        public string MODULE { get; set; }
        public string MIN { get; set; }
        public string MAX { get; set; }
        public string UNIT { get; set; }
        public string DESCRIPTION { get; set; }
    }

    public class TRACE
    {
        public string TRID { get; set; }
        public List<SV> SVs { get; set; }
        //miliseconds
        public double SamplingPeriod { get; set; }
        public int TotalSamplingCount { get; set; }
        public int SamplingCount { get; set; }
        public DateTime LastReportTime { get; set; }

        public TRACE()
        {
            LastReportTime = DateTime.Now;
            SamplingCount = 0;
            SVs = new List<SV>();
        }
    }

    public class RECIPE
    {
        public string INDEX { get;set; }
        public string PPID { get; set; }
        public string MODULE { get; set; }
        public string PARAMETERNAME { get; set; }
        public string PARAMETERVALUE { get; set; }
        public string DESCRIPTION { get; set; }
        public DateTime UPDATETIME { get; set; }
    }

    public class RECIPEPARAMETER
    {
        public string INDEX { get; set; }
        public string PARAMETERNAME { get; set; }
        public string MODULE { get; set; }
        public string UNIT { get; set; }
        public string MIN { get; set; }
        public string MAX { get; set; }
        public bool APC { get; set; }
        public string DESCRIPTION { get; set; }
    }

    public class DV
    {
        public string INDEX { get; set; }
        public string DVNAME { get; set; }
        public string MODULE { get; set; }
        public string PORTNAME { get; set; }
        public string UNIT { get; set; }
        public string DESCRIPTION { get; set; }
    }
    public class Attribute
    {
        public string INDEX { get; set; }
        public string ID { get; set; }
        public string ATTRIBUTENAME { get; set; }
        public string MODULE { get; set; }
        public string UNIT { get; set; }
        public string FORMAT { get; set; }
        public string DESCRIPTION { get; set; }
    }
}
