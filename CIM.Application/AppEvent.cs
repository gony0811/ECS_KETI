using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Data;
using INNO6.Core;
using INNO6.Core.Threading;
using INNO6.IO;
using CIM.Manager;
using CIM.Common;
using CIM.Host;
using CIM.Host.SECSII;

namespace CIM.Application
{
    public partial class StartUp
    {
        private void report_TraceDataSend(object sender, EventArgs e)
        {
            if(!_secsDriver.IsConnected)
            {
                FDCManager.Instance.AllTraceJobStop();
            }

            TRACE t = sender as TRACE;

            new S6F1(_secsDriver).DoWork("", t);
        }

        private void report_AlarmOccoured(object sender, EventArgs e)
        {
            ALARM alarm = sender as ALARM;
            LogHelper.Instance._debug.DebugFormat("[INFO] ALARM Changed : {0}", alarm.TEXT);
            if (_secsDriver == null) return;

            new CIM.Host.SECSII.S5F11(_secsDriver).DoWork("", alarm);

            CommonData.Instance.OnAlarmDataChanged();
        }

        private void work_AllWorkCompleted(object sender, EventArgs e)
        {
            stats = new int[6];
            LogHelper.Instance._debug.DebugFormat("[INFO] work_AllWorkCompleted RefreshCounts !!");
            RefreshCounts();
        }

        private void work_WorkerException(object sender, ResourceExceptionEventArgs e)
        {
            //Console.WriteLine("Error : {0}", e.Exception.Message);
            LogHelper.Instance._debug.DebugFormat("[ERROR] work_WorkerException : {0}", e.Exception.Message);
        }

        private void work_ChangedWorkItemState(object sender, ChangedWorkItemStateEventArgs e)
        {
            lock (this)
            {
                stats[(int)e.PreviousState] -= 1;
                stats[(int)e.WorkItem.State] += 1;
            }

            if (DateTime.Now > nextRefreshTime)
            {
                LogHelper.Instance._debug.DebugFormat("[INFO] work_ChangedWorkItemState RefreshCounts !!");
                RefreshCounts();
                nextRefreshTime = DateTime.Now + refreshInterval;
            }
        }
    }
}
