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
    /*
         * 1. Structure Name : Cell Lot Information Send
        1) 해당 없음
        2) Message 구조
        <L, 2
            1.<A[40] $EQPID> * 설비 고유 ID
            2.<L, n * Cell Lot List Count
                1.<L, 13 * Cell Lot Set
                    1.<A[40] $CELLID> * Cell 별로 부여 된 Unique ID ※ APN 사용 설비는 A[200] 사용 할 수 있다.
                    2.<A[40] $CASSETTEID> * Cassette ID
                    3.<A[20] $BATCHLOT> * Batch Lot ID
                    4.<A[40] $PRODUCTID> * 제품 이름
                    5.<A[4] $PRODUCT_TYPE> * EE/PP/RR/NR/DM
                    6.<A[4] $PRODUCT_KIND> * Glass Kind defined by INNO6
                    7.<A[40] $PRODUCTSPEC> * 제품 사양
                    8.<A[40] $STEPID> * STEP ID 정보
                    9.<A[20] $PPID> * Process Parameter Group ID (Recipe ID)
                    10.<A[16] $CELL_SIZE> * Cell Size(mm)
                    11.<A[16] $CELL_THICKNESS> * Cell Thickness(㎛)
                    12.<A[40] $COMMENT> * Comment
                    13.<, a * a = Cell 관련 정보(Specific)
                1.<L, 2 * Item Set
                    1.<A[40] $ITEM_NAME> * CELL 관련 특정 정보의 이름
                    2.<A[80] $ITEM_VALUE> * CELL 관련 특정 정보의 데이터
        3) 설명
        3-1) 발생 조건 (시점)
        HOST는 설비에 장착된 Cassette/Cell에 대한 정합성 검증이 완료 된 시점에 설비로 Cassette/Cell 정보를 전송한다.
     */
    public class S3F110 : SFMessage
    {
        uint _systembyte;

        string _eqpid;

        string _ACK3;
        string _CELLID = string.Empty;
        string _CASSETTEID = string.Empty;
        string _BATCHLOT = string.Empty;
        string _PRODUCTID = string.Empty;
        string _PRODUCT_TYPE = string.Empty;
        string _PRODUCT_KIND = string.Empty;
        string _PRODUCTSPEC = string.Empty;
        string _STEPID = string.Empty;
        string _CELL_SIZE = string.Empty;
        string _CELL_THICKNESS = string.Empty;
        string _COMMENT = string.Empty;

        public S3F110(SECSDriverBase driver)
            :base(driver)
        {
            Stream = 3; Function = 110;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            SecsMessage primaryMessage = obj as SecsMessage;
            bool dataSetResult = true;
            _systembyte = primaryMessage.SystemByte;
            int list1 = primaryMessage.GetItem().GetList();
            {
                _eqpid = primaryMessage.GetItem().GetAscii().Trim();
                int list2 = primaryMessage.GetItem().GetList();
                {
                    int list3 = primaryMessage.GetItem().GetList();
                    {
                        _CELLID = primaryMessage.GetItem().GetAscii().Trim();
                        _CASSETTEID = primaryMessage.GetItem().GetAscii();
                        _BATCHLOT = primaryMessage.GetItem().GetAscii();
                        _PRODUCTID = primaryMessage.GetItem().GetAscii();
                        _PRODUCT_TYPE = primaryMessage.GetItem().GetAscii();
                        _PRODUCT_KIND = primaryMessage.GetItem().GetAscii();
                        _PRODUCTSPEC = primaryMessage.GetItem().GetAscii();
                        _STEPID = primaryMessage.GetItem().GetAscii();
                        _CELL_SIZE = primaryMessage.GetItem().GetAscii();
                        _CELL_THICKNESS = primaryMessage.GetItem().GetAscii();
                        _COMMENT = primaryMessage.GetItem().GetAscii();
                    }
                }
            }

            VALIDATION_INFO info = CommonData.Instance.VALIDATION_DATA.FindValidationData(KindOfValidation.CELL_LOT_INFO_SEND, _CELLID, "");

            if (info != null)
            {

                string tagCellLotInfoCellID = string.Format("o{0}.CellLotInformationSend{1}.CellID", info.MODULE_NAME, info.PORTNO);
                string tagCellLotInfoCassetteID = string.Format("o{0}.CellLotInformationSend{1}.CassetteID", info.MODULE_NAME, info.PORTNO);
                string tagCellLotInfoBatchLot = string.Format("o{0}.CellLotInformationSend{1}.BatchLot", info.MODULE_NAME, info.PORTNO);
                string tagCellLotInfoProductID = string.Format("o{0}.CellLotInformationSend{1}.ProductID", info.MODULE_NAME, info.PORTNO);
                string tagCellLotInfoProductKind = string.Format("o{0}.CellLotInformationSend{1}.ProductKind", info.MODULE_NAME, info.PORTNO);
                string tagCellLotInfoProductSpec = string.Format("o{0}.CellLotInformationSend{1}.ProductSpec", info.MODULE_NAME, info.PORTNO);
                string tagCellLotInfoProductType = string.Format("o{0}.CellLotInformationSend{1}.ProductType", info.MODULE_NAME, info.PORTNO);
                string tagCellLotInfoStepID = string.Format("o{0}.CellLotInformationSend{1}.StepID", info.MODULE_NAME, info.PORTNO);
                string tagCellLotInfoCellSize = string.Format("o{0}.CellLotInformationSend{1}.CellSize", info.MODULE_NAME, info.PORTNO);
                string tagCellLotInfoCellThickness = string.Format("o{0}.CellLotInformationSend{1}.CellThickness", info.MODULE_NAME, info.PORTNO);
                string tagCellLotInfoComment = string.Format("o{0}.CellLotInformationSend{1}.Comment", info.MODULE_NAME, info.PORTNO);

                string tagCellLotInfoSendBit = string.Format("o{0}.Send.CellLotInformationSend{1}", info.MODULE_NAME, info.PORTNO);

                if (_eqpid != CommonData.Instance.EQP_SETTINGS.EQPID)
                {
                    _ACK3 = "5";
                    CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "CARRIER", "S3F109", null, "Not Match EQPID", null);
                }
                else if (CommonData.Instance.HOST_MODE != eHostMode.HostOnlineRemote)
                {
                    _ACK3 = "3";
                    CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "CARRIER", "S3F109", null, "Not Host Online Remote", null);
                }
                else
                {
                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagCellLotInfoCellID, _CELLID);
                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagCellLotInfoCassetteID, _CASSETTEID);
                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagCellLotInfoBatchLot, _BATCHLOT);
                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagCellLotInfoProductID, _PRODUCTID);
                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagCellLotInfoProductKind, _PRODUCT_KIND);
                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagCellLotInfoProductSpec, _PRODUCTSPEC);
                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagCellLotInfoProductType, _PRODUCT_TYPE);
                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagCellLotInfoStepID, _STEPID);
                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagCellLotInfoCellSize, _CELL_SIZE);
                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagCellLotInfoCellThickness, _CELL_THICKNESS);
                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagCellLotInfoComment, _COMMENT);

                    if (dataSetResult)
                    {
                        DataManager.Instance.SET_INT_DATA(tagCellLotInfoSendBit, 1);
                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "CARRIER", "S3F109", null, string.Format("Cell Lot Information Data Send #{0}", info.PORTNO), null);
                        _ACK3 = "0";
                    }
                    else
                    {
                        _ACK3 = "3";
                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "CARRIER", "S3F109", null, "Cell Lot Information Data Send", null);
                    }
                }
            }
            else
            {
                _ACK3 = "3";
                CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "CARRIER", "S3F109", null, "Cell Lot Information Data Send", null);
            }
           
            SecsMessage reply = new SecsMessage(Stream, Function, _systembyte);
            reply.AddAscii(_ACK3);
            SecsDriver.WriteLogAndSendMessage(reply, _ACK3);

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "CARRIER", "S3F110", null, "Cell Lot Information Data Reply", null);
        }
    }
}
