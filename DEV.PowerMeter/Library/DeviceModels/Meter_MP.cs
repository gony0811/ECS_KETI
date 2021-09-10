
using SharedLibrary;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DEV.PowerMeter.Library.DeviceModels
{
    public class Meter_MP : Meter, ILegacyMeterless, ISlowEnergyMode
    {
        public const DataFieldFlags DataFieldFlags_Default = DataFieldFlags.Primary | DataFieldFlags.Flags | DataFieldFlags.Sequence;

        public Device_MP Device => (Device_MP)base.Device;

        public Meter_MP()
          : base((Library.Device)new Device_MP())
        {
        }

        public override bool HasSensor => true;

        public override bool HasTriggering => false;

        public override bool SpeedupAllowed => !this.OperatingMode_IsEnergy;

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
            string protocolVersion = this.Device.ProtocolVersion;
            this.gainCompensationFactor = 1.0;
            this.gainCompensation_Enabled = false;
            this.SmoothingEnabled = false;
            this.Speedup_Enabled = false;
            this.WaitForTriggerModeEnabled = false;
            this.ContinuousMode = true;
            this.Capacity = this.DefaultCapacity;
            this.PreTrigger = 0U;
            this.operatingMode = OperatingMode.PowerWatts;
            this.selectedRange = 0.0;
            this.wavelengthCorrection = 0U;
            this.selectedRange = this.Device.SelectedRange_AsReal;
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

        [DataMember]
        public override double TriggerLevel
        {
            get => this.triggerLevel;
            set
            {
            }
        }

        public override PropertyChangedType PendingChanges
        {
            get => PropertyChangedType.None;
            protected set
            {
            }
        }

        public override PropertyChangedType GetAndClearPendingChanges() => PropertyChangedType.None;

        [API(APICategory.Unclassified)]
        public override bool SlowEnergyMode_IsSelected => this.HasSensor && this.OperatingMode_IsEnergy;

        public override void SetAcquisitionMode() => this.SelectedDataFields = (DataFieldFlags)(13 | (this.Sensor_IsQuad ? 2 : 0));

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
