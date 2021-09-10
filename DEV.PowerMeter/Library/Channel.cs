using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV.PowerMeter.Library
{
    [API(APICategory.Utility, "Channel is the abstract class for all Channels.")]
    public abstract class Channel
    {
        [API("True iff the SerialPort is IsOpen.")]
        public virtual bool IsOpen { get; }

        [API("Name of the SerialPort.")]
        public virtual string PortName { get; }

        public override string ToString() => string.Format("{0}( {1} )", (object)this.GetType().Name, (object)this.PortName);

        public virtual void Close()
        {
        }

        [API("Write a command to the port.")]
        public abstract void WriteLine(string command);

        [API("Write a command without FlushInput.")]
        public abstract void WriteLineNoFlush(string command);

        [API("Flush any pending input.")]
        public abstract void FlushInput();

        [API("Read one line from the port.")]
        public abstract string ReadLine();

        public bool FakeReadTimeout { get; set; }

        [API("Send command, read and return single line response.")]
        public string SendAndReceiveOneLine(string command)
        {
            this.WriteLine(command);
            return this.ReadLine();
        }

        [API("Read binary bytes from the port.")]
        public abstract int Read(byte[] data, int offset, int count);

        [API("Read binary bytes from the port, using the Port.Read method, and a 0 offset.")]
        public virtual int Read(byte[] data, int count) => this.Read(data, 0, count);

        [API("Read binary bytes from the port, using the Port.Read method.\r\n\t\t\tOffset is 0, and count is data.Length.")]
        public virtual int Read(byte[] data) => this.Read(data, 0, data.Length);

        public abstract string ReadData();

        public bool SkipUnusedData(float timeout_ms = 0.0f)
        {
            StringBuilder stringBuilder = new StringBuilder();
            byte[] numArray = new byte[1200];
            int num = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                do
                {
                    int count = this.Read(numArray);
                    if (count > 0)
                    {
                        string str = Encoding.UTF8.GetString(numArray, 0, count);
                        stringBuilder.Append(str);
                        num += count;
                    }
                }
                while ((double)timeout_ms <= 0.0 || stopwatch.Elapsed.TotalMilliseconds <= (double)timeout_ms);
                this.Trace("SkipUnused fails due to timeout limit");
            }
            catch (TimeoutException ex)
            {
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public void Trace(string message)
        {
        }

        [Conditional("TRACE_SKIP_UNUSED")]
        public void TraceSkip(string message)
        {
        }
    }
}
