using System;
using System.Collections.Generic;
using System.Linq;

namespace DEV.PowerMeter.Library.AlarmsAndLimits
{
    [Serializable]
    public class AlarmsAndLimitsSettings
    {
        public List<UserSetLimits> PassFailLimits = new List<UserSetLimits>();

        public bool AlarmsAndLimitsActive => this.PassFailLimits != null && this.PassFailLimits.Any<UserSetLimits>((Func<UserSetLimits, bool>)(pfl => pfl.PassFailDetectionEnabled));

        public AlarmsEnum SelectedAlarms
        {
            get
            {
                AlarmsEnum alarmsEnum = AlarmsEnum.noAlarm;
                if (this.StopAcquisition_IsSelected)
                    alarmsEnum |= AlarmsEnum.StopAcquisition;
                if (this.Beep_IsSelected)
                    alarmsEnum |= AlarmsEnum.SoundAlarm;
                if (this.ReverseBackgroundColor)
                    alarmsEnum |= AlarmsEnum.ReverseBackgroundColor;
                return alarmsEnum;
            }
        }

        public bool StopAcquisition_IsSelected { set; get; }

        public bool Beep_IsSelected { set; get; }

        public bool ReverseBackgroundColor { set; get; }

        public double SensorMaxRange { set; get; }

        public SensorType SensorType { set; get; }

        public static AlarmsAndLimitsSettings DeepCopyAlarmsAndLimitsSettings(
          AlarmsAndLimitsSettings original)
        {
            if (original == null)
            {
                original = new AlarmsAndLimitsSettings();
                original.Beep_IsSelected = true;
                original.ReverseBackgroundColor = true;
                original.StopAcquisition_IsSelected = true;
                original.SensorMaxRange = 900.0;
            }
            AlarmsAndLimitsSettings andLimitsSettings = new AlarmsAndLimitsSettings();
            andLimitsSettings.ReverseBackgroundColor = original.ReverseBackgroundColor;
            andLimitsSettings.Beep_IsSelected = original.Beep_IsSelected;
            andLimitsSettings.StopAcquisition_IsSelected = original.StopAcquisition_IsSelected;
            andLimitsSettings.SensorMaxRange = original.SensorMaxRange;
            andLimitsSettings.SensorType = original.SensorType;
            andLimitsSettings.PassFailLimits = new List<UserSetLimits>();
            foreach (StatisticsEnum statisticsEnum in Enum.GetValues(typeof(StatisticsEnum)))
            {
                if ((StatisticsEnum)original.PassFailLimits.Count <= statisticsEnum)
                    original.PassFailLimits.Add(new UserSetLimits()
                    {
                        Min = 0.0,
                        Max = 0.1,
                        ScaleIndex = 2
                    });
                andLimitsSettings.PassFailLimits.Add(original.PassFailLimits[(int)statisticsEnum]);
            }
            return andLimitsSettings;
        }

        public void DisableAllAlarms()
        {
            List<UserSetLimits> userSetLimitsList = new List<UserSetLimits>();
            foreach (UserSetLimits passFailLimit in this.PassFailLimits)
            {
                if (passFailLimit.PassFailDetectionEnabled)
                    userSetLimitsList.Add(new UserSetLimits()
                    {
                        Min = passFailLimit.Min,
                        Max = passFailLimit.Max
                    });
                else
                    userSetLimitsList.Add(passFailLimit);
            }
            this.PassFailLimits = userSetLimitsList;
        }
    }
}
