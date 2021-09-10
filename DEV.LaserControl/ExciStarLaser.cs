using INNO6.Core;
using INNO6.Core.Communication;
using INNO6.IO.Interface;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DEV.LaserControl
{

    public class ExciStarLaser : IDeviceHandler
    {
        #region Define

        // ID Define
        public const string ID_1_VIRTUAL = "0";
        public const string ID_1_INPUT = "1";
        public const string ID_1_OUTPUT = "2";

        public const string ID_2_OBJECT = "0";
        public const string ID_2_DOUBLE = "1";
        public const string ID_2_INT = "2";
        public const string ID_2_STRING = "3";

        public const string ID_3_UNKNOWN = "0";
        public const string ID_3_OPMODE = "1";
        public const string ID_3_ENERGY = "2";
        public const string ID_3_TRIGGER = "3";
        public const string ID_3_PULSE = "4";
        public const string ID_3_EGYMODE = "5";
        public const string ID_3_RESET = "6";
        public const string ID_3_STATUS = "7";

        #endregion

        private ExciStarCommands _LaserDevice;
        private XSerialComm xSerial;
        private string _deviceName;
        private eDevMode _deviceMode;
        private long _milisecondResponseTimeout;
        private bool isDeviceLogging = true;
        private static object _CriticalSectionKey = new object();

        public ExciStarLaser()
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
            _LaserDevice = new ExciStarCommands(xSerial, _milisecondResponseTimeout);

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
            xSerial.Close();
            return true;
        }

        public bool DeviceInit()
        {
            throw new NotImplementedException();
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
            result = false;
            double outData = 0.0;

            if (id_1 != ID_1_INPUT || id_2 != ID_2_DOUBLE) return outData;

            if (id_3 == ID_3_ENERGY && id_4 == "1")
            {
                result = _LaserDevice.GET_EGY(out outData);
            }
            else if (id_3 == ID_3_ENERGY && id_4 == "2")
            {
                result = _LaserDevice.GET_EGYSET(out outData);
            }
            else if (id_3 == ID_3_ENERGY && id_4 == "3")
            {
                result = _LaserDevice.GET_HV(out outData);
            }
            else if (id_3 == ID_3_STATUS && id_4 == "1")
            {
                result = _LaserDevice.GET_TUBETEMP(out outData);
            }

            return outData;
        }

        public int GET_INT_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            result = false;
            int outData = -1;

            if (id_1 != ID_1_INPUT || id_2 != ID_2_INT) return outData;

            if (id_3 == ID_3_PULSE && id_4 == "1")
            {
                result = _LaserDevice.GET_REPRATE(out outData);
            }
            else if (id_3 == ID_3_PULSE && id_4 == "2")
            {
                result = _LaserDevice.GET_BSTPULSES(out outData);
            }
            else if (id_3 == ID_3_PULSE && id_4 == "3")
            {
                result = _LaserDevice.GET_BSTPAUSE(out outData);
            }
            else if (id_3 == ID_3_PULSE && id_4 == "4")
            {
                result = _LaserDevice.GET_SEQBST(out outData);
            }
            else if (id_3 == ID_3_PULSE && id_4 == "5")
            {
                result = _LaserDevice.GET_SEQPAUSE(out outData);
            }
            else if (id_3 == ID_3_PULSE && id_4 == "6")
            {
                result = _LaserDevice.GET_COUNTS(out outData);
            }
            else if (id_3 == ID_3_STATUS && id_4 == "1")
            {
                result = _LaserDevice.GET_COUNTER(out outData);
            }
            else if (id_3 == ID_3_STATUS && id_4 == "2")
            {
                result = _LaserDevice.GET_COUNTERMAINT(out outData);
            }
            else if (id_3 == ID_3_STATUS && id_4 == "3")
            {
                result = _LaserDevice.GET_COUNTERTOTAL(out outData);
            }
            else if (id_3 == ID_3_STATUS && id_4 == "4")
            {
                result = _LaserDevice.GET_COUNTERNEWFILL(out outData);
            }
            else if (id_3 == ID_3_STATUS && id_4 == "5")
            {
                result = _LaserDevice.GET_PRESSURE(out outData);
            }
            else if (id_3 == ID_3_STATUS && id_4 == "6")
            {
                result = _LaserDevice.GET_MANPRESS(out outData);
            }
            else if (id_3 == ID_3_OPMODE && id_4 == "1")
            {
                result = _LaserDevice.GET_OPMODE(out string opMode, out string param, out string statusCode);

                if (param == "WAIT") outData = 1;
                else outData = 0;
            }


            return outData;
        }

        public string GET_STRING_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            result = false;
            string outData = "";

            if (id_1 != ID_1_INPUT || id_2 != ID_2_STRING) return outData;

            if (id_3 == ID_3_OPMODE && id_4 == "1")
            {
                result = _LaserDevice.GET_OPMODE(out string opMode, out string _, out string _);
                outData = opMode;
            }
            else if (id_3 == ID_3_OPMODE && id_4 == "2")
            {
                result = _LaserDevice.GET_OPMODE(out string _, out string _, out string statusCode);
                outData = statusCode;
            }
            else if (id_3 == ID_3_TRIGGER && id_4 == "1")
            {
                result = _LaserDevice.GET_TRIGGER(out outData);
            }
            else if (id_3 == ID_3_EGYMODE && id_4 == "1")
            {
                result = _LaserDevice.GET_MODE(out outData);
            }
            else if (id_3 == ID_3_STATUS && id_4 == "1")
            {
                result = _LaserDevice.GET_INTERLOCK(out outData);
            }
            else if (id_3 == ID_3_STATUS && id_4 == "2")
            {
                result = _LaserDevice.GET_MAINTENANCE(out outData);
            }
 
            return outData;
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
            result = false;

            if (id_1 != ID_1_OUTPUT || id_2 != ID_2_DOUBLE) return;

            if (id_3 == ID_3_ENERGY && id_4 == "1" && value == 1) 
            {
                result = _LaserDevice.SET_EGY(value);
            }
            else if (id_3 == ID_3_ENERGY && id_4 == "2" && value == 1)
            {
                result = _LaserDevice.SET_EGYSET(value);
            }
            else if (id_3 == ID_3_ENERGY && id_4 == "3" && value == 1)
            {
                result = _LaserDevice.SET_HV(value);
            }
        }

        public void SET_INT_OUT(string id_1, string id_2, string id_3, string id_4, int value, ref bool result)
        {
            result = false;

            if (id_1 != ID_1_OUTPUT || id_2 != ID_2_INT) return;


            if (id_3 == ID_3_OPMODE && id_4 == "1" && value == 1) // SET OPMODE=OFF [oLaser.iOpMode.Off]
            {
                result = _LaserDevice.SET_OPMODE(OPMODE.OFF);
            }
            else if (id_3 == ID_3_OPMODE && id_4 == "2" && value == 1)
            {
                result = _LaserDevice.SET_OPMODE(OPMODE.ON);
            }
            else if (id_3 == ID_3_OPMODE && id_4 == "3" && value == 1)
            {
                result = _LaserDevice.SET_OPMODE(OPMODE.STANDBY);
            }
            else if (id_3 == ID_3_OPMODE && id_4 == "4" && value == 1)
            {
                result = _LaserDevice.SET_OPMODE(OPMODE.SHUTDOWN);
            }
            else if (id_3 == ID_3_TRIGGER && id_4 == "1" && value == 1)
            {
                result = _LaserDevice.SET_TRIGGER(TRIGGER.INT);
            }
            else if (id_3 == ID_3_TRIGGER && id_4 == "2" && value == 1)
            {
                result = _LaserDevice.SET_TRIGGER(TRIGGER.INTB);
            }
            else if (id_3 == ID_3_TRIGGER && id_4 == "3" && value == 1)
            {
                result = _LaserDevice.SET_TRIGGER(TRIGGER.INT_COUNTS);
            }
            else if (id_3 == ID_3_EGYMODE && id_4 == "1" && value == 1)
            {
                result = _LaserDevice.SET_MODE(MODE.EGY_NGR);
            }
            else if (id_3 == ID_3_EGYMODE && id_4 == "2" && value == 1)
            {
                result = _LaserDevice.SET_MODE(MODE.EGYBURST_NGR);
            }
            else if (id_3 == ID_3_EGYMODE && id_4 == "3" && value == 1)
            {
                result = _LaserDevice.SET_MODE(MODE.HV_NGR);
            }
            else if (id_3 == ID_3_RESET && id_4 == "1" && value == 1)
             {
                result = _LaserDevice.RESET_COUNTER();
            }
            else if (id_3 == ID_3_RESET && id_4 == "2" && value == 1)
            {
                result = _LaserDevice.RESET_COUNTERMAINT();
            }
            else if (id_3 == ID_3_RESET && id_4 == "3" && value == 1)
            {
                result = _LaserDevice.RESET_FILTERCONTAMINATION();
            }
            else if (id_3 == ID_3_PULSE && id_4 == "1")
            {
                result = _LaserDevice.SET_REPRATE(value);
            }
            else if (id_3 == ID_3_PULSE && id_4 == "2")
            {
                result = _LaserDevice.SET_BSTPULSES(value);
            }
            else if (id_3 == ID_3_PULSE && id_4 == "3")
            {
                result = _LaserDevice.SET_BSTPAUSE(value);
            }
            else if (id_3 == ID_3_PULSE && id_4 == "4")
            {
                result = _LaserDevice.SET_SEQBST(value);
            }
            else if (id_3 == ID_3_PULSE && id_4 == "5")
            {
                result = _LaserDevice.SET_SEQPAUSE(value);
            }
            else if (id_3 == ID_3_PULSE && id_4 == "6")
            {
                result = _LaserDevice.SET_COUNTS(value);
            }
        }

        public void SET_STRING_OUT(string id_1, string id_2, string id_3, string id_4, string value, ref bool result)
        {
            throw new NotImplementedException();
        }
    }
}
