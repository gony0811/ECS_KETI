using CIM.Common;
using CIM.Manager;
using SDC.Core;
using SDC.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SYSWIN.Secl;

namespace CIM.Host.Swin.SECSII
{
    public class S6F11_PPIDParameterChange : SFMessage
    {
                /*S6F11(TRS OPCALL Confirm), CEID:511*/
        public S6F11_PPIDParameterChange(SECSDriver driver) 
            : base(driver)
        {
            
        }
        public override void DoWork(string driverName, object obj)
        {
            int stream = 6, function = 11;

            string sDataID = "0";
            string sCRST = ((int)m_lnkHostData.Mode).ToString();
            string sAVAILABILITYSTATE = m_lnkData.StringGetStatus(ParamString.IF_EQPAvailability);
            string sINTERLOCKSTATE = m_lnkData.StringGetStatus(ParamString.IF_EQPInterlock);
            string sMOVESTATE = m_lnkData.StringGetStatus(ParamString.IF_EQPMove);
            string sRUNSTATE = m_lnkData.StringGetStatus(ParamString.IF_EQPRun);
            string sREARSTATE = m_lnkData.StringGetStatus(ParamString.IF_EQPRear);
            string sMAINSTATE = "00"; string sREASONCODE = m_lnkData.StringGetStatus(ParamString.IF_AvailabilityReasonCode);
            string sDESCRIPTION = m_lnkData.StringGetStatus(ParamString.IF_AvailabilityDescription);

            string sPPID = m_lnkPPIDInfo.CurrentPPID;
            string sPPIDST = "1";
            short nParameterList = 0;

            List<PPIDInfo.tagPPIDBody> ppidbodylist = new List<PPIDInfo.tagPPIDBody>();

            ppidbodylist = m_lnkPPIDInfo.InquireCurPPIDBody();
            nParameterList = Convert.ToInt16(ppidbodylist.Count);

            #region S6F11(PPID Parameter Change), CEID: CEID: 108

            SecsMessage msg = new SecsMessage(stream, function, true, Convert.ToInt32(DeviceID));

            msg.AddList(3);                                                                                         //L3  PPID Event Info
            {
                msg.AddAscii(AppUtil.ToAscii(sDataID, gDefine.DEF_DATAID_SIZE));                                        //A4  Data ID
                msg.AddAscii(AppUtil.ToAscii(sCEID, gDefine.DEF_CEID_SIZE));                                            //A3  Collection Event ID
                msg.AddList(4);                                                                                         //L4  RPTID Set
                {
                    msg.AddList(2);                                                                                         //L2  RPTID 100 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("100", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID="100"
                        msg.AddList(2);                                                                                         //L2  EQP Control State Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sEQPID, gDefine.DEF_EQPID_SIZE));                                         //A40 HOST REQ EQPID
                            msg.AddAscii(sCRST);                                                                                   //A1  Online Control State
                        }
                    }
                    msg.AddList(2);                                                                                         //L2  RPTID 101 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("101", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID="101"
                        msg.AddList(7);                                                                                         //L7  EQP State Set
                        {
                            msg.AddAscii(sMAINSTATE);                                                                               //A1  EQ Avilability State Info
                            msg.AddAscii(sAVAILABILITYSTATE);                                                                       //A1  EQ Avilability State Info
                            msg.AddAscii(sINTERLOCKSTATE);                                                                          //A1  Interlock Avilability State Info
                            msg.AddAscii(sMOVESTATE);                                                                               //A1  EQ Move State Info
                            msg.AddAscii(sRUNSTATE);                                                                                //A1  Cell existence/nonexistence Check
                            msg.AddAscii(AppUtil.ToAscii(sREASONCODE, gDefine.DEF_REASONCODE_SIZE));                                //A20 Code Info
                            msg.AddAscii(AppUtil.ToAscii(sDESCRIPTION, gDefine.DEF_DESCRIPTION_SIZE));                              //A40 EQ Description
                        }
                    }
                    msg.AddList(2);                                                                                         //L2  RPTID 302 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("302", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID="302"
                        msg.AddList(2);                                                                                         //L2  PPID Set
                        {
                            msg.AddAscii(AppUtil.ToAscii(sPPID, gDefine.DEF_PPID_SIZE));                                            //A40 Changed PPID
                            msg.AddAscii(sPPIDST);                                                                                  //A1  PPID Type
                        }
                    }
                    msg.AddList(2);                                                                                         //L2  RPTID 303 Set
                    {
                        msg.AddAscii(AppUtil.ToAscii("303", gDefine.DEF_RPTID_SIZE));                                           //A3  RPTID="303"
                        msg.AddList(nParameterList);                                                                            //Ln  PARAMETERLIST
                        {
                            foreach (PPIDInfo.tagPPIDBody ppidbody in ppidbodylist)
                            {
                                string sPPARMNAME = ppidbody.ParamName;
                                string sPPARMVALUE = ppidbody.ParamValue;

                                msg.AddList(2);                                                                                     //L2  PARAMETERSET
                                {
                                    msg.AddAscii(AppUtil.ToAscii(sPPARMNAME, gDefine.DEF_PARAMETERNAME_SIZE));                          //A40 PARAMETERNAME
                                    msg.AddAscii(AppUtil.ToAscii(sPPARMVALUE, gDefine.DEF_PARAMETERVALUE_SIZE));                        //A40 PARAMETERVALUE
                                }
                            }
                        }
                    }
                }
            }

            this.SecsDriver.Send(msg);
            #endregion
        }
    }
}
