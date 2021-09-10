using DEV.PowerMeter.Library.AlarmsAndLimits;
using MvvmFoundation.Wpf;


namespace DEV.PowerMeter.Library
{
    public class NonvolatileUserSettings : ObservableObject
    {
        private static NonvolatileUserSettings _Instance;
        public AlarmsAndLimitsSettings _AppsAlarmsAndLimitsSettings;

        private static NonvolatileUserSettings Instance => NonvolatileUserSettings._Instance ?? (NonvolatileUserSettings._Instance = new NonvolatileUserSettings());

        private NonvolatileUserSettings()
        {
        }

        public static NonvolatileUserSettings getInstance() => NonvolatileUserSettings.Instance;

        public AlarmsAndLimitsSettings AlarmsAndLimitsSettings
        {
            set
            {
                this._AppsAlarmsAndLimitsSettings = AlarmsAndLimitsSettings.DeepCopyAlarmsAndLimitsSettings(value);
                this.RaiseSettingsChanged();
            }
            get => this._AppsAlarmsAndLimitsSettings;
        }

        public void RaiseSettingsChanged() => this.RaisePropertyChanged("AlarmsAndLimitsSettings");
    }
}
