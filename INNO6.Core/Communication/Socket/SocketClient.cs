using INNO6.Core.Communication.Socket.Checker;
using INNO6.Core.Communication.Socket.Codec;
using Mina.Core.Filterchain;
using Mina.Core.Future;
using Mina.Core.Service;
using Mina.Core.Session;
using Mina.Filter.Codec;
using Mina.Filter.Logging;
using Mina.Transport.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace INNO6.Core.Communication.Socket
{
    public class SocketClient : IoHandlerAdapter
    {
        private IoConnector _IoConnector;
        private string _IpAddress;
        private int _Port;
        private int _ConnectionTimeoutMiliseconds = 5000;
        private IDuplicateChecker _DuplicateChecker;
        private ProtocolCodecFilter _ProtocolCodecFilter;
        private LoggingFilter _LoggingFilter;
        private DefaultIoFilterChainBuilder _DefaultIoFilterChainBuilder;
        private IoSession _IoSession;
        private bool _IsOpen;

        public EventHandler<object> MessageReceivedEvent;
        public EventHandler OnConnectedEvent;
        public EventHandler OnDisconnectedEvent;

        public SocketClient()
        {
        }

        public SocketClient(string ipAddress, int port)
        {
            _IpAddress = ipAddress;
            _Port = port;
        }

        public string IpAddress
        {
            get { return _IpAddress; }
            set { _IpAddress = value; }
        }

        public int Port
        {
            get { return _Port; }
            set { _Port = value; }
        }

        public IDuplicateChecker DuplicateChecker
        {
            get { return _DuplicateChecker; }
            set { _DuplicateChecker = value; }
        }
        public int ConnectionTimeoutMiliSeconds
        {
            get { return _ConnectionTimeoutMiliseconds; }
            set { _ConnectionTimeoutMiliseconds = value; }
        }

        public ProtocolCodecFilter ProtocolCodecFilter
        {
            get { return _ProtocolCodecFilter; }
            set { _ProtocolCodecFilter = value; }
        }

        public LoggingFilter LoggingFilter
        {
            get { return _LoggingFilter; }
            set { _LoggingFilter = value; }
        }

        public DefaultIoFilterChainBuilder DefaultIoFilterChainBuilder
        {
            get { return _DefaultIoFilterChainBuilder; }
            set { _DefaultIoFilterChainBuilder = value; }
        }

        public bool IsOpen
        {
            get { return _IsOpen; }
            private set { _IsOpen = value; }
        }

        public virtual int Initialize()
        {
            _IoConnector = new AsyncSocketConnector();

            IPEndPoint address = new IPEndPoint(IPAddress.Parse(_IpAddress.Trim()), _Port);

            _IoConnector.DefaultRemoteEndPoint = address;
            _IoConnector.ConnectTimeoutInMillis = this.ConnectionTimeoutMiliSeconds;
            _ProtocolCodecFilter = new ProtocolCodecFilter(new DefaultPacketFactory());
            _DefaultIoFilterChainBuilder = new DefaultIoFilterChainBuilder();
            

            if(_DefaultIoFilterChainBuilder != null)
            {
                _IoConnector.FilterChainBuilder = _DefaultIoFilterChainBuilder;
            }

            if(ProtocolCodecFilter != null)
            {
                _IoConnector.FilterChain.AddLast("CodecFilter", ProtocolCodecFilter);
            }
            
            if(LoggingFilter != null)
            {
                _IoConnector.FilterChain.AddLast("LoggingFilter", LoggingFilter);
            }

            _IoConnector.Handler = this;

            return 0;
        }

        public override void MessageReceived(IoSession session, object message)
        {
            //string ReceiveString = Encoding.UTF8.GetString((byte[])message);
            MessageReceivedEvent?.Invoke(session, message);
        }

        public virtual bool Connect()
        {
            try
            {               
                _IsOpen = false;

                if (this._IoSession != null && this._IoSession.Connected)
                {
                    _IsOpen = true;
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback((s) =>
                    {
                        while (!_IsOpen)
                        {
                            IConnectFuture connectFuture = this._IoConnector.Connect();
                            connectFuture = connectFuture.Await();

                            if (connectFuture.Connected)
                            {
                                this._IoSession = connectFuture.Session;
                                _IsOpen = true;
                                break;
                            }
                            else
                            {
                                Thread.Sleep(this.ConnectionTimeoutMiliSeconds);
                            }
                        }
                    }));

                }

                return _IsOpen;
            }
            catch(Exception)
            {
                Thread.Sleep(this.ConnectionTimeoutMiliSeconds);
                return false;
            }
        }

        public virtual bool Disconnect()
        {          
            if(this._IoSession != null && this._IoSession.Connected)
            {
                ICloseFuture closeFuture = this._IoSession.Close(true);

                closeFuture.Await();

                if(closeFuture.Closed)
                {
                    _IsOpen = true;
                }
            }
            else
            {
                _IsOpen = true;
            }

            return _IsOpen;
        }

        public override void MessageSent(IoSession session, object message)
        {
            base.MessageSent(session, message);
        }

        public void Send(string message)
        {
            if (_IsOpen) _IoSession.Write(message);
        }

        public void Send(object message)
        {
            if (_IsOpen) _IoSession.Write(message); 
        }

        public override void SessionOpened(IoSession session)
        {
            IsOpen = true;
            base.SessionOpened(session);
            OnConnectedEvent?.Invoke(session, null);            
        }

        public override void SessionClosed(IoSession session)
        {
            IsOpen = false;
            base.SessionClosed(session);
            OnDisconnectedEvent?.Invoke(session, null);           
        }
    }
}
