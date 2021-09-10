using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core;
using INNO6.IO;
using CIM.Manager;
using CIM.Common;
using SYSWIN.Secl;

namespace CIM.Host.SECSII
{
    // S3F105 메시지 아이템으로 UNIT ID 항목 추가 필요
    // 신규메시지 배포 예정
    // **** S3F106 미완성 / 미사용
    public class S3F106 : SFMessage
    {
        uint _systemByte;

        string _EQPID;
        string _MATERIAL_EQPID;
        string _MATERIAL_BATCHID;
        string _MATERIALCODE;
        string _MATERIALUSEDATE;
        string _MATERIALDISEASEDATE;
        string _MATERIALMAKER;
        string _MATERIALVALIDATIONFLAGE;
        string _MATERIALDEFECTCODE;
        string _COMMENT;

        string _MATERIALID;
        string _MATERIALTYPE;
        string _MATERIALST;
        string _MATERIALPORTID;
        string _MATERIALSTATE;
        string _MATERIALTOTALQTY;
        string _MATERIALUSEQTY;
        string _MATERIALASSEMQTY;
        string _MATERIALNGQTY;
        string _MATERIALREMAINQTY;
        string _MATERIALPROCUSEQTY;

        string _REPLYSTATUS;
        string _REPLYCODE;
        string _REPLYTEXT;

        Common.TC.ACKC3 ACKC3;
        //Common.TC.ACKC7 ACKC7;

        public S3F106(SECSDriverBase driver)
        :base(driver)
        {
            Stream = 3; Function = 106;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            SecsMessage primaryMessage = obj as SecsMessage;

            bool bDataSetResult = true;
            _systemByte = primaryMessage.SystemByte;
            int list1 = primaryMessage.GetItem().GetList();
            {
                _EQPID = primaryMessage.GetItem().GetAscii();
                int list2 = primaryMessage.GetItem().GetList();                             

                if(list2 != 0)
                {
                    int list3 = primaryMessage.GetItem().GetList();
                    {
                        int list4 = primaryMessage.GetItem().GetList();
                        {

                            _MATERIAL_EQPID = primaryMessage.GetItem().GetAscii();
                            _MATERIAL_BATCHID = primaryMessage.GetItem().GetAscii();
                            _MATERIALCODE = primaryMessage.GetItem().GetAscii();
                            _MATERIALUSEDATE = primaryMessage.GetItem().GetAscii();
                            _MATERIALDISEASEDATE = primaryMessage.GetItem().GetAscii();
                            _MATERIALMAKER = primaryMessage.GetItem().GetAscii();
                            _MATERIALVALIDATIONFLAGE = primaryMessage.GetItem().GetAscii();
                            _MATERIALDEFECTCODE = primaryMessage.GetItem().GetAscii();
                            _COMMENT = primaryMessage.GetItem().GetAscii();
                        }

                        int list5 = primaryMessage.GetItem().GetList();
                        {
                            _MATERIALID = primaryMessage.GetItem().GetAscii();
                            _MATERIALTYPE = primaryMessage.GetItem().GetAscii();
                            _MATERIALST = primaryMessage.GetItem().GetAscii();
                            _MATERIALPORTID = primaryMessage.GetItem().GetAscii();
                            _MATERIALSTATE = primaryMessage.GetItem().GetAscii();
                            _MATERIALTOTALQTY = primaryMessage.GetItem().GetAscii();
                            _MATERIALUSEQTY = primaryMessage.GetItem().GetAscii();
                            _MATERIALASSEMQTY = primaryMessage.GetItem().GetAscii();
                            _MATERIALNGQTY = primaryMessage.GetItem().GetAscii();
                            _MATERIALREMAINQTY = primaryMessage.GetItem().GetAscii();
                            _MATERIALPROCUSEQTY = primaryMessage.GetItem().GetAscii();
                        }

                        int list6 = primaryMessage.GetItem().GetList();
                        {
                            _REPLYSTATUS = primaryMessage.GetItem().GetAscii();
                            _REPLYCODE = primaryMessage.GetItem().GetAscii();
                            _REPLYTEXT = primaryMessage.GetItem().GetAscii();
                        }
                    }

                    if(_MATERIALTOTALQTY == string.Empty) _MATERIALTOTALQTY ="0";
                    if(_MATERIALUSEQTY == string.Empty) _MATERIALUSEQTY = "0";
                    if(_MATERIALASSEMQTY == string.Empty) _MATERIALASSEMQTY ="0";
                    if(_MATERIALNGQTY == string.Empty) _MATERIALNGQTY ="0";
                    if(_MATERIALREMAINQTY == string.Empty) _MATERIALREMAINQTY ="0";
                    if (_MATERIALPROCUSEQTY == string.Empty) _MATERIALPROCUSEQTY = "0";
                        
                    VALIDATION_INFO v_info = CommonData.Instance.VALIDATION_DATA.FindValidationData(KindOfValidation.MATERIAL_INFO_SEND, _MATERIAL_BATCHID, _MATERIALID);

                    if(v_info != null)
                    {
                        string tagMaterialEqpId = string.Format("o{0}.MaterialInformation.MaterialEqpID{1}", v_info.MODULE_NAME, v_info.PORTNO);
                        string tagMaterialBatchId = string.Format("o{0}.MaterialInformation.MaterialBatchID{1}", v_info.MODULE_NAME, v_info.PORTNO);
                        string tagMaterialCode = string.Format("o{0}.MaterialInformation.MaterialCode{1}", v_info.MODULE_NAME, v_info.PORTNO);
                        string tagMaterialUseDate = string.Format("o{0}.MaterialInformation.MaterialUseDate{1}", v_info.MODULE_NAME, v_info.PORTNO);
                        string tagMaterialMaker = string.Format("o{0}.MaterialInformation.MaterialMaker{1}", v_info.MODULE_NAME, v_info.PORTNO);
                        string tagMaterialValidationFlag = string.Format("o{0}.MaterialInformation.MaterialValidationFlage{1}", v_info.MODULE_NAME, v_info.PORTNO);
                        string tagMaterialDefectCode = string.Format("o{0}.MaterialInformation.MaterialDefectCode{1}", v_info.MODULE_NAME, v_info.PORTNO);
                        string tagMaterialDiseaseDate = string.Format("o{0}.MaterialInformation.MaterialDiseaseDate{1}", v_info.MODULE_NAME, v_info.PORTNO);

                        string tagMaterialID = string.Format("o{0}.MaterialInformation.MaterialID{1}", v_info.MODULE_NAME, v_info.PORTNO);
                        string tagMaterialType = string.Format("o{0}.MaterialInformation.MaterialType{1}", v_info.MODULE_NAME, v_info.PORTNO);
                        string tagMaterialST = string.Format("o{0}.MaterialInformation.MaterialST{1}", v_info.MODULE_NAME, v_info.PORTNO);
                        string tagMaterialPortID = string.Format("o{0}.MaterialInformation.MaterialPortID{1}", v_info.MODULE_NAME, v_info.PORTNO);
                        string tagMaterialState = string.Format("o{0}.MaterialInformation.MaterialState{1}", v_info.MODULE_NAME, v_info.PORTNO);
                        string tagMaterialTotalQTY = string.Format("o{0}.MaterialInformation.MaterialTotalQTY{1}", v_info.MODULE_NAME, v_info.PORTNO);
                        string tagMaterialUseQTY = string.Format("o{0}.MaterialInformation.MaterialUseQTY{1}", v_info.MODULE_NAME, v_info.PORTNO);

                        string tagMaterialAssemQTY = string.Format("o{0}.MaterialInformation.MaterialAssemQTY{1}", v_info.MODULE_NAME, v_info.PORTNO);
                        string tagMaterialNGQTY = string.Format("o{0}.MaterialInformation.MaterialNGQTY{1}", v_info.MODULE_NAME, v_info.PORTNO);
                        string tagMaterialRemainQTY = string.Format("o{0}.MaterialInformation.MaterialRemainQTY{1}", v_info.MODULE_NAME, v_info.PORTNO);
                        string tagMaterialProceUseQTY = string.Format("o{0}.MaterialInformation.MaterialProceUseQTY{1}", v_info.MODULE_NAME, v_info.PORTNO);

                        string tagReplyCode = string.Format("o{0}.MaterialInformation.ReplyCode{1}", v_info.MODULE_NAME, v_info.PORTNO);
                        string tagReplyStatus = string.Format("o{0}.MaterialInformation.ReplyStatus{1}", v_info.MODULE_NAME, v_info.PORTNO);
                        string tagReplyText = string.Format("o{0}.MaterialInformation.ReplyText{1}", v_info.MODULE_NAME, v_info.PORTNO);

                        bDataSetResult &= DataManager.Instance.SET_STRING_DATA(tagMaterialEqpId, _MATERIAL_EQPID);
                        bDataSetResult &= DataManager.Instance.SET_STRING_DATA(tagMaterialBatchId, _MATERIAL_BATCHID);
                        bDataSetResult &= DataManager.Instance.SET_STRING_DATA(tagMaterialCode, _MATERIALCODE);
                        bDataSetResult &= DataManager.Instance.SET_STRING_DATA(tagMaterialUseDate, _MATERIALUSEDATE);

                        bDataSetResult &= DataManager.Instance.SET_STRING_DATA(tagMaterialMaker, _MATERIALMAKER);
                        bDataSetResult &= DataManager.Instance.SET_STRING_DATA(tagMaterialValidationFlag, _MATERIALVALIDATIONFLAGE);

                        bDataSetResult &= DataManager.Instance.SET_STRING_DATA(tagMaterialDefectCode, _MATERIALDEFECTCODE);
                        bDataSetResult &= DataManager.Instance.SET_STRING_DATA(tagMaterialDiseaseDate, _MATERIALDISEASEDATE);

                        bDataSetResult &= DataManager.Instance.SET_STRING_DATA(tagMaterialID, _MATERIALID);
                        bDataSetResult &= DataManager.Instance.SET_STRING_DATA(tagMaterialType, _MATERIALTYPE);
                        bDataSetResult &= DataManager.Instance.SET_STRING_DATA(tagMaterialST, _MATERIALST);
                        bDataSetResult &= DataManager.Instance.SET_STRING_DATA(tagMaterialPortID, _MATERIALPORTID);

                        bDataSetResult &= DataManager.Instance.SET_STRING_DATA(tagMaterialState, _MATERIALSTATE);
                        bDataSetResult &= DataManager.Instance.SET_INT_DATA(tagMaterialTotalQTY, Convert.ToInt32(_MATERIALTOTALQTY));
                        bDataSetResult &= DataManager.Instance.SET_INT_DATA(tagMaterialUseQTY, Convert.ToInt32(_MATERIALUSEQTY));

                        bDataSetResult &= DataManager.Instance.SET_INT_DATA(tagMaterialAssemQTY, Convert.ToInt32(_MATERIALASSEMQTY));
                        bDataSetResult &= DataManager.Instance.SET_INT_DATA(tagMaterialNGQTY, Convert.ToInt32(_MATERIALNGQTY));
                        bDataSetResult &= DataManager.Instance.SET_INT_DATA(tagMaterialRemainQTY, Convert.ToInt32(_MATERIALREMAINQTY));

                        bDataSetResult &= DataManager.Instance.SET_INT_DATA(tagMaterialProceUseQTY, Convert.ToInt32(_MATERIALPROCUSEQTY));
                        bDataSetResult &= DataManager.Instance.SET_STRING_DATA(tagReplyCode, _REPLYCODE);
                        bDataSetResult &= DataManager.Instance.SET_STRING_DATA(tagReplyStatus, _REPLYSTATUS);
                        bDataSetResult &= DataManager.Instance.SET_STRING_DATA(tagReplyText, _REPLYTEXT);

                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "MATERIAL", "S3F105", null, string.Format("Material Information Data #{0}", v_info.PORTNO), _REPLYSTATUS);

                        if (bDataSetResult)
                        {
                            string tagMaterialInfoSendBit = string.Format("o{0}.Send.MaterialInformationSend{1}", v_info.MODULE_NAME, v_info.PORTNO);
                            string tagMaterialInfoReplyBit = string.Format("i{0}.Reply.MaterialInformationSend{1}", v_info.MODULE_NAME, v_info.PORTNO);
                            bool bitSetResult = DataManager.Instance.SET_INT_DATA(tagMaterialInfoSendBit, 1);

                            if(bitSetResult && DataManager.Instance.IsDeviceMode("DEV1") != INNO6.IO.Interface.eDevMode.CONNECT)
                            {
                                DataManager.Instance.SET_INT_DATA(tagMaterialInfoReplyBit, 1);
                            }

                            ACKC3 = Common.TC.ACKC3.ACCEPTED;
                        }
                        else
                        {
                            ACKC3 = Common.TC.ACKC3.ERROR;
                            //ACKC3 = Common.TC.ACKC7.PPID_TYPEisnotmatch;
                            CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "MATERIAL", "S3F105", null, "Material Information Error", null);
                        }

                    }
                    else
                    {
                        ACKC3 = Common.TC.ACKC3.ERROR;
                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "MATERIAL", "S3F105", null, "Material Information Error", null);
                    }

                }
            }

            SecsMessage reply = new SecsMessage(Stream, Function, _systemByte);

            reply.AddAscii(Convert.ToInt32(ACKC3).ToString());

            SecsDriver.WriteLogAndSendMessage(reply, ACKC3);

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "MATERIAL", "S3F106", null, "Material Information Data", null);
        }
    }
}
