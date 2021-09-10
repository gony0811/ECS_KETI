using System;
using System.IO.Ports;

namespace DEV.PowerMeter.Library
{
    public sealed class Channel_Playback : Channel_SerialPort, IDisposable
    {
        private double originalElapsedMilliseconds;

        public string Filename { get; set; }

        public DataStreamReader DataStreamReader { get; set; }

        public string Settings => this.DataStreamReader.Settings;

        public override bool IsOpen => this.DataStreamReader != null;

        public override string PortName => "[Channel_Playback of " + this.Filename + "]";

        public double VirtualElapsedMilliseconds
        {
            get
            {
                if (this.originalElapsedMilliseconds != 0.0)
                    return this.originalElapsedMilliseconds;
                string str = this.DataStreamReader.ReadNextRemark();
                double result;
                if (!string.IsNullOrEmpty(str) && double.TryParse(str.Split()[1], out result))
                {
                    this.originalElapsedMilliseconds = result;
                    return this.originalElapsedMilliseconds;
                }
                this.originalElapsedMilliseconds = -1.0;
                return this.originalElapsedMilliseconds;
            }
        }

        public Channel_Playback(string filename)
        {
            this.Open(filename);
            this.Port = (SerialPort)null;
        }

        public void Open(string filename)
        {
            this.Filename = filename;
            this.DataStreamReader = new DataStreamReader(filename);
            this.DataStreamReader.Initialize();
        }

        public override void Close()
        {
            double elapsedMilliseconds = this.VirtualElapsedMilliseconds;
            this.DataStreamReader.Close();
        }

        public void Dispose()
        {
            this.Close();
            this.DataStreamReader.Dispose();
            this.DataStreamReader = (DataStreamReader)null;
        }

        public override void WriteLine(string command) => this.DataStreamReader.WriteCommand(command);

        public override void WriteLineNoFlush(string command) => this.DataStreamReader.WriteCommand(command);

        public override string ReadLine() => this.DataStreamReader.ReadCommand();

        public override int Read(byte[] data, int offset, int count) => this.DataStreamReader.ReadData(data, offset, count);

        public override string ReadData() => this.DataStreamReader.ReadData();

        public override void FlushInput()
        {
        }
    }
}
