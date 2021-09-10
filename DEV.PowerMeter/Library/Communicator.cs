using SharedLibrary;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace DEV.PowerMeter.Library
{
    public class Communicator
    {
        private IErrorReporter _ErrorReporter;
        private object CommLock = new object();
        private const int SkipBufferSize = 1200;
        public static DataSkipBuffer CurrentSkipBuffer = new DataSkipBuffer();

        public static IErrorReporter DefaultErrorReporter;// => ServiceLocator.Resolve<IErrorReporter>();

        public IErrorReporter ErrorReporter
        {
            set => this._ErrorReporter = value;
            get => this._ErrorReporter ?? (this._ErrorReporter = Communicator.DefaultErrorReporter);
        }

        public Channel Channel { get; set; }

        public string PortName => this.Channel?.PortName;

        public bool IsOpen => this.Channel != null && this.Channel.IsOpen;

        public string PreviousCommand { get; protected set; }

        public string NextPreviousCommand { get; protected set; }

        public Communicator()
        {
        }

        public Communicator(Channel channel) => this.Open(channel);

        public override string ToString() => string.Format("Communicator on {0}", (object)this.PortName);

        public void Open(Channel channel)
        {
            if (this.IsOpen)
                this.Close();
            lock (this.CommLock)
            {
                this.Channel = channel;
                this.WasStarted = false;
            }
        }

        public void Close()
        {
            lock (this.CommLock)
            {
                try
                {
                    if (this.Channel != null)
                        this.Channel.Close();
                }
                catch (Exception ex)
                {
                }
            }
            this.WasStarted = false;
        }

        public void LockAndDelegate(Action action)
        {
            lock (this.CommLock)
                action();
        }

        public bool WasStarted { get; protected set; }

        public void ClearWasStarted() => this.WasStarted = false;

        public void ThrowIfRunning(string command)
        {
            if (this.WasStarted)
                throw new CommunicationIsRunningException(command);
        }

        public void SendStartCommand(string command)
        {
            lock (this.CommLock)
            {
                this.SendCommandWhileRunning(command);
                this.WasStarted = true;
            }
        }

        public void SendStopCommand(string command)
        {
            lock (this.CommLock)
            {
                this.SendCommandWhileRunning(command);
                this.SkipUnusedData();
                this.Channel.FlushInput();
                this.WasStarted = false;
            }
        }

        public void SendAndReceiveCommand(string command, params object[] args)
        {
            lock (this.CommLock)
            {
                this.SendCommand(command, args);
                this.RequireOK();
            }
        }

        public string SendAndReceiveQuery(string command, params object[] args) => this.SendAndReceiveQueryNoSuffix(command + "?", args);

        public string SendAndReceiveQueryNoSuffix(string command, params object[] args)
        {
            lock (this.CommLock)
            {
                this.SendCommand(command, args);
                return this.ReceiveQuery();
            }
        }

        public string SendQueryAndReceiveOneLine(string command, params object[] args) => this.SendCommandAndReceiveOneLine(command + "?", args);

        public string SendCommandAndReceiveOneLine(string command, params object[] args)
        {
            lock (this.CommLock)
            {
                this.SendCommand(command, args);
                return this.Channel.ReadLine();
            }
        }

        protected string FormatCommand(string command, params object[] args)
        {
            string str = command + (args.Length != 0 ? " " + string.Join(" ", args) : "");
            this.NextPreviousCommand = this.PreviousCommand;
            string hhMmSs = DateTime.Now.ToHhMmSs();
            this.PreviousCommand = str + " #From " + (Thread.CurrentThread?.Name ?? "[Unk]") + " @" + hhMmSs;
            return str;
        }

        protected void SendCommand(string command, params object[] args)
        {
            this.ThrowIfRunning(command);
            this.Channel.WriteLine(this.FormatCommand(command, args));
        }

        public void SendCommandWhileRunning(string command, params object[] args)
        {
            lock (this.CommLock)
                this.SendCommandNoFlush(command, args);
        }

        protected void SendCommandNoFlush(string command, params object[] args) => this.Channel.WriteLineNoFlush(this.FormatCommand(command, args));

        public void SendCommandNoWaitReply(string command, params object[] args)
        {
            lock (this.CommLock)
                this.SendCommand(command, args);
        }

        public void SendCommandWaitTimeout(string command, params object[] args)
        {
            lock (this.CommLock)
            {
                this.SendCommand(command, args);
                this.ReceiveUntilTimeout();
            }
        }

        protected string ReceiveQuery()
        {
            string line = this.ReceiveLine();
            if (line != null && !line.IsErr())
                this.RequireOK(line);
            return line;
        }

        protected string ReceiveLine()
        {
            string s;
            try
            {
                s = this.Channel.ReadLine();
            }
            catch (TimeoutException ex)
            {
                throw;
            }
            if (s == null)
                this.ReportCommError("ReceiveLine: EOF on " + this.PortName);
            else if (s.IsErr())
                this.ReportCommError("ReceiveLine Error: " + s);
            return s;
        }

        protected void RequireOK(string query = null)
        {
            try
            {
                string line = this.ReceiveLine();
                if (line.IsErr() || line != null && line.IsOK())
                    return;
                this.ReportCommError("RequireOK missing OK: " + line);
            }
            catch (TimeoutException ex)
            {
                this.ReportCommError(query == null ? "RequireOK timeout" : "RequireOK timeout (query=" + query + ")");
                throw;
            }
        }

        protected void ReceiveUntilTimeout()
        {
            while (true)
            {
                try
                {
                    if (this.Channel.ReadLine() == null)
                    {
                        TraceLogger.TraceRead("ReceiveUntilTimeout: unexpected EOF on Communicator");
                        break;
                    }
                }
                catch (TimeoutException ex)
                {
                    break;
                }
                catch (Exception ex)
                {
                    TraceLogger.TraceRead("ReceiveUntilTimeout unexpected Exception: " + ex.Message);
                }
            }
        }

        public void ReportCommError(string error)
        {
            string hhMmSs = DateTime.Now.ToHhMmSs();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Communications Error " + hhMmSs + ": " + error);
            stringBuilder.AppendLine("  Command: " + this.PreviousCommand);
            string str = stringBuilder.ToString().Trim();
            if (this.ErrorReporter == null)
                throw new CommunicationException(str);
            this.ErrorReporter.ReportError(str);
        }

        protected void SkipUnusedData()
        {
            byte[] numArray = new byte[1200];
            Communicator.CurrentSkipBuffer = new DataSkipBuffer();
            while (true)
            {
                try
                {
                    int count = this.Channel.Read(numArray);
                    if (count <= 0)
                    {
                        this.TraceSkipUnusedData("SkipUnusedData: ended w/0 bytes read, {0} bytes total", (object)Communicator.CurrentSkipBuffer.Count);
                        break;
                    }
                    this.TraceSkipUnusedData(numArray, count);
                    Communicator.CurrentSkipBuffer.Add(numArray, count);
                }
                catch (TimeoutException ex)
                {
                    TraceLogger.TraceRead("TimeoutException, as expected, {0} bytes", (object)Communicator.CurrentSkipBuffer.Count);
                    break;
                }
                catch (Exception ex)
                {
                    this.TraceSkipUnusedData("SkipUnusedData ignoring unexpected Exception: " + ex.Message);
                }
            }
        }

        [Conditional("TRACE_SKIP_UNUSED_DATA")]
        public void TraceSkipUnusedData(string format, params object[] args) => TraceLogger.Trace(format, args);

        [Conditional("TRACE_SKIP_UNUSED_DATA")]
        public void TraceSkipUnusedData(byte[] SkipBuffer, int count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < count; ++index)
                stringBuilder.AppendFormat("{0:x2} ", (object)SkipBuffer[index]);
            this.TraceSkipUnusedData("SkipUnusedData[{0}]: {1}", (object)count, (object)stringBuilder.ToString());
        }
    }
}
