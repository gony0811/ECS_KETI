using INNO6.Core.Communication.Socket.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INNO6.Core.Communication.Socket.Checker
{
    public class PacketCheckHolder
    {
        private IPacket _ReceivedPacket;

        public PacketCheckHolder(IPacket receivedPacket)
        {
            _ReceivedPacket = receivedPacket;
        }

        public IPacket ReceivedPacket
        {
            get { return this._ReceivedPacket; }
            set { this._ReceivedPacket = value; }
        }
    }
}
