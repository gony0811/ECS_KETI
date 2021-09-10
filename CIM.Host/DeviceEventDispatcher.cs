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

namespace CIM.Host
{
    public partial class SECSDriver : SECSDriverBase
    {
        private void DataChanged(object sender, DataChangedEventHandlerArgs args)
        {
            Data data = args.Data;

            string[] tagSplit = data.Name.Split('.');

            string module = tagSplit[0].Remove(0, 1);

            if (tagSplit[1] != "ALARM" && tagSplit[2] != "AliveBit")
                LogHelper.Instance.BizLog.DebugFormat("[DataChanged] TagName : {0} (ADDRESS : {1} | Data : {2}", data.Name, data.Config1, data.StringValue);

            if (data.Direction == eDirection.OUT) return;

            if (/*data.Direction == eDirection.IN && */data.Group == "ALARM"/* &&tagSplit[1] == "Alarm"*/)
            {
                return;
                //AlarmManager.Instance.OnAlarmChanged(data);
                //List<ALARM> Occuredlarms = new List<ALARM>();
                //Occuredlarms = AlarmManager.Instance.GetCurrentOccurredAlarms();

                //AlarmManager.Instance.UpdateAlarmData(data, out Occuredlarms);
            }


            INNO6.IO.Interface.eDevMode devMode = DataManager.Instance.IsDeviceMode(data.DriverName);

            if (devMode != INNO6.IO.Interface.eDevMode.CONNECT && devMode != INNO6.IO.Interface.eDevMode.SIMULATE) return;
            if (!_secsDriver.IsStarted() || !_secsDriver.IsInitialized()) return;

            if (data.Direction == eDirection.IN && tagSplit[1] == "Reply" && (int)data.Value == 1)
            {
                StringBuilder sb = new StringBuilder();
                string direction = "Send";
                sb.AppendFormat("o{0}.{1}.{2}", module, direction, tagSplit[2]);

                DataManager.Instance.SET_INT_DATA(sb.ToString(), 0);
                return;
            }

            if (data.Direction == eDirection.IN && tagSplit[1] == "Send" && (int)data.Value == 0)
            {
                StringBuilder sb = new StringBuilder();
                string direction = "Reply";
                sb.AppendFormat("o{0}.{1}.{2}", module, direction, tagSplit[2]);

                DataManager.Instance.SET_INT_DATA(sb.ToString(), 0);

                if (tagSplit[2] == "AliveBit")
                {
                    //LogHelper.Instance.BizLog.DebugFormat("[{0}]ALIVE OFF", module, "", "");
                    CommonData.Instance.OnAliveBitSignalChanged(module, "PLC_Alive", false);
                    CommonData.Instance.OnAliveBitSignalChanged(module, "CIM_Alive", false);
                }
                return;
            }

            if (data.Direction == eDirection.IN && tagSplit[1] == "EQStatus")
            {
                //new S6F11_EQPStatusChange_CEID101(this).DoWork(tagSplit[0], data);
                new S6F11_UnitStatusChange_CEID102(this).DoWork(tagSplit[0], data);
                return;
            }

            if (data.Direction == eDirection.IN && tagSplit[1] == "Send" && (int)data.Value == 1)
            {
                if ((data.Name != "iPLC1.Send.GetAttributeRequest") && (data.Name != "iPLC2.Send.GetAttributeRequest") && (data.Name != "iPLC3.Send.GetAttributeRequest") && (data.Name != "iPLC4.Send.GetAttributeRequest") && (data.Name != "iPLC5.Send.GetAttributeRequest") && (data.Name != "iPLC6.Send.GetAttributeRequest") && (data.Name != "iPLC7.Send.GetAttributeRequest") && (data.Name != "iPLC8.Send.GetAttributeRequest"))
                {
                    StringBuilder sb = new StringBuilder();
                    string direction = "Reply";
                    sb.AppendFormat("o{0}.{1}.{2}", module, direction, tagSplit[2]);
                    DataManager.Instance.SET_INT_DATA(sb.ToString(), 1);
                }

                if (tagSplit[2] == "AliveBit")
                {
                    //LogHelper.Instance.BizLog.DebugFormat("[{0}]ALIVE ON", module, "", "");
                    CommonData.Instance.OnAliveBitSignalChanged(module, "PLC_Alive", true);
                    CommonData.Instance.OnAliveBitSignalChanged(module, "CIM_Alive", true);
                }

                switch (data.Name)
                {
                    case "iPLC1.Send.ControlStateChange":
                        {
                            new S6F11_ControlStateChange(this).DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC1.Send.PPIDChange":
                        {
                            new S6F11_EquipmentPPIDChange_CEID107(this).DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC1.Send.ParameterChange":
                    case "iPLC2.Send.ParameterChange":
                    case "iPLC3.Send.ParameterChange":
                    case "iPLC4.Send.ParameterChange":
                    case "iPLC5.Send.ParameterChange":
                    case "iPLC6.Send.ParameterChange":
                    case "iPLC7.Send.ParameterChange":
                    case "iPLC8.Send.ParameterChange":
                        {
                            new S7F107(this).DoWork(tagSplit[0], data);
                        }
                        break
                            ;
                    case "iPLC1.Send.MaterialAssembleProcess1_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "1", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialAssembleProcess1_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "1", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialAssembleProcess2_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "2", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialAssembleProcess2_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "2", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialAssembleProcess3_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "3", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialAssembleProcess3_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "3", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialAssembleProcess1_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "1", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialAssembleProcess1_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "1", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialAssembleProcess2_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "2", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialAssembleProcess2_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "2", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialAssembleProcess3_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "3", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialAssembleProcess3_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "3", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialAssembleProcess1_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "1", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialAssembleProcess1_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "1", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialAssembleProcess2_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "2", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialAssembleProcess2_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "2", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialAssembleProcess3_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "3", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialAssembleProcess3_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "3", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialAssembleProcess1_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "1", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialAssembleProcess1_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "1", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialAssembleProcess2_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "2", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialAssembleProcess2_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "2", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialAssembleProcess3_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "3", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialAssembleProcess3_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "3", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialAssembleProcess1_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "1", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialAssembleProcess1_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "1", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialAssembleProcess2_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "2", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialAssembleProcess2_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "2", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialAssembleProcess3_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "3", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialAssembleProcess3_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "3", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialAssembleProcess1_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "1", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialAssembleProcess1_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "1", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialAssembleProcess2_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "2", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialAssembleProcess2_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "2", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialAssembleProcess3_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "3", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialAssembleProcess3_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "3", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialAssembleProcess1_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "1", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialAssembleProcess1_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "1", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialAssembleProcess2_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "2", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialAssembleProcess2_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "2", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialAssembleProcess3_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "3", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialAssembleProcess3_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "3", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialAssembleProcess1_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "1", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialAssembleProcess1_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "1", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialAssembleProcess2_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "2", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialAssembleProcess2_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "2", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialAssembleProcess3_1":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "3", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialAssembleProcess3_2":
                        {
                            new S6F11_MaterialAssembleProcess_CEID215(this, "3", "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialNGProcess1_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "1", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialNGProcess2_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "2", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialNGProcess3_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "3", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialNGProcess1_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "1", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialNGProcess2_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "2", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialNGProcess3_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "3", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialNGProcess1_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "1", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialNGProcess2_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "2", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialNGProcess3_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "3", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialNGProcess1_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "1", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialNGProcess2_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "2", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialNGProcess3_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "3", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialNGProcess1_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "1", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialNGProcess2_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "2", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialNGProcess3_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "3", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialNGProcess1_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "1", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialNGProcess2_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "2", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialNGProcess3_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "3", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialNGProcess1_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "1", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialNGProcess2_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "2", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialNGProcess3_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "3", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialNGProcess1_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "1", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialNGProcess2_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "2", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialNGProcess3_1":
                        {
                            new S6F11_MaterialNGProcess_CEID222(this, "3", "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialKittingKittingCancel1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialKittingKittingCancel2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialWarning1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialWarning2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialShortage1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialShortage2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialLocationUpdate1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialLocationUpdate2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialSupplementRequest1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialSupplementRequest2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialSupplementComplete1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialSupplementComplete2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialKittingKittingCancel1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialKittingKittingCancel2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialWarning1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialWarning2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialShortage1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialShortage2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialLocationUpdate1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialLocationUpdate2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialSupplementRequest1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialSupplementRequest2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialSupplementComplete1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialSupplementComplete2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialKittingKittingCancel1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialKittingKittingCancel2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialWarning1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialWarning2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialShortage1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialShortage2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialLocationUpdate1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialLocationUpdate2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialSupplementRequest1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialSupplementRequest2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialSupplementComplete1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialSupplementComplete2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialKittingKittingCancel1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialKittingKittingCancel2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialWarning1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialWarning2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialShortage1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialShortage2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialLocationUpdate1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialLocationUpdate2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialSupplementRequest1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialSupplementRequest2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialSupplementComplete1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialSupplementComplete2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialKittingKittingCancel1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialKittingKittingCancel2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialWarning1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialWarning2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialShortage1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialShortage2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialLocationUpdate1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialLocationUpdate2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialSupplementRequest1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialSupplementRequest2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialSupplementComplete1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialSupplementComplete2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialKittingKittingCancel1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialKittingKittingCancel2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialWarning1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialWarning2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialShortage1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialShortage2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialLocationUpdate1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialLocationUpdate2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialSupplementRequest1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialSupplementRequest2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialSupplementComplete1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialSupplementComplete2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialKittingKittingCancel1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialKittingKittingCancel2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialWarning1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialWarning2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialShortage1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialShortage2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialLocationUpdate1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialLocationUpdate2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialSupplementRequest1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialSupplementRequest2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialSupplementComplete1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialSupplementComplete2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialKittingKittingCancel1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialKittingKittingCancel2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialWarning1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialWarning2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialShortage1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialShortage2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialLocationUpdate1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialLocationUpdate2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialSupplementRequest1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialSupplementRequest2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialSupplementComplete1":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialSupplementComplete2":
                        {
                            new S6F11_MaterialProcessChange_CEID211_225(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;

                    //CEID 254_255는 WORD로 설비상태변경으로 없음

                    case "iPLC1.Send.CarrierProcessChangeLoader":
                        {
                            new S6F11_CarrierProcessChange_Loader_CEID256_262(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.CarrierProcessChangeUnloader":
                        {
                            new S6F11_CarrierProcessChange_Unloader_CEID256_262(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.CarrierProcessChangeLoader":
                        {
                            new S6F11_CarrierProcessChange_Loader_CEID256_262(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.CarrierProcessChangeUnloader":
                        {
                            new S6F11_CarrierProcessChange_Unloader_CEID256_262(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.CarrierProcessChangeLoader":
                        {
                            new S6F11_CarrierProcessChange_Loader_CEID256_262(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.CarrierProcessChangeUnloader":
                        {
                            new S6F11_CarrierProcessChange_Unloader_CEID256_262(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.CarrierProcessChangeLoader":
                        {
                            new S6F11_CarrierProcessChange_Loader_CEID256_262(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.CarrierProcessChangeUnloader":
                        {
                            new S6F11_CarrierProcessChange_Unloader_CEID256_262(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.CarrierProcessChangeLoader":
                        {
                            new S6F11_CarrierProcessChange_Loader_CEID256_262(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.CarrierProcessChangeUnloader":
                        {
                            new S6F11_CarrierProcessChange_Unloader_CEID256_262(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.CarrierProcessChangeLoader":
                        {
                            new S6F11_CarrierProcessChange_Loader_CEID256_262(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.CarrierProcessChangeUnloader":
                        {
                            new S6F11_CarrierProcessChange_Unloader_CEID256_262(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.CarrierProcessChangeLoader":
                        {
                            new S6F11_CarrierProcessChange_Loader_CEID256_262(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.CarrierProcessChangeUnloader":
                        {
                            new S6F11_CarrierProcessChange_Unloader_CEID256_262(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.CarrierProcessChangeLoader":
                        {
                            new S6F11_CarrierProcessChange_Loader_CEID256_262(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.CarrierProcessChangeUnloader":
                        {
                            new S6F11_CarrierProcessChange_Unloader_CEID256_262(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.ProcessJobEvent1":
                        {
                            new S6F11_ProcessJob_CEID301_306(this, "1", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.ProcessJobEvent2":
                        {
                            new S6F11_ProcessJob_CEID301_306(this, "2", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.ProcessJobEvent1":
                        {
                            new S6F11_ProcessJob_CEID301_306(this, "1", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.ProcessJobEvent2":
                        {
                            new S6F11_ProcessJob_CEID301_306(this, "2", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.ProcessJobEvent1":
                        {
                            new S6F11_ProcessJob_CEID301_306(this, "1", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.ProcessJobEvent2":
                        {
                            new S6F11_ProcessJob_CEID301_306(this, "2", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.ProcessJobEvent1":
                        {
                            new S6F11_ProcessJob_CEID301_306(this, "1", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.ProcessJobEvent2":
                        {
                            new S6F11_ProcessJob_CEID301_306(this, "2", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.ProcessJobEvent1":
                        {
                            new S6F11_ProcessJob_CEID301_306(this, "1", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.ProcessJobEvent2":
                        {
                            new S6F11_ProcessJob_CEID301_306(this, "2", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.ProcessJobEvent1":
                        {
                            new S6F11_ProcessJob_CEID301_306(this, "1", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.ProcessJobEvent2":
                        {
                            new S6F11_ProcessJob_CEID301_306(this, "2", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.ProcessJobEvent1":
                        {
                            new S6F11_ProcessJob_CEID301_306(this, "1", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.ProcessJobEvent2":
                        {
                            new S6F11_ProcessJob_CEID301_306(this, "2", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.ProcessJobEvent1":
                        {
                            new S6F11_ProcessJob_CEID301_306(this, "1", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.ProcessJobEvent2":
                        {
                            new S6F11_ProcessJob_CEID301_306(this, "2", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.CassetteStateChangeEvent1_A":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "A").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.CassetteStateChangeEvent1_B":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.CassetteStateChangeEvent1_C":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "C").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.CassetteStateChangeEvent1_D":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "D").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.CassetteStateChangeEvent2_A":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "A").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.CassetteStateChangeEvent2_B":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.CassetteStateChangeEvent2_C":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "C").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.CassetteStateChangeEvent2_D":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "D").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.CassetteStateChangeEvent1_A":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "A").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.CassetteStateChangeEvent1_B":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.CassetteStateChangeEvent1_C":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "C").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.CassetteStateChangeEvent1_D":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "D").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.CassetteStateChangeEvent2_A":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "A").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.CassetteStateChangeEvent2_B":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.CassetteStateChangeEvent2_C":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "C").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.CassetteStateChangeEvent2_D":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "D").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.CassetteStateChangeEvent1_A":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "A").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.CassetteStateChangeEvent1_B":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.CassetteStateChangeEvent1_C":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "C").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.CassetteStateChangeEvent1_D":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "D").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.CassetteStateChangeEvent2_A":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "A").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.CassetteStateChangeEvent2_B":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.CassetteStateChangeEvent2_C":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "C").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.CassetteStateChangeEvent2_D":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "D").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.CassetteStateChangeEvent1_A":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "A").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.CassetteStateChangeEvent1_B":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.CassetteStateChangeEvent1_C":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "C").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.CassetteStateChangeEvent1_D":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "D").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.CassetteStateChangeEvent2_A":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "A").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.CassetteStateChangeEvent2_B":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.CassetteStateChangeEvent2_C":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "C").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.CassetteStateChangeEvent2_D":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "D").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.CassetteStateChangeEvent1_A":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "A").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.CassetteStateChangeEvent1_B":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.CassetteStateChangeEvent1_C":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "C").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.CassetteStateChangeEvent1_D":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "D").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.CassetteStateChangeEvent2_A":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "A").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.CassetteStateChangeEvent2_B":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.CassetteStateChangeEvent2_C":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "C").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.CassetteStateChangeEvent2_D":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "D").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.CassetteStateChangeEvent1_A":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "A").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.CassetteStateChangeEvent1_B":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.CassetteStateChangeEvent1_C":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "C").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.CassetteStateChangeEvent1_D":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "D").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.CassetteStateChangeEvent2_A":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "A").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.CassetteStateChangeEvent2_B":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.CassetteStateChangeEvent2_C":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "C").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.CassetteStateChangeEvent2_D":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "D").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.CassetteStateChangeEvent1_A":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "A").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.CassetteStateChangeEvent1_B":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.CassetteStateChangeEvent1_C":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "C").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.CassetteStateChangeEvent1_D":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "D").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.CassetteStateChangeEvent2_A":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "A").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.CassetteStateChangeEvent2_B":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.CassetteStateChangeEvent2_C":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "C").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.CassetteStateChangeEvent2_D":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "D").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.CassetteStateChangeEvent1_A":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "A").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.CassetteStateChangeEvent1_B":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.CassetteStateChangeEvent1_C":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "C").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.CassetteStateChangeEvent1_D":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "1", "D").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.CassetteStateChangeEvent2_A":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "A").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.CassetteStateChangeEvent2_B":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "B").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.CassetteStateChangeEvent2_C":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "C").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.CassetteStateChangeEvent2_D":
                        {
                            new S6F11_CassetteStatusChange_CEID350_356(this, "2", "D").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.CellStartEvent1":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.CellStartEvent2":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.CellStartEvent3":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "3").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.CellStartEvent1":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.CellStartEvent2":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.CellStartEvent3":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "3").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.CellStartEvent1":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.CellStartEvent2":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.CellStartEvent3":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "3").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.CellStartEvent1":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.CellStartEvent2":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.CellStartEvent3":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "3").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.CellStartEvent1":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.CellStartEvent2":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.CellStartEvent3":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "3").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.CellStartEvent1":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.CellStartEvent2":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.CellStartEvent3":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "3").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.CellStartEvent1":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.CellStartEvent2":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.CellStartEvent3":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "3").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.CellStartEvent1":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.CellStartEvent2":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.CellStartEvent3":
                        {
                            new S6F11_CellProcessStart_CEID401(this, "3").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.NormalDataCollection1":
                        {
                            new S6F11_NomalDataCollection_CEID403(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.NormalDataCollection2":
                        {
                            new S6F11_NomalDataCollection_CEID403(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.NormalDataCollection1":
                        {
                            new S6F11_NomalDataCollection_CEID403(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.NormalDataCollection2":
                        {
                            new S6F11_NomalDataCollection_CEID403(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.NormalDataCollection1":
                        {
                            new S6F11_NomalDataCollection_CEID403(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.NormalDataCollection2":
                        {
                            new S6F11_NomalDataCollection_CEID403(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.NormalDataCollection1":
                        {
                            new S6F11_NomalDataCollection_CEID403(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.NormalDataCollection2":
                        {
                            new S6F11_NomalDataCollection_CEID403(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.NormalDataCollection1":
                        {
                            new S6F11_NomalDataCollection_CEID403(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.NormalDataCollection2":
                        {
                            new S6F11_NomalDataCollection_CEID403(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.NormalDataCollection1":
                        {
                            new S6F11_NomalDataCollection_CEID403(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.NormalDataCollection2":
                        {
                            new S6F11_NomalDataCollection_CEID403(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.NormalDataCollection1":
                        {
                            new S6F11_NomalDataCollection_CEID403(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.NormalDataCollection2":
                        {
                            new S6F11_NomalDataCollection_CEID403(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.NormalDataCollection1":
                        {
                            new S6F11_NomalDataCollection_CEID403(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.NormalDataCollection2":
                        {
                            new S6F11_NomalDataCollection_CEID403(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.CellCompleteEvent1":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.CellCompleteEvent2":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.CellCompleteEvent3":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "3").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.CellCompleteEvent1":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.CellCompleteEvent2":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.CellCompleteEvent3":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "3").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.CellCompleteEvent1":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.CellCompleteEvent2":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.CellCompleteEvent3":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "3").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4Send.CellCompleteEvent1":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.CellCompleteEvent2":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.CellCompleteEvent3":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "3").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.CellCompleteEvent1":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.CellCompleteEvent2":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.CellCompleteEvent3":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "3").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.CellCompleteEvent1":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.CellCompleteEvent2":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.CellCompleteEvent3":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "3").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.CellCompleteEvent1":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.CellCompleteEvent2":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.CellCompleteEvent3":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "3").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.CellCompleteEvent1":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "1").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.CellCompleteEvent2":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "2").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.CellCompleteEvent3":
                        {
                            new S6F11_CellProcessEnd_CEID406(this, "3").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.OpcallConfirm":
                        {
                            new S6F11_UnitOPCALLConfirm_CEID513(this, "UNIT").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.OpcallConfirm":
                        {
                            new S6F11_UnitOPCALLConfirm_CEID513(this, "UNIT").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.OpcallConfirm":
                        {
                            new S6F11_UnitOPCALLConfirm_CEID513(this, "UNIT").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.OpcallConfirm":
                        {
                            new S6F11_UnitOPCALLConfirm_CEID513(this, "UNIT").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.OpcallConfirm":
                        {
                            new S6F11_UnitOPCALLConfirm_CEID513(this, "UNIT").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.OpcallConfirm":
                        {
                            new S6F11_UnitOPCALLConfirm_CEID513(this, "UNIT").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.OpcallConfirm":
                        {
                            new S6F11_UnitOPCALLConfirm_CEID513(this, "UNIT").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.OpcallConfirm":
                        {
                            new S6F11_UnitOPCALLConfirm_CEID513(this, "UNIT").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.InterlockConfirm":
                        {
                            new S6F11_UnitInterlockConfirm_CEID514(this, "UNIT").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.InterlockConfirm":
                        {
                            new S6F11_UnitInterlockConfirm_CEID514(this, "UNIT").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.InterlockConfirm":
                        {
                            new S6F11_UnitInterlockConfirm_CEID514(this, "UNIT").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.InterlockConfirm":
                        {
                            new S6F11_UnitInterlockConfirm_CEID514(this, "UNIT").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.InterlockConfirm":
                        {
                            new S6F11_UnitInterlockConfirm_CEID514(this, "UNIT").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.InterlockConfirm":
                        {
                            new S6F11_UnitInterlockConfirm_CEID514(this, "UNIT").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.InterlockConfirm":
                        {
                            new S6F11_UnitInterlockConfirm_CEID514(this, "UNIT").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.InterlockConfirm":
                        {
                            new S6F11_UnitInterlockConfirm_CEID514(this, "UNIT").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.EQPOpcallConfirm":
                        {
                            //new S6F11_OpcallConfirm_CEID501(this).DoWork(tagSplit[0], data);
                            new S6F11_UnitOPCALLConfirm_CEID513(this, "EQP").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.EQPOpcallConfirm":
                        {
                            //new S6F11_OpcallConfirm_CEID501(this).DoWork(tagSplit[0], data);
                            new S6F11_UnitOPCALLConfirm_CEID513(this, "EQP").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.EQPOpcallConfirm":
                        {
                            //new S6F11_OpcallConfirm_CEID501(this).DoWork(tagSplit[0], data);
                            new S6F11_UnitOPCALLConfirm_CEID513(this, "EQP").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.EQPOpcallConfirm":
                        {
                            //new S6F11_OpcallConfirm_CEID501(this).DoWork(tagSplit[0], data);
                            new S6F11_UnitOPCALLConfirm_CEID513(this, "EQP").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.EQPOpcallConfirm":
                        {
                            //new S6F11_OpcallConfirm_CEID501(this).DoWork(tagSplit[0], data);
                            new S6F11_UnitOPCALLConfirm_CEID513(this, "EQP").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.EQPOpcallConfirm":
                        {
                            //new S6F11_OpcallConfirm_CEID501(this).DoWork(tagSplit[0], data);
                            new S6F11_UnitOPCALLConfirm_CEID513(this, "EQP").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.EQPOpcallConfirm":
                        {
                            //new S6F11_OpcallConfirm_CEID501(this).DoWork(tagSplit[0], data);
                            new S6F11_UnitOPCALLConfirm_CEID513(this, "EQP").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.EQPOpcallConfirm":
                        {
                            //new S6F11_OpcallConfirm_CEID501(this).DoWork(tagSplit[0], data);
                            new S6F11_UnitOPCALLConfirm_CEID513(this, "EQP").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.EQPInterlockConfirm":
                        {
                            //new S6F11_InterlockConfirm_CEID502(this).DoWork(tagSplit[0], data);
                            new S6F11_UnitInterlockConfirm_CEID514(this, "EQP").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.EQPInterlockConfirm":
                        {
                            //new S6F11_InterlockConfirm_CEID502(this).DoWork(tagSplit[0], data);
                            new S6F11_UnitInterlockConfirm_CEID514(this, "EQP").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.EQPInterlockConfirm":
                        {
                            //new S6F11_InterlockConfirm_CEID502(this).DoWork(tagSplit[0], data);
                            new S6F11_UnitInterlockConfirm_CEID514(this, "EQP").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.EQPInterlockConfirm":
                        {
                            //new S6F11_InterlockConfirm_CEID502(this).DoWork(tagSplit[0], data);
                            new S6F11_UnitInterlockConfirm_CEID514(this, "EQP").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.EQPInterlockConfirm":
                        {
                            //new S6F11_InterlockConfirm_CEID502(this).DoWork(tagSplit[0], data);
                            new S6F11_UnitInterlockConfirm_CEID514(this, "EQP").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.EQPInterlockConfirm":
                        {
                            //new S6F11_InterlockConfirm_CEID502(this).DoWork(tagSplit[0], data);
                            new S6F11_UnitInterlockConfirm_CEID514(this, "EQP").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.EQPInterlockConfirm":
                        {
                            //new S6F11_InterlockConfirm_CEID502(this).DoWork(tagSplit[0], data);
                            new S6F11_UnitInterlockConfirm_CEID514(this, "EQP").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.EQPInterlockConfirm":
                        {
                            //new S6F11_InterlockConfirm_CEID502(this).DoWork(tagSplit[0], data);
                            new S6F11_UnitInterlockConfirm_CEID514(this, "EQP").DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.MaterialIDReadingResult":
                        {
                            new S6F11_MaterialIDReaderResult_CEID615(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.MaterialIDReadingResult":
                        {
                            new S6F11_MaterialIDReaderResult_CEID615(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.MaterialIDReadingResult":
                        {
                            new S6F11_MaterialIDReaderResult_CEID615(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.MaterialIDReadingResult":
                        {
                            new S6F11_MaterialIDReaderResult_CEID615(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.MaterialIDReadingResult":
                        {
                            new S6F11_MaterialIDReaderResult_CEID615(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.MaterialIDReadingResult":
                        {
                            new S6F11_MaterialIDReaderResult_CEID615(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.MaterialIDReadingResult":
                        {
                            new S6F11_MaterialIDReaderResult_CEID615(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.MaterialIDReadingResult":
                        {
                            new S6F11_MaterialIDReaderResult_CEID615(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.TPMLoss":
                        {
                            new S6F11_EquipmentLossCodeReport_CEID616(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.TPMLoss":
                        {
                            new S6F11_EquipmentLossCodeReport_CEID616(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.TPMLoss":
                        {
                            new S6F11_EquipmentLossCodeReport_CEID616(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.TPMLoss":
                        {
                            new S6F11_EquipmentLossCodeReport_CEID616(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.TPMLoss":
                        {
                            new S6F11_EquipmentLossCodeReport_CEID616(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.TPMLoss":
                        {
                            new S6F11_EquipmentLossCodeReport_CEID616(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.TPMLoss":
                        {
                            new S6F11_EquipmentLossCodeReport_CEID616(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.TPMLoss":
                        {
                            new S6F11_EquipmentLossCodeReport_CEID616(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC1.Send.SpecificValidationRequest1":
                        {
                            new S6F203(this, "1").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC1.Send.SpecificValidationRequest2":
                        {
                            new S6F203(this, "2").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC2.Send.SpecificValidationRequest1":
                        {
                            new S6F203(this, "1").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC2.Send.SpecificValidationRequest2":
                        {
                            new S6F203(this, "2").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC3.Send.SpecificValidationRequest1":
                        {
                            new S6F203(this, "1").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC3.Send.SpecificValidationRequest2":
                        {
                            new S6F203(this, "2").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC4.Send.SpecificValidationRequest1":
                        {
                            new S6F203(this, "1").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC4.Send.SpecificValidationRequest2":
                        {
                            new S6F203(this, "2").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC5.Send.SpecificValidationRequest1":
                        {
                            new S6F203(this, "1").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC5.Send.SpecificValidationRequest2":
                        {
                            new S6F203(this, "2").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC6.Send.SpecificValidationRequest1":
                        {
                            new S6F203(this, "1").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC6.Send.SpecificValidationRequest2":
                        {
                            new S6F203(this, "2").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC7.Send.SpecificValidationRequest1":
                        {
                            new S6F203(this, "1").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC7.Send.SpecificValidationRequest2":
                        {
                            new S6F203(this, "2").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC8.Send.SpecificValidationRequest1":
                        {
                            new S6F203(this, "1").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC8.Send.SpecificValidationRequest2":
                        {
                            new S6F203(this, "2").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC1.Send.CellLotInformationRequest1":
                        {
                            new S6F205(this, "1").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC1.Send.CellLotInformationRequest2":
                        {
                            new S6F205(this, "2").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC2.Send.CellLotInformationRequest1":
                        {
                            new S6F205(this, "1").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC2.Send.CellLotInformationRequest2":
                        {
                            new S6F205(this, "2").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC3.Send.CellLotInformationRequest1":
                        {
                            new S6F205(this, "1").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC3.Send.CellLotInformationRequest2":
                        {
                            new S6F205(this, "2").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC4.Send.CellLotInformationRequest1":
                        {
                            new S6F205(this, "1").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC4.Send.CellLotInformationRequest2":
                        {
                            new S6F205(this, "2").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC5.Send.CellLotInformationRequest1":
                        {
                            new S6F205(this, "1").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC5.Send.CellLotInformationRequest2":
                        {
                            new S6F205(this, "2").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC6.Send.CellLotInformationRequest1":
                        {
                            new S6F205(this, "1").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC6.Send.CellLotInformationRequest2":
                        {
                            new S6F205(this, "2").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC7.Send.CellLotInformationRequest1":
                        {
                            new S6F205(this, "1").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC7.Send.CellLotInformationRequest2":
                        {
                            new S6F205(this, "2").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC8.Send.CellLotInformationRequest1":
                        {
                            new S6F205(this, "1").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC8.Send.CellLotInformationRequest2":
                        {
                            new S6F205(this, "2").DoWork(tagSplit[0], null);
                        }
                        break;
                    case "iPLC1.Send.GetAttributeRequest":
                        {
                            new S14F1(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC2.Send.GetAttributeRequest":
                        {
                            new S14F1(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC3.Send.GetAttributeRequest":
                        {
                            new S14F1(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC4.Send.GetAttributeRequest":
                        {
                            new S14F1(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC5.Send.GetAttributeRequest":
                        {
                            new S14F1(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC6.Send.GetAttributeRequest":
                        {
                            new S14F1(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC7.Send.GetAttributeRequest":
                        {
                            new S14F1(this).DoWork(tagSplit[0], data);
                        }
                        break;
                    case "iPLC8.Send.GetAttributeRequest":
                        {
                            new S14F1(this).DoWork(tagSplit[0], data);
                        }
                        break;
                }
            }
        }
    }
}