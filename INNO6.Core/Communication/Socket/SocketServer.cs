using INNO6.Core.Communication.Socket.Checker;
using INNO6.Core.Communication.Socket.Codec;
using Mina.Core.Filterchain;
using Mina.Core.Service;
using Mina.Core.Session;
using Mina.Filter.Codec;
using Mina.Filter.Logging;
using Mina.Transport.Socket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace INNO6.Core.Communication.Socket
{
    public class SocketServer : IoHandlerAdapter
    {
        private IoAcceptor _IoAcceptor;
        private IoSession _IoSession;
        private bool _IsOpen;
        private int _Port;
        private string _IpAddress;
        private IDuplicateChecker _DuplicateChecker;
        private ProtocolCodecFilter _ProtocolCodecFilter;
        private LoggingFilter _LoggingFilter;
        private DefaultIoFilterChainBuilder _DefaultIoFilterChainBuilder;
        private int _IdleCount;
        private int _ManagedSessionCount;

        public EventHandler<object> MessageReceivedEvent;
        public EventHandler OnConnectedEvent;
        public EventHandler OnDisconnectedEvent;
        public EventHandler<object> OnMessageSentEvent;

        public int ManagedSessionCount
        {
            get { return _ManagedSessionCount; }
            set { _ManagedSessionCount = value; }
        }

        public int IdleCount
        {
            get { return _IdleCount; }
            set { _IdleCount = value; }
        }

        public int Port
        {
            get 
            { 
                return _Port; 
            }

            set
            {
                _Port = value;
            }
        }

        public string IpAddress
        {
            get { return _IpAddress; }
            set { _IpAddress = value; }
        }

        public IoAcceptor IoAcceptor
        {
            get { return _IoAcceptor; }
            private set { _IoAcceptor = value; }
        }

        public IoSession IoSession
        {
            get { return _IoSession; }
            private set { _IoSession = value; }
        }

        public bool IsOpen
        {
            get { return _IsOpen; }
            private set { _IsOpen = value; }
        }

        public IDuplicateChecker DuplicateChecker
        {
            get { return _DuplicateChecker; }
            set { _DuplicateChecker = value; }
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

        public virtual int Initialize()
        {
            _IoAcceptor = new AsyncSocketAcceptor();

            IPEndPoint address = new IPEndPoint(IPAddress.Parse(_IpAddress.Trim()), _Port);

            _IoAcceptor.DefaultLocalEndPoint = address;
                      
            if (_DefaultIoFilterChainBuilder == null)
            {
                _DefaultIoFilterChainBuilder = new DefaultIoFilterChainBuilder();
                
            }

            _IoAcceptor.FilterChainBuilder = _DefaultIoFilterChainBuilder;

            if (ProtocolCodecFilter == null)
            {
                _ProtocolCodecFilter = new ProtocolCodecFilter(new DefaultPacketFactory());
            }

            if (LoggingFilter != null)
            {
                _IoAcceptor.FilterChain.AddLast("LoggingFilter", LoggingFilter);
            }

            
            _IoAcceptor.FilterChain.AddLast("CodecFilter", ProtocolCodecFilter);
            
            _IoAcceptor.Handler = this;
            _IoAcceptor.SessionConfig.ReadBufferSize = 2048;
            _IoAcceptor.SessionConfig.SetIdleTime(IdleStatus.BothIdle, 10);

            return 0;
        }

        public virtual void Start()
        {          
            try
            {
                this.IoAcceptor.Bind();
                
                IsOpen = true;
            }
            catch (IOException)
            {

            }
        }

        public virtual void Stop()
        {
            this.IoAcceptor.Unbind();
            IsOpen = false;
        }

        public virtual int Send(string Message)
        {
            if (IoAcceptor.Active)
                this.IoSession.Write(Message);
            return 0;
        }

        public virtual int Send(object Message)
        {
            if(IoAcceptor.Active)
                this.IoSession.Write(Message);
            return 0;
        }

        public override void SessionCreated(IoSession session)
        {
            _IoSession = session;
        }

        public override void SessionOpened(IoSession session)
        {
            base.SessionOpened(session);
            _IsOpen = true;
            OnConnectedEvent?.Invoke(session, null);
        }

        public override void SessionClosed(IoSession session)
        {
            base.SessionClosed(session);
            _IsOpen = false;
            OnDisconnectedEvent?.Invoke(session, null);
        }

        public override void SessionIdle(IoSession session, IdleStatus status)
        {
            _IdleCount = session.GetIdleCount(IdleStatus.BothIdle);
            this.ManagedSessionCount = IoAcceptor.ManagedSessions.Count;
        }

        public override void MessageReceived(IoSession session, object message)
        {
            MessageReceivedEvent?.Invoke(session, message);
        }

        public override void MessageSent(IoSession session, object message)
        {
            base.MessageSent(session, message);
            OnMessageSentEvent?.Invoke(session, message);
        }
    }
}
