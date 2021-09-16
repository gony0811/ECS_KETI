using ECS.Common.Helper;
using INNO6.Core.Manager;
using INNO6.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Application
{
    public class DataChangedEvent
    {
        public static void DataManager_DataChangedEvent(object sender, DataChangedEventHandlerArgs args)
        {
            switch(args.Data.Name)
            {
                case IoNameHelper.IN_STR_LASER_STATUS_INTERLOCK:
                    {
                        if ((string)args.Data.Value == "NONE") return;

                        switch((string)args.Data.Value)
                        {
                            case "23":
                                AlarmManager.Instance.SetAlarm("E8023");
                                break;
                            case "25":
                                AlarmManager.Instance.SetAlarm("E8025");
                                break;
                            case "32":
                                AlarmManager.Instance.SetAlarm("E8032");
                                break;
                            case "47":
                                AlarmManager.Instance.SetAlarm("E8047");
                                break;
                            case "51":
                                AlarmManager.Instance.SetAlarm("E8051");
                                break;
                            case "64":
                                AlarmManager.Instance.SetAlarm("E8064");
                                break;
                            case "69":
                                AlarmManager.Instance.SetAlarm("E8069");
                                break;
                            case "73":
                                AlarmManager.Instance.SetAlarm("E8073");
                                break;
                            case "75":
                                AlarmManager.Instance.SetAlarm("E8075");
                                break;
                            case "89":
                                AlarmManager.Instance.SetAlarm("E8089");
                                break;
                            case "103":
                                AlarmManager.Instance.SetAlarm("E8103");
                                break;
                            case "123":
                                AlarmManager.Instance.SetAlarm("E8123");
                                break;
                            case "124":
                                AlarmManager.Instance.SetAlarm("E8124");
                                break;
                            case "126":
                                AlarmManager.Instance.SetAlarm("E8126");
                                break;
                            case "129":
                                AlarmManager.Instance.SetAlarm("E8129");
                                break;
                            case "192":
                                AlarmManager.Instance.SetAlarm("E8192");
                                break;
                            case "193":
                                AlarmManager.Instance.SetAlarm("E8193");
                                break;
                            case "198":
                                AlarmManager.Instance.SetAlarm("E8198");
                                break;
                            case "199":
                                AlarmManager.Instance.SetAlarm("E8199");
                                break;
                            case "223":
                                AlarmManager.Instance.SetAlarm("E8223");
                                break;
                            case "231":
                                AlarmManager.Instance.SetAlarm("E8231");
                                break;
                            case "250":
                                AlarmManager.Instance.SetAlarm("E8250");
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
