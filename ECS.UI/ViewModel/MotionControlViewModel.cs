using ECS.Common.Helper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using INNO6.Core.Manager;
using INNO6.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private string _ButtonKillAllContent;
        private string _ButtonServoContent;
        private string _ButtonJogPlusContent;
        private string _ButtonJogMinusContent;
        private string _ButtonSetPositionContent;
        private string _ButtonVisionMoveContent;
        private string _ButtonProcessMoveContent;
        private string _ButtonMoveStopContent;
        private string _TextBlockJogSpeedHighLow;
        private string _SelectedJogSpeed;

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
        private bool _ButtonSetPositionEnable;
        private bool _ButtonVisionMoveEnable;
        private bool _ButtonMoveStopEnable;
        private bool _ButtonProcessMoveEnable;

        private double _XAxisVelocity;
        private double _YAxisVelocity;

        private double _XAxisPosition;
        private double _YAxisPosition;

        private double _VisionPositionX;
        private double _VisionPositionY;

        private double _ProcessPositionX;
        private double _ProcessPositionY;


        private double _PostionLimitMax;
        private double _PostionLimitMin;
        private double _VelocityLimitMax;
        private double _VelocityLimitMin;
        private double _PositionInterval;
        private double _VelocityInterval;

        private bool _RadioButtonXAxisIsChecked;
        private bool _RadioButtonYAxisIsChecked;

        private string _SelectedPositionItem;

        private List<string> _JogSpeedModeList;
        private ObservableCollection<string> _SavePositionList;

        private ICommand _RadioButtonXAxisCheckedCommand;
        private ICommand _RadioButtonYAxisCheckedCommand;


        private ICommand _ButtonHommingCommand;
        private ICommand _ButtonServoCommand;
        private ICommand _ButtonServoKillAllCommand;
        private ICommand _JogPlusPreviewMouseLeftButtonUpCommand;
        private ICommand _JogPlusPreviewMouseLeftButtonDownCommand;
        private ICommand _JogMinusPreviewMouseLeftButtonUpCommand;
        private ICommand _JogMinusPreviewMouseLeftButtonDownCommand;

        private ICommand _JogLeftUpMouseLeftButtonDownCommand;
        private ICommand _JogLeftDownMouseLeftButtonDownCommand;
        private ICommand _JogRightUpMouseLeftButtonDownCommand;
        private ICommand _JogRightDownMouseLeftButtonDownCommand;

        private ICommand _JogLeftUpMouseLeftButtonUpCommand;
        private ICommand _JogLeftDownMouseLeftButtonUpCommand;
        private ICommand _JogRightUpMouseLeftButtonUpCommand;
        private ICommand _JogRightDownMouseLeftButtonUpCommand;

        private ICommand _JogUpMouseLeftButtonDownCommand;
        private ICommand _JogUpMouseLeftButtonUpCommand;

        private ICommand _JogDownMouseLeftButtonDownCommand;
        private ICommand _JogDownMouseLeftButtonUpCommand;

        private ICommand _JogLeftMouseLeftButtonDownCommand;
        private ICommand _JogLeftMouseLeftButtonUpCommand;

        private ICommand _JogRightMouseLeftButtonDownCommand;
        private ICommand _JogRightMouseLeftButtonUpCommand;


        private ICommand _ButtonAbsoluteMoveCommand;
        private ICommand _ButtonRelativePlusMoveCommand;
        private ICommand _ButtonRelativeMinusMoveCommand;

        private ICommand _JogSpeedHighCommand;
        private ICommand _JogSpeedLowCommand;

        private ICommand _RelativeVelocityInputCommand;
        private ICommand _MoveDistanceInputCommand;
        private ICommand _AbsoluteVelocityInputCommand;
        private ICommand _AbsolutePositionInputCommand;

        private ICommand _ButtonSetPositionCommand;
        private ICommand _ButtonVisionMoveCommand;
        private ICommand _ButtonProcessMoveCommand;
        private ICommand _ButtonMoveStopCommand;

        private ICommand _LoadedCommand;
        private ICommand _UnloadedCommand;

        private ICommand _KeyDownCommand;
        private ICommand _KeyUpCommand;

        private ICommand _JogSpeedSelectedCommand;

        private ICommand _SavePositionCommand;
        private ICommand _DeletePositionCommand;
        private ICommand _MovePosiitonCommand;

        #endregion

        #region Define Constructor
        public MotionControlViewModel()
        {
            RadioButtonXAxisIsChecked = true;
            ExecuteXAxisCheckedCommand();

            ButtonSetPositionContent = "Save Position";
            ButtonVisionMoveContent = "To Vision";
            ButtonProcessMoveContent = "To Process";
            ButtonMoveStopContent = "Move Stop";

            ButtonKillAllContent = "KILL ALL";
            ButtonSetPositionEnable = true;
            ButtonVisionMoveEnable = true;
            ButtonProcessMoveEnable = true;
            ButtonMoveStopEnable = true;
            ButtonServoKillAllEnable = true;

            SelectedJogSpeed = "HIGH";

            VisionPositionX = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_VISION_POSITION, out _);
            VisionPositionY = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_VISION_POSITION, out _);

            ProcessPositionX = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_PROCESS_POSITION, out _);
            ProcessPositionY = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_PROCESS_POSITION, out _);

            _SavePositionList = new ObservableCollection<string>();
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

        public List<string> JogSpeedModeList { get { return new List<string>() { "HIGH", "MID", "LOW", "VERY LOW" }; } set { _JogSpeedModeList = value; } }
        public ObservableCollection<string> SavePositionList { get { return _SavePositionList; } set { _SavePositionList = value; RaisePropertyChanged("SavePositionList"); } }

        public string ButtonHommingContent { get { return _ButtonHommingContent; } set { _ButtonHommingContent = value; RaisePropertyChanged("ButtonHommingContent"); } }
        public string ButtonServoContent { get { return _ButtonServoContent; } set { _ButtonServoContent = value; RaisePropertyChanged("ButtonServoContent"); } }
        public string ButtonKillAllContent { get { return _ButtonKillAllContent; } set { _ButtonKillAllContent = value; RaisePropertyChanged("ButtonKillAllContent"); } }
        public string ButtonJogPlusContent { get { return _ButtonJogPlusContent; } set { _ButtonJogPlusContent = value; RaisePropertyChanged("ButtonJogPlusContent"); } }
        public string ButtonJogMinusContent { get { return _ButtonJogMinusContent; } set { _ButtonJogMinusContent = value; RaisePropertyChanged("ButtonJogMinusContent"); } }
        public string ButtonAbsoluteMoveContent { get { return _ButtonAbsoluteMoveContent; } set { _ButtonAbsoluteMoveContent = value; RaisePropertyChanged("ButtonAbsoluteMoveContent"); } }
        public string ButtonRelativePlusMoveContent { get { return _ButtonRelativePlusMoveContent; } set { _ButtonRelativePlusMoveContent = value; RaisePropertyChanged("ButtonRelativePlusMoveContent"); } }
        public string ButtonRelativeMinusMoveContent { get { return _ButtonRelativeMinusMoveContent; } set { _ButtonRelativeMinusMoveContent = value; RaisePropertyChanged("ButtonRelativeMinusMoveContent"); } }
        public string ButtonVisionMoveContent { get { return _ButtonVisionMoveContent; } set { _ButtonVisionMoveContent = value; RaisePropertyChanged("ButtonVisionMoveContent"); } }
        public string ButtonProcessMoveContent { get { return _ButtonProcessMoveContent; } set { _ButtonProcessMoveContent = value; RaisePropertyChanged("ButtonProcessMoveContent"); } }
        public string ButtonMoveStopContent { get { return _ButtonMoveStopContent; } set { _ButtonMoveStopContent = value; RaisePropertyChanged("ButtonMoveStopContent"); } }
        public string ButtonSetPositionContent { get { return _ButtonSetPositionContent; } set { _ButtonSetPositionContent = value;  RaisePropertyChanged("ButtonSetPositionContent"); } }

        public string LabelAbsoluteMove { get { return _LabelAbsoluteMove; } set { _LabelAbsoluteMove = value; RaisePropertyChanged("LabelAbsoluteMove"); } }
        public double AbsolutePosition { get { return _AbsolutePosition; } set { if (_AbsolutePosition != value) { _AbsolutePosition = value; RaisePropertyChanged("AbsolutePosition"); } } }
        public double AbsoluteVelocity { get { return _AbsoluteVelocity; } set { _AbsoluteVelocity = value; RaisePropertyChanged("AbsoluteVelocity"); } }

        public string LabelRelativeMove { get { return _LabelRelativeMove; } set { _LabelRelativeMove = value; RaisePropertyChanged("LabelRelativeMove"); } }
        public double MoveDistance { get { return _MoveDistance; } set { _MoveDistance = value; RaisePropertyChanged("MoveDistance"); } }
        public double RelativeVelocity { get { return _RelativeVelocity; } set { _RelativeVelocity = value; RaisePropertyChanged("RelativeVelocity"); } }
        public double VisionPositionX { get { return _VisionPositionX; } set { _VisionPositionX = value; RaisePropertyChanged("VisionPositionX"); } }
        public double VisionPositionY { get { return _VisionPositionY; } set { _VisionPositionY = value; RaisePropertyChanged("VisionPositionY"); } }
        public double ProcessPositionX { get { return _ProcessPositionX; } set { _ProcessPositionX = value; RaisePropertyChanged("ProcessPositionX"); } }
        public double ProcessPositionY { get { return _ProcessPositionY; } set { _ProcessPositionY = value; RaisePropertyChanged("ProcessPositionY"); } }

        public bool ButtonHommingEnable { get { return _ButtonHommingEnable; } set { _ButtonHommingEnable = value; RaisePropertyChanged("ButtonHommingEnable"); } }
        public bool ButtonServoEnable { get { return _ButtonServoEnable; } set { _ButtonServoEnable = value; RaisePropertyChanged("ButtonServoEnable"); } }
        public bool ButtonServoKillAllEnable { get { return _ButtonServoKillAllEnable; } set { _ButtonServoKillAllEnable = value; RaisePropertyChanged("ButtonServoKillAllEnable"); } }
        public bool ButtonJogPlusEnable { get { return _ButtonJogPlusEnable; } set { _ButtonJogPlusEnable = value; RaisePropertyChanged("ButtonJogPlusEnable"); } }
        public bool ButtonJogMinusEnable { get { return _ButtonJogMinusEnable; } set { _ButtonJogMinusEnable = value; RaisePropertyChanged("ButtonJogMinusEnable"); } }

        public bool ButtonAbsoluteMoveEnable { get { return _ButtonAbsoluteMoveEnable; } set { _ButtonAbsoluteMoveEnable = value; RaisePropertyChanged("ButtonAbsoluteMoveEnable"); } }
        public bool ButtonRelativeMoveEnable { get { return _ButtonRelativeMoveEnable; } set { _ButtonRelativeMoveEnable = value; RaisePropertyChanged("ButtonRelativeMoveEnable"); } }
        public bool ButtonSetPositionEnable { get { return _ButtonSetPositionEnable; } set { _ButtonSetPositionEnable = value; RaisePropertyChanged("ButtonSetPositionEnable"); } }
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
        public string SelectedJogSpeed { get { return _SelectedJogSpeed; } set { _SelectedJogSpeed = value; RaisePropertyChanged("SelectedJogSpeed"); } }

        public string SelectedPositionItem { get { return _SelectedPositionItem; } set { _SelectedPositionItem = value; RaisePropertyChanged("SelectedPositionItem"); } }
        

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

        public ICommand JogLeftUpMouseLeftButtonDownCommand { get { return this._JogLeftUpMouseLeftButtonDownCommand ?? (this._JogLeftUpMouseLeftButtonDownCommand = new RelayCommand(ExecuteJogLeftUpMouseLeftButtonDownCommand)); } }
        public ICommand JogLeftUpMouseLeftButtonUpCommand { get { return this._JogLeftUpMouseLeftButtonUpCommand ?? (this._JogLeftUpMouseLeftButtonUpCommand = new RelayCommand(ExecuteJogLeftUpMouseLeftButtonUpCommand)); } }

        public ICommand JogLeftDownMouseLeftButtonDownCommand { get { return this._JogLeftDownMouseLeftButtonDownCommand ?? (this._JogLeftDownMouseLeftButtonDownCommand = new RelayCommand(ExecuteJogLeftDownMouseLeftButtonDownCommand)); } }
        public ICommand JogLeftDownMouseLeftButtonUpCommand { get { return this._JogLeftDownMouseLeftButtonUpCommand ?? (this._JogLeftDownMouseLeftButtonUpCommand = new RelayCommand(ExecuteJogLeftDownMouseLeftButtonUpCommand)); } }

        public ICommand JogRightUpMouseLeftButtonDownCommand { get { return this._JogRightUpMouseLeftButtonDownCommand ?? (this._JogRightUpMouseLeftButtonDownCommand = new RelayCommand(ExecuteJogRightUpMouseLeftButtonDownCommand)); } }
        public ICommand JogRightUpMouseLeftButtonUpCommand { get { return this._JogRightUpMouseLeftButtonUpCommand ?? (this._JogRightUpMouseLeftButtonUpCommand = new RelayCommand(ExecuteJogRightUpMouseLeftButtonUpCommand)); } }

        public ICommand JogRightDownMouseLeftButtonDownCommand { get { return this._JogRightDownMouseLeftButtonDownCommand ?? (this._JogRightDownMouseLeftButtonDownCommand = new RelayCommand(ExecuteJogRightDownMouseLeftButtonDownCommand)); } }
        public ICommand JogRightDownMouseLeftButtonUpCommand { get { return this._JogRightDownMouseLeftButtonUpCommand ?? (this._JogRightDownMouseLeftButtonUpCommand = new RelayCommand(ExecuteJogRightDownMouseLeftButtonUpCommand)); } }


        public ICommand JogUpMouseLeftButtonUpCommand { get { return this._JogUpMouseLeftButtonUpCommand ?? (this._JogUpMouseLeftButtonUpCommand = new RelayCommand(ExecuteJogUpMouseLeftButtonUpCommand)); } }
        public ICommand JogUpMouseLeftButtonDownCommand { get { return this._JogUpMouseLeftButtonDownCommand ?? (this._JogUpMouseLeftButtonDownCommand = new RelayCommand(ExecuteJogUpMouseLeftButtonDownCommand)); } }
        public ICommand JogDownMouseLeftButtonUpCommand { get { return this._JogDownMouseLeftButtonUpCommand ?? (this._JogDownMouseLeftButtonUpCommand = new RelayCommand(ExecuteJogDownMouseLeftButtonUpCommand)); } }
        public ICommand JogDownMouseLeftButtonDownCommand { get { return this._JogDownMouseLeftButtonDownCommand ?? (this._JogDownMouseLeftButtonDownCommand = new RelayCommand(ExecuteJogDownMouseLeftButtonDownCommand)); } }
        public ICommand JogLeftMouseLeftButtonUpCommand { get { return this._JogLeftMouseLeftButtonUpCommand ?? (this._JogLeftMouseLeftButtonUpCommand = new RelayCommand(ExecuteJogLeftMouseLeftButtonUpCommand)); } }
        public ICommand JogLeftMouseLeftButtonDownCommand { get { return this._JogLeftMouseLeftButtonDownCommand ?? (this._JogLeftMouseLeftButtonDownCommand = new RelayCommand(ExecuteJogLeftMouseLeftButtonDownCommand)); } }
        public ICommand JogRightMouseLeftButtonUpCommand { get { return this._JogRightMouseLeftButtonUpCommand ?? (this._JogRightMouseLeftButtonUpCommand = new RelayCommand(ExecuteJogRightMouseLeftButtonUpCommand)); } }
        public ICommand JogRightMouseLeftButtonDownCommand { get { return this._JogRightMouseLeftButtonDownCommand ?? (this._JogRightMouseLeftButtonDownCommand = new RelayCommand(ExecuteJogRightMouseLeftButtonDownCommand)); } }

        public ICommand RadioButtonXAxisCheckedCommand { get { return this._RadioButtonXAxisCheckedCommand ?? (this._RadioButtonXAxisCheckedCommand = new RelayCommand(ExecuteXAxisCheckedCommand)); } }
        public ICommand RadioButtonYAxisCheckedCommand { get { return this._RadioButtonYAxisCheckedCommand ?? (this._RadioButtonYAxisCheckedCommand = new RelayCommand(ExecuteYAxisCheckedCommand)); } }
    
        public ICommand RelativeVelocityInputCommand { get { return this._RelativeVelocityInputCommand ?? (this._RelativeVelocityInputCommand = new RelayCommand(ExecuteRelativeVelocityInputCommand)); } }
        public ICommand MoveDistanceInputCommand { get { return this._MoveDistanceInputCommand ?? (this._MoveDistanceInputCommand = new RelayCommand(ExecuteMoveDistanceInputCommand)); } }
        public ICommand AbsoluteVelocityInputCommand { get { return this._AbsoluteVelocityInputCommand ?? (this._AbsoluteVelocityInputCommand = new RelayCommand(ExecuteAbsoluteVelocityInputCommand)); } }
        public ICommand AbsolutePositionInputCommand { get { return this._AbsolutePositionInputCommand ?? (this._AbsolutePositionInputCommand = new RelayCommand(ExecuteAbsolutePositionInputCommand)); } }


        public ICommand ButtonVisionMoveCommand { get { return this._ButtonVisionMoveCommand ?? (this._ButtonVisionMoveCommand = new RelayCommand(ExecuteVisionMoveCommand)); } }
        public ICommand ButtonProcessMoveCommand { get { return this._ButtonProcessMoveCommand ?? (this._ButtonProcessMoveCommand = new RelayCommand(ExecuteProcessMoveCommand)); } }
        public ICommand ButtonMoveStopCommand { get { return this._ButtonMoveStopCommand ?? (this._ButtonMoveStopCommand = new RelayCommand(ExecuteMoveStopCommand)); } }
        public ICommand ButtonSetPositionCommand { get { return this._ButtonSetPositionCommand ?? (this._ButtonSetPositionCommand = new RelayCommand(ExecuteSetPositionCommand)); } }



        public ICommand LoadedCommand { get { return this._LoadedCommand ?? (this._LoadedCommand = new RelayCommand(ExecuteLoadedCommand)); } }
        public ICommand UnloadedCommand { get { return this._UnloadedCommand ?? (this._UnloadedCommand = new RelayCommand(ExecuteUnloadedCommand)); } }

        public ICommand KeyDownCommand { get { return this._KeyDownCommand ?? (this._KeyDownCommand = new RelayCommand<KeyEventArgs>(ExecuteKeyDownCommand)); } }
        public ICommand KeyUpCommand { get { return this._KeyUpCommand ?? (this._KeyUpCommand = new RelayCommand<KeyEventArgs>(ExecuteKeyUpCommand)); } }
        public ICommand JogSpeedSelectedCommand { get { return this._JogSpeedSelectedCommand ?? (this._JogSpeedSelectedCommand = new RelayCommand(ExecuteJogSpeedSelectedCommand)); } }

        public ICommand SavePositionCommand { get { return this._SavePositionCommand ?? (this._SavePositionCommand = new RelayCommand(ExecuteSavePositionCommand)); } }
        public ICommand DeletePositionCommand { get { return this._DeletePositionCommand ?? (this._DeletePositionCommand = new RelayCommand(Execute_eletePositionCommand)); } }
        
        public ICommand MovePosiitonCommand { get { return this._MovePosiitonCommand ?? (this._MovePosiitonCommand = new RelayCommand(ExecuteMovePositionCommand)); } }
        
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

        private void ExecuteKeyDownCommand(KeyEventArgs args)
        {
            if (args.Key == Key.Left)
            {
                FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.X_AXIS_JOG_MINUS);
            }
            else if (args.Key == Key.Right)
            {
                FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.X_AXIS_JOG_PLUS);
            }
            else if (args.Key == Key.Up)
            {
                FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.Y_AXIS_JOG_PLUS);
            }
            else if (args.Key == Key.Down)
            {
                FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.Y_AXIS_JOG_MINUS);
            }
            else
            {
                return;
            }
        }

        private void ExecuteKeyUpCommand(KeyEventArgs args)
        {
            if (args.Key == Key.Left || args.Key == Key.Right || args.Key == Key.Up || args.Key == Key.Down)
            {
                FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.X_AXIS_JOG_STOP);
                FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.Y_AXIS_JOG_STOP);
            }
            //if (args.Key == Key.Left)
            //{
            //    FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.X_AXIS_JOG_STOP);
            //}
            //else if (args.Key == Key.Right)
            //{
            //    FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.X_AXIS_JOG_STOP);
            //}
            //else if (args.Key == Key.Up)
            //{
            //    FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.Y_AXIS_JOG_STOP);
            //}
            //else if (args.Key == Key.Down)
            //{
            //    FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.Y_AXIS_JOG_STOP);
            //}
            //else
            //{
            //    return;
            //}
        }

        private void ExecuteSavePositionCommand()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(XAxisPosition.ToString("F3"));
            sb.Append(",");
            sb.Append(YAxisPosition.ToString("F3"));
            SavePositionList.Add(sb.ToString());
        }

        private void Execute_eletePositionCommand()
        {
            if (String.IsNullOrEmpty(SelectedPositionItem))
            {
                MessageBoxManager.ShowMessageBox("삭제할 위치를 선택하십시요!");
                return;
            }
            else
            {
                SavePositionList.Remove(SelectedPositionItem);
            }
        }

        private void ExecuteMovePositionCommand()
        {
            if(String.IsNullOrEmpty(SelectedPositionItem))
            {
                MessageBoxManager.ShowMessageBox("이동할 위치를 선택하십시요!");
                return;
            }
            else
            {
                string[] posArray = SelectedPositionItem.Split(',');
                double x = double.Parse(posArray[0]);
                double y = double.Parse(posArray[1]);

                if (MessageBoxManager.ShowYesNoBox(string.Format("X: {0:F3}, Y: {1:F3} 위치로 이동 하시겠습니까?", x, y), "Do you really want to move position ?") == MSGBOX_RESULT.OK)
                {
                    DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_ABS_POSITION, x);
                    DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_ABS_POSITION, y);

                    FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.X_AXIS_MOVE_TO_SETPOS);
                    FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.Y_AXIS_MOVE_TO_SETPOS);
                }
            }
        }

        private void ExecuteJogSpeedSelectedCommand()
        {
            DataManager.Instance.SET_STRING_DATA(IoNameHelper.V_STR_SET_X_JOGVEL_MODE, SelectedJogSpeed);
            DataManager.Instance.SET_STRING_DATA(IoNameHelper.V_STR_SET_Y_JOGVEL_MODE, SelectedJogSpeed);
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

            //TextBlockJogSpeedHighLow = DataManager.Instance.GET_STRING_DATA(V_STR_X_JOGVEL_MODE, out bool _);

            //SelectedJogSpeed = DataManager.Instance.GET_STRING_DATA(IoNameHelper.V_STR_SET_X_JOGVEL_MODE, out bool _);
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

            //if (TextBlockJogSpeedHighLow.StartsWith("L"))
            //{
            //    ToggleButtonJogSpeedHighLow = false;
            //}
            //else
            //{
            //    ToggleButtonJogSpeedHighLow = true;
            //}
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

            //SelectedJogSpeed = DataManager.Instance.GET_STRING_DATA(IoNameHelper.V_STR_SET_Y_JOGVEL_MODE, out bool _);

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

            //if (TextBlockJogSpeedHighLow.StartsWith("L"))
            //{
            //    ToggleButtonJogSpeedHighLow = false;
            //}
            //else
            //{
            //    ToggleButtonJogSpeedHighLow = true;
            //}
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

        private void ExecuteJogLeftUpMouseLeftButtonDownCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.X_AXIS_JOG_MINUS);
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.Y_AXIS_JOG_PLUS);
        }

        private void ExecuteJogLeftUpMouseLeftButtonUpCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.X_AXIS_JOG_STOP);
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.Y_AXIS_JOG_STOP);
        }

        private void ExecuteJogLeftDownMouseLeftButtonDownCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.X_AXIS_JOG_MINUS);
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.Y_AXIS_JOG_MINUS);
        }
        private void ExecuteJogLeftDownMouseLeftButtonUpCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.X_AXIS_JOG_STOP);
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.Y_AXIS_JOG_STOP);
        }

        private void ExecuteJogRightUpMouseLeftButtonDownCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.X_AXIS_JOG_PLUS);
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.Y_AXIS_JOG_PLUS);
        }

        private void ExecuteJogRightUpMouseLeftButtonUpCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.X_AXIS_JOG_STOP);
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.Y_AXIS_JOG_STOP);
        }

        private void ExecuteJogRightDownMouseLeftButtonDownCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.X_AXIS_JOG_PLUS);
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.Y_AXIS_JOG_MINUS);
        }
        private void ExecuteJogRightDownMouseLeftButtonUpCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.X_AXIS_JOG_STOP);
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.Y_AXIS_JOG_STOP);
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

        private void ExecuteJogUpMouseLeftButtonUpCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_Y_AXIS_JOG_STOP);
        }

        private void ExecuteJogUpMouseLeftButtonDownCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_Y_AXIS_JOG_PLUS);
        }

        private void ExecuteJogDownMouseLeftButtonUpCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_Y_AXIS_JOG_STOP);
        }

        private void ExecuteJogDownMouseLeftButtonDownCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_Y_AXIS_JOG_MINUS);
        }

        private void ExecuteJogLeftMouseLeftButtonUpCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_X_AXIS_JOG_STOP);
        }

        private void ExecuteJogLeftMouseLeftButtonDownCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_X_AXIS_JOG_MINUS);
        }

        private void ExecuteJogRightMouseLeftButtonUpCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_X_AXIS_JOG_STOP);
        }

        private void ExecuteJogRightMouseLeftButtonDownCommand()
        {
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(F_X_AXIS_JOG_PLUS);
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
            FunctionManager.Instance.ABORT_FUNCTION(FuncNameHelper.MOVE_VISION_POSITION);
            FunctionManager.Instance.ABORT_FUNCTION(FuncNameHelper.MOVE_PROCESS_POSITION);
            FunctionManager.Instance.EXECUTE_FUNCTION_ASYNC(FuncNameHelper.MOVE_MOTION_STOP);
        }

        private void ExecuteSetPositionCommand()
        {
            double offsetX = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_POSITION_OFFSET, out _);
            double offsetY = DataManager.Instance.GET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_POSITION_OFFSET, out _);

            VisionPositionX = XAxisPosition;
            VisionPositionY = YAxisPosition;

            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_VISION_POSITION, VisionPositionX);
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.V_DBL_SET_X_VISION_POSITION, VisionPositionX);

            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_VISION_POSITION, VisionPositionY);
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.V_DBL_SET_Y_VISION_POSITION, VisionPositionY);

            ProcessPositionX = XAxisPosition + offsetX;
            ProcessPositionY = YAxisPosition + offsetY;

            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_X_PROCESS_POSITION, ProcessPositionX);
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.V_DBL_SET_X_PROCESS_POSITION, ProcessPositionX);


            DataManager.Instance.SET_DOUBLE_DATA(IoNameHelper.V_DBL_SET_Y_PROCESS_POSITION, ProcessPositionY);
            DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.V_DBL_SET_Y_PROCESS_POSITION, ProcessPositionY);
        }

        #endregion
    }
}
