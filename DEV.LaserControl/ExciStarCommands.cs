using INNO6.Core;
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
        public static object key = new object();
        private XSerialComm xSerial;
        private long _milisecondResponseTimeout;
        private string _StatusCode;

        private string SendMessageAndWaitForReply(string request, out string returnValue)
        {
            lock (key)
            {
                LogHelper.Instance.DeviceLog.DebugFormat("Send Command : {0}", request);
                
                Stopwatch sw = new Stopwatch();
                xSerial.SendMessage(request);             
                sw.Start();

                while (true)
                {
                    Thread.SpinWait(10);
                    if (sw.ElapsedMilliseconds >= _milisecondResponseTimeout)
                    {
                        returnValue = null;
                        return COMM_TIMEOUT;
                    }
                    else if (ReplyMessage(out string reply))
                    {
                        returnValue = reply.TrimEnd(CR);
                        return COMM_SUCCESS;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        private bool ReplyMessage(out string reply)
        {
            lock (key)
            {
                reply = "";
                StringBuilder sb = new StringBuilder();
                if (xSerial.ReadBuffer(out string data))
                {
                    LogHelper.Instance.DeviceLog.DebugFormat("Received Message : {0}\n", data);
                    if (!string.IsNullOrEmpty(data) && data.Contains<char>('\r'))
                    {
                        reply = data;
                        return true;
                    }
                }


                return false;
            }
        }

        private void SendMessage(byte[] command)
        {
            if (this.xSerial.IsOpen)
            {
                xSerial.FlushBuffer();
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
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] SET OPMODE Error : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }
            }

            return false;
        }

        public bool GET_OPMODE(out string opMode, out string param, out string statusCode)
        {
            opMode = string.Empty;
            statusCode = string.Empty;
            param = string.Empty;
            char[] sep1 = { '=' };
            char[] sep2 = { ',' };
            StringBuilder data = new StringBuilder();

            data.Append("OPMODE?\r");

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND))
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET OPMODE Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }
                else if (!response.StartsWith("OPMODE")) 
                    return false;

                string[] reply = response.Split(sep1);
                string[] arrReply = reply[1].Split(sep2);

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
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("SET MODE Exception : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }
            }

            return false;
        }

        public bool GET_MODE(out string mode)
        {
            mode = "";
            char[] sep = { '=' };
            StringBuilder data = new StringBuilder();

            data.Append("MODE?\r");

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND))
                {
                    throw new ApplicationException(string.Format("GET MODE Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));
                }
                else if(!response.StartsWith("MODE"))
                {
                    return false;
                }

                string[] reply = response.Split(sep);

                if (reply != null && reply.Length > 0)
                {
                    mode = reply[1];
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
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("SET EGY Exception : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }
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
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET EGY Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }

                string[] reply = response.Split(sep);

                if (reply != null && reply.Length > 0 && double.TryParse(reply[1], out energy))
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
                                "{0:0.00}", energy)));

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.ACCEPT_COMMAND)) return true;
                else
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] SET EGYSET Error : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }
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
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET EGYSET Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }
                else if(!response.StartsWith("EGYSET"))
                {
                    return false;
                }

                string[] reply = response.Split(sep);

                if (reply != null && reply.Length > 0 && double.TryParse(reply[1], out energy))
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
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] SET HV Error : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }
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
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET HV Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }
                else if (!response.StartsWith("HV"))
                {
                    return false;
                }

                string[] reply = response.Split(sep);

                if (reply != null && reply.Length > 0 && double.TryParse(reply[1], out hv))
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
                else
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] SET REPRATE Exception : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }
            }
            return false;
        }

        public bool GET_REPRATE(out int frequency)
        {
            char[] sep = { '=' };
            StringBuilder data = new StringBuilder();
            frequency = 0;
            data.Append("REPRATE?\r");

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) 
                { 
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET REPRATE Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }
                
                string[] reply = response.Split(sep);

                if (reply != null && reply.Length > 1 && int.TryParse(reply[1], out frequency))
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

        public bool RESET_COUNTER()
        {
            StringBuilder data = new StringBuilder();

            data.Append("COUNTER=RESET\r");

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.ACCEPT_COMMAND)) return true;
                else 
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] COUNTER RESET : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }                
            }
            return false;
        }

        public bool GET_COUNTER(out int shots)
        {
            shots = -1;
            char[] sep = { '=' };
            StringBuilder data = new StringBuilder();
            data.Append("COUNTER?\r");

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) 
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET COUNTER Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }
                else if (!response.StartsWith("COUNTER"))
                {
                    return false;
                }

                string[] reply = response.Split(sep);

                if (reply != null && reply.Length > 1 && int.TryParse(reply[1], out shots)) return true;
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
                else 
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] COUNTERMAINT RESET : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }                 
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
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) 
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET COUNTERMAINT Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }                 

                string[] reply = response.Split(sep);

                if (reply != null && reply.Length > 1 && int.TryParse(reply[1], out shots)) return true;
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
            char[] sep = { '=' };
            StringBuilder data = new StringBuilder();
            data.Append("COUNTER TOTAL?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) 
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET COUNTERMAINT Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }

                string[] reply = response.Split(sep);

                if ( reply != null && reply.Length > 1 && int.TryParse(reply[1], out shots)) return true;
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
            char[] sep = { '=' };
            StringBuilder data = new StringBuilder();
            data.Append("COUNTER NEW FILL?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND))
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET COUNTERMAINT Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }
                else if (!response.StartsWith("COUNTER"))
                {
                    return false;
                }
                    
                string[] reply = response.Split(sep);

                if (reply != null && reply.Length > 1 && int.TryParse(reply[1], out shots)) return true;
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
                else
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] SET TRIGGER Error : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }

            }
            return false;
        }

        public bool GET_TRIGGER(out string trigger)
        {
            char[] sep = { '=' };
            StringBuilder data = new StringBuilder();
            trigger = "UNKNOWN";
            data.Append("TRIGGER?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) 
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET TRIGGER Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                } 
                else if (!response.StartsWith("TRIGGER"))
                {
                    return false;
                }

                string[] reply = response.Split(sep);

                if ( reply != null && reply.Length > 0)
                {
                    trigger = reply[1];
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
                else 
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] SET REPRATE Error : Command={0} ReplyCode={1}", data.ToString(), response);
                }                
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
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET REPRATE Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }
                  
                string[] reply = response.Split(sep);

                if ( reply != null && reply.Length > 1 && int.TryParse(reply[1], out pulses)) return true;
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
                else 
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] SET BSTPAUSE : Command={0} ReplyCode={1}", data.ToString(), response);
                }                
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
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) 
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET BSTPAUSE Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }
                    
                string[] reply = response.Split(sep);

                if ( reply != null && reply.Length > 1 && int.TryParse(reply[1], out milliseconds)) return true;
                else return false;
            }
            else
            {
                return false;
            }
        }

        public bool SET_SEQBST(int bursts)
        {
            char[] sep = { '=' };
            StringBuilder data = new StringBuilder();

            data.Append(string.Format("SEQBST={0}", String.Format(CultureInfo.InvariantCulture,
                                "{0:00000}", bursts)));
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.ACCEPT_COMMAND)) return true;
                else 
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] SET SEQBST Exception : Command={0} ReplyCode={1}", data.ToString(), response);
                }
            }
            return false;
        }

        public bool GET_SEQBST(out int bursts)
        {
            char[] sep = { '=' };
            StringBuilder data = new StringBuilder();
            bursts = 0;
            data.Append("SEQBST?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) 
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET SEQBST Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                }

                string[] reply = response.Split(sep);

                if ( reply != null && reply.Length > 1 && int.TryParse(reply[1], out bursts)) return true;
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
                else
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] SET SEQPAUSE Error : Command={0} ReplyCode={1}", data.ToString(), response);
                }
            }
            return false;
        }

        public bool GET_SEQPAUSE(out int milliseconds)
        {
            char[] sep = { '=' };
            StringBuilder data = new StringBuilder();
            milliseconds = 0;
            data.Append("SEQPAUSE?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND))
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET SEQPAUSE Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }

                string[] reply = response.Split(sep);

                if (reply != null && reply.Length > 1 && int.TryParse(reply[1], out milliseconds)) return true;
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
                else
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] SET COUNTS Error : Command={0} ReplyCode={1}", data.ToString(), response);
                }                 
            }
            return false;
        }

        public bool GET_COUNTS(out int pulses)
        {
            char[] sep = { '=' };
            StringBuilder data = new StringBuilder();
            pulses = 0;
            data.Append("COUNTS?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND))
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET COUNTS Error : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }
                else if (!response.StartsWith("COUNTS"))
                {
                    return false;
                }

                string[] reply = response.Split(sep);

                if (reply != null && reply.Length > 1 && int.TryParse(reply[1], out pulses)) return true;
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
                else
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] SET FILTER Exception : Command={0} ReplyCode={1}", data.ToString(), response);
                }
            }
            return false;
        }

        public bool GET_FILTER(out int second_1of10)
        {
            char[] sep = { '=' };
            StringBuilder data = new StringBuilder();
            second_1of10 = 0;
            data.Append("FILTER?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) 
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET FILTER Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                    return false;
                }
                else if (!response.StartsWith("FILTER"))
                {
                    return false;
                }

                string[] reply = response.Split(sep);

                if ( reply != null && reply.Length > 1 && int.TryParse(reply[1], out second_1of10)) return true;
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
                else 
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] FILTERCONTAMINATION RESET : Command={0} ReplyCode={1}", data.ToString(), response);
                }              
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
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND))
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET FILTERCONTAMINATION Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                }

                string[] reply = response.Split(sep);

                if (reply != null && reply.Length > 1 && int.TryParse(reply[1], out percent)) return true;
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
            char[] sep = { '=' };
            StringBuilder data = new StringBuilder();
            data.Append("INTERLOCK?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) 
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("GET INTERLOCK Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                }
                else if(!response.StartsWith("INTERLOCK"))
                {
                    return false;
                }


                string[] reply = response.Split(sep);

                if (reply != null && reply.Length > 0)
                {
                    interlocks = reply[1];
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
            char[] sep = { '=' };
            maintCodes = string.Empty;
            StringBuilder data = new StringBuilder();
            data.Append("MAINTENANCE?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND))
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET MAINTENANCE Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                }
                else if(!response.StartsWith("MAINTENANCE"))
                {
                    return false;
                }

                string[] reply = response.Split(sep);

                if (reply != null && reply.Length > 0)
                {
                    maintCodes = reply[1];
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
            char[] sep = { '=' };
            temperature = 0.0;
            StringBuilder data = new StringBuilder();
            data.Append("TUBETEMP?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) 
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET TUBETEMP Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                }
                else if(!response.StartsWith("TUBETEMP"))
                {
                    return false;
                }
                    //throw new ApplicationException(string.Format("GET TUBETEMP Exception Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response));

                string[] reply = response.Split(sep);

                

                if (reply != null && reply.Length > 0 && double.TryParse(reply[1], out temperature))
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
            char[] sep = { '=' };
            mbar = 0;
            StringBuilder data = new StringBuilder();
            data.Append("PRESSURE?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) 
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET PRESSURE Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                }
                else if (!response.StartsWith("PRESSURE"))
                {
                    return false;
                }


                string[] reply = response.Split(sep);

                if (reply != null && reply.Length > 1 && int.TryParse(reply[1], out mbar))
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
            char[] sep = { '=' };
            mbar = 0;
            StringBuilder data = new StringBuilder();
            data.Append("MANPRESS?");
            data.Append(CR);

            if (SendMessageAndWaitForReply(data.ToString(), out string response) == COMM_SUCCESS)
            {
                if (response.Equals(ReplyCodes.UNKNOWN_COMMAND)) 
                {
                    LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] GET MANPRESS Wrong Command : Command={0} ReplyCode={1}", data.ToString(), response);
                }
                else if (!response.StartsWith("MANPRESS"))
                {
                    return false;
                }

                string[] reply = response.Split(sep);

                if ( reply != null && reply.Length > 1 && int.TryParse(reply[1], out mbar))
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
