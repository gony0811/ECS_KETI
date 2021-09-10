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
    public class S2F42 : SFMessage
    {
        public S2F42(SECSDriverBase driver)
            : base(driver)
        {
            Stream = 2; Function = 42;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            SecsMessage primaryMessage = obj as SecsMessage;

            uint systemByte = primaryMessage.SystemByte;
            bool dataSetResult = true;
            int remoteCommand;
            string RCMD;
            string EQPID;
            string OPCALL;
            string OPCALLID;
            string INTERLOCK;
            string MODULEID;
            string INTERLOCKID;
            string MESSAGE;
            string PPID;
            string PORTNO;
            string RFID;
            string PARENTLOT;
            string CELLCNT;
            string EFID;
            string EFST;

            HCACK HCACK = HCACK.ACCEPTED;

            int nList = primaryMessage.GetItem().GetList();                                         //L2 RCMD Set
            {
                RCMD = primaryMessage.GetItem().GetAscii();                                        //A1 RCMD
                             
                if(int.TryParse(RCMD, out remoteCommand))
                {
                    switch (remoteCommand)
                    {
                        #region RCMD : 1 (EQP OPCALL)
                        case (int)Common.TC.RCMD.EQP_OPCALL:
                            {
                               
                                nList = primaryMessage.GetItem().GetList();                                         //L1 RCMD Type Set
                                nList = primaryMessage.GetItem().GetList();
                                OPCALL = primaryMessage.GetItem().GetAscii();
                                EQPID = primaryMessage.GetItem().GetAscii();
                                OPCALLID = primaryMessage.GetItem().GetAscii();
                                MESSAGE = primaryMessage.GetItem().GetAscii();                                

                                if(EQPID.ToUpper().Trim() != CommonData.Instance.EQP_SETTINGS.EQPID)
                                {
                                    HCACK = Common.TC.HCACK.REJECT_ALREADYINDESIRECONDITION;
                                    CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "HOST", "S2F41", RCMD.ToString(), "HOST Command Nack", null);
                                    break;
                                }

                                foreach (MODULE module in CommonData.Instance.MODULE_SETTINGS)
                                {
                                    string tagOpcall = string.Format("o{0}.Opcall.Opcall", module.MODULE_NAME);
                                    string tagOpcallID = string.Format("o{0}.Opcall.ID", module.MODULE_NAME);
                                    string tagText = string.Format("o{0}.Opcall.Text", module.MODULE_NAME);
                                    string tagRcmd = string.Format("o{0}.Opcall.RCMD", module.MODULE_NAME);

                                    string tagOpCallEvent = string.Format("o{0}.Send.EQPOperatorCall", module.MODULE_NAME);

                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagOpcall, OPCALL);
                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagOpcallID, OPCALLID);
                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagText, MESSAGE);
                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagRcmd, RCMD);


                                    dataSetResult &= DataManager.Instance.SET_INT_DATA(tagOpCallEvent, 1);

                                    if (!dataSetResult)
                                    {
                                        //Console.WriteLine("[Error] Opcall data set failed : Module Name ({0})", module.MODULE_NAME);
                                        LogHelper.Instance.SECSMessageLog.DebugFormat("[ERROR] Opcall data set failed : Module Name ({0})", module.MODULE_NAME);
                                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "HOST", "S2F41", RCMD.ToString(), "HOST Command Nack", null);
                                    }
                                    else
                                    {
                                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "OPCALL", "S2F41", "1", "Opicall Set", null);
                                    }
                                }                              
                            }
                            break;
                        #endregion

                        #region RCMD : 2 (EQP INTERLOCK)
                        case (int)Common.TC.RCMD.EQP_INTERLOCK:
                            {
                                nList = primaryMessage.GetItem().GetList();                                         //L1 RCMD Type Set
                                nList = primaryMessage.GetItem().GetList();
                                INTERLOCK = primaryMessage.GetItem().GetAscii();
                                EQPID = primaryMessage.GetItem().GetAscii();
                                INTERLOCKID = primaryMessage.GetItem().GetAscii();
                                MESSAGE = primaryMessage.GetItem().GetAscii();

                                if (EQPID.ToUpper().Trim() != CommonData.Instance.EQP_SETTINGS.EQPID)
                                {
                                    HCACK = Common.TC.HCACK.REJECT_ALREADYINDESIRECONDITION;
                                    CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "HOST", "S2F41", RCMD.ToString(), "HOST Command Nack", null);
                                    break;
                                }

                                foreach (MODULE module in CommonData.Instance.MODULE_SETTINGS)
                                {
                                    string tagInterlock = string.Format("o{0}.Interlock.Interlock", module.MODULE_NAME);
                                    string tagInterlockID = string.Format("o{0}.Interlock.ID", module.MODULE_NAME);
                                    string tagText = string.Format("o{0}.Interlock.Text", module.MODULE_NAME);
                                    string tagRcmd = string.Format("o{0}.Interlock.RCMD", module.MODULE_NAME);

                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagInterlock, INTERLOCK);
                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagInterlockID, INTERLOCKID);
                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagText, MESSAGE);
                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagRcmd, RCMD);

                                    if (!dataSetResult)
                                    {
                                        //Console.WriteLine("[Error] Interlock data set failed : Module Name ({0})", module.MODULE_NAME);
                                        LogHelper.Instance.SECSMessageLog.DebugFormat("[ERROR] Interlock data set failed : Module Name ({0})", module.MODULE_NAME);
                                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "HOST", "S2F41", RCMD.ToString(), "HOST Command Nack", null);
                                    }
                                }
                                foreach (MODULE module in CommonData.Instance.MODULE_SETTINGS)   //KTW 18.04.11
                                {
                                    string tagInterlockEvent = string.Format("o{0}.Send.EQPInterlock", module.MODULE_NAME);

                                    dataSetResult &= DataManager.Instance.SET_INT_DATA(tagInterlockEvent, 1);

                                    if (!dataSetResult)
                                    {
                                        //Console.WriteLine("[Error] Interlock data set failed : Module Name ({0})", module.MODULE_NAME);
                                        LogHelper.Instance.SECSMessageLog.DebugFormat("[ERROR] Interlock data set failed : Module Name ({0})", module.MODULE_NAME);
                                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "HOST", "S2F41", RCMD.ToString(), "HOST Command Nack", null);
                                    }
                                    else
                                    {
                                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "INTERLOCK", "S2F41", "2", "Interlock Set", null);
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region RCMD : 4, 5, 6, 7, 8 (JOB PROCESS)
                        case (int)Common.TC.RCMD.JOB_PROCESS_START:
                        case (int)Common.TC.RCMD.JOB_PROCESS_ABORT:
                        case (int)Common.TC.RCMD.JOB_PROCESS_PAUSE:
                        case (int)Common.TC.RCMD.JOB_PROCESS_RESUME:
                        case (int)Common.TC.RCMD.JOB_PROCESS_CANCEL:
                            {
                                nList = primaryMessage.GetItem().GetList();                                         //L1 RCMD Type Set
                                nList = primaryMessage.GetItem().GetList();
                                PARENTLOT = primaryMessage.GetItem().GetAscii();
                                RFID = primaryMessage.GetItem().GetAscii();
                                EQPID = primaryMessage.GetItem().GetAscii();
                                PORTNO = primaryMessage.GetItem().GetAscii();
                                PPID = primaryMessage.GetItem().GetAscii();
                                CELLCNT = primaryMessage.GetItem().GetAscii();
                                
                                MESSAGE = primaryMessage.GetItem().GetAscii();

                                if (EQPID.ToUpper().Trim() != CommonData.Instance.EQP_SETTINGS.EQPID)
                                {
                                    HCACK = Common.TC.HCACK.REJECT_ALREADYINDESIRECONDITION;
                                    CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "HOST", "S2F41", RCMD.ToString(), "HOST Command Nack", null);
                                    break;
                                }

                                foreach(MODULE module in CommonData.Instance.MODULE_SETTINGS)
                                {
                                    string tagParentLot = string.Format("o{0}.JobEvent1.ParentLot", module.MODULE_NAME);
                                    string tagPortNo = string.Format("o{0}.JobEvent1.PortNo", module.MODULE_NAME);
                                    string tagRfid = string.Format("o{0}.JobEvent1.RFID", module.MODULE_NAME);
                                    string tagRcmd = string.Format("o{0}.JobEvent1.RCMD", module.MODULE_NAME);
                                    string tagCellCnt = string.Format("o{0}.JobEvent1.CellCNT", module.MODULE_NAME);
                                    string tagPPID = string.Format("o{0}.JobEvent1.PPID", module.MODULE_NAME);
                                    string tagMessage = string.Format("o{0}.JobEvent1.Message", module.MODULE_NAME);
                                    string tagJobEvent1 = string.Format("o{0}.Send.JobEvent1", module.MODULE_NAME);

                                    string tagParentLot2 = string.Format("o{0}.JobEvent2.ParentLot", module.MODULE_NAME);
                                    string tagPortNo2 = string.Format("o{0}.JobEvent2.PortNo", module.MODULE_NAME);
                                    string tagRfid2 = string.Format("o{0}.JobEvent2.RFID", module.MODULE_NAME);
                                    string tagRcmd2 = string.Format("o{0}.JobEvent2.RCMD", module.MODULE_NAME);
                                    string tagCellCnt2 = string.Format("o{0}.JobEvent2.CellCNT", module.MODULE_NAME);
                                    string tagPPID2 = string.Format("o{0}.JobEvent2.PPID", module.MODULE_NAME);
                                    string tagMessage2 = string.Format("o{0}.JobEvent2.Message", module.MODULE_NAME);
                                    string tagJobEvent2 = string.Format("o{0}.Send.JobEvent2", module.MODULE_NAME);

                                    if (PORTNO.Contains("01"))
                                    {
                                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagParentLot, PARENTLOT);
                                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagPortNo, PORTNO);
                                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagRfid, RFID);
                                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagRcmd, RCMD);
                                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagPPID, PPID);
                                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagMessage, MESSAGE);

                                        dataSetResult &= DataManager.Instance.SET_INT_DATA(tagJobEvent1, 1);
                                    }
                                    else if (PORTNO.Contains("02"))
                                    {
                                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagParentLot2, PARENTLOT);
                                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagPortNo2, PORTNO);
                                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagRfid2, RFID);
                                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagRcmd2, RCMD);
                                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagPPID2, PPID);
                                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagMessage2, MESSAGE);

                                        dataSetResult &= DataManager.Instance.SET_INT_DATA(tagJobEvent2, 1);
                                    }

                                    if (!dataSetResult)
                                    {
                                        //Console.WriteLine("[Error] Opcall data set failed : Module Name ({0})", module.MODULE_NAME);
                                        LogHelper.Instance.SECSMessageLog.DebugFormat("[ERROR] Opcall data set failed : Module Name ({0})", module.MODULE_NAME);
                                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "HOST", "S2F41", RCMD.ToString(), "HOST Command Nack", null);
                                    }
                                    else
                                    {
                                        if (PORTNO.Contains("01"))
                                        {
                                            CommonData.Instance.OnStreamFunctionAdd("LOADER", "H->E", "IQC", "S2F41", RCMD, "Host Job Process Event #1", null);
                                        }
                                        else if (PORTNO.Contains("02"))
                                        {
                                            CommonData.Instance.OnStreamFunctionAdd("UNLOADER", "H->E", "IQC", "S2F41", RCMD, "Host Job Process Event #2", null);
                                        }
                                    }
                                }
                            }
                            break;
                        #endregion

                        #region RCMD : 10 (FUNCTION CHANGE)
                        case (int)Common.TC.RCMD.FUNCTION_CHANGE :
                            {
                                EQPID = primaryMessage.GetItem().GetAscii();
                                nList = primaryMessage.GetItem().GetList();                                         //L1 RCMD Type Set
                                nList = primaryMessage.GetItem().GetList();

                                MODULEID = primaryMessage.GetItem().GetAscii();
                                EFID = primaryMessage.GetItem().GetAscii();
                                EFST = primaryMessage.GetItem().GetAscii();
                                MESSAGE = primaryMessage.GetItem().GetAscii();

                                if (EQPID.ToUpper().Trim() != CommonData.Instance.EQP_SETTINGS.EQPID)
                                {
                                    HCACK = Common.TC.HCACK.REJECT_ALREADYINDESIRECONDITION;
                                    CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "HOST", "S2F41", RCMD.ToString(), "HOST Command Nack", null);
                                    break;
                                }

                                HCACK = Common.TC.HCACK.ACCEPTED;
                                CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "EPQFUNCTION", "S2F41", "10", "EQ Function Change Commnad", null);
                            }
                            break;
                        #endregion

                        #region RCMD : 11, 12, 13, 14 (UNIT MACHINE CONTROL)
                        case (int)Common.TC.RCMD.TRANSFER_STOP:
                        case (int)Common.TC.RCMD.LOADING_STOP:
                        case (int)Common.TC.RCMD.STEP_STOP:
                        case (int)Common.TC.RCMD.OWN_STOP:
                            {
                                EQPID = primaryMessage.GetItem().GetAscii();
                                nList = primaryMessage.GetItem().GetList();                                         //L1 RCMD Type Set
                                nList = primaryMessage.GetItem().GetList();
                                INTERLOCK = primaryMessage.GetItem().GetAscii();
                                MODULEID = primaryMessage.GetItem().GetAscii();
                                INTERLOCKID = primaryMessage.GetItem().GetAscii();
                                MESSAGE = primaryMessage.GetItem().GetAscii();
                                
                                if (EQPID.ToUpper().Trim() != CommonData.Instance.EQP_SETTINGS.EQPID)
                                {
                                    HCACK = Common.TC.HCACK.REJECT_ALREADYINDESIRECONDITION;
                                    CommonData.Instance.OnStreamFunctionAdd(MODULEID, "H->E", "HOST", "S2F41", RCMD.ToString(), "HOST Command Nack", null);
                                    break;
                                }

                                if(MODULEID != "")
                                {
                                    MODULE module = CommonData.Instance.MODULE_SETTINGS.Where(m => m.UNIT_ID == MODULEID.ToUpper().Trim()).FirstOrDefault();
                                    if (module == null)
                                    {
                                        HCACK = Common.TC.HCACK.ERROR;
                                        break;
                                    }
                                    string tagInterlock = string.Format("o{0}.Interlock.Interlock", module.MODULE_NAME);
                                    string tagInterlockID = string.Format("o{0}.Interlock.ID", module.MODULE_NAME);
                                    string tagText = string.Format("o{0}.Interlock.Text", module.MODULE_NAME);
                                    string tagRcmd = string.Format("o{0}.Interlock.RCMD", module.MODULE_NAME);

                                    string tagInterlockEvent = string.Format("o{0}.Send.Interlock", module.MODULE_NAME);

                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagInterlock, INTERLOCK);
                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagInterlockID, INTERLOCKID);
                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagText, MESSAGE);
                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagRcmd, RCMD);


                                    dataSetResult &= DataManager.Instance.SET_INT_DATA(tagInterlockEvent, 1);

                                    if (!dataSetResult)
                                    {
                                        //Console.WriteLine("[Error] Interlock data set failed : Module Name ({0})", module.MODULE_NAME);
                                        LogHelper.Instance.SECSMessageLog.DebugFormat("[ERROR] Interlock data set failed : Module Name ({0})", module.MODULE_NAME);
                                        CommonData.Instance.OnStreamFunctionAdd(MODULEID, "H->E", "HOST", "S2F41", RCMD.ToString(), "HOST Command Nack", null);
                                    }
                                    else
                                    {
                                        CommonData.Instance.OnStreamFunctionAdd(MODULEID, "H->E", "INTERLOCK", "S2F41", RCMD, string.Format("EquipmentMachineControl #{0}", module.MODULE_NAME.Substring(3, 1)), string.Format("{0}_INTERLOCK", MODULEID));
                                    }
                                }
                                else
                                {
                                    foreach (MODULE module in CommonData.Instance.MODULE_SETTINGS)
                                    {
                                        string tagInterlock = string.Format("o{0}.Interlock.Interlock", module.MODULE_NAME);
                                        string tagInterlockID = string.Format("o{0}.Interlock.ID", module.MODULE_NAME);
                                        string tagText = string.Format("o{0}.Interlock.Text", module.MODULE_NAME);
                                        string tagRcmd = string.Format("o{0}.Interlock.RCMD", module.MODULE_NAME);

                                        string tagInterlockEvent = string.Format("o{0}.Send.EQPInterlock", module.MODULE_NAME);

                                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagInterlock, INTERLOCK);
                                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagInterlockID, INTERLOCKID);
                                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagText, MESSAGE);
                                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagRcmd, RCMD);


                                        dataSetResult &= DataManager.Instance.SET_INT_DATA(tagInterlockEvent, 1);

                                        if (!dataSetResult)
                                        {
                                            //Console.WriteLine("[Error] Interlock data set failed : Module Name ({0})", module.MODULE_NAME);
                                            LogHelper.Instance.SECSMessageLog.DebugFormat("[ERROR] Interlock data set failed : Module Name ({0})", module.MODULE_NAME);
                                            CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "HOST", "S2F41", RCMD.ToString(), "HOST Command Nack", null);
                                        }
                                        else
                                        {
                                            CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "INTERLOCK", "S2F41", "2", "Interlock Set", null);
                                        }
                                    }
                                }
                                
                            }
                            break;
                        #endregion

                        #region RCMD : 15 (CONTROL INFORMATION)
                        case (int)Common.TC.RCMD.CONTROL_INFOMATION:
                            {
                                EQPID = primaryMessage.GetItem().GetAscii();
                                nList = primaryMessage.GetItem().GetList();                                         //L1 RCMD Type Set
                                nList = primaryMessage.GetItem().GetList();

                                if (EQPID.ToUpper().Trim() != CommonData.Instance.EQP_SETTINGS.EQPID)
                                {
                                    HCACK = Common.TC.HCACK.REJECT_ALREADYINDESIRECONDITION;
                                    CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "HOST", "S2F41", RCMD.ToString(), "HOST Command Nack", null);
                                    break;
                                }

                                HCACK = Common.TC.HCACK.ACCEPTED;
                                CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "EQPCONTROLINFORMATION", "S2F41", "15", "EquipmentcontrolInformation", null);
                            }
                            break;
                        #endregion

                        #region RCMD : 16 (UNIT OPCALL SEND)
                        case (int)Common.TC.RCMD.UNIT_OPCALL:
                            {
                                EQPID = primaryMessage.GetItem().GetAscii();
                                nList = primaryMessage.GetItem().GetList();                                         //L1 RCMD Type Set
                                nList = primaryMessage.GetItem().GetList();

                                OPCALL = primaryMessage.GetItem().GetAscii();
                                MODULEID = primaryMessage.GetItem().GetAscii();
                                OPCALLID = primaryMessage.GetItem().GetAscii();
                                MESSAGE = primaryMessage.GetItem().GetAscii();

                                if (EQPID.ToUpper().Trim() != CommonData.Instance.EQP_SETTINGS.EQPID)
                                {
                                    HCACK = Common.TC.HCACK.REJECT_ALREADYINDESIRECONDITION;
                                    CommonData.Instance.OnStreamFunctionAdd(MODULEID, "H->E", "HOST", "S2F41", RCMD.ToString(), "HOST Command Nack", null);
                                    break;
                                }

                                if(MODULEID != "")
                                {   

                                    MODULE module = CommonData.Instance.MODULE_SETTINGS.Where(m => m.UNIT_ID == MODULEID.ToUpper().Trim()).FirstOrDefault();
                                    if (module == null)
                                    {
                                        HCACK = Common.TC.HCACK.ERROR;
                                        break;
                                    }
                                    string tagInterlock = string.Format("o{0}.opcall.Opcall", module.MODULE_NAME);
                                    string tagInterlockID = string.Format("o{0}.Opcall.ID", module.MODULE_NAME);
                                    string tagText = string.Format("o{0}.Opcall.Text", module.MODULE_NAME);
                                    string tagRcmd = string.Format("o{0}.Opcall.RCMD", module.MODULE_NAME);

                                    string tagInterlockEvent = string.Format("o{0}.Send.OperatorCall", module.MODULE_NAME);

                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagInterlock, OPCALL);
                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagInterlockID, OPCALLID);
                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagText, MESSAGE);
                                    dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagRcmd, RCMD);


                                    dataSetResult &= DataManager.Instance.SET_INT_DATA(tagInterlockEvent, 1);

                                    if (!dataSetResult)
                                    {
                                        //Console.WriteLine("[Error] Interlock data set failed : Module Name ({0})", module.MODULE_NAME);
                                        LogHelper.Instance.SECSMessageLog.DebugFormat("[ERROR] Interlock data set failed : Module Name ({0})", module.MODULE_NAME);
                                        CommonData.Instance.OnStreamFunctionAdd(MODULEID, "H->E", "HOST", "S2F41", RCMD.ToString(), "HOST Command Nack", null);
                                    }
                                    else
                                    {
                                        CommonData.Instance.OnStreamFunctionAdd(MODULEID.Substring(EQPID.Length + 1, 4), "H->E", "OPCALL", "S2F41", "16", "Unit OPCall Send", null);
                                    }
                                }
                                else
                                {
                                    foreach (MODULE module in CommonData.Instance.MODULE_SETTINGS)
                                    {
                                        string tagOpcall = string.Format("o{0}.Opcall.Opcall", module.MODULE_NAME);
                                        string tagOpcallID = string.Format("o{0}.Opcall.ID", module.MODULE_NAME);
                                        string tagText = string.Format("o{0}.Opcall.Text", module.MODULE_NAME);
                                        string tagRcmd = string.Format("o{0}.Opcall.RCMD", module.MODULE_NAME);

                                        string tagOpCallEvent = string.Format("o{0}.Send.EQPOperatorCall", module.MODULE_NAME);

                                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagOpcall, OPCALL);
                                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagOpcallID, OPCALLID);
                                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagText, MESSAGE);
                                        dataSetResult &= DataManager.Instance.SET_STRING_DATA(tagRcmd, RCMD);


                                        dataSetResult &= DataManager.Instance.SET_INT_DATA(tagOpCallEvent, 1);

                                        if (!dataSetResult)
                                        {
                                            //Console.WriteLine("[Error] Opcall data set failed : Module Name ({0})", module.MODULE_NAME);
                                            LogHelper.Instance.SECSMessageLog.DebugFormat("[ERROR] Opcall data set failed : Module Name ({0})", module.MODULE_NAME);
                                            CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "HOST", "S2F41", RCMD.ToString(), "HOST Command Nack", null);
                                        }
                                        else
                                        {
                                            CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "OPCALL", "S2F41", "1", "Opicall Set", null);
                                        }
                                    }
                                }
                                
                            }
                            break;
                        #endregion
                        default:
                            {
                                HCACK = Common.TC.HCACK.COMMANDDOESNOEXIST;
                                CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "HOST", "S2F41", RCMD.ToString(), "HOST Command Nack", null);
                                break;
                            }
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
            }
        }
    }
}
