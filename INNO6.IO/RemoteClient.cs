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
    public class RemoteClient : IDisposable
    {
        private IpcClientChannel ipcClientChannel;
        private TcpChannel tcpClientChannel;
        private RemoteObject _remoteObject;

        public string IpAddress { get; set; }
        public string ObjectUri { get; set; }

        public RemoteObject RemoteObject
        {
            get { return _remoteObject; }
        }

        public RemoteClient()
        {
        }

        public RemoteClient(string objUri)
        {
            ObjectUri = objUri;
        }

        public bool Connect(eRemoteConnectionMode mode, string ipAddress, string port)
        {
            try
            {
                switch(mode)
                {
                    case eRemoteConnectionMode.TCP:
                        {
                            tcpClientChannel = new TcpChannel();
                            ChannelServices.RegisterChannel(tcpClientChannel, false);
                            _remoteObject = (RemoteObject)Activator.GetObject(typeof(RemoteObject), @"tcp://" + IpAddress + @":" + port + @"/" + ObjectUri);
                            LogHelper.Instance.SystemLog.DebugFormat("[Connect] TCP Client Connect. tcp://" + IpAddress + @":" + port + @"/" + ObjectUri);
                        }
                        break;

                    default:
                        {
                            ipcClientChannel = new IpcClientChannel();
                            ChannelServices.RegisterChannel(ipcClientChannel, false);
                            RemotingConfiguration.RegisterWellKnownClientType(typeof(RemoteObject), @"ipc://" + port + @"/" + ObjectUri);
                            _remoteObject = new RemoteObject();
                            LogHelper.Instance.SystemLog.DebugFormat("[Connect] IPC Client Connect. ipc://" + port + @"/" + ObjectUri);
                        }
                        break;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region IDisposable 멤버

        public void Dispose()
        {
            
        }

        #endregion
    }
}
