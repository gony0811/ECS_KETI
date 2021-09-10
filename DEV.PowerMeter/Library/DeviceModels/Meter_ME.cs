
using System.Collections.Generic;

namespace DEV.PowerMeter.Library.DeviceModels
{
    public class Meter_ME : Meter, ILegacyMeterless
    {
        public const DataFieldFlags DataFieldFlags_Default = DataFieldFlags.Primary | DataFieldFlags.Flags | DataFieldFlags.Sequence | DataFieldFlags.Period;

        public Device_ME Device => (Device_ME)base.Device;

        public Meter_ME()
          : base((Library.Device)new Device_ME())
        {
        }

        public override bool HasSensor => true;

        public override bool HasTriggering => true;

        public override bool SpeedupAllowed => false;

        public override bool AnalogOutputAllowed => false;

        public override bool SmoothingAllowed => false;

        public override void ReloadProperties()
        {
            this.ReloadMeterProperties();
            this.ReloadSensorProperties();
        }

        protected override void ReloadMeterProperties()
        {
            this.SetPollingMode();
            this.Identification = this.Device.Identification;
            this.Device.LoadSystemType();
            this.UpdateSystemStatusAndFaults();
            this.SerialNumber = this.Device.SerialNumber;
            this.PartNumber = this.Device.PartNumber;
            this.ModelName = this.Device.ModelName;
            this.CalibrationDate = this.Device.CalibrationDate;
            this.ManufactureDate = this.Device.ManufactureDate;
            this.MeterType = this.Device.MeterType;
            this.MeterHasPyro = false;
            this.FirmwareVersion = this.Device.FirmwareVersion;
            this.gainCompensationFactor = 1.0;
            this.gainCompensation_Enabled = false;
            this.decimationRate = 1U;
            this.decimation_Enabled = false;
            this.WaitForTriggerModeEnabled = false;
            this.ContinuousMode = true;
            this.Capacity = this.DefaultCapacity;
            this.PreTrigger = 0U;
            this.operatingMode = OperatingMode.PowerWatts;
            this.selectedRange = 0.0;
            this.wavelengthCorrection = 0U;
        }

        public override void ReloadSensorProperties()
        {
            this.SelectedSensor.Reload((Library.Device)this.Device);
            this.operatingMode = this.Device.OperatingMode_AsEnum;
            this.ReloadSelectedChannelAndRange();
            List<uint> wavelengthTableAsList = this.Device.WavelengthTable_AsList;
            if (wavelengthTableAsList == null)
                this.ReportError("Meter: Empty WavelengthTable from device");
            this.WavelengthsTable.LoadOriginals((IEnumerable<uint>)wavelengthTableAsList);
            this.wavelengthCorrection = this.Device.WavelengthCorrectionValue_AsUint;
            this.TriggerLevel_LPEM = TriggerLevel_LPEM.Low;
            this.ReloadSensorHasTemperature();
            this.SelectedSensor.IsInitialized = true;
        }

        public override void ZeroMeter() => this.Device.ConfigureZero();

        public override void WaitZeroMeter()
        {
        }

        public override double QueryZeroBaseline() => 0.0;

        public override PropertyChangedType PendingChanges
        {
            get => PropertyChangedType.None;
            protected set
            {
            }
        }

        public override PropertyChangedType GetAndClearPendingChanges() => PropertyChangedType.None;

        public override void SetAcquisitionMode() => this.SelectedDataFields = DataFieldFlags.Primary | DataFieldFlags.Flags | DataFieldFlags.Sequence | DataFieldFlags.Period;

        public override void SetPollingMode() => this.SetAcquisitionMode();

        public override bool UploadSequenceIDs
        {
            get => true;
            set
            {
            }
        }

        public override bool CanChangeUploadSequenceIDs => false;

        public override void Start()
        {
            this.Device.ClearSequenceId();
            base.Start();
        }
    }
}
