using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core;
using INNO6.IO;
using CIM.Manager;
using CIM.Common;
using CIM.Common.TC;
using SYSWIN.Secl;

namespace CIM.Host.SECSII
{
    class S14F2 : SFMessage
    {
        /*S14F2(Get Attribute Data)*/

        public S14F2(SECSDriverBase driver)
            : base(driver)
        {
            Stream = 14; Function = 2;
        }
        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            CommonData.Instance.OnStreamFunctionAdd("LOADER/UNLOADER", "H->E", "ATTRIBUTE", "S14F2", null, "Get Attribute Data", null);
            SecsMessage primaryMessage = obj as SecsMessage;

            uint systemByte = primaryMessage.SystemByte;
            bool dataSetResult = true;
            string EQPID;
            string OBJTYPE;
            string OBJID;
            string COMMENT;
            string REPLYCODE;
            string REPLYTEXT;
            int List2 = 0;
            string[] ATTRID = new string[100];
            string[] ATTRDATA = new string[100];
            bool bGetAttributeErr = false;

            int List1 = primaryMessage.GetItem().GetList();                                         //L3 Teminal Set
            {
                EQPID = primaryMessage.GetItem().GetAscii();
                OBJTYPE = primaryMessage.GetItem().GetAscii();
                OBJID = primaryMessage.GetItem().GetAscii();
                COMMENT = primaryMessage.GetItem().GetAscii();
                List2 = primaryMessage.GetItem().GetList();
                if (List2 != 0)
                {
                    //string[] ATTRID = new string[List2];
                    //string[] ATTRDATA = new string[List2];
                    if (List2 != 0)
                    {
                        for (int i = 0; i < List2; i++)
                        {
                            int List3 = primaryMessage.GetItem().GetList();
                            ATTRID[i] = primaryMessage.GetItem().GetAscii();
                            ATTRDATA[i] = primaryMessage.GetItem().GetAscii();
                        }
                    }
                }
                int List4 = primaryMessage.GetItem().GetList();
                {
                    REPLYCODE = primaryMessage.GetItem().GetAscii();
                    REPLYTEXT = primaryMessage.GetItem().GetAscii();
                }
            }

            VALIDATION_INFO v_info = CommonData.Instance.VALIDATION_DATA.FindValidationData(KindOfValidation.ATTRIBUTE_INFO_SEND, OBJID, "");

            int nATTR_NO = 0;

            //Dictionary<string, string> attrValueList = AttributeManager.Instance.GetAttributeValueList(v_info.MODULE_NAME, "");
            nATTR_NO = List2;
            List<string> aATTR_NAME = new List<string>(nATTR_NO);
            List<string> aATTR = new List<string>(nATTR_NO);


            string TEMP1 = string.Format("o{0}.GetAttributeRequestReply.ReplyCode", v_info.MODULE_NAME);
            dataSetResult &= DataManager.Instance.SET_STRING_DATA(TEMP1, REPLYCODE);

            string TEMP2 = string.Format("o{0}.GetAttributeRequestReply.ReplyText", v_info.MODULE_NAME);
            dataSetResult &= DataManager.Instance.SET_STRING_DATA(TEMP2, REPLYTEXT);

            if (REPLYCODE == "PASS")
            {
                
                if (v_info != null)
                {
                    for (int i = 0; i < nATTR_NO;i++)
                    {
                        //if (i < 2) { 
                        //string AttributeData = string.Format("o{0}.ATTR.{1}", v_info.MODULE_NAME, i+2);
                        //dataSetResult &= DataManager.Instance.SET_STRING_DATA(AttributeData, ATTRDATA[i]);
                        //}
                        //if (i == 2){
                        //    string AttributeData = string.Format("o{0}.ATTR.{1}", v_info.MODULE_NAME, i - 1);
                        //    dataSetResult &= DataManager.Instance.SET_STRING_DATA(AttributeData, ATTRDATA[i]);
                        //}
                        string type = AttributeManager.Instance.GetValueByattr((i+1).ToString(), v_info.MODULE_NAME);

                        string AttributeData = string.Format("o{0}.ATTR.{1}", v_info.MODULE_NAME, i + 1);

                        if ((type == "Int") || (type == "Double"))
                        {
                            dataSetResult &= DataManager.Instance.SET_INT_DATA(AttributeData, Convert.ToInt16(ATTRDATA[i]));
                        }
                        else if(type == "String")
                        {
                            dataSetResult &= DataManager.Instance.SET_STRING_DATA(AttributeData, ATTRDATA[i]);
                        }
                        else
                        {
                            bGetAttributeErr = true;
                        }
                    }
                 }
                else
                    bGetAttributeErr = true;
            }
            else
                bGetAttributeErr = true;

            if(bGetAttributeErr == true)
            {
                for (int i = 0; i < nATTR_NO;i++)
                {
                    ATTRID[i] = null;
                    ATTRDATA[i] = null;
                }
                bGetAttributeErr = false;
                string AttributeBit = string.Format("o{0}.Reply.GetAttributeRequest", v_info.MODULE_NAME);
                dataSetResult &= DataManager.Instance.SET_INT_DATA(AttributeBit, 1);
            }
            else
            {
                string AttributeBit = string.Format("o{0}.Reply.GetAttributeRequest", v_info.MODULE_NAME);
                dataSetResult &= DataManager.Instance.SET_INT_DATA(AttributeBit, 1);
                
                for (int i = 0; i < nATTR_NO;i++)
                {
                    ATTRID[i] = null;
                    ATTRDATA[i] = null;
                }
            }

            if (!dataSetResult)
            {
                //Console.WriteLine("[Error] Termianl data set failed : Module Name ({0})", module.MODULE_NAME);
                LogHelper.Instance._debug.DebugFormat("[ERROR] S14F2 data set failed : Module Name ({0})", v_info.MODULE_NAME);

            }

            CommonData.Instance.OnStreamFunctionAdd("LOADER/UNLOADER", "E->H", "GET ATTRIBUTE DATA", "S14F2", null, null, null);
        }
    }
}
