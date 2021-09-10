using CIM.Common;
using CIM.Manager;
using SDC.Core;
using SDC.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SYSWIN.Secl;
using System.Threading.Tasks;

namespace CIM.Host.Swin.SECSII
{
    public class S6F11_RFIDReaderResult : SFMessage
    {
        /*S6F11(RFID Reader Result), CEID:604*/
        public S6F11_RFIDReaderResult(SECSDriver driver) 
            : base(driver)
        {
            
        }
        public override void DoWork(string driverName, object obj)
        {
            int stream = 6, function = 11;

            string sDataID = "0";
            string sREADER_RESULT_CODE = "a"; string sREADER_ID = "a"; string sRFID = "a";
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sCEID = "604";
            #region S6F11(RFID Reader Result), CEID:604

            SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);                                                                                         //L3  Reader Result Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                        //A4  Data ID
                msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                            //A3  Collection Event ID
                msg.AddList(2);                                                                                         //L3  RPTID 805 Set
                {
                    msg.AddAscii(AppUtil.ToAscii("805", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID="805"
                    msg.AddList(4);                                                                                         //L4  RFID Result Info Set
                    {
                        msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                                          //A40 HOST REQ EQPID
                        msg.AddAscii(AppUtil.ToAscii(sRFID, gDefine.DEF_RFID_SIZE));                                            //A40 Cell Unique ID
                        msg.AddAscii(AppUtil.ToAscii(sREADER_ID, gDefine.DEF_READER_ID_SIZE));                                  //A10 MCR Reader Position/Order Info
                        msg.AddAscii(sREADER_RESULT_CODE);                                                                      //A1  Reading Result Value
                    }
                }
            }

            this.SecsDriver.Send(msg);
            #endregion
        }
    }
}
