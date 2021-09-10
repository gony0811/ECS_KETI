using SharedLibrary;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace DEV.PowerMeter.Library.DeviceModels
{
    [DataContract]
    [PreviouslyNamed("Meter")]
    public class Meter_SSIM : Meter, IHasBinary
    {
        private bool smoothingEnabled;
        public const DataFieldFlags DataFieldFlags_Default = DataFieldFlags.Primary | DataFieldFlags.Flags;

        public Device_SSIM Device => (Device_SSIM)base.Device;

        public Meter_SSIM()
          : base((Library.Device)new Device_SSIM())
        {
        }

        public override bool SmoothingEnabled
        {
            get => this.smoothingEnabled;
            set => this.Device.EnableSmoothing_AsBool = this.smoothingEnabled = value;
        }

        [API(APICategory.Unclassified)]
        public override bool SmoothingAllowed => (this.Sensor_IsPmPro && this.OperatingMode_IsPower || !this.Sensor_IsPyro) && !this.HighSpeedChannel_IsSelected;

        public override bool HasTriggering => this.HighSpeedChannel_IsSelected;

        public override void ZeroMeter()
        {
            base.ZeroMeter();
            this.DetectAutomaticChanges();
        }

        public override void WaitZeroMeter()
        {
            this.Device.ConfigureZero_Wait();
            this.DetectAutomaticChanges();
        }

        public override double QueryZeroBaseline() => this.Device.QueryZeroBaseline_AsReal();

        public override void SetAcquisitionMode()
        {
            this.SelectedDataEncoding = DataEncoding.Binary;
            this.SelectedDataFields = (DataFieldFlags)(5 | (this.OperatingMode_IsTrueEnergy ? 16 : 0) | (this.Sensor_IsQuad ? 2 : 0) | (this.UploadSequenceIDs ? 8 : 0));
            this.Device.PreTriggerDelay_AsUint = this.PreTrigger;
            this.Device.EnableSnapshotMode_AsBool = this.SnapshotMode_IsSelected;
            if (!this.OperatingMode_IsEnergy || !this.Sensor_IsPmPro)
                return;
            this.Device.MeasurementWindow_AsUint = this.MeasurementWindow;
        }

        [Conditional("INJECT_START_EXCEPTION")]
        private void InjectException()
        {
            bool? isChecked = SenseSwitchLocator.Current["1"].IsChecked;
            bool flag = true;
            if (isChecked.GetValueOrDefault() == flag & isChecked.HasValue)
                throw new TimeoutException();
        }

        public override void SetPollingMode()
        {
            this.Device.EnableSnapshotMode_AsBool = false;
            this.SelectedDataEncoding = DataEncoding.Ascii;
            this.SelectedDataFields = (DataFieldFlags)(5 | (this.OperatingMode_IsTrueEnergy ? 16 : 0) | (this.Sensor_IsQuad ? 2 : 0) | (this.UploadSequenceIDs ? 8 : 0));
        }

        public bool RetainBinary
        {
            get => DataRecordBase.RetainBinary;
            set => DataRecordBase.RetainBinary = value;
        }

        public override bool UploadSequenceIDs { get; set; }

        public override bool CanChangeUploadSequenceIDs => true;
    }
}
