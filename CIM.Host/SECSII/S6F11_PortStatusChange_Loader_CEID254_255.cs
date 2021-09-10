using CIM.Common;
using CIM.Manager;
using INNO6.Core;
using INNO6.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SYSWIN.Secl;

namespace CIM.Host.SECSII
{
    class S6F11_PortStatusChange_Loader_CEID254_255 : SFMessage
    {
        /*S6F11(PortStatusChange_Loader), CEID:254, 255*/
        private string _ceid;
        private string _portName;
        private string _portID;

        public S6F11_PortStatusChange_Loader_CEID254_255(SECSDriverBase driver, string CEID, string PortID, string PortName)
            : base(driver)
        {
            Stream = 6; Function = 11;
            _ceid = CEID; _portName = PortName; _portID = PortID;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            bool bResult;
            INNO6.IO.Interface.eDevMode connectStatus = DataManager.Instance.IsDeviceMode("DEV1");
            string sDATAID = "0";
            string sCEID = _ceid;
            string sCRST = ((int)CommonData.Instance.HOST_MODE).ToString();
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;

            MODULE module = CommonData.Instance.MODULE_SETTINGS.Where(m => m.TYPE == eModuleType.LOADER).FirstOrDefault();

            string portNoTagName = string.Format("i{0}.CassetteStateChange{1}_{2}.PortID", module.MODULE_NAME, _portID, _portName);
            string portAvailableStateTagName = string.Format("i{0}.CassetteStateChange{1}_{2}.PortAvailableState", module.MODULE_NAME, _portID, _portName);
            string portAccessModeTagName = string.Format("i{0}.CassetteStateChange{1}_{2}.PortAccessMode", module.MODULE_NAME, _portID, _portName);
            string portTransferStateTagName = string.Format("i{0}.CassetteStateChange{1}_{2}.PortTransferState", module.MODULE_NAME, _portID, _portName);
            string portProcessStateTagName = string.Format("i{0}.CassetteStateChange{1}_{2}.PortProcessingState", module.MODULE_NAME, _portID, _portName);

            string sPORT_NO = DataManager.Instance.GET_STRING_DATA(portNoTagName, out bResult);
            string sPORT_AVAILABLE_STATE = DataManager.Instance.GET_STRING_DATA(portAvailableStateTagName, out bResult);
            string sPORT_ACCESS_MODE = DataManager.Instance.GET_STRING_DATA(portAccessModeTagName, out bResult);
            string sPORT_TRANSFER_STATE = DataManager.Instance.GET_STRING_DATA(portTransferStateTagName, out bResult);
            string sPORT_PROCESSING_STATE = DataManager.Instance.GET_STRING_DATA(portProcessStateTagName, out bResult);

            #region S6F11(Port State Change), CEID:254,255

            SecsMessage msg = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);                                                                             //L3  EQP Status Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDATAID, gDefine.DEF_DATAID_SIZE));                            //A4  Data ID
                msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                //A3  Collection Event ID
                msg.AddList(2);                                                                             //L4  RPTID Set
                {
                    msg.AddList(2);                                                                             //L2  RPTID 100 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("100", gDefine.DEF_RPTID_SIZE));                               //A3  RPTID="100"
                        msg.AddList(2);                                                                             //L2  EQP Control State Set     
                        {
                            msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                              //A40 HOST REQ EQPID 
                            msg.AddAscii(sCRST);                                                                        //A1  Online Control State
                        }
                    }
                    msg.AddList(2);                                                                         //L2  RPTID 305Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("305", gDefine.DEF_RPTID_SIZE));                               //A3  RPTID="305"
                        msg.AddList(5);                                                                             //L5  Port Set
                        {
                            //PORTSTATE CHANGE??
                            //if (CommonData.Instance.PORTSTATE_CHANGE_1 == true)
                            //{
                                msg.AddAscii(sPORT_NO);                                                                    //A1  Port No
                                msg.AddAscii(sPORT_AVAILABLE_STATE);                                                       //A1  Port Avilability State Info
                                msg.AddAscii(sPORT_ACCESS_MODE);                                                           //A1  Port Access State Info
                                msg.AddAscii(sPORT_TRANSFER_STATE);                                                        //A1  Port Transfer State Info
                                msg.AddAscii(sPORT_PROCESSING_STATE);                                                      //A1  Port Processing State Info
                               //CommonData.Instance.PORTSTATE_CHANGE_1 = false;
                            //}
                        }
                    }
                }
            }
            this.SecsDriver.WriteLogAndSendMessage(msg, sCEID);

            #region for UI
            string Materiallog_name;
            if (sCEID == "254") Materiallog_name = string.Format("Port Available State Change #{0}_{1}", _portID, _portName);
            else if (sCEID == "255") Materiallog_name = string.Format("Port Access Mode Change #{0}_{1}", _portID, _portName);
            else Materiallog_name = "";

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "PORT", "S6F11", sCEID, Materiallog_name, null);
            #endregion
            #endregion
        }
    }
}