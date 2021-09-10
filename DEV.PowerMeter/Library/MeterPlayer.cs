using DEV.PowerMeter.Library.DeviceModels;
using DEV.PowerMeter.Library.ImportExport;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;


namespace DEV.PowerMeter.Library
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MeterPlayer : IDaqMeter, IDisposable
    {
        public const OperatingMode OpModeDefault = OperatingMode.PowerWatts;
        public const int SlowBufferSize = 25;
        public const int SlowPreTriggerSize = 0;
        public const double SelectedTriggerLevel = 2.5;
        public const int DefaultTimeout = 10000;

        [JsonIgnore]
        public Type MeterClassType { get; set; }

        [JsonProperty]
        public string MeterClassTypeName { get; set; }

        [JsonProperty]
        public Meter.DataAcquisitionMode DAQ_Mode { get; set; }

        [JsonProperty]
        public bool IsLegacyMeterless { get; set; }

        public Meter Meter { get; protected set; }

        public IDaqDevice Device { get; set; }

        public CaptureBuffer CaptureBuffer { get; set; }

        public Statistics Statistics { get; set; }

        [JsonProperty]
        public OperatingMode OperatingMode { get; set; }

        [JsonProperty]
        public uint Capacity { get; set; }

        [JsonProperty]
        public uint PreTrigger { get; set; }

        [JsonProperty]
        public bool ContinuousMode { get; set; }

        [JsonProperty]
        public DataEncoding SelectedDataEncoding { get; set; }

        [JsonIgnore]
        public Units SelectedUnits { get; set; } = Units.Watts;

        [JsonProperty]
        public string SelectedUnitsSerialization
        {
            get => this.SelectedUnits.Encode();
            set => this.SelectedUnits = Units.Decode(value);
        }

        [JsonProperty]
        public DataFieldFlags SelectedDataFields { get; set; }

        [JsonProperty]
        public bool SnapshotMode_IsSelected { get; set; }

        [JsonProperty]
        public bool WaitForTriggerModeEnabled { get; set; }

        [JsonProperty]
        public bool HighSpeedChannel_IsSelected { get; set; }

        [JsonProperty]
        public long SamplePeriod { get; set; }

        [JsonProperty]
        public bool Sensor_IsPyro { get; set; }

        [JsonProperty]
        public bool Sensor_IsQuad { get; set; }

        [JsonProperty]
        public long SamplePeriod_Ticks
        {
            get => Timestamper.SamplePeriod_Ticks;
            set => Timestamper.SamplePeriod_Ticks = value;
        }

        [JsonProperty]
        public bool QuadMode_IsSelected { get; set; }

        [JsonProperty]
        public bool OperatingMode_IsTrueEnergy { get; set; }

        [JsonProperty]
        public uint SnapshotMaxCapacity { get; set; }

        [JsonProperty]
        public bool AreaCorrection_Enabled { get; set; }

        [JsonProperty]
        public double AreaCorrectionValue { get; set; }

        [JsonProperty]
        public bool GainCompensation_Enabled { get; set; }

        [JsonProperty]
        public double GainCompensationFactor { get; set; }

        [JsonProperty]
        public bool Decimation_Enabled { get; set; }

        [JsonProperty]
        public uint DecimationRate { get; set; }

        [JsonProperty]
        public bool SmoothingEnabled { get; set; }

        [JsonProperty]
        public bool Speedup_Enabled { get; set; }

        [JsonIgnore]
        public Channel_Playback Channel_Playback { get; protected set; }

        [JsonIgnore]
        public DAQ_Thread DAQ_Thread { get; set; }

        public event Library.DAQ_StateChanged DAQ_StateChanged;

        [JsonIgnore]
        public bool IsRunning => this.DAQ_Thread != null && this.DAQ_Thread.IsRunning;

        public double VirtualElapsedSeconds => this.Channel_Playback.VirtualElapsedMilliseconds / 1000.0;

        public double ActualElapsedSeconds => this.DAQ_Thread.Elapsed.TotalSeconds;

        public string Serialize() => JsonConvert.SerializeObject((object)this, new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented
        });

        public void PopulateObject(string value) => JsonConvert.PopulateObject(value, (object)this, new JsonSerializerSettings()
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        });

        public static MeterPlayer DeSerialize(string value) => JsonConvert.DeserializeObject<MeterPlayer>(value, new JsonSerializerSettings()
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        });

        protected MeterPlayer()
        {
        }

        public MeterPlayer(Meter meter)
        {
            this.Meter = meter;
            this.MeterClassType = meter.GetType();
            this.MeterClassTypeName = this.MeterClassType.Name;
            this.Device = (IDaqDevice)this.Meter.Device;
            this.DAQ_Mode = this.Meter.DAQ_Mode;
            this.IsLegacyMeterless = this.Meter is ILegacyMeterless;
            this.OperatingMode = this.Meter.OperatingMode;
            this.SelectedUnits = Units.SelectedUnits;
            if (!this.SelectedUnits.IsConsistentWith(this.OperatingMode))
                this.SelectedUnits = new Units(this.OperatingMode, this.SelectedUnits.AreaCorrected);
            this.SelectedDataFields = this.Meter.SelectedDataFields;
            this.SelectedDataEncoding = this.Meter.SelectedDataEncoding;
            this.OperatingMode_IsTrueEnergy = this.Meter.OperatingMode_IsTrueEnergy;
            this.Capacity = this.Meter.Capacity;
            this.Sensor_IsPyro = this.Meter.Sensor_IsPyro;
            this.QuadMode_IsSelected = this.Meter.QuadMode_IsSelected;
            this.SnapshotMaxCapacity = this.Meter.SnapshotMaxCapacity;
            this.SamplePeriod = this.Meter.SamplePeriod;
            this.ContinuousMode = this.Meter.ContinuousMode;
            this.HighSpeedChannel_IsSelected = this.Meter.HighSpeedChannel_IsSelected;
            this.SnapshotMode_IsSelected = this.Meter.SnapshotMode_IsSelected;
            this.WaitForTriggerModeEnabled = this.Meter.WaitForTriggerModeEnabled;
            this.AreaCorrection_Enabled = this.Meter.AreaCorrection_Enabled;
            this.AreaCorrectionValue = this.Meter.AreaCorrectionValue;
            this.GainCompensation_Enabled = this.Meter.GainCompensation_Enabled;
            this.GainCompensationFactor = this.Meter.GainCompensationFactor;
            this.Decimation_Enabled = this.Meter.Decimation_Enabled;
            this.DecimationRate = this.Meter.DecimationRate;
            this.SmoothingEnabled = this.Meter.SmoothingEnabled;
            this.Speedup_Enabled = this.Meter.Speedup_Enabled;
        }

        public MeterPlayer(string playbackFilename)
        {
            try
            {
                this.Trace("Constructor ==================================");
                this.Trace("Filename: " + Path.GetFileName(playbackFilename));
                this.Channel_Playback = new Channel_Playback(playbackFilename);
                this.PopulateObject(this.Channel_Playback.Settings);
                this.MeterClassType = Models.NameTypeMap[this.MeterClassTypeName];
                this.Meter = (Meter)Activator.CreateInstance(this.MeterClassType);
                this.Meter.OpenRaw((Channel)this.Channel_Playback);
                this.Device = (IDaqDevice)new DevicePlayer(this.Meter, (Channel)this.Channel_Playback);
                this.Statistics = new Statistics();
                this.PrepareToStart();
            }
            catch (System.Exception ex)
            {
                this.Trace("MeterPlayer exception: " + ex.Message);
                this.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            this.Channel_Playback?.Dispose();
            this.Channel_Playback = (Channel_Playback)null;
        }

        public void PrepareToStart()
        {
            DataRecordBase.SelectedDataFields = this.SelectedDataFields;
            if (!this.SelectedUnits.IsConsistentWith(this.OperatingMode))
                this.SelectedUnits = new Units(this.OperatingMode, this.SelectedUnits.AreaCorrected);
            MagnitudeConverter.Units = Units.SelectedUnits = this.SelectedUnits ?? Units.Watts;
            Timestamper.SetSamplePeriod((IDaqMeter)this);
            Timestamper.Clear();
            bool Sensor_IsQuad = this.QuadMode_IsSelected || this.Meter.Sensor_IsQuad;
            this.CaptureBuffer = (CaptureBuffer)new CaptureBufferCircular(new Header(Units.SelectedUnits, Sensor_IsQuad, this.Meter.Sensor_IsPyro)
            {
                MeterSerial = nameof(MeterPlayer)
            }, this.Capacity);
            this.Meter?.SetCaptureBuffer(this.CaptureBuffer);
            this.DAQ_StateChanged = new Library.DAQ_StateChanged(this.OnDAQ_StateChanged);
            this.DAQ_Thread = (DAQ_Thread)Activator.CreateInstance(Meter.GetDAQ_ThreadType(this.DAQ_Mode, this.IsLegacyMeterless), (object)this.DAQ_StateChanged, (object)this, (object)this.Device, (object)this.CaptureBuffer);
        }

        public void Start()
        {
            this.CaptureBuffer.Clear();
            this.Statistics.Clear();
            this.Channel_Playback.DataStreamReader.UnexpectedRecordEncountered += new Action<DataStreamItem>(this.UnexpectedRecordEncountered);
            this.Trace("Starting ..................................");
            this.DAQ_Thread.Start();
        }

        public void Stop()
        {
            this.Trace("Stopping ..................................");
            this.DAQ_Thread.Stop();
        }

        private void OnDAQ_StateChanged(DAQ_Thread sender, DAQ_StateEventArgs e) => this.Trace(string.Format("DAQ_StateChanged: {0}", (object)e.DAQ_State));

        public void SetPollingMode()
        {
        }

        private void UnexpectedRecordEncountered(DataStreamItem item)
        {
            if (!item.IsAbortOrStopCommand)
                throw new NotImplementedException();
            this.Trace("Explicit Command: " + item.DataAsString.ToUpper());
            this.Stop();
        }

        public bool Playback(int timeout = 10000)
        {
            bool flag = false;
            this.Start();
            try
            {
                while (this.IsRunning)
                {
                    Thread.Sleep(500);
                    TimeSpan elapsed = this.DAQ_Thread.Elapsed;
                    this.Trace(string.Format("{0} ms, ", (object)elapsed.TotalMilliseconds) + this.CaptureBuffer.ToString());
                    elapsed = this.DAQ_Thread.Elapsed;
                    if (elapsed.TotalMilliseconds >= (double)timeout)
                    {
                        this.Trace("Playback Manually stopping meter after timeout expired");
                        flag = true;
                        break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                this.Trace("Playback Unexpected exception: " + ex.Message);
            }
            finally
            {
                if (this.IsRunning)
                {
                    this.Trace("Playback Meter still running, stopping manually");
                    this.Stop();
                }
            }
            this.Statistics.AddRange((IEnumerable<DataRecordSingle>)this.CaptureBuffer);
            this.Trace(string.Format("EXITING Elapsed={0}, ", (object)this.DAQ_Thread.Elapsed.TotalMilliseconds) + string.Format("TimedOut={0}", (object)flag));
            this.Trace(string.Format("Original Elapsed={0}", (object)this.Channel_Playback.VirtualElapsedMilliseconds));
            this.Trace("Buffer: " + this.CaptureBuffer.ToString());
            this.Trace("Filename: " + Path.GetFileName(this.Channel_Playback.Filename));
            return flag;
        }

        public void Trace(string message)
        {
        }
    }
}    
