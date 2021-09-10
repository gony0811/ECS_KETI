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
    public class OperationAutoViewModel : ViewModelBase
    {
        private Timer _Timer;
        private double _XAxisPosition;
        private double _YAxisPosition;

        private string _LaserOpMode;
        private string _LaserEnergyMode;
        private string _LaserInterlock;

        private double _LaserHV;
        private double _LaserEnergy;
        private int _TubeManPressure;
        private int _TubePressure;
        private double _TubeTemperature;
        private bool _IsEnableProcessButton;
        private bool _IsEnableInitButton;

        public bool IsEnableProcessButton { get { return _IsEnableProcessButton; } set { _IsEnableProcessButton = value; RaisePropertyChanged("IsEnableProcessButton"); } }
        public bool IsEnableInitButton { get { return _IsEnableInitButton; } set { _IsEnableInitButton = value; RaisePropertyChanged("IsEnableInitButton"); } }

        public string LaserOpMode { get { return _LaserOpMode; } set { _LaserOpMode = value; RaisePropertyChanged("LaserOpMode"); } }
        public string LaserEnergyMode { get { return _LaserEnergyMode; } set { _LaserEnergyMode = value; RaisePropertyChanged("LaserEnergyMode"); } }
        public string LaserInterlock { get { return _LaserInterlock; } set { _LaserInterlock = value; RaisePropertyChanged("LaserInterlock"); } }
        public double XAxisPosition { get { return _XAxisPosition; } set { _XAxisPosition = value; RaisePropertyChanged("XAxisPosition"); } }
        public double YAxisPosition { get { return _YAxisPosition; } set { _YAxisPosition = value; RaisePropertyChanged("YAxisPosition"); } }

        public double LaserHV { get { return _LaserHV; } set { _LaserHV = value; RaisePropertyChanged("LaserHV"); } }
        public double LaserEnergy { get { return _LaserEnergy; } set { _LaserEnergy = value; RaisePropertyChanged("LaserEnergy"); } }
        public int TubePressure { get { return _TubePressure; } set { _TubePressure = value; RaisePropertyChanged("TubePressure"); } }
        public int TubeManPressure { get { return _TubeManPressure; } set { _TubeManPressure = value; RaisePropertyChanged("TubeManPressure"); } }
        public double TubeTemperature { get { return _TubeTemperature; } set { _TubeTemperature = value; RaisePropertyChanged("TubeTemperarure"); } }

        private ICommand _LoadedCommand;
        private ICommand _UnloadedCommand;
        private ICommand _ProcessButtonCommand;
        private ICommand _InitButtonCommand;

        public ICommand LoadedCommand { get { return this._LoadedCommand ?? (this._LoadedCommand = new DelegateCommand(ExecuteLoadedCommand)); } }
        public ICommand UnloadedCommand { get { return this._UnloadedCommand ?? (this._UnloadedCommand = new DelegateCommand(ExecuteUnloadedCommand)); } }
        public ICommand ProcessButtonCommand { get { return this._ProcessButtonCommand ?? (this._ProcessButtonCommand = new DelegateCommand(ExecuteProcessButtonCommand)); } }
        public ICommand InitButtonCommand { get { return this._InitButtonCommand ?? (this._InitButtonCommand = new DelegateCommand(ExecuteInitButtonCommand)); } }


        public OperationAutoViewModel()
        {
            _IsEnableInitButton = true;
            _IsEnableProcessButton = false;
        }

        private void ExecuteProcessButtonCommand()
        {
            if (DataManager.Instance.GET_STRING_DATA(IoNameHelper.V_STR_SYS_OPERATION_MODE, out bool _) == "AUTO")
            {
                StringBuilder message = new StringBuilder();

                message.AppendLine("현재 설정된 Recipe로 Process를 시작하시겠습니까?");
                message.AppendLine("Substrate를 가공시작 위치에 정확히 Align 하세요.");

                if (MessageBoxManager.ShowYesNoBox(message.ToString(), "Do you really want to start processing ?") == MSGBOX_RESULT.OK)
                {
                    MessageBoxManager.ShowProgressWindow("AUTO PROCESSING", "Wait for a moments...", FuncNameHelper.AUTO_PROCESS);
                }
            }
        }

        private void ExecuteInitButtonCommand()
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("Do you really want to initialize equipment?");

            if (MessageBoxManager.ShowYesNoBox(message.ToString(), "INIT START") == MSGBOX_RESULT.OK)
            {
                PROCESS_RESULT result = MessageBoxManager.ShowProgressWindow("INIT PROCESSING", "Wait for a moments...", FuncNameHelper.INIT_PROCESS);

                if (result == PROCESS_RESULT.SUCCESS)
                {
                    IsEnableInitButton = false;
                    IsEnableProcessButton = true;
                }
            }

        }

        private void ExecuteLoadedCommand()
        {
            Start();
        }

        private void ExecuteUnloadedCommand()
        {
            Stop();
        }


        public void Start()
        {
            _Timer = new Timer(OperationAutoViewTimerEvent, this, 0, 500);
        }

        public void Stop()
        {
            _Timer.Dispose();
        }

        private void OperationAutoViewTimerEvent(object state)
        {
            XAxisPosition = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.IN_DBL_PMAC_X_POSITION, out _);
            YAxisPosition = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.IN_DBL_PMAC_Y_POSITION, out _);
            LaserOpMode = DataManager.Instance.GET_STRING_DATA(IoNameHelper.IN_STR_LASER_OPMODE_STATUS, out _);
            LaserEnergyMode = DataManager.Instance.GET_STRING_DATA(IoNameHelper.IN_STR_LASER_EGYMODE_STATUS, out _);
            LaserEnergy = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.IN_DBL_LASER_ENERGY_EGYSET, out _);
            LaserHV = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.IN_DBL_LASER_ENERGY_HV, out _);
            LaserInterlock = DataManager.Instance.GET_STRING_DATA(IoNameHelper.IN_STR_LASER_STATUS_INTERLOCK, out _);

            TubeManPressure = DataManager.Instance.GET_INT_DATA(IoNameHelper.IN_INT_LASER_STATUS_MANPRESSURE, out _);
            TubePressure = DataManager.Instance.GET_INT_DATA(IoNameHelper.IN_INT_LASER_STATUS_TUBEPRESSURE, out _);
            TubeTemperature = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.IN_DBL_LASER_STATUS_TUBETEMP, out _);
        }


    }
}
