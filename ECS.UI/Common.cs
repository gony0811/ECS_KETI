using INNO6.Core.Manager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECS.UI
{
    public enum MSGBOX_RESULT
    {
        OK,
        ABORT,
        CANCEL
    }

    public enum PROCESS_RESULT
    {
        SUCCESS,
        FAIL,
        ABORT
    }

    public class MESSAGE_NAME
    {
        public const string ReicipeGeneration = "ReicipeGeneration";
        public const string RecipeModify = "RecipeModify";
        public const string PPIDDelete = "PPIDDelete";
        public const string EquipmentGetLoss = "EquipmentGetLoss";
    }
    public class UI_EVENT_MESSAGE
    {
        public const string UI_RecipeConfirm_TEST = "UI_RecipeConfirm_TEST";
        public const string UI_Recipe_Delete = "UI_Recipe_Delete";
        public const string UI_Recipe_Generation = "UI_Recipe_Generation";
        public const string UI_Recipe_Download = "UI_Recipe_Download";
        public const string UI_Recipe_Download_Fail = "UI_Recipe_Download_Fail";
        public const string UI_Recipe_Modify = "UI_Recipe_Modify";
        public const string UI_WorkInfoChange = "UI_WorkInfoChange";
        public const string UI_TerminalMessage = "UI_TerminalMessage";
        public const string UI_RequestLossWindow = "UI_RequestLossWindow";
        public const string UI_JobInformationDownload = "UI_JobInformationDownload";
        public const string UI_LotInformationDownload = "UI_LotInformationDownload";
        public const string UI_BasketExist = "UI_BasketExist";
        public const string UI_PanelIN = "UI_PanelIN";
        public const string UI_PanelOUT = "UI_PanelOUT";
        public const string UI_Date_and_Time_Set = "UI_Date_and_Time_Set";
        public const string UI_NotBasket = "UI_NotBasket";
        public const string UI_UserBasketConfirm = "UI_UserBasketConfirm";
        public const string UI_PanelOver = "UI_PanelOver";
        public const string UI_PanelLack = "UI_PanelLack";
        public const string UI_JobCancel = "UI_JobCancel"; //20151201 JobCancel Logic 대응
        public const string UI_MessageBox = "UI_MessageBox";
        public const string UI_RFIDState = "UI_RFIDState";
        public const string UI_RecipeTimeOut = "UI_RecipeTimeOut";
        public const string UI_AllClearState = "UI_AllClearState";

        public const string OPC_LOTStart = "OPC_LOTStart";
        public const string OPC_LOTEnd = "OPC_LOTEnd";
    }

    public enum enumAUTHORITY
    {
        ADMIN,
        ENGINEER,
        OPERATOR,
        USER,
    }

    public class LogType
    {
        public const string LOT_DATA = "LOT_DATA";
        public const string BASKET_DATA = "BASKET_DATA";
        public const string PANEL_DATA = "PANEL_DATA";
        public const string PIO = "PIO";
        public const string Exception = "Exception";
        public const string ECSUI = "ECS";
        public const string DBLOG = "DBLOG";
    }

    public class VIRTUAL
    {
        public const string Basket = "VB_";
    }
    public class Common
    {
        public string UserAccount = "UNKNOWN";
        public eUserLevel UserAuthority
        {
            get;
            set;
        }
        bool _LoginState = false;
        public bool LoginState
        {
            get { return _LoginState; }
            set { _LoginState = value; }
        }
        bool _LoginStateOld = false;
        public bool LoginStateOld
        {
            get { return _LoginStateOld; }
            set { _LoginStateOld = value; }
        }

        bool _LoginStateFlag = false;
        public bool LoginStateFlag
        {
            get { return _LoginStateFlag; }
            set { _LoginStateFlag = value; }
        }

        bool _ThreadControl = false;
        public bool ThreadControl
        {
            get { return _ThreadControl; }
            set { _ThreadControl = value; }
        }

         //Singleton 객체
        private static Common _instance;

        public static Common GetInstance() //객체 생성 대신 사용.
        {
            if (_instance == null)
                return _instance = new Common();

            return _instance;
        }

        private Common()
        {

        }
    }    
}
