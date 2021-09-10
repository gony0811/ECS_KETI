using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using INNO6.IO;
using INNO6.IO.Service;
using ECS.Common.Helper;

namespace ECS.EMO
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ICommand _EMOStopButtonCommand;

        public ICommand EMOStopButtonCommand
        {
            get
            {
                if (_EMOStopButtonCommand == null)
                {
                    _EMOStopButtonCommand = new RelayCommand(ExecuteEMOStopButtonCommand);
                }

                return _EMOStopButtonCommand;
            }
        }

        public MainWindowViewModel()
        {
            DataConsumer.Instance.Initialize(@"./config/Server.Config.ini");
        }

        private void ExecuteEMOStopButtonCommand()
        {
            DataConsumer.Instance.SET_INT_DATA(IoNameHelper.V_INT_EMO_CMD_ESTOP, 1);
        }
    }
}
