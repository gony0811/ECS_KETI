using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using INNO6.Core.Threading;
using INNO6.Core;
using INNO6.IO;


namespace CIM.Manager
{
    public delegate void TraceDataReport(string trid, int sampleNo, SV[] variables);

    public class FDCManager
    {
        private readonly object eventLock = new object();
        private List<Data> _svDataList = new List<Data>();
        private SortedDictionary<string, SV> _svList = new SortedDictionary<string, SV>();
        private string _dbFilePath;
        private ConcurrentDictionary<string, TRACE> _traceJobList = new ConcurrentDictionary<string, TRACE>();
        private EventHandler _traceDataSend;

        public static readonly FDCManager Instance = new FDCManager();

        public event EventHandler TraceDataSend
        {
            add
            {
                lock (eventLock)
                {
                    _traceDataSend += value;
                }
            }
            remove
            {
                lock (eventLock)
                {
                    _traceDataSend -= value;
                }
            }
        }
        
        public int TotalSVListCount
        {
            get { return _svList.Count; }
        }

        public ConcurrentDictionary<string, TRACE> CurrentTraceJobList
        {
            get { return _traceJobList; }
        }

        private FDCManager()
        {
            _dbFilePath = "./io_db.mdb";
        }

        public void Initialize(string dbPath)
        {
            _dbFilePath = dbPath;

            _svDataList = DataManager.Instance.DataAccess.RemoteObject.DataList.Where(io => io.Group.ToUpper() == "FDC" || io.Group.ToUpper() == "SV").ToList();

            string queryCommand = @"SELECT * FROM tbl_svid";

            DataTable dt = DbHandler.Instance.ExecuteQuery(_dbFilePath, queryCommand);

            foreach (DataRow dr in dt.Rows)
            {
                SV sv = new SV()
                {
                    INDEX = Convert.ToInt64(dr["DataIndex"]).ToString(),
                    SVID = dr["SVID"] as string,
                    SVNAME = dr["SVName"] as string,
                    MODULE = dr["Module"] as string,
                    MIN = dr["Min"] as string,
                    MAX = dr["Max"] as string,
                    UNIT = dr["Unit"] as string,
                    DESCRIPTION = dr["Description"] as string
                };

                _svList.Add(sv.SVID, sv);
            }
            LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] SVID Initialize Success.");
        }       

        public bool IsExistSVID(string svid)
        {
            return _svList.ContainsKey(svid);
        }

        public bool TraceJobAddOrUpdate(TRACE trace)
        {
            try
            {
                if (!CurrentTraceJobList.TryAdd(trace.TRID, trace))
                {
                    CurrentTraceJobList[trace.TRID] = trace;
                }
                else
                {
                    ThreadPool.QueueUserWorkItem((sender) =>
                    {
                        TRACE traceJob = CurrentTraceJobList[trace.TRID];
                        while (CurrentTraceJobList.ContainsKey(traceJob.TRID))
                        {
                            DateTime currentDateTime = DateTime.Now;


                            TimeSpan ts = TimeSpan.FromMilliseconds(traceJob.SamplingPeriod);

                            if (currentDateTime - traceJob.LastReportTime > ts)
                            {
                                traceJob.LastReportTime = currentDateTime;
                                traceJob.SamplingCount += 1;
                                traceJob.SamplingCount %= 100000;
                                
                                if (_traceDataSend != null) _traceDataSend.BeginInvoke(traceJob, null, null, null);
                            }

                            if (traceJob.TotalSamplingCount == traceJob.SamplingCount
                                && traceJob.TotalSamplingCount != 0)
                            {
                                TraceJobStop(traceJob.TRID);
                            }

                            Thread.Sleep(100);
                        }

                        LogHelper.Instance.DBManagerLog.DebugFormat("[INFO] Trace({0}) is removed in Tracelist", trace.TRID);
                    });
                }

                return true;
            }
            catch
            {
                LogHelper.Instance.DBManagerLog.DebugFormat("[ERROR] TraceJobAddOrUpdate Fail.");
                return false;
            }

        }

        public void AllTraceJobStop()
        {
            CurrentTraceJobList.Clear();
        }

        public bool TraceJobStop(string traceId)
        {
            TRACE trace;

            if(CurrentTraceJobList.ContainsKey(traceId))
            {
                return CurrentTraceJobList.TryRemove(traceId, out trace);
            }
            else
            {
                return false;
            }    
        }
        
        public SV GetSVInfoBySvid(string svid)
        {
            if(!_svList.ContainsKey(svid)) return null;
            return _svList[svid];
        }

        public Dictionary<string, SV> GetSVInfoListBySVID(string[] svids = null)
        {
            Dictionary<string, SV> svInfoList = new Dictionary<string, SV>();

            if(svids != null && svids.Length > 0)
            {
                foreach(string svid in svids)
                {
                    var svInfo = _svList.Values.Where(s => s.SVID == svid).FirstOrDefault();
                    if (svInfo != null)
                        svInfoList.Add(svid, svInfo);
                }
            }
            else
            {
                foreach (SV sv in _svList.Values)
                    svInfoList.Add(sv.SVID, sv);
            }

            return svInfoList;
        }

        public Dictionary<string, string> GetNameListBySvids(string[] svids, out bool error)
        {
            Dictionary<string, string> resultSvNameList = new Dictionary<string, string>();
            error = false;
            try
            {
                if (svids != null && svids.Length > 0)
                {
                    foreach (string svid in svids)
                    {
                        var svInfo = _svList.Values.Where(s => s.SVID == svid).FirstOrDefault();
                        if (svInfo != null)
                            resultSvNameList.Add(svid, svInfo.SVNAME);
                        else
                            error = true;
                    }
                }
                else
                {
                    foreach (SV sv in _svList.Values)
                        resultSvNameList.Add(sv.SVID, sv.SVNAME);
                }
            }
            catch(ArgumentNullException e)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] FDCManager GetNameListBySvids : {0}", e.Message);
                error = true;
                return null;
            }
            catch (ArgumentException e)
            {
                LogHelper.Instance.ErrorLog.DebugFormat("[ERROR] FDCManager GetNameListBySvids : {0}", e.Message);
                error = true;
                return null;
            }
            LogHelper.Instance.DBManagerLog.DebugFormat("[GetNameListBySvids] Get Name List By SVID = " + resultSvNameList.Count);
            return resultSvNameList;
        }

        public string GetValueBySvid(string svid)
        {
            bool result;

            var svInfo = _svList.Values.Where(s => s.SVID == svid).FirstOrDefault();
            var svTag = _svDataList.Where(s => (s.Index.ToString() == svInfo.INDEX) && (s.Module == svInfo.MODULE)).FirstOrDefault();

            if (svInfo == null || svTag == null)
            {
                LogHelper.Instance.DBManagerLog.DebugFormat("[ERROR] GetValueBySvid - (svInfo == null || svTag == null)");
                return string.Empty;
            }

            switch (svTag.Type)
            {
                case eDataType.Int:
                    {
                        int value = DataManager.Instance.GET_INT_DATA(svTag.Name, out result);
                        if (result) return value.ToString();
                    }
                    break;
                case eDataType.Double:
                    {
                        double value = DataManager.Instance.GET_DOUBLE_DATA(svTag.Name, out result);
                        if (result) return value.ToString();
                    }
                    break;
                case eDataType.String:
                    {
                        string value = DataManager.Instance.GET_STRING_DATA(svTag.Name, out result);
                        if (result) return value;
                    }
                    break;
                default:
                    {
                        LogHelper.Instance.DBManagerLog.DebugFormat("[ERROR] GetValueBySvid - Not define DataType");
                        result = false;
                        return string.Empty;
                    }
            }

            return string.Empty;
        }

        public Dictionary<string, string> GetValueListBySvids(string[] svids = null)
        {
            Dictionary<string, string> resultSvList = new Dictionary<string, string>();
            bool result;

            if (svids != null && svids.Length > 0)
            {
                foreach(string svid in svids)
                {
                    var svInfo = _svList.Values.Where(s => s.SVID == svid).FirstOrDefault();
                    var svTag = _svDataList.Where(s => s.Index.ToString() == svInfo.INDEX).FirstOrDefault();

                    if (svInfo == null || svTag == null) return resultSvList;

                    switch(svTag.Type)
                    {
                        case eDataType.Int:
                            {
                                int value = DataManager.Instance.GET_INT_DATA(svTag.Name, out result);
                                if (result) resultSvList.Add(svid, value.ToString());
                            }
                            break;
                        case eDataType.Double:
                            {
                                double value = DataManager.Instance.GET_DOUBLE_DATA(svTag.Name, out result);
                                if (result) resultSvList.Add(svid, value.ToString());
                            }
                            break;
                        case eDataType.String:
                            {
                                string value = DataManager.Instance.GET_STRING_DATA(svTag.Name, out result);
                                if (result) resultSvList.Add(svid, value);
                            }
                            break;
                        default:
                            {
                                result = false;
                            }
                            break;
                    }

                    if (!result) //Console.WriteLine("[Error] SV Data Read Error : {0}", svTag.Name);
                        LogHelper.Instance.DBManagerLog.DebugFormat("[ERROR] SV Data Read Error : {0}", svTag.Name);
                }              
            }
            else
            {
                foreach (SV sv in _svList.Values)
                {
                    var svTag = _svDataList.Where(s => s.Index.ToString() == sv.INDEX).FirstOrDefault();
                    if (svTag == null) return resultSvList;

                    switch(svTag.Type)
                    {
                        case eDataType.Int:
                            {
                                int value = DataManager.Instance.GET_INT_DATA(svTag.Name, out result);
                                if (result) resultSvList.Add(sv.SVID, value.ToString());
                            }
                            break;
                        case eDataType.Double:
                            {
                                double value = DataManager.Instance.GET_DOUBLE_DATA(svTag.Name, out result);
                                if (result) resultSvList.Add(sv.SVID, value.ToString());
                            }
                            break;
                        case eDataType.String:
                            {
                                string value = DataManager.Instance.GET_STRING_DATA(svTag.Name, out result);
                                if (result) resultSvList.Add(sv.SVID, value);
                            }
                            break;
                        default:
                            {
                                result = false;
                            }
                            break;
                    }

                    if (!result) //Console.WriteLine("[Error] SV Data Read Error : {0}", svTag.Name);
                        LogHelper.Instance.DBManagerLog.DebugFormat("[ERROR] SV Data Read Error : {0}", svTag.Name);
                }
            }
            LogHelper.Instance.DBManagerLog.DebugFormat("[GetValueListBySvids] Success Get Value = " + resultSvList.Count);
            return resultSvList;
        }

    }
}
