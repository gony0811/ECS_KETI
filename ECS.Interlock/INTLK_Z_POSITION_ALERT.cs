using INNO6.Core.Interlock.Interface;
using INNO6.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core;
using INNO6.IO;

namespace ECS.Interlock
{
    public class INTLK_Z_POSITION_ALERT : IExecuteInterlock
    {
        private const string IO_GET_X_POSITION = "iPMAC.dAxisX.Position";
        private const string IO_GET_Y_POSITION = "iPMAC.dAxisY.Position";
        private const string IO_GET_Z_POSITION = "iPMAC.dAxisZ.Position";
        private const string IO_GET_R_POSITION = "iPMAC.dAxisR.Position";
        private const string IO_GET_T_POSITION = "iPMAC.dAxisX.Position";

        public void Execute()
        {
            double xPosition = DataManager.Instance.GET_DOUBLE_DATA(IO_GET_X_POSITION, out bool _);
            double yPosition = DataManager.Instance.GET_DOUBLE_DATA(IO_GET_Y_POSITION, out bool _);
            double zPosition = DataManager.Instance.GET_DOUBLE_DATA(IO_GET_Z_POSITION, out bool _);
            double rPosition = DataManager.Instance.GET_DOUBLE_DATA(IO_GET_R_POSITION, out bool _);
            double tPosition = DataManager.Instance.GET_DOUBLE_DATA(IO_GET_T_POSITION, out bool _);

            //if ()
            AlarmManager.Instance.SetAlarm("E5002");


        }
    }
}
