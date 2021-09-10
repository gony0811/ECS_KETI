using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CIM.Common
{
    namespace TC
    {
        public enum SFCD : int
        {
            EquipmentState = 1,
            UnitState = 2,
            MaterialState = 3,
            PortState = 4
        }

        public enum MODE : int
        {
            hostNone = -1,
            hostOffline = 0,
            hostOnlineRemote = 1,
            hostOnlineLocal = 2
        }

        public enum ACKC2 : int
        {
            ACCEPTED = 0,
            ERROR = 1
        }

        public enum ACKC3 : int
        {
            ACCEPTED = 0,
            ERROR = 1
        }

        public enum ACKC5 : int
        {
            ACCEPTED = 0,
            ERROR = 1
        }

        public enum ACKC6 : int
        {
            ACCEPTED = 0,
            ERROR = 1
        }

        public enum ACKC7 : int
        {
            ACCEPTED = 0,
            Permissionnotgranted = 1,
            LengthError = 2,
            Modeunsupported = 3,
            TargetmoduleisOffline = 4,
            TargetmoduleisDown = 5,
            TimeOutHappened = 6,
            EQPIDisnotexist = 7,
            ModuleIDisnotexist = 8,
            PPID_TYPEisnotmatch = 9
        }

        public enum ACKC8 : int
        {
            ACCEPTED = 0,
            EQPIDmismatch = 1,
            ModuleIDmismatch = 2,
            Atleastonedatainvalid = 3
        }

        public enum ACKC10 : int
        {
            ACCEPTED = 0,
            ERROR = 1,
            EQPIDisnotexist = 2
        }

        public enum OFLACK : int
        {
            ACCEPTED = 0,
            ERROR = 1
        }

        public enum ONLACK : int
        {
            ACCEPTED = 0,
            NOTACCEPTED = 1,
            ALREADYONLINELOCAL = 2,
            ALREADYONLINEREMOTE = 3
        }

        public enum HCACK : int
        {
            ACCEPTED = 0,
            CELLISINVALID = 1,
            COMMANDDOESNOEXIST = 2,
            REJECT_ALREADYINDESIRECONDITION = 3,
            OTHERERRORS = 4,
            NOTFOUNDFUNCTION = 5,
            ERROR = 6,
			UNKNOWN_EQPID = 7,
            UNKNOWN_PPIDTYPE = 8,
        }

        public enum TIAACK : int
        {
            ACCEPTED = 0,
            ToolManySVIDs = 1,
            NoMoretracesAllowed = 2,
            InvalidPeriod = 3,
            EquipmentSpecifiedError = 4,
            EquipmentOffline = 5
        }

        public enum MIACK : int
        {
            NOTACCEPTED = 2,
            DOWN = 0,
            UP = 1
        }

        public enum TIACK : int
        {
            ACCEPTED = 0,
            NOTACCEPTED = 1
        }

        public enum TMACK : int
        {
            ACCEPTED = 0,
            NOTACCEPTED = 1,
            MessageAvailable = 21,
            DataNotExist = 22,
            EQPIDMistmatch = 22,
            MODULEIDMistmatch = 23,
            SEQNOMistmatch = 24,
            CELLIDMistmatch = 25,
            PPIDMismatch = 26,
            PPIDTypeMistmatch = 27,
            ParamNameMismatch = 28,
            ParamValueMismatch = 29,
            ParamNameDuplicate = 30,
            ParamValueDuplicate = 31,
            SEQNORangeError = 32,
            ParamNameRangeError = 33,
            EQPAvailableDown = 34,
            CELLIDExist = 35
        }

        public enum RCMD : int
        {
            EQP_OPCALL = 1,
            EQP_INTERLOCK = 2,
            JOB_SELECT = 3,
            JOB_PROCESS_START = 4,
            JOB_PROCESS_ABORT = 5,
            JOB_PROCESS_PAUSE = 6,
            JOB_PROCESS_RESUME = 7,
            JOB_PROCESS_CANCEL = 8,
            JOB_CHANGE = 9,
            FUNCTION_CHANGE = 10,
            TRANSFER_STOP = 11,
            LOADING_STOP = 12,
            STEP_STOP = 13,
            OWN_STOP = 14,
            CONTROL_INFOMATION = 15,
            UNIT_OPCALL = 16,

            CELL_JOB_PROCESS_START = 21,
            CELL_JOB_PROCESS_CANCEL = 22,
            CELL_JOB_PROCESS_PAUSE = 23,
            CELL_JOB_PROCESS_RESUME = 24,

            EQP_APPROVE_PERMIT = 31,
            EQP_APPROVE_FORBID = 32,
        }
    }


}
