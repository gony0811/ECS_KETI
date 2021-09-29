using ECS.Common.Helper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using INNO6.Core.Manager;
using INNO6.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ECS.UI.ViewModel
{
    public class MotionControlViewModel : ViewModelBase
    {
        #region Define const. variable (IO NAME, FUNCTION NAME)
        private const string V_DBL_X_ABS_POSITION = "vSet.dAxisX.AbsPosition";
        private const string V_DBL_Y_ABS_POSITION = "vSet.dAxisY.AbsPosition";

        private const string V_DBL_X_MAX_POSITION = "vSet.dAxisX.MaxPosition";
        private const string V_DBL_Y_MAX_POSITION = "vSet.dAxisY.MaxPosition";

        private const string V_DBL_X_MIN_POSITION = "vSet.dAxisX.MinPosition";
        private const string V_DBL_Y_MIN_POSITION = "vSet.dAxisY.MinPosition";

        private const string V_DBL_X_MAX_VELOCITY = "vSet.dAxisX.MaxVelocity";
        private const string V_DBL_Y_MAX_VELOCITY = "vSet.dAxisY.MaxVelocity";

        private const string V_DBL_X_MIN_VELOCITY = "vSet.dAxisX.MinVelocity";
        private const string V_DBL_Y_MIN_VELOCITY = "vSet.dAxisY.MinVelocity";

        private const string V_DBL_X_REL_DISTANCE = "vSet.dAxisX.RelDistance";
        private const string V_DBL_Y_REL_DISTANCE = "vSet.dAxisY.RelDistance";

        private const string V_DBL_X_ABS_VELOCITY = "vSet.dAxisX.AbsVelocity";
        private const string V_DBL_Y_ABS_VELOCITY = "vSet.dAxisY.AbsVelocity";

        private const string V_DBL_X_REL_VELOCITY = "vSet.dAxisX.RelVelocity";
        private const string V_DBL_Y_REL_VELOCITY = "vSet.dAxisY.RelVelocity";


        private const string V_STR_X_JOGVEL_MODE = "vSet.sAxisX.JogVelMode";
        private const string V_STR_Y_JOGVEL_MODE = "vSet.sAxisY.JogVelMode";

        private const string V_DBL_X_JOGVEL_HIGH = "vSet.dAxisX.JogVelHigh";
        private const string V_DBL_Y_JOGVEL_HIGH = "vSet.dAxisY.JogVelHigh";

        private const string V_DBL_X_JOGVEL_LOW = "vSet.dAxisX.JogVelLow";
        private const string V_DBL_Y_JOGVEL_LOW = "vSet.dAxisY.JogVelLow";

        private const string INPUT_X_ISHOMMING = "iPMAC.iAxisX.IsHomming";
        private const string INPUT_Y_ISHOMMING = "iPMAC.iAxisY.IsHomming";

        private const string INPUT_X_ISHOME = "iPMAC.iAxisX.IsHome";
        private const string INPUT_Y_ISHOME = "iPMAC.iAxisY.IsHome";

        private const string INPUT_X_ISMOVING = "iPMAC.iAxisX.IsMoving";
        private const string INPUT_Y_ISMOVING = "iPMAC.iAxisY.IsMoving";

        private const string INPUT_X_VELOCITY = "iPMAC.dAxisX.Velocity";
        private const string INPUT_Y_VELOCITY = "iPMAC.dAxisY.Velocity";

        private const string INPUT_X_POSITION = "iPMAC.dAxisX.Position";
        private const string INPUT_Y_POSITION = "iPMAC.dAxisY.Position";

        private const string F_X_AXIS_HOMMING = "F_X_AXIS_HOMMING";
        private const string F_Y_AXIS_HOMMING = "F_Y_AXIS_HOMMING";

        private const string F_X_AXIS_HOME_STOP = "F_X_AXIS_HOME_STOP";
        private const string F_Y_AXIS_HOME_STOP = "F_Y_AXIS_HOME_STOP";

        private const string F_X_AXIS_JOG_PLUS = "F_X_AXIS_JOG_PLUS";
        private const string F_X_AXIS_JOG_MINUS = "F_X_AXIS_JOG_MINUS";
        private const string F_X_AXIS_JOG_STOP = "F_X_AXIS_JOG_STOP";
        private const string F_X_AXIS_ABSOLUTE_MOVE = "F_X_AXIS_MOVE_TO_SETPOS";
        private const string F_X_AXIS_RELATIVE_MOVE = "F_X_AXIS_MOVE_TO_SETDIS";

        private const string F_Y_AXIS_JOG_PLUS = "F_Y_AXIS_JOG_PLUS";
        private const string F_Y_AXIS_JOG_MINUS = "F_Y_AXIS_JOG_MINUS";
        private const string F_Y_AXIS_JOG_STOP = "F_Y_AXIS_JOG_STOP";
        private const string F_Y_AXIS_ABSOLUTE_MOVE = "F_Y_AXIS_MOVE_TO_SETPOS";
        private const string F_Y_AXIS_RELATIVE_MOVE = "F_Y_AXIS_MOVE_TO_SETDIS";


        #endregion

        #region Define Private Variable
        private Timer _Timer;

        private string _ButtonHommingContent;
        private string _ButtonServoKillAllContent;
        private string _ButtonServoContent;
        private string _ButtonJogPlusContent;
        private string _ButtonJogMinusContent;
        private string _ButtonVisionMoveContent;
        private string _ButtonProcessMoveContent;
        private string _ButtonMoveStopContent;
        private string _TextBlockJogSpeedHighLow;

        private string _LabelAbsoluteMove;
        private double _AbsolutePosition;
        private double _AbsoluteVelocity;
        private string _ButtonAbsoluteMoveContent;

        private string _LabelRelativeMove;
        private double _MoveDistance;
        private double _RelativeVelocity;
        private string _ButtonRelativePlusMoveContent;
        private string _ButtonRelativeMinusMoveContent;

        private bool _ToggleButtonJogSpeedHighLow;
        private bool _ButtonHommingEnable;
        private bool _ButtonServoEnable;
        private bool _ButtonServoKillAllEnable;
        private bool _ButtonJogPlusEnable;
        private bool _ButtonJogMinusEnable;
        private bool _ButtonAbsoluteMoveEnable;
        private bool _ButtonRelativeMoveEnable;
        private bool _ButtonVisionMoveEnable;
        private bool _ButtonMoveStopEnable;
        private bool _ButtonProcessMoveEnable;

        private double _XAxisVelocity;
        private double _YAxisVelocity;

        private double _XAxisPosition;
        private double _YAxisPosition;


        private double _PostionLimitMax;
        private double _PostionLimitMin;
        private double _VelocityLimitMax;
        private double _VelocityLimitMin;
        private double _PositionInterval;
        private double _VelocityInterval;

        private bool _RadioButtonXAxisIsChecked;
        private bool _RadioButtonYAxisIsChecked;

        private ICommand _RadioButtonXAxisCheckedCommand;
        private ICommand _RadioButtonYAxisCheckedCommand;


        private ICommand _ButtonHommingCommand;
        private ICommand _ButtonServoCommand;
        private ICommand _ButtonServoKillAllCommand;
        private ICommand _JogPlusPreviewMouseLeftButtonUpCommand;
        private ICommand _JogPlusPreviewMouseLeftButtonDownCommand;
        private ICommand _JogMinusPreviewMouseLeftButtonUpCommand;
        private ICommand _JogMinusPreviewMouseLeftButtonDownCommand;
        private ICommand _ButtonAbsoluteMoveCommand;
        private ICommand _ButtonRelativePlusMoveCommand;
        private ICommand _ButtonRelativeMinusMoveCommand;

        private ICommand _JogSpeedHighCommand;
        private ICommand _JogSpeedLowCommand;

        private ICommand _RelativeVelocityInputCommand;
        private ICommand _MoveDistanceInputCommand;
        private ICommand _AbsoluteVelocityInputCommand;
        private ICommand _AbsolutePositionInputCommand;

        private ICommand _ButtonVisionMoveCommand;
        private ICommand _ButtonProcessMoveCommand;
        private ICommand _ButtonMoveStopCommand;

        private ICommand _LoadedCommand;
        private ICommand _UnloadedCommand;

        #endregion

        #region Define Constructor
        public MotionControlViewModel()
        {
            RadioButtonXAxisIsChecked = true;
            ExecuteXAxisCheckedCommand();


            ButtonVisionMoveContent = "To Vision";
            ButtonProcessMoveContent = "To Process";
            ButtonMoveStopContent = "Move Stop";

            ButtonServoKillAllContent = "KILL ALL";

            ButtonVisionMoveEnable = true;
            ButtonProcessMoveEnable = true;
            ButtonMoveStopEnable = true;
            ButtonServoKillAllEnable = true;
        }
        #endregion


        public void Start()
        {
            _Timer = new Timer(MotionControlViewSchedulingTimmer, this, 0, 500);
        }

        public void Stop()
        {
            _Timer.Dispose();
        }


        #region Define Public Properties
        public string ButtonHommingContent { get { return _ButtonHommingContent; } set { _ButtonHommingContent = value; RaisePropertyChanged("ButtonHommingContent"); } }
        public string ButtonServoContent { get { return _ButtonServoContent; } set { _ButtonServoContent = value; RaisePropertyChanged("ButtonServoContent"); } }
        public string ButtonServoKillAllContent { get { return _ButtonServoKillAllContent; } set { _ButtonServoKillAllContent = value; RaisePropertyChanged("ButtonServoKillAllContent"); } }
        public string ButtonJogPlusContent { get { return _ButtonJogPlusContent; } set { _ButtonJogPlusContent = value; RaisePropertyChanged("ButtonJogPlusContent"); } }
        public string ButtonJogMinusContent { get { return _ButtonJogMinusContent; } set { _ButtonJogMinusContent = value; RaisePropertyChanged("ButtonJogMinusContent"); } }
        public string ButtonAbsoluteMoveContent { get { return _ButtonAbsoluteMoveContent; } set { _ButtonAbsoluteMoveContent = value; RaisePropertyChanged("ButtonAbsoluteMoveContent"); } }
        public string ButtonRelativePlusMoveContent { get { return _ButtonRelativePlusMoveContent; } set { _ButtonRelativePlusMoveContent = value; RaisePropertyChanged("ButtonRelativePlusMoveContent"); } }
        public string ButtonRelativeMinusMoveContent { get { return _ButtonRelativeMinusMoveContent; } set { _ButtonRelativeMinusMoveContent = value; RaisePropertyChanged("ButtonRelativeMinusMoveContent"); } }
        public string ButtonVisionMoveContent { get { return _ButtonVisionMoveContent; } set { _ButtonVisionMoveContent = value; RaisePropertyChanged("ButtonVisionMoveContent"); } }
        public string ButtonProcessMoveContent { get { return _ButtonProcessMoveContent; } set { _ButtonProcessMoveContent = value; RaisePropertyChanged("ButtonProcessMoveContent"); } }
        public string ButtonMoveStopContent { get { return _ButtonMoveStopContent; } set { _ButtonMoveStopContent = value; RaisePropertyChanged("ButtonMoveStopContent"); } }


        public string LabelAbsoluteMove { get { return _LabelAbsoluteMove; } set { _LabelAbsoluteMove = value; RaisePropertyChanged("LabelAbsoluteMove"); } }
        public double AbsolutePosition { get { return _AbsolutePosition; } set { if (_AbsolutePosition != value) { _AbsolutePosition = value; RaisePropertyChanged("AbsolutePosition"); } } }
        public double AbsoluteVelocity { get { return _AbsoluteVelocity; } set { _AbsoluteVelocity = value; RaisePropertyChanged("AbsoluteVelocity"); } }

        public string LabelRelativeMove { get { return _LabelRelativeMove; } set { _LabelRelativeMove = value; RaisePropertyChanged("LabelRelativeMove"); } }
        public double MoveDistance { get { return _MoveDistance; } set { _MoveDistance = value; RaisePropertyChanged("MoveDistance"); } }
        public double RelativeVelocity { get { return _RelativeVelocity; } set { _RelativeVelocity = value; RaisePropertyChanged("RelativeVelocity"); } }

        public bool ButtonHommingEnable { get { return _ButtonHommingEnable; } set { _ButtonHommingEnable = value; RaisePropertyChanged("ButtonHommingEnable"); } }
        public bool ButtonServoEnable { get { return _ButtonServoEnable; } set { _ButtonServoEnable = value; RaisePropertyChanged("ButtonServoEnable"); } }
        public bool ButtonServoKillAllEnable { get { return _ButtonServoKillAllEnable; } set { _ButtonServoKillAllEnable = value; RaisePropertyChanged("ButtonServoKillAllEnable"); } }
        public bool ButtonJogPlusEnable { get { return _ButtonJogPlusEnable; } set { _ButtonJogPlusEnable = value; RaisePropertyChanged("ButtonJogPlusEnable"); } }
        public bool ButtonJogMinusEnable { get { return _ButtonJogMinusEnable; } set { _ButtonJogMinusEnable = value; RaisePropertyChanged("ButtonJogMinusEnable"); } }

        public bool ButtonAbsoluteMoveEnable { get { return _ButtonAbsoluteMoveEnable; } set { _ButtonAbsoluteMoveEnable = value; RaisePropertyChanged("ButtonAbsoluteMoveEnable"); } }
        public bool ButtonRelativeMoveEnable { get { return _ButtonRelativeMoveEnable; } set { _ButtonRelativeMoveEnable = value; RaisePropertyChanged("ButtonRelativeMoveEnable"); } }
        public bool ButtonVisionMoveEnable { get { return _ButtonVisionMoveEnable; } set { _ButtonVisionMoveEnable = value; RaisePropertyChanged("ButtonVisionMoveEnable"); } }
        public bool ButtonProcessMoveEnable { get { return _ButtonProcessMoveEnable; } set { _ButtonProcessMoveEnable = value; RaisePropertyChanged("ButtonProcessMoveEnable"); } }
        public bool ButtonMoveStopEnable { get { return _ButtonMoveStopEnable; } set { _ButtonMoveStopEnable = value; RaisePropertyChanged("ButtonMoveStopEnable"); } }
        public bool ToggleButtonJogSpeedHighLow { get { return _ToggleButtonJogSpeedHighLow; } set { _ToggleButtonJogSpeedHighLow = value; RaisePropertyChanged("ToggleButtonJogSpeedHighLow"); } }

        public double PositionLimitMax { get { return _PostionLimitMax; } set { _PostionLimitMax = value; RaisePropertyChanged("PositionLimitMax"); } }
        public double PositionLimitMin { get { return _PostionLimitMin; } set { _PostionLimitMin = value; RaisePropertyChanged("PositionLimitMin"); } }

        public double VelocityLimitMax { get { return _VelocityLimitMax; } set { _VelocityLimitMax = value; RaisePropertyChanged("VelocityLimitMax"); } }
        public double VelocityLimitMin { get { return _VelocityLimitMin; } set { _VelocityLimitMin = value; RaisePropertyChanged("VelocityLimitMin"); } }

        public double PositionInterval { get { return _PositionInterval; } set { _PositionInterval = value; RaisePropertyChanged("PositionInterval"); } }
        public double VelocityInterval { get { return _VelocityInterval; } set { _VelocityInterval = value; RaisePropertyChanged("VelocityInterval"); } }

        public double XAxisVelocity { get { return _XAxisVelocity; } set { _XAxisVelocity = value; RaisePropertyChanged("XAxisVelocity"); } }
        public double YAxisVelocity { get { return _YAxisVelocity; } set { _YAxisVelocity = value; RaisePropertyChanged("YAxisVelocity"); } }

        public double XAxisPosition { get { return _XAxisPosition; } set { _XAxisPosition = value; RaisePropertyChanged("XAxisPosition"); } }
        public double YAxisPosition { get { return _YAxisPosition; } set { _YAxisPosition = value; RaisePropertyChanged("YAxisPosition"); } }

        public string TextBlockJogSpeedHighLow { get { return _TextBlockJogSpeedHighLow; } set { _TextBlockJogSpeedHighLow = value; RaisePropertyChanged("TextBlockJogSpeedHighLow"); } }

        public bool RadioButtonXAxisIsChecked { get { return _RadioButtonXAxisIsChecked; } set { _RadioButtonXAxisIsChecked = value; RaisePropertyChanged("RadioButtonXAxisChecked"); } }
        public bool RadioButtonYAxisIsChecked { get { return _RadioButtonYAxisIsChecked; } set { _RadioButtonYAxisIsChecked = value; RaisePropertyChanged("RadioButtonYAxisChecked"); } }


        public ICommand ButtonHommingCommand { get { return this._ButtonHommingCommand ?? (this._ButtonHommingCommand = new RelayCommand(ExecuteHommingCommand)); } }
        public ICommand ButtonServoCommand { get { return this._ButtonServoCommand ?? (this._ButtonServoCommand = new RelayCommand(ExecuteServoCommand)); } }
        public ICommand ButtonServoKillAllCommand { get { return this._ButtonServoKillAllCommand ?? (this._ButtonServoKillAllCommand = new RelayCommand(ExecuteServoKillAllCommand)); } }

        public ICommand JogSpeedHighCommand { get { return this._JogSpeedHighCommand ?? (this._JogSpeedHighCommand = new RelayCommand(ExecuteJogSpeedHighCommand)); } }
        public ICommand JogSpeedLowCommand { get { return this._JogSpeedLowCommand ?? (this._JogSpeedLowCommand = new RelayCommand(ExecuteJogSpeedLowCommand)); } }
        public ICommand ButtonAbsoluteMoveCommand { get { return this._ButtonAbsoluteMoveCommand ?? (this._ButtonAbsoluteMoveCommand = new RelayCommand(ExecuteAbsoluteMoveCommand)); } }
        public ICommand ButtonRelativePlusMoveCommand { get { return this._ButtonRelativePlusMoveCommand ?? (this._ButtonRelativePlusMoveCommand = new RelayCommand(ExecuteRelativePlusMoveCommand)); } }
        public ICommand ButtonRelativeMinusMoveCommand { get { return this._ButtonRelativeMinusMoveCommand ?? (this._ButtonRelativeMinusMoveCommand = new RelayCommand(ExecuteRelativeMinusMoveCommand)); } }


        public ICommand JogPlusPreviewMouseLeftButtonUpCommand { get { return this._JogPlusPreviewMouseLeftButtonUpCommand ?? (this._JogPlusPreviewMouseLeftButtonUpCommand = new RelayCommand(ExecuteJogPlusMouseLeftButtonUpCommand)); } }
        public ICommand JogPlusPreviewMouseLeftButtonDownCommand { get { return this._JogPlusPreviewMouseLeftButtonDownCommand ?? (this._JogPlusPreviewMouseLeftButtonDownCommand = new RelayCommand(ExecuteJogPlusMouseLeftButtonDownCommand)); } }
        public ICommand JogMinusPreviewMouseLeftButtonUpCommand { get { return this._JogMinusPreviewMouseLeftButtonUpCommand ?? (this._JogMinusPreviewMouseLeftButtonUpCommand = new RelayCommand(ExecuteJogMinusMouseLeftButtonUpCommand)); } }
        public ICommand JogMinusPreviewMouseLeftButtonDownCommand { get { return this._JogMinusPreviewMouseLeftButtonDownCommand ?? (this._JogMinusPreviewMouseLeftButtonDownCommand = new RelayCommand(ExecuteJogMinusMouseLeftButtonDownCommand)); } }

        public ICommand RadioButtonXAxisCheckedCommand { get { return this._RadioButtonXAxisCheckedCommand ?? (this._RadioButtonXAxisCheckedCommand = new RelayCommand(ExecuteXAxisCheckedCommand)); } }
        public ICommand RadioButtonYAxisCheckedCommand { get { return this._RadioButtonYAxisCheckedCommand ?? (this._RadioButtonYAxisCheckedCommand = new RelayCommand(ExecuteYAxisCheckedCommand)); } }
    
        public ICommand RelativeVelocityInputCommand { get { return this._RelativeVelocityInputCommand ?? (this._RelativeVelocityInputCommand = new RelayCommand(ExecuteRelativeVelocityInputCommand)); } }
        public ICommand MoveDistanceInputCommand { get { return this._MoveDistanceInputCommand ?? (this._MoveDistanceInputCommand = new RelayCommand(ExecuteMoveDistanceInputCommand)); } }
        public ICommand AbsoluteVelocityInputCommand { get { return this._AbsoluteVelocityInputCommand ?? (this._AbsoluteVelocityInputCommand = new RelayCommand(ExecuteAbsoluteVelocityInputCommand)); } }
        public ICommand AbsolutePositionInputCommand { get { return this._AbsolutePositionInputCommand ?? (this._AbsolutePositionInputCommand = new RelayCommand(ExecuteAbsolutePositionInputCommand)); } }

        public ICommand ButtonVisionMoveCommand { get { return this._ButtonVisionMoveCommand ?? (this._ButtonVisionMoveCommand = new RelayCommand(ExecuteVisionMoveCommand)); } }
        public ICommand ButtonProcessMoveCommand { get { return this._ButtonProcessMoveCommand ?? (this._ButtonProcessMoveCommand = new RelayCommand(ExecuteProcessMoveCommand)); } }
        public ICommand ButtonMoveStopCommand { get { return this._ButtonMoveStopCommand ?? (this._ButtonMoveStopCommand = new RelayCommand(ExecuteMoveStopCommand)); } }


        public ICommand LoadedCommand { get { return this._LoadedCommand ?? (this._LoadedCommand = new RelayCommand(ExecuteLoadedCommand)); } }
        public ICommand UnloadedCommand { get { return this._UnloadedCommand ?? (this._UnloadedCommand = new RelayCommand(ExecuteUnloadedCommand)); } }


        #endregion

        #region Define Private Method

        private void ExecuteLoadedCommand()
        {
            Start();
        }

        private void ExecuteUnloadedCommand()
        {
            Stop();
        }

        private void MotionControlViewSchedulingTimmer(object state)
        {
            //HommingButtonStatusChack();
            AbsoluteMoveButtonStatusCheck();
            MotionStatusCheck();
        }

        private void AbsoluteMoveButtonStatusCheck()
        {
            if (RadioButtonXAxisIsChecked)
            {
                if (DataManager.Instance.GET_INT_DATA(INPUT_X_ISMOVING, out bool _) == 1)
                {
                    ButtonAbsoluteMoveContent = "X-STOP";
                }
                else if (DataManager.Instance.GET_INT_DATA(INPUT_X_ISMOVING, out bool _) == 0)
                {
                    ButtonAbsoluteMoveContent = "X-MOVE [ABS]";
                }
            }
            else if (RadioButtonYAxisIsChecked)
            {

                if (DataManager.Instance.GET_INT_DATA(INPUT_Y_ISMOVING, out bool _) == 1)
                {
                    ButtonAbsoluteMoveContent = "Y-STOP";
                }
                else if (DataManager.Instance.GET_INT_DATA(INPUT_Y_ISMOVING, out bool _) == 0)
                {
                    ButtonAbsoluteMoveContent = "Y-MOVE [ABS]";
                }
            }         
            else
            {
                ButtonHommingEnable = false;
                ButtonAbsoluteMoveEnable = false;
            }
        }

        private void MotionStatusCheck()
        {
            XAxisVelocity = DataManager.Instance.GET_DOUBLE_DATA(INPUT_X_VELOCITY, out _);
            YAxisVelocity = DataManager.Instance.GET_DOUBLE_DATA(INPUT_Y_VELOCITY, out _);


            XAxisPosition = DataManager.Instance.GET_DOUBLE_DATA(INPUT_X_POSITION, out _);
            YAxisPosition = DataManager.Instance.GET_DOUBLE_DATA(INPUT_Y_POSITION, out _);
        }

        private void HommingButtonStatusChack()
        {
            if (RadioButtonXAxisIsChecked)
            {
                if (DataManager.Instance.GET_INT_DATA(INPUT_X_ISHOMMING, out bool _) == 1)
                {
                    ButtonHommingContent = "X-STOP";
                }
                else if (DataManager.Instance.GET_INT_DATA(INPUT_X_ISHOMMING, out bool _) == 0)
                {
                    ButtonHommingContent = "X-HOME";
                }
            }
            else if (RadioButtonYAxisIsChecked)
            {

                if (DataManager.Instance.GET_INT_DATA(INPUT_Y_ISHOMMING, out bool _) == 1 && RadioButtonYAxisIsChecked)
                {
                    ButtonHommingContent = "Y-STOP";
                }
                else
                {
                    ButtonHommingContent = "Y-HOME";
                }
            }           
            else
            {
                ButtonHommingEnable = false;
            }
        }


        private void ExecuteAbsoluteMoveCommand()
        {
            if (RadioButtonXAxisIsChecked)
            {
                if (!FunctionManager.Instance.CHECK_EXECUTING_FUNCTION_EXSIST(F_X_AXIS_ABSOLUTE_MOVE))
                {
                    FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_X_AXIS_ABSOLUTE_MOVE);
                }
            }
            else if (RadioButtonYAxisIsChecked)
            {
                if (!FunctionManager.Instance.CHECK_EXECUTING_FUNCTION_EXSIST(F_Y_AXIS_ABSOLUTE_MOVE))
                {
                    FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_Y_AXIS_ABSOLUTE_MOVE);
                }
            }
        }

        private void ExecuteRelativePlusMoveCommand()
        {
            if (RadioButtonXAxisIsChecked)
            {               
                if (!FunctionManager.Instance.CHECK_EXECUTING_FUNCTION_EXSIST(F_X_AXIS_RELATIVE_MOVE))
                {
                    DataManager.Instance.SET_DOUBLE_DATA(V_DBL_X_REL_DISTANCE, MoveDistance);
                    FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_X_AXIS_RELATIVE_MOVE);
                }
            }
            else if (RadioButtonYAxisIsChecked)
            {               
                if (!FunctionManager.Instance.CHECK_EXECUTING_FUNCTION_EXSIST(F_Y_AXIS_RELATIVE_MOVE))
                {
                    DataManager.Instance.SET_DOUBLE_DATA(V_DBL_Y_REL_DISTANCE, MoveDistance);
                    FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_Y_AXIS_RELATIVE_MOVE);
                }
            }
        }

        private void ExecuteRelativeMinusMoveCommand()
        {
            if (RadioButtonXAxisIsChecked)
            {
                if (!FunctionManager.Instance.CHECK_EXECUTING_FUNCTION_EXSIST(F_X_AXIS_RELATIVE_MOVE))
                {
                    DataManager.Instance.SET_DOUBLE_DATA(V_DBL_X_REL_DISTANCE, (MoveDistance * -1.0));
                    FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_X_AXIS_RELATIVE_MOVE);
                }
            }
            else if (RadioButtonYAxisIsChecked)
            {
                

                if (!FunctionManager.Instance.CHECK_EXECUTING_FUNCTION_EXSIST(F_Y_AXIS_RELATIVE_MOVE))
                {
                    DataManager.Instance.SET_DOUBLE_DATA(V_DBL_Y_REL_DISTANCE, (MoveDistance*-1.0));
                    FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_Y_AXIS_RELATIVE_MOVE);
                }
            }
        }

        private void ExecuteRelativeVelocityInputCommand()
        {
            if (RadioButtonXAxisIsChecked)
            {
                DataManager.Instance.SET_DOUBLE_DATA(V_DBL_X_REL_VELOCITY, RelativeVelocity);
            }
            else if (RadioButtonYAxisIsChecked)
            {
                DataManager.Instance.SET_DOUBLE_DATA(V_DBL_Y_REL_VELOCITY, RelativeVelocity);
            }
        }

        private void ExecuteMoveDistanceInputCommand()
        {
            if (RadioButtonXAxisIsChecked)
            {
                
            }
            else if (RadioButtonYAxisIsChecked)
            {
                
            }
        }

        private void ExecuteAbsoluteVelocityInputCommand()
        {
            if (RadioButtonXAxisIsChecked)
            {
                DataManager.Instance.SET_DOUBLE_DATA(V_DBL_X_ABS_VELOCITY, AbsoluteVelocity);
            }
            else if (RadioButtonYAxisIsChecked)
            {
                DataManager.Instance.SET_DOUBLE_DATA(V_DBL_Y_ABS_VELOCITY, AbsoluteVelocity);
            }
        }

        private void ExecuteAbsolutePositionInputCommand()
        {
            if (RadioButtonXAxisIsChecked)
            {
                DataManager.Instance.SET_DOUBLE_DATA(V_DBL_X_ABS_POSITION, AbsolutePosition);
            }
            else if (RadioButtonYAxisIsChecked)
            {
                DataManager.Instance.SET_DOUBLE_DATA(V_DBL_Y_ABS_POSITION, AbsolutePosition);
            }
        }

        private void ExecuteXJogPlusMouseLeftButtonUpCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_X_AXIS_JOG_STOP);
        }

        private void ExecuteXJogPlusMouseLeftButtonDownCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_X_AXIS_JOG_PLUS);
        }

        private void ExecuteXJogMinusMouseLeftButtonUpCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_X_AXIS_JOG_STOP);
        }

        private void ExecuteXAxisCheckedCommand()
        {
            ButtonHommingEnable = true;
            ButtonJogPlusEnable = true;
            ButtonJogMinusEnable = true;
            ButtonServoEnable = true;
            ButtonAbsoluteMoveEnable = true;
            ButtonRelativeMoveEnable = true;

            LabelAbsoluteMove = "X-Axis 절대좌표 이동";
            LabelRelativeMove = "X-Axis 상대좌표 이동";

            ButtonRelativePlusMoveContent = "X 상대이동(+)";
            ButtonRelativeMinusMoveContent = "X 상대이동(-)";

            ButtonJogPlusContent = "X-JOG+";
            ButtonJogMinusContent = "X-JOG-";

            ButtonServoContent = "E-STOP(X)";

            if (DataManager.Instance.GET_INT_DATA(INPUT_X_ISHOMMING, out bool _) == 1)
            {
                ButtonHommingContent = "X-STOP";
            }
            else if (DataManager.Instance.GET_INT_DATA(INPUT_X_ISHOMMING, out bool _) == 0)
            {
                ButtonHommingContent = "X-HOME";
            }

            //VelocityLimitMax = DataManager.Instance.GET_DOUBLE_DATA(V_DBL_X_MAX_VELOCITY, out bool _);

            TextBlockJogSpeedHighLow = DataManager.Instance.GET_STRING_DATA(V_STR_X_JOGVEL_MODE, out bool _);
            AbsolutePosition = DataManager.Instance.GET_DOUBLE_DATA(V_DBL_X_ABS_POSITION, out bool _);
            AbsoluteVelocity = DataManager.Instance.GET_DOUBLE_DATA(V_DBL_X_ABS_VELOCITY, out bool _);

            RelativeVelocity = DataManager.Instance.GET_DOUBLE_DATA(V_DBL_X_REL_VELOCITY, out bool _);
            MoveDistance = DataManager.Instance.GET_DOUBLE_DATA(V_DBL_X_REL_DISTANCE, out bool _);

            PositionLimitMax = DataManager.Instance.GET_DOUBLE_DATA(V_DBL_X_MAX_POSITION, out bool _);
            PositionLimitMin = DataManager.Instance.GET_DOUBLE_DATA(V_DBL_X_MIN_POSITION, out bool _);
            VelocityLimitMax = DataManager.Instance.GET_DOUBLE_DATA(V_DBL_X_MAX_VELOCITY, out bool _);
            VelocityLimitMin = DataManager.Instance.GET_DOUBLE_DATA(V_DBL_X_MIN_VELOCITY, out bool _);
            PositionInterval = 0.1F;
            VelocityInterval = 1.0F;

            if (TextBlockJogSpeedHighLow.StartsWith("L"))
            {
                ToggleButtonJogSpeedHighLow = false;
            }
            else
            {
                ToggleButtonJogSpeedHighLow = true;
            }
        }

        private void ExecuteYAxisCheckedCommand()
        {
            ButtonHommingEnable = true;
            ButtonJogPlusEnable = true;
            ButtonJogMinusEnable = true;
            ButtonServoEnable = true;

            ButtonAbsoluteMoveEnable = true;
            ButtonRelativeMoveEnable = true;

            LabelAbsoluteMove = "Y-Axis 절대좌표 이동";
            LabelRelativeMove = "Y-Axis 상대좌표 이동";

            ButtonRelativePlusMoveContent = "Y 상대이동(+)";
            ButtonRelativeMinusMoveContent = "Y 상대이동(-)";

            ButtonServoContent = "E-STOP(Y)";

            ButtonJogPlusContent = "Y-JOG+";
            ButtonJogMinusContent = "Y-JOG-";

            if (DataManager.Instance.GET_INT_DATA(INPUT_X_ISHOMMING, out bool _) == 1)
            {
                ButtonHommingContent = "Y-STOP";
            }
            else if (DataManager.Instance.GET_INT_DATA(INPUT_X_ISHOMMING, out bool _) == 0)
            {
                ButtonHommingContent = "Y-HOME";
            }

            TextBlockJogSpeedHighLow = DataManager.Instance.GET_STRING_DATA(V_STR_Y_JOGVEL_MODE, out bool _);

            AbsolutePosition = DataManager.Instance.GET_DOUBLE_DATA(V_DBL_Y_ABS_POSITION, out bool _);
            AbsoluteVelocity = DataManager.Instance.GET_DOUBLE_DATA(V_DBL_Y_ABS_VELOCITY, out bool _);
            RelativeVelocity = DataManager.Instance.GET_DOUBLE_DATA(V_DBL_Y_REL_VELOCITY, out bool _);
            MoveDistance = DataManager.Instance.GET_DOUBLE_DATA(V_DBL_Y_REL_DISTANCE, out bool _);

            PositionLimitMax = DataManager.Instance.GET_DOUBLE_DATA(V_DBL_Y_MAX_POSITION, out bool _);
            PositionLimitMin = DataManager.Instance.GET_DOUBLE_DATA(V_DBL_Y_MIN_POSITION, out bool _);
            VelocityLimitMax = DataManager.Instance.GET_DOUBLE_DATA(V_DBL_Y_MAX_VELOCITY, out bool _);
            VelocityLimitMin = DataManager.Instance.GET_DOUBLE_DATA(V_DBL_Y_MIN_VELOCITY, out bool _);
            PositionInterval = 0.1F;
            VelocityInterval = 1.0F;

            if (TextBlockJogSpeedHighLow.StartsWith("L"))
            {
                ToggleButtonJogSpeedHighLow = false;
            }
            else
            {
                ToggleButtonJogSpeedHighLow = true;
            }
        }

        private void ExecuteXJogMinusMouseLeftButtonDownCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_X_AXIS_JOG_MINUS);
        }

        private void ExecuteYJogPlusMouseLeftButtonUpCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_Y_AXIS_JOG_STOP);
        }


        private void ExecuteYJogPlusMouseLeftButtonDownCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_Y_AXIS_JOG_PLUS);
        }

        private void ExecuteYJogMinusMouseLeftButtonUpCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_Y_AXIS_JOG_STOP);
        }

        private void ExecuteYJogMinusMouseLeftButtonDownCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_Y_AXIS_JOG_MINUS);
        }

        private void ExecuteJogSpeedHighCommand()
        {
            TextBlockJogSpeedHighLow = "HIGH";

            if (RadioButtonXAxisIsChecked)
            {
                DataManager.Instance.SET_STRING_DATA(V_STR_X_JOGVEL_MODE, TextBlockJogSpeedHighLow);
            }
            else if (RadioButtonYAxisIsChecked)
            {
                DataManager.Instance.SET_STRING_DATA(V_STR_Y_JOGVEL_MODE, TextBlockJogSpeedHighLow);
            }
        }
        private void ExecuteJogSpeedLowCommand()
        {
            TextBlockJogSpeedHighLow = "LOW";

            if (RadioButtonXAxisIsChecked)
            {
                DataManager.Instance.SET_STRING_DATA(V_STR_X_JOGVEL_MODE, TextBlockJogSpeedHighLow);
            }
            else if (RadioButtonYAxisIsChecked)
            {
                DataManager.Instance.SET_STRING_DATA(V_STR_Y_JOGVEL_MODE, TextBlockJogSpeedHighLow);
            }
        }

        private void ExecuteHommingCommand()
        {
            if (RadioButtonXAxisIsChecked)
            {
                if (DataManager.Instance.GET_INT_DATA(INPUT_X_ISHOMMING, out bool _) == 1)
                {
                    FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_X_AXIS_HOME_STOP);
                }
                else
                {
                    FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_X_AXIS_HOMMING);
                }
            }
            else if (RadioButtonYAxisIsChecked)
            {
                if (DataManager.Instance.GET_INT_DATA(INPUT_Y_ISHOMMING, out bool _) == 1)
                {
                    FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_Y_AXIS_HOME_STOP);
                }
                else
                {
                    FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_Y_AXIS_HOMMING);
                }
            }
        }

        private void ExecuteServoCommand()
        {
            if (RadioButtonXAxisIsChecked)
            {
                FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.X_AXIS_SERVO_STOP);
            }
            else if (RadioButtonYAxisIsChecked)
            {
                FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.Y_AXIS_SERVO_STOP);
            }
        }

        private void ExecuteServoKillAllCommand()
        {
            DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_ALL_SERVO_STOP, 1);
        }

        private void ExecuteJogPlusMouseLeftButtonDownCommand()
        {
            if (RadioButtonXAxisIsChecked)
            {
                FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_X_AXIS_JOG_PLUS);
            }
            else if (RadioButtonYAxisIsChecked)
            {
                FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_Y_AXIS_JOG_PLUS);
            }
            else
            {
                ButtonJogPlusEnable = false;
            }
        }


        private void ExecuteJogPlusMouseLeftButtonUpCommand()
        {
            if (RadioButtonXAxisIsChecked)
            {
                FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_X_AXIS_JOG_STOP);
            }
            else if (RadioButtonYAxisIsChecked)
            {
                FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_Y_AXIS_JOG_STOP);
            }
            else
            {
                ButtonJogPlusEnable = false;
            }
        }

        private void ExecuteJogMinusMouseLeftButtonDownCommand()
        {
            if (RadioButtonXAxisIsChecked)
            {
                FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_X_AXIS_JOG_MINUS);
            }
            else if (RadioButtonYAxisIsChecked)
            {
                FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_Y_AXIS_JOG_MINUS);
            }
            else
            {
                ButtonJogPlusEnable = false;
            }
        }


        private void ExecuteJogMinusMouseLeftButtonUpCommand()
        {
            if (RadioButtonXAxisIsChecked)
            {
                FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_X_AXIS_JOG_STOP);
            }
            else if (RadioButtonYAxisIsChecked)
            {
                FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_Y_AXIS_JOG_STOP);
            }
            else
            {
                ButtonJogPlusEnable = false;
            }
        }

        private void ExecuteVisionMoveCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC("F_MOVE_VISION_POSITION");
        }

        private void ExecuteProcessMoveCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC("F_MOVE_PROCESS_POSITION");
        }

        private void ExecuteMoveStopCommand()
        {
            DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_ALL_MOVE_STOP, 1);
        }

        #endregion
    }
}
