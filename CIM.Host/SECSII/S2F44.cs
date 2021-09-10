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
    public class S2F44 : SFMessage
    {
        public S2F44(SECSDriverBase driver)      
            : base(driver)
        {
            Stream = 2; Function = 42;
        }


        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            
            SecsMessage primaryMessage = obj as SecsMessage;

            uint systemByte = primaryMessage.SystemByte;

            string RCMD;
            string JOBID;
            string CELLID;
            string PRODUCTID;
            string STEPID;
            string ACTIONTYPE;

            string EQPID;
            string APPROVECODE;
            string APPROVEINFO;
            string APPROVEID;
            string BYWHO;
            string APPROVETEXT;

            string attributeName;
            int listCount;
            int remoteCommand;
            bool dataSetResult = true;
            Common.TC.HCACK HCACK = Common.TC.HCACK.ACCEPTED;

            listCount = primaryMessage.GetItem().GetList();
            RCMD = primaryMessage.GetItem().GetAscii();

                if(int.TryParse(RCMD, out remoteCommand))
                {

                    switch(remoteCommand)
                    {
                        case (int)Common.TC.RCMD.CELL_JOB_PROCESS_START:
                        case (int)Common.TC.RCMD.CELL_JOB_PROCESS_CANCEL:
                        case (int)Common.TC.RCMD.CELL_JOB_PROCESS_PAUSE:
                        case (int)Common.TC.RCMD.CELL_JOB_PROCESS_RESUME:
                            {
                                listCount = primaryMessage.GetItem().GetList();
                                listCount = primaryMessage.GetItem().GetList();

                                attributeName = primaryMessage.GetItem().GetAscii();
                                JOBID = primaryMessage.GetItem().GetAscii();

                                listCount = primaryMessage.GetItem().GetList();
                                attributeName = primaryMessage.GetItem().GetAscii();
                                CELLID = primaryMessage.GetItem().GetAscii();

                                listCount = primaryMessage.GetItem().GetList();
                                attributeName = primaryMessage.GetItem().GetAscii();
                                PRODUCTID = primaryMessage.GetItem().GetAscii();

                                listCount = primaryMessage.GetItem().GetList();
                                attributeName = primaryMessage.GetItem().GetAscii();
                                STEPID = primaryMessage.GetItem().GetAscii();

                                listCount = primaryMessage.GetItem().GetList();
                                attributeName = primaryMessage.GetItem().GetAscii();
                                ACTIONTYPE = primaryMessage.GetItem().GetAscii();


                                int portNo = 0;
                                MODULE module = CommonData.Instance.MODULE_SETTINGS.Where(m => m.TRACK_IN_COUNT > 0).FirstOrDefault();
                                VALIDATION_INFO info = CommonData.Instance.VALIDATION_DATA.FindValidationData(KindOfValidation.TRACK_IN_VALIDATION, CELLID, "");
                                
                                if(info != null)
                                {
                                    portNo = info.PORTNO;
                                }

                                if(portNo >= 1)
                                {
                                    string tagRcmd = string.Format("o{0}.CellJobProcess{1}.RCMD", module.MODULE_NAME, portNo);
                                    string tagJobID = string.Format("o{0}.CellJobProcess{1}.JobID", module.MODULE_NAME, portNo);
                                    string tagCellID = string.Format("o{0}.CellJobProcess{1}.CellID", module.MODULE_NAME, portNo);
                                    string tagProductID = string.Format("o{0}.CellJobProcess{1}.ProductID", module.MODULE_NAME, portNo);
                                    string tagEqpID = string.Format("o{0}.CellJobProcess{1}.EqpID", module.MODULE_NAME, portNo);
                                    string tagActionType = string.Format("o{0}.CellJobProcess{1}.ActionType", module.MODULE_NAME, portNo);
                                    string tagEventBit = string.Format("o{0}.Send.CellJobProcess{1}", module.MODULE_NAME, portNo);

                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagRcmd, RCMD);
                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagJobID, JOBID);
                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagCellID, CELLID);
                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagProductID, PRODUCTID);
                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagActionType, ACTIONTYPE);

                                    dataSetResult &= DataManager.Instance.SET_INT_DATA(tagEventBit, 1);

                                    CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "CELL JOB PROCESS", "S2F43", RCMD.ToString(), "Cell Job Process Start", null);
                                }
                                else
                                {
                                    //Console.WriteLine("[Debug] CELL ID({0}) is not registed in validation list", CELLID);
                                    LogHelper.Instance.SECSMessageLog.DebugFormat("[DEBUG] CELL ID({0}) is not registed in validation list", CELLID);
                                    CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "CELL JOB PROCESS", "S2F43", RCMD.ToString(), "Cell Job Process Cancel", null);
                                }       
                            }
                            break;
                        case (int)Common.TC.RCMD.EQP_APPROVE_FORBID:
                            {
                                EQPID = primaryMessage.GetItem().GetAscii();

                                if (EQPID.ToUpper().Trim() != CommonData.Instance.EQP_SETTINGS.EQPID)
                                {
                                    HCACK = Common.TC.HCACK.REJECT_ALREADYINDESIRECONDITION;
                                    CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "CELL JOB PROCESS", "S2F43", RCMD.ToString(), "HOST Command Nack", null);
                                    break;
                                }

                                listCount = primaryMessage.GetItem().GetList();
                                listCount = primaryMessage.GetItem().GetList();

                                attributeName = primaryMessage.GetItem().GetAscii();
                                APPROVECODE = primaryMessage.GetItem().GetAscii();

                                attributeName = primaryMessage.GetItem().GetAscii();
                                APPROVEINFO = primaryMessage.GetItem().GetAscii();

                                attributeName = primaryMessage.GetItem().GetAscii();
                                APPROVEID = primaryMessage.GetItem().GetAscii();

                                attributeName = primaryMessage.GetItem().GetAscii();
                                BYWHO = primaryMessage.GetItem().GetAscii();

                                attributeName = primaryMessage.GetItem().GetAscii();
                                APPROVETEXT = primaryMessage.GetItem().GetAscii();
                            }
                            break;
                        default:
                            {
                                HCACK = Common.TC.HCACK.COMMANDDOESNOEXIST;
                                CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "CELL JOB PROCESS", "S2F43", RCMD.ToString(), "HOST Command Nack", null);
                            }
                            break;
                    }  
                }

                string sHCACK = ((int)HCACK).ToString();

            SecsMessage reply = new SecsMessage(Stream, Function, systemByte)
            {
                WaitBit = false
            };
            reply.AddList(2);                                                                   //L2 
                {
                    reply.AddAscii(RCMD);                                                              //A1 Remote Control Command
                    reply.AddAscii(sHCACK);                                                             //A1 Host Command Acknowledge
                }

                SecsDriver.WriteLogAndSendMessage(reply, sHCACK);

                CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "CELL JOB PROCESS", "S2F44", RCMD.ToString(), "Cell Job Process Reply ", null);
        }
    }
}
