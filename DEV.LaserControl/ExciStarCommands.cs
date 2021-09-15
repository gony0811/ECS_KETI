using INNO6.Core.Communication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DEV.LaserControl
{
    public class ExciStarCommands
    {
        public const string COMM_TIMEOUT = "TIMEOUT";
        public const string COMM_SUCCESS = "SUCCESS";
        public const string COMM_DISCONNECT = "DISCONNECT";
        public const string COMM_ERROR = "ERROR";
        public const string COMM_UNKNOWN = "UNKNOWN";
        public const char CR = '\r';
        private static object critical_section = new object();
        private XSerialComm xSerial;
        private long _milisecondResponseTimeout;
        private string _StatusCode;
        private string _buffer;


        private string SendMessageAndWaitForReply(string request, out string returnValue)
        {
            lock (critical_section)
            {
                xSerial.SendMessage(request);
                Stopwatch sw = new Stopwatch();
                sw.Start();

                while (true)
                {
                    if (sw.ElapsedMilliseconds >= _milisecondResponseTimeout)
                    {
                        returnValue = null;
                        return COMM_TIMEOUT;
                    }
                    else if (ReplyMessage(out string reply))
                    {
                        returnValue = reply;
                        return COMM_SUCCESS;
                    }
                    else
                    {
                        Thread.Sleep(10);
                        continue;
                    }
                }
            }
        }

        private bool ReplyMessage(out string reply)
        {
            reply = "";
            StringBuilder sb = new StringBuilder();

            if(xSerial.ReadBuffer(out string data))
            {
                _buffer += data;

                if (!string.IsNullOrEmpty(data) && data.Contains<char>('\r'))
                {
                    string[] strArr = data.Split(CR);
                }
            }

            return false;
        }

        private void SendMessage(byte[] command)
        {
            if (this.xSerial.IsOpen)
            {
                xSerial.SendMessage(command);
            }
            else
            {
                return;
            }
        }

        public ExciStarCommands(XSerialComm serialComm, long miliSecResonseTimeout)
        {
            xSerial = serialComm;
            _milisecondResponseTimeout = miliSecResonseTimeout;
            _StatusCode = StatusCode.NO_ERROR;
        }

        public bool SET_OPMODE(string OPMODE)
        {
            StringBuilder data = new StringBuilder();

            data.Append(string.Format("OPMODE={0}\r", OPMODE));

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.ACCEPT_COMMAND)) return true;
                else
                    throw new ApplicationException(string.Format("SET OPMODE Exception : Command={0} ReplyCode={1}", data.ToString(), response));
            }

            return false;
        }

        public bool GET_OPMODE(out string opMode, out string param, out string statusCode)
        {
            opMode = string.Empty;
            statusCode = string.Empty;
            param = string.Empty;
            char[] sep = { ',' };
            StringBuilder data = new StringBuilder();

            data.Append("OPMODE?\r");

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND))
                    throw new ApplicationException(string.Format("GET OPMODE Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));

                string reply = response.TrimStart('=');
                string[] arrReply = reply.Split(sep);

                if (arrReply.Length == 2)
                {
                    opMode = arrReply[0];
                    statusCode = arrReply[1];
                    return true;
                }
                else if (arrReply.Length == 3)
                {
                    opMode = arrReply[0];
                    param = arrReply[1];
                    statusCode = arrReply[2];
                    return true;
                }
            }

            return false;
        }

        public bool SET_MODE(string mode)
        {
            StringBuilder data = new StringBuilder();

            data.Append(string.Format("MODE={0}\r", mode));

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.ACCEPT_COMMAND)) return true;
                else
                    throw new ApplicationException(string.Format("SET MODE Exception : Command={0} ReplyCode={1}", data.ToString(), response));
            }

            return false;
        }

        public bool GET_MODE(out string mode)
        {
            StringBuilder data = new StringBuilder();

            data.Append("MODE?\r");

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND))
                    throw new ApplicationException(string.Format("GET MODE Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));

                string reply = response.TrimStart('=');

                if (string.IsNullOrEmpty(reply))
                {
                    mode = reply;
                    return true;
                }
            }

            mode = "UNKNOWN";
            return false;

        }

        public bool SET_EGY(double energy)
        {
            StringBuilder data = new StringBuilder();

            data.Append(string.Format("EGY={0}\r", String.Format(CultureInfo.InvariantCulture,
                                "{0:000.00}", energy)));

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.ACCEPT_COMMAND)) return true;
                else
                    throw new ApplicationException(string.Format("SET EGY Exception : Command={0} ReplyCode={1}", data.ToString(), response));
            }
            return false;
        }

        public bool GET_EGY(out double energy)
        {
            char[] sep = { '=' };
            StringBuilder data = new StringBuilder();
            energy = 0.0;
            data.Append("EGY?\r");

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND))
                    throw new ApplicationException(string.Format("GET EGY Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));


                string reply = response.TrimStart('=');

                if (!string.IsNullOrEmpty(reply) && double.TryParse(reply, out energy))
                {
                    return true;
                }
                else return false;
            }
            else
            {
                return false;
            }
        }

        public bool SET_EGYSET(double energy)
        {
            StringBuilder data = new StringBuilder();

            data.Append(string.Format("EGYSET={0}\r", String.Format(CultureInfo.InvariantCulture,
                                "{0:000.00}", energy)));

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.ACCEPT_COMMAND)) return true;
                else
                    throw new ApplicationException(string.Format("SET EGYSET Exception : Command={0} ReplyCode={1}", data.ToString(), response));
            }
            return false;
        }

        public bool GET_EGYSET(out double energy)
        {
            char[] sep = { '=' };
            StringBuilder data = new StringBuilder();
            energy = 0.0;
            data.Append("EGYSET?\r");

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND))
                    throw new ApplicationException(string.Format("GET EGYSET Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));

                string reply = response.TrimStart('=');

                if (!string.IsNullOrEmpty(reply) && double.TryParse(reply, out energy))
                {
                    return true;
                }
                else return false;
            }
            else
            {
                return false;
            }
        }

        public bool SET_HV(double hv)
        {
            StringBuilder data = new StringBuilder();

            data.Append(string.Format("HV={0}\r", String.Format(CultureInfo.InvariantCulture,
                                "{0:00.00}", hv)));

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.ACCEPT_COMMAND)) return true;
                else
                    throw new ApplicationException(string.Format("SET HV Exception : Command={0} ReplyCode={1}", data.ToString(), response));
            }
            return false;
        }

        public bool GET_HV(out double hv)
        {
            char[] sep = { '=' };
            StringBuilder data = new StringBuilder();
            hv = 0.0;
            data.Append("HV?\r");

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND))
                    throw new ApplicationException(string.Format("GET HV Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));

                string reply = response.TrimStart('=');

                if (!string.IsNullOrEmpty(reply) && double.TryParse(reply, out hv))
                {
                    return true;
                }
                else return false;
            }
            else
            {
                return false;
            }
        }
        public bool SET_REPRATE(int frequency)
        {
            StringBuilder data = new StringBuilder();

            data.Append(string.Format("REPRATE={0}\r", String.Format(CultureInfo.InvariantCulture,
                                "{0:0000}", frequency)));

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.ACCEPT_COMMAND)) return true;
                else { }
                    //throw new ApplicationException(string.Format("SET REPRATE Exception : Command={0} ReplyCode={1}", data.ToString(), response));
            }
            return false;
        }

        public bool GET_REPRATE(out int frequency)
        {
            StringBuilder data = new StringBuilder();
            frequency = 0;
            data.Append("REPRATE?\r");

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) { }
                    //throw new ApplicationException(string.Format("GET REPRATE Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));

                string reply = response.TrimStart('=');

                if (!string.IsNullOrEmpty(reply) && int.TryParse(reply, out frequency)) return true;
                else return false;
            }
            else
            {
                return false;
            }
        }

        public bool RESET_COUNTER()
        {
            StringBuilder data = new StringBuilder();

            data.Append("COUNTER=RESET\r");

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.ACCEPT_COMMAND)) return true;
                else { }
                    //throw new ApplicationException(string.Format("COUNTER RESET Exception : Command={0} ReplyCode={1}", data.ToString(), response));
            }
            return false;
        }

        public bool GET_COUNTER(out int shots)
        {
            shots = -1;
            StringBuilder data = new StringBuilder();
            data.Append("COUNTER?\r");

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) { }
                    //throw new ApplicationException(string.Format("GET COUNTER Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));


                string reply = response.TrimStart('=');

                if (!string.IsNullOrEmpty(reply) && int.TryParse(reply, out shots)) return true;
                else return false;
            }
            else
            {
                return false;
            }
        }

        public bool RESET_COUNTERMAINT()
        {
            StringBuilder data = new StringBuilder();

            data.Append("COUNTERMAINT.=RESET");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.ACCEPT_COMMAND)) return true;
                else { }
                    //throw new ApplicationException(string.Format("COUNTERMAINT RESET Exception : Command={0} ReplyCode={1}", data.ToString(), response));
            }
            return false;
        }

        public bool GET_COUNTERMAINT(out int shots)
        {
            char[] sep = { '=' };
            shots = -1;
            StringBuilder data = new StringBuilder();
            data.Append("COUNTERMAINT.?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) { }
                    //throw new ApplicationException(string.Format("GET COUNTERMAINT Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));


                string reply = response.TrimStart('=');

                if (!string.IsNullOrEmpty(reply) && int.TryParse(reply, out shots)) return true;
                else return false;
            }
            else
            {
                return false;
            }
        }

        public bool GET_COUNTERTOTAL(out int shots)
        {
            shots = -1;
            StringBuilder data = new StringBuilder();
            data.Append("COUNTER TOTAL?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) { }
                    //throw new ApplicationException(string.Format("GET COUNTERMAINT Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));

                string reply = response.TrimStart('=');

                if (!string.IsNullOrEmpty(reply) && int.TryParse(reply, out shots)) return true;
                else return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// COUNTER NEW FILL
        /// This parameter indicates the reading of a counter that counts the number of individual pulses that have been emitted since the excimer laser gas
        /// in the laser tube was last exchanged. 
        /// This counter is automatically reset to zero by the laser control software when OPMODE=NEW FILL is received.
        /// </summary>
        /// <param name="shots"></param>
        /// <returns></returns>
        public bool GET_COUNTERNEWFILL(out int shots)
        {
            shots = -1;
            StringBuilder data = new StringBuilder();
            data.Append("COUNTER NEW FILL?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) { }
                    //throw new ApplicationException(string.Format("GET COUNTERMAINT Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));

                string reply = response.TrimStart('=');

                if (!string.IsNullOrEmpty(reply) && int.TryParse(reply, out shots)) return true;
                else return false;
            }
            else
            {
                return false;
            }
        }


        public bool SET_TRIGGER(string trigger)
        {
            StringBuilder data = new StringBuilder();

            data.Append(string.Format("TRIGGER={0}", trigger));
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.ACCEPT_COMMAND)) return true;
                else { }
                    //throw new ApplicationException(string.Format("SET TRIGGER Exception : Command={0} ReplyCode={1}", data.ToString(), response));
            }

            return false;
        }

        public bool GET_TRIGGER(out string trigger)
        {
            char[] sep = { '=' };
            StringBuilder data = new StringBuilder();

            data.Append("TRIGGER?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) { }
                   // throw new ApplicationException(string.Format("GET TRIGGER Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));

                string reply = response.TrimStart('=');

                if (!string.IsNullOrEmpty(reply))
                {
                    trigger = reply;
                    return true;
                }

            }

            trigger = "UNKNOWN";
            return false;

        }

        public bool SET_BSTPULSES(int pulses)
        {
            StringBuilder data = new StringBuilder();

            data.Append(string.Format("BSTPULSES={0}", String.Format(CultureInfo.InvariantCulture,
                                "{0:00000}", pulses)));
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.ACCEPT_COMMAND)) return true;
                else { }
                    //throw new ApplicationException(string.Format("SET REPRATE Exception : Command={0} ReplyCode={1}", data.ToString(), response));
            }
            return false;
        }

        public bool GET_BSTPULSES(out int pulses)
        {
            char[] sep = { '=' };
            StringBuilder data = new StringBuilder();
            pulses = 0;
            data.Append("BSTPULSES?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) { }
                    //throw new ApplicationException(string.Format("GET REPRATE Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));

                string reply = response.TrimStart('=');

                if (!string.IsNullOrEmpty(reply) && int.TryParse(reply, out pulses)) return true;
                else return false;
            }
            else
            {
                return false;
            }
        }

        public bool SET_BSTPAUSE(int milliseconds)
        {
            StringBuilder data = new StringBuilder();

            data.Append(string.Format("BSTPAUSE={0}", String.Format(CultureInfo.InvariantCulture,
                                "{0:00000}", milliseconds)));
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.ACCEPT_COMMAND)) return true;
                else { }
                    //throw new ApplicationException(string.Format("SET BSTPAUSE Exception : Command={0} ReplyCode={1}", data.ToString(), response));
            }
            return false;
        }

        public bool GET_BSTPAUSE(out int milliseconds)
        {
            char[] sep = { '=' };
            StringBuilder data = new StringBuilder();
            milliseconds = 0;
            data.Append("BSTPAUSE?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) { }
                    //throw new ApplicationException(string.Format("GET BSTPAUSE Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));

                string reply = response.TrimStart('=');

                if (!string.IsNullOrEmpty(reply) && int.TryParse(reply, out milliseconds)) return true;
                else return false;
            }
            else
            {
                return false;
            }
        }

        public bool SET_SEQBST(int bursts)
        {
            StringBuilder data = new StringBuilder();

            data.Append(string.Format("SEQBST={0}", String.Format(CultureInfo.InvariantCulture,
                                "{0:00000}", bursts)));
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.ACCEPT_COMMAND)) return true;
                else { }
                    //throw new ApplicationException(string.Format("SET SEQBST Exception : Command={0} ReplyCode={1}", data.ToString(), response));
            }
            return false;
        }

        public bool GET_SEQBST(out int bursts)
        {
            StringBuilder data = new StringBuilder();
            bursts = 0;
            data.Append("SEQBST?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) { }
                    //throw new ApplicationException(string.Format("GET SEQBST Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));

                string reply = response.TrimStart('=');

                if (!string.IsNullOrEmpty(reply) && int.TryParse(reply, out bursts)) return true;
                else return false;
            }
            else
            {
                return false;
            }
        }

        public bool SET_SEQPAUSE(int milliseconds)
        {
            StringBuilder data = new StringBuilder();

            data.Append(string.Format("SEQPAUSE={0}", String.Format(CultureInfo.InvariantCulture,
                                "{0:00000}", milliseconds)));
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.ACCEPT_COMMAND)) return true;
                else { }
                    //throw new ApplicationException(string.Format("SET SEQPAUSE Exception : Command={0} ReplyCode={1}", data.ToString(), response));
            }
            return false;
        }

        public bool GET_SEQPAUSE(out int milliseconds)
        {
            StringBuilder data = new StringBuilder();
            milliseconds = 0;
            data.Append("SEQPAUSE?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) { }
                    //throw new ApplicationException(string.Format("GET SEQPAUSE Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));

                string reply = response.TrimStart('=');

                if (!string.IsNullOrEmpty(reply) && int.TryParse(reply, out milliseconds)) return true;
                else return false;
            }
            else
            {
                return false;
            }
        }

        public bool SET_COUNTS(int pulses)
        {
            StringBuilder data = new StringBuilder();

            data.Append(string.Format("COUNTS={0}", String.Format(CultureInfo.InvariantCulture,
                                "{0:0000000}", pulses)));
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.ACCEPT_COMMAND)) return true;
                else { }
                    //throw new ApplicationException(string.Format("SET COUNTS Exception : Command={0} ReplyCode={1}", data.ToString(), response));
            }
            return false;
        }

        public bool GET_COUNTS(out int pulses)
        {
            StringBuilder data = new StringBuilder();
            pulses = 0;
            data.Append("COUNTS?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) { }
                    //throw new ApplicationException(string.Format("GET COUNTS Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));

                string reply = response.TrimStart('=');

                if (!string.IsNullOrEmpty(reply) && int.TryParse(reply, out pulses)) return true;
                else return false;
            }
            else
            {
                return false;
            }
        }


        public bool SET_FILTER(int second_1of10)
        {
            StringBuilder data = new StringBuilder();

            data.Append(string.Format("FILTER={0}", String.Format(CultureInfo.InvariantCulture,
                                "{0:00}", second_1of10)));
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.ACCEPT_COMMAND)) return true;
                else { }
                    //throw new ApplicationException(string.Format("SET FILTER Exception : Command={0} ReplyCode={1}", data.ToString(), response));
            }
            return false;
        }

        public bool GET_FILTER(out int second_1of10)
        {
            StringBuilder data = new StringBuilder();
            second_1of10 = 0;
            data.Append("FILTER?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) { }
                    //throw new ApplicationException(string.Format("GET FILTER Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));

                string reply = response.TrimStart('=');

                if (!string.IsNullOrEmpty(reply) && int.TryParse(reply, out second_1of10)) return true;
                else return false;
            }
            else
            {
                return false;
            }
        }

        public bool RESET_FILTERCONTAMINATION()
        {
            StringBuilder data = new StringBuilder();

            data.Append("FILTERCONTAMINATION=RESET");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.ACCEPT_COMMAND)) return true;
                else { }
                   // throw new ApplicationException(string.Format("FILTERCONTAMINATION RESET Exception : Command={0} ReplyCode={1}", data.ToString(), response));
            }
            return false;
        }

        public bool GET_FILTERCONTAMINATION(out int percent)
        {
            char[] sep = { '=' };
            percent = -1;
            StringBuilder data = new StringBuilder();
            data.Append("FILTERCONTAMINATION?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) { }
                    //throw new ApplicationException(string.Format("GET FILTERCONTAMINATION Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));

                string reply = response.TrimStart('=');

                if (!string.IsNullOrEmpty(reply) && int.TryParse(reply, out percent)) return true;
                else return false;
            }
            else
            {
                return false;

            }
        }

        public bool GET_INTERLOCK(out string interlocks)
        {
            interlocks = string.Empty;
            StringBuilder data = new StringBuilder();
            data.Append("INTERLOCK?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) { }
                   // throw new ApplicationException(string.Format("GET INTERLOCK Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));


                string reply = response.TrimStart('=');

                if (reply != null)
                {
                    interlocks = reply;
                    return true;
                }
                else return false;
            }
            else
            {
                return false;

            }
        }


        public bool GET_MAINTENANCE(out string maintCodes)
        {
            char[] sep = { '=', ',' };
            maintCodes = string.Empty;
            StringBuilder data = new StringBuilder();
            data.Append("MAINTENANCE?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) { }
                    //throw new ApplicationException(string.Format("GET MAINTENANCE Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));

                string reply = response.TrimStart('=');

                if (reply != null)
                {
                    maintCodes = reply;
                    return true;
                }
                else return false;
            }
            else
            {
                return false;

            }
        }

        public bool GET_TUBETEMP(out double temperature)
        {
            char[] sep = { '=', ',' };
            temperature = 0.0;
            StringBuilder data = new StringBuilder();
            data.Append("TUBETEMP?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) { }
                    //throw new ApplicationException(string.Format("GET TUBETEMP Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));

                response = response.TrimStart('=');

                if (double.TryParse(response, out temperature))
                    return true;
                else 
                    return false;
            }
            else
            {
                return false;

            }
        }

        public bool GET_PRESSURE(out int mbar)
        {
            char[] sep = { '=', ',' };
            mbar = 0;
            StringBuilder data = new StringBuilder();
            data.Append("PRESSURE?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) { }
                    //throw new ApplicationException(string.Format("GET PRESSURE Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));

                response = response.TrimStart('=');

                if (int.TryParse(response, out mbar))
                    return true;
                else
                    return false;
            }
            else
            {
                return false;

            }
        }

        public bool GET_MANPRESS(out int mbar)
        {
            char[] sep = { '=', ',' };
            mbar = 0;
            StringBuilder data = new StringBuilder();
            data.Append("MANPRESS?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) { }
                    //throw new ApplicationException(string.Format("GET MANPRESS Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));

                response = response.TrimStart('=');

                if (int.TryParse(response, out mbar))
                    return true;
                else
                    return false;
            }
            else
            {
                return false;

            }
        }

    }
}
