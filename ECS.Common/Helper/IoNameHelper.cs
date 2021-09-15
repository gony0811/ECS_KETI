using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Common.Helper
{
    public class IoNameHelper
    {    
        public const string V_STR_SYS_OPERATION_MODE = "vSys.sEqp.OperationMode";
        public const string V_STR_SYS_SIMULATION_MODE = "vSys.sEqp.SimulationMode";
        public const string V_INT_SYS_SIGNAL_TOWER_YELLOW = "vSys.iSignalTower.Yellow";
        public const string V_INT_SYS_SIGNAL_TOWER_WHITE = "vSys.iSignalTower.White";
        public const string V_INT_SYS_SIGNAL_TOWER_RED = "vSys.iSignalTower.Red";
        public const string V_INT_SYS_SIGNAL_TOWER_GREEN = "vSys.iSignalTower.Green";
        public const string V_STR_SYS_RECIPE_FILE_FOLDER = "vSys.sEqp.RecipeFileFolder";
        public const string V_STR_SYS_CURRENT_RECIPE = "vSys.sEqp.CurrentRecipe";

        public const string V_INT_SYS_EQP_RUN = "vSys.iEqp.Run";
        public const string V_INT_SYS_EQP_MOVE = "vSys.iEqp.Move";
        public const string V_INT_SYS_EQP_INTERLOCK = "vSys.iEqp.Interlock";
        public const string V_INT_SYS_EQP_AVAILABILITY = "vSys.iEqp.Availability";
        public const string V_INT_SYS_EQP_ALARM = "vSys.iEqp.Alarm";
        public const string V_INT_SYS_EQP_EMOSTOP = "vSys.iEqp.EmoStop";
        public const string V_INT_SYS_EQP_LASER_WARNING_ONOFF = "vSys.iEqp.LaserWarningOnOff";

        public const string V_STR_SET_Y_JOGVEL_MODE = "vSet.sAxisY.JogVelMode";
        public const string V_STR_SET_X_JOGVEL_MODE = "vSet.sAxisX.JogVelMode";

        public const string V_DBL_SET_Y_VISION_POSITION = "vSet.dAxisY.VisionPosition";
        public const string V_DBL_SET_Y_REL_VELOCITY = "vSet.dAxisY.RelVelocity";
        public const string V_DBL_SET_Y_REL_DISTANCE = "vSet.dAxisY.RelDistance";
        public const string V_DBL_SET_Y_PROCESS_POSITION = "vSet.dAxisY.ProcessPosition";
        public const string V_DBL_SET_Y_MIN_VELOCITY = "vSet.dAxisY.MinVelocity";
        public const string V_DBL_SET_Y_MAX_VELOCITY = "vSet.dAxisY.MaxVelocity";
        public const string V_DBL_SET_Y_MIN_POSITION = "vSet.dAxisY.MinPosition";
        public const string V_DBL_SET_Y_MAX_POSITION = "vSet.dAxisY.MaxPosition";
        public const string V_DBL_SET_Y_JOGVEL_LOW = "vSet.dAxisY.JogVelLow";
        public const string V_DBL_SET_Y_JOGVEL_HIGH = "vSet.dAxisY.JogVelHigh";
        public const string V_DBL_SET_Y_INPOS_RANGE = "vSet.dAxisY.InPosRange";
        public const string V_DBL_SET_Y_ABS_VELOCITY = "vSet.dAxisY.AbsVelocity";
        public const string V_DBL_SET_Y_ABS_POSITION = "vSet.dAxisY.AbsPosition";

        public const string V_DBL_SET_X_VISION_POSITION = "vSet.dAxisX.VisionPosition";
        public const string V_DBL_SET_X_REL_VELOCITY = "vSet.dAxisX.RelVelocity";
        public const string V_DBL_SET_X_REL_DISTANCE = "vSet.dAxisX.RelDistance";
        public const string V_DBL_SET_X_PROCESS_POSITION = "vSet.dAxisX.ProcessPosition";
        public const string V_DBL_SET_X_MIN_VELOCITY = "vSet.dAxisX.MinVelocity";
        public const string V_DBL_SET_X_MAX_VELOCITY = "vSet.dAxisX.MaxVelocity";
        public const string V_DBL_SET_X_MIN_POSITION = "vSet.dAxisX.MinPosition";
        public const string V_DBL_SET_X_MAX_POSITION = "vSet.dAxisX.MaxPosition";
        public const string V_DBL_SET_X_JOGVEL_LOW = "vSet.dAxisX.JogVelLow";
        public const string V_DBL_SET_X_JOGVEL_HIGH = "vSet.dAxisX.JogVelHigh";
        public const string V_DBL_SET_X_INPOS_RANGE = "vSet.dAxisX.InPosRange";
        public const string V_DBL_SET_X_ABS_VELOCITY = "vSet.dAxisX.AbsVelocity";
        public const string V_DBL_SET_X_ABS_POSITION = "vSet.dAxisX.AbsPosition";

        public const string V_INT_SAFETY_LASER_INTERLOCK = "vSys.iSafety.LaserInterlock";

        public const string V_INT_EMO_CMD_ESTOP = "vEmo.iCmd.Estop";

        public const string IN_INT_LED_DATA_CH1 = "iLed.iData.Ch1";
        public const string IN_INT_LED_DATA_CH2 = "iLed.iData.Ch2";
        public const string IN_INT_LED_DATA_CH3 = "iLed.iData.Ch3";
        public const string IN_INT_LED_DATA_CH4 = "iLed.iData.Ch4";

        public const string OUT_INT_LED_DATA_ALL = "oLed.iDataSet.ChAll";
        public const string OUT_INT_LED_DATA_CH1 = "oLed.iDataSet.Ch1";
        public const string OUT_INT_LED_DATA_CH2 = "oLed.iDataSet.Ch2";
        public const string OUT_INT_LED_DATA_CH3 = "oLed.iDataSet.Ch3";
        public const string OUT_INT_LED_DATA_CH4 = "oLed.iDataSet.Ch4";

        public const string IN_INT_LED_ONOFF_CH1 = "iLed.iOnOff.Ch1";
        public const string IN_INT_LED_ONOFF_CH2 = "iLed.iOnOff.Ch2";
        public const string IN_INT_LED_ONOFF_CH3 = "iLed.iOnOff.Ch3";
        public const string IN_INT_LED_ONOFF_CH4 = "iLed.iOnOff.Ch4";

        public const string OUT_INT_LED_ONOFF_CH1 = "oLed.iOnOff.Ch1";
        public const string OUT_INT_LED_ONOFF_CH2 = "oLed.iOnOff.Ch2";
        public const string OUT_INT_LED_ONOFF_CH3 = "oLed.iOnOff.Ch3";
        public const string OUT_INT_LED_ONOFF_CH4 = "oLed.iOnOff.Ch4";

        public const string IN_DBL_PMAC_X_JOGVEL = "iPMAC.dAxisX.JogVel";
        public const string IN_DBL_PMAC_X_POSITION = "iPMAC.dAxisX.Position";
        public const string IN_DBL_PMAC_X_VELOCITY = "iPMAC.dAxisX.Velocity";
        public const string IN_DBL_PMAC_Y_JOGVEL = "iPMAC.dAxisY.JogVel";
        public const string IN_DBL_PMAC_Y_POSITION = "iPMAC.dAxisY.Position";
        public const string IN_DBL_PMAC_Y_VELOCITY = "iPMAC.dAxisY.Velocity";

        public const string IN_INT_PMAC_X_ISHOME = "iPMAC.iAxisX.IsHome";
        public const string IN_INT_PMAC_X_ISHOMMING = "iPMAC.iAxisX.IsHomming";
        public const string IN_INT_PMAC_X_ISMOVING = "iPMAC.iAxisX.IsMoving";
        public const string IN_INT_PMAC_X_MOTOR_ACTIVE = "iPMAC.iAxisX.MotorActive";

        public const string IN_INT_PMAC_Y_ISHOME = "iPMAC.iAxisY.IsHome";
        public const string IN_INT_PMAC_Y_ISHOMMING = "iPMAC.iAxisY.IsHomming";
        public const string IN_INT_PMAC_Y_ISMOVING = "iPMAC.iAxisY.IsMoving";
        public const string IN_INT_PMAC_Y_MOTOR_ACTIVE = "iPMAC.iAxisY.MotorActive";

        public const string IN_INT_PMAC_EMO_SWITCH_FRONT = "iPMAC.iEmo.FrontSwitch";
        public const string IN_INT_PMAC_EMO_SWITCH_BACK = "iPMAC.iEmo.BackSwitch";

        public const string IN_INT_PMAC_DOOR_FRONT = "iPMAC.iDoor.FrontStatus";
        public const string IN_INT_PMAC_DOOR_LEFT = "iPMAC.iDoor.LeftStatus";
        public const string IN_INT_PMAC_DOOR_RIGHT = "iPMAC.iDoor.RightStatus";

        public const string IN_INT_PMAC_GAS_ALARM = "iPMAC.iGAS.Alarm";

        public const string IN_INT_PMAC_DOOR1_FRONT = "iPMAC.iDoor1.Front";
        public const string IN_INT_PMAC_DOOR2_LEFT = "iPMAC.iDoor2.Left";
        public const string IN_INT_PMAC_DOOR3_RIGHT = "iPMAC.iDoor3.Right";

        public const string IN_INT_PMAC_SHUTTER_FORWARD_STATUS = "iPMAC.iShutter.FwdStatus";
        public const string IN_INT_PMAC_SHUTTER_BACKWARD_STATUS = "iPMAC.iShutter.BwdStatus";
        public const string IN_INT_PMAC_VACUUM_STATUS = "iPMAC.iVacuum.Status";

        public const string OUT_INT_PMAC_TOWERLAMP_YELLOW = "oPMAC.iTowerLamp.Yellow";
        public const string OUT_INT_PMAC_TOWERLAMP_RED = "oPMAC.iTowerLamp.Red";
        public const string OUT_INT_PMAC_TOWERLAMP_GREEN = "oPMAC.iTowerLamp.Green";
        public const string OUT_INT_PMAC_BUZZER_ONOFF = "oPMAC.iBuzzer.OnOff";
        public const string OUT_INT_PMAC_LAMP_ONOFF = "oPMAC.iLamp.OnOff";
        public const string OUT_INT_PMAC_SHUTTER_FORWARD = "oPMAC.iShutter.Forward";
        public const string OUT_INT_PMAC_SHUTTER_BACKWARD = "oPMAC.iShutter.Backward";
        public const string OUT_INT_PMAC_VACUUM_ONOFF = "oPMAC.iVacuum.OnOff";
        public const string OUT_INT_PMAC_PURGE_ONOFF = "oPMAC.iPurge.OnOff";



        public const string OUT_INT_PMAC_Y_MOVETOSETPOS = "oPMAC.iAxisY.MoveToSetPos";
        public const string OUT_INT_PMAC_Y_MOVETOSETDIS = "oPMAC.iAxisY.MoveToSetDis";
        public const string OUT_INT_PMAC_Y_MOVESTOP = "oPMAC.iAxisY.MoveStop";
        public const string OUT_INT_PMAC_Y_JOGSTOP = "oPMAC.iAxisY.JogStop";
        public const string OUT_INT_PMAC_Y_JOGFWD = "oPMAC.iAxisY.JogFwd";
        public const string OUT_INT_PMAC_Y_JOGBWD = "oPMAC.iAxisY.JogBwd";
        public const string OUT_INT_PMAC_Y_HOMMING = "oPMAC.iAxisY.Homming";
        public const string OUT_INT_PMAC_Y_HOMESTOP = "oPMAC.iAxisY.HomeStop";
        public const string OUT_INT_PMAC_Y_SERVO_STOP = "oPMAC.iAxisY.ServoStop";


        public const string OUT_INT_PMAC_X_MOVETOSETPOS = "oPMAC.iAxisX.MoveToSetPos";
        public const string OUT_INT_PMAC_X_MOVETOSETDIS = "oPMAC.iAxisX.MoveToSetDis";
        public const string OUT_INT_PMAC_X_MOVESTOP = "oPMAC.iAxisX.MoveStop";
        public const string OUT_INT_PMAC_X_JOGSTOP = "oPMAC.iAxisX.JogStop";
        public const string OUT_INT_PMAC_X_JOGFWD = "oPMAC.iAxisX.JogFwd";
        public const string OUT_INT_PMAC_X_JOGBWD = "oPMAC.iAxisX.JogBwd";
        public const string OUT_INT_PMAC_X_HOMMING = "oPMAC.iAxisX.Homming";
        public const string OUT_INT_PMAC_X_HOMESTOP = "oPMAC.iAxisX.HomeStop";
        public const string OUT_INT_PMAC_X_SERVO_STOP = "oPMAC.iAxisX.ServoStop";

        public const string OUT_DBL_PMAC_Y_SETVELOCITY = "oPMAC.dAxisY.SetVelocity";
        public const string OUT_DBL_PMAC_Y_SETPOSITION = "oPMAC.dAxisY.SetPosition";
        public const string OUT_DBL_PMAC_Y_SETDISTANCE = "oPMAC.dAxisY.SetDistance";
        public const string OUT_DBL_PMAC_Y_JOGVELOCITY = "oPMAC.dAxisY.JogVel";
        public const string OUT_DBL_PMAC_X_SETVELOCITY = "oPMAC.dAxisX.SetVelocity";
        public const string OUT_DBL_PMAC_X_SETPOSITION = "oPMAC.dAxisX.SetPosition";
        public const string OUT_DBL_PMAC_X_SETDISTANCE = "oPMAC.dAxisX.SetDistance";
        public const string OUT_DBL_PMAC_X_JOGVELOCITY = "oPMAC.dAxisX.JogVel";

        public const string OUT_INT_LASER_TRIGGER_INTCNT = "oLaser.iTrigger.INTCNT";
        public const string OUT_INT_LASER_TRIGGER_INTB = "oLaser.iTrigger.INTB";
        public const string OUT_INT_LASER_TRIGGER_INT = "oLaser.iTrigger.INT";

        public const string OUT_INT_LASER_RESET_FILTERCOUNT = "oLaser.iReset.FilterCount";
        public const string OUT_INT_LASER_RESET_COUNTERMAINT = "oLaser.iReset.CounterMaint";
        public const string OUT_INT_LASER_RESET_COUNTER = "oLaser.iReset.Counter";

        public const string OUT_INT_LASER_OPMODE_OFF = "oLaser.iOpMode.Off";
        public const string OUT_INT_LASER_OPMODE_ON = "oLaser.iOpMode.On";
        public const string OUT_INT_LASER_OPMODE_STANDBY = "oLaser.iOpMode.StandBy";
        public const string OUT_INT_LASER_OPMODE_SHUTDOWN = "oLaser.iOpMode.ShutDown";

        public const string OUT_INT_LASER_EGYMODE_HVNGR = "oLaser.iEgyMode.HVNgr";
        public const string OUT_INT_LASER_EGYMODE_EGYNGR = "oLaser.iEgyMode.EgyNgr";
        public const string OUT_INT_LASER_EGYMODE_EGYBSTNGR = "oLaser.iEgyMode.EgyBstNgr";

        public const string OUT_DBL_LASER_ENERGY_HV = "oLaser.dEnergy.HV";
        public const string OUT_DBL_LASER_ENERGY_EGYSET = "oLaser.dEnergy.EgySet";
        public const string OUT_DBL_LASER_ENERGY_EGY = "oLaser.dEnergy.Egy";

        public const string IN_STR_LASER_TRIGGER_MODE = "iLaser.sTrigger.Mode";
        public const string IN_STR_LASER_STATUS_MAINTREQUIRED = "iLaser.sStatus.MaintRequired";

        public const string IN_STR_LASER_STATUS_INTERLOCK = "iLaser.sStatus.Interlock";
        public const string IN_STR_LASER_OPMODE_STATUS = "iLaser.sOpMode.Status";
        public const string IN_STR_LASER_EGYMODE_STATUS = "iLaser.sEgyMode.Status";

        public const string IN_INT_LASER_STATUS_TUBEPRESSURE = "iLaser.iStatus.TubePressure";
        public const string IN_INT_LASER_STATUS_MANPRESSURE = "iLaser.iStatus.ManPressure";
        public const string IN_INT_LASER_STATUS_COUNTERTOTAL = "iLaser.iStatus.CounterTotal";
        public const string IN_INT_LASER_STATUS_COUNTERNEWFILL = "iLaser.iStatus.CounterNewFill";
        public const string IN_INT_LASER_STATUS_COUNTERMAINT = "iLaser.iStatus.CounterMaint";
        public const string IN_INT_LASER_STATUS_COUNTER = "iLaser.iStatus.Counter";

        public const string IN_INT_LASER_PULSE_SEQPAUSE = "iLaser.iPulse.SeqPause";
        public const string IN_INT_LASER_PULSE_SEQBST = "iLaser.iPulse.SeqBst";
        public const string IN_INT_LASER_PULSE_REPRATE = "iLaser.iPulse.RepRate";
        public const string IN_INT_LASER_PULSE_COUNTS = "iLaser.iPulse.Counts";
        public const string IN_INT_LASER_PULSE_BSTPULSES = "iLaser.iPulse.BstPulses";
        public const string IN_INT_LASER_PULSE_BSTPAUSE = "iLaser.iPulse.BstPause";

        public const string OUT_INT_LASER_PULSE_SEQPAUSE = "oLaser.iPulse.SeqPause";
        public const string OUT_INT_LASER_PULSE_SEQBST = "oLaser.iPulse.SeqBst";
        public const string OUT_INT_LASER_PULSE_REPRATE = "oLaser.iPulse.RepRate";
        public const string OUT_INT_LASER_PULSE_COUNTS = "oLaser.iPulse.Counts";
        public const string OUT_INT_LASER_PULSE_BSTPULSES = "oLaser.iPulse.BstPulses";
        public const string OUT_INT_LASER_PULSE_BSTPAUSE = "oLaser.iPulse.BstPause";

        public const string IN_DBL_LASER_STATUS_TUBETEMP = "iLaser.dStatus.TubeTemp";
        public const string IN_DBL_LASER_ENERGY_HV = "iLaser.dEnergy.HV";
        public const string IN_DBL_LASER_ENERGY_EGYSET = "iLaser.dEnergy.EgySet";
        public const string IN_DBL_LASER_ENERGY_EGY = "iLaser.dEnergy.Egy";

        public const string IN_INT_LASER_OPMODE_ISWAIT = "iLaser.iOpMode.IsWait";
        public const string IN_STR_LASER_OPMODE_ERRCODE = "iLaser.sOpMode.ErrCode";
    }
}
