using ECS.Common.Helper;
using INNO6.Core.Interlock.Interface;
using INNO6.Core.Manager;
using INNO6.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Interlock.SetPoint
{
    public class I_LASER_INTERLOCK : AbstractExecuteInterlock
    {
        public override bool Execute(object setvalue)
        {
            string statusCode = setvalue as string;

            switch (statusCode)
            {
                case "1":
                    AlarmManager.Instance.SetAlarm("E8001");
                    break;
                case "2":
                    AlarmManager.Instance.SetAlarm("E8002");
                    break;
                case "6":
                    AlarmManager.Instance.SetAlarm("E8006");
                    break;
                case "10":
                    AlarmManager.Instance.SetAlarm("E8010");
                    break;
                case "11":
                    AlarmManager.Instance.SetAlarm("E8011");
                    break;
                case "16":
                    AlarmManager.Instance.SetAlarm("E8016");
                    break;
                case "18":
                    AlarmManager.Instance.SetAlarm("E8018");
                    break;
                case "26":
                    AlarmManager.Instance.SetAlarm("E8026");
                    break;
                case "27":
                    AlarmManager.Instance.SetAlarm("E8027");
                    break;
                case "30":
                    AlarmManager.Instance.SetAlarm("E8030");
                    break;
                case "31":
                    AlarmManager.Instance.SetAlarm("E8031");
                    break;
                case "32":
                    AlarmManager.Instance.SetAlarm("E8032");
                    break;
                case "42":
                    AlarmManager.Instance.SetAlarm("E8042");
                    break;
                case "43":
                    AlarmManager.Instance.SetAlarm("E8043");
                    break;
                case "49":
                    AlarmManager.Instance.SetAlarm("E8049");
                    break;
                case "54":
                    AlarmManager.Instance.SetAlarm("E8054");
                    break;
                case "62":
                    AlarmManager.Instance.SetAlarm("E8062");
                    break;
                case "74":
                    AlarmManager.Instance.SetAlarm("E8074");
                    break;
                case "95":
                    AlarmManager.Instance.SetAlarm("E8095");
                    break;
                case "106":
                    AlarmManager.Instance.SetAlarm("E8106");
                    break;
                case "107":
                    AlarmManager.Instance.SetAlarm("E8107");
                    break;
                case "120":
                    AlarmManager.Instance.SetAlarm("E8120");
                    break;
                case "122":
                    AlarmManager.Instance.SetAlarm("E8122");
                    break;
                case "125":
                    AlarmManager.Instance.SetAlarm("E8125");
                    break;
                case "127":
                    AlarmManager.Instance.SetAlarm("E8127");
                    break;
                case "128":
                    AlarmManager.Instance.SetAlarm("E8128");
                    break;
                case "130":
                    AlarmManager.Instance.SetAlarm("E8130");
                    break;
                case "157":
                    AlarmManager.Instance.SetAlarm("E8157");
                    break;
                case "182":
                    AlarmManager.Instance.SetAlarm("E8182");
                    break;
                case "203":
                    AlarmManager.Instance.SetAlarm("E8203");
                    break;
                case "204":
                    AlarmManager.Instance.SetAlarm("E8204");
                    break;
                case "205":
                    AlarmManager.Instance.SetAlarm("E8205");
                    break;
                case "210":
                    AlarmManager.Instance.SetAlarm("E8210");
                    break;
                case "211":
                    AlarmManager.Instance.SetAlarm("E8211");
                    break;
                case "220":
                    AlarmManager.Instance.SetAlarm("E8220");
                    break;
                case "221":
                    AlarmManager.Instance.SetAlarm("E8221");
                    break;
                case "224":
                    AlarmManager.Instance.SetAlarm("E8224");
                    break;
                default:
                    return false;
            }

            return true;
        }
    }
}
