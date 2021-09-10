using DEV.PowerMeter.Library.AlarmsAndLimits;
using DEV.PowerMeter.Library.DeviceModels;
using MvvmFoundation.Wpf;
using RuntimeLibrary;
using SharedLibrary;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;
using UpdaterLibrary;
using ViewModels;

namespace DEV.PowerMeter.Library.ViewModels
{
    [DataContract]
    public class MainViewModel : ViewModelBase
    {
        private static UpdaterConfig UpdaterConfig = new UpdaterConfig()
        {
            AppMainWindowTitle = "Coherent Meter Connection Updater",
            ContainerName = "coherentmeterconnection",
            CurentVersion = new AssemblyProperties().VersionMoniker,
            AutoCheck = UpdaterConfig.AutoCheckMode.AlwaysOn
        };
        private bool isOpen;
        private bool isRunning;
        private ICommand startCommand;
        private ICommand stopCommand;
        private bool hasSensor;
        private bool sensorIsQuad;
        private bool sensorIsPyro;
        private bool bufferNonEmpty;
        private bool bufferIsPower;
        private bool bufferIsQuad;
        private bool forceSensorOrBufferIsQuad;
        private bool bufferIsPyro;
        private bool bufferCapacityIsEnabled;
        private bool removeCurrentWavelength_MenuItem_IsEnabled;
        private WavelengthOptions wavelengthOptions;
        private WavelengthOption selectedWavelengthOption;
        private bool highSpeedMode_Checkbox_IsEnabled;
        private bool highSpeedMode_Checkbox_IsChecked;
        private bool dataLogging_IsChecked;
        private bool snapshotMode_IsAllowed;
        private bool snapshotMode_IsSelected;
        private bool waitTriggerMode_IsAllowed;
        private bool waitTriggerMode_IsSelected;
        private bool triggerSettings_IsEnabled;
        private bool triggerLevelSettings_IsEnabled = true;
        private bool externalTriggerSettings_IsEnabled;
        private bool lpem_Group_IsEnabled;
        private bool pulseGroup_IsEnabled;
        private ICommand launchLicenseManagerCommand;
        private bool universalAccessAllowed;
        private bool suppressFreeTrialMessages;
        private string salesDemoOptionsText;
        private IPulseAnalysisSettingsService PulseAnalysisSettingsWindow;
        private bool pulseAnalysisSettingsWindow_IsVisible;
        public bool pulseAnalysisFeature_IsSupported;
        private bool showCursors_IsChecked;
        private bool selectionCursors_AreActive;
        private bool showTrackerCursors_IsChecked;
        private bool showTriggerMarkers_IsChecked;
        private bool showTriggerMarkers_IsEnabled = true;
        private bool _HighlightSamplePoints_IsChecked;
        private bool _HighlightSamplePoints_IsEnabled = true;
        private bool snapToTriggers_IsChecked;
        private bool triggerLevelFormatScientific_IsChecked;
        private bool triggerLevelFormatFixed_IsChecked;
        private bool triggerPercentFormatScientific_IsChecked;
        private bool triggerPercentFormatFixed_IsChecked;
        private bool triggerLevelFormatFixed_IsEnabled = true;
        private bool triggerPercentFormatFixed_IsEnabled = true;
        private bool _ContinuousUpdate_IsChecked;
        private bool _MeterPolling_IsChecked;
        private bool _ContinuousUpdate_IsEnabled = true;
        private bool _MeterPolling_IsEnabled = true;
        private bool _RetainBinaryData_IsChecked;
        private bool _RetainBinaryData_IsEnabled;
        private bool _UploadSeqIds_IsChecked;
        private bool _UploadSeqIds_IsEnabled;
        private bool zoomNone_IsChecked;
        private bool zoomHorizontal_IsChecked = true;
        private bool zoomVertical_IsChecked;
        private bool zoomBoth_IsChecked;
        private double _tuningPanelDialWidth = 260.0;
        private double _tuningPanelDialHeight = 260.0;

        private PhoenixMeter PhoenixMeter => ServiceLocator.Resolve<PhoenixMeter>();

        private Computations Computations => this.PhoenixMeter?.Computations;

        private PulseAnalysisOptions PulseAnalysisOptions => this.Computations?.PulseAnalysisOptions;

        private Meter Meter => this.PhoenixMeter?.Meter;

        public UpdaterViewModel UpdaterVM { get; protected set; } = new UpdaterViewModel(MainViewModel.UpdaterConfig);

        public void AttachCaptureBuffer(CaptureBuffer buffer)
        {
            this.BufferNonEmpty = buffer != null && buffer.Count > 0U;
            this.BufferIsPyro = this.BufferNonEmpty && buffer.Sensor_IsPyro;
            this.BufferIsQuad = this.BufferNonEmpty && buffer.Sensor_IsQuad;
            this.BufferIsPower = this.BufferNonEmpty && buffer.Units.IsPower;
        }

        public void Update(Meter meter)
        {
            this.IsOpen = meter.IsOpen;
            this.HasSensor = meter.HasSensor;
            this.IsRunning = meter.IsRunning;
            this.PulseAnalysisFeature_IsSupported = !(this.Meter is Meter_ME);
            this.SnapshotMode_IsSelected = meter.SnapshotModeEnabled;
            this.SensorIsQuad = meter.Sensor_IsQuad;
            this.SensorIsPyro = meter.Sensor_IsPyro;
            this.ContinuousUpdate_IsChecked = this.PhoenixMeter.Updates_Enabled;
            this.MeterPolling_IsChecked = this.PhoenixMeter.Polling_Enabled;
            if (this.Meter is IHasBinary meter1)
            {
                this.RetainBinaryData_IsEnabled = true;
                this.RetainBinaryData_IsChecked = meter1.RetainBinary;
            }
            else
            {
                this.RetainBinaryData_IsEnabled = false;
                this.RetainBinaryData_IsChecked = false;
            }
            ISequenceIds meter2;
            if ((Meter)(meter2 = (ISequenceIds)this.Meter) != null)
            {
                this.UploadSeqIds_IsEnabled = meter2.CanChangeUploadSequenceIDs;
                this.UploadSeqIds_IsChecked = meter2.UploadSequenceIDs;
            }
            else
            {
                this.UploadSeqIds_IsEnabled = false;
                this.UploadSeqIds_IsChecked = false;
            }
            ViewModelBase.Trace(string.Format("MVM.UpdateMeter RetainBinary: {0}/{1}, ", (object)this.RetainBinaryData_IsChecked, (object)this.RetainBinaryData_IsEnabled) + string.Format("UploadSID {0}/{1}", (object)this.UploadSeqIds_IsChecked, (object)this.UploadSeqIds_IsEnabled));
        }

        public void UpdateEnablement()
        {
            this.BufferCapacityIsEnabled = this.HasSensorNotRunning;
            this.LPEM_Group_IsEnabled = this.HasSensorNotRunning && this.LPEM_Group_IsSelected;
            this.PulseGroup_IsEnabled = this.HasSensorNotRunning && this.Meter.HighSpeedChannel_IsSelected && this.Meter.OperatingMode_IsEnergy && this.Meter.Sensor_IsPmPro;
            this.SnapshotMode_IsAllowed = this.HasSensorNotRunning && this.Meter.Sensor_IsPmPro;
            bool flag1 = this.Meter.HasTriggering && this.HasSensor;
            bool flag2 = flag1 && this.IsNotRunning;
            this.TriggerLevelSettings_IsEnabled = flag1 && !this.Meter.TriggerSource_IsExternal;
            this.TriggerSettings_IsEnabled = flag2;
            this.ExternalTriggerSettings_IsEnabled = flag2 && this.Meter.TriggerSource_IsExternal;
        }

        public bool LPEM_Group_IsSelected => this.Meter.LPEM_Mode_IsSelected || this.Meter.SlowEnergyMode_IsSelected;

        public Visibility VisibleOnlyIfDebug => Visibility.Collapsed;

        public bool IsOpen
        {
            get => this.isOpen;
            set
            {
                this.OnPropertyChanged<bool>(ref this.isOpen, value, nameof(IsOpen));
                this.OnPropertyChanged("HasSensorNotRunning", (object)this.HasSensorNotRunning);
            }
        }

        public bool IsRunning
        {
            get => this.isRunning;
            set
            {
                if (this.isRunning != value)
                    ViewModelBase.Trace(string.Format("VM.IsRunning: {0} -> {1}", (object)this.isRunning, (object)value));
                if (this.OnPropertyChanged<bool>(ref this.isRunning, value, nameof(IsRunning)))
                {
                    this.UpdateEnablement();
                    this.OnPropertyChanged("SaveScreenShots_IsEnabled", (object)this.SaveScreenShots_IsEnabled);
                    this.OnPropertyChanged("SavingMenuItems_IsEnabled", (object)this.SavingMenuItems_IsEnabled);
                    this.OnPropertyChanged("ShowCursors_IsEnabled", (object)this.ShowCursors_IsEnabled);
                }
                this.OnPropertyChanged("IsNotRunning", (object)this.IsNotRunning);
                this.OnPropertyChanged("HasSensorNotRunning", (object)this.HasSensorNotRunning);
            }
        }

        public bool IsNotRunning => !this.isRunning;

        public ICommand StartCommand => this.startCommand ?? (this.startCommand = (ICommand)new RelayCommand(new Action(this.Start), (Func<bool>)(() => this.CanStart())));

        public bool CanStart() => false;

        public void Start()
        {
        }

        public ICommand StopCommand => this.stopCommand ?? (this.stopCommand = (ICommand)new RelayCommand(new Action(this.Stop), (Func<bool>)(() => this.CanStop())));

        public void Stop()
        {
        }

        public bool CanStop() => false;

        public bool HasSensorNotRunning => this.IsOpen && this.HasSensor && this.IsNotRunning;

        public bool HasSensor
        {
            get => this.hasSensor;
            set
            {
                if (this.OnPropertyChanged<bool>(ref this.hasSensor, value, nameof(HasSensor)) && !value)
                {
                    this.SensorIsQuad = false;
                    this.SensorIsPyro = false;
                }
                this.OnPropertyChanged("HasSensorNotRunning", (object)value);
            }
        }

        public bool SensorIsQuad
        {
            get => this.sensorIsQuad;
            set
            {
                if (!this.OnPropertyChanged<bool>(ref this.sensorIsQuad, value, nameof(SensorIsQuad)))
                    return;
                this.OnPropertyChanged("SensorOrBufferIsQuad", (object)this.SensorOrBufferIsQuad);
            }
        }

        public bool SensorIsPyro
        {
            get => this.sensorIsPyro;
            set => this.OnPropertyChanged<bool>(ref this.sensorIsPyro, value, nameof(SensorIsPyro));
        }

        public bool BufferNonEmpty
        {
            get => this.bufferNonEmpty;
            set
            {
                if (!this.OnPropertyChanged<bool>(ref this.bufferNonEmpty, value, nameof(BufferNonEmpty)))
                    return;
                this.OnPropertyChanged("SavingMenuItems_IsEnabled", (object)this.SavingMenuItems_IsEnabled);
                this.OnPropertyChanged("ShowCursors_IsEnabled", (object)this.ShowCursors_IsEnabled);
            }
        }

        public bool BufferIsPower
        {
            get => this.bufferIsPower;
            set => this.OnPropertyChanged<bool>(ref this.bufferIsPower, value, nameof(BufferIsPower));
        }

        public bool BufferIsQuad
        {
            get => this.bufferIsQuad;
            set
            {
                if (!this.OnPropertyChanged<bool>(ref this.bufferIsQuad, value, nameof(BufferIsQuad)))
                    return;
                this.OnPropertyChanged("SensorOrBufferIsQuad", (object)this.SensorOrBufferIsQuad);
            }
        }

        public bool ForceSensorOrBufferIsQuad
        {
            get => this.forceSensorOrBufferIsQuad;
            set
            {
                this.OnPropertyChanged<bool>(ref this.forceSensorOrBufferIsQuad, this.ForceSensorOrBufferIsQuad, nameof(ForceSensorOrBufferIsQuad));
                this.OnPropertyChanged("SensorOrBufferIsQuad", (object)this.SensorOrBufferIsQuad);
            }
        }

        public bool SensorOrBufferIsQuad => this.SensorIsQuad || this.BufferIsQuad || this.ForceSensorOrBufferIsQuad;

        public bool BufferIsPyro
        {
            get => this.bufferIsPyro;
            set => this.OnPropertyChanged<bool>(ref this.bufferIsPyro, value, nameof(BufferIsPyro));
        }

        public bool BufferCapacityIsEnabled
        {
            get => this.bufferCapacityIsEnabled;
            set => this.OnPropertyChanged<bool>(ref this.bufferCapacityIsEnabled, value, nameof(BufferCapacityIsEnabled));
        }

        public bool SavingMenuItems_IsEnabled => this.IsNotRunning && this.BufferNonEmpty;

        public bool SaveScreenShots_IsEnabled => !this.isRunning;

        public bool HistogramSettings_IsEnabled => !this.isRunning;

        public bool RemoveCurrentWavelength_MenuItem_IsEnabled
        {
            get => this.removeCurrentWavelength_MenuItem_IsEnabled;
            set => this.OnPropertyChanged<bool>(ref this.removeCurrentWavelength_MenuItem_IsEnabled, value, nameof(RemoveCurrentWavelength_MenuItem_IsEnabled));
        }

        public WavelengthOptions WavelengthOptions
        {
            get => this.wavelengthOptions;
            set => this.OnPropertyChanged<WavelengthOptions>(ref this.wavelengthOptions, value, nameof(WavelengthOptions));
        }

        public WavelengthOption SelectedWavelengthOption
        {
            get => this.selectedWavelengthOption;
            set
            {
                this.selectedWavelengthOption = value;
                this.OnPropertyChanged<WavelengthOption>(ref this.selectedWavelengthOption, value, nameof(SelectedWavelengthOption));
            }
        }

        public bool HighSpeedMode_Checkbox_IsEnabled
        {
            get => this.highSpeedMode_Checkbox_IsEnabled;
            set => this.OnPropertyChanged<bool>(ref this.highSpeedMode_Checkbox_IsEnabled, value, nameof(HighSpeedMode_Checkbox_IsEnabled));
        }

        public bool HighSpeedMode_Checkbox_IsChecked
        {
            get => this.highSpeedMode_Checkbox_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.highSpeedMode_Checkbox_IsChecked, value, nameof(HighSpeedMode_Checkbox_IsChecked));
        }

        public bool DataLogging_IsChecked
        {
            get => this.dataLogging_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.dataLogging_IsChecked, value, nameof(DataLogging_IsChecked));
        }

        public bool SnapshotMode_IsAllowed
        {
            get => this.snapshotMode_IsAllowed;
            set => this.OnPropertyChanged<bool>(ref this.snapshotMode_IsAllowed, value, nameof(SnapshotMode_IsAllowed));
        }

        public bool SnapshotMode_IsSelected
        {
            get => this.snapshotMode_IsSelected;
            set
            {
                if (!this.OnPropertyChanged<bool>(ref this.snapshotMode_IsSelected, value, nameof(SnapshotMode_IsSelected)))
                    return;
                this.OnPropertyChanged("CapacityMaximumText", this.CapacityMaximumText);
            }
        }

        public string CapacityMaximumText => string.Format("{0}{1}", (object)this.PhoenixMeter.Meter.MaximumCapacity, this.snapshotMode_IsSelected ? (object)" [Snapshot mode]" : (object)"");

        public bool WaitTriggerMode_IsAllowed
        {
            get => this.waitTriggerMode_IsAllowed;
            set => this.OnPropertyChanged<bool>(ref this.waitTriggerMode_IsAllowed, value, nameof(WaitTriggerMode_IsAllowed));
        }

        public bool WaitTriggerMode_IsSelected
        {
            get => this.waitTriggerMode_IsSelected;
            set => this.OnPropertyChanged<bool>(ref this.waitTriggerMode_IsSelected, value, nameof(WaitTriggerMode_IsSelected));
        }

        public bool TriggerSettings_IsEnabled
        {
            get => this.triggerSettings_IsEnabled;
            set => this.OnPropertyChanged<bool>(ref this.triggerSettings_IsEnabled, value, nameof(TriggerSettings_IsEnabled));
        }

        public bool TriggerLevelSettings_IsEnabled
        {
            get => this.triggerLevelSettings_IsEnabled;
            set => this.OnPropertyChanged<bool>(ref this.triggerLevelSettings_IsEnabled, value, nameof(TriggerLevelSettings_IsEnabled));
        }

        public bool ExternalTriggerSettings_IsEnabled
        {
            get => this.externalTriggerSettings_IsEnabled;
            set => this.OnPropertyChanged<bool>(ref this.externalTriggerSettings_IsEnabled, value, nameof(ExternalTriggerSettings_IsEnabled));
        }

        public bool LPEM_Group_IsEnabled
        {
            get => this.lpem_Group_IsEnabled;
            set => this.OnPropertyChanged<bool>(ref this.lpem_Group_IsEnabled, value, nameof(LPEM_Group_IsEnabled));
        }

        public bool PulseGroup_IsEnabled
        {
            get => this.pulseGroup_IsEnabled;
            set => this.OnPropertyChanged<bool>(ref this.pulseGroup_IsEnabled, value, nameof(PulseGroup_IsEnabled));
        }

        public ICommand LaunchLicenseManagerCommand => this.launchLicenseManagerCommand ?? (this.launchLicenseManagerCommand = (ICommand)new RelayCommand(new Action(this.LaunchLicenseManager)));

        public void LaunchLicenseManager()
        {
            CertificateManagerMainWindow managerMainWindow = new CertificateManagerMainWindow();
            Window mainWindow = (Window)WindowExtensions.MainWindow;
            try
            {
                mainWindow.IsEnabled = false;
                managerMainWindow.ShowDialog();
                CMC_CLA.Current.GetAuthorization((ILicensedMeter)this.Meter);
            }
            finally
            {
                mainWindow.IsEnabled = true;
            }
            if (!(mainWindow is IUpdateMeterControls updateMeterControls))
                return;
            updateMeterControls.UpdateMeterControls();
        }

        public void UpdateAuthorizations()
        {
            this.UniversalAccessAllowed = CMC_CLA.Current.UniversalAccessAllowed;
            this.DisableUnlicensedFeatures();
        }

        public bool UniversalAccessAllowed
        {
            get => this.universalAccessAllowed;
            set => this.OnPropertyChanged<bool>(ref this.universalAccessAllowed, value, nameof(UniversalAccessAllowed));
        }

        public void DisableUnlicensedFeatures()
        {
            if (!CMC_CLA.Current.PulseAnalysis_IsAuthorized)
            {
                if (Computations != null) Computations.PulseAnalysisOptions.PulseAnalysisEnabled = false;
                //this.Computations?.PulseAnalysisOptions.PulseAnalysisEnabled = false;
                this.PulseAnalysisSettingsWindow_IsVisible = false;
            }
            if (!CMC_CLA.Current.LimitsAndAlarms_IsAuthorized)
            {
                NonvolatileUserSettings instance = NonvolatileUserSettings.getInstance();
                AlarmsAndLimitsSettings andLimitsSettings = instance.AlarmsAndLimitsSettings;
                andLimitsSettings.DisableAllAlarms();
                instance.AlarmsAndLimitsSettings = andLimitsSettings;
            }
            NonvolatileUserSettings.getInstance().RaiseSettingsChanged();
        }

        public bool PERMANANT_FREE_TRIAL => false;

        public bool HideSalesDemoMessage { get; set; }

        public Visibility SalesDemoWarningVisibility => !this.PERMANANT_FREE_TRIAL || this.HideSalesDemoMessage ? Visibility.Collapsed : Visibility.Visible;

        public bool SuppressFreeTrialMessages
        {
            get => this.suppressFreeTrialMessages;
            set
            {
                if (!this.OnPropertyChanged<bool>(ref this.suppressFreeTrialMessages, value, nameof(SuppressFreeTrialMessages)))
                    return;
                this.SalesDemoOptionsText = value ? "[ No Dialogs ]" : "";
            }
        }

        public string SalesDemoOptionsText
        {
            get => this.salesDemoOptionsText;
            set => this.OnPropertyChanged<string>(ref this.salesDemoOptionsText, value, nameof(SalesDemoOptionsText));
        }

        public void RegisterPulseAnalysisSettingsWindow(
          IPulseAnalysisSettingsService PulseAnalysisSettingsWindow)
        {
            this.PulseAnalysisSettingsWindow = PulseAnalysisSettingsWindow;
            this.PulseAnalysisSettingsWindow_IsVisible = this.PulseAnalysisSettingsWindow.IsVisible;
            this.PulseAnalysisSettingsWindow.IsVisibleChanged += (DependencyPropertyChangedEventHandler)((s, e) => this.PulseAnalysisSettingsWindow_IsVisible = PulseAnalysisSettingsWindow.IsVisible);
        }

        [DataMember]
        public bool PulseAnalysisSettingsWindow_IsVisible
        {
            get => this.pulseAnalysisSettingsWindow_IsVisible;
            set
            {
                if (value && !CMC_CLA.Current.PulseAnalysis_CheckAuthorization() || !this.OnPropertyChanged<bool>(ref this.pulseAnalysisSettingsWindow_IsVisible, value, nameof(PulseAnalysisSettingsWindow_IsVisible)))
                    return;
                if (value)
                    this.PulseAnalysisSettingsWindow.Show();
                else
                    this.PulseAnalysisSettingsWindow.Hide();
            }
        }

        public bool PulseAnalysisFeature_IsSupported
        {
            get => this.pulseAnalysisFeature_IsSupported;
            set => this.OnPropertyChanged<bool>(ref this.pulseAnalysisFeature_IsSupported, value, nameof(PulseAnalysisFeature_IsSupported));
        }

        [DataMember]
        public bool ShowCursors_IsChecked
        {
            get => this.showCursors_IsChecked;
            set
            {
                if (this.showCursors_IsChecked == value)
                    return;
                this.OnPropertyChanged<bool>(ref this.showCursors_IsChecked, value, nameof(ShowCursors_IsChecked));
            }
        }

        [DataMember]
        public bool SelectionCursors_AreActive
        {
            get => this.selectionCursors_AreActive;
            set
            {
                if (this.selectionCursors_AreActive == value)
                    return;
                this.OnPropertyChanged<bool>(ref this.selectionCursors_AreActive, value, nameof(SelectionCursors_AreActive));
            }
        }

        public bool ShowCursors_IsEnabled => this.IsNotRunning && this.BufferNonEmpty;

        [DataMember]
        public bool ShowTrackerCursors_IsChecked
        {
            get => this.showTrackerCursors_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showTrackerCursors_IsChecked, value, nameof(ShowTrackerCursors_IsChecked));
        }

        [DataMember]
        public bool ShowTriggerMarkers_IsChecked
        {
            get => this.showTriggerMarkers_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.showTriggerMarkers_IsChecked, value, nameof(ShowTriggerMarkers_IsChecked));
        }

        public bool ShowTriggerMarkers_IsEnabled
        {
            get => this.showTriggerMarkers_IsEnabled;
            set => this.OnPropertyChanged<bool>(ref this.showTriggerMarkers_IsEnabled, value, nameof(ShowTriggerMarkers_IsEnabled));
        }

        [DataMember]
        public bool HighlightSamplePoints_IsChecked
        {
            get => this._HighlightSamplePoints_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this._HighlightSamplePoints_IsChecked, value, nameof(HighlightSamplePoints_IsChecked));
        }

        public bool HighlightSamplePoints_IsEnabled
        {
            get => this._HighlightSamplePoints_IsEnabled;
            set => this.OnPropertyChanged<bool>(ref this._HighlightSamplePoints_IsEnabled, value, nameof(HighlightSamplePoints_IsEnabled));
        }

        [DataMember]
        public bool SnapToTriggers_IsChecked
        {
            get => this.snapToTriggers_IsChecked;
            set => this.OnPropertyChanged<bool>(ref this.snapToTriggers_IsChecked, value, nameof(SnapToTriggers_IsChecked));
        }

        public bool TriggerLevelFormatScientific_IsChecked
        {
            get => this.triggerLevelFormatScientific_IsChecked;
            set => this.updateTriggerLevelFormat(value);
        }

        public bool TriggerLevelFormatFixed_IsChecked
        {
            get => this.triggerLevelFormatFixed_IsChecked;
            set => this.updateTriggerLevelFormat(!value);
        }

        private void updateTriggerLevelFormat(bool isScientific)
        {
            this.triggerLevelFormatScientific_IsChecked = isScientific;
            this.triggerLevelFormatFixed_IsChecked = !isScientific;
            this.OnPropertyChanged("TriggerLevelFormatScientific_IsChecked", (object)this.triggerLevelFormatScientific_IsChecked);
            this.OnPropertyChanged("TriggerLevelFormatFixed_IsChecked", (object)this.triggerLevelFormatFixed_IsChecked);
        }

        public bool TriggerPercentFormatScientific_IsChecked
        {
            get => this.triggerPercentFormatScientific_IsChecked;
            set => this.updateTriggerPercentFormat(value);
        }

        public bool TriggerPercentFormatFixed_IsChecked
        {
            get => this.triggerPercentFormatFixed_IsChecked;
            set => this.updateTriggerPercentFormat(!value);
        }

        private void updateTriggerPercentFormat(bool isScientific)
        {
            this.triggerPercentFormatScientific_IsChecked = isScientific;
            this.triggerPercentFormatFixed_IsChecked = !isScientific;
            this.OnPropertyChanged("TriggerPercentFormatScientific_IsChecked", (object)this.triggerPercentFormatScientific_IsChecked);
            this.OnPropertyChanged("TriggerPercentFormatFixed_IsChecked", (object)this.triggerPercentFormatFixed_IsChecked);
        }

        public bool TriggerLevelFormatFixed_IsEnabled
        {
            get => this.triggerLevelFormatFixed_IsEnabled;
            set
            {
                this.triggerLevelFormatFixed_IsEnabled = value;
                this.OnPropertyChanged(nameof(TriggerLevelFormatFixed_IsEnabled), (object)this.triggerLevelFormatFixed_IsEnabled);
            }
        }

        public bool TriggerPercentFormatFixed_IsEnabled
        {
            get => this.triggerPercentFormatFixed_IsEnabled;
            set
            {
                this.triggerPercentFormatFixed_IsEnabled = value;
                this.OnPropertyChanged(nameof(TriggerPercentFormatFixed_IsEnabled), (object)this.triggerPercentFormatFixed_IsEnabled);
            }
        }

        public bool ContinuousUpdate_IsChecked
        {
            get => this._ContinuousUpdate_IsChecked;
            set
            {
                if (!this.OnPropertyChanged<bool>(ref this._ContinuousUpdate_IsChecked, value, nameof(ContinuousUpdate_IsChecked)))
                    return;
                this.PhoenixMeter.Updates_Enabled = value;
            }
        }

        public bool MeterPolling_IsChecked
        {
            get => this._MeterPolling_IsChecked;
            set
            {
                if (!this.OnPropertyChanged<bool>(ref this._MeterPolling_IsChecked, value, nameof(MeterPolling_IsChecked)))
                    return;
                this.PhoenixMeter.Polling_Enabled = value;
                this.ContinuousUpdate_IsEnabled = value;
            }
        }

        public bool ContinuousUpdate_IsEnabled
        {
            get => this._ContinuousUpdate_IsEnabled;
            set => this.OnPropertyChanged<bool>(ref this._ContinuousUpdate_IsEnabled, value, nameof(ContinuousUpdate_IsEnabled));
        }

        public bool MeterPolling_IsEnabled
        {
            get => this._MeterPolling_IsEnabled;
            set
            {
                if (!this.OnPropertyChanged<bool>(ref this._MeterPolling_IsEnabled, value, nameof(MeterPolling_IsEnabled)))
                    return;
                this.PhoenixMeter.Updates_Enabled = value;
            }
        }

        public bool RetainBinaryData_IsChecked
        {
            get => this._RetainBinaryData_IsChecked;
            set
            {
                if (!this.OnPropertyChanged<bool>(ref this._RetainBinaryData_IsChecked, value, nameof(RetainBinaryData_IsChecked)) || !(this.Meter is IHasBinary meter))
                    return;
                meter.RetainBinary = value;
            }
        }

        public bool RetainBinaryData_IsEnabled
        {
            get => this._RetainBinaryData_IsEnabled;
            set => this.OnPropertyChanged<bool>(ref this._RetainBinaryData_IsEnabled, value, nameof(RetainBinaryData_IsEnabled));
        }

        public bool UploadSeqIds_IsChecked
        {
            get => this._UploadSeqIds_IsChecked;
            set
            {
                if (!this.OnPropertyChanged<bool>(ref this._UploadSeqIds_IsChecked, value, nameof(UploadSeqIds_IsChecked)))
                    return;
                this.Meter.UploadSequenceIDs = value;
            }
        }

        public bool UploadSeqIds_IsEnabled
        {
            get => this._UploadSeqIds_IsEnabled;
            set => this.OnPropertyChanged<bool>(ref this._UploadSeqIds_IsEnabled, value, nameof(UploadSeqIds_IsEnabled));
        }

        private void updateZoomModeRadioBtns(MainViewModel.PanZoomMode mode, bool value)
        {
            if (!value)
                return;
            this.zoomNone_IsChecked = mode == MainViewModel.PanZoomMode.None;
            this.zoomHorizontal_IsChecked = mode == MainViewModel.PanZoomMode.Horizontal;
            this.zoomVertical_IsChecked = mode == MainViewModel.PanZoomMode.Vertical;
            this.zoomBoth_IsChecked = mode == MainViewModel.PanZoomMode.Both;
            this.OnPropertyChanged("ZoomNone_IsChecked", (object)this.zoomNone_IsChecked);
            this.OnPropertyChanged("ZoomHorizontal_IsChecked", (object)this.zoomNone_IsChecked);
            this.OnPropertyChanged("ZoomVertical_IsChecked", (object)this.zoomNone_IsChecked);
            this.OnPropertyChanged("ZoomBoth_IsChecked", (object)this.zoomNone_IsChecked);
        }

        public bool ZoomNone_IsChecked
        {
            get => this.zoomNone_IsChecked;
            set => this.updateZoomModeRadioBtns(MainViewModel.PanZoomMode.None, value);
        }

        public bool ZoomHorizontal_IsChecked
        {
            get => this.zoomHorizontal_IsChecked;
            set => this.updateZoomModeRadioBtns(MainViewModel.PanZoomMode.Horizontal, value);
        }

        public bool ZoomVertical_IsChecked
        {
            get => this.zoomVertical_IsChecked;
            set => this.updateZoomModeRadioBtns(MainViewModel.PanZoomMode.Vertical, value);
        }

        public bool ZoomBoth_IsChecked
        {
            get => this.zoomBoth_IsChecked;
            set => this.updateZoomModeRadioBtns(MainViewModel.PanZoomMode.Both, value);
        }

        public double tuningPanelDialWidth
        {
            protected set
            {
                this._tuningPanelDialWidth = value / 1.2;
                this.OnPropertyChanged(nameof(tuningPanelDialWidth));
            }
            get => this._tuningPanelDialWidth;
        }

        public double tuningPanelDialHeight
        {
            protected set
            {
                this._tuningPanelDialHeight = value / 1.18;
                this.OnPropertyChanged(nameof(tuningPanelDialHeight));
            }
            get => this._tuningPanelDialHeight;
        }

        public void TuningPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                if (e.NewSize.Width < this.tuningPanelDialWidth)
                    this.tuningPanelDialHeight = e.NewSize.Height;
                this.tuningPanelDialWidth = e.NewSize.Width;
            }
            else
            {
                if (e.NewSize.Height < this.tuningPanelDialHeight)
                    this.tuningPanelDialWidth = e.NewSize.Width;
                this.tuningPanelDialHeight = e.NewSize.Height;
            }
        }

        [API(APICategory.Unclassified)]
        public string Settings
        {
            get => SharedLibrary.Serialization.Serialize((object)this);
            set
            {
                try
                {
                    this.DeSerialize(value);
                }
                catch (Exception ex)
                {
                }
            }
        }

        [Conditional("TRACE_WAVE")]
        public void TraceWave(string format, params object[] args)
        {
        }

        private enum PanZoomMode
        {
            None,
            Horizontal,
            Vertical,
            Both,
        }
    }
}
