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

        private double _XJogVelHigh;
        private double _XJogVelLow;
        private double _YJogVelHigh;
        private double _YJogVelLow;

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


        private List<string> _EnergyModeList;
        private List<string> _TriggerModeList;

        private ICommand _VisionPosXSetButtonCommand;
        private ICommand _VisionPosYSetButtonCommand;

        private ICommand _ProcPosXSetButtonCommand;
        private ICommand _ProcPosYSetButtonCommand;


        private ICommand _XJogVelHighButtonCommand;
        private ICommand _YJogVelHighButtonCommand;


        private ICommand _XJogVelLowButtonCommand;
        private ICommand _YJogVelLowButtonCommand;

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

        public List<string> EnergyModeList { get { return new List<string>() { "EGY NGR", "EGYBURST NGR", "HV NGR" }; } set { _EnergyModeList = value; } }
        public List<string> TriggerModeList { get { return new List<string>() { "INT", "INTB", "INT COUNTS" }; } set { _TriggerModeList = value; } }

        public string SelectedEnergyMode { get { return _SelectedEnergyMode; } set { _SelectedEnergyMode = value; RaisePropertyChanged("SelectedEnergyMode"); } }
        public string SelectedTriggerMode { get { return _SelectedTriggerMode; } set { _SelectedTriggerMode = value; RaisePropertyChanged("SelectedTriggerMode"); } }

        public double VisionPositionX { get { return _VisionPositionX; } set { _VisionPositionX = value; RaisePropertyChanged("VisionPositionX"); } }
        public double VisionPositionY { get { return _VisionPositionY; } set { _VisionPositionY = value; RaisePropertyChanged("VisionPositionY"); } }

        public double ProcessPositionX { get { return _ProcessPositionX; } set { _ProcessPositionX = value; RaisePropertyChanged("ProcessPositionX"); } }
        public double ProcessPositionY { get { return _ProcessPositionY; } set { _ProcessPositionY = value; RaisePropertyChanged("ProcessPositionY"); } }

        public double XJogVelHigh { get { return _XJogVelHigh; } set { _XJogVelHigh = value; RaisePropertyChanged("XJogVelHigh"); } }
        public double YJogVelHigh { get { return _YJogVelHigh; } set { _YJogVelHigh = value; RaisePropertyChanged("YJogVelHigh"); } }
        public double XJogVelLow { get { return _XJogVelLow; } set { _XJogVelLow = value; RaisePropertyChanged("XJogVelLow"); } }
        public double YJogVelLow { get { return _YJogVelLow; } set { _YJogVelLow = value; RaisePropertyChanged("YJogVelLow"); } }
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

        public ICommand XJogVelHighButtonCommand { get { if (_XJogVelHighButtonCommand == null) { _XJogVelHighButtonCommand = new DelegateCommand(ExecuteXJogVelHighButtonCommand); } return _XJogVelHighButtonCommand; } }
        public ICommand YJogVelHighButtonCommand { get { if (_YJogVelHighButtonCommand == null) { _YJogVelHighButtonCommand = new DelegateCommand(ExecuteYJogVelHighButtonCommand); } return _YJogVelHighButtonCommand; } }

        public ICommand XJogVelLowButtonCommand { get { if (_XJogVelLowButtonCommand == null) { _XJogVelLowButtonCommand = new DelegateCommand(ExecuteXJogVelLowButtonCommand); } return _XJogVelLowButtonCommand; } }
        public ICommand YJogVelLowButtonCommand { get { if (_YJogVelLowButtonCommand == null) { _YJogVelLowButtonCommand = new DelegateCommand(ExecuteYJogVelLowButtonCommand); } return _YJogVelLowButtonCommand; } }
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



        public SettingParameterViewModel()
        {
            VisionPositionX = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_VISION_POSITION, out bool _);
            VisionPositionY = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_VISION_POSITION, out bool _);

            ProcessPositionX = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_PROCESS_POSITION, out bool _);
            ProcessPositionY = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_PROCESS_POSITION, out bool _);

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

        private void ExecuteVisionPosXSetButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisX.VisionPosition", VisionPositionX);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisX.VisionPosition", VisionPositionX);
        }

        private void ExecuteVisionPosYSetButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisY.VisionPosition", VisionPositionY);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisY.VisionPosition", VisionPositionY);

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

        private void ExecuteXJogVelHighButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisX.JogVelHigh", XJogVelHigh);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisX.JogVelHigh", XJogVelHigh);
        }

        private void ExecuteYJogVelHighButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisY.JogVelHigh", XJogVelHigh);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisY.JogVelHigh", XJogVelHigh);
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
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisX.JogVelLow", XJogVelHigh);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisX.JogVelLow", XJogVelHigh);
        }

        private void ExecuteYJogVelLowButtonCommand()
        {
            DataManager.Instance.SET_DOUBLE_DATA("vSet.dAxisY.JogVelLow", XJogVelHigh);
            DataManager.Instance.CHANGE_DEFAULT_DATA("vSet.dAxisY.JogVelLow", XJogVelHigh);
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
    }
}
