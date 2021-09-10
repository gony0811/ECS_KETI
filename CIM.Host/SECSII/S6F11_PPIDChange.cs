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
    public class S6F11_PPIDChange : SFMessage
    {
        /*S6F11(PPID Change), CEID:107*/
        public S6F11_PPIDChange(SECSDriver driver) 
            : base(driver)
        {
            
        }
        public override void DoWork(string driverName, object obj)
        {
            int stream = 6, function = 11;
            bool bResult;
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sCEID = "107";

            string sDataID = "0";
            string sCRST = ((int)CommonData.Instance.HOST_MODE).ToString();
            string sPPID = RMSManager.Instance.GetCurrentPPID();
            string sOLD_PPID = RMSManager.Instance.PostPPID;
            string sPPID_TYPE = "1";

            #region S6F11(PPID Change), CEID: CEID: 107

            SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);                                                                                 //L3  PPID Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                 //A4  Data ID
                msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                     //A3  Collection Event ID
                msg.AddList(2);                                                                                  //L2  RPTID Set
                {
                    msg.AddList(2);                                                                                 //L2 RPTID 100 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("100", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="100"
                        msg.AddList(2);                                                                                 //L2  EQP Control State Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                                  //A40 HOST REQ EQPID
                            msg.AddAscii(sCRST);                                                                            //A1  Online Control State
                        }
                    }
                    msg.AddList(2);                                                                                 //L2 RPTID 302 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("302", gDefine.DEF_RPTID_SIZE));                                   //A3 RPTID="302"
                        msg.AddList(3);                                                                                 //L3 PPID Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sPPID, gDefine.DEF_PPID_SIZE));                                    //A40 Changed PPID 
                            msg.AddAscii(sPPID_TYPE);                                                                       //A1  PPID Type
                            msg.AddAscii(AppUtil.ToAscii(sOLD_PPID, 40));                                                   //A40 Original PPID
                        }
                    }
                }
            }

            this.SecsDriver.Send(msg);
            #endregion
        }
    }
}
