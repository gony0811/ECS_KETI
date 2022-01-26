
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Practices.ServiceLocation;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using System.Reflection;
using CIM.Common;
using INNO6.Core;
using System.Threading;
using INNO6.IO;
using INNO6.Core.Manager;
using System.Windows.Input;
using Prism.Commands;
using ECS.UI.View;
using ECS.UI.Windows;
using ECS.Common.Helper;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace ECS.UI.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region I/O Define
        private readonly string IO_INT_AVAILABILITY = "vSys.iEqp.Availability";
        private readonly string IO_INT_RUN = "vSys.iEqp.Run";
        private readonly string IO_INT_INTERLOCK = "vSys.iEqp.Interlock";
        private readonly string IO_INT_MOVE = "vSys.iEqp.Move";
        private readonly string IO_INT_SIGNALTOWER_RED = "vSys.iSignalTower.Red";
        private readonly string IO_INT_SIGNALTOWER_GREEN = "vSys.iSignalTower.Green";
        private readonly string IO_INT_SIGNALTOWER_WHITE = "vSys.iSignalTower.White";
        private readonly string IO_INT_SIGNALTOWER_YELLOW = "vSys.iSignalTower.Yellow";
        #endregion

        private ECS.Application.Engine _engine = ECS.Application.Engine.Instance;
        private ICommand resourceReleaseCommand;
        private Timer _Timer;
        private Process _ManagedProcess;
        Common baseinfo = Common.GetInstance();
        public RelayCommand LoginCommand { get; private set; }

        public ICommand ResourceReleaseCommand
        {
            get
            {
                if (resourceReleaseCommand == null)
                {
                    resourceReleaseCommand = new DelegateCommand(ResourceRelease);
                }

                return resourceReleaseCommand;
            }
        }
        

        private bool signalTowerRed;
        private bool signalTowerGreen;
        private bool signalTowerWhite;
        private bool signalTowerYellow;

        public bool SignalTowerRed
        {
            get { return signalTowerRed; }
            set
            {
                signalTowerRed = value;
                RaisePropertyChanged("SignalTowerRed");
            }
        }

        public bool SignalTowerGreen
        {
            get { return signalTowerGreen; }
            set
            {
                signalTowerGreen = value;
                RaisePropertyChanged("signalTowerGreen");
            }
        }

        public bool SignalTowerWhite
        {
            get { return signalTowerWhite; }
            set
            {
                signalTowerWhite = value;
                RaisePropertyChanged("SignalTowerWhite");
            }
        }

        public bool SignalTowerYellow
        {
            get { return signalTowerYellow; }
            set
            {
                signalTowerYellow = value;
                RaisePropertyChanged("SignalTowerYellow");
            }
        }

        private int availability = 0;
        public int Availability
        {
            get { return availability; }
            set
            {
                if (availability != value)
                {
                    availability = value;
                    RaisePropertyChanged("Availability");
                }
            }
        }

        private int move = 0;
        public int Move
        { 
            get { return move; }
            set
            {
                if (move != value)
                {
                    move = value;
                    RaisePropertyChanged("Move");
                }
            }
        }

        private int run = 0;
        public int Run
        {
            get { return run; }
            set
            {
                if (run != value)
                {
                    run = value;
                    RaisePropertyChanged("Run");
                }
            }
        }

        private int interlock = 0;
        public int Interlock
        {
            get { return interlock; }
            set
            {
                if (interlock != value)
                {
                    interlock = value;
                    RaisePropertyChanged("Interlock");
                }
            }
        }

        private bool _IsEnableBuzzerButton;

        public bool IsEnableBuzzerButton
        { 
            get { return _IsEnableBuzzerButton; }
            set
            {
                if (_IsEnableBuzzerButton != value)
                {
                    _IsEnableBuzzerButton = value;
                    RaisePropertyChanged("IsEnableBuzzerButton");
                }
            }
        }


        private bool _IsEnableAlarmButton;

        public bool IsEnableAlarmButton
        {
            get { return _IsEnableAlarmButton; }
            set
            {
                if (_IsEnableAlarmButton != value)
                {
                    _IsEnableAlarmButton = value;
                    RaisePropertyChanged("IsEnableAlarmButton");
                }
            }
        }

        private bool _IsEnableEMOButton;

        public bool IsEnableEMOButton
        {
            get { return _IsEnableEMOButton; }
            set
            {
                if (_IsEnableEMOButton != value)
                {
                    _IsEnableEMOButton = value;
                    RaisePropertyChanged("IsEnableEMOButton");
                }
            }
        }

        private bool _IsEnableCautionButton;

        public bool IsEnableCautionButton
        {
            get { return _IsEnableCautionButton; }
            set
            {
                if (_IsEnableCautionButton != value)
                {
                    _IsEnableCautionButton = value;
                    RaisePropertyChanged("IsEnableCautionButton");
                }
            }
        }

        private ICommand _BuzzerButtonClickCommand;

        public ICommand BuzzerButtonClickCommand
        {
            get
            {
                if (_BuzzerButtonClickCommand == null)
                {
                    _BuzzerButtonClickCommand = new DelegateCommand(ExecuteBuzzerButtonClickCommand);
                }

                return _BuzzerButtonClickCommand;
            }
        }



        private ICommand _EmergencyStopCommand;

        public ICommand EmergencyStopCommand
        {
            get
            {
                if (_EmergencyStopCommand == null)
                {
                    _EmergencyStopCommand = new DelegateCommand(ExecuteEmergencyStopCommand);
                }

                return _EmergencyStopCommand;
            }
        }


        

        public enum Views { MAIN_SYSTEM, IO_LIST, CURRENT_ALARM, ALARM_HISTORY, SETTINGS, RECIPE, AUTO };

        public Views SelectedView { get; set; }

        private bool _IsSelectMainSystemView = true;

        public bool IsSelectMainSystemView
        {
            get { return _IsSelectMainSystemView; }
            set
            {
                if (_IsSelectMainSystemView != value)
                {
                    _IsSelectMainSystemView = value;
                    if (_IsSelectMainSystemView) SelectMenuChanged(Views.MAIN_SYSTEM);
                    RaisePropertyChanged("IsSelectMainSystemView");
                }
            }
        }

        private bool _IsSelectAutoView;

        public bool IsSelectAutoView
        {
            get { return _IsSelectAutoView; }
            set
            {
                if (_IsSelectAutoView != value)
                {
                    _IsSelectAutoView = value;
                    if (_IsSelectAutoView) SelectMenuChanged(Views.AUTO);
                    RaisePropertyChanged("IsSelectAutoView");
                }
            }
        }

        private bool _IsEnableAutoView;

        public bool IsEnableAutoView
        {
            get { return _IsEnableAutoView; }
            set
            {
                if (_IsEnableAutoView != value)
                {
                    _IsEnableAutoView = value;
                    RaisePropertyChanged("IsEnableAutoView");
                }
            }
        }



        private bool _IsEnableOpModeButton;

        public bool IsEnableOpModeButton
        {
            get { return _IsEnableOpModeButton; }
            set
            {
                if (_IsEnableOpModeButton != value)
                {
                    _IsEnableOpModeButton = value;
                    RaisePropertyChanged("IsEnableOpModeButton");
                }
            }
        }
        

        private bool _IsSelectIoListView;

        public bool IsSelectIoListView
        {
            get { return _IsSelectIoListView; }
            set
            {
                if (_IsSelectIoListView != value)
                {
                    _IsSelectIoListView = value;
                    if (_IsSelectIoListView) SelectMenuChanged(Views.IO_LIST);
                    RaisePropertyChanged("IsSelectIoListView");
                }
            }
        }

        private bool _IsEnableIoList = true;
        public bool IsEnableIoList
        {
            get { return _IsEnableIoList; }
            set
            {
                if (_IsEnableIoList != value)
                {
                    _IsEnableIoList = value;
                    RaisePropertyChanged("IsEnableIoList");
                }
            }
        }

        private bool _IsSelectRecipeManagerView;

        public bool IsSelectRecipeManagerView
        {
            get { return _IsSelectRecipeManagerView; }
            set
            {
                if (_IsSelectRecipeManagerView != value)
                {
                    _IsSelectRecipeManagerView = value;
                    if (_IsSelectRecipeManagerView) SelectMenuChanged(Views.RECIPE);
                    RaisePropertyChanged("IsSelectRecipeManagerView");
                }
            }
        }

        private bool _IsEnableRecipeManagerView = true;

        public bool IsEnableRecipeManagerView
        {
            get { return _IsEnableRecipeManagerView; }
            set
            {
                if (_IsEnableRecipeManagerView != value)
                {
                    _IsEnableRecipeManagerView = value;
                    RaisePropertyChanged("IsEnableRecipeManagerView");
                }
            }
        }
        //



        private bool _IsSelectCurrentAlarmView;

        public bool IsSelectCurrentAlarmView
        {
            get { return _IsSelectCurrentAlarmView; }
            set
            {
                if(_IsSelectCurrentAlarmView != value)
                {
                    _IsSelectCurrentAlarmView = value;
                    if (_IsSelectCurrentAlarmView) SelectMenuChanged(Views.CURRENT_ALARM);
                    RaisePropertyChanged("IsSelectCurrentAlarmView");
                }
            }
        }

        private bool _IsEnableCurrentAlarm = true;
        public bool IsEnableCurrentAlarm
        {
            get { return _IsEnableCurrentAlarm; }
            set
            {
                if (_IsEnableCurrentAlarm != value)
                {
                    _IsEnableCurrentAlarm = value;
                    RaisePropertyChanged("IsEnableCurrentAlarm");
                }
            }
        }

        private bool _IsSelectAlarmHistoryView;

        public bool IsSelectAlarmHistoryView
        {
            get { return _IsSelectAlarmHistoryView; }
            set
            {
                if (_IsSelectAlarmHistoryView != value)
                {
                    _IsSelectAlarmHistoryView = value;
                    if (_IsSelectAlarmHistoryView) SelectMenuChanged(Views.ALARM_HISTORY);
                    RaisePropertyChanged("IsSelectAlarmHistoryView");
                }
            }
        }

        private bool _IsEnableAlarmHistory = true;
        public bool IsEnableAlarmHistory
        {
            get { return _IsEnableAlarmHistory; }
            set
            {
                if (_IsEnableAlarmHistory != value)
                {
                    _IsEnableAlarmHistory = value;
                    RaisePropertyChanged("IsEnableAlarmHistory");
                }
            }
        }

        private bool _IsEnableSettingsView = true;
        public bool IsEnableSettingsView
        {
            get { return _IsEnableSettingsView; }
            set
            {
                if (_IsEnableSettingsView != value)
                {
                    _IsEnableSettingsView = value;
                    RaisePropertyChanged("IsEnableSettingsView");
                }
            }
        }

        private bool _IsSelectSettingsView;

        public bool IsSelectSettingsView
        {
            get { return _IsSelectSettingsView; }
            set
            {
                if (_IsSelectSettingsView != value)
                {
                    _IsSelectSettingsView = value;
                    if (_IsSelectSettingsView) SelectMenuChanged(Views.SETTINGS);
                    RaisePropertyChanged("IsSelectSettingsView");
                }
            }
        }

        private ICommand _CurrentAlarmButtonClickCommand;

        public ICommand CurrentAlarmButtonClickCommand
        {
            get
            {
                if (_CurrentAlarmButtonClickCommand == null)
                {
                    _CurrentAlarmButtonClickCommand = new DelegateCommand(CurrentAlarmButtonClicked);
                }

                return _CurrentAlarmButtonClickCommand;
            }
        }

        private string _WorkerAuthority;
        public string WorkerAuthority
        {
            get { return _WorkerAuthority; }
            set
            {
                if (_WorkerAuthority != value)
                {
                    _WorkerAuthority = value;
                    RaisePropertyChanged("WorkerAuthority");
                }
            }
        }



        private string _WorkerID;
        public string WorkerID
        {
            get { return _WorkerID; }
            set
            {
                if (_WorkerID != value)
                {
                    _WorkerID = value;
                    RaisePropertyChanged("WorkerID");
                }
            }
        }

        private string _WorkerName;
        public string WorkerName
        {
            get { return _WorkerName; }
            set
            {
                if (_WorkerName != value)
                {
                    _WorkerName = value;
                    RaisePropertyChanged("WorkerName");
                }
            }
        }

        private string _LoginCaption;
        public string LoginCaption
        {
            get { return _LoginCaption; }
            set
            {
                if (_LoginCaption != value)
                {
                    _LoginCaption = value;
                    RaisePropertyChanged("LoginCaption");
                }
            }
        }

        private string _SystemVersion;
        public string SystemVersion
        {
            get { return _SystemVersion; }
            set
            {
                if (_SystemVersion != value)
                {
                    _SystemVersion = value;
                    RaisePropertyChanged("SystemVersion");
                }
            }
        }


        /// <summary>
        /// Initializes a new instance of the MainWindowViewModel class.
        /// </summary>
        /// 
        public MainWindowViewModel()
        {
            LoginCommand = new RelayCommand(() => OnLoginEvent());
            LoginCaption = "LOGIN";
            UserInfoUpdate("", "", "", "");
            SystemVersion = string.Format("{0} {1}", "ECS Ver", Assembly.GetEntryAssembly().GetName().Version.ToString());


            _engine.ConfigFilePath = @"./config/Server.Config.ini";
            _engine.DbFilePath = @"./config/db_io.mdb";
            _engine.RecipeFolderPath = @"./config/recipe";
            _engine.Inialize();
            _engine.Start();

            IsEnableEMOButton = true;

            //bool result = DataManager.Instance.SET_INT_DATA("oLed.OnOff.Ch1", 1);
            //bool result = DataManager.Instance.SET_INT_DATA("oLed.OnOff.Ch1", 1);
            DataManager.Instance.DataAccess.DataChangedEvent += DataAccess_SystemDataChanged;
            AlarmManager.Instance.SetAlarmEvent += AlarmManager_SetAlarmEvent;
            AlarmManager.Instance.ResetAlarmEvent += AlarmManager_ResetAlarmEvent;

            InterlockManager.Instance.InterlockEvent += Instance_InterlockEvent;

            if (AlarmManager.Instance.GetCurrentAlarmAsList().Count > 0)
            {
                IsEnableAlarmButton = true;
            }
            else
            {
                IsEnableAlarmButton = false;
            }

            _IsEnableOpModeButton = true;
            _IsEnableBuzzerButton = true;

            Availability = (int)DataManager.Instance.GET_INT_DATA(IO_INT_AVAILABILITY, out _);
            Interlock = (int)DataManager.Instance.GET_INT_DATA(IO_INT_INTERLOCK, out _);
            Run = (int)DataManager.Instance.GET_INT_DATA(IO_INT_RUN, out _);
            Move = (int)DataManager.Instance.GET_INT_DATA(IO_INT_MOVE, out _);

            _ = DataManager.Instance.GET_INT_DATA(IO_INT_SIGNALTOWER_GREEN, out _) == 0 ? SignalTowerGreen = false : SignalTowerGreen = true;
            _ = DataManager.Instance.GET_INT_DATA(IO_INT_SIGNALTOWER_RED, out _) == 0 ? SignalTowerRed = false : SignalTowerRed = true;
            _ = DataManager.Instance.GET_INT_DATA(IO_INT_SIGNALTOWER_YELLOW, out _) == 0 ? SignalTowerYellow = false : SignalTowerYellow = true;
            _ = DataManager.Instance.GET_INT_DATA(IO_INT_SIGNALTOWER_WHITE, out _) == 0 ? SignalTowerWhite = false : SignalTowerWhite = true;

            TowerLampSetting(SignalTowerGreen, SignalTowerRed, SignalTowerYellow);

            _Timer = new Timer(TimerCallbackFunction, null, 0, 100);

            EMOProcessStart(@"./config/Server.Config.ini");
        }

        private void Instance_InterlockEvent(object sender, EventArgs e)
        {
            DataManager.Instance.SET_INT_DATA(IoNameHelper.V_INT_SYS_EQP_INTERLOCK, 1);
            TowerLampSetting(false, true, true);
        }

        private void TowerLampSetting(bool green, bool red, bool yellow)
        {
            if(green)
            {
                DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_TOWERLAMP_GREEN, 1);
            }
            else
            {
                DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_TOWERLAMP_GREEN, 0);
            }

            if (red)
            {
                DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_BUZZER_ONOFF, 1);
                DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_TOWERLAMP_RED, 1);
            }
            else
            {
                DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_BUZZER_ONOFF, 0);
                DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_TOWERLAMP_RED, 0);
            }


            if (yellow)
            {
                DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_TOWERLAMP_YELLOW, 1);
            }
            else
            {
                DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_TOWERLAMP_YELLOW, 0);
            }

        }

        private void EMOProcessStart(string configFilePath)
        {
            ConfigManager config = new ConfigManager(configFilePath);

            string exe = config.GetIniValue("SERVICE", "EMO");
            string filePath = Path.GetFullPath(exe);

            string processName = exe.Replace(".exe", "");

            Process[] processes = Process.GetProcessesByName(processName);

            if (processes != null)
            {
                foreach (Process p in processes)
                {
                    p.Kill();
                }
            }

            Process process = Process.Start(filePath);

            if (process != null) _ManagedProcess = process;
        }

        private void TimerCallbackFunction(object state)
        {
            if(DataManager.Instance.GET_INT_DATA(IoNameHelper.V_INT_EMO_CMD_ESTOP, out _) == 1)
            {
                DataManager.Instance.SET_INT_DATA(IoNameHelper.V_INT_EMO_CMD_ESTOP, 0);
                ExecuteEmergencyStopCommand();
            }

            if(DataManager.Instance.GET_STRING_DATA(IoNameHelper.IN_STR_LASER_OPMODE_STATUS, out _) == "ON")
            {
                IsEnableCautionButton = true;
            }
            else
            {
                IsEnableCautionButton = false;
            }
        }

        private void AlarmManager_ResetAlarmEvent(object sender, EventArgs e)
        {
            if (AlarmManager.Instance.GetCurrentAlarmAsList().Count == 0)
            {
                IsEnableAlarmButton = false;
            }

            if (e is AlarmEventArgs)
            {
                AlarmEventArgs eventArgs = e as AlarmEventArgs;

            }

            TowerLampSetting(SignalTowerGreen, SignalTowerRed, SignalTowerYellow);
        }

        private void AlarmManager_SetAlarmEvent(object sender, EventArgs e)
        {
            if (Dispatcher.CurrentDispatcher != System.Windows.Application.Current.Dispatcher)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                    if (e is AlarmEventArgs)
                    {
                        AlarmEventArgs eventArgs = e as AlarmEventArgs;
                        MessageBoxManager.ShowAlarmMessageBox(eventArgs.Alarm.ID, eventArgs.Alarm.TEXT, eventArgs.Alarm.DESCRIPTION, eventArgs.Alarm.LEVEL.ToString());
                    }
                });
            }
            else
            {
                if (e is AlarmEventArgs)
                {
                    AlarmEventArgs eventArgs = e as AlarmEventArgs;
                    MessageBoxManager.ShowAlarmMessageBox(eventArgs.Alarm.ID, eventArgs.Alarm.TEXT, eventArgs.Alarm.DESCRIPTION, eventArgs.Alarm.LEVEL.ToString());
                }
            }

            TowerLampSetting(SignalTowerGreen, SignalTowerRed, SignalTowerYellow);
            
            IsEnableAlarmButton = true;
        }

        private void CurrentAlarmButtonClicked()
        {
            if (DataManager.Instance.GET_STRING_DATA(IoNameHelper.V_STR_SYS_OPERATION_MODE, out bool _) == "AUTO")
            {
                if(OperationModeChange())
                {
                    UserInfoUpdate(WorkerName, WorkerID, "", WorkerAuthority);
                    IsSelectMainSystemView = true;
                    IsSelectAutoView = false;

                    CurrentAlarmWindow currentAlarmView = new CurrentAlarmWindow();
                    ViewModelLocator.Instance.CurrentAlarmViewModel.UpdateCurrentAlarmDisplay();
                    currentAlarmView.ShowDialog();

                }
                else
                {
                    return;
                }
                
            }
            else
            {
                CurrentAlarmWindow currentAlarmView = new CurrentAlarmWindow();
                ViewModelLocator.Instance.CurrentAlarmViewModel.UpdateCurrentAlarmDisplay();
                currentAlarmView.ShowDialog();
            }
        }


        private void ExecuteBuzzerButtonClickCommand()
        {
            DataManager.Instance.SET_INT_DATA(IoNameHelper.OUT_INT_PMAC_BUZZER_ONOFF, 0);
        }

        private void ExecuteEmergencyStopCommand()
        {
            DataManager.Instance.SET_INT_DATA("vSys.iEqp.EmoStop", 1);
        }

        private bool OperationModeChange()
        {
            StringBuilder message = new StringBuilder();
            if (DataManager.Instance.GET_STRING_DATA(IoNameHelper.V_STR_SYS_OPERATION_MODE, out bool _) == "MANUAL")
            {
                message.AppendLine("Do you really want to change auto mode ?");

                if (MessageBoxManager.ShowYesNoBox(message.ToString(), "OPERATION MODE : AUTO") == MSGBOX_RESULT.OK)
                {
                    DataManager.Instance.SET_STRING_DATA(IoNameHelper.V_STR_SYS_OPERATION_MODE, "AUTO");

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                message.AppendLine("Do you really want to change manual mode ?");

                if (MessageBoxManager.ShowYesNoBox(message.ToString(), "OPERATION MODE : MANUAL") == MSGBOX_RESULT.OK)
                {
                    DataManager.Instance.SET_STRING_DATA(IoNameHelper.V_STR_SYS_OPERATION_MODE, "MANUAL");

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void Instance_SetAlarmEvent(object sender, EventArgs e)
        {
            if (Dispatcher.CurrentDispatcher != System.Windows.Application.Current.Dispatcher)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(()=>{
                    if (e is AlarmEventArgs)
                    {
                        AlarmEventArgs eventArgs = e as AlarmEventArgs;
                        MessageBoxManager.ShowAlarmMessageBox(eventArgs.Alarm.ID, eventArgs.Alarm.TEXT, eventArgs.Alarm.DESCRIPTION, eventArgs.Alarm.LEVEL.ToString());
                    }
                });
            }
            else
            {
                if (e is AlarmEventArgs)
                {
                    AlarmEventArgs eventArgs = e as AlarmEventArgs;
                    MessageBoxManager.ShowAlarmMessageBox(eventArgs.Alarm.ID, eventArgs.Alarm.TEXT, eventArgs.Alarm.DESCRIPTION, eventArgs.Alarm.LEVEL.ToString());
                }
            }
        }

        public void ResourceRelease()
        {           
            ViewModelLocator.Instance.VisionCameraViewModel.Stop();
            _engine.Stop();

            if (_ManagedProcess != null)
            {
                try
                {
                    _ManagedProcess.Kill();
                }
                catch (Exception e)
                {
                    LogHelper.Instance.ErrorLog.ErrorFormat("Managed Process[ECS.EMO] is already exit : {0}", e.Message);
                }
            }
               
        }

        private void DataAccess_SystemDataChanged(object sender, DataChangedEventHandlerArgs args)
        {
            Data data = args.Data;

            if (data.Module.ToUpper() != "SYSTEM") return;

            if(data.Name.Equals(IO_INT_AVAILABILITY))
            {
                Availability = (int)data.Value;
            }
            else if(data.Name.Equals(IO_INT_INTERLOCK))
            {
                Interlock = (int)data.Value;
            }
            else if(data.Name.Equals(IO_INT_MOVE))
            {
                Move = (int)data.Value;
            }
            else if(data.Name.Equals(IO_INT_RUN))
            {
                Run = (int)data.Value;
            }
            else if(data.Name.Equals(IO_INT_SIGNALTOWER_RED))
            {
                if ((int)data.Value > 0) SignalTowerRed = true;
                else SignalTowerRed = false;
            }
            else if(data.Name.Equals(IO_INT_SIGNALTOWER_GREEN))
            {
                if ((int)data.Value > 0) SignalTowerGreen = true;
                else SignalTowerGreen = false;
            }
            else if(data.Name.Equals(IO_INT_SIGNALTOWER_WHITE))
            {
                if ((int)data.Value > 0) SignalTowerWhite = true;
                else SignalTowerWhite = false;
            }
            else if(data.Name.Equals(IO_INT_SIGNALTOWER_YELLOW))
            {
                if ((int)data.Value > 0) SignalTowerYellow = true;
                else SignalTowerYellow = false;
            }
        }

        public void OnLoginEvent()
        {
            if (!baseinfo.LoginState)
                OnLogin();
            else
                OnLogout();
        }

        public void OnLogin()
        {
            LogHelper.Instance.UILog.DebugFormat("[{0}] Login_Click ", this.GetType().Name);
            try
            {
                if (!baseinfo.LoginState)
                {                    
                    if (UserAutority.Check(this))
                    {
                        baseinfo.LoginState = true;
                        LoginCaption = "LOGOUT";
                        LogHelper.Instance.UILog.DebugFormat("[{0}] Login Done.", this.GetType().Name);
                    }
                    baseinfo.LoginStateFlag = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("{0} -> {1}", this.GetType().Name, ex.Message);
            }
        }

        public void OnLogout()
        {
            LogHelper.Instance.UILog.DebugFormat("[{0}] Logout_Click ", this.GetType().Name);
            try
            {
                if (baseinfo.LoginState)
                {                    
                    if (!UserAutority.Check(this))
                    {
                        baseinfo.LoginState = false;
                        LoginCaption = "LOGIN";
                        UserInfoUpdate("", "", "", "");
                        LogHelper.Instance.UILog.DebugFormat("[{0}] Logout Done.", this.GetType().Name);
                    }
                    baseinfo.LoginStateFlag = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("{0} -> {1}", this.GetType().Name, ex.Message);
            }
        }

        public void UserInfoUpdate(string name, string id, string pwd, string authority)
        {
            try
            {
                WorkerID = id;
                WorkerName = name;
                WorkerAuthority = authority;
               
                switch (WorkerAuthority)
                {
                    case "ADMIN":
                        if (SelectedView != Views.MAIN_SYSTEM) IsSelectMainSystemView = true;
                        IsEnableIoList = true;
                        IsEnableSettingsView = true;
                        IsEnableRecipeManagerView = true;
                        IsEnableAlarmHistory = true;
                        IsEnableAutoView = true;
                        break;
                    case "ENGINEER":
                        if (SelectedView != Views.MAIN_SYSTEM) IsSelectMainSystemView = true;
                        IsEnableIoList = true;
                        IsEnableSettingsView = true;
                        IsEnableRecipeManagerView = true;
                        IsEnableAlarmHistory = true;
                        IsEnableAutoView = true;
                        break;
                    case "OPERATOR":
                        if (SelectedView != Views.MAIN_SYSTEM) IsSelectMainSystemView = true;
                        IsEnableIoList = false;
                        IsEnableSettingsView = false;
                        IsEnableRecipeManagerView = false;
                        IsEnableAutoView = true;
                        IsEnableAlarmHistory = true;
                        break;
                    case "":
                        if (SelectedView != Views.MAIN_SYSTEM) IsSelectMainSystemView = true;
                        IsEnableIoList = false;
                        IsEnableSettingsView = false;
                        IsEnableRecipeManagerView = false;
                        IsEnableAlarmHistory = false;
                        IsEnableAutoView = false;
                        break;
                    default:
                        if (SelectedView != Views.MAIN_SYSTEM) IsSelectMainSystemView = true;
                        IsEnableIoList = false;
                        IsEnableSettingsView = false;
                        IsEnableRecipeManagerView = false;
                        IsEnableAlarmHistory = false;
                        IsEnableAutoView = false;
                        break;
                }

                if (SelectedView != Views.MAIN_SYSTEM) IsSelectMainSystemView = true;

            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("{0} -> {1}", this.GetType().Name, ex.Message);
            }
        }

        private void SelectMenuChanged(Views view)
        {
            SelectedView = view;
            LogHelper.Instance.UILog.DebugFormat("[{0}] SelectMenuChanged ({1})", this.GetType().Name, view.ToString());
            switch (view)
            {
                case Views.MAIN_SYSTEM:
                    if (DataManager.Instance.GET_STRING_DATA(IoNameHelper.V_STR_SYS_OPERATION_MODE, out _) == "AUTO")
                    {
                        if (!OperationModeChange())
                        {
                            IsSelectMainSystemView = false;
                            IsSelectAutoView = true;
                            return;
                        }
                        else
                        {
                            UserInfoUpdate(WorkerName, WorkerID, "", WorkerAuthority);
                        }
                    }

                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectAlarmHistoryView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectSettingsView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectIoListView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectRecipeManagerView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectAutoView = false;
                    break;
                case Views.IO_LIST:
                    if (DataManager.Instance.GET_STRING_DATA(IoNameHelper.V_STR_SYS_OPERATION_MODE, out _) == "AUTO")
                    {
                        if (!OperationModeChange())
                        {
                            IsSelectIoListView = false;
                            return;
                        }
                    }

                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectMainSystemView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectAlarmHistoryView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectSettingsView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectRecipeManagerView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectAutoView = false;
                    break;
                case Views.ALARM_HISTORY:
                    if (DataManager.Instance.GET_STRING_DATA(IoNameHelper.V_STR_SYS_OPERATION_MODE, out _) == "AUTO")
                    {
                        if (!OperationModeChange())
                        {
                            IsSelectAlarmHistoryView = false;
                            return;
                        }
                    }

                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectMainSystemView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectIoListView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectRecipeManagerView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectSettingsView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectAutoView = false;
                    break;
                case Views.SETTINGS:
                    if (DataManager.Instance.GET_STRING_DATA(IoNameHelper.V_STR_SYS_OPERATION_MODE, out _) == "AUTO")
                    {
                        if (!OperationModeChange())
                        {
                            IsSelectSettingsView = false;
                            return;
                        }
                    }

                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectMainSystemView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectIoListView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectAlarmHistoryView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectRecipeManagerView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectAutoView = false;
                    break;
                case Views.RECIPE:
                    if (DataManager.Instance.GET_STRING_DATA(IoNameHelper.V_STR_SYS_OPERATION_MODE, out _) == "AUTO")
                    {
                        if (!OperationModeChange())
                        {
                            IsSelectRecipeManagerView = false;
                            return;
                        }
                    }

                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectMainSystemView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectIoListView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectAlarmHistoryView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectSettingsView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectAutoView = false;
                    break;
                case Views.AUTO:
                    if (DataManager.Instance.GET_STRING_DATA(IoNameHelper.V_STR_SYS_OPERATION_MODE, out _) == "MANUAL")
                    {
                        if (!OperationModeChange())
                        {
                            IsSelectMainSystemView = true;
                            IsSelectAutoView = false;
                            return;
                        }
                        else
                        {
                            IsEnableIoList = false;
                            IsEnableAutoView = false;
                            IsEnableRecipeManagerView = false;
                            IsEnableSettingsView = false;
                            IsEnableAlarmHistory = false;

                            ViewModelLocator.Instance.OperationAutoViewModel.IsEnableInitButton = true;
                            ViewModelLocator.Instance.OperationAutoViewModel.IsEnableProcessButton = false;

                            ViewModelLocator.Instance.OperationAutoViewModel.Start();
                        }
                    }

                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectAlarmHistoryView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectSettingsView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectIoListView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectRecipeManagerView = false;
                    ViewModelLocator.Instance.MainWindowViewModel.IsSelectMainSystemView = false;
                    break;
                default:
                    break;
            }
        }
    }

    public class Clock : DependencyObject
    {
        public static DependencyProperty DateTimeProperty =
            DependencyProperty.Register("DateTime", typeof(DateTime), typeof(Clock));

        public DateTime DateTime
        {
            set { SetValue(DateTimeProperty, value); }
            get { return (DateTime)GetValue(DateTimeProperty); }
        }
        public Clock()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += TimerOnTick;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            DateTime = DateTime.Now;
        }
    }
}