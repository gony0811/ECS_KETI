using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS.Application
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

        public bool SIMULATION_MODE { get; set; }
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

    public class CommonData
    {
        public static readonly CommonData Instance = new CommonData();

        private readonly object eventLock = new object();

        private EQPSETTINGS _equipment;
        private EQPSTATUS _currentEqpStatus;
        private EQPSTATUS _postEqpStatus;

        private CommonData()
        {
            _equipment = new EQPSETTINGS();

            _currentEqpStatus = new EQPSTATUS();
            _postEqpStatus = new EQPSTATUS();
        }

        public EQPSETTINGS EQP_SETTINGS { get { return _equipment; } set { _equipment = value; } }
        public EQPSTATUS CURRENT_EQP_STATUS { get { return _currentEqpStatus; } set { _currentEqpStatus = value;} }
        public EQPSTATUS POST_EQP_STATUS { get { return _postEqpStatus; } set { _postEqpStatus = value; } }

    }
}
