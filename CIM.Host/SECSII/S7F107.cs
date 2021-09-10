using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core;
using INNO6.Core.Threading;
using INNO6.IO;
using CIM.Manager;
using CIM.Common;
using SYSWIN.Secl;

namespace CIM.Host.SECSII
{
    /*
 <L, 5 * PPID Event Info
    1.<A[1] $MODE> * PPID 변경 사항
    2.<A[40] $EQPID> * 설비 고유 ID
    3.<A[40] $PPID> * 설비에서 변경 된 PPID
    4.<A[1] $PPID_TYPE> * 설비에서 변경 된 Process Parameter Group ID (Recipe ID) TYPE
    5.<L, a * C o m m a n d Se t Li s t a = Number of process commands
        1.<L, 2 * CC O D E S et
            1.<A[3] $CCODE> *
            2.<L, b * Pa r am et e r L is t b = Number of parameters
                1.<L, 2 * Pa r am et e r Se t
                    1.<A[40] $P_PARM_NAME> * Parameter 이름
                    2.<A[40] $P_PARM> * Parameter 값
 */

    public class S7F107 : SFMessage
    {
        string _MODE;
        string _PPID;
        string _EQPID;
        string _PPIDTYPE;
        string _CCODE;
        Dictionary<string, string> _storedParamList = new Dictionary<string, string>();

        public S7F107(SECSDriverBase driver)
            : base(driver)
        {
            Stream = 7; Function = 107;
        }

        public override void DoWork(string driverName, object obj)
        {
            LogHelper.Instance.BizLog.DebugFormat("[{0}]DoWork : {1}, {2}", this.GetType().Name, driverName, obj);
            Data DataInfo = obj as Data;

            bool dataReadResult;

            _EQPID = CommonData.Instance.EQP_SETTINGS.EQPID.Trim();
            _PPID = DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.RMS.PPID", DataInfo.Module), out dataReadResult).Trim();
            _MODE = DataManager.Instance.GET_INT_DATA(string.Format("i{0}.RMS.Mode", DataInfo.Module), out dataReadResult).ToString();
            _PPIDTYPE = "1";
            _CCODE = " ";

            List<RECIPE> dbParams = new List<RECIPE>();
            SecsMessage secsMessage = new SecsMessage(Stream, Function, true, Convert.ToInt32(SecsDriver.DeviceID));
            secsMessage.AddList(5);
            {
                secsMessage.AddAscii(_MODE);
                secsMessage.AddAscii(_EQPID);
                secsMessage.AddAscii(_PPID);
                secsMessage.AddAscii(_PPIDTYPE);
                if (_MODE == "2") //Delete
                {
                    secsMessage.AddList(0);
                }
                else if(_MODE == "1") // Create
                {
                    secsMessage.AddList(1);
                    {
                        secsMessage.AddList(2);
                        {
                            secsMessage.AddAscii(_CCODE);
                            {
                                secsMessage.AddList(RMSManager.Instance.RecipeParameterList.Count);
                                {
                                    foreach (RECIPEPARAMETER rParam in RMSManager.Instance.RecipeParameterList)
                                    {
                                        RECIPE recipe = new RECIPE()
                                        {
                                            INDEX = rParam.INDEX,
                                            PPID = _PPID,
                                            MODULE = rParam.MODULE,
                                            PARAMETERNAME = rParam.PARAMETERNAME,
                                            DESCRIPTION = rParam.DESCRIPTION,
                                        };

                                        string tagName = string.Format("i{0}.RMS.{1}", recipe.MODULE, recipe.INDEX);

                                        Data tag = DataManager.Instance.DataAccess.RemoteObject.DataList.Where(t => t.Name == tagName).FirstOrDefault();

                                        if(tag != null)
                                        {
                                            switch (tag.Type)
                                            {
                                                case eDataType.Int:
                                                    {
                                                        recipe.PARAMETERVALUE = DataManager.Instance.GET_INT_DATA(tag.Name, out dataReadResult).ToString();
                                                    }
                                                    break;
                                                case eDataType.Double:
                                                    {
                                                        recipe.PARAMETERVALUE = DataManager.Instance.GET_DOUBLE_DATA(tag.Name, out dataReadResult).ToString();
                                                    }
                                                    break;
                                                case eDataType.String:
                                                    {
                                                        recipe.PARAMETERVALUE = DataManager.Instance.GET_STRING_DATA(tag.Name, out dataReadResult);
                                                    }
                                                    break;
                                            }

                                            secsMessage.AddList(2);
                                            {
                                                secsMessage.AddAscii(recipe.PARAMETERNAME);
                                                secsMessage.AddAscii(recipe.PARAMETERVALUE);
                                            }

                                            dbParams.Add(recipe);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else // Modify : 수정요청 모듈의 데이터만 갱신하여 업데이트
                {
                    secsMessage.AddList(1);
                    {
                        secsMessage.AddList(2);
                        {
                            secsMessage.AddAscii(_CCODE);
                            {
                                List<RECIPE> recipeList = RMSManager.Instance.GetRecipeDataList(_PPID);

                                secsMessage.AddList(RMSManager.Instance.RecipeParameterList.Count);
                                {
                                    foreach (RECIPE recipe in recipeList)
                                    {
                                        if(recipe.MODULE == DataInfo.Module)
                                        {
                                            string tagName = string.Format("i{0}.RMS.{1}", recipe.MODULE, recipe.INDEX);

                                            Data tag = DataManager.Instance.DataAccess.RemoteObject.DataList.Where(t => t.Name == tagName).FirstOrDefault();

                                            switch (tag.Type)
                                            {
                                                case eDataType.Int:
                                                    {
                                                        recipe.PARAMETERVALUE = DataManager.Instance.GET_INT_DATA(tag.Name, out dataReadResult).ToString();
                                                    }
                                                    break;
                                                case eDataType.Double:
                                                    {
                                                        recipe.PARAMETERVALUE = DataManager.Instance.GET_DOUBLE_DATA(tag.Name, out dataReadResult).ToString();
                                                    }
                                                    break;
                                                case eDataType.String:
                                                    {
                                                        recipe.PARAMETERVALUE = DataManager.Instance.GET_STRING_DATA(tag.Name, out dataReadResult);
                                                    }
                                                    break;
                                            }
                                        }

                                        secsMessage.AddList(2);
                                        {
                                            secsMessage.AddAscii(recipe.PARAMETERNAME);
                                            secsMessage.AddAscii(recipe.PARAMETERVALUE);
                                        }

                                        dbParams.Add(recipe);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            SecsDriver.WriteLogAndSendMessage(secsMessage, "");

            CommonData.Instance.OnStreamFunctionAdd("MAIN", "E->H", "RMS", "S7F107", null, "PPID Paramter Change", null);

            switch (_MODE)
            {
                case "1":
                case "3":
                    RMSManager.Instance.SetParametersToDBEx(_PPID, dbParams);
                    break;
                case "2":
                    RMSManager.Instance.DeleteParametersByPPID(_PPID);
                    break;
                default:
                    //Console.WriteLine("Recipe Mode : {0}", _MODE);
                    LogHelper.Instance.SECSMessageLog.DebugFormat("[ERROR] Recipe Mode : {0}", _MODE);
                    break;
            }      
        }
    }
}
