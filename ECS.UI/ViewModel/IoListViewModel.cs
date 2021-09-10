using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECS.UI.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using INNO6.IO;
using INNO6.Core;
using System.Windows.Input;

namespace ECS.UI.ViewModel
{
    public class IoListViewModel : ViewModelBase
    {
        private Timer _timer;
        private ObservableCollection<IoDataDisplay> filteredIoDataList;
        private ObservableCollection<IoDataDisplay> alIoDataList;

        private ICommand _LoadedCommand;
        private ICommand _UnloadedCommand;

        public ICommand LoadedCommand { get { return this._LoadedCommand ?? (this._LoadedCommand = new RelayCommand(ExecuteLoadedCommand)); } }
        public ICommand UnloadedCommand { get { return this._UnloadedCommand ?? (this._UnloadedCommand = new RelayCommand(ExecuteUnloadedCommand)); } }


        private void ExecuteLoadedCommand()
        {
            Start();
        }

        private void ExecuteUnloadedCommand()
        {
            Stop();
        }

        private void ItemName_TextChanged()
        {
            LogHelper.Instance.UILog.DebugFormat("[{0}] Search_Click", this.GetType().Name);
            try
            {
                if (!string.IsNullOrEmpty(ItemName))
                {
                    filteredIoDataList = new ObservableCollection<IoDataDisplay>(IoDataList.Where(x => x.Name.Contains(ItemName)));
                    //_FilteredSVData = (from m in SVData.AsEnumerable()
                    //                   where m.Field<string>("NAME").Contains(ItemName)
                    //                   select m).CopyToDataTable<DataRow>();

                    LogHelper.Instance.UILog.DebugFormat("[{0}] Search_Click Done. ({1})", this.GetType().Name, ItemName);
                }
                else
                {
                    filteredIoDataList = alIoDataList;
                }

                IoDataList = filteredIoDataList;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("{0} -> {1}", this.GetType().Name, ex.Message);
            }

        }

        private ObservableCollection<IoDataDisplay> _ioDataList;
        public ObservableCollection<IoDataDisplay> IoDataList
        {
            get { return _ioDataList; }
            set
            {
                if (_ioDataList != value)
                {
                    _ioDataList = value;
                    RaisePropertyChanged("IoDataList");
                }
            }
        }

        private string _ItemName;
        public string ItemName
        {
            get { return _ItemName; }
            set
            {
                if (_ItemName != value)
                {
                    _ItemName = value;
                    RaisePropertyChanged("ItemName");
                }
            }
        }

        public RelayCommand ItemNameChanged { get; private set; }

        public IoListViewModel()
        {
            Initialize();
            ItemNameChanged = new RelayCommand(() => ItemName_TextChanged());


        }

        private static void CallbackIoValueRead(object state)
        {
            ObservableCollection<IoDataDisplay> ioList = state as ObservableCollection<IoDataDisplay>;

            foreach (var io in ioList)
            {
                Data data = DataManager.Instance.DataAccess.RemoteObject.DataList.Find((o) => (o.Name == io.Name));

                if(data != null)
                {
                    io.Value = data.Value.ToString();
                }
                else
                {
                    continue;
                }
            }
        }

        private void Initialize()
        {
            IoDataList = new ObservableCollection<IoDataDisplay>();
            var _sortedIoData = from ioData in DataManager.Instance.DataAccess.RemoteObject.DataList
                                orderby ioData.Name ascending
                                select ioData;

            foreach (var item in _sortedIoData)
            {
                if (item == null) continue;

                IoDataList.Add(new IoDataDisplay()
                {
                    Name = item.Name,
                    DeviceModule = item.Module,
                    DriverName = item.DriverName,
                    Description = item.Description,
                    Type = item.Type.ToString(),
                    Direction = item.Direction.ToString(),
                    DefaultValue = item.DefaultValue,
                    DataResetTimeout = item.DataResetTimeout,
                    PollingTime = item.PollingTime,
                    Value = item.Value.ToString(),
                    Use = item.Use ? "YES" : "NO"
                });
            }

            alIoDataList = IoDataList;
        }

        private void Start()
        {
            _timer = new Timer(CallbackIoValueRead, IoDataList, 0, 1000);
        }

        private void Stop()
        {
            if(_timer != null)
                _timer.Dispose();
        }
    }
}
