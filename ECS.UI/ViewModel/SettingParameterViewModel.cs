using System.Windows;
using System.Threading;
using System.Windows.Input;
using ECS.UI.View;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using INNO6.Core;
using INNO6.IO;
using Prism.Commands;
using Microsoft.Win32;
using ECS.Common.Helper;
using System.Collections.Generic;
using INNO6.Core.Manager;

namespace ECS.UI.ViewModel
{
    public class SettingParameterViewModel : ViewModelBase
    {
        private double _VisionPositionX;
        private double _VisionPositionY;
        private double _ProcessPositionX;
        private double _ProcessPositionY;
        private double _PositionOffsetX;
        private double _PositionOffsetY;

        private double _XJogVelHigh;
        private double _XJogVelMid;
        private double _XJogVelLow;
        private double _XJogVelVeryLow;
        private double _YJogVelHigh;
        private double _YJogVelMid;
        private double _YJogVelLow;
        private double _YJogVelVeryLow;

        private double _LaserHV;
        private double _LaserEnergy;

        private int _TriggerRepRate;
        private int _TriggerPulseCounts;
        private int _TriggerBstPulses;
        private int _TriggerBstPause;
        private int _TriggerSeqBst;
        private int _TriggerSeqPause;

        private int _TotalCounts;
        private int _UserCounts;
        private int _NFCounts;

        private string _SelectedEnergyMode;
        private string _SelectedTriggerMode;

        private string _FrontDoorInterlockState;
        private string _RightDoorInterlockState;
        private string _LeftDoorInterlockState;

        private bool _FrontDoorInterlockChecked;
        private bool _LeftDoorInterlockChecked;
        private bool _RightDoorInterlockChecked;

        private List<string> _EnergyModeList;
        private List<string> _TriggerModeList;

        private ICommand _VisionPosXSetButtonCommand;
        private ICommand _VisionPosYSetButtonCommand;

        private ICommand _ProcPosXSetButtonCommand;
        private ICommand _ProcPosYSetButtonCommand;

        private ICommand _PositionOffsetXSetButtonCommand;
        private ICommand _PositionOffsetYSetButtonCommand;

        private ICommand _XJogVelHighButtonCommand;
        private ICommand _YJogVelHighButtonCommand;

        private ICommand _XJogVelMidButtonCommand;
        private ICommand _YJogVelMidButtonCommand;

        private ICommand _XJogVelLowButtonCommand;
        private ICommand _YJogVelLowButtonCommand;

        private ICommand _XJogVelVeryLowButtonCommand;
        private ICommand _YJogVelVeryLowButtonCommand;

        private ICommand _LaserEnergySetButtonCommand;
        private ICommand _LaserHVSetButtonCommand;

        private ICommand _EnergyModeSelectionChanged;
        private ICommand _TriggerModeSelectionChanged;

        private ICommand _EnergyModeSetButtonCommand;
        private ICommand _TriggerModeSetButtonCommand;
        private ICommand _TriggerRepRateSetButtonCommand;
        private ICommand _TriggerPulseCountsSetButtonCommand;
        private ICommand _TriggerBstPulsesSetButtonCommand;
        private ICommand _TriggerBstPauseSetButtonCommand;
        private ICommand _TriggerSeqBstSetButtonCommand;
        private ICommand _TriggerSeqPauseSetButtonCommand;
        private ICommand _UserCountsResetButtonCommand;
        private ICommand _FrontDoorInterlockActivate;
        private ICommand _FrontDoorInterlockDeactivate;
        private ICommand _LeftDoorInterlockActivate;
        private ICommand _LeftDoorInterlockDeactivate;
        private ICommand _RightDoorInterlockActivate;
        private ICommand _RightDoorInterlockDeactivate;
        private ICommand _SettingsLoadedCommand;

        public List<string> EnergyModeList { get { return new List<string>() { "EGY NGR", "EGYBURST NGR", "HV NGR" }; } set { _EnergyModeList = value; } }
        public List<string> TriggerModeList { get { return new List<string>() { "INT", "INTB", "INT COUNTS" }; } set { _TriggerModeList = value; } }

        public string SelectedEnergyMode { get { return _SelectedEnergyMode; } set { _SelectedEnergyMode = value; RaisePropertyChanged("SelectedEnergyMode"); } }
        public string SelectedTriggerMode { get { return _SelectedTriggerMode; } set { _SelectedTriggerMode = value; RaisePropertyChanged("SelectedTriggerMode"); } }
        public string FrontDoorInterlockState { get { return _FrontDoorInterlockState; } set { _FrontDoorInterlockState = value; RaisePropertyChanged("FrontDoorInterlockState"); } }
        public string LeftDoorInterlockState { get { return _LeftDoorInterlockState; } set { _LeftDoorInterlockState = value; RaisePropertyChanged("LeftDoorInterlockState"); } }
        public string RightDoorInterlockState { get { return _RightDoorInterlockState; } set { _RightDoorInterlockState = value; RaisePropertyChanged("RightDoorInterlockState"); } }

        public bool FrontDoorInterlockChecked { get { return _FrontDoorInterlockChecked; } set { _FrontDoorInterlockChecked = value; RaisePropertyChanged("FrontDoorInterlockChecked"); } }
        public bool LeftDoorInterlockChecked { get { return _LeftDoorInterlockChecked; } set { _LeftDoorInterlockChecked = value; RaisePropertyChanged("LeftDoorInterlockChecked"); } }
        public bool RightDoorInterlockChecked { get { return _RightDoorInterlockChecked; } set { _RightDoorInterlockChecked = value; RaisePropertyChanged("RightDoorInterlockChecked"); } }


        public double VisionPositionX { get { return _VisionPositionX; } set { _VisionPositionX = value; RaisePropertyChanged("VisionPositionX"); } }
        public double VisionPositionY { get { return _VisionPositionY; } set { _VisionPositionY = value; RaisePropertyChanged("VisionPositionY"); } }

        public double ProcessPositionX { get { return _ProcessPositionX; } set { _ProcessPositionX = value; RaisePropertyChanged("ProcessPositionX"); } }
        public double ProcessPositionY { get { return _ProcessPositionY; } set { _ProcessPositionY = value; RaisePropertyChanged("ProcessPositionY"); } }
        public double PositionOffsetX { get { return _PositionOffsetX; } set { _PositionOffsetX = value; RaisePropertyChanged("PositionOffsetX"); } }
        public double PositionOffsetY { get { return _PositionOffsetY; } set { _PositionOffsetY = value; RaisePropertyChanged("PositionOffsetY"); } }


        public double XJogVelHigh { get { return _XJogVelHigh; } set { _XJogVelHigh = value; RaisePropertyChanged("XJogVelHigh"); } }
        public double XJogVelMid { get { return _XJogVelMid; } set { _XJogVelMid = value; RaisePropertyChanged("XJogVelMid"); } }
        public double YJogVelHigh { get { return _YJogVelHigh; } set { _YJogVelHigh = value; RaisePropertyChanged("YJogVelHigh"); } }
        public double YJogVelMid { get { return _YJogVelMid; } set { _YJogVelMid = value; RaisePropertyChanged("YJogVelMid"); } }
        public double XJogVelLow { get { return _XJogVelLow; } set { _XJogVelLow = value; RaisePropertyChanged("XJogVelLow"); } }
        public double YJogVelLow { get { return _YJogVelLow; } set { _YJogVelLow = value; RaisePropertyChanged("YJogVelLow"); } }
        public double XJogVelVeryLow { get { return _XJogVelVeryLow; } set { _XJogVelVeryLow = value; RaisePropertyChanged("XJogVelVeryLow"); } }
        public double YJogVelVeryLow { get { return _YJogVelVeryLow; } set { _YJogVelVeryLow = value; RaisePropertyChanged("YJogVelVeryLow"); } }
        public double LaserHV { get { return _LaserHV; } set { _LaserHV = value; RaisePropertyChanged("LaserHV"); } }
        public double LaserEnergy { get { return _LaserEnergy; } set { _LaserEnergy = value; RaisePropertyChanged("LaserEnergy"); } }

        public int TriggerRepRate { get { return _TriggerRepRate; } set { _TriggerRepRate = value; RaisePropertyChanged("TriggerRepRate"); } }
        public int TriggerPulseCounts { get { return _TriggerPulseCounts; } set { _TriggerPulseCounts = value; RaisePropertyChanged("TriggerPulseCounts"); } }
        public int TriggerBstPulses { get { return _TriggerBstPulses; } set { _TriggerBstPulses = value; RaisePropertyChanged("TriggerBstPulses"); } }
        public int TriggerBstPause { get { return _TriggerBstPause; } set { _TriggerBstPause = value; RaisePropertyChanged("TriggerBstPause"); } }
        public int TriggerSeqBst { get { return _TriggerSeqBst; } set { _TriggerSeqBst = value; RaisePropertyChanged("TriggerSeqBst"); } }
        public int TriggerSeqPause { get { return _TriggerSeqPause; } set { _TriggerSeqPause = value; RaisePropertyChanged("TriggerSeqPause"); } }
        public int TotalCounts { get { return _TotalCounts; } set { _TotalCounts = value; RaisePropertyChanged("TotalCounts"); } }
        public int UserCounts { get { return _UserCounts; } set { _UserCounts = value; RaisePropertyChanged("UserCounts"); } }
        public int NFCounts { get { return _NFCounts; } set { _NFCounts = value; RaisePropertyChanged("NFCounts"); } }

        public ICommand VisionPosXSetButtonCommand { get { if (_VisionPosXSetButtonCommand == null) { _VisionPosXSetButtonCommand = new DelegateCommand(ExecuteVisionPosXSetButtonCommand); } return _VisionPosXSetButtonCommand; } }
        public ICommand VisionPosYSetButtonCommand { get { if (_VisionPosYSetButtonCommand == null) { _VisionPosYSetButtonCommand = new DelegateCommand(ExecuteVisionPosYSetButtonCommand); } return _VisionPosYSetButtonCommand; } }
      
        public ICommand ProcPosXSetButtonCommand { get { if (_ProcPosXSetButtonCommand == null) { _ProcPosXSetButtonCommand = new DelegateCommand(ExecuteProcPosXSetButtonCommand); } return _ProcPosXSetButtonCommand; } }
        public ICommand ProcPosYSetButtonCommand { get { if (_ProcPosYSetButtonCommand == null) { _ProcPosYSetButtonCommand = new DelegateCommand(ExecuteProcPosYSetButtonCommand); } return _ProcPosYSetButtonCommand; } }

        public ICommand PositionOffsetXSetButtonCommand { get { if (_PositionOffsetXSetButtonCommand == null) { _PositionOffsetXSetButtonCommand = new DelegateCommand(ExecutePositionOffsetXSetButtonCommand); } return _PositionOffsetXSetButtonCommand; } }
        public ICommand PositionOffsetYSetButtonCommand { get { if (_PositionOffsetYSetButtonCommand == null) { _PositionOffsetYSetButtonCommand = new DelegateCommand(ExecutePositionOffsetYSetButtonCommand); } return _PositionOffsetYSetButtonCommand; } }


        public ICommand XJogVelHighButtonCommand { get { if (_XJogVelHighButtonCommand == null) { _XJogVelHighButtonCommand = new DelegateCommand(ExecuteXJogVelHighButtonCommand); } return _XJogVelHighButtonCommand; } }
        public ICommand YJogVelHighButtonCommand { get { if (_YJogVelHighButtonCommand == null) { _YJogVelHighButtonCommand = new DelegateCommand(ExecuteYJogVelHighButtonCommand); } return _YJogVelHighButtonCommand; } }

        public ICommand XJogVelMidButtonCommand { get { if (_XJogVelMidButtonCommand == null) { _XJogVelMidButtonCommand = new DelegateCommand(ExecuteXJogVelMidButtonCommand); } return _XJogVelMidButtonCommand; } }
        public ICommand YJogVelMidButtonCommand { get { if (_YJogVelMidButtonCommand == null) { _YJogVelMidButtonCommand = new DelegateCommand(ExecuteYJogVelMidButtonCommand); } return _YJogVelMidButtonCommand; } }


        public ICommand XJogVelLowButtonCommand { get { if (_XJogVelLowButtonCommand == null) { _XJogVelLowButtonCommand = new DelegateCommand(ExecuteXJogVelLowButtonCommand); } return _XJogVelLowButtonCommand; } }
        public ICommand YJogVelLowButtonCommand { get { if (_YJogVelLowButtonCommand == null) { _YJogVelLowButtonCommand = new DelegateCommand(ExecuteYJogVelLowButtonCommand); } return _YJogVelLowButtonCommand; } }

        public ICommand XJogVelVeryLowButtonCommand { get { if (_XJogVelVeryLowButtonCommand == null) { _XJogVelVeryLowButtonCommand = new DelegateCommand(ExecuteXJogVelVeryLowButtonCommand); } return _XJogVelVeryLowButtonCommand; } }
        public ICommand YJogVelVeryLowButtonCommand { get { if (_YJogVelVeryLowButtonCommand == null) { _YJogVelVeryLowButtonCommand = new DelegateCommand(ExecuteYJogVelVeryLowButtonCommand); } return _YJogVelVeryLowButtonCommand; } }


        public ICommand LaserHVSetButtonCommand { get { if (_LaserHVSetButtonCommand == null) { _LaserHVSetButtonCommand = new DelegateCommand(ExecuteLaserHVSetButtonCommand); } return _LaserHVSetButtonCommand; } }
        public ICommand EnergyModeSelectionChanged { get { if (_EnergyModeSelectionChanged == null) { _EnergyModeSelectionChanged = new DelegateCommand(ExecuteEnergyModeSelectionChanged); } return _EnergyModeSelectionChanged; } }
        public ICommand TriggerModeSelectionChanged { get { if (_TriggerModeSelectionChanged == null) { _TriggerModeSelectionChanged = new DelegateCommand(ExecuteTriggerModeSelectionChanged); } return _TriggerModeSelectionChanged; } }

        
        public ICommand LaserEnergySetButtonCommand { get { if (_LaserEnergySetButtonCommand == null) { _LaserEnergySetButtonCommand = new DelegateCommand(ExecuteLaserEnergySetButtonCommand); } return _LaserEnergySetButtonCommand; } }
        public ICommand EnergyModeSetButtonCommand { get { if (_EnergyModeSetButtonCommand == null) { _EnergyModeSetButtonCommand = new DelegateCommand(ExecuteEnergyModeSetButtonCommand); } return _EnergyModeSetButtonCommand; } }
        public ICommand TriggerModeSetButtonCommand { get { if (_TriggerModeSetButtonCommand == null) { _TriggerModeSetButtonCommand = new DelegateCommand(ExecuteTriggerModeSetButtonCommand); } return _TriggerModeSetButtonCommand; } }
        public ICommand TriggerRepRateSetButtonCommand { get { if (_TriggerRepRateSetButtonCommand == null) { _TriggerRepRateSetButtonCommand = new DelegateCommand(ExecuteTriggerRepRateSetButtonCommand); } return _TriggerRepRateSetButtonCommand; } }
        public ICommand TriggerPulseCountsSetButtonCommand { get { if (_TriggerPulseCountsSetButtonCommand == null) { _TriggerPulseCountsSetButtonCommand = new DelegateCommand(ExecuteTriggerPulseCountsSetButtonCommand); } return _TriggerPulseCountsSetButtonCommand; } }
        public ICommand TriggerBstPulsesSetButtonCommand { get { if (_TriggerBstPulsesSetButtonCommand == null) { _TriggerBstPulsesSetButtonCommand = new DelegateCommand(ExecuteTriggerBstPulsesSetButtonCommand); } return _TriggerBstPulsesSetButtonCommand; } }
        public ICommand TriggerBstPauseSetButtonCommand { get { if (_TriggerBstPauseSetButtonCommand == null) { _TriggerBstPauseSetButtonCommand = new DelegateCommand(ExecuteTriggerBstPauseSetButtonCommand); } return _TriggerBstPauseSetButtonCommand; } }
        public ICommand TriggerSeqBstSetButtonCommand { get { if (_TriggerSeqBstSetButtonCommand == null) { _TriggerSeqBstSetButtonCommand = new DelegateCommand(ExecuteTriggerSeqBstSetButtonCommand); } return _TriggerSeqBstSetButtonCommand; } }
        public ICommand TriggerSeqPauseSetButtonCommand { get { if (_TriggerSeqPauseSetButtonCommand == null) { _TriggerSeqPauseSetButtonCommand = new DelegateCommand(ExecuteTriggerSeqPauseSetButtonCommand); } return _TriggerSeqPauseSetButtonCommand; } }

        public ICommand UserCountsResetButtonCommand { get { if (_UserCountsResetButtonCommand == null) { _UserCountsResetButtonCommand = new DelegateCommand(ExecuteUserCountsResetButtonCommand); } return _UserCountsResetButtonCommand; } }

        public ICommand FrontDoorInterlockActivate { get { if (_FrontDoorInterlockActivate == null) { _FrontDoorInterlockActivate = new DelegateCommand(ExecuteFrontDoorInterlockActivate); } return _FrontDoorInterlockActivate; } }
        public ICommand FrontDoorInterlockDeactivate { get { if (_FrontDoorInterlockDeactivate == null) { _FrontDoorInterlockDeactivate = new DelegateCommand(ExecuteFrontDoorInterlockDeactivate); } return _FrontDoorInterlockDeactivate; } }
        public ICommand LeftDoorInterlockActivate { get { if (_LeftDoorInterlockActivate == null) { _LeftDoorInterlockActivate = new DelegateCommand(ExecuteLeftDoorInterlockActivate); } return _LeftDoorInterlockActivate; } }
        public ICommand LeftDoorInterlockDeactivate { get { if (_LeftDoorInterlockDeactivate == null) { _LeftDoorInterlockDeactivate = new DelegateCommand(ExecuteLeftDoorInterlockDeactivate); } return _LeftDoorInterlockDeactivate; } }

        public ICommand RightDoorInterlockActivate { get { if (_RightDoorInterlockActivate == null) { _RightDoorInterlockActivate = new DelegateCommand(ExecuteRightDoorInterlockActivate); } return _RightDoorInterlockActivate; } }
        public ICommand RightDoorInterlockDeactivate { get { if (_RightDoorInterlockDeactivate == null) { _RightDoorInterlockDeactivate = new DelegateCommand(ExecuteRightDoorInterlockDeactivate); } return _RightDoorInterlockDeactivate; } }
        public ICommand SettingsLoadedCommand { get { if (_SettingsLoadedCommand == null) { _SettingsLoadedCommand = new DelegateCommand(ExecuteSettingsLoadedCommand); } return _SettingsLoadedCommand; } }


        public SettingParameterViewModel()
        {
            InitSettingValues();

            this.FrontDoorInterlockState = "작동중";
            this.LeftDoorInterlockState = "작동중";
            this.RightDoorInterlockState = "작동중";

            this.FrontDoorInterlockChecked = true;
            this.LeftDoorInterlockChecked = true;
            this.RightDoorInterlockChecked = true;
        }

        private void InitSettingValues()
        {
            VisionPositionX = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_VISION_POSITION, out bool _);
            VisionPositionY = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_VISION_POSITION, out bool _);

            ProcessPositionX = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_PROCESS_POSITION, out bool _);
            ProcessPositionY = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_PROCESS_POSITION, out bool _);

            PositionOffsetX = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_POSITION_OFFSET, out bool _);
            PositionOffsetY = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_POSITION_OFFSET, out bool _);

            XJogVelHigh = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_JOGVEL_HIGH, out bool _);
            YJogVelHigh = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_JOGVEL_HIGH, out bool _);


            XJogVelLow = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_JOGVEL_LOW, out bool _);
            YJogVelLow = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_JOGVEL_LOW, out bool _);

            SelectedEnergyMode = DataManager.Instance.GET_STRING_DATA(IoNameHelper.IN_STR_LASER_EGYMODE_STATUS, out bool _);
            SelectedTriggerMode = DataManager.Instance.GET_STRING_DATA(IoNameHelper.IN_STR_LASER_TRIGGER_MODE, out bool _);

            LaserEnergy = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.IN_DBL_LASER_ENERGY_EGYSET, out bool _);
            LaserHV = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.IN_DBL_LASER_ENERGY_HV, out bool _);

            TriggerPulseCounts = DataManager.Instance.GET_INT_DATA(IoNameHelper.IN_INT_LASER_PULSE_COUNTS, out bool _);
            TriggerBstPulses = DataManager.Instance.GET_INT_DATA(IoNameHelper.IN_INT_LASER_PULSE_BSTPULSES, out bool _);
            TriggerBstPause = DataManager.Instance.GET_INT_DATA(IoNameHelper.IN_INT_LASER_PULSE_BSTPAUSE, out bool _);

            TriggerSeqBst = DataManager.Instance.GET_INT_DATA(IoNameHelper.IN_INT_LASER_PULSE_SEQBST, out bool _);
            TriggerSeqPause = DataManager.Instance.GET_INT_DATA(IoNameHelper.IN_INT_LASER_PULSE_SEQPAUSE, out bool _);

            TriggerRepRate = DataManager.Instance.GET_INT_DATA(IoNameHelper.IN_INT_LASER_PULSE_REPRATE, out bool _);

            TotalCounts = DataManager.Instance.GET_INT_DATA(IoNameHelper.IN_INT_LASER_STATUS_COUNTERTOTAL, out bool _);
            UserCounts = DataManager.Instance.GET_INT_DATA(IoNameHelper.IN_INT_LASER_STATUS_COUNTER, out bool _);
            NFCounts = DataManager.Instance.GET_INT_DATA(IoNameHelper.IN_INT_LASER_STATUS_COUNTERNEWFILL, out bool _);
        }

        private void ExecuteSettingsLoadedCommand()
        {
            InitSettingValues();
        }

        private void ExecuteVisionPosXSetButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_VISION_POSITION, VisionPositionX);
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.V_DBL_SET_X_VISION_POSITION, VisionPositionX);

            ProcessPositionX = VisionPositionX + PositionOffsetX;

            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_PROCESS_POSITION, ProcessPositionX);
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.V_DBL_SET_X_PROCESS_POSITION, ProcessPositionX);
        }

        private void ExecuteVisionPosYSetButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_VISION_POSITION, VisionPositionY);
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.V_DBL_SET_Y_VISION_POSITION, VisionPositionY);

            ProcessPositionY = VisionPositionY + PositionOffsetY;

            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_PROCESS_POSITION, ProcessPositionY);
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.V_DBL_SET_Y_PROCESS_POSITION, ProcessPositionY);

        }

        private void ExecuteProcPosXSetButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisX.ProcessPosition", ProcessPositionX);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisX.ProcessPosition", ProcessPositionX);

        }

        private void ExecuteProcPosYSetButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisY.ProcessPosition", ProcessPositionY);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisY.ProcessPosition", ProcessPositionY);

        }

        private void ExecutePositionOffsetXSetButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_POSITION_OFFSET, PositionOffsetX);
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.V_DBL_SET_X_POSITION_OFFSET, PositionOffsetX);

            ProcessPositionX = VisionPositionX + PositionOffsetX;

            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_PROCESS_POSITION, ProcessPositionX);
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.V_DBL_SET_X_PROCESS_POSITION, ProcessPositionX);
        }

        private void ExecutePositionOffsetYSetButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_POSITION_OFFSET, PositionOffsetY);
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.V_DBL_SET_Y_POSITION_OFFSET, PositionOffsetY);

            ProcessPositionY = VisionPositionY + PositionOffsetY;

            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_PROCESS_POSITION, ProcessPositionY);
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.V_DBL_SET_Y_PROCESS_POSITION, ProcessPositionY);
        }

        private void ExecuteXJogVelHighButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisX.JogVelHigh", XJogVelHigh);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisX.JogVelHigh", XJogVelHigh);
        }

        private void ExecuteXJogVelMidButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisX.JogVelMid", XJogVelMid);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisX.JogVelMid", XJogVelMid);
        }

        private void ExecuteYJogVelHighButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisY.JogVelHigh", XJogVelHigh);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisY.JogVelHigh", XJogVelHigh);
        }

        private void ExecuteYJogVelMidButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisY.JogVelMid", YJogVelMid);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisY.JogVelMid", YJogVelMid);
        }

        private void ExecuteZJogVelHighButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisZ.JogVelHigh", XJogVelHigh);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisZ.JogVelHigh", XJogVelHigh);
        }

        private void ExecuteTJogVelHighButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisT.JogVelHigh", XJogVelHigh);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisT.JogVelHigh", XJogVelHigh);
        }

        private void ExecuteRJogVelHighButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisR.JogVelHigh", XJogVelHigh);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisR.JogVelHigh", XJogVelHigh);
        }

        private void ExecuteXJogVelLowButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisX.JogVelLow", XJogVelLow);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisX.JogVelLow", XJogVelLow);
        }

        private void ExecuteYJogVelLowButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisY.JogVelLow", YJogVelLow);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisY.JogVelLow", YJogVelLow);
        }

        private void ExecuteXJogVelVeryLowButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisX.JogVelVeryLow", XJogVelVeryLow);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisX.JogVelVeryLow", XJogVelVeryLow);
        }

        private void ExecuteYJogVelVeryLowButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisY.JogVelVeryLow", YJogVelVeryLow);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisY.JogVelVeryLow", YJogVelVeryLow);
        }

        private void ExecuteZJogVelLowButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisZ.JogVelLow", XJogVelHigh);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisZ.JogVelLow", XJogVelHigh);
        }

        private void ExecuteTJogVelLowButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisT.JogVelLow", XJogVelHigh);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisT.JogVelLow", XJogVelHigh);
        }

        private void ExecuteRJogVelLowButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisR.JogVelLow", XJogVelHigh);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisR.JogVelLow", XJogVelHigh);
        }

        private void ExecuteLaserHVSetButtonCommand()
        {
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.OUT_DBL_LASER_ENERGY_HV, LaserHV);
            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.OUT_DBL_LASER_ENERGY_HV, LaserHV);
        }

        private void ExecuteLaserEnergySetButtonCommand()
        {
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.OUT_DBL_LASER_ENERGY_EGYSET, LaserEnergy);
            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.OUT_DBL_LASER_ENERGY_EGYSET, LaserEnergy);
        }

        private void ExecuteEnergyModeSetButtonCommand()
        {
            if (SelectedEnergyMode.Equals("EGY NGR"))
            {
                FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.SET_MODE_EGYNGR);
            }
            else if (SelectedEnergyMode.Equals("EGYBURST NGR"))
            {
                FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.SET_MODE_EGYBURSTNGR);
            }
            else if (SelectedEnergyMode.Equals("HV NGR"))
            {
                FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.SET_MODE_HVNGR);
            }
        }

        private void ExecuteEnergyModeSelectionChanged()
        {

        }

        private void ExecuteTriggerModeSelectionChanged()
        {

        }

        private void ExecuteTriggerModeSetButtonCommand()
        {
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.V_STR_SET_LASER_TRIGGER_MODE, SelectedTriggerMode);
            DataManager.Instance.SET_STRING_DATA(IoNameHelper.V_STR_SET_LASER_TRIGGER_MODE, SelectedTriggerMode);

            switch (SelectedTriggerMode)
            {
                case "INT":
                    {
                        DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_LASER_TRIGGER_INT, 1);
                    }
                    break;
                case "INT COUNTS":
                    {
                        DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_LASER_TRIGGER_INTCNT, 1);
                    }
                    break;
                case "INTB":
                    {
                        DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_LASER_TRIGGER_INTB, 1);
                    }
                    break;
            }
        }

        private void ExecuteTriggerRepRateSetButtonCommand()
        {
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.OUT_INT_LASER_PULSE_REPRATE, TriggerRepRate);
            DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_LASER_PULSE_REPRATE, TriggerRepRate);
        }

        private void ExecuteTriggerPulseCountsSetButtonCommand()
        {
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.OUT_INT_LASER_PULSE_COUNTS, TriggerPulseCounts);
            DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_LASER_PULSE_COUNTS, TriggerPulseCounts);
        }

        private void ExecuteTriggerBstPulsesSetButtonCommand()
        {
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.OUT_INT_LASER_PULSE_BSTPULSES, TriggerBstPulses);
            DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_LASER_PULSE_BSTPULSES, TriggerBstPulses);
        }

        private void ExecuteTriggerBstPauseSetButtonCommand()
        {
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.OUT_INT_LASER_PULSE_BSTPAUSE, TriggerBstPause);
            DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_LASER_PULSE_BSTPAUSE, TriggerBstPause);
        }

        private void ExecuteTriggerSeqBstSetButtonCommand()
        {
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.OUT_INT_LASER_PULSE_SEQBST, TriggerSeqBst);
            DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_LASER_PULSE_SEQBST, TriggerSeqBst);
        }

        private void ExecuteTriggerSeqPauseSetButtonCommand()
        {
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.OUT_INT_LASER_PULSE_SEQPAUSE, TriggerSeqPause);
            DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_LASER_PULSE_SEQPAUSE, TriggerSeqPause);
        }
        private void ExecuteUserCountsResetButtonCommand()
        {
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.OUT_INT_LASER_RESET_COUNTER, 1);
            DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_LASER_RESET_COUNTER, 1);
        }

        private void ExecuteFrontDoorInterlockActivate()
        {
            InterlockManager.Instance.SET_SETPOINT_INTERLOCK_USE(InterlockNameHelper.I_SP_DOOR_OPEN_FRONT, true);
            FrontDoorInterlockState = "작동중";
        }

        private void ExecuteFrontDoorInterlockDeactivate()
        {
            InterlockManager.Instance.SET_SETPOINT_INTERLOCK_USE(InterlockNameHelper.I_SP_DOOR_OPEN_FRONT, false);
            FrontDoorInterlockState = "해제중";
        }

        private void ExecuteLeftDoorInterlockActivate()
        {
            InterlockManager.Instance.SET_SETPOINT_INTERLOCK_USE(InterlockNameHelper.I_SP_DOOR_OPEN_LEFT, true);
            LeftDoorInterlockState = "작동중";
        }

        private void ExecuteLeftDoorInterlockDeactivate()
        {
            InterlockManager.Instance.SET_SETPOINT_INTERLOCK_USE(InterlockNameHelper.I_SP_DOOR_OPEN_LEFT, false);
            LeftDoorInterlockState = "해제중";
        }

        private void ExecuteRightDoorInterlockActivate()
        {
            InterlockManager.Instance.SET_SETPOINT_INTERLOCK_USE(InterlockNameHelper.I_SP_DOOR_OPEN_RIGHT, true);
            RightDoorInterlockState = "작동중";
        }

        private void ExecuteRightDoorInterlockDeactivate()
        {
            InterlockManager.Instance.SET_SETPOINT_INTERLOCK_USE(InterlockNameHelper.I_SP_DOOR_OPEN_RIGHT, false);
            RightDoorInterlockState = "해제중";
        }
    }
}
