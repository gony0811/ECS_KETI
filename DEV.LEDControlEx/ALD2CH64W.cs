using INNO6.Core;
using INNO6.Core.Communication;
using INNO6.IO.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DEV.LEDControlEx
{
    public class ALD2CH64W : IDeviceHandler
    {
        enum IDX_CHANNEL
        {
            CH1 = 7,
            CH2 = 6,
            CH3 = 5,
            CH4 = 4,
            CH5 = 3,
            CH6 = 2,
            CH7 = 1,
            CH8 = 0
        };

        #region Define
        public const string COMM_TIMEOUT = "TIMEOUT";
        public const string COMM_SUCCESS = "SUCCESS";
        public const string COMM_DISCONNECT = "DISCONNECT";
        public const string COMM_ERROR = "ERROR";
        public const string COMM_UNKNOWN = "UNKNOWN";

        public const int ERROR_DATA_OUTPUT = -1;


        // ID Define
        public const string ID_1_VIRTUAL = "0";
        public const string ID_1_INPUT = "1";
        public const string ID_1_OUTPUT = "2";

        public const string ID_2_OBJECT = "0";
        public const string ID_2_DOUBLE = "1";
        public const string ID_2_INT = "2";
        public const string ID_2_STRING = "3";

        public const string ID_3_UNKNOWN = "0";
        public const string ID_3_DATA = "1";
        public const string ID_3_STATUS = "2";
        public const string ID_3_BOTH = "3";

        public const string ID_4_CHALL = "0";
        public const string ID_4_CH1 = "1";
        public const string ID_4_CH2 = "2";
        public const string ID_4_CH3 = "3";
        public const string ID_4_CH4 = "4";
        #endregion

        private XSerialComm xSerial;
        private string _deviceName;
        private eDevMode _deviceMode;
        private long _milisecondResponseTimeout;
        private bool isDeviceLogging = true;
        private static object _CriticalSectionKey = new object();

        public ALD2CH64W()
        {
            _deviceName = "";
            _deviceMode = eDevMode.UNKNOWN;
            _milisecondResponseTimeout = 10000;
          
        }

        public bool DeviceAttach(string deviceName, string portName, string baudRate, string parity, string dataBits, string stopBits, string scanTime, string responsTimeout, string logging = "Y", string mode = "NORMAL")
        {
            this._deviceName = deviceName;
            _milisecondResponseTimeout = int.Parse(responsTimeout);

            if (!string.IsNullOrEmpty(logging) && logging.Substring(0, 1).Equals("Y"))
            {
                isDeviceLogging = true;
            }
            else
            {
                isDeviceLogging = false;
            }

            xSerial = new XSerialComm(portName, int.Parse(baudRate), (Parity)int.Parse(parity), int.Parse(dataBits), (StopBits)int.Parse(stopBits));

            if (xSerial == null)
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] DeviceAttach() : DeviceName = {0}, DeviceMode = {1}, Cause = {2}", _deviceName, _deviceMode.ToString(), portName + " Can not create SerialPort object!");
                _deviceMode = eDevMode.ERROR;
                return false;
            }
            else
            {
                if (_deviceMode == eDevMode.SIMULATE)
                {
                    return true;
                }
                else
                {

                    Task.Run(() =>
                    {
                        while (!xSerial.IsOpen)
                        {
                            xSerial.Open();

                            Thread.Sleep(5000);
                        }

                        if (xSerial.IsOpen)
                        {
                            _deviceMode = eDevMode.CONNECT;
                        }
                        else
                        {
                            LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] DeviceAttach() : DeviceName = {0}, DeviceMode = {1}, Cause = {2}", _deviceName, _deviceMode.ToString(), portName + " Can Not Open !");
                            _deviceMode = eDevMode.DISCONNECT;
                        }

                    });

                    return true;

                }
            }
        }

        public bool DeviceDettach()
        {
            if (xSerial != null && xSerial.IsOpen)
            {
                xSerial.Close();
                _deviceMode = eDevMode.DISCONNECT;
                return true;
            }
            else if (_deviceMode == eDevMode.SIMULATE)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeviceInit()
        {
            return true;
        }

        public bool DeviceReset()
        {
            throw new NotImplementedException();
        }

        public object GET_DATA_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            throw new NotImplementedException();
        }

        public double GET_DOUBLE_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// CHANNEL 1 OUTPUT DATA : id1 = '1', id2 = '2', id3 = '1', id4 = '1'
        /// CHANNEL 2 OUTPUT DATA : id1 = '1', id2 = '2', id3 = '1', id4 = '2'
        /// CHANNEL 3 OUTPUT DATA : id1 = '1', id2 = '2', id3 = '1', id4 = '3'
        /// CHANNEL 4 OUTPUT DATA : id1 = '1', id2 = '2', id3 = '1', id4 = '4'
        /// 
        /// CHANNEL 1 ON/OFF STATUS : id1 = '1', id2 = '2', id3 = '2', id4 = '1'
        /// CHANNEL 2 ON/OFF STATUS : id1 = '1', id2 = '2', id3 = '2', id4 = '2'
        /// CHANNEL 3 ON/OFF STATUS : id1 = '1', id2 = '2', id3 = '2', id4 = '3'
        /// CHANNEL 4 ON/OFF STATUS : id1 = '1', id2 = '2', id3 = '2', id4 = '4'
        /// 
        /// </summary>
        /// <param name="id_1"></param>
        /// <param name="id_2"></param>
        /// <param name="id_3"></param>
        /// <param name="id_4"></param>
        /// <param name="result"></param>
        /// <returns></returns>

        public int GET_INT_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            if (id_1.Equals(ID_1_INPUT) && id_2.Equals(ID_2_INT) && id_3.Equals(ID_3_DATA))
            {
                if (id_4.Equals(ID_4_CH1))
                {
                    //channel 1 led output data 0~255
                    if (RequestOutputData('1', out int outdata) == COMM_SUCCESS)
                    {
                        result = true;
                        return outdata;
                    }
                }
                else if (id_4.Equals(ID_4_CH2))
                {
                    //channel 2 led output data 0~255
                    if (RequestOutputData('2', out int outdata) == COMM_SUCCESS)
                    {
                        result = true;
                        return outdata;
                    }
                }
                else if (id_4.Equals(ID_4_CH3))
                {
                    //channel 3 led output data 0~255
                    if (RequestOutputData('3', out int outdata) == COMM_SUCCESS)
                    {
                        result = true;
                        return outdata;
                    }
                }
                else if (id_4.Equals(ID_4_CH4))
                {
                    //channel 4 led output data 0~255
                    if (RequestOutputData('4', out int outdata) == COMM_SUCCESS)
                    {
                        result = true;
                        return outdata;
                    }
                }
            }
            else if (id_1.Equals(ID_1_INPUT) && id_2.Equals(ID_2_INT) && id_3.Equals(ID_3_STATUS))
            {
                if (id_4.Equals(ID_4_CH1))
                {
                    if (RequestTurnOnStatusAll(out bool[] status) == COMM_SUCCESS)
                    {
                        result = true;

                        if (status[(int)IDX_CHANNEL.CH1] == true)
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
            }

            result = false;
            return -1;
        }

        private string RequestTurnOnStatusAll(out bool[] status)
        {
            status = new bool[8];

            List<byte> requestCommand = new List<byte>();

            requestCommand.Add(Convert.ToByte('R'));
            requestCommand.Add(Convert.ToByte('N'));
            requestCommand.Add(Convert.ToByte('F')); // Request control channel data

            string result = GetCommandEx(requestCommand.ToArray(), out byte[] response);

            if (result != COMM_TIMEOUT && result != COMM_ERROR)
            {
                if (response[0] == 'O' && response[1] == 'N')
                {
                    status = AppUtil.ConvertByteToBoolArray(response[2]);

                    return COMM_SUCCESS;
                }
            }

            return result;
        }



        private string RequestOutputData(char channel, out int outputData)
        {
            outputData = ERROR_DATA_OUTPUT;

            if (_deviceMode != eDevMode.CONNECT && _deviceMode != eDevMode.SIMULATE)
            {
                return COMM_DISCONNECT;
            }

            List<byte> requestCommand = new List<byte>();

            requestCommand.Add(Convert.ToByte('R'));
            requestCommand.Add(Convert.ToByte(channel));
            requestCommand.Add(Convert.ToByte('D')); // Request control channel data

            string result = GetCommandEx(requestCommand.ToArray(), out byte[] response);

            if (result != COMM_TIMEOUT && result != COMM_ERROR)
            {
                // request channel is Normal or Error
                if (response[0] == 'R')
                {
                    if (channel == response[1])
                    {
                        outputData = response[2];
                        return COMM_SUCCESS;
                    }
                    else
                    {
                        LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] Wrong recieved data packet from the ALD4CH64W : Channel = {0}", channel);
                        return COMM_ERROR;
                    }
                }
                else
                {
                    return COMM_ERROR;
                }
            }

            return COMM_ERROR;
        }

        private string GetCommandEx(byte[] request, out byte[] response)
        {
            if (_deviceMode == eDevMode.CONNECT)
            {
                return GetCommand(request, out response);
            }
            else
            {
                response = null;
                return COMM_ERROR;
            }
        }

        private string GetCommand(byte[] request, out byte[] returnValue)
        {
            lock (_CriticalSectionKey)
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
                    else if (xSerial.ReadBuffer(out string response))
                    {
                        returnValue = Encoding.ASCII.GetBytes(response);
                        return COMM_SUCCESS;
                    }
                    else
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                }
            }
        }

        public string GET_STRING_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            throw new NotImplementedException();
        }

        public eDevMode IsDevMode()
        {
            return _deviceMode;
        }

        public void SET_DATA_OUT(string id_1, string id_2, string id_3, string id_4, object value, ref bool result)
        {
            throw new NotImplementedException();
        }

        public void SET_DOUBLE_OUT(string id_1, string id_2, string id_3, string id_4, double value, ref bool result)
        {
            throw new NotImplementedException();
        }

        public void SET_INT_OUT(string id_1, string id_2, string id_3, string id_4, int value, ref bool result)
        {
            if (id_1.Equals(ID_1_OUTPUT) && id_2.Equals(ID_2_INT) && id_3.Equals(ID_3_DATA))
            {
                if (id_4.Equals(ID_4_CHALL))
                {
                    OutputDataChange(Convert.ToChar(ID_4_CHALL), BitConverter.GetBytes(value)[0]);
                    result = true;
                }
                else if (id_4.Equals(ID_4_CH1))
                {
                    OutputDataChange(Convert.ToChar(ID_4_CH1), BitConverter.GetBytes(value)[0]);
                    result = true;
                }
            }
            else if(id_1.Equals(ID_1_OUTPUT) && id_2.Equals(ID_2_INT) && id_3.Equals(ID_3_STATUS))
            {
                if (id_4.Equals(ID_4_CHALL))
                {
                    
                    result = true;
                }
                else if (id_4.Equals(ID_4_CH1))
                {
                    LedTurnOnOffByChannelNumber(IDX_CHANNEL.CH1, value == 1);
                    result = true;
                }
            }
        }

        public void SET_STRING_OUT(string id_1, string id_2, string id_3, string id_4, string value, ref bool result)
        {
            throw new NotImplementedException();
        }

        private void OutputDataChange(char channel, byte output_current_data)
        {
            List<byte> data = new List<byte>();

            data.Add(Convert.ToByte('D'));
            data.Add(Convert.ToByte(channel));
            data.Add(output_current_data);

            SetCommandEx(data.ToArray());
        }

        private void LedTurnOnOffByChannelNumber(IDX_CHANNEL channel, bool onOff)
        {
            List<byte> data = new List<byte>();

            data.Add(0x4F);
            data.Add(0x4E);

            RequestTurnOnStatusAll(out bool[] ledOnffStatus);

            ledOnffStatus[(int)channel] = onOff;

            data.Add(AppUtil.ConvertBoolArrayToByte(ledOnffStatus));

            SetCommandEx(data.ToArray());
        }

        private void SetCommandEx(byte[] command)
        {
            lock (_CriticalSectionKey)
            {
                if (_deviceMode == eDevMode.CONNECT)
                {

                    xSerial.SendMessage(command);

                }
                else
                {
                    return;
                }
            }
        }
    }

}
