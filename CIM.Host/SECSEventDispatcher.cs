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
using CIM.Host.SECSII;
using CIM.Log;

namespace CIM.Host
{
    public partial class SECSDriver : SECSDriverBase
    {

        protected void OnReceived(SecsMessage secsMessage)
        {
            int stream = secsMessage.Stream;
            int function = secsMessage.Function;
            uint systemByte = secsMessage.SystemByte;
            byte[] messageHeader = secsMessage.Header;
            bool waitBit = secsMessage.WaitBit;

            SetLog(SFDirection.smNone, "", "Received S{0}F{1}, Sysbyte={2:X8}", stream, function, systemByte);

            string dispatch = "S" + stream + "F" + function;

            switch (dispatch)
            {
                case "S1F1": //Are you there Request
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S1F2(this).DoWork("", secsMessage);
                    new S2F17(this).DoWork("", null);
                    break;
                case "S1F2":                    
                    new S2F17(this).DoWork("", null);
                    break;
                case "S1F3": //Selected Equipment State Request 
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S1F4(this).DoWork("", secsMessage);
                    break;
                case "S1F5": //Formatted State Request (SFCD : 1, 2, 3, 4)       
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S1F6(this).DoWork("", secsMessage);
                    break;
                case "S1F11": // Status Variable Name List Request                     
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S1F12(this).DoWork("", secsMessage);
                    break;
                case "S2F13":
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S2F14(this).DoWork("", secsMessage);
                    break;
                case "S2F18": // 
				LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S2F18(this).DoWork("", secsMessage);
                    new S6F11_ControlStateChange(this).DoWork("", "");
                    break;
                case "S2F23":
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S2F24(this).DoWork("", secsMessage);
                    break;
                case "S2F29":
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S2F30(this).DoWork("", secsMessage);
                    break;
                case "S2F31":
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    //new S2F32_미사용메시지(this).DoWork("", secsMessage);
                    break;
                case "S2F41":
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S2F42(this).DoWork("", secsMessage);
                    break;
                case "S2F43":
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S2F44(this).DoWork("", secsMessage);
                    break;
                case "S3F103":
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S3F104(this).DoWork("", secsMessage);
                    break;
                case "S3F105": // Material Information Send   
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S3F106(this).DoWork("", secsMessage);
                    break;
                case "S3F109": // Cell Lot Information Send
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S3F110(this).DoWork("", secsMessage);
                    break;
                case "S3F115": // Carrier Information Send
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S3F116(this).DoWork("", secsMessage);
                    break;
                case "S5F103":
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S5F104(this).DoWork("", secsMessage);
                    break;
                case "S5F113":
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S5F114(this).DoWork("", secsMessage);
                    break;
                case "S7F25":
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S7F26(this).DoWork("", secsMessage);
                    break;
                case "S7F101":
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S7F102(this).DoWork("", secsMessage);
                    break;
                case "S7F109":
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S7F110(this).DoWork("", secsMessage);
                    break;
                case "S10F5":
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S10F6(this).DoWork("", secsMessage);
                    break;
                case "S14F2": // 
                    LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, "", secsMessage);
                    new S14F2(this).DoWork("", secsMessage);
                    break;
            }
        }

        protected void OnConnected(object sender, ConnectEventArgs e)
        {
            IsConnected = true;
            SetLog(SFDirection.smNone, "", "[EVENT] HSMS CONNECTED BUT NOT SELECTED");
            DataManager.Instance.SET_INT_DATA("oPLC1.Send.TCDisconnectAlarm", 0);
            DataManager.Instance.SET_INT_DATA("oPLC2.Send.TCDisconnectAlarm", 0);
            DataManager.Instance.SET_INT_DATA("oPLC3.Send.TCDisconnectAlarm", 0);
            DataManager.Instance.SET_INT_DATA("oPLC4.Send.TCDisconnectAlarm", 0);
            DataManager.Instance.SET_INT_DATA("oPLC5.Send.TCDisconnectAlarm", 0);
            DataManager.Instance.SET_INT_DATA("oPLC6.Send.TCDisconnectAlarm", 0);
            DataManager.Instance.SET_INT_DATA("oPLC7.Send.TCDisconnectAlarm", 0);
            DataManager.Instance.SET_INT_DATA("oPLC8.Send.TCDisconnectAlarm", 0);
            LogHelper.Instance.BizLog.DebugFormat("TC CONNECT", "TC CONNECT", "");
        }

        protected void OnDisconnected(object obj, DisconnectEventArgs e)
        {
            LogHelper.Instance.BizLog.DebugFormat("TC DISCONNECT", "TC DISCONNECT", "");
            IsConnected = false;
            SetLog(SFDirection.smNone, "", "[EVENT] HSMS DISCONNECTED(IP:{0}, Port:{1}, Mode:{2})",
                        _secsDriver.Active ? _secsDriver.RemoteAddress : _secsDriver.LocalIP,
                        _secsDriver.Active ? _secsDriver.RemotePort : _secsDriver.LocalPort,
                        _secsDriver.Active ? "ACTIVE" : "PASSIVE");
            DataManager.Instance.SET_INT_DATA("oPLC1.Send.TCDisconnectAlarm", 1);
            DataManager.Instance.SET_INT_DATA("oPLC2.Send.TCDisconnectAlarm", 1);
            DataManager.Instance.SET_INT_DATA("oPLC3.Send.TCDisconnectAlarm", 1);
            DataManager.Instance.SET_INT_DATA("oPLC4.Send.TCDisconnectAlarm", 1);
            DataManager.Instance.SET_INT_DATA("oPLC5.Send.TCDisconnectAlarm", 1);
            DataManager.Instance.SET_INT_DATA("oPLC6.Send.TCDisconnectAlarm", 1);
            DataManager.Instance.SET_INT_DATA("oPLC7.Send.TCDisconnectAlarm", 1);
            DataManager.Instance.SET_INT_DATA("oPLC8.Send.TCDisconnectAlarm", 1);

            CommonData.Instance.HOST_MODE = eHostMode.HostOffline;

            FDCManager.Instance.AllTraceJobStop();
            
        }

        protected void OnSelected(object obj, SelectEventArgs e)
        {
            SetLog(SFDirection.smNone, "", "[EVENT] HSMS SELECTED(IP:{0}, Port:{1}, Mode:{2})",
                        _secsDriver.Active ? _secsDriver.RemoteAddress : _secsDriver.LocalIP,
                        _secsDriver.Active ? _secsDriver.RemotePort : _secsDriver.LocalPort,
                        _secsDriver.Active ? "ACTIVE" : "PASSIVE");

            new S1F1(this).DoWork("", "");
        }

        private void OnS9Event(object sender, S9EventArgs e)
        {
            SetLog(SFDirection.smNone, "", "[EVENT] Send Fail(S{0} F{1})", e.Stream, e.Function);
            LogHelper.Instance.BizLog.DebugFormat("{0}, {1}", e.Stream, e.Function);
        }

        private void OnSendFail(object sender, SendFailEventArgs e)
        {
            SecsMessage sendFail = e.SecsMessage;
            SetLog(SFDirection.smNone, "", "[EVENT] Send Fail(S{0} F{1})", sendFail.Stream, sendFail.Function);
            LogHelper.Instance.BizLog.DebugFormat("{0}, {1}",sendFail.Stream ,sendFail.Function);
        }

        private void OnReplyTimeout(object sender, ReplyTimeoutEventArgs e)
        {
            LogHelper.Instance.ErrorLog.DebugFormat("Reply Time Out" + e.SecsMessage);
            throw new NotImplementedException();
        }

        private void SetLog(SFDirection dir, string SF, string message, params object[] args)
        {
            StringBuilder sbAddString = new StringBuilder("");
            string logDir;
            string log;

            if (dir == SFDirection.smPrimaryOut)
                logDir = "1st MSG SEND (H<-E)";
            else if (dir == SFDirection.smPrimaryIn)
                logDir = "1st MSG RECV (H->E)";
            else if (dir == SFDirection.smSecondaryOut)
                logDir = "2nd MSG SEND (H<-E)";
            else if (dir == SFDirection.smSecondaryIn)
                logDir = "2nd MSG RECV (H->E)";
            else
                logDir = "";

            if (logDir == "")
            {
                sbAddString.Append(string.Format(message, args));
                log = string.Format("HOST : {0}", sbAddString);
            }
            else
            {
                sbAddString.Append(string.Format(message, args));
                log = string.Format("HOST : {0}\t{1}\t{2}", logDir, SF, sbAddString);
            }

            LogHelper.Instance.BizLog.DebugFormat(log);
        }

    }
}
