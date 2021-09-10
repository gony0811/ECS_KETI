using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV.PowerMeter.Library
{
    public class SensorTypeAndQualifier
    {
        public SensorType SensorType;
        public SensorTypeQualifier SensorTypeQualifier;

        public SensorTypeAndQualifier(string value, char Separator)
        {
            this.SensorTypeQualifier = SensorTypeQualifier.None;
            this.SensorType = SensorType.None;
            string[] strArray = value.Split(Separator);
            switch (strArray.Length)
            {
                case 1:
                    this.SensorType = SCPI.SensorTypeConverter.FromString(strArray[0].Trim(), SensorType.None);
                    break;
                case 2:
                    this.SensorTypeQualifier = SCPI.SensorTypeQualifierConverter.FromString(strArray[1].Trim(), SensorTypeQualifier.None);
                    goto case 1;
            }
        }
    }
}
