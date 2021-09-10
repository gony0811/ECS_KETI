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
    class S6F11_MaterialAssembleProcess_CEID215 : SFMessage
    {
        /*S6F11(MaterialAssembleProcess), CEID :215*/

        private string _cellPortNo;
        private string _matNo;
           
        public S6F11_MaterialAssembleProcess_CEID215(SECSDriverBase driver, string CellPortNo, string MatNo)
            : base(driver)
        {
            Stream = 6; Function = 11;
            _cellPortNo = CellPortNo; _matNo = MatNo;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            Data data = obj as Data;
            bool bResult;
            INNO6.IO.Interface.eDevMode connectStatus = DataManager.Instance.IsDeviceMode("DEV1");
            string sDATAID = "0";
            string sCRST = ((int)CommonData.Instance.HOST_MODE).ToString();
            string sEQPID = CommonData.Instance.EQP_SETTINGS.EQPID;
            string sCEID = "215";
            
            string availabilityTagName = string.Format("i{0}.EQStatus.Availability", data.Module);
            string interlockTagName = string.Format("i{0}.EQStatus.Interlock", data.Module);
            string moveStateTagName = string.Format("i{0}.EQStatus.Move", data.Module);
            string runStateTagName = string.Format("i{0}.EQStatus.Run", data.Module);
            string frontStateTagName = string.Format("i{0}.EQStatus.Front", data.Module);
            string rearStateTagName = string.Format("i{0}.EQStatus.Rear", data.Module);
            string PPStateTagName = string.Format("i{0}.EQStatus.PP_SPL", data.Module);
            string reasonCodeTagName = string.Format("i{0}.Availability.ReasonCode", data.Module);
            string descriptionTagName = string.Format("i{0}.Availability.Description", data.Module);

            string sAVAILABILITYSTATE = DataManager.Instance.GET_STRING_DATA(availabilityTagName, out bResult);
            string sINTERLOCKSTATE = DataManager.Instance.GET_STRING_DATA(interlockTagName, out bResult);
            string sMOVESTATE = DataManager.Instance.GET_STRING_DATA(moveStateTagName, out bResult);
            string sRUNSTATE = DataManager.Instance.GET_STRING_DATA(runStateTagName, out bResult);
            string sFRONTSTATE = DataManager.Instance.GET_STRING_DATA(frontStateTagName, out bResult);
            string sREARSTATE = DataManager.Instance.GET_STRING_DATA(rearStateTagName, out bResult);
            string sPP_SPLSTATE = DataManager.Instance.GET_STRING_DATA(PPStateTagName, out bResult);
            string sREASONCODE = DataManager.Instance.GET_STRING_DATA(reasonCodeTagName, out bResult);
            string sDESCRIPTION = DataManager.Instance.GET_STRING_DATA(descriptionTagName, out bResult);

            short nMAT_NO = 1;
            //short nMAT_NO = Convert.ToInt16(MATERIAL.MATERIALDEFINE.MATERIAL_AMOUT);  // (현재는)1개 고정

            string cellIDTagName = string.Format("i{0}.MaterialAssemble.CellID{1}_{2}", data.Module, _cellPortNo, _matNo);
            string ppIDTagName = string.Format("i{0}.MaterialAssemble.PPID{1}_{2}", data.Module, _cellPortNo, _matNo);
            string productIDTagName = string.Format("i{0}.MaterialAssemble.ProductID{1}_{2}", data.Module, _cellPortNo, _matNo);
            string stepIDTagName = string.Format("i{0}.MaterialAssemble.StepID{1}_{2}", data.Module, _cellPortNo, _matNo);

            string EQmatbatIDTagName = string.Format("i{0}.MaterialAssemble.MaterialBatchID{1}_{2}", data.Module, _cellPortNo, _matNo);
            string EQmatbatnameTagName = string.Format("i{0}.MaterialAssemble.MaterialBatchName{1}_{2}", data.Module, _cellPortNo, _matNo);
            string EQmatIDTagName = string.Format("i{0}.MaterialAssemble.MaterialID{1}_{2}", data.Module, _cellPortNo, _matNo);
            string EQmattypeTagName = string.Format("i{0}.MaterialAssemble.MaterialType{1}_{2}", data.Module, _cellPortNo, _matNo);
            string EQmatSTTagName = string.Format("i{0}.MaterialAssemble.MaterialST{1}_{2}", data.Module, _cellPortNo, _matNo);
            string EQmatportIDTagName = string.Format("i{0}.MaterialAssemble.MaterialPortID{1}_{2}", data.Module, _cellPortNo, _matNo);
            string EQmatstateTagName = string.Format("i{0}.MaterialAssemble.MaterialState{1}_{2}", data.Module, _cellPortNo, _matNo);
            string EQmattotalQTYTagName = string.Format("i{0}.MaterialAssemble.MaterialTotalQTY{1}_{2}", data.Module, _cellPortNo, _matNo);
            string EQmatuseQTYTagName = string.Format("i{0}.MaterialAssemble.MaterialUseQTY{1}_{2}", data.Module, _cellPortNo, _matNo);
            string EQmatassemQTYTagName = string.Format("i{0}.MaterialAssemble.MaterialAssemQTY{1}_{2}", data.Module, _cellPortNo, _matNo);
            string EQmatngQTYTagName = string.Format("i{0}.MaterialAssemble.MaterialNGQTY{1}_{2}", data.Module, _cellPortNo, _matNo);
            string EQmatremainQTYTagName = string.Format("i{0}.MaterialAssemble.MaterialRemainQTY{1}_{2}", data.Module, _cellPortNo, _matNo);
            string EQmatproductQTYTagName = string.Format("i{0}.MaterialAssemble.MaterialProductQTY{1}_{2}", data.Module, _cellPortNo, _matNo);
            string EQmatproceuseQTYTagName = string.Format("i{0}.MaterialAssemble.MaterialProcessUseQTY{1}_{2}", data.Module, _cellPortNo, _matNo);
            string EQmatproceassemQTYTagName = string.Format("i{0}.MaterialAssemble.MaterialProcessAssemQTY{1}_{2}", data.Module, _cellPortNo, _matNo);
            string EQmatprocngQTYTagName = string.Format("i{0}.MaterialAssemble.MaterialProcessNGQTY{1}_{2}", data.Module, _cellPortNo, _matNo);
            string EQmatsupplyrequestQTYTagName = string.Format("i{0}.MaterialAssemble.MaterialSupplyRequestQTY{1}_{2}", data.Module, _cellPortNo, _matNo);

            string sCELLID = DataManager.Instance.GET_STRING_DATA(cellIDTagName, out bResult);
            string sPPID = DataManager.Instance.GET_STRING_DATA(ppIDTagName, out bResult);
            string sPRODUCTID = DataManager.Instance.GET_STRING_DATA(productIDTagName, out bResult);
            string sSTEPID = DataManager.Instance.GET_STRING_DATA(stepIDTagName, out bResult);

            string[] aEQ_MAT_BAT_ID = new string[nMAT_NO];
            string[] aEQ_MAT_BAT_NAME = new string[nMAT_NO];
            string[] aEQ_MAT_ID = new string[nMAT_NO];
            string[] aEQ_MAT_TYPE = new string[nMAT_NO];
            string[] aEQ_MAT_ST = new string[nMAT_NO];
            string[] aEQ_MAT_PORT_ID = new string[nMAT_NO];
            string[] aEQ_MAT_STATE = new string[nMAT_NO];
            string[] aEQ_MAT_TOTAL_QTY = new string[nMAT_NO];
            string[] aEQ_MAT_USE_QTY = new string[nMAT_NO];
            string[] aEQ_MAT_ASSEM_QTY = new string[nMAT_NO];
            string[] aEQ_MAT_NG_QTY = new string[nMAT_NO];
            string[] aEQ_MAT_REMAINQTY_QTY = new string[nMAT_NO];
            string[] aEQ_MAT_PRODUCT_QTY = new string[nMAT_NO];
            string[] aEQ_MAT_PROCE_USE_QTY = new string[nMAT_NO];
            string[] aEQ_MAT_PROCE_ASSEM_QTY = new string[nMAT_NO];
            string[] aEQ_MAT_PROC_NG_QTY = new string[nMAT_NO];
            string[] aEQ_MAT_SUPPLY_REQUEST_QTY = new string[nMAT_NO];

            for (int i = 0; i < nMAT_NO; i++)
            {
                aEQ_MAT_BAT_ID[i] = "";
                aEQ_MAT_BAT_NAME[i] = "";
                aEQ_MAT_ID[i] = "";
                aEQ_MAT_TYPE[i] = "";
                aEQ_MAT_ST[i] = "";
                aEQ_MAT_PORT_ID[i] = "";
                aEQ_MAT_STATE[i] = "";
                aEQ_MAT_TOTAL_QTY[i] = "";
                aEQ_MAT_USE_QTY[i] = "";
                aEQ_MAT_ASSEM_QTY[i] = "";
                aEQ_MAT_NG_QTY[i] = "";
                aEQ_MAT_REMAINQTY_QTY[i] = "";
                aEQ_MAT_PRODUCT_QTY[i] = "";
                aEQ_MAT_PROCE_USE_QTY[i] = "";
                aEQ_MAT_PROCE_ASSEM_QTY[i] = "";
                aEQ_MAT_PROC_NG_QTY[i] = "";
                aEQ_MAT_SUPPLY_REQUEST_QTY[i] = "";
            }

            aEQ_MAT_BAT_ID[0] = DataManager.Instance.GET_STRING_DATA(EQmatbatIDTagName, out bResult);
            aEQ_MAT_BAT_NAME[0] = DataManager.Instance.GET_STRING_DATA(EQmatbatnameTagName, out bResult);
            aEQ_MAT_ID[0] = DataManager.Instance.GET_STRING_DATA(EQmatIDTagName, out bResult);
            aEQ_MAT_TYPE[0] = DataManager.Instance.GET_STRING_DATA(EQmattypeTagName, out bResult);
            aEQ_MAT_ST[0] = DataManager.Instance.GET_STRING_DATA(EQmatSTTagName, out bResult);
            aEQ_MAT_PORT_ID[0] = DataManager.Instance.GET_STRING_DATA(EQmatportIDTagName, out bResult);
            aEQ_MAT_STATE[0] = DataManager.Instance.GET_STRING_DATA(EQmatstateTagName, out bResult);
            aEQ_MAT_TOTAL_QTY[0] = DataManager.Instance.GET_INT_DATA(EQmattotalQTYTagName, out bResult).ToString();
            aEQ_MAT_USE_QTY[0] = DataManager.Instance.GET_INT_DATA(EQmatuseQTYTagName, out bResult).ToString();
            aEQ_MAT_ASSEM_QTY[0] = DataManager.Instance.GET_INT_DATA(EQmatassemQTYTagName, out bResult).ToString();
            aEQ_MAT_NG_QTY[0] = DataManager.Instance.GET_INT_DATA(EQmatngQTYTagName, out bResult).ToString();
            aEQ_MAT_REMAINQTY_QTY[0] = DataManager.Instance.GET_INT_DATA(EQmatremainQTYTagName, out bResult).ToString();
            aEQ_MAT_PRODUCT_QTY[0] = DataManager.Instance.GET_INT_DATA(EQmatproductQTYTagName, out bResult).ToString();
            aEQ_MAT_PROCE_USE_QTY[0] = DataManager.Instance.GET_INT_DATA(EQmatproceuseQTYTagName, out bResult).ToString();
            aEQ_MAT_PROCE_ASSEM_QTY[0] = DataManager.Instance.GET_INT_DATA(EQmatproceassemQTYTagName, out bResult).ToString();
            aEQ_MAT_PROC_NG_QTY[0] = DataManager.Instance.GET_INT_DATA(EQmatprocngQTYTagName, out bResult).ToString();
            aEQ_MAT_SUPPLY_REQUEST_QTY[0] = DataManager.Instance.GET_INT_DATA(EQmatsupplyrequestQTYTagName, out bResult).ToString();

            #region S6F11(MaterialAssembleProcess), CEID: 215
            
            SecsMessage msg = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(3);                                                                             //L3  EQP Status Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDATAID, gDefine.DEF_DATAID_SIZE));                            //A4  Data ID
                msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                //A3  Collection Event ID
                msg.AddList(4);                                                                             //L4  RPTID Set
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
                    msg.AddList(2);                                                                             //L2  RPTID 300 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("101", gDefine.DEF_RPTID_SIZE));                                       //A3  RPTID="101"
                        msg.AddList(9);                                                                                     //L9  EQP State Set
                        {
                            msg.AddAscii(sAVAILABILITYSTATE);                                                                   //A1  EQ Avilability State Info
                            msg.AddAscii(sINTERLOCKSTATE);                                                                      //A1  Interlock Avilability State Info
                            msg.AddAscii(sMOVESTATE);                                                                           //A1  EQ Move State Info
                            msg.AddAscii(sRUNSTATE);                                                                            //A1  Cell existence/nonexistence Check
                            msg.AddAscii(sFRONTSTATE);                                                                          //A1  Upper EQ Processing State
                            msg.AddAscii(sREARSTATE);                                                                           //A1  Lower EQ Processing State
                            msg.AddAscii(sPP_SPLSTATE);                                                                         //A1  Sample Run-Normal Run State
                            msg.AddAscii(AppUtil.ToAscii(sREASONCODE, gDefine.DEF_REASONCODE_SIZE));                            //A20 Code Info
                            msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION, gDefine.DEF_DESCRIPTION_SIZE));                          //A40 EQ Description
                        }
                    }
                    msg.AddList(2);                                                                             //L2  RPTID 300 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("300", gDefine.DEF_RPTID_SIZE));                               //A3  RPTID="300"
                        msg.AddList(4);                                                                             //L4  Cell Info Set 
                        {
                            msg.AddAscii(AppUtil.ToAscii(sCELLID, gDefine.DEF_CELLID_SIZE));                            //A40 Cell Unique ID
                            msg.AddAscii(AppUtil.ToAscii(sPPID, gDefine.DEF_PPID_SIZE));                                //A40 LOT 지PPID
                            msg.AddAscii(AppUtil.ToAscii(sPRODUCTID, gDefine.DEF_PRODUCTID_SIZE));                      //A40 CELL Product Info
                            msg.AddAscii(AppUtil.ToAscii(sSTEPID, gDefine.DEF_STEPID_SIZE));                            //A40 CELL STEP ID Info
                        }
                    }
                    msg.AddList(2);                                                                             //L2  RPTID 201 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("201", gDefine.DEF_RPTID_SIZE));                               //A3 RPTID="201"
                        msg.AddList(nMAT_NO);                                                                       //La Material No
                        {
                            for (int i = 0; i < nMAT_NO; i++)
                            {
                                msg.AddList(17);                                                                        //L17 Material Info Set
                                {
                                    msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_BAT_ID[i], 40));                                   //A40 Batch MaterialID 
                                    msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_BAT_NAME[i], 40));                                 //A40 Batch Material Code
                                    msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_ID[i], 40));                                       //A40 Material ID
                                    msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_TYPE[i], 20));                                     //A20 Material Type
                                    msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_ST[i], 1));                                                            //A1  Material State Info
                                    msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_PORT_ID[i], 1));                                                       //A1  Material Port ID Info
                                    msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_STATE[i], 1));                                                         //A1  Material Avilability State Info
                                    msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_TOTAL_QTY[i], 10));                                //A10 Material Batch Quantity
                                    msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_USE_QTY[i], 10));                                  //A10 Material Batch Cumulative Usage Quantity
                                    msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_ASSEM_QTY[i], 10));                                //A10 Material Batch Cumulative Attachment Quantity
                                    msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_NG_QTY[i], 10));                                   //A10 Material Batch Cumulative NG Quantity
                                    msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_REMAINQTY_QTY[i], 10));                              //A10 Remaining Material Quantity 
                                    msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_PRODUCT_QTY[i], 10));                              //A10 Material Process Standard Quantity
                                    msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_PROCE_USE_QTY[i], 10));                            //A10 Material Process Usage Quantity
                                    msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_PROCE_ASSEM_QTY[i], 10));                          //A10 Material Process Attachment Quantity
                                    msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_PROC_NG_QTY[i], 10));                              //A10 Material Process NG Quantity
                                    msg.AddAscii(AppUtil.ToAscii(aEQ_MAT_SUPPLY_REQUEST_QTY[i], 10));                         //A10 Material Supply Quest Quantity
                                }
                            }
                        }
                    }
                }
            }
            this.SecsDriver.WriteLogAndSendMessage(msg, sCEID);

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "ASSEM", "S6F11", "215", string.Format("Material Assemble Process {0}_{1}", _cellPortNo, _matNo), null);
            LogHelper.Instance.MaterialLog.DebugFormat("Assemble, {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}",
                sCELLID, aEQ_MAT_BAT_ID[0], aEQ_MAT_TYPE[0], aEQ_MAT_TOTAL_QTY[0], aEQ_MAT_USE_QTY[0], aEQ_MAT_ASSEM_QTY[0], aEQ_MAT_NG_QTY[0], aEQ_MAT_REMAINQTY_QTY[0], aEQ_MAT_PRODUCT_QTY[0], aEQ_MAT_PROCE_USE_QTY[0], aEQ_MAT_PROCE_ASSEM_QTY[0], aEQ_MAT_PROC_NG_QTY[0]);
            #endregion
        }
    }
}
