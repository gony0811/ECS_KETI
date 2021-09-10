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
    class S6F11_CarrierProcessChange_Loader_CEID256_262 : SFMessage
    {
        /*S6F11(CarrierProcessChange_Loader), CEID:256, 257, 258, 259, 260, 261, 262*/
        private string _cellPortNo;

        public S6F11_CarrierProcessChange_Loader_CEID256_262(SECSDriverBase driver, string CellPortNo)
            : base(driver)
        {
            Stream = 6; Function = 11;
            _cellPortNo = CellPortNo;
        }
        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            bool bResult;

            string sDATAID = "0";
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sCRST = ((int)CommonData.Instance.HOST_MODE).ToString();

            MODULE module = CommonData.Instance.MODULE_SETTINGS.Where(m => m.TYPE == eModuleType.LOADER).FirstOrDefault();

            string ceidTagName = string.Format("i{0}.CarrierProcessChangeLoader.CEID", module.MODULE_NAME);
            string carrierIDTagName = string.Format("i{0}.CarrierProcessChangeLoader.CarrierID", module.MODULE_NAME);
            string carriertypeTagName = string.Format("i{0}.CarrierProcessChangeLoader.CarrierType", module.MODULE_NAME);
            string carrierproductTagName = string.Format("i{0}.CarrierProcessChangeLoader.CarrierProduct", module.MODULE_NAME);
            string carrierstepIDTagName = string.Format("i{0}.CarrierProcessChangeLoader.CarrierStepID", module.MODULE_NAME);
            string carrierscountTagName = string.Format("i{0}.CarrierProcessChangeLoader.CarrierSCount", module.MODULE_NAME);
            string carrierccountTagName = string.Format("i{0}.CarrierProcessChangeLoader.CarrierCCount", module.MODULE_NAME);
            string carrierportNoTagName = string.Format("i{0}.CarrierProcessChangeLoader.PortNo", module.MODULE_NAME);

            string subcarrierIDTagName = string.Format("i{0}.CarrierProcessChangeLoader.SubCarrierID1", module.MODULE_NAME);
            string cellQTYTagName = string.Format("i{0}.CarrierProcessChangeLoader.CellQTY", module.MODULE_NAME);

            string sCEID = DataManager.Instance.GET_INT_DATA(ceidTagName, out bResult).ToString();
            string sCARRIERID = DataManager.Instance.GET_STRING_DATA(carrierIDTagName, out bResult);
            string sCARRIERTYPE = DataManager.Instance.GET_STRING_DATA(carriertypeTagName, out bResult);
            string sCARRIERPRODUCT = DataManager.Instance.GET_STRING_DATA(carrierproductTagName, out bResult);
            string sCARRIERSTEPID = DataManager.Instance.GET_STRING_DATA(carrierstepIDTagName, out bResult);
            string sCARRIER_S_COUNT = DataManager.Instance.GET_INT_DATA(carrierscountTagName, out bResult).ToString();
            string sCARRIER_C_COUNT = DataManager.Instance.GET_INT_DATA(carrierccountTagName, out bResult).ToString();
            string sPORTNO = DataManager.Instance.GET_STRING_DATA(carrierportNoTagName, out bResult);

            string sPPID = RMSManager.Instance.CurrentPPID;

            string sSUBCARRIERID = DataManager.Instance.GET_STRING_DATA(subcarrierIDTagName, out bResult);
            string sCELLQTY = "0";
            if (sCARRIERTYPE != "")
            {
                if(sCARRIERTYPE == "1" || sCARRIERTYPE == "3")
                    CommonData.Instance.VALIDATION_DATA.SaveValidationData(KindOfValidation.SUBCARRIER_INFO_SEND, sSUBCARRIERID + "." + sCARRIERTYPE, Convert.ToInt32(_cellPortNo), module.MODULE_NAME);
                else
                    CommonData.Instance.VALIDATION_DATA.SaveValidationData(KindOfValidation.CARRIER_INFO_SEND, sCARRIERID + "." + sCARRIERTYPE, Convert.ToInt32(_cellPortNo), module.MODULE_NAME);

                //170320 HJKIM : CarrierType 변수에 저장 -> S3F115 Message에서 비교
                CommonData.Instance.LOADER_CARRIERTYPE = sCARRIERTYPE;
            }
            /*
            <L, 4 * Attr Info Set
                1.<A[40] $EQPID> * 설비 고유 ID
                2.<A[20] $OBJTYPE> * 개체 유형
                3.<A[20] $OBJID> * 개체 정보 I D
                4.<A[20] $COMMENT> * 추가 정보
            */

            #region S6F11(CarrierProcessChange), CEID:256, 257, 258, 259, 260, 261, 262
            SecsMessage msg = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);
            {
                msg.AddAscii(sDATAID);
                msg.AddAscii(sCEID);
                msg.AddList(3);
                {
                    msg.AddList(2);
                    {
                        msg.AddAscii("100");
                        msg.AddList(2);
                        {
                            msg.AddAscii(sEQPID);
                            msg.AddAscii(sCRST);
                        }
                    }
                    msg.AddList(2);
                    {
                        msg.AddAscii("309");
                        msg.AddList(8);
                        {
                            msg.AddAscii(sCARRIERID);
                            msg.AddAscii(sCARRIERTYPE);
                            msg.AddAscii(sPPID);
                            msg.AddAscii(sCARRIERPRODUCT);
                            msg.AddAscii(sCARRIERSTEPID);
                            msg.AddAscii(sCARRIER_S_COUNT);
                            msg.AddAscii(sCARRIER_C_COUNT);
                            msg.AddAscii(sPORTNO);
                        }
                    }
                    msg.AddList(2);
                    {
                        msg.AddAscii("310");
                        if (sCARRIERTYPE == ((int)CarrierType.UseBatchID).ToString() ||
                            sCARRIERTYPE == ((int)CarrierType.NotUseBatchID).ToString() ||
                            sCARRIERTYPE == ((int)CarrierType.ForcedUseBatchID).ToString() ||
                            sCARRIERTYPE == ((int)CarrierType.ForcedNotUseBatchID).ToString())
                        {
                            //170320 HJKIM : CarrierID 변수에 저장 -> S3F115 Message에서 비교
                            CommonData.Instance.LOADER_CARRIERID = sCARRIERID;

                            //Standard Loader Carrier Batch Lot Information Request(CEID : 262)일 때는 Carrier ID와 Carreir Type만 올려주면 됨.
                            if (sCEID == "262")
                            {
                                msg.AddList(0);
                            }
                            else
                            {//Standard Loader Carrier Assign Request(CEID:260) 빈 Tray Batch Assign 요청
                                int nTrayQty = Convert.ToInt32(sCARRIER_S_COUNT);
                                //group의 의미가 뭔지 모르겠음
                                List<string> SCID = CommonData.Instance.GetSubCarrierIDs(module.MODULE_NAME, "LoaderSubCarrierID", nTrayQty);

                                msg.AddList(nTrayQty);
                                {
                                    for (int i = 0; i < nTrayQty; i++)
                                    {
                                        sSUBCARRIERID = SCID[i];

                                        msg.AddList(3);
                                        {
                                            msg.AddAscii(sSUBCARRIERID);
                                            msg.AddAscii(sCELLQTY);
                                            msg.AddList(0);
                                        }
                                    }
                                }
                            }
                        }
                        // Tray Cell Lot
                        else if (sCARRIERTYPE == ((int)CarrierType.UseTrayID).ToString() ||
                                 sCARRIERTYPE == ((int)CarrierType.NotUseTrayID).ToString() ||
                                 sCARRIERTYPE == ((int)CarrierType.ForcedUseTrayID).ToString() ||
                                 sCARRIERTYPE == ((int)CarrierType.ForcedNotUseTrayID).ToString())
                        {
                            //Standard Loader Carrier Release Request(CEID : 256) 요청 (Cell 정보 필요 없음)
                            msg.AddList(1);
                            {
                                msg.AddList(3);
                                {
                                    sSUBCARRIERID = DataManager.Instance.GET_STRING_DATA(subcarrierIDTagName, out bResult);
                                    sCELLQTY = DataManager.Instance.GET_INT_DATA(cellQTYTagName, out bResult).ToString();
                                    //170320 HJKIM : SubCarrierID 변수에 저장 -> S3F115 Message에서 비교
                                    CommonData.Instance.LOADER_SUBCARRIERID = sSUBCARRIERID;

                                    msg.AddAscii(sSUBCARRIERID);
                                    msg.AddAscii(sCELLQTY);
                                    msg.AddList(0);
                                }
                            }
                        }
                        else
                        {
                            msg.AddList(0);
                        }
                    }
                }
            }
            this.SecsDriver.WriteLogAndSendMessage(msg, sCEID);

            #region for UI
            string Materiallog_name;
            if (sCEID == "256") Materiallog_name = "Carrier Release Request";
            else if (sCEID == "257") Materiallog_name = "Carrier Release Complete";
            else if (sCEID == "258") Materiallog_name = "Carrier Process Start";
            else if (sCEID == "259") Materiallog_name = "Carrier Process End";
            else if (sCEID == "260") Materiallog_name = "Carrier Assign Request";
            else if (sCEID == "261") Materiallog_name = "Carrier Assign Complete";
            else if (sCEID == "262") Materiallog_name = "Carrier Information Request";
            else Materiallog_name = "";
            CommonData.Instance.OnStreamFunctionAdd("LOADER", "E->H", "MARTERIAL", "S6F11", sCEID, Materiallog_name, null);
            #endregion
            #endregion
        }
    }
}
