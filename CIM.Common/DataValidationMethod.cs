using INNO6.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIM.Common
{
    public enum KindOfValidation
    {
        TRACK_IN_VALIDATION,
        SPECIFIC_VALIDATION,
        MATERIAL_INFO_SEND,
        CELL_LOT_INFO_SEND,
        CARRIER_INFO_SEND,
        ATTRIBUTE_INFO_SEND,
        SUBCARRIER_INFO_SEND,
    }

    public class VALIDATION_INFO
    {
        public string KEY_ITEM { get; set; }
        public int PORTNO { get; set; }
        public string MODULE_NAME { get; set; }

        public void CopyTo(VALIDATION_INFO data)
        {
            KEY_ITEM = data.KEY_ITEM;
            PORTNO = data.PORTNO;
            MODULE_NAME = data.MODULE_NAME;
        }
    }

    public class KeepValidationData
    {
        private List<VALIDATION_INFO> _keepTrackingValidationDataList = new List<VALIDATION_INFO>();
        private List<VALIDATION_INFO> _keepSpecificValidationDataList = new List<VALIDATION_INFO>();
        private List<VALIDATION_INFO> _keepMaterialInfoSendDataList = new List<VALIDATION_INFO>();
        private List<VALIDATION_INFO> _keepCellLotInfoSendDataList = new List<VALIDATION_INFO>();
        private List<VALIDATION_INFO> _keepCarrierInfoSendDataList = new List<VALIDATION_INFO>();
        private List<VALIDATION_INFO> _keepAttributeInfoSendDataList = new List<VALIDATION_INFO>();
        private List<VALIDATION_INFO> _keepSubCarrierInfoSendDataList = new List<VALIDATION_INFO>();

        public KeepValidationData()
        {
            
        }


        public void SaveValidationData(KindOfValidation whatKindOfValidation, string key, int portNo, string moduleName)
        {
            VALIDATION_INFO info = new VALIDATION_INFO() { KEY_ITEM = key, PORTNO = portNo, MODULE_NAME = moduleName };
            int remove;

            switch(whatKindOfValidation)
            {
                case KindOfValidation.TRACK_IN_VALIDATION:
                    {
                        remove = _keepTrackingValidationDataList.RemoveAll(v => (v.PORTNO == portNo && v.MODULE_NAME == moduleName) || v.KEY_ITEM == key);
                        _keepTrackingValidationDataList.Add(info);
                    }
                    break;

                case KindOfValidation.SPECIFIC_VALIDATION:
                    {
                        remove = _keepSpecificValidationDataList.RemoveAll(v => (v.PORTNO == portNo && v.MODULE_NAME == moduleName) || v.KEY_ITEM == key);
                        _keepSpecificValidationDataList.Add(info);
                    }
                    break;

                case KindOfValidation.MATERIAL_INFO_SEND:
                    {
                        remove = _keepMaterialInfoSendDataList.RemoveAll(v => (v.PORTNO == portNo && v.MODULE_NAME == moduleName) || v.KEY_ITEM == key);
                        _keepMaterialInfoSendDataList.Add(info);
                    }
                    break;
                case KindOfValidation.CELL_LOT_INFO_SEND:
                    {
                        remove = _keepMaterialInfoSendDataList.RemoveAll(v => (v.PORTNO == portNo && v.MODULE_NAME == moduleName) || v.KEY_ITEM == key);
                        _keepCellLotInfoSendDataList.Add(info);
                    }
                    break;
                case KindOfValidation.CARRIER_INFO_SEND:
                    {
                        remove = _keepCarrierInfoSendDataList.RemoveAll(v => (v.PORTNO == portNo && v.MODULE_NAME == moduleName) || v.KEY_ITEM == key);
                        _keepCarrierInfoSendDataList.Add(info);
                    }
                    break;
                case KindOfValidation.ATTRIBUTE_INFO_SEND:
                    {
                        remove = _keepAttributeInfoSendDataList.RemoveAll(v => (v.MODULE_NAME == moduleName) || v.KEY_ITEM == key);
                        _keepAttributeInfoSendDataList.Add(info);
                    }
                    break;
                case KindOfValidation.SUBCARRIER_INFO_SEND:
                    {
                        remove = _keepSubCarrierInfoSendDataList.RemoveAll(v => (v.PORTNO == portNo && v.MODULE_NAME == moduleName) || v.KEY_ITEM == key);
                        _keepSubCarrierInfoSendDataList.Add(info);
                    }
                    break;
            }

            LogHelper.Instance.BizLog.DebugFormat("[SaveValidationData] {0} , {1} , {2} , {3}", whatKindOfValidation, key, portNo, moduleName);
        }

        public VALIDATION_INFO FindValidationData(KindOfValidation whatKindOfValidation, string key, string validationId)
        {
            VALIDATION_INFO result = new VALIDATION_INFO();

            switch (whatKindOfValidation)
            {
                case KindOfValidation.TRACK_IN_VALIDATION:
                    {
                        var data = _keepTrackingValidationDataList.Find(t => t.KEY_ITEM == key);
                        if (data == null)
                            break;
                        result.CopyTo(data);
                        _keepTrackingValidationDataList.Remove(data);
                    }
                    break;

                case KindOfValidation.SPECIFIC_VALIDATION:
                    {
                        var data = _keepSpecificValidationDataList.Find(t => t.KEY_ITEM == key);
                        if (data == null)
                            break;
                        result.CopyTo(data);
                        _keepSpecificValidationDataList.Remove(data);
                    }
                    break;
                case KindOfValidation.MATERIAL_INFO_SEND:
                    {
                        var data = _keepMaterialInfoSendDataList.Find(t => t.KEY_ITEM == key);
                        if (data == null)
                        {
                            //var valid = _keepMaterialInfoSendDataList.Find(t => t.KEY_ITEM == validationId);
                            //if (valid == null)
                            //    break;
                            data = _keepMaterialInfoSendDataList.Find(t => t.KEY_ITEM == validationId);
                            if (data == null)
                                break;
                        }
                        result.CopyTo(data);
                        _keepMaterialInfoSendDataList.Remove(data);
                    }
                    break;
                case KindOfValidation.CELL_LOT_INFO_SEND:
                    {
                        var data = _keepCellLotInfoSendDataList.Find(t => t.KEY_ITEM == key);
                        if (data == null)
                            break;
                        result.CopyTo(data);
                        _keepCellLotInfoSendDataList.Remove(data);
                    }
                    break;
                case KindOfValidation.CARRIER_INFO_SEND:
                    {
                        var data = _keepCarrierInfoSendDataList.Find(t => t.KEY_ITEM == key);
                        if (data == null)
                            break;
                        result.CopyTo(data);
                        _keepCarrierInfoSendDataList.Remove(data);
                    }
                    break;
                case KindOfValidation.ATTRIBUTE_INFO_SEND:
                    {
                        var data = _keepAttributeInfoSendDataList.Find(t => t.KEY_ITEM == key);
                        if (data == null)
                            break;
                        result.CopyTo(data);
                        _keepAttributeInfoSendDataList.Remove(data);
                    }
                    break;
                case KindOfValidation.SUBCARRIER_INFO_SEND:
                    {
                        var data = _keepSubCarrierInfoSendDataList.Find(t => t.KEY_ITEM == key);
                        if (data == null)
                            break;
                        result.CopyTo(data);
                        _keepSubCarrierInfoSendDataList.Remove(data);
                    }
                    break;
                default:
                    {
                        result = null;
                        LogHelper.Instance._info.DebugFormat("[ERROR] Can not find FindValidationData");
                    }
                    break;
            }
            LogHelper.Instance.BizLog.DebugFormat("[FindValidationData] {0} , {1} , {2}", whatKindOfValidation, key, result);
            
            return result;
        }
    }
}
