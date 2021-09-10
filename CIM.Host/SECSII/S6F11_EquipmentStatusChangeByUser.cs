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
    public class S6F11_EquipmentStatusChangeByUser : SFMessage
    {
        /*S6F11(Equipment Status Change By User), CEID:603*/
        public S6F11_EquipmentStatusChangeByUser(SECSDriver driver) 
            : base(driver)
        {
            
        }
        public override void DoWork(string driverName, object obj)
        {
            int stream = 6, function = 11;

            string sDataID = "0";
            string sDATA_TYPE = "a"; string sADDRESS = "a"; string sVALUE = "a"; string sLOSS_DISPLAY = "a"; string sLOSS = "a";
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sCEID = "603";
            #region S6F11(Equipment Status Change By User), CEID:603

            SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);                                                                                         //L3  EQP Status Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                        //A4  Data ID
                msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                            //A3  Collection Event ID
                msg.AddList(3);                                                                                         //L3  RPTID Set
                {
                    msg.AddList(2);                                                                                         //L2  RPTID 802 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("802", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID="802"
                        msg.AddList(2);                                                                                         //L2  TYPE Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                                          //A40 HOST REQ EQPID
                            msg.AddAscii(AppUtil.ToAscii(sDATA_TYPE, 10));                                                          //A10 DATA TYPE
                        }
                    }
                    msg.AddList(2);                                                                                         //L2  RPTID 803 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("803", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID="803"
                        msg.AddList(2);                                                                                         //L2  PLC Data
                        {
                            msg.AddAscii(AppUtil.ToAscii(sADDRESS, 20));                                                            //A20 PLC Type Loss Code
                            msg.AddAscii(AppUtil.ToAscii(sVALUE, 20));                                                              //A20 Value
                        }
                    }
                    msg.AddList(2);                                                                                         //L2  RPTID 804 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("804", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID="804"
                        msg.AddList(2);                                                                                         //L2  PC Data
                        {
                            msg.AddAscii(AppUtil.ToAscii(sLOSS_DISPLAY, 20));                                                       //A20 PC Type Loss Code
                            msg.AddAscii(AppUtil.ToAscii(sLOSS, 20));                                                               //A20 Loss Code
                        }
                    }
                }
            }

            this.SecsDriver.Send(msg);
            #endregion
        }
    }
}
