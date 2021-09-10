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
    public class S6F11_CarrierProcessChange_Loader : SFMessage
    {
        /*S6F11(CarrierProcessChange), CEID:256, 257, 258, 259, 260, 261*/
    
        public S6F11_CarrierProcessChange_Loader(SECSDriver driver) 
            : base(driver)
        {
            
        }
        public override void DoWork(string driverName, object obj)
        {
            short stream = 6, function = 11;
            bool bResult;

            string sDataID = "0";
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sCEID = DataManager.Instance.GET_INT_DATA("iPLC7_EtoC_Loader_CarrierProcessChangeCEID", out bResult).ToString();
            string sCRST = ((int)DataManager.Instance.GET_INT_DATA("vSys_HostData_Mode", out bResult)).ToString();
            string sCarrierID = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_Loader_CarrierProcessChangeCarrierID", out bResult);
            string sCarrierType = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_Loader_CarrierProcessChangeCarrierType", out bResult);
            string sPPID = RMSManager.Instance.GetCurrentPPID();
            string sCarrierProduct = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_Loader_CarrierProcessChangeCarrierProduct", out bResult);
            string sCarrierStepID = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_Loader_CarrierProcessChangeCarrierStepID", out bResult);
            string sCarrierSCount = DataManager.Instance.GET_INT_DATA("iPLC7_EtoC_Loader_CarrierProcessChangeTrayQty", out bResult).ToString();
            string sCarrierCCount = DataManager.Instance.GET_INT_DATA("iPLC7_EtoC_Loader_CarrierProcessChangeTrayCellQty", out bResult).ToString(); ; // Carrier(Batch) 내 모든 Cell 수량
            string sCarrierPortNo = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_Loader_CarrierProcessChangePortNo", out bResult);
            string sSubCarrierID = string.Empty;
            string sCarrierCellQty = DataManager.Instance.GET_INT_DATA("iPLC7_EtoC_Loader_CarrierProcessChangeCellQty", out bResult).ToString();
            int nSubCarrierQty = 0;
            //int nCellQty = 0;

            
            SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);
            {
                msg.AddAscii(sDataID);
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
                            msg.AddAscii(sCarrierID);
                            msg.AddAscii(sCarrierType);
                            msg.AddAscii(sPPID);
                            msg.AddAscii(sCarrierProduct);
                            msg.AddAscii(sCarrierStepID);
                            msg.AddAscii(sCarrierSCount);
                            msg.AddAscii(sCarrierCCount);
                            msg.AddAscii(sCarrierPortNo);
                        }
                    }
                    msg.AddList(2);
                    {
                        msg.AddAscii("310");
                        if (sCarrierType == ((int)CommonData.CarrierType.UseBatchID).ToString() || sCarrierType == ((int)CommonData.CarrierType.NotUseBatchID).ToString() ||
                            sCarrierType == ((int)CommonData.CarrierType.ForcedUseBatchID).ToString() || sCarrierType == ((int)CommonData.CarrierType.ForcedNotUseBatchID).ToString())
                        {
                            //170320 HJKIM : CEID : 260 or 262 일 경우, CarrierID를 변수에 저장.
                            DataManager.Instance.SET_STRING_DATA("vCim_Loader_TrayID", sCarrierID);
                            DataManager.Instance.SET_STRING_DATA("vCim_Loader_CarrierType", sCarrierType);

                            //Carrier Batch Lot Release Request(CEID : 256)일 때는 Carrier ID와 Carreir Type만 올려주면 됨.
                            if (sCEID == "262")
                            {
                                msg.AddList(0);
                            }
                            else
                            {//170314 HJKIM : Unloader에서 Batch Lot Carrier Assign Reqeust(CEID : 260)일 때는 전체 Tray ID 올려줘야 함.
                                List<string> SubCarrierList = new List<string>();
                                SubCarrierList = CommonData.Instance.GetSubCarrierIDs("PLC7", " LD_SUBCARID");
                                nSubCarrierQty = SubCarrierList.Count;

                                sCarrierCellQty = "0";

                                msg.AddList(nSubCarrierQty);
                                {
                                    foreach (string subcarrierid in SubCarrierList)
                                    {
                                        sSubCarrierID = subcarrierid;

                                        msg.AddList(3);
                                        {
                                            msg.AddAscii(sSubCarrierID);
                                            msg.AddAscii(sCarrierCellQty);
                                            msg.AddList(0);
                                        }
                                    }
                                }
                            }
                        }
                        else if (sCarrierType == ((int)CommonData.CarrierType.UseTrayID).ToString() || sCarrierType == ((int)CommonData.CarrierType.NotUseTrayID).ToString() ||
                            sCarrierType == ((int)CommonData.CarrierType.ForcedUseTrayID).ToString() || sCarrierType == ((int)CommonData.CarrierType.ForcedNotUseTrayID).ToString())
                        {/*170314 HJKIM : Loader 설비는 Tray ID로 보고하는 항목은 Carrier Release Request(CEID : 256) 밖에 없음.
                                          Assign이 아니기 때문에, Cell 정보 필요없음. */
                            
                            msg.AddList(1);
                            {
                                sSubCarrierID = DataManager.Instance.GET_STRING_DATA("iPLC7_EtoC_Loader_CarrierProcessChangeSubCarrierID1", out bResult);

                                msg.AddList(3);
                                msg.AddAscii(sSubCarrierID);
                                msg.AddAscii(sCarrierCellQty);
                                msg.AddList(0);
                                {
                                    //foreach (BATCHInfo.tagCELLINFO cellinfo in CellInfoList)
                                    //{
                                    //    string sCellID = cellinfo.CellID;
                                    //    string sLocationNo = cellinfo.LocationNo;
                                    //    string sJudge = cellinfo.Judge;
                                    //    string sReasonCode = cellinfo.ReasonCode;

                                    //    msg.WriteNode(SX.SECSFormat.L, 4, "", "");
                                    //    {
                                    //        msg.WriteNode(sCellID);
                                    //        msg.WriteNode(sLocationNo);
                                    //        msg.WriteNode(sJudge);
                                    //        msg.WriteNode(sReasonCode);
                                    //    }
                                    //}
                                }
                            }
                        }
                    }
                }
            }

            this.SecsDriver.Send(msg);
        }
    }
}
