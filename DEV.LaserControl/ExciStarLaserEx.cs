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
    public class ExciStarLaserEx : IDeviceHandler
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
        private InputDeviceData inputDeviceData = new InputDeviceData();

        public ExciStarLaserEx()
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

            int iBaudRate = 9600;

            if (int.TryParse(baudRate, out int parse)) iBaudRate = parse;

            xSerial = new XSerialComm(portName, iBaudRate, (Parity)int.Parse(parity), int.Parse(dataBits), (StopBits)int.Parse(stopBits), '\r');
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
                        while (true)
                        {
                            if (!xSerial.IsOpen)
                            {
                                xSerial.Open();
                                Thread.Sleep(100);
                            }
                            else if (xSerial.IsOpen)
                            {
                                DeviceDataPolling(10);
                                _deviceMode = eDevMode.CONNECT;

                            }
                            else
                            {
                                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] DeviceAttach() : DeviceName = {0}, DeviceMode = {1}, Cause = {2}", _deviceName, _deviceMode.ToString(), portName + " Can Not Open !");
                                _deviceMode = eDevMode.DISCONNECT;
                            }
                        }
                    });

                    return true;
                }
            }
        }

        private void DeviceDataPolling(int sleepMilliseconds)
        {
            if (_LaserDevice.GET_OPMODE(out string opMode, out string param, out string statusCode))
            {
                inputDeviceData.OPMODE_STATUS = opMode;
                inputDeviceData.OPMODE_ERRORCODE = param;

                if (string.IsNullOrEmpty(param) && param.Equals("WAIT"))
                    inputDeviceData.OPMODE_ISWAIT = 1;
                else
                    inputDeviceData.OPMODE_ISWAIT = 0;
            }
            
            if (_LaserDevice.GET_EGY(out double egy))
            {
                inputDeviceData.ENERGY_EGY = egy;
            }

            if (_LaserDevice.GET_EGYSET(out double egySet))
            {
                inputDeviceData.ENERGY_EGYSET = egySet;
            }

            if (_LaserDevice.GET_HV(out double hv))
            {
                inputDeviceData.ENERGY_HV = hv;
            }

            if (_LaserDevice.GET_TUBETEMP(out double temp))
            {
                inputDeviceData.STATUS_TUBETEMP = temp;
            }

            if (_LaserDevice.GET_BSTPAUSE(out int bstPause))
            {
                inputDeviceData.PULSE_BSTPAUSE = bstPause;
            }

            if (_LaserDevice.GET_BSTPULSES(out int bstPulses))
            {
                inputDeviceData.PULSE_BSTPULSE = bstPulses;
            }

            if (_LaserDevice.GET_COUNTS(out int shots))
            {
                inputDeviceData.PULSE_COUNTS = shots;
            }

            if (_LaserDevice.GET_REPRATE(out int reprate))
            {
                inputDeviceData.PULSE_REPRATE = reprate;
            }

            if (_LaserDevice.GET_SEQBST(out int seqBst))
            {
                inputDeviceData.PULSE_SEQBST = seqBst;
            }

            if (_LaserDevice.GET_SEQPAUSE(out int milliseconds))
            {
                inputDeviceData.PULSE_SEQPAUSE = milliseconds;
            }

            if (_LaserDevice.GET_COUNTERNEWFILL(out int newFillCounts))
            {
                inputDeviceData.STATUS_COUNTERNEWFILL = newFillCounts;
            }

            if (_LaserDevice.GET_COUNTERMAINT(out int maintCounts))
            {
                inputDeviceData.STATUS_COUNTERMAINT = maintCounts;
            }

            if (_LaserDevice.GET_COUNTERTOTAL(out int totalCounts))
            {
                inputDeviceData.STATUS_COUNTERTOTAL = totalCounts;
            }

            if (_LaserDevice.GET_MANPRESS(out int manPressure))
            {
                inputDeviceData.STATUS_MANPRESSURE = manPressure;
            }

            if (_LaserDevice.GET_PRESSURE(out int tubePressure))
            {
                inputDeviceData.STATUS_TUBEPRESSURE = tubePressure;
            }

            if (_LaserDevice.GET_TRIGGER(out string trigger))
            {
                inputDeviceData.TRIGGER_MODE = trigger;
            }

            Thread.Sleep(sleepMilliseconds);
        }


        public bool DeviceDettach()
        {
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
            result = true;
            double outData = 0.0;

            if (id_1 != ID_1_INPUT || id_2 != ID_2_DOUBLE) return outData;

            if (id_3 == ID_3_ENERGY && id_4 == "1")
            {
                outData = inputDeviceData.ENERGY_EGY;
            }
            else if (id_3 == ID_3_ENERGY && id_4 == "2")
            {
                outData = inputDeviceData.ENERGY_EGY;
            }
            else if (id_3 == ID_3_ENERGY && id_4 == "3")
            {
                outData = inputDeviceData.ENERGY_HV;
            }
            else if (id_3 == ID_3_STATUS && id_4 == "1")
            {
                outData = inputDeviceData.STATUS_TUBETEMP;
            }

            return outData;

        }

        public int GET_INT_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            result = true;
            int outData = -1;

            if (id_1 != ID_1_INPUT || id_2 != ID_2_INT) return outData;

            if (id_3 == ID_3_PULSE && id_4 == "1")
            {
                outData = inputDeviceData.PULSE_REPRATE;
            }
            else if (id_3 == ID_3_PULSE && id_4 == "2")
            {
                outData = inputDeviceData.PULSE_BSTPULSE;
            }
            else if (id_3 == ID_3_PULSE && id_4 == "3")
            {
                outData = inputDeviceData.PULSE_BSTPAUSE;
            }
            else if (id_3 == ID_3_PULSE && id_4 == "4")
            {
                outData = inputDeviceData.PULSE_SEQBST;
            }
            else if (id_3 == ID_3_PULSE && id_4 == "5")
            {
                outData = inputDeviceData.PULSE_SEQPAUSE;
            }
            else if (id_3 == ID_3_PULSE && id_4 == "6")
            {
                outData = inputDeviceData.PULSE_COUNTS;
            }
            else if (id_3 == ID_3_STATUS && id_4 == "1")
            {
                outData = inputDeviceData.STATUS_COUNTER;
            }
            else if (id_3 == ID_3_STATUS && id_4 == "2")
            {
                outData = inputDeviceData.STATUS_COUNTERMAINT;
            }
            else if (id_3 == ID_3_STATUS && id_4 == "3")
            {
                outData = inputDeviceData.STATUS_COUNTERTOTAL;
            }
            else if (id_3 == ID_3_STATUS && id_4 == "4")
            {
                outData = inputDeviceData.STATUS_COUNTERNEWFILL;
            }
            else if (id_3 == ID_3_STATUS && id_4 == "5")
            {
                outData = inputDeviceData.STATUS_TUBEPRESSURE;
            }
            else if (id_3 == ID_3_STATUS && id_4 == "6")
            {
                outData = inputDeviceData.STATUS_MANPRESSURE;
            }
            else if (id_3 == ID_3_OPMODE && id_4 == "1")
            {
                outData = inputDeviceData.OPMODE_ISWAIT;
            }
          

            return outData;
        }

        public string GET_STRING_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            result = true;
            string outData = "";

            if (id_1 != ID_1_INPUT || id_2 != ID_2_STRING) return outData;

            if (id_3 == ID_3_OPMODE && id_4 == "1")
            {
                outData = inputDeviceData.OPMODE_STATUS;
            }
            else if (id_3 == ID_3_OPMODE && id_4 == "2")
            {
                outData = inputDeviceData.OPMODE_ERRORCODE;
            }
            else if (id_3 == ID_3_TRIGGER && id_4 == "1")
            {
                outData = inputDeviceData.TRIGGER_MODE;
            }
            else if (id_3 == ID_3_EGYMODE && id_4 == "1")
            {
                outData = inputDeviceData.ENERGY_MODE;
            }
            else if (id_3 == ID_3_STATUS && id_4 == "1")
            {
                outData = inputDeviceData.STATUS_INTERLOCK;
            }
            else if (id_3 == ID_3_STATUS && id_4 == "2")
            {
                outData = inputDeviceData.STATUS_MAINTREQUIRED;
            }

            return outData;
        }

        public eDevMode IsDevMode()
        {
            return _deviceMode;
        }

        public void SET_DATA_OUT(string id_1, string id_2, string id_3, string id_4, object value, ref bool result)
        {
            lock (_CriticalSectionKey)
            {
                throw new NotImplementedException();
            }
        }

        public void SET_DOUBLE_OUT(string id_1, string id_2, string id_3, string id_4, double value, ref bool result)
        {

            result = false;

            if (id_1 != ID_1_OUTPUT || id_2 != ID_2_DOUBLE) return;

            if (id_3 == ID_3_ENERGY && id_4 == "1")
            {
                result = _LaserDevice.SET_EGY(value);
            }
            else if (id_3 == ID_3_ENERGY && id_4 == "2")
            {
                result = _LaserDevice.SET_EGYSET(value);
            }
            else if (id_3 == ID_3_ENERGY && id_4 == "3")
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
            result = true;
        }
    }
}
