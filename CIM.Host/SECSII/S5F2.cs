using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDC.Core;
using SDC.IO;
using CIM.Manager;
using CIM.Common;
using SYSWIN.Secl;

namespace CIM.Host.Swin.SECSII
{
    public class S5F2 : SFMessage
    {
        public S5F2(SECSDriverBase driver)
            : base(driver)
        {
            Stream = 5; Function = 2;
        }

        public override void DoWork(string driverName, object obj)
        {
            SecsMessage primaryMessage = obj as SecsMessage;

            bool bResult;
            int stream = 5, function = 1;
            string sEQPID = DataManager.Instance.GET_STRING_DATA("vSys_Equipment_Id", out bResult);

            SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(SecsDriver.DeviceID));

            msg.AddList(5);                                                                                     //L5
            {
                msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                                      //A40 HOST REQ EQPID
                msg.AddAscii(sALST);                                                                                //A1  Alarm Occur/Clear
                msg.AddAscii(sALCD);                                                                                //A1  Alarm Level Code
                msg.AddAscii(AppUtil.ToAscii(sALID, gDefine.DEF_ALID_SIZE));                                        //A10 Alarm ID
                msg.AddAscii(AppUtil.ToAscii(sALTX, gDefine.DEF_ALTX_SIZE));                                        //A120 Alarm Description
            }

            writeLogSendMsg(stream, function, msg);


}
