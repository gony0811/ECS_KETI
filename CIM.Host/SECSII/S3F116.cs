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
    /*    1. Structure Name : Carrier Information Data Send
    1) 해당 없음
    2) Message 구조
    <L, 2
        1.<A[40] $EQPID> * 설비 고유 ID
        2.<L, 10 * Carrier Info Set
            1.<A[40] $CARRIERID> * Carrier 별로 부여 된 Unique ID
            2.<A[4] $CARRIERTYPE> * Carrier 종류
            3.<A[40] $CARRIERPPID> * Carrier Process Parameter Group ID (Recipe ID)
            4.<A[40] $CARRIERPRODUCT> * Carrier 제품 정보
            5.<A[40] $CARRIERSTEPID> * Carrier STEP ID 정보
            6.<A[4] $CARRIER_S_COUNT> * Carrier 내 Sub Carrier 총 수량
            7.<A[4] $CARRIER_C_COUNT> * Carrier 내 Cell/Panel 총 수량
            8.<A[4] $PORTNO> * Carrier 투입 된 설비 Port No
            9.<L, a * a = Sub Carrier Set Count (Max = 40)
                1.<L, 3 * Sub Carrier Info Set
                    1.<A[40] $SUBCARRIERID> * Sub Carrier 별로 부여 된 Unique ID
                    2.<A[4] $CELLQTY> * Sub Carrier 내 CELL 수량
                    3.<L, b * b = Cell/Panel 수량 (Max = 20)
                        1.<L, 4 * Cell/Panel Info Set
                            1.<A[40] $CELLID> * Cell 별로 부여 된 Unique ID ※ APN 사용 설비는 A[200] 사용 할 수 있다.
                            2.<A[4] $LOCATIONNO> * Carrier 내 위치 정보
                            3.<A[1] $JUDGE> * 제품 판정 값
                            4.<A[20] $REASONCODE> * 사유 코드 (사용 전 정의 필요)
           10.<L, 2 * Result Info
                1.<A[10] $REPLYCODE> * 정합성에 대한 코드 값
                2.<A[120] $REPLYTEXT> * 결과에 대한 내용
    3) 설명
    3-1) 발생 조건 (시점)
    HOST는 설비에서 보고 된 Carrier 데이터의 정합성을 검증 한 뒤 데이터의 처리 결과와 정보에 대해서 설비로 전송한다.
     */

    public class S3F116 : SFMessage
    {
        uint _systemByte;

        string _EQPID;
        string _CARRIERID;
        string _CARRIERTYPE;
        string _CARRIERPPID;
        string _CARRIERPRODUCT;
        string _CARRIERSTEPID;
        string _CARRIER_S_COUNT;
        string _CARRIER_C_COUNT;
        string _PORTNO;
        string _SUBCARRIERID;
        string _CELLQTY;
        string[] _CELLID;
        string[] _LOCATIONNO;
        string[] _JUDGE;
        string[] _REASONCODE;
        
        string _REPLYCODE;
        string _REPLYTEXT;

        int _listCountM = 0;

        string _ACK3 = string.Empty;

        public S3F116(SECSDriverBase driver)
            :base(driver)
        {
            Stream = 3; Function = 116;
        }



        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            SecsMessage primaryMessage = obj as SecsMessage;

            _systemByte = primaryMessage.SystemByte;

            int list0 = primaryMessage.GetItem().GetList();
            {
                _EQPID = primaryMessage.GetItem().GetAscii();
                
                int list1 = primaryMessage.GetItem().GetList();
                {
                    _CARRIERID = primaryMessage.GetItem().GetAscii();
                    _CARRIERTYPE = primaryMessage.GetItem().GetAscii();
                    _CARRIERPPID = primaryMessage.GetItem().GetAscii();
                    _CARRIERPRODUCT = primaryMessage.GetItem().GetAscii();
                    _CARRIERSTEPID = primaryMessage.GetItem().GetAscii();
                    _CARRIER_S_COUNT = primaryMessage.GetItem().GetAscii(); if (string.IsNullOrEmpty(_CARRIER_S_COUNT)) _CARRIER_S_COUNT = "0";
                    _CARRIER_C_COUNT = primaryMessage.GetItem().GetAscii(); if (string.IsNullOrEmpty(_CARRIER_C_COUNT)) _CARRIER_C_COUNT = "0";
                    _PORTNO = primaryMessage.GetItem().GetAscii();

                    int _listCountN = primaryMessage.GetItem().GetList();                   
                    {
                        if (_listCountN != 0)
                        {
                            for (int i = 0; i < _listCountN; i++)
                            {
                                int list3 = primaryMessage.GetItem().GetList();
                                {
                                    _SUBCARRIERID = primaryMessage.GetItem().GetAscii();
                                    _CELLQTY = primaryMessage.GetItem().GetAscii(); if (string.IsNullOrEmpty(_CELLQTY)) _CELLQTY = "0";

                                    int _listCountM = primaryMessage.GetItem().GetList();
                                    {
                                        _CELLID = new string[_listCountM];
                                        _LOCATIONNO = new string[_listCountM];
                                        _JUDGE = new string[_listCountM];
                                        _REASONCODE = new string[_listCountM];

                                        for (int j = 0; j < _listCountM; j++)
                                        {
                                            int list5 = primaryMessage.GetItem().GetList();
                                            {
                                                _CELLID[j] = primaryMessage.GetItem().GetAscii();
                                                _LOCATIONNO[j] = primaryMessage.GetItem().GetAscii();
                                                _JUDGE[j] = primaryMessage.GetItem().GetAscii();
                                                _REASONCODE[j] = primaryMessage.GetItem().GetAscii();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    int list6 = primaryMessage.GetItem().GetList();
                    {
                        _REPLYCODE = primaryMessage.GetItem().GetAscii();
                        _REPLYTEXT = primaryMessage.GetItem().GetAscii();
                    }
                }
            }

            if(_EQPID.Trim() != CommonData.Instance.EQP_SETTINGS.EQPID)
            {
                _ACK3 = "5";
                CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "CARRIER", "S3F115", null, "Not Match EQPID", _REPLYCODE);
            }
            else
            {
                _ACK3 = "0";
                CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "CARRIER", "S3F115", null, "Carrier Information Data", _REPLYCODE);
            }

            var info = new CIM.Common.VALIDATION_INFO();

            if(_CARRIERTYPE == "1" || _CARRIERTYPE == "3")
                info = CommonData.Instance.VALIDATION_DATA.FindValidationData(KindOfValidation.SUBCARRIER_INFO_SEND, _SUBCARRIERID + "." + _CARRIERTYPE, "");
            else
                info = CommonData.Instance.VALIDATION_DATA.FindValidationData(KindOfValidation.CARRIER_INFO_SEND, _CARRIERID + "." + _CARRIERTYPE, "");


            if (info.KEY_ITEM != null)
            {
                var module_info = CommonData.Instance.MODULE_SETTINGS.Where(m => m.MODULE_NAME == info.MODULE_NAME).FirstOrDefault();

                string tagGroupName = "CarrierInformationSend";

                // Standard Loader -> Carrier Information Request(CEID:262) / 빈 Tray군 Carrier Assign Request(CEID:260) 
                if (module_info.TYPE == eModuleType.LOADER && (_CARRIERTYPE == ((int)CarrierType.UseBatchID).ToString() ||
                                                                _CARRIERTYPE == ((int)CarrierType.NotUseBatchID).ToString() ||
                                                                _CARRIERTYPE == ((int)CarrierType.ForcedUseBatchID).ToString() ||
                                                                _CARRIERTYPE == ((int)CarrierType.ForcedNotUseBatchID).ToString()))
                {
                    tagGroupName += "Loader";

                    string tagCARRIERID = string.Format("o{0}.{1}.CarrierID", module_info.MODULE_NAME, tagGroupName);
                    string tagCARRIERTYPE = string.Format("o{0}.{1}.CarrierType", module_info.MODULE_NAME, tagGroupName);
                    string tagCARRIERPRODUCT = string.Format("o{0}.{1}.CarrierProduct", module_info.MODULE_NAME, tagGroupName);
                    string tagCARRIERSTEPID = string.Format("o{0}.{1}.CarrierStepID", module_info.MODULE_NAME, tagGroupName);
                    string tagCARRIERSCOUNT = string.Format("o{0}.{1}.CarrierSCount", module_info.MODULE_NAME, tagGroupName);
                    string tagREPLYCODE = string.Format("o{0}.{1}.ReplyCode", module_info.MODULE_NAME, tagGroupName);
                    string tagREPLYTEXT = string.Format("o{0}.{1}.ReplyText", module_info.MODULE_NAME, tagGroupName);

                    string tagSENDBIT = string.Format("o{0}.Send.CarrierInformationSend{1}", info.MODULE_NAME, "Loader");

                    DataManager.Instance.SET_STRING_DATA(tagCARRIERID, _CARRIERID);
                    DataManager.Instance.SET_STRING_DATA(tagCARRIERTYPE, _CARRIERTYPE);
                    DataManager.Instance.SET_STRING_DATA(tagCARRIERPRODUCT, _CARRIERPRODUCT);
                    DataManager.Instance.SET_STRING_DATA(tagCARRIERSCOUNT, _CARRIER_S_COUNT);
                    DataManager.Instance.SET_STRING_DATA(tagCARRIERSTEPID, _CARRIERSTEPID);

                    DataManager.Instance.SET_STRING_DATA(tagREPLYCODE, _REPLYCODE);
                    DataManager.Instance.SET_STRING_DATA(tagREPLYTEXT, _REPLYTEXT);

                    DataManager.Instance.SET_INT_DATA(tagSENDBIT, 1);

                }
                // Standard Loader -> Carrier Release Request(CEID:256) 보고 대응
                else if (module_info.TYPE == eModuleType.LOADER && (_CARRIERTYPE == ((int)CarrierType.UseTrayID).ToString() ||
                                                                    _CARRIERTYPE == ((int)CarrierType.NotUseTrayID).ToString() ||
                                                                    _CARRIERTYPE == ((int)CarrierType.ForcedUseTrayID).ToString() ||
                                                                    _CARRIERTYPE == ((int)CarrierType.ForcedNotUseTrayID).ToString()))
                {
                    tagGroupName += "Loader";
                    string tagCarrierProcessChangeLoaderCEID = string.Format("i{0}.CarrierProcessChangeLoader.CEID", module_info.MODULE_NAME);

                    bool bDataReadResult;
                    bool bDataWriteResult = true;
                    if (_listCountM != 0 && DataManager.Instance.GET_STRING_DATA(tagCarrierProcessChangeLoaderCEID, out bDataReadResult) == "256")
                    {
                        string tagCELLID = string.Format("o{0}.{1}.CellID", module_info.MODULE_NAME, tagGroupName);
                        string tagJUDGE = string.Format("o{0}.{1}.Judge", module_info.MODULE_NAME, tagGroupName);
                        string tagLOCATION = string.Format("o{0}.{1}.LocationNo", module_info.MODULE_NAME, tagGroupName);
                        string tagREASONCODE = string.Format("o{0}.{1}.ReasonCode", module_info.MODULE_NAME, tagGroupName);

                        for (int i = 0; i < _listCountM; i++)
                        {
                            bDataWriteResult &= DataManager.Instance.SET_STRING_DATA(tagCELLID + ((i + 1).ToString()), _CELLID[i]);
                            bDataWriteResult &= DataManager.Instance.SET_STRING_DATA(tagJUDGE + ((i + 1).ToString()), _JUDGE[i]);
                            bDataWriteResult &= DataManager.Instance.SET_STRING_DATA(tagLOCATION + ((i + 1).ToString()), _LOCATIONNO[i]);
                            bDataWriteResult &= DataManager.Instance.SET_STRING_DATA(tagREASONCODE + ((i + 1).ToString()), _REASONCODE[i]);
                        }
                    }

                    string tagCarrierId = string.Format("o{0}.{1}.CarrierID", module_info.MODULE_NAME, tagGroupName);
                    string tagCarrierType = string.Format("o{0}.{1}.CarrierType", module_info.MODULE_NAME, tagGroupName);
                    string tagCarrierProduct = string.Format("o{0}.{1}.CarrierProduct", module_info.MODULE_NAME, tagGroupName);
                    string tagCarrierSCount = string.Format("o{0}.{1}.CarrierSCount", module_info.MODULE_NAME, tagGroupName);
                    string tagCarrierStepID = string.Format("o{0}.{1}.CarrierStepID", module_info.MODULE_NAME, tagGroupName);
                    string tagSubCarrierID = string.Format("o{0}.{1}.SubCarrierID", module_info.MODULE_NAME, tagGroupName);
                    string tagCellQty = string.Format("o{0}.{1}.CellQTY", module_info.MODULE_NAME, tagGroupName);
                    string tagReplyCode = string.Format("o{0}.{1}.ReplyCode", module_info.MODULE_NAME, tagGroupName);
                    string tagReplyText = string.Format("o{0}.{1}.ReplyText", module_info.MODULE_NAME, tagGroupName);

                    string tagCarrierInformationSendBit = string.Format("o{0}.Send.CarrierInformationSendLoader", module_info.MODULE_NAME);

                    DataManager.Instance.SET_STRING_DATA(tagCarrierId, _CARRIERID);
                    DataManager.Instance.SET_STRING_DATA(tagCarrierType, _CARRIERTYPE);
                    DataManager.Instance.SET_STRING_DATA(tagCarrierProduct, _CARRIERPRODUCT);
                    DataManager.Instance.SET_STRING_DATA(tagCarrierSCount, _CARRIER_S_COUNT);
                    DataManager.Instance.SET_STRING_DATA(tagCarrierStepID, _CARRIERSTEPID);
                    DataManager.Instance.SET_STRING_DATA(tagSubCarrierID, _SUBCARRIERID);

                    int nCellQty = 0;
                    if (int.TryParse(_CELLQTY, out nCellQty))
                        DataManager.Instance.SET_INT_DATA(tagCellQty, nCellQty);

                    DataManager.Instance.SET_STRING_DATA(tagReplyCode, _REPLYCODE);
                    DataManager.Instance.SET_STRING_DATA(tagReplyText, _REPLYTEXT);

                    DataManager.Instance.SET_INT_DATA(tagCarrierInformationSendBit, 1);
                }
                // Standard Unloader -> Carrier Information Request(CEID:262) / 꽉 찬 Tray군 Carrier Assign Request(CEID:260)
                else if (module_info.TYPE == eModuleType.UNLOADER && (_CARRIERTYPE == ((int)CarrierType.UseBatchID).ToString() ||
                                                                      _CARRIERTYPE == ((int)CarrierType.NotUseBatchID).ToString() ||
                                                                      _CARRIERTYPE == ((int)CarrierType.ForcedUseBatchID).ToString() ||
                                                                      _CARRIERTYPE == ((int)CarrierType.ForcedNotUseBatchID).ToString()))
                {
                    tagGroupName += "Unloader";
                    string tagCarrierID = string.Format("o{0}.{1}.CarrierID", module_info.MODULE_NAME, tagGroupName);
                    string tagReplyCode = string.Format("o{0}.{1}.ReplyCode", module_info.MODULE_NAME, tagGroupName);
                    string tagReplyText = string.Format("o{0}.{1}.ReplyText", module_info.MODULE_NAME, tagGroupName);

                    string tagCarrierInformationSendBit = string.Format("o{0}.Send.CarrierInformationSendUnloader", module_info.MODULE_NAME);

                    DataManager.Instance.SET_STRING_DATA(tagReplyCode, _REPLYCODE);
                    DataManager.Instance.SET_STRING_DATA(tagReplyText, _REPLYTEXT);

                    DataManager.Instance.SET_INT_DATA(tagCarrierInformationSendBit, 1);
                }
                // Standard Unloader -> Carrier Assign Request(CEID:260) 보고 대응
                else if (module_info.TYPE == eModuleType.UNLOADER && (_CARRIERTYPE == ((int)CarrierType.UseTrayID).ToString() ||
                                                                      _CARRIERTYPE == ((int)CarrierType.NotUseTrayID).ToString() ||
                                                                      _CARRIERTYPE == ((int)CarrierType.ForcedUseTrayID).ToString() ||
                                                                      _CARRIERTYPE == ((int)CarrierType.ForcedNotUseTrayID).ToString()))
                {
                    tagGroupName += "Unloader";
                    string tagSubCarrierID = string.Format("o{0}.{1}.SubCarrierID", module_info.MODULE_NAME, tagGroupName);
                    string tagReplyCode = string.Format("o{0}.{1}.ReplyCode", module_info.MODULE_NAME, tagGroupName);
                    string tagReplyText = string.Format("o{0}.{1}.ReplyText", module_info.MODULE_NAME, tagGroupName);

                    string tagCarrierInformationSendBit = string.Format("o{0}.Send.CarrierInformationSendUnloader", module_info.MODULE_NAME);

                    DataManager.Instance.SET_STRING_DATA(tagSubCarrierID, _SUBCARRIERID);
                    DataManager.Instance.SET_STRING_DATA(tagReplyCode, _REPLYCODE);
                    DataManager.Instance.SET_STRING_DATA(tagReplyText, _REPLYTEXT);

                    DataManager.Instance.SET_INT_DATA(tagCarrierInformationSendBit, 1);

                }
                // Standard Loader -> Cassette Information Request(CEID:354) 보고 대응
                else if (module_info.TYPE == eModuleType.LOADER && CommonData.Instance.LOADER_CASSETTEID == _CARRIERID)
                {
                    string tagCarrierID = string.Format("o{0}.CarrierInformationSendCassette.CarrierID", module_info.MODULE_NAME);
                    string tagReplyCode = string.Format("o{0}.CarrierInformationSendCassette.ReplyCode", module_info.MODULE_NAME);
                    string tagReplyText = string.Format("o{0}.CarrierInformationSendCassette.ReplyText", module_info.MODULE_NAME);

                    string tagCarrierInformationSendBit = string.Format("o{0}.Send.CarrierInformationSendCassette", module_info.MODULE_NAME);

                    DataManager.Instance.SET_STRING_DATA(tagCarrierID, _CARRIERID);
                    DataManager.Instance.SET_STRING_DATA(tagReplyCode, _REPLYCODE);
                    DataManager.Instance.SET_STRING_DATA(tagReplyText, _REPLYTEXT);

                    DataManager.Instance.SET_INT_DATA(tagCarrierInformationSendBit, 1);
                }

                else
                {
                    _ACK3 = "9";
                    CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "CARRIER", "S3F115", null, "Carrier Information Data", "NACK");
                }
            }

            else
            {
                _ACK3 = "9";
                CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "CARRIER", "S3F115", null, "Carrier Information Data", "NACK");
            }

            SecsMessage reply = new SecsMessage(Stream, Function, _systemByte);
            reply.AddAscii(_ACK3);

            SecsDriver.WriteLogAndSendMessage(reply, _ACK3);

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "CARRIER", "S3F116", null, "Carrier Information Data Reply", null);
        }


    }
}
