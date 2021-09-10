using System;

namespace DEV.PowerMeter.Library.AlarmsAndLimits
{
    [Serializable]
    public class UserSetLimits
    {
        public double Min;
        public double Max;
        public bool PassFailDetectionEnabled;
        public int ScaleIndex;
    }
}
