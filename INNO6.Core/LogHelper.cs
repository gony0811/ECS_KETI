using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INNO6.Core
{

    //public class LogHelper
    //{
    //    private LogHelper()
    //    {
    //    }

    //    /*
    //     * 파일 Logging을 코드로 설정한다.
    //     * GetNewLogger(string name, string directoryName, string fileName, bool isHourly = true, double maxDays = 30, string LogTimeFormat = "[yyyy-MM-dd_HH:mm:ss.fff]");
    //     * name : 로깅 카테고리 이름.
    //     * directoryName : 로그 파일 생성 위치.
    //     * fileName : 로그 파일 이름.
    //     * isHourly : 시간단위로 파일 분리 저장 여부.
    //     * maxDays : 최대 보관 일수 지정.
    //     * LogTimeFormat : 로깅 시간 형식 지정.
    //     */

    //    public static readonly ILog SystemLog = LogManager.GetNewLogger("System", "./Log/System/", "System", true, 30);
    //    public static readonly ILog SECSMessageLog = LogManager.GetNewLogger("SECMsg", "/Log/SECSMsg/", "SECSMessage", true, 30);
    //    public static readonly ILog BizLog = LogManager.GetNewLogger("Biz", "./Log/Biz/", "Biz", true, 30);
    //    public static readonly ILog DBManagerLog = LogManager.GetNewLogger("Biz", "./Log/DB/", "DB", true, 30);
    //    public static readonly ILog UILog = LogManager.GetNewLogger("UI", "./Log/UI/", "UI", true, 30);
    //    public static readonly ILog TerminalLog = LogManager.GetNewLogger("Terminal", "./Log/Terminal/", "Terminal", true, 30);
    //    public static readonly ILog StreamFunctionLog = LogManager.GetNewLogger("SF", "./Log/SFMessage/", "SFMessage", true, 30);

    //}

    public class LogHelper
    {
        private static LogHelper instance = null;
        public static LogHelper Instance
        {
            get
            {
                if (instance == null)
                    instance = new LogHelper();

                return instance;
            }
        }

        public readonly log4net.ILog DeviceLog = log4net.LogManager.GetLogger("DeviceLog");
        public readonly log4net.ILog _info = log4net.LogManager.GetLogger("Information");
        public readonly log4net.ILog _debug = log4net.LogManager.GetLogger("Debug");
        public readonly log4net.ILog SystemLog = log4net.LogManager.GetLogger("System");            // Data Change , Get, Set 함수, INNO6.IO 
        public readonly log4net.ILog SECSMessageLog = log4net.LogManager.GetLogger("SecsMessage");  // msg 보내고 받는 것들.
        public readonly log4net.ILog BizLog = log4net.LogManager.GetLogger("Biz");                  // SECS Driver, Data Changed 함수 안의 내용들
        public readonly log4net.ILog DBManagerLog = log4net.LogManager.GetLogger("DBManager");      // CIM.Manager Project, Engine.cs 
        public readonly log4net.ILog UILog = log4net.LogManager.GetLogger("UI");
        public readonly log4net.ILog TerminalLog = log4net.LogManager.GetLogger("Terminal");
        public readonly log4net.ILog StreamFunctionLog = log4net.LogManager.GetLogger("StreamFunction");
        public readonly log4net.ILog PLCDataChange = log4net.LogManager.GetLogger("PLCDataChange"); // CCIE
        public readonly log4net.ILog ErrorLog = log4net.LogManager.GetLogger("ERROR");              // Catch 의 Exception
        public readonly log4net.ILog Alivebit = log4net.LogManager.GetLogger("ALIVEBIT");
        public readonly log4net.ILog Tracking = log4net.LogManager.GetLogger("Tracking");
        public readonly log4net.ILog MaterialLog = log4net.LogManager.GetLogger("MaterialLog");
        public readonly log4net.ILog Collection = log4net.LogManager.GetLogger("Collection_CEID403");
        private log4net.Repository.Hierarchy.Hierarchy hierarchy;

        // 설정변수. 이 부분 변경하면 됨
        private string _LogPath = @"\Logs\";                  //  파일 저장될 장소

        //생성자. 초기화
        public LogHelper()
        {
            //logger가 담긴 Repository 계층구조 가져옴.
            hierarchy = (log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository();
            
            hierarchy.Configured = true;  //이 옵션을 켜줘야 동적생성 가능
           
            // logger 초기화
            AddApender(DeviceLog);
            //AddApender(_info);
            AddApender(_debug);
            AddApender(SystemLog);
            //AddApender(SECSMessageLog);
            //AddApender(BizLog);
            AddApender(DBManagerLog);
            AddApender(UILog);
            //AddApender(TerminalLog);
            //AddApender(StreamFunctionLog);
            //AddApender(PLCDataChange);
            AddApender(ErrorLog);
            //AddApender(Alivebit);
            //AddApender(Tracking);
            //AddApender(MaterialLog);
            //AddApender(Collection);
        }
        private void AddApender(log4net.ILog log)
        {
            log4net.Repository.Hierarchy.Logger logger = (log4net.Repository.Hierarchy.Logger)log.Logger;
            log4net.Appender.ColoredConsoleAppender consoleAppender = new log4net.Appender.ColoredConsoleAppender();
            consoleAppender.Layout = new log4net.Layout.PatternLayout("%date{yyyy-MM-dd},%date{HH:mm:ss.fff} [%thread] %-5level %logger [%property{NDC}] - %message%newline");
            consoleAppender.AddMapping(new log4net.Appender.ColoredConsoleAppender.LevelColors()
            {
                Level = log4net.Core.Level.Error,
                ForeColor = log4net.Appender.ColoredConsoleAppender.Colors.Red,
            });

            consoleAppender.AddMapping(new log4net.Appender.ColoredConsoleAppender.LevelColors()
            {
                Level = log4net.Core.Level.Info,
                ForeColor = log4net.Appender.ColoredConsoleAppender.Colors.White,
                
            });

            consoleAppender.AddMapping(new log4net.Appender.ColoredConsoleAppender.LevelColors()
            {
                Level = log4net.Core.Level.Warn,
                ForeColor = log4net.Appender.ColoredConsoleAppender.Colors.Yellow,

            });

            consoleAppender.AddMapping(new log4net.Appender.ColoredConsoleAppender.LevelColors()
            {
                ForeColor = log4net.Appender.ColoredConsoleAppender.Colors.White,
                Level = log4net.Core.Level.Debug,
            });

            consoleAppender.ActivateOptions();



            log4net.Appender.RollingFileAppender fileAppender = new log4net.Appender.RollingFileAppender();
            fileAppender.File = _LogPath + logger.Name + "\\" + logger.Name + @"Log_.log"; // 전체 경로에 생성할 메인 로그 파일 이름
            fileAppender.PreserveLogFileNameExtension = true;
            fileAppender.StaticLogFileName = false;
            fileAppender.Encoding = System.Text.Encoding.Unicode;
            fileAppender.AppendToFile = true;
            fileAppender.LockingModel = new log4net.Appender.FileAppender.MinimalLock();
            fileAppender.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Date;

            //파일 사이즈 Rolling
            fileAppender.MaxSizeRollBackups = 0;  //0으로 지정시 용량오버되면 로그파일 새로만듬. 
            fileAppender.MaximumFileSize = "10MB";  // 파일 사이즈만 가지고 파일을 백업한다. 이보다 커지면 로그파일 새로 만든다.

            fileAppender.DatePattern = "yyyyMMdd";
            fileAppender.Layout = new log4net.Layout.PatternLayout("%date{yyyy-MM-dd},%date{HH:mm:ss.fff},%logger,%message%newline");
            //Appender 활성화 및 로그에 붙이기
            fileAppender.ActivateOptions();

            logger.AddAppender(fileAppender);
            logger.AddAppender(consoleAppender);
            logger.Hierarchy = hierarchy;
            logger.Level = logger.Hierarchy.LevelMap["ALL"];
        }
    }
}

