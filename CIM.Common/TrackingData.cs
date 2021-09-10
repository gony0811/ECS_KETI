using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIM.Common
{
    /// <summary>
    /// Tracking Data 객체
    /// </summary>
    public class TrackingData
    {
        public enum EventType
        {
            None,
            Kitting = 221,
            NG = 222,
            KittingCancel = 219,
            Assemble = 215,
            TrackIn = 401,
            TrackOut = 406,
            PairCellTrackIn = 411,
            PairCellTrackOut = 412,
            SupplyRequest = 211,
            SupplyComplete = 220,
            Warning = 223,
            Shortage = 224,

            KittingResult = 90001,
            LabelRequest = 90002,
            LabelValidation = 90003,
            LabelSend = 90004
        }

        public enum CellPort
        {
            None,
            Port1 = 1,
            Port2 = 2,
            Port3 = 3,
            Port4 = 4,
            Port5 = 5,
            Port6 = 6,
        }

        public enum MaterialPort
        {
            None,
            Port1 = 1,
            Port2 = 2,
            Port3 = 3,
            NG1 = 11
        }

        public struct Cell
        {
            public EventType EVENTNAME;
            public string CEID;
            public string PORTID;
            public string CELLID;
            public string PRODUCTID;
            public string STEPID;
            public string PROCESSJOBID;
            public string PLANQUANTITY;
            public string PROCESSQUANITITY;
            public string READERID;
            public string RRC;
            public string OPERATORID;
            public string JUDGE;
            public string EVENTTIME;
        }

        public class Material
        {
            public EventType EVENTNAME { get; set; }
            public string CEID { get; set; }
            public string BATCHID { get; set; }
            public string BATCHNAME { get; set; }
            public string CELLID { get; set; }
            public string PPID { get; set; }
            public string PRODUCTID { get; set; }
            public string STEPID { get; set; }
            public string ID { get; set; }
            public string TYPE { get; set; }
            public string ST { get; set; }
            public string PORTID { get; set; }
            public string STATE { get; set; }
            public string TOTALQTY { get; set; }
            public string USEQTY { get; set; }
            public string ASSEMQTY { get; set; }
            public string NGQTY { get; set; }
            public string REMAINQTY { get; set; }
            public string PRODUCTQTY { get; set; }
            public string PROCESSUSEQTY { get; set; }
            public string PROCESSASSEMQTY { get; set; }
            public string PROCESSNGQTY { get; set; }
            public string SUPPLYREQUESTQTY { get; set; }
            public string REPLYSTATUS { get; set; }
            public string REPLYCODE { get; set; }
            public string REPLYTEXT { get; set; }
            public string EVENTTIME { get; set; }
        }

        public struct Label
        {
            /* S3F111
             * <L, 3
                    1.<A[40] $EQPID> * 설비 고유 ID
                    2.<A[10] $OPTIONCODE> * Action 코드정보
                    3.<L, 6 * Cell Lot List Count
                    1.<A[40] $CELLID> * Cell별로 부여 된 Unique ID
                    2.<A[40] $PRODUCTID> * 제품 정보
                    3.<A[200] $LABELID> * Label별로 부여 된 Unique ID
                    4.<A[4] $REPLYSTATUS> * 정합성에 대한 결과 값
                    5.<A[10] $REPLYCODE> * 정합성에 대한 코드 값
                    6.<A[120] $REPLYTEXT> * 결과에 대한 내용
             */

            /* S6F201
             * <L, 3 * Label Info Set
                    1.<A[40] $EQPID> * 설비 고유 ID
                    2.<A[10] $OPTIONCODE> * Action Code Info
                    3.<L, 4 * Label List Set
                    1.<A[40] $CELLID> * Cell 별로 부여 된 Unique ID
                    2.<A[40] $PRODUCTID> * 제품 정보
                    3.<A[80] $OPTIONINFO> * Action Data
                    4.<A[200] $LABELID> * Label별로 부여 된 Unique ID
             */
            public EventType EVENTNAME;
            public string LABELID;
            public string CELLID;
            public string PRODUCTID;
            public string OPTIONCODE;
            public string REPLYSTATUS;
            public string REPLYCODE;
            public string REPLYTEXT;
            public string OPTIONINFO;
            public string RESULT;
            public string EVENTTIME;
        }
    }
}
