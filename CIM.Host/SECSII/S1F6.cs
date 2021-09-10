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

namespace CIM.Host.SECSII
{
    public class S1F6 : SFMessage
    {
        bool bResult;

        string _sfcd = string.Empty;
        string _eqpid;
        uint _systembyte;
        int _eqpListCount = 1;
        string _CRST;
        string _AVAILABILITYSTATE;
        string _INTERLOCKSTATE;
        string _MOVESTATE;
        string _RUNSTATE;
        string _FRONTSTATE;
        string _REARSTATE;
        string _PP_SPLSTATE;
        string _REASONCODE;
        string _DESCRIPTION;

        string _UNIT1_AVAILABILITYSTATE;
        string _UNIT1_INTERLOCKSTATE;
        string _UNIT1_MOVESTATE;
        string _UNIT1_RUNSTATE;
        string _UNIT1_FRONTSTATE;
        string _UNIT1_REARSTATE;
        string _UNIT1_PP_SPLSTATE;
        string _UNIT1_REASONCODE;
        string _UNIT1_DESCRIPTION;

        string _UNIT2_AVAILABILITYSTATE;
        string _UNIT2_INTERLOCKSTATE;
        string _UNIT2_MOVESTATE;
        string _UNIT2_RUNSTATE;
        string _UNIT2_FRONTSTATE;
        string _UNIT2_REARSTATE;
        string _UNIT2_PP_SPLSTATE;
        string _UNIT2_REASONCODE;
        string _UNIT2_DESCRIPTION;

        string _UNIT3_AVAILABILITYSTATE;
        string _UNIT3_INTERLOCKSTATE;
        string _UNIT3_MOVESTATE;
        string _UNIT3_RUNSTATE;
        string _UNIT3_FRONTSTATE;
        string _UNIT3_REARSTATE;
        string _UNIT3_PP_SPLSTATE;
        string _UNIT3_REASONCODE;
        string _UNIT3_DESCRIPTION;

        string _UNIT4_AVAILABILITYSTATE;
        string _UNIT4_INTERLOCKSTATE;
        string _UNIT4_MOVESTATE;
        string _UNIT4_RUNSTATE;
        string _UNIT4_FRONTSTATE;
        string _UNIT4_REARSTATE;
        string _UNIT4_PP_SPLSTATE;
        string _UNIT4_REASONCODE;
        string _UNIT4_DESCRIPTION;

        string _UNIT5_AVAILABILITYSTATE;
        string _UNIT5_INTERLOCKSTATE;
        string _UNIT5_MOVESTATE;
        string _UNIT5_RUNSTATE;
        string _UNIT5_FRONTSTATE;
        string _UNIT5_REARSTATE;
        string _UNIT5_PP_SPLSTATE;
        string _UNIT5_REASONCODE;
        string _UNIT5_DESCRIPTION;

        string _UNIT6_AVAILABILITYSTATE;
        string _UNIT6_INTERLOCKSTATE;
        string _UNIT6_MOVESTATE;
        string _UNIT6_RUNSTATE;
        string _UNIT6_FRONTSTATE;
        string _UNIT6_REARSTATE;
        string _UNIT6_PP_SPLSTATE;
        string _UNIT6_REASONCODE;
        string _UNIT6_DESCRIPTION;

        string _UNIT7_AVAILABILITYSTATE;
        string _UNIT7_INTERLOCKSTATE;
        string _UNIT7_MOVESTATE;
        string _UNIT7_RUNSTATE;
        string _UNIT7_FRONTSTATE;
        string _UNIT7_REARSTATE;
        string _UNIT7_PP_SPLSTATE;
        string _UNIT7_REASONCODE;
        string _UNIT7_DESCRIPTION;

        string _UNIT8_AVAILABILITYSTATE;
        string _UNIT8_INTERLOCKSTATE;
        string _UNIT8_MOVESTATE;
        string _UNIT8_RUNSTATE;
        string _UNIT8_FRONTSTATE;
        string _UNIT8_REARSTATE;
        string _UNIT8_PP_SPLSTATE;
        string _UNIT8_REASONCODE;
        string _UNIT8_DESCRIPTION;

        public S1F6(SECSDriverBase driver)
            : base(driver)
        {
            Stream = 1; Function = 6;

            _CRST = Convert.ToInt32(CommonData.Instance.HOST_MODE).ToString();
            _AVAILABILITYSTATE = DataManager.Instance.GET_STRING_DATA("iPLC1.EQStatus.Availability", out bResult);
            _INTERLOCKSTATE = DataManager.Instance.GET_STRING_DATA("iPLC1.EQStatus.Interlock", out bResult);
            _MOVESTATE = DataManager.Instance.GET_STRING_DATA("iPLC1.EQStatus.Move", out bResult);
            _RUNSTATE = DataManager.Instance.GET_STRING_DATA("iPLC1.EQStatus.Run", out bResult);
            _FRONTSTATE = DataManager.Instance.GET_STRING_DATA("iPLC1.EQStatus.Front", out bResult);
            _REARSTATE = DataManager.Instance.GET_STRING_DATA("iPLC1.EQStatus.Rear", out bResult);
            _PP_SPLSTATE = DataManager.Instance.GET_STRING_DATA("iPLC1.EQStatus.PP_SPL", out bResult);
            _REASONCODE = DataManager.Instance.GET_STRING_DATA("iPLC1.Availability.ReasonCode", out bResult);
            _DESCRIPTION = DataManager.Instance.GET_STRING_DATA("iPLC1.Availability.Description", out bResult);

            _UNIT1_AVAILABILITYSTATE = DataManager.Instance.GET_STRING_DATA("iPLC1.EQStatus.Availability", out bResult);
            _UNIT1_INTERLOCKSTATE = DataManager.Instance.GET_STRING_DATA("iPLC1.EQStatus.Interlock", out bResult);
            _UNIT1_MOVESTATE = DataManager.Instance.GET_STRING_DATA("iPLC1.EQStatus.Move", out bResult);
            _UNIT1_RUNSTATE = DataManager.Instance.GET_STRING_DATA("iPLC1.EQStatus.Run", out bResult);
            _UNIT1_FRONTSTATE = DataManager.Instance.GET_STRING_DATA("iPLC1.EQStatus.Front", out bResult);
            _UNIT1_REARSTATE = DataManager.Instance.GET_STRING_DATA("iPLC1.EQStatus.Rear", out bResult);
            _UNIT1_PP_SPLSTATE = DataManager.Instance.GET_STRING_DATA("iPLC1.EQStatus.PP_SPL", out bResult);
            _UNIT1_REASONCODE = DataManager.Instance.GET_STRING_DATA("iPLC1.Availability.ReasonCode", out bResult);
            _UNIT1_DESCRIPTION = DataManager.Instance.GET_STRING_DATA("iPLC1.Availability.Description", out bResult);

            _UNIT2_AVAILABILITYSTATE = DataManager.Instance.GET_STRING_DATA("iPLC2.EQStatus.Availability", out bResult);
            _UNIT2_INTERLOCKSTATE = DataManager.Instance.GET_STRING_DATA("iPLC2.EQStatus.Interlock", out bResult);
            _UNIT2_MOVESTATE = DataManager.Instance.GET_STRING_DATA("iPLC2.EQStatus.Move", out bResult);
            _UNIT2_RUNSTATE = DataManager.Instance.GET_STRING_DATA("iPLC2.EQStatus.Run", out bResult);
            _UNIT2_FRONTSTATE = DataManager.Instance.GET_STRING_DATA("iPLC2.EQStatus.Front", out bResult);
            _UNIT2_REARSTATE = DataManager.Instance.GET_STRING_DATA("iPLC2.EQStatus.Rear", out bResult);
            _UNIT2_PP_SPLSTATE = DataManager.Instance.GET_STRING_DATA("iPLC2.EQStatus.PP_SPL", out bResult);
            _UNIT2_REASONCODE = DataManager.Instance.GET_STRING_DATA("iPLC2.Availability.ReasonCode", out bResult);
            _UNIT2_DESCRIPTION = DataManager.Instance.GET_STRING_DATA("iPLC2.Availability.Description", out bResult);

            _UNIT3_AVAILABILITYSTATE = DataManager.Instance.GET_STRING_DATA("iPLC3.EQStatus.Availability", out bResult);
            _UNIT3_INTERLOCKSTATE = DataManager.Instance.GET_STRING_DATA("iPLC3.EQStatus.Interlock", out bResult);
            _UNIT3_MOVESTATE = DataManager.Instance.GET_STRING_DATA("iPLC3.EQStatus.Move", out bResult);
            _UNIT3_RUNSTATE = DataManager.Instance.GET_STRING_DATA("iPLC3.EQStatus.Run", out bResult);
            _UNIT3_FRONTSTATE = DataManager.Instance.GET_STRING_DATA("iPLC3.EQStatus.Front", out bResult);
            _UNIT3_REARSTATE = DataManager.Instance.GET_STRING_DATA("iPLC3.EQStatus.Rear", out bResult);
            _UNIT3_PP_SPLSTATE = DataManager.Instance.GET_STRING_DATA("iPLC3.EQStatus.PP_SPL", out bResult);
            _UNIT3_REASONCODE = DataManager.Instance.GET_STRING_DATA("iPLC3.Availability.ReasonCode", out bResult);
            _UNIT3_DESCRIPTION = DataManager.Instance.GET_STRING_DATA("iPLC3.Availability.Description", out bResult);

            _UNIT4_AVAILABILITYSTATE = DataManager.Instance.GET_STRING_DATA("iPLC4.EQStatus.Availability", out bResult);
            _UNIT4_INTERLOCKSTATE = DataManager.Instance.GET_STRING_DATA("iPLC4.EQStatus.Interlock", out bResult);
            _UNIT4_MOVESTATE = DataManager.Instance.GET_STRING_DATA("iPLC4.EQStatus.Move", out bResult);
            _UNIT4_RUNSTATE = DataManager.Instance.GET_STRING_DATA("iPLC4.EQStatus.Run", out bResult);
            _UNIT4_FRONTSTATE = DataManager.Instance.GET_STRING_DATA("iPLC4.EQStatus.Front", out bResult);
            _UNIT4_REARSTATE = DataManager.Instance.GET_STRING_DATA("iPLC4.EQStatus.Rear", out bResult);
            _UNIT4_PP_SPLSTATE = DataManager.Instance.GET_STRING_DATA("iPLC4.EQStatus.PP_SPL", out bResult);
            _UNIT4_REASONCODE = DataManager.Instance.GET_STRING_DATA("iPLC4.Availability.ReasonCode", out bResult);
            _UNIT4_DESCRIPTION = DataManager.Instance.GET_STRING_DATA("iPLC4.Availability.Description", out bResult);

            _UNIT5_AVAILABILITYSTATE = DataManager.Instance.GET_STRING_DATA("iPLC5.EQStatus.Availability", out bResult);
            _UNIT5_INTERLOCKSTATE = DataManager.Instance.GET_STRING_DATA("iPLC5.EQStatus.Interlock", out bResult);
            _UNIT5_MOVESTATE = DataManager.Instance.GET_STRING_DATA("iPLC5.EQStatus.Move", out bResult);
            _UNIT5_RUNSTATE = DataManager.Instance.GET_STRING_DATA("iPLC5.EQStatus.Run", out bResult);
            _UNIT5_FRONTSTATE = DataManager.Instance.GET_STRING_DATA("iPLC5.EQStatus.Front", out bResult);
            _UNIT5_REARSTATE = DataManager.Instance.GET_STRING_DATA("iPLC5.EQStatus.Rear", out bResult);
            _UNIT5_PP_SPLSTATE = DataManager.Instance.GET_STRING_DATA("iPLC5.EQStatus.PP_SPL", out bResult);
            _UNIT5_REASONCODE = DataManager.Instance.GET_STRING_DATA("iPLC5.Availability.ReasonCode", out bResult);
            _UNIT5_DESCRIPTION = DataManager.Instance.GET_STRING_DATA("iPLC5.Availability.Description", out bResult);

            _UNIT6_AVAILABILITYSTATE = DataManager.Instance.GET_STRING_DATA("iPLC6.EQStatus.Availability", out bResult);
            _UNIT6_INTERLOCKSTATE = DataManager.Instance.GET_STRING_DATA("iPLC6.EQStatus.Interlock", out bResult);
            _UNIT6_MOVESTATE = DataManager.Instance.GET_STRING_DATA("iPLC6.EQStatus.Move", out bResult);
            _UNIT6_RUNSTATE = DataManager.Instance.GET_STRING_DATA("iPLC6.EQStatus.Run", out bResult);
            _UNIT6_FRONTSTATE = DataManager.Instance.GET_STRING_DATA("iPLC6.EQStatus.Front", out bResult);
            _UNIT6_REARSTATE = DataManager.Instance.GET_STRING_DATA("iPLC6.EQStatus.Rear", out bResult);
            _UNIT6_PP_SPLSTATE = DataManager.Instance.GET_STRING_DATA("iPLC6.EQStatus.PP_SPL", out bResult);
            _UNIT6_REASONCODE = DataManager.Instance.GET_STRING_DATA("iPLC6.Availability.ReasonCode", out bResult);
            _UNIT6_DESCRIPTION = DataManager.Instance.GET_STRING_DATA("iPLC6.Availability.Description", out bResult);

            _UNIT7_AVAILABILITYSTATE = DataManager.Instance.GET_STRING_DATA("iPLC7.EQStatus.Availability", out bResult);
            _UNIT7_INTERLOCKSTATE = DataManager.Instance.GET_STRING_DATA("iPLC7.EQStatus.Interlock", out bResult);
            _UNIT7_MOVESTATE = DataManager.Instance.GET_STRING_DATA("iPLC7.EQStatus.Move", out bResult);
            _UNIT7_RUNSTATE = DataManager.Instance.GET_STRING_DATA("iPLC7.EQStatus.Run", out bResult);
            _UNIT7_FRONTSTATE = DataManager.Instance.GET_STRING_DATA("iPLC7.EQStatus.Front", out bResult);
            _UNIT7_REARSTATE = DataManager.Instance.GET_STRING_DATA("iPLC7.EQStatus.Rear", out bResult);
            _UNIT7_PP_SPLSTATE = DataManager.Instance.GET_STRING_DATA("iPLC7.EQStatus.PP_SPL", out bResult);
            _UNIT7_REASONCODE = DataManager.Instance.GET_STRING_DATA("iPLC7.Availability.ReasonCode", out bResult);
            _UNIT7_DESCRIPTION = DataManager.Instance.GET_STRING_DATA("iPLC7.Availability.Description", out bResult);

            _UNIT8_AVAILABILITYSTATE = DataManager.Instance.GET_STRING_DATA("iPLC8.EQStatus.Availability", out bResult);
            _UNIT8_INTERLOCKSTATE = DataManager.Instance.GET_STRING_DATA("iPLC8.EQStatus.Interlock", out bResult);
            _UNIT8_MOVESTATE = DataManager.Instance.GET_STRING_DATA("iPLC8.EQStatus.Move", out bResult);
            _UNIT8_RUNSTATE = DataManager.Instance.GET_STRING_DATA("iPLC8.EQStatus.Run", out bResult);
            _UNIT8_FRONTSTATE = DataManager.Instance.GET_STRING_DATA("iPLC8.EQStatus.Front", out bResult);
            _UNIT8_REARSTATE = DataManager.Instance.GET_STRING_DATA("iPLC8.EQStatus.Rear", out bResult);
            _UNIT8_PP_SPLSTATE = DataManager.Instance.GET_STRING_DATA("iPLC8.EQStatus.PP_SPL", out bResult);
            _UNIT8_REASONCODE = DataManager.Instance.GET_STRING_DATA("iPLC8.Availability.ReasonCode", out bResult);
            _UNIT8_DESCRIPTION = DataManager.Instance.GET_STRING_DATA("iPLC8.Availability.Description", out bResult);
        }

        private void SFCD_1()
        {
            SecsMessage reply = new SecsMessage(Stream, Function, _systembyte)
            {
                WaitBit = false
            };
            reply.AddList(2);                                                              //L2  SFCD Set
            {
                reply.AddAscii(_sfcd);                                                         //A1  State Form Code

                if (_eqpid != CommonData.Instance.EQP_SETTINGS.EQPID) // EQP Name Fail
                {
                    reply.AddList(0);
                }
                else
                {
                    reply.AddList(_eqpListCount);                                                                         //Ln  EQP State Info List
                    {
                        for (int i = 0; i < _eqpListCount; i++)
                        {
                            reply.AddList(2);                                                                                //L2  EQP State Info
                            {
                                reply.AddList(2);                                                                                //L2  EQP Control State Set
                                {
                                    reply.AddAscii(AppUtil.ToAscii(_eqpid, gDefine.DEF_EQPID_SIZE));                                 //A40 HOST REQ Equipment ID
                                    reply.AddAscii(_CRST);                                                                           //A1  Online Control State
                                }

                                reply.AddList(9);                                                                                //L9  EQP State Set
                                {
                                    reply.AddAscii(_AVAILABILITYSTATE);                                                              //A1 EQ Avilability State Info
                                    reply.AddAscii(_INTERLOCKSTATE);                                                                 //A1 Interlock Avilability State Info
                                    reply.AddAscii(_MOVESTATE);                                                                      //A1 EQ Move State Info
                                    reply.AddAscii(_RUNSTATE);                                                                       //A1 Cell existence/nonexistence Check
                                    reply.AddAscii(_FRONTSTATE);                                                                     //A1 Upper EQ Processing State
                                    reply.AddAscii(_REARSTATE);                                                                      //A1 Lower EQ Processing State
                                    reply.AddAscii(_PP_SPLSTATE);                                                                    //A1 Sample Run-Normal Run State
                                    reply.AddAscii(AppUtil.ToAscii(_REASONCODE, gDefine.DEF_REASONCODE_SIZE));                       //A20 Code Info
                                    reply.AddAscii(AppUtil.ToAscii(_DESCRIPTION, gDefine.DEF_DESCRIPTION_SIZE));                     //A40 EQ Description
                                }
                            }
                        }
                    }
                }
            }

            SecsDriver.WriteLogAndSendMessage(reply, "");

        }

        private void SFCD_2()
        {

            SecsMessage reply = new SecsMessage(Stream, Function, _systembyte)
            {
                WaitBit = false
            };
            int unitListCount = CommonData.Instance.MODULE_SETTINGS.Count;

            reply.AddList(2);                                                              //L2  SFCD Set
            {
                reply.AddAscii(_sfcd);                                                         //A1  State Form Code

                if (_eqpid != CommonData.Instance.EQP_SETTINGS.EQPID) // EQP Name Fail
                {
                    reply.AddList(0);
                }
                else
                {
                    reply.AddList(_eqpListCount);                                                                         //Ln  EQP State Info List
                    {
                        for (int i = 0; i < _eqpListCount; i++)
                        {
                            reply.AddList(3);                                                                                //L3  EQP State Info
                            {
                                reply.AddList(2);                                                                                //L2  EQP Control State Set
                                {
                                    reply.AddAscii(AppUtil.ToAscii(_eqpid, gDefine.DEF_EQPID_SIZE));                                 //A40 HOST REQ Equipment ID
                                    reply.AddAscii(_CRST);                                                                           //A1  Online Control State
                                }

                                reply.AddList(9);                                                                                //L9  EQP State Set
                                {
                                    reply.AddAscii(_AVAILABILITYSTATE);                                                              //A1 EQ Avilability State Info
                                    reply.AddAscii(_INTERLOCKSTATE);                                                                 //A1 Interlock Avilability State Info
                                    reply.AddAscii(_MOVESTATE);                                                                      //A1 EQ Move State Info
                                    reply.AddAscii(_RUNSTATE);                                                                       //A1 Cell existence/nonexistence Check
                                    reply.AddAscii(_FRONTSTATE);                                                                     //A1 Upper EQ Processing State
                                    reply.AddAscii(_REARSTATE);                                                                      //A1 Lower EQ Processing State
                                    reply.AddAscii(_PP_SPLSTATE);                                                                    //A1 Sample Run-Normal Run State
                                    reply.AddAscii(AppUtil.ToAscii(_REASONCODE, gDefine.DEF_REASONCODE_SIZE));                       //A20 Code Info
                                    reply.AddAscii(AppUtil.ToAscii(_DESCRIPTION, gDefine.DEF_DESCRIPTION_SIZE));                     //A40 EQ Description
                                }

                                if (unitListCount == 0)
                                {
                                    reply.AddList(unitListCount);
                                }
                                else
                                {
                                    reply.AddList(unitListCount);
                                    {
                                        for (int j = 0; j < unitListCount; j++)
                                        {
                                            MODULE module = CommonData.Instance.MODULE_SETTINGS[j];

                                            reply.AddList(2);
                                            {
                                                reply.AddAscii(AppUtil.ToAscii(module.UNIT_ID, 40));                                        //A40 UNIT ID
                                                reply.AddList(9);
                                                {
                                                    reply.AddAscii(DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.EQStatus.Availability", module.MODULE_NAME), out bResult));                                                              //A1 EQ Avilability State Info
                                                    reply.AddAscii(DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.EQStatus.Interlock", module.MODULE_NAME), out bResult));                                                                 //A1 Interlock Avilability State Info
                                                    reply.AddAscii(DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.EQStatus.Move", module.MODULE_NAME), out bResult));                                                                      //A1 EQ Move State Info
                                                    reply.AddAscii(DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.EQStatus.Run", module.MODULE_NAME), out bResult));                                                                       //A1 Cell existence/nonexistence Check
                                                    reply.AddAscii(DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.EQStatus.Front", module.MODULE_NAME), out bResult));                                                                     //A1 Upper EQ Processing State
                                                    reply.AddAscii(DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.EQStatus.Rear", module.MODULE_NAME), out bResult));                                                                      //A1 Lower EQ Processing State
                                                    reply.AddAscii(DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.EQStatus.PP_SPL", module.MODULE_NAME), out bResult));                                                                    //A1 Sample Run-Normal Run State
                                                    reply.AddAscii(AppUtil.ToAscii(DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.Availability.ReasonCode", module.MODULE_NAME), out bResult), gDefine.DEF_REASONCODE_SIZE));                       //A20 Code Info
                                                    reply.AddAscii(AppUtil.ToAscii(DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.Availability.Description", module.MODULE_NAME), out bResult), gDefine.DEF_DESCRIPTION_SIZE));                     //A40 EQ Description
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            SecsDriver.WriteLogAndSendMessage(reply, "");
        }

        private void SFCD_3()
        {
            int materialPortCount = 0;

            SecsMessage reply = new SecsMessage(Stream, Function, _systembyte)
            {
                WaitBit = false
            };
            List<MODULE> moduleList = CommonData.Instance.MODULE_SETTINGS.Where(m => m.MATERIAL_PORT_COUNT > 0).ToList();

            foreach (MODULE m in moduleList)
            {
                materialPortCount += m.MATERIAL_PORT_COUNT;
            }

            reply.AddList(2);                                                                   //L2  SFCD Set
            {
                reply.AddAscii(_sfcd);

                if (_eqpid != CommonData.Instance.EQP_SETTINGS.EQPID) // EQP Name Fail
                {
                    reply.AddList(0);
                }
                else
                {
                    reply.AddList(_eqpListCount);
                    {
                        for (int i = 0; i < _eqpListCount; i++)
                        {
                            reply.AddList(3);
                            {
                                reply.AddList(2);
                                {
                                    reply.AddAscii(AppUtil.ToAscii(_eqpid, gDefine.DEF_EQPID_SIZE));                           //A40 HOST REQ Equipment ID  
                                    reply.AddAscii(_CRST);
                                }
                                reply.AddList(9);
                                {
                                    reply.AddAscii(_AVAILABILITYSTATE);                                                        //A1  EQ Avilability State Info  
                                    reply.AddAscii(_INTERLOCKSTATE);                                                           //A1  Interlock Avilability State Info  
                                    reply.AddAscii(_MOVESTATE);                                                                //A1  EQ Move State Info  
                                    reply.AddAscii(_RUNSTATE);                                                                 //A1  Cell existence/nonexistence Check 
                                    reply.AddAscii(_FRONTSTATE);                                                               //A1  Upper EQ Processing State  
                                    reply.AddAscii(_REARSTATE);                                                                //A1  Lower EQ Processing State  
                                    reply.AddAscii(_PP_SPLSTATE);                                                              //A1  Sample Run-Normal Run State
                                    reply.AddAscii(AppUtil.ToAscii(_REASONCODE, gDefine.DEF_REASONCODE_SIZE));                 //A20 State Reason Code
                                    reply.AddAscii(AppUtil.ToAscii(_DESCRIPTION, gDefine.DEF_DESCRIPTION_SIZE));               //A40 State Description
                                }

                                if (materialPortCount == 0)
                                {
                                    reply.AddList(0);
                                }
                                else
                                {
                                    reply.AddList(materialPortCount);
                                    {
                                        if (materialPortCount != 0)
                                        {
                                            foreach (MODULE m in moduleList)
                                            {
                                                for (int k = 1; k <= m.MATERIAL_PORT_COUNT; k++)
                                                {
                                                    string group_name = "MaterialPortState";
                                                    string material_id_tag_name = string.Format("MaterialID{0}", k);
                                                    string material_type_tag_name = string.Format("MaterialType{0}", k);
                                                    string material_st_tag_name = string.Format("MaterialST{0}", k);
                                                    string material_portid_tag_name = string.Format("MaterialMLN{0}", k);
                                                    string material_usage_tag_name = string.Format("MaterialUsage{0}", k);

                                                    string material_id = TagNameHelper.MakeTagName(eDirection.IN, m.MODULE_NAME, group_name, material_id_tag_name);
                                                    string material_type = TagNameHelper.MakeTagName(eDirection.IN, m.MODULE_NAME, group_name, material_type_tag_name);
                                                    string material_st = TagNameHelper.MakeTagName(eDirection.IN, m.MODULE_NAME, group_name, material_st_tag_name);
                                                    string material_portid = TagNameHelper.MakeTagName(eDirection.IN, m.MODULE_NAME, group_name, material_portid_tag_name);
                                                    string material_usage = TagNameHelper.MakeTagName(eDirection.IN, m.MODULE_NAME, group_name, material_usage_tag_name);

                                                    reply.AddList(5);
                                                    {
                                                        reply.AddAscii(DataManager.Instance.GET_STRING_DATA(material_id, out bResult));
                                                        reply.AddAscii(DataManager.Instance.GET_STRING_DATA(material_type, out bResult));
                                                        reply.AddAscii(DataManager.Instance.GET_STRING_DATA(material_st, out bResult));
                                                        reply.AddAscii(DataManager.Instance.GET_INT_DATA(material_portid, out bResult).ToString());
                                                        reply.AddAscii(DataManager.Instance.GET_INT_DATA(material_usage, out bResult).ToString());
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            SecsDriver.WriteLogAndSendMessage(reply, "");
        }

        private void SFCD_4()
        {
            SecsMessage reply = new SecsMessage(Stream, Function, _systembyte)
            {
                WaitBit = false
            };
            bool bResult;
            string sPORTNO = "";
            string sPORTAVAILABLESTATE = "";
            string sPORTACCESSMODE = "";
            string sPORTTRANSFERSTATE = "";
            string sPORTPROCESSINGSTATE = "";
            string sREASONCODE = "";
            string sDESCRIPTION = "";

            int agvPortCount = 0;

            List<MODULE> moduleList = CommonData.Instance.MODULE_SETTINGS.Where(m => m.AGV_PORT_COUNT > 0).ToList();

            foreach (MODULE m in moduleList)
            {
                agvPortCount += m.AGV_PORT_COUNT;
            }

            reply.AddList(2);                                                                            //L2  SFCD Set
            {
                reply.AddAscii(_sfcd);

                if (_eqpid != CommonData.Instance.EQP_SETTINGS.EQPID)
                {
                    reply.AddList(0);
                }
                else
                {
                    reply.AddList(_eqpListCount);
                    {
                        for (int i = 0; i < _eqpListCount; i++)
                        {
                            reply.AddList(2);
                            {
                                reply.AddList(2);
                                {
                                    reply.AddAscii(AppUtil.ToAscii(_eqpid, gDefine.DEF_EQPID_SIZE));                                //A40 HOST REQ EQPID
                                    reply.AddAscii(_CRST);
                                }


                                reply.AddList(agvPortCount);
                                {
                                    foreach (MODULE m in moduleList)
                                    {
                                        for (int portNum = 1; portNum <= m.AGV_PORT_COUNT; portNum++)
                                        {
                                            sPORTNO = DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.{1}.{2}{3}", m.MODULE_NAME, "PortStatus", "PortNo", portNum), out bResult);
                                            sPORTAVAILABLESTATE = DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.{1}.{2}{3}", m.MODULE_NAME, "PortStatus", "PortAvailState", portNum), out bResult);
                                            sPORTACCESSMODE = DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.{1}.{2}{3}", m.MODULE_NAME, "PortStatus", "PortAccessMode", portNum), out bResult);
                                            sPORTTRANSFERSTATE = DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.{1}.{2}{3}", m.MODULE_NAME, "PortStatus", "PortTransferState", portNum), out bResult);
                                            sPORTPROCESSINGSTATE = DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.{1}.{2}{3}", m.MODULE_NAME, "PortStatus", "PortProcessingState", portNum), out bResult);
                                            sREASONCODE = DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.{1}.{2}", m.MODULE_NAME, "Availability", "ReasonCode"), out bResult);
                                            sDESCRIPTION = DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.{1}.{2}", m.MODULE_NAME, "Availability", "Description"), out bResult);

                                            reply.AddList(7);
                                            {
                                                reply.AddAscii(sPORTNO);                                                                        //A1  �비 Port�의 No
                                                reply.AddAscii(sPORTAVAILABLESTATE);                                                                //A1  Port Avilability State Info
                                                reply.AddAscii(sPORTACCESSMODE);                                                                //A1  Port Access State Info
                                                reply.AddAscii(sPORTTRANSFERSTATE);                                                             //A1  Port Transfer State Info
                                                reply.AddAscii(sPORTPROCESSINGSTATE);                                                           //A1  Port Processing State Info
                                                reply.AddAscii(sREASONCODE);                                                                           //A20 State Reason Code
                                                reply.AddAscii(sDESCRIPTION);
                                            }
                                        }
                                    }

                                }

                            }
                        }
                    }
                }

            }

            SecsDriver.WriteLogAndSendMessage(reply, "");
        }

        private void DEFAULT()
        {
            SecsMessage reply = new SecsMessage(Stream, Function, _systembyte);

            reply.AddList(2);
            {
                reply.AddAscii(_sfcd);
                reply.AddList(0);
            }

            SecsDriver.WriteLogAndSendMessage(reply, "");
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            SecsMessage primaryMessage = obj as SecsMessage;

            string data = string.Empty;
            _systembyte = primaryMessage.SystemByte;
            int list = primaryMessage.GetItem().GetList();
            {
                _sfcd = primaryMessage.GetItem().GetAscii();
                _eqpListCount = primaryMessage.GetItem().GetList();
                if (_eqpListCount == 0) _eqpListCount = 1;
                else
                {
                    _eqpid = primaryMessage.GetItem().GetAscii().Trim();
                }
            }

            switch (_sfcd)
            {
                case "1":
                    {
                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "STATE", "S1F5", "1", "Equipment State Request", null);
                        SFCD_1();
                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "STATE", "S1F6", "1", "Equipment State Send", null);
                    }
                    break;
                case "2":
                    {
                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "STATE", "S1F5", "2", "Unit State Request", null);
                        SFCD_2();
                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "STATE", "S1F6", "2", "Unit State Request", null);
                    }
                    break;
                case "3":
                    {
                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "STATE", "S1F5", "3", "Material State Request", null);
                        SFCD_3();
                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "STATE", "S1F6", "3", "Material State Request", null);
                    }
                    break;
                case "4":
                    {
                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "STATE", "S1F5", "4", "Port State Request", null);
                        SFCD_4();
                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "STATE", "S1F6", "4", "Port State Request", null);
                    }
                    break;
                default:
                    {
                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "H->E", "STATE", "S1F5", "100", "Undefined SFCD Request", null);
                        DEFAULT();
                        CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "STATE", "S1F6", "100", "Undefined SFCD Request", null);
                    }
                    break;
            }
        }
    }
}
