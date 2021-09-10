using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using CIM.Common;
using INNO6.Core;
using INNO6.IO;
using SYSWIN.Secl;
using Newtonsoft.Json;

namespace CIM.Host
{
    public delegate void SecsMessageEvent(SecsMessage data);
    public partial class SECSDriver : SECSDriverBase
    {  
        private Secl _secsDriver;
        private Sendata _sendData;
        private ConfigManager _config;
        private BlockingCollection<SecsMessage> _receiveMessageQueue = new BlockingCollection<SecsMessage>(new ConcurrentQueue<SecsMessage>());
        private BlockingCollection<SecsMessage> _sendMessageQueue = new BlockingCollection<SecsMessage>(new ConcurrentQueue<SecsMessage>());
        public event SecsMessageEvent OnReceivedEvent;
        public event SecsMessageEvent OnSendEvent;
        public bool WatchDogThreadRunning { get; private set; }
        private static object _key = new object();
        public SECSDriver()
        {
            _sendData = new Sendata();
            WatchDogThreadRunning = true;
            OnReceivedEvent += OnReceived;
            OnSendEvent += SendMessage;
            ThreadPool.QueueUserWorkItem(MessageReceiveWatchDog, this);
            ThreadPool.QueueUserWorkItem(MessageSendWatchDog, this);

        }     

        private void MessageSendWatchDog(object args)
        {
            while (WatchDogThreadRunning)
            {
                if (_sendMessageQueue.Count > 0)
                {
                    SecsMessage SendMessage;
                    if (_sendMessageQueue.TryTake(out SendMessage, -1))
                    {
                        if (OnSendEvent != null)
                            OnSendEvent(SendMessage);
                        else
                            _sendMessageQueue.Add(SendMessage);
                    }
                }

                Thread.Sleep(10);
            }
        }

        private void MessageReceiveWatchDog(object args)
        {
            while (WatchDogThreadRunning)
            {
                if (_receiveMessageQueue.Count > 0)
                {
                    SecsMessage receivedMessage;
                    if (_receiveMessageQueue.TryTake(out receivedMessage, -1))
                    {
                        if (OnReceivedEvent != null)
                            OnReceivedEvent(receivedMessage);
                        else
                            _receiveMessageQueue.Add(receivedMessage);
                    }
                }

                Thread.Sleep(10);
            }
        }

        public override void Initialize(string configFilePath)
        {
            _config = new ConfigManager(configFilePath);
            string secsConfigFilePath = _config.GetIniValue("HOST", "CONFIG");

            if (_config.GetIniValue("HOST", "FILTER_USE").ToUpper().Trim() == "Y")
                UseFilteringAsciiValue = true;
            else
                UseFilteringAsciiValue = false;

            FileteringRegularExp = _config.GetIniValue("HOST", "FILTER_REGEXP");
            SecsConfigFilePath = secsConfigFilePath;

            _secsDriver = new Secl();

            //secsDriverSettings();
            if (System.IO.File.Exists(SecsConfigFilePath))
            {
                string temp = System.IO.File.ReadAllText(SecsConfigFilePath);
                _secsDriver = Newtonsoft.Json.JavaScriptConvert.DeserializeObject<Secl>(temp);
            }
            else
            {
                LogHelper.Instance.ErrorLog.DebugFormat("SECL Configuration FIle Error");
                throw new Exception("SECL Configuration FIle Error");
            }

            DeviceID = _secsDriver.DeviceID;
            IpAddress = _secsDriver.LocalIP;
            Port = _secsDriver.LocalPort;
            
            _secsDriver.OnConnect += new EventHandler<ConnectEventArgs>(OnConnected);
            _secsDriver.OnSelect += new EventHandler<SelectEventArgs>(OnSelected);
            _secsDriver.OnDisconnect += new EventHandler<DisconnectEventArgs>(OnDisconnected);
            _secsDriver.OnReceive += new EventHandler<ReceiveEventArgs>(OnReceivedWatchDog);
            _secsDriver.OnReplyTimeout += new EventHandler<ReplyTimeoutEventArgs>(OnReplyTimeout);
            _secsDriver.OnSendFail += new EventHandler<SendFailEventArgs>(OnSendFail);
            _secsDriver.OnStream9 += new EventHandler<S9EventArgs>(OnS9Event);           
            _secsDriver.Initialize();

            LogHelper.Instance.BizLog.DebugFormat("_secsDriver.Initialize()");

            DataManager.Instance.DataAccess.DataChangedEvent += DataChanged;

            DataManager.Instance.SET_INT_DATA("oPLC1.Send.TCDisconnectAlarm", 1);
            DataManager.Instance.SET_INT_DATA("oPLC2.Send.TCDisconnectAlarm", 1);
            DataManager.Instance.SET_INT_DATA("oPLC3.Send.TCDisconnectAlarm", 1);
            DataManager.Instance.SET_INT_DATA("oPLC4.Send.TCDisconnectAlarm", 1);
            DataManager.Instance.SET_INT_DATA("oPLC5.Send.TCDisconnectAlarm", 1);
            DataManager.Instance.SET_INT_DATA("oPLC6.Send.TCDisconnectAlarm", 1);
            DataManager.Instance.SET_INT_DATA("oPLC7.Send.TCDisconnectAlarm", 1);
            DataManager.Instance.SET_INT_DATA("oPLC8.Send.TCDisconnectAlarm", 1);

        }

        protected void OnReceivedWatchDog(object obj, ReceiveEventArgs e)
        {
            SecsMessage message = e.SecsMessage;
            _receiveMessageQueue.Add(message);
        }

        public void InvalidMessage(int stream, uint systemByte)
        {
            SecsMessage msg = new SecsMessage(stream, 0, systemByte);

            _secsDriver.Send(msg);
            LogHelper.Instance.BizLog.DebugFormat("[InvalidMessage] Send " + msg);
        }

        public Sendata SetID(int stream, int function, uint systemByte)
        {
            _sendData.nStream = stream;
            _sendData.nFunc = function;
            _sendData.lSysbyte = systemByte;

            return _sendData;
        }

        private void SendMessage(SecsMessage msg)
        {
            lock (_key)
            {
                try
                {
                    if (!IsConnected) return;

                    var sm = msg as SecsMessage;
                    _secsDriver.Send(sm);
                }
                catch(SeclException e)
                {

                }
                catch(System.AccessViolationException e)
                {

                }
            }
        }

        public override void Send(object message)
        {
            SecsMessage secsMessage = message as SecsMessage;
            _sendMessageQueue.Add(secsMessage);
        }

        public override void Start()
        {
            if (_secsDriver.IsInitialized())
            {
                _secsDriver.Start();
            }
            else
            {
                LogHelper.Instance.ErrorLog.DebugFormat("SecsDriver Initialize Fail");
                throw new Exception("SecsDriver Initialize Fail");
            }

            if (!_secsDriver.IsStarted())
            {
                LogHelper.Instance.ErrorLog.DebugFormat("SecsDriver Start Fail");
                throw new Exception("SecsDriver Start Fail");
            }

        }

        public override void Stop()
        {
            _secsDriver.Stop();
        }

        public bool IsStarted()
        {
            return _secsDriver.IsStarted();
        }

        public bool IsTrialMode()
        {
            return _secsDriver.IsTrialMode();
        }

        public override void WriteLogAndSendMessage(object message, object args)
        {
            SecsMessage msg = message as SecsMessage;
            
            int stream = msg.Stream;
            int function = msg.Function;

            StringBuilder sbAddString = new StringBuilder("");
            string log;

            this.Send(msg);

            if (stream == 6 && function == 1) return;

            if (msg.WaitBit == true)
            {
                try
                {
                    string sCEID = args as string;

                    if (sCEID == "")
                    {
                        if (function == 0)
                            log = string.Format("COM : Abort Transaction S{0}F{1} was sent", stream, function);
                        else if (function == 9)
                        {
                            log = string.Format("COM : Stream9 Type Message S{0}F{1} was sent", stream, function);
                        }
                        else
                            log = string.Format("CIM  : S{0}F{1} was sent successfully", stream, function);
                    }
                    else
                    {
                        if (function == 0)
                            log = string.Format("COM : Abort Transaction S{0}F{1} was sent", stream, function);
                        else if (function == 9)
                        {
                            log = string.Format("COM : Stream9 Type Message S{0}F{1} was sent", stream, function);
                        }
                        else
                            log = string.Format("CIM  : S{0}F{1} CEID-{2} was sent successfully", stream, function, sCEID);
                    }

                    LogHelper.Instance.BizLog.DebugFormat(log);
                }
                catch (SeclException ex)
                {
                    log = string.Format("CIM  : Failed to send S{0}F{1}: error={2}", stream, function, ex.ErrorCode.ToString());
                    LogHelper.Instance.ErrorLog.DebugFormat(log);
                }
            }
            else
            {
                string sACK = args as string;

                //this.Send(msg);

                try
                {
                    if (sACK != "0")
                        log = string.Format("CIM : Not Accepted Message S{0}F{1}", stream, (int)(function - 1));
                    else
                        log = string.Format("CIM  : Reply S{0}F{1} was sent successfully", stream, function);

                    LogHelper.Instance.BizLog.DebugFormat(log);
                }
                catch (SeclException ex)
                {
                    log = string.Format("CIM  : Failed to reply S{0}F{1}: error={2}", stream, function, ex.ErrorCode.ToString());
                    LogHelper.Instance.ErrorLog.DebugFormat(log);
                }
            }
        }
    }

    public class Sendata
    {
        public int nStream = 0;
        public int nFunc = 0;
        public uint lSysbyte = 0;
    }
    public enum SFDirection
    {
        smPrimaryOut,
        smPrimaryIn,
        smSecondaryIn,
        smSecondaryOut,
        smNone,
    }


}
