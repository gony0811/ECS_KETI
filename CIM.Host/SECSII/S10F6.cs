using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core;
using INNO6.IO;
using CIM.Manager;
using CIM.Common;
using CIM.Common.TC;
using SYSWIN.Secl;

namespace CIM.Host.SECSII
{
    public class S10F6 : SFMessage
    {
        public S10F6(SECSDriverBase driver)
            : base(driver)
        {
            Stream = 10; Function = 6;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "TERMINAL", "S10F5", null, "Terminal Message Set", null);
            SecsMessage primaryMessage = obj as SecsMessage;

            uint systemByte = primaryMessage.SystemByte;
            bool dataSetResult = true;
            string EQPID;
            string TID;
            string TEXT;

            HCACK HCACK = HCACK.ACCEPTED;

            int nList = primaryMessage.GetItem().GetList();                                         //L3 Teminal Set
            {
                EQPID = primaryMessage.GetItem().GetAscii();
                TID = primaryMessage.GetItem().GetAscii();
                nList = primaryMessage.GetItem().GetList();
                TEXT = primaryMessage.GetItem().GetAscii();

                if(EQPID.ToUpper().Trim() != CommonData.Instance.EQP_SETTINGS.EQPID)
                {
                    HCACK = Common.TC.HCACK.REJECT_ALREADYINDESIRECONDITION;
                }
                else
                {
                    foreach (MODULE module in CommonData.Instance.MODULE_SETTINGS)
                    {
                        string tagTID = string.Format("o{0}.Terminal.TID", module.MODULE_NAME);
                        string tagText = string.Format("o{0}.Terminal.Text", module.MODULE_NAME);

                        string tagTerminalEvent = string.Format("o{0}.Send.TerminalDisply", module.MODULE_NAME);

                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagTID, TID);
                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagText, TEXT);

                        dataSetResult &= DataManager.Instance.SET_INT_DATA(tagTerminalEvent, 1);

                        if (!dataSetResult)
                        {
                            //Console.WriteLine("[Error] Termianl data set failed : Module Name ({0})", module.MODULE_NAME);
                            LogHelper.Instance.TerminalLog.DebugFormat("[ERROR] Termianl data set failed : Module Name ({0})", module.MODULE_NAME);

                        }

                    }
                }



                string sHCACK = ((int)HCACK).ToString();

                SecsMessage reply = new SecsMessage(Stream, Function, systemByte)
                {
                    WaitBit = false
                };
                reply.AddAscii(sHCACK);                                                             //A1 Host Command Acknowledge

                CommonData.Instance.OnTerminalChanged("", EQPID, TID, TEXT);

                SecsDriver.WriteLogAndSendMessage(reply, sHCACK);
            }
        }
    }
}
