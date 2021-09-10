using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Utility, "Access the Meter hardware via .NET's SerialPort class. \r\nThe device is usually USB, but RS-232 also may work, but at slower speed.")]
    public class Channel_SerialPort : Channel
    {
        public const int WRITE_TIMEOUT = 200;
        public const int READ_TIMEOUT = 300;
        public const int DefaultBaudRate = 115200;
        public const Parity DefaultParity = Parity.None;
        public const StopBits DefaultStopBits = StopBits.One;
        public const int DefaultDataBits = 8;
        public const Handshake DefaultHandshake = Handshake.None;
        public const string Newline = "\n";
        public const int ReadBufferSize = 40000;
        protected string InputData;
        protected bool ReadDataContinuation;

        [API("The SerialPort instance for this Channel, when IsOpen. SerialPort is recreated for each Open.")]
        public SerialPort Port { get; protected set; }

        public override bool IsOpen => this.Port != null && this.Port.IsOpen;

        public override string PortName => !this.IsOpen ? "[Unopened]" : this.Port.PortName;

        [API("Constructor for a Channel associated with a specific COM port.")]
        public Channel_SerialPort(string portName, int baudRate = 115200) => this.Open(portName, baudRate);

        [API("Constructor for a Channel associated with another channel.")]
        protected Channel_SerialPort(Channel_SerialPort channel) => this.Port = channel.Port;

        protected Channel_SerialPort()
        {
        }

        [API("Open the port. Throws DeviceOpenException if anything goes wrong.")]
        public void Open(string portName, int baudRate = 0)
        {
            if (baudRate == 0)
                baudRate = 115200;
            TraceLogger.TraceOpenClose("Opening {0} at {1}", (object)portName, (object)baudRate);
            try
            {
                this.Port = new SerialPort(portName);
                this.Port.BaudRate = baudRate;
                this.Port.Parity = Parity.None;
                this.Port.StopBits = StopBits.One;
                this.Port.DataBits = 8;
                this.Port.Handshake = Handshake.None;
                this.Port.NewLine = "\n";
                this.Port.ReadBufferSize = 40000;
                this.Port.ReadTimeout = 300;
                this.Port.WriteTimeout = 200;
                this.Port.Open();
                this.Port.ReadTimeout = 300;
                this.Port.WriteTimeout = 200;
                this.FlushInput();
            }
            catch (Exception ex)
            {
                this.Close();
                throw new DeviceOpenException(ex);
            }
        }

        [API("Close this Channel and associated COM port.")]
        public override void Close()
        {
            TraceLogger.TraceOpenClose("Closing {0}", (object)this.PortName);
            try
            {
                if (this.Port == null)
                    return;
                this.Port.Close();
            }
            catch
            {
            }
        }

        [API("FlushInput then Write a command string to the Channel.")]
        public override void WriteLine(string command)
        {
            this.FlushInput();
            this.WritelineBase(command);
        }

        [API("Write a command string to the Channel, without first Flushing the input stream.")]
        public override void WriteLineNoFlush(string command) => this.WritelineBase(command);

        protected void WritelineBase(string command)
        {
            if (!this.Port.IsOpen)
                return;
            TraceLogger.TraceWrite(command);
            this.Port.Write(command + "\r\n");
        }

        [API("Flush the input channel of the Port. \r\nNormally done before each command is written, \r\nto help keep commands and their responses in synch.")]
        public override void FlushInput()
        {
            if (!this.IsOpen)
                return;
            this.Port.DiscardInBuffer();
        }

        [API("Read one line from the Port, trim leading and trailing whitespace, and return it. \r\nThe Port returns null to indicate EOF (which shouldn't happen).\r\nA TimeoutException will be thrown if the device on the Port takes \r\nlonger than READ_TIMEOUT to reply.\r\n.NET also may throw other exceptions (per SerialPort doc).")]
        public override string ReadLine()
        {
            this.ThrowIfFakeReadTimeout();
            if (!this.Port.IsOpen)
                return (string)null;
            string text = this.Port.ReadLine().Trim();
            TraceLogger.TraceRead(text);
            return text;
        }

        protected void ThrowIfFakeReadTimeout()
        {
            if (this.FakeReadTimeout)
                throw new TimeoutException();
        }

        [API("Read binary bytes from the port, using the Port.Read method.")]
        public override int Read(byte[] data, int offset, int count)
        {
            this.ThrowIfFakeReadTimeout();
            return !this.Port.IsOpen ? 0 : this.Port.Read(data, offset, count);
        }

        [API("Read a byte from the port.")]
        protected byte GetByte()
        {
            this.ThrowIfFakeReadTimeout();
            byte[] buffer = new byte[1];
            return this.Port.Read(buffer, 0, 1) == 1 ? buffer[0] : throw new UnexpectedEofException("Channel_SerialPort( " + this.PortName + " )");
        }

        [API("Read a char from the port.")]
        protected char GetChar() => (char)this.GetByte();

        [API("Read a char from the port, truncating to 7 bit ASCII.")]
        protected char Get7BitChar() => (char)((uint)this.GetByte() & (uint)sbyte.MaxValue);

        [API("Read one line of ascii chars from the port, using the Channel.GetChar method.")]
        public override string ReadData()
        {
            if (!this.ReadDataContinuation)
                this.ReadDataReset();
            try
            {
                while (true)
                {
                    char ch;
                    do
                    {
                        ch = this.Get7BitChar();
                    }
                    while (ch == '\n');
                    if (ch != '\r')
                        this.InputData += ch.ToString();
                    else
                        break;
                }
                string inputData = this.InputData;
                this.ReadDataReset();
                return inputData;
            }
            catch (TimeoutException ex)
            {
                this.ReadDataContinuation = true;
                throw;
            }
        }

        protected void ReadDataReset()
        {
            this.InputData = string.Empty;
            this.ReadDataContinuation = false;
        }
    }
}
