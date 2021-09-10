using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Threading;
using INNO6.IO;
using INNO6.Core;

namespace INNO6.IO.Remote
{
    public enum eRemoteConnectionMode
    {
        IPC,
        TCP
    }

    public class RemoteServer : IDisposable
    {
        public bool Initialized { get; private set; }

        private IpcServerChannel _ipcServerChannel;
        private TcpChannel _tcpServerChannel;

        private string _objectUri;
        private RemoteObject _remoteObject;

        public RemoteServer(string objUri) 
        {
            _objectUri = objUri;
        }

        public RemoteObject RemoteObject
        {
            get { return _remoteObject; }
        }

        public bool Open(string port, eRemoteConnectionMode mode)
        {
            try
            {
                if(_objectUri == null)
                    _objectUri = "IPC_KEY";

                switch(mode)
                {
                    case eRemoteConnectionMode.TCP:
                        {
                            int portNum;
                            if (int.TryParse(port, out portNum) && (portNum < 65536))
                            {
                                _tcpServerChannel = new TcpChannel(portNum);
                                ChannelServices.RegisterChannel(_tcpServerChannel, false);
                            }

                        }
                        break;

                    case eRemoteConnectionMode.IPC:
                        {
                            _ipcServerChannel = new IpcServerChannel(port);
                            ChannelServices.RegisterChannel(_ipcServerChannel, false);
                            
                        }
                        break;
                }

                RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteObject), _objectUri, WellKnownObjectMode.Singleton);
                _remoteObject = new RemoteObject();
                LogHelper.Instance.SystemLog.DebugFormat("[Open] Server Open Success.");
                return true;
            }
            catch
            {
                LogHelper.Instance.SystemLog.DebugFormat("[ERROR] Server Open Error.");
                return false;
            }
        }




        #region IDisposable 멤버

        public void Dispose()
        {
            Initialized = false;
        }

        #endregion
    }
}
