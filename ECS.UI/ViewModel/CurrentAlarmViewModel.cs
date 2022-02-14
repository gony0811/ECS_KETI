using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECS.UI.Model;
using INNO6.Core;
using GalaSoft.MvvmLight.Command;
using INNO6.IO;
using INNO6.Core.Manager;
using INNO6.Core.Manager.Model;
using System.Windows.Input;
using Prism.Commands;

namespace ECS.UI.ViewModel
{
    public class CurrentAlarmViewModel : ViewModelBase
    {

        private ObservableCollection<CurrentAlarmDisplay> _CurrentAlarmList;

        private CurrentAlarmDisplay _SelectedAlarm;

        private ICommand _AlarmResetCommand;
        private ICommand _AlarmResetAllCommand;

        public ObservableCollection<CurrentAlarmDisplay> CurrentAlarmList
        {
            get { return _CurrentAlarmList; }
            set
            {
                if (_CurrentAlarmList != value)
                {
                    _CurrentAlarmList = value;
                    RaisePropertyChanged("CurrentAlarmList");
                }
            }
        }

        public CurrentAlarmDisplay SelectedAlarm
        {
            get { return _SelectedAlarm; }
            set
            {
                if (_SelectedAlarm != value)
                {
                    _SelectedAlarm = value;
                    RaisePropertyChanged("SelectedAlarm");
                }
            }
        }

        public ICommand AlarmResetCommand
        {
            get
            {
                if (_AlarmResetCommand == null)
                {
                    _AlarmResetCommand = new DelegateCommand(ResetAlarm);
                }

                return _AlarmResetCommand;
            }
        }

        public ICommand AlarmResetAllCommand
        {
            get
            {
                if (_AlarmResetAllCommand == null)
                {
                    _AlarmResetAllCommand = new DelegateCommand(ResetAllAlarm);
                }

                return _AlarmResetAllCommand;
            }
        }

        private void ResetAlarm()
        {
            if (SelectedAlarm != null && AlarmManager.Instance.ContainsCurrentAlarm(SelectedAlarm.ID))
            {
                AlarmManager.Instance.ResetAlarm(_SelectedAlarm.ID);
                UpdateCurrentAlarmDisplay();
            }
        }

        private void ResetAllAlarm()
        {
            AlarmManager.Instance.ResetAlarmAll();
            InterlockManager.Instance.INTERLOCK_RESET();
            UpdateCurrentAlarmDisplay();
        }

        public CurrentAlarmViewModel()
        {
            Initialize();
        }

        public void UpdateCurrentAlarmDisplay()
        {
            List<ALARM> currentAlarmList = AlarmManager.Instance.GetCurrentAlarmAsList();
            ObservableCollection<CurrentAlarmDisplay> currentAlarmDisplays = new ObservableCollection<CurrentAlarmDisplay>();

            foreach (var alarm in currentAlarmList)
            {
                if (alarm != null)
                {
                    currentAlarmDisplays.Add(new CurrentAlarmDisplay()
                    {
                        ID = alarm.ID,
                        NAME = alarm.TEXT,
                        DESCRIPTION = alarm.DESCRIPTION,
                        LEVEL = alarm.LEVEL.ToString(),
                        SET_TIME = alarm.SETTIME,
                    });
                }
                else
                {
                    continue;
                }
            }

            CurrentAlarmList = currentAlarmDisplays;
        }

        private void Initialize()
        {
            _CurrentAlarmList = new ObservableCollection<CurrentAlarmDisplay>();

            UpdateCurrentAlarmDisplay();
        }
    }
}
