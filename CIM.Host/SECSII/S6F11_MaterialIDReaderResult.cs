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

namespace CIM.Host.Swin.SECSII
{
    public class S6F11_MaterialIDReaderResult : SFMessage
    {
        /*S6F11(Material ID Reader Result), CEID:605*/
        public S6F11_MaterialIDReaderResult(SECSDriver driver) 
            : base(driver)
        {
            
        }
        public override void DoWork(string driverName, object obj)
        {
            int stream = 6, function = 11;

            string sDataID = "0"; string sMATERIAL_ID = "a"; string sMATERIAL_PORT_ID = "a";
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sCEID = "605";
            #region S6F11(MaterialID Reader Result), CEID:605

            SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));


            msg.AddList(3);                                                                                  //L3  Reader Result Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                //A4  Data ID
                msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                    //A3  Collection Event ID
                msg.AddList(2);                                                                                 //L2  RPTID 810Set
                {
                    msg.AddAscii(AppUtil.ToAscii("810", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="810"
                    msg.AddList(3);                                                                                 //L3  Result Info Set
                    {
                        msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                                  //A40 HOST REQ EQPID
                        msg.AddAscii(AppUtil.ToAscii(sMATERIAL_ID, 40));                                                //A40 EQP Material ID
                        msg.AddAscii(sMATERIAL_PORT_ID);                                                                //A1  Material Port ID
                    }
                }
            }

            this.SecsDriver.Send(msg);
            #endregion
        }

    }
}
