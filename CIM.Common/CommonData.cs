using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.IO;
using CIM.Manager;
using INNO6.Core;

namespace CIM.Common
{
    public enum eHostMode
    {
        HostNone = -1,
        HostOffline = 0,
        HostOnlineRemote = 1,
        HostOnlineLocal = 2,
    }

    public enum eModuleType
    {
        UNKNOWN,
        MASTER,
        NORMAL,
        LOADER,
        UNLOADER,
    }

    public enum ePORT_TYPE
    {
        INPUT = 0,
        OUTPUT = 1,
    }

    public enum ePORT_NUM
    {
        IP01,
        IP02,
        IP03,
        IP04,

        OP01,
        OP02,
        OP03,
        OP04,

        MP01,
        MP02,
        MP03,
        MP04,

        LI01,
        LI02,
        LI03,
        LI04,

        L001,
        LO02,
        LO03,
        LO04,

        UI01,
        UI02,
        UI03,
        UI04,

        UO01,
        UO02,
        UO03,
        UO04,
    }

    public enum PortStatus
    {
        LoadRequest = 250,
        LoadComplete = 251, // WA72
        UnloadRequest = 252,
        UnloadComplete = 253,
    }

    public enum CassetteStatus
    {
        LoadRequest = 350,
        LoadComplete = 351,
        UnloadRequest = 352,
        UnloadComplete = 353,
    }

    public enum CarrierType
    {
        UseTrayID = 1,
        NotUseTrayID = 2,
        UseBatchID = 11,
        NotUseBatchID = 12,
        ForcedUseTrayID = 3,
        ForcedNotUseTrayID = 4,
        ForcedUseBatchID = 13,
        ForcedNotUseBatchID = 14,
    }

    public enum eALST : int
    {
        SET = 1,
        RESET = 2,
    }

    public enum eALCD : int
    {
        LIGHT = 1,
        HEAVY = 2
    }

    public class EQPSETTINGS
    {
        public string EQPID { get; set; }
        public string EQPNAME { get; set; }
        public string MACADDRESS { get; set; }
        public string IPADDRESS { get; set; }
        public string SITEID { get; set; }
        public bool USE { get; set; }
        public int LOADPORTCOUNT { get; set; }
        public int MATERIALPORTCOUNT { get; set; }
        public int CELLPORTCOUNT { get; set; }
        public string CIM_SW_VERSION { get; set; }
        public string EQP_SW_VERSION { get; set; }
        public string ONLINE_MAP_VERSION { get; set; }
        public string FA_SPEC_VERSION { get; set; }
        public bool IQC_USE { get; set; }
        public string EXEC_CMD { get; set; }

        public string LOADER_MODULE_NAME { get; set; }
        public string UNLOADER_MODULE_NAME { get; set; }

        public string SERVER_PORT { get; set; }
    }

    public class MODULE
    {
        public string MODULE_NAME { get; set; }
        public string UNIT_ID { get; set; }
        public eModuleType TYPE { get; set; }
        public int MATERIAL_PORT_COUNT { get; set; }
        public int AGV_PORT_COUNT { get; set; }
        public int TRACK_IN_COUNT { get; set; }
        public int TRACK_OUT_COUNT { get; set; }
        public string DESCRIPTION { get; set; }

        public string UNIT_STATE { get; set; }
    }

    public class EQPSTATUS
    {
        public string AVAILABILITY { get; set; }
        public string MOVE { get; set; }
        public string RUN { get; set; }
        public string INTERLOCK { get; set; }
        public string FRONT { get; set; }
        public string REAR { get; set; }
        public bool HEAVY_ALARM { get; set; }
        public bool TPMLOSSREADY { get; set; }
    }

    public class CELLINFO
    {
        public string CELLID { get; set; }
        public string LOCATION { get; set; }
        public string JUDGE { get; set; }
        public string REASONCODE { get; set; }
    }


    public delegate void PropertyChangedEventHandler(object propertyName, object param);

    public class CommonData
    {
        public static readonly CommonData Instance = new CommonData();

        private readonly object eventLock = new object();

        private CommonData()
        {
            _equipment = new EQPSETTINGS();
            PPID_NOT_MATCH = false;
            INTERLOCK_SET = false;
            PORTSTATE_CHANGE_1 = false;
            PORTSTATE_CHANGE_2 = false;
            _moduleSettings = new List<MODULE>();
            _currentEqpStatus = new EQPSTATUS();
            _postEqpStatus = new EQPSTATUS();

            _propertyChanged += OnPropertyChanged;
        }

        private PropertyChangedEventHandler _propertyChanged;
        private eHostMode _hostMode;
        private EQPSETTINGS _equipment;
        private List<MODULE> _moduleSettings;
        private EQPSTATUS _currentEqpStatus;
        private EQPSTATUS _postEqpStatus;
        private KeepValidationData _validationDataMethod = new KeepValidationData();

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                lock (eventLock)
                {
                    _propertyChanged += value;
                }
            }
            remove
            {
                lock (eventLock)
                {
                    _propertyChanged -= value;
                }
            }
        }

        public List<MODULE> MODULE_SETTINGS { get { return _moduleSettings; } set { _moduleSettings = value; _propertyChanged.BeginInvoke("MODULE_SETTINGS", _moduleSettings, null, null); } }
        public EQPSETTINGS EQP_SETTINGS { get { return _equipment; } set { _equipment = value; _propertyChanged.BeginInvoke("EQP_INFO", _equipment, null, null); } }
        public EQPSTATUS CURRENT_EQP_STATUS { get { return _currentEqpStatus; } set { _currentEqpStatus = value; _propertyChanged.BeginInvoke("CURRENT_EQP_STATUS", _currentEqpStatus, null, null); } }
        public EQPSTATUS POST_EQP_STATUS { get { return _postEqpStatus; } set { _postEqpStatus = value; _propertyChanged.BeginInvoke("POST_EQP_STATUS", _postEqpStatus, null, null); } }

        public KeepValidationData VALIDATION_DATA { get { return _validationDataMethod; } set { _validationDataMethod = value; } }

        public bool PPID_NOT_MATCH { get;set; }
        public bool INTERLOCK_SET { get; set; }
        public bool PORTSTATE_CHANGE_1 { get; set; }
        public bool PORTSTATE_CHANGE_2 { get; set; }

        public string LOADER_CARRIERTYPE { get; set; }
        public string LOADER_CARRIERID { get; set; }
        public string LOADER_SUBCARRIERID { get; set; }
        public string UNLOADER_CARRIERTYPE { get; set; }
        public string UNLOADER_CARRIERID { get; set; }
        public string UNLOADER_SUBCARRIERID { get; set; }
        public string LOADER_CASSETTEID { get; set; }

        public eHostMode HOST_MODE
        {
            get { return _hostMode; }
            set { _hostMode = value; _propertyChanged.BeginInvoke("HOST_MODE", _hostMode, null, null); }
        }

        public List<string> GetSubCarrierIDs(string module, string group, int Count)
        {
            bool bResult = false;
            List<string> subCarrierIDs = new List<string>();

            //List<Data> subcarrierDataList = DataManager.Instance.DataAccess.RemoteObject.DataList.Where((t => t.Module == module && t.Group == group)).ToList();

            //foreach (Data data in subcarrierDataList)
            //{
            //    subCarrierIDs.Add(DataManager.Instance.GET_STRING_DATA(data.Name, out bResult));
            //}

            string defaultIOName = string.Empty;

            if (group == "LoaderSubCarrierID")
            {
                defaultIOName = "iPLC7.CarrierProcessChangeLoader.SubCarrierID";
            }
            else if (group == "UnloaderSubCarrierID")
            {
                defaultIOName = "iPLC8.CarrierProcessChangeUnloader.SubCarrierID";
            }

            for (int i = 1; i <= Count; i++)
            {
                string tagName = defaultIOName + string.Format("{0}", i);
                subCarrierIDs.Add(DataManager.Instance.GET_STRING_DATA(tagName, out bResult));
            }

            LogHelper.Instance._info.DebugFormat("[INFO] GetSubCarrierIDs() Success.");
            return subCarrierIDs;
            
        }

        public List<CELLINFO> GetCarrierCells(string module, string group)
        {
            int cellQty = 0;
            bool result = false;
            List<CELLINFO> cellDataList = new List<CELLINFO>();

            if(module.Equals("PLC7"))
            {
                if (group.Equals("LoaderSubCarrierID"))
                {
                    cellQty = DataManager.Instance.GET_INT_DATA("iPLC7.CarrierProcessChangeUnloader.CellQTY", out result);

                    for(int i = 0; i < cellQty; i++)
                    {
                        CELLINFO cellData = new CELLINFO();
                        //cellData.CELLID = DataManager.Instance.GET_STRING_DATA((IoDefine.PLC7.UNLOADER_CARRIER_INFO.CELL_ID_1 + i).ToString(), out result);
                        //cellData.JUDGE = DataManager.Instance.GET_STRING_DATA((IoDefine.PLC7.UNLOADER_CARRIER_INFO.JUDGE_1 + i).ToString(), out result);
                        //cellData.LOCATION = DataManager.Instance.GET_STRING_DATA((IoDefine.PLC7.UNLOADER_CARRIER_INFO.LOCATION_NO_1 + i).ToString(), out result);
                        //cellData.REASONCODE = DataManager.Instance.GET_STRING_DATA((IoDefine.PLC7.UNLOADER_CARRIER_INFO.REASONCODE_1 + i).ToString(), out result);

                        cellDataList.Add(cellData);
                    }
                }
                else
                {
                    LogHelper.Instance._info.DebugFormat("[ERROR] Check CARRIER_LOADER or CARRIER UNLOADER !!!");
                }
            }
            else if(module.Equals("PLC8"))
            {
                if (group.Equals("UnloaderSubCarrierID"))
                {
                    cellQty = DataManager.Instance.GET_INT_DATA("iPLC8.CarrierProcessChangeUnloader.CellQTY", out result);

                    for(int i = 0; i < cellQty; i++)
                    {
                        string cellidTagName = string.Format("iPLC8.CarrierProcessChangeUnloader.CellID{0}", i + 1);
                        string judgeTagName = string.Format("iPLC8.CarrierProcessChangeUnloader.Judge{0}", i + 1);
                        string locationTagName = string.Format("iPLC8.CarrierProcessChangeUnloader.LocationNo{0}", i + 1);
                        string reasonTagName = string.Format("iPLC8.CarrierProcessChangeUnloader.ReasonCode{0}", i + 1);

                        CELLINFO cellData = new CELLINFO();
                        cellData.CELLID = DataManager.Instance.GET_STRING_DATA(cellidTagName, out result);
                        cellData.JUDGE = DataManager.Instance.GET_STRING_DATA(judgeTagName, out result);
                        cellData.LOCATION = DataManager.Instance.GET_STRING_DATA(locationTagName, out result);
                        cellData.REASONCODE = DataManager.Instance.GET_STRING_DATA(reasonTagName, out result);

                        cellDataList.Add(cellData);
                    }
                }
            }
            else
            {
                LogHelper.Instance._info.DebugFormat("[ERROR] Check CARRIER_LOADER or CARRIER UNLOADER !!!");
            }

            return cellDataList;
        }

        private void OnPropertyChanged(object propertyName, object param)
        {
            string pName = (string)propertyName;
 
            switch(pName)
            {
                case "HOST_MODE":
                    {
                        this.OnConnectionChanged();
                    }
                    break;
            }
        }

        public delegate void MaterialDataChangedDele();
        public delegate void FDCDataChangedDelegate(string Name, string Value);
        public delegate void AlarmDataChangedDelegate(List<ALARM> ChangeAlarmList);
        //public delegate void UnitStatusChangedDelegate(MODULE UnitStatus);
        public delegate void ConnectionChangedDelegate(Common.eHostMode connectionState);
        public delegate void TerminalChangedDelegate(string Type, string UnitID, string ID, string Message);
        public delegate void StreamFunctionChangedDelegate(string Unit, string Eventer, string EventName, string SF_Function, string CEID, string Description, string Detail);
        //public delegate void AliveBitSignalChangedDelegate(string bitName, bool value);
        public delegate void AliveBitSignalChangedDelegate(string unitNo, string bitName, bool value);

        public delegate void LabelInformationChangeDelegate(string Cellid); // JYS : Label Information Send
        public delegate void CellValidationChangeDelegate(string Cellid, int PortID);
        public delegate void MsgSendFromUIDelegate(string name, object message, string handlerName); // JYS : UI에서 Test 할수 있게 하기 위한 Code
        public delegate void EqpStatusChangeDelegate(string available, string interlock, string run, string move, string ppspl);
        public delegate void UnitStatusChangeDelegate(string available, string interlock, string run, string move, string ppspl, string unitName, string moduleName);
        /// <summary>
        /// MaterialData ( UI 참조 정보 )
        /// </summary>
        public event MaterialDataChangedDele eMaterialDataChanged;
        /// <summary>
        /// FDCData ( UI 참조 정보 )
        /// </summary>
        public event FDCDataChangedDelegate eFDCDataChanged;
        /// <summary>
        /// AlarmData ( UI 참조 정보 )
        /// </summary>
        public event AlarmDataChangedDelegate eAlarmDataChanged;
        /// <summary>
        /// EQP or UNIT STATE ( UI 참조 정보 )
        /// </summary>
        //public event UnitStatusChangedDelegate eUnitStatusChanged;
        /// <summary>
        /// TC Connection State ( UI 참조 정보 )
        /// </summary>
        public event ConnectionChangedDelegate eConnectionChanged;
        /// <summary>
        /// Terminal Display ( UI 참조 정보 )
        /// </summary>
        public event TerminalChangedDelegate eTerminalChanged;
        /// <summary>
        /// SF_Function Data Display ( UI 참조 정보 )
        /// </summary>
        public event StreamFunctionChangedDelegate eStreamFunctionChanged;

        public event AliveBitSignalChangedDelegate eAliveSignalChanged;


        public event EqpStatusChangeDelegate eEqpStatusChanged;

        public event UnitStatusChangeDelegate UniteEqpStatusChanged;

        public void OnEqpStatusChanged(string available, string interlock, string run, string move, string ppspl)
        {
            if (eEqpStatusChanged != null)
                eEqpStatusChanged.BeginInvoke(available, interlock, run, move, ppspl, null, null);
        }
        public void OnUniteEqpStatusChanged(string available, string interlock, string run, string move, string ppspl, string unitNo, string moduleName)
        {
            if (UniteEqpStatusChanged != null)
                UniteEqpStatusChanged.BeginInvoke(available, interlock, run, move, ppspl, unitNo, moduleName, null, null);
        }

        public void OnAliveBitSignalChanged(string unitNo, string bitName, bool value)
        {
            if (eAliveSignalChanged != null)
            {
                eAliveSignalChanged.BeginInvoke(unitNo, bitName, value, null, null);
                LogHelper.Instance.Alivebit.DebugFormat(" [{0}] {1} / {2}", unitNo, bitName, value);
            }

        }

        public void OnMaterialDataChanged()
        {
            if (eMaterialDataChanged != null)
                eMaterialDataChanged.BeginInvoke(null, null);
        }

        public void OnFDCDataChanged(string svName, string svValue)
        {
            if (eFDCDataChanged != null)
                eFDCDataChanged.BeginInvoke(svName, svValue, null, null);
        }

        public void OnAlarmDataChanged()
        {
            if (eAlarmDataChanged != null)
            {
                eAlarmDataChanged.BeginInvoke(AlarmManager.Instance.GetCurrentOccurredAlarms(), null, null);
                //LogHelper.Instance.BizLog.DebugFormat("[OnAlarmDataChanged]" + AlarmManager.Instance.GetCurrentOccurredAlarms());
            }
        }

        //public void OnUnitStatusChanged(int uid, Unit unitStatus)
        //{
        //    if (eUnitStatusChanged != null)
        //    {
        //        eUnitStatusChanged.BeginInvoke(uid, unitStatus, null, null);
        //        //if (EQUIPMENT.UNITS.Count > 0) 

        //        //else
        //        //{
        //        //    //  확인 필요 ( 설비 오 보고 또는 설정 오류 ) Log?
        //        //}
        //    }
        //}

        public void OnConnectionChanged()
        {
            if (eConnectionChanged != null)
            {
                eConnectionChanged.BeginInvoke(CommonData.Instance.HOST_MODE, null, null);
                //LogHelper.Instance.BizLog.DebugFormat("[OnConnectionChanged]" + CommonData.Instance.HOST_MODE);
            }
        }

        public void OnTerminalChanged(string type, string unitid, string id, string message)
        {
            if (eTerminalChanged != null)
            {
                eTerminalChanged.BeginInvoke(type, unitid, id, message, null, null);
                //LogHelper.Instance.BizLog.DebugFormat("[OnTerminalChanged] Type : {0} , Unit ID : {1} , ID : {2} , Message : {3}", type, unitid, id, message);
            }
        }

        public void OnStreamFunctionAdd(string Unit, string Eventer, string EventName, string SF_Function, string CEID, string Description, string Detail)
        {
            if (eStreamFunctionChanged != null)
            {
                eStreamFunctionChanged.BeginInvoke(Unit, Eventer, EventName, SF_Function, CEID, Description, Detail, null, null);
                //LogHelper.Instance.BizLog.DebugFormat("[OnStreamFunctionAdd] Unit : {0} , Eventer : {1} , Event Name : {2} , SF : {3} , CEID : {4} , Description : {5}", Unit, Eventer, EventName, SF_Function, CEID , Description);
            }
        }

        public Dictionary<TrackingData.MaterialPort, TrackingData.Material> MaterialDataInfo { get; private set; }

        /// <summary>
        /// 자재 정보 변경 시 해당 정보를 저장한다.
        /// </summary>
        /// <param name="cellPort"></param>
        /// <param name="materialPort"></param>
        /// <param name="materialEvent"></param>
        /// <param name="nMaterialInfo"></param>
        public void SetMaterialData(TrackingData.CellPort cellPort, TrackingData.MaterialPort materialPort, TrackingData.EventType materialEvent, TrackingData.Material nMaterialInfo)
        {
            nMaterialInfo.EVENTNAME = materialEvent;
            if (MaterialDataInfo.ContainsKey(materialPort))
            {
                MaterialDataInfo[materialPort] = nMaterialInfo;
            }
            else
            {
                MaterialDataInfo.Add(materialPort, nMaterialInfo);
            }

            //CommonData.GetInstance().OnMaterialDataChanged();
        }

        /// <summary>
        /// 저장된 자재 정보를 삭제한다.
        /// </summary>
        /// <param name="materialPort"></param>
        public void RemoveMaterialData(TrackingData.MaterialPort materialPort)
        {
            if (MaterialDataInfo.ContainsKey(materialPort))
                MaterialDataInfo.Remove(materialPort);

            //CommonData.GetInstance().OnMaterialDataChanged();
        }

    }
}
