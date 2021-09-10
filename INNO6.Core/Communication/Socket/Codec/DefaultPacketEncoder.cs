using Mina.Core.Buffer;
using Mina.Core.Session;
using Mina.Filter.Codec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INNO6.Core.Communication.Socket.Codec
{
    public class DefaultPacketEncoder : IProtocolEncoder
    {
        public void Dispose(IoSession session)
        {
           
        }

        public void Encode(IoSession session, object message, IProtocolEncoderOutput @out)
        {
            byte[] packet = null;

            if (message is string)
            {
                packet = Encoding.UTF8.GetBytes(message as string);
            }
            else if (message is byte[])
            {
                packet = message as byte[];
            }
            else if (message is char[])
            {
                packet = Encoding.UTF8.GetBytes(message as char[]);
            }

            if (packet.Length > 3)
                return;

            IoBuffer buffer = IoBuffer.Allocate(3);
            buffer.Put(packet);
            buffer.Flip();
            @out.Write(buffer);
            buffer.Clear();
        }
    }
}
