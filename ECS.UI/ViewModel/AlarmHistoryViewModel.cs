using GalaSoft.MvvmLight;
using System;
using ECS.UI.Model;
using System.Collections.ObjectModel;
using INNO6.Core;
using GalaSoft.MvvmLight.Command;
using INNO6.Core.Manager;
using System.Data;

namespace ECS.UI.ViewModel
{
    public class AlarmHistoryViewModel : ViewModelBase
    {
        private ObservableCollection<AlarmHistoryDisplay> _AlarmHistoryList;
        public ObservableCollection<AlarmHistoryDisplay> AlarmHistoryList
        {
            get { return _AlarmHistoryList; }
            set
            {
                if (_AlarmHistoryList != value)
                {
                    _AlarmHistoryList = value;
                    RaisePropertyChanged("AlarmHistoryList");
                }
            }
        }

        private DateTime _StartTime;
        public DateTime StartTime
        {
            get { return _StartTime; }
            set
            {
                if (_StartTime != value)
                {
                    _StartTime = value;
                    RaisePropertyChanged("StartTime");
                }
            }
        }

        private DateTime _EndTime;
        public DateTime EndTime
        {
            get { return _EndTime; }
            set
            {
                if (_EndTime != value)
                {
                    _EndTime = value;
                    RaisePropertyChanged("EndTime");
                }
            }
        }

        private RelayCommand _AlarmHistoryCommand;

        //public RelayCommand AlarmHistoryCommand { get; private set; }

        public RelayCommand AlarmHistoryCommand
        {
            get
            {
                if (_AlarmHistoryCommand == null)
                {
                    _AlarmHistoryCommand = new RelayCommand(AlarmDBSearch_Click);
                }

                return _AlarmHistoryCommand;
            }
        }

        private RelayCommand<object> _DataGridSelectionChangedCommand;
        public RelayCommand<object> DataGridSelectionChangedCommand
        {
            get
            {
                return this._DataGridSelectionChangedCommand ?? (this._DataGridSelectionChangedCommand = new RelayCommand<object>(ExecuteDataGridSelectionChanged));
            }
        }

        private void ExecuteDataGridSelectionChanged(object obj)
        {
            
        }

        public AlarmHistoryViewModel()
        {
            AlarmHistoryList = new ObservableCollection<AlarmHistoryDisplay>();
            //AlarmHistoryCommand = new RelayCommand(() => AlarmDBSearch_Click());

            //하루 전 일 시각에서 현재까지를 기본으로 함
            var CnvSTime = DateTime.Now.AddDays(-1);
            StartTime = new DateTime(CnvSTime.Year, CnvSTime.Month, CnvSTime.Day, CnvSTime.Hour, 0, 0);
            EndTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour + 1, 0, 0);
            AlarmDBSearch_Click();
        }

        private void AlarmDBSearch_Click()
        {
            LogHelper.Instance.UILog.DebugFormat("[{0}] AlarmDBSearch_Click", this.GetType().Name);
            try
            {

                DateTime start_date = StartTime;
                DateTime end_date = EndTime;

                if (start_date > end_date)
                {
                    MessageBoxManager.ShowMessageBox("Start date can not be equal or greater...");
                }
                else
                {
                    //CurrentAlarm.Clear();
                    DataTable alarmHistData = AlarmManager.Instance.GetAlarmHistory(start_date, EndTime);

                    // DataTable 내림차순 정렬 ( 기준 : EVENTTIME Column )
                    alarmHistData.DefaultView.Sort = "UPDATETIME DESC";
                    alarmHistData = alarmHistData.DefaultView.ToTable(true);

                    AlarmHistoryList.Clear();
                    foreach (DataRow dr in alarmHistData.Rows)
                    {
                        AlarmHistoryList.Add(new AlarmHistoryDisplay()
                        {
                            ID = dr["ID"].ToString(),
                            LEVEL = dr["LEVEL"].ToString(),
                            ALARM_TEXT = dr["TEXT"].ToString(),
                            STATUS = dr["STATUS"].ToString(),
                            UPDATETIME = DateTime.Parse(dr["UPDATETIME"].ToString())
                        });
                    }
                }
                LogHelper.Instance.UILog.DebugFormat("[{0}] AlarmDBSearch_Click Done.", this.GetType().Name);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("{0} -> {1}", this.GetType().Name, ex.Message);
                MessageBoxManager.ShowMessageBox(ex.ToString());
            }
        }


    }
}
