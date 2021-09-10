using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core;
using INNO6.IO;
using CIM.Manager;
using CIM.Common;
using SYSWIN.Secl;

namespace CIM.Host.SECSII
{
    public class S5F104 : SFMessage
    {
        List<string> _eqpIdList = new List<string>();
        uint _systemByte;

        public S5F104(SECSDriverBase driver) : base(driver)
        {
            Stream = 5; Function = 104;
        }



        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "ALARM", "S5F103", null, "Current Alarm List Request", null);
            SecsMessage primaryMessage = obj as SecsMessage;

            _systemByte = primaryMessage.SystemByte;

            int eqpListCount = primaryMessage.GetItem().GetList();

            for(int i = 0; i < eqpListCount; i++)
            {
                _eqpIdList.Add(primaryMessage.GetItem().GetAscii());
            }

            SecsMessage reply = new SecsMessage(Stream, Function, _systemByte);



            if(_eqpIdList[0].Trim() == CommonData.Instance.EQP_SETTINGS.EQPID)
            {
                List<ALARM> _alarmList = AlarmManager.Instance.GetDefinedAlarms();
                List<ALARM> _occurredAlarmList = AlarmManager.Instance.GetCurrentOccurredAlarms();

                reply.AddList(_eqpIdList.Count);
                {
                    for (int i = 0; i < _eqpIdList.Count; i++)
                    {
                        reply.AddList(2);
                        {
                            reply.AddAscii(_eqpIdList[i]);
                            reply.AddList(_occurredAlarmList.Count);
                            {
                                if(_occurredAlarmList.Count == 0)
                                {
                                    break;
                                }
                                else
                                {
                                    foreach (ALARM a in _occurredAlarmList)
                                    {
                                        reply.AddList(3);
                                        {
                                            reply.AddAscii(1, ((int)a.LEVEL).ToString());
                                            reply.AddAscii(gDefine.DEF_ALID_SIZE, a.ID);
                                            reply.AddAscii(gDefine.DEF_ALTX_SIZE, a.TEXT);
                                        }
                                    }
                                }

                            }
                        }
                    }

                }
            }
            else
            {
                reply.AddList(0);
            }

            SecsDriver.WriteLogAndSendMessage(reply, "");

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "ALARM", "S5F104", null, "Current Alarm List Reply", null);
        }
    }
}
