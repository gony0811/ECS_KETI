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
    public class S5F114 : SFMessage
    {
        List<string> _moduleIdList = new List<string>();
        uint _systemByte;

        public S5F114(SECSDriverBase driver)
            : base(driver)
        {
            Stream = 5; Function = 114;
        }

        string _eqpid;

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "ALARM", "S5F113", null, "UNIT Current Alarm List Request", null);
            SecsMessage primaryMessage = obj as SecsMessage;
            bool bNack = false;
            _systemByte = primaryMessage.SystemByte;

            int list0 = primaryMessage.GetItem().GetList();
            {
                _eqpid = primaryMessage.GetItem().GetAscii();
                int moduleListCount = primaryMessage.GetItem().GetList();

                if (moduleListCount == 0)
                {
                    _moduleIdList = CommonData.Instance.MODULE_SETTINGS.Select(m => m.UNIT_ID).ToList();
                }
                else
                {
                    for (int i = 0; i < moduleListCount; i++)
                    {
                        string recvUnitID = primaryMessage.GetItem().GetAscii().Trim();
                        if (CommonData.Instance.MODULE_SETTINGS.Select(m => m.UNIT_ID).ToList().Contains(recvUnitID)) _moduleIdList.Add(recvUnitID);
                        else bNack = true;
                    }
                }
            }

            SecsMessage reply = new SecsMessage(Stream, Function, _systemByte);

            if (bNack)
            {
                reply.AddList(2);
                {
                    reply.AddAscii(_eqpid);
                    reply.AddList(0);
                }

                SecsDriver.WriteLogAndSendMessage(reply, "");

                CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "ALARM", "S5F114", null, "UNIT Current Alarm List Reply", null);

                return;
            }


            if (_eqpid.Trim() == CommonData.Instance.EQP_SETTINGS.EQPID)
            {
                reply.AddList(2);
                {
                    reply.AddAscii(_eqpid);

                    reply.AddList(_moduleIdList.Count);

                    foreach (string unitID in _moduleIdList)
                    {
                        MODULE module = CommonData.Instance.MODULE_SETTINGS.Where(m => m.UNIT_ID == unitID).FirstOrDefault();

                        if (module != null)
                        {
                            List<ALARM> _alarmList = AlarmManager.Instance.GetDefinedAlarms();
                            List<ALARM> _occurredAlarmList = AlarmManager.Instance.GetCurrentOccurredAlarms().Where(d => d.MODULE == module.MODULE_NAME).ToList();
                            reply.AddList(2);
                            reply.AddAscii(module.UNIT_ID);
                            reply.AddList(_occurredAlarmList.Count);
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
                        else
                        {
                            reply.AddList(0);
                        }
                    }
                }
            }
            else
            {
                reply.AddList(0);
            }


            SecsDriver.WriteLogAndSendMessage(reply, "");

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "ALARM", "S5F114", null, "UNIT Current Alarm List Reply", null);
        }
    }
}
