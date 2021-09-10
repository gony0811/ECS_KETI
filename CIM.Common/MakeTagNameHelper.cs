using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INNO6.IO;
using INNO6.Core;

namespace CIM.Common
{

    public class TagNameHelper
    {
        public static string MakeTagName(eDirection type, string moduleName, string groupName, string tagName)
        {
            string header;
            StringBuilder fullTagName = new StringBuilder();

            if (type == eDirection.IN) { header = "i"; }
            else if (type == eDirection.OUT) { header = "o"; }
            else { header = "v"; }

            fullTagName.AppendFormat("{0}{1}.{2}.{3}", header, moduleName, groupName, tagName);
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] Make Tag Name Success.");
            return fullTagName.ToString();
        }
    }
}
