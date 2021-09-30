using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using INNO6.Core;
using INNO6.IO.Interface;
using Pcomm32Functions;

namespace DEV.MotionControl
{

    public class PMAC : XSequence, IDeviceHandler
    {
        private UInt32 m_dwDevice;
        private Int32 m_bDriverOpen;
        System.Timers.Timer _100msTimer;
        private string _deviceName;
        private eDevMode _deviceMode;
        private int _deviceLog;

        #region Define 
        private const string JOG_FORWARD = "+";
        private const string JOG_BACKWARD = "-";
        private const string JOG_STOP = "/";

        private const int iAXIS_X = 2;
        private const int iAXIS_Y = 1;

        private const string sAXIS_X = "#2";
        private const string sAXIS_Y = "#1";

        public const string COMM_TIMEOUT = "TIMEOUT";
        public const string COMM_SUCCESS = "SUCCESS";
        public const string COMM_DISCONNECT = "DISCONNECT";
        public const string COMM_ERROR = "ERROR";
        public const string COMM_UNKNOWN = "UNKNOWN";


        // ID Define
        public const string ID_1_INPUT = "1";
        public const string ID_1_OUTPUT = "2";
        public const string ID_1_BOTH = "3";

        public const string ID_2_OBJECT = "0";
        public const string ID_2_DOUBLE = "1";
        public const string ID_2_INT = "2";
        public const string ID_2_STRING = "3";

        private const string AXIS_X_HOMMING = "P1100";
        private const string AXIS_Y_HOMMING = "P2100";

        private const string AXIS_X_HOMMING_STOP = "P1005";
        private const string AXIS_Y_HOMMING_STOP = "P2005";

        private const string AXIS_X_SET_ABSOLUTE_POSTION = "P1111";
        private const string AXIS_Y_SET_ABSOLUTE_POSTION = "P2111";

        private const string AXIS_X_SET_VELOCITY = "P1112";
        private const string AXIS_Y_SET_VELOCITY = "P2112";

        private const string AXIS_X_MOVE_TO_ABSOLUTE_POSTION = "P1110";
        private const string AXIS_Y_MOVE_TO_ABSOLUTE_POSTION = "P2110";

        private const string AXIS_X_MOVE_STOP = "P357";
        private const string AXIS_Y_MOVE_STOP = "P457";

        private const string AXIS_X_IS_HOMMING = "P1000";
        private const string AXIS_Y_IS_HOMMING = "P2000";

        private const string AXIS_X_IS_HOMMING_COMPLETED = "P1001";
        private const string AXIS_Y_IS_HOMMING_COMPLETED = "P2001";

        private const string AXIS_X_IS_MOVING = "P1010";
        private const string AXIS_Y_IS_MOVING = "P2010";

        private const string AXIS_Y_GET_POSITION = "P421";
        private const string AXIS_Y_GET_VELOCITY = "P422";

        private const string AXIS_X_GET_POSITION = "P411";
        private const string AXIS_X_GET_VELOCITY = "P412";

        private const string EMERGENCY_DOOR_STATUS = "M7100";
        private const string EMERGENCY_CPBOX_STATUS = "M7101";
        private const string GAS_ALARM_STATUS = "M7103";
        private const string LASER_SHUTTER_FORWARD_STATUS = "M7107";
        private const string LASER_SHUTTER_BACKWARD_STATUS = "M7108";
        private const string TABLE_VACCUM_STATUS = "M7109";
        private const string TABLE_VACCUM_PRESSURE_ON_STATUS = "M7110";
        private const string TABLE_VACCUM_DIGITAL_PRESSURE_STATUS = "M7111";

        private const string DOOR_OPEN_FRONT = "M7111";
        private const string DOOR_OPEN_LEFT = "M7112";
        private const string DOOR_OPEN_RIGHT = "M7113";

        private const string SET_TOWERLAMP_RED = "M7200";
        private const string SET_TOWERLAMP_YELLOW = "M7201";
        private const string SET_TOWERLAMP_GREEN = "M7202";
        private const string SET_BUZZER = "M7203";
        private const string SET_LEDLIGHT_ONOFF = "M7204";
        private const string SET_LASER_SHUTTER_FORWARD = "M7210";
        private const string SET_LASER_SHUTTER_BACKWARD = "M7211";
        private const string SET_TABLE_VACCUM_ONOFF = "M7212";
        private const string SET_TABLE_FURGE_ONOFF = "M7213";


        private const string GET_POSITION = "P";
        private const string GET_VELOCITY = "V";
        private const string SET_VELOCITY = "F";
        private const string SERVO_STOP = "K";

        #endregion

        private float _XAxisUnitPerCounts = 10000;
        private float _YAxisUnitPerCounts = 10000;


        // mm/s -> counts/msec -> 10000(counts)/1000(ms)
        private float _XAxisVelocityConvert = 10.0F;
        private float _YAxisVelocityConvert = 10.0F;

        private double _XAxisAbsSetPosition;
        private double _YAxisAbsSetPosition;

        private double _XAxisRelSetPosition;
        private double _YAxisRelSetPosition;

        private double _XAxisSetVelocity;
        private double _YAxisSetVelocity;

        private static object _key = new object();

        public PMAC()
        {
            _100msTimer = null;
            _deviceMode = eDevMode.UNKNOWN;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg0">DEVICE NAME</param>
        /// <param name="arg1">DEVICE NUMBER</param>
        /// <param name="arg2">DEVICE LOG</param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="arg5"></param>
        /// <param name="arg6"></param>
        /// <param name="arg7"></param>
        /// <param name="arg8"></param>
        /// <param name="arg9"></param>
        /// <returns></returns>
        public bool DeviceAttach(string arg0, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9)
        {
            _deviceName = arg0;
            uint deviceNumber = uint.Parse(arg1);
            m_dwDevice = PCOMM32.PmacSelect(deviceNumber);
            m_bDriverOpen = PCOMM32.OpenPmacDevice(m_dwDevice);
            _deviceLog = int.Parse(arg2);
            if (m_bDriverOpen == 0)
            {
                _deviceMode = eDevMode.DISCONNECT;

                if(_deviceLog > 0)
                {
                    LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] Device name : {0} is not open!", _deviceName);
                }
                
                return false;
            }

            _deviceMode = eDevMode.CONNECT;
            return true;
        }

        public bool DeviceDettach()
        {
            if (m_bDriverOpen == 0)
            {
                return true;
            }
            PCOMM32.ClosePmacDevice(m_dwDevice);
            _deviceMode = eDevMode.DISCONNECT;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id_1"></param>
        /// <param name="id_2"></param>
        /// <param name="id_3"></param>
        /// <param name="id_4"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public double GET_DOUBLE_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            double retValue = 0;
            result = false;

            if (id_1 == ID_1_INPUT && id_2 == ID_2_DOUBLE)
            {
                if (id_3 == "1") // Y-AXIS
                {
                    if (id_4.Equals("1")) // POSITION
                    {
                        result = QueryPosition(AXIS_Y_GET_POSITION, ref retValue);
                    }
                    else if (id_4.Equals("2")) // VELOCITY
                    {
                        result = QueryVelocity(AXIS_Y_GET_VELOCITY, ref retValue);
                    }
                }
                else if(id_3 == "2") // X-AXIS
                {
                    if (id_4.Equals("1")) // POSITION
                    {
                        result = QueryPosition(AXIS_X_GET_POSITION, ref retValue);
                    }
                    else if (id_4.Equals("2")) // VELOCITY
                    {
                        result = QueryVelocity(AXIS_X_GET_VELOCITY, ref retValue);
                    }
                }
            }


            return retValue;
        }

        /// <summary>
        /// X AXIS IS HOMMING? : id1 = '1', id2 = '2', id3 = '2', id4 = '1'
        /// Y AXIS IS HOMMING? : id1 = '1', id2 = '2', id3 = '1', id4 = '1'
        /// X AXIS HOMMING COMPLETED : id1 = '1', id2 = '2', id3 = '2', id4 = '2'
        /// Y AXIS HOMMING COMPLETED : id1 = '1', id2 = '2', id3 = '1', id4 = '2'
        /// X AXIS IS MOVING : id1 = '1', id2 = '2', id3 = '2', id4 = '3'
        /// Y AXIS IS MOVING : id1 = '1', id2 = '2', id3 = '1', id4 = '3'
        /// X AXIS IS ACTIVE : id1 = '1', id2 = '2', id3 = '2', id4 = '4'
        /// Y AXIS IS ACTIVE : id1 = '1', id2 = '2', id3 = '1', id4 = '4'
        /// 
        /// *INPUT I/0 LIST
        /// M7100 Emergency input (Door)  : id1 = '1', id2 = '2', id3 = '3', id4 = '1'
        /// M7101 Emergency input (CpBox) : id1 = '1', id2 = '2', id3 = '3', id4 = '2'
        /// M7102 RESERVE : id1 = '1', id2 = '2', id3 = '3', id4 = '3'
        /// M7103 GAS ALARM : id1 = '1', id2 = '2', id3 = '3', id4 = '4'
        /// M7104 RESERVE : id1 = '1', id2 = '2', id3 = '3', id4 = '5'
        /// M7105 RESERVE : id1 = '1', id2 = '2', id3 = '3', id4 = '6'
        /// M7106 RESERVE : id1 = '1', id2 = '2', id3 = '3', id4 = '7'
        /// M7107 Laser Shutter Fwd : id1 = '1', id2 = '2', id3 = '3', id4 = '8'
        /// M7108 Laser Shutter Bwd : id1 = '1', id2 = '2', id3 = '3', id4 = '9'
        /// M7109 Table Vaccum : id1 = '1', id2 = '2', id3 = '3', id4 = '10'
        /// M7110 Table Vaccum Pressure On : id1 = '1', id2 = '2', id3 = '3', id4 = '11'
        /// M7111 Table Vaccum Digital Pressure : id1 = '1', id2 = '2', id3 = '3', id4 = '12'
        /// </summary>
        /// <param name="id_1">INPUT/OUTPUT/BOTH</param>
        /// <param name="id_2">DATA TYPE ID</param>
        /// <param name="id_3">DEVICE TYPE ID</param>
        /// <param name="id_4">FUNCTION TYPE ID</param>
        /// <param name="value">DOUBLE VALUE</param>
        /// <param name="result">SET RESULT</param>
        public int GET_INT_IN(string id_1, string id_2, string id_3, string id_4, ref bool result)
        {
            int retValue = 0;
            result = false;

            if (id_1.Equals(ID_1_INPUT) && id_2.Equals(ID_2_INT))
            {
                if (id_3.Equals("2")) // AXIS X
                {
                    if (id_4.Equals("1")) // X AXIS IS HOMMING?
                    {
                        result = QueryIsHommingAxisX(ref retValue);
                    }
                    else if (id_4.Equals("2")) // X AXIS HOMMING COMPLETED
                    {
                        result = QueryHommingCompletedAxisX(ref retValue);
                    }
                    else if (id_4.Equals("3")) // X AXIS IS MOVING
                    {
                        result = QueryIsMovingAxisX(ref retValue);
                    }
                    else if (id_4.Equals("4"))
                    {
                        result = QueryMotorActivateAxisX(ref retValue);
                    }
                }
                else if(id_3.Equals("1")) // AXIS Y
                {
                    if (id_4.Equals("1")) // Y AXIS IS HOMMING?
                    {
                        result = QueryIsHommingAxisY(ref retValue);
                    }
                    else if (id_4.Equals("2")) // Y AXIS HOMMING COMPLETED
                    {
                        result = QueryHommingCompletedAxisY(ref retValue);
                    }
                    else if (id_4.Equals("3")) // Y AXIS IS MOVING
                    {
                        result = QueryIsMovingAxisY(ref retValue);
                    }
                    else if (id_4.Equals("4"))
                    {
                        result = QueryMotorActivateAxisY(ref retValue);
                    }
                }
                else if (id_3.Equals("3")) // INPUT IO
                {
                    if (id_4.Equals("1")) //M7100 Emergency input (Door)  : id1 = '1', id2 = '2', id3 = '3', id4 = '1'
                    {
                        result = QueryEmergencyDoor(ref retValue);
                    }
                    else if(id_4.Equals("2")) //M7101 Emergency input (CpBox) : id1 = '1', id2 = '2', id3 = '3', id4 = '2'
                    {
                        result = QueryEmergencyCpBox(ref retValue); 
                    }
                    else if(id_4.Equals("4")) //M7103 GAS ALARM: id1 = '1', id2 = '2', id3 = '3', id4 = '4'
                    {
                        result = QueryGasAlarmStatus(ref retValue);
                    }
                    else if (id_4.Equals("8")) //M7107 Laser Shutter Fwd : id1 = '1', id2 = '2', id3 = '3', id4 = '8'
                    {
                        result = QueryLaserShutterFwdStatus(ref retValue);
                    }
                    else if (id_4.Equals("9")) //M7107 Laser Shutter Fwd : iid1 = '1', id2 = '2', id3 = '3', id4 = '9'
                    {
                        result = QueryLaserShutterBwdStatus(ref retValue);
                    }
                    else if (id_4.Equals("10")) //M7109 Table Vaccum : id1 = '1', id2 = '2', id3 = '3', id4 = '10'
                    {
                        result = QueryTableVaccumStatus(ref retValue);
                    }
                    else if (id_4.Equals("11")) //M7110 Table Vaccum Pressure On : id1 = '1', id2 = '2', id3 = '3', id4 = '11'
                    {
                        result = QueryTableVaccumPressureOnStatus(ref retValue);
                    }
                    else if (id_4.Equals("12")) //M7111 Table Vaccum Digital Pressure : id1 = '1', id2 = '2', id3 = '3', id4 = '12'
                    {
                        result = QueryTableVaccumDigitalPressure(ref retValue);
                    }
                    else if (id_4.Equals("13"))
                    {
                        result = QueryFrontDoorOpenStatus(ref retValue);
                    }
                    else if (id_4.Equals("14"))
                    {
                        result = QueryLeftDoorOpenStatus(ref retValue);
                    }
                    else if (id_4.Equals("15"))
                    {
                        result = QueryRightDoorOpenStatus(ref retValue);
                    }
                }
            }
            
            return retValue;
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

        /// <summary>
        /// X AXIS SET POSTION(mm) : id1 = '2', id2 = '1', id3 = '1', id4 = '1'
        /// Y AXIS SET POSTION(mm) : id1 = '2', id2 = '1', id3 = '2', id4 = '1'
        /// X AXIS SET VELOCITY(mm/s) : id1 = '2', id2 = '1', id3 = '1', id4 = '2'
        /// Y AXIS SET VELOCITY(mm/s) : id1 = '2', id2 = '1', id3 = '2', id4 = '2'
        /// </summary>
        /// <param name="id_1">INPUT/OUTPUT/BOTH</param>
        /// <param name="id_2">DATA TYPE ID</param>
        /// <param name="id_3">DEVICE TYPE ID</param>
        /// <param name="id_4">FUNCTION TYPE ID</param>
        /// <param name="value">DOUBLE VALUE</param>
        /// <param name="result">SET RESULT</param>
        public void SET_DOUBLE_OUT(string id_1, string id_2, string id_3, string id_4, double value, ref bool result)
        {
            if (id_1.Equals(ID_1_OUTPUT) && id_2.Equals(ID_2_DOUBLE))
            {
                if (id_3.Equals("1")) // AXIS Y
                {
                    if (id_4.Equals("1")) // SET POSTION (mm)
                    {
                        result = SetAbsPosition(iAXIS_Y, value);
                    }
                    else if (id_4.Equals("2"))
                    {
                        result = SetVelocity(iAXIS_Y, value);
                    }
                    else if (id_4.Equals("3"))
                    {
                        result = SetVelocity(iAXIS_Y, value);
                    }
                    else if(id_4.Equals("4"))
                    {
                        result = SetRelPosition(iAXIS_Y, value);
                    }
                }
                else if (id_3.Equals("2")) // AXIS X
                {
                    if (id_4.Equals("1")) // SET POSTION (mm)
                    {
                        result = SetAbsPosition(iAXIS_X, value);
                    }
                    else if (id_4.Equals("2"))
                    {
                        result = SetVelocity(iAXIS_X, value);
                    }
                    else if (id_4.Equals("3"))
                    {
                        result = SetVelocity(iAXIS_X, value);
                    }
                    else if (id_4.Equals("4"))
                    {
                        result = SetRelPosition(iAXIS_X, value);
                    }
                }
            }
        }

        /// <summary>
        /// JOG X FORWARD : id1 = '2', id2 = '2', id3 = '1', id4 = '1' value = '1';
        /// JOG X BACKWARD : id1 = '2', id2 = '2', id3 = '1', id4 = '2' value = '1';
        /// JOG X STOP : id1 = '2', id2 = '2', id3 = '1', id4 = '7' value = '1';
        /// JOG Y FORWARD : id1 = '2', id2 = '2', id3 = '2', id4 = '1' value = '1';
        /// JOG Y BACKWARD : id1 = '2', id2 = '2', id3 = '2', id4 = '2' value = '1';
        /// JOG Y STOP : id1 = '2', id2 = '2', id3 = '2', id4 = '7' value = '1';
        /// 
        /// HOMMING X : id1 = '2', id2 = '2', id3 = '1', id4 = '3' value = '1';
        /// HOMMING Y : id1 = '2', id2 = '2', id3 = '2', id4 = '3' value = '1';
        /// HOMMING STOP X : id1 = '2', id2 = '2', id3 = '1', id4 = '4' value = '1';
        /// HOMMING STOP Y : id1 = '2', id2 = '2', id3 = '2', id4 = '4' value = '1';
        /// ABSOLUTE MOVE AXIS X : id1 = '2', id2 = '2', id3 = '1', id4 = '5' value = '1';
        /// ABSOULTE MOVE AXIS Y : id1 = '2', id2 = '2', id3 = '2', id4 = '5' value = '1';
        /// ABSOLUTE STOP AXIS X : id1 = '2', id2 = '2', id3 = '1', id4 = '6' value = '1';
        /// ABSOULTE STOP AXIS Y : id1 = '2', id2 = '2', id3 = '2', id4 = '6' value = '1';
        /// 
        /// *OUTPUT I/0 LIST
        /// M7200 Tower Lamp Red  : id1 = '2', id2 = '2', id3 = '3', id4 = '1'
        /// M7201 Tower Lamp Yellow : id1 = '2', id2 = '2', id3 = '3', id4 = '2'
        /// M7202 Tower Lamp Green : id1 = '2', id2 = '2', id3 = '3', id4 = '3'
        /// M7203 Buzzer  : id1 = '2', id2 = '2', id3 = '3', id4 = '4'
        /// M7204 Led Light On/Off  : id1 = '2', id2 = '2', id3 = '3', id4 = '5'
        /// M7210 Laser Shutter Forward : id1 = '2', id2 = '2', id3 = '3', id4 = '11'
        /// M7211 Laser Shutter Backward : id1 = '2', id2 = '2', id3 = '3', id4 = '12'
        /// M7212 Table Vaccum On : id1 = '2', id2 = '2', id3 = '3', id4 = '13'
        /// M7213 Table Furge : id1 = '2', id2 = '2', id3 = '3', id4 = '14'
        /// </summary>
        /// <param name="id_1">INPUT/OUTPUT/BOTH</param>
        /// <param name="id_2">DATA TYPE ID</param>
        /// <param name="id_3">DEVICE TYPE ID</param>
        /// <param name="id_4">FUNCTION TYPE ID</param>
        /// <param name="value"></param>
        /// <param name="result"></param>
        public void SET_INT_OUT(string id_1, string id_2, string id_3, string id_4, int value, ref bool result)
        {
            if(id_1.Equals(ID_1_OUTPUT) && id_2.Equals(ID_2_INT))
            {
                if (id_3.Equals("0"))
                {
                    if(id_4.Equals("0"))
                    {
                        if (value == 1) // RUN
                        {
                            result = CommandServoKillAll();
                        }
                    }
                    else if(id_4.Equals("1"))
                    {
                        if (value == 1)
                        {
                            result &= CommandJogStop(iAXIS_X);
                            result &= CommandJogStop(iAXIS_Y);
                        }
                    }
                }
                else if (id_3.Equals("2")) // AXIS X
                {
                    if (id_4.Equals("1")) // JOG FORWARD
                    {
                        if (value == 1) // RUN
                        {
                            result = CommandJogFoward(iAXIS_X);
                        }
                    }
                    else if (id_4.Equals("2")) // JOG BACKWARD
                    {
                        if (value == 1) // RUN
                        {
                            result = CommandJogBackward(iAXIS_X);
                        }
                    }
                    else if (id_4.Equals("3")) // HOMMING X AXIS
                    {
                        if(value == 1)
                        {
                            result = CommandAxisXHomming(value);
                        }
                    }
                    else if (id_4.Equals("4")) // HOMMING STOP X AXIS
                    {
                        if(value == 1)
                        {
                            result = CommandAxisXHommingStop(value);
                        }                       
                    }
                    else if (id_4.Equals("5")) // ABSOLUTE POSTION MOVE X AXIS
                    {
                        if(value == 1)
                        {
                            result = CommnadMoveToSetPosition(iAXIS_X, value);
                        }                     
                    }
                    else if (id_4.Equals("6")) // ABSOLUTE POSTION MOVE X AXIS
                    {
                        if (value == 1)
                        {
                            result = CommandJogStop(iAXIS_X);
                        }                                 
                    }
                    else if (id_4.Equals("7"))
                    {
                        if(value == 1)
                        {
                            result = CommandJogStop(iAXIS_X);
                        }                        
                    }
                    else if(id_4.Equals("8"))
                    {
                        if(value == 1)
                        {
                            result = CommandMoveToRelPosition(iAXIS_X, value);
                        }
                    }
                    else if(id_4.Equals("9"))
                    {
                        if(value == 1)
                        {
                            result = CommandServoStop(iAXIS_X);
                        }
                    }
                }
                else if (id_3.Equals("1")) // AXIS Y
                {
                    if (id_4.Equals("1")) // JOG FORWARD
                    {
                        if (value == 1)
                        {
                            result = CommandJogFoward(iAXIS_Y);
                        }
                    }
                    else if (id_4.Equals("2")) // JOG BACKWARD
                    {
                        if (value == 1) // RUN
                        {
                            result = CommandJogBackward(iAXIS_Y);
                        }
                    }
                    else if (id_4.Equals("3")) // HOMMING Y AXIS
                    {
                        if (value == 1)
                            result = CommandAxisYHomming(value);
                    }
                    else if (id_4.Equals("4")) // HOMMING STOP Y AXIS
                    {
                        if (value == 1)
                            result = CommandAxisYHommingStop(value);
                    }
                    else if (id_4.Equals("5")) // ABSOLUTE POSTION MOVE Y AXIS
                    {
                        if(value == 1)
                            result = CommnadMoveToSetPosition(iAXIS_Y, value);
                    }
                    else if (id_4.Equals("6")) // ABSOLUTE POSTION MOVE Y AXIS
                    {
                        if(value == 1)
                            result = CommandJogStop(iAXIS_Y);
                    }
                    else if (id_4.Equals("7")) // JOG STOP Y AXIS
                    {
                        if (value == 1)
                            result = CommandJogStop(iAXIS_Y);                   
                    }
                    else if (id_4.Equals("8"))
                    {
                        if (value == 1)
                            result = CommandMoveToRelPosition(iAXIS_Y, value);
                    }
                    else if (id_4.Equals("9"))
                    {
                        if (value == 1)
                        {
                            result = CommandServoStop(iAXIS_Y);
                        }
                    }
                }
                else if (id_3.Equals("3")) // OUTPUT I/O 
                {
                    if (id_4.Equals("1")) //M7200 Tower Lamp Red  : id1 = '2', id2 = '2', id3 = '3', id4 = '1'
                    {
                        result = CommandTowerLampRedOnOff(value);
                    }
                    else if (id_4.Equals("2")) //M7201 Tower Lamp Yellow : id1 = '2', id2 = '2', id3 = '3', id4 = '2'
                    {
                        result = CommandTowerLampYellowOnOff(value);
                    }
                    else if (id_4.Equals("3")) //M7202 Tower Lamp Green : id1 = '2', id2 = '2', id3 = '3', id4 = '3'
                    {
                        result = CommandTowerLampGreenOnOff(value);
                    }
                    else if (id_4.Equals("4")) //M7203 Buzzer  : id1 = '2', id2 = '2', id3 = '3', id4 = '4'
                    {
                        result = CommandBuzzerOnOff(value);
                    }
                    else if (id_4.Equals("5")) //M7204 Led Light On/Off  : id1 = '2', id2 = '2', id3 = '3', id4 = '5'
                    {
                        result = CommandLedLightOnOff(value);
                    }
                    else if (id_4.Equals("11")) //M7210 Laser Shutter Forward : id1 = '2', id2 = '2', id3 = '3', id4 = '11'
                    {
                        result = CommandLaserShutterFwd(value);
                    }
                    else if (id_4.Equals("12")) //M7211 Laser Shutter Backward : id1 = '2', id2 = '2', id3 = '3', id4 = '12'
                    {
                        result = CommandLaserShutterBwd(value);
                    }
                    else if (id_4.Equals("13")) //M7212 Table Vaccum On : id1 = '2', id2 = '2', id3 = '3', id4 = '13'
                    {
                        result = CommandTableVaccumOnOff(value);
                    }
                    else if (id_4.Equals("14")) //M7213 Table Purge : id1 = '2', id2 = '2', id3 = '3', id4 = '14'
                    {
                        result = CommandTablePurgeOnOff(value);
                    }
                }

                result = true;
            }
            else
            {
                result = false;
            }

        }

        private bool CommandServoKillAll()
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("#1K, #2K");
            //switch (axis)
            //{
            //    case iAXIS_X:
            //        strRequest.AppendFormat("{0}{1}", sAXIS_X, SERVO_STOP);
            //        break;
            //    case iAXIS_Y:
            //        strRequest.AppendFormat("{0}{1}", sAXIS_Y, SERVO_STOP);
            //        break;
            //    default:
            //        LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandServoStop() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
            //        return false;
            //}

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandServoStop() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandServoStop() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandServoStop(int axis)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            switch(axis)
            {
                case iAXIS_X:
                    strRequest.AppendFormat("{0}{1}", sAXIS_X, SERVO_STOP);
                    break;
                case iAXIS_Y:
                    strRequest.AppendFormat("{0}{1}", sAXIS_Y, SERVO_STOP);
                    break;
                default:
                    LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandServoStop() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                    return false;
            }

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandServoStop() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandServoStop() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }

        public void SET_STRING_OUT(string id_1, string id_2, string id_3, string id_4, string value, ref bool result)
        {
            throw new NotImplementedException();
        }

        private bool CommandTowerLampRedOnOff(int setOnOff)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";


            strRequest.AppendFormat("{0}={1}", SET_TOWERLAMP_RED, setOnOff);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandTowerLampRedOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandTowerLampRedOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandTowerLampYellowOnOff(int setOnOff)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";


            strRequest.AppendFormat("{0}={1}", SET_TOWERLAMP_YELLOW, setOnOff);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandTowerLampYellowOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandTowerLampYellowOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandTowerLampGreenOnOff(int setOnOff)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";


            strRequest.AppendFormat("{0}={1}", SET_TOWERLAMP_GREEN, setOnOff);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandTowerLampGreenOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandTowerLampGreenOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandBuzzerOnOff(int setOnOff)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";


            strRequest.AppendFormat("{0}={1}", SET_BUZZER, setOnOff);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandBuzzerOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandBuzzerOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }


        private bool CommandLedLightOnOff(int setOnOff)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";


            strRequest.AppendFormat("{0}={1}", SET_LEDLIGHT_ONOFF, setOnOff);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandLedLightOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandLedLightOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandLaserShutterFwd(int setOnOff)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";


            strRequest.AppendFormat("{0}={1}", SET_LASER_SHUTTER_FORWARD, setOnOff);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandLaserShutterFwd() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandLaserShutterFwd() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandLaserShutterBwd(int setOnOff)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("{0}={1}", SET_LASER_SHUTTER_BACKWARD, setOnOff);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandLaserShutterBwd() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandLaserShutterBwd() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandTableVaccumOnOff(int setOnOff)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("{0}={1}", SET_TABLE_VACCUM_ONOFF, setOnOff);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandTableVaccumOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandTableVaccumOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandTablePurgeOnOff(int setOnOff)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("{0}={1}", SET_TABLE_FURGE_ONOFF, setOnOff);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandTableFurgeOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandTableFurgeOnOff() : SendMessage={1}, ResponseMessage={2}", strRequest, strResponse);
                return false;
            }
        }

        private bool SetPostionXAxis(double pos)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            string strPosition = pos.ToString("F3", CultureInfo.CreateSpecificCulture("es-ES"));
            strRequest.AppendFormat("{0}={1}", AXIS_X_SET_ABSOLUTE_POSTION, strPosition);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] SetPostionXAxis() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] SetPostionXAxis() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

    

        private bool SetPostionYAxis(double pos)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            string strPosition = pos.ToString("F3", CultureInfo.CreateSpecificCulture("es-ES"));
            strRequest.AppendFormat("{0}={1}", AXIS_Y_SET_ABSOLUTE_POSTION, strPosition);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] SetPostionYAxis() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] SetPostionYAxis() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool SetVelocityXAxis(double velocity)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            string strVelocity = velocity.ToString("F3", CultureInfo.CreateSpecificCulture("es-ES"));
            strRequest.AppendFormat("{0}={1}", AXIS_X_SET_VELOCITY, strVelocity);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] SetVelocityXAxis() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] SetVelocityXAxis() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool SetVelocityYAxis(double velocity)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            string strVelocity = velocity.ToString("F3", CultureInfo.CreateSpecificCulture("es-ES"));
            strRequest.AppendFormat("{0}={1}", AXIS_Y_SET_VELOCITY, strVelocity);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] SetVelocityYAxis() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] SetVelocityYAxis() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool SetRelPosition(int axis, double distance)
        {
            switch (axis)
            {
                case iAXIS_X:
                    {
                        _XAxisRelSetPosition = distance;
                    }
                    break;
                case iAXIS_Y:
                    {
                        _YAxisRelSetPosition = distance;
                    }
                    break;
                default:
                    {
                        LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] Axis Number({0}) is unknown", axis);
                        return false;
                    }
                    break;
            }

            return true;
        }

        private bool SetVelocity(int axis, double velocity)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";


            switch (axis)
            {
                case iAXIS_X:
                    {
                        _XAxisSetVelocity = velocity * _XAxisVelocityConvert;
                        string strVelocity = velocity.ToString("F3", CultureInfo.CreateSpecificCulture("es-ES"));
                        strRequest.AppendFormat("{0}={1}", "I222", _XAxisSetVelocity);

                    }
                    break;
                case iAXIS_Y:
                    {
                        _YAxisSetVelocity = velocity * _YAxisVelocityConvert;
                        string strVelocity = _YAxisSetVelocity.ToString("F3", CultureInfo.CreateSpecificCulture("es-ES"));
                        strRequest.AppendFormat("{0}={1}", "I122", strVelocity);

                    }
                    break;
                default:
                    {
                        LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] Axis Number({0}) is unknown", axis);
                        return false;
                    }
                    break;
            }

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] SetVelocity() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] SetVelocity() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandMoveToRelPosition(int axis, int setValue)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            switch (axis)
            {
                case iAXIS_X:
                    {
                        strRequest.AppendFormat("{0}:{1}", "#2J", _XAxisRelSetPosition * _XAxisUnitPerCounts);
                    }
                    break;
                case iAXIS_Y:
                    {
                        strRequest.AppendFormat("{0}:{1}", "#1J", _YAxisRelSetPosition * _YAxisUnitPerCounts);
                    }
                    break;
                default:
                    {
                        LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] Axis Number({0}) is unknown", axis);
                        return false;
                    }
                    break;
            }

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommnadMoveToSetPosition() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommnadMoveToSetPosition() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                return false;
            }
        }

        private bool CommnadMoveToSetPosition(int axis, int setValue)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            switch (axis)
            {
                case iAXIS_X:
                    {
                        strRequest.AppendFormat("{0}={1}", "#2J", _XAxisAbsSetPosition * _XAxisUnitPerCounts);
                    }
                    break;
                case iAXIS_Y:
                    {
                        strRequest.AppendFormat("{0}={1}", "#1J", _YAxisAbsSetPosition * _YAxisUnitPerCounts);
                    }
                    break;
                default:
                    {
                        LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] Axis Number({0}) is unknown", axis);
                        return false;
                    }
                    break;
            }

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] CommnadMoveToSetPosition() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] CommnadMoveToSetPosition() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                return false;
            }
        }

        private bool CommandJogFoward(int axis)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            switch(axis)
            {
                case iAXIS_X:
                    {
                        strRequest.AppendFormat("{0} J{1}", sAXIS_X, JOG_FORWARD);
                    }
                    break;
                case iAXIS_Y:
                    {
                        strRequest.AppendFormat("{0} J{1}", sAXIS_Y, JOG_FORWARD);
                    }
                    break;
                default:
                    {
                        LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] Axis Number({0}) is unknown", axis);
                        return false;
                    }
                    break;

                    if (string.IsNullOrEmpty(strResponse))
                    {
                        if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandJogFoward() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                        return true;
                    }
                    else
                    {
                        LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandJogFoward() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                        return false;
                    }
            }

            CommandOrQuery(strRequest.ToString(), out strResponse);

            return true;
        }

        private bool CommandJogBackward(int axis)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            switch (axis)
            {
                case iAXIS_X:
                    {
                        strRequest.AppendFormat("{0} J{1}", sAXIS_X, JOG_BACKWARD);
                    }
                    break;
                case iAXIS_Y:
                    {
                        strRequest.AppendFormat("{0} J{1}", sAXIS_Y, JOG_BACKWARD);
                    }
                    break;
                default:
                    {
                        LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] Axis Number({0}) is unknown", axis);
                        return false;
                    }
                    break;

                    if (string.IsNullOrEmpty(strResponse))
                    {
                        if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandJogBackward() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                        return true;
                    }
                    else
                    {
                        LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandJogBackward() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                        return false;
                    }
            }

            CommandOrQuery(strRequest.ToString(), out strResponse);

            return true;
        }

        private bool CommandJogStop(int axis)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            switch (axis)
            {
                case iAXIS_X:
                    {
                        strRequest.AppendFormat("{0} J{1}", sAXIS_X, JOG_STOP);
                    }
                    break;
                case iAXIS_Y:
                    {
                        strRequest.AppendFormat("{0} J{1}", sAXIS_Y, JOG_STOP);
                    }
                    break;
                default:
                    {
                        LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] Axis Number({0}) is unknown", axis);
                        return false;
                    }
                    break;
            }

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandJogStop() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandJogStop() : Axis={0}, SendMessage={1}, ResponseMessage={2}", axis, strRequest, strResponse);
                return false;
            }
        }

        private bool CommandAxisXHomming(int setValue)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("{0}={1}", AXIS_X_HOMMING, setValue);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandAxisXHomming() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandAxisXHomming() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandAxisXHommingStop(int setValue)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("{0}={1}", AXIS_X_HOMMING_STOP, setValue);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandAxisXHommingStop() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandAxisXHommingStop() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandAxisYHomming(int setValue)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("{0}={1}", AXIS_Y_HOMMING, setValue);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandAxisYHomming() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandAxisYHomming() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool CommandAxisYHommingStop(int setValue)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";

            strRequest.AppendFormat("{0}={1}", AXIS_Y_HOMMING_STOP, setValue);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] CommandAxisYHommingStop() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] CommandAxisYHommingStop() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryMotorActivateAxisX(ref int isActive)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("I200");

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isActive))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] QueryMotorActivateAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] QueryMotorActivateAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryMotorActivateAxisY(ref int isActive)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("I100");

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isActive))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] QueryMotorActivateAxisY() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] QueryMotorActivateAxisY() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryIsHommingAxisX(ref int isHomming)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_X_IS_HOMMING);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isHomming))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] QueryIsHommingAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] QueryIsHommingAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryIsHommingAxisY(ref int isHomming)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_Y_IS_HOMMING);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isHomming))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] QueryIsHommingAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] QueryIsHommingAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryHommingCompletedAxisX(ref int isCompleted)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_X_IS_HOMMING_COMPLETED);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isCompleted))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] QueryHommingCompletedAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] QueryHommingCompletedAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryHommingCompletedAxisY(ref int isCompleted)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_Y_IS_HOMMING_COMPLETED);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isCompleted))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] QueryHommingCompletedAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] QueryHommingCompletedAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryIsMovingAxisX(ref int isMoving)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_X_IS_MOVING);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isMoving))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] QueryIsMovingAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] QueryIsMovingAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryIsMovingAxisY(ref int isMoving)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", AXIS_Y_IS_MOVING);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isMoving))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] QueryIsMovingAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] QueryIsMovingAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryEmergencyDoor(ref int isOpen)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", EMERGENCY_DOOR_STATUS);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isOpen))
            {
                if(this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] QueryEmergencyDoorOpen() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] QueryEmergencyDoorOpen() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryEmergencyCpBox(ref int isOpen)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", EMERGENCY_CPBOX_STATUS);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isOpen))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] QueryEmergencyCpBoxOpen() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] QueryEmergencyCpBoxOpen() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryGasAlarmStatus(ref int isAlarm)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", GAS_ALARM_STATUS);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isAlarm))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] QueryGasAlarmStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] QueryGasAlarmStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryLaserShutterFwdStatus(ref int isForward)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", LASER_SHUTTER_FORWARD_STATUS);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isForward))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] QueryLaserShutterFwdStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] QueryLaserShutterFwdStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryLaserShutterBwdStatus(ref int isBackward)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", LASER_SHUTTER_BACKWARD_STATUS);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isBackward))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] QueryLaserShutterBwdStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] QueryLaserShutterBwdStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryTableVaccumStatus(ref int isVaccumOn)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", TABLE_VACCUM_STATUS);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out isVaccumOn))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] QueryTableVaccumStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] QueryTableVaccumStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryTableVaccumPressureOnStatus(ref int vaccumPressure)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", TABLE_VACCUM_PRESSURE_ON_STATUS);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out vaccumPressure))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] QueryTableVaccumPressureOnStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] QueryTableVaccumPressureOnStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryTableVaccumDigitalPressure(ref int vaccumPressure)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", TABLE_VACCUM_DIGITAL_PRESSURE_STATUS);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out vaccumPressure))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] QueryTableVaccumDigitalPressure() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] QueryTableVaccumDigitalPressure() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryFrontDoorOpenStatus(ref int doorOpenStatus)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", DOOR_OPEN_FRONT);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out doorOpenStatus))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] QueryFrontDoorOpenStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] QueryFrontDoorOpenStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryLeftDoorOpenStatus(ref int doorOpenStatus)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", DOOR_OPEN_LEFT);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out doorOpenStatus))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] QueryLeftDoorOpenStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] QueryLeftDoorOpenStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryRightDoorOpenStatus(ref int doorOpenStatus)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", DOOR_OPEN_RIGHT);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (int.TryParse(strResponse, out doorOpenStatus))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.DebugFormat("[SUCCESS] QueryRightDoorOpenStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.DebugFormat("[ERROR] QueryRightDoorOpenStatus() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryPosition(string address, ref double pos)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", address);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (double.TryParse(strResponse, out pos))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryPositionAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryPositionAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool QueryVelocity(string address, ref double vel)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            int result = 0;

            strRequest.AppendFormat("{0}", address);

            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (double.TryParse(strResponse, out vel))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] QueryPositionAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] QueryPositionAxisX() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }

        private bool SetAbsPosition(int axis, double pos)
        {
            StringBuilder strRequest = new StringBuilder();
            string strResponse = "";
            string strPosition = pos.ToString("F3");

            switch (axis)
            {
                case iAXIS_X:
                    {
                        strRequest.AppendFormat("{0}={1}", AXIS_X_SET_ABSOLUTE_POSTION, strPosition);
                        _XAxisAbsSetPosition = pos;
                    }
                    break;
                case iAXIS_Y:
                    {
                        strRequest.AppendFormat("{0}={1}", AXIS_Y_SET_ABSOLUTE_POSTION, strPosition);
                        _YAxisAbsSetPosition = pos;
                    }
                    break;
                default:
                    {
                        LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] Axis Number({0}) is unknown", axis);
                        return false;
                    }
                    break;
            }


            CommandOrQuery(strRequest.ToString(), out strResponse);

            if (string.IsNullOrEmpty(strResponse))
            {
                if (this._deviceLog > 0) LogHelper.Instance.DeviceLog.InfoFormat("[SUCCESS] SetPostion() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return true;
            }
            else
            {
                LogHelper.Instance.DeviceLog.ErrorFormat("[ERROR] SetPostion() : SendMessage={0}, ResponseMessage={1}", strRequest, strResponse);
                return false;
            }
        }



        private void CommandOrQuery(string strRequest, out string strResponse)
        {

            strResponse = "";

            if (m_bDriverOpen == 0)
                return;

            //C# :  public static extern long PmacGetResponseExA(uint dwDevice, IntPtr response, uint maxchar, StringBuilder command);
            //C : long PmacGetResponseExA(DWORD dwDevice,PCHAR response,UINT maxchar,PCHAR command);
            IntPtr pResponse;
            long nRetVal = 0;
            long nRetCommCharLength = 0;
            uint dwMaxchar = 10240;
            byte[] bBuffer;


            //Unmanaged 동적영역 할당
            pResponse = System.Runtime.InteropServices.Marshal.AllocHGlobal((int)dwMaxchar);

            //PMAC에 값 쿼리
            nRetVal = PCOMM32.PmacGetResponseExA(m_dwDevice, pResponse, dwMaxchar, new StringBuilder(strRequest));

            //리턴값 검사 1
            if (pResponse != IntPtr.Zero)
            {
                //반환된 ASCII Char수 확인
                nRetCommCharLength = PCOMM32.getCOMM_CHARS(nRetVal);

                //Managed 동적영역 할당
                bBuffer = new byte[nRetCommCharLength];

                Marshal.Copy(pResponse, bBuffer, 0, (int)nRetCommCharLength);

                strResponse = System.Text.Encoding.ASCII.GetString(bBuffer);
            }
            else
            {
                //리턴값 검사 2
                strResponse = ErrorHandlingASCIICommunication(nRetVal);
            }
            //unmanaged 동적영역 해제
            System.Runtime.InteropServices.Marshal.FreeHGlobal(pResponse);


        }//end function - private void CommandOrQuery()

        private string ErrorHandlingASCIICommunication(long c)
        {
            long nRetVal;
            bool isErrorOccur = false;
            StringBuilder sbResult = new StringBuilder();
            //To check for individual error codes the MACROs below are very useful:
            //if (PMAC.IS_COMM_MORE(c)){ textBox_CmdResponse.AppendText("COMM_MORE" + "\n"); }
            //else if(PMAC.IS_COMM_EOT( c)){ textBox_CmdResponse.AppendText(Constants.STR_COMM_EOT); }
            //else 
            if (PCOMM32.IS_COMM_TIMEOUT(c)) { sbResult.Append(Constants.STR_COMM_TIMEOUT); }
            else if (PCOMM32.IS_COMM_BADCKSUM(c)) { sbResult.Append(Constants.STR_COMM_BADCKSUM); }
            else if (PCOMM32.IS_COMM_ERROR(c)) { sbResult.Append(Constants.STR_COMM_ERROR); }
            else if (PCOMM32.IS_COMM_FAIL(c)) { sbResult.Append(Constants.STR_COMM_FAIL); }
            else if (PCOMM32.IS_COMM_ANYERROR(c))
            {
                isErrorOccur = true;
            }
            else if (PCOMM32.IS_COMM_UNSOLICITED(c)) { sbResult.Append(Constants.STR_COMM_UNSOLICITED); }


            if (isErrorOccur)
            {
                StringBuilder strResponse = new StringBuilder();
                nRetVal = PCOMM32.PmacGetErrorStrA(m_dwDevice, strResponse, 512);
                if (nRetVal > 0)
                {
                    sbResult.Append(Convert.ToString(strResponse));
                }
            }

            return sbResult.ToString();
        }
    }
}
