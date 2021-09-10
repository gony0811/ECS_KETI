using MvvmFoundation.Wpf;
using System;
using System.Linq.Expressions;

namespace DEV.PowerMeter.Library.AlarmsAndLimits
{
    public class AlarmsAndLimits
    {
        private readonly PropertyObserver<NonvolatileUserSettings> _NonvolatileUserSettingsObserver;
        private AlarmsAndLimitsSettings settings;
        protected PhoenixMeter mPhoenixMeter;
        private DateTime LastUI_FaultUpdate;

        public event Library.AlarmsAndLimits.AlarmsAndLimits.FaultDetected OnFaultDetected;

        public event EventHandler OnClearAlarms;

        public event Library.AlarmsAndLimits.AlarmsAndLimits.AlarmsAndLimitAnalysisChanged OnAlarmsAndLimitAnalysisChanged;

        protected CaptureBuffer mCaptureBuffer => this.mPhoenixMeter?.CaptureBuffer;

        public AlarmsAndLimits(NonvolatileUserSettings userSettings, PhoenixMeter PhoenixMeter)
        {
            this._NonvolatileUserSettingsObserver = new PropertyObserver<NonvolatileUserSettings>(userSettings);
            this._NonvolatileUserSettingsObserver.RegisterHandler((Expression<Func<NonvolatileUserSettings, object>>)(nvus => nvus.AlarmsAndLimitsSettings), new Action<NonvolatileUserSettings>(this.SettingsChanged));
            this.mPhoenixMeter = PhoenixMeter;
            this.mPhoenixMeter.MeterStatusChanged += new MeterStatusChanged(this.PhoenixMeter_MeterStatusChanged);
            this.OnFaultDetected += new Library.AlarmsAndLimits.AlarmsAndLimits.FaultDetected(this.AlarmsAndLimits_OnFaultDetected);
        }

        private void SettingsChanged(NonvolatileUserSettings nvus)
        {
            this.settings = nvus.AlarmsAndLimitsSettings;
            this.Clear();
        }

        private void AlarmsAndLimits_OnFaultDetected(
          StatisticsEnum statEnum,
          AlarmsEnum selectedAlarms,
          CauseOfFault causeOfFault)
        {
            if ((selectedAlarms & AlarmsEnum.StopAcquisition) != AlarmsEnum.StopAcquisition)
                return;
            this.mPhoenixMeter.Stop();
        }

        private void PhoenixMeter_MeterStatusChanged(object sender, MeterStatus state)
        {
            switch (state)
            {
                case MeterStatus.SensorConnected:
                    NonvolatileUserSettings instance = NonvolatileUserSettings.getInstance();
                    if (instance == null)
                        break;
                    AlarmsAndLimitsSettings andLimitsSettings = instance.AlarmsAndLimitsSettings;
                    if (andLimitsSettings == null || andLimitsSettings.SensorType != SensorType.None && andLimitsSettings.SensorType.Equals((object)this.mPhoenixMeter.SelectedSensor.SensorType))
                        break;
                    instance.AlarmsAndLimitsSettings = (AlarmsAndLimitsSettings)null;
                    break;
                case MeterStatus.MeterStarted:
                    if (this.settings == null || !this.settings.AlarmsAndLimitsActive)
                        break;
                    this.mCaptureBuffer.DataAdded += new DataAdded(this.CaptureBuffer_DataAdded);
                    this.Clear();
                    break;
                case MeterStatus.MeterStopped:
                    if (this.settings == null || !this.settings.AlarmsAndLimitsActive)
                        break;
                    this.mCaptureBuffer.DataAdded -= new DataAdded(this.CaptureBuffer_DataAdded);
                    Library.AlarmsAndLimits.AlarmsAndLimits.AlarmsAndLimitAnalysisChanged limitAnalysisChanged = this.OnAlarmsAndLimitAnalysisChanged;
                    if (limitAnalysisChanged == null)
                        break;
                    limitAnalysisChanged(this);
                    break;
            }
        }

        private void CaptureBuffer_DataAdded(IDataRecordSingle data) => this.RunFaultDetectionTest(data);

        private void RunFaultDetectionTest(IDataRecordSingle data)
        {
            ++this.DataSampleCount;
            foreach (StatisticsEnum statEnum in Enum.GetValues(typeof(StatisticsEnum)))
            {
                if (this.settings.PassFailLimits[(int)statEnum].PassFailDetectionEnabled)
                {
                    CauseOfFault cause = CauseOfFault.NA;
                    if (this.RunTest(statEnum, data, out cause))
                    {
                        data.Flags |= MeasurementFlags.AlarmDetected;
                        ++this.TotalFaultCount;
                        DateTime now;
                        if (this.TotalFaultCount != 1)
                        {
                            now = DateTime.Now;
                            if (now.CompareTo(this.LastUI_FaultUpdate) <= 0)
                                continue;
                        }
                        Library.AlarmsAndLimits.AlarmsAndLimits.FaultDetected onFaultDetected = this.OnFaultDetected;
                        if (onFaultDetected != null)
                            onFaultDetected(statEnum, this.settings.SelectedAlarms, cause);
                        Library.AlarmsAndLimits.AlarmsAndLimits.AlarmsAndLimitAnalysisChanged limitAnalysisChanged = this.OnAlarmsAndLimitAnalysisChanged;
                        if (limitAnalysisChanged != null)
                            limitAnalysisChanged(this);
                        now = DateTime.Now;
                        this.LastUI_FaultUpdate = now.AddMilliseconds(3000.0);
                    }
                }
            }
        }

        public bool RunTest(StatisticsEnum statEnum, IDataRecordSingle data, out CauseOfFault cause)
        {
            bool flag = false;
            cause = CauseOfFault.NA;
            if (this.mPhoenixMeter.Meter.LPEM_Mode_IsSelected && (data.Flags & MeasurementFlags.FinalEnergyRecord) == (MeasurementFlags)0)
                return false;
            double num = Math.Max(0.0, data.Measurement);
            if (statEnum == StatisticsEnum.Sample)
            {
                if (num < this.settings.PassFailLimits[(int)statEnum].Min)
                {
                    flag = true;
                    cause = CauseOfFault.Min_detected;
                }
                else if (num > this.settings.PassFailLimits[(int)statEnum].Max)
                {
                    flag = true;
                    cause = CauseOfFault.Max_detected;
                }
            }
            return flag;
        }

        public void Clear()
        {
            this.DataSampleCount = 0;
            this.TotalFaultCount = 0;
            EventHandler onClearAlarms = this.OnClearAlarms;
            if (onClearAlarms == null)
                return;
            onClearAlarms((object)this, (EventArgs)null);
        }

        public int DataSampleCount { protected set; get; }

        public int TotalFaultCount { protected set; get; }

        public delegate void FaultDetected(
          StatisticsEnum statEnum,
          AlarmsEnum selectedAlarms,
          CauseOfFault causeOfFault);

        public delegate void AlarmsAndLimitAnalysisChanged(Library.AlarmsAndLimits.AlarmsAndLimits alarmsAndLimitsAnalysis);
    }
}
