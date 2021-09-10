using Mina.Core.Buffer;
using Mina.Core.Session;
using Mina.Filter.Codec;
using Mina.Filter.Codec.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INNO6.Core.Communication.Socket.Codec
{
    public class DefaultPacketDecoder : ObjectSerializationDecoder
    {
        protected override bool DoDecode(IoSession session, IoBuffer @in, IProtocolDecoderOutput @out)
        {
            @in.Mark();
            MemoryStream byteArrayOutputStream = new MemoryStream();
           
            byte[] bodyBytes = new byte[@in.Remaining];                   //3 Digit

            @in.Get(bodyBytes, 0, bodyBytes.Length);
            byteArrayOutputStream.Write(bodyBytes, 0, bodyBytes.Length); ;
            @out.Write(byteArrayOutputStream.ToArray());

            return true;
        }
    }
}
