using ECS.Common.Helper;
using GalaSoft.MvvmLight;
using INNO6.Core.Manager;
using INNO6.IO;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ECS.UI.ViewModel
{
    public class LaserControlViewModel : ViewModelBase
    {
        private List<string> _EnergyModeList;
        private string _SelectedEnergyMode;

        private string _LaserOffButtonContent;
        private string _LaserStandByButtonContent;
        private string _LaserOnButtonContent;
        private string _OpModeText;
        private string _InterlockText;

        private bool _LaserOffButtonEnable;
        private bool _LaserOnButtonEnable;
        private bool _LaserStandByButtonEnable;

        private int _TubePressure;
        private int _TubeManPressure;
        private double _TubeTemperature;

        private double _LaserHV;
        private double _LaserEnergy;

        private ICommand _EnergyModeSelectionChanged;
        private ICommand _LaserEnergyTextChangedCommand;
        private ICommand _LaserHVTextChangedCommand;

        private ICommand _LaserOffButtonCommand;
        private ICommand _LaserOnButtonCommand;
        private ICommand _LaserStandByButtonCommand;

        private Timer _Timer;
        public List<string> EnergyModeList { get { return new List<string>() { "EGY NGR", "EGYBURST NGR", "HV NGR" }; } set { _EnergyModeList = value; } }
        public string SelectedEnergyMode { get { return _SelectedEnergyMode; } set { _SelectedEnergyMode = value; RaisePropertyChanged("SelectedEnergyMode"); } }

        public string LaserOffButtonContent { get { return _LaserOffButtonContent; } set { _LaserOffButtonContent = value; RaisePropertyChanged("LaserOffButtonContent"); } }
        public string LaserOnButtonContent { get { return _LaserOnButtonContent; } set { _LaserOnButtonContent = value; RaisePropertyChanged("LaserOnButtonContent"); } }
        public string LaserStandByButtonContent { get { return _LaserStandByButtonContent; } set { _LaserStandByButtonContent = value; RaisePropertyChanged("LaserStandByButtonContent"); } }
        public string OpModeText { get { return _OpModeText; } set { _OpModeText = value; RaisePropertyChanged("OpModeText"); } }

        public string InterlockText { get { return _InterlockText; } set { _InterlockText = value; RaisePropertyChanged("InterlockText"); } }



        public bool LaserOffButtonEnable { get { return _LaserOffButtonEnable; } set { _LaserOffButtonEnable = value; RaisePropertyChanged("LaserOffButtonEnable"); } }
        public bool LaserOnButtonEnable { get { return _LaserOnButtonEnable; } set { _LaserOnButtonEnable = value; RaisePropertyChanged("LaserOnButtonEnable"); } }
        public bool LaserStandByButtonEnable { get { return _LaserStandByButtonEnable; } set { _LaserStandByButtonEnable = value; RaisePropertyChanged("LaserStandByButtonEnable"); } }

        public double LaserHV { get { return _LaserHV; } set { _LaserHV = value; RaisePropertyChanged("LaserHV"); } }
        public double LaserEnergy { get { return _LaserEnergy; } set { _LaserEnergy = value; RaisePropertyChanged("LaserEnergy"); } }

        public int TubePressure { get { return _TubePressure; } set { _TubePressure = value; RaisePropertyChanged("TubePressure"); } }
        public int TubeManPressure { get { return _TubeManPressure; } set { _TubeManPressure = value; RaisePropertyChanged("TubeManPressure"); } }
        public double TubeTemperature { get { return _TubeTemperature; } set { _TubeTemperature = value; RaisePropertyChanged("TubeTemperature"); } }

        public ICommand EnergyModeSelectionChanged { get { if (_EnergyModeSelectionChanged == null) { _EnergyModeSelectionChanged = new DelegateCommand(ExecuteEnergyModeSelectionChanged); } return _EnergyModeSelectionChanged; } }
        public ICommand LaserEnergyTextChangedCommand { get { if (_LaserEnergyTextChangedCommand == null) { _LaserEnergyTextChangedCommand = new DelegateCommand(ExecuteLaserEnergyTextChanged); } return _LaserEnergyTextChangedCommand; } }
        public ICommand LaserHVTextChangedCommand { get { if (_LaserHVTextChangedCommand == null) { _LaserHVTextChangedCommand = new DelegateCommand(ExecuteLaserHVTextChanged); } return _LaserHVTextChangedCommand; } }
        public ICommand LaserOffButtonCommand { get { if (_LaserOffButtonCommand == null) { _LaserOffButtonCommand = new DelegateCommand(ExecuteLaserOffButtonCommand); } return _LaserOffButtonCommand; } }
        public ICommand LaserOnButtonCommand { get { if (_LaserOnButtonCommand == null) { _LaserOnButtonCommand = new DelegateCommand(ExecuteLaserOnButtonCommand); } return _LaserOnButtonCommand; } }
        public ICommand LaserStandByButtonCommand { get { if (_LaserStandByButtonCommand == null) { _LaserStandByButtonCommand = new DelegateCommand(ExecuteLaserStandByButtonCommand); } return _LaserStandByButtonCommand; } }

        private ICommand _LoadedCommand;
        private ICommand _UnloadedCommand;

        public ICommand LoadedCommand { get { return this._LoadedCommand ?? (this._LoadedCommand = new DelegateCommand(ExecuteLoadedCommand)); } }
        public ICommand UnloadedCommand { get { return this._UnloadedCommand ?? (this._UnloadedCommand = new DelegateCommand(ExecuteUnloadedCommand)); } }



        public LaserControlViewModel()
        {
            SelectedEnergyMode = DataManager.Instance.GET_STRING_DATA(IoNameHelper.IN_STR_LASER_EGYMODE_STATUS, out bool _);

            LaserEnergy = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.IN_DBL_LASER_ENERGY_EGYSET, out bool _);
            LaserHV = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.IN_DBL_LASER_ENERGY_HV, out bool _);

            LaserOnButtonContent = "ON";
            LaserOffButtonContent = "OFF";
            LaserStandByButtonContent = "STANDBY";

            LaserOffButtonEnable = true;
            LaserOnButtonEnable = true;
            LaserStandByButtonEnable = true;
        }

        private void ExecuteLoadedCommand()
        {
            Start();
        }

        private void ExecuteUnloadedCommand()
        {
            Stop();
        }

        private void Start()
        {
            _Timer = new Timer(LaserControlViewSchedulingTimmer, this, 0, 1000);
        }

        private void Stop()
        {
            _Timer.Dispose();
        }

        private void LaserControlViewSchedulingTimmer(object state)
        {
            InterlockText = DataManager.Instance.GET_STRING_DATA(IoNameHelper.IN_STR_LASER_STATUS_INTERLOCK, out bool _);

            OpModeText = DataManager.Instance.GET_STRING_DATA(IoNameHelper.IN_STR_LASER_OPMODE_STATUS, out bool _);

            TubePressure = DataManager.Instance.GET_INT_DATA(IoNameHelper.IN_INT_LASER_STATUS_TUBEPRESSURE, out bool _);

            TubeManPressure = DataManager.Instance.GET_INT_DATA(IoNameHelper.IN_INT_LASER_STATUS_MANPRESSURE, out bool _);

            TubeTemperature = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.IN_DBL_LASER_STATUS_TUBETEMP, out bool _);

            SelectedEnergyMode = DataManager.Instance.GET_STRING_DATA(IoNameHelper.IN_STR_LASER_EGYMODE_STATUS, out bool _);

            LaserEnergy = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.IN_DBL_LASER_ENERGY_EGYSET, out bool _);

            LaserHV = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.IN_DBL_LASER_ENERGY_HV, out bool _);

            //if (OpModeText == "OFF")
            //{
            //    LaserOffButtonEnable = false;
            //    LaserOnButtonEnable = true;
            //    LaserStandByButtonEnable = true;
            //}
            //else if(OpModeText == "ON")
            //{
            //    LaserOffButtonEnable = true;
            //    LaserOnButtonEnable = false;
            //    LaserStandByButtonEnable = true;
            //}
            //else if (OpModeText == "STANDBY")
            //{
            //    LaserOffButtonEnable = true;
            //    LaserOnButtonEnable = true;
            //    LaserStandByButtonEnable = false;
            //}
            //else
            //{
            //    LaserOffButtonEnable = true;
            //    LaserOnButtonEnable = false;
            //    LaserStandByButtonEnable = false;
            //}
        }

        private void ExecuteEnergyModeSelectionChanged()
        {
            //if (SelectedEnergyMode.Equals("EGY NGR"))
            //{
            //    FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.SET_MODE_EGYNGR);
            //}
            //else if (SelectedEnergyMode.Equals("EGYBURST NGR"))
            //{
            //    FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.SET_MODE_EGYBURSTNGR);
            //}
            //else if (SelectedEnergyMode.Equals("HV NGR"))
            //{
            //    FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.SET_MODE_HVNGR);
            //}
        }

        private void ExecuteLaserHVTextChanged()
        {
            //DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.OUT_DBL_LASER_ENERGY_HV, LaserEnergy);
        }

        private void ExecuteLaserEnergyTextChanged()
        {
            //DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.OUT_DBL_LASER_ENERGY_EGYSET, LaserEnergy);
        }

        private void ExecuteLaserOffButtonCommand()
        {
            //FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.LASER_OFF);
            MessageBoxManager.ShowProgressRingWindow("LASER OFF Processing..", FuncNameHelper.LASER_OFF);
        }

        private void ExecuteLaserOnButtonCommand()
        {
            //FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.LASER_ON);
            MessageBoxManager.ShowProgressRingWindow("LASER ON Processing..", FuncNameHelper.LASER_OFF);
        }

        private void ExecuteLaserStandByButtonCommand()
        {
            //FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.LASER_STANDBY);
            MessageBoxManager.ShowProgressRingWindow("LASER STANDBY Processing..", FuncNameHelper.LASER_STANDBY);
        }
    }
}
