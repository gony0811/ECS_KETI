using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.Core.Manager.Model;

namespace INNO6.Core.Manager
{
    public class UserAuthorityManager
    {
        private string _dbPath;
        public SortedList<string, USER_INFO> Users { get; set; }


        private UserAuthorityManager()
        {

        }

        public static UserAuthorityManager Instance = new UserAuthorityManager();

        public void Initialize(string dbPath)
        {
            _dbPath = Path.GetFullPath(dbPath);

            Users = GetUserInfoAsList();
        }

        public DataTable GetUserInfoAsDataTable()
        {
            string dbFilePath = _dbPath;
            string queryCommand = @"SELECT * FROM sys_user_info";

            DataTable dt = DbHandler.Instance.ExecuteQuery(dbFilePath, queryCommand);

            return dt;
        }

        public SortedList<string, USER_INFO> GetUserInfoAsList()
        {
            DataTable dt = GetUserInfoAsDataTable();

            SortedList<string, USER_INFO> userInfoList = new SortedList<string, USER_INFO>();

            foreach (DataRow dr in dt.Rows)
            {
                USER_INFO userInfo = new USER_INFO();

                userInfo.UserId = (dr["USERID"] as string).ToUpper();
                userInfo.Password = dr["PASSWORD"] as string;
                userInfo.Description = dr["DESCRIPTION"] as string;
                string userLevel = dr["USERLEVEL"] as string;

                if(string.IsNullOrEmpty(userLevel)) userInfo.UserLevel = eUserLevel.UNKNOWN;
                else if (userLevel.ToUpper().StartsWith("A")) userInfo.UserLevel = eUserLevel.ADMIN;
                else if (userLevel.ToUpper().StartsWith("E")) userInfo.UserLevel = eUserLevel.ENGINEER;
                else if (userLevel.ToUpper().StartsWith("G")) userInfo.UserLevel = eUserLevel.GUEST;
                else if (userLevel.ToUpper().StartsWith("O")) userInfo.UserLevel = eUserLevel.OPERATOR;
                else userInfo.UserLevel = eUserLevel.UNKNOWN;

                userInfoList.Add(userInfo.UserId, userInfo);           
            }

            return userInfoList;
        }

        public bool Authentication(string userId, string password, out eUserLevel userLevel)
        {
            if(Users.ContainsKey(userId))
            {
                if(string.IsNullOrEmpty(Users[userId].Password))
                {
                    userLevel = eUserLevel.UNKNOWN;
                    return false;
                }
                else if (Users[userId].Password.Equals(password))
                {
                    userLevel = Users[userId].UserLevel;
                    return true;
                }
                else
                {
                    userLevel = eUserLevel.UNKNOWN;
                    return false;
                }
            }
            else
            {
                userLevel = eUserLevel.UNKNOWN;
                return false;
            }
        }
    }
}
