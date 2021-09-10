using Newtonsoft.Json;
using SharedLibrary;
using System;
using System.Diagnostics;
using System.IO;

namespace DEV.PowerMeter.Library
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RecorderConfig
    {
        protected static RecorderConfig singleton;
        public Stopwatch Timer = new Stopwatch();

        protected RecorderConfig()
        {
        }

        public static RecorderConfig Singleton => RecorderConfig.singleton ?? (RecorderConfig.singleton = new RecorderConfig());

        [JsonProperty]
        public string DestinationFolder { get; set; }

        [JsonProperty]
        public string RecordingPathname { get; set; }

        [JsonProperty]
        public bool RecordingEnabled { get; set; }

        [JsonProperty]
        public bool EnsureUniqueFilename { get; set; }

        protected Channel_Recorder Channel_Recorder { get; set; }

        public void Open(string folder)
        {
            this.DestinationFolder = folder;
            this.RecordingEnabled = true;
        }

        public string RecordingBasename { get; protected set; }

        public void SetRecordingBasename(string basename)
        {
            if (!(this.RecordingBasename != basename))
                return;
            this.RecordingBasename = basename;
            this.RecordingPathname = Path.Combine(this.DestinationFolder, this.RecordingBasename + ".dat");
        }

        public void SetRecordingPathname(string basename)
        {
            this.SetRecordingBasename(basename);
            if (!this.EnsureUniqueFilename)
                return;
            this.RecordingPathname = DestinationPathname.EnsureUniqueFilename(this.RecordingPathname);
        }

        public void PrepareToStart(Meter meter)
        {
            this.SetRecordingPathname(meter.GetFilename());
            this.Channel_Recorder = new Channel_Recorder(meter, this.RecordingPathname);
            meter.DataAcquisitionThread.OnExit += new Action<DAQ_Thread>(this.Finish);
            this.Timer.Restart();
        }

        public void Finish(DAQ_Thread obj)
        {
            this.Channel_Recorder.Close();
            this.Timer.Stop();
            Action recordingFinished = this.RecordingFinished;
            if (recordingFinished == null)
                return;
            recordingFinished();
        }

        public event Action RecordingFinished;
    }
}
