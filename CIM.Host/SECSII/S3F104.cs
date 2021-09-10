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
    public class S3F104 : SFMessage
    {
        string _EQPID;
        string _CARRIERID;
        string _UNIQUEID;
        string _UNIQUETYPE;
        string _PRODUCTID;
        string _PRODUCTSPEC;
        string _PRODUCT_TYPE;
        string _PRODUCT_KIND;
        string _PPID;
        string _STEPID;
        string _CELL_SIZE;
        string _CELL_THICKNESS;
        string _CELLINFORESULT;
        string _INS_COUNT;
        string _COMMENT;
        string _REPLYSTATUS;
        string _REPLYTEXT;

        Dictionary<string, string> _ITEM_LIST;

        string ACK3;

        public S3F104(SECSDriverBase driver)
            : base(driver)
        {
            Stream = 3; Function = 104;
        }


        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            bool dataSetResult = true;
            bool dataGetResult = true;
            string eqpStateAvailability = string.Empty;
            SecsMessage primaryMessage = obj as SecsMessage;

            uint systemByte = primaryMessage.SystemByte;

            int listCount1 = primaryMessage.GetItem().GetList();

            _EQPID = primaryMessage.GetItem().GetAscii();
            _CARRIERID = primaryMessage.GetItem().GetAscii();

            int listCount2 = primaryMessage.GetItem().GetList();

            _UNIQUEID = primaryMessage.GetItem().GetAscii();
            _UNIQUETYPE = primaryMessage.GetItem().GetAscii();
            _PRODUCTID = primaryMessage.GetItem().GetAscii();
            _PRODUCTSPEC = primaryMessage.GetItem().GetAscii();
            _PRODUCT_TYPE = primaryMessage.GetItem().GetAscii();
            _PRODUCT_KIND = primaryMessage.GetItem().GetAscii();
            _PPID = primaryMessage.GetItem().GetAscii();
            _STEPID = primaryMessage.GetItem().GetAscii();
            _CELL_SIZE = primaryMessage.GetItem().GetAscii();
            _CELL_THICKNESS = primaryMessage.GetItem().GetAscii();
            _CELLINFORESULT = primaryMessage.GetItem().GetAscii();
            _INS_COUNT = primaryMessage.GetItem().GetAscii();
            _COMMENT = primaryMessage.GetItem().GetAscii();

            int listCount3 = primaryMessage.GetItem().GetList();

            _ITEM_LIST = new Dictionary<string, string>(listCount3);

            for (int i = 0; i < listCount3; i++)
            {
                int listCount4 = primaryMessage.GetItem().GetList();
                string itemName = primaryMessage.GetItem().GetAscii();
                string itemValue = primaryMessage.GetItem().GetAscii();
                _ITEM_LIST.Add(itemName, itemValue);
            }

            int listCount5 = primaryMessage.GetItem().GetList();

            _REPLYSTATUS = primaryMessage.GetItem().GetAscii();
            _REPLYTEXT = primaryMessage.GetItem().GetAscii();

            VALIDATION_INFO info = CommonData.Instance.VALIDATION_DATA.FindValidationData(KindOfValidation.SPECIFIC_VALIDATION, _CARRIERID, "");

            if (info != null)
            {
                string tagSpecificValidCarrierId = string.Format("o{0}.SpecificValidationDataSend{1}.CarrierID", info.MODULE_NAME, info.PORTNO);
                string tagSpecificValidCellID = string.Format("o{0}.SpecificValidationDataSend{1}.CellID", info.MODULE_NAME, info.PORTNO);
                string tagSpecificValidUniqueType = string.Format("o{0}.SpecificValidationDataSend{1}.UniqueType", info.MODULE_NAME, info.PORTNO);
                string tagSpecificProductID = string.Format("o{0}.SpecificValidationDataSend{1}.ProductID", info.MODULE_NAME, info.PORTNO);
                string tagSpecificValidStepID = string.Format("o{0}.SpecificValidationDataSend{1}.StepID", info.MODULE_NAME, info.PORTNO);
                string tagSpecificValidReplyStatus = string.Format("o{0}.SpecificValidationDataSend{1}.ReplyStatus", info.MODULE_NAME, info.PORTNO);
                string tagSpecificValidReplyText = string.Format("o{0}.SpecificValidationDataSend{1}.ReplyText", info.MODULE_NAME, info.PORTNO);
                string tagSpecificValidDataSendBit = string.Format("o{0}.Send.SpecificValidationDataSend{1}", info.MODULE_NAME, info.PORTNO);
                string tagModuleAvailabilityState = string.Format("i{0}.EQStatus.Availability", info.MODULE_NAME);

                dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagSpecificValidCarrierId, _CARRIERID);
                dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagSpecificValidCellID, _UNIQUEID);
                dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagSpecificValidUniqueType, _UNIQUETYPE);
                dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagSpecificProductID, _PRODUCTID);
                dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagSpecificValidStepID, _STEPID);
                dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagSpecificValidReplyStatus, _REPLYSTATUS);
                dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagSpecificValidReplyText, _REPLYTEXT);
                dataSetResult &= DataManager.Instance.SET_INT_DATA(tagSpecificValidDataSendBit, 1);

                eqpStateAvailability = DataManager.Instance.GET_STRING_DATA(tagModuleAvailabilityState, out dataGetResult);

                CommonData.Instance.OnStreamFunctionAdd("UNLOADER", "H->E", "SPECIFIC", "S3F103", null, string.Format("Specific Validation Data Send #{0}", info.PORTNO), _REPLYSTATUS);
            }

                if (_EQPID != CommonData.Instance.EQP_SETTINGS.EQPID)
                {
                    // EQP ID doesn't Exist
                    ACK3 = "5";
                    CommonData.Instance.OnStreamFunctionAdd("UNLOADER", "H->E", "SPECIFIC", "S3F103", null, "Not Exist EQPID", null);
                }
                else if (string.IsNullOrEmpty(info.MODULE_NAME))
                {
                    ACK3 = "9";
                    CommonData.Instance.OnStreamFunctionAdd("UNLOADER", "H->E", "SPECIFIC", "S3F103", null, "Mismatch id", null);

                }

                else if (eqpStateAvailability != "2")
                {
                    // Can not Supported Mode(Local)
                    ACK3 = "3";
                    CommonData.Instance.OnStreamFunctionAdd("UNLOADER", "H->E", "SPECIFIC", "S3F103", null, "Availability Down", null);
                }
                else
                {
                    ACK3 = "0";
                }

            SecsMessage reply = new SecsMessage(Stream, Function, systemByte)
            {
                WaitBit = false
            };
            reply.AddAscii(ACK3);

                SecsDriver.WriteLogAndSendMessage(reply, ACK3);

                CommonData.Instance.OnStreamFunctionAdd("UNLOADER", "E->H", "SPECIFIC", "S3F104", null, "Specific Validation Data Reply", null);
            }    
         }
    }