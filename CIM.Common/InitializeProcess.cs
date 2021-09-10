using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.IO;
using INNO6.Core;
using CIM.Manager;
using CIM.Common;

namespace CIM.Common
{
    public class InitializeProcess
    {
        List<RECIPE> _recipeList = new List<RECIPE>();
        public static readonly InitializeProcess Instance = new InitializeProcess();

        public string DbFilePath { get; set; }

        private InitializeProcess()
        {
        }

        public void InitializeRMSData()
        {
            bool dataReadResult;
            List<Data> tagRmsList = DataManager.Instance.DataAccess.RemoteObject.DataList.Where(t => t.Group == "RMS").ToList();
            MODULE master = CommonData.Instance.MODULE_SETTINGS.Where(m => m.TYPE == eModuleType.MASTER).FirstOrDefault();
            string ppid = DataManager.Instance.GET_STRING_DATA(string.Format("i{0}.RMS.PPID", master.MODULE_NAME), out dataReadResult);
            int mode = DataManager.Instance.GET_INT_DATA(string.Format("i{0}.RMS.Mode", master.MODULE_NAME), out dataReadResult);

            if (RMSManager.Instance.GetParameterNameValueByPPID(ppid).Count > 0)
                RMSManager.Instance.DeleteParametersByPPID(ppid);

            List<RECIPE> readRecipeList = new List<RECIPE>();

            foreach (RECIPEPARAMETER rp in RMSManager.Instance.RecipeParameterList)
            {                
                var tag = tagRmsList.Where(t => t.Module == rp.MODULE && t.Index == Convert.ToInt32(rp.INDEX)).FirstOrDefault();
                
                RECIPE recipe = new RECIPE()
                {
                    INDEX = rp.INDEX,
                    PPID = ppid,
                    PARAMETERNAME = rp.PARAMETERNAME,
                    MODULE = rp.MODULE
                };

                if (tag != null)
                {
                    switch (tag.Type)
                    {
                        case eDataType.Int:
                            {
                                var value = DataManager.Instance.GET_INT_DATA(tag.Name, out dataReadResult);
                                recipe.PARAMETERVALUE = value.ToString();
                            }
                            break;
                        case eDataType.Double:
                            {
                                var value = DataManager.Instance.GET_DOUBLE_DATA(tag.Name, out dataReadResult);
                                recipe.PARAMETERVALUE = value.ToString();
                            }
                            break;
                        case eDataType.String:
                            {
                                var value = DataManager.Instance.GET_STRING_DATA(tag.Name, out dataReadResult);
                                recipe.PARAMETERVALUE = value.ToString();
                            }
                            break;
                        default:
                            {
                                recipe.PARAMETERVALUE = string.Empty;
                            }
                            break;
                    }

                    readRecipeList.Add(recipe);

                    
                }
                else
                {
                    LogHelper.Instance.DBManagerLog.DebugFormat("[Error] Not Exist RMS data io tag (ParameterName : {0} / Index : {1} / Module : {2})", rp.PARAMETERNAME, rp.INDEX, rp.MODULE);
                    return;
                }


            }


            RMSManager.Instance.SetParametersToDBEx(ppid, readRecipeList);
            RMSManager.Instance.GetCurrentPPID();
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] RMS Initialize Success : PPID = {0}", ppid);
        }
    }
}
