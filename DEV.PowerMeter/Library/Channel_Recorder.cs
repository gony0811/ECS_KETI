using System;

namespace DEV.PowerMeter.Library
{
    public sealed class Channel_Recorder : Channel_SerialPort, IDisposable
    {
        private Channel_SerialPort Channel_SerialPort;
        private Meter Meter;
        private Communicator Communicator;

        public string Filename { get; set; }

        public DataStreamWriter DataStreamWriter { get; set; }

        public override bool IsOpen => this.DataStreamWriter != null;

        public override string PortName => "[Channel_Recorder]";

        public Channel_Recorder(Meter meter, string filename)
        {
            this.Create(filename);
            this.Meter = meter;
            this.Communicator = this.Meter.Device.Communicator;
            this.Channel_SerialPort = this.Communicator.Channel as Channel_SerialPort;
            this.Port = (this.Communicator.Channel as Channel_SerialPort).Port;
            this.Communicator.Channel = (Channel)this;
            MeterPlayer meterPlayer = new MeterPlayer(meter);
            this.WriteRemark(meter.GetRemarks());
            this.WriteSettings(meterPlayer.Serialize());
        }

        public void Create(string filename)
        {
            this.Filename = filename;
            this.DataStreamWriter = new DataStreamWriter(filename);
        }

        public override void Close()
        {
            this.DataStreamWriter.WriteRemark(string.Format("Elapsed: {0}", (object)this.DataStreamWriter.Stopwatch.ElapsedMilliseconds));
            this.DataStreamWriter.Close();
            this.Communicator.Channel = (Channel)this.Channel_SerialPort;
        }

        public void Dispose() => this.Close();

        public void WriteRemark(string remark) => this.DataStreamWriter.WriteRemark(remark);

        public void WriteSettings(string settings) => this.DataStreamWriter.WriteSettings(settings);

        public override void WriteLine(string command)
        {
            base.WriteLine(command);
            this.DataStreamWriter.WriteCommand(command);
        }

        public override void WriteLineNoFlush(string command)
        {
            base.WriteLineNoFlush(command);
            this.DataStreamWriter.WriteCommand(command);
        }

        public override string ReadLine()
        {
            string command = base.ReadLine();
            this.DataStreamWriter.WriteCommand(command);
            return command;
        }

        public override int Read(byte[] data, int offset, int count)
        {
            int count1 = base.Read(data, offset, count);
            this.DataStreamWriter.WriteData(data, offset, count1);
            return count1;
        }

        public override string ReadData()
        {
            string data = base.ReadData();
            this.DataStreamWriter.WriteData(data);
            return data;
        }
    }
}
