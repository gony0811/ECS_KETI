using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INNO6.Core.Communication.Socket.Interface
{
    public interface IPacket
    {
        byte[] MakeRawData(params byte[][] bs);
        byte[] GetRawPayload();
        byte[] GetRawData();
    }
}
