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
    public class S6F11_OperatorLogInOutInformation : SFMessage
    {
        /*S6F11(Operator Information Report), CEID: 607*/
        public S6F11_OperatorLogInOutInformation(SECSDriver driver)
            : base(driver)
        {

        }
        public override void DoWork(string driverName, object obj)
        {
            int stream = 6, function = 11;
            bool bResult;
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sCEID = "607";

            string sDataID = "0";
            string sOPTIONINFO = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_OperatorInfoOption", out bResult);
            string sCOMMENT = "";
            string sOPERATORID = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_OperatorInfoOPID", out bResult);
            string sOPERATORPW = DataManager.Instance.GET_STRING_DATA("iPLC1_EtoC_OperatorInfoOPPW", out bResult);

            #region S6F11(Operator Infomation Report), CEID:607

            SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);                                                                                  //L3  EQP Status Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                //A4  Data ID
                msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                    //A3  Collection Event ID
                msg.AddList(2);                                                                                 //L3  RPTID Set
                {
                    msg.AddList(2);                                                                                 //L2  RPTID 100 Set 
                    {
                        msg.AddAscii(AppUtil.ToAscii("105", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="105" 
                        msg.AddList(3);                                                                                 //L3  EQP Control State Set 
                        {
                            msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                                  //A40 HOST REQ EQPID 
                            msg.AddAscii(sOPTIONINFO);
                            msg.AddAscii(sCOMMENT);
                        }
                    }
                    msg.AddList(2);                                                                                 //L2  RPTID 101 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("106", gDefine.DEF_RPTID_SIZE));                                   //A3  RPTID="101"
                        msg.AddList(2);                                                                                 //L9  EQP State Set
                        {
                            msg.AddAscii(sOPERATORID);                                                               //A1  EQ Avilability State Info
                            msg.AddAscii(sOPERATORPW);                                                                  //A1  Interlock Avilability State Info
                        }
                    }
                }
            }
            this.SecsDriver.Send(msg);
            #endregion
        }
    }
}
