using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core;
using System.Runtime.Serialization.Formatters.Binary;
using ECS.Recipe.Model;
using INNO6.IO;
using log4net;
using ECS.Common.Helper;
using System.Runtime.Serialization;

namespace ECS.Recipe
{
    public class RecipeManager
    {
        public SortedList<string, string> RecipeNameList;

        public static readonly RecipeManager Instance = new RecipeManager();
        private RecipeManager() { }

        private string _dbPath;
        private string _recipePath;
        private string _CurrentRecipeName;

        public string RecipePath { get { return _recipePath; } set { _recipePath = value; } }
        public string CurrentRecipeName { get { return _CurrentRecipeName; } set { _CurrentRecipeName = value; } }

        public bool Initialize(string dbPath, string recipePath, string currentRecipeName)
        {
            bool result = true;
            _recipePath = recipePath;
            _dbPath = Path.GetFullPath(dbPath);
            CurrentRecipeName = currentRecipeName;

            RecipeParameterSetting(currentRecipeName);

            return result;
        }

        public bool OPEN_RECIPE_FILE(string recipeFileName, out string currentRecipeName)
        {
            bool result = true;
            currentRecipeName = "";
            RECIPE recipe = (RECIPE)RecipeFileDeserialize(recipeFileName);

            if (recipe == null) return false;

            CurrentRecipeName = recipe.RECIPE_NAME;
            currentRecipeName = CurrentRecipeName;
            result &= DELETE_ALL_STEP();

            foreach (RECIPE_STEP step in recipe.STEP_LIST)
            {
                result &= INSERT_STEP(step);
            }

            return result;
        }

        public bool SAVE_RECIPE_FILE(string recipeFileName, string editor)
        {
            string filePath = _recipePath + @"\" + recipeFileName + @".rcp";

            RECIPE recipe = new RECIPE()
            {
                RECIPE_NAME = recipeFileName,
                EDITOR = editor,
                CREATETIME = DateTime.Now,
                CHANGETIME = DateTime.Now,
                SELECTED = true,
                RECIPEFILEPATH = filePath,
                STEP_COUNT = 1,
                USE = true,
                DESCRIPTION = "",
                STEP_LIST = GET_RECIPE_STEP_LIST()
            };

            RecipeFileSerialize(filePath, recipe);

            return true;
        }

        private void RecipeParameterSetting(string currentRecipeName)
        {
            string filePath = _recipePath + @"\" + currentRecipeName + @".rcp";
            RECIPE recipe = (RECIPE)RecipeFileDeserialize(filePath);

            DELETE_ALL_STEP();

            foreach (RECIPE_STEP step in recipe.STEP_LIST)
            {
                INSERT_STEP(step);
            }
        }

        private void RecipeFileSerialize(string filename, object recipe)
        {
            FileStream fileStreamObject = null;

            try
            {
                fileStreamObject = new FileStream(filename, FileMode.Create);

                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStreamObject, recipe);

            }
            finally
            {
                if (fileStreamObject != null)
                    fileStreamObject.Close();
            }
        }

        private object RecipeFileDeserialize(string filename)
        {
            FileStream fileStreamObject = null;

            try
            {
                fileStreamObject = new FileStream(filename, FileMode.Open);

                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return binaryFormatter.Deserialize(fileStreamObject);

            }
            catch (SerializationException e)
            {
                LogHelper.Instance.SystemLog.WarnFormat("[WARNING] Recipe file serialization Exception : " + e.Message);
                string newRecipeName = "new_recipe";
                DataManager.Instance.SET_STRING_DATA(IoNameHelper.V_STR_SYS_CURRENT_RECIPE, newRecipeName);
                DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.V_STR_SYS_CURRENT_RECIPE, newRecipeName);

                RecipeManager.Instance.SAVE_RECIPE_FILE(newRecipeName, "ecs");

                fileStreamObject = new FileStream(filename, FileMode.Open);

                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return binaryFormatter.Deserialize(fileStreamObject);
            }
            catch (FileNotFoundException e)
            {
                LogHelper.Instance.SystemLog.WarnFormat("[WARNING] Recipe file not found : " + e.Message);
                string newRecipeName = "new_recipe";
                DataManager.Instance.SET_STRING_DATA(IoNameHelper.V_STR_SYS_CURRENT_RECIPE, newRecipeName);
                DataManager.Instance.CHANGE_DEFAULT_DATA(IoNameHelper.V_STR_SYS_CURRENT_RECIPE, newRecipeName);

                RecipeManager.Instance.SAVE_RECIPE_FILE(newRecipeName, "ecs");

                fileStreamObject = new FileStream(newRecipeName, FileMode.Open);

                BinaryFormatter binaryFormatter = new BinaryFormatter();
                return binaryFormatter.Deserialize(fileStreamObject);
            }
            finally
            {
                if (fileStreamObject != null)
                    fileStreamObject.Close();
            }
        }

        public RECIPE NEW_RECIPE(string recipeName, string creator)
        {
            RECIPE recipe = new RECIPE()
            {
                RECIPE_NAME = recipeName,
                EDITOR = creator,
                CREATETIME = DateTime.Now,
                CHANGETIME = DateTime.Now,
                SELECTED = true,
                RECIPEFILEPATH = _recipePath + @"\" + recipeName + @".rcp",
                STEP_COUNT = 1,
                USE = true,
                DESCRIPTION = "",
                STEP_LIST = new List<RECIPE_STEP>()
            };

            return recipe;
        }

        public void SAVE_RECIPE(RECIPE recipe)
        {
            RecipeFileSerialize(recipe.RECIPEFILEPATH, recipe);
        }

        public bool IS_EXIST_STEPID(int STEPID)
        {
            string dbFilePath = _dbPath;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string queryCommand = @"SELECT * FROM sys_recipe_parameter_step WHERE STEP_ID=@STEPID";

            parameters.Add("@STEPID", STEPID);

            DataTable dt = DbHandler.Instance.ExecuteQuery(dbFilePath, queryCommand, parameters);

            if (dt.Rows.Count != 0) return true;
            else return false;
        }
        public List<RECIPE_STEP> GET_RECIPE_STEP_LIST()
        {
            string dbFilePath = _dbPath;
            string queryCommand = @"SELECT * FROM sys_recipe_parameter_step ORDER BY STEP_ID ASC";

            List<RECIPE_STEP> stepList = new List<RECIPE_STEP>();
            DataTable dt = DbHandler.Instance.ExecuteQuery(dbFilePath, queryCommand);

            foreach (DataRow dr in dt.Rows)
            {
                RECIPE_STEP step = new RECIPE_STEP();

                step.STEP_ID = (int)dr["STEP_ID"];
                if (double.TryParse((string)dr["X_POSITION"], out double xPos)) step.X_POSITION = xPos;
                if (double.TryParse((string)dr["Y_POSITION"], out double yPos)) step.Y_POSITION = yPos;
                if (double.TryParse((string)dr["ENERGY"], out double energy)) step.ENERGY = energy;
                if (double.TryParse((string)dr["HV"], out double hv)) step.HV = hv;

                step.EGY_MODE = (string)dr["EGY_MODE"];
                step.PROCESS_TIME = (int)dr["PROCESS_TIME"];

                stepList.Add(step);
            }

            return stepList;
        }

        public int GET_RECIPE_TOTALWORKTIME()
        {
            int totalWorkTime = 0;
            List<RECIPE_STEP> stepList = GET_RECIPE_STEP_LIST();

            foreach (RECIPE_STEP step in stepList)
            {
                totalWorkTime += step.PROCESS_TIME;
            }

            return totalWorkTime;
        }


        public bool EDIT_STEP(RECIPE_STEP step)
        {
            string dbFilePath = _dbPath;
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            string sql = @"UPDATE sys_recipe_parameter_step SET X_POSITION=@X_POSITION, Y_POSITION=@Y_POSITION, ENERGY=@ENERGY, HV=@HV, EGY_MODE=@EGY_MODE, PROCESS_TIME=@PROCESS_TIME 
                            WHERE STEP_ID=@STEP_ID;";

            parameters.Add("@X_POSITION", step.X_POSITION);
            parameters.Add("@Y_POSITION", step.Y_POSITION);
            parameters.Add("@ENERGY", step.ENERGY);
            parameters.Add("@HV", step.HV);
            parameters.Add("@EGY_MODE", step.EGY_MODE);
            parameters.Add("@PROCESS_TIME", step.PROCESS_TIME);
            parameters.Add("@STEP_ID", step.STEP_ID);

            int counts = DbHandler.Instance.ExecuteNonQuery(_dbPath, sql, parameters);

            if (counts > 0) return true;
            else return false;
        }


        public bool DELETE_ALL_STEP()
        {
            string dbFilePath = _dbPath;
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            string sql = @"DELETE * FROM sys_recipe_parameter_step;";

            int counts = DbHandler.Instance.ExecuteNonQuery(_dbPath, sql);

            if (counts >= 0) return true;
            else return false;
        }

        public bool DELETE_STEP(int STEPID)
        {
            string dbFilePath = _dbPath;
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            string sql = @"DELETE * FROM sys_recipe_parameter_step WHERE STEP_ID=@STEP_ID;";

            parameters.Add("@STEP_ID", STEPID);

            int counts = DbHandler.Instance.ExecuteNonQuery(_dbPath, sql, parameters);

            if (counts > 0) return true;
            else return false;
        }

        public bool INSERT_STEP(RECIPE_STEP step)
        {
            string dbFilePath = _dbPath;
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            string sql = @"INSERT INTO sys_recipe_parameter_step VALUES(                                                                   
                                                                  @STEP_ID,                                                                
                                                                  @X_POSITION,
                                                                  @Y_POSITION,
                                                                  @ENERGY,
                                                                  @HV,
                                                                  @EGY_MODE,
                                                                  @PROCESS_TIME
                                                                );";
            parameters.Add("@STEP_ID", step.STEP_ID);
            parameters.Add("@X_POSITION", step.X_POSITION);
            parameters.Add("@Y_POSITION", step.Y_POSITION);
            parameters.Add("@ENERGY", step.ENERGY);
            parameters.Add("@HV", step.HV);
            parameters.Add("@EGY_MODE", step.EGY_MODE);
            parameters.Add("@PROCESS_TIME", step.PROCESS_TIME);

            int counts = DbHandler.Instance.ExecuteNonQuery(_dbPath, sql, parameters);

            if (counts > 0) return true;
            else return false;
        }
    }
}
